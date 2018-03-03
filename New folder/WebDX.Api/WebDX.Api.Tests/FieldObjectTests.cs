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
	[TestClass]
	public class FieldObjectTests
	{
		ResourceManager m_ResMan;

        //MockObject m_DataSet;
        //MockObject m_Document;
        //MockObject m_Record;
        //MockObject m_Fields;
        //MockObject m_Sessions;
        //MockObject m_Session;
        //MockObject m_FieldDef;
        //MockObject m_EditDefs;
        //MockObject m_OnValidate;
        //MockObject m_RecordDef;
        //MockObject m_FieldDefs;
        //MockObject m_Project;

        IWDEDataSet m_DataSet;
        IWDEDocument m_Document;
        IWDERecord m_Record;
        IWDEFields m_Fields;
        IWDESessions m_Sessions;
        IWDESession_R1 m_Session;
        IWDEFieldDef m_FieldDef;
        IWDEEditDefs m_EditDefs;
        IWDEEventScriptDef m_OnValidate;
        IWDERecordDef m_RecordDef;
        IWDEFieldDefs m_FieldDefs;
        IWDEProject m_Project;
        IWDEField_R1 m_Field;
		IWDEFieldClient m_FieldClient;

		public FieldObjectTests()
		{
		}

		[TestInitialize]
		public void FieldObjectSetup()
		{
			//MockManager.Init();

			m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());

			//m_DataSet = MockManager.MockObject(typeof(IWDEDataSet));
			//m_Document = MockManager.MockObject(typeof(IWDEDocument));
			//m_Record = MockManager.MockObject(typeof(IWDERecord));
			//m_Fields = MockManager.MockObject(typeof(IWDEFields));
			//m_Sessions = MockManager.MockObject(typeof(IWDESessions));
			//m_Session = MockManager.MockObject(typeof(IWDESession_R1));
			//m_FieldDef = MockManager.MockObject(typeof(IWDEFieldDef));
			//m_EditDefs = MockManager.MockObject(typeof(IWDEEditDefs));
			//m_OnValidate = MockManager.MockObject(typeof(IWDEEventScriptDef));
			//m_RecordDef = MockManager.MockObject(typeof(IWDERecordDef));
			//m_FieldDefs = MockManager.MockObject(typeof(IWDEFieldDefs));
			//m_Project = MockManager.MockObject(typeof(IWDEProject));

			//m_OnValidate.ExpectGetAlways("Enabled", false);

			//m_Fields.ExpectGetAlways("DataRecord", m_Record.Object);
			//m_Record.ExpectGetAlways("Document", m_Document.Object);
			//m_Document.ExpectGetAlways("DataSet", m_DataSet.Object);
			//m_DataSet.ExpectGetAlways("Sessions", m_Sessions.Object);
			//m_Sessions.ExpectGetAlways("Item", m_Session.Object);
			//m_Document.ExpectGetAlways("Status", WDESessionStatus.None);
			//m_DataSet.ExpectGetAlways("Project", null);

			//m_RecordDef.ExpectGetAlways("FieldDefs", m_FieldDefs.Object);
			//m_FieldDefs.ExpectGetAlways("Item", m_FieldDef.Object);
			//m_FieldDefs.AlwaysReturn("Find", 0, null);

			//m_Session.ExpectGetAlways("SessionID", 1);
			//m_Session.ExpectGetAlways("Mode", WDEOpenMode.Edit);
			//m_FieldDef.ExpectGetAlways("Options", WDEFieldOption.None);
			//m_FieldDef.ExpectGetAlways("CharSet", "");
			//m_FieldDef.ExpectGetAlways("FieldName", "Field1");
			//m_FieldDef.ExpectGetAlways("EditDefs", m_EditDefs.Object);
			//m_FieldDef.ExpectGetAlways("OnValidate", m_OnValidate.Object);
			//m_FieldDef.ExpectGetAlways("DataLen", 255);

			//m_EditDefs.ExpectGetAlways("Count", 0);

			m_Field = WDEField.Create((IWDEFields) m_Fields);
			IWDEFieldInternal ifield = m_Field as IWDEFieldInternal;
			ifield.FieldDef = (IWDEFieldDef) m_FieldDef;
			m_FieldClient = m_Field as IWDEFieldClient;
		}

		[TestCleanup]
		public void Cleanup()
		{
			m_DataSet = null;
			m_Document = null;
			m_Record = null;
			m_Fields = null;
			m_Sessions = null;
			m_Session = null;
			m_FieldDef = null;
			m_EditDefs = null;
			m_OnValidate = null;
			m_Field = null;
			m_FieldClient = null;
			m_RecordDef = null;
			m_FieldDefs = null;
			m_Project = null;

			//MockManager.Verify();
			GC.Collect();
		}

		[TestMethod]
		public void SetValueAndStatus()
		{
			m_FieldClient.SetValueAndStatus("NEWVALUE", WDEFieldStatus.Keyed, 10, 20);
			Assert.AreEqual("NEWVALUE", m_Field.Value, "Value");
			Assert.AreEqual(WDEFieldStatus.Keyed, m_Field.Status, "Status");
			Assert.AreEqual(1, m_Field.Revisions.Count, "Revisions.Count");
			Assert.AreEqual(10, m_Field.Revisions[0].CharCount, "CharCount");
			Assert.AreEqual(20, m_Field.Revisions[0].VCECount, "VCECount");
		}

		[TestMethod]
		public void CountsIgnore()
		{			
			m_FieldClient.SetValueAndStatus("TEST", WDEFieldStatus.Keyed, 10, 20);
			m_FieldClient.SetValueAndStatus(m_Field.Value, m_Field.Status, 30, 40);

			Assert.AreEqual(1, m_Field.Revisions.Count, "Revisions.Count");
			Assert.AreEqual(10, m_Field.Revisions[0].CharCount, "CharCount");
			Assert.AreEqual(20, m_Field.Revisions[0].VCECount, "VCECount");
		}

		[TestMethod]
		public void CreateRevision()
		{
			m_FieldClient.SetValueAndStatus("TEST1", WDEFieldStatus.Keyed, 0, 0);
			Assert.AreEqual(1, m_Field.Revisions.Count, "Count (1)");
			m_FieldClient.SetValueAndStatus(m_Field.Value, m_Field.Status, 0, 0);
			Assert.AreEqual(1, m_Field.Revisions.Count, "Count after setting same value and status", new object[] {m_Field.Revisions.Count});

			m_FieldClient.SetValueAndStatus("TEST2", m_Field.Status, 0, 0);
			Assert.AreEqual(2, m_Field.Revisions.Count, "Count (2)");
			Assert.AreEqual("TEST2", m_Field.Value);

			m_FieldClient.SetValueAndStatus(m_Field.Value, WDEFieldStatus.Verified, 0, 0);
			Assert.AreEqual(3, m_Field.Revisions.Count, "Count (3)");
			Assert.AreEqual(WDEFieldStatus.Verified, m_Field.Status);

			//m_Session.ExpectGetAlways("SessionID", 2);
			//m_Sessions.ExpectGetAlways("Item", m_Session.Object);
			m_FieldClient.SetValueAndStatus(m_Field.Value, m_Field.Status, 0, 0);
			Assert.AreEqual(4, m_Field.Revisions.Count, "Count (4)");
		}

		[TestMethod]
		public void SessionID()
		{
			m_FieldClient.SetValueAndStatus("TEST", WDEFieldStatus.Keyed, 0, 0);
			IWDERevisionInternal irev = (IWDERevisionInternal) m_Field.Revisions[0];
			irev.SessionID = 2;
			Assert.AreEqual(1, m_Field.SessionID, "SessionID");
		}

		[TestMethod]
		public void ExtractGoodChars()
		{
			//const string testValue = "AB23C5D";            

   //         m_FieldDef.ExpectGetAlways("CharSet", "A-Z");
			//m_FieldDef.ExpectGetAlways("OCRCharSet", "0-9");
			
			//Assert.AreEqual("ABCD", m_Field.ExtractGoodChars(testValue), "ABCD");

			//m_Session.ExpectGetAlways("Mode", WDEOpenMode.CharRepair);
			//Assert.AreEqual("235", m_Field.ExtractGoodChars(testValue));

			//m_FieldDef.ExpectGetAlways("CharSet", "");
			//m_FieldDef.ExpectGetAlways("OCRCharSet", "");
			//Assert.AreEqual(testValue, m_Field.ExtractGoodChars(testValue), "testValue");

			//Assert.IsNull(m_Field.ExtractGoodChars(null), "ExtractGoodChars returned not null, expected null");
			//m_FieldDef.ExpectGetAlways("CharSet", "A-Z");
			//m_Session.ExpectGetAlways("Mode", WDEOpenMode.Edit);
			Assert.AreEqual("", m_Field.ExtractGoodChars(""), "blank");
		}

		[TestMethod]
		public void IsNumeric()
		{
			Assert.IsTrue(m_Field.IsNull, "Field is not null, expected null");
			Assert.IsFalse(m_Field.IsNumeric, "IsNumeric is true (null), expected false");

			m_FieldClient.SetValueAndStatus("1234567890", WDEFieldStatus.Plugged, 0, 0);
			Assert.IsTrue(m_Field.IsNumeric, "IsNumeric is false, expected true");

			m_FieldClient.SetValueAndStatus("1234567890A", WDEFieldStatus.Plugged, 0, 0);
			Assert.IsFalse(m_Field.IsNumeric, "IsNumeric is true, expected false");
		}

		[TestMethod]
		public void ValueTests()
		{
			Assert.IsNull(m_Field.Value, "Value is not null, expected null");
			m_Field.Value = "12345";
			Assert.AreEqual("12345", m_Field.Value, "Value");
			Assert.AreEqual(WDEFieldStatus.Plugged, m_Field.Status, "Status");
			Assert.AreEqual(0, m_Field.Revisions[0].CharCount, "CharCount");
			Assert.AreEqual(0, m_Field.Revisions[0].VCECount, "VCECount");
			Assert.AreEqual(1, m_Field.Revisions.Count, "Revisions.Count");
			Assert.AreEqual(m_Field.Value, m_Field.Revisions[0].Value, "Revisions.Value");
		}

		[TestMethod]
		public void StatusTests()
		{
			Assert.AreEqual(WDEFieldStatus.None, m_Field.Status);
			m_Field.Status = WDEFieldStatus.Validated;
			Assert.AreEqual(WDEFieldStatus.Validated, m_Field.Status);
			m_Field.Status = WDEFieldStatus.None;
			Assert.AreEqual(WDEFieldStatus.None, m_Field.Status);
			m_Field.Status = WDEFieldStatus.Plugged;
			Assert.AreEqual(WDEFieldStatus.Plugged, m_Field.Status);
			m_Field.Status = WDEFieldStatus.Flagged;
			Assert.AreEqual(WDEFieldStatus.Flagged, m_Field.Status);
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void StatusBad()
		{
			m_Field.Status = WDEFieldStatus.Keyed;
		}

		[TestMethod]
		public void IsNullTests()
		{
			Assert.IsTrue(m_Field.IsNull, "Field is not null, expected null");
			m_Field.Value = "12345";
			Assert.IsFalse(m_Field.IsNull, "Field is null, expected not null");
			m_Field.IsNull = true;
			Assert.IsTrue(m_Field.IsNull, "Field is not null after set, expected null");
			Assert.IsNull(m_Field.Value, "Value is not null after set, expected null");
			Assert.AreEqual(WDEFieldStatus.Plugged, m_Field.Status, "Status");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void IsNullFalseSet()
		{
			m_Field.IsNull = false;
		}

		[TestMethod]
		public void FlagDescriptionTests()
		{
			Assert.AreEqual("", m_Field.FlagDescription);
			m_Field.Value = "12345";
			Assert.AreEqual("", m_Field.FlagDescription);
			Assert.AreEqual(WDEFieldStatus.Plugged, m_Field.Status, "Status plugged");
			m_Field.FlagDescription = "This field is flagged";
			Assert.AreEqual(WDEFieldStatus.Flagged, m_Field.Status, "Status flag desc");
			Assert.AreEqual("This field is flagged", m_Field.FlagDescription, "FlagDescription");
			Assert.AreEqual(2, m_Field.Revisions.Count, "Revisions.Count");
			Assert.AreEqual(m_Field.FlagDescription, m_Field.Revisions[0].FlagDescription, "Revision.FlagDescription");

			m_Field.FlagDescription = "Another description";
			Assert.AreEqual("Another description", m_Field.FlagDescription, "Another FlagDescription");
			Assert.AreEqual(2, m_Field.Revisions.Count, "Another Revisions.Count");
			Assert.AreEqual(m_Field.FlagDescription, m_Field.Revisions[0].FlagDescription, "Another Revision.FlagDescription");
		}

		[TestMethod]
		public void FlaggedTests()
		{
			Assert.IsFalse(m_Field.Flagged, "Flagged is true, expected false");
			m_Field.Flagged = true;
			Assert.IsTrue(m_Field.Flagged, "Flagged is false, expected true");
			Assert.AreEqual(WDEFieldStatus.Flagged, m_Field.Status, "Status Flagged");
			Assert.AreEqual(1, m_Field.Revisions.Count, "Revisions.Count Flagged");
			
			m_Field.Flagged = false;
			Assert.AreEqual(WDEFieldStatus.None, m_Field.Status, "Status not Flagged");
			Assert.AreEqual(2, m_Field.Revisions.Count, "Revisions.Count not Flagged");
		}

		[TestMethod]
		public void FieldDefFind()
		{
			//m_Record.ExpectGetAlways("RecordDef", m_RecordDef.Object);
			//m_DataSet.ExpectGetAlways("Project", m_Project.Object);
			IWDEFieldInternal ifield = (IWDEFieldInternal) m_Field;
			ifield.FieldDef = null;
			Assert.IsNotNull(m_Field.FieldDef, "FieldDef is null. Expected not null");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void FieldDefFind_NP()
		{
			//m_Record.ExpectGetAlways("RecordDef", m_RecordDef.Object);
			IWDEFieldInternal ifield = (IWDEFieldInternal) m_Field;
			ifield.FieldDef = null;
			IWDEFieldDef except = m_Field.FieldDef;
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void FieldDefFind_NF()
		{
			//m_Record.ExpectGetAlways("RecordDef", m_RecordDef.Object);
			//m_FieldDefs.AlwaysReturn("Find", -1, null);
			//m_Document.ExpectGetAlways("DocType", "Document1");
			//m_Record.ExpectGetAlways("RecType", "Record1");
			//m_DataSet.ExpectGetAlways("Project", m_Project.Object);
			IWDEFieldInternal ifield = (IWDEFieldInternal) m_Field;
			ifield.FieldDef = null;
			IWDEFieldDef except = m_Field.FieldDef;
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void FieldDefNull()
		{
			IWDEFieldInternal ifield = m_Field as IWDEFieldInternal;
			ifield.FieldDef = null;
			IWDEFieldDef except = m_Field.FieldDef;
		}

		[TestMethod]
		public void FieldDefTests()
		{
			//m_FieldDef.ExpectGetAlways("FieldName", "Field1");
			//m_FieldDef.ExpectGetAlways("DataType", WDEDataType.Number);
			//m_FieldDef.ExpectGetAlways("DataLen", 10);
			IWDEFieldInternal ifield = m_Field as IWDEFieldInternal;
			ifield.FieldDef = (IWDEFieldDef) m_FieldDef;

			Assert.AreEqual("Field1", m_Field.FieldName, "FieldName");
			Assert.AreEqual(WDEDataType.Number, m_Field.DataType, "DataType");
			Assert.AreEqual(10, m_Field.DataLen, "DataLen");
		}

		[TestMethod]
		public void CharRepairs()
		{
			m_Field.Value = "12345";
			Assert.AreSame(m_Field.Revisions[0].CharRepairs, m_Field.CharRepairs, "CharRepairs are not the same");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void CharRepairsInvalid()
		{
			IWDECharRepairs except = m_Field.CharRepairs;
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void AsBoolean()
		{
			Assert.IsFalse(m_Field.AsBoolean, "Default true, expected false");

			m_Field.Value = "TRUE";
			Assert.IsTrue(m_Field.AsBoolean, "TRUE false, expected true");

			m_Field.Value = "FALSE";
			Assert.IsFalse(m_Field.AsBoolean, "FALSE true, expected false");

			m_Field.Value = "NOTBOOL";
			bool except = m_Field.AsBoolean;
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void AsCurrency()
		{
			Assert.AreEqual(0, m_Field.AsCurrency, "Default");

			m_Field.Value = "24.4";
			Assert.AreEqual((decimal)24.4, m_Field.AsCurrency);
			m_Field.Value = Decimal.MinValue.ToString();
			Assert.AreEqual(Decimal.MinValue, m_Field.AsCurrency, "MinValue");
			m_Field.Value = Decimal.MaxValue.ToString();
			Assert.AreEqual(Decimal.MaxValue, m_Field.AsCurrency, "MaxValue");

			m_Field.Value = "NOTCURRENCY";
			decimal except = m_Field.AsCurrency;
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void AsDateTime()
		{
			Assert.AreEqual(new DateTime(0), m_Field.AsDateTime);

			m_Field.Value = "10/25/06";
			Assert.AreEqual(new DateTime(2006, 10, 25), m_Field.AsDateTime);

			m_Field.Value = "NOTDATETIME";
			DateTime except = m_Field.AsDateTime;
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void AsFloat()
		{
			Assert.AreEqual(0, m_Field.AsFloat);

			m_Field.Value = "24.4";
			Assert.AreEqual(24.4, m_Field.AsFloat);

			m_Field.Value = "NOTFLOAT";
			double except = m_Field.AsFloat;
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void AsInteger()
		{
			Assert.AreEqual(0, m_Field.AsInteger);

			m_Field.Value = "24";
			Assert.AreEqual(24, m_Field.AsInteger);

			m_Field.Value = "NOTINT";
			int except = m_Field.AsInteger;
		}

		[TestMethod]
		public void AsString()
		{
			Assert.IsTrue(m_Field.IsNull, "AsString is not null, expected null");

			m_Field.Value = "DATA";
			Assert.AreEqual("DATA", m_Field.AsString);
		}

		[TestMethod]
		public void AsVariant()
		{
			m_Field.Value = "DATA";
			Assert.AreSame(m_Field.Value, m_Field.AsVariant);
		}

		[TestMethod]
		public void WriteToXmlDefault()
		{
			//m_DataSet.ExpectGetAlways("PersistFlags", false);
			IWDEXmlPersist ipers = m_Field as IWDEXmlPersist;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("FieldWriteToXmlDefault"), sw.ToString());
		}

		[TestMethod]
		public void ReadFromXmlDefault()
		{
			IWDEXmlPersist ipers = m_Field as IWDEXmlPersist;
			StringReader sr = new StringReader(m_ResMan.GetString("FieldWriteToXmlDefault"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.IsTrue(m_Field.IsNull, "Field is not null, expected null");
			Assert.IsFalse(m_Field.Exclude, "Exclude is true, expected false");
			Assert.AreEqual(WDEFieldFlags.None, m_Field.Flags, "Flags");
			Assert.AreEqual("", m_Field.ImageName, "ImageName");
			Assert.IsTrue(m_Field.ImageRect.IsEmpty, "ImageRect is not empty, expected empty");
			Assert.AreEqual(WDEMiscFlags.None, m_Field.MiscFlags, "MiscFlags");
			Assert.IsTrue(m_Field.OCRAreaRect.IsEmpty, "OCRAreaRect is not empty, expected empty");
			Assert.IsTrue(m_Field.OCRRect.IsEmpty, "OCRRect is not empty, expected empty");
			Assert.IsFalse(m_Field.QIFocusAudit, "QIFocusAudit is true, expected false");
		}

		[TestMethod]
		public void WriteToXmlFull()
		{
			//m_DataSet.ExpectGetAlways("PersistFlags", true);
			m_Field.Value = "VALUE";
			m_Field.Exclude = true;
			m_Field.Flags = WDEFieldFlags.Completed;
			m_Field.ImageName = "IMAGENAME";
			m_Field.ImageRect = new Rectangle(1, 2, 3, 4);
			m_Field.MiscFlags = WDEMiscFlags.Keyable | WDEMiscFlags.Verify;
			m_Field.OCRAreaRect = new Rectangle(5, 6, 7, 8);
			m_Field.OCRRect = new Rectangle(9, 10, 11, 12);
			m_Field.QIFocusAudit = true;

			IWDEXmlPersist ipers = m_Field as IWDEXmlPersist;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("FieldWriteToXmlFull"), sw.ToString());
		}

		[TestMethod]
		public void ReadFromXmlFull()
		{
			IWDEXmlPersist ipers = m_Field as IWDEXmlPersist;
			StringReader sr = new StringReader(m_ResMan.GetString("FieldWriteToXmlFull"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.IsFalse(m_Field.IsNull, "Field is null, expected not null");
			Assert.IsTrue(m_Field.Exclude, "Exclude is false, expected true");
			Assert.AreEqual(WDEFieldFlags.Completed, m_Field.Flags, "Flags");
			Assert.AreEqual("IMAGENAME", m_Field.ImageName, "ImageName");
			Assert.AreEqual(new Rectangle(1, 2, 3, 4), m_Field.ImageRect, "ImageRect");
			Assert.AreEqual(WDEMiscFlags.Keyable | WDEMiscFlags.Verify, m_Field.MiscFlags, "MiscFlags");
			Assert.AreEqual(new Rectangle(5, 6, 7, 8), m_Field.OCRAreaRect, "OCRAreaRect");
			Assert.AreEqual(new Rectangle(9, 10, 11, 12), m_Field.OCRRect, "OCRRect");
			Assert.IsTrue(m_Field.QIFocusAudit, "QIFocusAudit is false, expected true");
			Assert.AreEqual(1, m_Field.Revisions.Count, "Revisions.Count");
			Assert.AreEqual("VALUE", m_Field.Value, "Value");
		}

		[TestMethod]
		public void ReadFromXmlCharRepairs()
		{
			IWDEXmlPersist ipers = m_Field as IWDEXmlPersist;
			string test = m_ResMan.GetString("FieldReadFromXmlCR");
			StringReader sr = new StringReader(test);
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual(1, m_Field.Revisions.Count, "Revisions.Count");
			Assert.AreEqual(3, m_Field.Revisions[0].CharRepairs.Count, "CharRepairs.Count");
		}

		[TestMethod]
		public void DataTooLong()
		{
			//m_FieldDef.ExpectGetAlways("DataLen", 5);
			m_Field.Value = "1234567890";
			Assert.AreEqual("12345", m_Field.Value, "Field.Value");
		}

		[TestMethod]
		public void CustomData()
		{
			m_Field.CustomData = "CUSTOMDATA";
			Assert.AreEqual("CUSTOMDATA", m_Field.CustomData);
		}

		[TestMethod]
		public void CustomDataTooLong()
		{
			m_Field.CustomData = new string('A', 280);
			Assert.AreEqual(new string('A', 255), m_Field.CustomData);
		}

		[TestMethod]
		public void CustomDataWriteToXml()
		{
			m_Field.CustomData = "CUSTOMDATA";
			//m_DataSet.ExpectGetAlways("PersistFlags", false);
			IWDEXmlPersist ipers = m_Field as IWDEXmlPersist;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("FieldCustomData"), sw.ToString());
		}

		[TestMethod]
		public void CustomDataReadFromXml()
		{
			IWDEXmlPersist ipers = m_Field as IWDEXmlPersist;
			StringReader sr = new StringReader(m_ResMan.GetString("FieldCustomData"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual("CUSTOMDATA", m_Field.CustomData);
		}

		[TestMethod]
		public void BlankNull()
		{
			Assert.IsTrue(m_Field.IsNull);
			Assert.AreEqual("", m_Field.AsString);
		}

		[TestMethod]
		public void AsIntegerLarge()
		{
			string val = "1234567891";
			Console.WriteLine(Convert.ToInt32(val).ToString());
			m_Field.Value = val;
			Console.WriteLine(m_Field.AsInteger.ToString());

			string xml = "<Field FieldName=\"Field1\"><Revision SessionID=\"1\" Value=\"1234567891\" /></Field>";
			StringReader sr = new StringReader(xml);
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			IWDEXmlPersist ipers = m_Field as IWDEXmlPersist;
			ipers.ReadFromXml(XmlReader);
			Console.WriteLine(m_Field.AsInteger.ToString());
		}
	}
}
