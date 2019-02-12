using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AlbumClassLibrary;
using AlbumClassLibrary.CacheManager;
using NewWpfImageViewer.Classes;
using NewWpfImageViewer.Interfaces;
using AlbumClassLibrary.AlbumManager;

namespace NewWpfImageViewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        AlbumManager AlbumsManager = null;
        IImageCollection ImageCollection = null;

        public static Brush WinColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + ClassDir.WinColor.GetColor()));
        public string CacheFile => Properties.Settings.Default.CacheFilePath + Properties.Settings.Default.CacheFileName;

        public Dictionary<IImageStructure, object> LocalCache = new Dictionary<IImageStructure, object>();

        private AlbumClassLibrary.IAlbum _currentAlbum;
        private AlbumClassLibrary.IAlbum CurrentAlbum
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

        #endregion

        #region Local Settings

        private DispatcherTimer _resizeTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 50), IsEnabled = false };
        private DispatcherTimer _scrollDelayTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 50), IsEnabled = false };

        #endregion

        #region Events

        private event EventHandler CurrentAlbumChanged;

        public event EventHandler Scrolled;
        public event EventHandler Resized;

        #endregion

        // в настройки
        List<string> _suppFormats = new List<string>() { ".jpg", ".jpeg", ".gif", ".png" };

        #region Main

        public MainWindow()
        {
            ClassDir.SettingsManager.CheckSettings(); // ??? 

            ImageCollection = new ImageCollection(CacheFile, Interfaces.Size.SMALL);
            AlbumsManager = new AlbumManager(CacheFile);

            Scrolled += ImageCollection.ScrolledEventHandler;
            Resized += ImageCollection.ResizedEventHandler;

            CurrentAlbumChanged += MainWindow_CurrentAlbumChanged;

            _resizeTimer.Tick += _resizeTimer_Tick;
            _scrollDelayTimer.Tick += _scrollDelayTimer_Tick;

            InitializeComponent();

            AlbumsButtonsRenderer(AlbumsManager.Albums);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Перезагрузка всего окна, повторная загрузка альбомов, папок в них, изображений
        /// </summary>
        private void InitializeWindow() { throw new NotImplementedException(); }

        /// <summary>
        /// TODO Перерисовка кнопок альбомов
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

            var addBtn = new Forms.Albums.NewAlbumButton(AlbumsManager);
            addBtn.AlbumAdded += AddBtn_AlbumAdded;
            AlbumButtonPanel.Children.Add(addBtn);

            AlbumsManager.Albums.First().IsCurrent = true;

            CurrentAlbum = AlbumsManager.Albums.First();

            (FavoriteStackPanel.Children[0] as Forms.Favorites.FolderButton).MainButton_Click(this, new RoutedEventArgs());
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
            MainWrapPanel.ItemsSource = null;
            MainWrapPanel.ItemsSource = ImageCollection.Images;
        }

        private static void CollectGarbage()
        {
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                System.Threading.Thread.Sleep(500);
                GC.Collect();
            }));

            thread.Start();
        }

        private void AnimationFromToProp(UIElement element, DependencyProperty property, double? from, double? to, int milliseconds)
        {
            DoubleAnimation showAnimation = new DoubleAnimation();
            showAnimation.From = from;
            showAnimation.To = to;
            showAnimation.Duration = TimeSpan.FromMilliseconds(milliseconds);
            element.BeginAnimation(property, showAnimation);
        }

        #endregion

        #region Event Handlers

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

            if ((sender as IAlbum).Folders != null)
                foreach (var item in (sender as IAlbum).Folders)
                {
                    Random rand = new Random();
                    using (CacheManager manager = new CacheManager(CacheFile))
                    {
                        var files = System.IO.Directory.GetFiles(item.Path).Where(x => _suppFormats.Any(y => x.EndsWith(y)));
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
            {
                (FavoriteStackPanel.Children[0] as Forms.Favorites.FolderButton).MainButton_Click(this, new RoutedEventArgs());
            }
        }

        private void Folder_Mouse_Click(object sender, RoutedEventArgs e)
        {
            var reLst = Directory.GetFiles((sender as IFolder).Path).Where(x => _suppFormats.Any(y => x.EndsWith(y))).ToList();

            this.ImageCollection.ChangeCollection(reLst);
            LoadImagesOnBoard();
        }

        private void AddBtn_AlbumAdded(object sender, EventArgs e)
        {
            AlbumsButtonsRenderer(AlbumsManager.Albums);
            CurrentAlbum = sender as IAlbum;
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

        private void SmallSizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.ImageCollection.SetSize(Interfaces.Size.SMALL);
        }

        private void MedSizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.ImageCollection.SetSize(Interfaces.Size.MEDIUM);
        }

        private void BigSizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.ImageCollection.SetSize(Interfaces.Size.BIG);
        }


        private void Preview_HiResLoaded(object sender, EventArgs e)
        {
            try
            {
                if (sender is Tuple<IImageStructure, object>)
                {
                    var snd = sender as Tuple<IImageStructure, object>;
                    LocalCache.Add(snd.Item1, snd.Item2);
                    if (LocalCache.Count > 5)
                        LocalCache.Remove(LocalCache.First().Key);
                }
            }
            catch (Exception)
            {
            }
        }

        private void Preview_Close(object sender, EventArgs e)
        {
            MainGrid.Children.Remove(sender as Forms.ImagePreview.ImegePreview);

            (sender as Forms.ImagePreview.ImegePreview).MainImage.Source?.Freeze();

            (sender as Forms.ImagePreview.ImegePreview).MainGrid.Children.Clear();
            (sender as Forms.ImagePreview.ImegePreview).MainGrid = null;
            (sender as Forms.ImagePreview.ImegePreview).MainImage = null;

            CollectGarbage();

            this.Title = Properties.Settings.Default.MainWindowName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AnimationFromToProp(sender as Window, Window.OpacityProperty, 0, 1, 1000);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
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
                InitializeWindow();

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

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (MainGrid.Children.Cast<object>().Any(x => x.GetType() == typeof(Forms.ImagePreview.ImegePreview)))
            {
                var _preview = MainGrid.Children.Cast<object>().First(x => x.GetType() == typeof(Forms.ImagePreview.ImegePreview)) as Forms.ImagePreview.ImegePreview;

                if (e.Key == System.Windows.Input.Key.Right)
                    _preview.Forward();
                else if (e.Key == System.Windows.Input.Key.Left)
                    _preview.Back();
                else if (e.Key == System.Windows.Input.Key.Escape)
                    _preview.CloseForm();
            }
        }

        private void MainWrapPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _resizeTimer.IsEnabled = true;
            _resizeTimer.Stop();
            _resizeTimer.Start();
        }

        private void MainScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_scrollDelayTimer.IsEnabled)
                _scrollDelayTimer.Stop();

            _scrollDelayTimer.Start();

            SearchTextBox.Text = $"Total {ImageCollection.Images.Count()} Loaded source {ImageCollection.Images.Count(x => x.BitmapSource != null)} Loaded MaxSized  {ImageCollection.Images.Count(x => x.MaxSizeIsLoaded() == true)}";
        }

        void _resizeTimer_Tick(object sender, EventArgs e)
        {
            _resizeTimer.IsEnabled = false;
            Resized(MainWrapPanel, e);
        }

        private void _scrollDelayTimer_Tick(object sender, EventArgs e)
        {
            _scrollDelayTimer.Stop();
            Scrolled(MainScrollViewer, new EventArgs());
            CollectGarbage();
        }

        private void Image_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MainGrid.Children.Cast<object>().Any(x => x.GetType() == typeof(Forms.ImagePreview.ImegePreview)))
                return;

            var preview = new Forms.ImagePreview.ImegePreview(ImageCollection.Images, ImageCollection.Images.ToList().IndexOf(ImageCollection.Images.First(x => x.BitmapSource == (sender as Image).Source)), ref LocalCache, this);

            preview.HiResLoaded += Preview_HiResLoaded;
            preview.Close += Preview_Close;

            Grid.SetRowSpan(preview, 4);
            MainGrid.Children.Add(preview);

            this.Focusable = true;
            this.Focus();
        }

        private void Image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AnimationFromToProp(sender as Image, Image.OpacityProperty, 0, 1, 1000);
        }

        private void Image_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //(((sender) as Image).Tag as ClassDir.AutoSizeImage).WidthAdded += 50;
        }

        private void Image_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //(((sender) as Image).Tag as ClassDir.AutoSizeImage).WidthAdded -= 50;
        }

        #endregion

        private void Tst_Click()
        {

        }
    }
}