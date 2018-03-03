using LearningFakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests
{
    [TestClass]
    public class TestIWDImages
    {
        //Void Method

        IWDEImages images;


        [TestInitialize]
        public void Init()
        {
            images = new LearningFakes.Fakes.StubIWDEImages()
            {
                FindString = (ImageName) => { return 1; }                 
            };
        }
    }
}
