using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlbumClassLibrary.Interfaces.CacheManager;

namespace ClassLibraryUnitTestProject
{
    [TestClass]
    public class CachemanagerUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            CacheManager manager = new CacheManager(@"C:\Users\makarov\AppData\Roaming\NewWpfImageViewer\_cache");
        }
    }
}
