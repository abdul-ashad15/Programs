using System;
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
	/// <summary>
	/// Tests for WDEFields
	/// </summary>
	[TestClass]
	public class FieldsObjectTests
	{
		//ResourceManager m_ResMan;

		//MockObject m_DataSet;
		//MockObject m_Document;
		//MockObject m_Record;
		//MockObject m_Sessions;
		//MockObject m_Session;
		//MockObject m_FieldDef;
		//IWDEFields m_Fields;

        ResourceManager m_ResMan;
        IWDEDataSet m_DataSet;
        IWDEDocument m_Document;
        IWDERecord m_Record;
        IWDESessions m_Sessions;
        IWDESession_R1 m_Session;
        IWDEFieldDef m_FieldDef;
        IWDEFields m_Fields;

        public FieldsObjectTests()
		{
		}

		[TestInitialize]
		public void Init()
		{
			m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());

			//m_DataSet = MockManager.MockObject(typeof(IWDEDataSet));
			//m_Document = MockManager.MockObject(typeof(IWDEDocument));
			//m_Record = MockManager.MockObject(typeof(IWDERecord));
			//m_Sessions = MockManager.MockObject(typeof(IWDESessions));
			//m_Session = MockManager.MockObject(typeof(IWDESession_R1));
			//m_FieldDef = MockManager.MockObject(typeof(IWDEFieldDef));

			//m_Record.ExpectGetAlways("Document", m_Document.Object);
			//m_Document.ExpectGetAlways("DataSet", m_DataSet.Object);
			//m_DataSet.ExpectGetAlways("Sessions", m_Sessions.Object);
			//m_Sessions.ExpectGetAlways("Item", m_Session.Object);

			//m_Session.ExpectGetAlways("SessionID", 1);
			//m_Session.ExpectGetAlways("Mode", WDEOpenMode.Edit);
			//m_FieldDef.ExpectGetAlways("Options", WDEFieldOption.None);
			//m_FieldDef.ExpectGetAlways("CharSet", "");
			//m_FieldDef.ExpectGetAlways("DefaultValue", "");
			//m_FieldDef.ExpectGetAlways("FieldName", "Field1");

			m_Fields = WDEFields.Create((IWDERecord) m_Record);
		}

		[TestCleanup]
		public void Cleanup()
		{
			m_DataSet = null;
			m_Document = null;
			m_Record = null;
			m_Sessions = null;
			m_Session = null;
			m_FieldDef = null;
			m_Fields = null;

			//MockManager.Verify();
			GC.Collect();
		}

		[TestMethod]
		public void Add()
		{
			//m_FieldDef.ExpectGetAlways("FieldName", "Field1");
			IWDEFieldsInternal ifields = m_Fields as IWDEFieldsInternal;
			ifields.Add((IWDEFieldDef) m_FieldDef);
			Assert.AreEqual(1, m_Fields.Count, "Count");
			Assert.AreEqual("Field1", m_Fields[0].FieldName, "FieldName");
		}

		[TestMethod]
		public void DefaultValue()
		{
			//m_FieldDef.ExpectGetAlways("FieldName", "Field1");
			//m_FieldDef.ExpectGetAlways("DefaultValue", "ONE");
			IWDEFieldsInternal ifields = m_Fields as IWDEFieldsInternal;
			ifields.Add((IWDEFieldDef) m_FieldDef);
			Assert.AreEqual("ONE", m_Fields[0].Value);
		}

		[TestMethod]
		public void Find()
		{
			//MockObject fd1 = MockManager.MockObject(typeof(IWDEFieldDef));
			//MockObject fd2 = MockManager.MockObject(typeof(IWDEFieldDef));
			//MockObject fd3 = MockManager.MockObject(typeof(IWDEFieldDef));

			//fd1.ExpectGetAlways("Options", WDEFieldOption.None);
			//fd1.ExpectGetAlways("CharSet", "");
			//fd1.ExpectGetAlways("DefaultValue", "");
			//fd1.ExpectGetAlways("FieldName", "Field1");

			//fd2.ExpectGetAlways("Options", WDEFieldOption.None);
			//fd2.ExpectGetAlways("CharSet", "");
			//fd2.ExpectGetAlways("DefaultValue", "");
			//fd2.ExpectGetAlways("FieldName", "Field2");

			//fd3.ExpectGetAlways("Options", WDEFieldOption.None);
			//fd3.ExpectGetAlways("CharSet", "");
			//fd3.ExpectGetAlways("DefaultValue", "");
			//fd3.ExpectGetAlways("FieldName", "Field3");

			//IWDEFieldsInternal ifields = m_Fields as IWDEFieldsInternal;
			//ifields.Add((IWDEFieldDef) fd1.Object);
			//ifields.Add((IWDEFieldDef) fd2.Object);
			//ifields.Add((IWDEFieldDef) fd3.Object);

			//Assert.AreEqual(0, m_Fields.Find("Field1"), "Field1");
			//Assert.AreEqual(1, m_Fields.Find("Field2"), "Field2");
			//Assert.AreEqual(2, m_Fields.Find("Field3"), "Field3");
			//Assert.AreEqual(-1, m_Fields.Find("NotThere"), "NotThere");
		}

		[TestMethod]
		public void GetIndex()
		{
			IWDEFieldsInternal ifields = m_Fields as IWDEFieldsInternal;
			ifields.Add((IWDEFieldDef) m_FieldDef);
			IWDECollection icoll = m_Fields as IWDECollection;
			Assert.AreEqual(0, icoll.GetIndex(m_Fields[0]));
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void SetIndex()
		{
			//MockObject fd1 = MockManager.MockObject(typeof(IWDEFieldDef));
			//MockObject fd2 = MockManager.MockObject(typeof(IWDEFieldDef));
			//MockObject fd3 = MockManager.MockObject(typeof(IWDEFieldDef));

			//fd1.ExpectGetAlways("Options", WDEFieldOption.None);
			//fd1.ExpectGetAlways("CharSet", "");
			//fd1.ExpectGetAlways("DefaultValue", "");
			//fd1.ExpectGetAlways("FieldName", "Field1");

			//fd2.ExpectGetAlways("Options", WDEFieldOption.None);
			//fd2.ExpectGetAlways("CharSet", "");
			//fd2.ExpectGetAlways("DefaultValue", "");
			//fd2.ExpectGetAlways("FieldName", "Field2");

			//fd3.ExpectGetAlways("Options", WDEFieldOption.None);
			//fd3.ExpectGetAlways("CharSet", "");
			//fd3.ExpectGetAlways("DefaultValue", "");
			//fd3.ExpectGetAlways("FieldName", "Field3");

			//IWDEFieldsInternal ifields = m_Fields as IWDEFieldsInternal;
			//ifields.Add((IWDEFieldDef) fd1.Object);
			//ifields.Add((IWDEFieldDef) fd2.Object);
			//ifields.Add((IWDEFieldDef) fd3.Object);

			//IWDECollection icoll = m_Fields as IWDECollection;
			//icoll.SetIndex(m_Fields[2], 0);
			//Assert.AreEqual("Field3", m_Fields[0].FieldName, "Field3");
			//Assert.AreEqual("Field1", m_Fields[1].FieldName, "Field1");
			//Assert.AreEqual("Field2", m_Fields[2].FieldName, "Field2");

			//icoll.SetIndex(this, 3);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SetIndexNull()
		{
			IWDECollection icoll = m_Fields as IWDECollection;
			icoll.SetIndex(null, 0);
		}

		[TestMethod]
		public void WriteToXml()
		{
			//m_DataSet.ExpectGetAlways("PersistFlags", false);
			//MockObject fd1 = MockManager.MockObject(typeof(IWDEFieldDef));
			//MockObject fd2 = MockManager.MockObject(typeof(IWDEFieldDef));
			//MockObject fd3 = MockManager.MockObject(typeof(IWDEFieldDef));

			//fd1.ExpectGetAlways("Options", WDEFieldOption.None);
			//fd1.ExpectGetAlways("CharSet", "");
			//fd1.ExpectGetAlways("DefaultValue", "");
			//fd1.ExpectGetAlways("FieldName", "Field1");

			//fd2.ExpectGetAlways("Options", WDEFieldOption.None);
			//fd2.ExpectGetAlways("CharSet", "");
			//fd2.ExpectGetAlways("DefaultValue", "");
			//fd2.ExpectGetAlways("FieldName", "Field2");

			//fd3.ExpectGetAlways("Options", WDEFieldOption.None);
			//fd3.ExpectGetAlways("CharSet", "");
			//fd3.ExpectGetAlways("DefaultValue", "");
			//fd3.ExpectGetAlways("FieldName", "Field3");

			//IWDEFieldsInternal ifields = m_Fields as IWDEFieldsInternal;
			//ifields.Add((IWDEFieldDef) fd1.Object);
			//ifields.Add((IWDEFieldDef) fd2.Object);
			//ifields.Add((IWDEFieldDef) fd3.Object);

			IWDEXmlPersist ipers = m_Fields as IWDEXmlPersist;
			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("FieldsWriteToXml"), sw.ToString());
		}

		[TestMethod]
		public void ReadFromXml()
		{
			IWDEXmlPersist ipers = m_Fields as IWDEXmlPersist;
			string test = "<DataSet>" + m_ResMan.GetString("FieldsWriteToXml") + "</DataSet>";
			StringReader sr = new StringReader(test);
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			XmlReader.Read();
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			Assert.AreEqual(3, m_Fields.Count);
		}
	}
}
