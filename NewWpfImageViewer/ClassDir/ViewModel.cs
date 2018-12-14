using NewWpfImageViewer.ClassDir;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewWpfImageViewer
{
    public class ViewModel
    {
        public ObservableCollection<AutoStackImage> Images { get; } = new ObservableCollection<AutoStackImage>();
    }
}
