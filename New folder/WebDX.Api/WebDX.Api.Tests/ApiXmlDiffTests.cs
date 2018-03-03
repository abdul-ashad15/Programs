using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Resources;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace WebDX.Api.Tests
{
    /// <summary>
    /// The ApiXmlDiffTests class compares the writing out process of all the API XML versions 
    /// </summary>
    [TestClass]
    public class ApiXmlDiffTests
    {
        ResourceManager m_ResMan;
        IWDEDataSet m_DataSet;
        XmlDocument docExpected;
        XmlDocument docGenerated;


        public ApiXmlDiffTests()
        {
            //
            // TODO: Add constructor logic here
            //            
        }

        [TestInitialize]
        public void Init()
        {
            m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());
            m_DataSet = WDEDataSet.CreateInstance();            
        }

        [TestCleanup]
        public void Cleanup()
        {
            m_DataSet = null;
            docExpected = null;
            docGenerated = null;
            GC.Collect();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod]
        public void LoadFromStreamApiVersion1000()
        {
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            //sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1000"));
            sw.Write("DataSetSaveToStreamApiVersion1000");
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());

            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            Assert.IsNotNull(m_DataSet.Current, "Current is null. Expected not null.");
            Assert.AreEqual(4, m_DataSet.Documents.Count, "Documents.Count");
            Assert.AreEqual(13, m_DataSet.Sessions.Count, "Sessions.Count");
            Assert.AreEqual(13, m_DataSet.Sessions[0].SessionID, "Sessions0SessionID");
            Assert.AreEqual(12, m_DataSet.Sessions[1].SessionID, "Sessions1SessionID");
            Assert.AreEqual("1.0.4.0", m_DataSet.Documents[0].APIVersion, "Documents0APIVersion");
        }

        [TestMethod]
        public void SaveToStreamApiVersion1000()
        {
                VersionInfo.TargetVersionNumber = "1.0.4.0";
                //Load for XML file contained in the resources.
                MemoryStream memStream = new MemoryStream();
                StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
                sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1000"));
                sw.Close();
                MemoryStream loadStream = new MemoryStream(memStream.ToArray());
                m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
                //Save the XML file in the stream
                MemoryStream memStreamSave = new MemoryStream();
                m_DataSet.SaveToStream(memStreamSave);                
                memStreamSave.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(memStreamSave, true);
                string test = sr.ReadToEnd();
                string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion1000");
                Console.WriteLine(test);
                //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
                docExpected = new XmlDocument();
                docGenerated = new XmlDocument();
                docExpected.LoadXml(expected);
                docGenerated.LoadXml(test);
                Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion1000ReadAgain()
        {
            VersionInfo.TargetVersionNumber = "1.0.4.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1000"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion1000");
            Console.WriteLine(test);
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
            //Load again the XML file from the the test string.
            MemoryStream memStream2 = new MemoryStream();
            StreamWriter sw2 = new StreamWriter(memStream2, Encoding.UTF8);
            sw2.Write(test);
            sw2.Close();
            MemoryStream loadStream2 = new MemoryStream(memStream2.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream2);
            //Save the XML file in the stream
            MemoryStream memStreamSave3 = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave3);
            memStreamSave3.Seek(0, SeekOrigin.Begin);
            StreamReader sr3= new StreamReader(memStreamSave3, true);
            string test3 = sr3.ReadToEnd();
            docGenerated.LoadXml(test3);
            Compare();            
        }

        /// <summary>
        /// This test method waits for a data lost due the IsDeleted attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion1000WithIsDeletedInjectedAtRecord string
        /// resource. As we know the IsDeleted attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1000WithIsDeletedInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "1.0.4.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1000WithIsDeletedInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostIsDeletedInjectedAtRecord"), "It was expected a data lost on the IsDeleted attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the IsDeleted attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion1100WithIsDeletedInjectedAtRecord string
        /// resource. As we know the IsDeleted attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1100WithIsDeletedInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "1.1.1.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1100WithIsDeletedInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostIsDeletedInjectedAtRecord"), "It was expected a data lost on the IsDeleted attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the IsDeleted attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion1200WithIsDeletedInjectedAtRecord string
        /// resource. As we know the IsDeleted attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1200WithIsDeletedInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "1.2.0.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1200WithIsDeletedInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostIsDeletedInjectedAtRecord"), "It was expected a data lost on the IsDeleted attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the IsDeleted attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion1300WithIsDeletedInjectedAtRecord string
        /// resource. As we know the IsDeleted attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1300WithIsDeletedInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "1.3.2.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1300WithIsDeletedInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostIsDeletedInjectedAtRecord"), "It was expected a data lost on the IsDeleted attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the IsDeleted attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion1343WithIsDeletedInjectedAtRecord string
        /// resource. As we know the IsDeleted attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1343WithIsDeletedInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "1.3.4.3";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1343WithIsDeletedInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostIsDeletedInjectedAtRecord"), "It was expected a data lost on the IsDeleted attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the IsDeleted attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion14030WithIsDeletedInjectedAtRecord string
        /// resource. As we know the IsDeleted attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion14030WithIsDeletedInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "1.4.0.30";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion14030WithIsDeletedInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostIsDeletedInjectedAtRecord"), "It was expected a data lost on the IsDeleted attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost exception due the IsDeleted attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion21016WithIsDeletedInjectedAtRecord string
        /// resource. As we know the IsDeleted attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion21016WithIsDeletedInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "2.1.0.16";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion21016WithIsDeletedInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostIsDeletedInjectedAtRecord"), "It was expected a data lost on the IsDeleted attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the IsDeleted attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion22510WithIsDeletedInjectedAtRecord string
        /// resource. As we know the IsDeleted attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion22510WithIsDeletedInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "2.2.5.10";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion22510WithIsDeletedInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostIsDeletedInjectedAtRecord"), "It was expected a data lost on the IsDeleted attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the IsDeleted attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion2300WithIsDeletedInjectedAtRecord string
        /// resource. As we know the IsDeleted attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion2300WithIsDeletedInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "2.3.0.3";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion2300WithIsDeletedInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostIsDeletedInjectedAtRecord"), "It was expected a data lost on the IsDeleted attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the IsDeleted attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion2400WithIsDeletedInjectedAtRecord string
        /// resource. As we know the IsDeleted attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion2400WithIsDeletedInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "2.4.0.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion2400WithIsDeletedInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostIsDeletedInjectedAtRecord"), "It was expected a data lost on the IsDeleted attribute");
            }
        }

        /// <summary>
        /// This test method waits for data lost due the CustomData attribute was injected in the Field
        /// contained DataSetSaveToStreamApiVersion1000WithCustomData string
        /// resource. As we know the CustomData attributes exists in the next range: [1.4.0.30, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1000WithCustomDataInjectedAtField()
        {
            VersionInfo.TargetVersionNumber = "1.0.4.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1000WithCustomDataInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostCustomDataInjectedAtField"), "It was expected a data lost on the CustomData attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the FlagDescription attribute was injected Revision propierty contained in the 
        /// DataSetSaveToStreamApiVersion1000WithFlagDescriptionInjected string
        /// resource. As we know the FlagDescription attribute only exists in the next range: [1.1.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1000WithFlagDescriptionInjected()
        {
            VersionInfo.TargetVersionNumber = "1.0.4.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1000WithFlagDescriptionInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostFlagDescriptionInjectedAtRevision"), "It was expected a data lost on the FlagDescription attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the QIAutoAudit attribute was injected Document propierty contained in the 
        /// DataSetSaveToStreamApiVersion1000WithQIAutoAuditInjected string
        /// resource. As we know the QIAutoAudit attribute only exists in the next range: [1.2.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1000WithQIAutoAuditInjected()
        {
            VersionInfo.TargetVersionNumber = "1.0.4.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1000WithQIAutoAuditInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            
            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostQIAutoAuditInjectedAtDocument"), "It was expected a data lost on the QIAutoAudit attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the QIAutoAudit attribute was injected Document propierty contained in the 
        /// DataSetSaveToStreamApiVersion1100WithQIAutoAuditInjected string
        /// resource. As we know the QIAutoAudit attribute only exists in the next range: [1.2.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1100WithQIAutoAuditInjected()
        {
            VersionInfo.TargetVersionNumber = "1.1.1.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1100WithQIAutoAuditInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostQIAutoAuditInjectedAtDocument"), "It was expected a data lost on the QIAutoAudit attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the QIFocusAudit attribute was injected Document propierty contained in the 
        /// DataSetSaveToStreamApiVersion1000WithQIFocusAuditInjected string
        /// resource. As we know the QIFocusAudit attribute only exists in the next range: [1.2.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1000WithQIFocusAuditInjectedAtDocument()
        {
            VersionInfo.TargetVersionNumber = "1.0.4.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1000WithQIFocusAuditInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostQIFocusAuditInjectedAtDocument"), "It was expected a data lost on the QIFocusAudit attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the QIFocusAudit attribute was injected Document propierty contained in the 
        /// DataSetSaveToStreamApiVersion1100WithQIFocusAuditInjected string
        /// resource. As we know the QIFocusAudit attribute only exists in the next range: [1.2.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1100WithQIFocusAuditInjectedAtDocument()
        {
            VersionInfo.TargetVersionNumber = "1.1.1.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1100WithQIFocusAuditInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();            
            m_DataSet.SaveToStream(memStreamSave);
            
            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostQIFocusAuditInjectedAtDocument"), "It was expected a data lost on the QIFocusAudit attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the OverlayOffset attribute was injected Image propierty contained in the 
        /// DataSetSaveToStreamApiVersion1000WithOverlayOffsetInjected string
        /// resource. As we know the OverlayOffset attribute only exists in the next range: [1.2.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1000WithOverlayOffsetInjected()
        {
            VersionInfo.TargetVersionNumber = "1.0.4.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1000WithOverlayOffsetInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            
            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostOverlayOffsetInjectedAtImage"), "It was expected a data lost on the OverlayOffset attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the OverlayOffset attribute was injected Image propierty contained in the 
        /// DataSetSaveToStreamApiVersion1100WithOverlayOffsetInjected string
        /// resource. As we know the OverlayOffset attribute only exists in the next range: [1.2.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1100WithOverlayOffsetInjected()
        {
            VersionInfo.TargetVersionNumber = "1.1.1.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1100WithOverlayOffsetInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostOverlayOffsetInjectedAtImage"), "It was expected a data lost on the OverlayOffset attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the Exclude attribute was injected Field propierty contained in the 
        /// DataSetSaveToStreamApiVersion1000WithExcludeInjected string
        /// resource. As we know the Exclude attribute only exists in the next range: [1.2.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1000WithExcludeInjectedAtField()
        {
            VersionInfo.TargetVersionNumber = "1.0.4.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1000WithExcludeInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostExcludeInjectedAtField"), "It was expected a data lost on the Exclude attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the Exclude attribute was injected Field propierty contained in the 
        /// DataSetSaveToStreamApiVersion1100WithExcludeInjected string
        /// resource. As we know the Exclude attribute only exists in the next range: [1.2.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1100WithExcludeInjected()
        {
            VersionInfo.TargetVersionNumber = "1.1.1.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1100WithExcludeInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostExcludeInjectedAtField"), "It was expected a data lost on the Exclude attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the OCRAreaRect attribute was injected Field propierty contained in the 
        /// DataSetSaveToStreamApiVersion1000WithOCRAreaRectInjected string
        /// resource. As we know the OCRAreaRect attribute only exists in the next range: [1.2.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1000WithOCRAreaRectInjected()
        {
            VersionInfo.TargetVersionNumber = "1.0.4.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1000WithOCRAreaRectInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostOCRAreaRectInjectedAtField"), "It was expected a data lost on the OCRAreaRect attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the OCRAreaRect attribute was injected Field propierty contained in the 
        /// DataSetSaveToStreamApiVersion1100WithOCRAreaRectInjected string
        /// resource. As we know the OCRAreaRect attribute only exists in the next range: [1.2.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1100WithOCRAreaRectInjected()
        {
            VersionInfo.TargetVersionNumber = "1.1.1.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1100WithOCRAreaRectInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostOCRAreaRectInjectedAtField"), "It was expected a data lost on the OCRAreaRect attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the QIFocusAudit attribute was injected Field propierty contained in the 
        /// DataSetSaveToStreamApiVersion1000WithQIFocusAuditInjectedAtField string
        /// resource. As we know the QIFocusAudit attribute only exists in the next range: [1.2.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1000WithQIFocusAuditInjectedAtField()
        {
            VersionInfo.TargetVersionNumber = "1.0.4.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1000WithQIFocusAuditInjectedAtField"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostQIFocusAuditInjectedAtField"), "It was expected a data lost on the QIFocusAudit attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the QIFocusAudit attribute was injected Field propierty contained in the 
        /// DataSetSaveToStreamApiVersion1100WithQIFocusAuditInjectedAtField string
        /// resource. As we know the QIFocusAudit attribute only exists in the next range: [1.2.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1100WithQIFocusAuditInjectedAtField()
        {
            VersionInfo.TargetVersionNumber = "1.1.1.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1100WithQIFocusAuditInjectedAtField"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostQIFocusAuditInjectedAtField"), "It was expected a data lost on the QIFocusAudit attribute");
            }
        }


        /// <summary>
        /// This test method waits for a data lost due the SessionID attribute was injected Document propierty contained in the 
        /// DataSetSaveToStreamApiVersion21016WithSessionIDInjectedAtRecord string
        /// resource. As we know the SessionID attribute only exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion21016WithSessionIDInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "2.1.0.16";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion21016WithSessionIDInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostSessionIDInjectedAtRecord"), "It was expected a data lost on the SessionID attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the SessionID attribute was injected Document propierty contained in the 
        /// DataSetSaveToStreamApiVersion22510WithSessionIDInjectedAtRecord string
        /// resource. As we know the SessionID attribute only exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion22510WithSessionIDInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "2.2.5.10";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion22510WithSessionIDInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            
            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostSessionIDInjectedAtRecord"), "It was expected a data lost on the SessionID attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the SessionID attribute was injected Document propierty contained in the 
        /// DataSetSaveToStreamApiVersion2300WithSessionIDInjectedAtRecord string
        /// resource. As we know the SessionID attribute only exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion2300WithSessionIDInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "2.3.0.3";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion2300WithSessionIDInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostSessionIDInjectedAtRecord"), "It was expected a data lost on the SessionID attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the SessionID attribute was injected Document propierty contained in the 
        /// DataSetSaveToStreamApiVersion2400WithSessionIDInjectedAtDocument string
        /// resource. As we know the SessionID attribute only exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion2400WithSessionIDInjectedAtDocument()
        {
            VersionInfo.TargetVersionNumber = "2.4.0.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion2400WithSessionIDInjectedAtDocument"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostSessionIDInjectedAtRecord"), "It was expected a data lost on the SessionID attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the Tag attribute was injected Field propierty contained in the 
        /// DataSetSaveToStreamApiVersion1000WithTagInjectedAtField string
        /// resource. As we know the Tag attribute only exists in the next range: [1.4.0.30, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1000WithTagInjectedAtField()
        {
            VersionInfo.TargetVersionNumber = "1.0.4.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1000WithTagInjectedAtField"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostTagInjectedAtField"), "It was expected a data lost on the Tag attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the Tag attribute was injected Field propierty contained in the 
        /// DataSetSaveToStreamApiVersion1100WithTagInjectedAtField string
        /// resource. As we know the Tag attribute only exists in the next range: [1.4.0.30, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1100WithTagInjectedAtField()
        {
            VersionInfo.TargetVersionNumber = "1.1.1.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1100WithTagInjectedAtField"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostTagInjectedAtField"), "It was expected a data lost on the Tag attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the Tag attribute was injected Field propierty contained in the 
        /// DataSetSaveToStreamApiVersion1200WithTagInjectedAtField string
        /// resource. As we know the Tag attribute only exists in the next range: [1.4.0.30, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1200WithTagInjectedAtField()
        {
                VersionInfo.TargetVersionNumber = "1.2.0.0";
                //Load for XML file contained in the resources.
                MemoryStream memStream = new MemoryStream();
                StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
                sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1200WithTagInjectedAtField"));
                sw.Close();
                MemoryStream loadStream = new MemoryStream(memStream.ToArray());
                m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
                //Save the XML file in the stream
                MemoryStream memStreamSave = new MemoryStream();
                m_DataSet.SaveToStream(memStreamSave);

                Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
                if (VersionHelper.IsDataLost)
                {
                    Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostTagInjectedAtField"), "It was expected a data lost on the Tag attribute");
                }
        }

        /// <summary>
        /// This test method waits for data lost due the Tag attribute was injected Field propierty contained in the 
        /// DataSetSaveToStreamApiVersion1300WithTagInjectedAtField string
        /// resource. As we know the Tag attribute only exists in the next range: [1.4.0.30, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1300WithTagInjectedAtField()
        {
            VersionInfo.TargetVersionNumber = "1.3.2.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1300WithTagInjectedAtField"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostTagInjectedAtField"), "It was expected a data lost on the Tag attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the Tag attribute was injected Field propierty contained in the 
        /// DataSetSaveToStreamApiVersion1343WithTagInjectedAtField string
        /// resource. As we know the Tag attribute only exists in the next range: [1.4.0.30, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1343WithTagInjectedAtField()
        {
            VersionInfo.TargetVersionNumber = "1.3.4.3";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1343WithTagInjectedAtField"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostTagInjectedAtField"), "It was expected a data lost on the Tag attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the SessionID attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion1000WithSessionIDInjectedAtRecord string
        /// resource. As we know the SessionID attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1000WithSessionIDInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "1.0.4.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1000WithSessionIDInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostSessionIDInjectedAtRecord"), "It was expected a data lost on the SessionID attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the SessionID attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion1100WithSessionIDInjectedAtRecord string
        /// resource. As we know the SessionID attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1100WithSessionIDInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "1.1.1.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1100WithSessionIDInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostSessionIDInjectedAtRecord"), "It was expected a data lost on the SessionID attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the SessionID attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion1200WithSessionIDInjectedAtRecord string
        /// resource. As we know the SessionID attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1200WithSessionIDInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "1.2.0.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1200WithSessionIDInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostSessionIDInjectedAtRecord"), "It was expected a data lost on the SessionID attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the SessionID attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion1300WithSessionIDInjectedAtRecord string
        /// resource. As we know the SessionID attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1300WithSessionIDInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "1.3.2.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1300WithSessionIDInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostSessionIDInjectedAtRecord"), "It was expected a data lost on the SessionID attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the SessionID attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion1343WithSessionIDInjectedAtRecord string
        /// resource. As we know the SessionID attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1343WithSessionIDInjectedAtRecord()
        {
                VersionInfo.TargetVersionNumber = "1.3.4.3";
                //Load for XML file contained in the resources.
                MemoryStream memStream = new MemoryStream();
                StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
                sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1343WithSessionIDInjectedAtRecord"));
                sw.Close();
                MemoryStream loadStream = new MemoryStream(memStream.ToArray());
                m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
                //Save the XML file in the stream
                MemoryStream memStreamSave = new MemoryStream();
                m_DataSet.SaveToStream(memStreamSave);

                Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
                if (VersionHelper.IsDataLost)
                {
                    Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostSessionIDInjectedAtRecord"), "It was expected a data lost on the SessionID attribute");
                }
        }

        /// <summary>
        /// This test method waits for a data lost due the SessionID attribute was injected in the Record structure
        /// contained in the DataSetSaveToStreamApiVersion14030WithSessionIDInjectedAtRecord string
        /// resource. As we know the SessionID attribute exists in the next range: [2.5.0.0, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion14030WithSessionIDInjectedAtRecord()
        {
            VersionInfo.TargetVersionNumber = "1.4.0.30";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion14030WithSessionIDInjectedAtRecord"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostSessionIDInjectedAtRecord"), "It was expected a data lost on the SessionID attribute");
            }
        }

        [TestMethod]
        public void SaveToStreamApiVersion1100()
        {
            VersionInfo.TargetVersionNumber = "1.1.1.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1100"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion1100");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion1100ReadAgain()
        {
            VersionInfo.TargetVersionNumber = "1.1.1.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1100"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion1100");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
            //Load again the XML file from the the test string.
            MemoryStream memStream2 = new MemoryStream();
            StreamWriter sw2 = new StreamWriter(memStream2, Encoding.UTF8);
            sw2.Write(test);
            sw2.Close();
            MemoryStream loadStream2 = new MemoryStream(memStream2.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream2);
            //Save the XML file in the stream
            MemoryStream memStreamSave3 = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave3);
            memStreamSave3.Seek(0, SeekOrigin.Begin);
            StreamReader sr3 = new StreamReader(memStreamSave3, true);
            string test3 = sr3.ReadToEnd();
            docGenerated.LoadXml(test3);
            Compare();  
        }

        /// <summary>
        /// This test method waits for a data lost due the CustomData attribute was injected in the Field contained in the
        /// DataSetSaveToStreamApiVersion1100WithCustomData string
        /// resource. As we know the CustomData attributes exists in the next range: [1.4.0.30, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1100WithCustomDataInjected()
        {
            VersionInfo.TargetVersionNumber = "1.1.1.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1100WithCustomDataInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostCustomDataInjectedAtField"), "It was expected a data lost on the CustomData attribute");
            }
        }

        [TestMethod]
        public void SaveToStreamApiVersion1200()
        {
            VersionInfo.TargetVersionNumber = "1.2.0.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1200"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion1200");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion1200ReadAgain()
        {
            VersionInfo.TargetVersionNumber = "1.2.0.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1200"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion1200");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
            //Load again the XML file from the the test string.
            MemoryStream memStream2 = new MemoryStream();
            StreamWriter sw2 = new StreamWriter(memStream2, Encoding.UTF8);
            sw2.Write(test);
            sw2.Close();
            MemoryStream loadStream2 = new MemoryStream(memStream2.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream2);
            //Save the XML file in the stream
            MemoryStream memStreamSave3 = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave3);
            memStreamSave3.Seek(0, SeekOrigin.Begin);
            StreamReader sr3 = new StreamReader(memStreamSave3, true);
            string test3 = sr3.ReadToEnd();
            docGenerated.LoadXml(test3);
            Compare();
        }

        /// <summary>
        /// This test method waits for a data lost due the CustomData attribute was injected in the Field contained
        /// DataSetSaveToStreamApiVersion1200WithCustomData string
        /// resource. As we know the CustomData attributes exists in the next range: [1.4.0.30, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1200WithCustomDataInjected()
        {
            VersionInfo.TargetVersionNumber = "1.2.0.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1200WithCustomDataInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostCustomDataInjectedAtField"), "It was expected a data lost on the CustomData attribute");
            }
        }

        [TestMethod]
        public void SaveToStreamApiVersion1300()
        {
            VersionInfo.TargetVersionNumber = "1.3.2.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1300"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion1300");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion1300ReadAgain()
        {
            VersionInfo.TargetVersionNumber = "1.3.2.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1300"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion1300");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
            //Load again the XML file from the the test string.
            MemoryStream memStream2 = new MemoryStream();
            StreamWriter sw2 = new StreamWriter(memStream2, Encoding.UTF8);
            sw2.Write(test);
            sw2.Close();
            MemoryStream loadStream2 = new MemoryStream(memStream2.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream2);
            //Save the XML file in the stream
            MemoryStream memStreamSave3 = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave3);
            memStreamSave3.Seek(0, SeekOrigin.Begin);
            StreamReader sr3 = new StreamReader(memStreamSave3, true);
            string test3 = sr3.ReadToEnd();
            docGenerated.LoadXml(test3);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion1343()
        {
            VersionInfo.TargetVersionNumber = "1.3.4.3";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1343"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion1343");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion1343ReadAgain()
        {
            VersionInfo.TargetVersionNumber = "1.3.4.3";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1343"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion1343");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
            //Load again the XML file from the the test string.
            MemoryStream memStream2 = new MemoryStream();
            StreamWriter sw2 = new StreamWriter(memStream2, Encoding.UTF8);
            sw2.Write(test);
            sw2.Close();
            MemoryStream loadStream2 = new MemoryStream(memStream2.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream2);
            //Save the XML file in the stream
            MemoryStream memStreamSave3 = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave3);
            memStreamSave3.Seek(0, SeekOrigin.Begin);
            StreamReader sr3 = new StreamReader(memStreamSave3, true);
            string test3 = sr3.ReadToEnd();
            docGenerated.LoadXml(test3);
            Compare();
        }


        /// <summary>
        /// This test method waits for a data lost due the CustomData attribute was injected in the Field contained
        /// DataSetSaveToStreamApiVersion1300WithCustomData string
        /// resource. As we know the CustomData attributes exists in the next range: [1.4.0.30, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1300WithCustomDataInjected()
        {
            VersionInfo.TargetVersionNumber = "1.3.2.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1300WithCustomDataInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostCustomDataInjectedAtField"), "It was expected a data lost on the CustomData attribute");
            }
        }

        /// <summary>
        /// This test method waits for a data lost due the CustomData attribute was injected in the Field contained
        /// DataSetSaveToStreamApiVersion1300WithCustomData string
        /// resource. As we know the CustomData attributes exists in the next range: [1.4.0.30, 2.5.0.0]
        /// </summary>
        [TestMethod]
        public void SaveToStreamApiVersion1343WithCustomDataInjected()
        {
            VersionInfo.TargetVersionNumber = "1.3.4.3";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion1343WithCustomDataInjected"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);

            Assert.AreEqual(VersionHelper.IsDataLost, true, "Data lost was expected!");
            if (VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.LostData, m_ResMan.GetString("DataLostCustomDataInjectedAtField"), "It was expected a data lost on the CustomData attribute");
            }
        }


        [TestMethod]
        public void SaveToStreamApiVersion14030()
        {
            VersionInfo.TargetVersionNumber = "1.4.0.30";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion14030"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion14030");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion14030ReadAgain()
        {
            VersionInfo.TargetVersionNumber = "1.4.0.30";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion14030"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion14030");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
            //Load again the XML file from the the test string.
            MemoryStream memStream2 = new MemoryStream();
            StreamWriter sw2 = new StreamWriter(memStream2, Encoding.UTF8);
            sw2.Write(test);
            sw2.Close();
            MemoryStream loadStream2 = new MemoryStream(memStream2.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream2);
            //Save the XML file in the stream
            MemoryStream memStreamSave3 = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave3);
            memStreamSave3.Seek(0, SeekOrigin.Begin);
            StreamReader sr3 = new StreamReader(memStreamSave3, true);
            string test3 = sr3.ReadToEnd();
            docGenerated.LoadXml(test3);
            Compare();
        }


        [TestMethod]
        public void SaveToStreamApiVersion21016()
        {
            VersionInfo.TargetVersionNumber = "2.1.0.16";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion21016"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion21016");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion21016ReadAgain()
        {
            VersionInfo.TargetVersionNumber = "2.1.0.16";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion21016"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion21016");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
            //Load again the XML file from the the test string.
            MemoryStream memStream2 = new MemoryStream();
            StreamWriter sw2 = new StreamWriter(memStream2, Encoding.UTF8);
            sw2.Write(test);
            sw2.Close();
            MemoryStream loadStream2 = new MemoryStream(memStream2.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream2);
            //Save the XML file in the stream
            MemoryStream memStreamSave3 = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave3);
            memStreamSave3.Seek(0, SeekOrigin.Begin);
            StreamReader sr3 = new StreamReader(memStreamSave3, true);
            string test3 = sr3.ReadToEnd();
            docGenerated.LoadXml(test3);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion22510()
        {
            VersionInfo.TargetVersionNumber = "2.2.5.10";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion22510"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion22510");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion22510ReadAgain()
        {
            VersionInfo.TargetVersionNumber = "2.2.5.10";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion22510"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion22510");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
            //Load again the XML file from the the test string.
            MemoryStream memStream2 = new MemoryStream();
            StreamWriter sw2 = new StreamWriter(memStream2, Encoding.UTF8);
            sw2.Write(test);
            sw2.Close();
            MemoryStream loadStream2 = new MemoryStream(memStream2.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream2);
            //Save the XML file in the stream
            MemoryStream memStreamSave3 = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave3);
            memStreamSave3.Seek(0, SeekOrigin.Begin);
            StreamReader sr3 = new StreamReader(memStreamSave3, true);
            string test3 = sr3.ReadToEnd();
            docGenerated.LoadXml(test3);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion2300()
        {
            VersionInfo.TargetVersionNumber = "2.3.0.3";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion2300"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion2300");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion2300ReadAgain()
        {
            VersionInfo.TargetVersionNumber = "2.3.0.3";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion2300"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion2300");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
            //Load again the XML file from the the test string.
            MemoryStream memStream2 = new MemoryStream();
            StreamWriter sw2 = new StreamWriter(memStream2, Encoding.UTF8);
            sw2.Write(test);
            sw2.Close();
            MemoryStream loadStream2 = new MemoryStream(memStream2.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream2);
            //Save the XML file in the stream
            MemoryStream memStreamSave3 = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave3);
            memStreamSave3.Seek(0, SeekOrigin.Begin);
            StreamReader sr3 = new StreamReader(memStreamSave3, true);
            string test3 = sr3.ReadToEnd();
            docGenerated.LoadXml(test3);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion2400()
        {
            VersionInfo.TargetVersionNumber = "2.4.0.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion2400"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion2400");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion2400ReadAgain()
        {
            VersionInfo.TargetVersionNumber = "2.4.0.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion2400"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion2400");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
            //Load again the XML file from the the test string.
            MemoryStream memStream2 = new MemoryStream();
            StreamWriter sw2 = new StreamWriter(memStream2, Encoding.UTF8);
            sw2.Write(test);
            sw2.Close();
            MemoryStream loadStream2 = new MemoryStream(memStream2.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream2);
            //Save the XML file in the stream
            MemoryStream memStreamSave3 = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave3);
            memStreamSave3.Seek(0, SeekOrigin.Begin);
            StreamReader sr3 = new StreamReader(memStreamSave3, true);
            string test3 = sr3.ReadToEnd();
            docGenerated.LoadXml(test3);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion2500()
        {
            VersionInfo.TargetVersionNumber = "2.5.0.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion2500"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion2500");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
        }

        [TestMethod]
        public void SaveToStreamApiVersion2500ReadAgain()
        {
            VersionInfo.TargetVersionNumber = "2.5.0.0";
            //Load for XML file contained in the resources.
            MemoryStream memStream = new MemoryStream();
            StreamWriter sw = new StreamWriter(memStream, Encoding.UTF8);
            sw.Write(m_ResMan.GetString("DataSetSaveToStreamApiVersion2500"));
            sw.Close();
            MemoryStream loadStream = new MemoryStream(memStream.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream);
            //Save the XML file in the stream
            MemoryStream memStreamSave = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave);
            memStreamSave.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(memStreamSave, true);
            string test = sr.ReadToEnd();
            string expected = m_ResMan.GetString("DataSetSaveToStreamApiVersion2500");
            //TODO: Create two XMLDocument, the first one for the XML expected and the other one for the XML writen out and compare them.
            docExpected = new XmlDocument();
            docGenerated = new XmlDocument();
            docExpected.LoadXml(expected);
            docGenerated.LoadXml(test);
            Compare();
            //Load again the XML file from the the test string.
            MemoryStream memStream2 = new MemoryStream();
            StreamWriter sw2 = new StreamWriter(memStream2, Encoding.UTF8);
            sw2.Write(test);
            sw2.Close();
            MemoryStream loadStream2 = new MemoryStream(memStream2.ToArray());
            m_DataSet.LoadFromStream("User2", "Task2", WDEOpenMode.Verify, "Location2", loadStream2);
            //Save the XML file in the stream
            MemoryStream memStreamSave3 = new MemoryStream();
            m_DataSet.SaveToStream(memStreamSave3);
            memStreamSave3.Seek(0, SeekOrigin.Begin);
            StreamReader sr3 = new StreamReader(memStreamSave3, true);
            string test3 = sr3.ReadToEnd();
            docGenerated.LoadXml(test3);
            Compare();
        }

        public void Compare()
        {
            foreach (XmlNode ChNode in docExpected.ChildNodes)
            {
                CompareLower(ChNode);
            }
        }

        public void CompareLower(XmlNode NodeName)
        {

            foreach (XmlNode ChlNode in NodeName.ChildNodes)
            {
                string PathAttr;
                foreach (XmlAttribute attr in ChlNode.Attributes)
                {
                    PathAttr = CreatePathAttribute(ChlNode, attr);

                    if (docGenerated.SelectNodes(PathAttr).Count > 0)
                    {
                        continue;
                    }
                    else
                    {
                        //Checks if the attributes have a default value or an empty string
                        if (attr.Value.Contains("None") || string.IsNullOrEmpty(attr.Value))
                        {
                            continue;
                        }
                        else if (attr.Name.Equals("StartTime"))
                        {
                            if (ChlNode.Name.Equals("Session"))
                            {
                                try
                                {
                                    DateTime dtExpected = Convert.ToDateTime(attr.Value);
                                    XmlNodeList session = docGenerated.SelectNodes("//DataSet/Session[@SessionID=" + ChlNode.Attributes["SessionID"].Value + "]");
                                    if (session.Count > 0)
                                    {
                                        DateTime dtGenerated = Convert.ToDateTime(session.Item(0).Attributes["StartTime"].Value);
                                        Assert.AreEqual(dtExpected, dtGenerated, "StartTime are different at SessionID =" + ChlNode.Attributes["SessionID"].Value);
                                    }
                                }
                                finally
                                {
                                }
                            }
                        }
                        else if (attr.Name.Equals("EndTime"))
                        {
                            if (ChlNode.Name.Equals("Session"))
                            {
                                try
                                {
                                    DateTime dtExpected = Convert.ToDateTime(attr.Value);
                                    XmlNodeList session = docGenerated.SelectNodes("//DataSet/Session[@SessionID=" + ChlNode.Attributes["SessionID"].Value + "]");
                                    if (session.Count > 0)
                                    {
                                        DateTime dtGenerated = Convert.ToDateTime(session.Item(0).Attributes["EndTime"].Value);
                                        Assert.AreEqual(dtExpected, dtGenerated, "EndTime are different at SessionID =" + ChlNode.Attributes["SessionID"].Value);
                                    }
                                }
                                finally
                                {
                                }
                            }
                        }
                        else
                        {
                            Assert.AreEqual(docGenerated.SelectNodes(PathAttr).Count, docExpected.SelectNodes(PathAttr).Count, "This attribute was not found " + PathAttr);
                        }

                    }

                }

                string Path = CreatePath(ChlNode);

                if (docGenerated.SelectNodes(Path).Count == 0)
                {
                    Assert.AreEqual(docGenerated.SelectNodes(Path).Count, docExpected.SelectNodes(Path).Count, "This node was not found " + Path);
                }
                else
                {
                    CompareLower(ChlNode);
                }

            }
        }

        public string CreatePath(XmlNode Node)
        {
            string Path = "/" + Node.Name;

            while (!(Node.ParentNode.Name == "#document"))
            {
                Path = "/" + Node.ParentNode.Name + Path;
                Node = Node.ParentNode;
            }
            Path = "/" + Path;
            return Path;
        }

        public string CreatePathAttribute(XmlNode Node, XmlAttribute attr)
        {
            string Path = "/" + Node.Name;

            while (!(Node.ParentNode.Name == "#document"))
            {
                Path = "/" + Node.ParentNode.Name + Path;
                Node = Node.ParentNode;
            }
            Path = "/" + Path + "[@" + attr.Name + "='" + attr.Value + "']";
            return Path;
        }
    }
}
