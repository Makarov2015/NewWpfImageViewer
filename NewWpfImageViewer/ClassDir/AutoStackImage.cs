using System;
using System.Collections.Generic;
using Drawing = System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Control = System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Runtime.Caching;

namespace NewWpfImageViewer.ClassDir
{
    public class AutoStackImage
    {
        /// <summary>
        /// Исходное озображение вписанное в максимальный размер отображаемых строк
        /// </summary>
        public Drawing.Image MaxSizedImage { get; }

        private int MaxImageSizeToResize => 400;
        
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

        public double Width
        {
            get
            {
                var r = ((this.Height * MaxSizedImage.Width) / MaxSizedImage.Height) + WidthAdded;
                return r < 0 ? ((this.Height * MaxSizedImage.Width) / MaxSizedImage.Height) / 3 : r;
            }
        }

        private Control.Image imageControl;
        public Control.Image ImageControl
        {
            get
            {
                imageControl.Width = this.Width;
                imageControl.Height = this.Height;

                return imageControl;
            }
        }

        /// <summary>
        ///  Добавочная величина для динамического изменения ширины контролов
        /// </summary>
        public double WidthAdded { get; set; }

        public enum Size
        {
            MEDIUM,
            SMALL,
            BIG
        }

        public Size CurrentSize { get; set; }

        public AutoStackImage(string ImgPath)
        {
            MaxSizedImage = ResizeImage(ImgPath);
            imageControl = new Control.Image { Source = GetSource(this.MaxSizedImage as Drawing.Bitmap), Width = this.Width, Height = this.Height, Margin = new System.Windows.Thickness(5), Stretch = System.Windows.Media.Stretch.UniformToFill, StretchDirection = Control.StretchDirection.Both };
        }

        public AutoStackImage(Drawing.Image cachedImage)
        {
            MaxSizedImage = cachedImage;
            imageControl = new Control.Image { Source = GetSource(this.MaxSizedImage as Drawing.Bitmap), Width = this.Width, Height = this.Height, Margin = new System.Windows.Thickness(5), Stretch = System.Windows.Media.Stretch.UniformToFill, StretchDirection = Control.StretchDirection.Both };
        }

        private Drawing.Image CropImageForSizing(Drawing.Image img, Drawing.Rectangle cropArea)
        {
            Drawing.Bitmap bmpImage = new Drawing.Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        private System.Drawing.Image ResizeImage(string path, Drawing.Image img = null)
        {
            if (String.IsNullOrEmpty(path) && img != null)
            {
                return LocalGet(img);
            }
            else
            {
                using (var image = Drawing.Image.FromFile(path))
                {
                    return LocalGet(image);
                }
            }

            System.Drawing.Bitmap LocalGet(System.Drawing.Image image)
            {
                int rW = 0;
                int rH = 0;

                double c = 0;
                c = ((double)image.Height / (double)MaxImageSizeToResize);
                rW = (int)(image.Width / c);
                rH = MaxImageSizeToResize;

                var destRect = new System.Drawing.Rectangle(0, 0, rW, rH);
                var destImage = new System.Drawing.Bitmap(rW, rH);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = System.Drawing.Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel, wrapMode);
                    }
                }

                return destImage;
            }
        }

        private BitmapSource GetSource(System.Drawing.Bitmap image)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, null);
        }

        /// <summary>
        /// Метод подбирает оптимальную ширину картинок в ряду, переносит при возможности картинку в ряд выше, сужает и расширяет картинки для заполнения пространства
        /// C учетом внешних отступов картинок (считаем что они у всех одинаковые) и наличия скролбара
        /// </summary>
        /// <returns>Возвращает измененный список AutoStackImage с заполненным полем WidthAdded</returns>
        public static List<AutoStackImage> DynamicRowFormatter(List<ClassDir.AutoStackImage> images, double wrapPanel)
        {
            // При множестве ресайзов - будет неправильный результат
            foreach (var item in images)
            {
                item.WidthAdded = 0;
            }

            // Наполняем этот список, пока не забьем ряд
            List<ClassDir.AutoStackImage> currentRow = new List<ClassDir.AutoStackImage>();
            // Наполняем этот список заполненными рядами
            List<ClassDir.AutoStackImage> finalRow = new List<ClassDir.AutoStackImage>();

            // Сумма внешних отступов для картинок
            var marginSum = images.First(x => x.ImageControl != null).ImageControl.Margin.Left * 2;

            double panelWidth = wrapPanel;

            for (int i = 0; i < images.Count; i++)
            {
                // Сумма в ряду
                double sumOfCurrent = currentRow.Sum(x => x.Width);
                // Текущий размер картинки
                double currentSize = images[i].Width;
                // Остается места
                double freeSpace = panelWidth - sumOfCurrent - (marginSum * currentRow.Count);

                // Если места не хватает
                if (freeSpace < images[i].Width + marginSum)
                {
                    // Места мало - текущий ряд не меняется в составе, но растягивается на остаток
                    if (freeSpace < currentSize / 2)
                    {
                        // Посчитали надбавку
                        double needToAdd = freeSpace / currentRow.Count;

                        // Добавили надбавку
                        foreach (var predicredRow in currentRow)
                        {
                            predicredRow.WidthAdded = needToAdd;
                        }

                        // И теперь складываем измененные картинки в конечный лист
                        finalRow.AddRange(currentRow.ToList());
                        currentRow.Clear();

                        currentRow.Add(images[i]);
                    }
                    // Места много - в текущий ряд добавляем картинку из следующего, эту картинку удаляем оттуда и текущий ряд сжимаем для 
                    else
                    {
                        // Добавили первую из следующего ряда
                        currentRow.Add(images[i]);

                        // Считаем на сколько надо всех уменьшить
                        var curSize = ((currentRow.Sum(x => x.Width) + marginSum * currentRow.Count()) - panelWidth) / currentRow.Count();

                        // Добавили надбавку
                        foreach (var predicredRow in currentRow)
                        {
                            predicredRow.WidthAdded = curSize * (-1);
                        }

                        finalRow.AddRange(currentRow.ToList());
                        currentRow.Clear();
                    }
                }
                else
                {
                    currentRow.Add(images[i]);

                    // BUG Иногда пропадает последняя картинка
                    if (images[i] == images.Last())
                    {
                        finalRow.AddRange(currentRow.ToList());
                        break;
                    }
                }
            }

            return finalRow;
        }

        public static bool CheckScrollNeeded(List<AutoStackImage> autoStacks, double visPanelHeight, double visPanelWidth)
        {
            return false;
        }
    }
}
