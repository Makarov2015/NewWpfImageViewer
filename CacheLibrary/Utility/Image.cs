using System.Drawing.Drawing2D;
using Drawing = System.Drawing;
using System.Drawing;

namespace CacheClassLibrary.Utility
{
    internal class Image
    {
        public static Bitmap RealResize(Drawing.Image image, int MaxImageSizeToResize)
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

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static Size VirtualResize(double width, double height, int MaxImageSizeToResize)
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

            return new Size((int)rW, (int)rH);
        }
    }
}
