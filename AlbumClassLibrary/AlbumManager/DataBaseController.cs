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

        public List<Folder> FoldersSearch()
        {
            string sql = $"SELECT * FROM folders";

            List<Folder> folders = null;

            try
            {
                m_dbConnection.Open();

                System.Data.DataTable table = new System.Data.DataTable();

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, m_dbConnection);
                adapter.Fill(table);

                folders = m_dbConnection.Query<Folder>(sql).ToList();

                //bytes = (from row in table.Rows.Cast<System.Data.DataRow>()
                //         select (byte[])row[0]).Single();

                if (folders.Count != 0)
                    return folders;
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
            finally
            {
                m_dbConnection.Close();
            }
        }

        public void FolderAdd()
        {
            //SQLiteParameter param = new SQLiteParameter("@image", DbType.Binary);
            //param.Value = img;

            //string sql = $"INSERT INTO cache (filePath, imageBinary, addedDate) values ('{path}', @image, '{DateTime.Now.ToFileTimeUtc()}')";
            //ExecuteNonQuery(sql, param);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
