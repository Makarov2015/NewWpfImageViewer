using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NewWpfImageViewer.ClassDir
{
    /// <summary>
    /// Класс для управления кешем картинок. В случае использования является фабрикой для AutoStackImage.
    /// </summary>
    public class CacheFileManager : IDisposable
    {
        #region Свойства

        /// <summary>
        /// Ключ - начальный путь, значение - GUID-имя кешированного файла
        /// </summary>
        private Dictionary<string, string> CacheDictionary { get; set; } = new Dictionary<string, string>();
        private string CacheFolder = Properties.Settings.Default.ProgramDataFolder + "_cache\\";
        private string CacheTableFile => CacheFolder + "_cache";

        /// <summary>
        /// Размер папки Кеша в Мегабайтах
        /// </summary>
        public long CacheSize
        {
            get
            {
                string[] a = Directory.GetFiles(CacheFolder);

                long b = 0;
                foreach (string name in a)
                {
                    FileInfo info = new FileInfo(name);
                    b += info.Length;
                }

                return (b / 1024) / 1024;
            }
        }

        #endregion

        public CacheFileManager()
        {
            LoadDictionary();
        }

        /// <summary>
        /// Грузим словарь при инициализации и создаем директорию, если нет
        /// </summary>
        private void LoadDictionary()
        {
            if (Directory.Exists(CacheFolder))
            {
                if (File.Exists(CacheTableFile))
                    CacheDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(File.ReadAllBytes(CacheTableFile)));
            }
            else
            {
                Directory.CreateDirectory(CacheFolder);
            }
        }

        public void SaveDictionary()
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(CacheDictionary);
            ByteArrayToFile(CacheTableFile, System.Text.Encoding.UTF8.GetBytes(json));
        }

        public AutoStackImage GetImage(string pathToFile)
        {
            string findPath;

            if (Search(pathToFile, out findPath))
            {
                return new AutoStackImage(ByteArrayToImage(File.ReadAllBytes(CacheFolder + findPath)));
            }
            else
            {
                AutoStackImage cacheNotFound = new AutoStackImage(pathToFile);
                string guid = Guid.NewGuid().ToString();

                ByteArrayToFile(CacheFolder + guid, ImageToByteArray(cacheNotFound.MaxSizedImage, System.Drawing.Imaging.ImageFormat.Png));
                CacheDictionary.Add(pathToFile, guid);

                return cacheNotFound;
            }
        }

        private bool Search(string path, out string name)
        {
            try
            {
                name = CacheDictionary.Where(x => x.Key == path).First().Value;
                return true;
            }
            catch (Exception)
            {
                name = null;
                return false;
            }
        }

        public string Search(string path)
        {
            try
            {
                return CacheFolder + CacheDictionary.Where(x => x.Key == path).First().Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool Add()
        {
            return false;
        }

        private bool Remove()
        {
            return false;
        }

        private byte[] ImageToByteArray(System.Drawing.Image imageIn, System.Drawing.Imaging.ImageFormat format)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            imageIn.Save(ms, format);
            return ms.ToArray();
        }

        private System.Drawing.Bitmap ByteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                return new System.Drawing.Bitmap(ms);
            }
        }

        private bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                    CacheDictionary.Clear();
                    CacheFolder = null;
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~CacheFileManager() {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
