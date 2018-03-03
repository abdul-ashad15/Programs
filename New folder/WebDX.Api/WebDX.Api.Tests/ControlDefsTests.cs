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
	public class ControlDefsTests
	{
		ResourceManager m_ResMan;
		IWDEProject m_Project;

		public ControlDefsTests()
		{
            m_ResMan = new ResourceManager("WebDX.Api.Tests.ProjectExpectedResults", Assembly.GetExecutingAssembly());
		}

		[TestInitialize]
		public void Init()
		{
			m_Project = WDEProject.Create();
			m_Project.DocumentDefs.Add();
			m_Project.DocumentDefs[0].FormDefs.Add();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			m_Project.Clear();
			m_Project = null;
			GC.Collect();
		}

		[TestMethod]
		public void ControlDefsWTX()
		{
            VersionInfo.TargetVersionNumber = "2.5.0.0";
			IWDEControlDefs defs = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs;

			defs.AddLabel("Label1");
			defs.AddTextBox("TextBox1");
			defs.AddGroupBox("GroupBox1");
			defs.AddDetailGrid("DetailGrid1");

			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("ControlDefsWTX"), sw.ToString());
		}

		[TestMethod]
		public void ControlDefsRFX()
		{
			IWDEControlDefs defs = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs;

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();
			iproj.ConvertOldFormat = false;

			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;
			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("ControlDefsWTX") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.ReadEndElement();
			XmlReader.Close();

			Assert.AreEqual(4, defs.Count, "Count");
			Assert.IsTrue(defs[0] is IWDELabelDef, "[0] is not LabelDef. Expected LabelDef.");
			Assert.IsTrue(defs[1] is IWDETextBoxDef, "[1] is not TextBoxDef. Expected TextBoxDef.");
			Assert.IsTrue(defs[2] is IWDEGroupBoxDef, "[2] is not GroupBoxDef. Expected GroupBoxDef.");
			Assert.IsTrue(defs[3] is IWDEDetailGridDef, "[3] is not DetailGridDef. Expected DetailGridDef.");
		}

		[TestMethod]
		public void DetailGridWTX()
		{
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");

			IWDEControlDefs defs = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs;
			IWDEDetailGridDef def = defs.AddDetailGrid("DetailGrid1");

			def.BackColor = Color.Red;
			def.Description = "Description1";
			def.HeaderBackColor = Color.Blue;
			def.HeaderForeColor = Color.Black;
			def.HeaderHeight = 10;
			def.Help = "Help1";
			def.Hint = "Hint1";
			def.KeyOrder = 10;
			def.Location = new Rectangle(10, 20, 30, 40);
			def.OnEnter.Enabled = true;
			def.OnEnter.Description = "OnEnter Description";
			def.OnEnter.ScriptFullName = "ScriptFullName";
			def.Options = WDEDetailGridOption.RestrictExit;
			def.RecordNumberPosition = WDERecordNumberPosition.Left;
			def.Rows = 10;
			def.TabStop = true;
			def.RecordDef = m_Project.DocumentDefs[0].RecordDefs[0];

			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();

			Assert.AreEqual(m_ResMan.GetString("DetailGridDefWTX"), sw.ToString());
		}

		[TestMethod]
		public void DetailGridRFX()
		{
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");

			IWDEControlDefs defs = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs;
			IWDEDetailGridDef def = defs.AddDetailGrid("DetailGrid1");
	
			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0]);

			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			StringReader sr = new StringReader(m_ResMan.GetString("DetailGridDefWTX"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			iproj.Resolver.ResolveLinks();
			XmlReader.Close();

			Assert.AreEqual(Color.Red, def.BackColor, "BackColor");
			Assert.AreEqual(Color.Blue, def.HeaderBackColor, "HeaderBackColor");
			Assert.AreEqual(Color.Black, def.HeaderForeColor, "HeaderForeColor");
			Assert.AreEqual("DetailGrid1", def.ControlName, "ControlName");
			Assert.AreEqual("Description1", def.Description, "Description");
			Assert.AreEqual("Help1", def.Help, "Help");
			Assert.AreEqual("Hint1", def.Hint, "Hint");
			Assert.AreEqual(10, def.KeyOrder, "KeyOrder");
			Assert.AreEqual(new Rectangle(10, 20, 30, 40), def.Location, "Location");
			Assert.IsTrue(def.OnEnter.Enabled, "OnEnter.Enabled is false. Expected true.");
			Assert.AreEqual("OnEnter Description", def.OnEnter.Description, "OnEnter.Description");
			Assert.AreEqual("ScriptFullName", def.OnEnter.ScriptFullName, "OnEnter.ScriptFullName");
			Assert.AreEqual(WDEDetailGridOption.RestrictExit, def.Options, "Options");
			Assert.AreEqual(WDERecordNumberPosition.Left, def.RecordNumberPosition, "RecordNumberPosition");
			Assert.AreEqual(10, def.Rows, "Rows");
			Assert.IsTrue(def.TabStop, "TabStop is false. Expected true.");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0], def.RecordDef, "RecordDefs[0] and RecordDef are not the same. Expected same.");
		}

		[TestMethod]
		public void TextBoxGetZonesByImageType()
		{
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].ImageSourceDefs.Add("Image1");
			m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Add("Zone1");
			m_Project.DocumentDefs[0].ImageSourceDefs.Add("Image2");
			m_Project.DocumentDefs[0].ImageSourceDefs[1].ZoneDefs.Add("Zone2");

			IWDETextBoxDef def = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");
			def.ZoneLinks.Add(m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs[0]);
			def.ZoneLinks.Add(m_Project.DocumentDefs[0].ImageSourceDefs[1].ZoneDefs[0]);

			ArrayList al = def.GetZonesByImageType("Image1");
			Assert.AreEqual(1, al.Count, "Count1");
			Assert.AreSame(m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs[0], al[0], "Zone1 and al[0] are not the same. Expected same.");
			al = def.GetZonesByImageType("Image2");
			Assert.AreEqual(1, al.Count, "Count2");
			Assert.AreSame(m_Project.DocumentDefs[0].ImageSourceDefs[1].ZoneDefs[0], al[0], "Zone2 and al[0] are not the same. Expected same.");
		}

		[TestMethod]
		public void TextBoxGetZonesByImageTypeDetailGrid()
		{
            VersionInfo.TargetVersionNumber = "2.5.0.0";
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs.Add("Detail1");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("Field2");
			m_Project.DocumentDefs[0].ImageSourceDefs.Add("Image1");
			IWDEDetailZoneDef detailDef = m_Project.DocumentDefs[0].ImageSourceDefs[0].DetailZoneDefs.Add("DetailZone1");
			detailDef.LineHeight = 25;
			IWDEZoneDef zd1 = detailDef.ZoneDefs.Add("Zone1");
			zd1.ZoneRect = new Rectangle(10, 10, 500, 25);

			IWDEDetailGridDef dgDef = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddDetailGrid("DetailGrid1");
			dgDef.DetailZoneDef = detailDef;
			dgDef.RecordDef = m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0];
			IWDETextBoxDef def = dgDef.ControlDefs.AddTextBox("TextBox1");

			ArrayList al = def.GetZonesByImageType("Image1");
			Assert.AreEqual(1, al.Count, "Count");
			Assert.AreSame(zd1, al[0], "zd1 and al[0] are not the same. Expected same.");
		}

		[TestMethod]
		public void TextBoxGetZonesSubZones()
		{
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].ImageSourceDefs.Add("Image1");
			IWDEDetailZoneDef detailDef = m_Project.DocumentDefs[0].ImageSourceDefs[0].DetailZoneDefs.Add("Detail1");
			m_Project.DocumentDefs[0].ImageSourceDefs.Add("Image2");
			IWDESnippetDef snippetDef = m_Project.DocumentDefs[0].ImageSourceDefs[1].SnippetDefs.Add("Snippet1");

			IWDEZoneDef zone1 = detailDef.ZoneDefs.Add("Zone1");

			IWDETextBoxDef def = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");
			def.ZoneLinks.Add(zone1);

			ArrayList al = def.GetZonesByImageType("Image1");
			Assert.AreEqual(1, al.Count, "Count1");
			Assert.AreSame(zone1, al[0], "Zone1 and al[0] are not the same. Expected same.");
			al = def.GetZonesByImageType("Image2");
		}

		[TestMethod]
		public void TextBoxGetSnippet()
		{
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].ImageSourceDefs.Add("Image1");
			IWDEZoneDef zone1 = m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Add("Zone1");
			IWDEZoneDef zone2 = m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Add("Zone2");
			zone2.ZoneRect = new Rectangle(15, 15, 10, 10);
			IWDESnippetDef snip1 = m_Project.DocumentDefs[0].ImageSourceDefs[0].SnippetDefs.Add("Snippet1");
			snip1.SnippetRect = new Rectangle(10, 10, 30, 30);

			IWDETextBoxDef def = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");
			def.ZoneLinks.Add(zone1);

			Assert.AreSame(snip1, def.GetSnippet(zone2), "GetSnippet did not return snip1. Expected snip1");
			Assert.IsNull(def.GetSnippet(zone1), "GetSnippet did not return null for zone1. Expected null");
		}
	}
}
