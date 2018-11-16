using NewWpfImageViewer.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewWpfImageViewer.ClassDir
{
    /// <summary>
    /// Класс папки на панеле Избранных
    /// </summary>
    public class FolderEntity
    {
        /// <summary>
        /// Отображаемое имя на панеле с папками
        /// </summary>
        public string ShownName { get; set; }

        /// <summary>
        /// Физический путь к папке
        /// </summary>
        public string FolderPath { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_button != null)
                {
                    if (value == true)
                    {
                        _button.NamePlate.Background = System.Windows.Media.Brushes.Coral;
                        _isSelected = true;
                    }
                    else
                    {
                        _button.NamePlate.Background = System.Windows.Media.Brushes.CadetBlue;
                        _isSelected = false;
                    }
                }
            }
        }

        /// <summary>
        /// Путь к изображениями в папке
        /// </summary>
        public List<string> ImagesPaths { get; set; }

        public FolderEntity(string name, string path)
        {
            ShownName = name;
            FolderPath = path;

            if (name != null && path != null)
                LoadFoderFiles();
        }

        public void LoadFoderFiles()
        {
            ImagesPaths = new List<string>();
            ImagesPaths = System.IO.Directory.GetFiles(FolderPath).Where(x => x.EndsWith(".gif") || x.EndsWith(".jpeg") || x.EndsWith(".jpg")).ToList();
        }

        private FolderButton _button;

        /// <summary>
        /// Готовый контрол для отображения папки с превью
        /// </summary>
        /// <returns></returns>
        public FolderButton GetControl()
        {
            using (AlbumClassLibrary.CacheManager.CacheManager manager = new AlbumClassLibrary.CacheManager.CacheManager(@"C:\Users\makarov\_cacheDataBase.db"))
            {
                Random rand = new Random();

                _button = new FolderButton(this, null, IsSelected);

                return _button;
            }
        }
    }
}
