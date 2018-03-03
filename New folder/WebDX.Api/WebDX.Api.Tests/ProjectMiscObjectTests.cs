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
	public class ProjectMiscObjectTests
	{
		ResourceManager m_ResMan;
		IWDEProject m_Project;
        IWDEDocument m_Document;

		public ProjectMiscObjectTests()
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
        public void FieldFlagsAssign()
        {
            m_Project.DocumentDefs.Add();
            m_Project.DocumentDefs[0].RecordDefs.Add();
            m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add();
            m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0].Options = WDEFieldOption.AllowFlag | WDEFieldOption.MustComplete | WDEFieldOption.NumericShift | WDEFieldOption.UpperCase;

            m_Project.DocumentDefs[0].FormDefs.Add();
            m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
            ((IWDETextBoxDef)m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0]).Field = m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0];
            Assert.AreEqual(WDEControlOption.AllowFlag | WDEControlOption.MustComplete | WDEControlOption.NumericShift | WDEControlOption.UpperCase, 
                ((IWDETextBoxDef)m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0]).Options, "Options");
        }

        [TestMethod]
        public void FieldFlagsLoad()
        {
            m_Project.DocumentDefs.Add();
            m_Project.DocumentDefs[0].RecordDefs.Add();
            m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add();
            m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0].Options = WDEFieldOption.AllowFlag | WDEFieldOption.MustComplete | WDEFieldOption.NumericShift | WDEFieldOption.UpperCase;

            m_Project.DocumentDefs[0].FormDefs.Add();
            m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
            ((IWDETextBoxDef)m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0]).Field = m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0];

            using (MemoryStream ms = new MemoryStream())
            {
                ((IWDEProjectPM)m_Project).SaveToStream(ms);
                m_Project.LoadFromBytes(ms.ToArray());
            }

            Assert.AreEqual(WDEControlOption.AllowFlag | WDEControlOption.MustComplete | WDEControlOption.NumericShift | WDEControlOption.UpperCase,
                ((IWDETextBoxDef)m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0]).Options, "Options");
        }

        [TestMethod]
        public void ImageFormMapSave()
        {
            m_Project.DocumentDefs.Add();
            m_Project.DocumentDefs[0].FormDefs.Add();
            m_Project.DocumentDefs[0].ImageSourceDefs.Add();
            m_Project.SessionDefs.Add();
            m_Project.SessionDefs[0].Forms.Add(m_Project.DocumentDefs[0].FormDefs[0]);
            ((IWDESessionDef_R1)m_Project.SessionDefs[0]).SetImageFormMap(m_Project.DocumentDefs[0].ImageSourceDefs[0].ImageType, m_Project.SessionDefs[0].Forms[0]);

            MemoryStream ms = new MemoryStream();
            ((IWDEProjectPM)m_Project).SaveToStream(ms);

            ms = new MemoryStream(ms.ToArray());
            m_Project.Clear();
            m_Project.LoadFromStream(ms);

            Assert.AreSame(m_Project.SessionDefs[0].Forms[0], m_Project.SessionDefs[0].GetFormByImageType("ImageType1"));
        }

        [TestMethod]
        public void RefClear()
        {
            IWDEProject_R1 proj = (IWDEProject_R1)m_Project;
            proj.References.Add("someReference");
            proj.Clear();
            Assert.AreEqual(0, proj.References.Count);
        }

        [TestMethod]
        public void ExtClear()
        {
            IWDEProject_R1 proj = (IWDEProject_R1)m_Project;
            proj.ExternalAssemblies.Add("someAssembly");
            proj.Clear();
            Assert.AreEqual(0, proj.ExternalAssemblies.Count);
        }

        [TestMethod]
        public void ExtSave()
        {
            IWDEProject_R1 proj = (IWDEProject_R1)m_Project;
            proj.ExternalAssemblies.Add("someAssembly");

            MemoryStream ms = new MemoryStream();
            ((IWDEProjectPM)proj).SaveToStream(ms);

            ms = new MemoryStream(ms.ToArray());
            proj.Clear();
            proj.LoadFromStream(ms);
            Assert.AreEqual(1, proj.ExternalAssemblies.Count, "Count");
            Assert.AreEqual("someAssembly", proj.ExternalAssemblies[0], "Assembly Name");
        }

        [TestMethod]
        public void RefSave()
        {
            IWDEProject_R1 proj = (IWDEProject_R1)m_Project;
            proj.References.Add("someReference");

            MemoryStream ms = new MemoryStream();
            ((IWDEProjectPM)proj).SaveToStream(ms);

            ms = new MemoryStream(ms.ToArray());
            proj.Clear();
            proj.LoadFromStream(ms);
            Assert.AreEqual(1, proj.References.Count, "Count");
            Assert.AreEqual("someReference", proj.References[0], "Assembly Name");
        }

		[TestMethod]
		public void FormLinksWTX()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs.Add("Form2");

			IWDEFormLinks links = WDEFormLinks.Create(m_Project);
			links.Add(m_Project.DocumentDefs[0].FormDefs[0]);
			links.Add(m_Project.DocumentDefs[0].FormDefs[1]);

			IWDEXmlPersist ipers = (IWDEXmlPersist) links;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("FormLinksWTX"), sw.ToString());
		}

		[TestMethod]
		public void FormLinksRFX()
		{
			m_Project.DocumentDefs.Add("Document1");
			IWDEFormDef def1 = m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			IWDEFormDef def2 = m_Project.DocumentDefs[0].FormDefs.Add("Form2");

			IWDEFormLinks links = WDEFormLinks.Create(m_Project);

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) def1;
			iproj.Resolver.AddObject(obj.GetNamePath(), def1);
			obj = (WDEBaseCollectionItem) def2;
			iproj.Resolver.AddObject(obj.GetNamePath(), def2);

			IWDEXmlPersist ipers = (IWDEXmlPersist) links;
			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("FormLinksWTX") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.ReadEndElement();
			XmlReader.Close();
		
			iproj.Resolver.ResolveLinks();

			Assert.AreEqual(2, links.Count, "Count");
			Assert.AreSame(m_Project.DocumentDefs[0].FormDefs[0], links[0], "links[0] and FormDefs[0] are not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].FormDefs[1], links[1], "links[1] and FormDefs[1] are not the same. Expected same.");
		}

		[TestMethod]
		public void SessionDefsWTX()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs.Add("Form2");

			IWDESessionDefs defs = WDESessionDefs.Create(m_Project);
			defs.Add("Session1");
			defs.Add("Session2");

			defs[0].DataPanelHeight = 10;
			defs[0].FirstImage = 1;
			defs[0].ImageScale = WDEImageScale.ScalePercent;
			defs[0].ImageScalePercent = 50;
			defs[0].Options = WDESessionOption.AllowBatchReject | WDESessionOption.AllowDocReject |
				WDESessionOption.AllowFieldVerify | WDESessionOption.AllowFloatingWindow |
				WDESessionOption.AllowImageTypeChange | WDESessionOption.AllowModifyNonKeyables |
				WDESessionOption.AutoSave | WDESessionOption.ReviewAllImages | 
				WDESessionOption.ShowOverlay | WDESessionOption.ShowWorkZones;
			defs[0].PhotoStitch.ColCount = 3;
			defs[0].PhotoStitch.RowCount = 3;
			defs[0].PhotoStitch.Orientation = WDEPSOrientation.Vertical;
			defs[0].SessionStyle = WDESessionStyle.Vertical;
			defs[0].SessionType = WDESessionType.NumericRepair;
			defs[0].ShowTicker = true;
			defs[0].TickerCharHeight = 15;
			defs[0].Forms.Add(m_Project.DocumentDefs[0].FormDefs[0]);
			defs[0].Forms.Add(m_Project.DocumentDefs[0].FormDefs[1]);			

			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("SessionDefsWTX"), sw.ToString());
		}

		[TestMethod]
		public void SessionDefsRFX()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs.Add("Form2");			

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].FormDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].FormDefs[0]);
			obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].FormDefs[1];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].FormDefs[1]);		

			IWDESessionDefs defs = WDESessionDefs.Create(m_Project);
			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;
			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("SessionDefsWTX") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			ipers.ReadFromXml(XmlReader);
			XmlReader.ReadEndElement();
			XmlReader.Close();

			iproj.Resolver.ResolveLinks();

			Assert.AreEqual(2, defs.Count, "defs.count");
			Assert.AreEqual(10, defs[0].DataPanelHeight, "DataPanelHeight");
			Assert.AreEqual(1, defs[0].FirstImage, "FirstImage");
			Assert.AreEqual(WDEImageScale.ScalePercent, defs[0].ImageScale, "ImageScale");
			Assert.AreEqual(50, defs[0].ImageScalePercent, "ImageScalePercent");
			Assert.AreEqual(WDESessionOption.AllowBatchReject | WDESessionOption.AllowDocReject |
				WDESessionOption.AllowFieldVerify | WDESessionOption.AllowFloatingWindow |
				WDESessionOption.AllowImageTypeChange | WDESessionOption.AllowModifyNonKeyables |
				WDESessionOption.AutoSave | WDESessionOption.ReviewAllImages | 
				WDESessionOption.ShowOverlay | WDESessionOption.ShowWorkZones, defs[0].Options, "Options");
			Assert.AreEqual(3, defs[0].PhotoStitch.ColCount, "PS.ColCount");
			Assert.AreEqual(3, defs[0].PhotoStitch.RowCount, "PS.RowCount");
			Assert.AreEqual(WDEPSOrientation.Vertical, defs[0].PhotoStitch.Orientation);
			Assert.AreEqual(WDESessionStyle.Vertical, defs[0].SessionStyle, "SessionStyle");
			Assert.AreEqual(WDESessionType.NumericRepair, defs[0].SessionType, "SessionType");
			Assert.IsTrue(defs[0].ShowTicker, "ShowTicker is false. Expected true.");
			Assert.AreEqual(15, defs[0].TickerCharHeight, "TickerCharHeight");
			Assert.AreEqual(2, defs[0].Forms.Count, "Forms.Count");
			Assert.AreSame(m_Project.DocumentDefs[0].FormDefs[0], defs[0].Forms[0], "Forms[0] and FormDefs[0] are not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].FormDefs[1], defs[0].Forms[1], "Forms[0] and FormDefs[0] are not the same. Expected same.");
			
			Assert.AreEqual(0, defs[1].Forms.Count, "1 forms.count");
		}

		[TestMethod]
		public void RejectCodesWTX()
		{
			IWDERejectCodes codes = WDERejectCodes.Create(m_Document);
			codes.Add("001", "BAD SCAN", true);
			codes.Add("002", "BAD IMAGE", false);

            StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = (IWDEXmlPersist) codes;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("RejectCodesWTX"), sw.ToString());
		}

		[TestMethod]
		public void RejectCodesRFX()
		{
			IWDERejectCodes codes = WDERejectCodes.Create(m_Document);
			IWDEXmlPersist ipers = (IWDEXmlPersist) codes;

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();
			
			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("RejectCodesWTX") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.ReadEndElement();
			XmlReader.Close();

			Assert.AreEqual(2, codes.Count, "Count");
			Assert.AreEqual("01", codes[0].Description, true);
			Assert.AreEqual("BAD SCAN", codes[0].Description, "Description");
			Assert.IsFalse(codes[0].RequireReason, "RequireReason[0] is true. Expected false.");
			Assert.AreEqual("02", codes[1].Description, true);
			Assert.AreEqual("", codes[1].Description, "Description[1]");
			Assert.IsTrue(codes[1].RequireReason, "RequireDescription[1] is false. Expected true.");
		}

		[TestMethod]
		public void EventScriptDefWTX()
		{
			IWDEEventScriptDef def = WDEEventScriptDef.Create(m_Project, "Test");
			def.Enabled = true;
			def.Description = "SomeEvent";
			def.ScriptFullName = "Script.Full.Name";

			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();

			Assert.AreEqual(m_ResMan.GetString("EventScriptDefWTX"), sw.ToString());
		}

		[TestMethod]
		public void EventScriptDefRFX()
		{
			IWDEEventScriptDef def = WDEEventScriptDef.Create(m_Project, "Test");
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;

			StringReader sr = new StringReader(m_ResMan.GetString("EventScriptDefWTX"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			
			Assert.AreEqual("SomeEvent", def.Description, "Description");
			Assert.IsTrue(def.Enabled, "Enabled is false. Expected true.");
			Assert.AreEqual("Script.Full.Name", def.ScriptFullName, "ScriptFullName");
		}

		[TestMethod]
		public void EditDefsRFX()
		{
			IWDEEditDefs defs = WDEEditDefs.Create(m_Project);
			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();

			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("EditDefsWTX") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.ReadEndElement();
			XmlReader.Close();

			Assert.AreEqual(10, defs.Count, "Count");
            Assert.AreEqual("AddressCorrection", defs[0].DisplayName, "AddressCorrection");
            Assert.AreEqual("<CheckDigitEditDef />", ((IWDEEditDef_R1)defs[1]).EditParams, "CheckDigit");
            Assert.AreEqual("<ConditionalGotoEditDef />", ((IWDEEditDef_R1)defs[2]).EditParams, "ConditionalGoto");
            Assert.AreEqual("<DateEditDef />", ((IWDEEditDef_R1)defs[3]).EditParams, "DateEdit");
            Assert.AreEqual("<DiagnosisPtrEditDef />", ((IWDEEditDef_R1)defs[4]).EditParams, "DiagnosisPtr");
            Assert.AreEqual("<RangeEditDef />", ((IWDEEditDef_R1)defs[5]).EditParams, "Range");
            Assert.AreEqual("<RequiredEditDef />", ((IWDEEditDef_R1)defs[6]).EditParams, "Required");
            Assert.AreEqual("<SimpleListEditDef />", ((IWDEEditDef_R1)defs[7]).EditParams, "SimpleList");
            Assert.AreEqual("<TableLookupEditDef />", ((IWDEEditDef_R1)defs[8]).EditParams, "TableLookup");
            Assert.AreEqual("<ValidLengthsEditDef />", ((IWDEEditDef_R1)defs[9]).EditParams, "ValidLengths");
		}

        [TestMethod]
        public void EditDefsRFXNew()
        {
            IWDEEditDefs defs = WDEEditDefs.Create(m_Project);
            IWDEXmlPersist ipers = (IWDEXmlPersist)defs;

            IWDEProjectInternal iproj = (IWDEProjectInternal)m_Project;
            iproj.Resolver = new LinkResolver();

            StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("EditDefsWTXNew") + "</Project>");
            XmlTextReader XmlReader = new XmlTextReader(sr);
            XmlReader.MoveToContent();
            XmlReader.Read();
            XmlReader.MoveToContent();
            ipers.ReadFromXml(XmlReader);
            XmlReader.ReadEndElement();
            XmlReader.Close();

            Assert.AreEqual(10, defs.Count, "Count");
            Assert.AreEqual("AddressCorrection", defs[0].DisplayName, "AddressCorrection");
            Assert.AreEqual("CheckDigit", defs[1].DisplayName, "CheckDigit");
            Assert.AreEqual("ConditionalGoto", defs[2].DisplayName, "ConditionalGoto");
            Assert.AreEqual("ValidDate", defs[3].DisplayName, "DateEdit");
            Assert.AreEqual("ValidDiagnosisCodes", defs[4].DisplayName, "DiagnosisPtr");
            Assert.AreEqual("Range", defs[5].DisplayName, "Range");
            Assert.AreEqual("Required", defs[6].DisplayName, "Required");
            Assert.AreEqual("SimpleList", defs[7].DisplayName, "SimpleList");
            Assert.AreEqual("TableLookup", defs[8].DisplayName, "TableLookup");
            Assert.AreEqual("ValidLengths", defs[9].DisplayName, "ValidLengths");
        }

		[TestMethod]
		public void FormDefsWTX()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].ImageSourceDefs.Add("Image1");
			IWDEFormDef def = m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs.Add("Form2");

			def.Description = "Description1";
			def.BackColor = Color.Red;
			def.FormFont = new Font("Arial", 12, FontStyle.Italic);
			def.Help = "Help1";
			def.Hint = "Hint1";
			def.RecordDef = m_Project.DocumentDefs[0].RecordDefs[0];
			def.UseSnippets = true;
			def.DefaultImage = m_Project.DocumentDefs[0].ImageSourceDefs[0];

			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = (IWDEXmlPersist) m_Project.DocumentDefs[0].FormDefs;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();

			Assert.AreEqual(m_ResMan.GetString("FormDefsWTX"), sw.ToString());
		}

		[TestMethod]
		public void FormDefsRFX()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].ImageSourceDefs.Add("Image1");

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0]);
			obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].ImageSourceDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].ImageSourceDefs[0]);
			
			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("FormDefsWTX") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			IWDEXmlPersist ipers = (IWDEXmlPersist) m_Project.DocumentDefs[0].FormDefs;
			ipers.ReadFromXml(XmlReader);
			XmlReader.ReadEndElement();
			XmlReader.Close();

			iproj.Resolver.ResolveLinks();

			IWDEFormDefs defs = (IWDEFormDefs) m_Project.DocumentDefs[0].FormDefs;

			Assert.AreEqual(2, defs.Count, "Count");
			Assert.AreEqual("Description1", defs[0].Description, "Description");
			Assert.AreEqual(Color.Red, defs[0].BackColor, "FormColor");
			Assert.AreEqual("Arial", defs[0].FormFont.Name, "FontName");
			Assert.AreEqual(12, defs[0].FormFont.Size, "FontSize");
			Assert.AreEqual(FontStyle.Italic, defs[0].FormFont.Style, "FontStyle");
			Assert.AreEqual("Help1", defs[0].Help, "Help");
			Assert.AreEqual("Hint1", defs[0].Hint, "Hint");
			Assert.AreEqual("Form1", defs[0].FormName, "FormName");
			Assert.IsTrue(defs[0].UseSnippets, "UseSnippets is false. Expected true.");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0], defs[0].RecordDef);
			Assert.AreEqual("Form2", defs[1].FormName, "FormName1");
			Assert.AreSame(m_Project.DocumentDefs[0].ImageSourceDefs[0], defs[0].DefaultImage);
		}

		[TestMethod]
		public void FieldDefsWTX()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			IWDEFieldDef def = m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field2");

			def.CharSet = "A-Z";
			def.DataLen = 10;
			def.DataMask = "DataMask";
			def.DataType = WDEDataType.DateTime;
			def.DefaultValue = "DefaultValue";
			def.Description = "Description";
			def.FieldTitle = "FieldTitle";
			def.OCRAllowedErrors = 15;
			def.OCRCharSet = "Z-A";
			def.OCRConfidence = 75;
			def.OCRLine = 2;
			def.OCRName = "OCRName";
			def.OCRRepairMode = WDEOCRRepairMode.AlphaNumeric;
			def.Options = WDEFieldOption.UpperCase | WDEFieldOption.AllowFlag | WDEFieldOption.MustComplete;

			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = (IWDEXmlPersist) m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();

			Assert.AreEqual(m_ResMan.GetString("FieldDefsWTX"), sw.ToString());
		}

		[TestMethod]
		public void FieldDefsRFX()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();

			IWDEXmlPersist ipers = (IWDEXmlPersist) m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs;
			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("FieldDefsWTX") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.ReadEndElement();
			XmlReader.Close();

			IWDEFieldDefs defs = m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs;
			
			Assert.AreEqual(2, defs.Count, "Count");
			Assert.AreEqual("A-Z", defs[0].CharSet, "CharSet");
			Assert.AreEqual(10, defs[0].DataLen, "DataLen");
			Assert.AreEqual("DataMask", defs[0].DataMask, "DataMask");
			Assert.AreEqual(WDEDataType.DateTime, defs[0].DataType, "DataType");
			Assert.AreEqual("DefaultValue", defs[0].DefaultValue, "DefaultValue");
			Assert.AreEqual("Description", defs[0].Description, "Description");
			Assert.AreEqual("FieldTitle", defs[0].FieldTitle, "FieldTitle");
			Assert.AreEqual(15, defs[0].OCRAllowedErrors, "OCRAllowedErrors");
			Assert.AreEqual("Z-A", defs[0].OCRCharSet, "OCRCharSet");
			Assert.AreEqual(75, defs[0].OCRConfidence, "OCRConfidence");
			Assert.AreEqual(2, defs[0].OCRLine, "OCRLine");
			Assert.AreEqual("OCRName", defs[0].OCRName, "OCRName");
			Assert.AreEqual("Field1", defs[0].FieldName, "FieldName");
			Assert.AreEqual(WDEOCRRepairMode.AlphaNumeric, defs[0].OCRRepairMode, "OCRRepairMode");
			Assert.AreEqual(WDEFieldOption.UpperCase | WDEFieldOption.AllowFlag | WDEFieldOption.MustComplete, defs[0].Options, "Options");

			Assert.AreEqual("Field2", defs[1].FieldName, "FieldName1");
		}

		[TestMethod]
		public void RecordDefsWTX()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record2");

			IWDERecordDef def = m_Project.DocumentDefs[0].RecordDefs[0];

			def.Description = "Description";
			def.DesignRect = new Rectangle(10, 20, 30, 40);
			def.MaxRecs = 50;
			def.MinRecs = 20;

			IWDEXmlPersist ipers = (IWDEXmlPersist) m_Project.DocumentDefs[0].RecordDefs;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();

			Assert.AreEqual(m_ResMan.GetString("RecordDefsWTX"), sw.ToString());
		}

		[TestMethod]
		public void RecordDefsRFX()
		{
			m_Project.DocumentDefs.Add("Document1");

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();

			IWDERecordDefs defs = m_Project.DocumentDefs[0].RecordDefs;
			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;

			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("RecordDefsWTX") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.ReadEndElement();
			XmlReader.Close();

			Assert.AreEqual(2, defs.Count, "Count");
			Assert.AreEqual("Record1", defs[0].RecType, "RecType1");
			Assert.AreEqual("Description", defs[0].Description, "Description");
			Assert.AreEqual(new Rectangle(10, 20, 30, 40), defs[0].DesignRect, "DesignRect");
			Assert.AreEqual(50, defs[0].MaxRecs, "MaxRecs");
			Assert.AreEqual(20, defs[0].MinRecs, "MinRecs");

			Assert.AreEqual("Record2", defs[1].RecType, "RecType2");
		}

		[TestMethod]
		public void ImageSourceDefsWTX()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].ImageSourceDefs.Add("Image1");
			m_Project.DocumentDefs[0].ImageSourceDefs.Add("Image2");

			m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Add("Zone1");
			m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Add("Zone2");
			m_Project.DocumentDefs[0].ImageSourceDefs[0].SnippetDefs.Add("Snippet1");
			m_Project.DocumentDefs[0].ImageSourceDefs[0].SnippetDefs.Add("Snippet2");
			m_Project.DocumentDefs[0].ImageSourceDefs[0].DetailZoneDefs.Add("DetailZone1");
			m_Project.DocumentDefs[0].ImageSourceDefs[0].DetailZoneDefs.Add("DetailZone2");

			IWDEImageSourceDef def = m_Project.DocumentDefs[0].ImageSourceDefs[0];

			def.Overlay = "Overlay1";
			def.PerformOCR = true;
			def.StoredAttachType = "StoredAttachType1";
			def.Template = "Template1";

			IWDEXmlPersist ipers = (IWDEXmlPersist) m_Project.DocumentDefs[0].ImageSourceDefs;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();

			Assert.AreEqual(m_ResMan.GetString("ImageSourceDefsWTX"), sw.ToString());
		}

		[TestMethod]
		public void ImageSourceDefsRFX()
		{
			m_Project.DocumentDefs.Add("Document1");

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();
			iproj.ConvertOldFormat = false;

			IWDEImageSourceDefs defs = m_Project.DocumentDefs[0].ImageSourceDefs;
			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;

			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("ImageSourceDefsWTX") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.ReadEndElement();
			XmlReader.Close();
			iproj.Resolver.ResolveLinks();

			Assert.AreEqual(2, defs.Count, "Count");
			Assert.AreEqual("Image1", defs[0].ImageType, "ImageType1");
			Assert.AreEqual("Overlay1", defs[0].Overlay, "Overlay1");
			Assert.IsTrue(defs[0].PerformOCR, "PerformOCR is false. Expected true.");
			Assert.AreEqual("StoredAttachType1", defs[0].StoredAttachType, "StoredAttachType1");
			Assert.AreEqual("Template1", defs[0].Template, "Template");
			Assert.AreEqual(2, defs[0].ZoneDefs.Count, "Image1.ZoneDefs.Count");
			Assert.AreEqual("Zone1", defs[0].ZoneDefs[0].Name, "ZoneName1");
			Assert.AreEqual("Zone2", defs[0].ZoneDefs[1].Name, "ZoneName2");
			Assert.AreEqual(2, defs[0].SnippetDefs.Count, "Image1.SnippetDefs.Count");
			Assert.AreEqual("Snippet1", defs[0].SnippetDefs[0].Name, "Snippet1");
			Assert.AreEqual("Snippet2", defs[0].SnippetDefs[1].Name, "Snippet2");
			Assert.AreEqual(2, defs[0].DetailZoneDefs.Count, "DetailZoneDefs.Count");
			Assert.AreEqual("DetailZone1", defs[0].DetailZoneDefs[0].Name, "DetailZone1");
			Assert.AreEqual("DetailZone2", defs[0].DetailZoneDefs[1].Name, "DetailZone2");

			Assert.AreEqual("Image2", defs[1].ImageType, "ImageType2");
			Assert.AreEqual(0, defs[1].ZoneDefs.Count, "Image2.ZoneDefs.Count");
		}

		[TestMethod]
		public void DocumentDefsWTX()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs.Add("Document2");

			IWDEDocumentDef def = m_Project.DocumentDefs[0];
			def.Description = "Description1";
			def.StoredDocType = "StoredDocType1";
			
			IWDEXmlPersist ipers = (IWDEXmlPersist) m_Project.DocumentDefs;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();

			Assert.AreEqual(m_ResMan.GetString("DocumentDefsWTX"), sw.ToString());
		}

		[TestMethod]
		public void DocumentDefsRFX()
		{
			IWDEDocumentDefs defs = m_Project.DocumentDefs;
			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;
			
			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();

			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("DocumentDefsWTX") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.ReadEndElement();
			XmlReader.Close();

			Assert.AreEqual(2, defs.Count, "Count");
			Assert.AreEqual("Document1", defs[0].DocType, "DocType1");
			Assert.AreEqual("Description1", defs[0].Description, "Description");
			Assert.AreEqual("StoredDocType1", defs[0].StoredDocType, "StoredDocType");

			Assert.AreEqual("Document2", defs[1].DocType, "DocType2");
		}

		[TestMethod]
		public void ProjectSaveToStream()
		{
			m_Project.CreatedBy = "APITests";
			m_Project.Description = "Description";
			m_Project.ModifiedBy = "Modifier";
			m_Project.ScriptLanguage = WDEScriptLanguage.VBNet;
			m_Project.Options = WDEProjectOption.ShowCharSetError | WDEProjectOption.TrackImage;
			m_Project.Script = "SCRIPT";
		
			MemoryStream mst = new MemoryStream();
			IWDEProjectPM proj = (IWDEProjectPM) m_Project;
			proj.SaveToStream(mst);

			MemoryStream mst2 = new MemoryStream(mst.ToArray());
			
			DateTime cdate = m_Project.DateCreated;
			DateTime mdate = m_Project.DateModified;

			m_Project.Clear();
			m_Project.LoadFromStream(mst2);
			Assert.AreEqual("APITests", m_Project.CreatedBy, "CreatedBy");
			Assert.AreEqual("Description", m_Project.Description, "Description");
			Assert.AreEqual("Modifier", m_Project.ModifiedBy, "ModifiedBy");
			Assert.AreEqual(WDEScriptLanguage.VBNet, m_Project.ScriptLanguage, "ScriptLanguage");
			Assert.AreEqual(WDEProjectOption.ShowCharSetError | WDEProjectOption.TrackImage, m_Project.Options, "Options");
			Assert.AreEqual("SCRIPT", m_Project.Script, "Script");
			if(cdate.Subtract(m_Project.DateCreated).TotalMilliseconds < 1)
				cdate = m_Project.DateCreated;
			if(mdate.Subtract(m_Project.DateModified).TotalMilliseconds < 1)
				mdate = m_Project.DateModified;
			Assert.AreEqual(cdate, m_Project.DateCreated, "DateCreated");
			Assert.AreEqual(mdate, m_Project.DateModified, "DateModified");
			mst.Close();
			mst2.Close();
		}

		[TestMethod]
		public void SnippetDefsWTX()
		{
			IWDESnippetDefs defs = WDESnippetDefs.Create(m_Project);
			defs.Add("Snippet1");
			defs.Add("Snippet2");

			defs[0].SnippetRect = new Rectangle(10, 20, 30, 40);

			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();

			Assert.AreEqual(m_ResMan.GetString("SnippetDefsWTX"), sw.ToString());
		}

		[TestMethod]
		public void SnippetDefsRFX()
		{
			IWDESnippetDefs defs = WDESnippetDefs.Create(m_Project);

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();
			iproj.ConvertOldFormat = false;

			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;
			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("SnippetDefsWTX") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.ReadEndElement();
			XmlReader.Close();

			iproj.Resolver.ResolveLinks();

			Assert.AreEqual(2, defs.Count, "Count");
			Assert.AreEqual("Snippet1", defs[0].Name, "defs[0].SnippetName");
			Assert.AreEqual(new Rectangle(10, 20, 30, 40), defs[0].SnippetRect, "defs[0].SnippetRect");

			Assert.AreEqual("Snippet2", defs[1].Name, "defs[1].SnippetName");
		}

		[TestMethod]
		public void DetailZonesWTX()
		{
			IWDEDetailZoneDefs defs = WDEDetailZoneDefs.Create(m_Project);
			defs.Add("DetailZone1");
			defs.Add("DetailZone2");

			defs[0].LineCount = 6;
			defs[0].LineHeight = 150;
			defs[0].ZoneDefs.Add("Zone1");
			defs[0].ZoneDefs.Add("Zone2");

			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();

			Assert.AreEqual(m_ResMan.GetString("DetailZonesWTX"), sw.ToString());
		}

		[TestMethod]
		public void DetailZonesRFX()
		{
			IWDEDetailZoneDefs defs = WDEDetailZoneDefs.Create(m_Project);

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();
			iproj.ConvertOldFormat = false;

			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;
			StringReader sr = new StringReader("<Project>" + m_ResMan.GetString("DetailZonesWTX") + "</Project>");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.ReadEndElement();
			XmlReader.Close();

			Assert.AreEqual(2, defs.Count, "Count");
			Assert.AreEqual("DetailZone1", defs[0].Name, "defs[0].DetailZoneName");
			Assert.AreEqual(6, defs[0].LineCount, "LineCount");
			Assert.AreEqual(150, defs[0].LineHeight, "LineHeight");
			Assert.AreEqual(2, defs[0].ZoneDefs.Count, "ZoneDefs.Count");
			Assert.AreEqual("Zone1", defs[0].ZoneDefs[0].Name, "ZoneName1");
			Assert.AreEqual("Zone2", defs[0].ZoneDefs[1].Name, "ZoneName2");

			Assert.AreEqual("DetailZone2", defs[1].Name, "defs[1].DetailZoneName");
		}

		[TestMethod]
		public void SessionDefGetForm()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].ImageSourceDefs.Add("Image1");
            m_Project.DocumentDefs[0].ImageSourceDefs.Add("Image2");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs[0].DefaultImage = m_Project.DocumentDefs[0].ImageSourceDefs[0];

			m_Project.SessionDefs.Add("Session1");
			m_Project.SessionDefs[0].Forms.Add(m_Project.DocumentDefs[0].FormDefs[0]);
			IWDEFormDef def = m_Project.SessionDefs[0].GetFormByImageType("image1");
			Assert.AreSame(def, m_Project.DocumentDefs[0].FormDefs[0], "Forms[0] and def are not the same. Expected same.");
			def = m_Project.SessionDefs[0].GetFormByImageType("Image2");
			Assert.AreSame(def, m_Project.DocumentDefs[0].FormDefs[0], "Forms[0] and def are not the same during Image2. Expected same.");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void SessionDefNoForms()
		{
			m_Project.SessionDefs.Add("Session1");
			m_Project.SessionDefs[0].GetFormByImageType("NoForms");
		}

		[TestMethod]
		public void SessionDefGetFormNoImage()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].ImageSourceDefs.Add("Image1");
            m_Project.DocumentDefs[0].ImageSourceDefs.Add("Image2");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs.Add();

			m_Project.SessionDefs.Add("Session1");
			m_Project.SessionDefs[0].Forms.Add(m_Project.DocumentDefs[0].FormDefs[0]);
			m_Project.SessionDefs[0].Forms.Add(m_Project.DocumentDefs[0].FormDefs[1]);
			IWDEFormDef def = m_Project.SessionDefs[0].GetFormByImageType("image1");
			Assert.AreSame(def, m_Project.DocumentDefs[0].FormDefs[0], "Forms[0] and def are not the same. Expected same.");
			def = m_Project.SessionDefs[0].GetFormByImageType("Image2");
			Assert.AreSame(def, m_Project.DocumentDefs[0].FormDefs[0], "Forms[0] and def are not the same during Image2. Expected same.");
		}
	}
}
