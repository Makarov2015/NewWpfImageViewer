using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace NewWpfImageViewer.ClassDir
{
    /// <summary>
    /// Создали инстанс
    /// Из инстанса получаем набор кнопок для панели
    /// В инстансе обрабатываем нажатия кнопок и возвращаем текущую папку
    /// В интерфейсе уже работаем с текущей папкой
    /// Инстанс в зависимости от нажатий сам занимается менеджментом кнопок (Сохранением, загрузкой)
    /// </summary>
    class FavoritePanelManager
    {
        /// <summary>
        /// Путь к файлу списка Избранных папок
        /// </summary>
        string FavoriteFileDirectory { get; }

        /// <summary>
        /// Загруженный лист Избранных папок
        /// </summary>
        private List<FolderEntity> FavoriteFolderEntities { get; set; }

        private FolderEntity _currentFolder;
        /// <summary>
        /// Текущая выбранная папка
        /// </summary>
        public FolderEntity CurrentFolder
        {
            get
            {
                return _currentFolder;
            }
            set
            {
                bool n = _currentFolder != null;

                _currentFolder = value;

                if (n)
                    SelectedFolderChanged(_currentFolder);
            }
        }

        /// <summary>
        /// Кнопка добавления новой избранной папки
        /// </summary>
        private Forms.Favorites.AddFolderButton AddFolderButton
        {
            get
            {
                //var addBtn = new Forms.Favorites.AddFolderButton();
                //addBtn.Mouse_Click += AddFavoriteFolder_Mouse_Click;

                return null;
            }
        }

        /// <summary>
        /// Возвращает коллекцию элементов для добавления на панель
        /// </summary>
        public List<UserControl> ButtonsCollection
        {
            get
            {
                List<UserControl> controls = new List<UserControl>();

                foreach (var item in FavoriteFolderEntities)
                {
                    var ctrl = item.GetControl();
                    ctrl.Mouse_Click += FavoriteFolder_Mouse_Click;
                    ctrl.Delete_Click += Ctrl_Delete_Click;

                    controls.Add(ctrl);
                }

                controls.Add(AddFolderButton);

                return controls;
            }
        }

        public FavoritePanelManager(string programDirectory, string fileName)
        {
            // Получили директорию папки
            FavoriteFileDirectory = programDirectory + fileName;
            // Попробовали загрузить файл
            LoadFromFile(programDirectory);
        }

        #region Events

        public delegate void FavoriteFolderSelectionHandler(FolderEntity folder);
        public event FavoriteFolderSelectionHandler SelectedFolderChanged;

        public delegate void FavoriteFolderAddedHandler();
        public event FavoriteFolderAddedHandler NewFavoriteFolderAdded;

        private void FavoriteFolder_Mouse_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var obj = sender as FolderEntity;
            obj.IsSelected = true;
            
            if (sender == CurrentFolder)
                return;

            CurrentFolder.IsSelected = false;
            CurrentFolder = obj;
        }

        private void AddFavoriteFolder_Mouse_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    FavoriteFolderEntities.Add(new FolderEntity(new System.IO.DirectoryInfo(dialog.SelectedPath).Name, dialog.SelectedPath));
                    SaveToFile();
                    NewFavoriteFolderAdded();
                }
            }
        }

        private void Ctrl_Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FavoriteFolderEntities.Remove(sender as FolderEntity);
            SaveToFile();
            NewFavoriteFolderAdded();
        }

        #endregion

        #region JSON Read\Write File

        private void LoadFromFile(string prgmDir)
        {
            if (!Directory.Exists(prgmDir))
                Directory.CreateDirectory(prgmDir);

            // Если файл есть - загружаем в FolderEntities
            if (File.Exists(FavoriteFileDirectory))
            {
                FavoriteFolderEntities = JsonConvert.DeserializeObject<List<FolderEntity>>(Encoding.UTF8.GetString(File.ReadAllBytes(FavoriteFileDirectory)));

                foreach (var item in FavoriteFolderEntities)
                {
                    item.LoadFoderFiles();
                }
            }
            // Если файла нет - создаем первую папку в коллекции = дефолтной папке
            else
            {
                FavoriteFolderEntities = new List<FolderEntity>
                {
                    //new FolderEntity("Изображения", Properties.Settings.Default.DefaultDirectory)
                };
            }

            SaveToFile();

            CurrentFolder = FavoriteFolderEntities.First();
        }

        private void SaveToFile()
        {
            string json = JsonConvert.SerializeObject(FavoriteFolderEntities);
            ByteArrayToFile(FavoriteFileDirectory, System.Text.Encoding.UTF8.GetBytes(json));
        }

        public static bool ByteArrayToFile(string fileName, byte[] byteArray)
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

        #endregion
    }
}
