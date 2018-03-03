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
//using NUnit.Framework;
//using TypeMock;

namespace WebDX.Api.Tests
{
	[TestClass]
	public class TableLookupEditDefTests
	{
		ResourceManager m_ResMan;
		IWDEProject m_Project;

		public TableLookupEditDefTests()
		{
		}

		[ClassInitialize]
		public void Setup()
		{
			m_ResMan = new ResourceManager("WebDX.Api.Tests.ProjectExpectedResults", Assembly.GetExecutingAssembly());
		}

		[TestInitialize]
		public void Init()
		{
			m_Project = WDEProject.Create();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			m_Project.Clear();
			m_Project = null;
			GC.Collect();
		}

		[TestMethod]
		public void TableLookupWriteToXml()
		{
			IWDETableLookupEditDef def = WDETableLookupEditDef.Create();
			IWDETableLookup tl1 = def.TableLookups.Add();
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].RecordDefs.Add("Record1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field1");
			m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs.Add("Field2");
			
			tl1.Database = "Database1";
			tl1.Options = WDETableLookupOption.FailIfNoRecords | WDETableLookupOption.Filter |
				WDETableLookupOption.LookupIfBlank | WDETableLookupOption.NullsToSpaces | WDETableLookupOption.OneHitPopup |
				WDETableLookupOption.ReviewResults | WDETableLookupOption.SavePos | WDETableLookupOption.ShowDiffs | WDETableLookupOption.VerifyPluggedData;
			tl1.Query = "Query1";

			IWDETableLookup tl2 = def.TableLookups.Add();

			tl2.Database = "Database2";
			tl2.Query = "Query2";

			IWDELookupResultField res1 = tl1.ResultFields.Add();
			res1.DBFieldName = "DBField1";
			res1.Field = m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0];
			res1.Options = WDELookupResultFieldOption.AllowFilter | WDELookupResultFieldOption.Display;
			IWDELookupResultField res2 = tl1.ResultFields.Add();
			res2.DBFieldName = "DBField2";
			res2.Field = m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1];

			StringWriter sw = new StringWriter();
			XmlTextWriter XmlWriter = new XmlTextWriter(sw);
			XmlWriter.Formatting = Formatting.Indented;
			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.Close();
			Assert.AreEqual(m_ResMan.GetString("TableLookupEditDefWTX"), sw.ToString());
		}

		[TestMethod]
		public void TableLookupReadFromXml()
		{
			IWDEEditDefs defs = WDEEditDefs.Create(m_Project);
			IWDETableLookupEditDef def = WDETableLookupEditDef.Create();
			defs.Add(def);
			IWDETableLookup tl1 = def.TableLookups.Add();
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

			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			StringReader sr = new StringReader(m_ResMan.GetString("TableLookupEditDefWTX"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();

			iproj.Resolver.ResolveLinks();

			Assert.AreEqual(WDEExecuteOption.Validate, def.ExecuteOn);
			Assert.AreEqual(2, def.TableLookups.Count, "TableLookups.Count");
			Assert.AreEqual(2, def.TableLookups[0].ResultFields.Count, "ResultFields[0].Count");
			Assert.AreEqual(0, def.TableLookups[1].ResultFields.Count, "ResultFields[1].Count");
			Assert.AreEqual("Database1", def.TableLookups[0].Database, "Database1");
			Assert.AreEqual("Query1", def.TableLookups[0].Query, "Query1");
			Assert.AreEqual(WDETableLookupOption.FailIfNoRecords | WDETableLookupOption.Filter |
				WDETableLookupOption.LookupIfBlank | WDETableLookupOption.NullsToSpaces | WDETableLookupOption.OneHitPopup |
				WDETableLookupOption.ReviewResults | WDETableLookupOption.SavePos | WDETableLookupOption.ShowDiffs | WDETableLookupOption.VerifyPluggedData,
				def.TableLookups[0].Options, "Options[0]");
			Assert.AreEqual(WDELookupResultFieldOption.AllowFilter | WDELookupResultFieldOption.Display, def.TableLookups[0].ResultFields[0].Options);
			Assert.AreEqual("Database2", def.TableLookups[1].Database, "Database2");
			Assert.AreEqual("Query2", def.TableLookups[1].Query, "Query2");

			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[0], def.TableLookups[0].ResultFields[0].Field, "Field 0 not the same. Expected same.");
			Assert.AreSame(m_Project.DocumentDefs[0].RecordDefs[0].FieldDefs[1], def.TableLookups[0].ResultFields[1].Field, "Field 1 not the same. Expected same.");
		}
	}
}
