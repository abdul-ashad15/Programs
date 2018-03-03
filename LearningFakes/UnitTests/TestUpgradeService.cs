using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LearningFakes;
using Microsoft.QualityTools.Testing.Fakes;
using LearningFakes.Fakes;

namespace UnitTests
{
    [TestClass]
    public class TestUpgradeService
    {
        //Void Method
        
        IUpgradeService upgradeService;
     

        [TestInitialize]
        public void Init()
        {
            upgradeService = new LearningFakes.Fakes.StubIUpgradeService()
            {
                CurrentSWVersionInt32 = (DeviceID) => { return 10; },
                IsSWUpgradeRequiredInt32 = (DeviceID) => { return true; },
                ImagesGet = () => new StubIWDEImages()
            };  
        }



        [TestMethod]
        public void TestImage()
        {
            //arrange
            StubIWDEImages st = new StubIWDEImages();
            upgradeService = new LearningFakes.Fakes.StubIUpgradeService()
            {
                ImagesGet = () => st
            };
            //act
            var ob = upgradeService.Images;
            //Assert
            Assert.AreSame(st, ob);



            //using (ShimsContext.Create())
            //{
            //    WDEImages wdImages = new LearningFakes.Fakes.ShimWDEImages()
            //    {
            //        FindString = (ImageName) => 5
            //    };

            //    int expected = 5;
            //    int actual = wdImages.Find("test");
            //    Assert.AreEqual(expected, actual, "Same Versions found");
            //}            
        }

        [TestMethod]
        public void TestCurrentSWVersion()
        {
            int expected = 10;        
            int actual = upgradeService.currentSWVersion(1);
            Assert.AreEqual(expected, actual, "Same Versions found");
        }

        [TestMethod]
        public void TestLastUpgradeDate()
        {            
            using (ShimsContext.Create())
            {
                System.Fakes.ShimDateTime.NowGet = () => { return new DateTime(2010, 11, 5); };
                var fakeTime = DateTime.Now; // It is always DateTime(2010, 11, 5); 
                Console.WriteLine(fakeTime.ToShortDateString());
            }
            var correctTime = DateTime.Now;
            Console.WriteLine(correctTime.ToShortDateString());

            //WDEImages wdImages = new LearningFakes.Fakes.ShimWDEImages()
            //{
            //    FindString = (ImageName) => 5
            // };

            using (ShimsContext.Create())
            {
                WDEImages wi = new ShimWDEImages()
                {
                    MathInt32Int32 = (i, o) => 5
                };
            }

        }
    }
}
