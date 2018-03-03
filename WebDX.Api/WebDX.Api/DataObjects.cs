using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Text;
using System.Drawing;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace WebDX.Api
{
	/// <summary>
	/// Provides security safe access to the current version info. AssemblyInfo.cs also uses this class so that everything is in sync.
	/// </summary>
#if DEBUG
	public struct VersionInfo
#else
	internal struct VersionInfo
#endif
    {
        //Set this variable to assign the version for the API.
        public const string VersionNumber = "4.0.1.0";

        //Maintain this list of released versions with each new release.
        private static List<Version> _validVersions = new List<Version>(new Version[] {
            new Version(1, 0),
            new Version(1, 1),
            new Version(1, 2),
            new Version(1, 3),
			new Version(1, 4),
			new Version(2, 0),
            new Version(2, 1),
            new Version(2, 2),
            new Version(2, 3),
            new Version(2, 4),
            new Version(2, 5),
            new Version(2, 6),
            new Version(2, 7),
            new Version(2, 8),
			new Version(2, 9),
            new Version(3, 0),
			new Version(3, 1),
            new Version(3, 2),
			new Version(3, 3),
			new Version(3, 4),
			new Version(3, 5),
            new Version(4, 0)});

		private static string _targetVersionNumber;

		/// <summary>
		/// This property represents the target version number.
		/// </summary>
		public static string TargetVersionNumber
		{
			get 
			{
				if (!string.IsNullOrEmpty(_targetVersionNumber))
				{
					return _targetVersionNumber;
				}
				else
				{
					return VersionNumber;
				}
			}
			set
			{
				if (value != null)
				{
                    Version v = new Version(value);
                    Version valid = _validVersions.Find(new Predicate<Version>(delegate(Version apiVersion)
                    {
                        return apiVersion.Major == v.Major && apiVersion.Minor == v.Minor;
                    }));

                    if (valid == null)
                        throw new WDEException("API00040", new object[] { value });
 
					_targetVersionNumber = value;
				}
			}
		}
	}

	public class WDEDataSet : IWDEDataSet_R2
	{
		private bool m_Active;
		private string m_FileName;
		private IWDEProject m_Project;
		private IWDESessions m_Sessions;
		private bool m_PersistFlags;
		private IWDEDocuments m_Documents;
		private string m_ProjectFile;
		private bool m_DisplayDeletedRows;
		private bool m_UserClient;

		private WDEDataSet(bool userClient)
		{
			m_UserClient = userClient;
			m_Sessions = WDESessions.Create(this);
			m_Documents = WDEDocuments.Create(this);
			m_FileName = "";
			m_ProjectFile = "";
			m_DisplayDeletedRows = false;
			VersionHelper.Initialize();
		}

		public static IWDEDataSet Create()
		{
			return new WDEDataSet(false) as IWDEDataSet;
		}

		public static IWDEDataSet Create(bool userClient)
		{
			return new WDEDataSet(userClient) as IWDEDataSet;
		}

		public static void WriteSchema(Stream output)
		{
			XmlSchema schema = new XmlSchema();
			schema.Namespaces.Add("xs", SchemaHelpers.XSD_NAMESPACE);
			schema.Namespaces.Add("wdedata", SchemaHelpers.TARGET_NAMESPACE);
			schema.TargetNamespace = SchemaHelpers.TARGET_NAMESPACE;
            schema.ElementFormDefault = XmlSchemaForm.Qualified;

			XmlSchemaElement dsElement = new XmlSchemaElement();
			dsElement.Name = "DataSet";

			XmlSchemaComplexType complexType = new XmlSchemaComplexType();
			dsElement.SchemaType = complexType;

			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexType.Particle = seq;

			List<Type> usedEnums = new List<Type>();
			XmlSchemaElement recordElement = WDERecord.GetSchema(usedEnums);
			seq.Items.Add(WDEDocument.GetSchema(usedEnums));

			foreach (Type t in usedEnums)
			{
				XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();
				simpleType.Name = t.Name;

				XmlSchemaSimpleTypeRestriction rest = new XmlSchemaSimpleTypeRestriction();
				rest.BaseTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
				simpleType.Content = rest;

				foreach (string elementName in Enum.GetNames(t))
				{
					XmlSchemaEnumerationFacet ef = new XmlSchemaEnumerationFacet();
					ef.Value = elementName;
					rest.Facets.Add(ef);
				}

				schema.Items.Add(simpleType);
			}

			XmlSchemaSimpleType wdeBool = new XmlSchemaSimpleType();
			wdeBool.Name = "WDEBoolean";
			XmlSchemaSimpleTypeRestriction r = new XmlSchemaSimpleTypeRestriction();
			r.BaseTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			wdeBool.Content = r;
			XmlSchemaPatternFacet p = new XmlSchemaPatternFacet();
			p.Value = string.Format("(?i:{0}|{1})", bool.TrueString, bool.FalseString);
			r.Facets.Add(p);
			schema.Items.Add(wdeBool);

			schema.Items.Add(recordElement);
			schema.Items.Add(dsElement);

			schema.Write(output);
			output.Flush();
		}

		public static IWDEDataSet CreateInstance()
		{
			return Create();
		}

		public bool UserClient
		{
			get { return m_UserClient; }
		}

		#region IWDEDataSet Members

		public bool DisplayDeletedRows
		{
			get
			{
				return m_DisplayDeletedRows;
			}
		}

		public IWDEDocumentDefs DocumentDefs
		{
			get
			{
				if (m_Project == null)
						throw new WDEException("API00001");
				return m_Project.DocumentDefs;
			}
		}

		public IWDEDocuments Documents
		{
			get
			{
				return m_Documents;
			}
		}

		public bool BOF
		{
			get
			{
				return m_Documents.BOF;
			}
		}

		public bool EOF
		{
			get
			{
				return m_Documents.EOF;
			}
		}

		public int Count
		{
			get
			{
				return m_Documents.Count;
			}
		}

		public IWDEDocument Current
		{
			get
			{
				return m_Documents.Current;
			}
		}

		public string AltDCN
		{
			get
			{
				return m_Documents.AltDCN;
			}
			set
			{
				m_Documents.AltDCN = value;
			}
		}

		public string DCN
		{
			get
			{
				return m_Documents.DCN;
			}
			set
			{
				m_Documents.DCN = value;
			}
		}

		public string DocType
		{
			get
			{
				return m_Documents.DocType;
			}
		}

		public IWDEImages Images
		{
			get
			{
				return m_Documents.Images;
			}
		}

		public IWDERecords Records
		{
			get
			{
				return m_Documents.Records;
			}
		}

		public IWDEDocSessions DocSessions
		{
			get
			{
				return m_Documents.Sessions;
			}
		}

		public string RejectCode
		{
			get
			{
				return m_Documents.RejectCode;
			}
			set
			{
				m_Documents.RejectCode = value;
			}
		}

		public string RejectDescription
		{
			get
			{
				return m_Documents.RejectDescription;
			}
			set
			{
				m_Documents.RejectDescription = value;
			}
		}

		public WebDX.Api.WDESessionStatus Status
		{
			get
			{
				return m_Documents.Status;
			}
		}

		public IWDEProject Project
		{
			get
			{
				return m_Project;
			}
			set
			{
				m_ProjectFile = "";
				m_Project = value;
			}
		}

		[VersionPropertyFilter("1.0.0.0", "1.4.999.999", "Sessions", "IWDEDataSet")]
		public IWDESessions Sessions
		{
			get
			{
				return m_Sessions;
			}
		}

		public string About
		{
			get
			{
				return "Version " + VersionInfo.VersionNumber;
			}
			set
			{
				// Do nothing. This is here so that the Delphi Property Manager will show this property.
			}
		}

		public string FileName
		{
			get
			{
				return m_FileName;
			}
			set
			{
				if (value == null)
					m_FileName = "";
				else
					m_FileName = value;
			}
		}

		public string ProjectFile
		{
			get
			{
				return m_ProjectFile;
			}
			set
			{
				string newValue = value;
				if (newValue == null)
					newValue = "";

				if (m_ProjectFile != newValue)
				{
					m_Project = null;
					if (newValue != "")
					{
						try
						{
							m_Project = WDEProject.Create();
							m_Project.LoadFromFile(newValue);
							m_ProjectFile = newValue;
						}
						catch (Exception ex)
						{
							System.Diagnostics.Debug.WriteLine(ex.ToString());
							m_Project = null;
							m_ProjectFile = "";
							throw;
						}
					}
				}
			}
		}

		public bool PersistFlags
		{
			get
			{
				return m_PersistFlags;
			}

			set
			{
				m_PersistFlags = value;
			}
		}

		public string DataLossErrors
		{
			get
			{
				return VersionHelper.IsDataLost ? VersionHelper.LostData : null;
			}
		}

		public void Clear()
		{
			CloseSession();
		}

		public void ClearData()
		{
			for (int i = 0; i < m_Documents.Count; i++)
			{
				m_Documents[i].ClearData();
			}
			IWDESessionsInternal isess = (IWDESessionsInternal)m_Sessions;
			isess.Clear();
			GC.Collect();
		}

		public void CreateData(string User, string Task, WebDX.Api.WDEOpenMode Mode)
		{
			CreateData(User, Task, Mode, "");
		}

		public void CreateData(string User, string Task, WebDX.Api.WDEOpenMode Mode, string Location)
		{
			if (m_Project == null)
					throw new WDEException("API00001");
			if (User == null)
				throw new ArgumentNullException("User","User cannot be null");				
			if (Task == null)
				throw new ArgumentNullException("Task", "Task cannot be null");				
			if (Location == null)
				throw new ArgumentNullException("Location", "Location cannot be null");				

			CloseSession();
			NewSession(User, Task, Mode, Location);
			IWDESessionInternal isess = (IWDESessionInternal)Sessions[0];
			isess.IsCreateSession = true;
		}

		public void Open(string User, string Task, WebDX.Api.WDEOpenMode Mode)
		{
			Open(User, Task, Mode, "");
		}

		public void Open(string User, string Task, WebDX.Api.WDEOpenMode Mode, string Location)
		{
			if (m_FileName == "")			
					throw new WDEException("API00021");
			
			if (User == null)
				throw new ArgumentNullException("User", "User cannot be null");				
			if (Task == null)
				throw new ArgumentNullException("Task", "Task cannot be null");				
			if (Location == null)
				throw new ArgumentNullException("Location", "Location cannot be null");				

			string SaveFileName = m_FileName;
			LoadFromFile(User, Task, Mode, Location, m_FileName);
			m_FileName = SaveFileName;
		}

		public void Close()
		{
			if (m_FileName == "")
					throw new WDEException("API00021");
			SaveToFile(m_FileName);
			CloseSession();
		}

		public void LoadFromFile(string User, string Task, WebDX.Api.WDEOpenMode Mode, string FileName)
		{
			LoadFromFile(User, Task, Mode, "", FileName);
		}

		public void LoadFromFile(string User, string Task, WebDX.Api.WDEOpenMode Mode, string Location, string FileName)
		{
			FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			try
			{
				LoadFromStream(User, Task, Mode, Location, fs);
				m_FileName = FileName;
			}
			finally
			{
				fs.Close();
			}
		}

		public void LoadFromStream(string User, string Task, WebDX.Api.WDEOpenMode Mode, System.Runtime.InteropServices.ComTypes.IStream aStream)
		{
			LoadFromStream(User, Task, Mode, "", aStream);
		}

		public void LoadFromStream(string User, string Task, WebDX.Api.WDEOpenMode Mode, System.IO.Stream aStream)
		{
			LoadFromStream(User, Task, Mode, "", aStream);
		}

		public void LoadFromStream(string User, string Task, WebDX.Api.WDEOpenMode Mode, string Location, System.Runtime.InteropServices.ComTypes.IStream aStream)
		{
			if (aStream == null)
				throw new ArgumentNullException("aStream", "aStream cannot be null");				

			MemoryStream ms = new MemoryStream();
			try
			{
				Utils.UCOMIStreamToStream(aStream, ms);

				ms.Seek(0, SeekOrigin.Begin);
				LoadFromStream(User, Task, Mode, Location, ms);
			}
			finally
			{
				ms.Close();
			}
		}

		public void LoadFromStream(string User, string Task, WebDX.Api.WDEOpenMode Mode, string Location, System.IO.Stream aStream)
		{
			if (User == null)
				throw new ArgumentNullException("User", "User cannot be null");
				
			if (Task == null)
				throw new ArgumentNullException("Task", "Task cannot be null");
				
			if (Location == null)
				throw new ArgumentNullException("Location", "Location cannot be null");
				
			if (aStream == null)
				throw new ArgumentNullException("aStream", "aStream cannot be null");
				

			m_FileName = "";
			CloseSession();

			// Reset LostData 2.6.0.0
			VersionHelper.Initialize(); 

			aStream.Seek(0, SeekOrigin.Begin);
			try
			{
				ReadStream(aStream);

				if (Mode != WDEOpenMode.Resume)
					NewSession(User, Task, Mode, Location);
				else
					ResumeSession();

				if (Documents.Count > 0)
				{
					VersionInfo.TargetVersionNumber = Documents[0].APIVersion;
				}

				// Now that the data file is loaded completely, 
				// create missing fields in each document's records BPSENT - 1170
                foreach (IWDEDocument doc in Documents)
                    foreach (IWDERecordInternal rec in doc.Records)
                        rec.CreateMissingFields();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
				Clear();
				throw;
			}
		}

		public void LoadFromBytes(string User, string Task, WDEOpenMode Mode, string Location, byte[] Bytes)
		{
			MemoryStream loadStream = new MemoryStream(Bytes);
			try
			{
				LoadFromStream(User, Task, Mode, Location, loadStream);
			}
			finally
			{
				loadStream.Close();
			}
		}

		public void LoadFromBytes(string User, string Task, WDEOpenMode Mode, byte[] Bytes)
		{
			LoadFromBytes(User, Task, Mode, "", Bytes);
		}

		public void SaveToFile(string FileName)
		{
			SaveToFile(FileName, null);
		}

		public void SaveToFile(string FileName, string FileVersion)
		{
			if (FileName == null)
				throw new ArgumentNullException("FileName", "FileName cannot be null");				

			FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None);
			try
			{
				SaveToStream(fs, FileVersion);
			}
			finally
			{
				fs.Close();
			}
		}

		public void SaveToStream(System.Runtime.InteropServices.ComTypes.IStream aStream)
		{
			SaveToStream(aStream, null);
		}

		public void SaveToStream(System.Runtime.InteropServices.ComTypes.IStream aStream, string FileVersion)
		{
			MemoryStream ms = new MemoryStream();
			try
			{
				SaveToStream(ms, FileVersion);
				ms.Seek(0, SeekOrigin.Begin);

				Utils.StreamToUCOMIStream(ms, aStream);
			}
			finally
			{
				ms.Close();
			}
		}

		public void SaveToStream(System.IO.Stream aStream)
		{
			SaveToStream(aStream, null);
		}

		public void SaveToStream(System.IO.Stream aStream, string FileVersion)
		{
			if (!m_Active)
				throw new WDEException("API00022");

			VersionInfo.TargetVersionNumber = FileVersion;

			CheckCurrent();
			if (m_Sessions.Count > 0 && !m_UserClient)
			{
				IWDESessionInternal isess = (IWDESessionInternal)m_Sessions[0];
				isess.EndTime = DateTime.Now;
			}

			GC.Collect();

			// save to memory first so that we don't attach higher level objects to the calling stream.
			MemoryStream memStream = new MemoryStream();
			XmlTextWriter XmlWriter = new XmlTextWriter(memStream, new UTF8Encoding(false));
			try
			{
				XmlWriter.Formatting = Formatting.Indented;
				XmlWriter.WriteStartDocument(true);
				XmlWriter.WriteStartElement("DataSet");
				SaveDocuments(XmlWriter);
				SaveSessions(XmlWriter);
				XmlWriter.WriteEndElement();
				XmlWriter.WriteEndDocument();
			}
			finally
			{
				XmlWriter.Close();
			}
			MemoryStream saveStream = new MemoryStream(memStream.ToArray());

			Utils.CopyStream(saveStream, aStream);
			saveStream = null;
			XmlWriter = null;
			memStream = null;
			GC.Collect();
		}

		public byte[] SaveToBytes()
		{
			return SaveToBytes(null);
		}

		public byte[] SaveToBytes(string FileVersion)
		{
			MemoryStream saveStream = new MemoryStream();
			try
			{
				SaveToStream(saveStream, FileVersion);
				return saveStream.ToArray();
			}
			finally
			{
				saveStream.Close();
			}
		}

		public void First()
		{
			m_Documents.First();
		}

		public void Last()
		{
			m_Documents.Last();
		}

		public void Next()
		{
			m_Documents.Next();
		}

		public void Prior()
		{
			m_Documents.Prior();
		}

		public IWDEDocument Append(string DocType)
		{
			return m_Documents.Append(DocType);
		}

		public IWDEDocument Append(IWDEDocument Doc)
		{
			return m_Documents.Append(Doc);
		}

		public IWDEDocument Insert(string DocType)
		{
			if (Current != null)
				return m_Documents.Insert(Current.Index, DocType);
			else
				return m_Documents.Append(DocType);
		}

		public IWDEDocument Insert(int Index, string DocType)
		{
			return m_Documents.Insert(Index, DocType);
		}

		public void Delete()
		{
			m_Documents.Delete();
		}

		public void LoadDocumentFromFile(string FileName)
		{
			if (FileName == null)
				throw new ArgumentNullException("FileName", "FileName cannot be null");				
			FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			try
			{
				LoadDocumentFromStream(fs);
			}
			finally
			{
				fs.Close();
			}
		}

		public void LoadDocumentFromStream(System.Runtime.InteropServices.ComTypes.IStream aStream)
		{
			if (aStream == null)
				throw new ArgumentNullException("aStream", "aStream cannot be null");
				
			MemoryStream ms = new MemoryStream();
			try
			{
				Utils.UCOMIStreamToStream(aStream, ms);
				ms.Seek(0, SeekOrigin.Begin);
				LoadDocumentFromStream(ms);
			}
			finally
			{
				ms.Close();
			}
		}

		public void LoadDocumentFromStream(System.IO.Stream aStream)
		{
			IWDEDocumentsInternal idoc = (IWDEDocumentsInternal)Documents;
			idoc.LoadDocumentFromStream(aStream);
		}

		public void MergeDocumentFromStream(Stream aStream, bool displayDeletedRows)
		{
			m_DisplayDeletedRows = displayDeletedRows;
			MergeDocumentFromStream(aStream);
		}

		public void MergeDocumentFromStream(System.IO.Stream aStream)
		{
			IWDEDocumentsInternal idoc = (IWDEDocumentsInternal)Documents;
			idoc.MergeDocumentFromStream(aStream);
			m_Active = true;
		}

		public void SaveDocumentToFile(string FileName)
		{
			SaveDocumentToFile(FileName, null);
		}

		public void SaveDocumentToFile(string FileName, string FileVersion)
		{
			VersionInfo.TargetVersionNumber = FileVersion;
			CheckCurrent();
			Documents.Current.SaveToFile(FileName, true);
		}

		public void SaveDocumentToStream(System.Runtime.InteropServices.ComTypes.IStream aStream)
		{
			SaveDocumentToStream(aStream, null);
		}

		public void SaveDocumentToStream(System.Runtime.InteropServices.ComTypes.IStream aStream, string FileVersion)
		{
			VersionInfo.TargetVersionNumber = FileVersion;
			CheckCurrent();
			Documents.Current.SaveToStream(aStream, true);
		}

		public void SaveDocumentToStream(System.IO.Stream aStream)
		{
			SaveDocumentToStream(aStream, null);
		}

		public void SaveDocumentToStream(System.IO.Stream aStream, string FileVersion)
		{
			VersionInfo.TargetVersionNumber = FileVersion;

			CheckCurrent();
			Documents.Current.SaveToStream(aStream, true);
		}

		#region New overloads for DisplayDeletedRows
		public void LoadDocumentFromFile(string FileName, bool displayDeletedRows)
		{
			m_DisplayDeletedRows = displayDeletedRows;
			LoadDocumentFromFile(FileName);
		}

		public void LoadDocumentFromStream(System.Runtime.InteropServices.ComTypes.IStream aStream, bool displayDeletedRows)
		{
			m_DisplayDeletedRows = displayDeletedRows;
			LoadDocumentFromStream(aStream);
		}

		public void LoadDocumentFromStream(System.IO.Stream aStream, bool displayDeletedRows)
		{
			m_DisplayDeletedRows = displayDeletedRows;
			LoadDocumentFromStream(aStream);
		}

		public void LoadFromFile(string User, string Task, WDEOpenMode Mode, string FileName, bool displayDeletedRows)
		{
			m_DisplayDeletedRows = displayDeletedRows;
			LoadFromFile(User, Task, Mode, "", FileName);
		}

		public void LoadFromFile(string User, string Task, WDEOpenMode Mode, string Location, string FileName, bool displayDeletedRows)
		{
			m_DisplayDeletedRows = displayDeletedRows;
			LoadFromFile(User, Task, Mode, Location, FileName);
		}

		public void LoadFromStream(string User, string Task, WebDX.Api.WDEOpenMode Mode, System.Runtime.InteropServices.ComTypes.IStream aStream, bool displayDeletedRows)
		{
			LoadFromStream(User, Task, Mode, "", aStream, displayDeletedRows);
		}


		public void LoadFromStream(string User, string Task, WebDX.Api.WDEOpenMode Mode, System.IO.Stream aStream, bool displayDeletedRows)
		{
			LoadFromStream(User, Task, Mode, "", aStream, displayDeletedRows);
		}

		public void LoadFromStream(string User, string Task, WebDX.Api.WDEOpenMode Mode, string Location, System.Runtime.InteropServices.ComTypes.IStream aStream, bool displayDeletedRows)
		{
			m_DisplayDeletedRows = displayDeletedRows;
			LoadFromStream(User, Task, Mode, Location, aStream);
		}

		public void LoadFromStream(string User, string Task, WebDX.Api.WDEOpenMode Mode, string Location, System.IO.Stream aStream, bool displayDeletedRows)
		{
			m_DisplayDeletedRows = displayDeletedRows;
			LoadFromStream(User, Task, Mode, Location, aStream);
		}

		public void LoadFromBytes(string User, string Task, WDEOpenMode Mode, byte[] Bytes, bool displayDeletedRows)
		{
			LoadFromBytes(User, Task, Mode, "", Bytes, displayDeletedRows);
		}

		public void LoadFromBytes(string User, string Task, WDEOpenMode Mode, string Location, byte[] Bytes, bool displayDeletedRows)
		{
			m_DisplayDeletedRows = displayDeletedRows;
			LoadFromBytes(User, Task, Mode, Location, Bytes);
		}

		#endregion

		#endregion

		#region Private Methods

		private void CloseSession()
		{
			m_Active = false;
			m_Documents.Clear();
			IWDESessionsInternal isess = (IWDESessionsInternal)m_Sessions;
			isess.Clear();

			GC.Collect();
		}

		private void NewSession(string User, string Task, WDEOpenMode Mode, string Location)
		{
			if (User == null)
				throw new ArgumentNullException("User","User cannot be null");				
			if (Task == null)
				throw new ArgumentNullException(Task,"Task cannot be null");				
			if (Location == null)
				throw new ArgumentNullException(Location,"Location cannot be null");				

			IWDESessionsInternal isessions = (IWDESessionsInternal)m_Sessions;
			IWDESession_R1 session = isessions.Add(User, Task, Mode, Location);
			for (int i = 0; i < m_Documents.Count; i++)
			{
				IWDEDocSessionsInternal idocsessions = (IWDEDocSessionsInternal)m_Documents[i].Sessions;
				idocsessions.Add(User, Task, Mode, session.SessionID, Location);
			}
			m_Active = true;
			m_Documents.First();
		}

		private void ReadStream(Stream aStream)
		{
			StreamReader sr = new StreamReader(aStream, true);
			XmlTextReader XmlReader = new XmlTextReader(sr);
			try
			{
				XmlReader.MoveToContent();
				if (XmlReader.Name != "DataSet")
					throw new WDEException("API00020", new object[] { XmlReader.Name });

				IWDEXmlPersist idocspersist = (IWDEXmlPersist)m_Documents;
				idocspersist.ReadFromXml(XmlReader);
				IWDEXmlPersist isesspersist = (IWDEXmlPersist)m_Sessions;
				isesspersist.ReadFromXml(XmlReader);
				XmlReader.ReadEndElement();

				FixSessionGaps();
			}
			finally
			{
				XmlReader.Close();
			}
		}

		private void SaveDocuments(XmlTextWriter XmlWriter)
		{
			IWDEXmlPersist idocspersist = (IWDEXmlPersist)m_Documents;
			idocspersist.WriteToXml(XmlWriter);
		}

		private void SaveSessions(XmlTextWriter XmlWriter)
		{
			if (!VersionHelper.FilterPropertyOrCollection("IWDEDataSet.Sessions", VersionInfo.TargetVersionNumber))
			{
				IWDEXmlPersist isesspersist = (IWDEXmlPersist)m_Sessions;
				isesspersist.WriteToXml(XmlWriter);
			}
		}

		private void ResumeSession()
		{
			m_Active = true;
			m_Documents.First();
		}

		private void CheckCurrent()
		{
			if (Documents.Current == null)
			{
				if (Documents.Count == 0)
					throw new WDEException("API00017");
				else
					throw new WDEException("API00016");
			}
		}

		private void FixSessionGaps()
		{
			Hashtable sessionMap = new Hashtable();

			if (Sessions.Count > 0)
			{
				int lastSession = Sessions[Sessions.Count - 1].SessionID;
				for (int i = Sessions.Count - 2; i > -1; i--)
				{
					if (Sessions[i].SessionID != lastSession + 1)
					{
						sessionMap.Add(Sessions[i].SessionID, lastSession + 1);
						IWDESessionInternal isess = (IWDESessionInternal)Sessions[i];
						isess.SessionID = lastSession + 1;
					}
					lastSession = Sessions[i].SessionID;
				}

				if (sessionMap.Count > 0)
					FixDocSessions(sessionMap);
			}
		}

		private void FixDocSessions(Hashtable sessionMap)
		{
			for (int i = 0; i < m_Documents.Count; i++)
			{
				for (int j = 0; j < m_Documents[i].Sessions.Count; j++)
				{
					if (sessionMap.ContainsKey(m_Documents[i].Sessions[j].SessionID))
					{
						IWDEDocSessionInternal idoc = (IWDEDocSessionInternal)m_Documents[i].Sessions[j];
						idoc.SessionID = (int)sessionMap[m_Documents[i].Sessions[j].SessionID];
					}
				}

				FixRecordSessions(m_Documents[i].Records, sessionMap);
			}
		}

		private void FixRecordSessions(IWDERecords records, Hashtable sessionMap)
		{
			for (int i = 0; i < records.Count; i++)
			{
				IWDERecord rec = records[i];
				for (int j = 0; j < rec.Fields.Count; j++)
				{
					IWDEField_R1 field = rec.Fields[j];
					for (int k = 0; k < field.Revisions.Count; k++)
					{
						IWDERevision rev = field.Revisions[k];
						if (sessionMap.ContainsKey(rev.SessionID))
						{
							IWDERevisionInternal irev = (IWDERevisionInternal)rev;
							irev.SessionID = (int)sessionMap[rev.SessionID];
						}
					}
				}

				FixRecordSessions(records[i].Records, sessionMap);
			}
		}

		#endregion

	}

	public class WDEDocuments : IWDEDocuments, IWDEDocumentsInternal, IWDEXmlPersist, IWDECollection, IEnumerable
	{
		private ArrayList m_Docs;
		private IWDEDataSet m_DataSet;
		private bool m_BOF;
		private bool m_EOF;
		private int m_Index;

		private WDEDocuments(IWDEDataSet Owner)
		{
			m_DataSet = Owner;
			m_Index = -1;
			m_Docs = new ArrayList();
			m_BOF = true;
			m_EOF = true;
		}

		public static IWDEDocuments Create(IWDEDataSet Owner)
		{
			return new WDEDocuments(Owner) as IWDEDocuments;
		}

		public static IWDEDocuments CreateInstance(IWDEDataSet Owner)
		{
			return Create(Owner);
		}

		#region IWDEDocuments Members

		public bool BOF
		{
			get
			{
				return m_BOF;
			}
		}

		public bool EOF
		{
			get
			{
				return m_EOF;
			}
		}

		public IWDEDocument Current
		{
			get
			{
				if (m_Index != -1)
					return (IWDEDocument)m_Docs[m_Index];
				else
					return null;
			}
		}

		public IWDEDataSet DataSet
		{
			get
			{
				return m_DataSet;
			}
		}

		public string AltDCN
		{
			get
			{
				CheckCurrent();
				return Current.AltDCN;
			}

			set
			{
				CheckCurrent();
				Current.AltDCN = value;
			}
		}

		public string DCN
		{
			get
			{
				CheckCurrent();
				return Current.DCN;
			}

			set
			{
				CheckCurrent();
				Current.DCN = value;
			}
		}

		public string DocType
		{
			get
			{
				CheckCurrent();
				return Current.DocType;
			}
		}

		public IWDEImages Images
		{
			get
			{
				CheckCurrent();
				return Current.Images;
			}
		}

		public IWDERecords Records
		{
			get
			{
				CheckCurrent();
				return Current.Records;
			}
		}

		public int Index
		{
			get
			{
				return m_Index;
			}
			set
			{
				SetIndex(value);
			}
		}

		public IWDEDocument this[int Index]
		{
			get
			{
				return (IWDEDocument)m_Docs[Index];
			}
		}

		public IWDEDocSessions Sessions
		{
			get
			{
				CheckCurrent();
				return Current.Sessions;
			}
		}

		public string RejectCode
		{
			get
			{
				CheckCurrent();
				return Current.RejectCode;
			}
			set
			{
				CheckCurrent();
				Current.RejectCode = value;
			}
		}

		public string RejectDescription
		{
			get
			{
				CheckCurrent();
				return Current.RejectDescription;
			}
			set
			{
				CheckCurrent();
				Current.RejectDescription = value;
			}
		}

		public WebDX.Api.WDESessionStatus Status
		{
			get
			{
				CheckCurrent();
				return Current.Status;
			}
			set
			{
				CheckCurrent();
				IWDEDocumentInternal idoc = (IWDEDocumentInternal)Current;
				idoc.Status = value;
			}
		}

        public string DocNotes
        {
            get
            {
                CheckCurrent();
                return ((IWDEDocument_R3)Current).DocNotes;
            }
            set
            {
                CheckCurrent();
                ((IWDEDocument_R3)Current).DocNotes = value;
            }
        }

        public bool AllowRescan
        {
            get
            {
                CheckCurrent();
                return ((IWDEDocument_R4)Current).AllowRescan;
            }
            set
            {
                CheckCurrent();
                ((IWDEDocument_R4)Current).AllowRescan = value;
            }
        }

        public bool AllowReclassify
        {
            get
            {
                CheckCurrent();
                return ((IWDEDocument_R4)Current).AllowReclassify;
            }
            set
            {
                CheckCurrent();
                ((IWDEDocument_R4)Current).AllowReclassify = value;
            }
        }

        public IWDERejectCodes RejectCodes
        {
            get
            {
                CheckCurrent();
                return ((IWDEDocument_R4)Current).RejectCodes;
            }
        }

        public int Count
		{
			get
			{
				return m_Docs.Count;
			}
		}

        public void Clear()
		{
			m_Index = -1;
			m_BOF = true;
			m_EOF = true;
			m_Docs.Clear();
		}

		public void First()
		{
			SetIndex(Math.Min(0, Count - 1));
			m_BOF = true;
			m_EOF = (Count == 0);
		}

		public void Last()
		{
			SetIndex(Count - 1);
		}

		public void Next()
		{
			CheckCurrent();
			if (m_Index < Count - 1)
				Index += 1;
			else
				m_EOF = true;
		}

		public void Prior()
		{
			CheckCurrent();
			if (m_Index > 0)
				Index -= 1;
			else
				m_BOF = true;
		}

		public IWDEDocument Append(string DocType)
		{
			if (DocType == null)
				throw new ArgumentNullException("DocType", "DocType cannot be null");				

			if (DataSet.Sessions.Count == 0)
				throw new WDEException("API00019");

			int idx = DataSet.DocumentDefs.Find(DocType);
			if (idx != -1)
			{
				IWDEDocument result = WDEDocument.Create(this);
				m_Docs.Add(result);
				IWDEDocumentInternal idoc = (IWDEDocumentInternal)result;
				idoc.DocType = DocType;
				idoc.StoredDocType = DocType;
				IWDEDocSessionsInternal isess = (IWDEDocSessionsInternal)result.Sessions;
				isess.Add(DataSet.Sessions[0].User,
					DataSet.Sessions[0].Task,
					DataSet.Sessions[0].Mode,
					DataSet.Sessions[0].SessionID,
					DataSet.Sessions[0].Location);
				Index = Count - 1;
				return result;
			}
			else
				throw new WDEException("API00015", new object[] { DocType });

		}

		public IWDEDocument Append(IWDEDocument Document)
		{
			if (DataSet.Sessions.Count == 0)
				throw new WDEException("API00019");

			int idx = DataSet.DocumentDefs.Find(Document.DocType);
			if (idx != -1)
			{
				MemoryStream ms = new MemoryStream();
				try
				{
					Document.SaveToStream(ms);
					IWDEDocument result = WDEDocument.Create(this);
					m_Docs.Add(result);
					SetIndex(Count - 1);
					ms.Seek(0, SeekOrigin.Begin);
					result.LoadFromStream(ms);
					return result;
				}
				finally
				{
					ms.Close();
				}
			}
			else
				throw new WDEException("API00015", new object[] { DocType });
	
		}

		public IWDEDocument Insert(int Index, string DocType)
		{
			if (((Count > 0) && ((Index > Count - 1) || (Index < 0))) ||
				((Count == 0) && (Index != 0)))
				throw new WDEException("API00012", new object[] { "IWDEDocuments", Index, Count });

			IWDEDocument result = Append(DocType);
			result.Index = Index;
			return result;
		}

		public void Delete()
		{
			CheckCurrent();
			m_Docs.RemoveAt(m_Index);
			if (Count > 0)
				SetIndex(Math.Min(m_Index, Count - 1));
			else
				Clear();
		}

		public int Find(string DCN)
		{
			if (DCN == null)
				throw new ArgumentNullException("DCN", "DCN cannot be null");				

			int result = -1;
			for (int i = 0; i < m_Docs.Count; i++)
			{
				IWDEDocument doc = (IWDEDocument)m_Docs[i];
				if (string.Compare(doc.DCN, DCN, true) == 0)
				{
					result = i;
					Index = i;
					break;
				}
			}
			return result;
		}
		#endregion

		#region IWDEDocumentsInternal Members

		public void LoadDocumentFromStream(Stream aStream)
		{
			if (DataSet.Sessions.Count == 0)
					throw new WDEException("API00019");

			// Reset LostData 2.6.0.0
			VersionHelper.Initialize(); 

			MergeDocumentFromStream(aStream);
			IWDEDocument doc = (IWDEDocument)m_Docs[m_Docs.Count - 1];

			if ((DataSet.Sessions is IWDESessionsInternal) && (DataSet.Sessions[0].SessionID <= doc.Sessions[0].SessionID))
			{
				IWDESessionInternal s = (IWDESessionInternal)DataSet.Sessions[0];
				int targetId = doc.Sessions[0].SessionID + 1;
				UpdatePreviousDocSessionIDs(targetId);
				s.SessionID = targetId;
			}

			IWDEDocSessionsInternal isess = (IWDEDocSessionsInternal)doc.Sessions;
			isess.Add(DataSet.Sessions[0].User, DataSet.Sessions[0].Task, DataSet.Sessions[0].Mode, DataSet.Sessions[0].SessionID, DataSet.Sessions[0].Location);

			// Now that the data file is loaded completely, 
			// create missing fields in each document's records BPSENT - 1170
            foreach (IWDERecordInternal rec in Records)
                rec.CreateMissingFields();
		}

		public void MergeDocumentFromStream(Stream aStream)
		{
			IWDEDocument doc = WDEDocument.Create(this);
			doc.LoadFromStream(aStream);
			if (DataSet.Project != null)
			{
				if (DataSet.DocumentDefs.Find(doc.DocType) != -1)
				{
					m_Docs.Add(doc);
					SetIndex(Count - 1);
				}
				else
					throw new WDEException("API00015", new object[] { doc.DocType });
			}
			else
			{
				m_Docs.Add(doc);
				SetIndex(Count - 1);
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			// Get to an element
			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DataSet"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();
			}

			if ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Document"))
			{
				Clear();

				while ((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "Document"))
				{
					IWDEDocument doc = WDEDocument.Create(this);
					IWDEXmlPersist ipers = (IWDEXmlPersist)doc;
					ipers.ReadFromXml(XmlReader);
					m_Docs.Add(doc);
					SetIndex(0);

					XmlReader.MoveToContent();
					if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Document"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if (XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				

			for (int i = 0; i < Count; i++)
			{
				IWDEXmlPersist writedoc = (IWDEXmlPersist)m_Docs[i];
				writedoc.WriteToXml(XmlWriter);
			}
		}

		#endregion

		#region Private Members

		private void UpdatePreviousDocSessionIDs(int targetId)
		{
			//if (!((IWDESessionInternal)DataSet.Sessions[0]).IsCreateSession)
			//{
			int currentId = DataSet.Sessions[0].SessionID;

			for (int i = 0; i < m_Docs.Count - 1; i++)
			{
				IWDEDocSessionInternal isess = (IWDEDocSessionInternal)this[i].Sessions[0];
				if (isess.SessionID == currentId)
					isess.SessionID = targetId;
			}
			//}
		}

		private void SetIndex(int Value)
		{
			if (m_Index != Value)
			{
				if ((Value < 0) || (Value > Count - 1))
					throw new WDEException("API00012", new object[] { "IWDEDocuments", Value, Count });

				m_Index = Value;
				m_BOF = false;
				m_EOF = false;

				if (Current != null)
					Records.First();
			}
		}

		private void CheckCurrent()
		{
			if (Current == null)
			{
				if (Count == 0)
						throw new WDEException("API00017");
				else
						throw new WDEException("API00016");
			}
		}

		#endregion

		#region IWDECollection Members

		public int GetIndex(object Item)
		{
			return m_Docs.IndexOf(Item);
		}

		public void SetIndex(object Item, int NewIndex)
		{
			int index = GetIndex(Item);
			if (index != -1)
			{
				m_Docs.RemoveAt(index);
				m_Docs.Insert(NewIndex, Item);
			}
			else
				throw new WDEException("API90002", new object[] { "IWDEDocuments" });
		}

		public void RemoveAt(int Index)
		{
			m_Docs.RemoveAt(Index);
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			IEnumerable ienum = (IEnumerable)m_Docs;
			return ienum.GetEnumerator();
		}

		#endregion
	}

	public class WDEDocument : IWDEDocument_R4, IWDEDocumentInternal, IWDEXmlPersist, IWDERecordsOwner
	{
		private IWDEDocumentDef m_DocumentDef;
		private IWDEDocuments m_Documents;
		private string m_APIVersion;
		private IWDERecords m_Records;
		private IWDEImages m_Images;
		private IWDEDocSessions m_Sessions;
		private bool m_QIAutoAudit;
		private bool m_QIFocusAudit;
		private bool m_QISelected;
		private string m_StoredDocType;
		private string m_DCN;
		private string m_AltDCN;
		private string m_DocType;
		private string m_RejectCode;
		private string m_RejectDescription;
		private WDESessionStatus m_Status;
		private int m_SaveSessionID;
        private string m_RejectField;
        private int m_RejectRow;
        private string m_DocNotes;
        private bool m_AllowRescan;
        private bool m_AllowReclassify;
        private IWDERejectCodes m_RejectCodes;

        private WDEDocument(IWDEDocuments Owner)
		{
			m_Documents = Owner;
			m_Records = WDERecords.Create(this);
			m_Images = WDEImages.Create(this);
			m_Sessions = WDEDocSessions.Create(this);
            m_RejectCodes = WDERejectCodes.Create(this);

			m_APIVersion = "";
			m_StoredDocType = "";
			m_DCN = "";
			m_AltDCN = "";
			m_DocType = "";
			m_RejectCode = "";
			m_RejectDescription = "";
			m_Status = WDESessionStatus.None;
			m_SaveSessionID = -1;
            m_RejectField = "";
            m_RejectRow = -1;
            m_DocNotes = "";            
		}

		public static IWDEDocument Create(IWDEDocuments Owner)
		{
			return new WDEDocument(Owner) as IWDEDocument;
		}

		public static IWDEDocument CreateInstance(IWDEDocuments Owner)
		{
			return Create(Owner);
		}

		#region IWDEDocument Members

		public string APIVersion
		{
			get
			{
				if (m_APIVersion == "")
					return VersionInfo.VersionNumber;
				else
					return m_APIVersion;
			}
		}

		public int FlaggedFieldCount
		{
			get
			{
				int result = 0;
				for (int i = 0; i < Records.Count; i++)
					result += Records[i].FlaggedFieldCount;

				return result;
			}
		}

		public IWDEDocumentDef DocumentDef
		{
			get
			{
				if ((m_DocumentDef == null) || (string.Compare(m_DocumentDef.DocType, DocType, true) != 0))
				{
					if (DataSet == null)
							throw new WDEException("API90004");


					if (DataSet.Project != null)
					{
						int index = DataSet.Project.DocumentDefs.Find(DocType);
						if (index != -1)
						{
							m_DocumentDef = DataSet.Project.DocumentDefs[index];
						}
						else
							throw new WDEException("API00015", new object[] { DocType });

					}
					else
						throw new WDEException("API00001");
				}

				return m_DocumentDef;
			}
		}

		public string DCN
		{
			get
			{
				return m_DCN;
			}
			set
			{
				if (value == null)
					m_DCN = "";
				else
					m_DCN = value;
			}
		}

		public string AltDCN
		{
			get
			{
				return m_AltDCN;
			}
			set
			{
				if (value == null)
					m_AltDCN = "";
				else
					m_AltDCN = value;
			}
		}

		public string DocType
		{
			get
			{
				return m_DocType;
			}

			set
			{
				// only used by the internal interface
				if (value == null)
					m_DocType = "";
				else
					m_DocType = value;
			}
		}

		public IWDEImages Images
		{
			get
			{
				return m_Images;
			}
		}

		public IWDEDocSessions Sessions
		{
			get
			{
				return m_Sessions;
			}
		}

		public string RejectCode
		{
			get
			{
				return m_RejectCode;
			}
			set
			{
				string newValue = value;
				if (value == null)
					newValue = "";
				CheckSession();
				if (m_RejectCode != newValue)
				{
					if (newValue != "")
					{						
                        int index = m_RejectCodes.Find(newValue);
                        if (index != -1)
						{
							m_RejectCode = newValue;
							IWDEDocSessionInternal idocsess = (IWDEDocSessionInternal)Sessions[0];
							idocsess.RejectCode = newValue;                            
                            string desc = m_RejectCodes[index].Description;
                            m_RejectDescription = desc;
							idocsess.RejectDescription = desc;
							Status = WDESessionStatus.Rejected;
						}
						else
							throw new WDEException("API00023", new object[] { newValue });
					}
					else
					{
						m_RejectCode = newValue;
						IWDEDocSessionInternal idocsess = (IWDEDocSessionInternal)Sessions[0];
						idocsess.RejectCode = newValue;
						idocsess.RejectDescription = "";
						m_RejectDescription = "";
						Status = WDESessionStatus.None;
					}
				}
			}
		}

		public string RejectDescription
		{
			get
			{
				return m_RejectDescription;
			}
			set
			{
				string newValue = value;
				if (value == null)
					newValue = "";

				CheckSession();

				if (m_RejectCode != "")
				{
                    int index = m_RejectCodes.Find(RejectCode);
                    if (index != -1)
					{
                        if ((newValue == "") && (m_RejectCodes[index].RequireReason))
                            throw new WDEException("API00024", new object[] { RejectCode });
						m_RejectDescription = newValue;
						IWDEDocSessionInternal idocsess = (IWDEDocSessionInternal)Sessions[0];
						idocsess.RejectDescription = newValue;
					}
					else
						throw new WDEException("API00025", new object[] { RejectCode });
				}
				else
				{
					m_RejectDescription = newValue;
					IWDEDocSessionInternal idocsess = (IWDEDocSessionInternal)Sessions[0];
					idocsess.RejectDescription = newValue;
				}
			}
		}

		public bool Rejected
		{
			get
			{
				return Status == WDESessionStatus.Rejected;
			}
			set
			{
				if (value)
					Status = WDESessionStatus.Rejected;
				else
					Status = WDESessionStatus.None;
			}
		}

		[VersionPropertyFilter("1.2.0.0", null, "QIAutoAudit", "IWDEDocument")]
		public bool QIAutoAudit
		{
			get
			{
				return m_QIAutoAudit;
			}
			set
			{
				m_QIAutoAudit = value;
			}
		}

		[VersionPropertyFilter("1.2.0.0", null, "QIFocusAudit", "IWDEDocument")]
		public bool QIFocusAudit
		{
			get
			{
				return m_QIFocusAudit;
			}
			set
			{
				m_QIFocusAudit = value;
			}
		}

		public bool QISelected
		{
			get
			{
				return m_QISelected;
			}
			set
			{
				m_QISelected = value;
			}
		}

		[VersionPropertyFilter("1.4.0.0", null, "Status", "IWDEDocument")]
		public WebDX.Api.WDESessionStatus Status
		{
			get
			{
				return m_Status;
			}

			set
			{
				// only used by the internal interface
				IWDEDocSessionInternal idocsess = (IWDEDocSessionInternal)Sessions[0];
				idocsess.Status = value;
				m_Status = value;
				if (value != WDESessionStatus.Rejected)
				{
					m_RejectCode = "";
					m_RejectDescription = "";
				}
			}
		}

		public string StoredDocType
		{
			get
			{
				return m_StoredDocType;
			}
			set
			{
				if (value == null)
					m_StoredDocType = "";
				else
					m_StoredDocType = value;
			}
		}

		public IWDEDocuments ParentDocuments
		{
			get
			{
				return m_Documents;
			}
		}

		public IWDEDataSet DataSet
		{
			get
			{
				return m_Documents.DataSet;
			}
		}

		public IWDERecords Records
		{
			get
			{
				return m_Records;
			}
		}

		public int Index
		{
			get
			{
				IWDECollection icol = (IWDECollection)m_Documents;
				return icol.GetIndex((IWDEDocument)this);
			}
			set
			{
				IWDECollection icol = (IWDECollection)m_Documents;
				icol.SetIndex(this, value);
			}
		}
		
		public DateTime StartTime
		{
			get
			{
				return ((IWDEDocSession_R2)Sessions[0]).StartTime;
			}

			set
			{
				((IWDEDocSessionInternal)Sessions[0]).StartTime = value;
			}
		}
		
		public DateTime EndTime
		{
			get
			{
				return ((IWDEDocSession_R2)Sessions[0]).EndTime;
			}

			set
			{
				((IWDEDocSessionInternal)Sessions[0]).EndTime = value;
			}
		}

        [VersionPropertyFilter("3.2.1.0", null, "RejectField", "IWDEDocument")]
        public string RejectField
        {
            get
            {
                return m_RejectField;
            }
            set
            {
                string newValue = value;
                if (newValue == null)
                    newValue = "";
                CheckSession();

                if (m_RejectField != newValue)
                {
                    m_RejectField = newValue;                   
                    IWDEDocSessionInternal idocsess = (IWDEDocSessionInternal)Sessions[0];
                    idocsess.RejectField = newValue;
                }
            }
        }

        [VersionPropertyFilter("3.2.1.0", null, "RejectRow", "IWDEDocument")]
        public int RejectRow
        {
            get
            {
                return m_RejectRow;
            }
            set
            {
                int newValue = value;
                CheckSession();
                if (m_RejectRow != newValue)
                {
                    m_RejectRow = newValue;                    
                    IWDEDocSessionInternal idocsess = (IWDEDocSessionInternal)Sessions[0];
                    idocsess.RejectRow = newValue;
                }
            }
        }
        [VersionPropertyFilter("3.4.4.0", null, "DocNotes", "IWDEDocument")]
        public string DocNotes
        {
            get
            {
                return m_DocNotes;
            }
            set
            {
                if (value == null)
                    m_DocNotes = "";
                else
                    m_DocNotes = value;
            }
        }

        [VersionPropertyFilter("4.0.0.0", null, "AllowRescan", "IWDEDocument")]
        public bool AllowRescan
        {
            get
            {
                return m_AllowRescan;
            }
            set
            {
                m_AllowRescan = value;
            }
        }

        [VersionPropertyFilter("4.0.0.0", null, "AllowReclassify", "IWDEDocument")]
        public bool AllowReclassify
        {
            get
            {
                return m_AllowReclassify;
            }
            set
            {
                m_AllowReclassify = value;
            }
        }

        [VersionPropertyFilter("4.0.0.0", null, "RejectCodes", "IWDEDocument")]
        public IWDERejectCodes RejectCodes
        {
            get
            {
                return m_RejectCodes;
            }
        }

        public void Delete()
		{
			if (ParentDocuments.Index == Index)
				ParentDocuments.Delete();
			else
			{
				IWDECollection icol = (IWDECollection)m_Documents;
				icol.RemoveAt(Index);
			}
		}

		public void ClearData()
		{
			m_APIVersion = "";
			m_Status = WDESessionStatus.None;
			m_RejectCode = "";
			m_RejectDescription = "";
			m_QISelected = false;
			m_QIFocusAudit = false;
			m_QIAutoAudit = false;
            m_AllowReclassify = false;
            m_AllowRescan = false;
			m_Records.Clear();
			IWDEDocSessionsInternal idocsess = (IWDEDocSessionsInternal)m_Sessions;
			idocsess.Clear();
		}

		public IWDEFilteredRecords FilteredRecords(string RecType)
		{
			if (RecType == null)
				throw new ArgumentNullException("RecType", "RecType cannot be null");				

			IWDERecords rec = WDERecords.Create(this);
			IWDEFilteredRecordsInternal irec = (IWDEFilteredRecordsInternal)rec;
			irec.FilterRecords(RecType);
			return (IWDEFilteredRecords)rec;
		}

		public void LoadFromFile(string FileName)
		{
			if (FileName == null)
				throw new ArgumentNullException("FileName", "FileName cannot be null");				

			FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			try
			{
				LoadFromStream(fs);
			}
			finally
			{
				fs.Close();
			}
		}

		public void LoadFromStream(System.Runtime.InteropServices.ComTypes.IStream aStream)
		{
			if (aStream == null)
				throw new ArgumentNullException("aStream", "aStream cannot be null");				

			MemoryStream ms = new MemoryStream();
			try
			{
				Utils.UCOMIStreamToStream(aStream, ms);
				ms.Seek(0, SeekOrigin.Begin);
				LoadFromStream(ms);
			}
			finally
			{
				ms.Close();
			}
		}

		public void LoadFromStream(System.IO.Stream aStream)
		{
			if (aStream == null)
				throw new ArgumentException("aStream", "aStream cannot be null");				

			StreamReader sr = new StreamReader(aStream, true);
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();

			bool ChildOfDataSet = false;
			if ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DataSet"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();
				ChildOfDataSet = true;
			}

			ReadFromXml(XmlReader);

			// Read any batch level session elements that might be present.
			MergeBatchLevelSessions(XmlReader);

			if (ChildOfDataSet)
				XmlReader.ReadEndElement();

			SyncDataSetSessions();

			VersionInfo.TargetVersionNumber = APIVersion;
		}

		public void SaveToFile(string FileName)
		{
			SaveToFile(FileName, false);
		}

		public void SaveToFile(string FileName, bool ChildOfDataSet)
		{
			if (FileName == null)
				throw new ArgumentNullException("FileName", "FileName cannot be null");				

			FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None);
			try
			{
				SaveToStream(fs, ChildOfDataSet);
			}
			finally
			{
				fs.Close();
			}
		}

		public void SaveToStream(System.Runtime.InteropServices.ComTypes.IStream aStream)
		{
			SaveToStream(aStream, false);
		}

		public void SaveToStream(System.Runtime.InteropServices.ComTypes.IStream aStream, bool ChildOfDataSet)
		{
			if (aStream == null)
				throw new ArgumentNullException("aStream", "aStream cannot be null");
				
			MemoryStream ms = new MemoryStream();
			try
			{
				SaveToStream(ms, ChildOfDataSet);
				Utils.StreamToUCOMIStream(ms, aStream);
			}
			finally
			{
				ms.Close();
			}
		}

		public void SaveToStream(Stream aStream)
		{
			SaveToStream(aStream, false);
		}

		public void SaveToStream(Stream aStream, bool ChildOfDataSet)
		{
			if (aStream == null)
				throw new ArgumentNullException("aStream", "aStream cannot be null");
				
			// save to memory so we don't attach higher level objects to the calling stream.
			MemoryStream memStream = new MemoryStream();
			XmlTextWriter XmlWriter = new XmlTextWriter(memStream, UnicodeEncoding.UTF8);
			try
			{
				VersionHelper.Initialize();
				XmlWriter.Formatting = Formatting.Indented;
				XmlWriter.WriteStartDocument(true);

				if (ChildOfDataSet)
					XmlWriter.WriteStartElement("DataSet");

				WriteToXml(XmlWriter);

				if (ChildOfDataSet)
					XmlWriter.WriteEndElement();
			}
			finally
			{
				XmlWriter.Close();
			}
			MemoryStream saveStream = new MemoryStream(memStream.ToArray());

			Utils.CopyStream(saveStream, aStream);
		}

		#endregion

		#region IWDEDocumentInternal Members

		public IWDERecordDefs RecordDefs
		{
			get
			{
				if (DocumentDef == null)
				{
					if (DataSet.Project == null)
						throw new WDEException("API00001");
					else
						throw new WDEException("API00015", new object[] { m_DocType });
				}

				return DocumentDef.RecordDefs;
			}
		}

		public int SaveSessionID
		{
			get
			{
				return m_SaveSessionID;
			}
			set
			{
				m_SaveSessionID = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Document"))
			{
				m_DocType = Utils.GetAttribute(XmlReader, "DocType", "");
				m_DCN = Utils.GetAttribute(XmlReader, "DCN", "");
				m_AltDCN = Utils.GetAttribute(XmlReader, "AltDCN", "");
				m_RejectCode = Utils.GetAttribute(XmlReader, "RejectCode", "");
				m_RejectDescription = Utils.GetAttribute(XmlReader, "RejectDescription", "");

                m_RejectField = Utils.GetAttribute(XmlReader, "RejectField", "");
                m_RejectRow = int.Parse(Utils.GetAttribute(XmlReader, "RejectRow", "-1"));

				if (Utils.GetAttribute(XmlReader, "Status", "") != "")
					m_Status = Utils.GetStatus(XmlReader, "Status");
				else
					m_Status = WDESessionStatus.None;
				bool rejected = Utils.GetBoolValue(XmlReader, "Rejected", false);

				if (rejected)
					m_Status = WDESessionStatus.Rejected;

				m_QISelected = Utils.GetBoolValue(XmlReader, "QISelected", Utils.GetBoolValue(XmlReader, "QCSelected", false));
				m_QIAutoAudit = Utils.GetBoolValue(XmlReader, "QIAutoAudit", false);
				m_QIFocusAudit = Utils.GetBoolValue(XmlReader, "QIFocusAudit", false);
				m_StoredDocType = Utils.GetAttribute(XmlReader, "StoredDocType", "");
                m_DocNotes = Utils.GetAttribute(XmlReader, "DocNotes", "");
                m_AllowRescan = Utils.GetBoolValue(XmlReader, "AllowRescan", false);
                m_AllowReclassify = Utils.GetBoolValue(XmlReader, "AllowReclassify", false);
                m_APIVersion = Utils.GetAttribute(XmlReader, "APIVersion", "");

				XmlReader.Read();
				XmlReader.MoveToContent();

				IWDEXmlPersist ipers = (IWDEXmlPersist)Images;
				ipers.ReadFromXml(XmlReader);

                ipers = (IWDEXmlPersist)RejectCodes;
                ipers.ReadFromXml(XmlReader);

                ReadAccumulators(XmlReader);

				ipers = (IWDEXmlPersist)Records;
				ipers.ReadFromXml(XmlReader);

				ipers = (IWDEXmlPersist)Sessions;
				ipers.ReadFromXml(XmlReader);

				if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Document"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if (XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				

			XmlWriter.WriteStartElement("Document");
			if (DocType != "")
				XmlWriter.WriteAttributeString("DocType", DocType);
			if (DCN != "")
				XmlWriter.WriteAttributeString("DCN", DCN);

			if (AltDCN != "")
				XmlWriter.WriteAttributeString("AltDCN", AltDCN);
			if (StoredDocType != "")
				XmlWriter.WriteAttributeString("StoredDocType", StoredDocType);
			if (Status != WDESessionStatus.None)
			{
				if (!VersionHelper.FilterPropertyOrCollection("IWDEDocument.Status", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("Status", VersionHelper.GetEnumerationString(typeof(WDESessionStatus).Name, m_Status, VersionInfo.TargetVersionNumber));
				}
			}

            if (Rejected)
            {
                XmlWriter.WriteAttributeString("Rejected", bool.TrueString);
                XmlWriter.WriteAttributeString("RejectCode", m_RejectCode);
                if (m_RejectDescription != "")
                    XmlWriter.WriteAttributeString("RejectDescription", m_RejectDescription);                

                if (RejectField != "")
                {
                    if (!VersionHelper.FilterPropertyOrCollection("IWDEDocument.RejectField", VersionInfo.TargetVersionNumber))
                        XmlWriter.WriteAttributeString("RejectField", RejectField);
                    else
                        VersionHelper.LogDataLost("IWDEDocument.RejectField", RejectField);

                }
                if (RejectRow >= 0)
                {
                    if (!VersionHelper.FilterPropertyOrCollection("IWDEDocument.RejectRow", VersionInfo.TargetVersionNumber))
                        XmlWriter.WriteAttributeString("RejectRow", RejectRow.ToString());
                    else
                        VersionHelper.LogDataLost("IWDEDocument.RejectRow", RejectRow.ToString());
                }                
            }

			if (QISelected)
				XmlWriter.WriteAttributeString("QISelected", bool.TrueString);
			if (QIAutoAudit)
			{
				if (!VersionHelper.FilterPropertyOrCollection("IWDEDocument.QIAutoAudit", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("QIAutoAudit", bool.TrueString);
				}
				else
				{
					VersionHelper.LogDataLost("IWDEDocument.QIAutoAudit", bool.TrueString);
				}
			}
			if (QIFocusAudit)
			{
				if (!VersionHelper.FilterPropertyOrCollection("IWDEDocument.QIFocusAudit", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("QIFocusAudit", bool.TrueString);
				}
				else
				{
					VersionHelper.LogDataLost("IWDEDocument.QIFocusAudit", bool.TrueString);
				}
			}

            if (DocNotes != "")
            {
                if (!VersionHelper.FilterPropertyOrCollection("IWDEDocument.DocNotes", VersionInfo.TargetVersionNumber))
                    XmlWriter.WriteAttributeString("DocNotes", DocNotes);
                else
                    VersionHelper.LogDataLost("IWDEDocument.DocNotes", DocNotes);
            }

            XmlWriter.WriteAttributeString("AllowRescan", m_AllowRescan.ToString());

            XmlWriter.WriteAttributeString("AllowReclassify", m_AllowReclassify.ToString());

            XmlWriter.WriteAttributeString("APIVersion", VersionInfo.TargetVersionNumber);

			IWDEXmlPersist ipers = (IWDEXmlPersist)Images;
			ipers.WriteToXml(XmlWriter);

            ipers = (IWDEXmlPersist)RejectCodes;
            ipers.WriteToXml(XmlWriter);

            ipers = (IWDEXmlPersist)Records;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist)Sessions;
			ipers.WriteToXml(XmlWriter);

			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Members

		private void MergeBatchLevelSessions(XmlTextReader XmlReader)
		{
			XmlReader.MoveToContent();
			while ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Session"))
			{
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		private void SyncDataSetSessions()
		{
			if ((m_Documents.DataSet != null) && (m_Documents.DataSet.Sessions is IWDESessionsInternal))
			{
				foreach (IWDEDocSession_R1 docsess in m_Sessions)
				{
					int foundIndex = -1;
					int insertIndex = -1;
					for (int i = 0; i < m_Documents.DataSet.Sessions.Count; i++)
					{
						if (!((IWDESessionInternal)m_Documents.DataSet.Sessions[i]).IsCreateSession)
						{
							if (m_Documents.DataSet.Sessions[i].SessionID == docsess.SessionID)
							{
								foundIndex = i;
								insertIndex = -2;
								break;
							}
							else if (m_Documents.DataSet.Sessions[i].SessionID < docsess.SessionID)
							{
								insertIndex = i;
								break;
							}
							else if (i == 0)
								insertIndex = -1;
						}
					}

					if (insertIndex > -2)
					{
						IWDESessionsInternal isessions = (IWDESessionsInternal)m_Documents.DataSet.Sessions;
						IWDESessionInternal isess = null;
						if (insertIndex == -1)
						{
							// append
							isess = (IWDESessionInternal)isessions.Append(docsess.User, docsess.Task, docsess.Mode, docsess.Location);
						}
						else
						{
							// insert at index
							isess = (IWDESessionInternal)isessions.Insert(insertIndex, docsess.User, docsess.Task, docsess.Mode, docsess.Location);
						}

						isess.SessionID = docsess.SessionID;
						isess.StartTime = ((IWDEDocSessionInternal)docsess).StartTime;
						isess.EndTime = ((IWDEDocSessionInternal)docsess).EndTime;
					}
					else
					{
						IWDESessionInternal isess = (IWDESessionInternal)m_Documents.DataSet.Sessions[foundIndex];
						isess.SetEndTimeExplicit(((IWDEDocSessionInternal)docsess).EndTime);
					}
				}
			}
		}

		private void CheckSession()
		{
			if (Sessions.Count == 0)
					throw new WDEException("API90003");
		}

		private void ReadAccumulators(XmlTextReader XmlReader)
		{
			while ((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "Accumulator"))
			{
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		#endregion

		#region Internal Members

		internal static XmlSchemaElement GetSchema(List<Type> usedEnums)
		{
			XmlSchemaElement result = new XmlSchemaElement();
			result.Name = "Document";
			result.MinOccurs = 0;
			result.MaxOccursString = "unbounded";
			XmlSchemaComplexType complexType = new XmlSchemaComplexType();
			result.SchemaType = complexType;

			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexType.Particle = seq;

			XmlSchemaAttribute attr = new XmlSchemaAttribute();

			attr.Name = "DocType";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Use = XmlSchemaUse.Required;
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "DCN";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Use = XmlSchemaUse.Required;
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "AltDCN";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "StoredDocType";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "Status";
			attr.SchemaTypeName = new XmlQualifiedName(typeof(WDESessionStatus).Name, SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

			if (!usedEnums.Contains(typeof(WDESessionStatus)))
				usedEnums.Add(typeof(WDESessionStatus));

			attr = new XmlSchemaAttribute();
			attr.Name = "Rejected";
			attr.SchemaTypeName = new XmlQualifiedName("WDEBoolean", SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "RejectCode";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "RejectDescription";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "QISelected";
			attr.SchemaTypeName = new XmlQualifiedName("WDEBoolean", SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "QIAutoAudit";
			attr.SchemaTypeName = new XmlQualifiedName("WDEBoolean", SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "QIFocusAudit";
			attr.SchemaTypeName = new XmlQualifiedName("WDEBoolean", SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "APIVersion";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

            attr = new XmlSchemaAttribute();
            attr.Name = "RejectField";
            attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
            complexType.Attributes.Add(attr);

            attr = new XmlSchemaAttribute();
            attr.Name = "RejectRow";
            attr.SchemaTypeName = new XmlQualifiedName("int", SchemaHelpers.XSD_NAMESPACE);
            complexType.Attributes.Add(attr);

            attr = new XmlSchemaAttribute();
            attr.Name = "DocNotes";
            attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
            complexType.Attributes.Add(attr);

            attr = new XmlSchemaAttribute();
            attr.Name = "AllowRescan";
            attr.SchemaTypeName = new XmlQualifiedName("WDEBoolean", SchemaHelpers.TARGET_NAMESPACE);
            complexType.Attributes.Add(attr);

            attr = new XmlSchemaAttribute();
            attr.Name = "AllowReclassify";
            attr.SchemaTypeName = new XmlQualifiedName("WDEBoolean", SchemaHelpers.TARGET_NAMESPACE);
            complexType.Attributes.Add(attr);

            seq.Items.Add(WDEImage.GetSchema(usedEnums));
            seq.Items.Add(WDERejectCode.GetSchema(usedEnums));
			XmlSchemaElement record = new XmlSchemaElement();
			record.RefName = new XmlQualifiedName("Record", SchemaHelpers.TARGET_NAMESPACE);
			record.MinOccurs = 0;
			record.MaxOccursString = "unbounded";
            seq.Items.Add(record);
			seq.Items.Add(WDEDocSession.GetSchema(usedEnums));

			return result;
		}

		#endregion

		#region IWDERecordsOwner Members

		public IWDEDocument Document
		{
			get
			{
				return (IWDEDocument)this;
			}
		}

		public IWDERecord Record
		{
			get
			{
				return null;
			}
		}

		#endregion
	}

	public class WDEImages : IWDEImages, IWDEXmlPersist, IWDECollection, IEnumerable
	{
		private ArrayList m_Images;
		private IWDEDocument m_Document;

		private WDEImages(IWDEDocument Owner)
		{
			m_Images = new ArrayList();
			m_Document = Owner;
		}

		public static IWDEImages Create(IWDEDocument Owner)
		{
			return new WDEImages(Owner) as IWDEImages;
		}

		public static IWDEImages CreateInstance(IWDEDocument Owner)
		{
			return Create(Owner);
		}

		#region IWDEImages Members

		public int Count
		{
			get
			{
				return m_Images.Count;
			}
		}

		public IWDEImage this[int Index]
		{
			get
			{
				return (IWDEImage)m_Images[Index];
			}
		}

		public IWDEDocument Document
		{
			get
			{
				return m_Document;
			}
		}

		public void Clear()
		{
			m_Images.Clear();
		}

		public IWDEImage Append(string ImageType, string ImageName)
		{
			return Append(ImageType, ImageName, "", "", false);
		}

		public IWDEImage Append(string ImageType, string ImageName, string ImagePath, string ZipName, bool IsSnippet)
		{
			if (ImageType == null)
				throw new ArgumentNullException("ImageType", "ImageType cannot be null");
				
			if (ImageName == null)
				throw new ArgumentNullException("ImageName", "ImageName cannot be null");
				
			if (ImagePath == null)
				throw new ArgumentNullException("ImagePath", "ImagePath cannot be null");
				
			if (ZipName == null)
				throw new ArgumentNullException("ZipName", "ZipName cannot be null");				

			int defindex = m_Document.DocumentDef.ImageSourceDefs.Find(ImageType);
			if (defindex != -1)
			{
				IWDEImage image = WDEImage.Create(this);
				image.ImageName = ImageName;
				image.ImageType = ImageType;
				image.ImagePath = ImagePath;
				image.IsSnippet = IsSnippet;
				image.ZipName = ZipName;
				image.PerformOCR = m_Document.DocumentDef.ImageSourceDefs[defindex].PerformOCR;
				image.StoredAttachType = m_Document.DocumentDef.ImageSourceDefs[defindex].StoredAttachType;
				m_Images.Add(image);
				return image;
			}
			else
				throw new WDEException("API00014", new object[] { ImageType, m_Document.DocType });
		}

		public IWDEImage Insert(int Index, string ImageType, string ImageName)
		{
			return Insert(Index, ImageType, ImageName, "", "", false);
		}

		public IWDEImage Insert(int Index, string ImageType, string ImageName, string ImagePath, string ZipName, bool IsSnippet)
		{
			if (((Count > 0) && ((Index < 0) || (Index > Count - 1))) ||
				((Count == 0) && (Index != 0)))
					throw new WDEException("API00012", new object[] { "IWDEImages", Index, Count });

			IWDEImage result = Append(ImageType, ImageName, ImagePath, ZipName, IsSnippet);
			result.Index = Index;
			return result;
		}

		public int Find(string ImageName)
		{
			if (ImageName == null)
				throw new ArgumentNullException("ImageName", "ImageName cannot be null");				

			for (int i = 0; i < Count; i++)
			{
				if (string.Compare(this[i].ImageName, ImageName, true) == 0)
				{
					return i;
				}
			}

			return -1;
		}

        public void Delete(int Index)
        {
            if (((Count > 0) && ((Index < 0) || (Index > Count - 1))) ||
                ((Count == 0) && (Index != 0)))
                throw new WDEException("API00012", new object[] { "IWDEImages", Index, Count });

            m_Images.RemoveAt(Index);
        }

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "Image"))
			{
				m_Images.Clear();
				while ((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "Image"))
				{
					IWDEImage image = WDEImage.Create(this);
					IWDEXmlPersist ipers = (IWDEXmlPersist)image;
					ipers.ReadFromXml(XmlReader);
					m_Images.Add(image);
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if (XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				

			for (int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist)this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion

		#region IWDECollection Members

		public int GetIndex(object Item)
		{
			return m_Images.IndexOf(Item);
		}

		public void SetIndex(object Item, int NewIndex)
		{
			int index = GetIndex(Item);
			if (index != -1)
			{
				m_Images.RemoveAt(index);
				m_Images.Insert(NewIndex, Item);
			}
			else
					throw new WDEException("API90002", new object[] { "IWDEImages" });
		}

		public void RemoveAt(int Index)
		{
			m_Images.RemoveAt(Index);
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			IEnumerable ienum = (IEnumerable)m_Images;
			return ienum.GetEnumerator();
		}

		#endregion
	}

	public class WDEImage : IWDEImage_R2, IWDEXmlPersist
	{
		private IWDEImages m_Images;
		private string m_ImageType;
		private string m_ImageName;
		private string m_ImagePath;
		private bool m_IsSnippet;
		private bool m_PerformOCR;
		private string m_RegisteredImage;
		private int m_Rotate;
		private string m_StoredAttachType;
		private int m_Tag;
		private string m_ZipName;
		private Point m_ZoneOffset;
		private Point m_OverlayOffset;
        private int m_PageCount;
        private bool m_Modified;

		private WDEImage(IWDEImages Owner)
		{
			m_Images = Owner;
			m_ZoneOffset = new Point(0, 0);
			m_OverlayOffset = new Point(0, 0);
			m_ImageType = "";
			m_ImageName = "";
			m_ImagePath = "";
			m_RegisteredImage = "";
			m_StoredAttachType = "";
			m_ZipName = "";
            m_PageCount = 1;
		}

		public static IWDEImage_R2 Create(IWDEImages Owner)
		{
			return new WDEImage(Owner) as IWDEImage_R2;
		}

		public static IWDEImage_R2 CreateInstance(IWDEImages Owner)
		{
			return Create(Owner);
		}

		#region IWDEImage Members

		public IWDEDocument Document
		{
			get
			{
				return m_Images.Document;
			}
		}

		public string ImageType
		{
			get
			{
				return m_ImageType;
			}
			set
			{
				if (value == null)
					m_ImageType = "";
				else
					m_ImageType = value;
			}
		}

		public string ImageName
		{
			get
			{
				return m_ImageName;
			}
			set
			{
				if (value == null)
					m_ImageName = "";
				else
					m_ImageName = value;
			}
		}

		public string ImagePath
		{
			get
			{
				return m_ImagePath;
			}
			set
			{
				if (value == null)
					m_ImagePath = "";
				else
					m_ImagePath = value;
			}
		}

		public bool IsSnippet
		{
			get
			{
				return m_IsSnippet;
			}
			set
			{
				m_IsSnippet = value;
			}
		}

		public bool PerformOCR
		{
			get
			{
				return m_PerformOCR;
			}
			set
			{
				m_PerformOCR = value;
			}
		}

		public string RegisteredImage
		{
			get
			{
				return m_RegisteredImage;
			}
			set
			{
				if (value == null)
					m_RegisteredImage = "";
				else
					m_RegisteredImage = value;
			}
		}

		public int Index
		{
			get
			{
				IWDECollection icoll = (IWDECollection)m_Images;
				return icoll.GetIndex(this);
			}
			set
			{
				IWDECollection icoll = (IWDECollection)m_Images;
				icoll.SetIndex(this, value);
			}
		}

		public int Rotate
		{
			get
			{
				return m_Rotate;
			}
			set
			{
				m_Rotate = value % 360;

				if (m_Rotate < 0)
					m_Rotate += 360;
			}
		}

		public string StoredAttachType
		{
			get
			{
				return m_StoredAttachType;
			}
			set
			{
				if (value == null)
					m_StoredAttachType = "";
				else
					m_StoredAttachType = value;
			}
		}

		public int Tag
		{
			get
			{
				return m_Tag;
			}
			set
			{
				m_Tag = value;
			}
		}

		public string ZipName
		{
			get
			{
				return m_ZipName;
			}
			set
			{
				if (value == null)
					m_ZipName = "";
				else
					m_ZipName = value;
			}
		}

		public int ZoneOffsetX
		{
			get
			{
				return m_ZoneOffset.X;
			}
			set
			{
				m_ZoneOffset.X = value;
			}
		}

		public int ZoneOffsetY
		{
			get
			{
				return m_ZoneOffset.Y;
			}
			set
			{
				m_ZoneOffset.Y = value;
			}
		}

		[VersionPropertyFilter("1.2.0.0", null, "OverlayOffset", "IWDEImage")]
		public int OverlayOffsetX
		{
			get
			{
				return m_OverlayOffset.X;
			}
			set
			{
				m_OverlayOffset.X = value;
			}
		}

		
		public int OverlayOffsetY
		{
			get
			{
				return m_OverlayOffset.Y;
			}
			set
			{
				m_OverlayOffset.Y = value;
			}
		}

        [VersionPropertyFilter("2.6.0.0", null, "PageCount", "IWDEImage")]
        public int PageCount
        {
            get
            {
                return m_PageCount;
            }
            set
            {
                if (value > 0)
                    m_PageCount = value;
                else
                    throw new ArgumentOutOfRangeException("PageCount");
            }
        }

        [VersionPropertyFilter("4.0.0.0", null, "Modified", "IWDEImage")]
        public bool Modified
        {
            get
            {
                return m_Modified;
            }
            set
            {
                m_Modified = value;
            }
        }

        #endregion

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
		{
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			XmlReader.MoveToContent();
			if (XmlReader.NodeType != XmlNodeType.Element)
				throw new XmlException(string.Format("Unexpected node type reading Xml: ", new object[] {XmlReader.NodeType.ToString()}));				

			if (XmlReader.Name == "Image")
			{
				m_ImageType = Utils.GetAttribute(XmlReader, "ImageType", "");
				m_ImageName = Utils.GetAttribute(XmlReader, "ImageName", "");
				m_ImagePath = Utils.GetAttribute(XmlReader, "ImagePath", "");
				m_IsSnippet = Utils.GetBoolValue(XmlReader, "IsSnippet", false);
				m_PerformOCR = Utils.GetBoolValue(XmlReader, "PerformOCR", false);
				m_RegisteredImage = Utils.GetAttribute(XmlReader, "RegisteredImage", "");
				string temp = Utils.GetAttribute(XmlReader, "Rotate", "");
                if (!int.TryParse(temp, out m_Rotate))
                    m_Rotate = 0;
				m_StoredAttachType = Utils.GetAttribute(XmlReader, "StoredAttachType", "");
				m_ZipName = Utils.GetAttribute(XmlReader, "ZipName", "");
				m_ZoneOffset = Utils.GetPointValue(XmlReader, "ZoneOffset");
				m_OverlayOffset = Utils.GetPointValue(XmlReader, "OverlayOffset");
                m_PageCount = Utils.GetIntValue(XmlReader, "PageCount", 1);
                m_Modified = Utils.GetBoolValue(XmlReader, "Modified", false);

                XmlReader.Read();
				XmlReader.MoveToContent();

				if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Image"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if (XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				

			XmlWriter.WriteStartElement("Image");
            if (ImageType != "")
				XmlWriter.WriteAttributeString("ImageType", m_ImageType);
			XmlWriter.WriteAttributeString("ImageName", m_ImageName);
			if (ImagePath != "")
				XmlWriter.WriteAttributeString("ImagePath", m_ImagePath);
            //if (IsSnippet)
            XmlWriter.WriteAttributeString("IsSnippet", m_IsSnippet.ToString());
            if (PerformOCR)
				XmlWriter.WriteAttributeString("PerformOCR", m_PerformOCR.ToString());
			if (RegisteredImage != "")
				XmlWriter.WriteAttributeString("RegisteredImage", m_RegisteredImage);

			if (m_Rotate > 0)
				XmlWriter.WriteAttributeString("Rotate", m_Rotate.ToString());

			if (StoredAttachType != "")
				XmlWriter.WriteAttributeString("StoredAttachType", m_StoredAttachType);
			if (ZipName != "")
				XmlWriter.WriteAttributeString("ZipName", m_ZipName);
			if (!m_ZoneOffset.IsEmpty)
				XmlWriter.WriteAttributeString("ZoneOffset", string.Format("{0},{1}", new object[] { m_ZoneOffset.X, m_ZoneOffset.Y }));
			if (!m_OverlayOffset.IsEmpty)
			{
				if (!VersionHelper.FilterPropertyOrCollection("IWDEImage.OverlayOffset", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("OverlayOffset", string.Format("{0},{1}", new object[] { m_OverlayOffset.X, m_OverlayOffset.Y }));
				}
				else
				{
					VersionHelper.LogDataLost("IWDEImage.OverlayOffset", string.Format("{0},{1}", new object[] { m_OverlayOffset.X, m_OverlayOffset.Y }));
				}
			}
            if (m_PageCount > 1)
            {
                if (!VersionHelper.FilterPropertyOrCollection("IWDEImage.PageCount", VersionInfo.TargetVersionNumber))
                {
                    XmlWriter.WriteAttributeString("PageCount", m_PageCount.ToString());
                }
                else
                {
                    VersionHelper.LogDataLost("IWDEImage.PageCount", m_PageCount.ToString());
                }
            }

            XmlWriter.WriteAttributeString("Modified", m_Modified.ToString());

            XmlWriter.WriteEndElement();
		}

		#endregion

		#region Internal Methods

		internal static XmlSchemaElement GetSchema(List<Type> usedEnums)
		{
			XmlSchemaElement result = new XmlSchemaElement();
			result.Name = "Image";
			XmlSchemaComplexType complexType = new XmlSchemaComplexType();
            result.MinOccurs = 0;
            result.MaxOccursString = "unbounded";
			result.SchemaType = complexType;

			XmlSchemaAttribute attr = new XmlSchemaAttribute();
			attr.Name = "ImageType";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Use = XmlSchemaUse.Required;
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "ImageName";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Use = XmlSchemaUse.Required;
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "ImagePath";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "IsSnippet";
			attr.SchemaTypeName = new XmlQualifiedName("WDEBoolean", SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

            attr = new XmlSchemaAttribute();
            attr.Name = "Modified";
            attr.SchemaTypeName = new XmlQualifiedName("WDEBoolean", SchemaHelpers.TARGET_NAMESPACE);
            complexType.Attributes.Add(attr);

            attr = new XmlSchemaAttribute();
			attr.Name = "PerformOCR";
			attr.SchemaTypeName = new XmlQualifiedName("WDEBoolean", SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "RegisteredImage";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "Rotate";
			attr.SchemaTypeName = new XmlQualifiedName("int", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "StoredAttachType";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "ZoneOffset";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Annotation = new XmlSchemaAnnotation();
			XmlSchemaDocumentation documentation = new XmlSchemaDocumentation();
			attr.Annotation.Items.Add(documentation);
			documentation.Markup = SchemaHelpers.TextToNodeArray("A comma-separated list of integers: left, top, width, height");
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "OverlayOffset";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Annotation = new XmlSchemaAnnotation();
			documentation = new XmlSchemaDocumentation();
			attr.Annotation.Items.Add(documentation);
			documentation.Markup = SchemaHelpers.TextToNodeArray("A comma-separated list of integers: left, top, width, height");
			complexType.Attributes.Add(attr);

            attr = new XmlSchemaAttribute();
            attr.Name = "PageCount";
            attr.SchemaTypeName = new XmlQualifiedName("int", SchemaHelpers.XSD_NAMESPACE);
            complexType.Attributes.Add(attr);

            attr = new XmlSchemaAttribute();
            attr.Name = "ZipName";
            attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
            complexType.Attributes.Add(attr);

			return result;
		}

		#endregion
	}

    public class WDERejectCodes : IWDERejectCodes, IWDEXmlPersist, IWDECollection, IEnumerable
    {
        private ArrayList m_RejectCodes;
        private IWDEDocument m_Document;

        private WDERejectCodes(IWDEDocument Owner)
        {
            m_RejectCodes = new ArrayList();
            m_Document = Owner;
        }

        public static IWDERejectCodes Create(IWDEDocument Owner)
        {
            return new WDERejectCodes(Owner) as IWDERejectCodes;
        }

        public static IWDERejectCodes CreateInstance(IWDEDocument Owner)
        {
            return Create(Owner);
        }

        #region IWDERejectCodes Members

        public int Count
        {
            get
            {
                return m_RejectCodes.Count;
            }
        }

        public IWDERejectCode this[int Index]
        {
            get
            {
                return (IWDERejectCode)m_RejectCodes[Index];
            }
        }

        public IWDEDocument Document
        {
            get
            {
                return m_Document;
            }
        }

        public void Clear()
        {
            m_RejectCodes.Clear();
        }

        public IWDERejectCode Add(string Code, string Description, bool RequireReason)
        {
            if (Code == null)
                throw new ArgumentNullException("Code", "Code cannot be null");

            int codeIndex = Find(Code);
            
            if (codeIndex < 0)
            {
                IWDERejectCode rejCode = WDERejectCode.Create(this);
                rejCode.Code = Code;
                rejCode.Description = Description;
                rejCode.RequireReason = RequireReason;
                m_RejectCodes.Add(rejCode);
                return rejCode;
            }
            else
                throw new WDEException("API00038", new object[] { Code });
        }      

        public int Find(string Code)
        {
            if (Code == null)
                throw new ArgumentNullException("Code", "Code cannot be null");

            for (int i = 0; i < Count; i++)
            {
                if (string.Compare(this[i].Code, Code, true) == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        public void Delete(int Index)
        {
            if (((Count > 0) && ((Index < 0) || (Index > Count - 1))) ||
                ((Count == 0) && (Index != 0)))
                throw new WDEException("API00012", new object[] { "IWDERejectCodes", Index, Count });

            m_RejectCodes.RemoveAt(Index);
        }

        #endregion

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
        {
            if (XmlReader == null)
                throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");

            XmlReader.MoveToContent();
            if ((XmlReader.NodeType == XmlNodeType.Element) &&
                (XmlReader.Name == "AllowedRejectCode"))
            {
                m_RejectCodes.Clear();
                while ((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
                    (XmlReader.Name == "AllowedRejectCode"))
                {
                    IWDERejectCode rejCode = WDERejectCode.Create(this);
                    IWDEXmlPersist ipers = (IWDEXmlPersist)rejCode;
                    ipers.ReadFromXml(XmlReader);
                    m_RejectCodes.Add(rejCode);
                }
            }
        }

        public void WriteToXml(XmlTextWriter XmlWriter)
        {
            if (XmlWriter == null)
                throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");

            for (int i = 0; i < Count; i++)
            {
                IWDEXmlPersist ipers = (IWDEXmlPersist)this[i];
                ipers.WriteToXml(XmlWriter);
            }
        }

        #endregion

        #region IWDECollection Members

        public int GetIndex(object Item)
        {
            return m_RejectCodes.IndexOf(Item);
        }

        public void SetIndex(object Item, int NewIndex)
        {
            int index = GetIndex(Item);
            if (index != -1)
            {
                m_RejectCodes.RemoveAt(index);
                m_RejectCodes.Insert(NewIndex, Item);
            }
            else
                throw new WDEException("API90002", new object[] { "IWDERejectCodes" });
        }

        public void RemoveAt(int Index)
        {
            m_RejectCodes.RemoveAt(Index);
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            IEnumerable ienum = (IEnumerable)m_RejectCodes;
            return ienum.GetEnumerator();
        }

        #endregion
    }

    public class WDERejectCode : IWDERejectCode, IWDEXmlPersist
    {
        private IWDERejectCodes m_RejectCodes;
        private string m_Code;
        private string m_Description;
        private bool m_RequireReason;

        private WDERejectCode(IWDERejectCodes Owner)
        {
            m_RejectCodes = Owner;
            m_Code = "";
            m_Description = "";
        }

        public static IWDERejectCode Create(IWDERejectCodes Owner)
        {
            return new WDERejectCode(Owner) as IWDERejectCode;
        }

        public static IWDERejectCode CreateInstance(IWDERejectCodes Owner)
        {
            return Create(Owner);
        }

        #region IWDERejectCode Members

        public IWDEDocument Document
        {
            get
            {
                return m_RejectCodes.Document;
            }
        }

        public string Code
        {
            get
            {
                return m_Code;
            }
            set
            {
                if(string.IsNullOrEmpty(value))                
                        throw new ArgumentNullException("Code", "Code cannot be null or blank");
                else
                {
                    int index = m_RejectCodes.Find(value);
                    if(index < 0)
                        m_Code = value;
                    else
                        throw new WDEException("API00038", new object[] { value });

                }
            }
        }

        public string Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                if (value == null)
                    m_Description = "";
                else
                    m_Description = value;
            }
        }

        public bool RequireReason
        {
            get
            {
                return m_RequireReason;
            }
            set
            {
                m_RequireReason = value;
            }
        }

        public int Index
        {
            get
            {
                IWDECollection icoll = (IWDECollection)m_RejectCodes;
                return icoll.GetIndex(this);
            }
            set
            {
                IWDECollection icoll = (IWDECollection)m_RejectCodes;
                icoll.SetIndex(this, value);
            }
        }        

        #endregion

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
        {
            if (XmlReader == null)
                throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");

            XmlReader.MoveToContent();
            if (XmlReader.NodeType != XmlNodeType.Element)
                throw new XmlException(string.Format("Unexpected node type reading Xml: ", new object[] { XmlReader.NodeType.ToString() }));

            if (XmlReader.Name == "AllowedRejectCode")
            {
                m_Code = Utils.GetAttribute(XmlReader, "Code", "");
                m_Description = Utils.GetAttribute(XmlReader, "Description", "");                
                m_RequireReason = Utils.GetBoolValue(XmlReader, "RequireReason", false);

                XmlReader.Read();
                XmlReader.MoveToContent();

                if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "AllowedRejectCode"))
                {
                    XmlReader.ReadEndElement();
                    XmlReader.MoveToContent();
                }
            }
        }

        public void WriteToXml(XmlTextWriter XmlWriter)
        {
            if (XmlWriter == null)
                throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");

            XmlWriter.WriteStartElement("AllowedRejectCode");
            //if (Code != "")
            XmlWriter.WriteAttributeString("Code", Code);
           
            XmlWriter.WriteAttributeString("Description", Description);

            //if (RequireReason)
            XmlWriter.WriteAttributeString("RequireReason", RequireReason.ToString());

            XmlWriter.WriteEndElement();
        }

        #endregion

        #region Internal Methods

        internal static XmlSchemaElement GetSchema(List<Type> usedEnums)
        {
            XmlSchemaElement result = new XmlSchemaElement();
            result.Name = "AllowedRejectCode";
            XmlSchemaComplexType complexType = new XmlSchemaComplexType();
            result.MinOccurs = 0;
            result.MaxOccursString = "unbounded";
            result.SchemaType = complexType;

            XmlSchemaAttribute attr = new XmlSchemaAttribute();
            attr.Name = "Code";
            attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
            attr.Use = XmlSchemaUse.Required;
            complexType.Attributes.Add(attr);

            attr = new XmlSchemaAttribute();
            attr.Name = "Description";
            attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
            attr.Use = XmlSchemaUse.Required;
            complexType.Attributes.Add(attr);

            attr = new XmlSchemaAttribute();
            attr.Name = "RequireReason";
            attr.SchemaTypeName = new XmlQualifiedName("WDEBoolean", SchemaHelpers.TARGET_NAMESPACE);
            complexType.Attributes.Add(attr);            

            return result;
        }

        #endregion
    }

    internal class SchemaHelpers
	{
		public const string XSD_NAMESPACE = "http://www.w3.org/2001/XMLSchema";
		public const string TARGET_NAMESPACE = "http://www.acs-inc.com/2.2/WebDEData";

		public static XmlNode[] TextToNodeArray(string text)
		{
			XmlDocument doc = new XmlDocument();
			return new XmlNode[1] {
				doc.CreateTextNode(text)
			};
		}
	}

	public class WDERecords : IWDERecords_R1, IWDEFilteredRecords, IWDEFilteredRecordsInternal, IWDEXmlPersist, IWDECollection, IEnumerable
	{
		private ArrayList m_Records;
		private IWDERecordsOwner m_Owner;
		private bool m_BOF;
		private bool m_EOF;
		private int m_Index;

		private WDERecords(IWDERecordsOwner Owner)
		{
			m_Records = new ArrayList();
			m_Owner = Owner;
			m_Index = -1;
			m_BOF = true;
			m_EOF = true;
		}

		public static IWDERecords Create(IWDERecordsOwner Owner)
		{
			return new WDERecords(Owner) as IWDERecords;
		}

		public static IWDERecords CreateInstance(IWDERecordsOwner Owner)
		{
			return Create(Owner);
		}

		#region IWDERecords Members

		public int Count
		{
			get
			{
				int count = m_Records.Count;

				if (!this.Document.DataSet.DisplayDeletedRows && m_Records.Count > 0)
				{
					count = 0;
					foreach (IWDERecord rec in this)
					{
						if (!rec.IsDeleted)
							count++;
					}
				}
				return count;
			}
		}


		public bool BOF
		{
			get
			{
				return m_BOF;
			}
		}

		public bool EOF
		{
			get
			{
				return m_EOF;
			}
		}

		public IWDERecord Current
		{
			get
			{
				if (m_Index != -1)
				{
					return (IWDERecord)m_Records[m_Index];
				}
				else
					return null;
			}
		}

		public string RecType
		{
			get
			{
				CheckCurrent();
				return Current.RecType;
			}
		}

		public int Index
		{
			get
			{
				return m_Index;
			}
			set
			{
				SetIndex(value);
			}
		}

		public IWDERecord this[int Index]
		{
			get
			{
				int ActualIndex = Index;
				// determine the actual index in m_records when showing only non-deleted records
				if (!this.Document.DataSet.DisplayDeletedRows)
				{
					ActualIndex = -1;
					int NonDeletedIndex = -1;
					for (int i = 0; i < m_Records.Count; i++)
					{
						IWDERecord rec = m_Records[i] as IWDERecord;
						ActualIndex++;
						if (!rec.IsDeleted)
						{
							NonDeletedIndex++;
							if (NonDeletedIndex == Index)
								break;
						}
					}
				}
				return (IWDERecord)m_Records[ActualIndex];
			}
		}

		public IWDERecords Records
		{
			get
			{
				CheckCurrent();
				return Current.Records;
			}
		}

		public IWDEDocument Document
		{
			get
			{
				return m_Owner.Document;
			}
		}

		public IWDERecord OwnerRecord
		{
			get
			{
				return m_Owner.Record;
			}
		}

		public bool IsDeleted
		{
			get
			{
				CheckCurrent();
				return Current.IsDeleted;
			}
		}

		public int SessionID
		{
			get
			{
				CheckCurrent();
				return Current.IsDeleted ? Current.SessionID : -1;
			}
		}

		public void Clear()
		{
			m_Index = -1;
			m_BOF = true;
			m_EOF = true;
			m_Records.Clear();
		}

		public void First()
		{
			SetIndex(Math.Min(0, m_Records.Count - 1));
			m_BOF = true;
			m_EOF = (m_Records.Count == 0);
		}

		public void Last()
		{
			SetIndex(m_Records.Count - 1);
			m_EOF = false;
			m_BOF = false;
		}

		public void Next()
		{
			CheckCurrent();
			if (Index < m_Records.Count - 1)
				Index += 1;
			else
				m_EOF = true;
		}

		public void Prior()
		{
			CheckCurrent();
			if (Index > 0)
				Index -= 1;
			else
				m_BOF = true;
		}

		public IWDERecord Append(string RecType)
		{
			if (RecType == null)
				throw new ArgumentNullException("RecType", "RecType cannot be null");				

			int index = -1;
			if (OwnerRecord == null)
				index = Document.DocumentDef.RecordDefs.Find(RecType);
			else
				index = OwnerRecord.RecordDef.RecordDefs.Find(RecType);
			if (index != -1)
			{
				IWDERecord rec = WDERecord.Create(this);
				IWDERecordInternal irec = (IWDERecordInternal)rec;
				irec.RecType = RecType;
				IWDERecordDef def = null;
				if (OwnerRecord == null)
					def = Document.DocumentDef.RecordDefs[index];
				else
					def = OwnerRecord.RecordDef.RecordDefs[index];
				irec.CreateFields(def); // this sets RecType and RecordDef as well
				m_Records.Add(rec);
				m_Index = m_Records.Count - 1;

				for (int i = 0; i < m_Records.Count; i++)
				{
					((IWDERecordInternal)m_Records[i]).RecIndex = i;
				}
				m_Records.Sort();

				return rec;
			}
			else
				throw new WDEException("API00013", new object[] { RecType, m_Owner.Document.DocType });

		}

		public IWDERecord Insert(int Index, string RecType)
		{
			if (((m_Records.Count > 0) && ((Index > m_Records.Count - 1) || (Index < 0))) ||
				((m_Records.Count == 0) && (Index != 0)))
				throw new WDEException("API00012", new object[] { "IWDERecords", Index, m_Records.Count });


			IWDERecord result = Append(RecType);
			// Fix for BPSENT-946
			// Find out actual index in the array list and set the new record's index
			if (!this.Document.DataSet.DisplayDeletedRows)
			{
				int ActualIndex = -1;
				for (int i = 0; i < m_Records.Count; i++)
				{
					IWDERecord record = m_Records[i] as IWDERecord;
					if (record.RecType == RecType && !record.IsDeleted)
					{
						if (++ActualIndex == Index)
						{
							Index = i;
							break;
						}
					}
				}
			}
			// Fix for BPSENT-946
			result.Index = Index;
			return result;
		}

		public void Delete()
		{
			CheckCurrent();
			m_Records.RemoveAt(m_Index);
			if (m_Records.Count > 0)
				SetIndex(Math.Min(m_Index, m_Records.Count - 1));
			else
				Clear();
		}

		public void DeleteRow(int RowIndex, string RecType)
		{
			int ActualIndex = -1;
			if (!String.IsNullOrEmpty(RecType))
			{
				for (int i = 0; i <= m_Records.Count - 1; i++)
				{
					IWDERecord record = m_Records[i] as IWDERecord;
					if (record.RecType == RecType)
					{
						ActualIndex = ActualIndex + 1;
						if (ActualIndex == RowIndex)
						{
							ActualIndex = i;
							break;
						}
					}
				}
			}
			else
			{
				ActualIndex = RowIndex;
			}

			DeleteRow(ActualIndex);
		}

		public void DeleteRow(int RowIndex)
		{
			if (!this.Document.DataSet.DisplayDeletedRows)
			{
				int ActualIndex = -1;
				for (int i = 0; i < m_Records.Count; i++)
				{
					IWDERecord record = m_Records[i] as IWDERecord;
					if (!record.IsDeleted && ++ActualIndex == RowIndex)
					{
						RowIndex = i;
						break;
					}
				}
			}

			m_Records.RemoveAt(RowIndex);

			if (m_Records.Count > 0)
				SetIndex(Math.Min(RowIndex, m_Records.Count - 1));
			else
				Clear();
		}

		public IWDEField_R1 FieldByName(string FieldName)
		{
			CheckCurrent();
			return Current.FieldByName(FieldName);
		}

		#endregion

		#region IWDERecords_R1 Members

		public IWDEFilteredRecords Filter(string RecType)
		{
			if (string.IsNullOrEmpty(RecType))
				throw new ArgumentNullException("RecType");

			IWDERecords result = WDERecords.Create(m_Owner);
			((WDERecords)result).FilterRecords(this, RecType);
			return result as IWDEFilteredRecords;
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "Record"))
			{
				m_Records.Clear();
				while ((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "Record"))
				{
					IWDERecord record = WDERecord.Create(this);
					IWDEXmlPersist ipers = (IWDEXmlPersist)record;
					ipers.ReadFromXml(XmlReader);
					m_Records.Add(record);
					SetIndex(0);
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if (XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				

			IWDEDocumentInternal doc = null;
			if ((Document != null) && (Document is IWDEDocumentInternal))
				doc = (IWDEDocumentInternal)Document;

			for (int i = 0; i < m_Records.Count; i++)
			{
				IWDERecord record = m_Records[i] as IWDERecord;
				IWDEXmlPersist ipers = (IWDEXmlPersist)record;
				if ((doc == null) ||
					((doc != null) && ((IWDERecordInternal)record).ContainsSessionID(doc.SaveSessionID))) // SaveSessionID supports the SaveSnapshot feature
					ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion

		#region Private Members

		private ArrayList GetCloneOfNonDeletedRecords()
		{
			ArrayList ShallowCopy = new ArrayList();

			//create a shallow copy of array list with non-deleted records 
			for (int index = 0; index < m_Records.Count; index++)
			{
				IWDERecord record = m_Records[index] as IWDERecord;
				if (!record.IsDeleted)
					ShallowCopy.Add(record);
			}

			return ShallowCopy;
		}

		private void SetIndex(int Value)
		{
			if (m_Index != Value)
			{
				if ((Value < 0) || (Value > m_Records.Count - 1))
						throw new WDEException("API00012", new object[] { "IWDERecords", Value, m_Records.Count });

				m_Index = Value;
				m_BOF = false;
				m_EOF = false;
			}
		}

		private void CheckCurrent()
		{
			if (Current == null)
			{
				if (m_Records.Count == 0)
						throw new WDEException("API00018");
				else
						throw new WDEException("API00011");
			}
		}

		internal void FilterRecords(IWDERecords Source, string RecType)
		{
			for (int i = 0; i < Source.Count; i++)
			{
				if (string.Compare(Source[i].RecType, RecType, true) == 0)
					m_Records.Add(Source[i]);
			}
		}

		#endregion

		#region IWDECollection Members

		public int GetIndex(object Item)
		{
			return m_Records.IndexOf(Item);
		}

		void WebDX.Api.IWDECollection.SetIndex(object Item, int NewIndex)
		{
			int index = GetIndex(Item);
			if (index != -1)
			{
				m_Records.RemoveAt(index);
				m_Records.Insert(NewIndex, Item);
			}
			else
				throw new WDEException("API90002", new object[] { "IWDERecords" });
		}

		public void RemoveAt(int Index)
		{
			m_Records.RemoveAt(Index);
		}

		#endregion

		#region IWDEFilteredRecordsInternal Members

		public void FilterRecords(string RecType)
		{
			if (RecType == null)
				throw new ArgumentNullException("RecType", "RecType cannot be null");
				

			Clear();
			FilterRecords(m_Owner.Document.Records, RecType);
			First();
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			IEnumerable ienum;
			if (!this.Document.DataSet.DisplayDeletedRows)
			{
				ienum = (IEnumerable)GetCloneOfNonDeletedRecords();
				return ienum.GetEnumerator();
			}
			else
			{
				ienum = (IEnumerable)m_Records;
				return ienum.GetEnumerator();
			}
		}


		#endregion
	}

	public class WDERecord : IWDERecord_R1, IWDERecordInternal, IWDEXmlPersist, IWDERecordsOwner, IComparable
	{
		private IWDERecords m_OwnerRecords;
		private Rectangle m_DetailRect;
		private IWDEFields m_Fields;
		private IWDERecordDef m_RecordDef;
		private string m_RecType;
        private string m_ImageName;
		private IWDERecords m_ChildRecords;
		private bool m_IsDeleted;
		private int m_SessionID;
		private int m_RecIndex;
        private bool m_RemoveImageData;

		private WDERecord(IWDERecords Owner)
		{
			m_IsDeleted = false;
			m_SessionID = -1;
			m_OwnerRecords = Owner;
			m_DetailRect = new Rectangle(0, 0, 0, 0);
			m_Fields = WDEFields.Create(this);
			m_ChildRecords = WDERecords.Create(this);
            m_RemoveImageData = false;
		}

		public static IWDERecord Create(IWDERecords Owner)
		{
			return new WDERecord(Owner) as IWDERecord;
		}

		public static IWDERecord CreateInstance(IWDERecords Owner)
		{
			return Create(Owner);
		}

		#region IWDERecord Members
		[VersionPropertyFilter("2.5.0.0", null, "IsDeleted", "IWDERecord")]
		public bool IsDeleted
		{
			get
			{
				return m_IsDeleted;
			}
		}
		[VersionPropertyFilter("2.5.0.0", null, "SessionID", "IWDERecord")]
		public int SessionID
		{
			get
			{
				return m_SessionID;
			}
		}

		public IWDERecords OwnerRecords
		{
			get
			{
				return m_OwnerRecords;
			}
		}

		public IWDEDocument Document
		{
			get
			{
				return m_OwnerRecords.Document;
			}
		}

		public Rectangle DetailRect
		{
			get
			{
				return m_DetailRect;
			}
			set
			{
				m_DetailRect = value;
			}
		}

		public IWDEFields Fields
		{
			get
			{
				return m_Fields;
			}
		}

		public int FlaggedFieldCount
		{
			get
			{
				int result = 0;
				for (int i = 0; i < Records.Count; i++)
					result += Records[i].FlaggedFieldCount;

				for (int i = 0; i < Fields.Count; i++)
					result += Fields[i].Flagged ? 1 : 0;
				return result;
			}
		}

		public int Index
		{
			get
			{
				IWDECollection icoll = (IWDECollection)m_OwnerRecords;
				return icoll.GetIndex((IWDERecord)this);
			}
			set
			{
				IWDECollection icoll = (IWDECollection)m_OwnerRecords;
				icoll.SetIndex((IWDERecord)this, value);
			}
		}

		public IWDERecord ParentRecord
		{
			get
			{
				return m_OwnerRecords.OwnerRecord;
			}
		}

		public IWDERecordDef RecordDef
		{
			get
			{
				if ((m_RecordDef == null) || (string.Compare(m_RecordDef.RecType, RecType, true) != 0))
				{
					if (Document.DataSet.Project == null)
							throw new WDEException("API00001");
					else
					{
						int index = -1;
						if (ParentRecord == null)
							index = Document.DocumentDef.RecordDefs.Find(RecType);
						else
							index = ParentRecord.RecordDef.RecordDefs.Find(RecType);
						if (index == -1)
								throw new WDEException("API00035", new object[] { Document.DocType, RecType });
						else
						{
							if (ParentRecord == null)
								m_RecordDef = Document.DocumentDef.RecordDefs[index];
							else
								m_RecordDef = ParentRecord.RecordDef.RecordDefs[index];
						}
					}
				}
				return m_RecordDef;
			}

			set
			{
				m_RecordDef = value;
			}
		}

		public string RecType
		{
			get
			{
				return m_RecType;
			}

			set
			{
				if (value == null)
					m_RecType = "";
				else
					m_RecType = value;
			}
		}

        [VersionPropertyFilter("3.2.2.0", null, "ImageName", "IWDERecord")]
        public string ImageName
        {
            get
            {
                return m_ImageName;
            }

            set
            {
                if (value == null)
                    m_ImageName = "";
                else
                    m_ImageName = value;
            }
        }

		public IWDERecords Records
		{
			get
			{
				return m_ChildRecords;
			}
		}

		public void Delete()
		{
			if (!m_IsDeleted)
			{
				m_IsDeleted = true;
				m_SessionID = this.Document.DataSet.Sessions[0].SessionID;
				foreach (WDEField field in this.Fields)
					field.SetValueAndStatus(string.Empty, WDEFieldStatus.Verified, 0);
			}
		}

		public void RestoreDeletedRow()
		{
			if (m_IsDeleted)
			{
				int sessionId = this.Document.DataSet.Sessions[0].SessionID;

				if (m_SessionID == sessionId)
				{
					foreach (WDEField field in this.Fields)
						((IWDERevisionsInternal)field.Revisions).Delete();
				}

				foreach (WDEField field in this.Fields)
					field.SetValueAndStatus(null, WDEFieldStatus.None, 0);

				m_IsDeleted = false;
				m_SessionID = -1;
			}
		}

		public IWDEField_R1 FieldByName(string FieldName)
		{
			IWDEField_R1 result = FindField(FieldName);
			if (result == null)
				throw new WDEException("API00010", new object[] { FieldName });
			else
			{
				return result;
			}
		}

		public IWDEField_R1 FindField(string FieldName)
		{
			int index = Fields.Find(FieldName);
			if (index == -1)
			{
				CreateMissingFields();
				index = Fields.Find(FieldName);
				if (index == -1)
					return null;
				else
					return Fields[index];
			}
			else
				return Fields[index];
		}

        [VersionPropertyFilter("3.3.3.0", null, "RemoveImageData", "IWDERecord")]
        public bool RemoveImageData
        {
            get
            {
                return m_RemoveImageData;
            }
            set
            {
                m_RemoveImageData = value;
            }
        }

		#endregion

		#region IWDERecordInternal Members

		public void CreateMissingFields()
		{
			if (Document.DataSet.Project != null && RecordDef != null)
			{
				for (int i = 0; i < RecordDef.FieldDefs.Count; i++)
				{
					IWDEFieldDef def = RecordDef.FieldDefs[i];
					int index = Fields.Find(def.FieldName);
					if (index == -1)
					{
						IWDEFieldsInternal ifields = (IWDEFieldsInternal)Fields;
						ifields.Add(def);
					}
				}
			}

            foreach (IWDERecordInternal rec in Records)
                rec.CreateMissingFields();
		}

		public int RecIndex
		{
			get
			{
				return m_RecIndex;
			}
			set
			{
				m_RecIndex = value;
			}
		}

		public void CreateFields(IWDERecordDef RecordDef)
		{
			if ((this.RecordDef != RecordDef) || (Fields.Count == 0))
			{
				IWDEFieldsInternal ifields = (IWDEFieldsInternal)Fields;

				this.RecordDef = RecordDef;
				this.RecType = RecordDef.RecType;

				if (this.RecordDef != null)
					ifields.Clear();

				for (int i = 0; i < RecordDef.FieldDefs.Count; i++)
				{
					ifields.Add(RecordDef.FieldDefs[i]);
				}
			}
		}

		public bool ContainsSessionID(int SessionID)
		{
			if (SessionID == -1)
				return true;

			foreach (IWDEField field in Fields)
			{
				foreach (IWDERevision rev in field.Revisions)
				{
					if (rev.SessionID <= SessionID)
						return true;
				}
			}
			return false;
		}

		public bool IsBlankOrNull()
		{
			foreach (IWDEField_R1 field in Fields)
			{
				if (field.CustomData == "")
				{
					foreach (IWDERevision rev in field.Revisions)
					{
						if (((!rev.IsNull) && (rev.Value != "")) || (rev.Flagged))
							return false;
					}
				}
			}

			return true;
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "Record"))
			{
				m_DetailRect = Utils.GetRectValue(XmlReader, "DetailRect");
				m_RecType = Utils.GetAttribute(XmlReader, "RecType", "");
                m_ImageName = Utils.GetAttribute(XmlReader, "ImageName", "");
                m_IsDeleted = Utils.GetBoolValue(XmlReader, "IsDeleted", false);
				if (m_IsDeleted)
				{
					m_SessionID = Utils.GetIntValue(XmlReader, "SessionID");
				}
				bool empty = XmlReader.IsEmptyElement;

				XmlReader.Read();
				XmlReader.MoveToContent();

				if (!empty)
				{
					IWDEXmlPersist ipers = (IWDEXmlPersist)Fields;
					ipers.ReadFromXml(XmlReader);

					ipers = (IWDEXmlPersist)Records;
					ipers.ReadFromXml(XmlReader);
				}

				if (XmlReader.NodeType == XmlNodeType.EndElement)
					XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if (XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				

			XmlWriter.WriteStartElement("Record");
			if (!DetailRect.IsEmpty)
				XmlWriter.WriteAttributeString("DetailRect", Utils.RectToString(m_DetailRect));
			XmlWriter.WriteAttributeString("RecType", RecType);
            if (!string.IsNullOrEmpty(m_ImageName))
            {
                if (!VersionHelper.FilterPropertyOrCollection("IWDERecord.ImageName", VersionInfo.TargetVersionNumber))
                {
                    XmlWriter.WriteAttributeString("ImageName", m_ImageName);
                }
                else
                {
                    VersionHelper.LogDataLost("IWDERecord.ImageName", m_ImageName);
                }
            }
			if (m_IsDeleted)
			{
				if (!VersionHelper.FilterPropertyOrCollection("IWDERecord.IsDeleted", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("IsDeleted", m_IsDeleted.ToString());
				}
				else
				{
					VersionHelper.LogDataLost("IWDERecord.IsDeleted", m_IsDeleted.ToString());
				}

				if (!VersionHelper.FilterPropertyOrCollection("IWDERecord.SessionID", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("SessionID", m_SessionID.ToString());
				}
				else
				{
					VersionHelper.LogDataLost("IWDERecord.SessionID", m_SessionID.ToString());
				}
			}

			IWDEXmlPersist ipers = (IWDEXmlPersist)Fields;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist)Records;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.WriteEndElement();
		}

		#endregion

		#region IWDERecordsOwner Members

		public IWDERecord Record
		{
			get
			{
				return (IWDERecord)this;
			}
		}

		#endregion

		#region Internal Methods

		internal static XmlSchemaElement GetSchema(List<Type> usedEnums)
		{
			XmlSchemaElement result = new XmlSchemaElement();
			result.Name = "Record";

			XmlSchemaComplexType complexType = new XmlSchemaComplexType();
			result.SchemaType = complexType;

			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexType.Particle = seq;

			XmlSchemaAttribute attr = new XmlSchemaAttribute();
			attr.Name = "RecType";
			attr.Use = XmlSchemaUse.Required;
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

            attr = new XmlSchemaAttribute();
            attr.Name = "ImageName";
            attr.Use = XmlSchemaUse.Optional;
            attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
            complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "IsDeleted";
			attr.Use = XmlSchemaUse.Optional;
			attr.SchemaTypeName = new XmlQualifiedName("WDEBoolean", SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "SessionID";
			attr.Use = XmlSchemaUse.Optional;
			attr.SchemaTypeName = new XmlQualifiedName("int", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

            attr = new XmlSchemaAttribute();
            attr.Name = "DetailRect";
            attr.Use = XmlSchemaUse.Optional;
            attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
            complexType.Attributes.Add(attr);

			seq.Items.Add(WDEField.GetSchema(usedEnums));
			XmlSchemaElement recRef = new XmlSchemaElement();
			recRef.RefName = new XmlQualifiedName("Record", SchemaHelpers.TARGET_NAMESPACE);
			recRef.MinOccurs = 0;
			recRef.MaxOccursString = "unbounded";
			seq.Items.Add(recRef);

			return result;
		}

		#endregion

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (obj is IWDERecordInternal)
			{
				IWDERecordInternal record = obj as IWDERecordInternal;
				int retValue = this.RecType.CompareTo(record.RecType);
				if (retValue == 0)
					return this.RecIndex.CompareTo(record.RecIndex);
				else
					return retValue;
			}
			throw new ArgumentException( "Object is not a WDERecord");
		}

		#endregion
	}

	public class WDEDocSessions : IWDEDocSessions, IWDEDocSessionsInternal, IWDEXmlPersist, IEnumerable
	{
		private IWDEDocument m_Document;
		private ArrayList m_Sessions;

		private WDEDocSessions(IWDEDocument Owner)
		{
			m_Document = Owner;
			m_Sessions = new ArrayList(50);
		}

		public static IWDEDocSessions Create(IWDEDocument Owner)
		{
			return new WDEDocSessions(Owner) as IWDEDocSessions;
		}

		public static IWDEDocSessions CreateInstance(IWDEDocument Owner)
		{
			return Create(Owner);
		}

		#region IWDEDocSessions Members

		public IWDEDocument Document
		{
			get
			{
				return m_Document;
			}
		}

		public int Count
		{
			get
			{
				return m_Sessions.Count;
			}
		}

		public IWDEDocSession_R1 this[int Index]
		{
			get
			{
				return (IWDEDocSession_R1)m_Sessions[Index];
			}
		}

		public int FindByID(int ID)
		{
			for (int i = 0; i < Count; i++)
			{
				if (this[i].SessionID == ID)
					return i;
			}
			return -1;
		}

		public int FindByUser(string User)
		{
			if (User == null)
				throw new ArgumentNullException("User", "User cannot be null");
				

			for (int i = 0; i < Count; i++)
			{
				if (string.Compare(this[i].User, User, true) == 0)
					return i;
			}
			return -1;
		}

		public int FindByTask(string Task)
		{
			if (Task == null)
				throw new ArgumentNullException("Task", "Task cannot be null");				

			for (int i = 0; i < Count; i++)
			{
				if (string.Compare(this[i].Task, Task, true) == 0)
					return i;
			}
			return -1;
		}

		public void Clear()
		{
			m_Sessions.Clear();
		}

		#endregion

		#region IWDEDocSessionsInternal Members

		public IWDEDocSession_R1 Add(string User, string Task, WebDX.Api.WDEOpenMode Mode, int SessionID, string location)
		{
			if (User == null)
				throw new ArgumentNullException("User", "User cannot be null");				
			if (Task == null)
				throw new ArgumentNullException("Task", "Task cannot be null");				

			IWDEDocSession_R1 result = WDEDocSession.Create(this);
			m_Sessions.Insert(0, result);
			IWDEDocSessionInternal isess = (IWDEDocSessionInternal)result;
			isess.User = User;
			isess.Task = Task;
			isess.Mode = Mode;
			isess.SessionID = SessionID;
			isess.Location = location;
			if (m_Document.DataSet != null)
				isess.StartTime = m_Document.DataSet.Sessions[0].StartTime;
			return result;
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "Session"))
			{
				m_Sessions.Clear();
				while ((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "Session"))
				{
					IWDEDocSession_R1 session = WDEDocSession.Create(this);
					IWDEXmlPersist ipers = (IWDEXmlPersist)session;
					ipers.ReadFromXml(XmlReader);
					m_Sessions.Add(session);
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if (XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				

			IWDEDocumentInternal doc = null;
			if ((Document != null) && (Document is IWDEDocumentInternal))
				doc = (IWDEDocumentInternal)Document;
			for (int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist)this[i];
				if ((doc == null) || (doc.SaveSessionID == -1) ||
					((doc.SaveSessionID != -1) && (this[i].SessionID <= doc.SaveSessionID)))
					ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			IEnumerable ienum = (IEnumerable)m_Sessions;
			return ienum.GetEnumerator();
		}

		#endregion
	}

	public class WDEDocSession : IWDEDocSession_R4, IWDEDocSessionInternal, IWDEXmlPersist
	{
		private IWDEDocSessions m_Sessions;
		private string m_User;
		private string m_Task;
		private WDEOpenMode m_Mode;
		private string m_RejectCode;
		private string m_RejectDescription;
		private int m_SessionID;
		private string m_Location;
		private DateTime m_StartTime;
		private DateTime m_EndTime;
		private TimeSpan m_Duration;
		private WDESessionStatus m_Status;
        private string m_RejectField;
        private int m_RejectRow;

		private WDEDocSession(IWDEDocSessions Owner)
		{
			m_Sessions = Owner;
			m_User = "";
			m_Task = "";
			m_RejectCode = "";
			m_RejectDescription = "";
			m_Location = "";
			m_StartTime = DateTime.MinValue;
			m_EndTime = DateTime.MinValue;
			m_Duration = TimeSpan.Zero;
            m_RejectField = "";
            m_RejectRow = -1;
		}

		public static IWDEDocSession_R4 Create(IWDEDocSessions Owner)
		{
			return new WDEDocSession(Owner) as IWDEDocSession_R4;
		}

		public static IWDEDocSession_R4 CreateInstance(IWDEDocSessions Owner)
		{
			return Create(Owner);
		}

		#region IWDEDocSession_R1 Members

		public string User
		{
			get
			{
				return m_User;
			}

			set
			{
				if (value == null)
					m_User = "";
				else
					m_User = value;
			}
		}

		public string Task
		{
			get
			{
				return m_Task;
			}

			set
			{
				if (value == null)
					m_Task = "";
				else
					m_Task = value;
			}
		}

		public WebDX.Api.WDEOpenMode Mode
		{
			get
			{
				return m_Mode;
			}

			set
			{
				m_Mode = value;
			}
		}

		public string RejectCode
		{
			get
			{
				return m_RejectCode;
			}
			set
			{
				if (value == null)
					m_RejectCode = "";
				else
					m_RejectCode = value;
			}
		}

		public string RejectDescription
		{
			get
			{
				return m_RejectDescription;
			}
			set
			{
				if (value == null)
					m_RejectDescription = "";
				else
					m_RejectDescription = value;
			}
		}

		public int SessionID
		{
			get
			{
				return m_SessionID;
			}
			set
			{
				m_SessionID = value;
			}
		}

		public string Location
		{
			get
			{
				return m_Location;
			}
			set
			{
				if (value == null)
					m_Location = "";
				else
					m_Location = value;
			}
		}

		public WebDX.Api.WDESessionStatus Status
		{
			get
			{
				return m_Status;
			}
			set
			{
				m_Status = value;
			}
		}

		public int CharCount
		{
			get
			{
				return SumCharCount();
			}
		}

		public int VCECount
		{
			get
			{
				return SumVCECount();
			}
		}

		public void SaveSnapshot(string fileName)
		{
			FileStream fst = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
			try
			{
				SaveSnapshot(fst);
			}
			finally
			{
				fst.Close();
			}
		}

		public void SaveSnapshot(System.IO.Stream aStream)
		{
			GC.Collect();
			MemoryStream memStream = new MemoryStream();
			XmlTextWriter xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
			try
			{
				xmlWriter.Formatting = Formatting.Indented;
				xmlWriter.WriteStartDocument(true);
				SaveSnapshot(xmlWriter);
			}
			finally
			{
				xmlWriter.Close();
				xmlWriter = null;
			}

			MemoryStream saveStream = new MemoryStream(memStream.ToArray());
			try
			{
				Utils.CopyStream(saveStream, aStream);
			}
			finally
			{
				saveStream.Close();
				saveStream = null;
				memStream = null;
				GC.Collect();
			}
		}

		public void SaveSnapshot(System.Runtime.InteropServices.ComTypes.IStream aStream)
		{
			MemoryStream memStream = new MemoryStream();
			try
			{
				SaveSnapshot(memStream);
				memStream.Seek(0, SeekOrigin.Begin);
				Utils.StreamToUCOMIStream(memStream, aStream);
			}
			finally
			{
				memStream.Close();
			}
		}

		public byte[] SaveSnapshot()
		{
			MemoryStream memStream = new MemoryStream();
			try
			{
				SaveSnapshot(memStream);
				return memStream.ToArray();
			}
			finally
			{
				memStream.Close();
			}
		}

		#endregion

		#region IWDEDocSession_R2 Members

		public DateTime StartTime
		{
			get { return m_StartTime; }
			set { m_StartTime = value; }
		}

		public DateTime EndTime
		{
			get { return m_EndTime; }
			set { m_EndTime = value; }
		}

		#endregion

		#region IWDEDocSession_R3 Members

		[VersionPropertyFilter("2.6.0.0", null, "Duration", "IWDEDocSession")]
		public TimeSpan Duration
		{
			get { return m_Duration; }
			set { m_Duration = value; }
		}

		#endregion

        #region IWDEDocSession_R4 Members

        [VersionPropertyFilter("3.2.1.0", null, "RejectField", "IWDEDocSession")]
        public string RejectField
        {
            get
            {
                return m_RejectField;
            }
            set
            {
                if (value == null)
                    m_RejectField = "";
                else
                    m_RejectField = value;
            }
        }

        [VersionPropertyFilter("3.2.1.0", null, "RejectRow", "IWDEDocSession")]
        public int RejectRow
        {
            get
            {
                return m_RejectRow;                
            }
            set
            {
                m_RejectRow = value;
            }
        }

        #endregion

        #region IWDEDocSessionInternal Members

		public void SaveSnapshot(XmlTextWriter xmlWriter)
		{
			IWDEDocumentInternal doc = (IWDEDocumentInternal)m_Sessions.Document;
			doc.SaveSessionID = SessionID;
			try
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist)doc;
				ipers.WriteToXml(xmlWriter);
			}
			finally
			{
				doc.SaveSessionID = -1;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "Session"))
			{
				m_User = Utils.GetAttribute(XmlReader, "User", "");
				m_Task = Utils.GetAttribute(XmlReader, "Task", "");
				m_Location = Utils.GetAttribute(XmlReader, "Location", "");
				m_Mode = Utils.GetMode(XmlReader, "Mode");
				m_RejectCode = Utils.GetAttribute(XmlReader, "RejectCode", "");
				m_RejectDescription = Utils.GetAttribute(XmlReader, "RejectDescription", "");

                m_RejectField = Utils.GetAttribute(XmlReader, "RejectField", "");
                m_RejectRow = int.Parse(Utils.GetAttribute(XmlReader, "RejectRow", "-1"));

				m_Status = Utils.GetStatus(XmlReader, "Status");
				m_SessionID = Utils.GetIntValue(XmlReader, "SessionID");
				m_Duration = Utils.GetTimeSpanValue(XmlReader, "Duration");

				string temp = Utils.GetAttribute(XmlReader, "StartTime", "");
				if (temp != "")
				{
					try
					{
						m_StartTime = Utils.ISOToDateTime(temp);
					}
					catch
					{
						m_StartTime = Utils.USStrToDateTime(temp);
					}
				}

				temp = Utils.GetAttribute(XmlReader, "EndTime", "");
				if (temp != "")
				{
					try
					{
						m_EndTime = Utils.ISOToDateTime(temp);
					}
					catch
					{
						m_EndTime = Utils.USStrToDateTime(temp);
					}
				}
				XmlReader.Read();
				XmlReader.MoveToContent();
				if (XmlReader.NodeType == XmlNodeType.EndElement)
					XmlReader.ReadEndElement();
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if (XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				

			XmlWriter.WriteStartElement("Session");
			XmlWriter.WriteAttributeString("User", User);
			XmlWriter.WriteAttributeString("Task", Task);
			XmlWriter.WriteAttributeString("Mode", VersionHelper.GetEnumerationString(typeof(WDEOpenMode).Name, m_Mode, VersionInfo.TargetVersionNumber));
			if (!string.IsNullOrEmpty(RejectCode))
			{
				XmlWriter.WriteAttributeString("RejectCode", RejectCode);
			}
			if (RejectDescription != "")
				XmlWriter.WriteAttributeString("RejectDescription", RejectDescription);

            if (RejectField != "")
            {
                if (!VersionHelper.FilterPropertyOrCollection("IWDEDocSession.RejectField", VersionInfo.TargetVersionNumber))
                    XmlWriter.WriteAttributeString("RejectField", RejectField);
                else
                    VersionHelper.LogDataLost("IWDEDocSession.RejectField", RejectField);

            }
            if (RejectRow >= 0)
            {
                if (!VersionHelper.FilterPropertyOrCollection("IWDEDocSession.RejectRow", VersionInfo.TargetVersionNumber))
                    XmlWriter.WriteAttributeString("RejectRow", RejectRow.ToString());
                else
                    VersionHelper.LogDataLost("IWDEDocSession.RejectRow", RejectRow.ToString());
            }

			XmlWriter.WriteAttributeString("Status", VersionHelper.GetEnumerationString(typeof(WDESessionStatus).Name, Status, VersionInfo.TargetVersionNumber));

			XmlWriter.WriteAttributeString("SessionID", SessionID.ToString());
			if (m_StartTime != DateTime.MinValue)
				XmlWriter.WriteAttributeString("StartTime", Utils.DateTimeToISO(m_StartTime));
			if (m_EndTime != DateTime.MinValue)
                XmlWriter.WriteAttributeString("EndTime", Utils.DateTimeToISO(EndTime));			

			if (m_Duration != TimeSpan.Zero)
			{
                if (!VersionHelper.FilterPropertyOrCollection("IWDEDocSession.Duration", VersionInfo.TargetVersionNumber))
                    XmlWriter.WriteAttributeString("Duration", SoapDuration.ToString(m_Duration));
				else
					VersionHelper.LogDataLost("IWDEDocSession.Duration", SoapDuration.ToString(m_Duration));
			}
			

			if (CharCount != 0)
				XmlWriter.WriteAttributeString("CharCount", CharCount.ToString());

			if (VCECount != 0)
				XmlWriter.WriteAttributeString("VCECount", VCECount.ToString());

			if (Location != "")
				XmlWriter.WriteAttributeString("Location", Location);

			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Internal Methods

		internal static XmlSchemaElement GetSchema(List<Type> usedEnums)
		{
			XmlSchemaElement result = new XmlSchemaElement();
			result.Name = "Session";
			result.MinOccurs = 1;
			result.MaxOccursString = "unbounded";

			XmlSchemaComplexType complexType = new XmlSchemaComplexType();
			result.SchemaType = complexType;

			XmlSchemaAttribute attr = new XmlSchemaAttribute();
			attr.Name = "User";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Use = XmlSchemaUse.Required;
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "Task";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Use = XmlSchemaUse.Required;
			complexType.Attributes.Add(attr);

			if (!usedEnums.Contains(typeof(WDEOpenMode)))
				usedEnums.Add(typeof(WDEOpenMode));

			attr = new XmlSchemaAttribute();
			attr.Name = "Mode";
			attr.SchemaTypeName = new XmlQualifiedName(typeof(WDEOpenMode).Name, SchemaHelpers.TARGET_NAMESPACE);
			attr.Use = XmlSchemaUse.Required;
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "RejectCode";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Annotation = new XmlSchemaAnnotation();
			XmlSchemaDocumentation doc = new XmlSchemaDocumentation();
			attr.Annotation.Items.Add(doc);
			doc.Markup = SchemaHelpers.TextToNodeArray("Must match a valid reject code in the associated project file.");
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "RejectDescription";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Annotation = new XmlSchemaAnnotation();
			doc = new XmlSchemaDocumentation();
			attr.Annotation.Items.Add(doc);
			doc.Markup = SchemaHelpers.TextToNodeArray("Matches the reject description from the project file associated with RejectCode.");
			complexType.Attributes.Add(attr);

			if (!usedEnums.Contains(typeof(WDESessionStatus)))
				usedEnums.Add(typeof(WDESessionStatus));

			attr = new XmlSchemaAttribute();
			attr.Name = "Status";
			attr.SchemaTypeName = new XmlQualifiedName(typeof(WDESessionStatus).Name, SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "SessionID";
			attr.SchemaTypeName = new XmlQualifiedName("unsignedInt", SchemaHelpers.XSD_NAMESPACE);
			attr.Use = XmlSchemaUse.Required;
			attr.Annotation = new XmlSchemaAnnotation();
			doc = new XmlSchemaDocumentation();
			attr.Annotation.Items.Add(doc);
			doc.Markup = SchemaHelpers.TextToNodeArray("Identifer for this Session. Used in revisions to tie the revisions to this Session.");
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "StartTime";
			attr.SchemaTypeName = new XmlQualifiedName("dateTime", SchemaHelpers.XSD_NAMESPACE);
			attr.Use = XmlSchemaUse.Required;
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "EndTime";
			attr.SchemaTypeName = new XmlQualifiedName("dateTime", SchemaHelpers.XSD_NAMESPACE);
			attr.Use = XmlSchemaUse.Required;
			complexType.Attributes.Add(attr);

            attr = new XmlSchemaAttribute();
            attr.Name = "Duration";
            attr.SchemaTypeName = new XmlQualifiedName(SoapDuration.XsdType, SchemaHelpers.XSD_NAMESPACE);
            complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "CharCount";
			attr.SchemaTypeName = new XmlQualifiedName("unsignedInt", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "VCECount";
			attr.SchemaTypeName = new XmlQualifiedName("unsignedInt", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "Location";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			return result;
		}

		#endregion

		#region Private Members

		private int SumCharCount()
		{
			return GetCharSum(m_Sessions.Document.Records);
		}

		private int SumVCECount()
		{
			return GetVCESum(m_Sessions.Document.Records);
		}

		private int GetVCESum(IWDERecords recs)
		{
			int result = 0;
			for (int i = 0; i < recs.Count; i++)
			{
				IWDERecord rec = recs[i];
				if (rec.Document.DataSet.DisplayDeletedRows && rec.IsDeleted)
					continue;  // SKIP records marked as deleted

				for (int j = 0; j < recs[i].Fields.Count; j++)
				{
					for (int k = 0; k < recs[i].Fields[j].Revisions.Count; k++)
					{
						if (recs[i].Fields[j].Revisions[k].SessionID == SessionID)
							result += recs[i].Fields[j].Revisions[k].VCECount;
					}
				}
				result += GetVCESum(recs[i].Records);
			}
			return result;
		}

		private int GetCharSum(IWDERecords recs)
		{
			int result = 0;
			for (int i = 0; i < recs.Count; i++)
			{
				IWDERecord rec = recs[i];
				if (rec.Document.DataSet.DisplayDeletedRows && rec.IsDeleted)
					continue;  // SKIP records marked as deleted

				for (int j = 0; j < rec.Fields.Count; j++)
				{
					IWDEField_R1 field = rec.Fields[j];
					for (int k = 0; k < field.Revisions.Count; k++)
					{
						IWDERevision rev = field.Revisions[k];
                        if (rev.SessionID == SessionID)
						{
							int count = rev.CharCount;
							result += count;
							// the condition (rev.CharCount != 0) is required here to check
							// if there was a deletion ever on the row after the fresh data was keyed in (BPSENT - 1007)

							if ((rev.Status == WDEFieldStatus.Verified) &&
								(count != 0) &&
								(k < field.Revisions.Count - 1) &&
								(field.Revisions[k + 1].SessionID == SessionID) &&
								(field.Revisions[k + 1].Status == WDEFieldStatus.Keyed))
								result += field.Revisions[k + 1].CharCount;
							break;
						}
					}
				}
				result += GetCharSum(rec.Records);
			}
			return result;
		}

		#endregion
	}

	public class WDESessions : IWDESessions, IWDESessionsInternal, IWDEXmlPersist, IEnumerable
	{
		private IWDEDataSet m_DataSet;
		private ArrayList m_Sessions;

		private WDESessions(IWDEDataSet Owner)
		{
			m_DataSet = Owner;
			m_Sessions = new ArrayList();
		}

		public static IWDESessions Create(IWDEDataSet Owner)
		{
			return new WDESessions(Owner) as IWDESessions;
		}

		public static IWDESessions CreateInstance(IWDEDataSet Owner)
		{
			return Create(Owner);
		}

		#region IWDESessions Members

		public int Count
		{
			get
			{
				return m_Sessions.Count;
			}
		}

		public IWDESession_R1 this[int Index]
		{
			get
			{
				return (IWDESession_R1)m_Sessions[Index];
			}
		}

		public IWDEDataSet DataSet
		{
			get
			{
				return m_DataSet;
			}
		}

		public int FindByID(int ID)
		{
			for (int i = 0; i < Count; i++)
			{
				if (this[i].SessionID == ID)
					return i;
			}
			return -1;
		}

		public int FindByUser(string User)
		{
			if (User == null)
				throw new ArgumentNullException("User", "User cannot be null");				

			for (int i = 0; i < Count; i++)
			{
				if (string.Compare(this[i].User, User, true) == 0)
					return i;
			}
			return -1;
		}

		public int FindByTask(string Task)
		{
			if (Task == null)
				throw new ArgumentNullException("Task", "Task cannot be null");				

			for (int i = 0; i < Count; i++)
			{
				if (string.Compare(this[i].Task, Task, true) == 0)
					return i;
			}
			return -1;
		}

		#endregion

		#region IWDESessionsInternal Members

		public IWDESession_R1 Add(string User, string Task, WebDX.Api.WDEOpenMode Mode, string Location)
		{
			return Insert(0, User, Task, Mode, Location);
		}

		public IWDESession_R1 Insert(int Index, string User, string Task, WebDX.Api.WDEOpenMode Mode, string Location)
		{
			IWDESession_R1 sess = CreateNewSession(User, Task, Mode, Location);
			m_Sessions.Insert(Index, sess);
			return sess;
		}

		public IWDESession_R1 Append(string User, string Task, WebDX.Api.WDEOpenMode Mode, string Location)
		{
			IWDESession_R1 sess = CreateNewSession(User, Task, Mode, Location);
			m_Sessions.Add(sess);
			return sess;
		}

		public void Merge(IWDESession_R1 newSession)
		{
			if (m_Sessions.Count == 0)
				m_Sessions.Add(newSession);
			else
			{
				bool inserted = false;
				for (int i = 0; i < m_Sessions.Count; i++)
				{
					if (this[i].SessionID < newSession.SessionID)
					{
						m_Sessions.Insert(i, newSession);
						inserted = true;
						break;
					}
				}

				if (!inserted)
					m_Sessions.Add(newSession);
			}
		}

		public void Clear()
		{
			m_Sessions.Clear();
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			//Read session information from pre-2.0 data files.
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "Session"))
			{
				m_Sessions.Clear();
				while ((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "Session"))
				{
					IWDESession_R1 session = WDESession.Create(this);
					IWDEXmlPersist ipers = (IWDEXmlPersist)session;
					ipers.ReadFromXml(XmlReader);
					m_Sessions.Add(session);
				}
			}

			if (Count == 0)
				LoadFromDocData();
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			/*Do nothing. 2.0 and above stores this information at the document level.*/

			Version targetVersion = new Version(VersionInfo.TargetVersionNumber);
			if (targetVersion < new Version("2.0.0.0"))
			{

			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}

			}
		}

		#endregion

		#region Private Members

		private IWDESession_R1 CreateNewSession(string User, string Task, WebDX.Api.WDEOpenMode Mode, string Location)
		{
			if (User == null)
				throw new ArgumentNullException("User", "User cannot be null");			
			if (Task == null)
				throw new ArgumentNullException("Task", "Task cannot be null");				
			if (Location == null)
				throw new ArgumentNullException("Location", "Location cannot be null");
				
			IWDESession_R1 sess = WDESession.Create(this);
			IWDESessionInternal isess = (IWDESessionInternal)sess;
			isess.User = User;
			isess.Task = Task;
			isess.Mode = Mode;
			isess.Location = Location;
			isess.SessionID = GetNextSessionID();

			if (((WDEDataSet)m_DataSet).UserClient)
				isess.StartTime = DateTime.MinValue;
			else
			isess.StartTime = DateTime.Now;
			isess.EndTime = DateTime.MinValue;
			return sess;
		}

		private int GetNextSessionID()
		{
			int result = GetHighestSessionID();
			return result + 1;
		}

		private int GetHighestSessionID()
		{
			int result = 0;
			for (int i = 0; i < DataSet.Documents.Count; i++)
			{
				IWDEDocument doc = DataSet.Documents[i];
				for (int j = 0; j < doc.Sessions.Count; j++)
				{
					IWDEDocSession_R1 sess = doc.Sessions[j];
					if (sess.SessionID > result)
						result = sess.SessionID;
				}
			}

			for (int i = 0; i < m_Sessions.Count; i++)
			{
				IWDESession_R1 sess = (IWDESession_R1)m_Sessions[i];
				if (sess.SessionID > result)
					result = sess.SessionID;
			}

			return result;
		}

		private void LoadFromDocData()
		{
			if (m_DataSet == null)
				throw new WDEException("API90004");

			if (m_DataSet.Documents.Count > 0)
			{
				foreach (IWDEDocSession_R1 docsess in m_DataSet.Documents[0].Sessions)
				{
					IWDESession_R1 sess = WDESession.Create(this);
					IWDESessionInternal isess = (IWDESessionInternal)sess;
					isess.User = docsess.User;
					isess.Task = docsess.Task;
					isess.StartTime = ((IWDEDocSessionInternal)docsess).StartTime;
					isess.SessionID = docsess.SessionID;
					isess.Mode = docsess.Mode;
					isess.Location = docsess.Location;
					m_Sessions.Add(sess);
				}

				IWDEDocSessions lastSessions = m_DataSet.Documents[m_DataSet.Documents.Count - 1].Sessions;
				foreach (IWDESession_R1 sess in m_Sessions)
				{
					int index = lastSessions.FindByID(sess.SessionID);
					if (index != -1)
					{
						IWDESessionInternal isess = (IWDESessionInternal)sess;
						isess.EndTime = ((IWDEDocSessionInternal)lastSessions[index]).EndTime;
					}
				}
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			IEnumerable ienum = (IEnumerable)m_Sessions;
			return ienum.GetEnumerator();
		}

		#endregion
	}

	public class WDESession : IWDESession_R1, IWDESessionInternal, IWDEXmlPersist
	{
		private IWDESessions m_Sessions;
		private string m_User;
		private string m_Task;
		private WDEOpenMode m_Mode;
		private DateTime m_StartTime;
		private DateTime m_EndTime;
		private string m_Location;
		private int m_SessionID;
		private bool m_IsCreateSession;

		private WDESession(IWDESessions Owner)
		{
			m_Sessions = Owner;
			m_User = "";
			m_Task = "";
			m_Location = "";
		}

		public static IWDESession_R1 Create(IWDESessions Owner)
		{
			return new WDESession(Owner) as IWDESession_R1;
		}

		public static IWDESession_R1 CreateInstance(IWDESessions Owner)
		{
			return Create(Owner);
		}

		#region IWDESession_R1 Members
		public string User
		{
			get
			{
				return m_User;
			}
			set
			{
				if (value == null)
					m_User = "";
				else
					m_User = value;
			}
		}

		public string Task
		{
			get
			{
				return m_Task;
			}
			set
			{
				if (value == null)
					m_Task = "";
				else
					m_Task = value;
			}
		}

		public WebDX.Api.WDEOpenMode Mode
		{
			get
			{
				return m_Mode;
			}
			set
			{
				m_Mode = value;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return m_StartTime;
			}
			set
			{
				m_StartTime = value;
				UpdateDocStartTimes();
			}
		}

		public DateTime EndTime
		{
			get
			{
				return m_EndTime;
			}
			set
			{
				m_EndTime = value;
				UpdateDocEndTimes();
			}
		}

		public int SessionID
		{
			get
			{
				return m_SessionID;
			}
			set
			{
				m_SessionID = value;
			}
		}
	   
		public string Location
		{
			get
			{
				return m_Location;
			}
			set
			{
				if (value == null)
					m_Location = "";
				else
					m_Location = value;
			}
		}

		public int CharCount
		{
			get
			{
				int result = 0;
				for (int i = 0; i < m_Sessions.DataSet.Documents.Count; i++)
				{
					int index = m_Sessions.DataSet.Documents[i].Sessions.FindByID(SessionID);
					if (index != -1)
					{
						IWDEDocSession_R1 docsess = m_Sessions.DataSet.Documents[i].Sessions[index];
						result += docsess.CharCount;
					}
					else
						throw new WDEException("API00009", new object[] { SessionID, i });
				}

				return result;
			}
		}

		public int VCECount
		{
			get
			{
				int result = 0;
				for (int i = 0; i < m_Sessions.DataSet.Documents.Count; i++)
				{
					int index = m_Sessions.DataSet.Documents[i].Sessions.FindByID(SessionID);
					if (index != -1)
					{
						IWDEDocSession_R1 docsess = m_Sessions.DataSet.Documents[i].Sessions[index];
						result += docsess.VCECount;
					}
					else
						throw new WDEException("API00009", new object[] { SessionID, i });
				}

				return result;
			}
		}

		public void SaveSnapshot(string fileName)
		{
			FileStream fst = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
			try
			{
				SaveSnapshot(fst);
			}
			finally
			{
				fst.Close();
			}
		}

		public void SaveSnapshot(System.IO.Stream aStream)
		{
			GC.Collect();
			MemoryStream memStream = new MemoryStream();
			XmlTextWriter xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
			try
			{
				int index = 0;
				xmlWriter.Formatting = Formatting.Indented;
				xmlWriter.WriteStartDocument(true);
				xmlWriter.WriteStartElement("DataSet");
				for (int i = 0; i < m_Sessions.DataSet.Documents.Count; i++)
				{
					index = m_Sessions.DataSet.Documents[i].Sessions.FindByID(SessionID);
					if (index != -1)
					{
						IWDEDocSessionInternal docsess = (IWDEDocSessionInternal)m_Sessions.DataSet.Documents[i].Sessions[index];
						docsess.SaveSnapshot(xmlWriter);
					}
				}

				index = m_Sessions.FindByID(SessionID);
				for (int i = index; i < m_Sessions.Count; i++)
				{

					IWDEXmlPersist ipers = (IWDEXmlPersist)m_Sessions[i];
					ipers.WriteToXml(xmlWriter);
				}

				xmlWriter.WriteEndElement();
			}
			finally
			{
				xmlWriter.Close();
				xmlWriter = null;
			}

			MemoryStream saveStream = new MemoryStream(memStream.ToArray());
			try
			{
				Utils.CopyStream(saveStream, aStream);
			}
			finally
			{
				saveStream.Close();
				saveStream = null;
				memStream = null;
				GC.Collect();
			}
		}

		public void SaveSnapshot(System.Runtime.InteropServices.ComTypes.IStream aStream)
		{
			MemoryStream mst = new MemoryStream();
			try
			{
				SaveSnapshot(mst);
				mst.Seek(0, SeekOrigin.Begin);
				Utils.StreamToUCOMIStream(mst, aStream);
			}
			finally
			{
				mst.Close();
			}
		}

		public byte[] SaveSnapshot()
		{
			MemoryStream mst = new MemoryStream();
			try
			{
				SaveSnapshot(mst);
				return mst.ToArray();
			}
			finally
			{
				mst.Close();
			}
		}

		#endregion

		#region IWDESessionInternal Members

		public bool IsCreateSession
		{
			get
			{
				return m_IsCreateSession;
			}
			set
			{
				m_IsCreateSession = value;
			}
		}

		public void SetStartTimeExplicit(DateTime aTime)
		{
			//Bypasses updating document start times.
			m_StartTime = aTime;
		}

		public void SetEndTimeExplicit(DateTime aTime)
		{
			//Bypasses updating document end times.
			m_EndTime = aTime;
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			//Read session data from pre-2.0 data files. 2.0 and above does not store this information here.
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "Session"))
			{
				m_User = Utils.GetAttribute(XmlReader, "User", "");
				m_Task = Utils.GetAttribute(XmlReader, "Task", "");
				m_Mode = Utils.GetMode(XmlReader, "Mode");
				string temp = Utils.GetAttribute(XmlReader, "StartTime", "");
				try
				{
					m_StartTime = Utils.ISOToDateTime(temp);
				}
				catch
				{
					m_StartTime = Utils.USStrToDateTime(temp);
				}

				temp = Utils.GetAttribute(XmlReader, "EndTime", "");
				try
				{
					m_EndTime = Utils.ISOToDateTime(temp);
				}
				catch
				{
					m_EndTime = Utils.USStrToDateTime(temp);
				}
				m_SessionID = Utils.GetIntValue(XmlReader, "SessionID");
				m_Location = Utils.GetAttribute(XmlReader, "Location", "");
				XmlReader.Read();
				XmlReader.MoveToContent();
				if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Session"))
					XmlReader.ReadEndElement();
			}
			UpdateDocStartTimes();
			UpdateDocEndTimes();
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			/*
			 *Do nothing. The session data is written at the document level in 2.0 and above.
			 */

			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");

			XmlWriter.WriteStartElement("Session");
			XmlWriter.WriteAttributeString("User", m_User);
			XmlWriter.WriteAttributeString("Task", m_Task);

			XmlWriter.WriteAttributeString("Mode", VersionHelper.GetEnumerationString(typeof(WDEOpenMode).Name, m_Mode, VersionInfo.TargetVersionNumber));

			XmlWriter.WriteAttributeString("StartTime", Utils.DateTimeToISO(m_StartTime));

			XmlWriter.WriteAttributeString("EndTime", Utils.DateTimeToISO(m_EndTime));

			XmlWriter.WriteAttributeString("SessionID", m_SessionID.ToString());
			if(m_Location != "")
				XmlWriter.WriteAttributeString("Location", m_Location);
			int temp = CharCount;
			if(temp != 0)
				XmlWriter.WriteAttributeString("CharCount", temp.ToString());
			temp = VCECount;
			if(temp != 0)
				XmlWriter.WriteAttributeString("VCECount", temp.ToString());
			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Members

		private void UpdateDocStartTimes()
		{
			if (m_Sessions.DataSet != null)
			{
				for (int i = 0; i < m_Sessions.DataSet.Documents.Count; i++)
				{
					int index = m_Sessions.DataSet.Documents[i].Sessions.FindByID(SessionID);
					if (index != -1)
					{
						IWDEDocSessionInternal docsess = (IWDEDocSessionInternal)m_Sessions.DataSet.Documents[i].Sessions[index];
						if (docsess.StartTime == DateTime.MinValue)
							docsess.StartTime = m_StartTime;
					}
				}
			}
		}

		private void UpdateDocEndTimes()
		{
			if (m_Sessions.DataSet != null)
			{
				for (int i = 0; i < m_Sessions.DataSet.Documents.Count; i++)
				{
					int index = m_Sessions.DataSet.Documents[i].Sessions.FindByID(SessionID);
					if (index != -1)
					{
						IWDEDocSessionInternal docsess = (IWDEDocSessionInternal)m_Sessions.DataSet.Documents[i].Sessions[index];
						if (docsess.EndTime == DateTime.MinValue)
						{
							docsess.EndTime = m_EndTime;
							docsess.Duration = docsess.EndTime - docsess.StartTime;
						}
					}
				}
			}
		}

		#endregion
	}

	public class WDEFields : IWDEFields, IWDEFieldsInternal, IWDEXmlPersist, IWDECollection, IEnumerable
	{
		private IWDERecord m_Record;
		private ArrayList m_Fields;

		private WDEFields(IWDERecord Owner)
		{
			m_Record = Owner;
			m_Fields = new ArrayList();
		}

		public static IWDEFields Create(IWDERecord Owner)
		{
			return new WDEFields(Owner) as IWDEFields;
		}

		public static IWDEFields CreateInstance(IWDERecord Owner)
		{
			return Create(Owner);
		}

		#region IWDEFields Members

		public int Count
		{
			get
			{
				return m_Fields.Count;
			}
		}

		public IWDEField_R1 this[int Index]
		{
			get
			{
				return (IWDEField_R1)m_Fields[Index];
			}
		}

		public int Find(string FieldName)
		{
			if (FieldName == null)
				throw new ArgumentNullException("FieldName", "FieldName cannot be null");				

			for (int i = 0; i < Count; i++)
			{
				if (string.Compare(this[i].FieldName, FieldName, true) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		public IWDERecord DataRecord
		{
			get
			{
				return m_Record;
			}
		}

		#endregion

		#region IWDEFieldsInternal Members

		public IWDEField_R1 Add(IWDEFieldDef FieldDef)
		{
			if (FieldDef == null)
				throw new ArgumentNullException("FieldDef", "FieldDef cannot be null");				

			IWDEField_R1 result = WDEField.Create(this);
			IWDEFieldInternal ifield = (IWDEFieldInternal)result;
			ifield.FieldDef = FieldDef;
			string NewValue = null;
			if (FieldDef.DefaultValue != "")
			{
				NewValue = FieldDef.DefaultValue;
			}

			IWDERevisionsInternal irev = (IWDERevisionsInternal)result.Revisions;
			irev.Add(NewValue, WDEFieldStatus.None, result.DataSet.Sessions[0].SessionID, 0, 0);
			m_Fields.Add(result);
			return result;
		}

		public void Clear()
		{
			m_Fields.Clear();
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if (XmlReader == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				

			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "Field"))
			{
				m_Fields.Clear();
				while ((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "Field"))
				{
					IWDEField_R1 field = WDEField.Create(this);
					IWDEXmlPersist ipers = (IWDEXmlPersist)field;
					ipers.ReadFromXml(XmlReader);
					m_Fields.Add(field);
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if (XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				

			for (int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist)this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion

		#region IWDECollection Members

		public int GetIndex(object Item)
		{
			return m_Fields.IndexOf(Item);
		}

		public void SetIndex(object Item, int NewIndex)
		{
			if (Item == null)
				throw new ArgumentNullException("Item", "Item cannot be null");				

			int index = GetIndex(Item);
			if (index != -1)
			{
				m_Fields.RemoveAt(index);
				m_Fields.Insert(NewIndex, Item);
			}
			else
				throw new WDEException("API90002", new object[] { "IWDEFields" });
		}

		public void RemoveAt(int Index)
		{
			m_Fields.RemoveAt(Index);
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			IEnumerable ienum = (IEnumerable)m_Fields;
			return ienum.GetEnumerator();
		}

		#endregion
	}

	public class WDEField : IWDEField_R1, IWDEFieldClient_R1, IWDEFieldInternal, IWDEXmlPersist
	{
		private IWDEFields m_Fields;
		private IWDEFieldDef m_FieldDef;
		private bool m_Exclude;
		private WDEFieldFlags m_Flags;
		private WDEMiscFlags m_MiscFlags;
		private string m_ImageName;
		private Rectangle m_ImageRect;
		private Rectangle m_OCRAreaRect;
		private Rectangle m_OCRRect;
		private bool m_QIFocusAudit;
		private int m_Tag;
		private string m_FieldName;
		private IWDERevisions m_Revisions;
		private string m_CustomData;

		private WDEField(IWDEFields Owner)
		{
			m_Fields = Owner;
			m_Revisions = WDERevisions.Create(this);
			m_ImageName = "";
			m_FieldName = "";
			m_CustomData = "";
		}

		public static IWDEField_R1 Create(IWDEFields Owner)
		{
			return new WDEField(Owner) as IWDEField_R1;
		}

		public static IWDEField_R1 CreateInstance(IWDEFields Owner)
		{
			return Create(Owner);
		}

		#region IWDEField_R1 Members

		public bool AsBoolean
		{
			get
			{
				return Convert.ToBoolean(Value);
			}
			set
			{
				Value = value.ToString();
			}
		}

		public decimal AsCurrency
		{
			get
			{
				return Convert.ToDecimal(Value);
			}
			set
			{
				Value = value.ToString();
			}
		}

		public DateTime AsDateTime
		{
			get
			{
				return Convert.ToDateTime(Value);
			}
			set
			{
				Value = value.ToString();
			}
		}

		public double AsFloat
		{
			get
			{
				return Convert.ToDouble(Value);
			}
			set
			{
				Value = value.ToString();
			}
		}

		public int AsInteger
		{
			get
			{
				return Convert.ToInt32(Value);
			}
			set
			{
				Value = value.ToString();
			}
		}

		public string AsString
		{
			get
			{
				if (IsNull)
					return "";
				else
					return Value;
			}
			set
			{
				Value = value;
			}
		}

		public object AsVariant
		{
			get
			{
				return Value;
			}
			set
			{
				Value = Convert.ToString(value);
			}
		}

		public int DataLen
		{
			get
			{
				return FieldDef.DataLen;
			}
		}

		public IWDERecord DataRecord
		{
			get
			{
				return m_Fields.DataRecord;
			}
		}

		public IWDEDataSet DataSet
		{
			get
			{
				return m_Fields.DataRecord.Document.DataSet;
			}
		}

		[VersionPropertyFilter("1.0.0.0", "1.3.4.3", "DataType", "IWDEField")]
		public WebDX.Api.WDEDataType DataType
		{
			get
			{
				return FieldDef.DataType;
			}
		}

		[VersionPropertyFilter("1.2.0.0", null,"Exclude","IWDEField")]
		public bool Exclude
		{
			get
			{
				return m_Exclude;
			}
			set
			{
				m_Exclude = value;
			}
		}

		public IWDEFieldDef FieldDef
		{
			get
			{
				if (m_FieldDef == null)
				{
					if (DataSet.Project == null)
							throw new WDEException("API00001");
					else
					{
						int index = DataRecord.RecordDef.FieldDefs.Find(FieldName);
						if (index == -1)
								throw new WDEException("API00002",
									new object[] { DataRecord.Document.DocType, DataRecord.RecType, FieldName });

						else
							m_FieldDef = DataRecord.RecordDef.FieldDefs[index];
					}
				}
				return m_FieldDef;
			}

			set
			{
				m_FieldDef = value;
				if (value != null)
					m_FieldName = m_FieldDef.FieldName;
			}
		}

		public string FieldName
		{
			get
			{
				return m_FieldName;
			}
		}

		public bool Flagged
		{
			get
			{
				if (Revisions.Count > 0)
					return Revisions[0].Flagged;
				else
					return false;
			}
			set
			{
				if (Flagged != value)
				{
					if (value)
						SetValueAndStatus(Value, WDEFieldStatus.Flagged, 0);
					else
						SetValueAndStatus(Value, WDEFieldStatus.None, 0);
				}
			}
		}

		[VersionPropertyFilter("1.0.0.0", "1.3.4.3", "Flags", "IWDEField")]
		public WebDX.Api.WDEFieldFlags Flags
		{
			get
			{
				return m_Flags;
			}
			set
			{
				m_Flags = value;
			}
		}

		[VersionPropertyFilter("1.0.0.0", "1.3.4.3", "MiscFlags", "IWDEField")]
		public WebDX.Api.WDEMiscFlags MiscFlags
		{
			get
			{
				return m_MiscFlags;
			}
			set
			{
				m_MiscFlags = value;
			}
		}

		public string FlagDescription
		{
			get
			{
				if (Revisions.Count > 0)
					return Revisions[0].FlagDescription;
				else
					return "";
			}
			set
			{
				if ((value == "") || (value == null))
					SetValueAndStatus(Value, WDEFieldStatus.None, 0);
				else
					SetValueAndStatus(Value, WDEFieldStatus.Flagged, 0);
				IWDERevisionInternal irev = (IWDERevisionInternal)Revisions[0];
				irev.FlagDescription = value;
			}
		}

		public string ImageName
		{
			get
			{
				return m_ImageName;
			}
			set
			{
				if (value == null)
					m_ImageName = "";
				else
					m_ImageName = value;
			}
		}

		public Rectangle ImageRect
		{
			get
			{
				return m_ImageRect;
			}
			set
			{
				m_ImageRect = value;
			}
		}

		public bool IsNull
		{
			get
			{
				if (Value == null)
					return true;
				else
					return false;
			}
			set
			{
				if (value)
					SetValueAndStatus(null, WDEFieldStatus.Plugged, 0);
				else
					throw new WDEException("API00007", new object[] { FieldName });
			}
		}

		public int Index
		{
			get
			{
				IWDECollection icoll = (IWDECollection)m_Fields;
				return icoll.GetIndex((IWDEField_R1)this);
			}
			set
			{
				IWDECollection icoll = (IWDECollection)m_Fields;
				icoll.SetIndex((IWDEField_R1)this, value);
			}
		}

		[VersionPropertyFilter("1.2.0.0", null, "OCRAreaRect", "IWDEField")]
		public Rectangle OCRAreaRect
		{
			get
			{
				return m_OCRAreaRect;
			}
			set
			{
				m_OCRAreaRect = value;
			}
		}

		public Rectangle OCRRect
		{
			get
			{
				return m_OCRRect;
			}
			set
			{
				m_OCRRect = value;
			}
		}

		[VersionPropertyFilter("1.2.0.0", null, "QIFocusAudit", "IWDEField")]
		public bool QIFocusAudit
		{
			get
			{
				return m_QIFocusAudit;
			}
			set
			{
				m_QIFocusAudit = value;
			}
		}

		public IWDERevisions Revisions
		{
			get
			{
				return m_Revisions;
			}
		}

		public int SessionID
		{
			get
			{
				return DataSet.Sessions[0].SessionID;
			}
		}

		public WebDX.Api.WDEFieldStatus Status
		{
			get
			{
				if ((Revisions.Count > 0) && (Revisions[0].SessionID == SessionID))
					return Revisions[0].Status;
				else
					return WDEFieldStatus.None;
			}
			set
			{
				if ((value != WDEFieldStatus.None) &&
					(value != WDEFieldStatus.Plugged) &&
					(value != WDEFieldStatus.Validated) &&
					(value != WDEFieldStatus.Flagged))
						throw new WDEException("API00003", new object[] { value, FieldName });
	
				SetValueAndStatus(Value, value, 0);
			}
		}

		public string Value
		{
			get
			{
				if (Revisions.Count > 0)
					return Revisions[0].Value;
				else
					return null;
			}
			set
			{
				SetValueAndStatus(value, WDEFieldStatus.Plugged, 0);
			}
		}

		[VersionPropertyFilter("1.4.0.0", null, "Tag", "IWDEField")]
		public int Tag
		{
			get
			{
				return m_Tag;
			}
			set
			{
				m_Tag = value;
			}
		}

		[VersionPropertyFilter("1.4.0.0", null, "CustomData", "IWDEField")]
		public string CustomData
		{
			get
			{
				return m_CustomData;
			}
			set
			{
				string newValue = value;
				if (value == null)
					newValue = "";

				if (newValue.Length > 255)
					m_CustomData = newValue.Substring(0, 255);
				else
					m_CustomData = newValue;
			}
		}

		public bool IsNumeric
		{
			get
			{
				if (!IsNull)
				{
					string val = Value;
					for (int i = 0; i < val.Length; i++)
						if (!char.IsNumber(val, i))
							return false;
					return true;
				}
				else
					return false;
			}
		}

		[VersionPropertyFilter("1.0.0.0", "1.3.4.2", "CharRepairs", "IWDEField")]
		public IWDECharRepairs CharRepairs
		{
			get
			{
				if (Revisions.Count > 0)
					return Revisions[0].CharRepairs;
				else
					throw new WDEException("API90001", new object[] { FieldName });				
			}
		}

		public string ExtractGoodChars(string NewValue)
		{
			if ((NewValue != "") && (NewValue != null))
			{
				string charSet = "";
				if (DataSet.Sessions[0].Mode == WDEOpenMode.CharRepair)
					charSet = FieldDef.OCRCharSet;
				else
					charSet = FieldDef.CharSet;

				if (charSet != "")
				{
					WDECharSet cs = new WDECharSet(charSet);
					return cs.Filter(NewValue);
				}
				else
					return NewValue;
			}
			else
			{
				if (NewValue == null)
					return null;
				else
					return "";
			}
		}

		public void SetValueAndStatus(string NewValue, WebDX.Api.WDEFieldStatus NewStatus, int CharCount)
		{
			SetValueAndStatus(NewValue, NewStatus, CharCount, 0);
		}

		public void SetValueAndStatus(string NewValue, WebDX.Api.WDEFieldStatus NewStatus, int CharCount, int VCECount)
		{
			if (NewValue != null)
			{
				if ((DataSet.Sessions[0].Mode != WDEOpenMode.CharRepair) && (NewValue.Length > FieldDef.DataLen))
					NewValue = NewValue.Substring(0, FieldDef.DataLen);

				if (Utils.CheckEnumFlag((int)FieldDef.Options, (int)WDEFieldOption.UpperCase))
					NewValue = NewValue.ToUpper();

				//Moved these checks to the TextBox level in 2.0: CanChange(NewValue);
			}

			bool CreateRevision = false;
			if (Value != NewValue)
			{
				CreateRevision = true;
			}
			else if ((Revisions.Count == 0) || (DataSet.Sessions[0].SessionID != Revisions[0].SessionID) || (Status != NewStatus) || NewValue == string.Empty)
				CreateRevision = true;

			if (CreateRevision)
			{
				IWDERevisionsInternal irevs = (IWDERevisionsInternal)Revisions;
				irevs.Add(NewValue, NewStatus, DataSet.Sessions[0].SessionID, CharCount, VCECount);                
			}
		}

        public void SetValueAndStatus(string NewValue, WebDX.Api.WDEFieldStatus NewStatus, int CharCount, string FormName)
        {
            SetValueAndStatus(NewValue, NewStatus, CharCount, FormName, 0);
        }

        public void SetValueAndStatus(string NewValue, WebDX.Api.WDEFieldStatus NewStatus, int CharCount, string FormName, int VCECount)
        {
            if (NewValue != null)
            {
                if ((DataSet.Sessions[0].Mode != WDEOpenMode.CharRepair) && (NewValue.Length > FieldDef.DataLen))
                    NewValue = NewValue.Substring(0, FieldDef.DataLen);

                if (Utils.CheckEnumFlag((int)FieldDef.Options, (int)WDEFieldOption.UpperCase))
                    NewValue = NewValue.ToUpper();

                //Moved these checks to the TextBox level in 2.0: CanChange(NewValue);
            }

            bool CreateRevision = false;
            if (Value != NewValue)
            {
                CreateRevision = true;
            }
            else if ((Revisions.Count == 0) || (DataSet.Sessions[0].SessionID != Revisions[0].SessionID) || (Status != NewStatus) || NewValue == string.Empty)
                CreateRevision = true;

            if (CreateRevision)
            {
                IWDERevisionsInternal irevs = (IWDERevisionsInternal)Revisions;
                irevs.Add(NewValue, NewStatus, DataSet.Sessions[0].SessionID, CharCount, FormName, VCECount);
            }
        }

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "Field"))
			{
				m_FieldName = Utils.GetAttribute(XmlReader, "Name", "");
				m_Flags = Utils.GetFieldFlags(XmlReader, "Flags");
				m_MiscFlags = Utils.GetMiscFlags(XmlReader, "MiscFlags");
				ImageName = Utils.GetAttribute(XmlReader, "ImageName", "");
				ImageRect = Utils.GetRectValue(XmlReader, "ImageRect");
				OCRAreaRect = Utils.GetRectValue(XmlReader, "OCRAreaRect");
				OCRRect = Utils.GetRectValue(XmlReader, "OCRRect");
				QIFocusAudit = Utils.GetBoolValue(XmlReader, "QIFocusAudit", false);
				Exclude = Utils.GetBoolValue(XmlReader, "Exclude", false);
				m_Tag = Utils.GetIntValue(XmlReader, "Tag");
				m_CustomData = Utils.GetAttribute(XmlReader, "CustomData", "");

				string s = Utils.GetAttribute(XmlReader, "DataOwner", "");
				IWDERevision tempRev = null;
				if (s != "")
				{
					string val;
					if (Utils.GetBoolValue(XmlReader, "IsNull", false))
						val = null;
					else
						val = Utils.GetAttribute(XmlReader, "Value", "");

					WDEFieldStatus status = Utils.GetFieldStatus(XmlReader, "Status");
					int session = Utils.GetIntValue(XmlReader, "SessionID");

					int cc = Utils.GetIntValue(XmlReader, "CharCount");
					int vce = Utils.GetIntValue(XmlReader, "VCECount");

					Utils.SetStatusByDataOwner(s, ref status);

					if (Utils.GetBoolValue(XmlReader, "Flagged", false))
						status = WDEFieldStatus.Flagged;

					IWDERevisionsInternal irev = (IWDERevisionsInternal)Revisions;
					tempRev = irev.Add(val, status, session, cc, vce);
				}

				XmlReader.Read();
				XmlReader.MoveToContent();

				IWDEXmlPersist ipers = (IWDEXmlPersist)Revisions;
				ipers.ReadFromXml(XmlReader);

				if (tempRev != null)
				{
					IWDERevisionsInternal irev = (IWDERevisionsInternal)Revisions;
					irev.Insert(0, tempRev);
				}

				StringCollection repairs = new StringCollection();
				while ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "CharRepair"))
				{
					repairs.Add(XmlReader.ReadOuterXml());
					XmlReader.MoveToContent();
				}

				if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Field"))
					XmlReader.ReadEndElement();

				if (repairs.Count > 0)
				{
					ipers = (IWDEXmlPersist)Revisions[0].CharRepairs;
					StringBuilder sb = new StringBuilder();
					sb.Append("<Field>");
					for (int i = 0; i < repairs.Count; i++)
						sb.Append(repairs[i] + Environment.NewLine);
					sb.Append("</Field>");

					StringReader sr = new StringReader(sb.ToString());
					XmlTextReader charReader = new XmlTextReader(sr);
					charReader.MoveToContent();
					charReader.Read();
					charReader.MoveToContent();
					ipers.ReadFromXml(charReader);
					charReader.Close();
					sr.Close();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if (XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				
			
			#region StartElement
			XmlWriter.WriteStartElement("Field");
			#endregion

			#region Propierties
			/*                                         
			 * Name         - *Common                  
			 * DataType     - Changed               
			 * Flags        - Changed                   
			 * MiscFlags    - Changed   = with enum         
			 * Exclude      - Changed          
			 * Tag          - Changed                   
			 * ImageName    - *Common
			 * ImageRect    - *Common
			 * OCRRect      - *Common
			 * OCRAreaRect  - Changed
			 * QIFocusAudit - Changed 
			 * CustomData   - Changed
			 */
			#region CommonPropiertes

			XmlWriter.WriteAttributeString("Name", FieldName);
			if (ImageName != "")
				XmlWriter.WriteAttributeString("ImageName", ImageName);
			if (!ImageRect.IsEmpty)
				XmlWriter.WriteAttributeString("ImageRect", Utils.RectToString(ImageRect));
			if (!OCRRect.IsEmpty)
				XmlWriter.WriteAttributeString("OCRRect", Utils.RectToString(OCRRect));

			#endregion


			#region ChangedPropiertes

			if (!VersionHelper.FilterPropertyOrCollection("IWDEField.DataType", VersionInfo.TargetVersionNumber))
			{
				XmlWriter.WriteAttributeString("DataType", "dtText");
			}

			if (Flags != WDEFieldFlags.None)
			{
				if (!VersionHelper.FilterPropertyOrCollection("IWDEField.Flags", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("Flags", VersionHelper.GetEnumerationString(typeof(WDEFieldFlags).Name, Flags, VersionInfo.TargetVersionNumber));
				}
				else
				{
					VersionHelper.LogDataLost("IWDEField.Flags", Flags.ToString());
				}                
			}

			if(MiscFlags != WDEMiscFlags.None )
			{
				if (!VersionHelper.FilterPropertyOrCollection("IWDEField.MiscFlags", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("MiscFlags", VersionHelper.GetEnumerationString(typeof(WDEMiscFlags).Name,MiscFlags, VersionInfo.TargetVersionNumber));
				}
				else
				{
					VersionHelper.LogDataLost("IWDEField.MiscFlags", MiscFlags.ToString());
				} 
			}

			if (Exclude)
			{
				if (!VersionHelper.FilterPropertyOrCollection("IWDEField.Exclude", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("Exclude", Exclude.ToString());
				}
				else
				{
					VersionHelper.LogDataLost("IWDEField.Exclude", Exclude.ToString());
				}
			}

			if(!OCRAreaRect.IsEmpty)
			{
				if (!VersionHelper.FilterPropertyOrCollection("IWDEField.OCRAreaRect", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("OCRAreaRect", Utils.RectToString(OCRAreaRect));
				}
				else
				{
					VersionHelper.LogDataLost("IWDEField.OCRAreaRect", Utils.RectToString(OCRAreaRect));
				}
			}
			

			if (QIFocusAudit)
			{
				if (!VersionHelper.FilterPropertyOrCollection("IWDEField.QIFocusAudit", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("QIFocusAudit", QIFocusAudit.ToString());
				}
				else
				{
					VersionHelper.LogDataLost("IWDEField.QIFocusAudit", QIFocusAudit.ToString());
				}
			}
			if (m_Tag != 0)
			{
				if (!VersionHelper.FilterPropertyOrCollection("IWDEField.Tag", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("Tag", Tag.ToString());
				}
				else
				{
					VersionHelper.LogDataLost("IWDEField.Tag", Tag.ToString());
				}
			}
			if (m_CustomData != "")
			{
				if (!VersionHelper.FilterPropertyOrCollection("IWDEField.CustomData", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("CustomData", m_CustomData);
				}
				else
				{
					VersionHelper.LogDataLost("IWDEField.CustomData", m_CustomData.ToString());
				}
			}
			#endregion
			#endregion
			#region Collections

			IWDEXmlPersist ipers = (IWDEXmlPersist)Revisions;
			ipers.WriteToXml(XmlWriter);

			if (!VersionHelper.FilterPropertyOrCollection("IWDEField.CharRepairs", VersionInfo.TargetVersionNumber))
			{
				ipers = (IWDEXmlPersist)CharRepairs;
				ipers.WriteToXml(XmlWriter);
			}			
			#endregion


			#region EndElement
			XmlWriter.WriteEndElement();
			#endregion
		}
		
		#region Internal Methods

		internal static XmlSchemaElement GetSchema(List<Type> usedEnums)
		{
			XmlSchemaElement result = new XmlSchemaElement();
			result.Name = "Field";
			result.MinOccurs = 1;
			result.MaxOccursString = "unbounded";

			XmlSchemaComplexType complexType = new XmlSchemaComplexType();
			result.SchemaType = complexType;

			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexType.Particle = seq;

			seq.Items.Add(WDERevision.GetSchema(usedEnums));

			XmlSchemaAttribute attr = new XmlSchemaAttribute();
			attr.Name = "Name";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Use = XmlSchemaUse.Required;
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "ImageName";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "ImageRect";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Annotation = new XmlSchemaAnnotation();
			XmlSchemaDocumentation documentation = new XmlSchemaDocumentation();
			attr.Annotation.Items.Add(documentation);
			documentation.Markup = SchemaHelpers.TextToNodeArray("A comma-separated list of integers: left, top, width, height");
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "OCRAreaRect";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Annotation = new XmlSchemaAnnotation();
			documentation = new XmlSchemaDocumentation();
			attr.Annotation.Items.Add(documentation);
			documentation.Markup = SchemaHelpers.TextToNodeArray("A comma-separated list of integers: left, top, width, height");
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "OCRRect";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Annotation = new XmlSchemaAnnotation();
			documentation = new XmlSchemaDocumentation();
			attr.Annotation.Items.Add(documentation);
			documentation.Markup = SchemaHelpers.TextToNodeArray("A comma-separated list of integers: left, top, width, height");
			complexType.Attributes.Add(attr);

			if (!usedEnums.Contains(typeof(WDEFieldFlags)))
				usedEnums.Add(typeof(WDEFieldFlags));

			attr = new XmlSchemaAttribute();
			attr.Name = "Flags";
			attr.SchemaTypeName = new XmlQualifiedName(typeof(WDEFieldFlags).Name, SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

			if (!usedEnums.Contains(typeof(WDEMiscFlags)))
				usedEnums.Add(typeof(WDEMiscFlags));

			attr = new XmlSchemaAttribute();
			attr.Name = "MiscFlags";
			attr.SchemaTypeName = new XmlQualifiedName(typeof(WDEMiscFlags).Name, SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "QIFocusAudit";
			attr.SchemaTypeName = new XmlQualifiedName("WDEBoolean", SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "Exclude";
			attr.SchemaTypeName = new XmlQualifiedName("WDEBoolean", SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "Tag";
			attr.SchemaTypeName = new XmlQualifiedName("int", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "CustomData";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			return result;
		}

		#endregion

		#endregion
	}

	public class WDERevisions : IWDERevisions, IWDERevisionsInternal, IWDEXmlPersist, IWDECollection, IEnumerable
	{
		private ArrayList m_Revisions;
		private IWDEField_R1 m_Field;

		private WDERevisions(IWDEField_R1 Owner)
		{
			m_Field = Owner;
			m_Revisions = new ArrayList();
		}

		public static IWDERevisions Create(IWDEField_R1 Owner)
		{
			return new WDERevisions(Owner) as IWDERevisions;
		}

		public static IWDERevisions CreateInstance(IWDEField_R1 Owner)
		{
			return Create(Owner);
		}

		#region IWDERevisions Members

		public int Count
		{
			get
			{
				return m_Revisions.Count;
			}
		}

		public IWDERevision this[int Index]
		{
			get
			{
				return (IWDERevision)m_Revisions[Index];
			}
		}

		public IWDEField_R1 Field
		{
			get
			{
				return m_Field;
			}
		}

		#endregion

		#region IWDERevisionsInternal Members

		public IWDERevision Add(string Value, WebDX.Api.WDEFieldStatus Status, int SessionID, int CharCount, int VCECount)
		{
			IWDERevision result = WDERevision.Create(this);
			IWDERevisionInternal ires = (IWDERevisionInternal)result;
			ires.Value = Value;
			ires.Status = Status;
			ires.SessionID = SessionID;
			ires.CharCount = CharCount;
			ires.VCECount = VCECount;
			m_Revisions.Insert(0, result);
			return result;
		}

        public IWDERevision Add(string Value, WebDX.Api.WDEFieldStatus Status, int SessionID, int CharCount, string FormName, int VCECount)
        {
            IWDERevision result = WDERevision.Create(this);
            IWDERevisionInternal ires = (IWDERevisionInternal)result;
            ires.Value = Value;
            ires.Status = Status;
            ires.SessionID = SessionID;
            ires.CharCount = CharCount;
            ires.VCECount = VCECount;
            ires.FormName = FormName;            
            m_Revisions.Insert(0, result);
            return result;
        }

		public void Insert(int Index, IWDERevision rev)
		{
			m_Revisions.Insert(0, rev);
		}

		public void Clear()
		{
			m_Revisions.Clear();
		}

		public void Delete()
		{
			m_Revisions.RemoveAt(0);
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "Revision"))
			{
				m_Revisions.Clear();
				while ((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "Revision"))
				{
					IWDERevision rev = WDERevision.Create(this);
					IWDEXmlPersist ipers = (IWDEXmlPersist)rev;
					ipers.ReadFromXml(XmlReader);
					m_Revisions.Add(rev);
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if (XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				

			IWDEDocumentInternal doc = null;
			if ((m_Field != null) && (m_Field.DataRecord != null) &&
				(m_Field.DataRecord.Document != null) && (m_Field.DataRecord.Document is IWDEDocumentInternal))
				doc = (IWDEDocumentInternal)m_Field.DataRecord.Document;
			for (int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist)this[i];
				if ((doc == null) || (doc.SaveSessionID == -1) ||
					((doc != null) && (doc.SaveSessionID != -1) && (this[i].SessionID <= doc.SaveSessionID)))
					ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion

		#region IWDECollection Members

		public int GetIndex(object Item)
		{
			return m_Revisions.IndexOf(Item);
		}

		public void SetIndex(object Item, int NewIndex)
		{
			int index = GetIndex(Item);
			if (index != -1)
			{
				m_Revisions.RemoveAt(index);
				m_Revisions.Insert(NewIndex, Item);
			}
			else
				throw new WDEException("API90002", new object[] { "IWDERevisions" });
		}

		public void RemoveAt(int Index)
		{
			m_Revisions.RemoveAt(Index);
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			IEnumerable ienum = (IEnumerable)m_Revisions;
			return ienum.GetEnumerator();
		}

		#endregion
	}

    public class WDERevision : IWDERevision_R2, IWDERevisionInternal, IWDEXmlPersist
	{
		private IWDERevisions m_Revisions;
		private IWDECharRepairs m_CharRepairs;
		private string m_Value;
		private string m_FlagDescription;
		private WDEFieldStatus m_Status;
		private int m_CharCount;
		private int m_VCECount;
		private int m_SessionID;
		private string m_ErrorCategory;
        private string m_FormName;

		private WDERevision(IWDERevisions Owner)
		{
			m_FlagDescription = "";
			m_Revisions = Owner;
			m_ErrorCategory = "";
			m_CharRepairs = WDECharRepairs.Create(this);
            m_FormName = "";
		}

		public static IWDERevision Create(IWDERevisions Owner)
		{
			return new WDERevision(Owner) as IWDERevision;
		}

		public static IWDERevision CreateInstance(IWDERevisions Owner)
		{
			return Create(Owner);
		}

		#region IWDERevision Members

		public bool AsBoolean
		{
			get
			{
				if (IsNull)
					return false;
				else
					return Convert.ToBoolean(m_Value);
			}
		}

		public decimal AsCurrency
		{
			get
			{
				if (IsNull)
					return 0;
				else
					return Convert.ToDecimal(m_Value);
			}
		}

		public DateTime AsDateTime
		{
			get
			{
				if (IsNull)
					return DateTime.MinValue;
				else
					return Convert.ToDateTime(m_Value);
			}
		}

		public double AsFloat
		{
			get
			{
				if (IsNull)
					return 0;
				else
					return Convert.ToDouble(m_Value);
			}
		}

		public int AsInteger
		{
			get
			{
				if (IsNull)
					return 0;
				else
					return Convert.ToInt32(m_Value);
			}
		}

		public string AsString
		{
			get
			{
				if (IsNull)
					return "";
				else
					return m_Value;
			}
		}

		public object AsVariant
		{
			get
			{
				return m_Value;
			}
		}

		public bool Flagged
		{
			get
			{
				return (m_Status == WDEFieldStatus.Flagged);
			}

			set
			{
				if (value)
					m_Status = WDEFieldStatus.Flagged;
				else
					m_Status = WDEFieldStatus.None;
			}
		}

		[VersionPropertyFilter("1.1.0.0", null, "FlagDescription", "IWDERevision")]
		public string FlagDescription
		{
			get
			{
				return m_FlagDescription;
			}

			set
			{
				if (value == null)
					m_FlagDescription = "";
				else
					m_FlagDescription = value;
			}
		}

		public bool IsNull
		{
			get
			{
				return (m_Value == null);
			}
		}

		public string Value
		{
			get
			{
				return m_Value;
			}

			set
			{
				m_Value = value;
			}
		}

		public WebDX.Api.WDEFieldStatus Status
		{
			get
			{
				return m_Status;
			}

			set
			{
				m_Status = value;
			}
		}

		public int SessionID
		{
			get
			{
				return m_SessionID;
			}

			set
			{
				m_SessionID = value;
			}
		}

		public int CharCount
		{
			get
			{
				return m_CharCount;
			}

			set
			{
				m_CharCount = value;
			}
		}

		public int VCECount
		{
			get
			{
				return m_VCECount;
			}

			set
			{
				m_VCECount = value;
			}
		}

		[VersionPropertyFilter("1.3.4.3", null, "CharRepairs", "IWDERevision")]
		public IWDECharRepairs CharRepairs
		{
			get
			{
				return m_CharRepairs;
			}
		}

		public IWDEField_R1 Field
		{
			get
			{
				return m_Revisions.Field;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "Revision"))
			{
				if (Utils.GetBoolValue(XmlReader, "IsNull", false))
					Value = null;
				else
					Value = Utils.GetAttribute(XmlReader, "Value", "");

				m_Status = Utils.GetFieldStatus(XmlReader, "Status");
				FlagDescription = Utils.GetAttribute(XmlReader, "FlagDescription", "");
				CharCount = Utils.GetIntValue(XmlReader, "CharCount");
				VCECount = Utils.GetIntValue(XmlReader, "VCECount");
				SessionID = Utils.GetIntValue(XmlReader, "SessionID");
				m_ErrorCategory = Utils.GetAttribute(XmlReader, "ErrorCategory", "");
                m_FormName = Utils.GetAttribute(XmlReader, "FormName", "");

				string s = Utils.GetAttribute(XmlReader, "DataOwner", "");
				if (s != "")
				{
					Utils.SetStatusByDataOwner(s, ref m_Status);
					if (Utils.GetBoolValue(XmlReader, "Flagged", false))
						m_Status = WDEFieldStatus.Flagged;
				}

				bool empty = XmlReader.IsEmptyElement;
				XmlReader.Read();
				XmlReader.MoveToContent();

				if (!empty)
				{
					IWDEXmlPersist ipers = (IWDEXmlPersist)CharRepairs;
					ipers.ReadFromXml(XmlReader);
				}

				if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Revision"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if (XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				

			XmlWriter.WriteStartElement("Revision");
			if (IsNull)
				XmlWriter.WriteAttributeString("IsNull", bool.TrueString);
			else
				XmlWriter.WriteAttributeString("Value", this.Value);

			if (Status != WDEFieldStatus.None)
			{
				XmlWriter.WriteAttributeString("Status", VersionHelper.GetEnumerationString(typeof(WDEFieldStatus).Name, Status, VersionInfo.TargetVersionNumber));
			}
						

			if (FlagDescription != "")
			{
				if (!VersionHelper.FilterPropertyOrCollection("IWDERevision.FlagDescription", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("FlagDescription", FlagDescription);
				}
				else
				{
					VersionHelper.LogDataLost("IWDERevision.FlagDescription", FlagDescription.ToString());
				}
			}

			XmlWriter.WriteAttributeString("SessionID", SessionID.ToString());
			if (CharCount != 0)
				XmlWriter.WriteAttributeString("CharCount", CharCount.ToString());
			if (VCECount != 0)
				XmlWriter.WriteAttributeString("VCECount", VCECount.ToString());
			if (ErrorCategory != "")
				XmlWriter.WriteAttributeString("ErrorCategory", ErrorCategory);
            if (FormName != "")
                XmlWriter.WriteAttributeString("FormName", FormName);

			if (!VersionHelper.FilterPropertyOrCollection("IWDERevision.CharRepairs", VersionInfo.TargetVersionNumber))
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist)CharRepairs;
				ipers.WriteToXml(XmlWriter);
			}


			XmlWriter.WriteEndElement();
		}

		#endregion

		#region IWDERevision_R1 Members

		public string ErrorCategory
		{
			get
			{
				return m_ErrorCategory;
			}
			set
			{
				m_ErrorCategory = value == null ? "" : value;
			}
		}

		public IWDEDocSession Session
		{
			get
			{
				int index = this.Field.DataRecord.Document.Sessions.FindByID(this.SessionID);
				if (index > -1)
				{
					return this.Field.DataRecord.Document.Sessions[index];
				}
				else
					throw new WDEException("API00009", new object[] { this.SessionID, this.Field.DataRecord.Document.Index });
			}
		}

		#endregion

        #region IWDERevision_R2 Members

        public string FormName
        {
            get
            {
                return m_FormName;
            }
            set
            {
                m_FormName = value == null ? "" : value;
            }
        }

        #endregion

        #region Internal Methods

        internal static XmlSchemaElement GetSchema(List<Type> usedEnums)
		{
			XmlSchemaElement result = new XmlSchemaElement();
			result.Name = "Revision";
			result.MinOccurs = 1;
			result.MaxOccursString = "unbounded";

			XmlSchemaComplexType complexType = new XmlSchemaComplexType();
			result.SchemaType = complexType;

			XmlSchemaSequence seq = new XmlSchemaSequence();
			complexType.Particle = seq;

			seq.Items.Add(WDECharRepair.GetSchema(usedEnums));

			XmlSchemaAttribute attr = new XmlSchemaAttribute();
			attr.Name = "IsNull";
			attr.SchemaTypeName = new XmlQualifiedName("WDEBoolean", SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "Value";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			if (!usedEnums.Contains(typeof(WDEFieldStatus)))
				usedEnums.Add(typeof(WDEFieldStatus));

			attr = new XmlSchemaAttribute();
			attr.Name = "Status";
			attr.SchemaTypeName = new XmlQualifiedName(typeof(WDEFieldStatus).Name, SchemaHelpers.TARGET_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "FlagDescription";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "SessionID";
			attr.SchemaTypeName = new XmlQualifiedName("unsignedInt", SchemaHelpers.XSD_NAMESPACE);
			attr.Use = XmlSchemaUse.Required;
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "CharCount";
			attr.SchemaTypeName = new XmlQualifiedName("unsignedInt", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "VCECount";
			attr.SchemaTypeName = new XmlQualifiedName("unsignedInt", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "ErrorCategory";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			return result;
		}

		#endregion
	}

	public class WDECharRepairs : IWDECharRepairs, IWDEXmlPersist, IEnumerable
	{
		private IWDERevision m_Revision;
		private ArrayList m_Repairs;

		private WDECharRepairs(IWDERevision Owner)
		{
			m_Revision = Owner;
			m_Repairs = new ArrayList();
		}

		public static IWDECharRepairs Create(IWDERevision Owner)
		{
			return new WDECharRepairs(Owner) as IWDECharRepairs;
		}

		public static IWDECharRepairs CreateInstance(IWDERevision Owner)
		{
			return Create(Owner);
		}

		#region IWDECharRepairs Members

		public int Count
		{
			get
			{
				return m_Repairs.Count;
			}
		}

		public IWDECharRepair this[int Index]
		{
			get
			{
				return (IWDECharRepair)m_Repairs[Index];
			}
		}

		public IWDERevision Revision
		{
			get
			{
				return m_Revision;
			}
		}

		public IWDECharRepair Add(char Value, int Confidence, int CharPos, Rectangle OCRRect)
		{
			IWDECharRepair result = WDECharRepair.Create(this);
			result.Value = Value;
			result.Confidence = Confidence;
			result.CharPos = CharPos;
			result.OCRRect = OCRRect;
			m_Repairs.Add(result);
			return result;
		}

		public void Clear()
		{
			m_Repairs.Clear();
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "CharRepair"))
			{
				m_Repairs.Clear();
				while ((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "CharRepair"))
				{
					IWDECharRepair repair = WDECharRepair.Create(this);
					IWDEXmlPersist ipers = (IWDEXmlPersist)repair;
					ipers.ReadFromXml(XmlReader);
					m_Repairs.Add(repair);
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if (XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				

			for (int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist)this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			IEnumerable ienum = (IEnumerable)m_Repairs;
			return ienum.GetEnumerator();
		}

		#endregion
	}

	public class WDECharRepair : IWDECharRepair, IWDEXmlPersist
	{
		private IWDECharRepairs m_Repairs;
		private int m_Confidence;
		private int m_CharPos;
		private char m_Value;
		private Rectangle m_OCRRect;


		private WDECharRepair(IWDECharRepairs Owner)
		{
			m_Repairs = Owner;
		}

		public static IWDECharRepair Create(IWDECharRepairs Owner)
		{
			return new WDECharRepair(Owner) as IWDECharRepair;
		}

		public static IWDECharRepair CreateInstance(IWDECharRepairs Owner)
		{
			return Create(Owner);
		}

		#region IWDECharRepair Members

		public IWDEField_R1 Field
		{
			get
			{
				return Revision.Field;
			}
		}

		public IWDERevision Revision
		{
			get
			{
				return m_Repairs.Revision;
			}
		}

		[VersionPropertyFilter("1.4.0.0", null, "Confidence", "IWDECharRepair")]
		public int Confidence
		{
			get
			{
				return m_Confidence;
			}
			set
			{
				m_Confidence = value;
			}
		}

		public int CharPos
		{
			get
			{
				return m_CharPos;
			}
			set
			{
				m_CharPos = value;
			}
		}

		public char Value
		{
			get
			{
				return m_Value;
			}
			set
			{
				m_Value = value;
			}
		}

		public Rectangle OCRRect
		{
			get
			{
				return m_OCRRect;
			}
			set
			{
				m_OCRRect = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if (XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");				

			XmlReader.MoveToContent();
			if ((XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "CharRepair"))
			{
				Value = Utils.GetCharValue(XmlReader, "Value", '\0');
				CharPos = Utils.GetIntValue(XmlReader, "CharPos");
				if (!VersionHelper.FilterPropertyOrCollection("IWDECharRepair.Confidence", VersionInfo.TargetVersionNumber))
				{
					Confidence = Utils.GetIntValue(XmlReader, "Confidence");
				}
				else if(!VersionHelper.FilterPropertyOrCollection("IWDECharRepair.Conf", VersionInfo.TargetVersionNumber))
				{
					Confidence = Utils.GetIntValue(XmlReader, "Conf");
				}
				if (Confidence == 0)
					Confidence = Utils.GetIntValue(XmlReader, "Conf");
				OCRRect = Utils.GetRectValue(XmlReader, "OCRRect");
				XmlReader.Read();
				XmlReader.MoveToContent();
				if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "CharRepair"))
					XmlReader.ReadEndElement();
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if (XmlWriter == null)
				throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");				

			XmlWriter.WriteStartElement("CharRepair");
			if (Value != '\0')
				XmlWriter.WriteAttributeString("Value", Value.ToString());
			if (CharPos != 0)
				XmlWriter.WriteAttributeString("CharPos", CharPos.ToString());
			if (Confidence != 0)
			{
				if (!VersionHelper.FilterPropertyOrCollection("IWDECharRepair.Confidence", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("Confidence", Confidence.ToString());
				}else if (!VersionHelper.FilterPropertyOrCollection("IWDECharRepair.Conf", VersionInfo.TargetVersionNumber))
				{
					XmlWriter.WriteAttributeString("Conf", Confidence.ToString());
				}
			}
			if (!OCRRect.IsEmpty)
				XmlWriter.WriteAttributeString("OCRRect", Utils.RectToString(OCRRect));
			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Internal Methods

		internal static XmlSchemaElement GetSchema(List<Type> usedEnums)
		{
			XmlSchemaElement result = new XmlSchemaElement();
			result.Name = "CharRepair";
			result.MinOccurs = 0;
			result.MaxOccursString = "unbounded";

			XmlSchemaComplexType complexType = new XmlSchemaComplexType();
			result.SchemaType = complexType;

			XmlSchemaAttribute attr = new XmlSchemaAttribute();
			attr.Name = "Value";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "CharPos";
			attr.SchemaTypeName = new XmlQualifiedName("unsignedInt", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "Confidence";
			attr.SchemaTypeName = new XmlQualifiedName("unsignedInt", SchemaHelpers.XSD_NAMESPACE);
			complexType.Attributes.Add(attr);

			attr = new XmlSchemaAttribute();
			attr.Name = "OCRRect";
			attr.SchemaTypeName = new XmlQualifiedName("string", SchemaHelpers.XSD_NAMESPACE);
			attr.Annotation = new XmlSchemaAnnotation();
			XmlSchemaDocumentation documentation = new XmlSchemaDocumentation();
			attr.Annotation.Items.Add(documentation);
			documentation.Markup = SchemaHelpers.TextToNodeArray("A comma-separated list of integers: left, top, width, height");
			complexType.Attributes.Add(attr);

			return result;
		}

		#endregion
	}
}
