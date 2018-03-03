using System;
using System.Collections;
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
	/// <summary>
	/// Tests for the Sessions object
	/// </summary>
	[TestClass]
	public class SessionsObjectTests
	{
		ResourceManager m_ResMan;

        //MockObject m_DataSet;
        //MockObject m_Documents;
        //MockObject m_Document;
        //MockObject m_DocSessions;
        //MockObject m_DocSession;

        IWDEDataSet m_DataSet;
        IWDEDocuments m_Documents;
        IWDEDocument m_Document;
        IWDEDocSessions m_DocSessions;
        IWDEDocSession_R1 m_DocSession;

        public SessionsObjectTests()
		{
		}

		[TestInitialize]
		public void Init()
		{
			//MockManager.Init();
			m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());

			//m_DataSet = MockManager.MockObject(typeof(IWDEDataSet));
			//m_Documents = MockManager.MockObject(typeof(IWDEDocuments));
			//m_Document = MockManager.MockObject(typeof(IWDEDocument));
			//m_DocSessions = MockManager.MockObject(typeof(IWDEDocSessions));
			//m_DocSession = MockManager.MockObject(typeof(IWDEDocSession_R1));

			//m_DataSet.ExpectGetAlways("Documents", m_Documents.Object);
			//m_Documents.ExpectGetAlways("Count", 1);
			//m_Documents.ExpectGetAlways("Item", m_Document.Object);

			//m_Document.ExpectGetAlways("Sessions", m_DocSessions.Object);
			//m_DocSessions.ExpectGetAlways("Count", 0);
			//m_DocSessions.AlwaysReturn("FindByID", -1);
			//m_DocSessions.ExpectGetAlways("Item", m_DocSession.Object);

			//m_DocSession.ExpectGetAlways("CharCount", 0);
			//m_DocSession.ExpectGetAlways("VCECount", 0);
		}

		[TestCleanup]
		public void Cleanup()
		{
			m_DataSet = null;
			m_Documents = null;
			m_Document = null;
			m_DocSessions = null;
			m_DocSession = null;

			//MockManager.Verify();
			GC.Collect();
		}

		[TestMethod]
		public void Add()
		{
			IWDESessions sess = WDESessions.Create((IWDEDataSet) m_DataSet);
			Assert.AreEqual(0, sess.Count, "Default");
			IWDESessionsInternal isess = sess as IWDESessionsInternal;

			isess.Add("User", "Task", WDEOpenMode.DblEntry, "Location");
			Assert.AreEqual(1, sess.Count, "After Insert");
			Assert.AreEqual("User", sess[0].User, "User");
			Assert.AreEqual("Task", sess[0].Task, "Task");
			Assert.AreEqual(WDEOpenMode.DblEntry, sess[0].Mode, "Mode");
			Assert.AreEqual("Location", sess[0].Location, "Location");
			Assert.AreEqual(1, sess[0].SessionID, "SessionID");

			isess.Clear();
			Assert.AreEqual(0, sess.Count, "After Clear");
		}

		[TestMethod]
		public void AddMulti()
		{
			IWDESessions sess = WDESessions.Create((IWDEDataSet) m_DataSet);
			IWDESessionsInternal isess = sess as IWDESessionsInternal;
			isess.Add("User1", "Task1", WDEOpenMode.Create, "Location1");
			isess.Add("User2", "Task2", WDEOpenMode.Edit, "Location2");

			Assert.AreEqual(2, sess.Count, "Count");
			Assert.AreEqual("User2", sess[0].User, "User");
			Assert.AreEqual("User1", sess[1].User, "User1");
			Assert.AreEqual(2, sess[0].SessionID, "SessionID");
			Assert.AreEqual(1, sess[1].SessionID, "SessionID1");
		}

		[TestMethod]
		public void FindByID()
		{
			IWDESessions sess = WDESessions.Create((IWDEDataSet) m_DataSet);
			IWDESessionsInternal isess = sess as IWDESessionsInternal;
			isess.Add("User1", "Task1", WDEOpenMode.Create, "Location1");
			isess.Add("User2", "Task2", WDEOpenMode.Edit, "Location2");
			isess.Add("User3", "Task3", WDEOpenMode.Edit, "Location3");

			int index = sess.FindByID(2);
			Assert.AreEqual(1, index, "Index");
			Assert.AreEqual("User2", sess[index].User, "User");
			Assert.AreEqual(-1, sess.FindByID(45), "NotFound");
		}

		[TestMethod]
		public void FindByUser()
		{
			IWDESessions sess = WDESessions.Create((IWDEDataSet) m_DataSet);
			IWDESessionsInternal isess = sess as IWDESessionsInternal;
			isess.Add("User1", "Task1", WDEOpenMode.Create, "Location1");
			isess.Add("User2", "Task2", WDEOpenMode.Edit, "Location2");
			isess.Add("User3", "Task3", WDEOpenMode.Edit, "Location3");

			int index = sess.FindByUser("User3");
			Assert.AreEqual(0, index, "Index");
			Assert.AreEqual("User3", sess[index].User, "User");
			Assert.AreEqual(-1, sess.FindByUser("NotThere"), "NotFound");
		}

		[TestMethod]
		public void FindByTask()
		{
			IWDESessions sess = WDESessions.Create((IWDEDataSet) m_DataSet);
			IWDESessionsInternal isess = sess as IWDESessionsInternal;
			isess.Add("User1", "Task1", WDEOpenMode.Create, "Location1");
			isess.Add("User2", "Task2", WDEOpenMode.Edit, "Location2");
			isess.Add("User3", "Task3", WDEOpenMode.Edit, "Location3");

			int index = sess.FindByTask("Task1");
			Assert.AreEqual(2, index, "Index");
			Assert.AreEqual("User1", sess[index].User, "User");
			Assert.AreEqual(-1, sess.FindByTask("NotThere"), "NotFound");
		}

		[TestMethod]
		public void WriteToXml()
		{
			IWDESessions sess = WDESessions.Create((IWDEDataSet) m_DataSet);
			IWDESessionsInternal isess = sess as IWDESessionsInternal;
			isess.Add("User1", "Task1", WDEOpenMode.Create, "Location1");
			isess.Add("User2", "Task2", WDEOpenMode.Edit, "Location2");
			isess.Add("User3", "Task3", WDEOpenMode.Edit, "Location3");

			IWDESessionInternal intsess = sess[0] as IWDESessionInternal;
			intsess.StartTime = new DateTime(2005, 10, 25);
			intsess.EndTime = new DateTime(2005, 10, 25);
			intsess = sess[1] as IWDESessionInternal;
			intsess.StartTime = new DateTime(2005, 10, 25);
			intsess.EndTime = new DateTime(2005, 10, 25);
			intsess = sess[2] as IWDESessionInternal;
			intsess.StartTime = new DateTime(2005, 10, 25);
			intsess.EndTime = new DateTime(2005, 10, 25);

			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = sess as IWDEXmlPersist;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual("", sw.ToString());
		}

		[TestMethod]
		public void ReadFromXml()
		{
			IWDESessions sess = WDESessions.Create((IWDEDataSet) m_DataSet);
			IWDEXmlPersist ipers = sess as IWDEXmlPersist;
			string test = "<DataSet>" + m_ResMan.GetString("SessionsWriteToXml") + "</DataSet>";
			StringReader sr = new StringReader(test);
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual(3, sess.Count, "Count");
			Assert.AreEqual("User3", sess[0].User, "User3");
			Assert.AreEqual("User2", sess[1].User, "User2");
			Assert.AreEqual("User1", sess[2].User, "User1");
		}
	}
}
