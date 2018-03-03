using System;
using System.Data;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace WebDX.Api.Scripts
{
    internal enum EventNames
    {
        OnEnter,
        OnKeyPress,
        OnExit,
        OnValidate,
        OnDocumentRejected,
        OnDocumentUnRejected,
        OnStartWork,
        OnEndWork,
        OnPageChange
    }

	[Serializable] 
    public class ScriptLoader
    {
        private string _basePath;
        private AppDomain _scriptDomain;
        private ScriptEventMapper _mapper;

        /// <summary>
        /// Creates a new instance of the ScriptLoader class.
        /// </summary>
        /// <param name="basePath">The path or URL to the project directory.</param>
        public ScriptLoader(string basePath)
        {
            if (basePath == null)
                throw new ArgumentNullException("basePath");
            if (basePath == "")
                throw new ArgumentException("basePath cannot be blank.",  "basePath");

            _basePath = basePath;

            try
            {
                SecurityPermission perm = new SecurityPermission(SecurityPermissionFlag.ControlAppDomain);
                perm.Demand();

                CreateScriptDomain();
            }
            catch (SecurityException)
            {
                _scriptDomain = AppDomain.CurrentDomain;
                _mapper = new ScriptEventMapper();                
            }
        }

        /// <summary>
        /// Gets a value indicating whether the script engine is using a separate domain or the default AppDomain.
        /// </summary>
        public bool SeparateDomain
        {
            get { return _scriptDomain != AppDomain.CurrentDomain; }
        }

        /// <summary>
        /// Unloads the loaded scripts. Does nothing if SeparateDomain is false.
        /// </summary>
        public void Unload()
        {
            DoUnload(true);
        }

        /// <summary>
        /// Loads all assemblies in the given assemblyList.
        /// </summary>
        /// <param name="assemblyList">The list of assemblies to load.</param>
        public void LoadProjectAssemblies(List<string> assemblyList)
        {
            if (assemblyList == null)
                throw new ArgumentNullException("assemblyList");

			try
			{
				foreach( string assemblyFile in assemblyList )
				{
					string filePath = BuildAssemblyPath( assemblyFile );
					_mapper.LoadAssembly( filePath );           
				}
			}
			catch( Exception ex )
			{
				throw new Exception( ex.Message );
 
			}

			_scriptDomain.AssemblyResolve += new ResolveEventHandler( AssemblyResolveEventHandler );			
        }

		/// <summary>
		/// Loads failed assembly
		/// </summary>
		/// <param name="sender">Object</param>
		/// <param name="args">ResolveEventArgs</param>
		/// <returns>The loaded assembly reference.</returns>
        /// <remarks>USE THE <see cref="AssemblyName"/> CLASS TO PARSE args.Name PER MICROSOFT RECOMMENDATION.</remarks>
		private Assembly AssemblyResolveEventHandler( object sender, ResolveEventArgs args )
		{
			try
			{
                //do not manually parse args.Name.
                AssemblyName aName = new AssemblyName(args.Name);
                string assemblyName = Path.ChangeExtension(aName.Name, ".dll");				
				return LoadAssembly(BuildAssemblyPath(assemblyName));				
			}
			catch
			{
				return null; 
			}
		}

        /// <summary>
        /// Gets the plugin interface for the given session. Returns null if there is no plugin defined.
        /// </summary>
        /// <param name="sessionDef">The session def to load a plugin for.</param>
        /// <returns>The plugin object or null if none is defined.</returns>
        public ISessionPlugin GetSessionPlugin(IWDESessionDef_R2 sessionDef)
        {
            if (sessionDef == null)
                throw new ArgumentNullException("sessionDef");

            if (!string.IsNullOrEmpty(sessionDef.PluginName))
            {
                Assembly sessionAssembly = LoadAssembly(BuildAssemblyPath(sessionDef.PluginName));
                foreach (Type t in sessionAssembly.GetTypes())
                {
                    if (t.GetInterface(typeof(ISessionPlugin).Name) != null)
                    {
                        ConstructorInfo ci = t.GetConstructor(new Type[] { });
                        if (ci == null)
							throw new Exception("Type does not have a default constructor: " + t.FullName);
                        return (ISessionPlugin)ci.Invoke(null);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the list of client plugins. The list will be empty if no plugins are defined.
        /// </summary>
        /// <param name="pluginNames">The list of plugins to load.</param>
        /// <returns>The list of plugin objects. This list will be empty if no plugins are defined in pluginNames.</returns>
        public static List<IClientPlugin> GetClientPlugins(List<string> pluginNames)
        {
            if (pluginNames == null)
                throw new ArgumentNullException("pluginNames");

            List<IClientPlugin> result = new List<IClientPlugin>();

            foreach (string pluginName in pluginNames)
            {
                Assembly pluginAssembly = LoadAssembly(pluginName);
                foreach (Type t in pluginAssembly.GetTypes())
                {
                    if (t.GetInterface(typeof(IClientPlugin).Name) != null)
                    {
                        ConstructorInfo ci = t.GetConstructor(new Type[] { });
                        if (ci == null)
							throw new Exception("Type does not have a default constructor: " + t.FullName);
                        result.Add((IClientPlugin)ci.Invoke(null));
                    }
                }
            }

            return result;
        }

        public void LinkEvents(IScriptTextBoxEvents link, IWDETextBoxDef_R1 textBoxDef)
        {
            try
            {
                if (string.IsNullOrEmpty(textBoxDef.OnEnter.ScriptFullName) &&
                    string.IsNullOrEmpty(textBoxDef.OnExit.ScriptFullName) &&
                    string.IsNullOrEmpty(textBoxDef.OnKeyPress.ScriptFullName) &&
                    string.IsNullOrEmpty(textBoxDef.OnValidate.ScriptFullName))
                    return;
                else
                {
                    Dictionary<string, WDEEventInfo> def = new Dictionary<string, WDEEventInfo>();
                    def.Add(EventNames.OnEnter.ToString(), new WDEEventInfo(textBoxDef.OnEnter));
                    def.Add(EventNames.OnExit.ToString(), new WDEEventInfo(textBoxDef.OnExit));
                    def.Add(EventNames.OnKeyPress.ToString(), new WDEEventInfo(textBoxDef.OnKeyPress));
                    def.Add(EventNames.OnValidate.ToString(), new WDEEventInfo(textBoxDef.OnValidate));
                    new TextBoxEventsProxy(_mapper, link, def);
                }
            }
            catch (Exception ex)
            {
				throw new BindingException(string.Format("An error occurred linking events for text box \"{0}\": {1}", textBoxDef.ControlName, ex.Message), ex);
            }
        }

        public void LinkEvents(IScriptDetailGridEvents link, IWDEDetailGridDef detailGridDef)
        {
            try
            {
                if (string.IsNullOrEmpty(detailGridDef.OnEnter.ScriptFullName) &&
                    string.IsNullOrEmpty(detailGridDef.OnExit.ScriptFullName))
                    return;
                else
                {
                    Dictionary<string, WDEEventInfo> def = new Dictionary<string, WDEEventInfo>();
                    def.Add(EventNames.OnEnter.ToString(), new WDEEventInfo(detailGridDef.OnEnter));
                    def.Add(EventNames.OnExit.ToString(), new WDEEventInfo(detailGridDef.OnExit));
                    new DetailGridEventsProxy(_mapper, link, def);
                }
            }
            catch (Exception ex)
            {
				throw new BindingException(string.Format("An error occurred linking events for detail grid \"{0}\": {1}", detailGridDef.ControlName, ex.Message), ex);
            }
        }

        public void LinkEvents(IScriptFormEvents link, IWDEFormDef_R1 formDef)
        {
            try
            {
                if (string.IsNullOrEmpty(formDef.OnEnter.ScriptFullName) &&
                    string.IsNullOrEmpty(formDef.OnExit.ScriptFullName))
                    return;
                else
                {
                    Dictionary<string, WDEEventInfo> def = new Dictionary<string, WDEEventInfo>();
                    def.Add(EventNames.OnEnter.ToString(), new WDEEventInfo(formDef.OnEnter));
                    def.Add(EventNames.OnExit.ToString(), new WDEEventInfo(formDef.OnExit));
                    new FormEventsProxy(_mapper, link, def);
                }
            }
            catch (Exception ex)
            {
				throw new BindingException(string.Format("An error occurred linking events for form \"{0}\": {1}", formDef.FormName, ex.Message), ex);
            }
        }

        public void LinkEvents(IScriptProjectEvents link, IWDEProject_R1 project)
        {
            try
            {
                if (string.IsNullOrEmpty(project.OnDocumentRejected.ScriptFullName) &&
                    string.IsNullOrEmpty(project.OnDocumentUnRejected.ScriptFullName) &&
                    string.IsNullOrEmpty(project.OnEndWork.ScriptFullName) &&
                    string.IsNullOrEmpty(project.OnKeyPress.ScriptFullName) &&
                    string.IsNullOrEmpty(project.OnPageChange.ScriptFullName) &&
                    string.IsNullOrEmpty(project.OnStartWork.ScriptFullName))
                    return;
                else
                {
                    Dictionary<string, WDEEventInfo> def = new Dictionary<string, WDEEventInfo>();
                    def.Add(EventNames.OnDocumentRejected.ToString(), new WDEEventInfo(project.OnDocumentRejected));
                    def.Add(EventNames.OnDocumentUnRejected.ToString(), new WDEEventInfo(project.OnDocumentUnRejected));
                    def.Add(EventNames.OnEndWork.ToString(), new WDEEventInfo(project.OnEndWork));
                    def.Add(EventNames.OnKeyPress.ToString(), new WDEEventInfo(project.OnKeyPress));
                    def.Add(EventNames.OnStartWork.ToString(), new WDEEventInfo(project.OnStartWork));
                    def.Add(EventNames.OnPageChange.ToString(), new WDEEventInfo(project.OnPageChange));
                    new ProjectEventsProxy(_mapper, link, def);
                }
            }
            catch (Exception ex)
            {
				throw new BindingException(string.Format("An error occurred linking events for the project: {0}", ex.Message), ex);
            }
        }

        public void LinkEvents(IScriptImageEvents link, IWDEProject_R1 project)
        {
            try
            {
                Dictionary<string, WDEEventInfo> def = new Dictionary<string, WDEEventInfo>();
                def.Add(EventNames.OnPageChange.ToString(), new WDEEventInfo(project.OnPageChange));
                new ImageEventsProxy(_mapper, link, def);
            }
            catch (Exception ex)
            {
				throw new BindingException(string.Format("An error occurred linking image events for the project: {0}", ex.Message), ex);
            }
        }

        public void RunEdits(IWDETextBoxDef_R1 textBox)
        {
            try
            {
                List<IWDEEditDef_R1> list = new List<IWDEEditDef_R1>();
                
                foreach (IWDEEditDef_R1 def in textBox.EditDefs)
                {
                    if (def.EditParams.Trim().Length > 0)
                    {
                        list.Add(new WDEEditInfo(def));
                    }
                }
                _mapper.RunEdits(list);
            }
            catch (BindingException ex)
            {
				throw new BindingException(string.Format("Error loading edits for text box \"{0}\": {1}", textBox.ControlName, ex.Message), ex);
            }
        }

        #region Private Members

        public static Assembly LoadAssembly(string assemblyFile)
        {
            try
            {
                if (AppDomain.CurrentDomain.FriendlyName == "WebDX.ScriptDomain" && Environment.Version.Major < 4)
                {
                    object[] fullEv = { new Zone(SecurityZone.MyComputer) };
                    Evidence ev = new Evidence(fullEv, fullEv);                    
                    return Assembly.LoadFrom(assemblyFile, ev);
                }
                else
                    return Assembly.LoadFrom(assemblyFile);
            }
            catch
            {
                return Assembly.LoadFrom(assemblyFile);
            }
        }

        private string BuildAssemblyPath(string assemblyPath)
        {
            Uri path = new Uri(_basePath);
            if (path.Scheme == Uri.UriSchemeFile)
            {
                return Path.Combine(_basePath, assemblyPath);
            }
            else
            {
                if (_basePath.EndsWith("/"))
                    return _basePath + assemblyPath;
                else
                    return _basePath + "/" + assemblyPath;
            }
        }

        private void CreateScriptDomain()
        {
            AppDomainSetup info = new AppDomainSetup();
            info.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            object[] fullEv = { new Zone(SecurityZone.MyComputer) };
            Evidence ev = new Evidence(fullEv, fullEv);

            _scriptDomain = AppDomain.CreateDomain("WebDX.ScriptDomain", ev, info);
            _scriptDomain.Load(Assembly.GetExecutingAssembly().FullName);
            _mapper = (ScriptEventMapper)_scriptDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ScriptEventMapper).FullName);            
            Host.Mapper = _mapper;
        }

        private void DoUnload(bool reloadScriptDomain)
        {
            if (_mapper != null)
                _mapper.Unload();

            if ((SeparateDomain) && (_scriptDomain != null))
            {
                _mapper = null;
                Host.Mapper = null;
                AppDomain current = _scriptDomain;
                _scriptDomain = null;

                if (reloadScriptDomain)
                    CreateScriptDomain();

                //Unload the old domain in the background to save time on the calling thread.
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(delegate(object state)
                {
                    AppDomain.Unload(current);
                }));
            }
        }

        #endregion
    }

    [Serializable]
    public class WDEEventInfo : IWDEEventScriptDef
    {
        private string _description;
        private bool _enabled;
        private string _scriptFullName;

        public WDEEventInfo(IWDEEventScriptDef def)
        {
            _description = def.Description;
            _enabled = def.Enabled;
            _scriptFullName = def.ScriptFullName;
        }

        #region IWDEEventScriptDef Members

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return !string.IsNullOrEmpty(_scriptFullName);
            }
            set
            {
                //_enabled = value;
            }
        }

        public string ScriptFullName
        {
            get
            {
                return _scriptFullName;
            }
            set
            {
                _scriptFullName = value;
            }
        }

        public IWDEEventScriptDef Clone()
        {
            return (IWDEEventScriptDef)this.MemberwiseClone();
        }

        #endregion
    }

    [Serializable]
    public class WDEEditInfo : IWDEEditDef_R1
    {
        private string _displayName;
        private string _fullName;
        private string _editParams;
        private string _description;
        private bool _enabled;
        private string _errorMessage;
        private WDEEditErrorType _errorType;
        private WDESessionType _sessionMode;

        public WDEEditInfo(IWDEEditDef_R1 def)
        {
            _description = def.Description;
            _displayName = def.DisplayName;
            _editParams = def.EditParams;
            _enabled = def.Enabled;
            _errorMessage = def.ErrorMessage;
            _errorType = def.ErrorType;
            _fullName = def.FullName;
            _sessionMode = def.SessionMode;
        }

        #region IWDEEditDef_R1 Members

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        public string FullName
        {
            get
            {
                return _fullName;
            }
            set
            {
                _fullName = value;
            }
        }

        public string EditParams
        {
            get
            {
                return _editParams;
            }
            set
            {
                _editParams = value;
            }
        }

        #endregion

        #region IWDEEditDef Members


        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        public WDEEditErrorType ErrorType
        {
            get
            {
                return _errorType;
            }
            set
            {
                _errorType = value;
            }
        }

        public WDESessionType SessionMode
        {
            get
            {
                return _sessionMode;
            }
            set
            {
                _sessionMode = value;
            }
        }

        #endregion
    }

    public class ScriptEventMapper : MarshalByRefObject
    {
        private Dictionary<string, object> _createdTypes;
        
        public ScriptEventMapper()
        {
            _createdTypes = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        }

        public override object InitializeLifetimeService()
        {
            return null; // this object persists as long as the AppDomain is active.
        }

        public void RunEvent(string typeFullName, params object[] args)
        {
            MethodSplitter ms = new MethodSplitter(typeFullName);

            object scriptObject = GetScriptObject(ms);

            MethodInfo methodInfo = scriptObject.GetType().GetMethod(ms.MethodName);
            if (methodInfo == null)
                throw new BindingException("Method name not found: " + ms.TypeFullName); //DO NOT LOCALIZE!!!

            try
            {
                methodInfo.Invoke(scriptObject, args);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw PreserveStackTrace(ex.InnerException);
            }
            catch (TargetParameterCountException)
            {
                throw new BindingException("Invalid method signature: " + ms.TypeFullName); //DO NOT LOCALIZE!!!
            }
        }

        public void RunEdits(List<IWDEEditDef_R1> list)
        {
            foreach (IWDEEditDef_R1 def in list)
            {
                IScriptEdit edit = LoadEdit(def);
                RunEdit(def, edit);
            }
        }

        public void LoadAssembly(string assemblyFile)
        {            
            ScriptLoader.LoadAssembly(assemblyFile);
        }

        public void Unload()
        {
            _createdTypes.Clear();
        }


        #region Host object sync methods

        public void SetHostObject(IScriptDatabase database)
        {
            Host.Database = database;
        }

        public void SetHostObject(IScriptDocument document)
        {
            Host.Document = document;
        }

        public void SetHostObject(IScriptField field)
        {
            Host.Field = field;
        }

        public void SetHostObject(IScriptForm form)
        {
            Host.Form = form;
        }

        public void SetHostObject(IScriptImage image)
        {
            Host.Image = image;
        }

        public void SetHostObject(IScriptLookupDialog dialog)
        {
            Host.LookupDialog = dialog;
        }

        public void SetHostObject(IScriptRecord record)
        {
            Host.Record = record;
        }

        public void SetHostObject(IScriptTextBox textBox)
        {
            Host.TextBox = textBox;
        }

        public void SetHostObject(IScriptWork work)
        {
            Host.Work = work;
        }

        public void SetHostObject(IScriptDetailGrid detailGrid)
        {
            Host.DetailGrid = detailGrid;
        }

        #endregion

        #region Private Methods

        private Exception PreserveStackTrace(Exception e)
        {
            StreamingContext ctx = new StreamingContext(StreamingContextStates.CrossAppDomain);
            ObjectManager mgr = new ObjectManager(null, ctx);
            SerializationInfo si = new SerializationInfo(e.GetType(), new FormatterConverter());

            e.GetObjectData(si, ctx);
            mgr.RegisterObject(e, 1, si); // prepare for SetObjectData
            mgr.DoFixups(); // ObjectManager calls SetObjectData

            return e;
        }

        private object GetScriptObject(MethodSplitter ms)
        {
            object result = null;
            if (_createdTypes.TryGetValue(ms.TypeFullName, out result))
                return result;
            else
            {
                foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type t = loadedAssembly.GetType(ms.TypeFullName, false, true);
                    if (t != null)
                    {
                        if(Attribute.GetCustomAttribute(t, typeof(EventContainerAttribute)) == null)
                            throw new BindingException("Type: " + ms.TypeFullName + " is not marked as an event container.");
                            
                        result = Activator.CreateInstance(t);
                        _createdTypes.Add(ms.TypeFullName, result);
                        return result;
                    }
                }
            }

			throw new BindingException("Type: " + ms.ToString() + " not found.");
        }

        private IScriptEdit LoadEdit(IWDEEditDef_R1 def)
        {
            IScriptEdit edit = FindEdit(def);

            edit.ReadFromXml(def.EditParams);

            return edit;
        }

        private IScriptEdit FindEdit(IWDEEditDef_R1 def)
        {
            foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type loadedType in loadedAssembly.GetExportedTypes())
                    if (loadedType.IsClass && (string.Compare(loadedType.FullName, def.FullName, true) == 0))
                    {
                        if (loadedType.GetInterface(typeof(IScriptEdit).Name) != null)
                        {
                            ConstructorInfo ci = loadedType.GetConstructor(new Type[] { });
                            if (ci == null)
								throw new BindingException("Type does not have a default constructor: " + loadedType.FullName);
                            else
                            {
                                IScriptEdit edit = (IScriptEdit)ci.Invoke(null);                                
                                return edit;
                            }
                        }
                        else
							throw new BindingException("Type does not implement IScriptEdit: " + loadedType.FullName);
                    }

			throw new BindingException("Type not found: " + def.FullName);
        }

        private void RunEdit(IWDEEditDef_R1 def, IScriptEdit edit)
        {
            try
            {
                edit.Execute(def.ErrorMessage);
            }
            catch (ScriptException ex)
            {
                switch (def.ErrorType)
                {
                    case WDEEditErrorType.Failure:
                        if (string.IsNullOrEmpty(def.ErrorMessage))
                            throw;
                        else
                            throw new ScriptException(def.ErrorMessage);
                    case WDEEditErrorType.Ignore:
                        return;
                    case WDEEditErrorType.Warning:
                        if (string.IsNullOrEmpty(def.ErrorMessage))
                            throw new ScriptWarning(def.ErrorMessage, false);
                        else
                            throw new ScriptWarning(ex.Message, false);
                    case WDEEditErrorType.WarningWithRetry:
                        if (string.IsNullOrEmpty(def.ErrorMessage))
                            throw new ScriptWarning(def.ErrorMessage, true);
                        else
                            throw new ScriptWarning(ex.Message, true);
                }
            }
        }

        #endregion
    }

    #region Event Proxies

    internal class TextBoxEventsProxy
    {
        ScriptEventMapper _mapper;
        Dictionary<string, WDEEventInfo> _eventIndex;

        public TextBoxEventsProxy(ScriptEventMapper mapper, IScriptTextBoxEvents eventSource, Dictionary<string, WDEEventInfo> eventIndex)
        {
            _mapper = mapper;
            _eventIndex = eventIndex;
            eventSource.OnEnter += new ScriptBaseEvent(EnterHandler);
            eventSource.OnExit += new ScriptTextBoxExitEvent(ExitHandler);
            eventSource.OnKeyPress += new ScriptTextBoxKeyPressEvent(KeyPressHandler);
            eventSource.OnValidate += new ScriptBaseEvent(ValidateHandler);
        }

        public void EnterHandler()
        {
            WDEEventInfo info = _eventIndex[EventNames.OnEnter.ToString()];

            if(!string.IsNullOrEmpty(info.ScriptFullName))
                _mapper.RunEvent(info.ScriptFullName);
        }

        public void KeyPressHandler(TextBoxKeyPressEventArgs e)
        {
            WDEEventInfo info = _eventIndex[EventNames.OnKeyPress.ToString()];

            if(!string.IsNullOrEmpty(info.ScriptFullName))
                _mapper.RunEvent(info.ScriptFullName, e);
        }

        public void ValidateHandler()
        {
            WDEEventInfo info = _eventIndex[EventNames.OnValidate.ToString()];

            if (!string.IsNullOrEmpty(info.ScriptFullName))
                _mapper.RunEvent(info.ScriptFullName);
        }

        public void ExitHandler(ExitEventArgs e)
        {
            WDEEventInfo info = _eventIndex[EventNames.OnExit.ToString()];

            if (!string.IsNullOrEmpty(info.ScriptFullName))
                _mapper.RunEvent(info.ScriptFullName, e);
        }
    }

    internal class DetailGridEventsProxy
    {
        ScriptEventMapper _mapper;
        Dictionary<string, WDEEventInfo> _eventIndex;

        public DetailGridEventsProxy(ScriptEventMapper mapper, IScriptDetailGridEvents eventSource, Dictionary<string, WDEEventInfo> eventIndex)
        {
            _mapper = mapper;
            _eventIndex = eventIndex;

            eventSource.OnEnter += new ScriptBaseEvent(eventSource_OnEnter);
            eventSource.OnExit += new ScriptBaseEvent(eventSource_OnExit);
        }

        void eventSource_OnExit()
        {
            WDEEventInfo info = _eventIndex[EventNames.OnExit.ToString()];

            if (!string.IsNullOrEmpty(info.ScriptFullName))
                _mapper.RunEvent(info.ScriptFullName);
        }

        void eventSource_OnEnter()
        {
            WDEEventInfo info = _eventIndex[EventNames.OnEnter.ToString()];

            if (!string.IsNullOrEmpty(info.ScriptFullName))
                _mapper.RunEvent(info.ScriptFullName);
        }
    }

    internal class FormEventsProxy
    {
        ScriptEventMapper _mapper;
        Dictionary<string, WDEEventInfo> _eventIndex;

        public FormEventsProxy(ScriptEventMapper mapper, IScriptFormEvents eventSource, Dictionary<string, WDEEventInfo> eventIndex)
        {
            _mapper = mapper;
            _eventIndex = eventIndex;

            eventSource.OnEnter += new ScriptBaseEvent(eventSource_OnEnter);
            eventSource.OnExit += new ScriptBaseEvent(eventSource_OnExit);
        }

        void eventSource_OnExit()
        {
            WDEEventInfo info = _eventIndex[EventNames.OnExit.ToString()];

            if (!string.IsNullOrEmpty(info.ScriptFullName))
                _mapper.RunEvent(info.ScriptFullName);
        }

        void eventSource_OnEnter()
        {
            WDEEventInfo info = _eventIndex[EventNames.OnEnter.ToString()];

            if (!string.IsNullOrEmpty(info.ScriptFullName))
                _mapper.RunEvent(info.ScriptFullName);
        }
    }

    internal class ProjectEventsProxy
    {
        ScriptEventMapper _mapper;
        Dictionary<string, WDEEventInfo> _eventIndex;

        public ProjectEventsProxy(ScriptEventMapper mapper, IScriptProjectEvents eventSource, Dictionary<string, WDEEventInfo> eventIndex)
        {
            _mapper = mapper;
            _eventIndex = eventIndex;

            eventSource.OnDocumentReject += new ScriptDocumentRejectEvent(eventSource_OnDocumentReject);
            eventSource.OnDocumentUnreject += new ScriptBaseEvent(eventSource_OnDocumentUnreject);
            eventSource.OnEndWork += new ScriptEndWorkEvent(eventSource_OnEndWork);
            eventSource.OnKeyPress += new ScriptProjectKeyPressEvent(eventSource_OnKeyPress);
            eventSource.OnStartWork += new ScriptStartWorkEvent(eventSource_OnStartWork);
        }

        void eventSource_OnStartWork(StartWorkEventArgs e)
        {
            WDEEventInfo info = _eventIndex[EventNames.OnStartWork.ToString()];

            if (!string.IsNullOrEmpty(info.ScriptFullName))
                _mapper.RunEvent(info.ScriptFullName, e);
        }

        void eventSource_OnKeyPress(ProjectKeyEventArgs e)
        {
            WDEEventInfo info = _eventIndex[EventNames.OnKeyPress.ToString()];

            if (!string.IsNullOrEmpty(info.ScriptFullName))
                _mapper.RunEvent(info.ScriptFullName, e);
        }

        void eventSource_OnEndWork(EndWorkEventArgs e)
        {
            WDEEventInfo info = _eventIndex[EventNames.OnEndWork.ToString()];

            if (!string.IsNullOrEmpty(info.ScriptFullName))
                _mapper.RunEvent(info.ScriptFullName, e);
        }

        void eventSource_OnDocumentUnreject()
        {
            WDEEventInfo info = _eventIndex[EventNames.OnDocumentUnRejected.ToString()];

            if (!string.IsNullOrEmpty(info.ScriptFullName))
                _mapper.RunEvent(info.ScriptFullName);
        }

        void eventSource_OnDocumentReject(RejectEventArgs e)
        {
            WDEEventInfo info = _eventIndex[EventNames.OnDocumentRejected.ToString()];

            if (!string.IsNullOrEmpty(info.ScriptFullName))
                _mapper.RunEvent(info.ScriptFullName, e);
        }
    }

    internal class ImageEventsProxy
    {
        ScriptEventMapper _mapper;
        Dictionary<string, WDEEventInfo> _eventIndex;

        public ImageEventsProxy(ScriptEventMapper mapper, IScriptImageEvents eventSource, Dictionary<string, WDEEventInfo> eventIndex)
        {
            _mapper = mapper;
            _eventIndex = eventIndex;

            eventSource.OnPageChange += new ScriptBaseEvent(eventSource_OnPageChange);
        }

        void eventSource_OnPageChange()
        {
            WDEEventInfo info = _eventIndex[EventNames.OnPageChange.ToString()];

            if (!string.IsNullOrEmpty(info.ScriptFullName))
                _mapper.RunEvent(info.ScriptFullName);
        }
    }

    #endregion
    
    internal class MethodSplitter
    {
        private string _namespace;
        private string _typeName;
        private string _methodName;

        public MethodSplitter(string typeFullName)
        {
            if (typeFullName == null)
                throw new ArgumentNullException("typeFullName");

            string[] parts = typeFullName.Split('.');
            if (parts.Length > 2)
            {
                _methodName = parts[parts.Length - 1];
                _typeName = parts[parts.Length - 2];
                _namespace = string.Join(".", parts, 0, parts.Length - 2);
            }
            else
				throw new Exception("Invalid type name: " + typeFullName);
        }

        public string Namespace
        {
            get { return _namespace; }
        }

        public string TypeName
        {
            get { return _typeName; }
        }

        public string TypeFullName
        {
            get { return _namespace + "." + _typeName; }
        }

        public string MethodName
        {
            get { return _methodName; }
        }

        public override string ToString()
        {
            return _namespace + "." + _typeName + "." + _methodName;
        }
    }

    [Serializable]
    public class BindingException : Exception
    {
        public BindingException(string message) : base(message) { }
        public BindingException(string message, Exception innerException) : base(message, innerException) { }
        public BindingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
