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
        public void DataBaseReadTest()
        {
            using (CacheManager manager = new CacheManager(_testPath))
            {
                Assert.True(manager.Registries != null);
            }
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
                if(manager.Registries.Count > 0)
                {
                    Assert.False(registry.Save(manager));
                }
                else
                {
                    Assert.True(registry.Save(manager));
                }
            }
        }
    }
}
