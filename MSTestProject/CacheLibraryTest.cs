using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSTestProject
{
    [TestClass]
    public class CacheLibraryTest
    {
        public string _cahceFile = @"C:\Users\user\_cacheDataBase.db";
        public CacheClassLibrary.CacheManager manager = null;
        public int _height = 350;

        public CacheLibraryTest()
        {
            manager = new CacheClassLibrary.CacheManager(_cahceFile, _height);
        }

        [TestMethod]
        public void CacheGetSizeTest1()
        {
            var _ret = manager.GetSize(@"C:\Users\user\Pictures\chris-bair-392187-unsplash.jpg");

            if (_ret.Width != 466)
                Assert.Fail();
        }

        [TestMethod]
        public void CacheGetSizeTest2()
        {
            var _ret = manager.GetSize(@"C:\Users\user\Pictures\al-x-413259-unsplash.jpg");

            if (_ret.Width != 525)
                Assert.Fail();
        }

        [TestMethod]
        public void CacheGetSizeTest3()
        {
            var _ret = manager.GetSize(@"C:\Users\user\Pictures\clem-onojeghuo-540801-unsplash.jpg");

            if (_ret.Width != 197)
                Assert.Fail();
        }
    }
}
