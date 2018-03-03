using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Xml;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using WebDX.Api.Scripts;
using NUnit.Framework;
using TypeMock;

namespace WebDX.Api.Tests
{
	[TestFixture]
	public class TempProjectTests
	{
		private IWDEProject m_Project;

		public TempProjectTests()
		{
		}

		[TestFixtureSetUp]
		public void Setup()
		{
			m_Project = WDEProject.Create();
			//m_Project.LoadFromFile(@"c:\projects\webde\c#TestProject.xml");
		}

		[Test]
		public void AnalyzeGroupBox1()
		{
			IWDEGroupBoxDef def = (IWDEGroupBoxDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[5];
			Assert.AreEqual("GroupBox1", def.ControlName, "ControlName");
			Assert.AreEqual(3, def.KeyOrder, "KeyOrder");
			Assert.AreEqual(WDEGroupBoxOption.ClearDuringRepair | WDEGroupBoxOption.RepairEntireGroup,
				def.Options, "Options");
			Assert.IsTrue(def.TabStop, "TabStop is false. Expected true.");
			Assert.AreEqual("GroupBoxDesc", def.Description, "Description");
			Assert.AreEqual("GroupBoxCaption", def.Caption, "Caption");
			Assert.AreEqual(Color.Lime, def.BackColor, "BackColor");
			Assert.AreEqual("Tahoma, 8pt", Utils.FontToString(def.ControlFont), "Font");
			Assert.AreEqual("GroupBoxHelp", def.Help, "Help");
			Assert.AreEqual("GroupBoxHint", def.Hint, "Hint");
			Assert.AreEqual(new Rectangle(395, 13, 415, 83), def.Location, "Location");
			Assert.AreSame(m_Project.DocumentDefs[0].ImageSourceDefs[0].SnippetDefs[0], def.ZoneLinks[0], "Zone5 and ZoneLinks[0] are not the same. Expected same.");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.WindowText), def.ForeColor, "ForeColor");
		}

		[Test]
		public void AnalyzeGroupBoxControls()
		{
			IWDEGroupBoxDef def = (IWDEGroupBoxDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[5];
			Assert.AreEqual(2, def.ControlDefs.Count, "ControlDefs.Count");
			IWDELabelDef ldef = (IWDELabelDef) def.ControlDefs[0];
			IWDETextBoxDef tdef = (IWDETextBoxDef) def.ControlDefs[1];

			Assert.AreEqual("Label3", ldef.ControlName, "ControlName");
			Assert.IsTrue(ldef.AutoSize, "AutoSize is false. Expected true.");
			Assert.AreEqual(ContentAlignment.MiddleRight, ldef.Alignment, "Alignment");
			Assert.AreEqual("Field3", ldef.Caption, "Caption");
			Assert.AreEqual(Color.Lime, ldef.BackColor, "BackColor");
			Assert.AreEqual("Tahoma, 8pt", Utils.FontToString(ldef.ControlFont), "Font");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.WindowText), ldef.ForeColor, "ForeColor");
			Assert.AreEqual(new Rectangle(35, 35, 37,16), ldef.Location, "Location");
			Assert.AreSame(tdef, ldef.NextControl, "NextControl and tdef are not the same. Expected same.");

			Assert.AreEqual("TextBox3", tdef.ControlName, "ControlName");
			Assert.AreEqual(WDEControlOption.AutoAdvance, tdef.Options, "Options");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[2], tdef.Field, "Field");
			Assert.AreEqual(0, tdef.KeyOrder, "KeyOrder");
			Assert.IsTrue(tdef.TabStop, "TabStop is false. Expected true.");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.Window), tdef.BackColor, "BackColor");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.WindowText), tdef.ForeColor, "ForeColor");
			Assert.AreEqual("Courier New, 10pt", Utils.FontToString(tdef.ControlFont), "TB3Font");
			Assert.AreSame(ldef, tdef.LabelLinks[0], "LabelLinks");
			Assert.AreEqual(1, tdef.LabelLinks.Count, "LabelLinks.Count");
			Assert.AreSame(m_Project.DocumentDefs[0].ImageSourceDefs[0].SnippetDefs[0].ZoneDefs[0], tdef.ZoneLinks[0], "ZoneLinks[0]");
            Assert.AreEqual(new Rectangle(72, 30, 227, 23), tdef.Location, "Location");
			
			IWDEAddressCorrectionEditDef adDef = (IWDEAddressCorrectionEditDef) tdef.EditDefs[0];
			Assert.IsFalse(adDef.Enabled, "adDef.Enabled is true. Expected false.");

			IWDEBalanceCheckEditDef bcDef = (IWDEBalanceCheckEditDef) tdef.EditDefs[1];
			Assert.IsTrue(bcDef.Enabled, "bcDef.Enabled is false. Expected true.");
			Assert.AreEqual(WDEEditErrorType.WarningWithRetry, bcDef.ErrorType, "ErrorType");
			Assert.AreEqual(WDESessionType.FullForm | WDESessionType.Field | WDESessionType.PhotoStitch,
				bcDef.SessionMode, "SessionMode");
			Assert.AreEqual("Accum1Desc", bcDef.Description, "Description");
			Assert.AreEqual("Accum1Message", bcDef.ErrorMessage, "ErrorMessage");
			Assert.AreEqual(3, bcDef.SumFields.Count, "SumFields.Count");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0], bcDef.SumFields[1], "SumFields[1] and Field1 are not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[2], bcDef.SumFields[2], "SumFields[2] and Field3 are not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0].FieldDefs[2], bcDef.SumFields[0], "SumFields[0] and Field30 are not the same. Expected same.");
		}

		[Test]
		public void AnalyzeDetailGrid()
		{
			IWDEDetailGridDef dg = (IWDEDetailGridDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[4];
			Assert.AreEqual("DetailGrid1", dg.ControlName, "ControlName");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].RecordDefs[0], dg.RecordDef, "RecordDef and Record2 are not the same. Expected same.");
			Assert.AreEqual(2, dg.KeyOrder, "KeyOrder");
			Assert.AreEqual(WDEDetailGridOption.RestrictExit, dg.Options, "Options");
			Assert.AreEqual(3, dg.Rows, "Rows");
			Assert.AreEqual(WDERecordNumberPosition.Left, dg.RecordNumberPosition, "RecordNumberPosition");
			Assert.IsTrue(dg.TabStop, "TabStop is false. Expected true.");
			Assert.AreEqual("Description", dg.Description, "Description");
			Assert.AreEqual(Color.Aqua, dg.BackColor, "BackColor");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.WindowText), dg.ForeColor, "ForeColor");
			Assert.AreEqual("Microsoft Sans Serif, 8pt", Utils.FontToString(dg.ControlFont), "ControlFont");
			Assert.AreEqual(Color.Blue, dg.HeaderBackColor, "HeaderBackColor");
			Assert.AreEqual(Color.FromKnownColor(KnownColor.Window), dg.HeaderForeColor, "HeaderForeColor");
			Assert.AreEqual("Microsoft Sans Serif, 8pt, style=Bold", Utils.FontToString(dg.HeaderFont), "HeaderFont");
			Assert.AreEqual(30, dg.HeaderHeight, "HeaderHeight");
			Assert.AreEqual("Help", dg.Help, "Help");
			Assert.AreEqual("Hint", dg.Hint, "Hint");
			Assert.AreEqual(new Rectangle(86, 103, 726, 153), dg.Location, "Location");
			Assert.AreEqual(m_Project.DocumentDefs[0].ImageSourceDefs[0].DetailZoneDefs[0], dg.DetailZoneDef, "DetailZoneDef");
			Assert.IsTrue(dg.OnEnter.Enabled, "OnEnter.Enabled is false. Expected true.");
			Assert.AreEqual("Document1.Form1.DetailGrid1.OnEnter", dg.OnEnter.ScriptFullName, "OnEnterScriptFullName");
			Assert.AreEqual("Document1.Form1.DetailGrid1.OnExit", dg.OnExit.ScriptFullName, "OnExitScriptFullName");
			Assert.AreEqual("Document1.Form1.DetailGrid1.OnValidate", dg.OnValidate.ScriptFullName, "OnValidateScriptFullName");
			Assert.IsTrue(dg.OnExit.Enabled, "OnExit.Enabled is false. Expected true.");
			Assert.IsFalse(dg.OnValidate.Enabled, "OnValidate.Enabled is true. Expected false.");
		}

		[Test]
		public void AnalyzeDetailControls()
		{
			IWDEDetailGridDef dg = (IWDEDetailGridDef) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[4];
			Assert.AreEqual(3, dg.HeaderControlDefs.Count, "Header.Count");
			IWDELabelDef ldef = (IWDELabelDef) dg.HeaderControlDefs[0];
			Assert.AreEqual("Label4", ldef.ControlName);
			ldef = (IWDELabelDef) dg.HeaderControlDefs[1];
			Assert.AreEqual("Label5", ldef.ControlName);
			ldef = (IWDELabelDef) dg.HeaderControlDefs[2];
			Assert.AreEqual("Label6", ldef.ControlName);

			Assert.AreEqual(3, dg.ControlDefs.Count, "ControlDefs.Count");
			IWDETextBoxDef tdef = (IWDETextBoxDef) dg.ControlDefs[0];
			Assert.AreEqual("TextBox4", tdef.ControlName, "TextBox4");
			Assert.AreSame(m_Project.DocumentDefs[0].ImageSourceDefs[0].DetailZoneDefs[0].ZoneDefs[0], tdef.ZoneLinks[0], "tb4.ZoneLink");
			tdef = (IWDETextBoxDef) dg.ControlDefs[1];
			Assert.AreEqual("TextBox5", tdef.ControlName, "TextBox5");
			Assert.AreSame(m_Project.DocumentDefs[0].ImageSourceDefs[0].DetailZoneDefs[0].ZoneDefs[1], tdef.ZoneLinks[0], "tb5.ZoneLink");
			tdef = (IWDETextBoxDef) dg.ControlDefs[2];
			Assert.AreEqual("TextBox6", tdef.ControlName, "TextBox6");
			Assert.AreSame(m_Project.DocumentDefs[0].ImageSourceDefs[0].DetailZoneDefs[0].ZoneDefs[2], tdef.ZoneLinks[0], "tb6.ZoneLink");
		}
		
		[Test]
		public void SaveTest()
		{
			IWDEProjectPM iproj = (IWDEProjectPM) m_Project;
			iproj.SaveToFile(@"c:\projects\webde\C#Converted.xml");
			m_Project.Clear();
			m_Project.LoadFromFile(@"c:\projects\webde\C#Converted.xml");
		}

		[Test]
		public void StdHcfa()
		{
			m_Project.LoadFromFile(@"c:\temp\forjeffm\stdhcfa.xml");
			IWDEDataSet ds = WDEDataSet.Create();
			ds.Project = m_Project;
			ds.CreateData("User", "Task", WDEOpenMode.Create, "Location");
			ds.Documents.Append("HCFA");
			ds.Documents[0].Records.Append("Header");
			ds.Documents[0].Records[0].Records.Append("Detail");

			Assert.AreEqual(1, ds.Documents.Count, "Count");
			Assert.AreEqual("HCFA", ds.Documents[0].DocumentDef.DocType, "DocType");
			Assert.AreEqual("Header", ds.Documents[0].Records[0].RecordDef.RecType, "RecTypeH");
			Assert.AreEqual("Detail", ds.Documents[0].Records[0].Records[0].RecordDef.RecType, "RecTypeD");
		}

		[Test]
		public void MeridianUgly()
		{
			m_Project.LoadFromFile(@"c:\temp\yadhu\Meridian.xml");
		}

		[Test]
		public void RP3()
		{
			IWDEDataSet ds = WDEDataSet.Create();
			ds.LoadFromFile("User", "Task", WDEOpenMode.Edit, @"c:\temp\yadhu\rp3files\200508250001829.xml");
		}
	}
}
