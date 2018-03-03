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
using System.Collections.Generic;
using WebDX.Api.Fakes;
using Microsoft.QualityTools.Testing.Fakes;

namespace WebDX.Api.Tests
{
    /// <summary>
    /// Tests for the API Data Interfaces
    /// </summary>
    [TestClass]
    public class DataObjectsTests
    {
        ResourceManager m_ResMan;

        public DataObjectsTests()
        {
            m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());
        }
        private bool m_Active;
        private string m_FileName;
        private IWDEProject m_Project;
        private IWDESessions m_Sessions;
        private bool m_PersistFlags;
        private IWDEDocuments m_Documents;
        private string m_ProjectFile;
        private bool m_DisplayDeletedRows;
        private bool m_UserClient;
        private IWDEDataSet_R2 _iWdeDataset;
       

        [TestInitialize]
        public void Initialize()
        {        

            _iWdeDataset = new StubIWDEDataSet_R2()
            {
                SaveToFileString = (a) =>
                  {
                      if (a == null)
                          throw new ArgumentNullException();
                  },
                SaveToStreamStream = (a) =>
                {
                    if (a == null)
                        throw new ArgumentNullException();
                },                
                SaveDocumentToFileString = (a) =>
                {
                    if (a == null)
                        throw new ArgumentNullException();

                },
                SaveToStreamIStreamString = (a, b) =>
                {

                },
            };
            //using ShimsContext.Create()
            //{
            //}
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveToFile_PassFileisEmpty_ExceptedArgumentNullException()
        {
            _iWdeDataset.SaveToFile(null);
        }

        [TestMethod]
        public void SaveToFile_PassFileisValid_ExceptedFileSavedSuccessFully()
        {
            //Act
            var fileName = "text.xml";

            //Arrange
            _iWdeDataset.SaveToFile(fileName);

            //Asset

        }

        [TestMethod]
        public void SaveDocumentToFile__PassFileisValid_ExceptedFileSavedSuccessFully()
        {
            //Act
            var fileName = "text.xml";

            //Arrange
            _iWdeDataset.SaveDocumentToFile(fileName);

            //Asset
        }

        [TestCleanup]
        public void TestCleanup()
        {
            GC.Collect();
        }

        [TestMethod]
        public void CharRepairWriteXml()
        {
            IWDECharRepair rep = WDECharRepair.Create(null);
            rep.CharPos = 1;
            rep.Confidence = 2;
            rep.OCRRect = new Rectangle(3, 4, 5, 6);
            rep.Value = 'C';
            IWDEXmlPersist ipers = rep as IWDEXmlPersist;
            StringWriter sw = new StringWriter();
            XmlTextWriter XmlWriter = new XmlTextWriter(sw);
            ipers.WriteToXml(XmlWriter);
            XmlWriter.Close();
            Assert.AreEqual(m_ResMan.GetString("CharRepairWriteXml"), sw.ToString());
        }

        [TestMethod]
        public void CharRepairWriteXmlDefaults()
        {
            IWDECharRepair rep = WDECharRepair.Create(null);
            rep.CharPos = 0;
            rep.Confidence = 0;
            rep.OCRRect = Rectangle.Empty;
            rep.Value = '\0';
            IWDEXmlPersist ipers = rep as IWDEXmlPersist;
            StringWriter sw = new StringWriter();
            XmlTextWriter XmlWriter = new XmlTextWriter(sw);
            ipers.WriteToXml(XmlWriter);
            XmlWriter.Close();
            Assert.AreEqual(m_ResMan.GetString("CharRepairWriteXmlDefaults"), sw.ToString());
        }

        [TestMethod]
        public void CharRepairReadXml()
        {
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Dictionary<string, WebDXApiVersion> dXmlChangesVersionOnPropiertiesAndCollections = new Dictionary<string, WebDXApiVersion>();
            dXmlChangesVersionOnPropiertiesAndCollections.Add("IWDECharRepair.Confidence", new WebDXApiVersion("1.0.0.0", null));

            IWDECharRepair rep = WDECharRepair.Create(null);
            IWDEXmlPersist ipers = rep as IWDEXmlPersist;
            StringReader sr = new StringReader(m_ResMan.GetString("CharRepairWriteXml"));
            XmlTextReader XmlReader = new XmlTextReader(sr);
            XmlReader.Read();
            ipers.ReadFromXml(XmlReader);
            XmlReader.Close();
            Assert.AreEqual(1, rep.CharPos);
            Assert.AreEqual(2, rep.Confidence);
            Assert.AreEqual(new Rectangle(3, 4, 5, 6), rep.OCRRect);
            Assert.AreEqual('C', rep.Value);
        }

        [TestMethod]
        public void CharRepairReadXmlDefaults()
        {
            IWDECharRepair rep = WDECharRepair.Create(null);
            IWDEXmlPersist ipers = rep as IWDEXmlPersist;
            StringReader sr = new StringReader(m_ResMan.GetString("CharRepairWriteXmlDefaults"));
            XmlTextReader XmlReader = new XmlTextReader(sr);
            XmlReader.Read();
            ipers.ReadFromXml(XmlReader);
            XmlReader.Close();
            Assert.AreEqual(0, rep.CharPos);
            Assert.AreEqual(0, rep.Confidence);
            Assert.AreEqual(Rectangle.Empty, rep.OCRRect);
            Assert.AreEqual('\0', rep.Value);
        }

        [TestMethod]
        public void CharRepairsTests()
        {
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            IWDECharRepairs reps = WDECharRepairs.Create(null);
            IWDECharRepair res = reps.Add('A', 90, 4, new Rectangle(10, 20, 30, 40));
            Assert.AreEqual(1, reps.Count);
            IWDECharRepair rep = reps[0];
            Assert.AreSame(rep, res);
            Assert.AreEqual('A', rep.Value);
            Assert.AreEqual(90, rep.Confidence);
            Assert.AreEqual(4, rep.CharPos);
            Assert.AreEqual(new Rectangle(10, 20, 30, 40), rep.OCRRect);
            reps.Clear();
            Assert.AreEqual(0, reps.Count);
        }

        [TestMethod]
        public void CharRepairsWriteXml()
        {
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            IWDECharRepairs reps = WDECharRepairs.Create(null);
            reps.Add('A', 100, 4, new Rectangle(10, 20, 30, 40));
            reps.Add('B', 20, 30, new Rectangle(20, 30, 40, 50));
            reps.Add('C', 15, 40, new Rectangle(1, 2, 3, 4));
            IWDEXmlPersist ipers = reps as IWDEXmlPersist;
            StringWriter sw = new StringWriter();
            XmlTextWriter XmlWriter = new XmlTextWriter(sw);
            XmlWriter.Formatting = Formatting.Indented;
            ipers.WriteToXml(XmlWriter);
            XmlWriter.Close();
            Assert.AreEqual(m_ResMan.GetString("CharRepairsWriteXml"), sw.ToString());
        }

        [TestMethod]
        public void CharRepairsReadXml()
        {
            IWDECharRepairs reps = WDECharRepairs.Create(null);
            IWDEXmlPersist ipers = reps as IWDEXmlPersist;
            string test = "<DataSet>" + m_ResMan.GetString("CharRepairsWriteXml") + "</DataSet>";
            StringReader sw = new StringReader(test);
            XmlTextReader XmlReader = new XmlTextReader(sw);
            XmlReader.MoveToContent();
            XmlReader.Read();
            XmlReader.MoveToContent();
            ipers.ReadFromXml(XmlReader);
            XmlReader.Close();
            Assert.AreEqual(3, reps.Count);
        }

        [TestMethod]
        public void RevisionWriteXml()
        {
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            IWDERevision rev = WDERevision.Create(null);
            IWDERevisionInternal irev = rev as IWDERevisionInternal;
            irev.SessionID = 1;
            irev.Value = "NotNull";
            irev.Status = WDEFieldStatus.Keyed;
            irev.CharCount = 10;
            irev.VCECount = 20;
            irev.FlagDescription = "Flag Description";
            StringWriter sw = new StringWriter();
            XmlTextWriter XmlWriter = new XmlTextWriter(sw);
            IWDEXmlPersist ipers = rev as IWDEXmlPersist;
            ipers.WriteToXml(XmlWriter);
            XmlWriter.Close();
            Assert.AreEqual(m_ResMan.GetString("RevisionWriteXml"), sw.ToString());
        }

        [TestMethod]
        public void RevisionWriteXmlDefaults()
        {
            IWDERevision rev = WDERevision.Create(null);
            StringWriter sw = new StringWriter();
            XmlTextWriter XmlWriter = new XmlTextWriter(sw);
            IWDEXmlPersist ipers = rev as IWDEXmlPersist;
            ipers.WriteToXml(XmlWriter);
            XmlWriter.Close();
            Assert.AreEqual(m_ResMan.GetString("RevisionWriteXmlDefaults"), sw.ToString());
        }

        [TestMethod]
        public void RevisionReadXmlDefaults()
        {
            IWDERevision rev = WDERevision.Create(null);
            string test = m_ResMan.GetString("RevisionWriteXmlDefaults");
            StringReader sr = new StringReader(test);
            XmlTextReader XmlReader = new XmlTextReader(sr);
            XmlReader.MoveToContent();
            IWDEXmlPersist ipers = rev as IWDEXmlPersist;
            ipers.ReadFromXml(XmlReader);
            XmlReader.Close();

            Assert.IsTrue(rev.IsNull, "IsNull is false, expected true");
            Assert.AreEqual(0, rev.CharCount, "CharCount = {0}, expected 0", new object[] { rev.CharCount });
            Assert.AreEqual("", rev.FlagDescription, "FlagDescription = {0}, expected \"\"", new object[] { rev.FlagDescription });
            Assert.AreEqual(0, rev.SessionID, "SessionID = {0}, expected 0", new object[] { rev.SessionID });
            Assert.IsNull(rev.Value, "Value is not null, expected null");
            Assert.AreEqual(0, rev.VCECount, "VCECount = {0}, expected 0", new object[] { rev.VCECount });
        }

        [TestMethod]
        public void RevisionReadXml()
        {
            IWDERevision rev = WDERevision.Create(null);
            string test = m_ResMan.GetString("RevisionWriteXml");
            StringReader sr = new StringReader(test);
            XmlTextReader XmlReader = new XmlTextReader(sr);
            XmlReader.MoveToContent();
            IWDEXmlPersist ipers = rev as IWDEXmlPersist;
            ipers.ReadFromXml(XmlReader);
            XmlReader.Close();

            Assert.IsFalse(rev.IsNull, "IsNull is true, expected false");
            Assert.AreEqual(10, rev.CharCount, "CharCount = {0}, expected 10", new object[] { rev.CharCount });
            Assert.AreEqual("Flag Description", rev.FlagDescription, "FlagDescription = {0}, expected \"Flag Description\"", new object[] { rev.FlagDescription });
            Assert.AreEqual(1, rev.SessionID, "SessionID = {0}, expected 1", new object[] { rev.SessionID });
            Assert.AreEqual("NotNull", rev.Value, "Value = {0}, expected \"NotNull\"", new object[] { rev.Value });
            Assert.AreEqual(20, rev.VCECount, "VCECount = {0}, expected 0", new object[] { rev.VCECount });
        }

        [TestMethod]
        public void RevisionAsBoolean()
        {
            IWDERevision rev = WDERevision.Create(null);
            Assert.IsFalse(rev.AsBoolean);

            IWDERevisionInternal irev = rev as IWDERevisionInternal;
            irev.Value = "TRUE";
            Assert.IsTrue(rev.AsBoolean);
            irev.Value = "FALSE";
            Assert.IsFalse(rev.AsBoolean);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void RevisionAsBoolBad()
        {
            IWDERevision rev = WDERevision.Create(null);
            IWDERevisionInternal irev = rev as IWDERevisionInternal;

            irev.Value = "NOTBOOL";
            Assert.IsTrue(rev.AsBoolean);
        }

        [TestMethod]
        public void RevisionAsCurrency()
        {
            IWDERevision rev = WDERevision.Create(null);
            Assert.AreEqual(0, rev.AsCurrency);

            IWDERevisionInternal irev = rev as IWDERevisionInternal;
            irev.Value = "24.4";
            Assert.AreEqual((decimal)24.4, rev.AsCurrency);

            irev.Value = Decimal.MaxValue.ToString();
            Assert.AreEqual(Decimal.MaxValue, rev.AsCurrency);
            irev.Value = Decimal.MinValue.ToString();
            Assert.AreEqual(Decimal.MinValue, rev.AsCurrency);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void RevisionAsCurrBad()
        {
            IWDERevision rev = WDERevision.Create(null);
            Assert.AreEqual(0, rev.AsCurrency);

            IWDERevisionInternal irev = rev as IWDERevisionInternal;
            irev.Value = "NOTCURRENCY";
            Assert.AreEqual(0, rev.AsCurrency);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void RevisionAsDateTime()
        {
            IWDERevision rev = WDERevision.Create(null);
            Assert.AreEqual(DateTime.MinValue, rev.AsDateTime);

            IWDERevisionInternal irev = rev as IWDERevisionInternal;
            irev.Value = "10/26/2003";
            Assert.AreEqual(new DateTime(2003, 10, 26, 0, 0, 0, 0), rev.AsDateTime);

            irev.Value = "NOTDATETIME";
            Assert.AreEqual(DateTime.MinValue, rev.AsDateTime);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void RevisionAsFloat()
        {
            IWDERevision rev = WDERevision.Create(null);
            Assert.AreEqual(0, rev.AsFloat);

            IWDERevisionInternal irev = rev as IWDERevisionInternal;
            irev.Value = "24.4";
            Assert.AreEqual(24.4, rev.AsFloat);

            irev.Value = "NOTFLOAT";
            Assert.AreEqual(0, rev.AsFloat);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void RevisionAsInteger()
        {
            IWDERevision rev = WDERevision.Create(null);
            Assert.AreEqual(0, rev.AsInteger);

            IWDERevisionInternal irev = rev as IWDERevisionInternal;
            irev.Value = "24";
            Assert.AreEqual(24, rev.AsInteger);

            irev.Value = "NOTINT";
            Assert.AreEqual(0, rev.AsInteger);
        }

        [TestMethod]
        public void RevisionAsString()
        {
            IWDERevision rev = WDERevision.Create(null);
            Assert.AreEqual("", rev.AsString);

            IWDERevisionInternal irev = rev as IWDERevisionInternal;
            irev.Value = "SOMESTRING";
            Assert.AreEqual("SOMESTRING", rev.AsString);
        }

        [TestMethod]
        public void RevisionAsVariant()
        {
            IWDERevision rev = WDERevision.Create(null);
            Assert.IsNull(rev.AsVariant);

            IWDERevisionInternal irev = rev as IWDERevisionInternal;
            irev.Value = "SOMESTRING";
            Assert.AreSame(rev.AsVariant, rev.Value);
        }

        [TestMethod]
        public void RevisionFlagged()
        {
            IWDERevision rev = WDERevision.Create(null);
            Assert.IsFalse(rev.Flagged);

            IWDERevisionInternal irev = rev as IWDERevisionInternal;
            irev.Status = WDEFieldStatus.Flagged;
            Assert.IsTrue(rev.Flagged);
            irev.Status = WDEFieldStatus.Keyed;
            Assert.IsFalse(rev.Flagged);
        }

        [TestMethod]
        public void RevisionsTests()
        {
            IWDERevisions revs = WDERevisions.Create(null);
            IWDERevisionsInternal irevs = revs as IWDERevisionsInternal;
            IWDERevision res = irevs.Add("ONE", WDEFieldStatus.Keyed, 10, 20, 30);
            Assert.AreEqual(1, revs.Count, "Count = {0}, expected 1", new object[] { revs.Count });
            IWDERevision rev = revs[0];
            Assert.AreSame(res, rev, "Res and Rev are not the same");
            Assert.AreEqual("ONE", rev.Value, "Value = {0}, expected \"ONE\"", new object[] { rev.Value });
            Assert.AreEqual(WDEFieldStatus.Keyed, rev.Status, "Status = {0}, expected Keyed", new object[] { rev.Status });
            Assert.AreEqual(10, rev.SessionID, "SessionID = {0}, expected 10", new object[] { rev.SessionID });
            Assert.AreEqual(20, rev.CharCount, "CharCount = {0}, expected 20", new object[] { rev.SessionID });
            Assert.AreEqual(30, rev.VCECount, "VCECount = {0}, expected 30", new object[] { rev.SessionID });
            irevs.Clear();
            Assert.AreEqual(0, revs.Count, "Clear did not work");
        }

        [TestMethod]
        public void RevisionsWriteXml()
        {
            VersionInfo.TargetVersionNumber = "2.5.0.0";
            IWDERevisions revs = WDERevisions.Create(null);
            IWDERevisionsInternal irevs = revs as IWDERevisionsInternal;
            IWDEXmlPersist ipers = revs as IWDEXmlPersist;
            irevs.Add("ONE", WDEFieldStatus.Plugged, 1, 0, 0);
            irevs.Add("TWO", WDEFieldStatus.Plugged, 2, 0, 0);
            irevs.Add("THREE", WDEFieldStatus.Plugged, 3, 0, 0);
            StringWriter sw = new StringWriter();
            XmlTextWriter XmlWriter = new XmlTextWriter(sw);
            XmlWriter.Formatting = Formatting.Indented;
            ipers.WriteToXml(XmlWriter);
            XmlWriter.Close();
            Assert.AreEqual(m_ResMan.GetString("RevisionsWriteXml"), sw.ToString());
        }

        [TestMethod]
        public void RevisionsReadXml()
        {
            IWDERevisions revs = WDERevisions.Create(null);
            IWDEXmlPersist ipers = revs as IWDEXmlPersist;
            string test = "<DataSet>" + m_ResMan.GetString("RevisionsWriteXml") + "</DataSet>";
            StringReader sr = new StringReader(test);
            XmlTextReader XmlReader = new XmlTextReader(sr);
            XmlReader.MoveToContent();
            XmlReader.Read();
            XmlReader.MoveToContent();
            ipers.ReadFromXml(XmlReader);
            XmlReader.Close();
            Assert.AreEqual(3, revs.Count);
            Assert.AreEqual("THREE", revs[0].Value);
            Assert.AreEqual("TWO", revs[1].Value);
            Assert.AreEqual("ONE", revs[2].Value);
        }

        [TestMethod]
        public void SnapshotTest()
        {
            VersionInfo.TargetVersionNumber = "1.3.4.3";
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("SnapshotProject"));
            sw.Flush();
            memStream.Seek(0, SeekOrigin.Begin);

            IWDEProject proj = WDEProject.Create();
            proj.LoadFromStream(memStream);
            sw.Close();

            memStream = new MemoryStream();
            sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("SnapshotData"));
            sw.Flush();
            memStream.Seek(0, SeekOrigin.Begin);

            IWDEDataSet ds = WDEDataSet.Create();
            ds.Project = proj;
            ds.LoadFromStream("User", "Test", WDEOpenMode.Edit, memStream);
            sw.Close();

            memStream = new MemoryStream();
            ds.Sessions[1].SaveSnapshot(memStream);
            memStream.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStream);
            string test = sr.ReadToEnd();
            sr.Close();

            test = Regex.Replace(test, "APIVersion=\"" + "[^\"]*" + "\"", "APIVersion=\"" + VersionInfo.TargetVersionNumber + "\"");
            Assert.AreEqual(Regex.Replace(m_ResMan.GetString("SnapshotThreeTask"), "APIVersion=\"" + "[^\"]*" + "\"", "APIVersion=\"" + VersionInfo.TargetVersionNumber + "\""), test, "Sessions[1]");

            memStream = new MemoryStream();
            ds.Sessions[2].SaveSnapshot(memStream);
            memStream.Seek(0, SeekOrigin.Begin);
            sr = new StreamReader(memStream);
            test = sr.ReadToEnd();
            sr.Close();

            test = Regex.Replace(test, "APIVersion=\"" + "[^\"]*" + "\"", "APIVersion=\"" + VersionInfo.TargetVersionNumber + "\"");
            Assert.AreEqual(Regex.Replace(m_ResMan.GetString("SnapshotTwoTask"), "APIVersion=\"" + "[^\"]*" + "\"", "APIVersion=\"" + VersionInfo.TargetVersionNumber + "\""), test, "Sessions[2]");

            memStream = new MemoryStream();
            ds.Sessions[3].SaveSnapshot(memStream);
            memStream.Seek(0, SeekOrigin.Begin);
            sr = new StreamReader(memStream);
            test = sr.ReadToEnd();
            sr.Close();

            test = Regex.Replace(test, "APIVersion=\"" + "[^\"]*" + "\"", "APIVersion=\"" + VersionInfo.TargetVersionNumber + "\"");
            Assert.AreEqual(Regex.Replace(m_ResMan.GetString("SnapshotSingleTask"), "APIVersion=\"" + "[^\"]*" + "\"", "APIVersion=\"" + VersionInfo.TargetVersionNumber + "\""), test, "Sessions[3]");
        }
    }
}
