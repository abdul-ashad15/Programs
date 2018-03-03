using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDX.Api;
using Microsoft.QualityTools.Testing.Fakes;

namespace WebDXTestApplication
{
    [TestClass]
    public class DataObjectTest : IImageData
    {
        private IWDEDocuments m_Documents;

        [TestInitialize]
        public void Init()
        {

        }

        [TestMethod]
        public void DataSetImageTest()
        {
            using (ShimsContext.Create())
            {
                
            }
            IWDEDataSet dataSet = new WebDX.Api.Fakes.StubIWDEDataSet()
            {
                AltDCNGet = (m_Documents.AltDCN) =>
                    
            };
        }

        public IWDEImages Images()
        {
            throw new NotImplementedException();
        }
    }
}
