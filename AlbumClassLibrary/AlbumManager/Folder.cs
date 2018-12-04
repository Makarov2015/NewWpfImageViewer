using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary.AlbumManager
{
    public class Folder
    {
        public int id { get; set; }
        public string shownName { get; set; }

        private Guid _albumGuid;
        public string albumGuid
        {
            get
            {
                return _albumGuid.ToString();
            }
            set
            {
                Guid.TryParse(value, out _albumGuid);
            }
        }

        public string folderPath { get; set; }
        public int creationDate { get; set; }
    }
}
