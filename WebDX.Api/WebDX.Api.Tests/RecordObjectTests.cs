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
	/// Record Object Tests
	/// </summary>
	[TestClass]
	public class RecordObjectTests
	{
		ResourceManager m_ResMan;
		IWDERecord m_Record;
		MockObject m_Document;
		MockObject m_DocDef;
		MockObject m_DocSessions;
		MockObject m_DocSession;
		MockObject m_DataSet;
		MockObject m_Sessions;
		MockObject m_Session;
		MockObject m_Records;
		MockObject m_RecordDefs;
		MockObject m_RecordDef;
		MockObject m_FieldDefs;
		MockObject m_FieldDef;
		MockObject m_Project;

		public RecordObjectTests()
		{
		}

		[TestInitialize]
		public void Init()
		{
			MockManager.Init();

			m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());
			m_DataSet = MockManager.MockObject(typeof(IWDEDataSet));
			m_DocSessions = MockManager.MockObject(typeof(IWDEDocSessions));
			m_DocSession = MockManager.MockObject(typeof(IWDEDocSession_R1));
			m_Document = MockManager.MockObject(typeof(IWDEDocument));
			m_DocDef = MockManager.MockObject(typeof(IWDEDocumentDef));
			m_Sessions = MockManager.MockObject(typeof(IWDESessions));
			m_Session = MockManager.MockObject(typeof(IWDESession_R1));
			m_Records = MockManager.MockObject(typeof(IWDERecords));
			m_RecordDefs = MockManager.MockObject(typeof(IWDERecordDefs));
			m_RecordDef = MockManager.MockObject(typeof(IWDERecordDef));
			m_FieldDefs = MockManager.MockObject(typeof(IWDEFieldDefs));
			m_FieldDef = MockManager.MockObject(typeof(IWDEFieldDef));
			m_Project = MockManager.MockObject(typeof(IWDEProject));

			m_RecordDef.ExpectGetAlways("RecType", "Record1");
			m_RecordDef.ExpectGetAlways("FieldDefs", m_FieldDefs.Object);
			m_FieldDefs.ExpectGetAlways("Count", 1);
			m_FieldDefs.ExpectGetAlways("Item", m_FieldDef.Object);
			m_FieldDef.ExpectGetAlways("CharSet", "");
			m_FieldDef.ExpectGetAlways("DataLen", 20);
			m_FieldDef.ExpectGetAlways("Options", WDEFieldOption.None);
			m_FieldDef.ExpectGetAlways("DefaultValue", "");
			m_FieldDef.ExpectGetAlways("FieldName", "Field1");

			m_DataSet.ExpectGetAlways("Sessions", m_Sessions.Object);
			m_DataSet.ExpectGetAlways("PersistFlags", false);
			m_DataSet.ExpectGetAlways("Project", m_Project.Object);
			m_Sessions.ExpectGetAlways("Item", m_Session.Object);
			m_Session.ExpectGetAlways("SessionID", 1);
			m_Document.ExpectGetAlways("DataSet", m_DataSet.Object);
			m_Document.ExpectGetAlways("DocumentDef", m_DocDef.Object);
			m_Document.ExpectGetAlways("Sessions", m_DocSessions.Object);
			m_Document.ExpectGetAlways("DocType", "Document1");
			m_DocSessions.ExpectGetAlways("Item", m_DocSession.Object);
			m_DocSession.ExpectGetAlways("SessionID", 1);

			m_DocDef.ExpectGetAlways("RecordDefs", m_RecordDefs.Object);
			m_RecordDefs.AlwaysReturn("Find", 0, null);
			m_RecordDefs.ExpectGetAlways("Item", m_RecordDef.Object);
			m_Records.ExpectGetAlways("Document", m_Document.Object);
			m_Records.ExpectGetAlways("OwnerRecord", null);
			
			m_Record = WDERecord.Create((IWDERecords) m_Records.Object);
		}

		[TestCleanup]
		public void Cleanup()
		{
			m_Record = null;
			m_Document = null;
			m_DocSessions = null;
			m_DocSession = null;
			m_DataSet = null;
			m_Sessions = null;
			m_Session = null;
			m_Records = null;
			m_RecordDefs = null;
			m_RecordDef = null;
			m_FieldDefs = null;
			m_FieldDef = null;
			m_Project = null;
			m_DocDef = null;

			MockManager.Verify();
			GC.Collect();
		}

		[TestMethod]
		public void CreateMissingFields()
		{
			MockObject fielddefs = MockManager.MockObject(typeof(IWDEFieldDefs));
			m_RecordDef.ExpectGetAlways("FieldDefs", fielddefs.Object);
			MockObject fielddef = MockManager.MockObject(typeof(IWDEFieldDef));
			fielddefs.ExpectGetAlways("Item", fielddef.Object);
			fielddef.ExpectGet("FieldName", "Field1");
			fielddef.ExpectGet("FieldName", "Field2");
			fielddef.ExpectGet("FieldName", "Field1");
			fielddef.ExpectGet("FieldName", "Field2");
			fielddef.ExpectGet("FieldName", "Field3");
			fielddef.ExpectGet("FieldName", "Field3");
			fielddef.ExpectGetAlways("CharSet", "");
			fielddef.ExpectGetAlways("DataLen", 20);
			fielddef.ExpectGetAlways("Options", WDEFieldOption.None);
			fielddef.ExpectGetAlways("DefaultValue", "");

			IWDERecordInternal irec = m_Record as IWDERecordInternal;
			fielddefs.ExpectGetAlways("Count", 2);
			irec.CreateFields((IWDERecordDef) m_RecordDef.Object);
			Assert.AreEqual(2, m_Record.Fields.Count, "2 fields");
			Assert.AreEqual("Field1", m_Record.Fields[0].FieldName, "Field1");
			Assert.AreEqual("Field2", m_Record.Fields[1].FieldName, "Field2");
			fielddefs.ExpectGetAlways("Count", 3);
			Assert.IsNotNull(m_Record.FindField("Field3"));
			Assert.AreEqual(3, m_Record.Fields.Count, "3 fields");
			Assert.AreEqual("Field1", m_Record.Fields[0].FieldName, "Field1");
			Assert.AreEqual("Field2", m_Record.Fields[1].FieldName, "Field2");
			Assert.AreEqual("Field3", m_Record.Fields[2].FieldName, "Field3");
		}

		[TestMethod]
		public void RecordDef()
		{
			Assert.IsNotNull(m_Record.RecordDef, "RecordDef is null. Expected not null.");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void RecordDef_NP()
		{
			m_DataSet.ExpectGetAlways("Project", null);
			IWDERecordDef def = m_Record.RecordDef;
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void RecordDef_NR()
		{
			m_RecordDefs.AlwaysReturn("Find", -1, null);
			
			IWDERecordInternal irec = m_Record as IWDERecordInternal;
			irec.RecType = "Record1";

			IWDERecordDef def = m_Record.RecordDef;
		}

		[TestMethod]
		public void CreateFields()
		{
			IWDERecordInternal irec = m_Record as IWDERecordInternal;

			m_FieldDefs.ExpectGetAlways("Count", 3);
			MockObject fielddef = MockManager.MockObject(typeof(IWDEFieldDef));
			m_FieldDefs.ExpectGetAlways("Item", fielddef.Object);
			fielddef.ExpectGet("FieldName", "Field1");
			fielddef.ExpectGet("FieldName", "Field2");
			fielddef.ExpectGet("FieldName", "Field3");
			fielddef.ExpectGetAlways("CharSet", "");
			fielddef.ExpectGetAlways("DataLen", 20);
			fielddef.ExpectGetAlways("Options", WDEFieldOption.None);
			fielddef.ExpectGetAlways("DefaultValue", "");

			IWDERecordDef def = (IWDERecordDef) m_RecordDef.Object;
			irec.CreateFields(def);
			m_FieldDef.Verify();
			Assert.AreSame(def, m_Record.RecordDef, "rec and RecordDef do not match");
			Assert.AreEqual(3, m_Record.Fields.Count, "Fields.Count");
			Assert.AreEqual("Field1", m_Record.Fields[0].FieldName, "Field1");
			Assert.AreEqual("Field2", m_Record.Fields[1].FieldName, "Field2");
			Assert.AreEqual("Field3", m_Record.Fields[2].FieldName, "Field3");
		}

		[TestMethod]
		public void FlaggedFields()
		{
			Assert.AreEqual(0, m_Record.FlaggedFieldCount, "NoFields");

			m_FieldDefs.ExpectGetAlways("Count", 3);
			IWDERecordInternal irec = m_Record as IWDERecordInternal;
			irec.CreateFields((IWDERecordDef) m_RecordDef.Object);
			m_Record.Fields[0].Flagged = true;
			m_Record.Fields[2].Flagged = true;

			Assert.AreEqual(2, m_Record.FlaggedFieldCount, "3 Fields");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void FieldByName()
		{
			IWDERecordInternal irec = m_Record as IWDERecordInternal;
			irec.CreateFields((IWDERecordDef) m_RecordDef.Object);
			IWDEField res = m_Record.FindField("Field1");
			Assert.IsNotNull(res, "Res is null, expected not null");
			Assert.AreEqual("Field1", res.FieldName, "Field1");
			res = m_Record.FieldByName("NotThere");
		}

		[TestMethod]
		public void FindField()
		{
			m_FieldDefs.ExpectGetAlways("Count", 3);
			MockObject fielddef = MockManager.MockObject(typeof(IWDEFieldDef));
			m_FieldDefs.ExpectGetAlways("Item", fielddef.Object);
			fielddef.ExpectGet("FieldName", "Field1");
			fielddef.ExpectGet("FieldName", "Field2");
			fielddef.ExpectGet("FieldName", "Field3");
			fielddef.ExpectGet("FieldName", "Field1");
			fielddef.ExpectGet("FieldName", "Field2");
			fielddef.ExpectGet("FieldName", "Field3");
			fielddef.ExpectGetAlways("CharSet", "");
			fielddef.ExpectGetAlways("DataLen", 20);
			fielddef.ExpectGetAlways("Options", WDEFieldOption.None);
			fielddef.ExpectGetAlways("DefaultValue", "");
			IWDERecordInternal irec = m_Record as IWDERecordInternal;
			irec.CreateFields((IWDERecordDef) m_RecordDef.Object);

			IWDEField res = m_Record.FindField("Field1");
			Assert.IsNotNull(res, "Res is null Field1");
			res = m_Record.FindField("Field2");
			Assert.IsNotNull(res, "Res is null Field2");
			res = m_Record.FindField("Field3");
			Assert.IsNotNull(res, "Res is null Field3");
			res = m_Record.FindField("NotThere");
			Assert.IsNull(res, "Res is not null NotThere");
		}

		[TestMethod]
		public void WriteToXmlDefault()
		{
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = m_Record as IWDEXmlPersist;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("RecordWriteToXmlDefault"), sw.ToString());
		}

		[TestMethod]
		public void WriteToXmlFull()
		{
			m_FieldDefs.ExpectGetAlways("Count", 3);
			MockObject fielddef = MockManager.MockObject(typeof(IWDEFieldDef));
			m_FieldDefs.ExpectGetAlways("Item", fielddef.Object);
			fielddef.ExpectGet("FieldName", "Field1");
			fielddef.ExpectGet("FieldName", "Field2");
			fielddef.ExpectGet("FieldName", "Field3");
			fielddef.ExpectGetAlways("CharSet", "");
			fielddef.ExpectGetAlways("DataLen", 20);
			fielddef.ExpectGetAlways("Options", WDEFieldOption.None);
			fielddef.ExpectGetAlways("DefaultValue", "");

			m_Record.DetailRect = new Rectangle(10, 20, 30, 40);
			IWDERecordInternal irec = m_Record as IWDERecordInternal;
			irec.RecType = "Record1";
			irec.CreateFields((IWDERecordDef) m_RecordDef.Object);
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = m_Record as IWDEXmlPersist;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("RecordWriteToXmlFull"), sw.ToString());
		}

		[TestMethod]
		public void ReadFromXmlDefault()
		{
			StringReader sr = new StringReader(m_ResMan.GetString("RecordWriteToXmlDefault"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			IWDEXmlPersist ipers = m_Record as IWDEXmlPersist;
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual("", m_Record.RecType, "RecType");
			Assert.IsTrue(m_Record.DetailRect.IsEmpty, "DetailRect is not empty, expected empty");
			Assert.AreEqual(0, m_Record.Fields.Count, "Fields.Count");
		}

		[TestMethod]
		public void ReadFromXmlFull()
		{
			StringReader sr = new StringReader(m_ResMan.GetString("RecordWriteToXmlFull"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			IWDEXmlPersist ipers = m_Record as IWDEXmlPersist;
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual("Record1", m_Record.RecType, "RecType");
			Assert.AreEqual(new Rectangle(10, 20, 30, 40), m_Record.DetailRect, "DetailRect");
			Assert.AreEqual(3, m_Record.Fields.Count, "Fields.Count");
			Assert.AreEqual("Field1", m_Record.Fields[0].FieldName, "Field1");
			Assert.AreEqual("Field2", m_Record.Fields[1].FieldName, "Field2");
			Assert.AreEqual("Field3", m_Record.Fields[2].FieldName, "Field3");
		}
	}
}
