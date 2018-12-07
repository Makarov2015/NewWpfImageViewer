using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;

namespace AlbumClassLibrary.ControllerBase
{
    internal abstract class DataBaseControllerBase
    {
        protected SQLiteConnection m_dbConnection;

        public DataBaseControllerBase(string dataBaseFilePath)
        {
            m_dbConnection = new SQLiteConnection($"Data Source={dataBaseFilePath};Version=3;");

            if (!System.IO.File.Exists(dataBaseFilePath))
                CreateEmptyDataBaseFile(dataBaseFilePath);
        }

        protected void CreateEmptyDataBaseFile(string name)
        {
            SQLiteConnection.CreateFile(name);

            string createCache = @"CREATE TABLE `cache` (
							`id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
							`filePath`	TEXT NOT NULL UNIQUE,
							`imageBinary`	BLOB NOT NULL,
							`addedDate`	INTEGER NOT NULL,
							`lastTaken`	INTEGER
							);";

            createCache += @"CREATE TABLE `albums` (
							`album`	TEXT NOT NULL PRIMARY KEY,
							`albumtype`	TEXT NOT NULL,
							`displayname`	TEXT NOT NULL
							); ";

            createCache += @"CREATE TABLE `folders` (
							`id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
							`albumGuid`	TEXT NOT NULL,
							`name`	TEXT NOT NULL,
							`path`	TEXT NOT NULL
							); ";

            var gd = Guid.NewGuid().ToString();

            createCache += $@"INSERT INTO albums (album, albumtype, displayname) VALUES ('{gd}','804C01FD-772B-48E0-916C-1C24FC99968E', 'Библиотека'); ";

            var imgPath = System.IO.Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Pictures\\");
            
            ExecuteNonQuery(createCache);

            if (System.IO.Directory.Exists(imgPath))
            {
                AddFolder(new AlbumManager.Folder(0, gd, "Изображения", imgPath));
            }
        }

        public void AddFolder(IFolder folder)
        {
            string sql = $@"INSERT INTO folders (albumGuid, name, path) VALUES ('{folder.Album.ToString()}', '{folder.Name}', '{folder.Path}'); ";
            this.ExecuteNonQuery(sql);
        }

        protected void ExecuteNonQuery(string sql, SQLiteParameter param = null)
        {
            try
            {
                m_dbConnection.Open();
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);

                if (param != null)
                    command.Parameters.Add(param);

                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                m_dbConnection.Close();
            }
        }
    }
}
