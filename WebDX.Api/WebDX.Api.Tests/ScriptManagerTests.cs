using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;

using WebDX.Api.Scripts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Microsoft.CSharp;

namespace WebDX.Api.Tests
{
    [TestClass]
    public class ScriptManagerTests
    {
        private string _tempDir;

        public ScriptManagerTests() { }

        [TestInitialize]
        public void SetUp()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
            string currDir = Environment.CurrentDirectory;
            AppDomain.CurrentDomain.SetData("APPBASE", currDir);
        }

        [TestCleanup]
        public void TearDown()
        {
            Directory.Delete(_tempDir, true);
        }

        [TestMethod]
        public void CompileScript()
        {
            ScriptManager scriptman = new ScriptManager();
            try
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerResults res = scriptman.CompileScript(provider, ScriptResources.SimpleScript, new string[] { "System.dll" }, Path.Combine(_tempDir, "TestScript.dll"));
                Assert.AreEqual(0, res.Errors.Count, "ErrorCount");
                Assert.IsTrue(File.Exists(Path.Combine(_tempDir, "TestScript.dll")), "TestScript.dll does not exist.");
            }
            finally
            {
                scriptman.Unload();
            }
        }

        [TestMethod]
        public void LoadAssembly()
        {
            ScriptManager scriptman = new ScriptManager();
            try
            {
                CopyTestDll();

                scriptman.LoadAssembly(Path.Combine(_tempDir, "TestProjectScript.dll"));
            }
            finally
            {
                scriptman.Unload();
            }
        }

        [TestMethod]
        public void LoadEdit()
        {
            ScriptManager scriptman = new ScriptManager();
            try
            {
                CopyTestDll();

                scriptman.LoadAssembly(Path.Combine(_tempDir, "TestProjectScript.dll"));
                scriptman.GetEdit("WebDX.Api.Tests.TestEdit");
            }
            finally
            {
                scriptman.Unload();
            }
        }

        [TestMethod]
        public void GetLoadedEvents()
        {
            ScriptManager scriptman = new ScriptManager();
            try
            {
                CopyTestDll();

                scriptman.LoadAssembly(Path.Combine(_tempDir, "TestProjectScript.dll"));
                
                List<string> events = scriptman.GetLoadedEvents(EventType.TextBoxEnter);
                Assert.IsTrue(events.Count > 0, "No events found");
            }
            finally
            {
                scriptman.Unload();
            }
        }

        [TestMethod]
        public void GetLoadedEdits()
        {
            ScriptManager scriptman = new ScriptManager();
            try
            {
                CopyTestDll();

                scriptman.LoadAssembly(Path.Combine(_tempDir, "TestProjectScript.dll"));

                List<string> events = scriptman.GetLoadedEdits();
                Assert.IsTrue(events.Count > 0, "No edits found");
            }
            finally
            {
                scriptman.Unload();
            }
        }

        [TestMethod]
        public void GetDeployList()
        {
            ScriptManager scriptman = new ScriptManager();
            try
            {
                CopyTestDll();

                scriptman.LoadAssembly(Path.Combine(_tempDir, "TestProjectScript.dll"));

                List<string> list = scriptman.GetAssemblyDeploymentList();
                Assert.AreEqual(1, list.Count, "Count");
                Assert.AreEqual("TestProjectScript.dll", list[0], "List[0]");
            }
            finally
            {
                scriptman.Unload();
            }
        }

        private void CopyTestDll()
        {
            string testDllPath = Path.GetFullPath(@"..\..\..\TestProjectScript\bin\Debug\TestProjectScript.dll");
            Assert.IsTrue(File.Exists(testDllPath), "Test dll file not found.");

            string dest = Path.Combine(_tempDir, "TestProjectScript.dll");
            File.Copy(testDllPath, dest, true);
        }
    }
}
