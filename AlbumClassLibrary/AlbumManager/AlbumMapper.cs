using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary.AlbumManager
{
    sealed internal class AlbumMapper : IAlbum
    {
        public string DisplayName { get; set; }

        public string AlbumName => null;

        public Guid AlbumGuid => Guid.Parse(album);
        private string album { get; }

        public List<IFolder> Folders { get; set; }

        public Guid AlbumTypeGuid => Guid.Parse(albumType);
        private string albumType { get; }

        public void AddFolder()
        {
            throw new NotImplementedException();
        }
    }
}
