using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AlbumClassLibrary;

namespace NewWpfImageViewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Таймер для зарежки пересчета размеров изображений 
        /// </summary>
        System.Windows.Threading.DispatcherTimer _resizeTimer = new System.Windows.Threading.DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 250), IsEnabled = false };

        /// <summary>
        /// Текущее состояние отрисовки масштаба
        /// </summary>
        private ClassDir.AutoStackImage.Size currentSize = ClassDir.AutoStackImage.Size.MEDIUM;

        /// <summary>
        /// Текущая галерея. Меняется при смене папок.
        /// </summary>
        private List<ClassDir.AutoStackImage> ImageGallery = new List<ClassDir.AutoStackImage>();

        AlbumClassLibrary.AlbumManager.AlbumManager albumsManager;

        public MainWindow()
        {
            // Загрузили альбомы
            albumsManager = new AlbumClassLibrary.AlbumManager.AlbumManager(Properties.Settings.Default.CacheFilePath);

            InitializeComponent();

            // Сгенерировали и раскидали кнопки для каждого альбома (Кнопка имеет название и сам альбом)
            AlbumsButtonsRenderer(albumsManager.Albums);

            _resizeTimer.Tick += _resizeTimer_Tick;
        }

        #region Search

        #endregion

        #region Collection

        private void AlbumsButtonsRenderer(List<IAlbum> albums)
        {
            AlbumButtonPanel.Children.Clear();

            if (albums != null)
                foreach (var item in albums)
                {
                    var def = new Forms.Albums.AlbumButton(item);
                    def.AlbumButtonClicked += AlbumButtonClicked;
                    AlbumButtonPanel.Children.Add(def);
                }

            var addBtn = new Forms.Albums.NewAlbumButton(albumsManager);
            addBtn.AlbumAdded += AddBtn_AlbumAdded;
            AlbumButtonPanel.Children.Add(addBtn);
        }

        private void AlbumButtonClicked(object sender, EventArgs e)
        {
            FavoriteStackPanel.Children.Clear();

            foreach (var item in (sender as IAlbum).Folders)
            {
                FavoriteStackPanel.Children.Add(new Forms.Favorites.FolderButton(item));
            }

            FavoriteStackPanel.Children.Add(new Forms.Favorites.AddFolderButton(sender as IAlbum));
        }

        private void AddBtn_AlbumAdded(object sender, EventArgs e)
        {
            AlbumsButtonsRenderer(albumsManager.Albums);
        }

        #endregion

        #region Favorite

        private void FavoritesConstructor()
        {
            // Отчистиили панель
            FavoriteStackPanel.Children.Clear();

            //foreach (var item in favoritePanelManager.ButtonsCollection)
            //{
            //    FavoriteStackPanel.Children.Add(item);
            //}
        }

        private void FavoritePanelManager_NewFavoriteFolderAdded()
        {
            FavoritesConstructor();
        }

        private void FavoritePanelManager_SelectedFolderChanged(ClassDir.FolderEntity folder)
        {
            //LoadFolderToGallery(favoritePanelManager.CurrentFolder.ImagesPaths);
            LoadImagesOnBoard();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;

            if (e.Delta > 0)
                scrollviewer.PageLeft();
            else
                scrollviewer.PageRight();
            e.Handled = true;
        }

        private void ExpanderGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FavoitesExpander.IsExpanded = !FavoitesExpander.IsExpanded;
        }

        #endregion

        #region Images

        /// <summary>
        /// Создаем список сущностей изображений из выбранной (текущей) папки
        /// </summary>
        private void LoadFolderToGallery(List<string> files)
        {
            ImageGallery = new List<ClassDir.AutoStackImage>();

            using (AlbumClassLibrary.CacheManager.CacheManager manager = new AlbumClassLibrary.CacheManager.CacheManager(Properties.Settings.Default.CacheFilePath))
            {
                foreach (var item in files)
                {
                    ImageGallery.Add(new ClassDir.AutoStackImage(manager.GetImage(item), item));
                }
            }
        }

        /// <summary>
        /// Заполняем изображениями из списка сущностей текущей папки форму
        /// </summary>
        /// <param name="autoStacks"></param>
        private void LoadImagesOnBoard()
        {
            if (ImageGallery.Count == 0)
                return;

            MainWrapPanel.Children.Clear();

            foreach (var item in ImageGallery)
            {
                item.CurrentSize = currentSize;
            }

            ClassDir.AutoStackImage.DynamicRowFormatter(ImageGallery, MainWrapPanel.ActualWidth);

            foreach (var item in ImageGallery)
            {
                var img = item.ImageControl;
                img.MouseUp += Img_MouseUp;
                MainWrapPanel.Children.Add(img);
            }
        }

        private void SmallSizeButton_Click(object sender, RoutedEventArgs e)
        {
            currentSize = ClassDir.AutoStackImage.Size.SMALL;
            LoadImagesOnBoard();
        }

        private void MedSizeButton_Click(object sender, RoutedEventArgs e)
        {
            currentSize = ClassDir.AutoStackImage.Size.MEDIUM;
            LoadImagesOnBoard();
        }

        private void BigSizeButton_Click(object sender, RoutedEventArgs e)
        {
            currentSize = ClassDir.AutoStackImage.Size.BIG;
            LoadImagesOnBoard();
        }

        #endregion

        #region ImagePrevie

        private void Img_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MainGrid.Children.Cast<object>().Any(x => x.GetType() == typeof(Forms.ImagePreview.ImegePreview)))
                return;

            var preview = new Forms.ImagePreview.ImegePreview(ImageGallery, ImageGallery.IndexOf(ImageGallery.First(x => x.ImageControl == sender)));

            preview.Close += Preview_Close;

            Grid.SetRowSpan(preview, 4);
            MainGrid.Children.Add(preview);
        }

        private void Preview_Close(object sender, EventArgs e)
        {
            MainGrid.Children.Remove(sender as Forms.ImagePreview.ImegePreview);

            (sender as Forms.ImagePreview.ImegePreview).MainImage.Source.Freeze();

            (sender as Forms.ImagePreview.ImegePreview).MainGrid.Children.Clear();
            (sender as Forms.ImagePreview.ImegePreview).MainGrid = null;
            (sender as Forms.ImagePreview.ImegePreview).MainImage = null;

            GC.Collect();

            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                System.Threading.Thread.Sleep(500);
                GC.Collect();
            }));

            thread.Start();
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadImagesOnBoard();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _resizeTimer.IsEnabled = true;
            _resizeTimer.Stop();
            _resizeTimer.Start();
        }

        void _resizeTimer_Tick(object sender, EventArgs e)
        {
            _resizeTimer.IsEnabled = false;
            LoadImagesOnBoard();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            //using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            //{
            //    System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            //    if (result == System.Windows.Forms.DialogResult.OK)
            //    {
            //        Directory.
            //        var a = dialog.SelectedPath;
            //    }
            //}
        }
    }
}
