using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Forms;

using WebDX.Api.Scripts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mscoree;
using WebDX.Api.Moles;

namespace WebDX.Api.Tests
{
    [TestClass]
    public class ScriptLoaderTests
    {
        private string _tempDir;

        public ScriptLoaderTests()
        {
            string currDir = Environment.CurrentDirectory;
            AppDomain.CurrentDomain.SetData("APPBASE", currDir);
        }

        [TestInitialize]
        public void SetUp()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
        }

        [TestCleanup]
        public void TearDown()
        {
            bool succeeded = false;
            while (!succeeded)
            {
                try
                {
                    Directory.Delete(_tempDir, true);
                    succeeded = true;
                }
                catch(UnauthorizedAccessException)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        [TestMethod]
        public void Constructor()
        {
            ScriptLoader l = new ScriptLoader(@"c:\temp");
            try
            {
                Assert.IsTrue(l.SeparateDomain, "SeparateDomain is false. Expected true.");
                List<string> domainList = ListDomains();
                Assert.IsTrue(domainList.Contains("ACS.WebDE.ScriptDomain"), "ScriptDomain not in domainList");
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void LoadAssemblies()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                AppDomain scriptDomain = GetScriptDomain();
                scriptDomain.DoCallBack(new CrossAppDomainDelegate(ScriptUtils.CrossDomainGetAssemblies));
                string[] list = (string[])scriptDomain.GetData("AssemblyList");
                List<string> assemblyList = new List<string>();
                assemblyList.AddRange(list);
                Assert.IsFalse(assemblyList.Contains("TestProjectScript"), "Test assembly exists.");

                CopyTestDll();

                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);
                scriptDomain.DoCallBack(new CrossAppDomainDelegate(ScriptUtils.CrossDomainGetAssemblies));
                list = (string[])scriptDomain.GetData("AssemblyList");
                assemblyList.Clear();
                assemblyList.AddRange(list);
                Assert.IsTrue(assemblyList.Contains("TestProjectScript"), "Test assembly does not exist.");
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void TextBoxEnter()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                TextBoxEventImpl tb = new TextBoxEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                proj.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
                IWDETextBoxDef_R1 def = (IWDETextBoxDef_R1)proj.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                def.OnEnter.Enabled = true;
                def.OnEnter.ScriptFullName = "WebDX.Api.Tests.TextBoxEvents.OnEnter";

                l.LinkEvents(tb, def);

                tb.DoEnter();
                Assert.AreEqual("TB OnEnter executed", Host.Field.Value, "Field value");
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void TextBoxNullScriptFullName()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                TextBoxEventImpl tb = new TextBoxEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                proj.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
                IWDETextBoxDef_R1 def = (IWDETextBoxDef_R1)proj.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                def.OnEnter.Enabled = true;
                def.OnEnter.ScriptFullName = "WebDX.Api.Tests.TextBoxEvents.OnEnter";

                l.LinkEvents(tb, def);

                tb.DoValidate();
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void TextBoxEmptyScriptFullName()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                TextBoxEventImpl tb = new TextBoxEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                proj.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
                IWDETextBoxDef_R1 def = (IWDETextBoxDef_R1)proj.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                def.OnEnter.Enabled = true;
                def.OnEnter.ScriptFullName = "WebDX.Api.Tests.TextBoxEvents.OnEnter";
                def.OnValidate.ScriptFullName = "";

                l.LinkEvents(tb, def);

                tb.DoValidate();
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void TextBoxExit()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                TextBoxEventImpl tb = new TextBoxEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                proj.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
                IWDETextBoxDef_R1 def = (IWDETextBoxDef_R1)proj.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                def.OnExit.Enabled = true;
                def.OnExit.ScriptFullName = "WebDX.Api.Tests.TextBoxEvents.OnExit";

                l.LinkEvents(tb, def);

                ExitEventArgs ex = new ExitEventArgs(true);
                tb.DoExit(ex);
                Assert.AreEqual("TB OnExit advancing", Host.Field.Value, "Field value advancing");
                ex = new ExitEventArgs(false);
                tb.DoExit(ex);
                Assert.AreEqual("TB OnExit retreating", Host.Field.Value, "Field value retreating");
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void TextBoxKeyPress()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                TextBoxEventImpl tb = new TextBoxEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                proj.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
                IWDETextBoxDef_R1 def = (IWDETextBoxDef_R1)proj.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                def.OnKeyPress.Enabled = true;
                def.OnKeyPress.ScriptFullName = "WebDX.Api.Tests.TextBoxEvents.OnKeyPress";

                l.LinkEvents(tb, def);

                TextBoxKeyPressEventArgs ex = new TextBoxKeyPressEventArgs('F');
                Assert.IsFalse(ex.Handled, "Handled is true. Expected false.");
                tb.DoKeyPress(ex);
                Assert.IsTrue(ex.Handled, "Handled is false. Expected true.");
                Assert.AreEqual('X', ex.KeyChar, "KeyChar");
                Assert.AreEqual("F", Host.Field.Value, "Field.Value");
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void TextBoxValidate()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                TextBoxEventImpl tb = new TextBoxEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                proj.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
                IWDETextBoxDef_R1 def = (IWDETextBoxDef_R1)proj.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                def.OnValidate.Enabled = true;
                def.OnValidate.ScriptFullName = "WebDX.Api.Tests.TextBoxEvents.OnValidate";

                l.LinkEvents(tb, def);

                tb.DoValidate();
                Assert.AreEqual("TB OnValidate executed", Host.Field.Value, "Field.Value");
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void DetailGridEvents()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                DetailGridEventImpl dg = new DetailGridEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                IWDEDetailGridDef def = proj.DocumentDefs[0].FormDefs[0].ControlDefs.AddDetailGrid();
                def.OnEnter.Enabled = true;
                def.OnEnter.ScriptFullName = "WebDX.Api.Tests.DetailGridEvents.OnEnter";
                def.OnExit.Enabled = true;
                def.OnExit.ScriptFullName = "WebDX.Api.Tests.DetailGridEvents.OnExit";

                l.LinkEvents(dg, def);

                dg.DoEnter();
                Assert.AreEqual("DG OnEnter executed", Host.Field.Value, "Field.Value OnEnter");
                dg.DoExit();
                Assert.AreEqual("DG OnExit executed", Host.Field.Value, "Field.Value OnExit");
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void FormEvents()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                FormEventImpl fm = new FormEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                IWDEFormDef_R1 def = (IWDEFormDef_R1)proj.DocumentDefs[0].FormDefs[0];
                def.OnEnter.Enabled = true;
                def.OnEnter.ScriptFullName = "WebDX.Api.Tests.FormEvents.OnEnter";
                def.OnExit.Enabled = true;
                def.OnExit.ScriptFullName = "WebDX.Api.Tests.FormEvents.OnExit";

                l.LinkEvents(fm, def);

                fm.DoEnter();
                Assert.AreEqual("FM OnEnter executed", Host.Field.Value, "Field.Value OnEnter");
                fm.DoExit();
                Assert.AreEqual("FM OnExit executed", Host.Field.Value, "Field.Value OnExit");
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void ImageEvents()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                ImageEventImpl im = new ImageEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                IWDEProject_R1 def = (IWDEProject_R1)proj;
                def.OnPageChange.Enabled = true;
                def.OnPageChange.ScriptFullName = "WebDX.Api.Tests.ImageEvents.OnPageChange";

                l.LinkEvents(im, def);

                im.DoPageChange();
                Assert.AreEqual("IM OnPageChange executed", Host.Field.Value, "Field.Value");
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void ProjectEvents()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                ProjectEventImpl pj = new ProjectEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject_R1 proj = (IWDEProject_R1)WDEProject.Create();
                proj.OnDocumentRejected.Enabled = true;
                proj.OnDocumentRejected.ScriptFullName = "WebDX.Api.Tests.ProjectEvents.OnDocReject";
                proj.OnDocumentUnRejected.Enabled = true;
                proj.OnDocumentUnRejected.ScriptFullName = "WebDX.Api.Tests.ProjectEvents.OnDocUnreject";
                proj.OnEndWork.Enabled = true;
                proj.OnEndWork.ScriptFullName = "WebDX.Api.Tests.ProjectEvents.OnEndWork";
                proj.OnKeyPress.Enabled = true;
                proj.OnKeyPress.ScriptFullName = "WebDX.Api.Tests.ProjectEvents.OnKeyPress";
                proj.OnStartWork.Enabled = true;
                proj.OnStartWork.ScriptFullName = "WebDX.Api.Tests.ProjectEvents.OnStartWork";

                l.LinkEvents(pj, proj);

                RejectEventArgs rex = new RejectEventArgs("01", "BadScan");
                pj.DoDocumentReject(rex);
                Assert.AreEqual("01|BadScan", Host.Field.Value, "DocReject");
                pj.DoDocumentUnreject();
                Assert.AreEqual("PJ OnDocumentUnreject executed", Host.Field.Value, "DocUnReject");
                EndWorkEventArgs eex = new EndWorkEventArgs(EndWorkReason.Reject, "01", "BadScan");
                pj.DoEndWork(eex);
                Assert.AreEqual("Reject|01|BadScan", Host.Field.Value, "EndWork");
                ProjectKeyEventArgs kex = new ProjectKeyEventArgs(false, false, false, Keys.F1);
                pj.DoKeyPress(kex);
                Assert.AreEqual("False|False|False|F1|False", Host.Field.Value, "KeyPressValue");
                Assert.IsTrue(kex.Alt, "Alt");
                Assert.IsTrue(kex.Control, "Control");
                Assert.IsTrue(kex.Handled, "Handled");
                Assert.IsTrue(kex.Shift, "Shift");
                Assert.AreEqual(Keys.F12, kex.KeyCode, "KeyCode");
                StartWorkEventArgs stex /* :) */ = new StartWorkEventArgs(StartWorkType.SpecificGet);
                pj.DoStartWork(stex);
                Assert.AreEqual("SpecificGet", Host.Field.Value, "StartWork");
            }
            finally
            {
                l.Unload();
            }
        }

        /*
        [TestMethod]
        [ExpectedException(typeof(Exception), "Type: WebDX.Api.Tests.NoContainer is not marked as an event container.")]
        public void NoContainer()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                TextBoxEventImpl tb = new TextBoxEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                proj.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
                IWDETextBoxDef_R1 def = (IWDETextBoxDef_R1)proj.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                def.OnEnter.Enabled = true;
                def.OnEnter.ScriptFullName = "WebDX.Api.Tests.NoContainer.OnEnter";

                l.LinkEvents(tb, def);
            }
            finally
            {
                l.Unload();
            }
        }
         */

        [TestMethod]
        [ExpectedException(typeof(BindingException))]
        public void BadSig()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                TextBoxEventImpl tb = new TextBoxEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                proj.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
                IWDETextBoxDef_R1 def = (IWDETextBoxDef_R1)proj.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                def.OnKeyPress.Enabled = true;
                def.OnKeyPress.ScriptFullName = "WebDX.Api.Tests.BadSig.BadKeyPress";

                l.LinkEvents(tb, def);

                TextBoxKeyPressEventArgs ex = new TextBoxKeyPressEventArgs('c');
                tb.DoKeyPress(ex); // no binding exceptions happen until invocation.
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void ErrorExcept()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                TextBoxEventImpl tb = new TextBoxEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                proj.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
                IWDETextBoxDef_R1 def = (IWDETextBoxDef_R1)proj.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                def.OnEnter.Enabled = true;
                def.OnEnter.ScriptFullName = "WebDX.Api.Tests.Exceptions.Error";

                l.LinkEvents(tb, def);

                try
                {
                    tb.DoEnter();
                    Assert.Fail("DoEnter did not throw an exception.");
                }
                catch (ScriptException ex)
                {
                    Assert.AreEqual("An error message", ex.Message, "Error");
                }
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void ErrorWarn()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                TextBoxEventImpl tb = new TextBoxEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                proj.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
                IWDETextBoxDef_R1 def = (IWDETextBoxDef_R1)proj.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                def.OnEnter.Enabled = true;
                def.OnEnter.ScriptFullName = "WebDX.Api.Tests.Exceptions.Warning";

                l.LinkEvents(tb, def);

                try
                {
                    tb.DoEnter();
                    Assert.Fail("DoEnter did not throw an exception.");
                }
                catch (ScriptWarning ex)
                {
                    Assert.AreEqual("Warning message", ex.Message, "Error");
                    Assert.IsFalse(ex.Retry, "Retry is true. Expected false.");
                }
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void ErrorWarnRetry()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                TextBoxEventImpl tb = new TextBoxEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                proj.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
                IWDETextBoxDef_R1 def = (IWDETextBoxDef_R1)proj.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                def.OnEnter.Enabled = true;
                def.OnEnter.ScriptFullName = "WebDX.Api.Tests.Exceptions.WarningRetry";

                l.LinkEvents(tb, def);

                try
                {
                    tb.DoEnter();
                    Assert.Fail("DoEnter did not throw an exception.");
                }
                catch (ScriptWarning ex)
                {
                    Assert.AreEqual("Warning retry message", ex.Message, "Error");
                    Assert.IsTrue(ex.Retry, "Retry is false. Expected true.");
                }
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void ErrorOther()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                TextBoxEventImpl tb = new TextBoxEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                proj.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
                IWDETextBoxDef_R1 def = (IWDETextBoxDef_R1)proj.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                def.OnEnter.Enabled = true;
                def.OnEnter.ScriptFullName = "WebDX.Api.Tests.Exceptions.Except";

                l.LinkEvents(tb, def);

                try
                {
                    tb.DoEnter();
                    Assert.Fail("DoEnter did not throw an exception.");
                }
                catch (Exception ex)
                {
                    Assert.AreSame(typeof(Exception), ex.GetType(), "Ex is not an exception");
                    Assert.AreEqual("Exception", ex.Message, "Error");
                }
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AppDomainUnloadedException))]
        public void Unload()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                TextBoxEventImpl tb = new TextBoxEventImpl();
                Host.Field = new FieldImpl("Field1") as IScriptField;

                IWDEProject proj = WDEProject.Create();
                proj.DocumentDefs.Add();
                proj.DocumentDefs[0].FormDefs.Add();
                proj.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
                IWDETextBoxDef_R1 def = (IWDETextBoxDef_R1)proj.DocumentDefs[0].FormDefs[0].ControlDefs[0];
                def.OnEnter.Enabled = true;
                def.OnEnter.ScriptFullName = "WebDX.Api.Tests.TextBoxEvents.OnEnter";

                l.LinkEvents(tb, def);

                tb.DoEnter();
                Assert.AreEqual("TB OnEnter executed", Host.Field.Value, "Field value");
                l.Unload();
                Host.Field.Value = "";
                tb.DoEnter();
                Assert.AreEqual("", Host.Field.Value, "Field blank");
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        public void Edit()
        {
            ScriptLoader l = new ScriptLoader(_tempDir);
            try
            {
                CopyTestDll();
                List<string> newList = new List<string>();
                newList.Add("TestProjectScript.dll");

                l.LoadProjectAssemblies(newList);

                IWDEProject p = WDEProject.Create();
                p.DocumentDefs.Add();
                p.DocumentDefs[0].FormDefs.Add();
                IWDETextBoxDef tb = p.DocumentDefs[0].FormDefs[0].ControlDefs.AddTextBox();
                IWDEEditDef def = WDEEditDef.Create();
                tb.EditDefs.Add(def);
                def.Enabled = true;
                ((IWDEEditDef_R1)def).FullName = "WebDX.Api.Tests.TestEdit";
                ((IWDEEditDef_R1)def).EditParams = "<DocumentEdit />";

                Host.Field = new FieldImpl("Field1") as IScriptField;

                l.RunEdits((IWDETextBoxDef_R1)tb);
                Assert.AreEqual("DocumentEdit", Host.Field.Value);
            }
            finally
            {
                l.Unload();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetSessionPlugin_NullArg()
        {
            ScriptLoader loader = new ScriptLoader(_tempDir);
            try
            {
                loader.GetSessionPlugin(null);
            }
            finally
            {
                loader.Unload();
            }
        }

        [TestMethod]
        public void GetSessionPlugin_NullProp()
        {
            var sessDef = new SIWDESessionDef_R2();

            sessDef.PluginNameGet += () => { return null; };
            ScriptLoader loader = new ScriptLoader(_tempDir);
            try
            {
                ISessionPlugin plug = loader.GetSessionPlugin((IWDESessionDef_R2)sessDef);
                Assert.IsNull(plug, "Plug is not null. Expected null.");
            }
            finally
            {
                loader.Unload();
            }
        }

        [TestMethod]
        public void GetSessionPlugin_EmptyProp()
        {
            var sessDef = new SIWDESessionDef_R2();

            sessDef.PluginNameGet += () => { return ""; };
            ScriptLoader loader = new ScriptLoader(_tempDir);
            try
            {
                ISessionPlugin plug = loader.GetSessionPlugin((IWDESessionDef_R2)sessDef);
                Assert.IsNull(plug, "Plug is not null. Expected null.");
            }
            finally
            {
                loader.Unload();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.IO.FileNotFoundException))]
        public void GetSessionPlugin_BadName()
        {
            var sessDef = new SIWDESessionDef_R2();

            sessDef.PluginNameGet += () => { return "BadName.dll"; };
            ScriptLoader loader = new ScriptLoader(_tempDir);
            try
            {
                loader.GetSessionPlugin((IWDESessionDef_R2)sessDef);
            }
            finally
            {
                loader.Unload();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetClientPlugins_NoArg()
        {
            ScriptLoader.GetClientPlugins(null);
        }

        [TestMethod]
        public void GetClientPlugins_EmptyArg()
        {
            ScriptLoader loader = new ScriptLoader(_tempDir);
            try
            {
                List<string> pluginNames = new List<string>();
                List<IClientPlugin> res = ScriptLoader.GetClientPlugins(pluginNames);
                Assert.IsNotNull(res, "Res is null. Expected not null.");
                Assert.AreEqual(0, res.Count, "Count");
            }
            finally
            {
                loader.Unload();
            }
        }

        #region Private Methods

        private void CopyTestDll()
        {
            string testDllPath = Path.GetFullPath(@"..\..\..\..\TestProjectScript\bin\Debug\TestProjectScript.dll");
            Assert.IsTrue(File.Exists(testDllPath), "Test dll file not found.");

            string dest = Path.Combine(_tempDir, "TestProjectScript.dll");
            File.Copy(testDllPath, dest, true);
        }

        private AppDomain GetScriptDomain()
        {
            AppDomain result = null;
            CorRuntimeHostClass host = new CorRuntimeHostClass();
            try
            {
                IntPtr enumHandle;
                host.EnumDomains(out enumHandle);
                while (true)
                {
                    object domain = null;
                    host.NextDomain(enumHandle, out domain);
                    if (domain == null)
                        break;
                    AppDomain appd = (AppDomain)domain;
                    if (appd.FriendlyName == "ACS.WebDE.ScriptDomain")
                    {
                         result = appd;
                        //break;
                    }
                    appd = null;
                }
                host.CloseEnum(enumHandle);
            }
            finally
            {
                Marshal.ReleaseComObject(host);
            }

            return result;
        }

        private List<string> ListDomains()
        {
            List<string> result = new List<string>();
            CorRuntimeHostClass host = new CorRuntimeHostClass();
            try
            {
                IntPtr enumHandle;
                host.EnumDomains(out enumHandle);
                while (true)
                {
                    object domain = null;
                    host.NextDomain(enumHandle, out domain);
                    if (domain == null)
                        break;
                    AppDomain appd = (AppDomain)domain;
                    result.Add(appd.FriendlyName);
                    appd = null;
                }
                host.CloseEnum(enumHandle);
            }
            finally
            {
                Marshal.ReleaseComObject(host);
            }

            return result;
        }

        #endregion
    }

    public class TextBoxEventImpl : MarshalByRefObject, IScriptTextBoxEvents
    {
        public TextBoxEventImpl() { }

        #region IScriptTextBoxEvents Members

        public event ScriptBaseEvent OnEnter;

        public event ScriptTextBoxKeyPressEvent OnKeyPress;

        public event ScriptBaseEvent OnValidate;

        public event ScriptTextBoxExitEvent OnExit;

        #endregion

        public void DoEnter()
        {
            if (OnEnter != null)
                OnEnter();
        }

        public void DoExit(ExitEventArgs e)
        {
            if (OnExit != null)
                OnExit(e);
        }

        public void DoKeyPress(TextBoxKeyPressEventArgs e)
        {
            if (OnKeyPress != null)
                OnKeyPress(e);
        }

        public void DoValidate()
        {
            if (OnValidate != null)
                OnValidate();
        }
    }

    public class DetailGridEventImpl : MarshalByRefObject, IScriptDetailGridEvents
    {
        public DetailGridEventImpl() { }

        #region IScriptDetailGridEvents Members

        public event ScriptBaseEvent OnEnter;

        public event ScriptBaseEvent OnExit;

        #endregion

        public void DoEnter()
        {
            if (OnEnter != null)
                OnEnter();
        }

        public void DoExit()
        {
            if (OnExit != null)
                OnExit();
        }
    }

    public class FormEventImpl : MarshalByRefObject, IScriptFormEvents
    {
        public FormEventImpl() { }

        #region IScriptFormEvents Members

        public event ScriptBaseEvent OnEnter;

        public event ScriptBaseEvent OnExit;

        #endregion

        public void DoEnter()
        {
            if (OnEnter != null)
                OnEnter();
        }

        public void DoExit()
        {
            if (OnExit != null)
                OnExit();
        }
    }

    public class ImageEventImpl : MarshalByRefObject, IScriptImageEvents
    {

        public ImageEventImpl() { }

        #region IScriptImageEvents Members

        public event ScriptBaseEvent OnPageChange;

        #endregion

        public void DoPageChange()
        {
            if (OnPageChange != null)
                OnPageChange();
        }
    }

    public class ProjectEventImpl : MarshalByRefObject, IScriptProjectEvents
    {
        public ProjectEventImpl() { }

        #region IScriptProjectEvents Members

        public event ScriptProjectKeyPressEvent OnKeyPress;

        public event ScriptBaseEvent OnDocumentUnreject;

        public event ScriptDocumentRejectEvent OnDocumentReject;

        public event ScriptStartWorkEvent OnStartWork;

        public event ScriptEndWorkEvent OnEndWork;

        #endregion

        public void DoKeyPress(ProjectKeyEventArgs e)
        {
            if (OnKeyPress != null)
                OnKeyPress(e);
        }

        public void DoDocumentUnreject()
        {
            if (OnDocumentUnreject != null)
                OnDocumentUnreject();
        }

        public void DoDocumentReject(RejectEventArgs e)
        {
            if (OnDocumentReject != null)
                OnDocumentReject(e);
        }

        public void DoStartWork(StartWorkEventArgs e)
        {
            if (OnStartWork != null)
                OnStartWork(e);
        }

        public void DoEndWork(EndWorkEventArgs e)
        {
            if (OnEndWork != null)
                OnEndWork(e);
        }
    }

    public class FieldImpl : MarshalByRefObject, IScriptField
    {
        private string _value;
        private string _fieldName;
        private int _length;

        public FieldImpl(string fieldName)
        {
            _fieldName = fieldName;
            _length = 0;
        }

        #region IScriptField Members

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public bool Exclude
        {
            get { return false; }
        }

        public int Length
        {
            get { return _length; }
        }

        public string FlagDescription
        {
            get
            {
                return "";
            }
            set
            {
                // do nothing
            }
        }

        public string CustomData
        {
            get
            {
                return "";
            }
            set
            {
                // do nothing
            }
        }

        public string FieldName
        {
            get { return _fieldName; }
        }

        public void Dupe()
        {
            // do nothing.
        }

        public IScriptRevisions Revisions
        {
            get { return null; }
        }

        public WDEFieldStatus UpdateStatus
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

		public double NumberValue
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}

    public class ScriptUtils : MarshalByRefObject
    {
        public ScriptUtils() { }

        public static void CrossDomainGetAssemblies()
        {
            List<string> assemblyList = GetAssemblyList(AppDomain.CurrentDomain);
            string[] list = new string[assemblyList.Count];
            assemblyList.CopyTo(list);
            AppDomain.CurrentDomain.SetData("AssemblyList", list);
        }

        private static List<string> GetAssemblyList(AppDomain domain)
        {
            List<string> result = new List<string>();
            foreach (Assembly domAssembly in domain.GetAssemblies())
                result.Add(domAssembly.GetName().Name);
            return result;
        }
    }
}
