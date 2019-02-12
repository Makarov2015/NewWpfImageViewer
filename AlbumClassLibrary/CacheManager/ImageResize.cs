using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary.CacheManager
{
    /// <summary>
    /// Класс занимается ресайзом картинок
    /// </summary>
    internal static class ImageResize
    {
        /// <summary>
        /// Изменение размера изображения с сохранением пропорций и без потери качества
        /// </summary>
        /// <param name="pathToOriginalFile">Абсолютный путь к оригиналу</param>
        /// <param name="MaxImageSizeToResize">Размер квадрата, в которй вписывать изображение</param>
        /// <returns></returns>
        public static System.Drawing.Bitmap Resize(string pathToOriginalFile, int MaxImageSizeToResize)
        {
            using (var image = System.Drawing.Image.FromFile(pathToOriginalFile))
            {
                return LocalGet(image);
            }

            System.Drawing.Bitmap LocalGet(System.Drawing.Image image)
            {
                int rW = 0;
                int rH = 0;

                double c = 0;

                if (MaxImageSizeToResize > (double)image.Height)
                {
                    c = ((double)image.Height / (double)MaxImageSizeToResize);
                    rW = image.Width;
                    rH = image.Height;
                }
                else
                {
                    c = ((double)image.Height / (double)MaxImageSizeToResize);
                    rW = (int)(image.Width / c);
                    rH = MaxImageSizeToResize;
                }

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

        public static System.Windows.Size Resize(double width, double height, int MaxImageSizeToResize)
        {
            double rW = 0;
            double rH = 0;

            double c = 0;

            if (MaxImageSizeToResize > (double)height)
            {
                c = ((double)height / (double)MaxImageSizeToResize);
                rW = width;
                rH = height;
            }
            else
            {
                c = ((double)height / (double)MaxImageSizeToResize);
                rW = (int)(width / c);
                rH = MaxImageSizeToResize;
            }

            return new System.Windows.Size(rW, rH);
        }
    }
}