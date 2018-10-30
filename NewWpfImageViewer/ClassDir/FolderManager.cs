using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewWpfImageViewer.ClassDir
{
    /// <summary>
    /// Класс для сохранения, изменения, удаления и чтения состава "Любимых" папок
    /// </summary>
    public static class FolderManager
    {
        /// <summary>
        /// Получаем список папок
        /// </summary>
        public static List<FolderEntity> Load()
        {
            string path = Directory.Exists(Properties.Settings.Default.ProgramDataFolder) + "_fav\\";

            if (Directory.Exists(path))
            {
                if (File.Exists(path + "_favorites"))
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<FolderEntity>>(Encoding.UTF8.GetString(File.ReadAllBytes(path + "_favorites")));
                else
                    throw new FileNotFoundException();
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }
        
        /// <summary>
        /// Добавляем в список папку
        /// </summary>
        public static void Add()
        {
            string path = Directory.Exists(Properties.Settings.Default.ProgramDataFolder) + "_fav\\";

            if (File.Exists(path + "_favorites"))
            {
                // Взять файл, добавить в конец новую запись и перезаписать файл
            }
            else
            {
                // Создать новый и сохранить
            }
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(CacheDictionary);
            ByteArrayToFile(CacheTableFile, System.Text.Encoding.UTF8.GetBytes(json));

            Save();
        }

        /// <summary>
        /// Удаляем из списка папку
        /// </summary>
        public static void Remove()
        {
            Save();
        }
        
        /// <summary>
        /// Сохраняем измененный файл
        /// </summary>
        private static void Save()
        { }

        public static List<FolderEntity> LoadFavFolders()
        {
            

        }

        //public static bool ByteArrayToFile(string fileName, byte[] byteArray)
        //{
        //    try
        //    {
        //        using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
        //        {
        //            fs.Write(byteArray, 0, byteArray.Length);
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Exception caught in process: {0}", ex);
        //        return false;
        //    }
        //}
    }
}
