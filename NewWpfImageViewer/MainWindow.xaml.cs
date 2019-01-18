using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        public static Brush WinColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + ClassDir.WinColor.GetColor()));

        /// <summary>
        /// Таймер для зарежки пересчета размеров изображений 
        /// </summary>
        System.Windows.Threading.DispatcherTimer _resizeTimer = new System.Windows.Threading.DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 250), IsEnabled = false };

        /// <summary>
        /// Текущее состояние отрисовки масштаба
        /// </summary>
        private ClassDir.AutoStackImage.Size currentSize = ClassDir.AutoStackImage.Size.MEDIUM;

        /// <summary>
        /// Прототип локального кеша на 5 изображений
        /// </summary>
        public Dictionary<ClassDir.AutoStackImage, object> LocalCache = new Dictionary<ClassDir.AutoStackImage, object>();

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

        private string CacheFile => Properties.Settings.Default.CacheFilePath + Properties.Settings.Default.CacheFileName;

        public MainWindow()
        {
            ClassDir.SettingsManager.CheckSettings();
            ReInitializer();
        }

        private void ReInitializer()
        {
            albumsManager = new AlbumClassLibrary.AlbumManager.AlbumManager(CacheFile);

            InitializeComponent();

            CurrentAlbumChanged += MainWindow_CurrentAlbumChanged;
            _resizeTimer.Tick += _resizeTimer_Tick;

            AlbumsButtonsRenderer(albumsManager.Albums);
        }

        #region Search

        #endregion

        #region Collection

        /// <summary>
        /// Перерисовка кнопок альбомов
        /// </summary>
        /// <param name="albums">Набор альбомов</param>
        private void AlbumsButtonsRenderer(List<IAlbum> albums)
        {
            AlbumButtonPanel.Children.Clear();

            if (albums != null)
                foreach (var item in albums)
                {
                    item.FolderAdded += Album_FolderAdded;

                    var def = new Forms.Albums.AlbumButton(item);
                    def.AlbumButtonClicked += AlbumButtonClicked;
                    AlbumButtonPanel.Children.Add(def);
                }

            var addBtn = new Forms.Albums.NewAlbumButton(albumsManager);
            addBtn.AlbumAdded += AddBtn_AlbumAdded;
            AlbumButtonPanel.Children.Add(addBtn);

            albumsManager.Albums.First().IsCurrent = true;

            CurrentAlbum = albumsManager.Albums.First();

            (FavoriteStackPanel.Children[0] as Forms.Favorites.FolderButton).MainButton_Click(this, new RoutedEventArgs());
        }

        private void Album_FolderAdded(object sender, EventArgs e)
        {
            MainWindow_CurrentAlbumChanged(CurrentAlbum, new EventArgs());
        }

        private void AlbumButtonClicked(object sender, EventArgs e)
        {
            CurrentAlbum = sender as IAlbum;
        }

        private void MainWindow_CurrentAlbumChanged(object sender, EventArgs e)
        {
            FavoriteStackPanel.Children.Clear();
            MainWrapPanel.Children.Clear();
            ImageGallery.Clear();

            if ((sender as IAlbum).Folders != null)
                foreach (var item in (sender as IAlbum).Folders)
                {
                    Random rand = new Random();
                    using (CacheManager manager = new CacheManager(CacheFile))
                    {
                        var files = System.IO.Directory.GetFiles(item.Path).Where(x => x.EndsWith(".jpg") || x.EndsWith(".jpeg") || x.EndsWith(".gif"));
                        var previewIndex = rand.Next(0, files.Count() - 1);
                        item.PreviewImage = manager.GetImage((files.ElementAt(previewIndex)));
                    }

                    var def = new Forms.Favorites.FolderButton(item);
                    def.Mouse_Click += Folder_Mouse_Click;
                    FavoriteStackPanel.Children.Add(def);
                }

            FavoriteStackPanel.Children.Add(new Forms.Favorites.AddFolderButton(sender as IAlbum));
            LocalCache.Clear();

            if (FavoriteStackPanel.Children.Count > 1)
                (FavoriteStackPanel.Children[0] as Forms.Favorites.FolderButton).MainButton_Click(this, new RoutedEventArgs());
        }

        private void Folder_Mouse_Click(object sender, RoutedEventArgs e)
        {
            List<string> _suppFormats = new List<string>() { ".jpg", ".jpeg", ".gif" };
            var reLst = Directory.GetFiles((sender as IFolder).Path).Where(x => _suppFormats.Any(y => x.EndsWith(y))).ToList();

            //Random rnd = new Random();
            //var randomizedList = from item in list
            //                     orderby rnd.Next()
            //                     select item;

            if (Properties.Settings.Default.MaxFilesToLoad != 0)
                LoadFolderToGallery(reLst.Take(Properties.Settings.Default.MaxFilesToLoad).ToList());
            else
                LoadFolderToGallery(reLst);
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
        /// Загрузка изображений из папки в список AutoStackImages
        /// </summary>
        /// <param name="files"></param>
        private void LoadFolderToGallery(List<string> files)
        {
            ImageGallery = new List<ClassDir.AutoStackImage>();

            // Кормим менеджер путем к файлу, получаем кешированную копию, генерим АвтоСтаки и складываем в список
            using (AlbumClassLibrary.CacheManager.CacheManager manager = new AlbumClassLibrary.CacheManager.CacheManager(CacheFile))
            {
                foreach (var item in files)
                {
                    ImageGallery.Add(new ClassDir.AutoStackImage(manager.GetImage(item), item));
                }
            }

            LoadImagesOnBoard();
        }

        /// <summary>
        /// Заполняем изображениями из списка форму
        /// </summary>
        /// <remarks>
        /// Этот метод вынесен отдельно, так как отвечает не только за наполнение формы контролами, но и за изменение масштаба
        /// </remarks>
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

            ClassDir.DynamicRowFormatter.FormatPlate(ImageGallery, MainWrapPanel.ActualWidth);

            foreach (var item in ImageGallery)
            {
                var img = item.ImageControl;
                img.MouseUp += Img_MouseUp;

                MainWrapPanel.Children.Add(img);
            }

            MainScrollViewer.ScrollToTop();
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

            var preview = new Forms.ImagePreview.ImegePreview(ImageGallery, ImageGallery.IndexOf(ImageGallery.First(x => x.ImageControl == sender)), ref LocalCache, this);

            preview.HiResLoaded += Preview_HiResLoaded;
            preview.Close += Preview_Close;

            Grid.SetRowSpan(preview, 4);
            MainGrid.Children.Add(preview);
        }

        private void Preview_HiResLoaded(object sender, EventArgs e)
        {
            if (sender is Tuple<ClassDir.AutoStackImage, object>)
            {
                var snd = sender as Tuple<ClassDir.AutoStackImage, object>;
                LocalCache.Add(snd.Item1, snd.Item2);
                if (LocalCache.Count > 5)
                    LocalCache.Remove(LocalCache.First().Key);
            }
        }

        private void Preview_Close(object sender, EventArgs e)
        {
            MainGrid.Children.Remove(sender as Forms.ImagePreview.ImegePreview);

            (sender as Forms.ImagePreview.ImegePreview).MainImage.Source?.Freeze();

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

            this.Title = Properties.Settings.Default.MainWindowName;
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

        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.Children.Cast<object>().Any(x => x.GetType() == typeof(Forms.Settings.SettingsLayout)))
                MainGrid.Children.Remove(MainGrid.Children.Cast<object>().First(x => x.GetType() == typeof(Forms.Settings.SettingsLayout)) as Forms.Settings.SettingsLayout);

            var sett = new Forms.Settings.SettingsLayout();
            sett.Close += Sett_Close;

            Grid.SetRowSpan(sett, 4);
            MainGrid.Children.Add(sett);

            this.Title = "Settings";
        }

        private void Sett_Close(object sender, EventArgs e)
        {
            while ((sender as Forms.Settings.SettingsLayout).Width == 0)
            {
                MainGrid.Children.Remove(sender as Forms.Settings.SettingsLayout);
            }

            if ((sender as Forms.Settings.SettingsLayout).Edited)
                ReInitializer();

            this.Title = Properties.Settings.Default.MainWindowName;
        }

        private async void FavoitesExpander_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            await System.Threading.Tasks.Task.Delay(200);

            if ((sender as Expander).IsMouseOver)
                (sender as Expander).IsExpanded = true;
        }

        private void FavoitesExpander_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as Expander).IsExpanded = false;
        }
    }
}
