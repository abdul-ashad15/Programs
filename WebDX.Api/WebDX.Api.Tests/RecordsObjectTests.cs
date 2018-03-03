using System;
using System.Drawing;
using System.Xml;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;

using WebDX.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDX.Api.Moles;

namespace WebDX.Api.Tests
{
	/// <summary>
	/// Records Object Tests
	/// </summary>
	[TestClass]
	public class RecordsObjectTests
	{
		ResourceManager m_ResMan;
		IWDERecords m_Records;
		SIWDEDocument m_Document;
		SIWDERecordsOwner m_DocOwner;
		SIWDEDocSessions m_DocSessions;
		SIWDEDocSession_R1 m_DocSession;
		SIWDEDataSet m_DataSet;
		SIWDESessions m_Sessions;
		SIWDESession_R1 m_Session;
		SIWDEDocumentDef m_DocumentDef;
		SIWDERecordDef m_RecordDef;
		SIWDERecordDefs m_RecordDefs;
		SIWDEFieldDefs m_FieldDefs;
		SIWDEFieldDef m_FieldDef;
		SIWDEProject m_Project;
		SIWDEDocumentDefs m_DocumentDefs;

		public RecordsObjectTests()
		{
		}

		[TestInitialize]
		public void Init()
		{

			m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());
			m_DataSet = new SIWDEDataSet();
			m_DocSessions = new SIWDEDocSessions();
			m_DocSession = new SIWDEDocSession_R1();
			m_Document = new SIWDEDocument();
			m_DocOwner = new SIWDERecordsOwner();
			m_Sessions = new SIWDESessions();
			m_Session = new SIWDESession_R1();
			m_RecordDef = new SIWDERecordDef();
			m_FieldDefs = new SIWDEFieldDefs();
			m_FieldDef = new SIWDEFieldDef();
			m_RecordDefs = new SIWDERecordDefs();
			m_DocumentDef = new SIWDEDocumentDef();
			m_Project = new SIWDEProject();
			m_DocumentDefs = new SIWDEDocumentDefs();

			m_Project.DocumentDefsGet = () =>
			{
				return m_DocumentDefs;
			};
			m_DataSet.SessionsGet = () =>
				{
					return m_Sessions;
				};
			m_DataSet.DisplayDeletedRowsGet = () =>
				{
					return false;
				};
			m_DataSet.ProjectGet = () =>
				{
					return m_Project;
				};
			m_DataSet.PersistFlagsGet = () =>
				{
					return false;
				};
			m_Sessions.ItemGetInt32 = (index) =>
				{
					return m_Session;
				};
			m_Session.SessionIDGet01 = () =>
				{
					return 1;
				};
			m_Session.SessionIDGet = () =>
				{
					return 1;
				};
			m_Document.SessionsGet = () =>
				{
					return m_DocSessions;
				};
			m_Document.DocumentDefGet = () =>
				{
					return m_DocumentDef;
				};
			m_Document.DataSetGet = () =>
				{
					return m_DataSet;
				};
			m_Document.DocTypeGet = () =>
				{
					return "Document1";
				};
			m_DocSessions.ItemGetInt32 = (index) =>
				{
					return m_DocSession;
				};
			m_DocSession.SessionIDGet01 = () =>
				{
					return 1;
				};

			m_DocumentDefs.FindString = (name) =>
				{
					return 0;
				};
			m_DocumentDefs.ItemGetInt32 = (index) =>
				{
					return m_DocumentDef;
				};

			m_DocOwner.DocumentGet = () =>
				{
					return m_Document;
				};
			m_DocOwner.RecordGet = () =>
				{
					return null;
				};
			m_DocumentDef.RecordDefsGet = () =>
				{
					return m_RecordDefs;
				};
			m_RecordDef.FieldDefsGet = () =>
				{
					return m_FieldDefs;
				};
			m_RecordDef.RecTypeGet = () =>
				{
					return "Record1";
				};
			m_FieldDefs.CountGet = () =>
				{
					return 1;
				};
			m_FieldDefs.ItemGetInt32 = (index) =>
				{
					return m_FieldDef;
				};
			m_FieldDef.CharSetGet = () =>
				{
					return "";
				};
			m_FieldDef.DataLenGet = () =>
				{
					return 20;
				};
			m_FieldDef.OptionsGet = () =>
				{
					return WDEFieldOption.MustComplete;
				};
			m_FieldDef.DefaultValueGet = () =>
				{
					return "";
				};
			m_FieldDef.FieldNameGet = () =>
				{
					return "Field1";
				};
			m_RecordDefs.FindString = (name) =>
				{
					return 1;
				};
			m_RecordDefs.ItemGetInt32 = (index) =>
				{
					return m_RecordDef;
				};

			m_Records = WDERecords.Create((IWDERecordsOwner) m_DocOwner);
		}

		[TestCleanup]
		public void Cleanup()
		{
			m_Records = null;
			m_Document = null;
			m_DocOwner = null;
			m_DocSessions = null;
			m_DocSession = null;
			m_DataSet = null;
			m_Sessions = null;
			m_Session = null;
			m_DocumentDef = null;
			m_RecordDef = null;
			m_RecordDefs = null;
			m_FieldDefs = null;
			m_FieldDef = null;
			m_Project = null;
			m_DocumentDefs = null;

			GC.Collect();
		}

		[TestMethod]
		public void Append()
		{
			IWDERecord res = m_Records.Append("Record1");
			Assert.AreEqual(1, m_Records.Count, "Count");
			Assert.AreSame(res, m_Records[0], "Res does not match m_Records[0], expected match");
			Assert.AreEqual("Record1", m_Records[0].RecType, "RecType");
			Assert.AreEqual(1, m_Records[0].Fields.Count, "Fields.Count");
			Assert.AreEqual(0, m_Records.Index, "Index");
			m_Records.Clear();
			Assert.AreEqual(0, m_Records.Count, "Clear");
		}

		[TestMethod]
		public void AppendMulti()
		{
			m_Records.Append("Record1");
			m_Records.Append("Record1");
			m_Records.Append("Record1");
			Assert.AreEqual(3, m_Records.Count, "Count");
			Assert.AreEqual(2, m_Records.Index, "Index");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void AppendInvalid()
		{
			m_RecordDefs.FindString = (name) =>
				{
					return -1;
				};
			m_Records.Append("Invalid");
		}

		[TestMethod]
		public void Insert()
		{
			m_Records.Insert(0, "Record1");
			Assert.AreEqual(1, m_Records.Count, "Insert with no records");
			m_RecordDef.RecTypeGet = () =>
				{
					return "Record2";
				};
			m_Records.Insert(0, "Record2");
			Assert.AreEqual(2, m_Records.Count, "FirstInsert");
			Assert.AreEqual("Record2", m_Records[0].RecType, "RecType2");
			Assert.AreEqual("Record1", m_Records[1].RecType, "RecType1");
			m_Records.Clear();

			m_RecordDef.RecTypeGet = () =>
				{
					return "Record1";
				};
			m_Records.Append("Record1");
			m_Records.Append("Record1");
			m_Records.Append("Record1");
			m_RecordDef.RecTypeGet = () =>
			{
				return "Record2";
			};
			m_Records.Insert(1, "Record2");
			Assert.AreEqual(4, m_Records.Count, "MiddleInsert");
			Assert.AreEqual("Record2", m_Records[1].RecType, "RecTypeMiddle");
			m_Records.Clear();
			m_RecordDef.RecTypeGet = () =>
			{
				return "Record1";
			};
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void InsertIndexLow()
		{
			m_Records.Append("Record1");
			m_Records.Insert(-1, "Record1");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void InsertIndexHigh()
		{
			m_Records.Append("Record1");
			m_Records.Insert(20, "Record1");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void InsertIndexHighPlus()
		{
			m_Records.Append("Record1");
			m_Records.Insert(1, "Record1");
		}

		[TestMethod]
		public void Delete()
		{
			m_Records.Append("Record1");
			m_RecordDef.RecTypeGet = () =>
			{
				return "Record2";
			};
			m_Records.Append("Record2");
			m_RecordDef.RecTypeGet = () =>
			{
				return "Record3";
			};
			m_Records.Append("Record3");

			m_Records.Delete();
			Assert.AreEqual(2, m_Records.Count, "First delete");
			Assert.AreEqual("Record1", m_Records[0].RecType, "Rec1 first");
			Assert.AreEqual("Record2", m_Records[1].RecType, "Rec2 first");
			
			m_Records.Delete();
			Assert.AreEqual(1, m_Records.Count, "Second delete");
			Assert.AreEqual("Record1", m_Records[0].RecType, "Rec1 second");

			m_Records.Delete();
			Assert.AreEqual(0, m_Records.Count, "Third delete");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DeleteNoCurrent()
		{
			m_Records.Delete();
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DeleteNoCurrentAppend()
		{
			m_Records.Append("Record1");
			m_Records.Delete();
			m_Records.Delete();
		}

		[TestMethod]
		public void Current()
		{
			Assert.IsNull(m_Records.Current, "Current is not null, expected null");

			m_Records.Append("Record1");
			Assert.IsNotNull(m_Records.Current, "Current is null, expected not null");
		}

		[TestMethod]
		public void First()
		{
			IWDERecord firstRec = m_Records.Append("Record1");
			m_Records.Append("Record1");
			m_Records.Append("Record1");

			m_Records.First();
			Assert.AreSame(firstRec, m_Records.Current, "first and current are not the same, expected same");
			Assert.IsTrue(m_Records.BOF, "BOF is false, expected true");
			Assert.IsFalse(m_Records.EOF, "EOF is true, expected false");
			Assert.AreEqual(0, m_Records.Index, "Index");

			m_Records.Index = 2;
			Assert.IsFalse(m_Records.BOF, "BOF is true, expected false");
			Assert.IsFalse(m_Records.EOF, "EOF is true after index, expected false");
			m_Records.First();
			Assert.AreSame(firstRec, m_Records.Current, "first and current are not the same after index, expected same");
			Assert.IsTrue(m_Records.BOF, "BOF is false, expected true after index");
			Assert.IsFalse(m_Records.EOF, "EOF is true after second First call, expected false");
			Assert.AreEqual(0, m_Records.Index, "Index2");
		}

		[TestMethod]
		public void Last()
		{
			m_Records.Append("Record1");
			m_Records.Append("Record1");
			IWDERecord lastRec = m_Records.Append("Record1");

			m_Records.Last();
			Assert.AreSame(lastRec, m_Records.Current, "last and current are not the same, expected same");
			Assert.IsFalse(m_Records.BOF, "BOF is true, expected false");
			Assert.IsFalse(m_Records.EOF, "EOF is true, expected false");
			Assert.AreEqual(2, m_Records.Index, "Index");

			m_Records.First();
			Assert.AreEqual(0, m_Records.Index, "Index");
			m_Records.Last();
			Assert.AreSame(lastRec, m_Records.Current, "last and current are not the same, expected same");
			Assert.IsFalse(m_Records.BOF, "BOF is true, expected false");
			Assert.IsFalse(m_Records.EOF, "EOF is true, expected false");
			Assert.AreEqual(2, m_Records.Index, "Index2");
		}

		[TestMethod]
		public void Next()
		{
			IWDERecord firstRec = m_Records.Append("Record1");
			IWDERecord midRec = m_Records.Append("Record1");
			IWDERecord lastRec = m_Records.Append("Record1");

			m_Records.First();
			Assert.AreSame(firstRec, m_Records.Current, "first and current are not the same, expected same");
			Assert.IsTrue(m_Records.BOF, "First BOF is false, expected true");
			Assert.IsFalse(m_Records.EOF, "First EOF is false, expected false");
			Assert.AreEqual(0, m_Records.Index, "Index1");

			m_Records.Next();
			Assert.AreSame(midRec, m_Records.Current, "mid and current are not the same, expected same");
			Assert.IsFalse(m_Records.BOF, "Mid BOF is true, expected false");
			Assert.IsFalse(m_Records.EOF, "Mid EOF is true, expected false");
			Assert.AreEqual(1, m_Records.Index, "Index2");

			m_Records.Next();
			Assert.AreSame(lastRec, m_Records.Current, "last and current are not the same, expected same");
			Assert.IsFalse(m_Records.BOF, "Last BOF is true, expected false");
			Assert.IsFalse(m_Records.EOF, "Last EOF is true, expected false");
			Assert.AreEqual(2, m_Records.Index, "Index2");

			m_Records.Next();
			Assert.AreSame(lastRec, m_Records.Current, "EOF last and current are not the same, expected same");
			Assert.IsFalse(m_Records.BOF, "EOF Last BOF is true, expected false");
			Assert.IsTrue(m_Records.EOF, "EOF Last EOF is false, expected true");
			Assert.AreEqual(2, m_Records.Index, "Index3");
		}

		[TestMethod]
		public void Prior()
		{
			IWDERecord firstRec = m_Records.Append("Record1");
			IWDERecord midRec = m_Records.Append("Record1");
			IWDERecord lastRec = m_Records.Append("Record1");

			m_Records.Last();
			Assert.AreSame(lastRec, m_Records.Current, "last and current are not same, expected same");
			Assert.IsFalse(m_Records.BOF, "last BOF is true, expected false");
			Assert.IsFalse(m_Records.EOF, "last EOF is true, expected false");
			Assert.AreEqual(2, m_Records.Index, "Index1");

			m_Records.Prior();
			Assert.AreSame(midRec, m_Records.Current, "mid and current are not same, expected same");
			Assert.IsFalse(m_Records.BOF, "mid BOF is true, expected false");
			Assert.IsFalse(m_Records.EOF, "mid EOF is true, expected false");
			Assert.AreEqual(1, m_Records.Index, "Index2");

			m_Records.Prior();
			Assert.AreSame(firstRec, m_Records.Current, "first and current are not same, expected same");
			Assert.IsFalse(m_Records.BOF, "first BOF is true, expected false");
			Assert.IsFalse(m_Records.EOF, "first EOF is true, expected false");
			Assert.AreEqual(0, m_Records.Index, "Index3");

			m_Records.Prior();
			Assert.AreSame(firstRec, m_Records.Current, "BOF first and current are not same, expected same");
			Assert.IsTrue(m_Records.BOF, "BOF first BOF is false, expected true");
			Assert.IsFalse(m_Records.EOF, "BOF first EOF is true, expected false");
			Assert.AreEqual(0, m_Records.Index, "Index4");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void IndexEmpty()
		{
			m_Records.Index = 0;
		}

		[TestMethod]
		public void Index()
		{
			IWDERecord firstRec = m_Records.Append("Record1");
			IWDERecord midRec = m_Records.Append("Record1");
			IWDERecord lastRec = m_Records.Append("Record1");

			m_Records.Index = 0;
			Assert.AreSame(firstRec, m_Records.Current, "");
			Assert.IsFalse(m_Records.BOF, "first BOF is true, expected false");
			Assert.IsFalse(m_Records.EOF, "first EOF is true, expected false");
			m_Records.Index = 1;
			Assert.AreSame(midRec, m_Records.Current, "");
			Assert.IsFalse(m_Records.BOF, "mid BOF is true, expected false");
			Assert.IsFalse(m_Records.EOF, "mid EOF is true, expected false");
			m_Records.Index = 2;
			Assert.AreSame(lastRec, m_Records.Current, "");
			Assert.IsFalse(m_Records.BOF, "last BOF is true, expected false");
			Assert.IsFalse(m_Records.EOF, "last EOF is true, expected false");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void IndexHigh()
		{
			m_Records.Append("Record1");
			m_Records.Index = 1;
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void IndexLow()
		{
			m_Records.Append("Record1");
			m_Records.Index = -1;
		}

		[TestMethod]
		public void Filter()
		{
			m_Records.Append("Record1");
			m_Records.Append("Record1");
			m_RecordDef.RecTypeGet = () =>
			{
				return "Record2";
			};
			m_Records.Append("Record2");

			IWDEFilteredRecords filtered1 = ((IWDERecords_R1)m_Records).Filter("Record1");
			IWDEFilteredRecords filtered2 = ((IWDERecords_R1)m_Records).Filter("Record2");
			Assert.AreEqual<int>(2, filtered1.Count);
			Assert.AreEqual<int>(1, filtered2.Count);
			Assert.AreEqual<int>(3, m_Records.Count);

			Assert.AreEqual<string>("Record1", filtered1[0].RecType);
			Assert.AreEqual<string>("Record1", filtered1[1].RecType);

			Assert.AreEqual<string>("Record2", filtered2[0].RecType);

			Assert.AreEqual<string>("Record1", m_Records[0].RecType);
			Assert.AreEqual<string>("Record1", m_Records[1].RecType);
			Assert.AreEqual<string>("Record2", m_Records[2].RecType);
		}

		[TestMethod]
		public void WriteToXml()
		{
			m_Records.Append("Record1");
			m_Records.Append("Record1");
			m_Records.Append("Record1");

			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = m_Records as IWDEXmlPersist;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("RecordsWriteToXml"), sw.ToString());
		}

		[TestMethod]
		public void ReadFromXml()
		{
			string test = "<DataSet>" + m_ResMan.GetString("RecordsWriteToXml") + "</DataSet>";
			StringReader sr = new StringReader(test);
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			IWDEXmlPersist ipers = m_Records as IWDEXmlPersist;
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual(3, m_Records.Count, "Count");
			Assert.AreEqual("Record1", m_Records[0].RecType, "RecType");
		}

		[TestMethod]
		public void FilterRecords_AllMatch()
		{
			m_Records.Append("Record1");
			m_Records.Append("Record1");
			m_Records.Append("Record1");

			IWDERecords_R1 recs = m_Records as IWDERecords_R1;
			IWDEFilteredRecords filtered = recs.Filter("Record1");
			Assert.AreEqual<int>(3, filtered.Count);
		}

		[TestMethod]
		public void FilterRecords_NoMatch()
		{
			m_Records.Append("Record1");
			m_Records.Append("Record1");
			m_Records.Append("Record1");

			IWDERecords_R1 recs = m_Records as IWDERecords_R1;
			IWDEFilteredRecords filtered = recs.Filter("Record2");
			Assert.AreEqual<int>(0, filtered.Count);
		}

		[TestMethod]
		public void FilterRecords_SomeMatch()
		{
			m_Records.Append("Record1");
			m_Records.Append("Record1");
			m_RecordDef.RecTypeGet = () =>
				{
					return "Record2";
				};
			m_Records.Append("Record2");

			IWDERecords_R1 recs = m_Records as IWDERecords_R1;
			IWDEFilteredRecords filtered = recs.Filter("Record1");
			Assert.AreEqual<int>(2, filtered.Count);
		}
	}
}
