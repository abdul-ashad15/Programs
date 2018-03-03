using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDXApi;

namespace WebDX.Api.TestProject
{
    [TestClass]
    public class WebDXDataTest
    {
        [TestMethod]
        public void TestCurrentSWVersion()
        {
            int expected = 10;
            IWebDXData webDXData = new WebDXApi.Fakes.StubIWebDXData()
            {
                CurrentSWVersionInt32 = (DeviceID) => 
                {
                    return 10;
                }
            };

            int actual = webDXData.CurrentSWVersion(1);

            Assert.AreEqual(expected, actual, "Same Versions found");

        }

    }
}
