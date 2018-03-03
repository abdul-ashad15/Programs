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
	/// Tests for Session object.
	/// </summary>
	[TestClass]
	public class SessionObjectTests
	{
		MockObject m_Sessions;
		MockObject m_DataSet;
		MockObject m_Documents;
		MockObject m_Document;
		MockObject m_DocSessions;
		MockObject m_DocSession;

		public SessionObjectTests()
		{
		}

		[TestInitialize]
		public void Init()
		{
			MockManager.Init();

			m_Sessions = MockManager.MockObject(typeof(IWDESessions));
			m_DataSet = MockManager.MockObject(typeof(IWDEDataSet));
			m_Documents = MockManager.MockObject(typeof(IWDEDocuments));
			m_Document = MockManager.MockObject(typeof(IWDEDocument));
			m_DocSessions = MockManager.MockObject(typeof(IWDEDocSessions));
			m_DocSession = MockManager.MockObject(typeof(IWDEDocSession_R1));

			m_Sessions.ExpectGetAlways("DataSet", m_DataSet.Object);
			m_DataSet.ExpectGetAlways("Documents", m_Documents.Object);
			m_Documents.ExpectGetAlways("Count", 3);
			m_Documents.ExpectGetAlways("Item", m_Document.Object);

			m_Document.ExpectGetAlways("Sessions", m_DocSessions.Object);
			m_DocSessions.AlwaysReturn("FindByID", 0);
			m_DocSessions.ExpectGetAlways("Item", m_DocSession.Object);
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
		[ExpectedException(typeof(WDEException))]
		public void CharCount()
		{
			IWDESession sess = WDESession.Create((IWDESessions) m_Sessions.Object);
			IWDESessionInternal isess = sess as IWDESessionInternal;
			isess.SessionID = 1;
			m_DocSession.ExpectGet("CharCount", 100);
			m_DocSession.ExpectGet("CharCount", 400);
			m_DocSession.ExpectGet("CharCount", 700);
			Assert.AreEqual(1200, sess.CharCount, "CharCount1");
			isess.SessionID = 2;
			m_DocSessions.AlwaysReturn("FindByID", 1);
			m_DocSession.ExpectGet("CharCount", 200);
			m_DocSession.ExpectGet("CharCount", 500);
			m_DocSession.ExpectGet("CharCount", 800);
			Assert.AreEqual(1500, sess.CharCount, "CharCount2");
			isess.SessionID = 3;
			m_DocSessions.AlwaysReturn("FindByID", 2);
			m_DocSession.ExpectGet("CharCount", 300);
			m_DocSession.ExpectGet("CharCount", 600);
			m_DocSession.ExpectGet("CharCount", 900);
			Assert.AreEqual(1800, sess.CharCount, "CharCount3");

			isess.SessionID = 4;
			m_DocSessions.AlwaysReturn("FindByID", -1);
			int except = sess.CharCount;
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void VCECount()
		{
			IWDESession sess = WDESession.Create((IWDESessions) m_Sessions.Object);
			IWDESessionInternal isess = sess as IWDESessionInternal;
			isess.SessionID = 1;
			m_DocSessions.AlwaysReturn("FindByID", 0);
			m_DocSession.ExpectGet("VCECount", 150);
			m_DocSession.ExpectGet("VCECount", 450);
			m_DocSession.ExpectGet("VCECount", 750);
			Assert.AreEqual(1350, sess.VCECount, "VCECount1");
			isess.SessionID = 2;
			m_DocSessions.AlwaysReturn("FindByID", 1);
			m_DocSession.ExpectGet("VCECount", 250);
			m_DocSession.ExpectGet("VCECount", 550);
			m_DocSession.ExpectGet("VCECount", 850);
			Assert.AreEqual(1650, sess.VCECount, "VCECount2");
			isess.SessionID = 3;
			m_DocSessions.AlwaysReturn("FindByID", 2);
			m_DocSession.ExpectGet("VCECount", 350);
			m_DocSession.ExpectGet("VCECount", 650);
			m_DocSession.ExpectGet("VCECount", 950);
			Assert.AreEqual(1950, sess.VCECount, "VCECount3");

			isess.SessionID = 4;
			m_DocSessions.AlwaysReturn("FindByID", -1);
			int except = sess.VCECount;
		}
	}
}
