using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary.AlbumManager
{
    internal class Folder : IFolder
    {
        public string Name { get; }
        public string Path { get; }
        public byte[] PreviewImage { get; }
        
        public Guid Album => Guid.Parse(_albumGuid);
        private string _albumGuid { get; }

        public Folder(Int64 id, string albumGuid, string name, string path, byte[] previewImage)
        {
            Name = name;
            Path = path;
            _albumGuid = albumGuid;
        }
    }
}
