using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary.AlbumManager
{
    internal class DataBaseController : ControllerBase.DataBaseControllerBase, IDisposable
    {
        public DataBaseController(string dataBaseFilePath) : base(dataBaseFilePath) { }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
