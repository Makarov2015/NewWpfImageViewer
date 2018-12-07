using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

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
                long a = new FileInfo(_cacheFilePath).Length;
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
        /// <returns>Кешированная версия</returns>
        public Bitmap GetImage(string pathToFile)
        {
            Bitmap gettedBitmap = null;

            using (DataBaseController controller = new DataBaseController(_cacheFilePath))
            {
                if (controller.CacheSeach(pathToFile, out byte[] bytes))
                {
                    using (MemoryStream ms = new MemoryStream(bytes))
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
                MemoryStream ms = new MemoryStream();
                imageIn.Save(ms, format);
                return ms.ToArray();
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
