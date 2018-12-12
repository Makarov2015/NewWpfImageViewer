using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary
{
    /// <summary>
    /// Класс папки, являющейся частью альбома
    /// </summary>
    public interface IFolder
    {
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Путь к папке
        /// </summary>
        string Path { get; }

        /// <summary>
        /// GUID родительского альбома
        /// </summary>
        Guid Album { get; }

        /// <summary>
        /// Превью папки
        /// </summary>
        Bitmap PreviewImage { get; set; }
    }
}
