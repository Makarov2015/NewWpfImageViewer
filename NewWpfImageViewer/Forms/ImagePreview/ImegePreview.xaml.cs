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
        public event EventHandler HiResLoaded;
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
                    //BackButton.IsEnabled = false;
                    //ForwardButton.IsEnabled = true;

                    //BackPreviewImage = null;
                }
                else if (value >= AutoStackImages.Count - 1)
                {
                    //BackButton.IsEnabled = true;
                    //ForwardButton.IsEnabled = false;

                    //BackPreviewImage = AutoStackImages[value - 1].ImageControl;
                }
                else if (value > AutoStackImages.Count - 1 || value < 0)
                    throw new IndexOutOfRangeException();
                else
                {
                    //BackButton.IsEnabled = true;
                    //ForwardButton.IsEnabled = true;

                    //BackPreviewImage = AutoStackImages[value - 1].ImageControl;
                }

                _currentIndex = value;
            }
        }
        private List<ClassDir.AutoStackImage> AutoStackImages;
        private Dictionary<ClassDir.AutoStackImage, object> cached = null;

        private object _curImage;
        private BitmapSource CurrentImage
        {
            set
            {
                if (AutoStackImages[CurrentIndex].IsAnimation)
                {
                    AnimationProgressBar.Visibility = Visibility.Visible;
                    _curImage = value;

                    WpfAnimatedGif.ImageBehavior.SetAnimatedSource(MainImage, _curImage as BitmapImage);

                    AnimationController = WpfAnimatedGif.ImageBehavior.GetAnimationController(MainImage);
                    AnimationController.CurrentFrameChanged += Controller_CurrentFrameChanged;

                    AnimationProgressBar.Maximum = AnimationController.FrameCount;
                }
                else
                {
                    AnimationProgressBar.Visibility = Visibility.Hidden;
                    _curImage = value;
                    this.MainImage.Source = _curImage as BitmapSource;
                }
            }
        }

        WpfAnimatedGif.ImageAnimationController AnimationController = null;

        public ImegePreview(List<ClassDir.AutoStackImage> gallery, int curIndex, ref Dictionary<ClassDir.AutoStackImage, object> localCacheList)
        {
            InitializeComponent();

            AnimationProgressBar.Loaded += AnimationProgressBar_Loaded;

            cached = localCacheList as Dictionary<ClassDir.AutoStackImage, object>;

            AutoStackImages = gallery;
            CurrentIndex = curIndex;

            LoadMainImageAsync();
        }

        private void AnimationProgressBar_Loaded(object sender, RoutedEventArgs e)
        {
            var p = (ProgressBar)sender;
            p.ApplyTemplate();

            ((System.Windows.Shapes.Rectangle)p.Template.FindName("Animation", p)).Fill = MainWindow.WinColor;
        }

        private async void LoadMainImageAsync()
        {
            this.MainImage.Source = AutoStackImages[CurrentIndex].GetBitmapSource;

            this.Cursor = Cursors.Wait;
            await Task.Delay(1);

            if (!AutoStackImages[CurrentIndex].IsAnimation)
                if (cached.Any(x => x.Key == AutoStackImages[CurrentIndex]))
                {
                    CurrentImage = cached.Single(x => x.Key == AutoStackImages[CurrentIndex]).Value as BitmapSource;

                    this.Cursor = Cursors.Arrow;
                    return;
                }

            if (AutoStackImages[CurrentIndex].IsAnimation)
            {
                CurrentImage = await GetAnimationSource(AutoStackImages[CurrentIndex].OriginalFilepath);
            }
            else
            {
                CurrentImage = await GetNewImageAsync(AutoStackImages[CurrentIndex].OriginalFilepath);
            }

            this.Cursor = Cursors.Arrow;

            if (!AutoStackImages[CurrentIndex].IsAnimation)
                HiResLoaded(new Tuple<ClassDir.AutoStackImage, object>(AutoStackImages[CurrentIndex], _curImage), new EventArgs());

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

                tcs.SetResult(AlbumClassLibrary.Extensions.BitmapSourceExtension.GetSource(path, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height));

                return tcs.Task;
            }
        }

        private void Controller_CurrentFrameChanged(object sender, EventArgs e)
        {
            AnimationProgressBar.Value = (sender as WpfAnimatedGif.ImageAnimationController).CurrentFrame;
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
