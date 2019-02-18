using System;
using System.Collections.Generic;
using System.Linq;
using CacheClassLibrary.Classes;
using System.Drawing;

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

       private int MaxSizeToSerize { get; }

        public CacheManager(string dataBaseFilePath, int maxSizeToResize)
        {
            // создали датабейз сонтроллер и поместили в свойство, через него общаемся с бд
            DataBaseController = new DataBaseController.DataBaseController(dataBaseFilePath);

            MaxSizeToSerize = maxSizeToResize;

            // коннекшн уже открыт - получаем список текущего состава кеша
            Registries = DataBaseController.ExecuteQuery<CacheRegistry>(CacheRegistry._sqlSelector);
        }

        public System.Drawing.Size GetSize(string pathToFile)
        {
            // Путь известен, возвращаем
            if(Registries.Any(x => x.filePath == pathToFile))
            {
                var _registry = Registries.Single(x => x.filePath == pathToFile);
                return new System.Drawing.Size((int)_registry.width, (int)_registry.height);
            }
            // Путь неизвестен, открываем, считаем, пишем, возвращаем
            else
            {
                var _size = IO.Image.GetImageSize(pathToFile);
                _size = Utility.Image.VirtualResize(_size.Width, _size.Height, this.MaxSizeToSerize);

                CacheRegistry newRegistry = new CacheRegistry()
                {
                    filePath = pathToFile,
                    width = _size.Width,
                    height = _size.Height
                };

                newRegistry.Save(this);
                Registries.Add(newRegistry);

                return _size;
            }
        }

        public void GetImage()
        {
            throw new NotImplementedException();
        }
    }
}
