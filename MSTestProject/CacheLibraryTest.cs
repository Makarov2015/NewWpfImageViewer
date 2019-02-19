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

        //[Theory]
        //[InlineData("0,0,0", 0)]
        //[InlineData("0,1,2", 3)]
        //[InlineData("1,2,3", 6)]
        //public void Add_MultipleNumbers_ReturnsSumOfNumbers(string input, int expected)
        //{
        //    var stringCalculator = new StringCalculator();

        //    var actual = stringCalculator.Add(input);

        //    Assert.Equal(expected, actual);
        //}

        // Имя метода - Сценарий выполнения - Ожидаемое поведение
        // public void Initialization_ExistingWithRightsPath_RegistriesNotTull()
        //{
        //// Arrange - Упорядочивание
        //var stringCalculator = new StringCalculator();

        //// Act - Действие         
        //var actual = stringCalculator.Add("");

        //// Assert - Проверка
        //Assert.Equal(0, actual);
        //}

        // Initialization_NotExistingOrNoRightsPath_ThrowException

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
