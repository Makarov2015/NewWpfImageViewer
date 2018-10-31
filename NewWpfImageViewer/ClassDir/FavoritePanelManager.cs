using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
                if (_currentFolder != null)
                    SelectedFolderChanged(_currentFolder);

                _currentFolder = value;
            }
        }

        /// <summary>
        /// Кнопка добавления новой избранной папки
        /// </summary>
        private Forms.Favorites.AddFolderButton AddFolderButton
        {
            get
            {
                var addBtn = new Forms.Favorites.AddFolderButton();
                addBtn.Mouse_Click += AddFavoriteFolder_Mouse_Click;

                return addBtn;
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

        private void FavoriteFolder_Mouse_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CurrentFolder = sender as FolderEntity;
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
                }
            }
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
            }
            // Если файла нет - создаем первую папку в коллекции = дефолтной папке
            else
            {
                FavoriteFolderEntities = new List<FolderEntity>
                {
                    new FolderEntity("Изображения", Properties.Settings.Default.DefaultDirectory, true)
                };
                SaveToFile();
            }

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
