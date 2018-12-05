using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbumClassLibrary
{
    public interface IFolder
    {
        string Name { get; }
        string Path { get; }
        Guid Album { get; }
        byte[] PreviewImage { get; }
    }
}
