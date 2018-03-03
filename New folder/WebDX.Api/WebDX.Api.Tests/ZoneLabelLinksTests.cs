using System;
using System.Collections;
using System.Collections.Specialized;
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
	[TestClass]
	public class ZoneLabelLinksTests
	{
		ResourceManager m_ResMan;
		IWDEProject m_Project;

		public ZoneLabelLinksTests()
        {
            m_ResMan = new ResourceManager("WebDX.Api.Tests.ProjectExpectedResults", Assembly.GetExecutingAssembly());
		}

		[TestInitialize]
		public void Init()
		{
			m_Project = WDEProject.Create();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			m_Project.Clear();
			m_Project = null;
			GC.Collect();
		}

		[TestMethod]
		public void ZoneLinks()
		{
			m_Project.DocumentDefs.Add();
			m_Project.DocumentDefs[0].ImageSourceDefs.Add();
			IWDEZoneDef ZoneDef1 = m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Add();
			IWDEZoneDef ZoneDef2 = m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Add();

			IWDEZoneLinks ZoneLinks1 = WDEZoneLinks.Create(m_Project);
			ZoneLinks1.Add(ZoneDef1);
			ZoneLinks1.Add(ZoneDef2);
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) ZoneDef2;
			obj.NotifyLinks();
			Assert.AreEqual(1, ZoneLinks1.Count, "Count");
			Assert.AreSame(ZoneDef1, ZoneLinks1[0], "ZoneDef1 and ZoneLinks1[0] are not the same. Expected same.");

			ZoneLinks1.Add(ZoneDef2);
			obj = (WDEBaseCollectionItem) ZoneDef1;
			obj.NotifyLinks();
			Assert.AreEqual(1, ZoneLinks1.Count, "Count2");
			Assert.AreSame(ZoneDef2, ZoneLinks1[0], "ZoneDef2 and ZoneLinks1[0] are not the same. Expected same.");
		}

		[TestMethod]
		public void ZoneLinksWriteToXml()
		{
			m_Project.DocumentDefs.Add();
			m_Project.DocumentDefs[0].ImageSourceDefs.Add();
			IWDEZoneDef ZoneDef1 = m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Add();
			IWDEZoneDef ZoneDef2 = m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Add();

			IWDEZoneLinks ZoneLinks1 = WDEZoneLinks.Create(m_Project);
			ZoneLinks1.Add(ZoneDef1);
			ZoneLinks1.Add(ZoneDef2);
			
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = (IWDEXmlPersist) ZoneLinks1;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("ZoneLinksWriteToXml"), sw.ToString());
		}

		[TestMethod]
		public void ZoneLinksReadFromXml()
		{
			m_Project.DocumentDefs.Add();
			m_Project.DocumentDefs[0].ImageSourceDefs.Add();
			IWDEZoneDef ZoneDef1 = m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Add();
			IWDEZoneDef ZoneDef2 = m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Add();

			IWDEProjectInternal pi = (IWDEProjectInternal) m_Project;
			pi.Resolver = new LinkResolver();
			pi.Resolver.AddObject(((WDEBaseCollectionItem) ZoneDef1).GetNamePath(), ZoneDef1);
			pi.Resolver.AddObject(((WDEBaseCollectionItem) ZoneDef2).GetNamePath(), ZoneDef2);

			IWDEZoneLinks ZoneLinks1 = WDEZoneLinks.Create(m_Project);

			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("ZoneLinksWriteToXml") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			IWDEXmlPersist ipers = (IWDEXmlPersist) ZoneLinks1;
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();

			pi.Resolver.ResolveLinks();
			pi.Resolver = null;

			Assert.AreEqual(2, ZoneLinks1.Count);
			Assert.AreSame(ZoneDef1, ZoneLinks1[0], "ZoneDef1 and ZoneLinks1[0] are not the same. Expected same.");
			Assert.AreSame(ZoneDef2, ZoneLinks1[1], "ZoneDef2 ans ZoneLinks1[1] are not the same. Expected same.");
		}

		[TestMethod]
		public void LabelLinks()
		{
			m_Project.DocumentDefs.Add();
			m_Project.DocumentDefs[0].FormDefs.Add();
			IWDELabelDef LabelDef1 = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddLabel();
			IWDELabelDef LabelDef2 = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddLabel();

			IWDELabelLinks LabelLinks1 = WDELabelLinks.Create(m_Project);
			LabelLinks1.Add(LabelDef1);
			LabelLinks1.Add(LabelDef2);
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) LabelDef2;
			obj.NotifyLinks();
			Assert.AreEqual(1, LabelLinks1.Count, "Count");
			Assert.AreSame(LabelDef1, LabelLinks1[0], "LabelDef1 and LabelLinks1[0] are not the same. Expected same.");

			LabelLinks1.Add(LabelDef2);
			obj = (WDEBaseCollectionItem) LabelDef1;
			obj.NotifyLinks();
			Assert.AreEqual(1, LabelLinks1.Count, "Count2");
			Assert.AreSame(LabelDef2, LabelLinks1[0], "LabelDef2 and LabelLinks1[0] are not the same. Expected same.");
		}
	
		[TestMethod]
		public void LabelLinksWriteToXml()
		{
			m_Project.DocumentDefs.Add();
			m_Project.DocumentDefs[0].FormDefs.Add();
			IWDELabelDef LabelDef1 = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddLabel();
			IWDELabelDef LabelDef2 = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddLabel();

			IWDELabelLinks LabelLinks1 = WDELabelLinks.Create(m_Project);
			LabelLinks1.Add(LabelDef1);
			LabelLinks1.Add(LabelDef2);
			
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = (IWDEXmlPersist) LabelLinks1;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("LabelLinksWriteToXml"), sw.ToString());
		}

		[TestMethod]
		public void LabelLinksReadFromXml()
		{
			m_Project.DocumentDefs.Add();
			m_Project.DocumentDefs[0].FormDefs.Add();
			IWDELabelDef LabelDef1 = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddLabel();
			IWDELabelDef LabelDef2 = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddLabel();

			IWDEProjectInternal pi = (IWDEProjectInternal) m_Project;
			pi.Resolver = new LinkResolver();
			pi.Resolver.AddObject(((WDEBaseCollectionItem) LabelDef1).GetNamePath(), LabelDef1);
			pi.Resolver.AddObject(((WDEBaseCollectionItem) LabelDef2).GetNamePath(), LabelDef2);

			IWDELabelLinks LabelLinks1 = WDELabelLinks.Create(m_Project);

			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("LabelLinksWriteToXml") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			IWDEXmlPersist ipers = (IWDEXmlPersist) LabelLinks1;
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();

			pi.Resolver.ResolveLinks();
			pi.Resolver = null;

			Assert.AreEqual(2, LabelLinks1.Count);
			Assert.AreSame(LabelDef1, LabelLinks1[0], "LabelDef1 and LabelLinks1[0] are not the same. Expected same.");
			Assert.AreSame(LabelDef2, LabelLinks1[1], "LabelDef2 ans LabelLinks1[1] are not the same. Expected same.");
		}

		[TestMethod]
		public void ZoneDefs()
		{
			m_Project.DocumentDefs.Add();
			m_Project.DocumentDefs[0].ImageSourceDefs.Add();
			IWDEZoneDefs defs = m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs;
			IWDEZoneDef zd1 = defs.Add("Zone1");
			IWDEZoneDef zd2 = defs.Add("Zone2");
			IWDEZoneDef zd3 = defs.Add("Zone3");
			Assert.AreEqual(3, defs.Count, "Count");
			Assert.AreSame(zd1, defs[0], "zd1 and defs[0] are not the same. Expected same.");
			Assert.AreSame(zd2, defs[1], "zd2 and defs[1] are not the same. Expected same.");
			Assert.AreSame(zd3, defs[2], "zd3 and defs[2] are not the same. Expected same.");

			int res = defs.Find("Zone1");
			Assert.AreEqual(0, res, "res1");
			res = defs.Find("Zone2");
			Assert.AreEqual(1, res, "res2");
			res = defs.Find("Zone3");
			Assert.AreEqual(2, res, "res3");
			res = defs.Find("NotThere");
			Assert.AreEqual(-1, res, "res not there");
		}

		[TestMethod]
		public void ZoneDefWriteToXml()
		{
			m_Project.DocumentDefs.Add();
			m_Project.DocumentDefs[0].ImageSourceDefs.Add();
			IWDEZoneDef def = m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Add();
			def.ZoneRect = new Rectangle(10, 20, 30, 40);

			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("ZoneDefWriteToXml"), sw.ToString());
		}

		[TestMethod]
		public void ZoneDefReadFromXml()
		{
			m_Project.DocumentDefs.Add();
			m_Project.DocumentDefs[0].ImageSourceDefs.Add();
			IWDEZoneDef def = m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Add();
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;

			StringReader sr = new StringReader(m_ResMan.GetString("ZoneDefWriteToXml"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual("Zone1", def.Name, "ZoneName");
			Assert.AreEqual(new Rectangle(10, 20, 30, 40), def.ZoneRect, "ZoneRect");
		}

		[TestMethod]
		public void ZoneDefsWriteToXml()
		{
			m_Project.DocumentDefs.Add();
			m_Project.DocumentDefs[0].ImageSourceDefs.Add();
			IWDEZoneDefs defs = m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs;
			defs.Add("Zone1");
			defs.Add("Zone2");
			defs.Add("Zone3");
			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;
			
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("ZoneDefsWriteToXml"), sw.ToString());
		}

		[TestMethod]
		public void ZoneDefsReadFromXml()
		{
			m_Project.DocumentDefs.Add();
			m_Project.DocumentDefs[0].ImageSourceDefs.Add();
			IWDEZoneDefs defs = m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs;
			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();
			
			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("ZoneDefsWriteToXml") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			Assert.AreEqual(3, defs.Count);
			Assert.AreEqual("Zone1", defs[0].Name, "Zone1");
			Assert.AreEqual("Zone2", defs[1].Name, "Zone2");
			Assert.AreEqual("Zone3", defs[2].Name, "Zone3");
			Assert.AreEqual(XmlReader.Name, "Project");
			
			XmlReader.Close();
		}
	}
}
