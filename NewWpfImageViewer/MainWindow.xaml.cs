using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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

        ClassDir.FavoritePanelManager favoritePanelManager;
        //AlbumClassLibrary.AlbumManager.AlbumsManager albumsManager = new AlbumClassLibrary.AlbumManager.AlbumsManager();

        private Grid PrevieRectangle;
        private BitmapSource PreviewSource;

        public MainWindow()
        {
            InitializeComponent();

            AlbumClassLibrary.AlbumManager.AlbumsManager albumsManager = new AlbumClassLibrary.AlbumManager.AlbumsManager(Properties.Settings.Default.CacheFilePath);

            favoritePanelManager = new ClassDir.FavoritePanelManager(@"C:\Users\user\AppData\Roaming\NewWpfImageViewer\", "_favorites");
            favoritePanelManager.SelectedFolderChanged += FavoritePanelManager_SelectedFolderChanged;
            favoritePanelManager.NewFavoriteFolderAdded += FavoritePanelManager_NewFavoriteFolderAdded;
            // Отрисовываем коллекцию добавленых папок в Фэйворитс
            {
                FavoritesConstructor();
            }

            // Отрисовывается на Window_Loaded
            LoadFolderToGallery(favoritePanelManager.CurrentFolder.ImagesPaths);

            // Пока все. Папки нарисованы, дефолтная показана, остальные показаны, интерфейс готов

            _resizeTimer.Tick += _resizeTimer_Tick;
        }

        #region Search

        #endregion

        #region Collection

        #endregion

        #region Favorite

        private void FavoritesConstructor()
        {
            // Отчистиили панель
            FavoriteStackPanel.Children.Clear();

            foreach (var item in favoritePanelManager.ButtonsCollection)
            {
                FavoriteStackPanel.Children.Add(item);
            }
        }

        private void FavoritePanelManager_NewFavoriteFolderAdded()
        {
            FavoritesConstructor();
        }

        private void FavoritePanelManager_SelectedFolderChanged(ClassDir.FolderEntity folder)
        {
            LoadFolderToGallery(favoritePanelManager.CurrentFolder.ImagesPaths);
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
            if (PrevieRectangle != null)
                return;

            var autoStackImage = ImageGallery.First(x => x.ImageControl == sender);

            PrevieRectangle = new Grid
            {
                Visibility = Visibility.Visible,
                Background = this.Resources["TransparentBlack"] as System.Windows.Media.Brush
            };
            
            PreviewSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap((System.Drawing.Bitmap.FromFile(autoStackImage.OriginalFilepath) as System.Drawing.Bitmap).GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, null);

            PrevieRectangle.Children.Add(new Image { Source = PreviewSource, Margin = new Thickness(50), Width = Double.NaN, Height = Double.NaN, Opacity = 1 });
            PrevieRectangle.MouseUp += PrevieRectangle_MouseRightButtonUp;

            Grid.SetRowSpan(PrevieRectangle, 4);
            MainGrid.Children.Add(PrevieRectangle);
        }

        private void PrevieRectangle_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainGrid.Children.Remove(PrevieRectangle);

            PreviewSource.Freeze();

            PrevieRectangle.Children.Clear();
            PrevieRectangle = null;
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
