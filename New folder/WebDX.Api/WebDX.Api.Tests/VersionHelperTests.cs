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

namespace WebDX.Api.Tests
{
    /// <summary>
    /// Tests for the API Data Interfaces
    /// </summary>
    [TestClass]
    public class VersionHelperTests
    {
        ResourceManager m_ResMan;

        public VersionHelperTests()
        {
            m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());
        }

        [TestInitialize]
        public void Init()
        {
            //Cheking the IsDataLost property will populate the dicctionaries.
            if (!VersionHelper.IsDataLost)
            {
                Assert.AreEqual(VersionHelper.IsDataLost, false);
            }

        }

        [TestCleanup]
        public void TestCleanup()
        {
            GC.Collect();
        }

        /// <summary>
        /// This test validates the FlagsDescription attribute
        /// Initial version: 1.1.0.0
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDERevisionFlagDescription()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.1.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERevision.FlagDescription", VersionInfo.TargetVersionNumber), false,"The FlagDescription does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERevision.FlagDescription", VersionInfo.TargetVersionNumber), false, "The FlagDescription does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERevision.FlagDescription", VersionInfo.TargetVersionNumber), false, "The FlagDescription does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERevision.FlagDescription", VersionInfo.TargetVersionNumber), false, "The FlagDescription does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "1.0.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERevision.FlagDescription", VersionInfo.TargetVersionNumber), true, "The FlagDescription does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the QIAutoAudit attribute
        /// Initial version: 1.2.0.0
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDEDocumentQIAutoAudit()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.2.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.QIAutoAudit", VersionInfo.TargetVersionNumber), false, "The QIAutoAudit does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.QIAutoAudit", VersionInfo.TargetVersionNumber), false, "The QIAutoAudit does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.QIAutoAudit", VersionInfo.TargetVersionNumber), false, "The QIAutoAudit does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.QIAutoAudit", VersionInfo.TargetVersionNumber), false, "The QIAutoAudit does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "1.0.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.QIAutoAudit", VersionInfo.TargetVersionNumber), true, "The QIAutoAudit does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the QIFocusAudit attribute
        /// Initial version: 1.2.0.0
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDEDocumentQIFocusAudit()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.2.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.QIFocusAudit", VersionInfo.TargetVersionNumber), false, "The QIFocusAudit does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.QIFocusAudit", VersionInfo.TargetVersionNumber), false, "The QIFocusAudit does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.QIFocusAudit", VersionInfo.TargetVersionNumber), false, "The QIFocusAudit does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.QIFocusAudit", VersionInfo.TargetVersionNumber), false, "The QIFocusAudit does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "1.0.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.QIFocusAudit", VersionInfo.TargetVersionNumber), true, "The QIFocusAudit does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the OverlayOffset attribute
        /// Initial version: 1.2.0.0
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDEImageOverlayOffset()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.2.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEImage.OverlayOffset", VersionInfo.TargetVersionNumber), false, "The OverlayOffset does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEImage.OverlayOffset", VersionInfo.TargetVersionNumber), false, "The OverlayOffset does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEImage.OverlayOffset", VersionInfo.TargetVersionNumber), false, "The OverlayOffset does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEImage.OverlayOffset", VersionInfo.TargetVersionNumber), false, "The OverlayOffset does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "1.0.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEImage.OverlayOffset", VersionInfo.TargetVersionNumber), true, "The OverlayOffset does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the Exclude attribute
        /// Initial version: 1.2.0.0
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDEFieldExclude()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.2.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Exclude", VersionInfo.TargetVersionNumber), false, "The Exclude does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Exclude", VersionInfo.TargetVersionNumber), false, "The Exclude does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Exclude", VersionInfo.TargetVersionNumber), false, "The Exclude does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Exclude", VersionInfo.TargetVersionNumber), false, "The Exclude does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "1.0.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Exclude", VersionInfo.TargetVersionNumber), true, "The Exclude does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the OCRAreaRect attribute
        /// Initial version: 1.2.0.0
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDEFieldOCRAreaRect()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.2.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.OCRAreaRect", VersionInfo.TargetVersionNumber), false, "The OCRAreaRect does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.OCRAreaRect", VersionInfo.TargetVersionNumber), false, "The OCRAreaRect does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.OCRAreaRect", VersionInfo.TargetVersionNumber), false, "The OCRAreaRect does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.OCRAreaRect", VersionInfo.TargetVersionNumber), false, "The OCRAreaRect does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "1.0.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.OCRAreaRect", VersionInfo.TargetVersionNumber), true, "The OCRAreaRect does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the QIFocusAudit attribute
        /// Initial version: 1.2.0.0
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDEFieldQIFocusAudit()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.2.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.QIFocusAudit", VersionInfo.TargetVersionNumber), false, "The QIFocusAudit does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.QIFocusAudit", VersionInfo.TargetVersionNumber), false, "The QIFocusAudit does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.QIFocusAudit", VersionInfo.TargetVersionNumber), false, "The QIFocusAudit does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.QIFocusAudit", VersionInfo.TargetVersionNumber), false, "The QIFocusAudit does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "1.0.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.QIFocusAudit", VersionInfo.TargetVersionNumber), true, "The QIFocusAudit does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the CharRepairs attribute
        /// Initial version: 1.0.0.0
        /// Final version: 1.3.4.2
        /// </summary>
        [TestMethod]
        public void VerifyIWDEFieldCharRepairs()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.0.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.CharRepairs", VersionInfo.TargetVersionNumber), false, "The CharRepairs does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "1.3.4.2";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.CharRepairs", VersionInfo.TargetVersionNumber), false, "The CharRepairs does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "1.3.4.3";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.CharRepairs", VersionInfo.TargetVersionNumber), true, "The CharRepairs does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.CharRepairs", VersionInfo.TargetVersionNumber), true, "The CharRepairs does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "0.0.0.9";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.CharRepairs", VersionInfo.TargetVersionNumber), true, "The CharRepairs does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the CharRepairs attribute
        /// Initial version: 1.3.4.3
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDERevisionCharRepairs()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.3.4.3";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERevision.CharRepairs", VersionInfo.TargetVersionNumber), false, "The CharRepairs does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERevision.CharRepairs", VersionInfo.TargetVersionNumber), false, "The CharRepairs does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERevision.CharRepairs", VersionInfo.TargetVersionNumber), false, "The CharRepairs does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERevision.CharRepairs", VersionInfo.TargetVersionNumber), false, "The CharRepairs does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "1.3.4.2";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERevision.CharRepairs", VersionInfo.TargetVersionNumber), true, "The CharRepairs does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the Status attribute
        /// Initial version: 1.4.0.30
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDEDocumentStatus()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.4.0.30";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.Status", VersionInfo.TargetVersionNumber), false, "The Status does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.Status", VersionInfo.TargetVersionNumber), false, "The Status does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.Status", VersionInfo.TargetVersionNumber), false, "The Status does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.Status", VersionInfo.TargetVersionNumber), false, "The Status does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "1.4.0.29";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDocument.Status", VersionInfo.TargetVersionNumber), true, "The Status does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the DataType attribute
        /// Initial version: 1.0.0.0
        /// Final version: 1.3.4.3
        /// </summary>
        [TestMethod]
        public void VerifyIWDEFieldDataType()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.0.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.DataType", VersionInfo.TargetVersionNumber), false, "The DataType does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "1.3.4.3";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.DataType", VersionInfo.TargetVersionNumber), false, "The DataType does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "1.3.4.31";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.DataType", VersionInfo.TargetVersionNumber), true, "The DataType does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.DataType", VersionInfo.TargetVersionNumber), true, "The DataType does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "0.0.0.9";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.DataType", VersionInfo.TargetVersionNumber), true, "The DataType does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the Flags attribute
        /// Initial version: 1.0.0.0
        /// Final version: 1.3.4.3
        /// </summary>
        [TestMethod]
        public void VerifyIWDEFieldFlags()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.0.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Flags", VersionInfo.TargetVersionNumber), false, "The Flags does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "1.3.4.3";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Flags", VersionInfo.TargetVersionNumber), false, "The Flags does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "1.3.4.31";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Flags", VersionInfo.TargetVersionNumber), true, "The Flags does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Flags", VersionInfo.TargetVersionNumber), true, "The Flags does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "0.0.0.9";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Flags", VersionInfo.TargetVersionNumber), true, "The Flags does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the MiscFlags attribute
        /// Initial version: 1.0.0.0
        /// Final version: 1.3.4.3
        /// </summary>
        [TestMethod]
        public void VerifyIWDEFieldMiscFlags()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.0.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.MiscFlags", VersionInfo.TargetVersionNumber), false, "The MiscFlags does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "1.3.4.3";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.MiscFlags", VersionInfo.TargetVersionNumber), false, "The MiscFlags does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "1.3.4.31";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.MiscFlags", VersionInfo.TargetVersionNumber), true, "The MiscFlags does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.MiscFlags", VersionInfo.TargetVersionNumber), true, "The MiscFlags does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "0.0.0.9";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.MiscFlags", VersionInfo.TargetVersionNumber), true, "The MiscFlags does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the Tag attribute
        /// Initial version: 1.4.0.30
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDEFieldTag()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.4.0.30";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Tag", VersionInfo.TargetVersionNumber), false, "The Tag does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Tag", VersionInfo.TargetVersionNumber), false, "The Tag does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Tag", VersionInfo.TargetVersionNumber), false, "The Tag does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Tag", VersionInfo.TargetVersionNumber), false, "The Tag does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "1.4.0.29";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.Tag", VersionInfo.TargetVersionNumber), true, "The Tag does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the CustomData attribute
        /// Initial version: 1.4.0.30
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDEFieldCustomData()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.4.0.30";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.CustomData", VersionInfo.TargetVersionNumber), false, "The CustomData does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.CustomData", VersionInfo.TargetVersionNumber), false, "The CustomData does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.CustomData", VersionInfo.TargetVersionNumber), false, "The CustomData does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.CustomData", VersionInfo.TargetVersionNumber), false, "The CustomData does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "1.4.0.29";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEField.CustomData", VersionInfo.TargetVersionNumber), true, "The CustomData does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the Conf attribute
        /// Initial version: 1.0.0.0
        /// Final version: 1.3.4.3
        /// </summary>
        [TestMethod]
        public void VerifyIWDECharRepairConf()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.0.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDECharRepair.Conf", VersionInfo.TargetVersionNumber), false, "The Conf does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "1.3.4.3";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDECharRepair.Conf", VersionInfo.TargetVersionNumber), false, "The Conf does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "1.3.4.31";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDECharRepair.Conf", VersionInfo.TargetVersionNumber), true, "The Conf does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDECharRepair.Conf", VersionInfo.TargetVersionNumber), true, "The Conf does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "0.0.0.9";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDECharRepair.Conf", VersionInfo.TargetVersionNumber), true, "The Conf does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the Confidence attribute
        /// Initial version: 1.4.0.30
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDECharRepairConfidence()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.4.0.30";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDECharRepair.Confidence", VersionInfo.TargetVersionNumber), false, "The Confidence does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDECharRepair.Confidence", VersionInfo.TargetVersionNumber), false, "The Confidence does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDECharRepair.Confidence", VersionInfo.TargetVersionNumber), false, "The Confidence does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDECharRepair.Confidence", VersionInfo.TargetVersionNumber), false, "The Confidence does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "1.4.0.29";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDECharRepair.Confidence", VersionInfo.TargetVersionNumber), true, "The Confidence does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the StartTime attribute
        /// Initial version: 2.1.0.16
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDESessionStartTime()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "2.1.0.16";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDESession.StartTime", VersionInfo.TargetVersionNumber), false, "The StartTime does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDESession.StartTime", VersionInfo.TargetVersionNumber), false, "The StartTime does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDESession.StartTime", VersionInfo.TargetVersionNumber), false, "The StartTime does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDESession.StartTime", VersionInfo.TargetVersionNumber), false, "The StartTime does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "2.1.0.15";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDESession.StartTime", VersionInfo.TargetVersionNumber), true, "The StartTime does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the EndTime attribute
        /// Initial version: 2.1.0.16
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDESessionEndTime()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "2.1.0.16";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDESession.EndTime", VersionInfo.TargetVersionNumber), false, "The EndTime does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDESession.EndTime", VersionInfo.TargetVersionNumber), false, "The EndTime does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDESession.EndTime", VersionInfo.TargetVersionNumber), false, "The EndTime does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDESession.EndTime", VersionInfo.TargetVersionNumber), false, "The EndTime does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "2.1.0.15";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDESession.EndTime", VersionInfo.TargetVersionNumber), true, "The EndTime does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the IsDeleted attribute
        /// Initial version: 2.5.0.0
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDERecordIsDeleted()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "2.5.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERecord.IsDeleted", VersionInfo.TargetVersionNumber), false, "The IsDeleted does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERecord.IsDeleted", VersionInfo.TargetVersionNumber), false, "The IsDeleted does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERecord.IsDeleted", VersionInfo.TargetVersionNumber), false, "The IsDeleted does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERecord.IsDeleted", VersionInfo.TargetVersionNumber), false, "The IsDeleted does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "2.4.0.9";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERecord.IsDeleted", VersionInfo.TargetVersionNumber), true, "The IsDeleted does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the SessionID attribute
        /// Initial version: 2.5.0.0
        /// Final version: *.*.*.*
        /// </summary>
        [TestMethod]
        public void VerifyIWDERecordSessionID()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "2.5.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERecord.SessionID", VersionInfo.TargetVersionNumber), false, "The SessionID does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "2.6.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERecord.SessionID", VersionInfo.TargetVersionNumber), false, "The SessionID does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "2.6.0.1";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERecord.SessionID", VersionInfo.TargetVersionNumber), false, "The SessionID does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERecord.SessionID", VersionInfo.TargetVersionNumber), false, "The SessionID does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "2.4.0.9";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDERecord.SessionID", VersionInfo.TargetVersionNumber), true, "The SessionID does not accomplish with version criteria.");
        }

        /// <summary>
        /// This test validates the Sessions collection
        /// Initial version: 1.0.0.0
        /// Final version: 1.4.0.30
        /// </summary>
        [TestMethod]
        public void VerifyIWDEDataSetSessions()
        {
            //Validate inferior version
            VersionInfo.TargetVersionNumber = "1.0.0.0";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDataSet.Sessions", VersionInfo.TargetVersionNumber), false, "The Sessions does not accomplish with version criteria.");

            //Validate superior version
            VersionInfo.TargetVersionNumber = "1.4.0.30";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDataSet.Sessions", VersionInfo.TargetVersionNumber), false, "The Sessions does not accomplish with version criteria.");

            //Validate after the superior version
            VersionInfo.TargetVersionNumber = "1.4.0.31";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDataSet.Sessions", VersionInfo.TargetVersionNumber), true, "The Sessions does not accomplish with version criteria.");

            //Validate with TargetVersionNumber equal to null
            VersionInfo.TargetVersionNumber = null;
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDataSet.Sessions", VersionInfo.TargetVersionNumber), true, "The Sessions does not accomplish with version criteria.");

            //Validate before the inferior version
            VersionInfo.TargetVersionNumber = "0.0.0.9";
            Assert.AreEqual(VersionHelper.FilterPropertyOrCollection("IWDEDataSet.Sessions", VersionInfo.TargetVersionNumber), true, "The Sessions does not accomplish with version criteria.");
        }
    }
}
