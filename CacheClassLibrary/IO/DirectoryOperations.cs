using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CacheClassLibrary.IO
{
    static internal class DirectoryOperations
    {
        static bool IsDirectory(string path)
        {
            return Directory.Exists(path);
        }
    }
}
