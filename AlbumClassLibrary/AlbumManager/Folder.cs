using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary
{
    internal class Folder : IFolder
    {
        public string Name { get; }
        public string Path { get; }
        public byte[] PreviewImage { get; }
        
        public Guid Album => Guid.Parse(albumGuid);
        private string albumGuid { get; }

        public Folder(string _name, string _path, string _albumGuid)
        {
            Name = _name;
            Path = _path;
            albumGuid = _albumGuid;
        }
    }
}
