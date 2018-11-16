using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary.ControllerBase
{
    public abstract class DataBaseControllerBase
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

            string createCache = "CREATE TABLE \"cache\" ( `id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `filePath` TEXT NOT NULL UNIQUE, `imageBinary` BLOB NOT NULL, `addedDate` INTEGER NOT NULL, `lastTaken` INTEGER ); ";

            createCache += "CREATE TABLE \"albums\" ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `typeGuid` TEXT NOT NULL, `shownName` TEXT UNIQUE, `creationDate` INTEGER NOT NULL ); ";

            createCache += "CREATE TABLE \"albumFolders\" ( `id` INTEGER PRIMARY KEY AUTOINCREMENT, `folderPath` INTEGER NOT NULL, `albumId` INTEGER, FOREIGN KEY(`albumId`) REFERENCES `albums`(`id`) ); ";

            ExecuteNonQuery(createCache);
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
