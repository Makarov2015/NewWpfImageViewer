using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary
{
    /// <summary>
    /// Album interface
    /// </summary>
    public interface IAlbum
    {
        #region Album Class Fields

        /// <summary>
        /// Album Type Guid
        /// </summary>
        Guid AlbumTypeGuid { get; }

        /// <summary>
        /// Album Type Discription
        /// </summary>
        string AlbumName { get; }

        #endregion

        #region Album Personal Fields 

        /// <summary>
        /// Uniqe Album Guid
        /// </summary>
        Guid AlbumGuid { get; }

        /// <summary>
        /// Personal Album Name
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Album folders
        /// </summary>
        List<IFolder> Folders { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Add-Button realozation
        /// </summary>
        void AddFolder();

        #endregion
    }
}
