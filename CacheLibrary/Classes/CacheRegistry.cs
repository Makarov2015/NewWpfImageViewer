using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheClassLibrary.Classes
{
    /// <summary>
    /// Запись кеша в урезанном варианте, без изображения. Нужна для расчетов и ускорения поиска
    /// </summary>
    public class CacheRegistry
    {
        public static string _sqlSelector = "SELECT id, filePath, width, height, CASE WHEN imageBinary IS NULL THEN 0 ELSE 1 END as haveBinary FROM cache";

        public int id { get; set; }
        public string filePath { get; set; }
        public bool haveBinary { get; set; }
        public double width { get; set; }
        public double height { get; set; }

        public bool Save(CacheManager manager)
        {
            if (manager.Registries.Any(x => x.filePath == this.filePath))
            {
                var _existed = manager.Registries.Single(x => x.filePath == this.filePath);

                if (!_existed.Equals(this))
                {
                    manager.DataBaseController.ExecuteNonQuery($"UPDATE cache SET width = {width}, height = {height} WHERE filePath = {filePath}; ");
                    return true;
                }
                else
                    return false;
            }
            else
            {
                manager.DataBaseController.ExecuteNonQuery($"INSERT INTO cache (filePath, width, height, addedDate) VALUES('{filePath}', {width}, {height}, '{DateTime.Now.ToOADate()}'); ");
                return true;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is CacheRegistry)
            {
                var _t = obj as CacheRegistry;

                if (this.filePath == _t.filePath)
                {
                    if (_t.width == this.width && _t.height == this.height)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return filePath.GetHashCode() + width.GetHashCode() +height.GetHashCode();
            }
        }
    }
}
