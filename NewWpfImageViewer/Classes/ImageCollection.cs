using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewWpfImageViewer.Interfaces;

namespace NewWpfImageViewer.Classes
{
    public class ImageCollection : IImageCollection
    {
        #region Properties

        private IEnumerable<IImageStructure> _images;
        public IEnumerable<IImageStructure> Images
        {
            get
            {
                if (_images == null)
                {
                    _images = new List<IImageStructure>();
                }

                return _images;
            }
            private set
            {
                if (value.Count() == 0 && (_images != null && _images.Count() > 0))
                    foreach (var item in _images)
                    {
                        item.Dispose();
                    }

                _images = value;
            }
        }

        #endregion

        #region Settings

        private Size CurrentSize;

        private string cacheFilePath;

        private event EventHandler Scrolled;
        //private event EventHandler Resized;

        #endregion

        #region Constructors

        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        /// <param name="_cacheFilePath">Путь к файлу кеша картинок</param>
        /// <param name="_size">Размер картинок при инициализации</param>
        public ImageCollection(string _cacheFilePath, Size _size)
        {
            cacheFilePath = _cacheFilePath;
            CurrentSize = _size;
        }

        #endregion

        #region Methods

        public void ChangeCollection(List<string> folderPath)
        {
            Scrolled = null;
            // Некий класс-фильтр вернет список файлов. Класс-фильтр сам решает, какие типы грузить, загружать подпапки или нет

            //List<string> files = new List<string>();

            Images = new List<IImageStructure>();

            var _tmp = new List<IImageStructure>();

            // Кормим менеджер путем к файлу, получаем кешированную копию, генерим АвтоСтаки и складываем в список
            using (AlbumClassLibrary.CacheManager.CacheManager manager = new AlbumClassLibrary.CacheManager.CacheManager(this.cacheFilePath))
            {
                foreach (var item in folderPath)
                {
                    var n = new ClassDir.ImageStructure(manager.GetImage(item), item);
                    n.Size = this.CurrentSize;
                    this.Scrolled += n.VisabilityChanged;

                    _tmp.Add(n);
                }
            }

            this.Images = _tmp;
        }

        // Нужно подумать, странное решение
        public void SetSize(Size size)
        {
            if (this.CurrentSize != size)
            {
                this.CurrentSize = size;

                Parallel.ForEach(this.Images, (x) =>
                {
                    x.Size = this.CurrentSize;
                });
            }
            else
                return;
        }

        #endregion

        #region Events

        // При скроле меняем видимость
        public void ScrolledEventHandler(object sender, EventArgs e)
        {
            Scrolled(sender, e);
        }

        // При ресайзе меняем размеры
        public void ResizedEventHandler(object sender, EventArgs e)
        {
            // Размер уже изменился - расчитываем размер картинок под этот новый размер
            CalculateNewWidth((sender as System.Windows.Controls.Control).ActualWidth);

            // После высчитываем новые координаты расположения картинок
            CalculateNewCoordinates(sender as System.Windows.Controls.Control);

            void CalculateNewWidth(double newParentWidth)
            {
                ClassDir.DynamicRowFormatter.FormatPlate(this.Images.ToList(), newParentWidth);
            }

            void CalculateNewCoordinates(System.Windows.Controls.Control control)
            {
                foreach (var item in (control as System.Windows.Controls.ItemsControl).Items)
                {
                    var container = (control as System.Windows.Controls.ItemsControl).ItemContainerGenerator.ContainerFromItem(item) as System.Windows.FrameworkElement;

                    if (container.DataContext is IImageStructure)
                    {
                        var str = (container.DataContext as IImageStructure);
                        var ctrl = control as System.Windows.Controls.ItemsControl;

                        var pos = container.TransformToAncestor(ctrl).Transform(new System.Windows.Point(0, 0));

                        str.Position = pos;
                    }
                }
            }
        }

        #endregion
    }
}
