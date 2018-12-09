using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace NewWpfImageViewer.Forms.ImagePreview
{
    /// <summary>
    /// Логика взаимодействия для ImegePreview.xaml
    /// </summary>
    public partial class ImegePreview : UserControl
    {
        public event EventHandler Close;

        private int _currentIndex;
        private int CurrentIndex
        {
            get
            {
                return _currentIndex;
            }
            set
            {
                if (value == 0)
                {
                    BackButton.IsEnabled = false;
                    ForwardButton.IsEnabled = true;

                    BackPreviewImage = null;
                }
                else if (value >= AutoStackImages.Count - 1)
                {
                    BackButton.IsEnabled = true;
                    ForwardButton.IsEnabled = false;

                    BackPreviewImage = AutoStackImages[value - 1].ImageControl;
                }
                else if (value > AutoStackImages.Count - 1 || value < 0)
                    throw new IndexOutOfRangeException();
                else
                {
                    BackButton.IsEnabled = true;
                    ForwardButton.IsEnabled = true;

                    BackPreviewImage = AutoStackImages[value - 1].ImageControl;
                }

                _currentIndex = value;
            }
        }
        private List<ClassDir.AutoStackImage> AutoStackImages;

        private BitmapSource _curImage;
        private BitmapSource CurrentImage
        {
            set
            {
                _curImage = value;
                this.MainImage.Source = _curImage;
            }
        }

        public ImegePreview(List<ClassDir.AutoStackImage> gallery, int curIndex)
        {
            InitializeComponent();

            AutoStackImages = gallery;
            CurrentIndex = curIndex;

            LoadMainImageAsync();
        }

        private async void LoadMainImageAsync()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            CurrentImage = AutoStackImages[CurrentIndex].GetBitmapSource;

            System.Diagnostics.Debug.WriteLine(sw.Elapsed);
            sw.Restart();

            await Task.Delay(1);

            if (AutoStackImages[CurrentIndex].IsAnimation)
            {
                this.Cursor = Cursors.Wait;

                var im = await GetAnimationSource(AutoStackImages[CurrentIndex].OriginalFilepath);
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(MainImage, im);

                this.Cursor = Cursors.Arrow;
            }
            else
            {
                CurrentImage = await GetNewImageAsync(AutoStackImages[CurrentIndex].OriginalFilepath);
            }

            System.Diagnostics.Debug.WriteLine(sw.Elapsed);
            sw.Stop();

            Task<BitmapImage> GetAnimationSource(string path)
            {
                return Task.Run<BitmapImage>(() =>
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(path);
                    image.EndInit();

                    image.Freeze();

                    return image;
                });
            }

            Task<BitmapSource> GetNewImageAsync(string path)
            {
                var tcs = new TaskCompletionSource<BitmapSource>();
                var bitmap = new BitmapImage();

                tcs.SetResult(AlbumClassLibrary.Extensions.BitmapSourceExtension.GetSource(System.Drawing.Bitmap.FromFile(path) as System.Drawing.Bitmap));

                return tcs.Task;
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentIndex++;

            this.MainImage.Source.Freeze();

            GC.Collect();

            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                System.Threading.Thread.Sleep(500);
                GC.Collect();
            }));

            thread.Start();

            this.MainImage.Source = null;
            LoadMainImageAsync();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentIndex--;

            this.MainImage.Source.Freeze();

            GC.Collect();

            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                System.Threading.Thread.Sleep(500);
                GC.Collect();
            }));

            thread.Start();

            this.MainImage.Source = null;
            LoadMainImageAsync();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(this, new EventArgs());
        }
    }
}
