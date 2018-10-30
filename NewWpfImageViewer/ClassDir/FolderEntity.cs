using NewWpfImageViewer.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace NewWpfImageViewer.ClassDir
{
    /// <summary>
    /// Класс представляющий папку с изображениями
    /// </summary>
    public class FolderEntity
    {
        /// <summary>
        /// Маркер, является ли данная папка - папкой по умолчанию
        /// </summary>
        public bool IsDfault { get; }

        /// <summary>
        /// Отображаемое имя на панеле с папками
        /// </summary>
        public string ShownName { get; set; }

        /// <summary>
        /// Физический путь к папке
        /// </summary>
        public string FolderPath { get; set; }

        /// <summary>
        /// Превью на панеле с папками
        /// </summary>
        public Bitmap PreviewImage { get; set; }

        /// <summary>
        /// Путь к изображениями в папке
        /// </summary>
        public List<string> ImagesPaths { get; set; }

        public FolderEntity(string name, string path, bool @default = false)
        {
            IsDfault = @default;
            ShownName = name;
            FolderPath = path;

            LoadFoderFiles();
            GetRandomImageForPreview();
        }

        /// <summary>
        /// Пробуем найти в кеше хоть какую-то картинку для превью
        /// </summary>
        private void GetRandomImageForPreview()
        {
            Random rand = new Random();

            using (ClassDir.CacheFileManager manager = new ClassDir.CacheFileManager())
            {
                while (PreviewImage == null)
                {
                    try
                    {
                        PreviewImage = Bitmap.FromFile(manager.Search(ImagesPaths.ElementAt(rand.Next(0, ImagesPaths.Count - 1)))) as Bitmap;
                    }
                    catch (ArgumentNullException)
                    {
                        Bitmap flag = new Bitmap(200, 100);
                        Graphics flagGraphics = Graphics.FromImage(flag);

                        flagGraphics.FillRectangle(Brushes.GhostWhite, 0, 0, 200, 100);

                        PreviewImage = flag;
                    }
                }
            }
        }

        private void LoadFoderFiles()
        {
            ImagesPaths = new List<string>();

            foreach (var item in System.IO.Directory.GetFiles(FolderPath).Where(x => x.EndsWith(".gif") || x.EndsWith(".jpeg") || x.EndsWith(".jpg")))
            {
                ImagesPaths.Add(item);
            }
        }

        /// <summary>
        /// Готовый контрол для отображения папки с превью
        /// </summary>
        /// <returns></returns>
        public FolderButton GetControl()
        {
            return new FolderButton(this);
        }
    }
}
