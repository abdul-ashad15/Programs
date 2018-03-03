using System;
using System.Drawing;
using System.Xml;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;

using WebDX.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock;

namespace WebDX.Api.Tests
{
	/// <summary>
	/// Image Object Tests
	/// </summary>
	[TestClass]
	public class ImageObjectTests
	{
		ResourceManager m_ResMan;
		IWDEImage m_Image;
		MockObject m_Images;
		MockObject m_Document;
		MockObject m_DataSet;
		MockObject m_Project;

		public ImageObjectTests()
		{
		}

		[TestInitialize]
		public void Init()
		{
			MockManager.Init();

			m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());

			m_Images = MockManager.MockObject(typeof(IWDEImages));
			m_Document = MockManager.MockObject(typeof(IWDEDocument));
			m_DataSet = MockManager.MockObject(typeof(IWDEDataSet));
			m_Project = MockManager.MockObject(typeof(IWDEProject));

			m_Images.ExpectGetAlways("Document", m_Document.Object);
			m_Document.ExpectGetAlways("DataSet", m_DataSet.Object);
			m_DataSet.ExpectGetAlways("Project", m_Project.Object);
			m_Project.ExpectGetAlways("Options", WDEProjectOption.None);
			m_Image = WDEImage.Create((IWDEImages) m_Images.Object);
		}

		[TestCleanup]
		public void Cleanup()
		{
			m_Image = null;
			m_Images = null;
			m_Document = null;
			m_DataSet = null;
			m_Project = null;

			MockManager.Verify();
			GC.Collect();
		}

		[TestMethod]
		public void WriteToXmlDefault()
		{
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = m_Image as IWDEXmlPersist;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("ImageWriteToXmlDefault"), sw.ToString());
		}

		[TestMethod]
		public void WriteToXmlFull()
		{
			m_Project.ExpectGetAlways("Options", WDEProjectOption.TrackImage);
			m_Image.ImageName = "ImageName";
			m_Image.ImagePath = "ImagePath";
			m_Image.ImageType = "ImageType";
			m_Image.IsSnippet = true;
			m_Image.OverlayOffsetX = 1;
			m_Image.OverlayOffsetY = 2;
			m_Image.PerformOCR = true;
			m_Image.RegisteredImage = "RegisteredImage";
			m_Image.Rotate = 3;
			m_Image.StoredAttachType = "StoredAttachType";
			m_Image.ZipName = "ZipName";
			m_Image.ZoneOffsetX = 4;
			m_Image.ZoneOffsetY = 5;

			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = m_Image as IWDEXmlPersist;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("ImageWriteToXmlFull"), sw.ToString());
		}

		[TestMethod]
		public void ReadFromXmlDefault()
		{
			StringReader sr = new StringReader(m_ResMan.GetString("ImageWriteToXmlDefault"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			IWDEXmlPersist ipers = m_Image as IWDEXmlPersist;
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual("", m_Image.ImageName, "ImageName");
			Assert.AreEqual("", m_Image.ImagePath, "ImagePath");
			Assert.AreEqual("", m_Image.ImageType, "ImageType");
			Assert.AreEqual(false, m_Image.IsSnippet, "IsSnippet");
			Assert.AreEqual(0, m_Image.OverlayOffsetX, "OverlayOffsetX");
			Assert.AreEqual(0, m_Image.OverlayOffsetY, "OverlayOffsetY");
			Assert.AreEqual(false, m_Image.PerformOCR, "PerformOCR");
			Assert.AreEqual("", m_Image.RegisteredImage, "RegisteredImage");
			Assert.AreEqual(0, m_Image.Rotate, "Rotate");
			Assert.AreEqual("", m_Image.StoredAttachType, "StoredAttachType");
			Assert.AreEqual("", m_Image.ZipName, "ZipName");
			Assert.AreEqual(0, m_Image.ZoneOffsetX, "ZoneOffsetX");
			Assert.AreEqual(0, m_Image.ZoneOffsetY, "ZoneOffsetY");
		}

		[TestMethod]
		public void ReadFromXmlFull()
		{
			StringReader sr = new StringReader(m_ResMan.GetString("ImageWriteToXmlFull"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			IWDEXmlPersist ipers = m_Image as IWDEXmlPersist;
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual("ImageName", m_Image.ImageName, "ImageName");
			Assert.AreEqual("ImagePath", m_Image.ImagePath, "ImagePath");
			Assert.AreEqual("ImageType", m_Image.ImageType, "ImageType");
			Assert.AreEqual(true, m_Image.IsSnippet, "IsSnippet");
			Assert.AreEqual(1, m_Image.OverlayOffsetX, "OverlayOffsetX");
			Assert.AreEqual(2, m_Image.OverlayOffsetY, "OverlayOffsetY");
			Assert.AreEqual(true, m_Image.PerformOCR, "PerformOCR");
			Assert.AreEqual("RegisteredImage", m_Image.RegisteredImage, "RegisteredImage");
			Assert.AreEqual(3, m_Image.Rotate, "Rotate");
			Assert.AreEqual("StoredAttachType", m_Image.StoredAttachType, "StoredAttachType");
			Assert.AreEqual("ZipName", m_Image.ZipName, "ZipName");
			Assert.AreEqual(4, m_Image.ZoneOffsetX, "ZoneOffsetX");
			Assert.AreEqual(5, m_Image.ZoneOffsetY, "ZoneOffsetY");
		}
	}
}
