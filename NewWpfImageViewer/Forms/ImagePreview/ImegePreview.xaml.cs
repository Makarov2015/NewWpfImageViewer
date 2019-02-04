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
                _currentIndex = value;
                IndexChanged(_currentIndex);
            }
        }
        private List<ClassDir.AutoSizeImage> AutoStackImages;
        private Dictionary<ClassDir.AutoSizeImage, object> cached = null;

        private object _curImage;
        private BitmapSource CurrentImage
        {
            set
            {
                if (AutoStackImages[CurrentIndex].IsAnimation)
                {
                    _curImage = value;

                    WpfAnimatedGif.ImageBehavior.SetAnimatedSource(MainImage, _curImage as BitmapImage);

                    AnimationController = WpfAnimatedGif.ImageBehavior.GetAnimationController(MainImage);

                    if (AnimationController != null)
                        AnimationController.CurrentFrameChanged += Controller_CurrentFrameChanged;
                    else
                        return;

                    AnimationProgressBar.Maximum = AnimationController.FrameCount;
                    AnimationProgressBar.Visibility = Visibility.Visible;
                }
                else
                {
                    if (AnimationController != null)
                        AnimationController.Dispose();

                    AnimationProgressBar.Visibility = Visibility.Hidden;
                    _curImage = value;
                    this.MainImage.Source = _curImage as BitmapSource;
                }
            }
        }

        WpfAnimatedGif.ImageAnimationController AnimationController = null;

        private readonly Window _parent;

        public ImegePreview(List<ClassDir.AutoSizeImage> gallery, int curIndex, ref Dictionary<ClassDir.AutoSizeImage, object> localCacheList, Window parent)
        {
            _parent = parent;

            InitializeComponent();
            AnimationProgressBar.Loaded += AnimationProgressBar_Loaded;

            cached = localCacheList as Dictionary<ClassDir.AutoSizeImage, object>;

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

        private object _lock = new object();

        private async void LoadMainImageAsync()
        {
            _parent.Focus();
            var _size = (double)(new System.IO.FileInfo(AutoStackImages[CurrentIndex].OriginalFilepath).Length / 1024F / 1024F);

            this._parent.Title = $"({_size.ToString("F2")}" + " Mb) " + System.IO.Path.GetFileName(AutoStackImages[CurrentIndex].OriginalFilepath);

            this.Cursor = Cursors.Wait;
            await Task.Delay(1);

            if (!AutoStackImages[CurrentIndex].IsAnimation)
                if (cached.Any(x => x.Key == AutoStackImages[CurrentIndex]))
                {
                    CurrentImage = cached.Single(x => x.Key == AutoStackImages[CurrentIndex]).Value as BitmapSource;

                    this.Cursor = Cursors.Arrow;
                    ChangeSize();
                    return;
                }

            this.MainImage.Source = await AutoStackImages[CurrentIndex].GetBitmapSource;

            if (AutoStackImages[CurrentIndex].IsAnimation)
            {
                CurrentImage = await GetAnimationSource(AutoStackImages[CurrentIndex].OriginalFilepath);
            }
            else
            {
                CurrentImage = await GetNewImageAsync(AutoStackImages[CurrentIndex].OriginalFilepath);
            }

            this.Cursor = Cursors.Arrow;
            ChangeSize();

            if (!AutoStackImages[CurrentIndex].IsAnimation)
                HiResLoaded(new Tuple<ClassDir.AutoSizeImage, object>(AutoStackImages[CurrentIndex], _curImage), new EventArgs());

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
                return Task.Run<BitmapSource>(() =>
                {
                    var a = AlbumClassLibrary.Extensions.BitmapSourceExtension.GetSource(path, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
                    a.Freeze();

                    return a;
                });
            }
        }

        private void Controller_CurrentFrameChanged(object sender, EventArgs e)
        {
            AnimationProgressBar.Value = (sender as WpfAnimatedGif.ImageAnimationController).CurrentFrame;
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            Forward();
        }

        public void Forward()
        {
            if (AutoStackImages.Count <= CurrentIndex + 1)
                return;

            CurrentIndex++;

            this.MainImage.Source.Freeze();

            GC.Collect();

            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                System.Threading.Thread.Sleep(500);
                GC.Collect();
            }));

            thread.Start();

            LoadMainImageAsync();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }

        public void Back()
        {
            if (CurrentIndex - 1 < 0)
                return;

            CurrentIndex--;

            this.MainImage.Source.Freeze();

            GC.Collect();

            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                System.Threading.Thread.Sleep(500);
                GC.Collect();
            }));

            thread.Start();

            LoadMainImageAsync();
        }

        private void IndexChanged(int currentIndex)
        {
            if (currentIndex + 1 >= AutoStackImages.Count)
                ForwardButton.IsEnabled = false;
            else
                ForwardButton.IsEnabled = true;

            if (currentIndex - 1 < 0)
                BackButton.IsEnabled = false;
            else
                BackButton.IsEnabled = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(this, new EventArgs());
        }

        private void ChangeSize()
        {
            if (MainImage.Source is null)
                return;

            var _sourceWidth = MainImage.Source.Width;
            var _sourceHeight = MainImage.Source.Height;

            int _z = 100;

            MainImage.MaxHeight = this.ActualHeight - _z;
            MainImage.MaxWidth = this.ActualWidth - _z;

            if (MainImage.MaxHeight > _sourceHeight * 2)
                MainImage.MaxHeight = _sourceHeight * 2;

            if (MainImage.MaxWidth > _sourceWidth * 2)
                MainImage.MaxWidth = _sourceWidth * 2;
        }

        public void CloseForm()
        {
            this.Close(this, new EventArgs());
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeSize();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
