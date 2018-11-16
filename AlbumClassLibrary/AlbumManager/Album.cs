using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary.AlbumManager
{
    public class Album
    {
        public string Name { get; set; }
        public Guid Type { get; set; }

        public List<Folder> Folders { get; set; }
    }
}
