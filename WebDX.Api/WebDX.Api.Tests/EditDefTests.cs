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
using TypeMock;

namespace WebDX.Api.Tests
{
	[TestClass]
	public class EditDefTests
	{
		ResourceManager m_ResMan;
		IWDEProject m_Project;

		public EditDefTests()
        {
            m_ResMan = new ResourceManager("WebDX.Api.Tests.ProjectExpectedResults", Assembly.GetExecutingAssembly());
		}

		[TestInitialize]
		public void Init()
		{
			m_Project = WDEProject.Create();
		}
		
		[TestCleanup]
		public void TearDown()
		{
			m_Project.Clear();
			m_Project = null;
			GC.Collect();
		}

		[TestMethod]
		public void WriteToXml()
		{
            IWDEEditDef def = WDEEditDef.Create();
            def.Description = "Desc";
            ((IWDEEditDef_R1)def).DisplayName = "DisplayName";
            def.Enabled = true;
            def.ErrorMessage = "ErrorMessage";
            def.ErrorType = WDEEditErrorType.WarningWithRetry;
            ((IWDEEditDef_R1)def).FullName = "FullName";
            ((IWDEEditDef_R1)def).EditParams = m_ResMan.GetString("AddressCorrEditDefWTX");
            def.SessionMode = WDESessionType.FullForm;

			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;

			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();

			Assert.AreEqual(m_ResMan.GetString("EditDefWTX"), sw.ToString());
		}

		[TestMethod]
		public void ReadFromXml()
		{
			IWDEEditDefs defs = WDEEditDefs.Create(m_Project);
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field2");

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0]);
			obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1]);

			IWDEXmlPersist ipers = (IWDEXmlPersist) defs;
			StringReader sr = new StringReader(m_ResMan.GetString("AddressCorrEditDefWTX"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();
			
			iproj.Resolver.ResolveLinks();

            Assert.AreEqual(1, defs.Count, "Count");
            Assert.AreEqual(m_ResMan.GetString("AddressCorrEditDefWTX"), ((IWDEEditDef_R1)defs[0]).EditParams, "EditParams");
            Assert.AreEqual("AddressCorrection", defs[0].DisplayName, "DisplayName");
            Assert.AreEqual(true, defs[0].Enabled, "Enabled");
            Assert.AreEqual("ACS.WebDE.Edits.AddressCorrection", defs[0].FullName, "FullName");
		}
	}
}
