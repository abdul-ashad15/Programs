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
using Microsoft.QualityTools.Testing.Fakes;
using WebDX.Api.Fakes;
//using NUnit.Framework;
//using TypeMock;

namespace WebDX.Api.Tests
{
	[TestClass]
	public class ConditionalGotoEditDefTest
	{
		ResourceManager m_ResMan;
		IWDEProject m_Project;

		public ConditionalGotoEditDefTest()
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
            //m_Project = new WebDX.Api.Fakes.StubIWDEProject();
            m_Project = WDEProject.Create();

            //m_Project = new WebDX.Api.Fakes.StubIWDEProject()
            //{
                
            //};

            //using (ShimsContext.Create())
            //{
            //    WDEProject wdp = new ShimWDEProject();
            //    wdp.CreateObjRef;               
            //}

        }

		[TestCleanup]
		public void TestCleanup()
		{
			m_Project.Clear();
			m_Project = null;
			GC.Collect();
		}     

        [TestMethod]
		public void ConditionalGotosWriteToXml()
		{
            using (ShimsContext.Create())
            {
                WDEConditionalGotoEditDef def = new ShimWDEConditionalGotoEditDef();

                //IWDEConditionalGotoEditDef def = WDEConditionalGotoEditDef.Create();
                IWDEConditionalGoto cgoto = def.Gotos.Add();
                IWDEConditionalGoto cg2 = def.Gotos.Add();

                m_Project.DocumentDefs.Add("Document1");
                m_Project.DocumentDefs[0].FormDefs.Add("Form1");
                m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");

                cgoto.Control = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                cgoto.ClearFields = true;
                cgoto.Expression = "Expression1";
                cg2.Release = true;
                cg2.Expression = "Expression2";
                cg2.ClearFields = true;

                StringWriter sw = new StringWriter();
                XmlTextWriter XmlWriter = new XmlTextWriter(sw);
                XmlWriter.Formatting = Formatting.Indented;
                IWDEXmlPersist ipers = (IWDEXmlPersist)def;
                ipers.WriteToXml(XmlWriter);
                XmlWriter.Close();
                //Assert.AreEqual(m_ResMan.GetString("ConditionalGotoEditDefWTX"), sw.ToString());
                Console.WriteLine(sw.ToString());


                //IWDEConditionalGotoEditDef def = WDEConditionalGotoEditDef.Create();
                //IWDEConditionalGoto cgoto = def.Gotos.Add();
                //IWDEConditionalGoto cg2 = def.Gotos.Add();

                //m_Project.DocumentDefs.Add("Document1");
                //m_Project.DocumentDefs[0].FormDefs.Add("Form1");
                //m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");

                //cgoto.Control = m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                //cgoto.ClearFields = true;
                //cgoto.Expression = "Expression1";
                //cg2.Release = true;
                //cg2.Expression = "Expression2";
                //cg2.ClearFields = true;

                //StringWriter sw = new StringWriter();
                //XmlTextWriter XmlWriter = new XmlTextWriter(sw);
                //XmlWriter.Formatting = Formatting.Indented;
                //IWDEXmlPersist ipers = (IWDEXmlPersist) def;
                //ipers.WriteToXml(XmlWriter);
                //XmlWriter.Close();
                ////Assert.AreEqual(m_ResMan.GetString("ConditionalGotoEditDefWTX"), sw.ToString());
                //         Console.WriteLine(sw.ToString());
            }
        }

		[TestMethod]
		public void ConditionalGotosReadFromXml()
		{
			IWDEEditDefs defs = WDEEditDefs.Create(m_Project);
			IWDEConditionalGotoEditDef def = WDEConditionalGotoEditDef.Create();
			defs.Add(def);
			m_Project.DocumentDefs.Add("Document1");
			m_Project.DocumentDefs[0].FormDefs.Add("Form1");
			m_Project.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox("TextBox1");

			IWDEProjectInternal iproj = (IWDEProjectInternal) m_Project;
			iproj.Resolver = new LinkResolver();
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0];
			iproj.Resolver.AddObject(obj.GetNamePath(), m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0]);

			IWDEXmlPersist ipers = (IWDEXmlPersist) def;
			StringReader sr = new StringReader(m_ResMan.GetString("ConditionalGotoEditDefWTX"));
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			ipers.ReadFromXml(XmlReader);
			XmlReader.Close();

			iproj.Resolver.ResolveLinks();
			Assert.AreEqual(2, def.Gotos.Count, "Count");
			Assert.AreEqual("Expression1", def.Gotos[0].Expression, "Expression1");
			Assert.IsTrue(def.Gotos[0].ClearFields, "ClearFields1 is false. Expected true.");
			Assert.AreSame(m_Project.DocumentDefs[0].FormDefs[0].ControlDefs[0], def.Gotos[0].Control, "ControlDefs and Control are not the same. Expected same.");
			Assert.IsFalse(def.Gotos[0].Release, "Release is true. Expected false.");

			Assert.AreEqual("Expression2", def.Gotos[1].Expression, "Expression1");
			Assert.IsTrue(def.Gotos[1].ClearFields, "ClearFields2 is false. Expected true.");
			Assert.IsTrue(def.Gotos[1].Release, "Release2 is false. Expected true.");
			Assert.IsNull(def.Gotos[1].Control, "Control2 is not null. Expected null.");
		}
	}
}
