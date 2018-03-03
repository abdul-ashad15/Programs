using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;

namespace WebDX.Api.Scripts
{
    /* NOTE: These object implement a single Remoting infrastructure from the Main AppDomain to the Script AppDomain
     * Without these objects, new Remoting proxies and sinks are created each time a new object is returned from
     * the Host object to the Script Domain.
     * This is both bad for performance and memory and can also cause unintended consequenses because unused Remoting
     * proxies can hold on to references to WebDX objects in the Main AppDomain, causing memory leaks.
     * 
     * The Remoting proxies can be controlled using these classes.
     */

    public interface IScriptDisconnect
    {
        void Disconnect();
    }

    public class ScriptDatabaseProxy : MarshalByRefObject, IScriptDatabase
    {
        private IScriptDatabase _realObject;

        public ScriptDatabaseProxy() { }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptDatabase Members

        public System.Data.DataSet RunQuery(string databaseName, string query)
        {
            return _realObject.RunQuery(databaseName, query);
        }

        #endregion

        public IScriptDatabase RealObject
        {
            get { return _realObject; }
            set { _realObject = value; }
        }
    }

    public class ScriptDetailGridProxy : MarshalByRefObject, IScriptDetailGrid
    {
        private IScriptDetailGrid _realObject;

        public ScriptDetailGridProxy() { }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptDetailGrid Members

        public void GotoNextRecord()
        {
            _realObject.GotoNextRecord();
        }

        public void Exit()
        {
            _realObject.Exit();
        }

        public bool LastRecord
        {
            get
            {
                return _realObject.LastRecord;
            }
        }

        public void ReleaseRow()
        {
            _realObject.ReleaseRow();
        }
    
        #endregion       

        public IScriptDetailGrid RealObject
        {
            get { return _realObject; }
            set { _realObject = value; }
        }
    }

    public class ScriptDocumentProxy : MarshalByRefObject, IScriptDocument
    {
        private IScriptDocument _realObject;

        public ScriptDocumentProxy() { }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptDocument Members

        public string DCN
        {
            get { return _realObject.DCN; }
        }

        public string AltDCN
        {
            get { return _realObject.AltDCN; }
        }

        public string ItemType
        {
            get { return _realObject.ItemType; }
        }

        public IDictionary<string, string> UDFS
        {
            get { return _realObject.UDFS; }
        }

        public bool IsRejected
        {
            get { return _realObject.IsRejected; }
        }

        #endregion

        public IScriptDocument RealObject
        {
            get { return _realObject; }
            set { _realObject = value; }
        }        
    }

    public class ScriptFieldsProxy : MarshalByRefObject, IScriptFields, IScriptDisconnect
    {
        private IScriptFields _realObject;
        private Dictionary<string, ScriptFieldProxy> _fieldProxies;

        public ScriptFieldsProxy()
        {
            _fieldProxies = new Dictionary<string, ScriptFieldProxy>();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptFields Members

        public int Count
        {
            get { return _realObject.Count; }
        }

        public IScriptField this[int index]
        {
            get
            {
                IScriptField field = _realObject[index];
                if (_fieldProxies.ContainsKey(field.FieldName))
                {
                    return _fieldProxies[field.FieldName];
                }
                else
                {
                    ScriptFieldProxy proxy = Host.GetProxy(field);
                    _fieldProxies.Add(field.FieldName, proxy);
                    return proxy;
                }
            }
        }

        public IScriptField this[string fieldName]
        {
            get
            {
                if (_fieldProxies.ContainsKey(fieldName))
                {
                    return _fieldProxies[fieldName];
                }
                else
                {
                    ScriptFieldProxy proxy = Host.GetProxy(_realObject[fieldName]);
                    _fieldProxies.Add(fieldName, proxy);
                    return proxy;
                }
            }
        }

        #endregion

        #region IScriptDisconnect Members

        public void Disconnect()
        {
            foreach (KeyValuePair<string, ScriptFieldProxy> kv in _fieldProxies)
                RemotingServices.Disconnect(kv.Value);
            _fieldProxies.Clear();
        }

        #endregion

        public IScriptFields RealObject
        {
            get { return _realObject; }
            set { _realObject = value; }
        }
    }

    public class ScriptFieldProxy : MarshalByRefObject, IScriptField, IScriptDisconnect
    {
        private IScriptField _realObject;
        private ScriptRevisionsProxy _revisionsProxy;

        public ScriptFieldProxy()
        {
            _revisionsProxy = new ScriptRevisionsProxy();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptField Members

        public string Value
        {
            get
            {
                return _realObject.Value;
            }
            set
            {
                _realObject.Value = value;
            }
        }

        public bool Exclude
        {
            get { return _realObject.Exclude; }
        }

        public string FlagDescription
        {
            get
            {
                return _realObject.FlagDescription;
            }
            set
            {
                _realObject.FlagDescription = value;
            }
        }

        public string CustomData
        {
            get
            {
                return _realObject.CustomData;
            }
            set
            {
                _realObject.CustomData = value;
            }
        }

        public string FieldName
        {
            get { return _realObject.FieldName; }
        }

        public int Length
        {
            get { return _realObject.Length; }
        }

        public WDEFieldStatus UpdateStatus
        {
            get
            {
                return _realObject.UpdateStatus;
            }
            set
            {
                _realObject.UpdateStatus = value;
            }
        }

        public void Dupe()
        {
            _realObject.Dupe();
        }

        public IScriptRevisions Revisions
        {
            get { return _revisionsProxy; }
        }

        public double NumberValue
        {
            get { return _realObject.NumberValue; }
        }

        #endregion        

        #region IScriptDisposable Members

        public void Disconnect()
        {
            RemotingServices.Disconnect(_revisionsProxy);
        }

        #endregion

        public IScriptField RealObject
        {
            get { return _realObject; }
            set
            {
                _realObject = value;
                _revisionsProxy.RealObject = _realObject == null ? null : _realObject.Revisions;
            }
        }
    }

    public class ScriptRevisionsProxy : MarshalByRefObject, IScriptRevisions
    {
        private IScriptRevisions _realObject;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public ScriptRevisionsProxy()
        {
        }

        #region IScriptRevisions Members

        public int Count
        {
            get { return _realObject.Count; }
        }

        public IScriptRevision this[int index]
        {
            get
            {
                IScriptRevision rev = _realObject[index];
                return Host.GetProxy(rev);
            }
        }

        #endregion

        public IScriptRevisions RealObject
        {
            get { return _realObject; }
            set
            {
                _realObject = value;
            }
        }
    }

    public class ScriptRevisionProxy : MarshalByRefObject, IScriptRevision
    {
        private IScriptRevision _realObject;

        public ScriptRevisionProxy() { }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptRevision Members

        public string Value
        {
            get { return _realObject.Value; }
        }

        public string FlagDescription
        {
            get { return _realObject.FlagDescription; }
        }

        public WDEFieldStatus RevisionStatus
        {
            get { return _realObject.RevisionStatus; }
        }

        public string User
        {
            get { return _realObject.User; }
        }

        public string Task
        {
            get { return _realObject.Task; }
        }

        public WDEOpenMode SessionMode
        {
            get { return _realObject.SessionMode; }
        }

        #endregion

        public IScriptRevision RealObject
        {
            get { return _realObject; }
            set { _realObject = value; }
        }
    }

    public class ScriptFormsProxy : MarshalByRefObject, IScriptForms
    {
        private IScriptForms _realObject;

        public ScriptFormsProxy() { }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptForms Members

        public int Count
        {
            get { return _realObject.Count; }
        }

        public string this[int index]
        {
            get
            {
                return _realObject[index];
            }
        }

        #endregion

        public IScriptForms RealObject
        {
            get { return _realObject; }
            set { _realObject = value; }
        }
    }

    public class ScriptFormProxy : MarshalByRefObject, IScriptForm, IScriptDisconnect
    {
        private IScriptForm _realObject;
        private ScriptLabelsProxy _labelsProxy;

        public ScriptFormProxy()
        {
            _labelsProxy = new ScriptLabelsProxy();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptForm Members

        public IScriptLabels Labels
        {
            get { return _labelsProxy; }
        }

        public void ExcludeField(bool excluded, string fieldName)
        {
            _realObject.ExcludeField(excluded, fieldName);
        }

        public void ExcludeField(bool excluded, string fieldName, int recordNumber)
        {
            _realObject.ExcludeField(excluded, fieldName, recordNumber);
        }

        public void GotoField(string fieldName)
        {
            _realObject.GotoField(fieldName);
        }

        public void GotoField(string fieldName, int recordNumber)
        {
            _realObject.GotoField(fieldName, recordNumber);
        }

        public void GotoTextBox(string textBoxName)
        {
            _realObject.GotoTextBox(textBoxName);
        }

        public void Release()
        {
            _realObject.Release();
        }

        public void Advance()
        {
            _realObject.Advance();
        }

        public void GotoField(string fieldName, bool skip)
        {
            _realObject.GotoField(fieldName, skip);
        }

        public void GotoField(string fieldName, int recordNumber, bool skip)
        {
            _realObject.GotoField(fieldName, recordNumber, skip);
        }

        #endregion

        #region IScriptDisconnect Members

        public void Disconnect()
        {
            _labelsProxy.Disconnect();
            RemotingServices.Disconnect(_labelsProxy);
        }

        #endregion

        public IScriptForm RealObject
        {
            get { return _realObject; }
            set
            {
                _realObject = value;
                _labelsProxy.RealObject = _realObject == null ? null : _realObject.Labels;
            }
        }
    }

    public class ScriptLabelsProxy : MarshalByRefObject, IScriptLabels, IScriptDisconnect
    {
        private IScriptLabels _realObject;
        private Dictionary<string, ScriptLabelProxy> _labelProxies;

        public ScriptLabelsProxy()
        {
            _labelProxies = new Dictionary<string, ScriptLabelProxy>();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptDisconnect Members

        public void Disconnect()
        {
            foreach (KeyValuePair<string, ScriptLabelProxy> kv in _labelProxies)
                RemotingServices.Disconnect(kv.Value);
            _labelProxies.Clear();
        }

        #endregion

        #region IScriptLabels Members

        public int Count
        {
            get { return _realObject.Count; }
        }

        public IScriptLabel this[int index]
        {
            get
            {
                IScriptLabel label = _realObject[index];
                if (_labelProxies.ContainsKey(label.LabelName))
                    return _labelProxies[label.LabelName];
                else
                {
                    ScriptLabelProxy proxy = new ScriptLabelProxy();
                    proxy.RealObject = label;
                    _labelProxies.Add(label.LabelName, proxy);
                    return proxy;
                }
            }
        }

        public IScriptLabel this[string labelName]
        {
            get
            {
                IScriptLabel label = _realObject[labelName];
                if (_labelProxies.ContainsKey(label.LabelName))
                    return _labelProxies[label.LabelName];
                else
                {
                    ScriptLabelProxy proxy = new ScriptLabelProxy();
                    proxy.RealObject = label;
                    _labelProxies.Add(label.LabelName, proxy);
                    return proxy;
                }
            }
        }

        #endregion

        public IScriptLabels RealObject
        {
            get { return _realObject; }
            set
            {
                if (value == null)
                    Disconnect();
                _realObject = value;
            }
        }
    }

    public class ScriptLabelProxy : MarshalByRefObject, IScriptLabel
    {
        private IScriptLabel _realObject;

        public ScriptLabelProxy() { }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptLabel Members

        public string Text
        {
            get
            {
                return _realObject.Text;
            }
            set
            {
                _realObject.Text = value;
            }
        }

        public string LabelName
        {
            get { return _realObject.LabelName; }
        }

        public System.Drawing.Color BackColor
        {
            get
            {
                return _realObject.BackColor;
            }
            set
            {
                _realObject.BackColor = value;
            }
        }

        public System.Drawing.Color ForeColor
        {
            get
            {
                return _realObject.ForeColor;
            }
            set
            {
                _realObject.ForeColor = value;
            }
        }

        public System.Drawing.Font Font
        {
            get
            {
                return _realObject.Font;
            }
            set
            {
                _realObject.Font = value;
            }
        }

        #endregion

        public IScriptLabel RealObject
        {
            get { return _realObject; }
            set { _realObject = value; }
        }
    }

    public class ScriptImageProxy : MarshalByRefObject, IScriptImage, IScriptDisconnect
    {
        private IScriptImage _realObject;
        private ScriptImageNamesProxy _imageNamesProxy;
        private ScriptImageTypesProxy _imageTypesProxy;

        public ScriptImageProxy()
        {
            _imageNamesProxy = new ScriptImageNamesProxy();
            _imageTypesProxy = new ScriptImageTypesProxy();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptImage Members

        public int PageCount
        {
            get { return _realObject.PageCount; }
        }

        public IScriptImageNames ImageName
        {
            get { return _imageNamesProxy; }
        }

        public IScriptImageTypes ImageType
        {
            get { return _imageTypesProxy; }
        }

        public int SelectedPage
        {
            get
            {
                return _realObject.SelectedPage;
            }
            set
            {
                _realObject.SelectedPage = value;
            }
        }

        public string SelectedZone
        {
            get
            {
                return _realObject.SelectedZone;
            }
            set
            {
                _realObject.SelectedZone = value;
            }
        }

        public bool OverlayVisible
        {
            get
            {
                return _realObject.OverlayVisible;
            }
            set
            {
                _realObject.OverlayVisible = value;
            }
        }

        public System.Drawing.Rectangle Viewport
        {
            get
            {
                return _realObject.Viewport;
            }
            set
            {
                _realObject.Viewport = value;
            }
        }

        public void FitWidth()
        {
            _realObject.FitWidth();
        }

        public void FitHeight()
        {
            _realObject.FitHeight();
        }

        public void FitFull()
        {
            _realObject.FitFull();
        }

        public int Scaling
        {
            get
            {
                return _realObject.Scaling;
            }
            set
            {
                _realObject.Scaling = value;
            }
        }

        public void Rotate(Orientations direction)
        {
            _realObject.Rotate(direction);
        }

        public void Scroll(ScrollDisplay direction)
        {
            _realObject.Scroll(direction);
        }        

        public void ScrollTo(int x, int y)
        {
            _realObject.ScrollTo(x, y);
        }

        #endregion        

        #region IScriptDisconnect Members

        public void Disconnect()
        {
            _imageNamesProxy.RealObject = null;
            _imageTypesProxy.RealObject = null;
            RemotingServices.Disconnect(_imageNamesProxy);
            RemotingServices.Disconnect(_imageTypesProxy);
        }

        #endregion

        public IScriptImage RealObject
        {
            get { return _realObject; }
            set
            {
                _realObject = value;
                _imageNamesProxy.RealObject = _realObject == null ? null : _realObject.ImageName;
                _imageTypesProxy.RealObject = _realObject == null ? null : _realObject.ImageType;
            }
        }
    }

    public class ScriptImageNamesProxy : MarshalByRefObject, IScriptImageNames
    {
        private IScriptImageNames _realObject;

        public ScriptImageNamesProxy() { }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptImageNames Members

        public string this[int index]
        {
            get { return _realObject[index]; }
        }

        #endregion

        public IScriptImageNames RealObject
        {
            get { return _realObject; }
            set { _realObject = value; }
        }
    }

    public class ScriptImageTypesProxy : MarshalByRefObject, IScriptImageTypes
    {
        private IScriptImageTypes _realObject;

        public ScriptImageTypesProxy() { }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptImageTypes Members

        public string this[int index]
        {
            get
            {
                return _realObject[index];
            }
            set
            {
                _realObject[index] = value;
            }
        }

        #endregion

        public IScriptImageTypes RealObject
        {
            get { return _realObject; }
            set { _realObject = value; }
        }
    }

    public class ScriptLookupDialogProxy : MarshalByRefObject, IScriptLookupDialog
    {
        private IScriptLookupDialog _realObject;

        public ScriptLookupDialogProxy() { }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptLookupDialog Members

        public System.Data.DataTable TableLookup(System.Data.DataTable lookupTable, string[] displayColumns, bool showDifferences)
        {
            return _realObject.TableLookup(lookupTable, displayColumns, showDifferences);
        }

        public System.Data.DataTable TableLookup(System.Data.DataTable lookupTable, string[] displayColumns, bool showDifferences, System.Drawing.Size dialogSize)
        {
            return _realObject.TableLookup(lookupTable, displayColumns, showDifferences, dialogSize);
        }

        public System.Data.DataTable TableLookup(System.Data.DataTable lookupTable, string[] displayColumns, bool showDifferences, System.Drawing.Size dialogSize, System.Drawing.Point dialogLocation)
        {
            return _realObject.TableLookup(lookupTable, displayColumns, showDifferences, dialogSize, dialogLocation);
        }

        public bool ZipLookup(string databaseName, IScriptField zipCodeField, IScriptField cityCodeField, IScriptField cityField, IScriptField stateField, bool oneHitPopup)
        {
            return _realObject.ZipLookup(databaseName, zipCodeField, cityCodeField, cityField, stateField, oneHitPopup);
        }

        #endregion

        public IScriptLookupDialog RealObject
        {
            get { return _realObject; }
            set { _realObject = value; }
        }
    }

    public class ScriptRecordsProxy : MarshalByRefObject, IScriptRecords
    {
        private IScriptRecords _realObject;

        public ScriptRecordsProxy()
        {
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptRecords Members

        public int Count
        {
            get
            {
                return _realObject.Count;
            }
        }

        public IScriptRecord this[int index]
        {
            get
            {
                IScriptRecord target = _realObject[index];
                return Host.GetProxy(target);
            }
        }

        public void Append()
        {
            _realObject.Append();
        }

        #endregion

        public IScriptRecords RealObject
        {
            get { return _realObject; }
            set
            {
                _realObject = value;
            }
        }
    }

    public class ScriptRecordProxy : MarshalByRefObject, IScriptRecord, IScriptDisconnect
    {
        private IScriptRecord _realObject;
        private ScriptFieldsProxy _fieldsProxy;
        private ScriptRecordsProxy _siblingProxy;

        public ScriptRecordProxy()
        {
            
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptRecord Members

        public string RecType
        {
            get
            {
                return _realObject.RecType;
            }
        }

        public IScriptRecord Parent
        {
            get
            {
                if (_realObject.Parent == null)
                    return null;
                else
                    return Host.GetProxy(_realObject.Parent);
            }
        }

        public IScriptFields Fields
        {
            get
            {
                return _fieldsProxy;
            }
        }

        public IScriptRecords SiblingRecords
        {
            get
            {
                if (_siblingProxy == null)
                {
                    _siblingProxy = new ScriptRecordsProxy();
                    _siblingProxy.RealObject = _realObject.SiblingRecords;
                }
                return _siblingProxy;
            }
        }

        public IScriptRecords GetChildRecords(string recType)
        {
            ScriptRecordsProxy result = new ScriptRecordsProxy();
            IScriptRecords realObj = _realObject.GetChildRecords(recType);
            if (realObj == null)
                return null;
            else
            {
                result.RealObject = realObj;
                return result;
            }
        } 

        public bool IsDeleted
        {
            get { return _realObject.IsDeleted; }
        }

        public int SessionID
        {
            get { return _realObject.SessionID; }
        }

        public void Delete()
        {
            _realObject.Delete();
        }

        public void RestoreDeletedRow()
        {
            _realObject.RestoreDeletedRow();
        }

        public int SiblingPosition
        {
            get { return _realObject.SiblingPosition; }
        }

        #endregion        

        public IScriptRecord RealObject
        {
            get { return _realObject; }
            set
            {
                _realObject = value;
                if (_realObject != null)
                {
                    _fieldsProxy = new ScriptFieldsProxy();
                    _fieldsProxy.RealObject = _realObject.Fields;
                }
                else
                {
                    Disconnect();
                    _fieldsProxy = null;
                }
            }
        }

        #region IScriptDisconnect Members

        public void Disconnect()
        {
            if (_fieldsProxy != null)
            {
                _fieldsProxy.Disconnect();
                _fieldsProxy.RealObject = null;
                RemotingServices.Disconnect(_fieldsProxy);
            }
            if (_siblingProxy != null)
            {
                _siblingProxy.RealObject = null;
                RemotingServices.Disconnect(_siblingProxy);
            }
        }

        #endregion
    }

    public class ScriptTextBoxProxy : MarshalByRefObject, IScriptTextBox
    {
        private IScriptTextBox _realObject;

        public ScriptTextBoxProxy() { }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptTextBox Members

        public string Text
        {
            get
            {
                return _realObject.Text;
            }
            set
            {
                _realObject.Text = value;
            }
        }

        public int SelStart
        {
            get
            {
                return _realObject.SelStart;
            }
            set
            {
                _realObject.SelStart = value;
            }
        }

        public int SelLength
        {
            get
            {
                return _realObject.SelLength;
            }
            set
            {
                _realObject.SelLength = value;
            }
        }

        public string SelText
        {
            get
            {
                return _realObject.SelText;
            }
            set
            {
                _realObject.SelText = value;
            }
        }

        public System.Drawing.Color ForeColor
        {
            get
            {
                return _realObject.ForeColor;
            }
            set
            {
                _realObject.ForeColor = value;
            }
        }

        public System.Drawing.Color BackColor
        {
            get
            {
                return _realObject.BackColor;
            }
            set
            {
                _realObject.BackColor = value;
            }
        }

        public bool Bold
        {
            get
            {
                return _realObject.Bold;
            }
            set
            {
                _realObject.Bold = value;
            }
        }

        public double NumberValue
        {
            get { return _realObject.NumberValue; }
        }

        public string Value
        {
            get { return _realObject.Value; }
            set { _realObject.Value = value; }
        }

        #endregion

        public IScriptTextBox RealObject
        {
            get { return _realObject; }
            set { _realObject = value; }
        }
    }

    public class ScriptWorkProxy : MarshalByRefObject, IScriptWork, IScriptDisconnect
    {
        private IScriptWork _realObject;
        private ScriptFormsProxy _formsProxy;

        public ScriptWorkProxy()
        {
            _formsProxy = new ScriptFormsProxy();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region IScriptWork Members

        public int CurrentDocument
        {
            get
            {
                return _realObject.CurrentDocument;
            }
            set
            {
                _realObject.CurrentDocument = value;
            }
        }

        public int DocumentCount
        {
            get { return _realObject.DocumentCount; }
        }

        public string CurrentFormType
        {
            get
            {
                return _realObject.CurrentFormType;
            }
            set
            {
                _realObject.CurrentFormType = value;
            }
        }

        public WDEEntryMode EntryMode
        {
            get { return _realObject.EntryMode; }
        }

        public string BatchName
        {
            get { return _realObject.BatchName; }
        }       

        public string TaskName
        {
            get { return _realObject.TaskName; }
        }

        public IScriptForms AvailableFormTypes
        {
            get { return _formsProxy; }
        }

        public string Hint
        {
            get
            {
                return _realObject.Hint;
            }
            set
            {
                _realObject.Hint = value;
            }
        }

        public IDictionary<string, string> CustomParameters
        {
            get { return _realObject.CustomParameters; }
        }

        public void Next()
        {
            _realObject.Next();
        }

        public void Prior()
        {
            _realObject.Prior();
        }

        public void Home()
        {
            _realObject.Home();
        }

        public void End()
        {
            _realObject.End();
        }

        public System.Drawing.Point RequestDialogPos(System.Drawing.Size dialogSize)
        {
            return _realObject.RequestDialogPos(dialogSize);
        }

        public void LoadSupplemental(string formType)
        {
            _realObject.LoadSupplemental(formType);
        }

        public bool ExitSupplemental()
        {
            return _realObject.ExitSupplemental();
        }

        public string UserName
        {
            get { return _realObject.UserName; }
        }

        public string SessionName
        {
            get { return _realObject.SessionName; }
        }

        public System.Drawing.Color HintForeColor
        {
            get
            {
                return _realObject.HintForeColor;
            }
            set
            {
                _realObject.HintForeColor = value;
            }
        }

        public System.Drawing.Color HintBackColor
        {
            get
            {
                return _realObject.HintBackColor;
            }
            set
            {
                _realObject.HintBackColor = value;
            }
        }

        public int TotalDocCompleted
        {
            get { return _realObject.TotalDocCompleted; }
        }

        public int TotalDocCancelled
        {
            get { return _realObject.TotalDocCancelled; }
        }

        public int TotalDocRejected
        {
            get { return _realObject.TotalDocRejected; }
        }

        public void AddRejectCode (string rejectCode, string rejectDescription, bool requireReason)
        {
            _realObject.AddRejectCode(rejectCode, rejectDescription, requireReason);
        }

        #endregion        

        #region IScriptDisconnect Members

        public void Disconnect()
        {
            _formsProxy.RealObject = null;
            RemotingServices.Disconnect(_formsProxy);
        }

        #endregion

        public IScriptWork RealObject
        {
            get { return _realObject; }
            set
            {
                _realObject = value;
                _formsProxy.RealObject = _realObject == null ? null : _realObject.AvailableFormTypes;
            }
        }
    }
}
