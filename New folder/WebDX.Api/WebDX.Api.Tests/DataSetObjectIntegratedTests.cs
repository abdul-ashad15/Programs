using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using WebDX.Api.Moles;

namespace WebDX.Api.Tests
{
    [TestClass]
    public class DataSetObjectIntegratedTests
    {
        private IWDEProject m_Project;

        //Requirement: The start time should not be set when initializing the DataSet in UserClient mode.
        [TestMethod]
        public void DataSet_UserClient_StartTime()
        {
            IWDEDataSet_R2 dataSet = (IWDEDataSet_R2)WDEDataSet.Create(true);
            var project = m_Project;
            dataSet.Project = project;
            dataSet.CreateData("User", "Task", WDEOpenMode.Create);
            Assert.AreEqual(DateTime.MinValue, dataSet.Sessions[0].StartTime);
        }

        //Requirement: The start time should be set when initializing the DataSet in normal mode.
        [TestMethod]
        public void DataSet_StartTime()
        {
            IWDEDataSet_R2 dataSet = (IWDEDataSet_R2)WDEDataSet.Create();
            var project = m_Project;
            dataSet.Project = project;
            dataSet.CreateData("User", "Task", WDEOpenMode.Create);
            Assert.AreNotEqual(DateTime.MinValue, dataSet.Sessions[0].StartTime);
        }

        //Requirement: The end time should be set when saving a new session in Normal mode.
        [TestMethod]
        public void DataSet_EndTime()
        {
            using (MemoryStream mst = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(mst))
                {
                    sw.Write(DataExpectedResults.DataSetSaveToStreamApiVersion2500);
                    sw.Flush();

                    mst.Seek(0, SeekOrigin.Begin);

                    IWDEDataSet_R2 dataSet = (IWDEDataSet_R2)WDEDataSet.Create();
                    dataSet.LoadFromStream("User", "Task", WDEOpenMode.Edit, mst);

                    using(MemoryStream mst2 = new MemoryStream())
                        dataSet.SaveToStream(mst2);

                    Assert.AreNotEqual(DateTime.MinValue, dataSet.Sessions[0].EndTime);
                }
            }
        }

        //Requirement: The end time should not be set when saving a new session in UserClient mode.
        [TestMethod]
        public void DataSet_UserClient_EndTime()
        {
            using (MemoryStream mst = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(mst))
                {
                    sw.Write(DataExpectedResults.DataSetSaveToStreamApiVersion2500);
                    sw.Flush();

                    mst.Seek(0, SeekOrigin.Begin);

                    IWDEDataSet_R2 dataSet = (IWDEDataSet_R2)WDEDataSet.Create(true);
                    dataSet.LoadFromStream("User", "Task", WDEOpenMode.Edit, mst);

                    using (MemoryStream mst2 = new MemoryStream())
                        dataSet.SaveToStream(mst2);

                    Assert.AreEqual(DateTime.MinValue, dataSet.Sessions[0].EndTime);
                }
            }
        }

        //Requirement: The duration should not be set when saving a session in UserClient mode.
        [TestMethod]
        public void DataSet_UserClient_Duration()
        {
            using (MemoryStream mst = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(mst))
                {
                    sw.Write(DataExpectedResults.DataSetSaveToStreamApiVersion2500);
                    sw.Flush();

                    mst.Seek(0, SeekOrigin.Begin);

                    IWDEDataSet_R2 dataSet = (IWDEDataSet_R2)WDEDataSet.Create(true);
                    dataSet.LoadFromStream("User", "Task", WDEOpenMode.Edit, mst);

                    using (MemoryStream mst2 = new MemoryStream())
                        dataSet.SaveToStream(mst2);

                    Assert.AreEqual(TimeSpan.Zero, ((IWDEDocSession_R3)dataSet.Documents[0].Sessions[0]).Duration);
                }
            }
        }

        //Requirement: Duration should be set when saving a session in normal mode.
        [TestMethod]
        public void DataSet_Duration()
        {
            using (MemoryStream mst = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(mst))
                {
                    sw.Write(DataExpectedResults.DataSetSaveToStreamApiVersion2500);
                    sw.Flush();

                    mst.Seek(0, SeekOrigin.Begin);

                    IWDEDataSet_R2 dataSet = (IWDEDataSet_R2)WDEDataSet.Create();
                    dataSet.LoadFromStream("User", "Task", WDEOpenMode.Edit, mst);
                    System.Threading.Thread.Sleep(10);

                    using (MemoryStream mst2 = new MemoryStream())
                        dataSet.SaveToStream(mst2);

                    Assert.AreNotEqual(TimeSpan.Zero, ((IWDEDocSession_R3)dataSet.Documents[0].Sessions[0]).Duration);
                }
            }
        }

        //Requirement: Start time should not be set when loading in UserClient mode.
        [TestMethod]
        public void DataSet_UserClient_LoadStartTime()
        {
            using (MemoryStream mst = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(mst))
                {
                    sw.Write(DataExpectedResults.DataSetSaveToStreamApiVersion2500);
                    sw.Flush();

                    mst.Seek(0, SeekOrigin.Begin);

                    IWDEDataSet_R2 dataSet = (IWDEDataSet_R2)WDEDataSet.Create(true);
                    dataSet.LoadFromStream("User", "Task", WDEOpenMode.Edit, mst);
                    Assert.AreEqual(DateTime.MinValue, dataSet.Sessions[0].StartTime);
                }
            }
        }

        //Requirement: Start time should be set when loading in normal mode.
        [TestMethod]
        public void DataSet_LoadStartTime()
        {
            using (MemoryStream mst = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(mst))
                {
                    sw.Write(DataExpectedResults.DataSetSaveToStreamApiVersion2500);
                    sw.Flush();

                    mst.Seek(0, SeekOrigin.Begin);

                    IWDEDataSet_R2 dataSet = (IWDEDataSet_R2)WDEDataSet.Create();
                    dataSet.LoadFromStream("User", "Task", WDEOpenMode.Edit, mst);
                    Assert.AreNotEqual(DateTime.MinValue, dataSet.Sessions[0].StartTime);
                }
            }
        }
    }
}
