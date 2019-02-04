using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AlbumClassLibrary.Extensions
{
    /// <summary>
    /// Класс борьбы с утечкой памяти <see cref="BitmapSource"/> при получении сурса картинки. Без него сурс оставляет ссылку на байты картинки и сборщик мусора не убирает их
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/1546091/wpf-createbitmapsourcefromhbitmap-memory-leak"/>
    public static class BitmapSourceExtension
    {
        public static BitmapSource GetSource(System.Drawing.Bitmap bitmap)
        {
            lock (bitmap)
            {
                BitmapSource source;

                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(bitmap))
                {
                    IntPtr hBitmap = bmp.GetHbitmap();

                    try
                    {
                        source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    }
                    finally
                    {
                        DeleteObject(hBitmap);
                    }
                }

                source.Freeze();
                return source; 
            }
        }

        public static BitmapSource GetSource(string path, int squareSide)
        {
            BitmapSource source;

            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(CacheManager.ImageResize.Resize(path, squareSide)))
            {
                IntPtr hBitmap = bmp.GetHbitmap();

                try
                {
                    source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    DeleteObject(hBitmap);
                }
            }

            source.Freeze();
            return source;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hObject);
    }
}
