using System;
using System.Collections;
using System.Collections.Specialized;
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
	public class DataSetObjectTests
	{
		ResourceManager m_ResMan;
		IWDEDataSet m_DataSet;

        private IWDEProject m_Project;
        private IWDERejectCodes m_RejectCodes;
        private IWDEDocumentDefs m_DocumentDefs;
        private IWDEDocumentDef m_DocumentDef;
        private IWDERecordDefs m_RecordDefs;
        private IWDERecordDef m_RecordDef;
        private IWDEFieldDefs m_FieldDefs;
        private IWDEFieldDef m_FieldDef;
        private IWDEImageSourceDefs m_ImageSourceDefs;
        private IWDEImageSourceDef m_ImageSourceDef;


        //MockObject m_Project;
        //MockObject m_RejectCodes;
        //MockObject m_DocumentDefs;
        //MockObject m_DocumentDef;
        //MockObject m_RecordDefs;
        //MockObject m_RecordDef;
        //MockObject m_FieldDefs;
        //MockObject m_FieldDef;
        //MockObject m_ImageSourceDefs;
        //MockObject m_ImageSourceDef;

        public DataSetObjectTests()
		{
		}

		[TestInitialize]
		public void Init()
		{
			//MockManager.Init();

			m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());

			//m_Project = MockManager.MockObject(typeof(IWDEProject));

			//m_RejectCodes = MockManager.MockObject(typeof(IWDERejectCodes));
			//m_DocumentDefs = MockManager.MockObject(typeof(IWDEDocumentDefs));
			//m_DocumentDef = MockManager.MockObject(typeof(IWDEDocumentDef));
			//m_RecordDefs = MockManager.MockObject(typeof(IWDERecordDefs));
			//m_RecordDef = MockManager.MockObject(typeof(IWDERecordDef));
			//m_FieldDef = MockManager.MockObject(typeof(IWDEFieldDef));
			//m_FieldDefs = MockManager.MockObject(typeof(IWDEFieldDefs));
			//m_ImageSourceDefs = MockManager.MockObject(typeof(IWDEImageSourceDefs));
			//m_ImageSourceDef = MockManager.MockObject(typeof(IWDEImageSourceDef));

			//m_Project.ExpectGetAlways("RejectCodes", m_RejectCodes.Object);
			//m_Project.ExpectGetAlways("DocumentDefs", m_DocumentDefs.Object);
			//m_DocumentDefs.AlwaysReturn("Find", 0);
			//m_DocumentDefs.ExpectGetAlways("Item", m_DocumentDef.Object);
			//m_DocumentDef.ExpectGetAlways("RecordDefs", m_RecordDefs.Object);
			//m_DocumentDef.ExpectGetAlways("DocType", "Document1");
			//m_RejectCodes.AlwaysReturn("Find", -1);

			//m_RecordDefs.ExpectGetAlways("Count", 1);
			//m_RecordDefs.AlwaysReturn("Find", 0);
			//m_RecordDefs.ExpectGetAlways("Item", m_RecordDef.Object);
			//m_RecordDef.ExpectGetAlways("FieldDefs", m_FieldDefs.Object);
			//m_RecordDef.ExpectGetAlways("RecType", "Record1");
			
			//m_FieldDefs.ExpectGetAlways("Count", 1);
			//m_FieldDefs.ExpectGetAlways("Item", m_FieldDef.Object);
			//m_FieldDef.ExpectGetAlways("FieldName", "Field1");
			//m_FieldDef.ExpectGetAlways("CharSet", "");
			//m_FieldDef.ExpectGetAlways("DataLen", 20);
			//m_FieldDef.ExpectGetAlways("Options", WDEFieldOption.None);
			//m_FieldDef.ExpectGetAlways("DefaultValue", "1234");

			m_DataSet = WDEDataSet.Create();
            m_DataSet.Project = (IWDEProject)m_Project;
		}

		[TestCleanup]
		public void Cleanup()
		{
			m_DataSet = null;
			m_Project = null;
			m_RejectCodes = null;
			m_DocumentDefs = null;
			m_DocumentDef = null;
			m_RecordDefs = null;
			m_RecordDef = null;
			m_FieldDefs = null;
			m_FieldDef = null;
			m_ImageSourceDefs = null;
			m_ImageSourceDef = null;

			//MockManager.Verify();
			GC.Collect();
		}

		[TestMethod]
		public void SessionConversion()
		{
			m_DataSet.Project = null;

			string data = m_ResMan.GetString("SessionConversion");
			MemoryStream ms = new MemoryStream();
			using (StreamWriter sw = new StreamWriter(ms))
				sw.Write(data);
			ms = new MemoryStream(ms.ToArray());

			m_DataSet.LoadFromStream("User", "Task", WDEOpenMode.Resume, ms);
			Assert.AreEqual(3, m_DataSet.Sessions.Count, "Session Count");
			Assert.AreEqual(3, m_DataSet.Documents[0].Sessions.Count, "DocSession Count");

			foreach (IWDESession sess in m_DataSet.Sessions)
			{
				int index = m_DataSet.Documents[0].Sessions.FindByID(sess.SessionID);
				Assert.AreNotEqual(-1, index, "Session " + sess.SessionID.ToString() + " not found.");
				IWDEDocSession_R2 docsess = (IWDEDocSession_R2)m_DataSet.Documents[0].Sessions[index];
				Assert.AreEqual(sess.StartTime, docsess.StartTime, "StartTime " + docsess.SessionID);
				Assert.AreEqual(sess.EndTime, docsess.EndTime, "EndTime " + docsess.SessionID);
			}
		}

		[TestMethod]
		public void SessionConversion_NotResume()
		{
			m_DataSet.Project = null;

			string data = m_ResMan.GetString("SessionConversion");
			MemoryStream ms = new MemoryStream();
			using (StreamWriter sw = new StreamWriter(ms))
				sw.Write(data);
			ms = new MemoryStream(ms.ToArray());

			m_DataSet.LoadFromStream("User", "Task", WDEOpenMode.Edit, ms);
			Assert.AreEqual(4, m_DataSet.Sessions.Count, "Session Count");
			Assert.AreEqual(4, m_DataSet.Documents[0].Sessions.Count, "DocSession Count");

			foreach (IWDESession sess in m_DataSet.Sessions)
			{
				int index = m_DataSet.Documents[0].Sessions.FindByID(sess.SessionID);
				Assert.AreNotEqual(-1, index, "Session " + sess.SessionID.ToString() + " not found.");
				IWDEDocSession_R2 docsess = (IWDEDocSession_R2)m_DataSet.Documents[0].Sessions[index];
				Assert.AreEqual(sess.StartTime, docsess.StartTime, "StartTime " + docsess.SessionID);
				Assert.AreEqual(sess.EndTime, docsess.EndTime, "EndTime " + docsess.SessionID);
			}
		}

		[TestMethod]
		public void RebuildSessions()
		{
			m_DataSet.Project = null;

			string data = m_ResMan.GetString("RebuildSessions");
			MemoryStream ms = new MemoryStream();
			using (StreamWriter sw = new StreamWriter(ms))
				sw.Write(data);
			ms = new MemoryStream(ms.ToArray());

			m_DataSet.LoadFromStream("User", "Task", WDEOpenMode.Resume, ms);
			Assert.AreEqual(3, m_DataSet.Sessions.Count, "Session Count");
			Assert.AreEqual(3, m_DataSet.Documents[0].Sessions.Count, "DocSession Count");

			foreach (IWDESession sess in m_DataSet.Sessions)
			{
				int index = m_DataSet.Documents[0].Sessions.FindByID(sess.SessionID);
				Assert.AreNotEqual(-1, index, "Session " + sess.SessionID.ToString() + " not found.");
				IWDEDocSession_R2 docsess = (IWDEDocSession_R2)m_DataSet.Documents[0].Sessions[index];
				Assert.AreEqual(sess.StartTime, docsess.StartTime, "StartTime " + docsess.SessionID);
				Assert.AreEqual(sess.EndTime, docsess.EndTime, "EndTime " + docsess.SessionID);
			}
		}

		[TestMethod]
		public void MergeDoc()
		{
			m_DataSet.Project = null;

			string data = m_ResMan.GetString("RebuildSessions");
			MemoryStream ms = new MemoryStream();
			using (StreamWriter sw = new StreamWriter(ms))
				sw.Write(data);
			ms = new MemoryStream(ms.ToArray());

			((IWDEDataSet_R1)m_DataSet).MergeDocumentFromStream(ms);
			Assert.AreEqual(3, m_DataSet.Sessions.Count, "Session Count");
			Assert.AreEqual(3, m_DataSet.Documents[0].Sessions.Count, "DocSession Count");

			for(int i = m_DataSet.Sessions.Count - 1; i > 0; i--)
			{
				IWDESession sess = m_DataSet.Sessions[i];
				IWDEDocSession_R2 docsess = (IWDEDocSession_R2)m_DataSet.Documents[0].Sessions[i];
				Assert.AreEqual(sess.SessionID, docsess.SessionID, "SessionID " + i.ToString());
				Assert.AreEqual(sess.StartTime, docsess.StartTime, "StartTime " + docsess.SessionID);
				Assert.AreEqual(sess.EndTime, docsess.EndTime, "EndTime " + docsess.SessionID);
			}
		}

		[TestMethod]
		public void MergeDoc_Multi()
		{
			m_DataSet.Project = null;

			string data = m_ResMan.GetString("MergeDocMulti1");
			MemoryStream ms = new MemoryStream();
			using (StreamWriter sw = new StreamWriter(ms))
				sw.Write(data);
			ms = new MemoryStream(ms.ToArray());

			((IWDEDataSet_R1)m_DataSet).MergeDocumentFromStream(ms);
			data = m_ResMan.GetString("MergeDocMulti2");
			ms = new MemoryStream();
			using (StreamWriter sw = new StreamWriter(ms))
				sw.Write(data);
			ms = new MemoryStream(ms.ToArray());
			((IWDEDataSet_R1)m_DataSet).MergeDocumentFromStream(ms);

			Assert.AreEqual(2, m_DataSet.Documents.Count, "Document count");
			Assert.AreEqual(3, m_DataSet.Sessions.Count, "Session Count");
			Assert.AreEqual(3, m_DataSet.Documents[0].Sessions.Count, "DocSession Count");

			foreach (IWDESession sess in m_DataSet.Sessions)
			{
				int index = m_DataSet.Documents[0].Sessions.FindByID(sess.SessionID);
				int index2 = m_DataSet.Documents[1].Sessions.FindByID(sess.SessionID);
				IWDEDocSession_R2 docsess = (IWDEDocSession_R2)m_DataSet.Documents[0].Sessions[index];
				IWDEDocSession_R2 docsess2 = (IWDEDocSession_R2)m_DataSet.Documents[1].Sessions[index2];
				Assert.AreEqual(sess.Location, docsess.Location, "Location " + docsess.SessionID);
				Assert.AreEqual(sess.Mode, docsess.Mode, "Mode " + docsess.SessionID);
				Assert.AreEqual(sess.Task, docsess.Task, "Task " + docsess.SessionID);
				Assert.AreEqual(sess.User, docsess.User, "User " + docsess.SessionID);
				Assert.AreEqual(sess.StartTime, docsess.StartTime, "StartTime " + docsess.SessionID);
				Assert.AreEqual(sess.EndTime, docsess2.EndTime, "EndTime " + docsess.SessionID);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void DocDefsNP()
		{
			m_DataSet.Project = null;
			int except = m_DataSet.DocumentDefs.Count;
		}

		[TestMethod]
		public void About()
		{
			string res = m_DataSet.About;
			Assert.AreEqual("Version " + VersionInfo.VersionNumber, res, "About");

			m_DataSet.About = "NOTHING";
			string res2 = m_DataSet.About;
			Assert.AreEqual(res, res2, "About2");
		}

		[TestMethod]
		public void ClearDataND()
		{
			m_DataSet.ClearData();
		}

		[TestMethod]
		public void ClearData()
		{
			m_DataSet.CreateData("User", "Task", WDEOpenMode.Create);
			m_DataSet.Append("Document1");
			Assert.AreEqual(1, m_DataSet.Documents.Count, "Documents.Count");
			Assert.AreEqual(1, m_DataSet.Documents[0].Sessions.Count, "Document.Sessions.Count");
			Assert.AreEqual(1, m_DataSet.Sessions.Count, "Sessions.Count");
			m_DataSet.ClearData();
			Assert.AreEqual(1, m_DataSet.Documents.Count, "Documents.Count");
			Assert.AreEqual(0, m_DataSet.Documents[0].Sessions.Count, "Document.Sessions.Count");
			Assert.AreEqual(0, m_DataSet.Sessions.Count, "Sessions.Count");
		}

		[TestMethod]
		public void Clear()
		{
			m_DataSet.CreateData("User", "Task", WDEOpenMode.Create);
			m_DataSet.Append("Document1");
			Assert.AreEqual(1, m_DataSet.Documents.Count, "Documents.Count");
			Assert.AreEqual(1, m_DataSet.Documents[0].Sessions.Count, "Document.Sessions.Count");
			Assert.AreEqual(1, m_DataSet.Sessions.Count, "Sessions.Count");
			m_DataSet.Clear();
			Assert.AreEqual(0, m_DataSet.Documents.Count, "Documents.Count");
			Assert.AreEqual(0, m_DataSet.Sessions.Count, "Sessions.Count");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void CreateDataNP()
		{
			m_DataSet.Project = null;
			m_DataSet.CreateData("CausesException", "Task", WDEOpenMode.Create);
		}

		[TestMethod]
		public void CreateData()
		{
			m_DataSet.CreateData("User", "Task", WDEOpenMode.Entry, "Location");
			Assert.AreEqual(1, m_DataSet.Sessions.Count, "Count1");
			Assert.AreEqual("User", m_DataSet.Sessions[0].User, "User");
			Assert.AreEqual("Task", m_DataSet.Sessions[0].Task, "Task");
			Assert.AreEqual(WDEOpenMode.Entry, m_DataSet.Sessions[0].Mode, "Mode");
			Assert.AreEqual("Location", m_DataSet.Sessions[0].Location, "Location");
			m_DataSet.Append("Document1");
			Assert.AreEqual(1, m_DataSet.Documents[0].Sessions.Count, "Document.Count");
			Assert.AreEqual("Location", m_DataSet.Documents[0].Sessions[0].Location);

			m_DataSet.CreateData("User2", "Task2", WDEOpenMode.Verify, "Location2");
			Assert.AreEqual(1, m_DataSet.Sessions.Count, "Count2");
			Assert.AreEqual("User2", m_DataSet.Sessions[0].User, "User2");
			Assert.AreEqual("Task2", m_DataSet.Sessions[0].Task, "Task2");
			Assert.AreEqual(WDEOpenMode.Verify, m_DataSet.Sessions[0].Mode, "Mode2");
			Assert.AreEqual("Location2", m_DataSet.Sessions[0].Location, "Location2");
			Assert.AreEqual(0, m_DataSet.Documents.Count, "Document.Count2");
		}

		[TestMethod]
		public void SaveToStream()
		{
			m_DataSet.CreateData("User", "Task", WDEOpenMode.Entry, "Location");
			m_DataSet.Append("Document1");
			MemoryStream memStream = new MemoryStream();
			m_DataSet.SaveToStream(memStream);
			memStream.Seek(0, SeekOrigin.Begin);
			StreamReader sr = new StreamReader(memStream, true);
			string test = sr.ReadToEnd();
			string expected = m_ResMan.GetString("DataSetSaveToStream");
			expected = Regex.Replace(expected, "StartTime=\"[^\"]+\"", "StartTime=\"" + m_DataSet.Sessions[0].StartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") + "\"");
			expected = Regex.Replace(expected, "EndTime=\"[^\"]+\"", "EndTime=\"" + m_DataSet.Sessions[0].EndTime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") + "\"");
			expected = Regex.Replace(expected, "APIVersion=\"[^\"]+\"", "APIVersion=\"" + VersionInfo.VersionNumber + "\"");
			Console.WriteLine(test);
			Assert.AreEqual(expected, test);

			memStream = new MemoryStream();
			m_DataSet.SaveToStream(memStream);
			Assert.IsTrue(m_DataSet.Sessions[0].StartTime < m_DataSet.Sessions[0].EndTime, "Start and End time are the same. Expected Start < End.");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void SaveToStreamInitial()
		{
			MemoryStream memStream = new MemoryStream();
			m_DataSet.SaveToStream(memStream);
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void SaveToStreamAfterClear()
		{
			m_DataSet.CreateData("User", "Task", WDEOpenMode.Entry, "Location");
			m_DataSet.Append("Document1");
			m_DataSet.Clear();
			MemoryStream memStream = new MemoryStream();
			m_DataSet.SaveToStream(memStream);
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void SaveToStreamNoDocs()
		{
			m_DataSet.CreateData("User", "Task", WDEOpenMode.Entry, "Location");
			MemoryStream memStream = new MemoryStream();
			m_DataSet.SaveToStream(memStream);
		}

		[TestMethod]
		public void LoadFromStream()
		{
			Assert.IsNull(m_DataSet.Current, "Current is not null. Expected null.");
			MemoryStream memStream = new MemoryStream();
			StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
			sw.Write(m_ResMan.GetString("DataSetSaveToStream"));
			sw.Close();
			MemoryStream loadStream = new MemoryStream(memStream.ToArray());

			m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
			Assert.IsNotNull(m_DataSet.Current, "Current is null. Expected not null.");
			Assert.AreEqual(1, m_DataSet.Documents.Count, "Documents.Count");
			Assert.AreEqual(2, m_DataSet.Sessions.Count, "Sessions.Count");
			Assert.AreEqual(2, m_DataSet.Sessions[0].SessionID, "Sessions0SessionID");
			Assert.AreEqual(1, m_DataSet.Sessions[1].SessionID, "Sessions1SessionID");
		}

		[TestMethod]
		public void SessionIDProg()
		{
			m_DataSet.CreateData("User", "Task", WDEOpenMode.Entry, "Location");
			m_DataSet.Append("Document1");
			for(int i = 0; i < 10; i++)
			{
				MemoryStream memStream = new MemoryStream();
				m_DataSet.SaveToStream(memStream);
				memStream.Seek(0, SeekOrigin.Begin);
				m_DataSet.LoadFromStream("User" + i.ToString(), "Task" + i.ToString(), WDEOpenMode.Entry, memStream);
			}
			Assert.AreEqual(11, m_DataSet.Sessions.Count, "Sessions.Count");
			int j = 1;
			for(int i = 10; i > -1; i--)
			{
				Assert.AreEqual(j, m_DataSet.Sessions[i].SessionID, "SessionID" + i.ToString());
				j++;
			}

			m_DataSet.Clear();
			m_DataSet.CreateData("new", "new", WDEOpenMode.Create);
			Assert.AreEqual(1, m_DataSet.Sessions[0].SessionID, "After Clear");
		}

		[TestMethod]
		public void FixSessionIDNoGaps()
		{
			MemoryStream memStream = new MemoryStream();
			StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
			sw.Write(m_ResMan.GetString("DataSetFixSessNoGaps"));
			sw.Flush();
			memStream.Seek(0, SeekOrigin.Begin);

			m_DataSet.LoadFromStream("User", "Task", WDEOpenMode.Edit, memStream);
			Assert.AreEqual(4, m_DataSet.Sessions.Count, "Sessions.Count");
			int j = 1;
			for(int i = 3; i > 0; i--)
			{
				Assert.AreEqual(j, m_DataSet.Sessions[i].SessionID, "Session.SessionID");
				j++;
			}

			for(int k = 0; k < 3; k++)
			{
				j = 1;
				for(int i = 2; i > -1; i--)
				{
					Assert.AreEqual(j, m_DataSet.Documents[0].Records[0].Fields[0].Revisions[i].SessionID, "Rev.SessionID");
					j++;
				}
			}
			sw.Close();
		}

		[TestMethod]
		public void FixSessionIDGaps()
		{
			MemoryStream memStream = new MemoryStream();
			StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
			sw.Write(m_ResMan.GetString("DataSetFixSessGaps"));
			sw.Flush();
			memStream.Seek(0, SeekOrigin.Begin);

			m_DataSet.LoadFromStream("User", "Task", WDEOpenMode.Edit, memStream);
			Assert.AreEqual(4, m_DataSet.Sessions.Count, "Sessions.Count");
			int j = 1;
			for(int i = 3; i > 0; i--)
			{
				Assert.AreEqual(j, m_DataSet.Sessions[i].SessionID, "Session.SessionID");
				j++;
			}

			for(int k = 0; k < 3; k++)
			{
				j = 1;
				for(int i = 2; i > -1; i--)
				{
					Assert.AreEqual(j, m_DataSet.Documents[0].Records[0].Fields[0].Revisions[i].SessionID, "Rev.SessionID");
					j++;
				}
			}
			sw.Close();
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void LoadDocumentEmptyDataSet()
		{
			MemoryStream ms = new MemoryStream();
			m_DataSet.LoadDocumentFromStream(ms);
		}
	}
}
