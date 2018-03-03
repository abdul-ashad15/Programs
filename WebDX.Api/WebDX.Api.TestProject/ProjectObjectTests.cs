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
using WebDX.Api.Fakes;
//using TypeMock;

namespace WebDX.Api.TestProject
{
	[TestClass]
	public class ProjectObjectTests
	{
        IWDEProject_R5 project;

        [TestInitialize]
		public void Init()
		{
            project = new Fakes.StubIWDEProject_R5();
            project = (StubIWDEProject_R5)WDEProject.Create();
            //project = Fakes.StubWDEProject.Create();
        }

		[TestCleanup]
		public void TearDown()
		{
            project.Clear();
            project = null;
			GC.Collect();
		}

        [TestMethod]
        public void WebDXExternalAssembly()
        {
            project.ExternalAssemblies.Add("WebDX.StandardEdits.dll");
            project.ExternalAssemblies.Add("StudioScripts_Scripts.dll");

            MemoryStream ms = new MemoryStream();
            ((IWDEProjectPM)project).SaveToStream(ms);

            ms = new MemoryStream(ms.ToArray());
            project.Clear();
            project.LoadFromStream(ms);
            Assert.AreEqual(2, project.ExternalAssemblies.Count, "Count");
            Assert.AreEqual("WebDX.StandardEdits.dll", project.ExternalAssemblies[0], "Assembly Name");
        }

        [TestMethod]
        public void WebDXRefAssembly()
        {
            project.References.Add("WebDX.Api.dll");

            MemoryStream ms = new MemoryStream();
            ((IWDEProjectPM)project).SaveToStream(ms);

            ms = new MemoryStream(ms.ToArray());
            project.Clear();
            project.LoadFromStream(ms);
            Assert.AreEqual(1, project.References.Count, "Count");
            Assert.AreEqual("WebDX.Api.dll", project.References[0], "Assembly Name");
        }

        [TestMethod]
        public void WebDXRefAssemblyClear()
        {
            project.References.Add("WebDX.Api.dll");
            project.Clear();
            Assert.AreEqual(0, project.References.Count);
        }

        [TestMethod]
        public void WebDXExternalAssemblyClear()
        {
            project.ExternalAssemblies.Add("WebDX.StandardEdits.dll");
            project.ExternalAssemblies.Remove("WebDX.StandardEdits.dll");
            Assert.AreEqual(0, project.ExternalAssemblies.Count);
        }

        [TestMethod]
		public void ProjectSaveToStream()
		{
			project.CreatedBy = "APITests";
            project.Description = "Description";
            project.ModifiedBy = "Modifier";
            project.ScriptLanguage = WDEScriptLanguage.CSharpNet;
            project.Options = WDEProjectOption.ShowCharSetError | WDEProjectOption.TrackImage;
            project.Script = "SCRIPT";
		
			MemoryStream mst = new MemoryStream();
            ((IWDEProjectPM)project).SaveToStream(mst);

			MemoryStream mst2 = new MemoryStream(mst.ToArray());
			
			DateTime cdate = project.DateCreated;
			DateTime mdate = project.DateModified;

            project.Clear();
            project.LoadFromStream(mst2);
			Assert.AreEqual("APITests", project.CreatedBy, "CreatedBy");
			Assert.AreEqual("Description", project.Description, "Description");
			Assert.AreEqual("Modifier", project.ModifiedBy, "ModifiedBy");
			Assert.AreEqual(WDEScriptLanguage.VBNet, project.ScriptLanguage, "ScriptLanguage");
			Assert.AreEqual(WDEProjectOption.ShowCharSetError | WDEProjectOption.TrackImage, project.Options, "Options");
			Assert.AreEqual("SCRIPT", project.Script, "Script");
			if(cdate.Subtract(project.DateCreated).TotalMilliseconds < 1)
				cdate = project.DateCreated;
			if(mdate.Subtract(project.DateModified).TotalMilliseconds < 1)
				mdate = project.DateModified;
			Assert.AreEqual(cdate, project.DateCreated, "DateCreated");
			Assert.AreEqual(mdate, project.DateModified, "DateModified");
			mst.Close();
			mst2.Close();
		}
	}
}
