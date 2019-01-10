using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewWpfImageViewer.Properties;

namespace NewWpfImageViewer.ClassDir
{
    /// <summary>
    /// Class for managing user settings
    /// </summary>
    public static class SettingsManager
    {
        public static bool CheckSettings()
        {
            if (CheckCacheFile())
                return true;
            else
                return false;
        }

        private static bool CheckCacheFile()
        {
            // настройка пустая
            if(String.IsNullOrEmpty(Settings.Default.CacheFilePath))
            {
                //выясняем текущего юзера и формируем путь в его папку
                string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                //пишем эту папку в настройки
                Settings.Default.CacheFilePath = userFolder;
                //сейвим и по идее в следующий раз настройка пустой не будет
                Settings.Default.Save();
            }

            if (Settings.Default.MaxFilesToLoad == 0)
            {
                Settings.Default.MaxFilesToLoad = 500;
                Settings.Default.Save();
            }

            return true;
        }
        
        public static void ChangeCacheFileFolder(string newPath)
        {
            Settings.Default.CacheFilePath = newPath;
            Settings.Default.Save();
        }
    }
}
