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

namespace NewWpfImageViewer.ClassDir
{
    /// <summary>
    /// Класс для представления изображения
    /// Ресайзит изображение, хранит сжатый исходник и помогает сделать красивое отображение картинок на форме
    /// (Динамически меняет ширину картинок, подгоняет их так, чтобы ряд был заполнен до конца), а так же помогает менять размер
    /// </summary>
    public class AutoSizeImage : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Исходное изображение вписанное в максимальный размер отображаемых строк
        /// </summary>
        public Drawing.Image MaxSizedImage { get; set; }

        public Point Position { get; set; }

        public bool IsAnimation => OriginalFilepath.EndsWith(".gif");

        public enum Size
        {
            MEDIUM,
            SMALL,
            BIG
        }

        /// <summary>
        /// Доступные варианты высоты
        /// </summary>
        public double Height
        {
            get
            {
                switch (CurrentSize)
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
                    var r = ((this.Height * MaxSizedImage.Width) / MaxSizedImage.Height) + WidthAdded;
                    return r < 0 ? ((this.Height * MaxSizedImage.Width) / MaxSizedImage.Height) / 3 : r; 
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private System.Windows.Media.ImageSource _source;

        public System.Windows.Media.ImageSource BitmapSource
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
            }
        }
        public void VisabilityChanged(object sender, EventArgs e)
        {
            Control.ScrollViewer viewer = sender as Control.ScrollViewer;

            if (Position.Y >= viewer.VerticalOffset - Height * 5 && Position.Y <= viewer.VerticalOffset + viewer.ActualHeight + Height * 3)
            {
                this.LoadSourceAsync();
            }
            else
            {
                this.DisableSource();
            }
        }

        public async void LoadSourceAsync()
        {
            if(BitmapSource is null)
            {
                BitmapSource = await GetBitmapSource;
                OnPropertyChanged("BitmapSource");
            }
        }

        public void LoadSource()
        {
            if (BitmapSource is null)
            {
                BitmapSource = GetBitmapSource.Result;
                OnPropertyChanged("BitmapSource");
            }
        }

        public void DisableSource()
        {
            if (BitmapSource != null)
            {
                BitmapSource = null;
                OnPropertyChanged("BitmapSource"); 
            }
        }

        public Task<BitmapSource> GetBitmapSource =>
            Task.Run(() =>
            {
                lock (this)
                {
                    if (this._isForDispose)
                        return null;

                    return BitmapSourceExtension.GetSource(this.MaxSizedImage as Drawing.Bitmap, this); 
                }
            });

        /// <summary>
        ///  Добавочная величина для динамического изменения ширины контролов
        /// </summary>
        public double WidthAdded { get; set; }


        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Хранилка текущего размера
        /// </summary>
        private Size _curSize;
        public Size CurrentSize
        {
            get
            {
                return _curSize;
            }
            set
            {
                _curSize = value;
            }
        }

        public void ChangeTest() => OnPropertyChanged("Size");

        public string OriginalFilepath { get; }

        /// <summary>
        /// Констуктор через кеш
        /// </summary>
        /// <param name="cachedImage">Ресайзнутое изображение</param>
        public AutoSizeImage(Drawing.Image cachedImage, string Original)
        {
            // Без этого юзинга память не чистится
            using (cachedImage)
                MaxSizedImage = new Drawing.Bitmap(cachedImage);

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

