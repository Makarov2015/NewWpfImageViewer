using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheClassLibrary.Classes;
using CacheClassLibrary.DataBaseController;
using System.Windows;

namespace CacheClassLibrary
{
    /// <summary>
    /// Одиночка-реестр
    /// Менеджер картинок будет держать этот экземпляр, пока нужно общаться с бд
    /// Экземпляр все время существования единолично держит коннекшн к БД
    /// Менеджер кеша должен выполнять 2 операции - возвращать размер и возвращать картинку
    /// Работает как бы асинхронно
    /// </summary>
    public partial class CacheManager
    {
        internal DataBaseController.DataBaseController DataBaseController = null;

        public List<CacheRegistry> Registries = null;

        private System.Collections.Concurrent.ConcurrentQueue<Task> _sizeQueue = new System.Collections.Concurrent.ConcurrentQueue<Task>();

        public CacheManager(string dataBaseFilePath)
        {
            // создали датабейз сонтроллер и поместили в свойство, через него общаемся с бд
            DataBaseController = new DataBaseController.DataBaseController(dataBaseFilePath);
            // коннекшн уже открыт - получаем список текущего состава кеша
            Registries = DataBaseController.ExecuteQuery<CacheRegistry>(CacheRegistry._sqlSelector);
        }

        public (double, double) GetSize(string pathToFile)
        {
            // Путь известен, возвращаем
            if(Registries.Any(x => x.filePath == pathToFile))
            {
                var _registry = Registries.Single(x => x.filePath == pathToFile);
                return (_registry.width, _registry.height);
            }
            // Путь неизвестен, кидаем таск на просчет в очередь
            else
            {
                _sizeQueue.Enqueue(NewSizeRequest());
            }
        }

        private Task<(double, double)> NewSizeRequest()
        {
            return new Task<(double, double)>(() =>
            {
                return (10, 10);
            });
        }

        public void GetImage()
        {
            throw new NotImplementedException();
        }
    }
}
