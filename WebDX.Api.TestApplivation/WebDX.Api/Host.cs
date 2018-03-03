using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;

namespace WebDX.Api.Scripts
{
    /// <summary>
    /// Main host interface for scripts.
    /// </summary>
    public static class Host
    {
        //Note: The interface references are used in the Script AppDomain copy of Host,
        //      The Proxy objects are used in the Main AppDomain copy of Host.
        private static IScriptImage _image;
        private static ScriptImageProxy _imageProxy;
        private static IScriptDatabase _database;
        private static ScriptDatabaseProxy _databaseProxy;
        private static IScriptLookupDialog _lookupDialog;
        private static ScriptLookupDialogProxy _lookupDialogProxy;
        private static IScriptWork _work;
        private static ScriptWorkProxy _workProxy;
        private static IScriptDocument _document;
        private static ScriptDocumentProxy _documentProxy;
        private static IScriptForm _form;
        private static ScriptFormProxy _formProxy;
        private static IScriptTextBox _textBox;
        private static ScriptTextBoxProxy _textBoxProxy;
        private static IScriptField _field;
        private static ScriptFieldProxy _fieldProxy;
        private static IScriptRecord _record;
        private static ScriptRecordProxy _recordProxy;
        private static ScriptEventMapper _mapper;
        private static IScriptDetailGrid _detailGrid;
        private static ScriptDetailGridProxy _detailGridProxy;
        private static Dictionary<IScriptRecord, ScriptRecordProxy> _records;
        private static Dictionary<IScriptField, ScriptFieldProxy> _fields;
        private static Dictionary<IScriptRevision, ScriptRevisionProxy> _revisions;
        private static IScriptClient _client;

        #region IScriptWebDX Members

        /// <summary>
        /// Provides access to image related properties and methods.
        /// </summary>
        public static IScriptImage Image
        {
            get
            {
                if (_image == null)
                    throw new WDEException("API80001", new object[] { "Image" });
                return _image;
            }

            set
            {
                if (_mapper != null)
                {
                    if (_imageProxy == null)
                    {
                        _imageProxy = new ScriptImageProxy();
                        _mapper.SetHostObject(_imageProxy); // Set the Script AppDomain Reference and create Remoting proxies.
                    }

                    _imageProxy.RealObject = value;
                    _image = value;
                }
                else
                {
                    _image = value;
                }
            }
        }

        /// <summary>
        /// Provides access to database related properties and methods.
        /// </summary>
        public static IScriptDatabase Database
        {
            get
            {
                if (_database == null)
                        throw new WDEException("API80001", new object[] { "Database" });                    
                return _database;
            }

            set
            {
                if (_mapper != null)
                {
                    if (_databaseProxy == null)
                    {
                        _databaseProxy = new ScriptDatabaseProxy();
                        _mapper.SetHostObject(_databaseProxy); // Set the Script AppDomain Reference and create Remoting proxies.
                    }

                    _databaseProxy.RealObject = value;
                    _database = value;
                }
                else
                {
                    _database = value;
                }
            }
        }

        /// <summary>
        /// Provides access to the standard lookup dialog.
        /// </summary>
        public static IScriptLookupDialog LookupDialog
        {
            get
            {
                if (_lookupDialog == null)
                    throw new WDEException("API80001", new object[] { "LookupDialog" });

                return _lookupDialog;
            }

            set
            {
                if (_mapper != null)
                {
                    if (_lookupDialogProxy == null)
                    {
                        _lookupDialogProxy = new ScriptLookupDialogProxy();
                        _mapper.SetHostObject(_lookupDialogProxy); // Set the Script AppDomain Reference and create Remoting proxies.
                    }

                    _lookupDialogProxy.RealObject = value;
                    _lookupDialog = value;
                }
                else
                {
                    _lookupDialog = value;
                }
            }
        }

        /// <summary>
        /// Provides access to detail grid related functions.
        /// </summary>
        public static IScriptDetailGrid DetailGrid
        {
            get
            {
                if (_detailGrid == null)
                    throw new WDEException("API80001", new object[] { "DetailGrid" });
                return _detailGrid;
            }
            set
            {
                if (_mapper != null)
                {
                    if (_detailGridProxy == null)
                    {
                        _detailGridProxy = new ScriptDetailGridProxy();
                        _mapper.SetHostObject(_detailGridProxy); // Set the Script AppDomain Reference and create Remoting proxies.
                    }

                    _detailGridProxy.RealObject = value;
                    _detailGrid = value;
                }
                else
                {
                    _detailGrid = value;
                }
            }
        }

        /// <summary>
        /// Provides access to batch-level properties and methods.
        /// </summary>
        public static IScriptWork Work
        {
            get
            {
                if (_work == null)
                    throw new WDEException("API80001", new object[] { "Work" });

                return _work;
            }

            set
            {
                if (_mapper != null)
                {
                    if (_workProxy == null)
                    {
                        _workProxy = new ScriptWorkProxy();
                        _mapper.SetHostObject(_workProxy); // Set the Script AppDomain Reference and create Remoting proxies.
                    }

                    _workProxy.RealObject = value;
                    _work = value;
                }
                else
                {
                    _work = value;
                }
            }
        }

        /// <summary>
        /// Provides access to document related properties and methods.
        /// </summary>
        public static IScriptDocument Document
        {
            get
            {
                if (_document == null)
                    throw new WDEException("API80001", new object[] { "Document" });
                return _document;
            }

            set
            {
                if (_mapper != null)
                {
                    if (_documentProxy == null)
                    {
                        _documentProxy = new ScriptDocumentProxy();
                        _mapper.SetHostObject(_documentProxy); // Set the Script AppDomain Reference and create Remoting proxies.
                    }

                    _documentProxy.RealObject = value;
                    _document = value;
                }
                else
                {
                    _document = value;
                }
            }
        }

        /// <summary>
        /// Provides access to form related properties and methods.
        /// </summary>
        public static IScriptForm Form
        {
            get
            {
                if (_form == null)
                    throw new WDEException("API80001", new object[] { "Form" });
               return _form;
            }

            set
            {
                if (_mapper != null)
                {
                    if (_formProxy == null)
                    {
                        _formProxy = new ScriptFormProxy();
                        _mapper.SetHostObject(_formProxy); // Set the Script AppDomain Reference and create Remoting proxies.
                    }

                    _formProxy.RealObject = value;
                    _form = value;

                    if (_form == null)
                    {
                        CleanupRecObjects();
                        _recordProxy = null;
                        _fieldProxy = null;
                    }
                }
                else
                {
                    _form = value;
                }
            }
        }

        /// <summary>
        /// Provides access to textbox related properties and methods.
        /// </summary>
        public static IScriptTextBox TextBox
        {
            get
            {
                return _textBox;
            }

            set
            {
                if (_mapper != null)
                {
                    if (_textBoxProxy == null)
                    {
                        _textBoxProxy = new ScriptTextBoxProxy();
                    }

                    if (value != null)
                        _mapper.SetHostObject(_textBoxProxy); // Set the Script AppDomain Reference and create Remoting proxies.
                    else
                    {
                        _mapper.SetHostObject(value);
                    }

                    _textBoxProxy.RealObject = value;
                    _textBox = value;
                }
                else
                {
                    _textBox = value;
                }
            }
        }

        /// <summary>
        /// Provides access to field related properties and methods.
        /// </summary>
        public static IScriptField Field
        {
            get
            {
                if (_field == null)
                    throw new WDEException("API80001", new object[] { "Field" });
                return _field;
            }

            set
            {
                if (_mapper != null)
                {
                    _field = value;

                    if (_field != null)
                    {
                        ScriptFieldProxy proxy = GetProxy(_field);
                        if (_field != proxy)
                        {
                            _mapper.SetHostObject(proxy);
                            _field = proxy;
                        }
                    }
                    else
                        _fieldProxy = null;
                }
                else
                {
                    _field = value;
                }
            }
        }

        /// <summary>
        /// Provides access to record related properties and methods.
        /// </summary>
        public static IScriptRecord Record
        {
            get
            {
                if (_record == null)
                    throw new WDEException("API80001", new object[] { "Record" });                
                return _record;
            }

            set
            {
                if (_mapper != null)
                {
                    _record = value;

                    if (_record != null)
                    {
                        ScriptRecordProxy proxy = GetProxy(_record);
                        if (proxy != _recordProxy)
                        {
                            _mapper.SetHostObject(proxy);
                            _recordProxy = proxy;
                        }
                    }
                    else
                        _recordProxy = null;
                }
                else
                {
                    _record = value;
                }
            }
        }

        /// <summary>
        /// Provides access to client related properties and methods.
        /// </summary>
        public static IScriptClient Client
        {
            get
            {
                if (_client == null)
                    throw new WDEException("API80001", new object[] { "Client" });
                return _client;
            }

            set
            {
                _client = value;
            }
        }

        #endregion

        // This is necessary for cross-domain support
        public static ScriptEventMapper Mapper
        {
            get { return _mapper; }
            set
            {
                if (value == null && _mapper != null)
                {
                    Disconnect(_databaseProxy);
                    Disconnect(_detailGridProxy);
                    Disconnect(_documentProxy); 
                    Disconnect(_fieldProxy);
                    Disconnect(_formProxy);
                    Disconnect(_imageProxy);
                    Disconnect(_lookupDialogProxy); 
                    Disconnect(_recordProxy);
                    Disconnect(_textBoxProxy);
                    Disconnect(_workProxy);

                    _databaseProxy = null;
                    _detailGrid = null;
                    _detailGridProxy = null;
                    _document = null;
                    _documentProxy = null;
                    _field = null;
                    _fieldProxy = null;
                    _form = null;
                    _formProxy = null;
                    _image = null;
                    _imageProxy = null;
                    _lookupDialog = null;
                    _lookupDialogProxy = null;
                    _record = null;
                    _recordProxy = null;
                    _textBox = null;
                    _textBoxProxy = null;
                    _work = null;
                    _workProxy = null;
                    CleanupRecObjects();
                }
                _mapper = value;
            }
        }

        internal static ScriptRecordProxy GetProxy(IScriptRecord record)
        {
            if(_records != null)
            {
                ScriptRecordProxy result = null;
                if (_records.TryGetValue(record, out result))
                    return result;
                else
                {
                    result = new ScriptRecordProxy();
                    result.RealObject = record;
                    _records.Add(record, result);
                    return result;
                }
            }
            else
            {
                _records = new Dictionary<IScriptRecord,ScriptRecordProxy>();
                ScriptRecordProxy result = new ScriptRecordProxy();
                result.RealObject = record;
                _records.Add(record, result);
                return result;
            }
        }

        internal static ScriptFieldProxy GetProxy(IScriptField field)
        {
            if (_fields != null)
            {
                ScriptFieldProxy result = null;
                if (_fields.TryGetValue(field, out result))
                    return result;
                else
                {
                    result = new ScriptFieldProxy();
                    result.RealObject = field;
                    _fields.Add(field, result);
                    return result;
                }
            }
            else
            {
                _fields = new Dictionary<IScriptField, ScriptFieldProxy>();
                ScriptFieldProxy result = new ScriptFieldProxy();
                result.RealObject = field;
                _fields.Add(field, result);
                return result;
            }
        }

        internal static ScriptRevisionProxy GetProxy(IScriptRevision revision)
        {
            if (_revisions != null)
            {
                ScriptRevisionProxy result = null;
                if (_revisions.TryGetValue(revision, out result))
                    return result;
                else
                {
                    result = new ScriptRevisionProxy();
                    result.RealObject = revision;
                    _revisions.Add(revision, result);
                    return result;
                }
            }
            else
            {
                _revisions = new Dictionary<IScriptRevision, ScriptRevisionProxy>();
                ScriptRevisionProxy result = new ScriptRevisionProxy();
                result.RealObject = revision;
                _revisions.Add(revision, result);
                return result;
            }
        }

        private static void CleanupRecObjects()
        {
            if(_records != null)
            {
                foreach(KeyValuePair<IScriptRecord, ScriptRecordProxy> kv in _records)
                    Disconnect(kv.Value);

                _records.Clear();
                _records = null;
            }

            if(_fields != null)
            {
                foreach(KeyValuePair<IScriptField, ScriptFieldProxy> kv in _fields)
                    Disconnect(kv.Value);

                _fields.Clear();
                _fields = null;
            }

            if (_revisions != null)
            {
                foreach (KeyValuePair<IScriptRevision, ScriptRevisionProxy> kv in _revisions)
                    Disconnect(kv.Value);

                _revisions.Clear();
                _revisions = null;
            }
        }

        private static void Disconnect(MarshalByRefObject obj)
        {
            if (obj != null && _mapper != null)
            {
                if (obj is IScriptDisconnect)
                    ((IScriptDisconnect)obj).Disconnect();
                RemotingServices.Disconnect(obj);
            }
        }
    }
}