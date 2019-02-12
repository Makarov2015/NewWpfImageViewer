using System;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;
using Control = System.Windows.Controls;
using AlbumClassLibrary.Extensions;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using NewWpfImageViewer.Interfaces;
using Size = NewWpfImageViewer.Interfaces.Size;

namespace NewWpfImageViewer.ClassDir
{
    public class asc1
    {
        void Main()
        {
            //ImageStructure autoSize = new ImageStructure(null, null);
            //autoSize.
        }
    }

    /// <summary>
    /// Класс для представления изображения
    /// Ресайзит изображение, хранит сжатый исходник и помогает сделать красивое отображение картинок на форме
    /// (Динамически меняет ширину картинок, подгоняет их так, чтобы ряд был заполнен до конца), а так же помогает менять размер
    /// </summary>
    public class ImageStructure : IImageStructure
    {
        /// <summary>
        /// Исходное изображение вписанное в максимальный размер отображаемых строк
        /// </summary>
        private Drawing.Image MaxSizedImage { get; set; }

        public event EventHandler ImageNeeded;

        private System.Windows.Size _imageSize;
        private System.Windows.Size ImageSize
        {
            get
            {
                return _imageSize;
            }
            set
            {
                _imageSize = value;
            }
        }


        public Point Position { get; set; }

        public bool IsAnimation => OriginalFilepath.EndsWith(".gif");

        /// <summary>
        /// Доступные варианты высоты
        /// </summary>
        public double Height
        {
            get
            {
                switch (Size)
                {
                    case Size.MEDIUM:
                        return 250;
                    case Size.SMALL:
                        return 150;
                    case Size.BIG:
                        return 350;
                    default:
                        return 250;
                }
            }
        }

        /// <summary>
        /// Динамический подсчет ширины в зависимости от подгона и масштаба
        /// </summary>
        public double Width
        {
            get
            {
                lock (this)
                {
                    if (MaxSizedImage is null)
                    {
                        var r = ((this.Height * ImageSize.Width) / ImageSize.Height) + WidthAdded;
                        return r < 0 ? ((this.Height * ImageSize.Width) / ImageSize.Height) / 3 : r;
                    }
                    else
                    {
                        var r = ((this.Height * MaxSizedImage.Width) / MaxSizedImage.Height) + WidthAdded;
                        return r < 0 ? ((this.Height * MaxSizedImage.Width) / MaxSizedImage.Height) / 3 : r;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public System.Windows.Media.ImageSource BitmapSource { get; private set; }

        public void VisabilityChanged(object sender, EventArgs e)
        {
            Control.ScrollViewer viewer = sender as Control.ScrollViewer;

            if (Position.Y >= viewer.VerticalOffset - Height * 5 && Position.Y <= viewer.VerticalOffset + viewer.ActualHeight + Height * 3)
            {
                this.BitmapSourceEnableAsync();
            }
            else if (Position.Y >= viewer.VerticalOffset - Height * 15 && Position.Y <= viewer.VerticalOffset + viewer.ActualHeight + Height * 20)
            {
                return;
            }
            else
            {
                this.BitmapSourceDisableAsync();
            }
        }

        public void LoadMaxSizedImage(System.Drawing.Bitmap cachedImage)
        {
            this.MaxSizedImage = new Drawing.Bitmap(cachedImage);
        }

        public bool MaxSizeIsLoaded() => MaxSizedImage != null;

        public async void BitmapSourceEnableAsync()
        {
            if (BitmapSource is null)
            {
                ImageNeeded(this, new EventArgs());
                BitmapSource = await GetBitmapSource;
                OnPropertyChanged("BitmapSource");
            }
        }

        public async void BitmapSourceDisableAsync()
        {
            if (BitmapSource != null)
            {
                await Task.Run(() =>
                {
                    if (MaxSizedImage != null)
                        MaxSizedImage.Dispose();

                    MaxSizedImage = null;
                    BitmapSource = null;
                });

                OnPropertyChanged("BitmapSource");
            }
        }

        private Task<BitmapSource> GetBitmapSource =>
            Task.Run(() =>
            {
                lock (this)
                {
                    if (this._isForDispose)
                        return null;

                    if (MaxSizedImage is null)
                        return null;
                    else
                        return BitmapSourceExtension.GetSource(this.MaxSizedImage as Drawing.Bitmap, this);
                }
            });

        /// <summary>
        ///  Добавочная величина для динамического изменения ширины контролов
        /// </summary>
        private double _widthAdded;
        public double WidthAdded
        {
            get
            {
                return _widthAdded;
            }
            set
            {
                _widthAdded = value;
                OnPropertyChanged("Width");
            }
        }


        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Хранилка текущего размера
        /// </summary>
        private Size _curSize;
        public Size Size
        {
            get
            {
                return _curSize;
            }
            set
            {
                _curSize = value;
                OnPropertyChanged("Height");
            }
        }

        public string OriginalFilepath { get; }

        /// <summary>
        /// Констуктор через кеш
        /// </summary>
        /// <param name="cachedImage">Ресайзнутое изображение</param>
        public ImageStructure(Drawing.Image cachedImage, string Original)
        {
            // Без этого юзинга память не чистится
            using (cachedImage)
                MaxSizedImage = new Drawing.Bitmap(cachedImage);

            OriginalFilepath = Original;
        }

        public ImageStructure(System.Windows.Size size, string Original)
        {
            MaxSizedImage = null;
            this.ImageSize = size;
            OriginalFilepath = Original;
        }

        private bool _isForDispose = false;

        public void Dispose()
        {
            if (this.BitmapSource is null)
                return;

            this.BitmapSource.Freeze();
            this.BitmapSource = null;

            _isForDispose = true;

            this.MaxSizedImage.Dispose();
        }
    }
}

