using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace NewWpfImageViewer.Classes
{
    public class AutoStackImage
    {
        /// <summary>
        /// Исходное озображение вписанное в максимальный размер отображаемых строк
        /// </summary>
        private System.Drawing.Image MaxSizedImage { get; }

        private int MaxImageSizeToResize => 350;

        /// <summary>
        /// 
        /// </summary>
        public double Height { get; } = 300;
        public double Width
        {
            get
            {
                var r = ((this.Height * MaxSizedImage.Width) / MaxSizedImage.Height) + WidthAdded;
                return r < 0 ? ((this.Height * MaxSizedImage.Width) / MaxSizedImage.Height) / 3 : r;
            }
        }

        public Image ImageControl
        {
            get
            {
                return new Image { Source = GetSource(new System.Drawing.Bitmap(ResizeImage(MaxSizedImage))), Width = this.Width, Height = this.Height, Margin = new System.Windows.Thickness(5) };
            }
        }

        /// <summary>
        ///  Добавочная величина для динамического изменения ширины контролов
        /// </summary>
        public double WidthAdded { get; set; }
        
        public AutoStackImage(string ImgPath)
        {
            MaxSizedImage = ResizeImage(System.Drawing.Image.FromFile(ImgPath));
        }

        private System.Drawing.Image ResizeImage(System.Drawing.Image image)
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

        private BitmapSource GetSource(System.Drawing.Bitmap image)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, null);
        }

        /// <summary>
        /// Метод подбирает оптимальную ширину картинок в ряду, переносит при возможности картинку в ряд выше, сужает и расширяет картинки для заполнения пространства
        /// C учетом внешних отступов картинок (считаем что они у всех одинаковые) и наличия скролбара
        /// </summary>
        /// <returns>Возвращает измененный список AutoStackImage с заполненным полем WidthAdded</returns>
        public static List<AutoStackImage> DynamicRowFormatter(List<Classes.AutoStackImage> images, double wrapPanel)
        {
            // При множестве ресайзов - будет неправильный результат
            foreach (var item in images)
            {
                item.WidthAdded = 0;
            }

            // Наполняем этот список, пока не забьем ряд
            List<Classes.AutoStackImage> currentRow = new List<Classes.AutoStackImage>();
            // Наполняем этот список заполненными рядами
            List<Classes.AutoStackImage> finalRow = new List<Classes.AutoStackImage>();

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
    }
}
