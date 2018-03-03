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
	/// Session Read and Write Tests
	/// </summary>
	[TestClass]
	public class SessionReadWrite
	{
		ResourceManager m_ResMan;

		MockObject m_Sessions;
		MockObject m_DataSet;
		MockObject m_Documents;
		MockObject m_Document;
		MockObject m_DocSessions;
		MockObject m_DocSession;

		public SessionReadWrite()
		{
		}

		[TestInitialize]
		public void Init()
		{
			MockManager.Init();
			m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());

			m_Sessions = MockManager.MockObject(typeof(IWDESessions));
			m_DataSet = MockManager.MockObject(typeof(IWDEDataSet));
			m_Documents = MockManager.MockObject(typeof(IWDEDocuments));
			m_Document = MockManager.MockObject(typeof(IWDEDocument));
			m_DocSessions = MockManager.MockObject(typeof(IWDEDocSessions));
			m_DocSession = MockManager.MockObject(typeof(IWDEDocSession_R1));

			m_Sessions.ExpectGetAlways("DataSet", m_DataSet.Object);
			m_DataSet.ExpectGetAlways("Documents", m_Documents.Object);
			m_Documents.ExpectGetAlways("Count", 1);
			m_Documents.ExpectGetAlways("Item", m_Document.Object);

			m_Document.ExpectGetAlways("Sessions", m_DocSessions.Object);
			m_DocSessions.AlwaysReturn("FindByID", 0);
			m_DocSessions.ExpectGetAlways("Item", m_DocSession.Object);

			m_DocSession.ExpectGetAlways("CharCount", 0);
			m_DocSession.ExpectGetAlways("VCECount", 0);
		}

		[TestCleanup]
		public void Cleanup()
		{
			m_Sessions = null;
			m_DataSet = null;
			m_Documents = null;
			m_Document = null;
			m_DocSessions = null;
			m_DocSession = null;

			MockManager.Verify();
			GC.Collect();
		}

		[TestMethod]
		public void WriteToXmlDefault()
		{
			IWDESession sess = WDESession.Create((IWDESessions) m_Sessions.Object);
			IWDEXmlPersist ipers = sess as IWDEXmlPersist;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual("", sw.ToString());
		}

		[TestMethod]
		public void WriteToXmlFull()
		{
            m_Sessions.ExpectGetAlways("DataSet", null);
			IWDESession sess = WDESession.Create((IWDESessions) m_Sessions.Object);
			IWDESessionInternal isess = sess as IWDESessionInternal;
			isess.EndTime = new DateTime(2005, 10, 25, 15, 31, 35, 443);
			isess.StartTime = new DateTime(2005, 10, 25, 15, 29, 25, 564);
			isess.Location = "Location";
			isess.Mode = WDEOpenMode.CharRepair;
			isess.SessionID = 3;
			isess.Task = "Task";
			isess.User = "User";
			m_DocSession.ExpectGetAlways("CharCount", 100);
			m_DocSession.ExpectGetAlways("VCECount", 200);

			IWDEXmlPersist ipers = sess as IWDEXmlPersist;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual("", sw.ToString());
		}

		[TestMethod]
		public void ReadFromXmlDefault()
		{
            m_Sessions.ExpectGetAlways("DataSet", null);
			IWDESession sess = WDESession.Create((IWDESessions) m_Sessions.Object);
			IWDEXmlPersist ipers = sess as IWDEXmlPersist;
			StringReader sr = new StringReader(m_ResMan.GetString("SessionWriteToXmlDefault"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual(new DateTime(0), sess.EndTime, "EndTime");
			Assert.AreEqual(new DateTime(0), sess.StartTime, "StartTime");
			Assert.AreEqual("", sess.Location, "Location");
			Assert.AreEqual(WDEOpenMode.Create, sess.Mode, "Mode");
			Assert.AreEqual(0, sess.SessionID, "SessionID");
			Assert.AreEqual("", sess.Task, "Task");
			Assert.AreEqual("", sess.User, "User");
		}

		[TestMethod]
		public void ReadFromXmlFull()
		{
            m_Sessions.ExpectGetAlways("DataSet", null);
			IWDESession sess = WDESession.Create((IWDESessions) m_Sessions.Object);
			IWDEXmlPersist ipers = sess as IWDEXmlPersist;
			StringReader sr = new StringReader(m_ResMan.GetString("SessionWriteToXmlFull"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual(new DateTime(2005, 10, 25, 15, 31, 35, 443), sess.EndTime, "EndTime");
			Assert.AreEqual(new DateTime(2005, 10, 25, 15, 29, 25, 564), sess.StartTime, "StartTime");
			Assert.AreEqual("Location", sess.Location, "Location");
			Assert.AreEqual(WDEOpenMode.CharRepair, sess.Mode, "Mode");
			Assert.AreEqual(3, sess.SessionID, "SessionID");
			Assert.AreEqual("Task", sess.Task, "Task");
			Assert.AreEqual("User", sess.User, "User");
		}
	}
}
