using System;
using System.Collections;
using System.Collections.Specialized;
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
	[TestClass]
	public class NamingTests
	{
		private IWDEProject m_Project;

		public NamingTests()
		{
		}

		[TestInitialize]
		public void Init()
		{
			m_Project = WDEProject.Create();
		}

		[TestCleanup]
		public void Cleanup()
		{
			m_Project = null;
			GC.Collect();
		}

		[TestMethod]
		public void AddNull()
		{
			TestException(m_Project.DocumentDefs, "Add", new Type[] {typeof(string)}, null, "DocumentDefs.Add(null)", typeof(ArgumentNullException));
			//TestException(m_Project.RejectCodes, "Add", new Type[] {typeof(string)}, null, "RejectCodes.Add(null)", typeof(ArgumentNullException));
			TestException(m_Project.SessionDefs, "Add", new Type[] {typeof(string)}, null, "SessionDefs.Add(null)", typeof(ArgumentNullException));

			m_Project.DocumentDefs.Add();
			TestException(m_Project.DocumentDefs[0].RecordDefs, "Add", new Type[] {typeof(string)}, null, "RecordDefs.Add(null)", typeof(ArgumentNullException));
			m_Project.DocumentDefs[0].RecordDefs.Add();
			TestException(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs, "Add", new Type[] {typeof(string)}, null, "FieldDefs.Add(null)", typeof(ArgumentNullException));
			
			TestException(m_Project.DocumentDefs[0].FormDefs, "Add", new Type[] {typeof(string)}, null, "FormDefs.Add(null)", typeof(ArgumentNullException));
			TestException(m_Project.DocumentDefs[0].ImageSourceDefs, "Add", new Type[] {typeof(string)}, null, "ImageSourceDefs.Add(null)", typeof(ArgumentNullException));
			m_Project.DocumentDefs[0].ImageSourceDefs.Add();
			TestException(m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs, "Add", new Type[] {typeof(string)}, null, "ZoneDefs.Add(null)", typeof(ArgumentNullException));
			TestException(m_Project.DocumentDefs[0].ImageSourceDefs[0].SnippetDefs, "Add", new Type[] {typeof(string)}, null, "SnippetDefs.Add(null)", typeof(ArgumentNullException));
			TestException(m_Project.DocumentDefs[0].ImageSourceDefs[0].DetailZoneDefs, "Add", new Type[] {typeof(string)}, null, "DetailZoneDefs.Add(null)", typeof(ArgumentNullException));

			m_Project.DocumentDefs[0].FormDefs.Add();
			TestException(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs, "AddDetailGrid", new Type[] {typeof(string)}, null, "ControlDefs.AddDetailGrid(null)", typeof(ArgumentNullException));
			TestException(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs, "AddLabel", new Type[] {typeof(string)}, null, "ControlDefs.AddLabel(null)", typeof(ArgumentNullException));
			TestException(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs, "AddTextBox", new Type[] {typeof(string)}, null, "ControlDefs.AddTextBox(null)", typeof(ArgumentNullException));
			TestException(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs, "AddGroupBox", new Type[] {typeof(string)}, null, "ControlDefs.AddGroupBox(null)", typeof(ArgumentNullException));
		}

		[TestMethod]
		public void AddInvalidChars()
		{
			RunInvalidCharTests(m_Project.DocumentDefs, "Add", new Type[] {typeof(string)}, "DocumentDefs.Add({0})");
			//RunInvalidCharTests(m_Project.RejectCodes, "Add", new Type[] {typeof(string)}, "RejectCodes.Add({0})", new string[] {" invalid", "invalid ", "inval id", "!@#$!#@$", "in-valid"});
			RunInvalidCharTests(m_Project.SessionDefs, "Add", new Type[] {typeof(string)}, "SessionDefs.Add({0})");

			m_Project.DocumentDefs.Add();
			RunInvalidCharTests(m_Project.DocumentDefs[0].RecordDefs, "Add", new Type[] {typeof(string)}, "RecordDefs.Add({0})");
			m_Project.DocumentDefs[0].RecordDefs.Add();
			RunInvalidCharTests(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs, "Add", new Type[] {typeof(string)}, "FieldDefs.Add({0})");
			
			RunInvalidCharTests(m_Project.DocumentDefs[0].FormDefs, "Add", new Type[] {typeof(string)}, "FormDefs.Add({0})");
			RunInvalidCharTests(m_Project.DocumentDefs[0].ImageSourceDefs, "Add", new Type[] {typeof(string)}, "ImageSourceDefs.Add({0})");
			m_Project.DocumentDefs[0].ImageSourceDefs.Add();
			RunInvalidCharTests(m_Project.DocumentDefs[0].ImageSourceDefs[0].ZoneDefs, "Add", new Type[] {typeof(string)}, "ZoneDefs.Add({0})");
			RunInvalidCharTests(m_Project.DocumentDefs[0].ImageSourceDefs[0].SnippetDefs, "Add", new Type[] {typeof(string)}, "SnippetDefs.Add({0})");
			RunInvalidCharTests(m_Project.DocumentDefs[0].ImageSourceDefs[0].DetailZoneDefs, "Add", new Type[] {typeof(string)}, "DetailZoneDefs.Add({0})");

			m_Project.DocumentDefs[0].FormDefs.Add();
			RunInvalidCharTests(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs, "AddDetailGrid", new Type[] {typeof(string)}, "ControlDefs.AddDetailGrid({0})");
			RunInvalidCharTests(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs, "AddLabel", new Type[] {typeof(string)}, "ControlDefs.AddLabel({0})");
			RunInvalidCharTests(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs, "AddTextBox", new Type[] {typeof(string)}, "ControlDefs.AddTextBox({0})");
			RunInvalidCharTests(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs, "AddGroupBox", new Type[] {typeof(string)}, "ControlDefs.AddGroupBox({0})");
		}

		[TestMethod]
		public void DuplicateInCollection()
		{
			m_Project.DocumentDefs.Add("Document1");
			TestException(m_Project.DocumentDefs, "Add", new Type[] {typeof(string)}, "Document1", "DocumentDefs.Add(Document1)", typeof(WDEException));
			//m_Project.RejectCodes.Add("Reject1");
			//TestException(m_Project.RejectCodes, "Add", new Type[] {typeof(string)}, "Reject1", "RejectCodes.Add(Reject1)", typeof(WDEException));
			m_Project.SessionDefs.Add("SessionDef1");
			TestException(m_Project.SessionDefs, "Add", new Type[] {typeof(string)}, "SessionDef1", "SessionDefs.Add(SessionDef1)", typeof(WDEException));

			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			TestException(m_Project.DocumentDefs[0].RecordDefs, "Add", new Type[] {typeof(string)}, "Record1", "RecordDefs.Add(Record1)", typeof(WDEException));
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			TestException(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs, "Add", new Type[] {typeof(string)}, "Field1", "FieldDefs.Add(Field1)", typeof(WDEException));
			
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			TestException(m_Project.DocumentDefs[0].FormDefs, "Add", new Type[] {typeof(string)}, "Form1", "FormDefs.Add(Form1)", typeof(WDEException));
			m_Project.DocumentDefs[0].ImageSourceDefs.Add("ImageType1");
			TestException(m_Project.DocumentDefs[0].ImageSourceDefs, "Add", new Type[] {typeof(string)}, "ImageType1", "ImageSourceDefs.Add(ImageType1)", typeof(WDEException));

			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddDetailGrid("DetailGrid1");
			TestException(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs, "AddDetailGrid", new Type[] {typeof(string)}, "DetailGrid1", "ControlDefs.AddDetailGrid(DetailGrid1)", typeof(WDEException));
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddLabel("Label1");
			TestException(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs, "AddLabel", new Type[] {typeof(string)}, "Label1", "ControlDefs.AddLabel(Label1)", typeof(WDEException));
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");
			TestException(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs, "AddTextBox", new Type[] {typeof(string)}, "TextBox1", "ControlDefs.AddTextBox(TextBox1)", typeof(WDEException));
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddGroupBox("GroupBox1");
			TestException(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs, "AddGroupBox", new Type[] {typeof(string)}, "GroupBox1", "ControlDefs.AddGroupBox(GroupBox1)", typeof(WDEException));
		}

		[TestMethod]
		public void DuplicateBetweenCollection()
		{
			m_Project.DocumentDefs.Add("Document1");
			//TestException(m_Project.RejectCodes, "Add", new Type[] {typeof(string)}, "Document1", "RejectCodes.Add(Document1)", typeof(WDEException));
			TestException(m_Project.SessionDefs, "Add", new Type[] {typeof(string)}, "Document1", "SessionDefs.Add(Document1)", typeof(WDEException));
			m_Project.Clear();
			//m_Project.RejectCodes.Add("Reject1");
			TestException(m_Project.DocumentDefs, "Add", new Type[] {typeof(string)}, "Reject1", "DocumentDefs.Add(Reject1)", typeof(WDEException));
			m_Project.Clear();
			m_Project.DocumentDefs.Add("Document1");

			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			TestException(m_Project.DocumentDefs[0].FormDefs, "Add", new Type[] {typeof(string)}, "Record1", "FormDefs.Add(Record1)", typeof(WDEException));
			TestException(m_Project.DocumentDefs[0].ImageSourceDefs, "Add", new Type[] {typeof(string)}, "Record1", "ImageSourceDefs.Add(Record1)", typeof(WDEException));
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			TestException(m_Project.DocumentDefs[0].RecordDefs, "Add", new Type[] {typeof(string)}, "Form1", "RecordDefs.Add(Form1)", typeof(WDEException));
		}

		[TestMethod]
		public void DuplicateControl()
		{
			m_Project.DocumentDefs.Add();
			m_Project.DocumentDefs[0].FormDefs.Add();
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");
			TestException(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs, "AddTextBox", new Type[] {typeof(string)}, "TextBox1", "ControlDefs.AddTextBox(TextBox1)", typeof(WDEException));

			IWDEDetailGridDef def = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddDetailGrid();
			TestException(def.HeaderControlDefs, "AddLabel", new Type[] {typeof(string)}, "TextBox1", "DetailGridDef.HeaderDefs.AddLabel(TextBox1)", typeof(WDEException));
			TestException(def.ControlDefs, "AddTextBox", new Type[] {typeof(string)}, "TextBox1", "DetailGridDef.HeaderDefs.AddTextBox(TextBox1)", typeof(WDEException));
			def.ControlDefs.AddTextBox("TextBox2");
			TestException(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs, "AddTextBox", new Type[] {typeof(string)}, "TextBox2", "ControlDefs.AddTextBox(TextBox2)", typeof(WDEException));
		}

		[TestMethod]
		public void AddName()
		{
			m_Project.DocumentDefs.Add("Document2");
			Assert.AreEqual("Document2", m_Project.DocumentDefs[0].DocType, "DocType");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record2");
			Assert.AreEqual("Record2", m_Project.DocumentDefs[0].RecordDefs[0].RecType, "RecType");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field2");
			Assert.AreEqual("Field2", m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0].FieldName, "FieldName");

			m_Project.DocumentDefs[0].FormDefs.Add("Form2");
			Assert.AreEqual("Form2", m_Project.DocumentDefs[0].FormDefs[0].FormName, "FormName");
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddDetailGrid("DetailGrid2");
			Assert.AreEqual("DetailGrid2", m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0].ControlName, "DetailGrid2");
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddLabel("Label2");
			Assert.AreEqual("Label2", m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[1].ControlName, "Label2");
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox2");
			Assert.AreEqual("TextBox2", m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[2].ControlName, "TextBox2");
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddGroupBox("GroupBox2");
			Assert.AreEqual("GroupBox2", m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[3].ControlName, "GroupBox2");

			//m_Project.RejectCodes.Add("Reject2");
			//Assert.AreEqual("Reject2", m_Project.RejectCodes[0].Name, "Reject2");
			m_Project.SessionDefs.Add("Session2");
			Assert.AreEqual("Session2", m_Project.SessionDefs[0].SessionDefName, "Session2");
		}

		[TestMethod]
		public void AddDefault()
		{
			m_Project.DocumentDefs.Add();
			Assert.AreEqual("Document1", m_Project.DocumentDefs[0].DocType, "DocType");
			m_Project.DocumentDefs[0].RecordDefs.Add();
			Assert.AreEqual("Record1", m_Project.DocumentDefs[0].RecordDefs[0].RecType, "RecType");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add();
			Assert.AreEqual("Field1", m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0].FieldName, "FieldName");

			m_Project.DocumentDefs[0].FormDefs.Add();
			Assert.AreEqual("Form1", m_Project.DocumentDefs[0].FormDefs[0].FormName, "FormName");
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddDetailGrid();
			Assert.AreEqual("DetailGrid1", m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0].ControlName, "DetailGrid1");
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddLabel();
			Assert.AreEqual("Label1", m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[1].ControlName, "Label1");
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
			Assert.AreEqual("TextBox1", m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[2].ControlName, "TextBox1");
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddGroupBox();
			Assert.AreEqual("GroupBox1", m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[3].ControlName, "GroupBox1");

			//m_Project.RejectCodes.Add();
			//Assert.AreEqual("RejectCode1", m_Project.RejectCodes[0].Name, "Reject1");
			m_Project.SessionDefs.Add();
			Assert.AreEqual("SessionDef1", m_Project.SessionDefs[0].SessionDefName, "Session1");

			m_Project.DocumentDefs.Add();
			Assert.AreEqual("Document2", m_Project.DocumentDefs[1].DocType, "DocType2");
		}

		private void RunInvalidCharTests(object collection, string methodName, Type[] argTypes, string failMessage)
		{
			RunInvalidCharTests(collection, methodName, argTypes, failMessage, null);
		}

		private void RunInvalidCharTests(object collection, string methodName, Type[] argTypes, string failMessage, string[] tests)
		{
			if (tests == null) 
				tests = new string[] {" invalid", "invalid ", "inval id", "!@#$!#@$", "9invalid", "in-valid"};

			foreach(string testString in tests)
			{
				TestException(collection, methodName, argTypes, testString, string.Format(failMessage, new object[] {testString}), typeof(WDEException));
			}
		}

		private void TestException(object collection, string methodName, Type[] argTypes, string nameValue, string failMessage, Type expectedExceptionType)
		{
			MethodInfo info = collection.GetType().GetMethod(methodName, argTypes);

			try
			{
				info.Invoke(collection, new object[] {nameValue});
				Assert.Fail(string.Format("No exception on {0}. Expected exception.", new object[] {failMessage}));
			}
			catch(TargetInvocationException ex)
			{
				if(ex.InnerException.GetType() != expectedExceptionType)
					throw ex.InnerException;
			}
		}
	}
}
