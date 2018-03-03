using System;
using System.Drawing;
using System.Xml;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Text.RegularExpressions;

using WebDX.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using TypeMock;

namespace WebDX.Api.Tests
{
	[TestClass]
	public class DocumentObjectTests
	{
		ResourceManager m_ResMan;
		IWDEDocument m_Document;
		IWDEDocuments m_TestDocuments;

        //MockObject m_Documents;
        //MockObject m_DataSet;
        //MockObject m_Project;
        //MockObject m_RejectCodes;
        //MockObject m_RejectCode;
        //MockObject m_DocumentDefs;
        //MockObject m_DocumentDef;
        //MockObject m_RecordDefs;
        //MockObject m_RecordDef;
        //MockObject m_ChildRecDefs;
        //MockObject m_FieldDefs;
        //MockObject m_FieldDef;
        //MockObject m_Sessions;
        //MockObject m_Session;
        //MockObject m_ImageSourceDefs;
        //MockObject m_ImageSourceDef;

        IWDEDocuments m_Documents;
        IWDEDataSet m_DataSet;
        IWDEProject m_Project;
        IWDERejectCodes m_RejectCodes;
        IWDERejectCode m_RejectCode;
        IWDEDocumentDefs m_DocumentDefs;
        IWDEDocumentDef m_DocumentDef;
        IWDERecordDefs m_RecordDefs;
        IWDERecordDef m_RecordDef;
        //IWDERecordDef m_ChildRecDefs;
        IWDEFieldDef m_FieldDefs;
        IWDEFieldDefs m_FieldDef;
        IWDESessions m_Sessions;
        IWDESession_R1 m_Session;
        IWDEImageSourceDefs m_ImageSourceDefs;
        IWDEImageSourceDef m_ImageSourceDef;

        public DocumentObjectTests()
		{
		}

		[TestInitialize]
		public void Init()
		{
			//MockManager.Init();

			m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());

			//m_Documents = MockManager.MockObject(typeof(IWDEDocuments));
			//m_DataSet = MockManager.MockObject(typeof(IWDEDataSet));
			//m_Project = MockManager.MockObject(typeof(IWDEProject));
			//m_RejectCodes = MockManager.MockObject(typeof(IWDERejectCodes));
			//m_RejectCode = MockManager.MockObject(typeof(IWDERejectCode));
			//m_DocumentDefs = MockManager.MockObject(typeof(IWDEDocumentDefs));
			//m_DocumentDef = MockManager.MockObject(typeof(IWDEDocumentDef));
			//m_RecordDefs = MockManager.MockObject(typeof(IWDERecordDefs));
			//m_RecordDef = MockManager.MockObject(typeof(IWDERecordDef));
			//m_FieldDef = MockManager.MockObject(typeof(IWDEFieldDef));
			//m_FieldDefs = MockManager.MockObject(typeof(IWDEFieldDefs));
			//m_Sessions = MockManager.MockObject(typeof(IWDESessions));
			//m_Session = MockManager.MockObject(typeof(IWDESession_R1));
			//m_ImageSourceDefs = MockManager.MockObject(typeof(IWDEImageSourceDefs));
			//m_ImageSourceDef = MockManager.MockObject(typeof(IWDEImageSourceDef));
			//m_ChildRecDefs = MockManager.MockObject(typeof(IWDERecordDefs));

			//m_Documents.ExpectGetAlways("DataSet", m_DataSet.Object);
			//m_DataSet.ExpectGetAlways("Project", m_Project.Object);
			//m_DataSet.ExpectGetAlways("Sessions", m_Sessions.Object);
			//m_DataSet.ExpectGetAlways("DocumentDefs", m_DocumentDefs.Object);
			//m_Sessions.ExpectGetAlways("Item", m_Session.Object);
			//m_Sessions.ExpectGetAlways("Count", 1);
			//m_Session.ExpectGetAlways("SessionID", 1);
   //         m_Session.ExpectGetAlways("StartTime", DateTime.Now);
			//m_Project.ExpectGetAlways("RejectCodes", m_RejectCodes.Object);
			//m_Project.ExpectGetAlways("DocumentDefs", m_DocumentDefs.Object);
			//m_DocumentDefs.AlwaysReturn("Find", 0);
			//m_DocumentDefs.ExpectGetAlways("Item", m_DocumentDef.Object);
			//m_DocumentDef.ExpectGetAlways("RecordDefs", m_RecordDefs.Object);
			//m_DocumentDef.ExpectGetAlways("DocType", "Document1");
			//m_RejectCodes.ExpectGetAlways("Item", m_RejectCode.Object);
			
			//m_RecordDefs.ExpectGetAlways("Count", 1);
			//m_RecordDefs.AlwaysReturn("Find", 0);
			//m_RecordDefs.ExpectGetAlways("Item", m_RecordDef.Object);
			//m_RecordDef.ExpectGetAlways("FieldDefs", m_FieldDefs.Object);
			//m_RecordDef.ExpectGetAlways("RecType", "Record1");
			//m_RecordDef.ExpectGetAlways("RecordDefs", m_ChildRecDefs.Object);

			//m_ChildRecDefs.AlwaysReturn("Find", 0);
			//m_ChildRecDefs.ExpectGetAlways("Item", m_RecordDef.Object);
			
			//m_FieldDefs.ExpectGetAlways("Count", 1);
			//m_FieldDefs.ExpectGetAlways("Item", m_FieldDef.Object);
			//m_FieldDef.ExpectGetAlways("FieldName", "Field1");
			//m_FieldDef.ExpectGetAlways("CharSet", "");
			//m_FieldDef.ExpectGetAlways("DataLen", 20);
			//m_FieldDef.ExpectGetAlways("Options", WDEFieldOption.None);
			//m_FieldDef.ExpectGetAlways("DefaultValue", "1234");

			//m_RejectCodes.AlwaysReturn("Find", 0);
			//m_RejectCode.ExpectGetAlways("Description", "This document was rejected");
			//m_RejectCode.ExpectGetAlways("RequireDescription", false);

			//m_DocumentDef.ExpectGetAlways("ImageSourceDefs", m_ImageSourceDefs.Object);
			//m_ImageSourceDefs.AlwaysReturn("Find", 0);
			//m_ImageSourceDefs.ExpectGetAlways("Item", m_ImageSourceDef.Object);
			//m_ImageSourceDef.ExpectGetAlways("PerformOCR", false);
			//m_ImageSourceDef.ExpectGetAlways("StoredAttachType", "");

			//m_Session.ExpectGetAlways("User", "User");
			//m_Session.ExpectGetAlways("Task", "Task");
			//m_Session.ExpectGetAlways("Mode", WDEOpenMode.Create);

			m_Document = WDEDocument.Create((IWDEDocuments) m_Documents);
			IWDEDocSessionsInternal isess = m_Document.Sessions as IWDEDocSessionsInternal;
			isess.Add("User", "Task", WDEOpenMode.Edit, 1, "");

			m_TestDocuments = WDEDocuments.Create((IWDEDataSet) m_DataSet);
		}

		[TestCleanup]
		public void Cleanup()
		{
			m_Document = null;
			m_TestDocuments = null;

			m_Documents = null;
			m_DataSet = null;
			m_Project = null;
			m_RejectCodes = null;
			m_RejectCode = null;
			m_DocumentDefs = null;
			m_DocumentDef = null;
			m_RecordDefs = null;
			m_RecordDef = null;
			m_FieldDefs = null;
			m_FieldDef = null;
			m_Sessions = null;
			m_Session = null;
			m_ImageSourceDefs = null;
			m_ImageSourceDef = null;
			//m_ChildRecDefs = null;

			//MockManager.Verify();
			GC.Collect();
		}

		[TestMethod]
		public void APIVersion()
		{
			Assert.AreEqual(VersionInfo.VersionNumber, m_Document.APIVersion);
		}

        [TestMethod]
        public void Times()
        {
            IWDEDocument_R1 newDoc = (IWDEDocument_R1)m_Document;
            DateTime testDate = DateTime.Now;
            newDoc.StartTime = testDate;
            newDoc.EndTime = testDate;

            Assert.AreEqual(newDoc.StartTime, newDoc.EndTime, "Start/End");
            Assert.AreEqual(testDate, newDoc.StartTime, "StartTime");
            Assert.AreEqual(testDate, ((IWDEDocSession_R2)newDoc.Sessions[0]).StartTime, "SessStart");
            Assert.AreEqual(testDate, ((IWDEDocSession_R2)newDoc.Sessions[0]).EndTime, "SessEnd");
        }

		[TestMethod]
		public void RejectCode()
		{
			m_Document.RejectCode = "01";
			Assert.AreEqual("01", m_Document.RejectCode, "RejectCode");
			Assert.AreEqual("This document was rejected", m_Document.RejectDescription, "RejectDescription");
			Assert.AreEqual(WDESessionStatus.Rejected, m_Document.Status, "Status");
			Assert.IsTrue(m_Document.Rejected, "Rejected is false, expected true");
			Assert.AreEqual("01", m_Document.Sessions[0].RejectCode, "SessionRejectCode");
			Assert.AreEqual("This document was rejected", m_Document.Sessions[0].RejectDescription, "SessionRejectDescription");
			Assert.AreEqual(WDESessionStatus.Rejected, m_Document.Sessions[0].Status, "SessionStatus");

			m_Document.RejectCode = "";
			Assert.AreEqual("", m_Document.RejectCode, "RejectCodeBlank");
			Assert.AreEqual("", m_Document.RejectDescription, "RejectDescriptionBlank");
			Assert.AreEqual(WDESessionStatus.None, m_Document.Status, "StatusBlank");
			Assert.IsFalse(m_Document.Rejected, "Rejected is true, expected false when blank");
			Assert.AreEqual("", m_Document.Sessions[0].RejectCode, "SessionRejectCodeBlank");
			Assert.AreEqual("", m_Document.Sessions[0].RejectDescription, "SessionRejectDescriptionBlank");
			Assert.AreEqual(WDESessionStatus.None, m_Document.Sessions[0].Status, "SessionStatusBlank");
		}

		[TestMethod]
		public void RejectCodeSession()
		{
			m_Document.RejectCode = "01";
			IWDEDocSessionsInternal isess = m_Document.Sessions as IWDEDocSessionsInternal;
			isess.Add("User", "Task", WDEOpenMode.Compare, 2, "");
			Assert.AreEqual("01", m_Document.RejectCode);
			Assert.IsTrue(m_Document.Rejected, "Rejected is false. Expected true");
		}

		[TestMethod]
		public void RejectDescriptionSession()
		{
			m_Document.RejectCode = "01";
			m_Document.RejectDescription = "A description";
			IWDEDocSessionsInternal isess = m_Document.Sessions as IWDEDocSessionsInternal;
			isess.Add("User", "Task", WDEOpenMode.Compare, 2, "");
			Assert.AreEqual("A description", m_Document.RejectDescription);
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void RejectCodeProjectNull()
		{
			//m_DataSet.ExpectGetAlways("Project", null);
			m_Document.RejectCode = "01";
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void RejectCodeNoSessions()
		{
			IWDEDocSessionsInternal isess = m_Document.Sessions as IWDEDocSessionsInternal;
			isess.Clear();
			m_Document.RejectCode = "01";
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void RejectCodeInvalid()
		{
			//m_RejectCodes.AlwaysReturn("Find", -1);
			m_Document.RejectCode = "01";
		}

		[TestMethod]
		public void RejectDescription()
		{
			m_Document.RejectCode = "01";
			m_Document.RejectDescription = "A different description";
			Assert.AreEqual("A different description", m_Document.RejectDescription, "RejectDescription");
			Assert.AreEqual("A different description", m_Document.Sessions[0].RejectDescription, "SessionRejectDescription");

			m_Document.RejectDescription = "";
			Assert.AreEqual("", m_Document.RejectDescription, "Blank");
			Assert.AreEqual("", m_Document.Sessions[0].RejectDescription, "SessionBlank");

			m_Document.RejectCode = "";
			m_Document.RejectDescription = "A different description";
			Assert.AreEqual("A different description", m_Document.RejectDescription, "RejectDescriptionNR");
			Assert.AreEqual("A different description", m_Document.Sessions[0].RejectDescription, "SessionRejectDescriptionNR");

			m_Document.RejectDescription = "";
			Assert.AreEqual("", m_Document.RejectDescription, "BlankNR");
			Assert.AreEqual("", m_Document.Sessions[0].RejectDescription, "SessionBlankNR");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void RejectDescProjectNull()
		{
			//m_DataSet.ExpectGetAlways("Project", null);
			m_Document.RejectDescription = "Desc";
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void RejectDescNoSessions()
		{
			IWDEDocSessionsInternal isess = m_Document.Sessions as IWDEDocSessionsInternal;
			isess.Clear();
			m_Document.RejectDescription = "Desc";
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void RejectDescRCInvalid()
		{
			m_Document.RejectCode = "01";
			//m_RejectCodes.AlwaysReturn("Find", -1);
			m_Document.RejectDescription = "Desc";
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void RequireDescription()
		{
			m_Document.RejectCode = "01";
			//m_RejectCode.ExpectGetAlways("RequireDescription", true);
			m_Document.RejectDescription = "";
		}

		[TestMethod]
		public void Reject()
		{
			Assert.IsFalse(m_Document.Rejected, "Rejected is true, expected false");
			Assert.AreEqual(WDESessionStatus.None, m_Document.Status, "Status");
			m_Document.Rejected = true;
			Assert.IsTrue(m_Document.Rejected, "Rejected is false, expected true");
			Assert.AreEqual(WDESessionStatus.Rejected, m_Document.Status, "StatusRejected");
		}

		[TestMethod]
		public void Status()
		{
			m_Document.RejectCode = "01";
			IWDEDocumentInternal idoc = m_Document as IWDEDocumentInternal;
			idoc.Status = WDESessionStatus.None;
			Assert.AreEqual("", m_Document.RejectCode, "RejectCode");
			Assert.AreEqual("", m_Document.RejectDescription, "RejectDescription");
		}

		[TestMethod]
		public void DocumentDef()
		{
			Assert.IsNotNull(m_Document.DocumentDef, "DocumentDef is null. Expected not null.");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentDefNotFound()
		{
			//m_DocumentDefs.AlwaysReturn("Find", -1);
			Assert.IsNull(m_Document.DocumentDef, "DocumentDef is not null. Expected null.");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentDefNoProject()
		{
			//m_DocumentDefs.AlwaysReturn("Find", 0);
			//m_DataSet.ExpectGetAlways("Project", null);
			Assert.IsNull(m_Document.DocumentDef, "DocumentDef (Project) is not null. Expected null.");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentDefNullDataSet()
		{
			//m_Documents.ExpectGetAlways("DataSet", null);
			object except = m_Document.DocumentDef;
		}

		[TestMethod]
		public void RecordDefs()
		{
			IWDEDocumentInternal idoc = m_Document as IWDEDocumentInternal;
			Assert.AreSame(m_Document.DocumentDef.RecordDefs, idoc.RecordDefs, "RecordDefs are not the same. Expected same");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void RecordDefsNullDocDef()
		{
			IWDEDocumentInternal idoc = m_Document as IWDEDocumentInternal;
			idoc.DocType = "Document1";
			//m_DocumentDefs.AlwaysReturn("Find", -1);
			object except = idoc.RecordDefs;
		}

		[TestMethod]
		public void WriteToXmlDefault()
		{
			IWDEDocSessionsInternal isess = m_Document.Sessions as IWDEDocSessionsInternal;
			isess.Clear();
			IWDEXmlPersist ipers = m_Document as IWDEXmlPersist;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual("<Document APIVersion=\"" + VersionInfo.VersionNumber + "\" />", sw.ToString());
		}
		
		[TestMethod]
		public void ReadFromXmlDefault()
		{
			IWDEDocSessionsInternal isess = m_Document.Sessions as IWDEDocSessionsInternal;
			isess.Clear();
			StringReader sr = new StringReader("<Document APIVersion=\"1.2.0.0\" />");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			IWDEXmlPersist ipers = m_Document as IWDEXmlPersist;
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			
			Assert.AreEqual("1.2.0.0", m_Document.APIVersion, "APIVersion");
			Assert.AreEqual("", m_Document.AltDCN, "AltDCN");
			Assert.AreEqual("", m_Document.DCN, "DCN");
			Assert.AreEqual("", m_Document.DocType, "DocType");
			Assert.IsFalse(m_Document.QIAutoAudit, "QIAutoAudit is true. Expected false");
			Assert.IsFalse(m_Document.QIFocusAudit, "QIFocusAudit is true. Expected false");
			Assert.IsFalse(m_Document.QISelected, "QISelected is true. Expected false");
			Assert.AreEqual("", m_Document.RejectCode, "RejectCode");
			Assert.AreEqual("", m_Document.RejectDescription, "RejectDescription");
			Assert.IsFalse(m_Document.Rejected, "Rejected is true. Expected false");
			Assert.AreEqual("", m_Document.StoredDocType, "StoredDocType");

			Assert.AreEqual(0, m_Document.Images.Count, "Images.Count");
			Assert.AreEqual(0, m_Document.Sessions.Count, "Sessions.Count");
			Assert.AreEqual(0, m_Document.Records.Count, "Records.Count");
		}

		[TestMethod]
		public void WriteToXmlFull()
		{
			//m_DataSet.ExpectGetAlways("PersistFlags", false);
			m_Document.Images.Append("ImageType", "Image1");
			m_Document.Images.Append("ImageType", "Image2");
			m_Document.Images.Append("ImageType", "Image3");
			m_Document.Records.Append("Record1");
			m_Document.Records.Append("Record1");
            m_Document.Records.Append("Record1");

			IWDEDocSessionsInternal idocsess = m_Document.Sessions as IWDEDocSessionsInternal;
			IWDEDocSessionInternal dsr2 = (IWDEDocSessionInternal)idocsess.Add("User", "Task2", WDEOpenMode.Create, 2, "");
            dsr2.StartTime = DateTime.MinValue;
            dsr2.EndTime = DateTime.MinValue;
            dsr2 = (IWDEDocSessionInternal)idocsess.Add("User", "Task3", WDEOpenMode.Create, 3, "");
            dsr2.StartTime = DateTime.MinValue;
            dsr2.EndTime = DateTime.MinValue;

            dsr2 = (IWDEDocSessionInternal)m_Document.Sessions[m_Document.Sessions.Count - 1];
            dsr2.StartTime = DateTime.MinValue;
            dsr2.EndTime = DateTime.MinValue;

			m_Document.AltDCN = "AltDCN";
			m_Document.DCN = "DCN";
			IWDEDocumentInternal idoc = m_Document as IWDEDocumentInternal;
			idoc.DocType = "DocType";
			m_Document.QIAutoAudit = true;
			m_Document.QIFocusAudit = true;
			m_Document.QISelected = true;
			m_Document.RejectCode = "01";
			m_Document.StoredDocType = "StoredDocType";

			IWDEXmlPersist ipers = m_Document as IWDEXmlPersist;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			string test = m_ResMan.GetString("DocumentWriteToXmlFull");
			test = Regex.Replace(test, "APIVersion=\"[^\"]+\"", "APIVersion=\"" + VersionInfo.VersionNumber + "\"");
			Assert.AreEqual(test, sw.ToString());
		}

		[TestMethod]
		public void SaveToStream()
		{
			//m_DataSet.ExpectGetAlways("PersistFlags", false);
			m_Document.Images.Append("ImageType", "Image1");
			m_Document.Images.Append("ImageType", "Image2");
			m_Document.Images.Append("ImageType", "Image3");
			m_Document.Records.Append("Record1");
			m_Document.Records.Append("Record1");
			m_Document.Records.Append("Record1");

			IWDEDocSessionsInternal idocsess = m_Document.Sessions as IWDEDocSessionsInternal;
            IWDEDocSessionInternal dsr2 = (IWDEDocSessionInternal)idocsess.Add("User", "Task2", WDEOpenMode.Create, 2, "");
            dsr2.StartTime = DateTime.MinValue;
            dsr2.EndTime = DateTime.MinValue;
            dsr2 = (IWDEDocSessionInternal)idocsess.Add("User", "Task3", WDEOpenMode.Create, 3, "");
            dsr2.StartTime = DateTime.MinValue;
            dsr2.EndTime = DateTime.MinValue;

            dsr2 = (IWDEDocSessionInternal)m_Document.Sessions[m_Document.Sessions.Count - 1];
            dsr2.StartTime = DateTime.MinValue;
            dsr2.EndTime = DateTime.MinValue;

			m_Document.AltDCN = "AltDCN";
			m_Document.DCN = "DCN";
			IWDEDocumentInternal idoc = m_Document as IWDEDocumentInternal;
			idoc.DocType = "DocType";
			m_Document.QIAutoAudit = true;
			m_Document.QIFocusAudit = true;
			m_Document.QISelected = true;
			m_Document.RejectCode = "01";
			m_Document.StoredDocType = "StoredDocType";

			MemoryStream stream = new MemoryStream();
			m_Document.SaveToStream(stream, false);
			stream.Seek(0, SeekOrigin.Begin);

			StreamReader sr = new StreamReader(stream);
			string results = sr.ReadToEnd();

			string test = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" + Environment.NewLine + m_ResMan.GetString("DocumentWriteToXmlFull");
			test = Regex.Replace(test, "APIVersion=\"[^\"]+\"", "APIVersion=\"" + VersionInfo.VersionNumber + "\"");
			Assert.AreEqual(test, results);
		}

		[TestMethod]
		public void SaveToStreamChild()
		{
			IWDEDocSessionsInternal isess = m_Document.Sessions as IWDEDocSessionsInternal;
			isess.Clear();

			MemoryStream stream = new MemoryStream();
			m_Document.SaveToStream(stream, true);
			stream.Seek(0, SeekOrigin.Begin);

			StreamReader sr = new StreamReader(stream);
			string results = sr.ReadToEnd();

			string test = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" + Environment.NewLine + "<DataSet>" + Environment.NewLine + 
				"  <Document APIVersion=\"" + VersionInfo.VersionNumber + "\" />" + Environment.NewLine + "</DataSet>";
			test = Regex.Replace(test, "APIVersion=\"[^\"]+\"", "APIVersion=\"" + VersionInfo.VersionNumber + "\"");
			Assert.AreEqual(test, results);
		}

		[TestMethod]
		public void ReadFromXmlFull()
		{
			IWDEDocSessionsInternal isess = m_Document.Sessions as IWDEDocSessionsInternal;
			isess.Clear();
			string test = m_ResMan.GetString("DocumentWriteToXmlFull");
			test = Regex.Replace(test, "APIVersion=\"[^\"]+\"", "APIVersion=\"" + "1.2.0.0" + "\"");
			StringReader sr = new StringReader(test);
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			IWDEXmlPersist ipers = m_Document as IWDEXmlPersist;
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			
			Assert.AreEqual("1.2.0.0", m_Document.APIVersion, "APIVersion");
			Assert.AreEqual("AltDCN", m_Document.AltDCN, "AltDCN");
			Assert.AreEqual("DCN", m_Document.DCN, "DCN");
			Assert.AreEqual("DocType", m_Document.DocType, "DocType");
			Assert.IsTrue(m_Document.QIAutoAudit, "QIAutoAudit is false. Expected true");
			Assert.IsTrue(m_Document.QIFocusAudit, "QIFocusAudit is false. Expected true");
			Assert.IsTrue(m_Document.QISelected, "QISelected is false. Expected true");
			Assert.AreEqual("01", m_Document.RejectCode, "RejectCode");
			Assert.AreEqual("This document was rejected", m_Document.RejectDescription, "RejectDescription");
			Assert.IsTrue(m_Document.Rejected, "Rejected is false. Expected true");
			Assert.AreEqual("StoredDocType", m_Document.StoredDocType, "StoredDocType");

			Assert.AreEqual(3, m_Document.Images.Count, "Images.Count");
			Assert.AreEqual(3, m_Document.Sessions.Count, "Sessions.Count");
			Assert.AreEqual(3, m_Document.Records.Count, "Records.Count");
		}

		[TestMethod]
		public void LoadFromStream()
		{
			IWDEDocSessionsInternal isess = m_Document.Sessions as IWDEDocSessionsInternal;
			isess.Clear();
			string test = m_ResMan.GetString("DocumentWriteToXmlFull");
			test = Regex.Replace(test, "APIVersion=\"[^\"]+\"", "APIVersion=\"" + "1.2.0.0" + "\"");

			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms, UnicodeEncoding.UTF8);
			sw.Write(test);
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			m_Document.LoadFromStream(ms);

			Assert.AreEqual("1.2.0.0", m_Document.APIVersion, "APIVersion");
			Assert.AreEqual("AltDCN", m_Document.AltDCN, "AltDCN");
			Assert.AreEqual("DCN", m_Document.DCN, "DCN");
			Assert.AreEqual("DocType", m_Document.DocType, "DocType");
			Assert.IsTrue(m_Document.QIAutoAudit, "QIAutoAudit is false. Expected true");
			Assert.IsTrue(m_Document.QIFocusAudit, "QIFocusAudit is false. Expected true");
			Assert.IsTrue(m_Document.QISelected, "QISelected is false. Expected true");
			Assert.AreEqual("01", m_Document.RejectCode, "RejectCode");
			Assert.AreEqual("This document was rejected", m_Document.RejectDescription, "RejectDescription");
			Assert.IsTrue(m_Document.Rejected, "Rejected is false. Expected true");
			Assert.AreEqual("StoredDocType", m_Document.StoredDocType, "StoredDocType");

			Assert.AreEqual(3, m_Document.Images.Count, "Images.Count");
			Assert.AreEqual(3, m_Document.Sessions.Count, "Sessions.Count");
			Assert.AreEqual(3, m_Document.Records.Count, "Records.Count");
			sw.Close();
		}

		[TestMethod]
		public void LoadFromStreamDS()
		{
			IWDEDocSessionsInternal isess = m_Document.Sessions as IWDEDocSessionsInternal;
			isess.Clear();
			string test = "<DataSet>" + m_ResMan.GetString("DocumentWriteToXmlFull") + "</DataSet>";
			test = Regex.Replace(test, "APIVersion=\"[^\"]+\"", "APIVersion=\"" + "1.2.0.0" + "\"");

			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms, UnicodeEncoding.UTF8);
			sw.Write(test);
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			m_Document.LoadFromStream(ms);

			Assert.AreEqual("1.2.0.0", m_Document.APIVersion, "APIVersion");
			Assert.AreEqual("AltDCN", m_Document.AltDCN, "AltDCN");
			Assert.AreEqual("DCN", m_Document.DCN, "DCN");
			Assert.AreEqual("DocType", m_Document.DocType, "DocType");
			Assert.IsTrue(m_Document.QIAutoAudit, "QIAutoAudit is false. Expected true");
			Assert.IsTrue(m_Document.QIFocusAudit, "QIFocusAudit is false. Expected true");
			Assert.IsTrue(m_Document.QISelected, "QISelected is false. Expected true");
			Assert.AreEqual("01", m_Document.RejectCode, "RejectCode");
			Assert.AreEqual("This document was rejected", m_Document.RejectDescription, "RejectDescription");
			Assert.IsTrue(m_Document.Rejected, "Rejected is false. Expected true");
			Assert.AreEqual("StoredDocType", m_Document.StoredDocType, "StoredDocType");

			Assert.AreEqual(3, m_Document.Images.Count, "Images.Count");
			Assert.AreEqual(3, m_Document.Sessions.Count, "Sessions.Count");
			Assert.AreEqual(3, m_Document.Records.Count, "Records.Count");
			sw.Close();
		}

		[TestMethod]
		public void DocumentsAppend()
		{
			Assert.AreEqual(-1, m_TestDocuments.Index, "Begin Index");
			IWDEDocument res = m_TestDocuments.Append("Document1");
			Assert.AreEqual(1, m_TestDocuments.Count, "Count");
			Assert.IsFalse(m_TestDocuments.BOF, "BOF is true. Expected false.");
			Assert.IsFalse(m_TestDocuments.EOF, "EOF is true. Expected false.");
			Assert.AreSame(res, m_TestDocuments[0], "res and m_TestDocuments[0] are not the same. Expected same");
			Assert.IsNotNull(m_TestDocuments[0].DocumentDef, "DocumentDef is null. Expected not null");
			Assert.AreEqual("Document1", m_TestDocuments[0].DocType, "DocType");
			Assert.AreEqual("Document1", m_TestDocuments[0].StoredDocType, "StoredDocType");
			Assert.AreEqual(1, m_TestDocuments[0].Sessions.Count, "Sessions.Count");
			Assert.AreEqual("User", m_TestDocuments[0].Sessions[0].User, "Sessions.User");
			Assert.AreEqual("Task", m_TestDocuments[0].Sessions[0].Task, "Sessions.Task");
			Assert.AreEqual(WDEOpenMode.Create, m_TestDocuments[0].Sessions[0].Mode, "Sessions.Mode");
			Assert.AreEqual(1, m_TestDocuments[0].Sessions[0].SessionID, "Sessions.SessionID");
			Assert.AreEqual(0, m_TestDocuments.Index, "Index");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentsAppendNoSess()
		{
			//m_Sessions.ExpectGetAlways("Count", 0);
			m_TestDocuments.Append("Document1");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentsAppendWrongType()
		{
			//m_DocumentDefs.AlwaysReturn("Find", -1);
			m_TestDocuments.Append("Document1");
		}

		[TestMethod]
		public void DocumentsInsert()
		{
			m_TestDocuments.Insert(0, "Document1");
			Assert.AreEqual(1, m_TestDocuments.Count, "Initial Count");
			m_TestDocuments.Insert(0, "Document2");
			Assert.AreEqual(2, m_TestDocuments.Count, "Next Count");
			Assert.AreEqual("Document2", m_TestDocuments[0].DocType, "DocType");
			Assert.AreEqual("Document1", m_TestDocuments[1].DocType, "DocType1");
			m_TestDocuments.Clear();

			m_TestDocuments.Append("Document1");
			m_TestDocuments.Append("Document1");
			m_TestDocuments.Append("Document1");

			m_TestDocuments.Insert(0, "Document2");
			m_TestDocuments.Insert(2, "Document3");
			m_TestDocuments.Insert(4, "Document4");
			Assert.AreEqual("Document2", m_TestDocuments[0].DocType, "DocType2");
			Assert.AreEqual("Document1", m_TestDocuments[1].DocType, "DocType3");
			Assert.AreEqual("Document3", m_TestDocuments[2].DocType, "DocType4");
			Assert.AreEqual("Document1", m_TestDocuments[3].DocType, "DocType5");
			Assert.AreEqual("Document4", m_TestDocuments[4].DocType, "DocType6");
			Assert.AreEqual("Document1", m_TestDocuments[5].DocType, "DocType7");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentsInsertHigh()
		{
			m_TestDocuments.Insert(1, "Document1");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentsInsertLow()
		{
			m_TestDocuments.Insert(-1, "Document1");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentsInsertHighMany()
		{
			m_TestDocuments.Append("Document1");
			m_TestDocuments.Append("Document1");
			m_TestDocuments.Append("Document1");
			m_TestDocuments.Insert(3, "Document1");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentsInsertLowMany()
		{
			m_TestDocuments.Append("Document1");
			m_TestDocuments.Append("Document1");
			m_TestDocuments.Append("Document1");
			m_TestDocuments.Insert(-1, "Document1");
		}

		[TestMethod]
		public void DocumentsFirstND()
		{
			m_TestDocuments.First();
			Assert.IsTrue(m_TestDocuments.BOF, "BOF is false. Expected true.");
			Assert.IsTrue(m_TestDocuments.EOF, "EOF is false. Expected true.");
		}

		[TestMethod]
		public void DocumentsFirst()
		{
			IWDEDocument first = m_TestDocuments.Append("Document1");
			IWDEDocument mid = m_TestDocuments.Append("Document1");
			IWDEDocument last = m_TestDocuments.Append("Document1");

			Assert.AreSame(last, m_TestDocuments.Current, "last and Current are not the same. Expected same.");
			m_TestDocuments.First();
			Assert.AreSame(first, m_TestDocuments.Current, "first and Current are not the same. Expected same.");
			Assert.IsTrue(m_TestDocuments.BOF, "BOF is false. Expected true.");
			Assert.IsFalse(m_TestDocuments.EOF, "EOF is true. Expected false.");
		}

		[TestMethod]
		public void DocumentsLastND()
		{
			m_TestDocuments.Last();
			Assert.IsTrue(m_TestDocuments.BOF, "BOF is false. Expected true.");
			Assert.IsTrue(m_TestDocuments.EOF, "EOF is false. Expected true.");
		}

		[TestMethod]
		public void DocumentsLast()
		{
			IWDEDocument first = m_TestDocuments.Append("Document1");
			IWDEDocument mid = m_TestDocuments.Append("Document1");
			IWDEDocument last = m_TestDocuments.Append("Document1");

			m_TestDocuments.First();
			m_TestDocuments.Last();
			Assert.AreSame(last, m_TestDocuments.Current, "last and Current are not the same. Expected same.");
			Assert.IsFalse(m_TestDocuments.BOF, "BOF is true. Expected false.");
			Assert.IsFalse(m_TestDocuments.EOF, "EOF is true. Expected false.");
		}

		[TestMethod]
		public void DocumentsNext()
		{
			IWDEDocument first = m_TestDocuments.Append("Document1");
			IWDEDocument mid = m_TestDocuments.Append("Document1");
			IWDEDocument last = m_TestDocuments.Append("Document1");
	
			m_TestDocuments.First();
			m_TestDocuments.Next();
			Assert.AreSame(mid, m_TestDocuments.Current, "mid and Current are not the same. Expected same.");
			Assert.IsFalse(m_TestDocuments.BOF, "BOF is true. Expected false.");
			Assert.IsFalse(m_TestDocuments.EOF, "EOF is true. Expected false.");
			m_TestDocuments.Next();
			Assert.AreSame(last, m_TestDocuments.Current, "last and Current are not the same. Expected same.");
			Assert.IsFalse(m_TestDocuments.BOF, "BOF is true. Expected false.");
			Assert.IsFalse(m_TestDocuments.EOF, "EOF is true. Expected false.");
			m_TestDocuments.Next();
			Assert.AreSame(last, m_TestDocuments.Current, "last2 and Current are not the same. Expected same.");
			Assert.IsFalse(m_TestDocuments.BOF, "BOF is true. Expected false.");
			Assert.IsTrue(m_TestDocuments.EOF, "EOF is false. Expected true.");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentsNextND()
		{
			m_TestDocuments.Next();
		}

		[TestMethod]
		public void DocumentsPrior()
		{
			IWDEDocument first = m_TestDocuments.Append("Document1");
			IWDEDocument mid = m_TestDocuments.Append("Document1");
			IWDEDocument last = m_TestDocuments.Append("Document1");
	
			m_TestDocuments.Last();
			m_TestDocuments.Prior();
			Assert.AreSame(mid, m_TestDocuments.Current, "mid and Current are not the same. Expected same.");
			Assert.IsFalse(m_TestDocuments.BOF, "BOF is true. Expected false.");
			Assert.IsFalse(m_TestDocuments.EOF, "EOF is true. Expected false.");
			m_TestDocuments.Prior();
			Assert.AreSame(first, m_TestDocuments.Current, "first and Current are not the same. Expected same.");
			Assert.IsFalse(m_TestDocuments.BOF, "BOF is true. Expected false.");
			Assert.IsFalse(m_TestDocuments.EOF, "EOF is true. Expected false.");
			m_TestDocuments.Prior();
			Assert.AreSame(first, m_TestDocuments.Current, "first2 and Current are not the same. Expected same.");
			Assert.IsTrue(m_TestDocuments.BOF, "BOF is false. Expected true.");
			Assert.IsFalse(m_TestDocuments.EOF, "EOF is true. Expected false.");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentsPriorND()
		{
			m_TestDocuments.Prior();
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentsIndexND()
		{
			Assert.AreEqual(-1, m_TestDocuments.Index, "Index1");
			m_TestDocuments.Index = 0;
		}

		[TestMethod]
		public void DocumentsIndex()
		{
			IWDEDocument first = m_TestDocuments.Append("Document1");
			IWDEDocument mid = m_TestDocuments.Append("Document1");
			IWDEDocument last = m_TestDocuments.Append("Document1");

			m_TestDocuments.Index = 0;
			Assert.AreSame(first, m_TestDocuments.Current, "first and Current are not the same. Expected same.");
			Assert.IsFalse(m_TestDocuments.BOF, "BOF is true. Expected false.");
			Assert.IsFalse(m_TestDocuments.EOF, "EOF is true. Expected false.");
			m_TestDocuments.Index = 1;
			Assert.AreSame(mid, m_TestDocuments.Current, "mid and Current are not the same. Expected same.");
			Assert.IsFalse(m_TestDocuments.BOF, "BOF1 is true. Expected false.");
			Assert.IsFalse(m_TestDocuments.EOF, "EOF1 is true. Expected false.");
			m_TestDocuments.Index = 2;
			Assert.AreSame(last, m_TestDocuments.Current, "last and Current are not the same. Expected same.");
			Assert.IsFalse(m_TestDocuments.BOF, "BOF2 is true. Expected false.");
			Assert.IsFalse(m_TestDocuments.EOF, "EOF2 is true. Expected false.");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentsIndexHigh()
		{
			IWDEDocument first = m_TestDocuments.Append("Document1");
			IWDEDocument mid = m_TestDocuments.Append("Document1");
			IWDEDocument last = m_TestDocuments.Append("Document1");

			m_TestDocuments.Index = 3;
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentsIndexLow()
		{
			IWDEDocument first = m_TestDocuments.Append("Document1");
			IWDEDocument mid = m_TestDocuments.Append("Document1");
			IWDEDocument last = m_TestDocuments.Append("Document1");

			m_TestDocuments.Index = -1;
		}

		[TestMethod]
		public void DocumentsDelete()
		{
			IWDEDocument first = m_TestDocuments.Append("Document1");
			IWDEDocument mid = m_TestDocuments.Append("Document1");
			IWDEDocument last = m_TestDocuments.Append("Document1");

			Assert.AreSame(last, m_TestDocuments.Current, "last and Current are not the same. Expected same.");
			Assert.IsFalse(m_TestDocuments.BOF, "BOF is true. Expected false.");
			Assert.IsFalse(m_TestDocuments.EOF, "EOF is true. Expected false.");
			m_TestDocuments.Delete();
			Assert.AreSame(mid, m_TestDocuments.Current, "mid and Current are not the same. Expected same.");
			Assert.IsFalse(m_TestDocuments.BOF, "BOF1 is true. Expected false.");
			Assert.IsFalse(m_TestDocuments.EOF, "EOF1 is true. Expected false.");
			m_TestDocuments.Delete();
			Assert.AreSame(first, m_TestDocuments.Current, "first and Current are not the same. Expected same.");
			Assert.IsFalse(m_TestDocuments.BOF, "BOF2 is true. Expected false.");
			Assert.IsFalse(m_TestDocuments.EOF, "EOF2 is true. Expected false.");
			m_TestDocuments.Delete();
			Assert.AreEqual(0, m_TestDocuments.Count, "Count");
			Assert.IsTrue(m_TestDocuments.BOF, "BOF3 is false. Expected true.");
			Assert.IsTrue(m_TestDocuments.EOF, "EOF3 is false. Expected true.");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentsDeleteNC()
		{
			m_TestDocuments.Delete();
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentsDeleteHigh()
		{
			m_TestDocuments.Append("Document1");
			m_TestDocuments.Delete();
			m_TestDocuments.Delete();
		}

		[TestMethod]
		public void DocumentsFind()
		{
			int res = m_TestDocuments.Find("NotThere");
			Assert.AreEqual(-1, res, "Empty");

			IWDEDocument doc = m_TestDocuments.Append("Document1");
			doc.DCN = "1";
			doc = m_TestDocuments.Append("Document1");
			doc.DCN = "2";
			doc = m_TestDocuments.Append("Document1");
			doc.DCN = "3";

			res = m_TestDocuments.Find("1");
			Assert.AreEqual(0, res, "res 1");
			res = m_TestDocuments.Find("2");
			Assert.AreEqual(1, res, "res 2");
			res = m_TestDocuments.Find("3");
			Assert.AreEqual(2, res, "res 3");

			res = m_TestDocuments.Find("NotThere");
			Assert.AreEqual(-1, res, "NotThere");
		}

		[TestMethod]
		public void DocumentsWriteToXml()
		{
			IWDEDocument first = m_TestDocuments.Append("Document1");
			IWDEDocument mid = m_TestDocuments.Append("Document1");
			IWDEDocument last = m_TestDocuments.Append("Document1");

			first.DCN = "DCN1";
			mid.DCN = "DCN2";
			last.DCN = "DCN3";

			IWDEXmlPersist ipers = m_TestDocuments as IWDEXmlPersist;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			XmlWriter.WriteStartElement("DataSet");
			ipers.WriteToXml(XmlWriter);
			XmlWriter.WriteEndElement();
			XmlWriter.Close();

			XmlDocument xdoc = new XmlDocument();
			xdoc.LoadXml(sw.ToString());
			XmlNodeList docs = xdoc.DocumentElement.ChildNodes;
			Assert.AreEqual(3, docs.Count, "DocCount");
			Assert.AreEqual("DCN1", docs[0].Attributes["DCN"].Value, "DCN1");
			Assert.AreEqual("DCN2", docs[1].Attributes["DCN"].Value, "DCN2");
			Assert.AreEqual("DCN3", docs[2].Attributes["DCN"].Value, "DCN3");
		}

		[TestMethod]
		public void DocumentsReadFromXml()
		{
			IWDEXmlPersist ipers = (IWDEXmlPersist) m_TestDocuments;
			string test = m_ResMan.GetString("DocumentsWriteToXml");
			StringReader sr = new StringReader(test);
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual(3, m_TestDocuments.Count, "Count");
			Assert.AreEqual("DCN1", m_TestDocuments[0].DCN, "DCN1");
			Assert.AreEqual("DCN2", m_TestDocuments[1].DCN, "DCN2");
			Assert.AreEqual("DCN3", m_TestDocuments[2].DCN, "DCN3");
		}

		[TestMethod]
		public void DocumentsReadFromXmlDS()
		{
			IWDEXmlPersist ipers = (IWDEXmlPersist) m_TestDocuments;
			string test = "<DataSet>" + m_ResMan.GetString("DocumentsWriteToXml") + "</DataSet>";
			StringReader sr = new StringReader(test);
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual(3, m_TestDocuments.Count, "Count");
			Assert.AreEqual("DCN1", m_TestDocuments[0].DCN, "DCN1");
			Assert.AreEqual("DCN2", m_TestDocuments[1].DCN, "DCN2");
			Assert.AreEqual("DCN3", m_TestDocuments[2].DCN, "DCN3");
		}

		[TestMethod]
		public void DocumentsLoadDocumentFromStream()
		{
			string test = "<DataSet>" + m_ResMan.GetString("DocumentWriteToXmlFull") + "</DataSet>";
			IWDEDocumentsInternal idoc = (IWDEDocumentsInternal) m_TestDocuments;
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms, UnicodeEncoding.UTF8);
			sw.Write(test);
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			idoc.LoadDocumentFromStream(ms);
			Assert.AreEqual(1, m_TestDocuments.Count, "Count");
			Assert.AreSame(m_TestDocuments.Current, m_TestDocuments[0], "Current and m_TestDocuments[0] are not the same. Expected same.");
			sw.Close();
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocumentsLoadDocumentFromStreamBadType()
		{
			//m_DocumentDefs.AlwaysReturn("Find", -1);
			string test = "<DataSet>" + m_ResMan.GetString("DocumentWriteToXmlFull") + "</DataSet>";
			IWDEDocumentsInternal idoc = (IWDEDocumentsInternal) m_TestDocuments;
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms, UnicodeEncoding.UTF8);
			sw.Write(test);
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			idoc.LoadDocumentFromStream(ms); // throws exception
			sw.Close();
		}

		[TestMethod]
		public void DocumentsLoadDocumentFromStreamNP()
		{
			//m_DocumentDefs.AlwaysReturn("Find", -1);
			//m_DataSet.ExpectGetAlways("Project", null);
			string test = "<DataSet>" + m_ResMan.GetString("DocumentWriteToXmlFull") + "</DataSet>";
			IWDEDocumentsInternal idoc = (IWDEDocumentsInternal) m_TestDocuments;
			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms, UnicodeEncoding.UTF8);
			sw.Write(test);
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			idoc.LoadDocumentFromStream(ms);
			Assert.AreEqual(1, m_TestDocuments.Count, "Count");
			Assert.AreSame(m_TestDocuments.Current, m_TestDocuments[0], "Current and m_TestDocuments[0] are not the same. Expected same.");
			sw.Close();
		}

		[TestMethod]
		public void FilteredRecords()
		{
			m_Document.Records.Append("Record1");
			//m_RecordDef.ExpectGetAlways("RecType", "Record2");
			m_Document.Records[0].Records.Append("Record2");
			//m_RecordDef.ExpectGetAlways("RecType", "Record3");
			m_Document.Records[0].Records.Append("Record3");
			//m_RecordDef.ExpectGetAlways("RecType", "Record2");
			m_Document.Records[0].Records.Append("Record2");
			m_Document.Records[0].Records.Append("Record2");
			//m_RecordDef.ExpectGetAlways("RecType", "Record3");
			m_Document.Records[0].Records.Append("Record3");
			IWDEFilteredRecords frec = m_Document.FilteredRecords("Record1");
			Assert.AreEqual(1, frec.Count, "Record1 count");
			frec = m_Document.FilteredRecords("Record2");
			Assert.AreEqual(3, frec.Count, "Record2 count");
			frec = m_Document.FilteredRecords("Record3");
			Assert.AreEqual(2, frec.Count, "Record3 count");
			frec = m_Document.FilteredRecords("NotThere");
			Assert.AreEqual(0, frec.Count, "NotThere count");
		}
	}
}
