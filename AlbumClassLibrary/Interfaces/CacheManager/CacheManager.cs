using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary.Interfaces.CacheManager
{
    /// <summary>
    /// Во время использования кеша мы только получаем от него файлы, запрашивая их по пути к оригиналу
    /// Кеш должен диспоузится и инициализироваться в консtрукции using
    /// </summary>
    public interface ICacheManager : IDisposable
    {
        System.Drawing.Bitmap GetImage(string pathToFile);
    }

    public abstract class CacheManagerBase : ICacheManager
    {
        #region Fields

        private readonly string _cacheFolderPath;
        private const string _cacheTableFileName = "_cache";

        private Dictionary<string, string> _cacheTableDictionary = new Dictionary<string, string>();

        #endregion

        #region Properties

        private string CacheTableFilePath => _cacheFolderPath + _cacheTableFileName;

        #endregion

        #region Constructor

        public CacheManagerBase(string CacheFolderPath)
        {
            _cacheFolderPath = CacheFolderPath;
            InitialCacheCheck();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Проверка указанной директории кеша
        /// </summary>
        /// <remarks>Проверка на наличие директории, файла кеша в ней, читабельности файла, наличия прав на запись в директорию и ее доступность</remarks>
        /// <returns></returns>
        private void InitialCacheCheck()
        {
            if (Directory.Exists(_cacheFolderPath))
            {
                if (File.Exists(CacheTableFilePath))
                {
                    // читаем файл
                }
                else
                {
                    // создаем файл
                }
            }
            else
            {
                // создаем директорию
                // создаем пустой файл
            }
        }

        #endregion

        #region Events

        #endregion

        #region Destructors

        #endregion

        string PathToCacheFile { get; set; }

        ///// <summary>
        ///// Кеш - это пара "путь оригинала" - "путь к кешу"
        ///// Файл один на всех и зашифрован
        ///// </summary>
        //Dictionary<string, string> CachePairs { get; set; }

        ///// <summary>
        ///// Чтение \ сохранение таблицы кеша
        ///// </summary>
        ///// <returns></returns>
        //Dictionary<string, string> LoadCachePairsFromFile();
        //void SaveCachePairsToFile();

        ///// <summary>
        ///// Проверка наличия файла в кеше
        ///// </summary>
        ///// <param name="path">Путь к оригиналу</param>
        ///// <param name="name">Название файла кеша (полный путь не нужен - файлы в CacheFilePath)</param>
        ///// <returns></returns>
        //bool Search(string path, out string name);

        //// Картинка в байты
        //byte[] ImageToByteArray(System.Drawing.Image imageIn, System.Drawing.Imaging.ImageFormat format);

        ////Байты в файл
        //bool ByteArrayToFile(string fileName, byte[] byteArray) { }

        //// Файл из кеша в картинку
        //System.Drawing.Bitmap ByteArrayToImage(string pathToFile) { }

        Bitmap ICacheManager.GetImage(string pathToFile)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class CacheManager : CacheManagerBase
    {
        public CacheManager(string CacheFolder) : base(CacheFolder) { }
    }
}
