using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace NewWpfImageViewer.ClassDir
{
    class CacheManager : IDisposable
    {
        bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        FileCache cache = new FileCache();
        Dictionary<string, System.Drawing.Image> fileContents;

        List<string> filePaths = new List<string>();
        CacheItemPolicy policy = new CacheItemPolicy();

        public bool CacheIsEmpty => fileContents == null;

        public CacheManager(string cacheName, string directory)
        {
            fileContents = cache[cacheName] as Dictionary<string, System.Drawing.Image>;
            filePaths.AddRange(System.IO.Directory.GetFiles(directory).Where(x => x.EndsWith(".gif") || x.EndsWith(".jpeg") || x.EndsWith(".jpg")).ToList());

            policy.AbsoluteExpiration = DateTimeOffset.Now.AddDays(10.0);
        }

        public List<ClassDir.AutoStackImage> DealWithCache()
        {
            List<ClassDir.AutoStackImage> ret = new List<AutoStackImage>();
            
            if (CacheIsEmpty)
            {
                fileContents = new Dictionary<string, System.Drawing.Image>();

                foreach (var item in filePaths)
                {
                    var cls = new ClassDir.AutoStackImage(item);

                    ret.Add(cls);
                    fileContents.Add(item, cls.MaxSizedImage);
                }
            }
            else
            {
                var newFiles = filePaths.Except(fileContents.Select(x => x.Key)).ToList();
                var cached = filePaths.Intersect(fileContents.Select(x => x.Key)).ToList();

                foreach (var item in cached)
                {
                    ret.Add(new ClassDir.AutoStackImage((System.Drawing.Image)fileContents.Where(x => x.Key == item).Select(x => x.Value).First()));
                }

                foreach (var item in newFiles)
                {
                    var cls = new ClassDir.AutoStackImage(item);
                    ret.Add(cls);

                    fileContents.Add(item, cls.MaxSizedImage);
                }
            }

            policy.ChangeMonitors.Add(new HostFileChangeMonitor(filePaths));
            cache.Set("images", fileContents, policy);

            return ret;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                cache = null;
                fileContents.Clear();
                fileContents = null;
            }

            disposed = true;
        }
    }
}
