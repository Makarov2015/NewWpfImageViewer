using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewWpfImageViewer.Interfaces
{
    /// <summary>
    /// Интерфейс описывает коллекцию <seealso cref="Interfaces.IImageStructure"/>
    /// </summary>
    public interface IImageCollection
    {
        IEnumerable<IImageStructure> Images { get;}

        void SetSize(Size size);

        void ChangeCollection(List<string> folderPath);

        void ScrolledEventHandler(object sender, EventArgs e);

        void ResizedEventHandler(object sender, EventArgs e);
    }
}
