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
	/// Tests for DocSessions
	/// </summary>
	[TestClass]
	public class DocSessionsObjectTests
	{
		ResourceManager m_ResMan;
		IWDEDocSessions m_Sessions;
		MockObject m_Document;
		MockObject m_Records;
		MockObject m_Record;
		MockObject m_Fields;
		MockObject m_Field;
		MockObject m_Revisions;
		MockObject m_Revision;
		MockObject m_ChildRecs;

		public DocSessionsObjectTests()
		{
		}

		[TestInitialize]
		public void Init()
		{
			MockManager.Init();

			m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());

			m_Document = MockManager.MockObject(typeof(IWDEDocument));
			m_Records = MockManager.MockObject(typeof(IWDERecords));
			m_Record = MockManager.MockObject(typeof(IWDERecord));
			m_Fields = MockManager.MockObject(typeof(IWDEFields));
			m_Field = MockManager.MockObject(typeof(IWDEField_R1));
			m_Revisions = MockManager.MockObject(typeof(IWDERevisions));
			m_Revision = MockManager.MockObject(typeof(IWDERevision));
			m_ChildRecs = MockManager.MockObject(typeof(IWDERecords));

			m_Document.ExpectGetAlways("Records", m_Records.Object);
            m_Document.ExpectGetAlways("DataSet", null);
			m_Records.ExpectGetAlways("Item", m_Record.Object);
			m_Record.ExpectGetAlways("Fields", m_Fields.Object);
			m_Record.ExpectGetAlways("Records", m_ChildRecs.Object);
			m_Fields.ExpectGetAlways("Item", m_Field.Object);
			m_Field.ExpectGetAlways("Revisions", m_Revisions.Object);
			m_Revisions.ExpectGetAlways("Item", m_Revision.Object);

			m_ChildRecs.ExpectGetAlways("Count", 0);
			m_Records.ExpectGetAlways("Count", 0);
			
			m_Sessions = WDEDocSessions.Create((IWDEDocument) m_Document.Object);
		}

		[TestCleanup]
		public void Cleanup()
		{
			m_Sessions = null;
			m_Document = null;
			m_Records = null;
			m_Record = null;
			m_Fields = null;
			m_Field = null;
			m_Revisions = null;
			m_Revision = null;
			m_ChildRecs = null;

			MockManager.Verify();
		}

		[TestMethod]
		public void Add()
		{
			IWDEDocSessionsInternal isess = m_Sessions as IWDEDocSessionsInternal;
			IWDEDocSession res = isess.Add("User", "Task", WDEOpenMode.DblEntry, 1, "");
			Assert.AreEqual(1, m_Sessions.Count, "Count");
			Assert.AreSame(res, m_Sessions[0], "Res and m_Sessions[0] are not the same");
			Assert.AreEqual("User", m_Sessions[0].User, "User");
			Assert.AreEqual("Task", m_Sessions[0].Task, "Task");
			Assert.AreEqual(WDEOpenMode.DblEntry, m_Sessions[0].Mode, "Mode");
			Assert.AreEqual(1, m_Sessions[0].SessionID, "SessionID");
			isess.Clear();
			Assert.AreEqual(0, m_Sessions.Count, "Clear");
		}

		[TestMethod]
		public void AddMulti()
		{
			IWDEDocSessionsInternal isess = m_Sessions as IWDEDocSessionsInternal;
			isess.Add("User1", "Task1", WDEOpenMode.DblEntry, 1, "");
			isess.Add("User2", "Task2", WDEOpenMode.DblEntry, 2, "");
			isess.Add("User3", "Task3", WDEOpenMode.DblEntry, 3, "");
			Assert.AreEqual("User3", m_Sessions[0].User, "User3");
			Assert.AreEqual("User2", m_Sessions[1].User, "User2");
			Assert.AreEqual("User1", m_Sessions[2].User, "User1");
		}

		[TestMethod]
		public void FindTests()
		{
			IWDEDocSessionsInternal isess = m_Sessions as IWDEDocSessionsInternal;
			isess.Add("User1", "Task1", WDEOpenMode.DblEntry, 1, "");
			isess.Add("User2", "Task2", WDEOpenMode.DblEntry, 2, "");
			isess.Add("User3", "Task3", WDEOpenMode.DblEntry, 3, "");

			int index = m_Sessions.FindByID(3);
			Assert.AreEqual(0, index, "ID 3");
			index = m_Sessions.FindByID(40);
			Assert.AreEqual(-1, index, "ID 40");
			index = m_Sessions.FindByTask("Task2");
			Assert.AreEqual(1, index, "Task2");
			index = m_Sessions.FindByTask("NotThere");
			Assert.AreEqual(-1, index, "NotThere");
			index = m_Sessions.FindByUser("User1");
			Assert.AreEqual(2, index, "User1");
			index = m_Sessions.FindByUser("NoUser");
			Assert.AreEqual(-1, index, "NoUser");
		}

		[TestMethod]
		public void WriteToXml()
		{
			IWDEDocSessionsInternal isess = m_Sessions as IWDEDocSessionsInternal;
			isess.Add("User1", "Task1", WDEOpenMode.DblEntry, 1, "");
			isess.Add("User2", "Task2", WDEOpenMode.DblEntry, 2, "");
			isess.Add("User3", "Task3", WDEOpenMode.DblEntry, 3, "");
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = m_Sessions as IWDEXmlPersist;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("DocSessionsWriteToXml"), sw.ToString());
		}

		[TestMethod]
		public void ReadFromXml()
		{
			IWDEXmlPersist ipers = m_Sessions as IWDEXmlPersist;
			string test = "<DataSet>" + m_ResMan.GetString("DocSessionsWriteToXml") + "</DataSet>";
			StringReader sr = new StringReader(test);
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual(3, m_Sessions.Count, "Count");
			Assert.AreEqual("User3", m_Sessions[0].User, "User3");
			Assert.AreEqual("User2", m_Sessions[1].User, "User2");
			Assert.AreEqual("User1", m_Sessions[2].User, "User1");
		}
	}
}
