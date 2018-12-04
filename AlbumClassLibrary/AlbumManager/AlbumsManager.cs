using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary.AlbumManager
{
    public class AlbumsManager
    {
        public List<Folder> Folders { get; set; }
        private string _dataBaseFilePath { get; }

        public AlbumsManager(string dataBaseFilePath)
        {
            _dataBaseFilePath = dataBaseFilePath;

            using (DataBaseController controller = new DataBaseController(_dataBaseFilePath))
            {
                Folders = controller.FoldersSearch();
            }
        }

        public void AddFolder()
        {
            using (DataBaseController controller = new DataBaseController(_dataBaseFilePath))
            {
                controller.FolderAdd();
            }
        }
    }
}
