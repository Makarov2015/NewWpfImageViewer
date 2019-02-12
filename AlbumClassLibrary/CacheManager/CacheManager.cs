using System;
using System.Drawing;

namespace AlbumClassLibrary.CacheManager
{
    /// <summary>
    /// Класс управления кешем картинок. Имеет единственный публичный метод GetImage и свойство CacheSize
    /// </summary>
    public class CacheManager : IDisposable
    {
        #region Fields

        /// <summary>
        /// Путь к файлу БД
        /// </summary>
        private readonly string _cacheFilePath;

        private const int MaxResizedSize = 350;

        #endregion

        #region Properties

        /// <summary>
        /// Размер файла кеша
        /// </summary>
        public long CacheSize
        {
            get
            {
                long a = new System.IO.FileInfo(_cacheFilePath).Length;
                return (a / 1024) / 1024;
            }
        }

        #endregion

        #region Constructor

        public CacheManager(string CacheFilePath)
        {
            _cacheFilePath = CacheFilePath;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Получение кешированной(и уменьшиной) копии картинки
        /// </summary>
        /// <param name="pathToFile">Путь к оригиналу картинки</param>
        /// <param name="forCalculate">Для расчета - пустая картинка, иначе - изображение</param>
        /// <returns>Кешированная версия</returns>
        public Bitmap GetImage(string pathToFile)
        {
            Bitmap gettedBitmap = null;

            using (DataBaseController controller = new DataBaseController(_cacheFilePath))
            {
                if (controller.CacheSeach(pathToFile, out byte[] bytes))
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes))
                    using (WrappingStream wrapper = new WrappingStream(ms))
                    {
                        gettedBitmap = new Bitmap(wrapper);
                    }
                }
                else
                {
                    gettedBitmap = ImageResize.Resize(pathToFile, MaxResizedSize);
                    controller.CacheAdd(pathToFile, ImageToByteArray(gettedBitmap, System.Drawing.Imaging.ImageFormat.Png));
                }
            }

            return gettedBitmap;

            byte[] ImageToByteArray(Image imageIn, System.Drawing.Imaging.ImageFormat format)
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                imageIn.Save(ms, format);
                return ms.ToArray();
            }
        }

        public System.Windows.Size GetSize(string pathToFile)
        {
            using (DataBaseController controller = new DataBaseController(_cacheFilePath))
            {
                if (controller.CacheSeach(pathToFile, out System.Windows.Size _size))
                {
                    return _size;
                }
                else
                {
                    using (System.IO.FileStream file = new System.IO.FileStream(pathToFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        using (Image tif = Image.FromStream(stream: file,
                                                            useEmbeddedColorManagement: false,
                                                            validateImageData: false))
                        {
                            float width = tif.PhysicalDimension.Width;
                            float height = tif.PhysicalDimension.Height;
                            float hresolution = tif.HorizontalResolution;
                            float vresolution = tif.VerticalResolution;

                            _size = new System.Windows.Size(width, height);
                        }

                        var _final = ImageResize.Resize(_size.Width, _size.Height, MaxResizedSize);
                        controller.SizeAdd(pathToFile, _final);

                        return _final;
                    }
                }
            }
        }

        #endregion

        #region Events

        #endregion

        #region Destructors

        #endregion

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
