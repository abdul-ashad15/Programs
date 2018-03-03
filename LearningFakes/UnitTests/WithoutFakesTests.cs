using LearningFakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests
{
    [TestClass]
    public class WithoutFakesTests
    {
        [TestMethod]
        public void TestCurrentSWVersion()
        {
            int expected = 5;
            UpgradeService us = new UpgradeService();
            int actual = us.currentSWVersion(5);
            Assert.AreEqual(expected, actual, "Same Versions found");
        }
    }
}
