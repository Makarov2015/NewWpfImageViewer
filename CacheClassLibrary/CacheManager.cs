using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheClassLibrary.Classes;

namespace CacheClassLibrary
{
    /// <summary>
    /// Одиночка-реестр
    /// Менеджер картинок будет держать этот экземпляр, пока нужно общаться с бд
    /// Экземпляр все время существования единолично держит коннекшн к БД
    /// Менеджеру картинок говорят желаемую папку. Менеджер картинок говрит менеджеру кеша
    /// </summary>
    public partial class CacheManager
    {
        internal DataBaseController DataBaseController = null;

        public List<CacheRegistry> Registries = null;

        public CacheManager(string dataBaseFilePath)
        {
            // создали датабейз сонтроллер и поместили в свойство, через него общаемся с бд
            DataBaseController = new DataBaseController(dataBaseFilePath);
            // коннекшн уже открыт - получаем список текущего состава кеша
            Registries = DataBaseController.ExecuteQuery<CacheRegistry>(CacheRegistry._sqlSelector);
        }
    }
}
