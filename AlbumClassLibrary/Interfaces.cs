using System;
using System.Collections.Generic;
using System.Drawing;

namespace AlbumClassLibrary.Interfaces
{
    /// <summary>
    /// Менеджер альбомов - занимается получением списка альбомов из файла и записью его в файл
    /// </summary>
    public interface IAlbumManager
    {
        List<IAlbum> GetAlbums();
        void SetAlbums(List<IAlbum> albums);
    }

    /// <summary>
    /// Альбом - вкладка в просмотрщике
    /// У альбома есть название
    /// Альбом содержит список добавленых в него папок
    /// </summary>
    public interface IAlbum
    {
        /// <summary>
        /// Отображаемое название альбома
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Тип альбома
        /// </summary>
        Guid AlbumType { get; set; }

        /// <summary>
        /// Список папок альбома
        /// </summary>
        List<IFolder> Folders { get; set; }
    }

    /// <summary>
    /// Папка - это сущность, в которой находятся файлы
    /// В данном случае - это просто путь к папке
    /// На основе файлов внутри папки загружаются отображаемые изображения
    /// </summary>
    public interface IFolder
    {
        /// <summary>
        /// Фактический путь к папке
        /// </summary>
        string FolderPath { get; set; }

        /// <summary>
        /// Файлы внутри папки
        /// </summary>
        List<string> FilesInFolder { get; set; }
    }



    
}
