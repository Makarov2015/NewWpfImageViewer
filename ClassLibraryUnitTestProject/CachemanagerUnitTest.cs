using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlbumClassLibrary.CacheManager;

namespace ClassLibraryUnitTestProject
{
    [TestClass]
    public class CachemanagerUnitTest
    {
        private string _pathToCacheFolder = @"C:\Users\makarov\AppData\Roaming\NewWpfImageViewer\_cache\";

        [TestMethod]
        public void OpenGoodFileTest_1()
        {
            using (CacheManager manager = new CacheManager(_pathToCacheFolder))
            {
                var a = manager.GetImage(@"C:\Users\makarov\AppData\Roaming\NewWpfImageViewer\_imagesForTest\david-kovalenko-414249-unsplash.jpg");

                if (a == null || a.Width == 0 || a.Height == 0)
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void OpenGoodFileTest_2()
        {
            using (CacheManager manager = new CacheManager(_pathToCacheFolder))
            {
                var a = manager.GetImage(@"C:\Users\makarov\AppData\Roaming\NewWpfImageViewer\_imagesForTest\david-pisnoy-660309-unsplash.jpg");

                if (a == null || a.Width == 0 || a.Height == 0)
                {
                    Assert.Fail();
                }
            }
        }

        //[TestMethod]
        //public void OpenBadFileTest_1()
        //{
        //    using (CacheManager manager = new CacheManager(_pathToCacheFolder))
        //    {
        //        //if (!manager.GetImage(""))
        //        //    Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void OpenNotExistFileTest_1()
        //{
        //    using (CacheManager manager = new CacheManager(_pathToCacheFolder))
        //    {
        //        //if (!manager.GetImage(""))
        //        //    Assert.Fail();
        //    }
        //}

        //[TestMethod]
        //public void OpenNotExistFileTest_2()
        //{
        //    using (CacheManager manager = new CacheManager(_pathToCacheFolder))
        //    {
        //        //if (!manager.GetImage(""))
        //        //    Assert.Fail();
        //    }
        //}
    }
}
