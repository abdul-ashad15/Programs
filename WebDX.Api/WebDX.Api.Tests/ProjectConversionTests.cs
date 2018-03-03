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
using TypeMock;

namespace WebDX.Api.Tests
{
	[TestClass]
	public class ProjectConversionTests
	{
		ResourceManager m_ResMan;
		IWDEProject m_Project;

		public ProjectConversionTests()
        {
            m_ResMan = new ResourceManager("WebDX.Api.Tests.ProjectConversion", Assembly.GetExecutingAssembly());
		}

		[TestInitialize]
		public void Init()
		{
			m_Project = WDEProject.Create();
			IWDEProjectInternal proj = (IWDEProjectInternal) m_Project;
			proj.Resolver = new LinkResolver();
		}

		[TestCleanup]
		public void TearDown()
		{
			m_Project.Clear();
			m_Project = null;
			GC.Collect();
		}

		[TestMethod]
		public void ProjectObject()
		{
			MemoryStream mst = new MemoryStream(Encoding.UTF8.GetBytes(m_ResMan.GetString("ProjectObject")));
			m_Project.LoadFromStream(mst);
			
			Assert.AreEqual(Color.Black, m_Project.ProjectColors.CellColor, "CellColor");
			Assert.AreEqual(Color.Purple, m_Project.ProjectColors.CellBorderColor, "CellBorderColor");
			Assert.AreEqual("Creator", m_Project.CreatedBy, "CreatedBy");
			Assert.AreEqual(DateTime.Parse("2006-02-28T15:22:24.265-07:00"), m_Project.DateCreated, "DateCreated");
			Assert.AreEqual(DateTime.Parse("2006-02-28T15:22:24.265-07:00"), m_Project.DateModified, "DateModified");
			Assert.AreEqual("Modifier", m_Project.ModifiedBy, "ModifiedBy");
			Assert.AreEqual(WDEProjectOption.TrackImage | WDEProjectOption.ShowCharSetError, m_Project.Options, "Options");
			Assert.AreEqual(WDEScriptLanguage.DelphiScript, m_Project.ScriptLanguage, "ScriptLanguage");
			Assert.AreEqual(Color.Maroon, m_Project.ProjectColors.SnippetColor, "SnippetColor");
			Assert.AreEqual(Color.Olive, m_Project.ProjectColors.SnippetBorderColor, "SnippetBorderColor");
			Assert.AreEqual(Color.Teal, m_Project.ProjectColors.ZoneColor, "ZoneColor");
			Assert.AreEqual(Color.Gray, m_Project.ProjectColors.ZoneBorderColor, "ZoneBorderColor");
			Assert.AreEqual("1.3.0.0", m_Project.APIVersion, "APIVersion");
			Assert.AreEqual("var script1;" + Environment.NewLine + Environment.NewLine, m_Project.Script, "Script");
			Assert.AreEqual("Description", m_Project.Description, "Description");
		}

		[TestMethod]
		public void DocumentDefs()
		{
			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;

			StringReader sr = new StringReader(m_ResMan.GetString("DocumentDefs"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();

			IWDEXmlPersist ipers = (IWDEXmlPersist) m_Project.DocumentDefs;
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			
			Assert.AreEqual(1, m_Project.DocumentDefs.Count, "Count");
			Assert.AreEqual("StoredDocType", m_Project.DocumentDefs[0].StoredDocType, "StoredDocType");
			Assert.AreEqual("Description", m_Project.DocumentDefs[0].Description, "Description");
			Assert.AreEqual("Document1", m_Project.DocumentDefs[0].DocType, "DocType");
		}

		[TestMethod]
		public void RejectCodeLinks()
		{
			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();

			IWDERejectCode code = m_Project.RejectCodes.Add("01");
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) code;
			iproj.Resolver.AddObject(obj.GetNamePath(), code);
			m_Project.DocumentDefs.Add("Document1");
			IWDEXmlPersist ipers = (IWDEXmlPersist) m_Project.DocumentDefs[0].AllowedRejectCodes;
			
			StringReader sr = new StringReader(m_ResMan.GetString("RejectCodeLinks"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			iproj.Resolver.ResolveLinks();
			Assert.AreSame(m_Project.RejectCodes[0], m_Project.DocumentDefs[0].AllowedRejectCodes[0], "Allowed and RejectCodes[0] are not the same. Expected same.");
		}

		[TestMethod]
		public void ImageSourceDefs()
		{
			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();

			m_Project.DocumentDefs.Add("Document1");
			IWDEXmlPersist ipers = (IWDEXmlPersist) m_Project.DocumentDefs[0].ImageSourceDefs;
			
			StringReader sr = new StringReader(m_ResMan.GetString("ImageSourceDefs"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();

			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual(1, m_Project.DocumentDefs[0].ImageSourceDefs.Count, "Count");
			Assert.AreEqual("ImageSource1", m_Project.DocumentDefs[0].ImageSourceDefs[0].ImageType, "ImageType");
			Assert.AreEqual("overlay", m_Project.DocumentDefs[0].ImageSourceDefs[0].Overlay, "Overlay");
			Assert.AreEqual("template", m_Project.DocumentDefs[0].ImageSourceDefs[0].Template, "Template");
			Assert.IsTrue(m_Project.DocumentDefs[0].ImageSourceDefs[0].PerformOCR, "PerformOCR is false. Expected true.");
			Assert.AreEqual("DESC", m_Project.DocumentDefs[0].ImageSourceDefs[0].Description, "Description");
		}

		[TestMethod]
		public void RecordDefs()
		{
			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();

			m_Project.DocumentDefs.Add("Document1");
			IWDEXmlPersist ipers = (IWDEXmlPersist) m_Project.DocumentDefs[0].RecordDefs;

			StringReader sr = new StringReader(m_ResMan.GetString("RecordDefs"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();

			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();

			Assert.AreEqual(1, m_Project.DocumentDefs[0].RecordDefs.Count, "Count");
			Assert.AreEqual("Record1", m_Project.DocumentDefs[0].RecordDefs[0].RecType, "RecType");
			Assert.AreEqual(new Rectangle(20, 20, 300, 125), m_Project.DocumentDefs[0].RecordDefs[0].DesignRect, "DesignRect");
			Assert.AreEqual("DESC", m_Project.DocumentDefs[0].RecordDefs[0].Description, "Description");
			Assert.AreEqual(10, m_Project.DocumentDefs[0].RecordDefs[0].MaxRecs, "MaxRecs");
			Assert.AreEqual(5, m_Project.DocumentDefs[0].RecordDefs[0].MinRecs, "MinRecs");
		}

		[TestMethod]
		public void FieldDefs()
		{
			m_Project.ScriptLanguage = WDEScriptLanguage.DelphiScript;
			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();

			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			IWDEFieldDefs defs = m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs;
			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;
			
			StringReader sr = new StringReader(m_ResMan.GetString("FieldDefs"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();

			Assert.AreEqual(1, defs.Count);
			Assert.AreEqual("Field1", defs[0].FieldName, "FieldName");
			Assert.AreEqual(WDEFieldOption.AllowFlag | WDEFieldOption.MustComplete | 
				WDEFieldOption.NumericShift | WDEFieldOption.UpperCase, defs[0].Options, "Options");
			Assert.AreEqual("A-Z", defs[0].CharSet, "CharSet");
			Assert.AreEqual(20, defs[0].DataLen, "DataLen");
			Assert.AreEqual(WDEDataType.Text, defs[0].DataType, "DataType");
			Assert.AreEqual("DEFAULT", defs[0].DefaultValue, "DefaultValue");
			Assert.AreEqual(10, defs[0].OCRAllowedErrors, "OCRAllowedErrors");
			Assert.AreEqual("A-Z", defs[0].OCRCharSet, "OCRCharSet");
			Assert.AreEqual(75, defs[0].OCRConfidence, "OCRConfidence");
			Assert.AreEqual(WDEOCRRepairMode.AlphaNumeric, defs[0].OCRRepairMode, "OCRRepairMode");
			Assert.AreEqual("Field1", defs[0].OCRName, "OCRName");
			Assert.AreEqual(2, defs[0].OCRLine, "OCRLine");
			Assert.AreEqual(WDEQIOption.Exclude, defs[0].QIOption, "QIOption");
			Assert.AreEqual("DESC", defs[0].Description, "Description");
			Assert.AreEqual("Field1", defs[0].FieldTitle, "Title");

			IWDEEditDefs editDefs = defs[0].EditDefs;
			Assert.AreEqual(5, editDefs.Count, "EditDefs.Count");
			Assert.IsTrue(editDefs[0] is IWDECheckDigitEditDef, "editDefs[0] is " + editDefs[0].GetType().FullName + ". Expected IWDECheckDigitEditDef.");
			Assert.IsTrue(editDefs[1] is IWDERangeEditDef, "editDefs[0] is " + editDefs[0].GetType().FullName + ". Expected IWDERangeEditDef.");
			Assert.IsTrue(editDefs[2] is IWDERequiredEditDef, "editDefs[0] is " + editDefs[0].GetType().FullName + ". Expected IWDERequiredEditDef.");
			Assert.IsTrue(editDefs[3] is IWDESimpleListEditDef, "editDefs[0] is " + editDefs[0].GetType().FullName + ". Expected IWDESimpleListEditDef.");
			Assert.IsTrue(editDefs[4] is IWDEValidLengthsEditDef, "editDefs[0] is " + editDefs[0].GetType().FullName + ". Expected IWDEValidLengthsEditDef.");

			IWDECheckDigitEditDef checkDigit = (IWDECheckDigitEditDef) editDefs[0];
			IWDERangeEditDef range = (IWDERangeEditDef) editDefs[1];
			IWDERequiredEditDef required = (IWDERequiredEditDef) editDefs[2];
			IWDESimpleListEditDef simpleList = (IWDESimpleListEditDef) editDefs[3];
			IWDEValidLengthsEditDef validLengths = (IWDEValidLengthsEditDef) editDefs[4];
			
			Assert.IsTrue(checkDigit.Enabled, "checkDigit.Enabled is false. Expected true.");
			Assert.AreEqual(WDECheckDigitMethods.AMEX | WDECheckDigitMethods.Diners |
				WDECheckDigitMethods.Discover | WDECheckDigitMethods.ISBN |
				WDECheckDigitMethods.MasterCard | WDECheckDigitMethods.Mod10 |
				WDECheckDigitMethods.UPC | WDECheckDigitMethods.UPSPBN |
				WDECheckDigitMethods.UPSTracking | WDECheckDigitMethods.Visa, checkDigit.Methods, "checkDigit.Methods");
			Assert.AreEqual("DESC", checkDigit.Description, "checkDigit.Description");
			Assert.AreEqual("ErrorMessage", checkDigit.ErrorMessage, "checkDigit.ErrorMessage");

			Assert.IsTrue(range.Enabled, "range.Enabled is false. Expected true.");
			Assert.AreEqual("10", range.HighRange, "range.HighRange");
			Assert.AreEqual("5", range.LowRange, "range.LowRange");
			Assert.AreEqual("DESC", range.Description, "range.Description");
			Assert.AreEqual("ErrorMessage", range.ErrorMessage, "range.ErrorMessage");

			Assert.IsTrue(required.Enabled, "required.Enabled is false. Expected true.");
			Assert.AreEqual("DESC", required.Description, "required.Description");
			Assert.AreEqual("ErrorMessage", required.ErrorMessage, "required.ErrorMessage");
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) required;
			Assert.AreEqual(obj.GetNamePath(), required.Expression, "Expression");
			Assert.AreEqual(Environment.NewLine + "function Document1.Record1.Field1.Required: Boolean; begin Result := Expression; end;", m_Project.Script, "Script");

			Assert.IsTrue(simpleList.Enabled, "simpleList.Enabled is false. Expected true.");
			Assert.AreEqual("DESC", simpleList.Description, "simpleList.Description");
			Assert.AreEqual("ErrorMessage", simpleList.ErrorMessage, "simpleList.ErrorMessage");
			Assert.AreEqual(2, simpleList.List.Length, "simpleList.List.Length");
			Assert.AreEqual("ONE", simpleList.List[0], "simpleList.List[0]");
			Assert.AreEqual("TWO", simpleList.List[1], "simpleList.List[1]");

			Assert.IsTrue(validLengths.Enabled, "validLengths.Enabled is false. Expected true.");
			Assert.AreEqual("DESC", validLengths.Description, "validLengths.Description");
			Assert.AreEqual("ErrorMessage", validLengths.ErrorMessage, "validLengths.ErrorMessage");
			Assert.AreEqual(2, validLengths.Lengths.Length, "validLengths.Lengths.Length");
			Assert.AreEqual(1, validLengths.Lengths[0], "validLengths.Lengths[0]");
			Assert.AreEqual(2, validLengths.Lengths[1], "validLengths.Lengths[1]");
		}

		[TestMethod]
		public void FormDefs()
		{
			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();

			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0]);
			obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0]);

			IWDEFormDefs defs = m_Project.DocumentDefs[0].FormDefs;
			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;

			StringReader sr = new StringReader(m_ResMan.GetString("FormDefs"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			iproj.Resolver.ResolveLinks();

			Assert.AreEqual(1, defs.Count, "Count");
			Assert.AreEqual("Form1", defs[0].FormName, "FormName");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0], defs[0].RecordDef, "m_Project.DocumentDefs[0].RecordDefs[0] and defs[0].RecordDef are not the same. Expected same.");
			Assert.IsTrue(defs[0].UseSnippets, "UseSnippets is false. Expected true.");
			Assert.AreEqual("DESC", defs[0].Description, "Description");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.Control), defs[0].BackColor, "BackColor");
			Assert.AreEqual("Microsoft Sans Serif", defs[0].FormFont.Name, "FormFont.Name");
			Assert.AreEqual("Help", defs[0].Help, "Help");
			Assert.AreEqual("Hint", defs[0].Hint, "Hint");

			Assert.AreEqual(2, m_Project.DocumentDefs[0].ImageSourceDefs.Count, "ImageSourceDefs.Count");
			Assert.AreEqual("ImageSource1", m_Project.DocumentDefs[0].ImageSourceDefs[0].ImageType, "ImageSourceDefs[0].ImageType");
			Assert.AreEqual("ImageSource2", m_Project.DocumentDefs[0].ImageSourceDefs[1].ImageType, "ImageSourceDefs[1].ImageType");
			Assert.AreSame(m_Project.DocumentDefs[0].ImageSourceDefs[0], defs[0].DefaultImage, "ImageSourceDefs[0] and defs[0].DefaultImage are not the same. Expected same.");
			Assert.AreEqual(1, m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Count, "ImageSourceDefs[0].ZoneDefs.Count");
			Assert.AreEqual("Form1_Zone1", m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs[0].Name, "Zone1");
			Assert.AreEqual(new Rectangle(982, 551, 310, 156), m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs[0].ZoneRect, "ZoneRect1");

			Assert.AreEqual(3, m_Project.DocumentDefs[0].ImageSourceDefs[1].ZoneDefs.Count, "ImageSourceDefs[1].ZoneDefs.Count");
			Assert.AreEqual("Form1_Zone1", m_Project.DocumentDefs[0].ImageSourceDefs[1].ZoneDefs[0].Name, "[1]Zone1");
			Assert.AreEqual("Form1_Zone4", m_Project.DocumentDefs[0].ImageSourceDefs[1].ZoneDefs[1].Name, "[1]Zone4");
			Assert.AreEqual("Form1_Zone5", m_Project.DocumentDefs[0].ImageSourceDefs[1].ZoneDefs[2].Name, "[1]Zone5");
			Assert.AreEqual(new Rectangle(155, 436, 166, 140), m_Project.DocumentDefs[0].ImageSourceDefs[1].ZoneDefs[0].ZoneRect, "[1]ZoneRect");
			Assert.AreEqual(1, m_Project.DocumentDefs[0].ImageSourceDefs[1].SnippetDefs.Count, "ImageSourceDefs[1].SnippetDefs.Count");
			Assert.AreEqual("Form1_Zone3", m_Project.DocumentDefs[0].ImageSourceDefs[1].SnippetDefs[0].Name, "SnippetName");
			Assert.AreEqual(new Rectangle(551, 431, 220, 150), m_Project.DocumentDefs[0].ImageSourceDefs[1].SnippetDefs[0].SnippetRect, "SnippetRect");
			Assert.AreEqual(1, m_Project.DocumentDefs[0].ImageSourceDefs[1].DetailZoneDefs.Count, "ImageSourceDefs[1].DetailZoneDefs.Count");
			Assert.AreEqual("Form1_Zone2", m_Project.DocumentDefs[0].ImageSourceDefs[1].DetailZoneDefs[0].Name, "DetailZoneName");
			Assert.AreEqual(6, m_Project.DocumentDefs[0].ImageSourceDefs[1].DetailZoneDefs[0].LineCount, "LineCount");
			Assert.AreEqual(95, m_Project.DocumentDefs[0].ImageSourceDefs[1].DetailZoneDefs[0].LineHeight, "LineHeight");
		}

		[TestMethod]
		public void LabelDef()
		{
			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();

			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			iproj.Resolver.AddObject(obj.GetNamePath(), obj);

			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			IWDETextBoxDef textBoxDef = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");

			obj = (WDEBaseCollectionItem) textBoxDef;
			iproj.Resolver.AddObject(obj.GetNamePath(), textBoxDef);

			IWDELabelDef def = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddLabel("Label1");
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;

			StringReader sr = new StringReader(m_ResMan.GetString("LabelDef"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			iproj.Resolver.ResolveLinks();

			Assert.AreEqual("Label1", def.ControlName, "ControlName");
			Assert.IsTrue(def.AutoSize, "AutoSize is false. Expected true.");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0], def.Field, "def.Field and FieldDefs[0] are not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0], def.NextControl, "def.NextControl and ControlDefs[0] are not the same. Expected same.");
			Assert.AreEqual("DESC", def.Description, "Description");
			//			Assert.AreEqual(ContentAlignment.MiddleCenter, def.Alignment, "Alignment");
			Assert.AreEqual("Field1", def.Caption, "Caption");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.WindowText), def.ForeColor, "ForeColor");
			Assert.AreEqual(new Rectangle(26, 23, 39, 16), def.Location, "Location");
		}

		[TestMethod]
		public void CloneTest()
		{
			IWDEBalanceCheckEditDef def = WDEBalanceCheckEditDef.Create();
			IWDEFieldDef fieldDef = WDEFieldDef.Create();
			def.SumFields.Add(fieldDef);
		}

		[TestMethod]
		public void EventScript()
		{
			m_Project.ScriptLanguage = WDEScriptLanguage.DelphiScript;
			m_Project.Script = "var x: Integer;" + Environment.NewLine;
			IWDEEventScriptDef def = WDEEventScriptDef.Create(m_Project, "OnValidate");
			IWDEProjectInternal proj = (IWDEProjectInternal) m_Project;
			proj.ConvertOldFormat = true;

			StringReader sr = new StringReader(m_ResMan.GetString("EventScript"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			
			Assert.AreEqual(m_ResMan.GetString("EventScriptResult"), m_Project.Script, "Script");
			Assert.AreEqual("OnValidate", def.ScriptFullName, "ScriptFullName");
		}

		[TestMethod]
		public void AddressCorrection()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field2");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs[0].RecordDef = m_Project.DocumentDefs[0].RecordDefs[0];

			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");
			IWDETextBoxDef tdef = (IWDETextBoxDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0];
			tdef.EditDefs.Add(WDEAddressCorrectionEditDef.Create());
			IWDEAddressCorrectionEditDef def = (IWDEAddressCorrectionEditDef) tdef.EditDefs[0];

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();
			
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0]);
			obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1]);

			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			StringReader sr = new StringReader(m_ResMan.GetString("AddressCorrection"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			iproj.Resolver.ResolveLinks();

			Assert.IsTrue(def.Enabled, "Enabled is false. Expected true.");
			Assert.AreEqual("DESC", def.Description, "Description");
			Assert.AreEqual("ERROR_MESSAGE", def.ErrorMessage, "ErrorMessage");
			Assert.AreEqual(WDEEditErrorType.Failure, def.ErrorType, "ErrorType");
			Assert.IsTrue(def.ReviewResults, "ReviewResults is false. Expected true.");
			Assert.IsTrue(def.UseDPV, "UseDPV is false. Expected true.");
			Assert.AreEqual(WDESessionType.FullForm | WDESessionType.Field | WDESessionType.PhotoStitch, def.SessionMode, "SessionMode");

			IWDEFieldDef field1 = m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0];
			IWDEFieldDef field2 = m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1];
			Assert.AreSame(field1, def.LookupFields.AddressField, "Lookup.AddressField and field1 are not the same. Expected same.");
			Assert.AreSame(field2, def.LookupFields.CityField, "Lookup.CityField and field2 are not the same. Expected same.");
			Assert.AreSame(field1, def.LookupFields.ZipCodeField, "Lookup.ZipCodeField and field1 are not the same. Expected same.");

			Assert.AreSame(field1, def.ResultFields.AddressField, "Result.AddressField and field1 are not the same. Expected same.");
			Assert.AreSame(field2, def.ResultFields.CityField, "Result.CityField and field2 are not the same. Expected same.");
			Assert.AreSame(field2, def.ResultFields.StateField, "Result.StateField and field2 are not the same. Expected same.");
			Assert.AreSame(field1, def.ResultFields.ZipCodeField, "Result.ZipCodeField and field1 are not the same. Expected same.");
			Assert.AreSame(field2, def.ResultFields.ZipPlus4Field, "Result.ZipPlus4Field and field2 are not the same. Expected same.");
			Assert.AreSame(field1, def.ResultFields.HouseNumberField, "Result.HouseNumberField and field1 are not the same. Expected same.");
			Assert.AreSame(field1, def.ResultFields.PreDirectionField, "Result.PreDirectionField and field1 are not the same. Expected same.");
			Assert.AreSame(field1, def.ResultFields.StreetNameField, "Result.StreetNameField and field1 are not the same. Expected same.");
			Assert.AreSame(field2, def.ResultFields.StreetSuffixField, "Result.StreetSuffixField and field2 are not the same. Expected same.");
			Assert.AreSame(field2, def.ResultFields.PostDirectionField, "Result.PostDirectionField and field2 are not the same. Expected same.");
			Assert.AreSame(field1, def.ResultFields.AptUnitAbbrField, "Result.AptUnitAbbrField and field1 are not the same. Expected same.");
			Assert.AreSame(field2, def.ResultFields.AptNumberField, "Result.AptNumberField and field2 are not the same. Expected same.");
		}

		[TestMethod]
		public void ConditionalGoto()
		{
			m_Project.ScriptLanguage = WDEScriptLanguage.DelphiScript;
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field2");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs[0].RecordDef = m_Project.DocumentDefs[0].RecordDefs[0];

			IWDETextBoxDef tb = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");
			tb.Field = m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0];

			tb = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox2");
			tb.Field = m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1];

			IWDEConditionalGotoEditDef def = WDEConditionalGotoEditDef.Create();
			tb = (IWDETextBoxDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0];
			tb.EditDefs.Add(def);

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();
			iproj.ConvertOldFormat = true;
			
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0]);
			obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[1];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[1]);

			StringReader sr = new StringReader(m_ResMan.GetString("ConditionalGoto"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			iproj.Resolver.ResolveLinks();

			Assert.IsTrue(def.Enabled, "Enabled is false. Expected true.");
			Assert.AreEqual(WDEEditErrorType.Failure, def.ErrorType, "ErrorType");
			Assert.AreEqual(WDESessionType.FullForm | WDESessionType.Field | WDESessionType.PhotoStitch,
				def.SessionMode, "SessionMode");
			Assert.AreEqual("DESC", def.Description, "Description");
			Assert.AreEqual("ERROR_MESSAGE", def.ErrorMessage, "ErrorMessage");
			Assert.AreEqual(1, def.Gotos.Count, "Gotos.Count");
			Assert.AreSame(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[1], def.Gotos[0].Control, "Control and ControlDefs[1] are not the same. Expected same.");
			Assert.IsTrue(def.Gotos[0].ClearFields, "ClearFields is false. Expected true.");
			Assert.AreEqual("Document1.Form1.TextBox1.ConditionalGoto.Goto0", def.Gotos[0].Expression, "Expression");
			Assert.IsTrue(def.Gotos[0].Release, "Release is false. Expected true.");
			Assert.AreEqual(Environment.NewLine + "function Document1.Form1.TextBox1.ConditionalGoto.Goto0: Boolean; begin Result := True; end;",
				m_Project.Script, "Script");
		}

		[TestMethod]
		public void DateEdit()
		{
			m_Project.ScriptLanguage = WDEScriptLanguage.DelphiScript;
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field2");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs[0].RecordDef = m_Project.DocumentDefs[0].RecordDefs[0];

			IWDETextBoxDef tb = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");
			tb.Field = m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0];

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;

			IWDEDateEditDef def = WDEDateEditDef.Create();
			tb.EditDefs.Add(def);
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;

			StringReader sr = new StringReader(m_ResMan.GetString("DateEdit"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();

			Assert.IsTrue(def.Enabled, "Enabled is false. Expected true.");
			Assert.IsTrue(def.AllowFutureDates, "AllowFutureDates is false. Expected true.");
			Assert.AreEqual("DESC", def.Description, "Description");
			Assert.AreEqual("ERROR_MESSAGE", def.ErrorMessage, "ErrorMessage");
			Assert.AreEqual(WDEEditErrorType.Failure, def.ErrorType, "ErrorType");
			Assert.AreEqual(WDESessionType.FullForm | WDESessionType.Field | WDESessionType.PhotoStitch,
				def.SessionMode, "SessionMode");
		}

		[TestMethod]
		public void DiagnosisCodes()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field2");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs[0].RecordDef = m_Project.DocumentDefs[0].RecordDefs[0];

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0]);
			obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1]);

			IWDEEditDefs defs = WDEEditDefs.Create(m_Project);
			IWDEDiagnosisPtrEditDef def = WDEDiagnosisPtrEditDef.Create();
			defs.Add(def);
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			
			StringReader sr = new StringReader(m_ResMan.GetString("DiagnosisPtr"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			iproj.Resolver.ResolveLinks();

			Assert.IsTrue(def.Enabled, "Enabled is false. Expected true.");
			Assert.AreEqual(WDEEditErrorType.Ignore, def.ErrorType, "ErrorType");
			Assert.AreEqual(WDESessionType.FullForm | WDESessionType.Field | 
				WDESessionType.PhotoStitch, def.SessionMode, "SessionMode");
			Assert.AreEqual("Test", def.Database, "Database");
			Assert.AreEqual("QUERY", def.Query, "Query");
			Assert.AreEqual("DESC", def.Description, "Description");
			Assert.AreEqual("ERROR_MESSAGE", def.ErrorMessage, "ErrorMessage");

			Assert.AreEqual(8, def.DiagnosisCodes.Count, "Count");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0], def.DiagnosisCodes[0], "FieldDefs[0] and DiagnosisCodes[0] are not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1], def.DiagnosisCodes[1], "FieldDefs[1] and DiagnosisCodes[0] are not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0], def.DiagnosisCodes[2], "FieldDefs[0] and DiagnosisCodes[0] are not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1], def.DiagnosisCodes[3], "FieldDefs[1] and DiagnosisCodes[0] are not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0], def.DiagnosisCodes[4], "FieldDefs[0] and DiagnosisCodes[0] are not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1], def.DiagnosisCodes[5], "FieldDefs[1] and DiagnosisCodes[0] are not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0], def.DiagnosisCodes[6], "FieldDefs[0] and DiagnosisCodes[0] are not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1], def.DiagnosisCodes[7], "FieldDefs[1] and DiagnosisCodes[0] are not the same. Expected same.");
		}

		[TestMethod]
		public void MustEnter()
		{
			m_Project.ScriptLanguage = WDEScriptLanguage.DelphiScript;
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs[0].RecordDef = m_Project.DocumentDefs[0].RecordDefs[0];
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");
			IWDETextBoxDef tb = (IWDETextBoxDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0];
			tb.Field = m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0];
			tb.ControlName = "TextBox1";
			tb.EditDefs.Add(WDERequiredEditDef.Create());
			IWDERequiredEditDef def = (IWDERequiredEditDef) tb.EditDefs[0];
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();

			StringReader sr = new StringReader(m_ResMan.GetString("MustEnter"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();

			Assert.IsTrue(def.Enabled, "Enabled is false. Expected true.");
			Assert.AreEqual(WDEEditErrorType.Failure, def.ErrorType, "ErrorType");
			Assert.AreEqual(WDESessionType.FullForm | WDESessionType.Field | 
				WDESessionType.PhotoStitch, def.SessionMode, "SessionMode");
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) def;
			Assert.AreEqual(obj.GetNamePath(), def.Expression, "Expression");
			Assert.AreEqual(Environment.NewLine + "function Document1.Form1.TextBox1.Required: Boolean; begin Result := EXPR; end;", m_Project.Script, "Script");
			Assert.AreEqual("DESC", def.Description, "Description");
			Assert.AreEqual("ERROR_MESSAGE", def.ErrorMessage, "ErrorMessage");
		}

		[TestMethod]
		public void TableLookup()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field2");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs[0].RecordDef = m_Project.DocumentDefs[0].RecordDefs[0];

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0]);
			obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1]);

			IWDEEditDefs defs = WDEEditDefs.Create(m_Project);
			defs.Add(WDETableLookupEditDef.Create());
			IWDETableLookupEditDef def = (IWDETableLookupEditDef) defs[0];
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;

			StringReader sr = new StringReader(m_ResMan.GetString("TableLookup"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			iproj.Resolver.ResolveLinks();

			Assert.IsTrue(def.Enabled, "Enabled is false. Expected true.");
			Assert.AreEqual("DESC", def.Description, "Description");
			Assert.AreEqual("ERROR_MESSAGE", def.ErrorMessage, "ErrorMessage");
			Assert.AreEqual(WDEEditErrorType.Failure, def.ErrorType, "ErrorType");
			Assert.AreEqual(WDESessionType.FullForm | WDESessionType.Field |
				WDESessionType.PhotoStitch, def.SessionMode, "SessionMode");
			Assert.AreEqual(WDEExecuteOption.Validate, def.ExecuteOn, "ExecuteOn");

			Assert.AreEqual(1, def.TableLookups.Count, "TableLookups.Count");
			Assert.AreEqual("Test", def.TableLookups[0].Database, "Database");
			Assert.AreEqual("SELECT * FROM MCPUSER", def.TableLookups[0].Query, "Query");
			Assert.AreEqual(WDETableLookupOption.FailIfNoRecords | WDETableLookupOption.Filter |
				WDETableLookupOption.LookupIfBlank |
				WDETableLookupOption.OneHitPopup | WDETableLookupOption.ReviewResults |
				WDETableLookupOption.ReviewResults | WDETableLookupOption.SavePos |
				WDETableLookupOption.ShowDiffs | WDETableLookupOption.VerifyPluggedData,
				def.TableLookups[0].Options, "Options");
			Assert.AreEqual(2, def.TableLookups[0].ResultFields.Count, "ResultFields.Count");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0], def.TableLookups[0].ResultFields[0].Field, "Field0 and ResultFields[0] are not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1], def.TableLookups[0].ResultFields[1].Field, "Field1 and ResultFields[1] are not the same. Expected same.");
			Assert.AreEqual(WDELookupResultFieldOption.AllowFilter | WDELookupResultFieldOption.Display,
				def.TableLookups[0].ResultFields[0].Options, "Options0");
			Assert.AreEqual(WDELookupResultFieldOption.AllowFilter | WDELookupResultFieldOption.Display,
				def.TableLookups[0].ResultFields[1].Options, "Options1");
		}

		[TestMethod]
		public void Validate()
		{
			m_Project.ScriptLanguage = WDEScriptLanguage.DelphiScript;
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs[0].RecordDef = m_Project.DocumentDefs[0].RecordDefs[0];
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");
			IWDETextBoxDef tb = (IWDETextBoxDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0];
			tb.Field = m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0];
			tb.ControlName = "TextBox1";
			tb.EditDefs.Add(WDEValidateEditDef.Create());
			IWDEValidateEditDef def = (IWDEValidateEditDef) tb.EditDefs[0];
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();

			StringReader sr = new StringReader(m_ResMan.GetString("Validate"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();

			Assert.IsTrue(def.Enabled, "Enabled is false. Expected true.");
			Assert.AreEqual(WDEEditErrorType.Failure, def.ErrorType, "ErrorType");
			Assert.AreEqual(WDESessionType.FullForm | WDESessionType.Field | 
				WDESessionType.PhotoStitch, def.SessionMode, "SessionMode");
			Assert.AreEqual("DESC", def.Description, "Description");
			Assert.AreEqual("ERROR_MESSAGE", def.ErrorMessage, "ErrorMessage");
			
			Assert.AreEqual(1, def.Validations.Count, "Count");
			Assert.AreEqual("ERROR_MESSAGE", def.Validations[0].ErrorMessage, "ErrorMessage[0]");
			Assert.AreEqual(Environment.NewLine + "function Document1.Form1.TextBox1.Validate.Validation0: Boolean; begin Result := EXPR; end;", m_Project.Script, "Script");
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) def.Validations[0];
			Assert.AreEqual(obj.GetNamePath(), def.Validations[0].Expression, "Expression");
		}

		[TestMethod]
		public void TextBox()
		{
			m_Project.ScriptLanguage = WDEScriptLanguage.DelphiScript;
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs[0].RecordDef = m_Project.DocumentDefs[0].RecordDefs[0];

			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");
			IWDETextBoxDef def = (IWDETextBoxDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0];
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			IWDELabelDef ldef = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddLabel("Label1");

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0]);
			obj = (WDEBaseCollectionItem) ldef;
			iproj.Resolver.AddObject(obj.GetNamePath(), ldef);

			StringReader sr = new StringReader(m_ResMan.GetString("TextBox"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			try
			{
				Assert.IsTrue(XmlReader.EOF, "EOF is false. " + XmlReader.Name);
			}
			finally
			{
				XmlReader.Close();
			}
			iproj.Resolver.ResolveLinks();

			Assert.AreEqual("A-Z", def.CharSet, "CharSet");
			Assert.AreEqual("TextBox1", def.ControlName, "ControlName");
			Assert.AreEqual(WDEControlOption.AllowFlag | WDEControlOption.AutoAdvance |
				WDEControlOption.EntryModeInVerify | WDEControlOption.ExcludeFromFieldRepair | 
				WDEControlOption.FieldVerify | WDEControlOption.ShowEntireImage |
				WDEControlOption.ShowOverlay | WDEControlOption.ShowSnippetImage | 
				WDEControlOption.SkipQIVerify | WDEControlOption.SkipVerify, def.Options);
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0], def.Field, "Field and FieldDefs[0] are not the same. Expected same.");
			Assert.AreEqual("{A-Z}&&&&&&&&&&&&&&&&&&&", def.InputMask, "InputMask");
			Assert.AreEqual(1, def.KeyOrder, "KeyOrder");
			Assert.IsTrue(def.TabStop, "TabStop is false. Expected true.");
			Assert.AreEqual("DESC", def.Description, "Description");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.Window), def.BackColor, "BackColor");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.WindowText), def.ForeColor, "ForeColor");
			Assert.AreEqual("Courier New", def.ControlFont.Name, "Font");
			Assert.AreEqual(23, def.Location.Height, "Height");
			Assert.AreEqual("Help", def.Help, "Help");
			Assert.AreEqual("Hint", def.Hint, "Hint");
			Assert.AreEqual(1, def.LabelLinks.Count, "LabelLinks.Count");
			Assert.AreSame(ldef, def.LabelLinks[0], "LabelLinks[0]");
			Assert.AreEqual(67, def.Location.Left, "Left");
			Assert.AreEqual(18, def.Location.Top, "Top");
			Assert.AreEqual(226, def.Location.Width, "Width");
			Assert.AreEqual(6, def.EditDefs.Count, "EditDefs.Count");
			Assert.IsTrue(def.OnValidate.Enabled, "OnValidate.Enabled is false. Expected true.");
			Assert.IsFalse(def.OnEnter.Enabled, "OnEnter.Enabled is true. Expected false.");
			Assert.IsFalse(def.OnExit.Enabled, "OnExit.Enabled is true. Expected false.");
			Assert.AreEqual(m_ResMan.GetString("TextBoxOnValidate"), m_Project.Script, "Project.Script");
			Assert.AreEqual("Document1.Form1.TextBox1.OnValidate", def.OnValidate.ScriptFullName, "ScriptFullName");
		}

		[TestMethod]
		public void Accumulator()
		{
			m_Project.ScriptLanguage = WDEScriptLanguage.DelphiScript;
			IWDEDocumentDef doc = m_Project.DocumentDefs.Add("Document1");
			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();

			IWDEXmlPersist ipers = (IWDEXmlPersist) doc;
			StringReader sr = new StringReader(m_ResMan.GetString("Accum"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			try
			{
				Assert.IsTrue(XmlReader.EOF, "EOF is false. " + XmlReader.Name);
			}
			finally
			{
				XmlReader.Close();
			}
			iproj.Resolver.ResolveLinks();

			IWDETextBoxDef tb = (IWDETextBoxDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0];
			IWDEBalanceCheckEditDef bc = null;
			foreach(IWDEEditDef def in tb.EditDefs)
			{
				if(def is IWDEBalanceCheckEditDef)
				{
					bc = (IWDEBalanceCheckEditDef) def;
					break;
				}
			}
			Assert.IsNotNull(bc, "bc is null. Expected not null.");
			Assert.AreEqual(1, bc.SumFields.Count, "bc.Count");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0], bc.SumFields[0], "FieldDefs[0] and SumFields[0] are not the same. Expected same.");
			Assert.IsTrue(bc.Enabled, "Enabled is false. Expected true.");
			Assert.AreEqual(WDEEditErrorType.Failure, bc.ErrorType, "ErrorType");
			Assert.AreEqual("DESC", bc.Description, "Description");
			Assert.AreEqual("ERROR_MESSAGE", bc.ErrorMessage, "ErrorMessage");
			Assert.AreEqual(WDESessionType.FullForm | WDESessionType.Field |
				WDESessionType.PhotoStitch, bc.SessionMode, "SessionMode");
		}

		[TestMethod]
		public void ZoneLabelLinks()
		{
			m_Project.ScriptLanguage = WDEScriptLanguage.DelphiScript;
			IWDEDocumentDef doc = m_Project.DocumentDefs.Add("Document1");
			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();

			IWDEXmlPersist ipers = (IWDEXmlPersist) doc;
			StringReader sr = new StringReader(m_ResMan.GetString("ZoneLabelLinks"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			try
			{
				Assert.IsTrue(XmlReader.EOF, "EOF is false. " + XmlReader.Name);
			}
			finally
			{
				XmlReader.Close();
			}
			iproj.Resolver.ResolveLinks();
			
			IWDETextBoxDef tb = (IWDETextBoxDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0];
			Assert.AreSame(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[1], tb.LabelLinks[0], "LabelLinks and ControlDefs[1] are not the same. Expected same.");
			Assert.AreEqual(2, tb.ZoneLinks.Count, "Count");
			Assert.AreSame(m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs[1], tb.ZoneLinks[0], "ZoneDefs[1] and ZoneLinks[0] are not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].ImageSourceDefs[0].SnippetDefs[0], tb.ZoneLinks[1], "SnippetDefs[0] and ZoneLinks[1] are not the same. Expected same.");
		}

		[TestMethod]
		public void GroupBoxDef()
		{
			m_Project.ScriptLanguage = WDEScriptLanguage.DelphiScript;
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs[0].RecordDef = m_Project.DocumentDefs[0].RecordDefs[0];

			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddGroupBox("GroupBox1");
			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			
			IWDEGroupBoxDef def = (IWDEGroupBoxDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0];
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;

			StringReader sr = new StringReader(m_ResMan.GetString("GroupBox"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			try
			{
				Assert.IsTrue(XmlReader.EOF, "EOF is false. " + XmlReader.Name);
			}
			finally
			{
				XmlReader.Close();
			}

			Assert.AreEqual("GroupBox1", def.ControlName, "ControlName");
			Assert.AreEqual(WDEGroupBoxOption.ClearDuringRepair | 
				WDEGroupBoxOption.RepairEntireGroup, def.Options, "Options");
			Assert.AreEqual("DESC", def.Description, "Description");
			Assert.AreEqual("Caption", def.Caption, "Caption");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.Control), def.BackColor, "BackColor");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.WindowText), def.ForeColor, "ForeColor");
			Assert.AreEqual("Tahoma", def.ControlFont.Name, "Font");
			Assert.AreEqual("Help", def.Help, "Help");
			Assert.AreEqual("Hint", def.Hint, "Hint");
			Assert.AreEqual(new Rectangle(416, 25, 515, 223), def.Location, "Location");
		}

		[TestMethod]
		public void DetailGrid()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs.Add("Record2");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("Field3");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("Field4");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs[0].RecordDef = m_Project.DocumentDefs[0].RecordDefs[0];

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0]);
			obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs[0]);
			obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs[1];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs[1]);
			iproj.ConvertOldFormat = true;

			IWDEDetailGridDef def = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddDetailGrid("DetailGrid1");
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;

			StringReader sr = new StringReader(m_ResMan.GetString("DetailGrid"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			try
			{
				Assert.IsTrue(XmlReader.EOF, "EOF is false. " + XmlReader.Name);
			}
			finally
			{
				XmlReader.Close();
			}
			iproj.Resolver.ResolveLinks();

			Assert.AreEqual("DetailGrid1", def.ControlName, "ControlName");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0], def.RecordDef, "RecordDef and RecordDefs[0] are not the same. Expected same.");
			Assert.AreEqual(3, def.KeyOrder, "KeyOrder");
			Assert.AreEqual(WDEDetailGridOption.RestrictExit, def.Options, "Options");
			Assert.AreEqual(6, def.Rows, "Rows");
			Assert.AreEqual(WDERecordNumberPosition.Right, def.RecordNumberPosition, "RecordNumberPosition");
			Assert.IsTrue(def.TabStop, "TabStop is false. Expected true.");
			Assert.AreEqual("DESC", def.Description, "Description");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.Control), def.BackColor, "BackColor");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.WindowText), def.ForeColor, "ForeColor");
			Assert.AreEqual("Courier New", def.ControlFont.Name, "ControlFont");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.Desktop), def.HeaderBackColor, "HeaderBackColor");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.Window), def.HeaderForeColor, "HeaderForeColor");
			Assert.AreEqual("Microsoft Sans Serif", def.HeaderFont.Name, "HeaderFont");
			Assert.AreEqual(30, def.HeaderHeight, "HeaderHeight");
			Assert.AreEqual("Help", def.Help, "Help");
			Assert.AreEqual("Hint", def.Hint, "Hint");
			Assert.AreEqual(new Rectangle(102, 257, 497, 273), def.Location, "Location");
			Assert.AreEqual(2, def.HeaderControlDefs.Count, "HeaderControlDefs.Count");
            Assert.IsInstanceOfType(def.HeaderControlDefs[0], typeof(WDELabelDef), "HeaderControlDef1");
            Assert.IsInstanceOfType(def.HeaderControlDefs[1], typeof(WDELabelDef), "HeaderControlDef2");
			Assert.AreEqual(2, def.ControlDefs.Count, "ControlDefs.Count");
            Assert.IsInstanceOfType(def.ControlDefs[0], typeof(WDETextBoxDef), "ControlDef1");
            Assert.IsInstanceOfType(def.ControlDefs[1], typeof(WDETextBoxDef), "ControlDef2");
		}

		[TestMethod]
		public void RejectCodes()
		{
			IWDERejectCodes codes = m_Project.RejectCodes;
			IWDEXmlPersist ipers = (IWDEXmlPersist) codes;

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;

			StringReader sr = new StringReader(m_ResMan.GetString("RejectCodes"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			try
			{
				Assert.IsTrue(XmlReader.EOF, "EOF is false. " + XmlReader.Name);
			}
			finally
			{
				XmlReader.Close();
			}

			Assert.AreEqual(2, codes.Count, "Count");
			Assert.AreEqual("01", codes[0].Name, "Name[0]");
			Assert.IsFalse(codes[0].RequireDescription, "RequireDescription[0]");
			Assert.AreEqual("BadScan", codes[0].Description, "Description[0]");

			Assert.AreEqual("02", codes[1].Name, "Name[1]");
			Assert.IsTrue(codes[1].RequireDescription, "RequireDescription[1]");
			Assert.AreEqual("Other", codes[1].Description, "Description[1]");
		}

		[TestMethod]
		public void ViewDefs()
		{
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs[0].RecordDef = m_Project.DocumentDefs[0].RecordDefs[0];
			m_Project.RejectCodes.Add("01");
			m_Project.RejectCodes.Add("02");

			IWDESessionDefs defs = m_Project.SessionDefs;
			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();

			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].FormDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].FormDefs[0]);
			iproj.Resolver.AddObject("01", m_Project.RejectCodes[0]);
			iproj.Resolver.AddObject("02", m_Project.RejectCodes[1]);

			StringReader sr = new StringReader(m_ResMan.GetString("ViewDefs"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			try
			{
				Assert.IsTrue(XmlReader.EOF, "EOF is false. " + XmlReader.Name);
			}
			finally
			{
				XmlReader.Close();
			}

			iproj.Resolver.ResolveLinks();

			Assert.AreEqual(1, defs.Count, "Count");
			Assert.AreEqual("DefaultView", defs[0].SessionDefName, "Name");
			Assert.AreEqual(10, defs[0].DataPanelHeight, "DataPanelHeight");
			Assert.AreEqual(15, defs[0].FirstImage, "FirstImage");
			Assert.AreEqual(WDESessionOption.AllowBatchReject | WDESessionOption.AllowDocReject |
				WDESessionOption.AllowFieldVerify | WDESessionOption.AllowFloatingWindow |
				WDESessionOption.AllowImageTypeChange | WDESessionOption.AllowModifyNonKeyables |
				WDESessionOption.AutoSave | WDESessionOption.ReviewAllImages |
				WDESessionOption.ShowOverlay | WDESessionOption.ShowWorkZones,
				defs[0].Options, "Options");
			Assert.IsTrue(defs[0].ShowTicker, "ShowTicker is false. Expected true.");
			Assert.AreEqual(50, defs[0].TickerCharHeight, "TickerCharHeight");
			Assert.AreEqual(WDEImageScale.FitBest, defs[0].ImageScale, "ImageScale");
			Assert.AreEqual(50, defs[0].ImageScalePercent, "ImageScalePercent");
			Assert.AreEqual(WDESessionType.OnePass, defs[0].SessionType, "SessionType");
			Assert.AreEqual(WDESessionStyle.Vertical, defs[0].SessionStyle, "SessionStyle");
			
			Assert.AreEqual(1, defs[0].Forms.Count, "Forms.Count");
			Assert.AreSame(m_Project.DocumentDefs[0].FormDefs[0], defs[0].Forms[0], "Forms[0] and FormDefs[0] are not the same. Expected same.");
			Assert.AreEqual(3, defs[0].PhotoStitch.ColCount, "ColCount");
			Assert.AreEqual(3, defs[0].PhotoStitch.RowCount, "RowCount");
			Assert.AreEqual(WDEPSOrientation.Vertical, defs[0].PhotoStitch.Orientation, "Orientation");
			
			Assert.AreEqual(2, defs[0].AllowedRejectCodes.Count, "AllowedRejectCodes.Count");
			Assert.AreSame(m_Project.RejectCodes[0], defs[0].AllowedRejectCodes[0], "RejectCodes[0] and AllowedRejectCodes[0] are not the same. Expected same.");
			Assert.AreSame(m_Project.RejectCodes[1], defs[0].AllowedRejectCodes[1], "RejectCodes[1] and AllowedRejectCodes[1] are not the same. Expected same.");
		}

		[TestMethod]
		public void DEGridDef()
		{
			m_Project.ScriptLanguage = WDEScriptLanguage.DelphiScript;
			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.ConvertOldFormat = true;
			iproj.Resolver = new LinkResolver();

			m_Project.DocumentDefs.Add("HCFA");
			m_Project.DocumentDefs[0].RecordDefs.Add("Header");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs.Add("Detail");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("od_from_date");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("od_to_date");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("od_place_ser");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("od_tos");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("od_code");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("od_mod");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("od_dcode");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("od_charges");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("od_days");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("od_epsdt");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("od_emg");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("od_cob");
			m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs.Add("od_reserved");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("od_diag1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("od_diag2");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("od_diag3");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("od_diag4");

			foreach(WDEBaseCollectionItem obj in m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs)
			{
				iproj.Resolver.AddObject(obj.GetNamePath(), obj);
			}

			foreach(WDEBaseCollectionItem obj in m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs)
			{
				iproj.Resolver.AddObject(obj.GetNamePath(), obj);
			}

			iproj.Resolver.AddObject("HCFA.Header.Detail", m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0]);

			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			IWDEDetailGridDef def = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddDetailGrid("DetailGrid1");
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			
			StringReader sr = new StringReader(m_ResMan.GetString("DEGridDef"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			Assert.IsTrue(XmlReader.EOF, "Eof is not true: " + XmlReader.Name);
			XmlReader.Close();
			iproj.Resolver.ResolveLinks();

			Assert.AreEqual("DEGrid1", def.ControlName, "ControlName");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0], def.RecordDef, "RecordDef and Detail are not the same. Expected same.");
			Assert.AreEqual(13, def.KeyOrder, "KeyOrder");
			Assert.IsFalse(def.TabStop, "TabStop is true. Expected false.");

			Assert.AreEqual(Color.FromKnownColor(KnownColor.Control), def.BackColor, "BackColor");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.WindowText), def.ForeColor, "ForeColor");
			Assert.AreEqual("Courier New", def.ControlFont.Name, "ControlFont");
			Assert.AreEqual(new Rectangle(4, 952, 1227, 255), def.Location, "Location");
			Assert.AreEqual("Tahoma", def.HeaderFont.Name, "HeaderFont");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.WindowText), def.HeaderForeColor, "HeaderForeColor");
			Assert.AreEqual(13, def.ControlDefs.Count, "ControlDefs.Count");
			Assert.AreEqual(13, def.HeaderControlDefs.Count, "HeaderControlDefs.Count");
			Assert.AreEqual(10, def.Rows, "Rows");
			IWDELabelDef ldef = (IWDELabelDef) def.HeaderControlDefs[0];
			Assert.AreEqual("From Date", ldef.Caption, "ldef.Caption");
			Assert.AreEqual(104, def.ControlDefs[1].Location.Left, "Left");
		}

		[TestMethod]
		public void ProjectMultiScriptConvert()
		{
			StringReader sr = new StringReader(m_ResMan.GetString("ProjectMultiScriptConvert"));
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
			sw.Write(sr.ReadToEnd());
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			m_Project.LoadFromStream(ms);
			sr.Close();
			sw.Close();

			Assert.AreEqual(m_ResMan.GetString("ProjectMultiScriptResult"), m_Project.Script, "Script");
		}

		[TestMethod]
		public void DetailZoneNormal()
		{
			StringReader sr = new StringReader(m_ResMan.GetString("DetailZoneNormal"));
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms);
			sw.Write(sr.ReadToEnd());
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			m_Project.LoadFromStream(ms);
			sw.Close();
			sr.Close();

			IWDEDetailGridDef def = (IWDEDetailGridDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[2];
			Assert.IsNotNull(def.DetailZoneDef, "DetailZoneDef is null. Expected not null.");
			Assert.AreEqual(1, def.DetailZoneDef.ZoneDefs.Count, "ZoneDefs.Count");
			Assert.AreEqual(new Rectangle(228, 775, 206, 63), def.DetailZoneDef.ZoneDefs[0].ZoneRect, "ZoneRect");
            IWDETextBoxDef tbdef = (IWDETextBoxDef)def.ControlDefs[0];
            Console.WriteLine(tbdef.ZoneLinks[0].Name);
		}

		[TestMethod]
		public void DetailZoneNone()
		{
			StringReader sr = new StringReader(m_ResMan.GetString("DetailZoneNone"));
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms);
			sw.Write(sr.ReadToEnd());
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			m_Project.LoadFromStream(ms);
			sw.Close();
			sr.Close();

			IWDEDetailGridDef def = (IWDEDetailGridDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[2];
			Assert.IsNull(def.DetailZoneDef, "DetailZoneDef is not null. Expected null.");	
		}

		[TestMethod]
		public void DetailZoneNoSubs()
		{
			StringReader sr = new StringReader(m_ResMan.GetString("DetailZoneNoSubs"));
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms);
			sw.Write(sr.ReadToEnd());
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			m_Project.LoadFromStream(ms);
			sw.Close();
			sr.Close();

			IWDEDetailGridDef def = (IWDEDetailGridDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[2];
			Assert.IsNotNull(def.DetailZoneDef, "DetailZoneDef is null. Expected not null.");
			Assert.AreEqual(1, def.DetailZoneDef.ZoneDefs.Count, "Count");
			Assert.AreEqual("{X=317,Y=339,Width=1063,Height=63}", def.DetailZoneDef.ZoneDefs[0].ZoneRect.ToString(), "ZoneRect");
		}

		[TestMethod]
		public void DetailZoneEmpty()
		{
			StringReader sr = new StringReader(m_ResMan.GetString("DetailZoneEmpty"));
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms);
			sw.Write(sr.ReadToEnd());
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			m_Project.LoadFromStream(ms);
			sw.Close();
			sr.Close();

			IWDEDetailGridDef def = (IWDEDetailGridDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[2];
			Assert.IsNotNull(def.DetailZoneDef, "DetailZoneDef is null. Expected not null.");
			Assert.AreEqual(1, def.DetailZoneDef.ZoneDefs.Count, "ZoneDefs.Count");
			Assert.AreEqual(new Rectangle(196, 698, 1310, 91), def.DetailZoneDef.ZoneDefs[0].ZoneRect, "ZoneDefs[0].ZoneRect");
		}

		[TestMethod]
		public void DetailZoneRegularZone()
		{
			StringReader sr = new StringReader(m_ResMan.GetString("DetailZoneRegularZone"));
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms);
			sw.Write(sr.ReadToEnd());
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			m_Project.LoadFromStream(ms);
			sw.Close();
			sr.Close();

			IWDEDetailGridDef def = (IWDEDetailGridDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[2];
			Assert.IsNotNull(def.DetailZoneDef, "DetailZoneDef is null. Expected not null.");
			Assert.AreEqual(1, def.DetailZoneDef.ZoneDefs.Count, "ZoneDefs.Count");
			Assert.AreEqual(new Rectangle(277, 310, 756, 465), def.DetailZoneDef.ZoneDefs[0].ZoneRect, "ZoneRect");
			Assert.AreEqual(1, def.DetailZoneDef.LineCount, "LineCount");
			Assert.AreEqual(465, def.DetailZoneDef.LineHeight, "LineHeight");
		}

		[TestMethod]
		public void GroupBoxFlattened()
		{
			StringReader sr = new StringReader(m_ResMan.GetString("GroupBoxFlattened"));
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms);
			sw.Write(sr.ReadToEnd());
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			m_Project.LoadFromStream(ms);
			sw.Close();
			sr.Close();

			int index = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.Find("TextBox1");
			Assert.AreEqual(0, m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[index].KeyOrder, "TextBox1 keyorder");
			index = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.Find("TextBox2");
			IWDETextBoxDef tb2 = (IWDETextBoxDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[index];
			Assert.AreEqual(1, m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[index].KeyOrder, "TextBox2 keyorder");
			index = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.Find("TextBox3");
			IWDETextBoxDef tb3 = (IWDETextBoxDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[index];
			Assert.AreEqual(2, m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[index].KeyOrder, "TextBox3 keyorder");

			index = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.Find("Label2");
			IWDELabelDef lb2 = (IWDELabelDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[index];
			index = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.Find("Label3");
			IWDELabelDef lb3 = (IWDELabelDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[index];

			index = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.Find("GroupBox1");
			IWDEGroupBoxDef def = (IWDEGroupBoxDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[index];
			Assert.AreEqual(4, def.ControlDefs.Count);
			Assert.AreSame(tb2, def.ControlDefs[0]);
			Assert.AreSame(lb2, def.ControlDefs[1]);
			Assert.AreSame(tb3, def.ControlDefs[2]);
			Assert.AreSame(lb3, def.ControlDefs[3]);
			Assert.AreEqual("{X=119,Y=148,Width=182,Height=23}", tb2.Location.ToString());
		}

		[TestMethod]
		public void DEGridHeaderHeight()
		{
			IWDEProject proj = WDEProject.Create();
			IWDEProjectInternal iproj = (IWDEProjectInternal) proj;
			iproj.Resolver = new LinkResolver();
			iproj.ConvertOldFormat = true;
			proj.DocumentDefs.Add();
			proj.DocumentDefs[0].FormDefs.Add();
			IWDEControlDefs defs = proj.DocumentDefs[0].FormDefs[0].ControlDefs;

			IWDEDetailGridDef def = defs.AddDetailGrid();
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			StringReader sr = new StringReader(m_ResMan.GetString("DEGridHeaderHeight"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();

			Assert.AreEqual(19, def.HeaderHeight);
		}

		[TestMethod]
		public void DEGridTextBoxKeyOrder()
		{
			IWDEProject proj = WDEProject.Create();
			IWDEProjectInternal iproj = (IWDEProjectInternal) proj;
			iproj.Resolver = new LinkResolver();
			iproj.ConvertOldFormat = true;
			proj.DocumentDefs.Add();
			proj.DocumentDefs[0].FormDefs.Add();
			IWDEControlDefs defs = proj.DocumentDefs[0].FormDefs[0].ControlDefs;

			IWDEDetailGridDef def = defs.AddDetailGrid();
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			StringReader sr = new StringReader(m_ResMan.GetString("DEGridTextBoxKeyOrder"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();

			Assert.AreEqual(2, def.ControlDefs.Count, "Count");
			Assert.AreEqual(0, def.ControlDefs[0].KeyOrder, "KeyOrder1");
			Assert.AreEqual(1, def.ControlDefs[1].KeyOrder, "KeyOrder2");
		}

		[TestMethod]
		public void GetZonesOnlyZones()
		{
			m_Project.DocumentDefs.Add();
			m_Project.DocumentDefs[0].FormDefs.Add();
			m_Project.DocumentDefs[0].ImageSourceDefs.Add();
			m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs.Add();
			m_Project.DocumentDefs[0].ImageSourceDefs[0].SnippetDefs.Add();
			IWDETextBoxDef def = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
			
			def.ZoneLinks.Add(m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs[0]);
			def.ZoneLinks.Add(m_Project.DocumentDefs[0].ImageSourceDefs[0].SnippetDefs[0]);
			ArrayList al = def.GetZonesByImageType("ImageType1");
			Assert.AreEqual(1, al.Count, "Count");
			Assert.AreSame(m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs[0], al[0], "al[0] is not the same as ZoneDefs[0]. Expected same.");
		}

		[TestMethod]
		public void DuplicateRecForm()
		{
			StringReader sr = new StringReader(m_ResMan.GetString("DuplicateRecForm"));
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms);
			sw.Write(sr.ReadToEnd());
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			m_Project.LoadFromStream(ms);
			sw.Close();
			sr.Close();

			Assert.AreEqual("DataForm", m_Project.DocumentDefs[0].RecordDefs[0].RecType, "RecType");
			Assert.AreEqual("DataForm_1", m_Project.DocumentDefs[0].FormDefs[0].FormName, "FormName");
			Assert.AreSame(m_Project.SessionDefs[0].Forms[0], m_Project.DocumentDefs[0].FormDefs[0], "Forms[0] and FormDefs[0] are not the same. Expected same.");
		}

		[TestMethod]
		public void Databases()
		{
			StringReader sr = new StringReader(m_ResMan.GetString("Databases"));
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms);
			sw.Write(sr.ReadToEnd());
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			m_Project.LoadFromStream(ms);
			sw.Close();
			sr.Close();

			Assert.AreEqual(2, m_Project.Databases.Count, "Count");
			Assert.AreEqual("ConnStr1", m_Project.Databases["Database1"].ConnectionString, "Database1");
			Assert.AreEqual("ConnStr2", m_Project.Databases["Database2"].ConnectionString, "Database2");
		}
	}
}
