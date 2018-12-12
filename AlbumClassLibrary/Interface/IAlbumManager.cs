using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary
{
    /// <summary>
    /// Менеджер альбомов.
    /// Отвечает за загрузку, сохранение альбомов, маппинг их в нужные классы
    /// </summary>
    public interface IAlbumManager
    {
        /// <summary>
        /// Поддерживаемые классы альбомов. Как те, что объявлены в библиотеке, так и те, что лежат в DLL
        /// </summary>
        List<IAlbum> AvailableAlbums { get; }

        /// <summary>
        /// Загруженные альбомы
        /// </summary>
        List<IAlbum> Albums { get; }
        
        /// <summary>
        /// Добавть альбом
        /// </summary>
        /// <param name="albumToAdd">Экземпляр класса</param>
        void AddAlbum(IAlbum albumToAdd);
    }
}
