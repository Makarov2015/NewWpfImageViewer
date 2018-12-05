using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary
{
    public interface IAlbumManager
    {
        List<IAlbum> Albums { get; }
        List<IAlbum> AvailableAlbums { get; }

        void AddAlbum(IAlbum albumToAdd);
    }
}
