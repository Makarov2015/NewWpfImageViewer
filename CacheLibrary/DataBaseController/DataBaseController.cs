using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheClassLibrary.DataBaseController
{
    public class DataBaseController
    {
        protected IDbConnection m_dbConnection;

        public DataBaseController(string dataBaseFilePath)
        {
            m_dbConnection = new SQLiteConnection($"Data Source={dataBaseFilePath};Version=3;");

            if (!System.IO.File.Exists(dataBaseFilePath))
                CreateEmptyDataBaseFile(dataBaseFilePath);

            OpenConnection();
        }

        public void OpenConnection()
        {
            if(this.m_dbConnection.State != ConnectionState.Open)
                this.m_dbConnection.Open();
        }

        public void CloseConnection()
        {
            if (this.m_dbConnection.State == System.Data.ConnectionState.Open)
                this.m_dbConnection.Close();
        }

        protected void CreateEmptyDataBaseFile(string name)
        {
            SQLiteConnection.CreateFile(name);

            string createCache = @"CREATE TABLE `cache` (
							`id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
							`filePath`	TEXT NOT NULL UNIQUE,
							`imageBinary`	BLOB,
							`addedDate`	INTEGER,
							`lastTaken`	INTEGER,
	                        `width`	REAL,
	                        `height` REAL
							);";

            ExecuteNonQuery(createCache);
        }

        public List<T> ExecuteQuery<T>(string selectQuery)
        {
            return m_dbConnection.Query<T>(selectQuery).ToList();
        }

        internal void ExecuteNonQuery(string sql, SQLiteParameter param = null)
        {
            if (this.m_dbConnection.State != ConnectionState.Open)
                this.m_dbConnection.Open();

            SQLiteCommand command = new SQLiteCommand(sql, (SQLiteConnection)m_dbConnection);

            if (param != null)
                command.Parameters.Add(param);

            command.ExecuteNonQuery();
        }
    }
}
