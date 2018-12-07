using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary.AlbumManager
{
    internal class Folder : IFolder
    {
        public string Name { get; }
        public string Path { get; }
        public Bitmap PreviewImage { get; set; }
        
        public Guid Album => Guid.Parse(_albumGuid);
        private string _albumGuid { get; }

        public Folder(Int64 id, string albumGuid, string name, string path)
        {
            Name = name;
            Path = path;
            _albumGuid = albumGuid;
        }
    }
}
