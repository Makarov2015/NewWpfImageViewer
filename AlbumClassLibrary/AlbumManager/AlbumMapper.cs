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

        public Guid AlbumGuid { get => Guid.Parse(Album); set { } }
        private string Album { get; }

        public List<IFolder> Folders { get; set; }

        public Guid AlbumTypeGuid => Guid.Parse(AlbumType);
        private string AlbumType { get; }
        public bool IsCurrent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler FolderAdded;
        public event EventHandler PriorityChanged;

        public void AddFolder()
        {
            throw new NotImplementedException();
        }

        public IAlbum FromMapper(IAlbum mapper)
        {
            throw new NotImplementedException();
        }
    }
}
