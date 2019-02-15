using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheClassLibrary.Classes
{
    public class CacheRegistry
    {
        public static string _sqlSelector = "SELECT CASE WHEN imageBinary IS NULL THEN 0 ELSE 1 END as haveBinary FROM cache";

        public int id { get; set; }
        public string filePath { get; set; }
        public bool haveBinary { get; set; }
        public double width { get; set; }
        public double height { get; set; }

        public bool Save(CacheManager manager)
        {
            try
            {
                manager.DataBaseController.ExecuteNonQuery($"INSERT INTO cache (filePath, width, height) VALUES({filePath}, {width}, {height}); ");
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
