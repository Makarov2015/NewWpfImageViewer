using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary.AlbumManager
{
    public class AlbumManager : IAlbumManager
    {
        private string dataBaseFilePath { get; set; }

        public List<IAlbum> Albums { get; private set; }

        public List<IAlbum> AvailableAlbums
        {
            get
            {
                var dic = new List<IAlbum>();

                dic.Add(new SystemAlbum(null));

                return dic;
            }
        }

        public AlbumManager(string dbFilePath)
        {
            dataBaseFilePath = dbFilePath;

            using (DataBaseController controller = new DataBaseController(dataBaseFilePath))
            {
                Albums = controller.LoadAlbums();

                if (Albums == null)
                    Albums = new List<IAlbum>();
            }
        }

        public void AddAlbum(IAlbum album)
        {
            Albums.Add(album);

            using (DataBaseController controller = new DataBaseController(dataBaseFilePath))
            {
                controller.AddAlbum(album);
            }
        }
    }
}
