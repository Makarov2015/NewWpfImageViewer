using System;

namespace AlbumInterface
{
    public interface IConnectedCollection
    {
        /// <summary>
        /// GUID Альбома для идентификации
        /// </summary>
        Guid CollectionGuid { get; }

        /// <summary>
        /// Отображаемое на форме имя альбома
        /// </summary>
        string ShownAlbumName { get; }

        /// <summary>
        /// Кнопка создания папки и наполнения ее изображениями
        /// </summary>
        object AddButton { get; }
    }
}
