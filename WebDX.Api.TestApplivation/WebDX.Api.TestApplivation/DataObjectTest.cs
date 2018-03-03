using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebDX.Api.TestApplivation
{

    [TestClass]
    public class DataObjectTest
    {
        [TestMethod]
        public void TestCurrentSWVersion()
        {
            IWDEDataSet dataSet = new WebDX.Api.Fakes.StubIWDEDataSet()
            {

            };
           


            Assert.AreEqual(expected, actual, "Same Versions found");

        }
    }
}
