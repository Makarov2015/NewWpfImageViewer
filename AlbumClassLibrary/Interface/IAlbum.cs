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
        Guid AlbumGuid { get; set; }

        /// <summary>
        /// Personal Album Name
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Album folders
        /// </summary>
        List<IFolder> Folders { get; set; }

        /// <summary>
        /// Is album currently selected
        /// </summary>
        bool IsCurrent { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Add-Button realization
        /// </summary>
        void AddFolder();

        /// <summary>
        /// Constructod analog for transform from mapper to direct type
        /// </summary>
        /// <param name="mapper">mapper object</param>
        /// <returns></returns>
        IAlbum FromMapper(IAlbum mapper);

        #endregion

        #region Events

        /// <summary>
        /// Event of added folder / selection changed
        /// </summary>
        event EventHandler FolderAdded;
        event EventHandler PriorityChanged;

        #endregion
    }
}
