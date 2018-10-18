using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Runtime.Caching;

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

        List<ClassDir.AutoStackImage> imagesPath = new List<ClassDir.AutoStackImage>();

        public MainWindow()
        {
            InitializeComponent();
            
            using (ClassDir.CacheManager mngr = new ClassDir.CacheManager("images", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)))
                imagesPath = mngr.DealWithCache();

            _resizeTimer.Tick += _resizeTimer_Tick;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadSizes();
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
            ReloadSizes();
        }

        // BUG При ресайзе из размера, при котором слайдбар не был виден в размер, при котором слайдбар появится - мы не учитываем слайдбар и все едет
        // При ресайзе от большого к маленькому - слайдбар есть и мы его тоже не учитываем - получается отступ
        public void ReloadSizes()
        {
            MainWrapPanel.Children.Clear();

            // TODO нужна считалка для imagesPath - проверять влазиет ли коллекция с учетом всех картинок данноый высоты в видимую часть
            // Влазиет - ширина без скрола. Не влазиет - со скроллом.

            var coll = ClassDir.AutoStackImage.DynamicRowFormatter(imagesPath, MainWrapPanel.ActualWidth);

            foreach (var item in coll)
            {
                MainWrapPanel.Children.Add(item.ImageControl);
            }
        }

        private void SmallSizeButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in imagesPath)
            {
                item.CurrentSize = ClassDir.AutoStackImage.Size.SMALL;
            }

            ReloadSizes();
        }

        private void MedSizeButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in imagesPath)
            {
                item.CurrentSize = ClassDir.AutoStackImage.Size.MEDIUM;
            }

            ReloadSizes();
        }

        private void BigSizeButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in imagesPath)
            {
                item.CurrentSize = ClassDir.AutoStackImage.Size.BIG;
            }

            ReloadSizes();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
