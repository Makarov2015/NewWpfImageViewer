using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

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

        /// <summary>
        /// Список папок для выбора
        /// </summary>
        private List<ClassDir.FolderEntity> FavoriteList = new List<ClassDir.FolderEntity>();

        /// <summary>
        /// Скорее всего не понадобится
        /// </summary>
        ClassDir.FolderEntity defaultFolder;
        ClassDir.FolderEntity currentFolder;

        public MainWindow()
        {
            InitializeComponent();

            // Выбираем дефолтную папку в зависимости от запуска
            {
                // При первом запуске - дефолтная "Изображения". Создаем список папок, кидаем первой - дефолтную, докидываем остальные и в конец кнопку "ADD"
                // TODO Нужен конструктор. При добавлении папок нужно будет налету пересобирать очередь
                if (Properties.Settings.Default.IsFirstLaunch)
                {
                    Properties.Settings.Default.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    Properties.Settings.Default.IsFirstLaunch = false;
                    Properties.Settings.Default.ProgramDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\NewWpfImageViewer\\";

                    Properties.Settings.Default.Save();
                }

                defaultFolder = new ClassDir.FolderEntity("Defaulr Folder", Properties.Settings.Default.DefaultDirectory);
                currentFolder = new ClassDir.FolderEntity("Defaulr Folder", Properties.Settings.Default.DefaultDirectory);
            }

            // Загружаем и отрисовываем картинки для дефолтной папки
            // Отрисовывается на Window_Loaded
            LoadFolderToGallery(defaultFolder.ImagesPaths);

            // Отрисовываем коллекцию добавленых папок в Фэйворитс
            {
                FirstInit();
                FavoritesConstructor();
            }
            
            // Пока все. Папки нарисованы, дефолтная показана, остальные показаны, интерфейс готов
            
            _resizeTimer.Tick += _resizeTimer_Tick;
        }

        private void FavoritesConstructor()
        {
            // Отчистиили панель
            FavoriteStackPanel.Children.Clear();

            var fldr = defaultFolder.GetControl();
            fldr.Mouse_Click += Fldr_Mouse_Click;
            FavoriteStackPanel.Children.Add(fldr);

            foreach (var item in FavoriteList)
            {
                var folderToAdd = item.GetControl();
                folderToAdd.Mouse_Click += Fldr_Mouse_Click;

                FavoriteStackPanel.Children.Add(folderToAdd);
            }

            Forms.Favorites.AddFolderButton addFolder = new Forms.Favorites.AddFolderButton();
            addFolder.Mouse_Click += AddFolder_Mouse_Click;

            FavoriteStackPanel.Children.Add(addFolder);
        }

        private void FirstInit()
        {
            var fldr2 = new ClassDir.FolderEntity("Вторая папочка", @"C:\Users\makarov\Pictures\Saved Pictures");
            FavoriteList.Add(fldr2);
        }

        /// <summary>
        /// Создаем список сущностей изображений из выбранной (текущей) папки
        /// </summary>
        private void LoadFolderToGallery(List<string> files)
        {
            ImageGallery = new List<ClassDir.AutoStackImage>();

            using (ClassDir.CacheFileManager manager = new ClassDir.CacheFileManager())
            {
                this.Title = "Main Window cache size : " + manager.CacheSize + " Mb";

                foreach (var item in files)
                {
                    // imagesPath.Add(new ClassDir.AutoStackImage(item)); // Вариант без кеша
                    ImageGallery.Add(manager.GetImage(item)); // Вариант с кешем
                }

                manager.SaveDictionary();

                System.Diagnostics.Debug.WriteLine(manager.CacheSize.ToString());
            }
        }

        /// <summary>
        /// Заполняем изображениями из списка сущностей текущей папки форму
        /// </summary>
        /// <param name="autoStacks"></param>
        private void LoadImagesOnBoard()
        {
            MainWrapPanel.Children.Clear();

            foreach (var item in ImageGallery)
            {
                item.CurrentSize = currentSize;
            }

            ClassDir.AutoStackImage.DynamicRowFormatter(ImageGallery, MainWrapPanel.ActualWidth);

            foreach (var item in ImageGallery)
            {
                MainWrapPanel.Children.Add(item.ImageControl);
            }
        }

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Fldr_Mouse_Click(object sender, RoutedEventArgs e)
        {
            currentFolder = sender as ClassDir.FolderEntity;
            LoadFolderToGallery(currentFolder.ImagesPaths);
            LoadImagesOnBoard();
        }

        private void AddFolder_Mouse_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
