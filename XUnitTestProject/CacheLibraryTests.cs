using System;
using Xunit;
using CacheClassLibrary;
using CacheClassLibrary.Classes;

namespace XUnitTestProject
{
    public class CacheLibraryTests
    {
        string _testPath = @"C:\Users\makarov\_cacheTest";

        [Fact]
        public void DataBaseCreationTest()
        {
            System.IO.File.Delete(_testPath);

            using (CacheManager manager = new CacheManager(_testPath))
            {
                Assert.True(System.IO.File.Exists(_testPath));
            }
        }

        [Fact]
        public void DataBaseLockTest()
        {
            bool _result = false;

            using (CacheManager manager = new CacheManager(_testPath))
            {
                try
                {
                    System.IO.File.Delete(_testPath);
                }
                catch (System.IO.IOException)
                {
                    _result = true;
                }
            }

            Assert.True(_result);
        }

        [Fact]
        public void DataBaseWriteTest()
        {
            CacheRegistry registry = new CacheRegistry()
            {
                filePath = @"G:\pics\Dandonfuga-artist-Tyrande-Whisperwind-Warcraft-2408380.jpg",
                haveBinary = false,
                width = 500,
                height = 500
            };

            using (CacheManager manager = new CacheManager(_testPath))
            {
                Assert.True(registry.Save(manager));
            }
        }

        [Fact]
        public void DataBaseReadTest()
        {
            using (CacheManager manager = new CacheManager(_testPath))
            {
                Assert.True(manager.Registries.Count > 0);
            }
        }
    }
}
