using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Resources;
using System.Reflection;

using WebDX.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Moles.Framework;
using WebDX.Api.Moles;

namespace WebDX.Api.Tests
{
	/// <summary>
	/// DocSession tests.
	/// </summary>
	[TestClass]
	public class DocSessionObjectTests
	{
		private ResourceManager m_ResMan;

		public DocSessionObjectTests()
		{
		}

		[TestInitialize]
		public void Init()
		{
			m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());
		}

		[TestMethod]
		public void CharCount()
		{
            int sessCall = 0;
            int statCall = 0;
            int countCall = 0;
            List<int> countVal = new List<int>(new int[] { 1,2,4,16,64,128 });

            var sessions = new SIWDEDocSessions();
            var document = new SIWDEDocument();
            var records = new SIWDERecords();
            var emptyRecords = new SIWDERecords();
            emptyRecords.CountGet = () => 0;
            var record = new SIWDERecord();
            record.RecordsGet = () => emptyRecords;
            var field = new SIWDEField_R1();
            var dataset = new SIWDEDataSet_R2();
            dataset.DisplayDeletedRowsGet = () => false;
            dataset.DisplayDeletedRowsGet01 = () => false;
            records.CountGet = () => 3;
            var fields = new SIWDEFields();
            fields.CountGet = () => 2;
            var revisions = new SIWDERevisions();
            revisions.CountGet = () => 2;
            var revision = new SIWDERevision();
            revision.SessionIDGet = () =>
            {
                int retVal = 0;
                if (sessCall == 0)
                    retVal = 55;
                else if (sessCall == 3 || sessCall == 6)
                    retVal = 2;
                else
                    retVal = 1;

                sessCall++;
                return retVal;
            };

            revision.StatusGet = () =>
            {
                WDEFieldStatus retVal = WDEFieldStatus.None;
                List<int> keyed = new List<int>(new int[] {1,2,3,5,7,10,12});

                if (statCall == 0 || statCall == 9)
                    retVal = WDEFieldStatus.Plugged;
                else if (keyed.Contains(statCall))
                    retVal = WDEFieldStatus.Keyed;
                else retVal = WDEFieldStatus.Verified;
			
                statCall++;
                return retVal;
            };

            revision.CharCountGet = () =>
            {
                int retVal = countVal[countCall++];
                return retVal;
            };

            sessions.DocumentGet = () => document;
            document.RecordsGet = () => records;
            document.DataSetGet = () => dataset;
            records.ItemGetInt32 = (int i) => record;
            record.FieldsGet = () => fields;
            record.DocumentGet = () => document;
            fields.ItemGetInt32 = (int i) => field;
            field.RevisionsGet = () => revisions;
            revisions.ItemGetInt32 = (int i) => revision;
            IWDEDocSession session = WDEDocSession.Create(sessions as IWDEDocSessions) as IWDEDocSession;
			
			IWDEDocSessionInternal idoc = session as IWDEDocSessionInternal;
			idoc.SessionID = 1;
			int res = session.CharCount;
			Assert.AreEqual(215, res);
		}

		[TestMethod]
		public void VCECount()
		{
            int sessCall = 0;
            int countCall = 0;
            List<int> countVal = new List<int>(new int[] { 1, 2, 4, 8 });

            var sessions = new SIWDEDocSessions();
            var document = new SIWDEDocument();
            var records = new SIWDERecords();
            var emptyRecords = new SIWDERecords();
            emptyRecords.CountGet = () => 0;
            var record = new SIWDERecord();
            record.RecordsGet = () => emptyRecords;
            var field = new SIWDEField_R1();
            var dataset = new SIWDEDataSet_R2();
            records.CountGet = () => 3;
            var fields = new SIWDEFields();
            fields.CountGet = () => 2;
            var revisions = new SIWDERevisions();
            revisions.CountGet = () => 2;
            var revision = new SIWDERevision();
            revision.SessionIDGet = () =>
            {
                List<int> sess1 = new List<int>(new int[] { 0, 4, 5, 11 });
                int retVal = 0;
                if (sess1.Contains(sessCall))
                    retVal = 1;
                else
                    retVal = 2;

                sessCall++;
                return retVal;
            };
            revision.VCECountGet = () =>
            {
                int retVal = countVal[countCall++];
                return retVal;
            };

            sessions.DocumentGet = () => document;
            document.RecordsGet = () => records;
            document.DataSetGet = () => dataset;
            records.ItemGetInt32 = (int i) => record;
            record.FieldsGet = () => fields;
            record.DocumentGet = () => document;
            fields.ItemGetInt32 = (int i) => field;
            field.RevisionsGet = () => revisions;
            revisions.ItemGetInt32 = (int i) => revision;
            IWDEDocSession session = WDEDocSession.Create(sessions as IWDEDocSessions) as IWDEDocSession;

			IWDEDocSessionInternal idoc = session as IWDEDocSessionInternal;
			idoc.SessionID = 1;
			int res = session.VCECount;
			Assert.AreEqual(15, res);
		}
		
		[TestMethod]
		public void WriteToXmlDefault()
		{
            var sessions = new SIWDEDocSessions();
            var document = new SIWDEDocument();
            var records = new SIWDERecords();
            var dataset = new SIWDEDataSet_R2();
            records.CountGet = () => 0;

            sessions.DocumentGet = () => document;
            document.RecordsGet = () => records;
            document.DataSetGet = () => dataset;

            IWDEDocSession session = WDEDocSession.Create(sessions as IWDEDocSessions) as IWDEDocSession;

			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
            IWDEXmlPersist ipers = session as IWDEXmlPersist;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("DocSessionWriteToXmlDefault"), sw.ToString());
		}

		[TestMethod]
		public void ReadFromXmlDefault()
		{
            var sessions = new SIWDEDocSessions();
            var document = new SIWDEDocument();
            var records = new SIWDERecords();
            var dataset = new SIWDEDataSet_R2();
            records.CountGet = () => 0;

            sessions.DocumentGet = () => document;
            document.RecordsGet = () => records;
            document.DataSetGet = () => dataset;

            IWDEDocSession session = WDEDocSession.Create(sessions as IWDEDocSessions) as IWDEDocSession;

			StringReader sr = new StringReader(m_ResMan.GetString("DocSessionWriteToXmlDefault"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
            IWDEXmlPersist ipers = session as IWDEXmlPersist;
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();

            Assert.AreEqual("", session.Location, "Location");
            Assert.AreEqual(WDEOpenMode.Create, session.Mode, "Mode");
            Assert.AreEqual("", session.RejectCode, "RejectCode");
            Assert.AreEqual("", session.RejectDescription, "Reject Description");
            Assert.AreEqual(0, session.SessionID, "SessionID");
            Assert.AreEqual(WDESessionStatus.None, session.Status, "Status");
            Assert.AreEqual("", session.Task, "Task");
            Assert.AreEqual("", session.User, "User");
		}

		[TestMethod]
		public void WriteToXmlFull()
		{
            var sessions = new SIWDEDocSessions();
            var document = new SIWDEDocument();
            var records = new SIWDERecords();
            var emptyRecords = new SIWDERecords();
            emptyRecords.CountGet = () => 0;
            var record = new SIWDERecord();
            record.RecordsGet = () => emptyRecords;
            var field = new SIWDEField_R1();
            var dataset = new SIWDEDataSet_R2();
            dataset.DisplayDeletedRowsGet = () => false;
            dataset.DisplayDeletedRowsGet01 = () => false;
            records.CountGet = () => 1;
            var fields = new SIWDEFields();
            fields.CountGet = () => 1;
            var revisions = new SIWDERevisions();
            revisions.CountGet = () => 1;
            var revision = new SIWDERevision();
            revision.SessionIDGet = () => 1;
            revision.VCECountGet = () => 1;
            revision.CharCountGet = () => 1;
            revision.StatusGet = () => WDEFieldStatus.Keyed;

            sessions.DocumentGet = () => document;
            document.RecordsGet = () => records;
            document.DataSetGet = () => dataset;
            records.ItemGetInt32 = (int i) => record;
            record.FieldsGet = () => fields;
            record.DocumentGet = () => document;
            fields.ItemGetInt32 = (int i) => field;
            field.RevisionsGet = () => revisions;
            revisions.ItemGetInt32 = (int i) => revision;

            IWDEDocSession session = WDEDocSession.Create(sessions as IWDEDocSessions) as IWDEDocSession;

            IWDEDocSessionInternal idoc = session as IWDEDocSessionInternal;
			idoc.SessionID = 1;
			idoc.Location = "Location";
			idoc.Mode = WDEOpenMode.FocusAudit;
			idoc.RejectCode = "01";
			idoc.RejectDescription = "Reject Description";
			idoc.Status = WDESessionStatus.Completed;
			idoc.Task = "Task";
			idoc.User = "User";
            DateTime testDate = DateTime.Now;
            idoc.StartTime = testDate;
            idoc.EndTime = testDate;
            idoc.Duration = new TimeSpan(1, 1, 1);

            string expected = m_ResMan.GetString("DocSessionWriteToXmlFull");
            expected = Regex.Replace(expected, "StartTime=\"[^\"]+\"", "StartTime=\"" + testDate.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") + "\"");
            expected = Regex.Replace(expected, "EndTime=\"[^\"]+\"", "EndTime=\"" + testDate.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") + "\"");

			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
            IWDEXmlPersist ipers = session as IWDEXmlPersist;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(expected, sw.ToString());
		}

		[TestMethod]
		public void ReadFromXmlFull()
		{
            var sessions = new SIWDEDocSessions();
            var document = new SIWDEDocument();
            var records = new SIWDERecords();
            var dataset = new SIWDEDataSet_R2();
            records.CountGet = () => 0;

            sessions.DocumentGet = () => document;
            document.RecordsGet = () => records;
            document.DataSetGet = () => dataset;

            IWDEDocSession_R3 session = WDEDocSession.Create(sessions as IWDEDocSessions) as IWDEDocSession_R3;

            DateTime testDate = DateTime.Now;
            testDate = new DateTime(testDate.Year, testDate.Month, testDate.Day, testDate.Hour, testDate.Minute, testDate.Second);
            string expected = m_ResMan.GetString("DocSessionWriteToXmlFull");
            expected = Regex.Replace(expected, "StartTime=\"[^\"]+\"", "StartTime=\"" + testDate.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") + "\"");
            expected = Regex.Replace(expected, "EndTime=\"[^\"]+\"", "EndTime=\"" + testDate.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz") + "\"");

			StringReader sr = new StringReader(expected);
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
            IWDEXmlPersist ipers = session as IWDEXmlPersist;
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();

            Assert.AreEqual(1, session.SessionID, "SessionID");
            Assert.AreEqual("Location", session.Location, "Location");
            Assert.AreEqual(WDEOpenMode.FocusAudit, session.Mode, "Mode");
            Assert.AreEqual("01", session.RejectCode, "RejectCode");
            Assert.AreEqual("Reject Description", session.RejectDescription, "RejectDescription");
            Assert.AreEqual(WDESessionStatus.Completed, session.Status, "Status");
            Assert.AreEqual("Task", session.Task, "Task");
            Assert.AreEqual("User", session.User, "User");
            Assert.AreEqual(new TimeSpan(1, 1, 1), session.Duration, "Duration");
            Assert.AreEqual(testDate, ((IWDEDocSessionInternal)session).StartTime, "StartTime");
            Assert.AreEqual(testDate, ((IWDEDocSessionInternal)session).EndTime, "EndTime");
		}
	}
}
