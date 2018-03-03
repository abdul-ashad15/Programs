using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace WebDX.Api.Scripts
{
    public class ScriptManager : MarshalByRefObject
    {
        #region Private Fields

        private string _targetDomainName;
        private List<DomainData> _loadedDomains;

        #endregion

        #region Constructor

        public ScriptManager()
        {
            _loadedDomains = new List<DomainData>();
        }

        #endregion

        #region Public Members

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public CompilerResults CompileScript(CodeDomProvider provider, string script, string[] referencedAssemblies, string outputFileName)
        {
            return CompileScript(provider, script, referencedAssemblies, outputFileName, false);
        }

        public CompilerResults CompileScript(CodeDomProvider provider, string script, string[] referencedAssemblies, string outputFileName, bool includeDebugInformation)
        {
            _targetDomainName = Path.GetFileName(outputFileName);
            DomainData data = _loadedDomains.Find(new Predicate<DomainData>(AppDomainMatch));
            if (data != null)
            {
                AppDomain.Unload(data.Domain);
            }

            AppDomain compileDomain = CreateNewDomain(Path.GetFileName(outputFileName));
            data = _loadedDomains.Find(new Predicate<DomainData>(AppDomainMatch));
            ScriptCompiler compiler = (ScriptCompiler)compileDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ScriptCompiler).FullName);
            data.Loader = (AssemblyLoader)compileDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(AssemblyLoader).FullName);
            return compiler.CompileScript(provider, script, referencedAssemblies, outputFileName, includeDebugInformation);
        }

        public List<string> GetLoadedEvents(EventType targetType)
        {
            List<string> result = new List<string>();

            foreach (DomainData data in _loadedDomains)
            {
                result.AddRange(data.Loader.ListLoadedEvents(targetType));
            }

            return result;
        }

        public List<string> GetLoadedEdits()
        {
            List<string> result = new List<string>();

            foreach (DomainData data in _loadedDomains)
            {
                result.AddRange(data.Loader.ListLoadedEdits());
            }

            return result;
        }

        public void LoadAssembly(string assemblyFile)
        {
            if (assemblyFile == null)
                throw new ArgumentNullException("assemblyFile");

            if (!File.Exists(assemblyFile))
				throw new FileNotFoundException("Assembly file not found.", assemblyFile);

            Unload(assemblyFile);

            AppDomain loader = CreateNewDomain(Path.GetFileName(assemblyFile));
            DomainData data = _loadedDomains[_loadedDomains.Count - 1];
            data.Loader = (AssemblyLoader)loader.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(AssemblyLoader).FullName);
            data.Loader.LoadAssembly(assemblyFile);            
        }

        public List<string> GetAssemblyEventList(string assemblyFile)
        {
            List<string> result = new List<string>();

            _targetDomainName = Path.GetFileName(assemblyFile);
            DomainData data = _loadedDomains.Find(new Predicate<DomainData>(AppDomainMatch));
            if (data != null)
            {
                result.AddRange(data.Loader.ListLoadedEvents(EventType.DetailGridEnter, true));
            }

            return result;
        }

        public List<string> GetAssemblyEditList(string assemblyFile)
        {
            List<string> result = new List<string>();

            _targetDomainName = Path.GetFileName(assemblyFile);
            DomainData data = _loadedDomains.Find(new Predicate<DomainData>(AppDomainMatch));
            if (data != null)
            {
                result.AddRange(data.Loader.ListLoadedEdits());
            }

            return result;
        }

        public List<string> GetAssemblyDeploymentList()
        {
            List<string> result = new List<string>();

            foreach (DomainData data in _loadedDomains)
            {
                result.AddRange(data.Loader.ListReferencedAssemblies());
            }

            return result;
        }

        public void Unload()
        {
            while(_loadedDomains.Count > 0)
                AppDomain.Unload(_loadedDomains[0].Domain);
        }

        public void Unload(string assemblyFile)
        {
            _targetDomainName = Path.GetFileName(assemblyFile);
            DomainData data = _loadedDomains.Find(new Predicate<DomainData>(AppDomainMatch));
            if (data != null)
            {
                data.Loader.Unload();
                AppDomain.Unload(data.Domain);
            }
        }

        public IScriptEdit GetEdit(string editName)
        {
            foreach (DomainData data in _loadedDomains)
            {
                IScriptEdit result = data.Loader.GetEdit(editName);
                if (result != null)
                    return result;
            }

            return null;
        }

        #endregion

        #region Private Members

        private AppDomain CreateNewDomain(string domainName)
        {
            AppDomainSetup info = new AppDomainSetup();
            info.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            AppDomain newDomain = AppDomain.CreateDomain(domainName, AppDomain.CurrentDomain.Evidence, info);
            newDomain.DomainUnload += new EventHandler(newDomain_DomainUnload);
            DomainData data = new DomainData();
            data.Domain = newDomain;
            _loadedDomains.Add(data);
            return newDomain;
        }

        private void newDomain_DomainUnload(object sender, EventArgs e)
        {
            AppDomain unloading = (AppDomain)sender;
            _targetDomainName = unloading.FriendlyName;
            DomainData data = _loadedDomains.Find(new Predicate<DomainData>(AppDomainMatch));
            if (data != null)
                _loadedDomains.Remove(data);
        }

        private bool AppDomainMatch(DomainData target)
        {
            if (string.Compare(target.Domain.FriendlyName, _targetDomainName, true) == 0)
                return true;
            else
                return false;
        }

        #endregion
    }

    internal class DomainData
    {
        public DomainData() { }
        public AppDomain Domain;
        public AssemblyLoader Loader;
    }

    internal class ScriptCompiler : MarshalByRefObject
    {
        public ScriptCompiler()
        {
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public CompilerResults CompileScript(CodeDomProvider provider, string script, string[] referencedAssemblies, string outputFileName, bool includeDebugInformation)
        {
            CompilerParameters options = new CompilerParameters(referencedAssemblies, outputFileName, includeDebugInformation);
			options.WarningLevel = 3;  // to show the warning message always (if no errors);

            if (includeDebugInformation)
            {
                string sourceFile = Path.ChangeExtension(outputFileName, provider.FileExtension);
                using (StreamWriter sw = new StreamWriter(sourceFile))
                {
                    sw.Write(script);
                    sw.Flush();
                }

                return provider.CompileAssemblyFromFile(options, new string[] { sourceFile });
            }
            else
                return provider.CompileAssemblyFromSource(options, new string[] { script });
        }
    }

    internal class AssemblyLoader : MarshalByRefObject
    {
        public AssemblyLoader() { }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void LoadAssembly(string assemblyFile)
        {            
            ScriptLoader.LoadAssembly(assemblyFile);
        }

        public List<string> ListLoadedEvents(EventType targetType)
        {
            return ListLoadedEvents(targetType, false);
        }

        public List<string> ListLoadedEvents(EventType targetType, bool ignoreTarget)
        {
            List<string> result = new List<string>();

            foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (loadedAssembly.GlobalAssemblyCache)
                    continue;

                foreach (Type t in loadedAssembly.GetExportedTypes())
                {
                    if ((t.IsClass) && (Attribute.GetCustomAttribute(t, typeof(EventContainerAttribute)) != null))
                    {
                        if (t.GetConstructor(new Type[] { }) != null)
                        {
                            foreach (MethodInfo info in t.GetMethods())
                            {
                                EventAttribute attr = (EventAttribute)Attribute.GetCustomAttribute(info, typeof(EventAttribute));
                                if ((attr != null) && (ignoreTarget || (attr.EventType == targetType)))
                                {
                                    result.Add(t.FullName + "." + info.Name);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public List<string> ListLoadedEdits()
        {
            List<string> result = new List<string>();

            foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (loadedAssembly.GlobalAssemblyCache)
                    continue;

                foreach (Type t in loadedAssembly.GetExportedTypes())
                {
                    if (t.GetConstructor(new Type[] { }) != null)
                    {
                        if (t.GetInterface(typeof(IScriptEdit).Name) != null)
                        {
                            result.Add(t.FullName);
                        }
                    }
                }
            }

            return result;
        }

        public List<string> ListReferencedAssemblies()
        {
            List<string> result = new List<string>();

            string targetAssembly = AppDomain.CurrentDomain.FriendlyName;
            if (targetAssembly.ToLower().EndsWith(".dll"))
                targetAssembly = targetAssembly.Remove(targetAssembly.ToLower().IndexOf(".dll"));

            foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (loadedAssembly.GlobalAssemblyCache)
                    continue;

                if (loadedAssembly.GetName().Name == targetAssembly)
                {
                    result.Add(targetAssembly + ".dll");

                    foreach (AssemblyName name in loadedAssembly.GetReferencedAssemblies())
                    {
                        if(!((name.Name.StartsWith("System")) || 
                            (name.Name.StartsWith("mscorlib")) || 
                            (name.Name.StartsWith("Microsoft")) ||
                            (name.Name.StartsWith("WebDX.Api"))))
                            result.Add(name.Name);
                    }
                }
            }

            return result;
        }

        public IScriptEdit GetEdit(string editName)
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = asm.GetType(editName);
                if (t != null)
                {
                    if (t.GetInterface(typeof(IScriptEdit).FullName, true) == null)
						throw new Exception("Type does not implement IScriptEdit: " + editName);

                    ConstructorInfo info = t.GetConstructor(new Type[] { });
                    if (info == null)
						throw new Exception("Type does not have a default constructor: " + editName);

                    IScriptEdit edit = (IScriptEdit)info.Invoke(null);
                    return edit;
                }
            }

            return null;
        }

        public void Unload()
        {
        }
    }
}
