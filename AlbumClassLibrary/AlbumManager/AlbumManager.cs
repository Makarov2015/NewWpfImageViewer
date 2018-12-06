using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary.AlbumManager
{
    public class AlbumManager : IAlbumManager
    {
        private string DataBaseFilePath { get; set; }
        
        private List<IAlbum> LoadedAlbums { get; set; }

        public List<IAlbum> Albums { get; private set; }

        public List<IAlbum> AvailableAlbums
        {
            get
            {
                var dic = new List<IAlbum>
                {
                    new SystemAlbum()
                };

                return dic;
            }
        }

        public AlbumManager(string dbFilePath)
        {
            DataBaseFilePath = dbFilePath;

            using (DataBaseController controller = new DataBaseController(DataBaseFilePath))
            {
                LoadedAlbums = controller.LoadAlbums();
                Albums = new List<IAlbum>();

                if (LoadedAlbums == null)
                    LoadedAlbums = new List<IAlbum>();
                else
                {
                    // Тут альбомы - это AlbumMapper-ы
                    // Их нужно привести на основе Guid-а к нужному типу
                    foreach (var item in LoadedAlbums)
                    {
                        // В списке альбомов нашли подходящий по GUID-типа (В этом списке в идеале должны храниться все доступные типы альбомов)
                        var ty = AvailableAlbums.First(x => x.AlbumTypeGuid == item.AlbumTypeGuid).GetType();
                        // Создали инстанс этого альбома (ВАЖНО - у альбомов не должно быть конструктора, роль конструктора у FromMapper метода интерфейса IAlbum)
                        var inst = (IAlbum)Activator.CreateInstance(ty);
                        // И теперь взяли пустой инстанс и вызвали "констуктор" на основе маппера
                        Albums.Add(inst.FromMapper(item));
                    }

                    foreach (var item in Albums)
                    {
                        item.FolderAdded += Item_FolderAdded;
                        item.PriorityChanged += Item_PriorityChanged;
                    }
                }
            }
        }

        private void Item_PriorityChanged(object sender, EventArgs e)
        {
            if ((sender as IAlbum).IsCurrent == false)
                return;

            foreach (var item in Albums.Where(x => x != sender as IAlbum))
            {
                item.IsCurrent = false;
            }
        }

        public void AddAlbum(IAlbum album)
        {
            Albums.Add(album);
            album.FolderAdded += Item_FolderAdded;
            album.PriorityChanged += Item_PriorityChanged;

            using (DataBaseController controller = new DataBaseController(DataBaseFilePath))
            {
                controller.AddAlbum(album);
            }
        }

        private void Item_FolderAdded(object sender, EventArgs e)
        {
            using (DataBaseController controller = new DataBaseController(DataBaseFilePath))
            {
                controller.AddFolder(sender as IFolder);
            }
        }
    }
}
