using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Xml;
using System.Xml.Schema;

namespace WebDX.Api
{
	/// <summary>
	/// The open mode for a data file session.
	/// </summary>
	/// <remarks> Describes the type of work being done during the session.</remarks>
    [OutputPrefix("ds", "1.0.0.0", "1.4.0.29", "WDEOpenMode")]
	public enum WDEOpenMode 
	{
		/// <summary>
		/// The data file is being created initially.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Create", "WDEOpenMode")]
		Create,
		/// <summary>
		/// The data file is being opened by the WebDX Client for data entry.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Entry", "WDEOpenMode")]
		Entry,
		/// <summary>
		/// The data file is being opened by the WebDX Client for verification.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Verify", "WDEOpenMode")]
		Verify,
		/// <summary>
		/// The data file is being opened by the WebDX Client for flag review.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Review", "WDEOpenMode")]
		Review,
		/// <summary>
		/// The data file is being opened by a program for automatic data validation.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Validate", "WDEOpenMode")]
		Validate,
		/// <summary>
		/// The data file is being opened to be edited.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", null, "Edit", "WDEOpenMode")]
		Edit,
		/// <summary>
		/// The data file is being opened to be output to the customer.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", null, "Output", "WDEOpenMode")]
		Output,
		/// <summary>
		/// The data file is being opened by the WebDX Client in QIVerify mode.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", null, "QI", "WDEOpenMode")]
		QI,
		/// <summary>
		/// The data file is being resumed by the WebDX Client.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", null, "Resume", "WDEOpenMode")]
		Resume,
		/// <summary>
		/// The data file is being opened by the WebDX Client in Double Entry mode.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", null, "DblEntry", "WDEOpenMode")]
		DblEntry,
		/// <summary>
		/// The data file is being opened by the WebDX Client in Double Entry/Compare mode.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", null, "Compare", "WDEOpenMode")]
		Compare,
		/// <summary>
		/// The data file is being opened by the WebDX Client in Character Repair mode.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", null, "CharRepair", "WDEOpenMode")]
		CharRepair,
		/// <summary>
		/// The data file is being opened by the QISampler program.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", null, "QISelect", "WDEOpenMode")]
		QISelect,
		/// <summary>
		/// The data file is being opened by the WebDX Client in Focus Audit mode.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", null, "FocusAudit", "WDEOpenMode")]
		FocusAudit
	};
	/// <summary>
	/// The session status. Determines the status of a document or batch. Used by the WebDX Client in determining work to be processed.
	/// </summary>
    [OutputPrefix("ss", "1.0.0.0", "1.4.0.29", "WDESessionStatus")]
	public enum WDESessionStatus 
	{
		/// <summary>
		/// The batch or document has not been processed.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "None", "WDESessionStatus")]
		None,
		/// <summary>
		/// The batch or document has been processed.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Completed", "WDESessionStatus")]
		Completed,
		/// <summary>
		/// The batch or document was cancelled by the user.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Canceled", "WDESessionStatus")]
		Canceled,
		/// <summary>
		/// The batch or document was rejected by the user.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Rejected", "WDESessionStatus")]
		Rejected
	};
	/// <summary>
	/// Field flags. Used by the WebDX Client.
	/// </summary>
    [OutputPrefix("ff", "1.0.0.0", "1.4.0.29", "WDEFieldFlags")]
	public enum WDEFieldFlags 
	{
		/// <summary>
		/// The field has not been processed.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "None", "WDEFieldFlags")]
		None,
		/// <summary>
		/// The field is currently being processed.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Active", "WDEFieldFlags")]
		Active,
		/// <summary>
		/// The field has been plugged by program code.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Plugged", "WDEFieldFlags")]
		Plugged,
		/// <summary>
		/// The field has been processed.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Completed", "WDEFieldFlags")]
		Completed
	};

	/// <summary>
	/// Used internally by the WebDX Client. Not intended for use by your code.
	/// </summary>
	[Flags]
    [OutputPrefix("mf", "1.0.0.0", "1.4.0.29", "WDEMiscFlags")]
	public enum WDEMiscFlags {
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "None", "WDEMiscFlags")]
        None = 0,
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Keyable", "WDEMiscFlags")]
        Keyable = 1,
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Verify", "WDEMiscFlags")]
        Verify = 2};
	/// <summary>
	/// The field status. Represents the source of the data in a field.
	/// </summary>
    [OutputPrefix("fs", "1.0.0.0", "1.4.0.29", "WDEFieldStatus")]
	public enum WDEFieldStatus 
	{
		/// <summary>
		/// The field has not been processed.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "None", "WDEFieldStatus")]
		None,
		/// <summary>
		/// The field has been keyed by a human keyer using the WebDX Client.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Keyed", "WDEFieldStatus")]
		Keyed,
		/// <summary>
		/// The field value has been plugged by program code.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Plugged", "WDEFieldStatus")]
		Plugged,
		/// <summary>
		/// The field value has been verified by a human keyer using the WebDX Client.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Verified", "WDEFieldStatus")]
		Verified,
		/// <summary>
		/// The field value has been reviewed by a human keyer using the WebDX Client.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Reviewed", "WDEFieldStatus")]
		Reviewed,
		/// <summary>
		/// The field has been flagged for review.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Flagged", "WDEFieldStatus")]
		Flagged,
		/// <summary>
		/// The field has been validated by program code external to the WebDX Client.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Validated", "WDEFieldStatus")]
		Validated,
		/// <summary>
		/// The field has been keyed by a human keyer using the WebDX Client in Double Entry mode.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "DblEntered", "WDEFieldStatus")]
		DblEntered,
		/// <summary>
		/// The field has been keyed by a human keyer using the WebDX Client in Double Entry/Compare mode.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Compared", "WDEFieldStatus")]
		Compared,
		/// <summary>
		/// The field has been duplicated by the duplication function in the WebDX Client.
		/// </summary>
        [VersionEnumFilterAttribute("1.0.0.0", "1.4.0.29", "Duped", "WDEFieldStatus")]
		Duped
	};

	public interface IWDEXmlPersist
	{
		void ReadFromXml(XmlTextReader XmlReader);
		void WriteToXml(XmlTextWriter XmlWriter);
	}

    public interface IWDEXmlPersistFlat
    {
        void WriteToXmlFlat(XmlTextWriter xmlWriter);
    }

#if DEBUG
	public interface IWDECollection
#else
	internal interface IWDECollection
#endif
	{
		int GetIndex(object Item);
		void SetIndex(object Item, int NewIndex);
		void RemoveAt(int Index);
	}

	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("0D22B25D-11A3-409a-8806-2A5E4D08E9DB")]
	public interface IWDERecordsOwner
	{
		IWDEDocument Document {get;}
		IWDERecord Record {get;}
	}

	/// <summary>
	/// Manages WebDX data files.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("7E118965-BED7-4449-983B-0B94382DA804")]
	public interface IWDEDataSet
	{
		/// <summary>
		/// Gets the <see cref="IWDEDocumentDefs"/> collection in the project assigned to <see cref="Project"/>.
		/// </summary>
		IWDEDocumentDefs DocumentDefs {get;}
		/// <summary>
		/// Gets the <see cref="IWDEDocuments"/> collection.
		/// </summary>
		IWDEDocuments Documents {get;}
		/// <summary>
		/// Gets BOF status. True if <see cref="Current"/> points to the first document.
		/// </summary>
		bool BOF {get;}
		/// <summary>
		/// Gets EOF status. True if <see cref="Next"/> has moved past the end of the document list.
		/// </summary>
		bool EOF {get;}
		/// <summary>
		/// Gets the number of documents.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the current <see cref="IWDEDocument"/>. <see cref="First"/>, <see cref="Last"/>, <see cref="Prior"/> and <see cref="Next"/> affect this property.
		/// </summary>
		IWDEDocument Current {get;}
		/// <summary>
		/// Gets or sets the AltDCN property from <see cref="Current"/>.
		/// </summary>
		string AltDCN {get; set;}
		/// <summary>
		/// Gets or sets the DCN property from <see cref="Current"/>.
		/// </summary>
		string DCN {get; set;}
		/// <summary>
		/// Gets the DocType property from <see cref="Current"/>.
		/// </summary>
		string DocType {get;}
		/// <summary>
		/// Gets the Images collection from <see cref="Current"/>.
		/// </summary>
		IWDEImages Images {get;}
		/// <summary>
		/// Gets the Records collection from <see cref="Current"/>.
		/// </summary>
		IWDERecords Records {get;}
		/// <summary>
		/// Gets the DocSessions collection from <see cref="Current"/>.
		/// </summary>
		IWDEDocSessions DocSessions {get;}
		/// <summary>
		/// Gets or sets the RejectCode property from <see cref="Current"/>.
		/// </summary>
		string RejectCode {get; set;}
		/// <summary>
		/// Gets or sets the RejectDescription property from <see cref="Current"/>.
		/// </summary>
		string RejectDescription {get; set;}
		/// <summary>
		/// Gets the Status property from <see cref="Current"/>.
		/// </summary>
		WDESessionStatus Status {get;}
		/// <summary>
		/// Gets or sets the project object to use when editing data.
		/// </summary>
		IWDEProject Project {get; set;}
		/// <summary>
		/// Gets the <see cref="IWDESessions"/> collection.
		/// </summary>
		IWDESessions Sessions {get;}
		/// <summary>
		/// Gets version information.
		/// </summary>
		string About {get; set;}
		/// <summary>
		/// Gets or sets the filename to use when opening and closing.
		/// </summary>
		string FileName {get; set;}
		/// <summary>
		/// Gets or sets the project filename to use when editing data.
		/// </summary>
		string ProjectFile {get; set;}
		/// <summary>
		/// Gets or sets an indicator to save field flags when saving.
		/// </summary>
		bool PersistFlags {get; set;}
		/// <summary>
		/// Gets whether api is serving all rows or only non-deleted rows
		/// </summary>
		bool DisplayDeletedRows { get;}
		/// <summary>
		/// Clears all data and objects.
		/// </summary>
		void Clear();
		/// <summary>
		/// Clears all data but leaves the document structure intact.
		/// </summary>
		void ClearData();
		/// <summary>
		/// Initializes the DataSet and creates initial session information. Requires a project to be assigned.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		void CreateData(string User, string Task, WDEOpenMode Mode);
		/// <summary>
		/// Initializes the DataSet and creates initial session information. Requires a <see cref="Project"/> to be assigned.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="Location">The location for the new session.</param>
		void CreateData(string User, string Task, WDEOpenMode Mode, string Location);
		/// <summary>
		/// Loads the file in the FileName property and initializes a new session.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		void Open(string User, string Task, WDEOpenMode Mode);
		/// <summary>
		/// Loads the file in the FileName property and initializes a new session.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="Location">The location for the new session.</param>
		void Open(string User, string Task, WDEOpenMode Mode, string Location);
		/// <summary>
		/// Saves data to the file in the FileName property and clears all objects and data.
		/// </summary>
		void Close();
		/// <summary>
		/// Loads the file in the FileName property and initializes a new session. Updates the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="FileName">The file to open.</param>
		void LoadFromFile(string User, string Task, WDEOpenMode Mode, string FileName);
		/// <summary>
		/// Loads the file in the FileName property and initializes a new session. Updates the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="Location">The location for the new session.</param>
		/// <param name="FileName">The file to open.</param>
		void LoadFromFile(string User, string Task, WDEOpenMode Mode, string Location, string FileName);
		/// <summary>
		/// Loads from the given stream and initializes a new session. Clears the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="aStream">The COM stream to load from.</param>
		void LoadFromStream(string User, string Task, WDEOpenMode Mode, System.Runtime.InteropServices.ComTypes.IStream aStream);
		/// <summary>
		/// Loads from the given stream and initializes a new session. Clears the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="aStream">The .Net stream to load from.</param>
		void LoadFromStream(string User, string Task, WDEOpenMode Mode, System.IO.Stream aStream);
		/// <summary>
		/// Loads from the given stream and initializes a new session. Clears the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="Location">The location for the new session.</param>
		/// <param name="aStream">The COM stream to load from.</param>
		void LoadFromStream(string User, string Task, WDEOpenMode Mode, string Location, System.Runtime.InteropServices.ComTypes.IStream aStream);
		/// <summary>
		/// Loads from the given stream and initializes a new session. Clears the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="Location">The location for the new session.</param>
		/// <param name="aStream">The .Net stream to load from.</param>
		void LoadFromStream(string User, string Task, WDEOpenMode Mode, string Location, System.IO.Stream aStream);
		/// <summary>
		/// Loads from the given byte array and initializes a new session. Clears the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="Location">The location for the new session.</param>
		/// <param name="Bytes">The byte array to load from.</param>
		void LoadFromBytes(string User, string Task, WDEOpenMode Mode, string Location, byte[] Bytes);
		/// <summary>
		/// Loads from the given byte array and initializes a new session. Clears the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="Bytes">The byte array to load from.</param>
		void LoadFromBytes(string User, string Task, WDEOpenMode Mode, byte[] Bytes);

		/// <summary>
		/// Saves to the given file name. Does not update the FileName property.
		/// </summary>
		/// <param name="FileName">The file to save to.</param>
		void SaveToFile(string FileName);
		/// <summary>
		/// Saves to the given stream.
		/// </summary>
		/// <param name="aStream">The COM stream to save to.</param>
		void SaveToStream(System.Runtime.InteropServices.ComTypes.IStream aStream);
		/// <summary>
		/// Saves to the given stream.
		/// </summary>
		/// <param name="aStream">The .Net stream to save to.</param>
		void SaveToStream(System.IO.Stream aStream);
		/// <summary>
		/// Saves to a byte array.
		/// </summary>
		/// <returns>The byte array containing the saved data.</returns>
		byte[] SaveToBytes();

		/// <summary>
		/// Moves <see cref="Current"/> to the first document in the <see cref="Documents"/> collection.
		/// </summary>
		void First();
		/// <summary>
		/// Moves <see cref="Current"/> to the last document in the <see cref="Documents"/> collection.
		/// </summary>
		void Last();
		/// <summary>
		/// Moves <see cref="Current"/> to the next document in the <see cref="Documents"/> collection.
		/// </summary>
		void Next();
		/// <summary>
		/// Moves <see cref="Current"/> to the previous document in the <see cref="Documents"/> collection.
		/// </summary>
		void Prior();

		/// <summary>
		/// Appends a new document to the <see cref="Documents"/> collection.
		/// </summary>
		/// <param name="DocType">The DocType for the new document. Must match a DocType in the project.</param>
		/// <returns>The newly appended document.</returns>
		IWDEDocument Append(string DocType);
		/// <summary>
		/// Appends a new document to the <see cref="Documents"/> collection and copies data from an existing document.
		/// </summary>
		/// <param name="Doc">Document to copy.</param>
		/// <returns>The newly appended document.</returns>
		IWDEDocument Append(IWDEDocument Doc);
		/// <summary>
		/// Inserts a document into the <see cref="Documents"/> collection. The document is inserted in the position Current points to.
		/// </summary>
		/// <param name="DocType">The DocType for the new document. Must match a DocType in the project.</param>
		/// <returns>The newly inserted document.</returns>
		IWDEDocument Insert(string DocType);
		/// <summary>
		/// Inserts a document into the <see cref="Documents"/> collection at the given index.
		/// </summary>
		/// <param name="Index">The position to insert into.</param>
		/// <param name="DocType">The DocType for the new document. Must match a DocType in the <see cref="Project"/>.</param>
		/// <returns>The newly inserted document.</returns>
		IWDEDocument Insert(int Index, string DocType);
		/// <summary>
		/// Delete the current document from the <see cref="Documents"/> collection.
		/// </summary>
		void Delete();

		/// <summary>
		/// Loads a document from the given file and appends it to the <see cref="Documents"/> collection.
		/// </summary>
		/// <param name="FileName">The file to load from.</param>
		void LoadDocumentFromFile(string FileName);
		/// <summary>
		/// Loads a document from the given stream and appends it to the <see cref="Documents"/> collection.
		/// </summary>
		/// <param name="aStream">The COM stream to load from.</param>
		void LoadDocumentFromStream(System.Runtime.InteropServices.ComTypes.IStream aStream);
		/// <summary>
		/// Loads a document from the given stream and appends it to the <see cref="Documents"/> collection.
		/// </summary>
		/// <param name="aStream">The .Net stream to load from.</param>
		void LoadDocumentFromStream(System.IO.Stream aStream);
		/// <summary>
		/// Saves the current document to the given file name.
		/// </summary>
		/// <param name="FileName">The file name to save to.</param>
		void SaveDocumentToFile(string FileName);
		/// <summary>
		/// Saves the current document to the given stream.
		/// </summary>
		/// <param name="aStream">The COM stream to save to.</param>
		void SaveDocumentToStream(System.Runtime.InteropServices.ComTypes.IStream aStream);
		/// <summary>
		/// Saves the current document to the given stream.
		/// </summary>
		/// <param name="aStream">The .Net stream to save to.</param>
		void SaveDocumentToStream(System.IO.Stream aStream);
		/// <summary>
		/// Loads a document from the given file and appends it to the <see cref="Documents"/> collection.
		/// </summary>
		/// <param name="FileName">The file to load from.</param>
		/// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
		void LoadDocumentFromFile(string FileName, bool displayDeletedRows);
		/// <summary>
		/// Loads a document from the given stream and appends it to the <see cref="Documents"/> collection.
		/// </summary>
		/// <param name="aStream">The COM stream to load from.</param>
		/// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
		void LoadDocumentFromStream(System.Runtime.InteropServices.ComTypes.IStream aStream, bool displayDeletedRows);
		/// <summary>
		/// Loads a document from the given stream and appends it to the <see cref="Documents"/> collection.
		/// </summary>
		/// <param name="aStream">The .Net stream to load from.</param>
		/// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
		void LoadDocumentFromStream(System.IO.Stream aStream, bool displayDeletedRows);
		/// <summary>
		/// Loads the file in the FileName property and initializes a new session. Updates the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="FileName">The file to open.</param>
		/// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
		void LoadFromFile(string User, string Task, WDEOpenMode Mode, string FileName, bool displayDeletedRows);
		/// <summary>
		/// Loads the file in the FileName property and initializes a new session. Updates the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="Location">The location for the new session.</param>
		/// <param name="FileName">The file to open.</param>
		/// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
		void LoadFromFile(string User, string Task, WDEOpenMode Mode, string Location, string FileName, bool displayDeletedRows);
		/// <summary>
		/// Loads from the given stream and initializes a new session. Clears the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="aStream">The COM stream to load from.</param>
		/// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
		void LoadFromStream(string User, string Task, WDEOpenMode Mode, System.Runtime.InteropServices.ComTypes.IStream aStream, bool displayDeletedRows);
		/// <summary>
		/// Loads from the given stream and initializes a new session. Clears the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="aStream">The .Net stream to load from.</param>
		/// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
		void LoadFromStream(string User, string Task, WDEOpenMode Mode, System.IO.Stream aStream, bool displayDeletedRows);
		/// <summary>
		/// Loads from the given stream and initializes a new session. Clears the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="Location">The location for the new session.</param>
		/// <param name="aStream">The COM stream to load from.</param>
		/// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
		void LoadFromStream(string User, string Task, WDEOpenMode Mode, string Location, System.Runtime.InteropServices.ComTypes.IStream aStream, bool displayDeletedRows);
		/// <summary>
		/// Loads from the given stream and initializes a new session. Clears the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="Location">The location for the new session.</param>
		/// <param name="aStream">The .Net stream to load from.</param>
		/// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
		void LoadFromStream(string User, string Task, WDEOpenMode Mode, string Location, System.IO.Stream aStream, bool displayDeletedRows);
		/// <summary>
		/// Loads from the given byte array and initializes a new session. Clears the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="Bytes">The byte array to load from.</param>
		/// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
		void LoadFromBytes(string User, string Task, WDEOpenMode Mode, byte[] Bytes, bool displayDeletedRows);
		/// <summary>
		/// Loads from the given byte array and initializes a new session. Clears the FileName property.
		/// </summary>
		/// <param name="User">The user name for the new session.</param>
		/// <param name="Task">The task name for the new session.</param>
		/// <param name="Mode">The open mode for the new session.</param>
		/// <param name="Location">The location for the new session.</param>
		/// <param name="Bytes">The byte array to load from.</param>
		/// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
		void LoadFromBytes(string User, string Task, WDEOpenMode Mode, string Location, byte[] Bytes, bool displayDeletedRows);

	}

    /// <summary>
    /// Manages WebDX data files.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("AEB9455C-3771-4ddc-99CE-9D8A7A7DB928")]
    public interface IWDEDataSet_R1 : IWDEDataSet
    {
        void MergeDocumentFromStream(System.IO.Stream aStream);
		void MergeDocumentFromStream(System.IO.Stream aStream, bool displayDeletedRows);
    }

    /// <summary>
    /// Manages WebDX data files.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("834ff30c-86f2-4a1b-91d4-334205a72ae2")]
    public interface IWDEDataSet_R2 : IWDEDataSet_R1
    {
        #region IWDEDataSet_R2[NewStuff]
		string DataLossErrors { get; }

        void SaveToFile(string FileName, string FileVersion);
        void SaveToStream(System.IO.Stream aStream, string FileVersion);
        void SaveToStream(System.Runtime.InteropServices.ComTypes.IStream aStream, string FileVersion);
        byte[] SaveToBytes(string FileVersion);
        void SaveDocumentToFile(string FileName, string FileVersion);
        void SaveDocumentToStream(System.IO.Stream aStream, string FileVersion);
        void SaveDocumentToStream(System.Runtime.InteropServices.ComTypes.IStream aStream, string FileVersion);

        #endregion

        //Merge area
        #region IWDEDataSet
        /// <summary>
        /// Gets the <see cref="IWDEDocumentDefs"/> collection in the project assigned to <see cref="Project"/>.
        /// </summary>
        IWDEDocumentDefs DocumentDefs { get; }
        /// <summary>
        /// Gets the <see cref="IWDEDocuments"/> collection.
        /// </summary>
        IWDEDocuments Documents { get; }
        /// <summary>
        /// Gets BOF status. True if <see cref="Current"/> points to the first document.
        /// </summary>
        bool BOF { get; }
        /// <summary>
        /// Gets EOF status. True if <see cref="Next"/> has moved past the end of the document list.
        /// </summary>
        bool EOF { get; }
        /// <summary>
        /// Gets the number of documents.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets the current <see cref="IWDEDocument"/>. <see cref="First"/>, <see cref="Last"/>, <see cref="Prior"/> and <see cref="Next"/> affect this property.
        /// </summary>
        IWDEDocument Current { get; }
        /// <summary>
        /// Gets or sets the AltDCN property from <see cref="Current"/>.
        /// </summary>
        string AltDCN { get; set; }
        /// <summary>
        /// Gets or sets the DCN property from <see cref="Current"/>.
        /// </summary>
        string DCN { get; set; }
        /// <summary>
        /// Gets the DocType property from <see cref="Current"/>.
        /// </summary>
        string DocType { get; }
        /// <summary>
        /// Gets the Images collection from <see cref="Current"/>.
        /// </summary>
        IWDEImages Images { get; }
        /// <summary>
        /// Gets the Records collection from <see cref="Current"/>.
        /// </summary>
        IWDERecords Records { get; }
        /// <summary>
        /// Gets the DocSessions collection from <see cref="Current"/>.
        /// </summary>
        IWDEDocSessions DocSessions { get; }
        /// <summary>
        /// Gets or sets the RejectCode property from <see cref="Current"/>.
        /// </summary>
        string RejectCode { get; set; }
        /// <summary>
        /// Gets or sets the RejectDescription property from <see cref="Current"/>.
        /// </summary>
        string RejectDescription { get; set; }
        /// <summary>
        /// Gets the Status property from <see cref="Current"/>.
        /// </summary>
        WDESessionStatus Status { get; }
        /// <summary>
        /// Gets or sets the project object to use when editing data.
        /// </summary>
        IWDEProject Project { get; set; }
        /// <summary>
        /// Gets the <see cref="IWDESessions"/> collection.
        /// </summary>
        IWDESessions Sessions { get; }
        /// <summary>
        /// Gets version information.
        /// </summary>
        string About { get; set; }
        /// <summary>
        /// Gets or sets the filename to use when opening and closing.
        /// </summary>
        string FileName { get; set; }
        /// <summary>
        /// Gets or sets the project filename to use when editing data.
        /// </summary>
        string ProjectFile { get; set; }
        /// <summary>
        /// Gets or sets an indicator to save field flags when saving.
        /// </summary>
        bool PersistFlags { get; set; }
        /// <summary>
        /// Gets whether api is serving all rows or only non-deleted rows
        /// </summary>
        bool DisplayDeletedRows { get; }
        /// <summary>
        /// Clears all data and objects.
        /// </summary>
        void Clear();
        /// <summary>
        /// Clears all data but leaves the document structure intact.
        /// </summary>
        void ClearData();
        /// <summary>
        /// Initializes the DataSet and creates initial session information. Requires a project to be assigned.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        void CreateData(string User, string Task, WDEOpenMode Mode);
        /// <summary>
        /// Initializes the DataSet and creates initial session information. Requires a <see cref="Project"/> to be assigned.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="Location">The location for the new session.</param>
        void CreateData(string User, string Task, WDEOpenMode Mode, string Location);
        /// <summary>
        /// Loads the file in the FileName property and initializes a new session.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        void Open(string User, string Task, WDEOpenMode Mode);
        /// <summary>
        /// Loads the file in the FileName property and initializes a new session.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="Location">The location for the new session.</param>
        void Open(string User, string Task, WDEOpenMode Mode, string Location);
        /// <summary>
        /// Saves data to the file in the FileName property and clears all objects and data.
        /// </summary>
        void Close();
        /// <summary>
        /// Loads the file in the FileName property and initializes a new session. Updates the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="FileName">The file to open.</param>
        void LoadFromFile(string User, string Task, WDEOpenMode Mode, string FileName);
        /// <summary>
        /// Loads the file in the FileName property and initializes a new session. Updates the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="Location">The location for the new session.</param>
        /// <param name="FileName">The file to open.</param>
        void LoadFromFile(string User, string Task, WDEOpenMode Mode, string Location, string FileName);
        /// <summary>
        /// Loads from the given stream and initializes a new session. Clears the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="aStream">The COM stream to load from.</param>
        void LoadFromStream(string User, string Task, WDEOpenMode Mode, System.Runtime.InteropServices.ComTypes.IStream aStream);
        /// <summary>
        /// Loads from the given stream and initializes a new session. Clears the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="aStream">The .Net stream to load from.</param>
        void LoadFromStream(string User, string Task, WDEOpenMode Mode, System.IO.Stream aStream);
        /// <summary>
        /// Loads from the given stream and initializes a new session. Clears the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="Location">The location for the new session.</param>
        /// <param name="aStream">The COM stream to load from.</param>
        void LoadFromStream(string User, string Task, WDEOpenMode Mode, string Location, System.Runtime.InteropServices.ComTypes.IStream aStream);
        /// <summary>
        /// Loads from the given stream and initializes a new session. Clears the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="Location">The location for the new session.</param>
        /// <param name="aStream">The .Net stream to load from.</param>
        void LoadFromStream(string User, string Task, WDEOpenMode Mode, string Location, System.IO.Stream aStream);
        /// <summary>
        /// Loads from the given byte array and initializes a new session. Clears the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="Location">The location for the new session.</param>
        /// <param name="Bytes">The byte array to load from.</param>
        void LoadFromBytes(string User, string Task, WDEOpenMode Mode, string Location, byte[] Bytes);
        /// <summary>
        /// Loads from the given byte array and initializes a new session. Clears the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="Bytes">The byte array to load from.</param>
        void LoadFromBytes(string User, string Task, WDEOpenMode Mode, byte[] Bytes);

        /// <summary>
        /// Saves to the given file name. Does not update the FileName property.
        /// </summary>
        /// <param name="FileName">The file to save to.</param>
        void SaveToFile(string FileName);
        /// <summary>
        /// Saves to the given stream.
        /// </summary>
        /// <param name="aStream">The COM stream to save to.</param>
        void SaveToStream(System.Runtime.InteropServices.ComTypes.IStream aStream);
        /// <summary>
        /// Saves to the given stream.
        /// </summary>
        /// <param name="aStream">The .Net stream to save to.</param>
        void SaveToStream(System.IO.Stream aStream);
        /// <summary>
        /// Saves to a byte array.
        /// </summary>
        /// <returns>The byte array containing the saved data.</returns>
        byte[] SaveToBytes();

        /// <summary>
        /// Moves <see cref="Current"/> to the first document in the <see cref="Documents"/> collection.
        /// </summary>
        void First();
        /// <summary>
        /// Moves <see cref="Current"/> to the last document in the <see cref="Documents"/> collection.
        /// </summary>
        void Last();
        /// <summary>
        /// Moves <see cref="Current"/> to the next document in the <see cref="Documents"/> collection.
        /// </summary>
        void Next();
        /// <summary>
        /// Moves <see cref="Current"/> to the previous document in the <see cref="Documents"/> collection.
        /// </summary>
        void Prior();

        /// <summary>
        /// Appends a new document to the <see cref="Documents"/> collection.
        /// </summary>
        /// <param name="DocType">The DocType for the new document. Must match a DocType in the project.</param>
        /// <returns>The newly appended document.</returns>
        IWDEDocument Append(string DocType);
        /// <summary>
        /// Appends a new document to the <see cref="Documents"/> collection and copies data from an existing document.
        /// </summary>
        /// <param name="Doc">Document to copy.</param>
        /// <returns>The newly appended document.</returns>
        IWDEDocument Append(IWDEDocument Doc);
        /// <summary>
        /// Inserts a document into the <see cref="Documents"/> collection. The document is inserted in the position Current points to.
        /// </summary>
        /// <param name="DocType">The DocType for the new document. Must match a DocType in the project.</param>
        /// <returns>The newly inserted document.</returns>
        IWDEDocument Insert(string DocType);
        /// <summary>
        /// Inserts a document into the <see cref="Documents"/> collection at the given index.
        /// </summary>
        /// <param name="Index">The position to insert into.</param>
        /// <param name="DocType">The DocType for the new document. Must match a DocType in the <see cref="Project"/>.</param>
        /// <returns>The newly inserted document.</returns>
        IWDEDocument Insert(int Index, string DocType);
        /// <summary>
        /// Delete the current document from the <see cref="Documents"/> collection.
        /// </summary>
        void Delete();

        /// <summary>
        /// Loads a document from the given file and appends it to the <see cref="Documents"/> collection.
        /// </summary>
        /// <param name="FileName">The file to load from.</param>
        void LoadDocumentFromFile(string FileName);
        /// <summary>
        /// Loads a document from the given stream and appends it to the <see cref="Documents"/> collection.
        /// </summary>
        /// <param name="aStream">The COM stream to load from.</param>
        void LoadDocumentFromStream(System.Runtime.InteropServices.ComTypes.IStream aStream);
        /// <summary>
        /// Loads a document from the given stream and appends it to the <see cref="Documents"/> collection.
        /// </summary>
        /// <param name="aStream">The .Net stream to load from.</param>
        void LoadDocumentFromStream(System.IO.Stream aStream);
        /// <summary>
        /// Saves the current document to the given file name.
        /// </summary>
        /// <param name="FileName">The file name to save to.</param>
        void SaveDocumentToFile(string FileName);
        /// <summary>
        /// Saves the current document to the given stream.
        /// </summary>
        /// <param name="aStream">The COM stream to save to.</param>
        void SaveDocumentToStream(System.Runtime.InteropServices.ComTypes.IStream aStream);
        /// <summary>
        /// Saves the current document to the given stream.
        /// </summary>
        /// <param name="aStream">The .Net stream to save to.</param>
        void SaveDocumentToStream(System.IO.Stream aStream);
        /// <summary>
        /// Loads a document from the given file and appends it to the <see cref="Documents"/> collection.
        /// </summary>
        /// <param name="FileName">The file to load from.</param>
        /// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
        void LoadDocumentFromFile(string FileName, bool displayDeletedRows);
        /// <summary>
        /// Loads a document from the given stream and appends it to the <see cref="Documents"/> collection.
        /// </summary>
        /// <param name="aStream">The COM stream to load from.</param>
        /// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
        void LoadDocumentFromStream(System.Runtime.InteropServices.ComTypes.IStream aStream, bool displayDeletedRows);
        /// <summary>
        /// Loads a document from the given stream and appends it to the <see cref="Documents"/> collection.
        /// </summary>
        /// <param name="aStream">The .Net stream to load from.</param>
        /// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
        void LoadDocumentFromStream(System.IO.Stream aStream, bool displayDeletedRows);
        /// <summary>
        /// Loads the file in the FileName property and initializes a new session. Updates the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="FileName">The file to open.</param>
        /// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
        void LoadFromFile(string User, string Task, WDEOpenMode Mode, string FileName, bool displayDeletedRows);
        /// <summary>
        /// Loads the file in the FileName property and initializes a new session. Updates the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="Location">The location for the new session.</param>
        /// <param name="FileName">The file to open.</param>
        /// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
        void LoadFromFile(string User, string Task, WDEOpenMode Mode, string Location, string FileName, bool displayDeletedRows);
        /// <summary>
        /// Loads from the given stream and initializes a new session. Clears the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="aStream">The COM stream to load from.</param>
        /// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
        void LoadFromStream(string User, string Task, WDEOpenMode Mode, System.Runtime.InteropServices.ComTypes.IStream aStream, bool displayDeletedRows);
        /// <summary>
        /// Loads from the given stream and initializes a new session. Clears the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="aStream">The .Net stream to load from.</param>
        /// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
        void LoadFromStream(string User, string Task, WDEOpenMode Mode, System.IO.Stream aStream, bool displayDeletedRows);
        /// <summary>
        /// Loads from the given stream and initializes a new session. Clears the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="Location">The location for the new session.</param>
        /// <param name="aStream">The COM stream to load from.</param>
        /// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
        void LoadFromStream(string User, string Task, WDEOpenMode Mode, string Location, System.Runtime.InteropServices.ComTypes.IStream aStream, bool displayDeletedRows);
        /// <summary>
        /// Loads from the given stream and initializes a new session. Clears the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="Location">The location for the new session.</param>
        /// <param name="aStream">The .Net stream to load from.</param>
        /// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
        void LoadFromStream(string User, string Task, WDEOpenMode Mode, string Location, System.IO.Stream aStream, bool displayDeletedRows);
        /// <summary>
        /// Loads from the given byte array and initializes a new session. Clears the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="Bytes">The byte array to load from.</param>
        /// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
        void LoadFromBytes(string User, string Task, WDEOpenMode Mode, byte[] Bytes, bool displayDeletedRows);
        /// <summary>
        /// Loads from the given byte array and initializes a new session. Clears the FileName property.
        /// </summary>
        /// <param name="User">The user name for the new session.</param>
        /// <param name="Task">The task name for the new session.</param>
        /// <param name="Mode">The open mode for the new session.</param>
        /// <param name="Location">The location for the new session.</param>
        /// <param name="Bytes">The byte array to load from.</param>
        /// <param name="displayDeletedRows">whether to serve all rows or only non-deleted rows</param>
        void LoadFromBytes(string User, string Task, WDEOpenMode Mode, string Location, byte[] Bytes, bool displayDeletedRows);

        #endregion

        #region IWDEDataSet_R1
        void MergeDocumentFromStream(System.IO.Stream aStream);
        void MergeDocumentFromStream(System.IO.Stream aStream, bool displayDeletedRows);
        #endregion
    }

	/// <summary>
	/// The Documents Collection
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("F63B1096-D5CC-448a-86E4-30265AB74C89")]
	public interface IWDEDocuments : IEnumerable
	{
		/// <summary>
		/// Gets BOF status. True if <see cref="Current"/> points to the first document.
		/// </summary>
		bool BOF {get;}
		/// <summary>
		/// Gets EOF status. True if <see cref="Next"/> has moved past the end of the document list.
		/// </summary>
		bool EOF {get;}
		/// <summary>
		/// Gets the current <see cref="IWDEDocument"/>. <see cref="First"/>, <see cref="Last"/>, <see cref="Prior"/> and <see cref="Next"/> affect this property.
		/// </summary>
		IWDEDocument Current {get;}
		/// <summary>
		/// Gets the <see cref="IWDEDataSet"/> this collection belongs to.
		/// </summary>
		IWDEDataSet DataSet {get;}
		/// <summary>
		/// Gets or sets the AltDCN property from <see cref="Current"/>.
		/// </summary>
		string AltDCN {get; set;}
		/// <summary>
		/// Gets or sets the DCN property from <see cref="Current"/>.
		/// </summary>
		string DCN {get; set;}
		/// <summary>
		/// Gets the DocType property from <see cref="Current"/>.
		/// </summary>
		string DocType {get;}
		/// <summary>
		/// Gets the <see cref="IWDEImages"/> collection from <see cref="Current"/>
		/// </summary>
		IWDEImages Images {get;}
		/// <summary>
		/// Gets the <see cref="IWDERecords"/> collection from <see cref="Current"/>
		/// </summary>
		IWDERecords Records {get;}
		/// <summary>
		/// Gets or sets the current index in the collection.
		/// </summary>
		int Index {get; set;}
		/// <summary>
		/// Gets the indicated <see cref="IWDEDocument"/> from the collection.
		/// </summary>
		IWDEDocument this [int Index] {get;}
		/// <summary>
		/// Gets the <see cref="IWDEDocSessions"/> collection from <see cref="Current"/>
		/// </summary>
		IWDEDocSessions Sessions {get;}
		/// <summary>
		/// Gets or sets the RejectCode property from <see cref="Current"/>
		/// </summary>
		string RejectCode {get; set;}
		/// <summary>
		/// Gets or sets the RejectDescription property from <see cref="Current"/>
		/// </summary>
		string RejectDescription {get; set;}
		/// <summary>
		/// Gets or sets the Status property from <see cref="Current"/>
		/// </summary>
		WDESessionStatus Status {get; set;}
		/// <summary>
		/// Gets the number of documents in the collection.
		/// </summary>
		int Count {get;}

		/// <summary>
		/// Clears all documents from the collection.
		/// </summary>
		void Clear();
		/// <summary>
		/// Moves <see cref="Current"/> to the first <see cref="IWDEDocument"/> in the collection.
		/// </summary>
		void First();
		/// <summary>
		/// Moves <see cref="Current"/> to the last <see cref="IWDEDocument"/> in the collection.
		/// </summary>
		void Last();
		/// <summary>
		/// Moves <see cref="Current"/> to the next <see cref="IWDEDocument"/> in the collection.
		/// </summary>
		void Next();
		/// <summary>
		/// Moves <see cref="Current"/> to the previous <see cref="IWDEDocument"/> in the collection.
		/// </summary>
		void Prior();
		/// <summary>
		/// Appends a new document to the collection.
		/// </summary>
		/// <param name="DocType">The DocType for the new document.</param>
		/// <returns>The newly added <see cref="IWDEDocument"/></returns>
		IWDEDocument Append(string DocType);
		/// <summary>
		/// Appends a new document to the collection and copies data from the given document.
		/// </summary>
		/// <param name="Document">The <see cref="IWDEDocument"/> to copy data from.</param>
		/// <returns>The newly appended <see cref="IWDEDocument"/></returns>
		IWDEDocument Append(IWDEDocument Document);
		/// <summary>
		/// Inserts a new document into the collection at the given index.
		/// </summary>
		/// <param name="Index">The position to insert into.</param>
		/// <param name="DocType">The DocType for the new document.</param>
		/// <returns>The newly inserted <see cref="IWDEDocument"/></returns>
		IWDEDocument Insert(int Index, string DocType);
		/// <summary>
		/// Deletes the document in <see cref="Current"/>
		/// </summary>
		void Delete();
		/// <summary>
		/// Finds the document with the given DCN.
		/// </summary>
		/// <param name="DCN">The DCN to find.</param>
		/// <returns>The index of the found document. Returns -1 if the DCN is not found.</returns>
		int Find(string DCN);
	}

#if DEBUG
	public interface IWDEDocumentsInternal
#else
	internal interface IWDEDocumentsInternal
#endif
	{
		void LoadDocumentFromStream(System.IO.Stream aStream);
        void MergeDocumentFromStream(System.IO.Stream aStream);
	}

	/// <summary>
	/// A WebDX document
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("28CB2CFA-1311-48bb-A60B-4394C9DCF989")]
	public interface IWDEDocument
	{
		/// <summary>
		/// Gets the version of the WebDX API this document was last saved by.
		/// </summary>
		/// <remarks>This returns the current version of the API if the document has not yet been saved.</remarks>
		string APIVersion {get;}
		/// <summary>
		/// Gets the number of fields that are flagged.
		/// </summary>
		int FlaggedFieldCount {get;}
		/// <summary>
		/// Gets the DocumentDef that corresponds to <see cref="DocType"/>.
		/// </summary>
		/// <remarks>This property is null if a project is not assigned to the DataSet or the <see cref="DocType"/> for this document is not found in that project.</remarks>
		IWDEDocumentDef DocumentDef {get;}
		/// <summary>
		/// Gets or sets the DCN.
		/// </summary>
		string DCN {get; set;}
		/// <summary>
		/// Gets or sets the AltDCN.
		/// </summary>
		string AltDCN {get; set;}
		/// <summary>
		/// Gets the DocType.
		/// </summary>
		string DocType {get;}
		/// <summary>
		/// Gets the <see cref="IWDEImages"/> collection.
		/// </summary>
		IWDEImages Images {get;}
		/// <summary>
		/// Gets the <see cref="IWDEDocSessions"/> collection.
		/// </summary>
		IWDEDocSessions Sessions {get;}
		/// <summary>
		/// Gets or sets the reject code. Must match a reject code in the Project.
		/// </summary>
		string RejectCode {get; set;}
		/// <summary>
		/// Gets or sets the reject description.
		/// </summary>
		string RejectDescription {get; set;}
		/// <summary>
		/// Gets or sets the rejected status.
		/// </summary>
		/// <remarks>True if <see cref="Status"/> is Rejected. False otherwise. Setting this property to false sets <see cref="Status"/> to None. </remarks>
		bool Rejected {get; set;}
		/// <summary>
		/// Gets or sets the QIAutoAudit flag.
		/// </summary>
		bool QIAutoAudit {get; set;}
		/// <summary>
		/// Gets or sets the QIFocusAudit flag.
		/// </summary>
		bool QIFocusAudit {get; set;}
		/// <summary>
		/// Gets or sets the QISelected flag.
		/// </summary>
		bool QISelected {get; set;}
		/// <summary>
		/// Gets the document Status.
		/// </summary>
		WDESessionStatus Status {get;}
		/// <summary>
		/// Gets or sets the DocType used when MCP3 stores this document.
		/// </summary>
		string StoredDocType {get; set;}
		/// <summary>
		/// Gets the <see cref="IWDEDocuments"/> collection containing this document.
		/// </summary>
		IWDEDocuments ParentDocuments {get;}
		/// <summary>
		/// Gets the <see cref="IWDEDataSet"/> containing this document.
		/// </summary>
		IWDEDataSet DataSet {get;}
		/// <summary>
		/// Gets the immediate child <see cref="IWDERecords"/> collection.
		/// </summary>
		IWDERecords Records {get;}
		/// <summary>
		/// Gets or sets this documents index in the <see cref="ParentDocuments"/> collection.
		/// </summary>
		int Index {get; set;}

		/// <summary>
		/// Deletes this document
		/// </summary>
		void Delete();
		/// <summary>
		/// Clears all records and data from this document. Does not affect <see cref="DCN"/>, <see cref="AltDCN"/> or <see cref="DocType"/> properties.
		/// </summary>
		void ClearData();
		/// <summary>
		/// Returns a read-only record set containing only records of the given type.
		/// </summary>
		/// <param name="RecType">The type of records to read.</param>
		/// <returns>A record set containing only the records of the given type.</returns>
		IWDEFilteredRecords FilteredRecords(string RecType);
		/// <summary>
		/// Loads data for this document from the given file.
		/// </summary>
		/// <param name="FileName">The file name to load from.</param>
		void LoadFromFile(string FileName);
		/// <summary>
		/// Loads the data for this document from the given stream.
		/// </summary>
		/// <param name="aStream">The COM stream to load from.</param>
		void LoadFromStream(System.Runtime.InteropServices.ComTypes.IStream aStream);
		/// <summary>
		/// Loads the data for this document from the given stream.
		/// </summary>
		/// <param name="aStream">The .Net stream to load from.</param>
		void LoadFromStream(System.IO.Stream aStream);
		/// <summary>
		/// Save this document to the given file.
		/// </summary>
		/// <param name="FileName">The file name to save to.</param>
		void SaveToFile(string FileName);
		/// <summary>
		/// Save this document to the given file.
		/// </summary>
		/// <param name="FileName">The file name to save to.</param>
		/// <param name="ChildOfDataSet">Wrap the Xml with a DataSet tag.</param>
		void SaveToFile(string FileName, bool ChildOfDataSet);
		/// <summary>
		/// Save this document to the given stream.
		/// </summary>
		/// <param name="aStream">The COM stream to save to.</param>
		void SaveToStream(System.Runtime.InteropServices.ComTypes.IStream aStream);
		/// <summary>
		/// Save this document to the given stream.
		/// </summary>
		/// <param name="aStream">The COM stream to save to.</param>
		/// <param name="ChildOfDataSet">Wrap the Xml with a DataSet tag.</param>
		void SaveToStream(System.Runtime.InteropServices.ComTypes.IStream aStream, bool ChildOfDataSet);
		/// <summary>
		/// Save this document to the given stream.
		/// </summary>
		/// <param name="aStream">The .Net stream to save to.</param>
		void SaveToStream(System.IO.Stream aStream);
		/// <summary>
		/// Save this document to the given stream.
		/// </summary>
		/// <param name="aStream">The .Net stream to save to.</param>
		/// <param name="ChildOfDataSet">Wrap the Xml with a DataSet tag.</param>
		void SaveToStream(System.IO.Stream aStream, bool ChildOfDataSet);
	}

    /// <summary>
	/// A WebDX document
	/// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("F08124DF-1B45-4fc0-855D-3D4F7063B915")]
    public interface IWDEDocument_R1 : IWDEDocument
    {
        DateTime StartTime { get; set;}
        DateTime EndTime { get; set;}
    }

    /// <summary>
    /// A WebDX document
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("08397A8D-FE20-4CBA-AF87-3C722C52DF29")]
    public interface IWDEDocument_R2 : IWDEDocument_R1
    {
        /// <summary>
        /// Gets or sets the name of the field which was focused when the document was rejected.
        /// </summary>
        string RejectField { get; set; }
        /// <summary>
        /// Gets or sets the row id of the detail grid which was focused when the document was rejected. Value will be -1 for non detail grid fields.
        /// </summary>
        int RejectRow { get; set; }
    }

    /// <summary>
    /// A WebDX document
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("A28CA5D0-0195-444A-93EC-A053B17A259C")]
    public interface IWDEDocument_R3 : IWDEDocument_R2
    {
        /// <summary>
        /// Gets or sets the notes mentioned by the operator in WebDX client.
        /// </summary>
        string DocNotes { get; set; }        
    }

    /// <summary>
    /// A WebDX document
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("77106905-65E1-42F3-A594-BB270AAABB68")]
    public interface IWDEDocument_R4 : IWDEDocument_R3
    {
        /// <summary>
        /// Gets or sets the AllowRescan flag.
        /// </summary>
        bool AllowRescan { get; set; }
        /// <summary>
        /// Gets or sets the AllowReclassify flag.
        /// </summary>
        bool AllowReclassify { get; set; }
        /// <summary>
        /// Gets the<see cref= "IWDERejectCodes" /> collection.
        /// </summary>
        IWDERejectCodes RejectCodes { get; }
    }

#if DEBUG
    public interface IWDEDocumentInternal
#else
	internal interface IWDEDocumentInternal
#endif
	{
		IWDERecordDefs RecordDefs {get;}
		string DocType {get; set;}
		string StoredDocType {get; set;}
		WDESessionStatus Status {get; set;}
		int SaveSessionID {get; set;}
	}

	/// <summary>
	/// The images collection for a document.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("33611254-44BD-4c42-97EA-BF2ABFA339B4")]
	public interface IWDEImages : IEnumerable
	{
		/// <summary>
		/// The number of <see cref="IWDEImage"/>s.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the <see cref="IWDEImage"/> at the given index.
		/// </summary>
		IWDEImage this [int Index] {get;}
		/// <summary>
		/// Gets the <see cref="IWDEDocument"/> this image collection belongs to.
		/// </summary>
		IWDEDocument Document {get;}

		/// <summary>
		/// Removes all <see cref="IWDEImage"/>s from the collection.
		/// </summary>
		void Clear();
		/// <summary>
		/// Appends a new <see cref="IWDEImage"/> to the collection.
		/// </summary>
		/// <param name="ImageType">The ImageType for the new image.</param>
		/// <param name="ImageName">The ImageName for the new image. This should be the file name only with no path information.</param>
		/// <returns>The newly appended image.</returns>
		IWDEImage Append(string ImageType, string ImageName);
		/// <summary>
		/// Appends a new <see cref="IWDEImage"/> to the collection.
		/// </summary>
		/// <param name="ImageType">The ImageType for the new image.</param>
		/// <param name="ImageName">The ImageName for the new image. This should be the file name only with no path information.</param>
		/// <param name="ImagePath">The ImagePath for the new image. An alternative path for the server to search.</param>
		/// <param name="ZipName">The ZipName that contains this image.</param>
		/// <param name="IsSnippet">The IsSnippet value for the new image.</param>
		/// <returns>The newly appended image.</returns>
		IWDEImage Append(string ImageType, string ImageName, string ImagePath, string ZipName, bool IsSnippet);
		/// <summary>
		/// Inserts a new image at the given index.
		/// </summary>
		/// <param name="Index">The index to insert the new image into.</param>
		/// <param name="ImageType">The ImageType for the new image.</param>
		/// <param name="ImageName">The ImageName for the new image. This should be the file name only with no path information.</param>
		/// <returns>The newly appended image.</returns>
		IWDEImage Insert(int Index, string ImageType, string ImageName);
		/// <summary>
		/// Inserts a new image at the given index.
		/// </summary>
		/// <param name="Index">The index to insert the new image into.</param>
		/// <param name="ImageType">The ImageType for the new image.</param>
		/// <param name="ImageName">The ImageName for the new image. This should be the file name only with no path information.</param>
		/// <param name="ImagePath">The ImagePath for the new image. An alternative path for the server to search.</param>
		/// <param name="ZipName">The ZipName that contains this image.</param>
		/// <param name="IsSnippet">The IsSnippet value for the new image.</param>
		/// <returns>The newly appended image.</returns>
		IWDEImage Insert(int Index, string ImageType, string ImageName, string ImagePath, string ZipName, bool IsSnippet);
		/// <summary>
		/// Finds the image with the given image name.
		/// </summary>
		/// <param name="ImageName">The image name to search for.</param>
		/// <returns>The index of the <see cref="IWDEImage"/> with the given ImageName. Returns -1 if the ImageName is not found.</returns>
		int Find(string ImageName);
        /// <summary>
        /// Delete the image at the given index
        /// </summary>
        /// <param name="Index">The index of the image that needs to be deleted.</param>
        void Delete(int Index);
	}

	/// <summary>
	/// An image for a document.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("C1AD72AD-DF1F-428e-B902-B9670F684754")]
	public interface IWDEImage
	{
		/// <summary>
		/// Gets the document associated with this image.
		/// </summary>
		IWDEDocument Document {get;}
		/// <summary>
		/// Gets or sets the image type for this image. This should match an InputSource name in the Project. Affects the zones for this image in the WebDX Client.
		/// </summary>
		string ImageType {get; set;}
		/// <summary>
		/// Gets or sets the image file name for this image. This should be the file name only with no path information.
		/// </summary>
		string ImageName {get; set;}
		/// <summary>
		/// Gets or sets an alternative path to use when the server searches for this image. Used only by the Central Server.
		/// </summary>
		string ImagePath {get; set;}
		/// <summary>
		/// Gets or sets an indicator that determines if this image is a snippet image.
		/// </summary>
		bool IsSnippet {get; set;}
		/// <summary>
		/// This property is designed for use by the Central OCR group. It is not intended to be used in your code.
		/// </summary>
		bool PerformOCR {get; set;}
		/// <summary>
		/// Gets or sets the RegisteredImage name for this image. Used by the Central OCR group.
		/// </summary>
		string RegisteredImage {get; set;}
		/// <summary>
		/// Gets or sets the index of this image in the <see cref="IWDEImages"/> collection.
		/// </summary>
		int Index {get; set;}

		/// <summary>
		/// Gets or sets the rotate angle to be used when displaying this image in the WebDX Client.
		/// </summary>
		int Rotate {get; set;}
		/// <summary>
		/// Gets or sets the Attachment Type used when storing this image in MCP3.
		/// </summary>
		string StoredAttachType {get; set;}
		/// <summary>
		/// Gets or sets an arbitrary integer related to this image.
		/// </summary>
		/// <remarks>This property is not used by the WebDX system and is designed to be defined and used by your code.</remarks>
		int Tag {get; set;}
		/// <summary>
		/// Gets or sets the ZipName this image is stored in. Used by the Central Server when retrieving this image.
		/// </summary>
		string ZipName {get; set;}
		/// <summary>
		/// Gets or sets the X value of the zone offset for this image. Used to adjust the zone location on the image when displayed in the WebDX Client.
		/// </summary>
		int ZoneOffsetX {get; set;}
		/// <summary>
		/// Gets or sets the Y value of the zone offset for this image. Used to adjust the zone location on the image when displayed in the WebDX Client.
		/// </summary>
		int ZoneOffsetY {get; set;}
		/// <summary>
		/// Gets or sets the X value of the overlay offset for this image. Used to adjust the overlay location on the image when displayed in the WebDX Client.
		/// </summary>
		int OverlayOffsetX {get; set;}
		/// <summary>
		/// Gets or sets the Y value of the overlay offset for this image. Used to adjust the overlay location on the image when displayed in the WebDX Client.
		/// </summary>
		int OverlayOffsetY {get; set;}
	}

    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("A9B06152-9415-4B6B-9D8A-45757D6BDF05")]
    public interface IWDEImage_R1 : IWDEImage
    {
        /// <summary>
        /// Gets the document associated with this image.
        /// </summary>
        IWDEDocument Document { get; }
        /// <summary>
        /// Gets or sets the image type for this image. This should match an InputSource name in the Project. Affects the zones for this image in the WebDX Client.
        /// </summary>
        string ImageType { get; set; }
        /// <summary>
        /// Gets or sets the image file name for this image. This should be the file name only with no path information.
        /// </summary>
        string ImageName { get; set; }
        /// <summary>
        /// Gets or sets an alternative path to use when the server searches for this image. Used only by the Central Server.
        /// </summary>
        string ImagePath { get; set; }
        /// <summary>
        /// Gets or sets an indicator that determines if this image is a snippet image.
        /// </summary>
        bool IsSnippet { get; set; }
        /// <summary>
        /// This property is designed for use by the Central OCR group. It is not intended to be used in your code.
        /// </summary>
        bool PerformOCR { get; set; }
        /// <summary>
        /// Gets or sets the RegisteredImage name for this image. Used by the Central OCR group.
        /// </summary>
        string RegisteredImage { get; set; }
        /// <summary>
        /// Gets or sets the index of this image in the <see cref="IWDEImages"/> collection.
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// Gets or sets the rotate angle to be used when displaying this image in the WebDX Client.
        /// </summary>
        int Rotate { get; set; }
        /// <summary>
        /// Gets or sets the Attachment Type used when storing this image in MCP3.
        /// </summary>
        string StoredAttachType { get; set; }
        /// <summary>
        /// Gets or sets an arbitrary integer related to this image.
        /// </summary>
        /// <remarks>This property is not used by the WebDX system and is designed to be defined and used by your code.</remarks>
        int Tag { get; set; }
        /// <summary>
        /// Gets or sets the ZipName this image is stored in. Used by the Central Server when retrieving this image.
        /// </summary>
        string ZipName { get; set; }
        /// <summary>
        /// Gets or sets the X value of the zone offset for this image. Used to adjust the zone location on the image when displayed in the WebDX Client.
        /// </summary>
        int ZoneOffsetX { get; set; }
        /// <summary>
        /// Gets or sets the Y value of the zone offset for this image. Used to adjust the zone location on the image when displayed in the WebDX Client.
        /// </summary>
        int ZoneOffsetY { get; set; }
        /// <summary>
        /// Gets or sets the X value of the overlay offset for this image. Used to adjust the overlay location on the image when displayed in the WebDX Client.
        /// </summary>
        int OverlayOffsetX { get; set; }
        /// <summary>
        /// Gets or sets the Y value of the overlay offset for this image. Used to adjust the overlay location on the image when displayed in the WebDX Client.
        /// </summary>
        int OverlayOffsetY { get; set; }
        /// <summary>
        /// Gets or sets the number of pages in this image file. Used for multi-page file support. Defaults to 1.
        /// </summary>
        int PageCount { get; set; }
    }

    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("9FE8046B-1769-4CD2-91FC-338A6B2BF623")]
    public interface IWDEImage_R2 : IWDEImage_R1
    {
        /// <summary>
        /// Gets or sets an indicator that determines if this image is modified.
        /// </summary>
        bool Modified { get; set; }
    }

    /// <summary>
	/// The reject code collection for a document.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("88E3D3C7-6B32-4814-97D3-6D1A33BBD867")]
    public interface IWDERejectCodes : IEnumerable
    {
        /// <summary>
        /// The number of <see cref="IWDERejectCode"/>s.
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets the <see cref="IWDERejectCode"/> at the given index.
        /// </summary>
        IWDERejectCode this[int Index] { get; }
        /// <summary>
        /// Gets the <see cref="IWDEDocument"/> this reject code collection belongs to.
        /// </summary>
        IWDEDocument Document { get; }
        /// <summary>
        /// Removes all <see cref="IWDERejectCode"/>s from the collection.
        /// </summary>
        void Clear();
        /// <summary>
        /// Creates a reject code using the given code, description and require reason information.
        /// </summary>
        /// <param name="Code">The name of the reject code to create.</param>
        /// <param name="Description">The description of the reject code.</param>
        /// <param name="RequireReason">Bool determines if the user needs to enter the description during the rejection of the document.</param>
        /// <returns>The newly created reject code.</returns>
        IWDERejectCode Add(string Code, string Description, bool RequireReason);
        /// <summary>
        /// Finds the reject code with the given code.
        /// </summary>
        /// <param name="Code">The code to search for.</param>
        /// <returns>The index of the <see cref="IWDERejectCode"/> with the given Code. Returns -1 if the Code is not found.</returns>
        int Find(string Code);
        /// <summary>
        /// Delete the reject code at the given index
        /// </summary>
        /// <param name="Index">The index of the reject code that needs to be deleted.</param>
        void Delete(int Index);
    }

    /// <summary>
    /// A reject code for a document.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("23B15F63-BB87-4128-9162-1A59EA029A16")]
    public interface IWDERejectCode
    {
        /// <summary>
        /// Gets the document associated with this image.
        /// </summary>
        IWDEDocument Document { get; }        
        /// <summary>
        /// Gets or sets the reject code.
        /// </summary>
        string Code { get; set; }
        /// <summary>
        /// Gets or sets a description.
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// Gets or sets a value indicating that the keyer must enter a description when rejecting using this code.
        /// </summary>
        bool RequireReason { get; set; }
    }

    /// <summary>
    /// The records for a document
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("7807B2B3-2B75-4498-B234-0B49DDB61AC6")]
	public interface IWDERecords : IEnumerable
	{
		/// <summary>
		/// Gets number of records in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the BOF status. True if <see cref="Current"/> is at the first record in the collection.
		/// </summary>
		bool BOF {get;}
		/// <summary>
		/// Gets the EOF status. True of <see cref="Next"/> has moved past the end of the collection.
		/// </summary>
		bool EOF {get;}
		/// <summary>
		/// Gets the current <see cref="IWDERecord"/>. <see cref="First"/>, <see cref="Last"/>, <see cref="Next"/>, <see cref="Prior"/> and <see cref="Index"/> affect this property.
		/// </summary>
		IWDERecord Current {get;}
		/// <summary>
		/// Gets the RecType for this record. Corresponds to a RecType in the Project.
		/// </summary>
		string RecType {get;}
		/// <summary>
		/// Gets or sets the current index. Affects the <see cref="Current"/> property.
		/// </summary>
		int Index {get; set;}
		/// <summary>
		/// Gets the <see cref="IWDERecord"/> at the given index.
		/// </summary>
		IWDERecord this [int Index] {get;}
		/// <summary>
		/// Gets the records property from <see cref="Current"/>.
		/// </summary>
		IWDERecords Records {get;}
		/// <summary>
		/// Gets the <see cref="IWDEDocument"/> that this record collection belongs to.
		/// </summary>
		IWDEDocument Document {get;}
		/// <summary>
		/// Gets the <see cref="IWDERecord"/> that is the immediate parent of this records collection. Returns null if there is no parent record.
		/// </summary>
		IWDERecord OwnerRecord {get;}
		/// <summary>
		/// Gets the row status whether it is marked as deleted or not
		/// </summary>
		bool IsDeleted { get;}
		/// <summary>
		/// Gets the session id in which the row was marked as deleted
		/// </summary>
		int SessionID { get;}
		/// <summary>
		/// Removes all <see cref="IWDERecord"/>s from this collection.
		/// </summary>
		void Clear();
		/// <summary>
		/// Moves <see cref="Current"/> to the first record.
		/// </summary>
		void First();
		/// <summary>
		/// Moves <see cref="Current"/> to the last record.
		/// </summary>
		void Last();
		/// <summary>
		/// Moves <see cref="Current"/> to the next record.
		/// </summary>
		void Next();
		/// <summary>
		/// Moves <see cref="Current"/> to the previous record.
		/// </summary>
		void Prior();
		/// <summary>
		/// Deletes the record in <see cref="Current"/>.
		/// </summary>
		void Delete();
        /// <summary>
        /// Deletes specific row based on index and RecType
        /// </summary>
        /// <param name="intIndex">The index for delete.</param>
        /// <param name="RecType">The RecType of the record to delete. Must match a record name in the Project.</param>
        void DeleteRow(int intIndex, string RecType);
		/// <summary>
		/// Deletes a specific row based on index
		/// </summary>
		/// <param name="intIndex">The index to delete</param>
		void DeleteRow(int intIndex);
        /// <summary>
		/// Appends a new record.
		/// </summary>
		/// <param name="RecType">The RecType of the record to append. Must match a record name in the Project.</param>
		/// <returns>The newly inserted <see cref="IWDERecord"/></returns>
		IWDERecord Append(string RecType);
		/// <summary>
		/// Inserts a new record at the given index.
		/// </summary>
		/// <param name="Index">The index to insert into.</param>
		/// <param name="RecType">The RecType of the record to append. Must match a record name in the Project.</param>
		/// <returns>The newly inserted <see cref="IWDERecord"/></returns>
		IWDERecord Insert(int Index, string RecType);
		/// <summary>
		/// Finds the field in <see cref="Current"/> that matches the given field name.
		/// </summary>
		/// <param name="FieldName">The field name to search for.</param>
		/// <returns>The <see cref="IWDEField"/> that matches the given field name.</returns>
		/// <exception cref="WDEException">Throws API00018 if there are no records, API00011 if there is no current record and API00010 if the field is not found.</exception>
		IWDEField_R1 FieldByName(string FieldName);
	}

	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("7807B2B3-2B75-4498-B234-0B49DDB61AC6")]
	public interface IWDERecords_R1 : IWDERecords, IEnumerable
	{
		/// <summary>
		/// Gets number of records in the collection.
		/// </summary>
		int Count { get; }
		/// <summary>
		/// Gets the BOF status. True if <see cref="Current"/> is at the first record in the collection.
		/// </summary>
		bool BOF { get; }
		/// <summary>
		/// Gets the EOF status. True of <see cref="Next"/> has moved past the end of the collection.
		/// </summary>
		bool EOF { get; }
		/// <summary>
		/// Gets the current <see cref="IWDERecord"/>. <see cref="First"/>, <see cref="Last"/>, <see cref="Next"/>, <see cref="Prior"/> and <see cref="Index"/> affect this property.
		/// </summary>
		IWDERecord Current { get; }
		/// <summary>
		/// Gets the RecType for this record. Corresponds to a RecType in the Project.
		/// </summary>
		string RecType { get; }
		/// <summary>
		/// Gets or sets the current index. Affects the <see cref="Current"/> property.
		/// </summary>
		int Index { get; set; }
		/// <summary>
		/// Gets the <see cref="IWDERecord"/> at the given index.
		/// </summary>
		IWDERecord this[int Index] { get; }
		/// <summary>
		/// Gets the records property from <see cref="Current"/>.
		/// </summary>
		IWDERecords Records { get; }
		/// <summary>
		/// Gets the <see cref="IWDEDocument"/> that this record collection belongs to.
		/// </summary>
		IWDEDocument Document { get; }
		/// <summary>
		/// Gets the <see cref="IWDERecord"/> that is the immediate parent of this records collection. Returns null if there is no parent record.
		/// </summary>
		IWDERecord OwnerRecord { get; }
		/// <summary>
		/// Gets the row status whether it is marked as deleted or not
		/// </summary>
		bool IsDeleted { get; }
		/// <summary>
		/// Gets the session id in which the row was marked as deleted
		/// </summary>
		int SessionID { get; }
		/// <summary>
		/// Removes all <see cref="IWDERecord"/>s from this collection.
		/// </summary>
		void Clear();
		/// <summary>
		/// Moves <see cref="Current"/> to the first record.
		/// </summary>
		void First();
		/// <summary>
		/// Moves <see cref="Current"/> to the last record.
		/// </summary>
		void Last();
		/// <summary>
		/// Moves <see cref="Current"/> to the next record.
		/// </summary>
		void Next();
		/// <summary>
		/// Moves <see cref="Current"/> to the previous record.
		/// </summary>
		void Prior();
		/// <summary>
		/// Deletes the record in <see cref="Current"/>.
		/// </summary>
		void Delete();
		/// <summary>
		/// Deletes specific row based on index and RecType
		/// </summary>
		/// <param name="intIndex">The index for delete.</param>
		/// <param name="RecType">The RecType of the record to delete. Must match a record name in the Project.</param>
		void DeleteRow(int intIndex, string RecType);
		/// <summary>
		/// Deletes a specific row based on index
		/// </summary>
		/// <param name="intIndex">The index to delete</param>
		void DeleteRow(int intIndex);
		/// <summary>
		/// Appends a new record.
		/// </summary>
		/// <param name="RecType">The RecType of the record to append. Must match a record name in the Project.</param>
		/// <returns>The newly inserted <see cref="IWDERecord"/></returns>
		IWDERecord Append(string RecType);
		/// <summary>
		/// Inserts a new record at the given index.
		/// </summary>
		/// <param name="Index">The index to insert into.</param>
		/// <param name="RecType">The RecType of the record to append. Must match a record name in the Project.</param>
		/// <returns>The newly inserted <see cref="IWDERecord"/></returns>
		IWDERecord Insert(int Index, string RecType);
		/// <summary>
		/// Finds the field in <see cref="Current"/> that matches the given field name.
		/// </summary>
		/// <param name="FieldName">The field name to search for.</param>
		/// <returns>The <see cref="IWDEField"/> that matches the given field name.</returns>
		/// <exception cref="WDEException">Throws API00018 if there are no records, API00011 if there is no current record and API00010 if the field is not found.</exception>
		IWDEField_R1 FieldByName(string FieldName);
		/// <summary>
		/// Filters the records and returns the filtered record set.
		/// </summary>
		/// <param name="RecType">The RecType to filter for.</param>
		/// <returns>The filtered records collection.</returns>
		IWDEFilteredRecords Filter(string RecType);
	}

	/// <summary>
	/// A set of filtered records
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("2260BDD6-AF41-4b2b-A71D-08C235799ADF")]
	public interface IWDEFilteredRecords : IEnumerable
	{
		/// <summary>
		/// Gets number of records in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the BOF status. True if <see cref="Current"/> is at the first record in the collection.
		/// </summary>
		bool BOF {get;}
		/// <summary>
		/// Gets the EOF status. True of <see cref="Next"/> has moved past the end of the collection.
		/// </summary>
		bool EOF {get;}
		/// <summary>
		/// Gets the current <see cref="IWDERecord"/>. <see cref="First"/>, <see cref="Last"/>, <see cref="Next"/>, <see cref="Prior"/> and <see cref="Index"/> affect this property.
		/// </summary>
		IWDERecord Current {get;}
		/// <summary>
		/// Gets the RecType for this record. Corresponds to a RecType in the Project.
		/// </summary>
		string RecType {get;}
		/// <summary>
		/// Gets or sets the current index. Affects the <see cref="Current"/> property.
		/// </summary>
		int Index {get; set;}
		/// <summary>
		/// Gets the <see cref="IWDERecord"/> at the given index.
		/// </summary>
		IWDERecord this [int Index] {get;}
		/// <summary>
		/// Gets the records property from <see cref="Current"/>.
		/// </summary>
		IWDERecords Records {get;}
		/// <summary>
		/// Gets the <see cref="IWDEDocument"/> that this record collection belongs to.
		/// </summary>
		IWDEDocument Document {get;}

		/// <summary>
		/// Moves <see cref="Current"/> to the first record.
		/// </summary>
		void First();
		/// <summary>
		/// Moves <see cref="Current"/> to the last record.
		/// </summary>
		void Last();
		/// <summary>
		/// Moves <see cref="Current"/> to the next record.
		/// </summary>
		void Next();
		/// <summary>
		/// Moves <see cref="Current"/> to the previous record.
		/// </summary>
		void Prior();
		/// <summary>
		/// Finds the field in <see cref="Current"/> that matches the given field name.
		/// </summary>
		/// <param name="FieldName">The field name to search for.</param>
		/// <returns>The <see cref="IWDEField"/> that matches the given field name.</returns>
		/// <exception cref="WDEException">Throws API00018 if there are no records, API00011 if there is no current record and API00010 if the field is not found.</exception>
		IWDEField_R1 FieldByName(string FieldName);
	}

#if DEBUG
	public interface IWDEFilteredRecordsInternal
#else
	internal interface IWDEFilteredRecordsInternal
#endif
	{
		void FilterRecords(string RecType);
	}

	/// <summary>
	/// A record in a document.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("804A204E-CAFE-4c83-9271-D85A304AD65D")]
	public interface IWDERecord
	{
		/// <summary>
		/// Gets the <see cref="IWDERecords"/> that contains this record.
		/// </summary>
		IWDERecords OwnerRecords {get;}
		/// <summary>
		/// Gets the <see cref="IWDEDocument"/> that contains this record.
		/// </summary>
		IWDEDocument Document {get;}
		/// <summary>
		/// Gets or sets the Rectangle to display, if this is a detail record, in the WebDX Client.
		/// </summary>
		System.Drawing.Rectangle DetailRect {get; set;}
		/// <summary>
		/// Gets the <see cref="IWDEFields"/> collection.
		/// </summary>
		IWDEFields Fields {get;}
		/// <summary>
		/// Gets the number of flagged fields.
		/// </summary>
		int FlaggedFieldCount {get;}
		/// <summary>
		/// Gets or sets the index of this record in the <see cref="OwnerRecords"/> collection.
		/// </summary>
		int Index {get; set;}
		/// <summary>
		/// Gets the record that contains this record. Returns null if there is no containing record.
		/// </summary>
		IWDERecord ParentRecord {get;}
		/// <summary>
		/// Gets the <see cref="IWDERecordDef"/> from the Project that is associated with this record.
		/// </summary>
		IWDERecordDef RecordDef {get;}
		/// <summary>
		/// Gets the record type.
		/// </summary>
		string RecType {get;}
		/// <summary>
		/// Gets the child <see cref="IWDERecords"/>.
		/// </summary>
		IWDERecords Records {get;}
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
		/// Finds the field whose name matches the given field name.
		/// </summary>
		/// <param name="FieldName">The field name to find.</param>
		/// <returns>The <see cref="IWDEField"/> that matches FieldName.</returns>
		/// <exception cref="WDEException">Throws API00011 if the field is not found.</exception>
		IWDEField_R1 FieldByName(string FieldName);
		/// <summary>
		/// Finds the field whose name matches the given field name.
		/// </summary>
		/// <param name="FieldName">The field name to find.</param>
		/// <returns>The <see cref="IWDEField"/> that matches FieldName. Returns null if the field is not found.</returns>
		IWDEField_R1 FindField(string FieldName);
        /// <summary>
        /// Gets or sets the status for the record, whether it needs to be removed during Put, applicable for Individual image indexing feature.  
        /// </summary>
        bool RemoveImageData { get; set; }
	}

    /// <summary>
    /// A record in a document.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("49B85F37-9450-4316-8D95-898FE079A2C3")]
    public interface IWDERecord_R1 : IWDERecord
    {
        /// <summary>
        /// Gets the <see cref="IWDERecords"/> that contains this record.
        /// </summary>
        IWDERecords OwnerRecords { get; }
        /// <summary>
        /// Gets the <see cref="IWDEDocument"/> that contains this record.
        /// </summary>
        IWDEDocument Document { get; }
        /// <summary>
        /// Gets or sets the Rectangle to display, if this is a detail record, in the WebDX Client.
        /// </summary>
        System.Drawing.Rectangle DetailRect { get; set; }
        /// <summary>
        /// Gets the <see cref="IWDEFields"/> collection.
        /// </summary>
        IWDEFields Fields { get; }
        /// <summary>
        /// Gets the number of flagged fields.
        /// </summary>
        int FlaggedFieldCount { get; }
        /// <summary>
        /// Gets or sets the index of this record in the <see cref="OwnerRecords"/> collection.
        /// </summary>
        int Index { get; set; }
        /// <summary>
        /// Gets the record that contains this record. Returns null if there is no containing record.
        /// </summary>
        IWDERecord ParentRecord { get; }
        /// <summary>
        /// Gets the <see cref="IWDERecordDef"/> from the Project that is associated with this record.
        /// </summary>
        IWDERecordDef RecordDef { get; }
        /// <summary>
        /// Gets the record type.
        /// </summary>
        string RecType { get; }
        /// <summary>
        /// Gets the child <see cref="IWDERecords"/>.
        /// </summary>
        IWDERecords Records { get; }
        /// <summary>
        /// Gets the row status whether it is marked as deleted or not
        /// </summary>
        bool IsDeleted { get; }
        /// <summary>
        /// Gets the session id in which the row was marked as deleted
        /// </summary>
        int SessionID { get; }
        /// <summary>
        /// Marks a row as deleted(sets IsDeleted and SessionID values)
        /// </summary>
        void Delete();
        /// <summary>
        /// Rolls back a deleted row
        /// </summary>
        void RestoreDeletedRow();
        /// <summary>
        /// Finds the field whose name matches the given field name.
        /// </summary>
        /// <param name="FieldName">The field name to find.</param>
        /// <returns>The <see cref="IWDEField"/> that matches FieldName.</returns>
        /// <exception cref="WDEException">Throws API00011 if the field is not found.</exception>
        IWDEField_R1 FieldByName(string FieldName);
        /// <summary>
        /// Finds the field whose name matches the given field name.
        /// </summary>
        /// <param name="FieldName">The field name to find.</param>
        /// <returns>The <see cref="IWDEField"/> that matches FieldName. Returns null if the field is not found.</returns>
        IWDEField_R1 FindField(string FieldName);
        /// <summary>
        /// Gets the link to the document image.
        /// </summary>
        string ImageName { get; }
    }

#if DEBUG
	public interface IWDERecordInternal
#else
	internal interface IWDERecordInternal
#endif
	{
		IWDERecordDef RecordDef {get; set;}
		string RecType {get; set;}
		int RecIndex { get; set;}
		void CreateFields(IWDERecordDef RecordDef);
		bool ContainsSessionID(int SessionID);
		bool IsBlankOrNull();
		void CreateMissingFields();
	}

	/// <summary>
	/// A field collection for a record.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("8B61311C-C71C-4300-8A48-80C879772915")]
	public interface IWDEFields : IEnumerable
	{
		/// <summary>
		/// Gets the number of fields.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the <see cref="IWDEField"/> at the given index.
		/// </summary>
		IWDEField_R1 this [int Index] {get;}
		/// <summary>
		/// Finds the field whose name matches the given field name.
		/// </summary>
		/// <param name="FieldName">The field name to search for.</param>
		/// <returns>The index of the field. Returns -1 if the field is not found.</returns>
		int Find(string FieldName);
		/// <summary>
		/// Gets the <see cref="IWDERecord"/> containing this field.
		/// </summary>
		IWDERecord DataRecord {get;}
	}

#if DEBUG
	public interface IWDEFieldsInternal
#else
	internal interface IWDEFieldsInternal
#endif
	{
		IWDEField_R1 Add(IWDEFieldDef FieldDef);
		void Clear();
	}

	/// <summary>
	/// A field contained in a record.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("45FA26BE-859E-46c6-8E42-309C16EE5AAB")]
	public interface IWDEField
	{
		/// <summary>
		/// Gets or sets the value as a Boolean value.
		/// </summary>
		bool AsBoolean {get; set;}
		/// <summary>
		/// Gets or sets the value as a Decimal value.
		/// </summary>
		decimal AsCurrency {get; set;}
		/// <summary>
		/// Gets or sets the value as a DateTime value.
		/// </summary>
		DateTime AsDateTime {get; set;}
		/// <summary>
		/// Gets or sets the value as a Double value.
		/// </summary>
		double AsFloat {get; set;}
		/// <summary>
		/// Gets or sets the value as an Int32 value.
		/// </summary>
		int AsInteger {get; set;}
		/// <summary>
		/// Gets or sets the value as a String value.
		/// </summary>
		string AsString {get; set;}
		/// <summary>
		/// Gets or sets the value as an Object value.
		/// </summary>
		object AsVariant {get; set;}
		/// <summary>
		/// Gets the maximum length as defined in the Project.
		/// </summary>
		int DataLen {get;}
		/// <summary>
		/// Gets the <see cref="IWDERecord"/> containing this field.
		/// </summary>
		IWDERecord DataRecord {get;}
		/// <summary>
		/// Gets the <see cref="IWDEDataSet"/> containing this field.
		/// </summary>
		IWDEDataSet DataSet {get;}
		/// <summary>
		/// Gets the DataType as defined in the Project.
		/// </summary>
		WDEDataType DataType {get;}
		/// <summary>
		/// Gets or sets an indicator for whether this field is excluded in OnePass mode in the WebDX Client.
		/// </summary>
		bool Exclude {get; set;}
		/// <summary>
		/// Gets the FieldDef in the Project.
		/// </summary>
		IWDEFieldDef FieldDef {get;}
		/// <summary>
		/// Gets the field name.
		/// </summary>
		string FieldName {get;}
		/// <summary>
		/// Gets or sets an indicator determing if this field is flagged. Used by Review mode in the WebDX Client.
		/// </summary>
		bool Flagged {get; set;}
		/// <summary>
		/// Gets or sets field flags. Used internally by the WebDX Client.
		/// </summary>
		WDEFieldFlags Flags {get; set;}
		/// <summary>
		/// Gets or sets misc flags. Used internally by the WebDX Client.
		/// </summary>
		WDEMiscFlags MiscFlags {get; set;}
		/// <summary>
		/// Gets or sets the FlagDescription. This is the reason why the field was flagged. Used by Review mode in the WebDX Client.
		/// </summary>
		string FlagDescription {get; set;}
		/// <summary>
		/// Gets or sets the ImageName used to key this field. Used by the WebDX Client if the TrackImage Project option is set.
		/// </summary>
		string ImageName {get; set;}
		/// <summary>
		/// Gets or sets the view rectangle used on the image in <see cref="ImageName"/> when keying this field. Used by the WebDX Client if the TrackImage Project option is set.
		/// </summary>
		System.Drawing.Rectangle ImageRect {get; set;}
		/// <summary>
		/// Gets or sets an indicator determining if this field is null. Setting this property to false throws an exception.
		/// </summary>
		bool IsNull {get; set;}
		/// <summary>
		/// Gets or sets the index of this field in the containing <see cref="IWDEFields"/> collection.
		/// </summary>
		int Index {get; set;}
		/// <summary>
		/// Gets or sets the OCRAreaRect for this field. This is the entire area assigned to be read by OCR.
		/// </summary>
		System.Drawing.Rectangle OCRAreaRect {get; set;}
		/// <summary>
		/// Gets or sets the OCRRect for this field. This is the actual area read by OCR.
		/// </summary>
		System.Drawing.Rectangle OCRRect {get; set;}
		/// <summary>
		/// Gets or sets the QIFocusAudit indicator.
		/// </summary>
		bool QIFocusAudit {get; set;}
		/// <summary>
		/// Gets the <see cref="IWDERevisions"/>.
		/// </summary>
		IWDERevisions Revisions {get;}
		/// <summary>
		/// Gets the current session ID.
		/// </summary>
		int SessionID {get;}
		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		WDEFieldStatus Status {get; set;}
		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		string Value {get; set;}
		/// <summary>
		/// Gets or sets the an arbitrary integer value. This value is not used by WebDX and is intended to be defined and used in your code.
		/// </summary>
		int Tag {get; set;}
		/// <summary>
		/// Gets an indicator of whether the field contains all numeric characters.
		/// </summary>
		bool IsNumeric {get;}
		/// <summary>
		/// Gets the current <see cref="IWDECharRepairs"/> collection.
		/// </summary>
		IWDECharRepairs CharRepairs {get;}

		/// <summary>
		/// Filters a string of characters removing characters that do not match the Character Set.
		/// </summary>
		/// <param name="Value">The string to filter.</param>
		/// <returns>A string with characters not matching the Character Set removed.</returns>
		string ExtractGoodChars(string Value);
	}

	/// <summary>
	/// A Field contained in a record.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("E15AF9BA-F0EE-4c4b-A804-B4E3B14B90AF")]
	public interface IWDEField_R1 : IWDEField
	{	
	//The rest of the members are duplicated from the IWDEField interface in order for COM Interop to function properly.
	//This causes compiler warnings, but makes the interfaces work as expected.
		/// <summary>
		/// Gets or sets the value as a Boolean value.
		/// </summary>
		bool AsBoolean {get; set;}
		/// <summary>
		/// Gets or sets the value as a Decimal value.
		/// </summary>
		decimal AsCurrency {get; set;}
		/// <summary>
		/// Gets or sets the value as a DateTime value.
		/// </summary>
		DateTime AsDateTime {get; set;}
		/// <summary>
		/// Gets or sets the value as a Double value.
		/// </summary>
		double AsFloat {get; set;}
		/// <summary>
		/// Gets or sets the value as an Int32 value.
		/// </summary>
		int AsInteger {get; set;}
		/// <summary>
		/// Gets or sets the value as a String value.
		/// </summary>
		string AsString {get; set;}
		/// <summary>
		/// Gets or sets the value as an Object value.
		/// </summary>
		object AsVariant {get; set;}
		/// <summary>
		/// Gets the maximum length as defined in the Project.
		/// </summary>
		int DataLen {get;}
		/// <summary>
		/// Gets the <see cref="IWDERecord"/> containing this field.
		/// </summary>
		IWDERecord DataRecord {get;}
		/// <summary>
		/// Gets the <see cref="IWDEDataSet"/> containing this field.
		/// </summary>
		IWDEDataSet DataSet {get;}
		/// <summary>
		/// Gets the DataType as defined in the Project.
		/// </summary>
		WDEDataType DataType {get;}
		/// <summary>
		/// Gets or sets an indicator for whether this field is excluded in OnePass mode in the WebDX Client.
		/// </summary>
		bool Exclude {get; set;}
		/// <summary>
		/// Gets the FieldDef in the Project.
		/// </summary>
		IWDEFieldDef FieldDef {get;}
		/// <summary>
		/// Gets the field name.
		/// </summary>
		string FieldName {get;}
		/// <summary>
		/// Gets or sets an indicator determing if this field is flagged. Used by Review mode in the WebDX Client.
		/// </summary>
		bool Flagged {get; set;}
		/// <summary>
		/// Gets or sets field flags. Used internally by the WebDX Client.
		/// </summary>
		WDEFieldFlags Flags {get; set;}
		/// <summary>
		/// Gets or sets misc flags. Used internally by the WebDX Client.
		/// </summary>
		WDEMiscFlags MiscFlags {get; set;}
		/// <summary>
		/// Gets or sets the FlagDescription. This is the reason why the field was flagged. Used by Review mode in the WebDX Client.
		/// </summary>
		string FlagDescription {get; set;}
		/// <summary>
		/// Gets or sets the ImageName used to key this field. Used by the WebDX Client if the TrackImage Project option is set.
		/// </summary>
		string ImageName {get; set;}
		/// <summary>
		/// Gets or sets the view rectangle used on the image in <see cref="ImageName"/> when keying this field. Used by the WebDX Client if the TrackImage Project option is set.
		/// </summary>
		System.Drawing.Rectangle ImageRect {get; set;}
		/// <summary>
		/// Gets or sets an indicator determining if this field is null. Setting this property to false throws an exception.
		/// </summary>
		bool IsNull {get; set;}
		/// <summary>
		/// Gets or sets the index of this field in the containing <see cref="IWDEFields"/> collection.
		/// </summary>
		int Index {get; set;}
		/// <summary>
		/// Gets or sets the OCRAreaRect for this field. This is the entire area assigned to be read by OCR.
		/// </summary>
		System.Drawing.Rectangle OCRAreaRect {get; set;}
		/// <summary>
		/// Gets or sets the OCRRect for this field. This is the actual area read by OCR.
		/// </summary>
		System.Drawing.Rectangle OCRRect {get; set;}
		/// <summary>
		/// Gets or sets the QIFocusAudit indicator.
		/// </summary>
		bool QIFocusAudit {get; set;}
		/// <summary>
		/// Gets the <see cref="IWDERevisions"/>.
		/// </summary>
		IWDERevisions Revisions {get;}
		/// <summary>
		/// Gets the current session ID.
		/// </summary>
		int SessionID {get;}
		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		WDEFieldStatus Status {get; set;}
		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		string Value {get; set;}
		/// <summary>
		/// Gets or sets the an arbitrary integer value. This value is not used by WebDX and is intended to be defined and used in your code.
		/// </summary>
		int Tag {get; set;}
		/// <summary>
		/// Gets an indicator of whether the field contains all numeric characters.
		/// </summary>
		bool IsNumeric {get;}
		/// <summary>
		/// Gets the current <see cref="IWDECharRepairs"/> collection.
		/// </summary>
		IWDECharRepairs CharRepairs {get;}

		/// <summary>
		/// Filters a string of characters removing characters that do not match the Character Set.
		/// </summary>
		/// <param name="Value">The string to filter.</param>
		/// <returns>A string with characters not matching the Character Set removed.</returns>
		string ExtractGoodChars(string Value);

		/// <summary>
		/// User defined data that is saved with the data file.
		/// </summary>
		string CustomData {get; set;}
	}

	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("0A17D7C8-1214-47e9-942F-ADBD5C18E537")]
	public interface IWDEFieldClient
	{
		void SetValueAndStatus(string NewValue, WDEFieldStatus NewStatus, int CharCount);
		void SetValueAndStatus(string NewValue, WDEFieldStatus NewStatus, int CharCount, int VCECount);       
	}

    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("2C84A5E3-8163-41F9-809C-CCC646A530DC")]
    public interface IWDEFieldClient_R1 : IWDEFieldClient
    {       
        void SetValueAndStatus(string NewValue, WebDX.Api.WDEFieldStatus NewStatus, int CharCount, string FormName);
        void SetValueAndStatus(string NewValue, WDEFieldStatus NewStatus, int CharCount, string FormName, int VCECount);
    }

#if DEBUG
	public interface IWDEFieldInternal
#else
	internal interface IWDEFieldInternal
#endif
	{
		IWDEFieldDef FieldDef {get; set;}
	}

	/// <summary>
	/// The revisions for a field.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("E7BABFD0-8E19-419a-9389-354460655A66")]
	public interface IWDERevisions : IEnumerable
	{
		/// <summary>
		/// Gets the number of revisions.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the revision at the given index.
		/// </summary>
		IWDERevision this [int Index] {get;}
		/// <summary>
		/// Gets the <see cref="IWDEField"/> that contains this collection.
		/// </summary>
		IWDEField_R1 Field {get;}
	}

#if DEBUG
	public interface IWDERevisionsInternal
#else
	internal interface IWDERevisionsInternal
#endif
	{
		IWDERevision Add(string Value, WDEFieldStatus Status, int SessionID, int CharCount, int VCECount);
		void Insert(int Index, IWDERevision rev);
		void Clear();
		void Delete();
        IWDERevision Add(string Value, WDEFieldStatus Status, int SessionID, int CharCount, string FormName, int VCECount);
	}

	/// <summary>
	/// A revision for a field.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("CE1A7391-E4BB-4d0a-813B-3599F884BFBA")]
	public interface IWDERevision
	{
		/// <summary>
		/// Gets the value as a Boolean.
		/// </summary>
		bool AsBoolean {get;}
		/// <summary>
		/// Gets the value as a Decimal.
		/// </summary>
		decimal AsCurrency {get;}
		/// <summary>
		/// Gets the value as a DateTime.
		/// </summary>
		DateTime AsDateTime {get;}
		/// <summary>
		/// Gets the value as a Double.
		/// </summary>
		double AsFloat {get;}
		/// <summary>
		/// Gets the value as an Int32.
		/// </summary>
		int AsInteger {get;}
		/// <summary>
		/// Gets the value as a String.
		/// </summary>
		string AsString {get;}
		/// <summary>
		/// Gets the value as an Object.
		/// </summary>
		object AsVariant {get;}
		/// <summary>
		/// Gets an indicator of whether this revision is flagged.
		/// </summary>
		bool Flagged {get;}
		/// <summary>
		/// Gets the flag description.
		/// </summary>
		string FlagDescription {get;}
		/// <summary>
		/// Gets an indicator of whether this revision is null.
		/// </summary>
		bool IsNull {get;}
		/// <summary>
		/// Gets the value.
		/// </summary>
		string Value {get;}
		/// <summary>
		/// Gets the status.
		/// </summary>
		WDEFieldStatus Status {get;}
		/// <summary>
		/// Gets the session ID.
		/// </summary>
		int SessionID {get;}
		/// <summary>
		/// Gets the number of characters keyed.
		/// </summary>
		int CharCount {get;}
		/// <summary>
		/// Gets the number of times the keyer received a Verify Compare Error in the WebDX Client.
		/// </summary>
		int VCECount {get;}
		/// <summary>
		/// Gets the <see cref="IWDECharRepairs"/>.
		/// </summary>
		IWDECharRepairs CharRepairs {get;}
		/// <summary>
		/// Gets the <see cref="IWDEField"/> containing this revision.
		/// </summary>
		IWDEField_R1 Field {get;}
	}

    public interface IWDERevision_R1 : IWDERevision
    {
        string ErrorCategory { get; set;}
        IWDEDocSession Session { get;}
    }

    public interface IWDERevision_R2 : IWDERevision_R1
    {
        string FormName { get; set; }
    }

#if DEBUG
	public interface IWDERevisionInternal
#else
	internal interface IWDERevisionInternal
#endif
	{
		bool Flagged {get; set;}
		string FlagDescription {get; set;}
		bool IsNull {get;}
		string Value {get; set;}
		WDEFieldStatus Status {get; set;}
		int SessionID {get; set;}
		int CharCount {get; set;}
		int VCECount {get; set;}
        string FormName { get; set; }
	}

	/// <summary>
	/// The Character Repair collection for a revision.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("6B104874-BE38-4369-8648-04A007F068D0")]
	public interface IWDECharRepairs : IEnumerable
	{
		/// <summary>
		/// Gets the number of <see cref="IWDECharRepair"/>s in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the <see cref="IWDECharRepair"/> at the given index.
		/// </summary>
		IWDECharRepair this [int Index] {get;}
		/// <summary>
		/// Gets the revision containing this collection.
		/// </summary>
		IWDERevision Revision {get;}
		/// <summary>
		/// Appends a new <see cref="IWDECharRepair"/> to the collection.
		/// </summary>
		/// <param name="Value">The character value.</param>
		/// <param name="Confidence">The OCR confidence level.</param>
		/// <param name="CharPos">The zero-based position of this repair in the field.</param>
		/// <param name="OCRRect">The rectangle that contains this character on the image.</param>
		/// <returns></returns>
		IWDECharRepair Add(char Value, int Confidence, int CharPos, System.Drawing.Rectangle OCRRect);
		/// <summary>
		/// Removes all <see cref="IWDECharRepair"/>s from the collection.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// A character repair for a revision.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("6952987B-3E15-496d-8746-F83D5B2BCD9C")]
	public interface IWDECharRepair
	{
		/// <summary>
		/// The <see cref="IWDEField"/> containing this repair.
		/// </summary>
		IWDEField_R1 Field {get;}
		/// <summary>
		/// The <see cref="IWDERevision"/> containing this repair.
		/// </summary>
		IWDERevision Revision {get;}
		/// <summary>
		/// The OCR confidence level.
		/// </summary>
		int Confidence {get; set;}
		/// <summary>
		/// The zero-based position of this repair in the field.
		/// </summary>
		int CharPos {get; set;}
		/// <summary>
		/// The character value.
		/// </summary>
		char Value {get; set;}
		/// <summary>
		/// The rectangle that contains this character on the image.
		/// </summary>
		System.Drawing.Rectangle OCRRect {get; set;}
	}

	/// <summary>
	/// The sessions for a DataSet.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("E90148E7-FD0C-42c6-8131-6DDFC6958B31")]
	public interface IWDESessions : IEnumerable
	{
		/// <summary>
		/// Gets the number of <see cref="IWDESessions"/> in this collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the <see cref="IWDESession"/> at the given index.
		/// </summary>
		IWDESession_R1 this [int Index] {get;}
		/// <summary>
		/// Gets the <see cref="IWDEDataSet"/> containing this collection.
		/// </summary>
		IWDEDataSet DataSet {get;}
		/// <summary>
		/// Finds a session that matches the given session ID.
		/// </summary>
		/// <param name="ID">The session ID to find.</param>
		/// <returns>The index of the session found. Returns -1 if the session was not found.</returns>
		int FindByID(int ID);
		/// <summary>
		/// Finds a session that matches the given user.
		/// </summary>
		/// <param name="User">The user to find.</param>
		/// <returns>The index of the session found. Returns -1 if the session was not found.</returns>
		int FindByUser(string User);
		/// <summary>
		/// Finds a session that matches the given task.
		/// </summary>
		/// <param name="Task">The task to find.</param>
		/// <returns>The index of the session found. Returns -1 if the session was not found.</returns>
		int FindByTask(string Task);
	}

#if DEBUG
	public interface IWDESessionsInternal
#else
	internal interface IWDESessionsInternal
#endif
	{
		IWDESession_R1 Add(string User, string Task, WDEOpenMode Mode, string Location);
        IWDESession_R1 Append(string User, string Task, WDEOpenMode Mode, string Location);
        IWDESession_R1 Insert(int Index, string User, string Task, WDEOpenMode Mode, string Location);
        void Merge(IWDESession_R1 newSession);
		void Clear();
	}

	/// <summary>
	/// A session for a DataSet.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("F1F3AA36-CBAC-41b0-8041-90E7DDB4A8C3")]
	public interface IWDESession
	{
		/// <summary>
		/// Gets user who opened this session.
		/// </summary>
		string User {get;}
		/// <summary>
		/// Gets task that opened this session.
		/// </summary>
		string Task {get;}
		/// <summary>
		/// Gets mode for this session.
		/// </summary>
		WDEOpenMode Mode {get;}
		/// <summary>
		/// Gets time the session was created.
		/// </summary>
		DateTime StartTime {get;}
		/// <summary>
		/// Gets time the session was saved.
		/// </summary>
		DateTime EndTime {get;}
		/// <summary>
		/// Gets session ID for this session. Used internally to link revisions, sessions and docsessions.
		/// </summary>
		int SessionID {get;}
		/// <summary>
		/// Gets location where this session opened.
		/// </summary>
		string Location {get;}
		/// <summary>
		/// Gets number of keystrokes for this session.
		/// </summary>
		int CharCount {get;}
		/// <summary>
		/// Gets number of times a user received a Verify Compare Error for this session.
		/// </summary>
		int VCECount {get;}
	}

	/// <summary>
	/// A session for a DataSet.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("50E3ED9B-EC5D-4888-8168-1FF75C2B2D9C")]
	public interface IWDESession_R1 : IWDESession
	{
		// copied from IWDESession
		/// <summary>
		/// Gets user who opened this session.
		/// </summary>
		string User {get;}
		/// <summary>
		/// Gets task that opened this session.
		/// </summary>
		string Task {get;}
		/// <summary>
		/// Gets mode for this session.
		/// </summary>
		WDEOpenMode Mode {get;}
		/// <summary>
		/// Gets time the session was created.
		/// </summary>
		DateTime StartTime {get;}
		/// <summary>
		/// Gets time the session was saved.
		/// </summary>
		DateTime EndTime {get;}
		/// <summary>
		/// Gets session ID for this session. Used internally to link revisions, sessions and docsessions.
		/// </summary>
		int SessionID {get;}
		/// <summary>
		/// Gets location where this session opened.
		/// </summary>
		string Location {get;}
		/// <summary>
		/// Gets number of keystrokes for this session.
		/// </summary>
		int CharCount {get;}
		/// <summary>
		/// Gets number of times a user received a Verify Compare Error for this session.
		/// </summary>
		int VCECount {get;}

		//new in IWDESession_R1
		/// <summary>
		/// Save a snapshot of the file as it was at the end of this session
		/// </summary>
		/// <param name="fileName">The file name to save to</param>
		void SaveSnapshot(string fileName);
		/// <summary>
		/// Save a snapshot of the file as it was at the end of this session
		/// </summary>
		/// <param name="aStream">The stream to save to</param>
		void SaveSnapshot(System.IO.Stream aStream);
		/// <summary>
		/// Save a snapshot of the file as it was at the end of this session
		/// </summary>
		/// <param name="aStream">The COM stream to save to</param>
		void SaveSnapshot(System.Runtime.InteropServices.ComTypes.IStream aStream);
		/// <summary>
		/// Save a snapshot of the file as it was at the end of this session
		/// </summary>
		/// <returns>The saved file as a byte array</returns>
		byte[] SaveSnapshot();
	}

#if DEBUG
	public interface IWDESessionInternal
#else
	internal interface IWDESessionInternal
#endif
	{
		string User {get; set;}
		string Task {get; set;}
		WDEOpenMode Mode {get; set;}
		DateTime StartTime {get; set;}
		DateTime EndTime {get; set;}
		int SessionID {get; set;}
		string Location {get; set;}
        bool IsCreateSession { get; set;}
		int CharCount {get;}
		int VCECount {get;}
        void SetStartTimeExplicit(DateTime aTime);
        void SetEndTimeExplicit(DateTime aTime);
	}

	/// <summary>
	/// The sessions for a document.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("B5953943-3A18-4eab-B441-5AFFE1B8F73D")]
	public interface IWDEDocSessions : IEnumerable
	{
		/// <summary>
		/// Gets the document containing this collection.
		/// </summary>
		IWDEDocument Document {get;}
		/// <summary>
		/// Gets the number of <see cref="IWDEDocSession"/>s in the collection.
		/// </summary>
		int Count {get;}
		/// <summary>
		/// Gets the <see cref="IWDESession"/> at the given index.
		/// </summary>
		IWDEDocSession_R1 this [int Index] {get;}
		/// <summary>
		/// Finds the session that matches the given session ID.
		/// </summary>
		/// <param name="ID">The session ID to find.</param>
		/// <returns>The index of the found session. Returns -1 if the session was not found.</returns>
		int FindByID(int ID);
		/// <summary>
		/// Finds the session that matches the given user.
		/// </summary>
		/// <param name="User">The user to find.</param>
		/// <returns>The index of the found session. Returns -1 if the session was not found.</returns>
		int FindByUser(string User);
		/// <summary>
		/// Finds the session that matches the given task.
		/// </summary>
		/// <param name="Task">The task to find.</param>
		/// <returns>The index of the found session. Returns -1 of the session was not found.</returns>
		int FindByTask(string Task);
	}

#if DEBUG
	public interface IWDEDocSessionsInternal
#else
	internal interface IWDEDocSessionsInternal
#endif
	{
		IWDEDocSession_R1 Add(string User, string Task, WDEOpenMode Mode, int SessionID, string location);
		void Clear();
	}

	/// <summary>
	/// A session for a document.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("092A4767-E844-41af-B719-030A7BBC6E02")]
	public interface IWDEDocSession
	{
		/// <summary>
		/// Gets the user who opened this session.
		/// </summary>
		string User {get;}
		/// <summary>
		/// Gets the task that opened this session.
		/// </summary>
		string Task {get;}
		/// <summary>
		/// Gets the mode for this session.
		/// </summary>
		WDEOpenMode Mode {get;}
		/// <summary>
		/// Gets the reject code for this session.
		/// </summary>
		string RejectCode {get;}
		/// <summary>
		/// Gets the RejectDescription for this session. 
		/// </summary>
		string RejectDescription {get;}
		/// <summary>
		/// Gets the session ID for this session.
		/// </summary>
		int SessionID {get;}
		/// <summary>
		/// Gets the location where this session opened.
		/// </summary>
		string Location {get;}
		/// <summary>
		/// Gets the status for this session.
		/// </summary>
		WDESessionStatus Status {get;}
		/// <summary>
		/// Gets the number of keystrokes for this session.
		/// </summary>
		int CharCount {get;}
		/// <summary>
		/// Gets the number of times the keyer received a Verify Compare Error during this session.
		/// </summary>
		int VCECount {get;}
	}

	/// <summary>
	/// A session for a document.
	/// </summary>
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	[Guid("ED715D7F-93AC-429d-85B1-3D54487F316A")]
	public interface IWDEDocSession_R1 : IWDEDocSession
	{
		//copied from IWDEDocSession
		/// <summary>
		/// Gets the user who opened this session.
		/// </summary>
		string User {get;}
		/// <summary>
		/// Gets the task that opened this session.
		/// </summary>
		string Task {get;}
		/// <summary>
		/// Gets the mode for this session.
		/// </summary>
		WDEOpenMode Mode {get;}
		/// <summary>
		/// Gets the reject code for this session.
		/// </summary>
		string RejectCode {get;}
		/// <summary>
		/// Gets the RejectDescription for this session. 
		/// </summary>
		string RejectDescription {get;}
		/// <summary>
		/// Gets the session ID for this session.
		/// </summary>
		int SessionID {get;}
		/// <summary>
		/// Gets the location where this session opened.
		/// </summary>
		string Location {get;}
		/// <summary>
		/// Gets the status for this session.
		/// </summary>
		WDESessionStatus Status {get;}
		/// <summary>
		/// Gets the number of keystrokes for this session.
		/// </summary>
		int CharCount {get;}
		/// <summary>
		/// Gets the number of times the keyer received a Verify Compare Error during this session.
		/// </summary>
		int VCECount {get;}

		//new in IWDEDocSession_R1
		/// <summary>
		/// Save a snapshot of the file as it was at the end of this session
		/// </summary>
		/// <param name="fileName">The file name to save to</param>
		void SaveSnapshot(string fileName);
		/// <summary>
		/// Save a snapshot of the file as it was at the end of this session
		/// </summary>
		/// <param name="aStream">The stream to save to</param>
		void SaveSnapshot(System.IO.Stream aStream);
		/// <summary>
		/// Save a snapshot of the file as it was at the end of this session
		/// </summary>
		/// <param name="aStream">The COM stream to save to</param>
		void SaveSnapshot(System.Runtime.InteropServices.ComTypes.IStream aStream);
		/// <summary>
		/// Save a snapshot of the file as it was at the end of this session
		/// </summary>
		/// <returns>The saved file as a byte array</returns>
		byte[] SaveSnapshot();
	}

    /// <summary>
    /// A session for a document.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("CB24B346-CA07-4e07-81FE-873977CAFA25")]
    public interface IWDEDocSession_R2 : IWDEDocSession_R1
    {
        DateTime StartTime { get;}
        DateTime EndTime { get;}
    }

    /// <summary>
    /// A session for a document.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("6BFBD369-607F-4E04-AC74-A4C5985EC6A8")]
    public interface IWDEDocSession_R3 : IWDEDocSession_R2
    {
        //copied from IWDEDocSession
        /// <summary>
        /// Gets the user who opened this session.
        /// </summary>
        string User { get; }
        /// <summary>
        /// Gets the task that opened this session.
        /// </summary>
        string Task { get; }
        /// <summary>
        /// Gets the mode for this session.
        /// </summary>
        WDEOpenMode Mode { get; }
        /// <summary>
        /// Gets the reject code for this session.
        /// </summary>
        string RejectCode { get; }
        /// <summary>
        /// Gets the RejectDescription for this session. 
        /// </summary>
        string RejectDescription { get; }
        /// <summary>
        /// Gets the start time for this session.
        /// </summary>
        DateTime StartTime { get; }
        /// <summary>
        /// Gets the end time for this session.
        /// </summary>
        DateTime EndTime { get; }
        /// <summary>
        /// Gets the elapsed time for this session.
        /// </summary>
        TimeSpan Duration { get; }
        /// <summary>
        /// Gets the session ID for this session.
        /// </summary>
        int SessionID { get; }
        /// <summary>
        /// Gets the location where this session opened.
        /// </summary>
        string Location { get; }
        /// <summary>
        /// Gets the status for this session.
        /// </summary>
        WDESessionStatus Status { get; }
        /// <summary>
        /// Gets the number of keystrokes for this session.
        /// </summary>
        int CharCount { get; }
        /// <summary>
        /// Gets the number of times the keyer received a Verify Compare Error during this session.
        /// </summary>
        int VCECount { get; }

        //new in IWDEDocSession_R1
        /// <summary>
        /// Save a snapshot of the file as it was at the end of this session
        /// </summary>
        /// <param name="fileName">The file name to save to</param>
        void SaveSnapshot(string fileName);
        /// <summary>
        /// Save a snapshot of the file as it was at the end of this session
        /// </summary>
        /// <param name="aStream">The stream to save to</param>
        void SaveSnapshot(System.IO.Stream aStream);
        /// <summary>
        /// Save a snapshot of the file as it was at the end of this session
        /// </summary>
        /// <param name="aStream">The COM stream to save to</param>
        void SaveSnapshot(System.Runtime.InteropServices.ComTypes.IStream aStream);
        /// <summary>
        /// Save a snapshot of the file as it was at the end of this session
        /// </summary>
        /// <returns>The saved file as a byte array</returns>
        byte[] SaveSnapshot();
    }

    /// <summary>
    /// A session for a document.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("8E02EBFA-D3A2-4D5C-A3CA-D8B425EA19BC")]
    public interface IWDEDocSession_R4 : IWDEDocSession_R3
    {
        /// <summary>
        /// Gets the name of the field which was focused when the document was rejected.
        /// </summary>
        string RejectField { get; }
        /// <summary>
        /// Gets the row id of the detail grid which was focused when the document was rejected. Value will be -1 for non detail grid fields.
        /// </summary>
        int RejectRow { get; }
    }

	public interface IWDEDocSessionInternal
	{
		string User {get; set;}
		string Task {get; set;}
		WDEOpenMode Mode {get; set;}
		string RejectCode {get; set;}
		string RejectDescription {get; set;}
		int SessionID {get; set;}
		string Location {get; set;}
        DateTime StartTime { get; set;}
        DateTime EndTime { get; set;}
        TimeSpan Duration { get; set; }
		WDESessionStatus Status {get; set;}
		void SaveSnapshot(XmlTextWriter xmlWriter);
        string RejectField { get; set; }
        int RejectRow { get; set; }
	}
}
