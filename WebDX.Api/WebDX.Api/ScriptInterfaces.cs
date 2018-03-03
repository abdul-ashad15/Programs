using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace WebDX.Api.Scripts
{
    #region Attributes

    /// <summary>
    /// Marks classes as containers for WebDX events.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=true, AllowMultiple=false)]
    public class EventContainerAttribute : Attribute
    {
        public EventContainerAttribute() {  }
    }

    /// <summary>
    /// Identifies the type of event a method implements.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Identifies a TextBox enter event.
        /// </summary>
        TextBoxEnter,
        /// <summary>
        /// Identifies a TextBox exit event.
        /// </summary>
        TextBoxExit,
        /// <summary>
        /// Identifies a TextBox key press event.
        /// </summary>
        TextBoxKeyPress,
        /// <summary>
        /// Identifies a TextBox validate event.
        /// </summary>
        TextBoxValidate,
        /// <summary>
        /// Identifies a DetailGrid enter event.
        /// </summary>
        DetailGridEnter,
        /// <summary>
        /// Identifies a DetailGrid exit event.
        /// </summary>
        DetailGridExit,
        /// <summary>
        /// Identifies a Form enter event.
        /// </summary>
        FormEnter,
        /// <summary>
        /// Identifies a Form exit event.
        /// </summary>
        FormExit,
        /// <summary>
        /// Identifies a Project key press event.
        /// </summary>
        ProjectKeyPress,
        /// <summary>
        /// Identifies a Project document reject event.
        /// </summary>
        ProjectDocumentReject,
        /// <summary>
        /// Identifies a Project document unreject event.
        /// </summary>
        ProjectDocumentUnReject,
        /// <summary>
        /// Identifies a Project start work event.
        /// </summary>
        ProjectStartWork,
        /// <summary>
        /// Identifies a Project end work event.
        /// </summary>
        ProjectEndWork,
        /// <summary>
        /// Identifies an Image page change event.
        /// </summary>
        ImagePageChange
    }

    /// <summary>
    /// Marks methods as WebDX events. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited=true, AllowMultiple=false)]
    public class EventAttribute : Attribute
    {
        private EventType _eventType;

        public EventAttribute(EventType eventType)
        {
            _eventType = eventType;
        }

        public EventType EventType
        {
            get { return _eventType; }
        }
    }

    #endregion

    #region Controlling Interfaces

    /// <summary>
    /// Image related information
    /// </summary>
    public interface IScriptImage
    {
        /// <summary>
        /// Gets the number of image pages for the current document.
        /// </summary>
        int PageCount { get;}
        /// <summary>
        /// Gets the image names for each image in the current document.
        /// </summary>
        IScriptImageNames ImageName { get;}
        /// <summary>
        /// Gets the image types for each image in the current document.
        /// </summary>
        IScriptImageTypes ImageType { get;}
        /// <summary>
        /// Gets or sets the index of the currently selected image.
        /// </summary>
        int SelectedPage { get; set;}
        /// <summary>
        /// Gets or sets the visible zone.
        /// </summary>
        string SelectedZone { get; set;}
        /// <summary>
        /// Gets or sets whether the overlay is visible.
        /// </summary>
        bool OverlayVisible { get; set;}
		/// <summary>
		/// Gets or sets the rectangle of the displayed portion of the image.
		/// </summary>
		System.Drawing.Rectangle Viewport { get; set; }
        /// <summary>
        /// Zooms to fit the width of the image.
        /// </summary>
        void FitWidth();
        /// <summary>
        /// Zooms to fit the height of the image.
        /// </summary>
        void FitHeight();
        /// <summary>
        /// Zooms to show the entire page (FitPage / FitBest).
        /// </summary>
        void FitFull();
        /// <summary>
        /// Gets or sets the ImageScalePercent of the currently selected image.
        /// </summary>
        int Scaling { get; set; }
        /// <summary>
        /// Rotates the image to the specified orientation.
        /// </summary>
        /// <param name="direction">The direction.</param>
        void Rotate(Orientations direction);
        /// <summary>
        /// Scrolls the specified direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        void Scroll(ScrollDisplay direction);
        /// <summary>
        /// ScrollTo to the specified x & y coordinate of the image.
        /// </summary>
        /// <param name="x">The image x coordinate.</param>
        /// <param name="y">The image y coordinate.</param>
        void ScrollTo(int x, int y);
	}

    /// <summary>
    /// The image names for all images in the current document.
    /// </summary>
    public interface IScriptImageNames
    {
        /// <summary>
        /// Gets the image name for a specific image in the current document.
        /// </summary>
        /// <param name="index">The image index.</param>
        /// <returns>The name of the given image.</returns>
        string this[int index] { get;}
    }

    /// <summary>
    /// The image types for all images in the current document.
    /// </summary>
    public interface IScriptImageTypes
    {
        /// <summary>
        /// Gets or sets the image type for a specific image in the current document.
        /// </summary>
        /// <param name="index">The image index.</param>
        /// <returns>The image type for the given image.</returns>
        string this[int index] { get; set;}
    }

    /// <summary>
    /// Database related methods.
    /// </summary>
    public interface IScriptDatabase
    {
        /// <summary>
        /// Runs a query on the given database and returns the results in a System.Data.DataSet.
        /// </summary>
        /// <param name="databaseName">The name of the database to query. This name must match a database name configured on the WebDX server.</param>
        /// <param name="query">The query to run.</param>
        /// <returns>The query results.</returns>
        DataSet RunQuery(string databaseName, string query);
    }

    /// <summary>
    /// The standard lookup dialogs.
    /// </summary>
    public interface IScriptLookupDialog
    {
        /// <summary>
        /// Shows the standard table lookup dialog using the given data.
        /// </summary>
        /// <param name="lookupTable">The table to display.</param>
        /// <param name="displayColumns">The result columns to display.</param>
        /// <param name="showDifferences">Highlight differences in red.</param>
        /// <returns>A table containing only the selected row or null if the dialog was canceled.</returns>
        DataTable TableLookup(DataTable lookupTable, string[] displayColumns, bool showDifferences);
		/// <summary>
		/// Shows the standard table lookup dialog using the given data.
		/// </summary>
		/// <param name="lookupTable">The table to display.</param>
		/// <param name="displayColumns">The result columns to display.</param>
		/// <param name="showDifferences">Highlight differences in red.</param>
		/// <param name="dialogSize">Specify the width and/or height of the dialog.</param>
		/// <returns>A table containing only the selected row or null if the dialog was canceled.</returns>
		DataTable TableLookup(DataTable lookupTable, string[] displayColumns, bool showDifferences, Size dialogSize);
        /// <summary>
        /// Shows the standard table lookup dialog using the given data.
        /// </summary>
        /// <param name="lookupTable">The table to display.</param>
        /// <param name="displayColumns">The result columns to display.</param>
        /// <param name="showDifferences">Highlight differences in red.</param>
        /// <param name="dialogSize">Specify the width and/or height of the dialog.</param>
        /// <param name="dialogLocation">Specify the location (horizontal & vertical position) of the dialog.</param>
        /// <returns>A table containing only the selected row or null if the dialog was canceled.</returns>
        DataTable TableLookup(DataTable lookupTable, string[] displayColumns, bool showDifferences, Size dialogSize, Point dialogLocation);
		/// <summary>
        /// Shows the zip code lookup dialog using the given data.
        /// </summary>
        /// <param name="databaseName">The database containing the zip code table.</param>
        /// <param name="zipCodeField">The zip code field.</param>
        /// <param name="cityCodeField">The city code field.</param>
        /// <param name="cityField">The city field.</param>
        /// <param name="stateField">The state field.</param>
        /// <param name="oneHitPopup">Pop up the dialog even if there is only one record in the result set.</param>
        /// <returns>True if the user selected a city. False if the dialog was canceled.</returns>
        bool ZipLookup(string databaseName, IScriptField zipCodeField, IScriptField cityCodeField, IScriptField cityField, IScriptField stateField, bool oneHitPopup);
    }

    /// <summary>
    /// Form related information.
    /// </summary>
    public interface IScriptForm
    {
        /// <summary>
        /// Gets the labels on the current form.
        /// </summary>
        IScriptLabels Labels { get;}
        /// <summary>
        /// Excludes the field on the form making it read-only.
        /// </summary>
		/// <param name="excluded">States whether or not the field should be excluded.</param>
		/// <param name="fieldName">The name of the field to exclude.</param>
		void ExcludeField(bool excluded, string fieldName);
        /// <summary>
        /// Excludes the field on the form in the given detail grid record number.
        /// </summary>
		/// <param name="excluded">States whether or not the field should be excluded.</param>
		/// <param name="fieldName">The name of the field to exclude.</param>
        /// <param name="recordNumber">The record number of the field.</param>
        void ExcludeField(bool excluded, string fieldName, int recordNumber);
		/// <summary>
		/// Navigates to the textbox attached to the given form.
		/// </summary>
		/// <param name="fieldName">The name of the field to navigate to.</param>
		void GotoField(string fieldName);
		/// <summary>
		/// Navigates to the textbox attached to the given form in the given detail grid record number.
		/// </summary>
		/// <param name="fieldName">The name of the field to navigate to.</param>
		/// <param name="recordNumber">The record number to navigate to.</param>
		void GotoField(string fieldName, int recordNumber);
		/// <summary>
        /// Navigates to the specified textbox.
        /// </summary>
        /// <param name="textBoxName">The name of the textbox to navigate to.</param>
        void GotoTextBox(string textBoxName);
        /// <summary>
        /// Exits the current document and navigates to the next document or to the Put dialog.
        /// </summary>
        void Release();
        /// <summary>
        /// Advances to the next TextBox as if the user had pressed the Enter key.
        /// </summary>
        void Advance();
        /// <summary>
        /// Navigates to the textbox attached to the given form.
        /// </summary>
        /// <param name="fieldName">The name of the field to navigate to.</param>
        /// <param name="skip">If true, advances to the next non skip field when the navigating field is a skip field.</param>
        void GotoField(string fieldName, bool skip);
        /// <summary>
        /// Navigates to the textbox attached to the given form in the given detail grid record number.
        /// </summary>
        /// <param name="fieldName">The name of the field to navigate to.</param>
        /// <param name="recordNumber">The record number to navigate to.</param>
        /// <param name="skip">If true, advances to the next non skip field when the navigating field is a skip field.</param>
        void GotoField(string fieldName, int recordNumber, bool skip);
    }

    /// <summary>
    /// Form labels.
    /// </summary>
    public interface IScriptLabels
    {
        /// <summary>
        /// Gets the number of labels on the current form.
        /// </summary>
        int Count { get;}
        /// <summary>
        /// Gets the label at the given index.
        /// </summary>
        /// <param name="index">The index to get.</param>
        /// <returns>The label at the given index.</returns>
        IScriptLabel this[int index] { get;}
        /// <summary>
        /// Gets the label that matches the given name.
        /// </summary>
        /// <param name="labelName">The name of the label to get.</param>
        /// <returns>The matching label.</returns>
        IScriptLabel this[string labelName] { get;}
    }

    /// <summary>
    /// A form label.
    /// </summary>
    public interface IScriptLabel
    {
        /// <summary>
        /// Gets or sets the text for this label.
        /// </summary>
        string Text { get; set;}
        /// <summary>
        /// Gets the name of this label.
        /// </summary>
        string LabelName { get;}
        /// <summary>
        /// Gets or sets the background color for this label.
        /// </summary>
        Color BackColor { get; set;}
        /// <summary>
        /// Gets or sets the foreground color for this label.
        /// </summary>
        Color ForeColor { get; set;}
        /// <summary>
        /// Gets or sets the font for this label.
        /// </summary>
        Font Font { get; set;}
    }

    /// <summary>
    /// Session related information.
    /// </summary>
    public interface IScriptWork
    {
        /// <summary>
        /// Gets or sets the index of the current document. Setting this property will navigate to the intended document.
        /// </summary>
        int CurrentDocument { get; set;}
        /// <summary>
        /// Gets the total document count for the current batch.
        /// </summary>
        int DocumentCount { get;}
		/// <summary>
        /// Gets or sets the current form type.
        /// </summary>
        string CurrentFormType { get; set;}
        /// <summary>
        /// Gets the current entry mode.
        /// </summary>
        WDEEntryMode EntryMode { get;}
        /// <summary>
        /// Gets the current workflow batch name.
        /// </summary>
        string BatchName { get;}
        /// <summary>
        /// Gets the current workflow task name if applicable.
        /// </summary>
        string TaskName { get;}
        /// <summary>
        /// Gets the available forms in the current session definition.
        /// </summary>
        IScriptForms AvailableFormTypes { get;}
		/// <summary>
		/// Sets the text in the hint bar.
		/// </summary>
		string Hint { get; set; }
		/// <summary>
        /// Gets the current workflow custom parameters.
        /// </summary>
        IDictionary<string, string> CustomParameters { get;}
        /// <summary>
        /// Navigates to the beginning of the next document.
        /// </summary>
        void Next();
        /// <summary>
        /// Navigates to the beginning of the previous document.
        /// </summary>
        void Prior();
        /// <summary>
        /// Navigates to the beginning of the first document in the batch.
        /// </summary>
        void Home();
        /// <summary>
        /// Navigates to the spot the user last keyed.
        /// </summary>
        void End();
        /// <summary>
        /// Request an appropriate place to position a dialog.
        /// </summary>
        /// <param name="dialogSize">The dialog size.</param>
        /// <returns>The recommended position for the dialog.</returns>
        Point RequestDialogPos(Size dialogSize);
		/// <summary>
		/// Loads a form as a supplemental record.
		/// </summary>
		/// <param name="formType">Name of the form to use. Cannot be associated with a record of the current type.</param>
		void LoadSupplemental(string formType);
		/// <summary>
		/// Exits the supplemental record mode.
		/// </summary>
		/// <returns>True if it exits the mode successfully.</returns>
		bool ExitSupplemental();
        /// <summary>
        /// Gets the current workflow user name.
        /// </summary>
        string UserName { get; }
        /// <summary>
        /// Gets the current workflow session name.
        /// </summary>
        string SessionName { get; }
        /// <summary>
        /// Gets or sets the foreground color for the hint label .
        /// </summary>
        Color HintForeColor { get; set; }
        /// <summary>
        /// Gets or sets the background color for the hint label.
        /// </summary>
        Color HintBackColor { get; set; }
        /// <summary>
        /// Gets the total documents completed.
        /// </summary>
        int TotalDocCompleted { get; }
        /// <summary>
        /// Gets the total documents cancelled.
        /// </summary>
        int TotalDocCancelled { get; }
        /// <summary>
        /// Gets the total documents rejected.
        /// </summary>
        int TotalDocRejected { get; }
        /// <summary>
        /// Adds a reject code from script to the reject code table
        /// </summary>
        /// <param name="rejectCode">Reject Code</param>
        /// <param name="rejectDescription">Reject Description</param>
        /// <param name="requireReason">Indicates whether keyer reason is required. true/false</param>
        void AddRejectCode(string rejectCode, string rejectDescription, bool requireReason);        
    }   

    /// <summary>
    /// The forms in the current session def.
    /// </summary>
    public interface IScriptForms
    {
        /// <summary>
        /// Gets the number of form types in the current session def.
        /// </summary>
        int Count { get;}
        /// <summary>
        /// Gets the form type at the given index.
        /// </summary>
        /// <param name="index">The index to get.</param>
        /// <returns>The given form type.</returns>
        string this[int index] { get;}
    }

    /// <summary>
    /// Document related information.
    /// </summary>
    public interface IScriptDocument
    {
        /// <summary>
        /// Gets the current DCN.
        /// </summary>
        string DCN { get;}
        /// <summary>
        /// Gets the current alternate DCN.
        /// </summary>
        string AltDCN { get;}
		/// <summary>
		/// Gets the current item type.
		/// </summary>
		string ItemType { get; }
		/// <summary>
        /// Gets the current workflow UDF values for the current document.
        /// </summary>
        IDictionary<string, string> UDFS { get;}
        /// <summary>
        /// Gets the reject status for the current document.
        /// </summary>
        bool IsRejected { get; }
    }

    /// <summary>
    /// Record related information.
    /// </summary>
    public interface IScriptRecord
    {
        /// <summary>
        /// Gets the record type for this record.
        /// </summary>
        string RecType { get;}
        /// <summary>
        /// Gets this record's parent or null if there is no parent.
        /// </summary>
        IScriptRecord Parent { get; }
        /// <summary>
        /// Gets the fields in this record.
        /// </summary>
        IScriptFields Fields { get;}
        /// <summary>
        /// Gets the collection this record belongs to.
        /// </summary>
        IScriptRecords SiblingRecords { get;}
        /// <summary>
        /// Gets the child records of this record.
        /// </summary>
        IScriptRecords GetChildRecords(string recType);
		/// <summary>
		/// Gets the row status whether it is marked as deleted or not
		/// </summary>
		bool IsDeleted { get;}
		/// <summary>
		/// Gets the session id in which the row was marked as deleted
		/// </summary>
		int SessionID { get;}
		/// <summary>
		/// Marks a row as deleted(sets IsDeleted and SessionID values)
		/// </summary>
		void Delete();
		/// <summary>
		/// Rolls back a deleted row
		/// </summary>
		void RestoreDeletedRow();
        /// <summary>
        /// Gets the position or index of the non deleted record.
        /// </summary>
        int SiblingPosition { get; } 
    }

    /// <summary>
    /// The fields in a record.
    /// </summary>
    public interface IScriptFields
    {
        /// <summary>
        /// Gets the number of fields.
        /// </summary>
        int Count { get;}
        /// <summary>
        /// Gets the field at the given index.
        /// </summary>
        /// <param name="index">The field index to retrieve.</param>
        /// <returns>The field at the given index.</returns>
        IScriptField this[int index] { get;}
        /// <summary>
        /// Gets the field matching the given field name.
        /// </summary>
        /// <param name="fieldName">The field name to find.</param>
        /// <returns>The field matching the given field name.</returns>
        IScriptField this[string fieldName] { get;}
    }

    /// <summary>
    /// The record collection.
    /// </summary>
    public interface IScriptRecords
    {
        /// <summary>
        /// Gets the number of records.
        /// </summary>
        int Count { get;}
        /// <summary>
        /// Gets the record at the given index.
        /// </summary>
        /// <param name="index">The index to retrieve.</param>
        /// <returns>The record at the given index.</returns>
        IScriptRecord this[int index] { get;}
        /// <summary>
        /// Adds a new record to the end of the detail grid.
        /// </summary>
        void Append();
    }

    /// <summary>
    /// Field related information.
    /// </summary>
    public interface IScriptField
    {
        /// <summary>
        /// Gets or sets the value of this field.
        /// </summary>
        string Value { get; set;}
        /// <summary>
        /// Gets whether this field has been excluded from data entry.
        /// </summary>
        bool Exclude { get;}
        /// <summary>
        /// Gets or sets the flag description for this field. A value of null means the field is not flagged.
        /// </summary>
        string FlagDescription { get; set;}
        /// <summary>
        /// Gets or sets the custom data for this field. This information is stored in the data file but not otherwise used by WebDX.
        /// </summary>
        string CustomData { get; set;}
        /// <summary>
        /// Gets the name of this field.
        /// </summary>
        string FieldName { get;}
		/// <summary>
		/// Gets the maximum character length of the field
		/// </summary>
		int Length { get; }
		/// <summary>
		/// Gets or sets the status for when the value of this field is updated.
		/// </summary>
		WDEFieldStatus UpdateStatus { get; set; }
		/// <summary>
        /// Performs a data duplication operation on this field.
        /// </summary>
        void Dupe();
        /// <summary>
        /// Gets the revisions for this field.
        /// </summary>
        IScriptRevisions Revisions { get;}
        /// <summary>
        /// Get the value as number if the field value is a number or zero if the field value is string 
        /// </summary>
        double NumberValue { get; }
    }

    /// <summary>
    /// The revisions colletion.
    /// </summary>
    public interface IScriptRevisions
    {
        /// <summary>
        /// Gets the number of revisions.
        /// </summary>
        int Count { get;}
        /// <summary>
        /// Gets the revision at the given index.
        /// </summary>
        /// <param name="index">The index to retrieve.</param>
        /// <returns>The revision at the given index.</returns>
        IScriptRevision this[int index] { get;}
    }

    /// <summary>
    /// A field revision.
    /// </summary>
    public interface IScriptRevision
    {
        /// <summary>
        /// Gets the revision value.
        /// </summary>
        string Value { get;}
        /// <summary>
        /// Gets the FlagDescription if the field was flagged. Returns null if the field was not flagged.
        /// </summary>
        string FlagDescription { get;}
        /// <summary>
        /// Gets the field status for the revision.
        /// </summary>
        WDEFieldStatus RevisionStatus { get;}
        /// <summary>
        /// Gets the user for the revision.
        /// </summary>
        string User { get;}
        /// <summary>
        /// Gets the task for the revision.
        /// </summary>
        string Task { get;}
        /// <summary>
        /// Gets the open mode for the revision.
        /// </summary>
        WDEOpenMode SessionMode { get;}
    }

    /// <summary>
    /// Text box related information.
    /// </summary>
    public interface IScriptTextBox
    {
        /// <summary>
        /// Gets or sets the text. Data is not posted to the field until the user exits the text box.
        /// </summary>
        string Text { get; set;}
        /// <summary>
        /// Gets or sets the current selection start position.
        /// </summary>
        int SelStart { get; set;}
        /// <summary>
        /// Gets or sets the number of characters selected.
        /// </summary>
        int SelLength { get; set;}
        /// <summary>
        /// Gets or sets the text within the selected portion.
        /// </summary>
        string SelText { get; set;}
        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        Color ForeColor { get; set;}
        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        Color BackColor { get; set;}
        /// <summary>
        /// Gets or sets whether the text is bolded.
        /// </summary>
        bool Bold { get; set;}
        /// <summary>
        /// Get the value as number if the field value is a number or zero if the field value is string 
        /// </summary>
        double NumberValue { get; }
        /// <summary>
        /// Gets the textbox value without any input mask characters.
        /// </summary>
        string Value { get; set; }
    }

    /// <summary>
    /// Detail grid related information.
    /// </summary>
    public interface IScriptDetailGrid
    {
        /// <summary>
        /// Exits the current record and moves to the next record. A new record is created if needed.
        /// </summary>
        void GotoNextRecord();
        /// <summary>
        /// Exits the detail grid.
        /// </summary>
        void Exit();
        /// <summary>
        /// Indicates whether the current record is the last record in the detail grid.
        /// </summary>
        bool LastRecord { get; }
        /// <summary>
        /// Exits the current record and moves to the next record's non skip field. A new record is created if needed.
        /// </summary>
        void ReleaseRow();
    }

    /// <summary>
    /// Client plugin related methods.
    /// </summary>
    public interface IScriptClient
    {
        /// <summary>
        /// Get item by DCN. Should be implemented only in the client plugin.
        /// </summary>
        /// <param name="dcnName">DCN Name</param>
        void GetItem(string dcnName);
    }

    #endregion

    #region Event Interfaces

    /// <summary>
    /// Standard script event.
    /// </summary>
    public delegate void ScriptBaseEvent();
    /// <summary>
    /// Occurs when a character key is pressed in a text box.
    /// </summary>
    /// <param name="e">The key event information.</param>
    public delegate void ScriptTextBoxKeyPressEvent(TextBoxKeyPressEventArgs e);
    /// <summary>
    /// Occurs when a text box is exited.
    /// </summary>
    /// <param name="e">The exit event information.</param>
    public delegate void ScriptTextBoxExitEvent(ExitEventArgs e);
    /// <summary>
    /// Occurs when any key is pressed.
    /// </summary>
    /// <param name="e">The key event information.</param>
    public delegate void ScriptProjectKeyPressEvent(ProjectKeyEventArgs e);
    /// <summary>
    /// Occurs when a document is rejected.
    /// </summary>
    /// <param name="e">Document reject event args.</param>
    public delegate void ScriptDocumentRejectEvent(RejectEventArgs e);
    /// <summary>
    /// Occurs when a batch is started.
    /// </summary>
    /// <param name="e">Batch start information.</param>
    public delegate void ScriptStartWorkEvent(StartWorkEventArgs e);
    /// <summary>
    /// Occurs when a batch is completed.
    /// </summary>
    /// <param name="e">Batch end information.</param>
    public delegate void ScriptEndWorkEvent(EndWorkEventArgs e);

    /// <summary>
    /// Text box exit event information.
    /// </summary>
    public class ExitEventArgs : MarshalByRefObject
    {
        private bool _advancing;

        /// <summary>
        /// Creates an instance of the ExitEventArgs class, specifying whether the keying flow is advancing or not.
        /// </summary>
        /// <param name="advancing">True if the keyer is going forward in the keying order.</param>
        public ExitEventArgs(bool advancing)
            : base()
        {
            _advancing = advancing;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Gets a value indicating whether the keyer is going forward in the keying order.
        /// </summary>
        public bool Advancing
        {
            get { return _advancing; }
        }
    }

    /// <summary>
    /// Key press information.
    /// </summary>
    public class ProjectKeyEventArgs : MarshalByRefObject
    {
        private bool _alt;
        private bool _control;
        private bool _shift;
        private Keys _keyCode;
        private bool _handled;

        /// <summary>
        /// Creates a new instance of the ProjectKeyEventArgs class.
        /// </summary>
        /// <param name="alt">True if the Alt key is down.</param>
        /// <param name="control">True if the Control key is down.</param>
        /// <param name="shift">True if the Shift key is down.</param>
        /// <param name="keyCode">The key code for the key pressed.</param>
        public ProjectKeyEventArgs(bool alt, bool control, bool shift, Keys keyCode)
            : base()
        {
            _alt = alt;
            _control = control;
            _shift = shift;
            _keyCode = keyCode;
            _handled = false;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Gets or sets a value indicating the Alt key is down.
        /// </summary>
        public bool Alt
        {
            get { return _alt; }
            set { _alt = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating the Control key is down.
        /// </summary>
        public bool Control
        {
            get { return _control; }
            set { _control = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating the Shift key is down.
        /// </summary>
        public bool Shift
        {
            get { return _shift; }
            set { _shift = value; }
        }

        /// <summary>
        /// Gets or sets the key that was pressed.
        /// </summary>
        public Keys KeyCode
        {
            get { return _keyCode; }
            set { _keyCode = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the key press was handled. True indicates that WebDX should no longer process this key event.
        /// </summary>
        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }
    }

    /// <summary>
    /// TextBox key press information.
    /// </summary>
    public class TextBoxKeyPressEventArgs : MarshalByRefObject
    {
        private bool _handled;
        private char _keyChar;

        /// <summary>
        /// Creates a new instance of the TextBoxKeyPressEventArgs class.
        /// </summary>
        /// <param name="keyChar">The character pressed.</param>
        public TextBoxKeyPressEventArgs(char keyChar)
        {
            _keyChar = keyChar;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the key event should not be processed by WebDX. Set to True to indicate that WebDX should not process the key event.
        /// </summary>
        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }

        /// <summary>
        /// Gets or sets the character keyed.
        /// </summary>
        public char KeyChar
        {
            get { return _keyChar; }
            set { _keyChar = value; }
        }
    }

    /// <summary>
    /// Document reject information.
    /// </summary>
    public class RejectEventArgs : MarshalByRefObject
    {
        private string _rejectCode;
        private string _rejectDescription;

        /// <summary>
        /// Creates a new instance of the RejectEventArgs class.
        /// </summary>
        /// <param name="rejectCode">The reject code selected for this reject.</param>
        /// <param name="rejectDescription">The reject description for this reject.</param>
        public RejectEventArgs(string rejectCode, string rejectDescription)
            : base()
        {
            _rejectCode = rejectCode;
            _rejectDescription = rejectDescription;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Gets the reject code.
        /// </summary>
        public string RejectCode
        {
            get { return _rejectCode; }
        }

        /// <summary>
        /// Gets the reject description.
        /// </summary>
        public string RejectDescription
        {
            get { return _rejectDescription; }
        }
    }

    /// <summary>
    /// Batch start information.
    /// </summary>
    public class StartWorkEventArgs : MarshalByRefObject
    {
        private StartWorkType _batchGetType;

        /// <summary>
        /// Creates a new instance of the StartWorkEventArgs class.
        /// </summary>
        /// <param name="batchGetType">The type of Get performed.</param>
        public StartWorkEventArgs(StartWorkType batchGetType)
            : base()
        {
            _batchGetType = batchGetType;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Gets the type of workflow Get performed.
        /// </summary>
        public StartWorkType BatchGetType
        {
            get { return _batchGetType; }
        }
    }

    /// <summary>
    /// The type of workflow Get performed.
    /// </summary>
    public enum StartWorkType
    {
        /// <summary>
        /// A multi-get was performed.
        /// </summary>
        MultiGet,
        /// <summary>
        /// A specific get was performed.
        /// </summary>
        SpecificGet
    }
    /// <summary>
    /// Rotates the image to the specified orientation.
    /// </summary>
    public enum Orientations
    {
        /// <summary>
        /// Does not rotate the image.
        /// </summary>
        NORMAL,
        /// <summary>
        /// Rotate the image to left.
        /// </summary>
        LEFT,
        /// <summary>
        /// Rotate the image to right.
        /// </summary>
        RIGHT,
        /// <summary>
        /// Rotate the image by 180 degrees.
        /// </summary>
        ONEEIGHTY
    }
    /// <summary>
    /// Scrolls the image the specified scroll display.
    /// </summary>
    public enum ScrollDisplay
    {
        /// <summary>
        /// Scrolls the image Up.
        /// </summary>
        UP,
        /// <summary>
        /// Scrolls the image to LEFT.
        /// </summary>
        LEFT,
        /// <summary>
        /// Scrolls the image to RIGHT.
        /// </summary>
        RIGHT,
        /// <summary>
        /// Scrolls the image Down.
        /// </summary>
        DOWN,
        /// <summary>
        /// Scrolls the image to Top.
        /// </summary>
        TOP,
        /// <summary>
        /// Scrolls the image to Bottom.
        /// </summary>
        BOTTOM
    }

    /// <summary>
    /// Batch end information.
    /// </summary>
    public class EndWorkEventArgs : MarshalByRefObject
    {
        private EndWorkReason _reason;
        private string _rejectCode;
        private string _rejectDescription;

        /// <summary>
        /// Creates a new instance of the EndWorkEventArgs class.
        /// </summary>
        /// <param name="reason">The reason that work is ending.</param>
        /// <param name="rejectCode">The reject code if reason is Reject. Null otherwise.</param>
        /// <param name="rejectDescription">The reject description if reason is Reject. Null otherwise.</param>
        public EndWorkEventArgs(EndWorkReason reason, string rejectCode, string rejectDescription)
            : base()
        {
            _reason = reason;
            if (_reason == EndWorkReason.Reject)
            {
                _rejectCode = rejectCode;
                _rejectDescription = rejectDescription;
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Gets the reason that work is ending.
        /// </summary>
        public EndWorkReason Reason
        {
            get { return _reason; }
        }

        /// <summary>
        /// Gets the reject code if Reason is Reject. Null otherwise.
        /// </summary>
        public string RejectCode
        {
            get { return _rejectCode; }
        }

        /// <summary>
        /// Gets the reject description if Reason is Reject. Null otherwise.
        /// </summary>
        public string RejectDescription
        {
            get { return _rejectDescription; }
        }
    }

    /// <summary>
    /// The reason work is ending.
    /// </summary>
    public enum EndWorkReason
    {
        /// <summary>
        /// Work is being put normally.
        /// </summary>
        Normal,
        /// <summary>
        /// The batch is being rejected.
        /// </summary>
        Reject,
        /// <summary>
        /// The batch is being cancelled.
        /// </summary>
        Cancel
    }

    /// <summary>
    /// Used to wire up text box events.
    /// </summary>
    public interface IScriptTextBoxEvents
    {
        event ScriptBaseEvent OnEnter;
        event ScriptTextBoxKeyPressEvent OnKeyPress;
        event ScriptBaseEvent OnValidate;
        event ScriptTextBoxExitEvent OnExit;
    }

    /// <summary>
    /// Used to wire up detail grid events.
    /// </summary>
    public interface IScriptDetailGridEvents
    {
        event ScriptBaseEvent OnEnter;
        event ScriptBaseEvent OnExit;
    }

    /// <summary>
    /// Used to wire up form events.
    /// </summary>
    public interface IScriptFormEvents
    {
        event ScriptBaseEvent OnEnter;
        event ScriptBaseEvent OnExit;
    }

    /// <summary>
    /// Used to wire up project events.
    /// </summary>
    public interface IScriptProjectEvents
    {
        event ScriptProjectKeyPressEvent OnKeyPress;
        event ScriptBaseEvent OnDocumentUnreject;
        event ScriptDocumentRejectEvent OnDocumentReject;
        event ScriptStartWorkEvent OnStartWork;
        event ScriptEndWorkEvent OnEndWork;
    }

    /// <summary>
    /// Used to wire up image events.
    /// </summary>
    public interface IScriptImageEvents
    {
        event ScriptBaseEvent OnPageChange;
    }

    #endregion

    #region Edit Interfaces

    /// <summary>
    /// Allows definition of a re-usable edit that integrates with WebDX Studio.
    /// </summary>
    public interface IScriptEdit
    {
        /// <summary>
        /// Shows the edit's configuration dialog, allowing the edit to be configured from within WebDX Studio.
        /// </summary>
        /// <param name="project">The project being configured.</param>
        /// <returns>True if the Ok button was clicked. False if the cancel button was clicked.</returns>
        bool ShowConfigDialog(IWDEProject project);
        /// <summary>
        /// Allows the edit to read its configuration from the Project XML.
        /// </summary>
        /// <param name="xml">The xml to read.</param>
        void ReadFromXml(string xml);
        /// <summary>
        /// Allows the edit to write its configuration to the Project XML.
        /// </summary>
        string WriteToXml();
        /// <summary>
        /// Runs the edit in the client.
        /// </summary>
        /// <param name="message">The message configured for this edit.</param>
        void Execute(string message);        
    }

    #endregion

    #region Exceptions
    /// <summary>
    /// Shows an error message and prevents the user from exiting the current textbox.
    /// </summary>
    [Serializable]
    public class ScriptException : Exception
    {
        /// <summary>
        /// Show the given error message and prevent the user from exiting the current textbox.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public ScriptException(string message)
            : base(message)
        {
        }

        public ScriptException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// Shows a warning message optionally allowing the user to fix the problem before moving on.
    /// </summary>
    [Serializable]
    public class ScriptWarning : Exception
    {
        private bool _retry;

        /// <summary>
        /// Show the given error message and optionally allow the user to fix the problem before moving on.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="retry">Allow the user to retry editing the field before moving on.</param>
        public ScriptWarning(string message, bool retry)
            : base(message)
        {
            _retry = retry;
        }

        public ScriptWarning(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            _retry = info.GetBoolean("Retry");
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Retry", _retry);
        }

        /// <summary>
        /// True if the user should be allowed to edit the field before moving on.
        /// </summary>
        public bool Retry
        {
            get { return _retry; }
        }
    }

    #endregion
}