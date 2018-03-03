using System;
using System.Drawing;
using System.Xml;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;

using WebDX.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using TypeMock;

namespace WebDX.Api.Tests
{
	/// <summary>
	///Images Object Tests
	/// </summary>
	[TestClass]
	public class ImagesObjectTests
	{
		ResourceManager m_ResMan;
		IWDEImages m_Images;
        //MockObject m_Document;
        //MockObject m_DataSet;
        //MockObject m_Project;
        //MockObject m_DocumentDefs;
        //MockObject m_DocumentDef;
        //MockObject m_ImageSourceDefs;
        //MockObject m_ImageSourceDef;

        IWDEDocument m_Document;
        IWDEDataSet m_DataSet;
        IWDEProject m_Project;
        IWDEDocumentDefs m_DocumentDefs;
        IWDEDocumentDef m_DocumentDef;
        IWDEImageSourceDefs m_ImageSourceDefs;
        IWDEImageSourceDef m_ImageSourceDef;

        public ImagesObjectTests()
		{
		}

		[TestInitialize]
		public void Init()
		{
			//MockManager.Init();

			m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());
			//m_Document = MockManager.MockObject(typeof(IWDEDocument));
			//m_DataSet = MockManager.MockObject(typeof(IWDEDataSet));
			//m_Project = MockManager.MockObject(typeof(IWDEProject));
			//m_DocumentDefs = MockManager.MockObject(typeof(IWDEDocumentDefs));
			//m_DocumentDef = MockManager.MockObject(typeof(IWDEDocumentDef));
			//m_ImageSourceDefs = MockManager.MockObject(typeof(IWDEImageSourceDefs));
			//m_ImageSourceDef = MockManager.MockObject(typeof(IWDEImageSourceDef));

			//m_Document.ExpectGetAlways("DataSet", m_DataSet.Object);
			//m_Document.ExpectGetAlways("DocumentDef", m_DocumentDef.Object);
			//m_Document.ExpectGetAlways("DocType", "Document1");
			//m_DataSet.ExpectGetAlways("Project", m_Project.Object);
			//m_Project.ExpectGetAlways("Options", WDEProjectOption.None);

			//m_DataSet.ExpectGetAlways("DocumentDefs", m_DocumentDefs.Object);
			//m_DocumentDefs.AlwaysReturn("Find", 0);
			//m_DocumentDefs.ExpectGetAlways("Item", m_DocumentDef.Object);
			//m_DocumentDef.ExpectGetAlways("ImageSourceDefs", m_ImageSourceDefs.Object);
			//m_ImageSourceDefs.AlwaysReturn("Find", 0);
			//m_ImageSourceDefs.ExpectGetAlways("Item", m_ImageSourceDef.Object);
			//m_ImageSourceDef.ExpectGetAlways("PerformOCR", true);
			//m_ImageSourceDef.ExpectGetAlways("StoredAttachType", "StoredAttachType");

			m_Images = WDEImages.Create((IWDEDocument) m_Document);
		}

		[TestCleanup]
		public void Cleanup()
		{
			m_Images = null;
			m_Document = null;
			m_DataSet = null;
			m_Project = null;
			m_DocumentDefs = null;
			m_DocumentDef = null;
			m_ImageSourceDefs = null;
			m_ImageSourceDef = null;

			//MockManager.Verify();
			GC.Collect();
		}

		[TestMethod]
		public void Add()
		{
			IWDEImage res = m_Images.Append("ImageType", "ImageName", "ImagePath", "ZipName", true);
			Assert.AreEqual(1, m_Images.Count, "Count");
			Assert.AreSame(res, m_Images[0], "Res and m_Images[0] are not the same, expected same");
			Assert.AreEqual("ImageName", res.ImageName, "ImageName");
			Assert.AreEqual("ImageType", res.ImageType, "ImageType");
			Assert.AreEqual("ImagePath", res.ImagePath, "ImagePath");
			Assert.AreEqual("ZipName", res.ZipName, "ZipName");
			Assert.IsTrue(res.IsSnippet, "IsSnippet is false, expected true");
			Assert.IsTrue(res.PerformOCR, "PerformOCR is false, expected true");
			Assert.AreEqual("StoredAttachType", res.StoredAttachType, "StoredAttachType");

			m_Images.Clear();
			Assert.AreEqual(0, m_Images.Count, "Clear");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void AddNoImage()
		{
			//m_ImageSourceDefs.AlwaysReturn("Find", -1);
			m_Images.Append("Image1", "ImageName");
		}

		[TestMethod]
		public void Insert()
		{
			IWDEImage res = m_Images.Insert(0, "ImageType", "Image1");
			Assert.AreEqual(1, m_Images.Count, "Count");
			Assert.AreSame(res, m_Images[0], "res and m_Images[0] are not the same, expected same");

			m_Images.Insert(0, "ImageType", "Image2");
			Assert.AreEqual(2, m_Images.Count, "Count");
			Assert.AreEqual("Image2", m_Images[0].ImageName, "ImageName2");

			m_Images.Insert(1, "ImageType", "Image3");
			Assert.AreEqual("Image3", m_Images[1].ImageName, "ImageName3");
			Assert.AreEqual("Image1", m_Images[2].ImageName, "ImageName1");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void InsertNoRecsInvalid()
		{
			m_Images.Insert(1, "ImageType", "Image1");
		}

		[TestMethod]
		public void InsertNoRecs()
		{
			m_Images.Insert(0, "ImageType", "Image1");
			Assert.AreEqual(1, m_Images.Count);
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void InsertLow()
		{
			m_Images.Insert(-1, "ImageType", "Image1");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void InsertHigh()
		{
			m_Images.Append("ImageType", "Image1");
			m_Images.Insert(1, "ImageType", "Image2");
		}
		
		[TestMethod]
		public void Find()
		{
			m_Images.Append("ImageType", "Image1");
			m_Images.Append("ImageType", "Image2");
			m_Images.Append("ImageType", "Image3");

			int index = m_Images.Find("Image1");
			Assert.AreEqual(0, index, "Image1");
			index = m_Images.Find("Image2");
			Assert.AreEqual(1, index, "Image2");
			index = m_Images.Find("Image3");
			Assert.AreEqual(2, index, "Image3");
			index = m_Images.Find("NotThere");
			Assert.AreEqual(-1, index, "NotThere");
		}

		[TestMethod]
		public void WriteToXml()
		{
			//m_ImageSourceDef.ExpectGetAlways("PerformOCR", false);
			//m_ImageSourceDef.ExpectGetAlways("StoredAttachType", "");

			m_Images.Append("ImageType", "Image1");
			m_Images.Append("ImageType", "Image2");
			m_Images.Append("ImageType", "Image3");

			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = m_Images as IWDEXmlPersist;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("ImagesWriteToXml"), sw.ToString());
		}

		[TestMethod]
		public void ReadFromXml()
		{
			IWDEXmlPersist ipers = m_Images as IWDEXmlPersist;
			string test = "<DataSet>" + m_ResMan.GetString("ImagesWriteToXml") + "</DataSet>";
			StringReader sr = new StringReader(test);
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.ReadEndElement();
			XmlReader.Close();
			Assert.AreEqual(3, m_Images.Count);
		}
	}
}
