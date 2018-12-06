using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace AlbumClassLibrary.AlbumManager
{
    internal class DataBaseController : ControllerBase.DataBaseControllerBase, IDisposable
    {
        public DataBaseController(string dataBaseFilePath) : base(dataBaseFilePath) { }

        public List<IAlbum> LoadAlbums()
        {
            string albumSql = $"SELECT * FROM albums";
            string folderSql = $"SELECT * FROM folders";

            List<IAlbum> albums = null;
            List<IFolder> folders = null;

            m_dbConnection.Open();

            albums = m_dbConnection.Query<AlbumMapper>(albumSql).Cast<IAlbum>().ToList();
            folders = m_dbConnection.Query<Folder>(folderSql).Cast<IFolder>().ToList();

            m_dbConnection.Close();

            if (albums != null)
                foreach (var album in albums)
                {
                    album.Folders = folders.Where(x => x.Album == album.AlbumGuid).ToList();
                }

            if (albums.Count != 0)
                return albums;
            else
                return null;
        }

        public void AddAlbum(IAlbum album)
        {
            album.AlbumGuid = Guid.NewGuid();

            string sql = $@"INSERT INTO albums (album, albumtype, displayname) VALUES ('{album.AlbumGuid.ToString()}', '{album.AlbumTypeGuid}', '{album.DisplayName}'); ";
            this.ExecuteNonQuery(sql);
        }

        public void AddFolder(IFolder folder)
        {
            string sql = $@"INSERT INTO folders (albumGuid, name, path) VALUES ('{folder.Album.ToString()}', '{folder.Name}', '{folder.Path}'); ";
            this.ExecuteNonQuery(sql);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
