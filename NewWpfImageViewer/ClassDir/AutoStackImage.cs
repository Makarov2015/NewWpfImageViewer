using System;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Drawing = System.Drawing;
using Control = System.Windows.Controls;
using AlbumClassLibrary.Extensions;
using System.ComponentModel;

namespace NewWpfImageViewer.ClassDir
{
    /// <summary>
    /// Класс для представления изображения
    /// Ресайзит изображение, хранит сжатый исходник и помогает сделать красивое отображение картинок на форме
    /// (Динамически меняет ширину картинок, подгоняет их так, чтобы ряд был заполнен до конца), а так же помогает менять размер
    /// </summary>
    public class AutoStackImage : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Исходное изображение вписанное в максимальный размер отображаемых строк
        /// </summary>
        public Drawing.Image MaxSizedImage { get; set; }

        public bool IsAnimation => OriginalFilepath.EndsWith(".gif");

        /// <summary>
        /// Максимальная высота, до которой изображение будет уменьшено
        /// </summary>
        private int MaxImageSizeToResize => 350;

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
                var r = ((this.Height * MaxSizedImage.Width) / MaxSizedImage.Height) + WidthAdded;
                return r < 0 ? ((this.Height * MaxSizedImage.Width) / MaxSizedImage.Height) / 3 : r;
            }
        }

        /// <summary>
        /// Тут возвращаем готовый контрол, который сразу закидываем на форму
        /// </summary>
        private Control.Image imageControl;

        public event PropertyChangedEventHandler PropertyChanged;

        public Control.Image ImageControl
        {
            get
            {
                imageControl.Width = this.Width;
                imageControl.Height = this.Height;

                return imageControl;
            }
        }

        public BitmapSource GetBitmapSource => BitmapSourceExtension.GetSource(this.MaxSizedImage as Drawing.Bitmap);

        /// <summary>
        ///  Добавочная величина для динамического изменения ширины контролов
        /// </summary>
        public double WidthAdded { get; set; }

        /// <summary>
        /// Хранилка текущего размера
        /// </summary>
        public Size CurrentSize { get; set; }

        public string OriginalFilepath { get; }

        /// <summary>
        /// Констуктор через кеш
        /// </summary>
        /// <param name="cachedImage">Ресайзнутое изображение</param>
        public AutoStackImage(Drawing.Image cachedImage, string Original)
        {
            // Без этого юзинга память не чистится
            using (cachedImage)
                MaxSizedImage = new Drawing.Bitmap(cachedImage);

            OriginalFilepath = Original;

            imageControl = new Control.Image { Source = GetBitmapSource, Width = this.Width, Height = this.Height, Margin = new System.Windows.Thickness(5), Stretch = System.Windows.Media.Stretch.UniformToFill, StretchDirection = Control.StretchDirection.Both };
        }

        public void Dispose()
        {
            this.imageControl.Source.Freeze();
            this.imageControl.Source = null;
            this.imageControl = null;

            this.MaxSizedImage.Dispose();
        }
    }
}

