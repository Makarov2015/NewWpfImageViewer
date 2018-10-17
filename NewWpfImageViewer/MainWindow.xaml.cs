using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        System.Windows.Threading.DispatcherTimer _resizeTimer = new System.Windows.Threading.DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 500), IsEnabled = false };

        /// <summary>
        /// 
        /// </summary>
        List<Classes.AutoStackImage> imagesPath = new List<Classes.AutoStackImage>();

        public MainWindow()
        {
            InitializeComponent();

            foreach (var item in System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)).Where(x => x.EndsWith(".gif") || x.EndsWith(".jpeg") || x.EndsWith(".jpg")))
            {
                imagesPath.Add(new Classes.AutoStackImage(item));
            }

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

        public void ReloadSizes()
        {
            MainWrapPanel.Children.Clear();

            foreach (var item in Classes.AutoStackImage.DynamicRowFormatter(imagesPath, MainWrapPanel.ActualWidth))
            {
                MainWrapPanel.Children.Add(item.ImageControl);
            }

            GC.Collect();
        }
    }
}
