using System.IO;
using System.Drawing;

namespace CacheClassLibrary.IO
{
    internal static class Image
    {
        public static Size GetImageSize(string pathToFile)
        {
            using (FileStream file = new FileStream(pathToFile, FileMode.Open, FileAccess.Read))
            {
                using (System.Drawing.Image tif = System.Drawing.Image.FromStream(stream: file,
                                                    useEmbeddedColorManagement: false,
                                                    validateImageData: false))
                {
                    float width = tif.PhysicalDimension.Width;
                    float height = tif.PhysicalDimension.Height;
                    float hresolution = tif.HorizontalResolution;
                    float vresolution = tif.VerticalResolution;

                    return new Size((int)width, (int)height);
                }
            }
        }

        public static Bitmap GetResizedImage(string pathToOriginalFile, int MaxImageSizeToResize)
        {
            using (var image = System.Drawing.Image.FromFile(pathToOriginalFile))
            {
                return Utility.Image.RealResize(image, MaxImageSizeToResize);
            }
        }
    }
}
