using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AlbumClassLibrary;
using AlbumClassLibrary.CacheManager;

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

        private Dictionary<ClassDir.AutoStackImage, BitmapSource> LocalCache = new Dictionary<ClassDir.AutoStackImage, BitmapSource>();

        /// <summary>
        /// Текущая галерея. Меняется при смене папок.
        /// </summary>
        private List<ClassDir.AutoStackImage> _imageGallery;
        private List<ClassDir.AutoStackImage> ImageGallery
        {
            get
            {
                if (_imageGallery == null)
                {
                    _imageGallery = new List<ClassDir.AutoStackImage>();
                }

                return _imageGallery;
            }
            set
            {
                if (value.Count == 0 && (_imageGallery != null && _imageGallery.Count > 0))
                    foreach (var item in _imageGallery)
                    {
                        item.Dispose();
                    }

                _imageGallery = value;
            }
        }

        AlbumClassLibrary.AlbumManager.AlbumManager albumsManager;

        private event EventHandler CurrentAlbumChanged;

        private AlbumClassLibrary.IAlbum _currentAlbum;
        AlbumClassLibrary.IAlbum CurrentAlbum
        {
            get
            {
                return _currentAlbum;
            }
            set
            {
                _currentAlbum = value;
                value.IsCurrent = true;
                CurrentAlbumChanged(value, new EventArgs());
            }
        }

        public MainWindow()
        {
            albumsManager = new AlbumClassLibrary.AlbumManager.AlbumManager(Properties.Settings.Default.CacheFilePath);

            InitializeComponent();

            CurrentAlbumChanged += MainWindow_CurrentAlbumChanged;
            _resizeTimer.Tick += _resizeTimer_Tick;

            AlbumsButtonsRenderer(albumsManager.Albums);
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

            albumsManager.Albums.First().IsCurrent = true;

            if (CurrentAlbum == null)
            {
                CurrentAlbum = albumsManager.Albums.First();
                (FavoriteStackPanel.Children[0] as Forms.Favorites.FolderButton).MainButton_Click(this, new RoutedEventArgs());
            }
        }

        private void AlbumButtonClicked(object sender, EventArgs e)
        {
            CurrentAlbum = sender as IAlbum;
        }

        private void MainWindow_CurrentAlbumChanged(object sender, EventArgs e)
        {
            FavoriteStackPanel.Children.Clear();

            if ((sender as IAlbum).Folders != null)
                foreach (var item in (sender as IAlbum).Folders)
                {
                    Random rand = new Random();
                    using (CacheManager manager = new CacheManager(Properties.Settings.Default.CacheFilePath))
                        item.PreviewImage = manager.GetImage((System.IO.Directory.GetFiles(item.Path).Where(x => x.EndsWith(".jpg") || x.EndsWith(".jpeg") || x.EndsWith(".gif")).ElementAt(rand.Next(0, (int)(System.IO.Directory.GetFiles(item.Path).Count() - 1)))));

                    var def = new Forms.Favorites.FolderButton(item);
                    def.Mouse_Click += Folder_Mouse_Click;
                    FavoriteStackPanel.Children.Add(def);
                }

            FavoriteStackPanel.Children.Add(new Forms.Favorites.AddFolderButton(sender as IAlbum));
        }

        private void Folder_Mouse_Click(object sender, RoutedEventArgs e)
        {
            LoadFolderToGallery(Directory.GetFiles((sender as IFolder).Path).Where(x => x.EndsWith(".jpg") || x.EndsWith(".jpeg") || x.EndsWith(".gif")).ToList());
            LoadImagesOnBoard();
        }

        private void AddBtn_AlbumAdded(object sender, EventArgs e)
        {
            AlbumsButtonsRenderer(albumsManager.Albums);
            CurrentAlbum = sender as IAlbum;
        }

        #endregion

        #region Favorite

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

            preview.HiResLoaded += Preview_HiResLoaded;
            preview.Close += Preview_Close;

            Grid.SetRowSpan(preview, 4);
            MainGrid.Children.Add(preview);
        }

        private void Preview_HiResLoaded(object sender, EventArgs e)
        {
            if (sender is Tuple<ClassDir.AutoStackImage, BitmapSource>)
            {
                var snd = sender as Tuple<ClassDir.AutoStackImage, BitmapSource>;
                LocalCache.Add(snd.Item1, snd.Item2);
                if (LocalCache.Count > 5)
                    LocalCache.Remove(LocalCache.First().Key);
            }
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
