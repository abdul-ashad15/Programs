using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace WebDX.Api
{
	//TODO: Implement "Form On Demand" parsing
	//TODO: Implement "Font On Demand"
    public class WDEProject : MarshalByRefObject, IWDEProject_R5, IWDEProjectPM, IWDEProjectInternal
	{
		private string m_APIVersion;
		private string m_CreatedBy;
		private DateTime m_DateCreated;
		private DateTime m_DateModified;
		private string m_ModifiedBy;
		private string m_Description;
		private IWDEDocumentDefs m_DocumentDefs;
		private WDEProjectOption m_Options;
		private int m_ScrollInterval;
		private WDEScriptLanguage m_ScriptLanguage;
        private WDEKeyMode m_KeyboardModes;
        private string m_Script;
		private IWDESessionDefs m_SessionDefs;
		private IWDEProjectColors m_ProjectColors;
		private IWDEEventScriptDef m_DocRejected;
		private IWDEEventScriptDef m_DocUnRejected;
		private LinkResolver m_LinkResolver;
		private string m_Version;
		private WDEDatabases m_Databases;
        private IWDEEventScriptDef m_OnKeyPress;
        private IWDEEventScriptDef m_OnStartWork;
        private IWDEEventScriptDef m_OnEndWork;
        private IWDEEventScriptDef m_OnPageChange;
        private List<string> m_ExternalAssemblies;
        private List<string> m_References;
        private Color SELECTED_FORE_COLOR = Color.White;
        private Color SELECTED_BACK_COLOR = Color.Black;
        private Color m_SelectedForeColor;
        private Color m_SelectedBackColor;
        private string SELECTED_HINT_FONT = "Microsoft Sans Serif, 8.25pt";
        private string m_HintFont;

		private bool m_ConvertOldFormat;

		private WDEProject()
		{			
			m_DocumentDefs = WDEDocumentDefs.Create(this);
			m_SessionDefs = WDESessionDefs.Create(this);
			m_ProjectColors = WDEProjectColors.Create();
			m_DocRejected = WDEEventScriptDef.Create(this, "DocRejected");
			m_DocUnRejected = WDEEventScriptDef.Create(this, "DocUnRejected");
            m_OnKeyPress = WDEEventScriptDef.Create(this, "OnKeyPress");
            m_OnStartWork = WDEEventScriptDef.Create(this, "OnStartWork");
            m_OnEndWork = WDEEventScriptDef.Create(this, "OnEndWork");
            m_OnPageChange = WDEEventScriptDef.Create(this, "OnPageChange");
			m_LinkResolver = null;
			m_Version = "1.0.0.0";
			m_Databases = (WDEDatabases) WDEDatabases.Create(this);
            m_ExternalAssemblies = new List<string>();
            m_References = new List<string>();

			Clear();
		}

        public override object InitializeLifetimeService()
        {
            return null;
        }

		public static IWDEProject Create()
		{
			return new WDEProject() as IWDEProject;
		}

		public static IWDEProject CreateInstance()
		{
			return Create();
		}

		#region IWDEProject Members

		public string APIVersion
		{
			get
			{
				if(m_APIVersion == "")
					return VersionInfo.VersionNumber;
				else
					return m_APIVersion;
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

		public string CreatedBy
		{
			get
			{
				return m_CreatedBy;
			}
			set
			{
				if(value == null)
					m_CreatedBy = "";
				else
					m_CreatedBy = value;
			}
		}

		public DateTime DateCreated
		{
			get
			{
				return m_DateCreated;
			}
			set
			{
				m_DateCreated = value;
			}
		}

		public DateTime DateModified
		{
			get
			{
				return m_DateModified;
			}
			set
			{
				m_DateModified = value;
			}
		}

		public string ModifiedBy
		{
			get
			{
				return m_ModifiedBy;
			}
			set
			{
				if(value == null)
					m_ModifiedBy = "";
				else
					m_ModifiedBy = value;
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
				if(value == null)
					m_Description = "";
				else
					m_Description = value;
			}
		}

		public IWDEDocumentDefs DocumentDefs
		{
			get
			{
				return m_DocumentDefs;
			}
		}

		public WebDX.Api.WDEProjectOption Options
		{
			get
			{
				return m_Options;
			}
			set
			{
				m_Options = value;
			}
		}

		public int ImageScrollInterval
		{
			get
			{
				return m_ScrollInterval;
			}
			set
			{
				m_ScrollInterval = value;
			}
		}

		public WebDX.Api.WDEScriptLanguage ScriptLanguage
		{
			get
			{
				return m_ScriptLanguage;
			}
			set
			{
				m_ScriptLanguage = value;
			}
		}

		public string Script
		{
			get
			{
				return m_Script;
			}
			set
			{
				m_Script = value;
			}
		}

		public IWDESessionDefs SessionDefs
		{
			get
			{
				return m_SessionDefs;
			}
		}

		public IWDEProjectColors ProjectColors
		{
			get
			{
				return m_ProjectColors;
			}
		}

		public IWDEEventScriptDef OnDocumentRejected
		{
			get
			{
				return m_DocRejected;
			}
		}

		public IWDEEventScriptDef OnDocumentUnRejected
		{
			get
			{
				return m_DocUnRejected;
			}
		}

		public string Version
		{
			get { return m_Version; }
			set { m_Version = value == null ? string.Empty : value; }
		}

		public IWDEDatabases Databases
		{
			get { return (IWDEDatabases) m_Databases; }
		}

		public void LoadFromStream(System.IO.Stream aStream)
		{
			if(aStream == null)
				throw new ArgumentNullException("aStream", "aStream cannot be null");

			aStream.Seek(0, SeekOrigin.Begin);

			StreamReader sr = new StreamReader(aStream, true);
			XmlTextReader XmlReader = new XmlTextReader(sr);
			try
			{
				XmlReader.MoveToContent();
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Project"))
				{
					Clear();

					m_LinkResolver = new LinkResolver();
					try
					{
						if(Utils.GetAttribute(XmlReader, "ActiveLanguage", "") != "")
						{
							ConvertOldProject(XmlReader);
						}
						else
						{
							m_APIVersion = Utils.GetAttribute(XmlReader, "APIVersion", "");
							m_CreatedBy = Utils.GetAttribute(XmlReader, "CreatedBy", "");
							m_Version = Utils.GetAttribute(XmlReader, "Version", "1.0.0.0");
							string temp = Utils.GetAttribute(XmlReader, "DateCreated", "");
							if(temp != "")
							{
								try
								{
									m_DateCreated = Utils.ISOToDateTime(temp);
								}
								catch
								{
									m_DateCreated = Utils.USStrToDateTime(temp);
								}
							}

							temp = Utils.GetAttribute(XmlReader, "DateModified", "");
							if(temp != "")
							{
								try
								{
									m_DateModified = Utils.ISOToDateTime(temp);
								}
								catch
								{
									m_DateModified = Utils.USStrToDateTime(temp);
								}
							}

							m_ModifiedBy = Utils.GetAttribute(XmlReader, "ModifiedBy", "");
							m_Description = Utils.GetAttribute(XmlReader, "Description", "");
						
							m_Options = Utils.GetProjectOption(XmlReader, "Options");

                            Version projVersion = new Version(m_APIVersion);
                            Version trdVersion = new Version("3.2.0.3");
                            if (projVersion < trdVersion)
                                m_Options |= WDEProjectOption.TrackRowDeletion;

							m_ScrollInterval = int.Parse(Utils.GetAttribute(XmlReader, "ImageScrollInterval", "30"));
							m_ScriptLanguage = Utils.GetScriptLanguage(XmlReader, "ScriptLanguage");
                            m_SelectedForeColor = Utils.GetColor(XmlReader, "SelectedForeColor", SELECTED_FORE_COLOR);
                            m_SelectedBackColor = Utils.GetColor(XmlReader, "SelectedBackground", SELECTED_BACK_COLOR);
                            m_HintFont = Utils.GetAttribute(XmlReader, "HintFont", SELECTED_HINT_FONT);
                            m_KeyboardModes = Utils.GetKeyMode(XmlReader, "KeyboardModes");

							XmlReader.Read();
							XmlReader.MoveToContent();

							IWDEXmlPersist ipers = (IWDEXmlPersist) m_DocumentDefs;
							ipers.ReadFromXml(XmlReader);

                            while ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "RejectCode"))
                            {
                                XmlReader.Read();
                                XmlReader.MoveToContent();
                            }

                            ipers = (IWDEXmlPersist) m_SessionDefs;
							ipers.ReadFromXml(XmlReader);

							ipers = (IWDEXmlPersist) m_ProjectColors;
							ipers.ReadFromXml(XmlReader);

							ipers = (IWDEXmlPersist) m_DocRejected;
							ipers.ReadFromXml(XmlReader);
					
							ipers = (IWDEXmlPersist) m_DocUnRejected;
							ipers.ReadFromXml(XmlReader);

                            ipers = (IWDEXmlPersist)m_OnKeyPress;
                            ipers.ReadFromXml(XmlReader);

                            ipers = (IWDEXmlPersist)m_OnStartWork;
                            ipers.ReadFromXml(XmlReader);

                            ipers = (IWDEXmlPersist)m_OnEndWork;
                            ipers.ReadFromXml(XmlReader);

                            ipers = (IWDEXmlPersist)m_OnPageChange;
                            ipers.ReadFromXml(XmlReader);                                                        

                            if ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ExternalAssemblies"))
                            {
                                XmlReader.Read();
                                XmlReader.MoveToContent();

                                while ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Assembly"))
                                {
                                    string assemblyName = Utils.GetAttribute(XmlReader, "Name", "");
                                    if (assemblyName != "")
                                        m_ExternalAssemblies.Add(assemblyName);

                                    XmlReader.Read();
                                    XmlReader.MoveToContent();
                                }

                                if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ExternalAssemblies"))
                                {
                                    XmlReader.ReadEndElement();
                                    XmlReader.MoveToContent();
                                }
                            }

                            if ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "References"))
                            {
                                XmlReader.Read();
                                XmlReader.MoveToContent();

                                while ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Assembly"))
                                {
                                    string assemblyName = Utils.GetAttribute(XmlReader, "Name", "");
                                    if (assemblyName != "")
                                        m_References.Add(assemblyName);

                                    XmlReader.Read();
                                    XmlReader.MoveToContent();
                                }

                                if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "References"))
                                {
                                    XmlReader.ReadEndElement();
                                    XmlReader.MoveToContent();
                                }
                            }

							if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Script"))
							{
								m_Script = XmlReader.ReadElementString();
								XmlReader.MoveToContent();

								if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Script"))
								{
									XmlReader.ReadEndElement();
									XmlReader.MoveToContent();
								}
							}
						}

						if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Project"))
						{
							XmlReader.ReadEndElement();
							XmlReader.MoveToContent();
						}                        

                        if(!XmlReader.EOF)
							throw new XmlException("Unrecognized xml format", null, XmlReader.LineNumber, XmlReader.LinePosition);
						m_LinkResolver.ResolveLinks();
					}
					finally
					{
						m_LinkResolver = null;
					}
				}
				else
					throw new XmlException("Invalid root element in project", null, XmlReader.LineNumber, XmlReader.LinePosition);
			}
			catch
			{
				Clear();
				throw;
			}
			finally
			{
				m_ConvertOldFormat = false;
				XmlReader.Close();
			}
		}

		public void LoadFromStream(System.Runtime.InteropServices.ComTypes.IStream aStream)
		{
			if(aStream == null)
				throw new ArgumentNullException("aStream", "aStream cannot be null");

			MemoryStream mst = new MemoryStream();
			try
			{
				Utils.UCOMIStreamToStream(aStream, mst);
				mst.Seek(0, SeekOrigin.Begin);
				LoadFromStream(mst);
			}
			finally
			{
				mst.Close();
			}
		}

		public void LoadFromFile(string FileName)
		{
			if(FileName == null)
				throw new ArgumentNullException("FileName","FileName cannot be null");

			FileStream fst = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			try
			{
				LoadFromStream(fst);
			}
			finally
			{
				fst.Close();
			}
		}

		public void LoadFromBytes(byte[] bytes)
		{
			MemoryStream memStream = new MemoryStream(bytes);
			try
			{
				LoadFromStream(memStream);
			}
			finally
			{
				memStream.Close();
			}
		}

		public void Clear()
		{
			m_ConvertOldFormat = false;
			m_APIVersion = "";
			m_CreatedBy = "";
			m_DateCreated = DateTime.Now;
			m_DateModified = DateTime.Now;
			m_ModifiedBy = "";
			m_Description = "";
            m_Options = WDEProjectOption.TrackRowDeletion; 
			m_ScrollInterval = 30;
			m_ScriptLanguage = WDEScriptLanguage.CSharpNet;
            m_KeyboardModes = WDEKeyMode.Normal;
			m_Script = "";

            m_SelectedForeColor = SELECTED_FORE_COLOR;
            m_SelectedBackColor = SELECTED_BACK_COLOR;
            m_HintFont = SELECTED_HINT_FONT;

			m_DocumentDefs.Clear();
			m_SessionDefs.Clear();
			m_ProjectColors.SetDefaults();

			m_DocRejected.Description = "";
			m_DocRejected.Enabled = false;
			m_DocRejected.ScriptFullName = "";

			m_DocUnRejected.Description = "";
			m_DocUnRejected.Enabled = false;
			m_DocUnRejected.ScriptFullName = "";

			m_OnEndWork.Description = "";
			m_OnEndWork.Enabled = false;
			m_OnEndWork.ScriptFullName = "";

			m_OnKeyPress.Description = "";
			m_OnKeyPress.Enabled = false;
			m_OnKeyPress.ScriptFullName = "";

			m_OnPageChange.Description = "";
			m_OnPageChange.Enabled = false;
			m_OnPageChange.ScriptFullName = "";

			m_OnStartWork.Description = "";
			m_OnStartWork.Enabled = false;
			m_OnStartWork.ScriptFullName = "";

			m_Version = "1.0.0.0";
			m_Databases.Clear();

            m_ExternalAssemblies.Clear();
            m_References.Clear();			
			GC.Collect();
		}

		#endregion

        #region IWDEProject_R1 Members

        public IWDEEventScriptDef OnKeyPress
        {
            get { return m_OnKeyPress; }
        }

        public IWDEEventScriptDef OnStartWork
        {
            get { return m_OnStartWork; }
        }

        public IWDEEventScriptDef OnEndWork
        {
            get { return m_OnEndWork; }
        }

        public IWDEEventScriptDef OnPageChange
        {
            get { return m_OnPageChange; }
        }

        public List<string> ExternalAssemblies
        {
            get { return m_ExternalAssemblies; }
        }

        public List<string> References
        {
            get { return m_References; }
        }

        #endregion

        #region IWDEProjectPM Members

		public void SaveToStream(System.IO.Stream aStream)
		{
			if(aStream == null)
				throw new ArgumentNullException("aStream","aStream cannot be null");

			StreamWriter sr = new StreamWriter(aStream, new UTF8Encoding(false));
			XmlTextWriter XmlWriter = new XmlTextWriter(sr);
			try
			{
				XmlWriter.Formatting = Formatting.Indented;
				XmlWriter.WriteStartDocument(true);
				XmlWriter.WriteStartElement("Project");
				XmlWriter.WriteAttributeString("APIVersion", VersionInfo.VersionNumber);
				if(m_Version != "1.0.0.0")
					XmlWriter.WriteAttributeString("Version", m_Version);
				if(m_CreatedBy != "")
					XmlWriter.WriteAttributeString("CreatedBy", m_CreatedBy);
				XmlWriter.WriteAttributeString("DateCreated", Utils.DateTimeToISO(m_DateCreated));
				m_DateModified = DateTime.Now;
				XmlWriter.WriteAttributeString("DateModified", Utils.DateTimeToISO(m_DateModified));
				
				if(m_ModifiedBy != "")
					XmlWriter.WriteAttributeString("ModifiedBy", m_ModifiedBy);
				if(m_Description != "")
					XmlWriter.WriteAttributeString("Description", m_Description);
				if(m_Options != WDEProjectOption.None)
					XmlWriter.WriteAttributeString("Options", m_Options.ToString());
				XmlWriter.WriteAttributeString("ImageScrollInterval", m_ScrollInterval.ToString());
				if (m_ScriptLanguage != WDEScriptLanguage.CSharpNet)
					XmlWriter.WriteAttributeString("ScriptLanguage", m_ScriptLanguage.ToString());

                if (m_SelectedForeColor != SELECTED_FORE_COLOR)
                    XmlWriter.WriteAttributeString("SelectedForeColor", m_SelectedForeColor.Name);
                if (m_SelectedBackColor != SELECTED_BACK_COLOR)
                    XmlWriter.WriteAttributeString("SelectedBackground", m_SelectedBackColor.Name);
                if (m_HintFont != SELECTED_HINT_FONT)
                    XmlWriter.WriteAttributeString("HintFont", m_HintFont);
                if (m_KeyboardModes != WDEKeyMode.Normal)
                    XmlWriter.WriteAttributeString("KeyboardModes", m_KeyboardModes.ToString());

				IWDEXmlPersist ipers = (IWDEXmlPersist) m_DocumentDefs;
				ipers.WriteToXml(XmlWriter);				

				ipers = (IWDEXmlPersist) m_SessionDefs;
				ipers.WriteToXml(XmlWriter);

				ipers = (IWDEXmlPersist) m_ProjectColors;
				ipers.WriteToXml(XmlWriter);

				ipers = (IWDEXmlPersist) m_DocRejected;
				ipers.WriteToXml(XmlWriter);
					
				ipers = (IWDEXmlPersist) m_DocUnRejected;
				ipers.WriteToXml(XmlWriter);

                ipers = (IWDEXmlPersist)m_OnKeyPress;
                ipers.WriteToXml(XmlWriter);

                ipers = (IWDEXmlPersist)m_OnStartWork;
                ipers.WriteToXml(XmlWriter);

                ipers = (IWDEXmlPersist)m_OnEndWork;
                ipers.WriteToXml(XmlWriter);

                ipers = (IWDEXmlPersist)m_OnPageChange;
                ipers.WriteToXml(XmlWriter);

                if (m_ExternalAssemblies.Count > 0)
                {
                    XmlWriter.WriteStartElement("ExternalAssemblies");
                    foreach(string assemblyName in m_ExternalAssemblies)
                    {
                        XmlWriter.WriteStartElement("Assembly");
                        XmlWriter.WriteAttributeString("Name", assemblyName);
                        XmlWriter.WriteEndElement();
                    }
                    XmlWriter.WriteEndElement();
                }

                if (m_References.Count > 0)
                {
                    XmlWriter.WriteStartElement("References");
                    foreach (string assemblyName in m_References)
                    {
                        XmlWriter.WriteStartElement("Assembly");
                        XmlWriter.WriteAttributeString("Name", assemblyName);
                        XmlWriter.WriteEndElement();
                    }
                    XmlWriter.WriteEndElement();
                }

				if(m_Script != "")
				{
                    XmlWriter.WriteStartElement("Script");
                    XmlWriter.WriteCData(m_Script);
                    XmlWriter.WriteEndElement();
				}

				XmlWriter.WriteEndElement();
				XmlWriter.WriteEndDocument();
			}
			finally
			{
				XmlWriter.Close();
			}
		}

		public void SaveToStream(System.Runtime.InteropServices.ComTypes.IStream aStream)
		{
			if(aStream == null)
				throw new ArgumentNullException("aStream","aStream cannot be null");

			MemoryStream mst = new MemoryStream();
			try
			{
				SaveToStream(mst);
				mst.Seek(0, SeekOrigin.Begin);
				Utils.StreamToUCOMIStream(mst, aStream);
			}
			finally
			{
				mst.Close();
			}
		}

		public void SaveToFile(string FileName)
		{
			if(FileName == null)
				throw new ArgumentNullException("FileName","FileName cannot be null");

			FileStream fst = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None);
			try
			{
				SaveToStream(fst);
			}
			finally
			{
				fst.Close();
			}
		}

		#endregion

		#region IWDEProjectInternal Members

		public LinkResolver Resolver
		{
			get
			{
                if (m_LinkResolver == null)
                    throw new WDEException("API90005");
				return m_LinkResolver;
			}
			set
			{
				m_LinkResolver = value;
			}
		}

		public bool ConvertOldFormat
		{
			get
			{
				return m_ConvertOldFormat;
			}

			set
			{
				m_ConvertOldFormat = value;
			}
		}
		
		public void AppendOldScriptText(string ScriptText, string NamePath)
		{
			SimpleCodeParser parser = null;
			switch(ScriptLanguage)
			{
				case WDEScriptLanguage.DelphiScript:
					parser = new SimpleDelphiParser();
					break;
				case WDEScriptLanguage.JavaScript:
					parser = new SimpleJScriptParser();
					break;
				case WDEScriptLanguage.VBScript:
					parser = new SimpleVBParser();
					break;
				default:
                    throw new WDEException("API00032", new object[] {ScriptLanguage});
			}

			parser.ScriptText = ScriptText;
			parser.ReplaceMethodDecl(NamePath);
			m_Script += Environment.NewLine + parser.ScriptText;
		}

		public void AppendOldExpression(string Expression, string NamePath)
		{
			string template = "";
			switch(ScriptLanguage)
			{
				case WDEScriptLanguage.DelphiScript:
					template = "function {0}: Boolean; begin Result := {1}; end;";
					break;
				case WDEScriptLanguage.JavaScript:
					template = "function {0}(); { _Calc = {1}; }";
					break;
				case WDEScriptLanguage.VBScript:
					template = "Function {0}() _Calc = {1} End Function";
					break;
				default:
                   throw new WDEException("API00032", new object[] {ScriptLanguage});
			}
			m_Script += Environment.NewLine + string.Format(template, new object[] {NamePath, Expression});
		}

		public ArrayList GetTopLevelCollections()
		{
			ArrayList result = new ArrayList();
			result.Add(m_DocumentDefs);
			result.Add(m_SessionDefs);
			return result;
		}

		#endregion

		#region Private Members

		private void ConvertOldProject(XmlTextReader XmlReader)
		{
			m_ConvertOldFormat = true;

			m_ProjectColors.CellColor = Utils.GetColor(XmlReader, "CellColor", m_ProjectColors.CellColor);
			m_ProjectColors.CellBorderColor = Utils.GetColor(XmlReader, "CellBorderColor", m_ProjectColors.CellBorderColor);
			m_ProjectColors.SnippetBorderColor = Utils.GetColor(XmlReader, "SnippetBorderColor", m_ProjectColors.SnippetBorderColor);
			m_ProjectColors.SnippetColor = Utils.GetColor(XmlReader, "SnippetColor", m_ProjectColors.SnippetColor);
			m_ProjectColors.ZoneBorderColor = Utils.GetColor(XmlReader, "ZoneBorderColor", m_ProjectColors.ZoneBorderColor);
			m_ProjectColors.ZoneColor = Utils.GetColor(XmlReader, "ZoneColor", m_ProjectColors.ZoneColor);

			m_APIVersion = Utils.GetAttribute(XmlReader, "APIVersion", "");
			m_Version = Utils.GetAttribute(XmlReader, "Version", "1.0.0.0");
			m_CreatedBy = Utils.GetAttribute(XmlReader, "CreatedBy", "");
			string temp = Utils.GetAttribute(XmlReader, "DateCreated", "");
			if(temp != "")
			{
				try
				{
					m_DateCreated = Utils.ISOToDateTime(temp);
				}
				catch
				{
					m_DateCreated = Utils.USStrToDateTime(temp);
				}
			}

			temp = Utils.GetAttribute(XmlReader, "DateModified", "");
			if(temp != "")
			{
				try
				{
					m_DateModified = Utils.ISOToDateTime(temp);
				}
				catch
				{
					m_DateModified = Utils.USStrToDateTime(temp);
				}
			}

			m_ModifiedBy = Utils.GetAttribute(XmlReader, "ModifiedBy", "");
			m_Options = Utils.GetProjectOption(XmlReader, "Options");
			m_ScrollInterval = int.Parse(Utils.GetAttribute(XmlReader, "ImageScrollInterval", "30"));
			m_ScriptLanguage = Utils.GetScriptLanguage(XmlReader, "ScriptLanguage");
		    m_KeyboardModes = Utils.GetKeyMode(XmlReader, "KeyboardModes");

			XmlReader.Read();
			XmlReader.MoveToContent();

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Languages"))
			{
				XmlReader.ReadInnerXml(); // discard this element
				XmlReader.MoveToContent();
			}

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Databases"))
			{
				m_Databases.Clear();
				XmlReader.Read();
				XmlReader.MoveToContent();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Database"))
				{
					string name = Utils.GetAttribute(XmlReader, "Name", "");
					string connStr = Utils.GetAttribute(XmlReader, "ConnectionString", "");
					bool dl = Utils.GetBoolValue(XmlReader, "Download", false);

					m_Databases.Add(name, connStr, dl);

					XmlReader.Read();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Databases"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
				ReadDescription(XmlReader);

			IWDEXmlPersist ipers = (IWDEXmlPersist) m_DocumentDefs;
			ipers.ReadFromXml(XmlReader);

            while ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "RejectCode"))
            {
                XmlReader.Read();
                XmlReader.MoveToContent();
            }

            if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Scripts"))
				ReadScripts(XmlReader);

			ipers = (IWDEXmlPersist) m_SessionDefs;
			ipers.ReadFromXml(XmlReader);
		}

		/// <summary>
		/// Used in the conversion process for reading older file formats
		/// </summary>
		private void ReadDescription(XmlTextReader XmlReader)
		{
			m_Description = Utils.GetAttribute(XmlReader, "ENGLISH", "");
			XmlReader.Read();
			XmlReader.MoveToContent();
		}

		/// <summary>
		/// Used in the conversion process for reading older file formats
		/// </summary>
		private void ReadScripts(XmlTextReader XmlReader)
		{
			StringBuilder sb = new StringBuilder();
			XmlReader.Read();
			XmlReader.MoveToContent();

			while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Script"))
			{
                if (XmlReader.IsEmptyElement)
                {
                    XmlReader.Read();
                    XmlReader.MoveToContent();
                }
                else
                {
                    XmlReader.Read();
                    XmlReader.MoveToContent();

                    if ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Script"))
                    {
                        XmlReader.Read();
                        XmlReader.MoveToContent();

                        while ((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "String"))
                        {
                            sb.Append(Utils.GetAttribute(XmlReader, "Value", "") + Environment.NewLine);
                            XmlReader.Read();
                            XmlReader.MoveToContent();
                        }

                        if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Script"))
                        {
                            XmlReader.ReadEndElement();
                            XmlReader.MoveToContent();
                        }
                    }

                    if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Script"))
                    {
                        XmlReader.ReadEndElement();
                        XmlReader.MoveToContent();
                    }
                }
			}
			
			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Scripts"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}

			m_Script = sb.ToString() + Environment.NewLine + m_Script;
		}
		
		#endregion

        #region IWDEProject_R3 Members

        public Color SelectedForeColor
        {
            get
            {
                return m_SelectedForeColor;
            }
            set
            {
                m_SelectedForeColor = value;
            }
        }

        public Color SelectedBackground
        {
            get
            {
                return m_SelectedBackColor;
            }
            set
            {
                m_SelectedBackColor = value;
            }
        }

        #endregion

        #region IWDEProject_R4 Members

        public Font HintFont
        {
            get
            {
                return Utils.StringToFont(m_HintFont);
            }
            set
            {
                m_HintFont = Utils.FontToString(value);
            }
        }

        #endregion

        #region IWDEProject_R5 Members
        public WDEKeyMode KeyboardModes
        {
            get
            {
                return m_KeyboardModes;
            }
            set
            {
                m_KeyboardModes = value;
            }
        }
        #endregion
    }

	public class WDEDatabases : WDEBaseCollection, IWDEDatabases
	{
		private WDEDatabases(object Parent) : base(Parent) {}

		public static IWDEDatabases Create(IWDEProject Parent)
		{
			return new WDEDatabases(Parent) as IWDEDatabases;
		}

		public static IWDEDatabases CreateInstance(IWDEProject Parent)
		{
			return Create(Parent);
		}

		protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			IWDEDatabase def = (IWDEDatabase) Item;
			return (string.Compare(def.Name, Name, true) == 0);
		}

		#region IWDEDatabases Members

		public IWDEDatabase this[string Name]
		{
			get
			{
				return (IWDEDatabase) base.InternalFind(Name);
			}
		}

		public IWDEDatabase this[int Index]
		{
			get
			{
				return (IWDEDatabase) base.InternalGetIndex(Index);
			}
		}

		#endregion

		public void Add(string name, string connStr, bool download)
		{
			WDEDatabase db = (WDEDatabase) WDEDatabase.Create();
			db.Name = name;
			db.ConnectionString = connStr;
			db.Download = download;
			base.InternalAdd(db);
		}
	}

	public class WDEDatabase : WDEBaseCollectionItem, IWDEDatabase
	{
		private string m_Name;
		private string m_ConnectionString;
		private bool m_Download;

		private WDEDatabase()
		{
		}

		public static IWDEDatabase Create()
		{
			return new WDEDatabase() as IWDEDatabase;
		}

		public static IWDEDatabase CreateInstance()
		{
			return Create();
		}

		protected override string InternalGetNodeName()
		{
			return m_Name;
		}

		#region IWDEDatabase Members

		public string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;
			}
		}

		public string ConnectionString
		{
			get
			{
				return m_ConnectionString;
			}
			set
			{
				m_ConnectionString = value;
			}
		}

		public bool Download
		{
			get
			{
				return m_Download;
			}
			set
			{
				m_Download = value;
			}
		}

		#endregion
	}

	public class WDEProjectColors : IWDEProjectColors, IWDEXmlPersist
	{
		private Color m_LIGHT_YELLOW;

		private Color m_CellBorderColor;
		private Color m_CellColor;
		private Color m_SnippetBorderColor;
		private Color m_SnippetColor;
		private Color m_ZoneBorderColor;
		private Color m_ZoneColor;
		private Color m_SelectedColor;
		private Color m_ExcludedColor;

		private WDEProjectColors()
		{
			m_LIGHT_YELLOW = Color.FromArgb(0xE7, 0xFF, 0xFF);

			SetDefaults();
		}

		public static IWDEProjectColors Create()
		{
			return new WDEProjectColors() as IWDEProjectColors;
		}

		public static IWDEProjectColors CreateInstance()
		{
			return Create();
		}

		#region IWDEProjectColors Members

		public Color CellBorderColor
		{
			get
			{
				return m_CellBorderColor;
			}
			set
			{
				m_CellBorderColor = value;
			}
		}

		public Color CellColor
		{
			get
			{
				return m_CellColor;
			}
			set
			{
				m_CellColor = value;
			}
		}

		public Color SnippetBorderColor
		{
			get
			{
				return m_SnippetBorderColor;
			}
			set
			{
				m_SnippetBorderColor = value;
			}
		}

		public Color SnippetColor
		{
			get
			{
				return m_SnippetColor;
			}
			set
			{
				m_SnippetColor = value;
			}
		}

		public Color ZoneBorderColor
		{
			get
			{
				return m_ZoneBorderColor;
			}
			set
			{
				m_ZoneBorderColor = value;
			}
		}

		public Color ZoneColor
		{
			get
			{
				return m_ZoneColor;
			}
			set
			{
				m_ZoneColor = value;
			}
		}

		public Color SelectedColor
		{
			get
			{
				return m_SelectedColor;
			}
			set
			{
				m_SelectedColor = value;
			}
		}

		public Color ExcludedColor
		{
			get
			{
				return m_ExcludedColor;
			}
			set
			{
				m_ExcludedColor = value;
			}
		}

		public void SetDefaults()
		{
			m_CellBorderColor = Color.Red;
			m_CellColor = m_LIGHT_YELLOW;
			m_SnippetBorderColor = Color.Red;
			m_SnippetColor = m_LIGHT_YELLOW;
			m_ZoneBorderColor = Color.Red;
			m_ZoneColor = Color.Yellow;
			m_SelectedColor = Color.Yellow;
			m_ExcludedColor = Color.DarkGray;
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ProjectColors"))
			{
				m_CellBorderColor = Utils.GetColor(XmlReader, "CellBorderColor", Color.Red);
				m_CellColor = Utils.GetColor(XmlReader, "CellColor", m_LIGHT_YELLOW);
				m_SnippetBorderColor = Utils.GetColor(XmlReader, "SnippetBorderColor", Color.Red);
				m_SnippetColor = Utils.GetColor(XmlReader, "SnippetColor", m_LIGHT_YELLOW);
				m_ZoneBorderColor = Utils.GetColor(XmlReader, "ZoneBorderColor", Color.Red);
				m_ZoneColor = Utils.GetColor(XmlReader, "ZoneColor", Color.Yellow);
				m_SelectedColor = Utils.GetColor(XmlReader, "SelectedColor", Color.Yellow);
				m_ExcludedColor = Utils.GetColor(XmlReader, "ExcludedColor", Color.DarkGray);

				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ProjectColors"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("ProjectColors");
			if(CellBorderColor != Color.Red)
				XmlWriter.WriteAttributeString("CellBorderColor", CellBorderColor.Name); 
			if(CellColor != m_LIGHT_YELLOW)
				XmlWriter.WriteAttributeString("CellColor", CellColor.Name);
			if(SnippetBorderColor != Color.Red)
				XmlWriter.WriteAttributeString("SnippetBorderColor", SnippetBorderColor.Name);
			if(SnippetColor != m_LIGHT_YELLOW)
				XmlWriter.WriteAttributeString("SnippetColor", SnippetColor.Name);
			if(ZoneBorderColor != Color.Red)
				XmlWriter.WriteAttributeString("ZoneBorderColor", ZoneBorderColor.Name);
			if(ZoneColor != Color.Yellow)
				XmlWriter.WriteAttributeString("ZoneColor", ZoneColor.Name);
			if(SelectedColor != Color.Yellow)
				XmlWriter.WriteAttributeString("SelectedColor", SelectedColor.Name);
			if(ExcludedColor != Color.DarkGray)
				XmlWriter.WriteAttributeString("ExcludedColor", ExcludedColor.Name);
			XmlWriter.WriteEndElement();
		}

		#endregion
	}

	public class WDEDocumentDefs : WDEBaseCollection, IWDEDocumentDefs, IWDEXmlPersist
	{
		private WDEDocumentDefs(object Parent) : base(Parent)
		{
		}

		public static IWDEDocumentDefs Create(IWDEProject Parent)
		{
			return new WDEDocumentDefs(Parent) as IWDEDocumentDefs;
		}

		public static IWDEDocumentDefs CreateInstance(IWDEProject Parent)
		{
			return Create(Parent);
		}

		protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			IWDEDocumentDef def = (IWDEDocumentDef) Item;
			return (string.Compare(def.DocType, Name, true) == 0);
		}

		protected override ArrayList GetCollectionList()
		{
			return base.GetSameLevelCollections();
		}

		#region IWDEDocumentDefs Members

		public IWDEDocumentDef this[int index]
		{
			get
			{
				return (IWDEDocumentDef) base.InternalGetIndex(index);
			}
		}

		public int Find(string DocType)
		{
			return base.InternalIndexOf(DocType);
		}

		public IWDEDocumentDef Add(string DocType)
		{
			int res = base.VerifyName(DocType);
			if(res == 0)
			{
				IWDEDocumentDef newDef = WDEDocumentDef.Create();
				base.InternalAdd((WDEBaseCollectionItem) newDef);
				newDef.DocType = DocType;
				return newDef;
			}
            else if (res == -1)
                 throw new WDEException("API00037", new object[] { DocType });
            else
                 throw new WDEException("API00038", new object[] { DocType });
		}

		public IWDEDocumentDef Add()
		{
			string newName = base.GetNextDefaultName("Document");
			IWDEDocumentDef newDef = WDEDocumentDef.Create();
			base.InternalAdd((WDEBaseCollectionItem) newDef);
			newDef.DocType = newName;
			return newDef;
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DocumentDefs"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();
			}

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DocumentDef"))
			{
				Clear();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "DocumentDef"))
				{
					IWDEDocumentDef def = WDEDocumentDef.Create();
					base.InternalAdd((WDEBaseCollectionItem) def);
					IWDEXmlPersist ipers = (IWDEXmlPersist) def;
					ipers.ReadFromXml(XmlReader);
					base.RegisterObject((WDEBaseCollectionItem) def);
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "DocumentDefs"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}                
            }
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion
	}

	public class WDEDocumentDef : WDEBaseCollectionItem, IWDEDocumentDef, IWDEXmlPersist
	{
		private string m_Desc;
		private string m_DocType;
		private IWDEFormDefs m_FormDefs;
		private string m_StoredDocType;
		private IWDERecordDefs m_RecordDefs;
		private IWDEImageSourceDefs m_ImageSourceDefs;

		private WDEDocumentDef()
		{
			m_Desc = "";
			m_DocType = "";
			m_StoredDocType = "";
			
			m_FormDefs = WDEFormDefs.Create(this);
			m_RecordDefs = WDERecordDefs.Create(this);
			m_ImageSourceDefs = WDEImageSourceDefs.Create(this);
		}

		public static IWDEDocumentDef Create()
		{
			return new WDEDocumentDef() as IWDEDocumentDef;
		}

		public static IWDEDocumentDef CreateInstance()
		{
			return Create();
		}

		protected override string InternalGetNodeName()
		{
			return m_DocType;
		}

		protected override void InternalClearNotify()
		{
			m_FormDefs.Clear();
			m_RecordDefs.Clear();
			m_ImageSourceDefs.Clear();
		}

		public override ArrayList GetChildCollections()
		{
			ArrayList al = new ArrayList();
			al.Add(m_FormDefs);
			al.Add(m_RecordDefs);
			al.Add(m_ImageSourceDefs);
			return al;
		}


		#region IWDEDocumentDef Members

		public string Description
		{
			get
			{
				return m_Desc;
			}
			set
			{
				if(value == null)
					m_Desc = "";
				else
					m_Desc = value;
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
				string newName = value;
				if(newName == null)
					newName = "";

				if(newName != m_DocType)
				{
					int res = base.Collection.VerifyName(newName);
					if(res == 0)
						m_DocType = newName;
                    else if (res == -1)
                         throw new WDEException("API00037", new object[] { newName });
                    else if (res == -2)
                         throw new WDEException("API00038", new object[] { newName });
				}
			}
		}

		public IWDEFormDefs FormDefs
		{
			get
			{
				return m_FormDefs;
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
				if(value == null)
					m_StoredDocType = "";
				else
					m_StoredDocType = value;
			}
		}

		public IWDERecordDefs RecordDefs
		{
			get
			{
				return m_RecordDefs;
			}
		}

		public IWDEImageSourceDefs ImageSourceDefs
		{
			get
			{
				return m_ImageSourceDefs;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DocumentDef"))
			{
				if(ConvertingOldProject)
				{	
					ConvertOldDocument(XmlReader);
				}
				else
				{
					m_DocType = Utils.GetAttribute(XmlReader, "DocType", "");
				
					m_Desc = Utils.GetAttribute(XmlReader, "Description", "");
					m_StoredDocType = Utils.GetAttribute(XmlReader, "StoredDocType", "");

					XmlReader.Read();
					XmlReader.MoveToContent();

                    IWDEXmlPersist ipers = (IWDEXmlPersist) FormDefs;
					ipers.ReadFromXml(XmlReader);

					ipers = (IWDEXmlPersist) RecordDefs;
					ipers.ReadFromXml(XmlReader);

					ipers = (IWDEXmlPersist) ImageSourceDefs;
					ipers.ReadFromXml(XmlReader);
				}
				

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "DocumentDef"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}	
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("DocumentDef");
		
			if(DocType != "")
				XmlWriter.WriteAttributeString("DocType", DocType);
			if(Description != "")
				XmlWriter.WriteAttributeString("Description", Description);
			if(StoredDocType != "")
				XmlWriter.WriteAttributeString("StoredDocType", StoredDocType);

            IWDEXmlPersist ipers = (IWDEXmlPersist) FormDefs;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist) RecordDefs;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist) ImageSourceDefs;
			ipers.WriteToXml(XmlWriter);

			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Methods

		private void ConvertOldDocument(XmlTextReader XmlReader)
		{
			m_DocType = Utils.GetAttribute(XmlReader, "Name", "");
			m_StoredDocType = Utils.GetAttribute(XmlReader, "StoredDocType", "");

			XmlReader.Read();
			XmlReader.MoveToContent();

			IWDEXmlPersist ipers = (IWDEXmlPersist) m_RecordDefs;
			ipers.ReadFromXml(XmlReader);

			ConvertAccumulators(XmlReader);

			ReadDescription(XmlReader);

			ipers = (IWDEXmlPersist) m_FormDefs;
			ipers.ReadFromXml(XmlReader);

			ipers = (IWDEXmlPersist) m_ImageSourceDefs;
			ipers.ReadFromXml(XmlReader);			

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "DocumentDef"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		private void ReadDescription(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
			{
				Description = Utils.GetAttribute(XmlReader, "ENGLISH", "");
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		private void ConvertAccumulators(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Accumulators"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "Accumulator"))
				{
					IWDEBalanceCheckEditDef def = WDEBalanceCheckEditDef.Create();
					def.ErrorMargin = Utils.GetDoubleValue(XmlReader, "ErrorMargin");
					def.ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
					string id = this.DocType + "." + Utils.GetAttribute(XmlReader, "Name", "");
					
					XmlReader.Read();
					XmlReader.MoveToContent();

					if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ErrorMessages"))
					{
						def.ErrorMessage = Utils.GetAttribute(XmlReader, "ENGLISH", "");
						XmlReader.Read();
						XmlReader.MoveToContent();

						if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ErrorMessages"))
						{
							XmlReader.ReadEndElement();
							XmlReader.MoveToContent();
						}
					}

					IWDEProjectInternal iproj = this.GetProjectInternal();
					iproj.Resolver.AddObject(id, def);
					iproj.Resolver.AddObject(id + ".SumFields", def.SumFields);

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Accumulator"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Accumulators"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
	}

	public class WDEImageSourceDefs : WDEBaseCollection, IWDEImageSourceDefs, IWDEXmlPersist
	{
		private WDEImageSourceDefs(object Parent) : base(Parent)
		{
		}

		public static IWDEImageSourceDefs Create(IWDEDocumentDef Parent)
		{
			return new WDEImageSourceDefs(Parent) as IWDEImageSourceDefs;
		}

		public static IWDEImageSourceDefs CreateInstance(IWDEDocumentDef Parent)
		{
			return Create(Parent);
		}

		protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			IWDEImageSourceDef def = (IWDEImageSourceDef) Item;
			return (string.Compare(def.ImageType, Name, true) == 0);
		}

		protected override ArrayList GetCollectionList()
		{
			return base.GetSameLevelCollections();
		}

		#region IWDEImageSourceDefs Members

		public IWDEImageSourceDef this[int index]
		{
			get
			{
				return (IWDEImageSourceDef) base.InternalGetIndex(index);
			}
		}

		public int Find(string ImageType)
		{
			return base.InternalIndexOf(ImageType);
		}

		public override string GetNextDefaultName(string nameRoot)
		{
			if (nameRoot == null)
				throw new ArgumentNullException("nameRoot","nameRoot cannot be null");
			if (nameRoot == "")
				throw new ArgumentException("nameRoot cannot be blank","nameRoot");

            int suffix = 1;		
			IWDEDocumentDefs docDefs = (IWDEDocumentDefs)((WDEBaseCollectionItem)this.Parent).Collection;
			foreach (IWDEDocumentDef doc in docDefs)
			{
				foreach (WDEBaseCollectionItem item in doc.ImageSourceDefs)
				{
					string itemName = item.GetNodeName();
					string testName = itemName;
					if (testName.Length > nameRoot.Length)
						testName = testName.Substring(0, nameRoot.Length);

					if (string.Compare(testName, nameRoot, true) == 0)
					{
						if (itemName.Length > testName.Length)
						{
							string end = itemName.Substring(testName.Length);
							int a = 0;
							if (int.TryParse(end, out a))
							{
								if (suffix <= a)
									suffix = a + 1;
							}
						}
					}
				}
			}

            return nameRoot + suffix.ToString();
		}

		public IWDEImageSourceDef Add(string ImageType)
		{
			int res = base.VerifyName(ImageType);
			if(res == 0)
			{
				IWDEImageSourceDef newItem = WDEImageSourceDef.Create();
				base.InternalAdd((WDEBaseCollectionItem) newItem);
				newItem.ImageType = ImageType;
				return newItem;
			}
            else if (res == -1)
                 throw new WDEException("API00037", new object[] { ImageType });
            else
                 throw new WDEException("API00038", new object[] { ImageType });
		}

		public IWDEImageSourceDef Add()
		{
			string newName = GetNextDefaultName("ImageType");
			IWDEImageSourceDef newItem = WDEImageSourceDef.Create();
			base.InternalAdd((WDEBaseCollectionItem) newItem);
			newItem.ImageType = newName;
			return newItem;
		}

        public void Add(IWDEImageSourceDef newItem)
        {
            base.InternalAdd((WDEBaseCollectionItem)newItem);
        }

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			bool convertingOld = false;

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "InputSourceDefs"))
			{
				convertingOld = true;
				XmlReader.Read();
				XmlReader.MoveToContent();
			}

			if(convertingOld)
			{
				ReadOldImageSource(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ImageSourceDef"))
				{
					Clear();

					while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
						(XmlReader.Name == "ImageSourceDef"))
					{	
						IWDEImageSourceDef def = WDEImageSourceDef.Create();
						base.InternalAdd((WDEBaseCollectionItem) def);
						IWDEXmlPersist ipers = (IWDEXmlPersist) def;
						ipers.ReadFromXml(XmlReader);
						base.RegisterObject((WDEBaseCollectionItem) def);
					}
				}
			}

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "InputSourceDefs"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion

		#region Private Methods

		private void ReadOldImageSource(XmlTextReader XmlReader)
		{
			while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
				(XmlReader.Name == "ImageSource"))
			{
				string temp = Utils.GetAttribute(XmlReader, "Name", "");
				int index = Find(temp);
				if(index == -1)
				{
					IWDEImageSourceDef def = WDEImageSourceDef.Create();
					base.InternalAdd((WDEBaseCollectionItem) def);
					IWDEXmlPersist ipers = (IWDEXmlPersist) def;
					ipers.ReadFromXml(XmlReader);
					base.RegisterObject((WDEBaseCollectionItem) def);
				}
				else
				{
					IWDEXmlPersist ipers = (IWDEXmlPersist) this[index];
					ipers.ReadFromXml(XmlReader);
				}
			}
		}

		#endregion
	}

	public class WDEImageSourceDef : WDEBaseCollectionItem, IWDEImageSourceDef_R1, IWDEXmlPersist
	{
		private string m_Desc;
		private string m_ImageType;
		private string m_StoredAttachType;
		private string m_Overlay;
		private string m_Template;
		private bool m_PerformOCR;
		private IWDEZoneDefs m_ZoneDefs;
		private IWDESnippetDefs m_SnippetDefs;
		private IWDEDetailZoneDefs m_DetailZoneDefs;

		private WDEImageSourceDef()
		{
			m_Desc = "";
			m_ImageType = "";
			m_StoredAttachType = "";
			m_Overlay = "";
			m_Template = "";

			m_ZoneDefs = WDEZoneDefs.Create(this);
			m_SnippetDefs = WDESnippetDefs.Create(this);
			m_DetailZoneDefs = WDEDetailZoneDefs.Create(this);
		}

		public static IWDEImageSourceDef Create()
		{
			return new WDEImageSourceDef() as IWDEImageSourceDef;
		}

		public static IWDEImageSourceDef CreateInstance()
		{
			return Create();
		}

		protected override string InternalGetNodeName()
		{
			return m_ImageType;
		}

		public override ArrayList GetChildCollections()
		{
			ArrayList al = new ArrayList();
			al.Add(m_ZoneDefs);
			al.Add(m_SnippetDefs);
			al.Add(m_DetailZoneDefs);
			return al;
		}

		#region IWDEImageSourceDef Members

		public string Description
		{
			get
			{
				return m_Desc;
			}
			set
			{
				if(value == null)
					m_Desc = "";
				else
					m_Desc = value;
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
				string newName = value;
				if(newName == null)
					newName = "";

				if(newName != m_ImageType)
				{
					int res = base.Collection.VerifyName(newName);
					if(res == 0)
						m_ImageType = newName;
                    else if (res == -1)
                         throw new WDEException("API00037", new object[] {newName});
                    else if (res == -2)
                        throw new WDEException("API00038", new object[] { newName });
				}
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
				if(value == null)
					m_StoredAttachType = "";
				else
					m_StoredAttachType = value;
			}
		}

		public string Overlay
		{
			get
			{
				return m_Overlay;
			}
			set
			{
				if(value == null)
					m_Overlay = "";
				else
					m_Overlay = value;
			}
		}

		public string Template
		{
			get
			{
				return m_Template;
			}
			set
			{
				if(value == null)
					m_Template = "";
				else
					m_Template = value;
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

		public IWDEZoneDefs ZoneDefs
		{
			get
			{
				return m_ZoneDefs;
			}
		}

		public IWDESnippetDefs SnippetDefs
		{
			get
			{
				return m_SnippetDefs;
			}
		}

		public IWDEDetailZoneDefs DetailZoneDefs
		{
			get
			{
				return m_DetailZoneDefs;
			}
		}

        public IWDEImageSourceDef Clone()
        {
            return (IWDEImageSourceDef)this.MemberwiseClone(false);
        }

        #endregion

        #region IWDEImageSourceDef_R1 Members

        public IWDEZoneDef[] GetFlatZoneDefs()
        {
            List<IWDEZoneDef> result = new List<IWDEZoneDef>();
            foreach (IWDEZoneDef def in m_ZoneDefs)
                result.Add(def);

            foreach (IWDEDetailZoneDef def in m_DetailZoneDefs)
            {
                foreach (IWDEZoneDef zdef in def.ZoneDefs)
                    result.Add(zdef);
            }

            return result.ToArray();
        }

        #endregion

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			string id = "ImageSourceDef";
			if(this.ConvertingOldProject)
				id = "ImageSource";

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == id))
			{
				if(this.ConvertingOldProject)
				{
					m_ImageType = Utils.GetAttribute(XmlReader, "Name", "");
				}
				else
				{
					m_Desc = Utils.GetAttribute(XmlReader, "Description", "");
					m_ImageType = Utils.GetAttribute(XmlReader, "ImageType", "");
				}
				m_StoredAttachType = Utils.GetAttribute(XmlReader, "StoredAttachType", "");
				m_Overlay = Utils.GetAttribute(XmlReader, "Overlay", "");
				m_Template = Utils.GetAttribute(XmlReader, "Template", "");
				m_PerformOCR = Utils.GetBoolValue(XmlReader, "PerformOCR", false);

				XmlReader.Read();
				XmlReader.MoveToContent();

				if(this.ConvertingOldProject)
					ConvertDescription(XmlReader);

				IWDEXmlPersist ipers = (IWDEXmlPersist) m_ZoneDefs;
				ipers.ReadFromXml(XmlReader);

				ipers = (IWDEXmlPersist) m_SnippetDefs;
				ipers.ReadFromXml(XmlReader);

				ipers = (IWDEXmlPersist) m_DetailZoneDefs;
				ipers.ReadFromXml(XmlReader);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == id))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("ImageSourceDef");
			if(ImageType != "")
				XmlWriter.WriteAttributeString("ImageType", ImageType);
			if(StoredAttachType != "")
				XmlWriter.WriteAttributeString("StoredAttachType", StoredAttachType);
			if(Overlay != "")
				XmlWriter.WriteAttributeString("Overlay", Overlay);
			if(Template != "")
				XmlWriter.WriteAttributeString("Template", Template);
			if(PerformOCR)
				XmlWriter.WriteAttributeString("PerformOCR", PerformOCR.ToString());
			if(Description != "")
				XmlWriter.WriteAttributeString("Description", Description);

			IWDEXmlPersist ipers = (IWDEXmlPersist) m_ZoneDefs;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist) m_SnippetDefs;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist) m_DetailZoneDefs;
			ipers.WriteToXml(XmlWriter);

			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Methods

		private void ConvertDescription(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
			{
				m_Desc = Utils.GetAttribute(XmlReader, "ENGLISH", "");
				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Descriptions"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
    }

	public class WDERecordDefs : WDEBaseCollection, IWDERecordDefs, IWDEXmlPersist
	{
		private WDERecordDefs(object Parent) : base(Parent)
		{
		}

		public static IWDERecordDefs Create(object Parent)
		{
			if(!(Parent is IWDERecordDef) && !(Parent is IWDEDocumentDef))
                 throw new WDEException("API00030", new object[] {"RecordDefs", "RecordDef or DocumentDef"});          
  
			return new WDERecordDefs(Parent) as IWDERecordDefs;
		}

		public static IWDERecordDefs CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			IWDERecordDef def = (IWDERecordDef) Item;
			return (string.Compare(def.RecType, Name, true) == 0);
		}

        //The Implementation of override method GetCollectionList is removed to allow same name 
        //in record definition across different document definition but not in same document definition
        //protected override ArrayList GetCollectionList()
        //{
        //    //Comment to bug fix, should allow the recordname to be same in different document
        //    /*if(Parent is IWDEDocumentDef)
        //        return base.GetSameLevelCollections();
        //    else*/
        //        return base.GetCollectionList();
        //}

		#region IWDERecordDefs Members

		public IWDERecordDef this[int index]
		{
			get
			{
				return (IWDERecordDef) base.InternalGetIndex(index);
			}
		}

		public int Find(string RecType)
		{
			return base.InternalIndexOf(RecType);
		}

		public IWDERecordDef Add(string RecType)
		{
			int res = base.VerifyName(RecType);
			if(res == 0)
			{
				IWDERecordDef newDef = WDERecordDef.Create();
				base.InternalAdd((WDEBaseCollectionItem) newDef);
				newDef.RecType = RecType;
				return newDef;
			}
            else if (res == -1)
                 throw new WDEException("API00037", new object[] {RecType});
            else
                throw new WDEException("API00038", new object[] {RecType});
		}

		public IWDERecordDef Add()
		{
			string newName = GetNextDefaultName("Record");
			IWDERecordDef newDef = WDERecordDef.Create();
			base.InternalAdd((WDEBaseCollectionItem) newDef);
			newDef.RecType = newName;
			return newDef;
		}

        public void Add(IWDERecordDef def)
        {
            base.InternalAdd((WDEBaseCollectionItem)def);
		}		

		public override string GetNextDefaultName(string nameRoot)
		{
			if (nameRoot == null)
				throw new ArgumentNullException("nameRoot", "nameRoot cannot be null");
			if (nameRoot == "")
				throw new ArgumentException("nameRoot cannot be blank", "nameRoot");

			IWDEDocumentDefs docDefs;
			int suffix = 1;
			
			if (this.Parent is IWDEDocumentDef)
				docDefs = (IWDEDocumentDefs)((WDEBaseCollectionItem)this.Parent).Collection;
			else
				docDefs = (IWDEDocumentDefs)((WDEBaseCollectionItem)((WDEBaseCollectionItem)this.Parent).Parent).Collection;
			
			foreach (IWDEDocumentDef doc in docDefs)
				GetHighestSuffix(nameRoot, doc.RecordDefs, ref suffix);

			return nameRoot + suffix.ToString();
		}

		private int GetHighestSuffix(string nameRoot, IWDERecordDefs RecDefs, ref int suffix)
		{
			foreach (WDEBaseCollectionItem item in RecDefs)
			{
				string itemName = item.GetNodeName();
				string testName = itemName;
				if (testName.Length > nameRoot.Length)
					testName = testName.Substring(0, nameRoot.Length);

				if (string.Compare(testName, nameRoot, true) == 0)
				{
					if (itemName.Length > testName.Length)
					{
						string end = itemName.Substring(testName.Length);
						int a = 0;
						if (int.TryParse(end, out a))
						{
							if (suffix <= a)
								suffix = a + 1;
						}
					}
				}				 
				suffix = GetHighestSuffix(nameRoot, ((WDERecordDef)(item)).RecordDefs, ref suffix);
			}
			return suffix;
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "RecordDefs"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();
			}

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "RecordDef"))
			{
				Clear();
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "RecordDef"))
				{
					IWDERecordDef def = WDERecordDef.Create();
					base.InternalAdd((WDEBaseCollectionItem) def);
					IWDEXmlPersist ipers = (IWDEXmlPersist) def;
					ipers.ReadFromXml(XmlReader);
					base.RegisterObject((WDEBaseCollectionItem) def);
				}
			}

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "RecordDefs"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion
	}

	public class WDERecordDef : WDEBaseCollectionItem, IWDERecordDef, IWDEXmlPersist
	{
		private string m_Desc;
		private Rectangle m_DesignRect;
		private IWDEFieldDefs m_FieldDefs;
		private string m_RecType;
		private int m_MaxRecs;
		private int m_MinRecs;
		private IWDERecordDefs m_RecordDefs;

		private WDERecordDef()
		{
			m_Desc = "";
			m_DesignRect = Rectangle.Empty;
			m_FieldDefs = WDEFieldDefs.Create(this);
			m_RecType = "";
			m_RecordDefs = WDERecordDefs.Create(this);
		}

		public static IWDERecordDef Create()
		{
			return new WDERecordDef() as IWDERecordDef;
		}

		public static IWDERecordDef CreateInstance()
		{
			return Create();
		}

		protected override string InternalGetNodeName()
		{
			return m_RecType;
		}

		protected override void InternalClearNotify()
		{
			m_RecordDefs.Clear();
			m_FieldDefs.Clear();
		}

		public override ArrayList GetChildCollections()
		{
			ArrayList al = new ArrayList();
			al.Add(m_FieldDefs);
			al.Add(m_RecordDefs);
			return al;
		}

		#region IWDERecordDef Members

		public string Description
		{
			get
			{
				return m_Desc;
			}
			set
			{
				if(value == null)
					m_Desc = "";
				else
					m_Desc = value;
			}
		}

		public Rectangle DesignRect
		{
			get
			{
				return m_DesignRect;
			}
			set
			{
				m_DesignRect = value;
			}
		}

		public IWDEDocumentDef Document
		{
			get
			{
				object cur = base.Parent;
				while((cur != null) && !(cur is IWDEDocumentDef))
				{
					if(cur is WDEBaseCollectionItem)
					{
						WDEBaseCollectionItem item = (WDEBaseCollectionItem) cur;
						cur = item.Parent;
					}
					else
					{
						cur = null;
					}
				}
				return (IWDEDocumentDef) cur;
			}
		}

		public IWDEFieldDefs FieldDefs
		{
			get
			{
				return m_FieldDefs;
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
				string newName = value;
				if(newName == null)
					newName = "";

                if (newName.ToLower() == m_RecType.ToLower())
                    m_RecType = newName;
                else
				{
					int res = base.Collection.VerifyName(newName);
					if(res == 0)
						m_RecType = newName;
                    else if (res == -1)
                         throw new WDEException("API00037", new object[] {newName});
                    else if (res == -2)
                         throw new WDEException("API00038", new object[] {newName});
				}
			}
		}

		public int MaxRecs
		{
			get
			{
				return m_MaxRecs;
			}
			set
			{
				m_MaxRecs = value;
			}
		}

		public int MinRecs
		{
			get
			{
				return m_MinRecs;
			}
			set
			{
				m_MinRecs = value;
			}
		}

		public IWDERecordDefs RecordDefs
		{
			get
			{
				return m_RecordDefs;
			}
		}

        public IWDERecordDef Clone()
        {
            return (IWDERecordDef)this.MemberwiseClone();
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "RecordDef"))
			{
				if(this.ConvertingOldProject)
				{
					m_RecType = Utils.GetAttribute(XmlReader, "Name", "");
					m_DesignRect = new Rectangle(Utils.GetIntValue(XmlReader, "DesignLeft"),
						Utils.GetIntValue(XmlReader, "DesignTop"),
						Utils.GetIntValue(XmlReader, "DesignWidth"),
						Utils.GetIntValue(XmlReader, "DesignHeight"));
				}
				else
				{
					m_Desc = Utils.GetAttribute(XmlReader, "Description", "");
					m_RecType = Utils.GetAttribute(XmlReader, "RecType", "");
					m_DesignRect = Utils.GetRectValue(XmlReader, "DesignRect");
				}

				m_MaxRecs = Utils.GetIntValue(XmlReader, "MaxRecs");
				m_MinRecs = Utils.GetIntValue(XmlReader, "MinRecs");

				bool empty = XmlReader.IsEmptyElement;

				XmlReader.Read();
				XmlReader.MoveToContent();
				IWDEXmlPersist ipers = null;

				if(this.ConvertingOldProject && !empty)
				{
					ipers = (IWDEXmlPersist) m_RecordDefs;
					ipers.ReadFromXml(XmlReader);
					ConvertDescription(XmlReader);

					ipers = (IWDEXmlPersist) m_FieldDefs;
					ipers.ReadFromXml(XmlReader);
				}
				else if(!ConvertingOldProject && !empty)
				{
					ipers = (IWDEXmlPersist) m_FieldDefs;
					ipers.ReadFromXml(XmlReader);

					ipers = (IWDEXmlPersist) m_RecordDefs;
					ipers.ReadFromXml(XmlReader);
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "RecordDef"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("RecordDef");
			if(Description != "")
				XmlWriter.WriteAttributeString("Description", Description);
			if(DesignRect != Rectangle.Empty)
				XmlWriter.WriteAttributeString("DesignRect", Utils.RectToString(m_DesignRect));
			if(RecType != "")
				XmlWriter.WriteAttributeString("RecType", RecType);
			if(MaxRecs != 0)
				XmlWriter.WriteAttributeString("MaxRecs", MaxRecs.ToString());
			if(MinRecs != 0)
				XmlWriter.WriteAttributeString("MinRecs", MinRecs.ToString());

			IWDEXmlPersist ipers = (IWDEXmlPersist) FieldDefs;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist) RecordDefs;
			ipers.WriteToXml(XmlWriter);

			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Methods

		private void ConvertDescription(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
			{
				m_Desc = Utils.GetAttribute(XmlReader, "ENGLISH", "");
				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Descriptions"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
	}

	public class WDEFieldDefs : WDEBaseCollection, IWDEFieldDefs, IWDEXmlPersist
	{
		private bool m_FindOCR;

		private WDEFieldDefs(object Parent) : base(Parent)
		{
		}

		public static IWDEFieldDefs Create(IWDERecordDef Parent)
		{
			return new WDEFieldDefs(Parent) as IWDEFieldDefs;
		}

		public static IWDEFieldDefs CreateInstance(IWDERecordDef Parent)
		{
			return Create(Parent);
		}

		protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			IWDEFieldDef def = (IWDEFieldDef) Item;
			if(m_FindOCR)
				return (string.Compare(def.OCRName, Name, true) == 0);
			else
				return (string.Compare(def.FieldName, Name, true) == 0);
		}

		#region IWDEFieldDefs Members

		public IWDEFieldDef this[int index]
		{
			get
			{
				return (IWDEFieldDef) base.InternalGetIndex(index);
			}
		}

		public int Find(string FieldName)
		{
			m_FindOCR = false;
			return base.InternalIndexOf(FieldName);
		}

		public int FindOCR(string OCRName)
		{
			m_FindOCR = true;
			return base.InternalIndexOf(OCRName);
		}

		public IWDEFieldDef Add(string FieldName)
		{
			int res = base.VerifyName(FieldName);
			if(res == 0)
			{
				IWDEFieldDef newDef = WDEFieldDef.Create();
				base.InternalAdd((WDEBaseCollectionItem) newDef);
				newDef.FieldName = FieldName;
				return newDef;
			}
            else if (res == -1)
                throw new WDEException("API00037", new object[] {FieldName});
            else
                throw new WDEException("API00038", new object[] {FieldName});
 		}

		public IWDEFieldDef Add()
		{
			string newName = GetNextDefaultName("Field");
			IWDEFieldDef newDef = WDEFieldDef.Create();
			base.InternalAdd((WDEBaseCollectionItem) newDef);
			newDef.FieldName = newName;
			return newDef;
		}

		public override string GetNextDefaultName(string nameRoot)
		{
			if (nameRoot == null)
				throw new ArgumentNullException("nameRoot", "nameRoot cannot be null");
			if (nameRoot == "")
				throw new ArgumentException("nameRoot cannot be blank", "nameRoot");

			IWDEDocumentDefs docDefs;
			int suffix = 1;

			if (((WDEBaseCollectionItem)this.Parent).Parent  is IWDEDocumentDef)
				docDefs = (IWDEDocumentDefs)((WDEBaseCollectionItem)((WDEBaseCollectionItem)this.Parent).Parent).Collection;
			else
				docDefs = (IWDEDocumentDefs)((WDEBaseCollectionItem)((WDEBaseCollectionItem)(((WDEBaseCollectionItem)this.Parent).Parent)).Parent).Collection;

			foreach (IWDEDocumentDef doc in docDefs)
				foreach (IWDERecordDef rec in doc.RecordDefs)
					GetHighestSuffix(nameRoot, doc.RecordDefs, ref suffix);

			return nameRoot + suffix.ToString();
		}

		private int GetHighestSuffix(string nameRoot, IWDERecordDefs recDefs, ref int suffix)
		{
			foreach (WDEBaseCollectionItem recdef in recDefs)
			{
				foreach (WDEBaseCollectionItem item in ((WDERecordDef)(recdef)).FieldDefs) 
				{
					string itemName = item.GetNodeName();
					string testName = itemName;
					if (testName.Length > nameRoot.Length)
						testName = testName.Substring(0, nameRoot.Length);

					if (string.Compare(testName, nameRoot, true) == 0)
					{
						if (itemName.Length > testName.Length)
						{
							string end = itemName.Substring(testName.Length);
							int a = 0;
							if (int.TryParse(end, out a))
							{
								if (suffix <= a)
									suffix = a + 1;
							}
						}
					}
				}				
				suffix = GetHighestSuffix(nameRoot, ((WDERecordDef)(recdef)).RecordDefs , ref suffix);				
			}
			return suffix;
		}

        public void Add(IWDEFieldDef def)
        {
            base.InternalAdd((WDEBaseCollectionItem)def);
        }

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "FieldDefs"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();
			}

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "FieldDef"))
			{
				Clear();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "FieldDef"))
				{
					IWDEFieldDef def = WDEFieldDef.Create();
					base.InternalAdd((WDEBaseCollectionItem) def);
					IWDEXmlPersist ipers = (IWDEXmlPersist) def;
					ipers.ReadFromXml(XmlReader);
					base.RegisterObject((WDEBaseCollectionItem) def);
				}
			}

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "FieldDefs"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion
	}

	public class WDEFieldDef : WDEBaseCollectionItem, IWDEFieldDef, IWDEXmlPersist
	{
		private string m_CharSet;
		private int m_DataLen;
		private string m_DataMask;
		private WDEDataType m_DataType;
		private string m_DefaultValue;
		private string m_Desc;
		private int m_OCRAllowedErrors;
		private string m_OCRCharSet;
		private int m_OCRConfidence;
		private WDEOCRRepairMode m_OCRRepairMode;
		private WDEFieldOption m_Options;
		private string m_OCRName;
		private int m_OCRLine;
		private string m_FieldTitle;
		private IWDEEditDefs m_EditDefs;
		private IWDEEventScriptDef m_OnValidate;
		private string m_FieldName;
		private WDEQIOption m_QIOption;

		private WDEFieldDef()
		{
			m_CharSet = "";
			m_DataMask = "";
			m_DataType = WDEDataType.Text;
			m_DefaultValue = "";
			m_Desc = "";
			m_OCRCharSet = "";
			m_OCRRepairMode = WDEOCRRepairMode.None;
			m_Options = WDEFieldOption.None;
			m_QIOption = WDEQIOption.NonCritical;
			m_OCRName = "";
			m_FieldTitle = "";
			m_FieldName = "";
            m_OCRConfidence = 70;
			
			m_EditDefs = WDEEditDefs.Create(this);
			m_OnValidate = WDEEventScriptDef.Create(this, "OnValidate");
		}

		public static IWDEFieldDef Create()
		{
			return new WDEFieldDef() as IWDEFieldDef;
		}

		public static IWDEFieldDef CreateInstance()
		{
			return Create();
		}

		protected override string InternalGetNodeName()
		{
			return m_FieldName;
		}

		#region IWDEFieldDef Members

		public string CharSet
		{
			get
			{
				return m_CharSet;
			}
			set
			{
				if(value == null)
					m_CharSet = "";
				else
					m_CharSet = value;
			}
		}

		public int DataLen
		{
			get
			{
				return m_DataLen;
			}
			set
			{
				m_DataLen = value;
			}
		}

		public string DataMask
		{
			get
			{
				return m_DataMask;
			}
			set
			{
				if(value == null)
					m_DataMask = "";
				else
					m_DataMask = value;
			}
		}

		public WebDX.Api.WDEDataType DataType
		{
			get
			{
				return m_DataType;
			}
			set
			{
				m_DataType = value;
			}
		}

		public string DefaultValue
		{
			get
			{
				return m_DefaultValue;
			}
			set
			{
				if(value == null)
					m_DefaultValue = "";
				else
					m_DefaultValue = value;
			}
		}

		public string Description
		{
			get
			{
				return m_Desc;
			}
			set
			{
				if(value == null)
					m_Desc = "";
				else
					m_Desc = value;
			}
		}

		public int OCRAllowedErrors
		{
			get
			{
				return m_OCRAllowedErrors;
			}
			set
			{
				m_OCRAllowedErrors = value;
			}
		}

		public string OCRCharSet
		{
			get
			{
				return m_OCRCharSet;
			}
			set
			{
				if(value == null)
					m_OCRCharSet = "";
				else
					m_OCRCharSet = value;
			}
		}

		public int OCRConfidence
		{
			get
			{
				return m_OCRConfidence;
			}
			set
			{
				m_OCRConfidence = value;
			}
		}

		public WebDX.Api.WDEOCRRepairMode OCRRepairMode
		{
			get
			{
				return m_OCRRepairMode;
			}
			set
			{
				m_OCRRepairMode = value;
			}
		}

		public WebDX.Api.WDEFieldOption Options
		{
			get
			{
				return m_Options;
			}
			set
			{
				m_Options = value;
			}
		}

		public string OCRName
		{
			get
			{
				return m_OCRName;
			}
			set
			{
				if(value == null)
					m_OCRName = "";
				else
					m_OCRName = value;
			}
		}

		public int OCRLine
		{
			get
			{
				return m_OCRLine;
			}
			set
			{
				m_OCRLine = value;
			}
		}

		public string FieldTitle
		{
			get
			{
				return m_FieldTitle;
			}
			set
			{
				m_FieldTitle = value;
			}
		}

		public IWDEEditDefs EditDefs
		{
			get
			{
				return m_EditDefs;
			}
		}

		public IWDEEventScriptDef OnValidate
		{
			get
			{
				return m_OnValidate;
			}
		}

		public string FieldName
		{
			get
			{
				return m_FieldName;
			}
			set
			{
				string newName = value;
				if(newName == null)
					newName = "";

                if (newName.ToLower() == m_FieldName.ToLower())
                    m_FieldName = newName;
                else
				{
					int res = base.Collection.VerifyName(newName);
					if(res == 0)
						m_FieldName = newName;
                    else if (res == -1)
                         throw new WDEException("API00037", new object[] {newName});
                    else if (res == -2)
                         throw new WDEException("API00038", new object[] {newName});
				}
			}
		}

		public WDEQIOption QIOption
		{
			get
			{
				return m_QIOption;
			}
			set
			{
				m_QIOption = value;
			}
			}

        public IWDEFieldDef Clone()
        {
            return (IWDEFieldDef)this.MemberwiseClone();
		}


		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "FieldDef"))
			{
				m_CharSet = Utils.GetAttribute(XmlReader, "CharSet", "");
				m_DataLen = Utils.GetIntValue(XmlReader, "DataLen");
				m_DataMask = Utils.GetAttribute(XmlReader, "DataMask", "");
				m_DataType = Utils.GetDataType(XmlReader, "DataType");
				m_DefaultValue = Utils.GetAttribute(XmlReader, "DefaultValue", "");
				m_Desc = Utils.GetAttribute(XmlReader, "Description", "");
				m_OCRAllowedErrors = Utils.GetIntValue(XmlReader, "OCRAllowedErrors");
				m_OCRCharSet = Utils.GetAttribute(XmlReader, "OCRCharSet", "");
				m_OCRConfidence = Utils.GetIntValue(XmlReader, "OCRConfidence");
				m_OCRRepairMode = Utils.GetRepairMode(XmlReader, "OCRRepairMode");
				m_Options = Utils.GetFieldOption(XmlReader, "Options");
				m_OCRName = Utils.GetAttribute(XmlReader, "OCRName", "");
				m_OCRLine = Utils.GetIntValue(XmlReader, "OCRLine");
				m_FieldTitle = Utils.GetAttribute(XmlReader, "FieldTitle", "");
				m_QIOption = Utils.GetQIOption(XmlReader, "QIOption");
				
				if(this.ConvertingOldProject)
				{
					m_Options = WDEFieldOption.None;
					if(Utils.GetBoolValue(XmlReader, "AllowFlag", false))
						m_Options |= WDEFieldOption.AllowFlag;
					if(Utils.GetBoolValue(XmlReader, "MustComplete", false))
						m_Options |= WDEFieldOption.MustComplete;                    
                    if (Utils.GetBoolValue(XmlReader, "MustEnter", false))
                        m_Options |= WDEFieldOption.MustEnter;                    
					if(Utils.GetBoolValue(XmlReader, "UpperCase", false))
						m_Options |= WDEFieldOption.UpperCase;
					if(Utils.GetBoolValue(XmlReader, "NumericShift", false))
						m_Options |= WDEFieldOption.NumericShift;

					m_FieldName = Utils.GetAttribute(XmlReader, "Name", "");
				}
				else
					m_FieldName = Utils.GetAttribute(XmlReader, "FieldName", "");


				XmlReader.Read();
				XmlReader.MoveToContent();

				if(this.ConvertingOldProject)
				{
					ConvertOldField(XmlReader);
				}
				else
				{
					IWDEXmlPersist ipers = (IWDEXmlPersist) m_EditDefs;
					ipers.ReadFromXml(XmlReader);

					ipers = (IWDEXmlPersist) m_OnValidate;
					ipers.ReadFromXml(XmlReader);
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "FieldDef"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("FieldDef");
			if(CharSet != "")
				XmlWriter.WriteAttributeString("CharSet", CharSet);
			if(DataLen != 0)
				XmlWriter.WriteAttributeString("DataLen", DataLen.ToString());
			if(DataMask != "")
				XmlWriter.WriteAttributeString("DataMask", DataMask);
			//if(DataType != WDEDataType.Text)
			//	XmlWriter.WriteAttributeString("DataType", DataType.ToString());
            if (!VersionHelper.FilterPropertyOrCollection("IWDEField.DataType", VersionInfo.TargetVersionNumber))
            {
                XmlWriter.WriteAttributeString("DataType", "dtText");
            }
			if(DefaultValue != "")
				XmlWriter.WriteAttributeString("DefaultValue", DefaultValue);
			if(Description != "")
				XmlWriter.WriteAttributeString("Description", Description);
			if(OCRAllowedErrors != 0)
				XmlWriter.WriteAttributeString("OCRAllowedErrors", OCRAllowedErrors.ToString());
			if(OCRCharSet != "")
				XmlWriter.WriteAttributeString("OCRCharSet", OCRCharSet);
			if(OCRConfidence != 0)
				XmlWriter.WriteAttributeString("OCRConfidence", OCRConfidence.ToString());
			if(OCRRepairMode != WDEOCRRepairMode.None)
				XmlWriter.WriteAttributeString("OCRRepairMode", OCRRepairMode.ToString());
			if(Options != WDEFieldOption.None)
				XmlWriter.WriteAttributeString("Options", Options.ToString());
			if(OCRName != "")
				XmlWriter.WriteAttributeString("OCRName", OCRName);
			if(OCRLine != 0)
				XmlWriter.WriteAttributeString("OCRLine", OCRLine.ToString());
			if(FieldTitle != "")
				XmlWriter.WriteAttributeString("FieldTitle", FieldTitle);
			if(FieldName != "")
				XmlWriter.WriteAttributeString("FieldName", FieldName);
			if(QIOption != WDEQIOption.NonCritical)
				XmlWriter.WriteAttributeString("QIOption", QIOption.ToString());

			IWDEXmlPersist ipers = (IWDEXmlPersist) m_EditDefs;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist) m_OnValidate;
			ipers.WriteToXml(XmlWriter);

			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Methods

		private void ConvertOldField(XmlTextReader XmlReader)
		{
			ConvertDescription(XmlReader);
			ConvertAccumulatorCalc(XmlReader);
			ConvertCheckDigit(XmlReader);
			ConvertRange(XmlReader);
			ConvertRequired(XmlReader);
			ConvertSimpleList(XmlReader);

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Titles"))
			{
				m_FieldTitle = Utils.GetAttribute(XmlReader, "ENGLISH", "");
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}

			ConvertValidLengths(XmlReader);

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Scripts"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				IWDEXmlPersist ipers = (IWDEXmlPersist) m_OnValidate;
				ipers.ReadFromXml(XmlReader);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Scripts"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ConvertDescription(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
			{
				m_Desc = Utils.GetAttribute(XmlReader, "ENGLISH", "");
				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Descriptions"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ConvertAccumulatorCalc(XmlTextReader XmlReader)
		{
			while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "AccumulatorCalc"))
			{
				bool enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
				string temp = Utils.GetAttribute(XmlReader, "Accumulator", "");
				if((enabled) && (temp != ""))
				{
					IWDEProjectInternal iproj = this.GetProjectInternal();
					iproj.Resolver.AddMethodRequest(temp + ".SumFields", "Add", new object[] {this});
				}

				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		private void ConvertCheckDigit(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "CheckDigit"))
			{
				IWDECheckDigitEditDef def = WDECheckDigitEditDef.Create();
				m_EditDefs.Add(def);
				def.Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
				def.ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
				def.SessionMode = Utils.GetSessionType(XmlReader, "ViewModes");
				def.Methods = Utils.GetCheckDigitMethods(XmlReader, "Methods");

				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
				{
					def.Description = Utils.GetAttribute(XmlReader, "ENGLISH", "");
					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ErrorMessages"))
				{
					def.ErrorMessage = Utils.GetAttribute(XmlReader, "ENGLISH", "");
					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "CheckDigit"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ConvertRange(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Range"))
			{
				IWDERangeEditDef def = WDERangeEditDef.Create();
				m_EditDefs.Add(def);
				def.Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
				def.ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
				def.SessionMode = Utils.GetSessionType(XmlReader, "ViewModes");
				def.HighRange = Utils.GetAttribute(XmlReader, "HighRange", "");
				def.LowRange = Utils.GetAttribute(XmlReader, "LowRange", "");
				
				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
				{
					def.Description = Utils.GetAttribute(XmlReader, "ENGLISH", "");
					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ErrorMessages"))
				{
					def.ErrorMessage = Utils.GetAttribute(XmlReader, "ENGLISH", "");
					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Range"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ConvertRequired(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Required"))
			{
				IWDERequiredEditDef def = WDERequiredEditDef.Create();
				m_EditDefs.Add(def);
				def.Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
				def.ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
				def.SessionMode = Utils.GetSessionType(XmlReader, "ViewModes");
				string temp = Utils.GetAttribute(XmlReader, "Expression", "");
				if(temp != "")
				{
					IWDEProjectInternal iproj = GetProjectInternal();
                    if (iproj == null)
                        throw new WDEException("API90006", new object[] {"FieldDef"});

					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) def;
					iproj.AppendOldExpression(temp, obj.GetNamePath());
					def.Expression = obj.GetNamePath();
				}
				
				
				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
				{
					def.Description = Utils.GetAttribute(XmlReader, "ENGLISH", "");
					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ErrorMessages"))
				{
					def.ErrorMessage = Utils.GetAttribute(XmlReader, "ENGLISH", "");
					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Required"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ConvertSimpleList(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "SimpleList"))
			{
				IWDESimpleListEditDef def = WDESimpleListEditDef.Create();
				m_EditDefs.Add(def);
				def.Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
				def.ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
				def.SessionMode = Utils.GetSessionType(XmlReader, "ViewModes");
				
				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
				{
					def.Description = Utils.GetAttribute(XmlReader, "ENGLISH", "");
					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ErrorMessages"))
				{
					def.ErrorMessage = Utils.GetAttribute(XmlReader, "ENGLISH", "");
					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "List"))
				{
					XmlReader.Read();
					XmlReader.MoveToContent();

					ArrayList list = new ArrayList();

					while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
						(XmlReader.Name == "String"))
					{
						list.Add(Utils.GetAttribute(XmlReader, "Value", ""));

						XmlReader.ReadInnerXml();
						XmlReader.MoveToContent();
					}

					string[] newList = new string[list.Count];
					list.CopyTo(newList);
					def.List = newList;

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "List"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "SimpleList"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ConvertValidLengths(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ValidLengths"))
			{
				IWDEValidLengthsEditDef def = WDEValidLengthsEditDef.Create();
				m_EditDefs.Add(def);
				def.Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
				def.ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
				def.SessionMode = Utils.GetSessionType(XmlReader, "ViewModes");
				
				ArrayList al = new ArrayList();
				string temp = Utils.GetAttribute(XmlReader, "Lengths", "");
				string[] lengths = temp.Split(',');
				foreach(string len in lengths)
				{
					try
					{
						al.Add(int.Parse(len.Trim()));
					}
					catch(FormatException ex)
					{
						throw new XmlException("Invalid length element", ex, XmlReader.LineNumber, XmlReader.LinePosition);
					}
				}

				int[] l = new int[al.Count];
				al.CopyTo(l);
				def.Lengths = l;

				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
				{
					def.Description = Utils.GetAttribute(XmlReader, "ENGLISH", "");
					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ErrorMessages"))
				{
					def.ErrorMessage = Utils.GetAttribute(XmlReader, "ENGLISH", "");
					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ValidLengths"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
	}

	public class WDEFieldDefLinks : WDEBaseCollection, IWDEFieldDefLinks, IWDEXmlPersist
	{
		private WDEFieldDefLinks(object Parent) : base(Parent)
		{
		}

		public static IWDEFieldDefLinks Create(object Parent)
		{
			return new WDEFieldDefLinks(Parent) as IWDEFieldDefLinks;
		}

		public static IWDEFieldDefLinks CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			base.Remove(LinkedItem);
		}

		#region IWDEFieldDefLinks Members

		public IWDEFieldDef this[int index]
		{
			get
			{
				return (IWDEFieldDef) base.InternalGetIndex(index);
			}
			set
			{
				while(Count <= index)
					base.InternalAdd(null);

				WDEBaseCollectionItem oldItem = base.InternalGetIndex(index);
				if(oldItem != null)
					oldItem.RemoveLink(this);

				base.InternalSetIndex(index, (WDEBaseCollectionItem) value, false);

				oldItem = (WDEBaseCollectionItem) value;
				oldItem.AddLink(this);
			}
		}

		public void Add(IWDEFieldDef FieldDef)
		{
			WDEBaseCollectionItem def = (WDEBaseCollectionItem) FieldDef;
			def.AddLink(this);
			base.InternalAdd(def, false);
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			IWDEProjectInternal proj = null;
            if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                proj = (IWDEProjectInternal)base.TopParent();
            else
                throw new WDEException("API90006", new object[] {"FieldDefLink"});

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "FieldDefLink"))
			{
				Clear();

				int index = 0;
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "FieldDefLink"))
				{
					proj.Resolver.AddRequest(this, "Item", Utils.GetAttribute(XmlReader, "Name", ""), index++);

					XmlReader.Read();
					XmlReader.MoveToContent();

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "FieldDefLink"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				XmlWriter.WriteStartElement("FieldDefLink");
				WDEBaseCollectionItem obj = (WDEBaseCollectionItem) this[i];
				XmlWriter.WriteAttributeString("Name", obj.GetNamePath());
				XmlWriter.WriteEndElement();
			}
		}

		#endregion
	}

	public class WDEFormDefs : WDEBaseCollection, IWDEFormDefs, IWDEXmlPersist
	{
		private WDEFormDefs(object Parent) : base(Parent)
		{
		}

		public static IWDEFormDefs Create(IWDEDocumentDef Parent)
		{
			return new WDEFormDefs(Parent) as IWDEFormDefs;
		}

		public static IWDEFormDefs CreateInstance(IWDEDocumentDef Parent)
		{
			return Create(Parent);
		}

		protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			IWDEFormDef def = (IWDEFormDef) Item;
			return (string.Compare(def.FormName, Name, true) == 0);
		}

        // We should only care about uniqueness within the same document. I.E. within this collection.
        //protected override ArrayList GetCollectionList()
        //{
        //    return base.GetSameLevelCollections();
        //}

		#region IWDEFormDefs Members

		public IWDEFormDef this[int index]
		{
			get
			{
				return (IWDEFormDef) base.InternalGetIndex(index);
			}
		}

		public int Find(string FormName)
		{
			return base.InternalIndexOf(FormName);
		}

		public IWDEFormDef Add(string FormName)
		{
            int res = base.VerifyName(FormName);
            if(res == 0)
			{
                IWDEFormDef newDef = WDEFormDef.Create();
				base.InternalAdd((WDEBaseCollectionItem) newDef);				
                newDef.FormName = FormName;
				return newDef;
			}
            else if (res == -1)
                 throw new WDEException("API00037", new object[] {FormName});
            else
                 throw new WDEException("API00038", new object[] {FormName});
		}

		public IWDEFormDef Add()
		{
			string newName = GetNextDefaultName("Form");
            IWDEFormDef newDef = WDEFormDef.Create();
			base.InternalAdd((WDEBaseCollectionItem) newDef);
			newDef.FormName = newName;
			return newDef;
		}

        public override string GetNextDefaultName(string nameRoot)
        {
            if (nameRoot == null)
                throw new ArgumentNullException("nameRoot", "nameRoot cannot be null");
            if (nameRoot == "")
                throw new ArgumentException("nameRoot cannot be blank", "nameRoot");

            IWDEDocumentDefs docDefs = null;
            int suffix = 1;            

            if ((WDEBaseCollectionItem)this.Parent is IWDEDocumentDef)
                docDefs = (IWDEDocumentDefs)((WDEBaseCollectionItem)(WDEBaseCollectionItem)this.Parent).Collection;            

            if (docDefs != null)
            {
                foreach (IWDEDocumentDef doc in docDefs)
                    foreach (IWDEFormDef frm in doc.FormDefs)
                        GetHighestSuffix(nameRoot, doc.FormDefs, ref suffix);
            }

            return nameRoot + suffix.ToString();
        }

        private int GetHighestSuffix(string nameRoot, IWDEFormDefs formDefs, ref int suffix)
        {
            foreach (WDEBaseCollectionItem formDef in formDefs)
            {
                string formName = formDef.GetNodeName();
                string actualName = formName;
                if (actualName.Length > nameRoot.Length)
                    actualName = actualName.Substring(0, nameRoot.Length);

                if (string.Compare(actualName, nameRoot, true) == 0)
                {
                    if (formName.Length > actualName.Length)
                    {
                        string currFormID = formName.Substring(actualName.Length);
                        int formID = 0;
                        if (int.TryParse(currFormID, out formID))
                        {
                            if (suffix <= formID)
                                suffix = formID + 1;
                        }
                    }
                }                
            }
            return suffix;
        }

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "FormDefs"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();
			}

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "FormDef"))
			{
				Clear();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "FormDef"))
				{
					IWDEFormDef def = WDEFormDef.Create();
					base.InternalAdd((WDEBaseCollectionItem) def);
					IWDEXmlPersist ipers = (IWDEXmlPersist) def;
					ipers.ReadFromXml(XmlReader);
					base.RegisterObject((WDEBaseCollectionItem) def);
				}
			}

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "FormDefs"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion
	}

	internal class TempZone
	{
		public string Name;
		public Rectangle ZoneRect;
	}

	public class WDEFormDef : WDEBaseCollectionItem, IWDEFormDef_R1, IWDEXmlPersist
	{
		private Color m_BackColor;
		private Color m_ForeColor;
		private IWDEControlDefs m_ControlDefs;
		private IWDERecordDef m_RecordDef;
		private string m_Desc;
		private string m_FormFont;
		private string m_Help;
		private string m_Hint;
		private bool m_UseSnippets;
		private string m_FormName;
		private IWDEImageSourceDef m_DefaultImage;
        private IWDEEventScriptDef m_OnEnter;
        private IWDEEventScriptDef m_OnExit;

		private WDEFormDef()
		{
			m_BackColor = Color.FromKnownColor(KnownColor.Control);
			m_ForeColor = Color.FromKnownColor(KnownColor.WindowText);
			m_Desc = "";
			m_Help = "";
			m_Hint = "";
			m_FormName = "";
            m_OnEnter = WDEEventScriptDef.Create(this, "OnEnter");
            m_OnExit = WDEEventScriptDef.Create(this, "OnExit");
			
			m_ControlDefs = WDEControlDefs.Create(this);
            m_FormFont = "Tahoma, 10pt";
		}

		public static IWDEFormDef Create()
		{
			return new WDEFormDef() as IWDEFormDef;
		}

		public static IWDEFormDef CreateInstance()
		{
			return Create();
		}

		protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			if(LinkedItem == m_RecordDef)
				m_RecordDef = null;
			if(LinkedItem == m_DefaultImage)
				m_DefaultImage = null;
		}

		protected override string InternalGetNodeName()
		{
			return m_FormName;
		}

		protected override void InternalClearNotify()
		{
			m_ControlDefs.Clear();
		}

		public override ArrayList GetChildCollections()
		{
			ArrayList al = new ArrayList();
			al.Add(m_ControlDefs);
			return al;
		}

		#region IWDEFormDef Members

		public Color BackColor
		{
			get
			{
				return m_BackColor;
			}
			set
			{
				m_BackColor = value;
			}
		}

		public Color ForeColor
		{
			get
			{
				return m_ForeColor;
			}
			set
			{
				m_ForeColor = value;
			}
		}

		public IWDEControlDefs ControlDefs
		{
			get
			{
				return m_ControlDefs;
			}
		}

		public IWDERecordDef RecordDef
		{
			get
			{
				return m_RecordDef;
			}
			set
			{
				WDEBaseCollectionItem def = (WDEBaseCollectionItem) m_RecordDef;
				if(def != null)
					def.RemoveLink(this);

				m_RecordDef = value;

                if (value != null)
                {
                    def = (WDEBaseCollectionItem)m_RecordDef;
                    def.AddLink(this);
                }
			}
		}

		public string Description
		{
			get
			{
				return m_Desc;
			}
			set
			{
				if(value == null)
					m_Desc = "";
				else
					m_Desc = value;
			}
		}

		public Font FormFont
		{
			get
			{
				return Utils.StringToFont(m_FormFont);
			}
			set
			{
				m_FormFont = Utils.FontToString(value);
			}
		}

		public string Help
		{
			get
			{
				return m_Help;
			}
			set
			{
				if(value == null)
					m_Help = "";
				else
					m_Help = value;
			}
		}

		public string Hint
		{
			get
			{
				return m_Hint;
			}
			set
			{
				if(value == null)
					m_Hint = "";
				else
					m_Hint = value;
			}
		}

		public bool UseSnippets
		{
			get
			{
				return m_UseSnippets;
			}
			set
			{
				m_UseSnippets = value;
			}
		}

		public string FormName
		{
			get
			{
				return m_FormName;
			}
			set
			{
				string newName = value;
				if(newName == null)
					newName = "";

				if(newName != m_FormName)
				{
					int res = base.Collection.VerifyName(newName);
					if(res == 0)
						m_FormName = newName;
                    else if (res == -1)
                         throw new WDEException("API00037", new object[] {newName});
                    else if (res == -2)
                         throw new WDEException("API00038", new object[] {newName});
				}
			}
		}

		public IWDEImageSourceDef DefaultImage
		{
			get
			{
				return m_DefaultImage;
			}
			set
			{
				WDEBaseCollectionItem def = (WDEBaseCollectionItem) m_DefaultImage;
				if(def != null)
					def.RemoveLink(this);

				m_DefaultImage = value;

				def = (WDEBaseCollectionItem) m_DefaultImage;
				def.AddLink(this);
			}
		}

		#endregion

        #region IWDEFormDef_R1 Members

        public IWDEEventScriptDef OnEnter
        {
            get 
            { 
                return m_OnEnter; 
            }
            set
            {
                m_OnEnter = value;
            }
        }

        public IWDEEventScriptDef OnExit
        {
            get 
            { 
                return m_OnExit; 
            }
            set
            {
                m_OnExit = value;
            }
        }

        #endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			IWDEProjectInternal proj = null;
            if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                proj = (IWDEProjectInternal)base.TopParent();
            else
               throw new WDEException("API90006", new object[] {"FormDef"});
            

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "FormDef"))
			{
				if(ConvertingOldProject)
				{
					ConvertOldForm(XmlReader, proj);
				}
				else
				{
					m_BackColor = Utils.GetColor(XmlReader, "BackColor", Color.FromKnownColor(KnownColor.Control));
					m_ForeColor = Utils.GetColor(XmlReader, "ForeColor", Color.FromKnownColor(KnownColor.WindowText));
					string temp = Utils.GetAttribute(XmlReader, "Record", "");
					if(temp != "")
						proj.Resolver.AddRequest(this, "RecordDef", temp);

					temp = Utils.GetAttribute(XmlReader, "DefaultImage", "");
					if(temp != "")
						proj.Resolver.AddRequest(this, "DefaultImage", temp);

					m_Desc = Utils.GetAttribute(XmlReader, "Description", "");
					m_FormFont = Utils.GetAttribute(XmlReader, "Font", "Tahoma,10,Regular");
					m_Help = Utils.GetAttribute(XmlReader, "Help", "");
					m_Hint = Utils.GetAttribute(XmlReader, "Hint", "");
					m_UseSnippets = Utils.GetBoolValue(XmlReader, "UseSnippets", false);
					m_FormName = EnsureUniqueName(Utils.GetAttribute(XmlReader, "FormName", ""));

					XmlReader.Read();
					XmlReader.MoveToContent();

					IWDEXmlPersist ipers = (IWDEXmlPersist) ControlDefs;
					ipers.ReadFromXml(XmlReader);

                    ipers = (IWDEXmlPersist)m_OnEnter;
                    ipers.ReadFromXml(XmlReader);

                    ipers = (IWDEXmlPersist)m_OnExit;
                    ipers.ReadFromXml(XmlReader);

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "FormDef"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("FormDef");
			if(BackColor != Color.FromKnownColor(KnownColor.Control))
				XmlWriter.WriteAttributeString("BackColor", BackColor.Name);
			if(ForeColor != Color.FromKnownColor(KnownColor.WindowText))
				XmlWriter.WriteAttributeString("ForeColor", ForeColor.Name);
			if(RecordDef != null)
			{
				WDEBaseCollectionItem obj = (WDEBaseCollectionItem) RecordDef;
				XmlWriter.WriteAttributeString("Record", obj.GetNamePath());
			}

			if(DefaultImage != null)
			{
				WDEBaseCollectionItem obj = (WDEBaseCollectionItem) DefaultImage;
				XmlWriter.WriteAttributeString("DefaultImage", obj.GetNamePath());
			}
			
			if(Description != "")
				XmlWriter.WriteAttributeString("Description", Description);
			XmlWriter.WriteAttributeString("Font", m_FormFont);
			if(Help != "")
				XmlWriter.WriteAttributeString("Help", Help);
			if(Hint != "")
				XmlWriter.WriteAttributeString("Hint", Hint);
			if(UseSnippets)
				XmlWriter.WriteAttributeString("UseSnippets", UseSnippets.ToString());
			if(FormName != "")
				XmlWriter.WriteAttributeString("FormName", FormName.ToString());

			IWDEXmlPersist ipers = (IWDEXmlPersist) ControlDefs;
			ipers.WriteToXml(XmlWriter);

            ipers = (IWDEXmlPersist)m_OnEnter;
            ipers.WriteToXml(XmlWriter);

            ipers = (IWDEXmlPersist)m_OnExit;
            ipers.WriteToXml(XmlWriter);

			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Members

		private void ConvertOldForm(XmlTextReader XmlReader, IWDEProjectInternal iproj)
		{
			m_FormName = EnsureUniqueName(Utils.GetAttribute(XmlReader, "Name", ""));
			string temp = Utils.GetAttribute(XmlReader, "DataRecord", "");
			if(temp != "")
				iproj.Resolver.AddRequest(this, "RecordDef", temp);

			m_UseSnippets = Utils.GetBoolValue(XmlReader, "UseSnippets", false);

			XmlReader.Read();
			XmlReader.MoveToContent();

			ReadDescription(XmlReader);
			ReadColor(XmlReader);

			IWDEXmlPersist ipers = (IWDEXmlPersist) m_ControlDefs;
			ipers.ReadFromXml(XmlReader);

			ReadFont(XmlReader);
			ReadHelp(XmlReader);
			ReadHint(XmlReader);
			ReadImageLinks(XmlReader);

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Scripts"))
			{
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "FormDef"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		private void ReadDescription(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
			{
				m_Desc = Utils.GetAttribute(XmlReader, "ENGLISH", "");
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		private void ReadColor(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Colors"))
			{
				m_BackColor = Utils.GetColor(XmlReader, "ENGLISH", Color.FromKnownColor(KnownColor.Control));
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		private void ReadFont(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Fonts"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();
				
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "Font"))
				{
					if(Utils.GetAttribute(XmlReader, "Name", "") == "ENGLISH")
					{
						string name = Utils.GetAttribute(XmlReader, "FontName", "");
						float height = Utils.ConvertFontHeight(Utils.GetIntValue(XmlReader, "Height"));
						Utils.ConvertBitmapFont(ref name, ref height);
						FontStyle style = Utils.ConvertFontStyle(Utils.GetAttribute(XmlReader, "Style", ""), XmlReader.LineNumber, XmlReader.LinePosition);

						using(Font temp = new Font(name, height, style))
                            m_FormFont = Utils.FontToString(temp);
					}

					XmlReader.Read();
					XmlReader.MoveToContent();
					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Font"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Fonts"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ReadHelp(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Helps"))
			{
				m_Help = Utils.GetAttribute(XmlReader, "ENGLISH", "");
				
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		private void ReadHint(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Hints"))
			{
				m_Hint = Utils.GetAttribute(XmlReader, "ENGLISH", "");

				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		private void ReadImageLinks(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ImageSourceLinks"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				IWDEProjectInternal iproj = GetProjectInternal();
                if (iproj == null)
                    throw new WDEException("API90006", new object[] {"FormDef"});

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "ImageSourceLink"))
				{
					string imageName = Utils.GetAttribute(XmlReader, "ImageSource", "");
					string linkName = Utils.GetAttribute(XmlReader, "Name", "");

					if(linkName == "")
                        throw new XmlException("Invalid ImageLink name", null, XmlReader.LineNumber, XmlReader.LinePosition);

					linkName = this.GetNamePath() + "." + linkName;
					
					IWDEDocumentDef doc = null;
                    if (Parent is IWDEDocumentDef)
                        doc = (IWDEDocumentDef)Parent;
                    else
                        throw new WDEException("API00031", new object[] { "IWDEDocumentDef", "IWDEFormDef" });

					string[] nameParse = imageName.Split('.');
					if(nameParse.Length != 2)
						throw new XmlException("Invalid ImageSource attribute", null, XmlReader.LineNumber, XmlReader.LinePosition);					

					iproj.Resolver.AddConvertName(linkName, imageName);

					IWDEImageSourceDef def = null;
					int i = 0;
					if((i = doc.ImageSourceDefs.Find(nameParse[1])) == -1)
						def = doc.ImageSourceDefs.Add(nameParse[1]);
					else
						def = doc.ImageSourceDefs[i];

					if(DefaultImage == null)
						DefaultImage = def;

					XmlReader.Read();
					XmlReader.MoveToContent();

					ReadZones(XmlReader, def, linkName, imageName);

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ImageSourceLink"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ImageSourceLinks"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ReadZones(XmlTextReader XmlReader, IWDEImageSourceDef def, string linkName, string imageName)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ZoneDefs"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				IWDEProjectInternal iproj = GetProjectInternal();
                if (iproj == null)
                     throw new WDEException("API90006", new object[] {"FormDef"});

				ArrayList zones = new ArrayList();
				
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "Zone"))
				{					
					string zoneType = Utils.GetAttribute(XmlReader, "ZoneType", "");
					string Name = FormName + "_" + Utils.GetAttribute(XmlReader, "Name", "");
					int l = Utils.GetIntValue(XmlReader, "Left");
					int t = Utils.GetIntValue(XmlReader, "Top");
					int w = Utils.GetIntValue(XmlReader, "Width");
					int h = Utils.GetIntValue(XmlReader, "Height");
					Rectangle zoneRect = new Rectangle(l, t, w, h);

					switch(zoneType)
					{
						case "ztZone":
							TempZone zoneDef = new TempZone();
							zones.Add(zoneDef);
							zoneDef.Name = Name;
							zoneDef.ZoneRect = zoneRect;
							XmlReader.ReadInnerXml();
							XmlReader.MoveToContent();
							break;
						case "ztDetail":
							IWDEDetailZoneDef zone = def.DetailZoneDefs.Add(Name);
							WDEBaseCollectionItem obj = (WDEBaseCollectionItem) zone;
							iproj.Resolver.AddObject(obj.GetNamePath(), zone);

							int lineCount = Utils.GetIntValue(XmlReader, "LineCount");
							if(lineCount != 0)
								zone.LineHeight = h / lineCount;
							zone.LineCount = lineCount;
							zoneRect.Height = zone.LineHeight;
							((IWDEDetailZoneDefInternal) zone).ZoneRect = zoneRect;
							XmlReader.Read();
							XmlReader.MoveToContent();
							ReadDetailSubZones(XmlReader, zone, iproj, linkName, imageName);

							if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Zone"))
							{
								XmlReader.ReadEndElement();
								XmlReader.MoveToContent();
							}
							break;
						case "ztSnippet":
							IWDESnippetDef snippetDef = def.SnippetDefs.Add(Name);
							obj = (WDEBaseCollectionItem) snippetDef;
							iproj.Resolver.AddObject(obj.GetNamePath(), snippetDef);

							snippetDef.SnippetRect = zoneRect;
							int x = Utils.GetIntValue(XmlReader, "X");
							int y = Utils.GetIntValue(XmlReader, "Y");
							snippetDef.Location = new Point(x, y);
							XmlReader.ReadInnerXml();
							XmlReader.MoveToContent();
							break;
						default:
							throw new XmlException("Invalid ZoneType attribute", null, XmlReader.LineNumber, XmlReader.LinePosition);
					}
				}

				//TODO: Duplicate Zone names
				foreach(TempZone zone in zones)
				{
					Rectangle tempRect = zone.ZoneRect;
					IWDEZoneDef newZone = def.ZoneDefs.Add(zone.Name);
					newZone.ZoneRect = tempRect;
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) newZone;
					iproj.Resolver.AddObject(obj.GetNamePath(), newZone);
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ZoneDefs"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ReadDetailSubZones(XmlTextReader XmlReader, IWDEDetailZoneDef Zone, IWDEProjectInternal iproj, string linkName, string imageName)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ZoneDefs"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				WDEBaseCollectionItem z = (WDEBaseCollectionItem) Zone;
				string namePath = z.GetNamePath();
				string temp = namePath.Substring(imageName.Length);
				iproj.Resolver.AddConvertName(linkName + temp, namePath);
				Rectangle detailRect = ((IWDEDetailZoneDefInternal) Zone).ZoneRect;

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "Zone"))
				{					
					string zoneType = Utils.GetAttribute(XmlReader, "ZoneType", "");
                    string[] zn = Zone.Name.Split('_');
					string Name = FormName + "_" + Utils.GetAttribute(XmlReader, "Name", "");
					int l = Utils.GetIntValue(XmlReader, "Left") + detailRect.Left;
					int t = Utils.GetIntValue(XmlReader, "Top") + detailRect.Top;
					int w = Utils.GetIntValue(XmlReader, "Width");
					int h = Utils.GetIntValue(XmlReader, "Height");
					Rectangle zoneRect = new Rectangle(l, t, w, h);

					switch(zoneType)
					{
						case "ztZone":
						case "ztSnippet":
							IWDEZoneDef def = Zone.ZoneDefs.Add(Name);
							def.ZoneRect = zoneRect;
							WDEBaseCollectionItem obj = (WDEBaseCollectionItem) def;
							iproj.Resolver.AddObject(obj.GetNamePath(), def);
							break;
						case "ztDetail":
							throw new XmlException("Detail zone within detail zone is not allowed", null, XmlReader.LineNumber, XmlReader.LinePosition);
						default:
							throw new XmlException("Invalid ZoneType attribute", null, XmlReader.LineNumber, XmlReader.LinePosition);
					}

					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if(Zone.ZoneDefs.Count == 0)
				{
					IWDEZoneDef defaultDef = Zone.ZoneDefs.Add("DefaultDetailZone");
					defaultDef.ZoneRect = new Rectangle(detailRect.Left, detailRect.Top, detailRect.Width, Zone.LineHeight);
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ZoneDefs"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private string EnsureUniqueName(string formName)
		{
			if(Collection.NameExists(formName))
			{
				string newName = Collection.RepairNameCollsion(formName);
				string baseName = GetNamePath();
				IWDEProjectInternal pi = GetProjectInternal();
				pi.Resolver.AddFormNameConversion(baseName + formName, baseName + newName);
				return newName;
			}
			else
				return formName;
		}

		#endregion
	}

	public class WDEControlDefs : WDEBaseCollection, IWDEControlDefs, IWDEXmlPersist, IWDEControlDefsInternal
	{
		private KeyOrderList m_KeyOrderList;

		private WDEControlDefs(object Parent) : base(Parent)
		{
		}

		public static IWDEControlDefs Create(object Parent)
		{
			return new WDEControlDefs(Parent) as IWDEControlDefs;
		}

		public static IWDEControlDefs CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			IWDEControlDef def = (IWDEControlDef) Item;
			return (string.Compare(def.ControlName, Name, true) == 0);
		}

        public int GetHighestSuffix(string nameRoot)
        {
            return GetHighestSuffix(nameRoot, true);
        }

		public int GetHighestSuffix(string nameRoot, bool examineForm)
		{
			int suffix;
            if (nameRoot.StartsWith("TB_"))
                suffix = 0; 
            else
                suffix = 1;

            IWDEControlDefs controlDefs = null;

            if (examineForm)
            {
                IWDEFormDef form = GetContainingForm();
                controlDefs = form.ControlDefs;
            }
            else
            {
                controlDefs = this;
            }

			foreach(WDEBaseCollectionItem item in controlDefs)
			{
                string itemName = item.GetNodeName();
                string testName = itemName;
				if(testName.Length > nameRoot.Length)
					testName = testName.Substring(0, nameRoot.Length);

                if (string.Compare(testName, nameRoot, true) == 0)
                {
                    if (itemName.Length > testName.Length)
                    {
                        string end = itemName.Substring(testName.Length);
                        int a = 0;
                        if (int.TryParse(end, out a))
                        {
                            if (suffix <= a)
                                suffix = a + 1;
                        }
                    }
                }
				
				if((item is IWDEDetailGridDef) && (examineForm))
				{
					IWDEControlDefsInternal idefs = (IWDEControlDefsInternal) ((IWDEDetailGridDef) item).HeaderControlDefs;
                    int highestHeader = idefs.GetHighestSuffix(nameRoot, false);
                    if (suffix <= highestHeader)
                        suffix = highestHeader + 1;

					idefs = (IWDEControlDefsInternal) ((IWDEDetailGridDef) item).ControlDefs;
                    int highestDetail = idefs.GetHighestSuffix(nameRoot, false);
                    if (suffix <= highestDetail)
                        suffix = highestDetail + 1;
				}
			}

			return suffix;
		}

		public override string GetNextDefaultName(string nameRoot)
		{
			if(nameRoot == null)
				throw new ArgumentNullException("nameRoot","nameRoot cannot be null");
			if(nameRoot == "")
				throw new ArgumentException("nameRoot cannot be blank","nameRoot");

			int suffix = GetHighestSuffix(nameRoot);
			return nameRoot + (suffix == 0 ? string.Empty : suffix.ToString()); 
		}

		public override bool NameExists(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name","name cannot be null");
			if(name == "")
				throw new ArgumentException("name cannot be blank","name");

			IWDEFormDef form = GetContainingForm();
			if(CheckChildNames((WDEBaseCollection) form.ControlDefs, name))
				return true;
			else
				return false;
		}

		public override string RepairNameCollsion(string duplicateName)
		{
			if(duplicateName == null)
				throw new ArgumentNullException("duplicateName","duplicateName cannot be null");
			if(duplicateName == "")
				throw new ArgumentException("duplicateName cannot be blank","duplicateName");

			duplicateName += "_";
			int suffix = this.GetHighestSuffix(duplicateName);

			return duplicateName + suffix.ToString();
		}

		#region IWDEControlDefs Members

		public IWDEControlDef this[int index]
		{
			get
			{
				return (IWDEControlDef) base.InternalGetIndex(index);
			}
		}

		public int Find(string ControlName)
		{
			return base.InternalIndexOf(ControlName);
		}

		public void Add(IWDEControlDef def)
		{
			base.InternalAdd((WDEBaseCollectionItem) def);
		}

		public IWDELabelDef AddLabel(string Name)
		{
			int res = this.VerifyName(Name);
			if(res == 0)
			{
				IWDELabelDef def = WDELabelDef.Create();
				Add(def);
				def.ControlName = Name;
				return def;
			}
            else if (res == -1)
                 throw new WDEException("API00037", new object[] {Name});
            else
                 throw new WDEException("API00038", new object[] {Name});	
 		}

		public IWDELabelDef AddLabel()
		{
			string newName = GetNextDefaultName("Label");
			IWDELabelDef def = WDELabelDef.Create();
			Add(def);
			def.ControlName = newName;
			return def;
		}

		public IWDETextBoxDef AddTextBox(string Name)
		{
			int res = this.VerifyName(Name);
			if(res == 0)
			{
				IWDETextBoxDef def = WDETextBoxDef.Create();
				Add(def);
				def.ControlName = Name;
				return def;
			}
            else if (res == -1)
                 throw new WDEException("API00037", new object[] {Name});
            else
                throw new WDEException("API00038", new object[] {Name});
		}

		public IWDETextBoxDef AddTextBox()
		{
			string newName = GetNextDefaultName("TextBox");
			IWDETextBoxDef def = WDETextBoxDef.Create();
			Add(def);
			def.ControlName = newName;
			return def;
		}

		public IWDEGroupBoxDef AddGroupBox(string Name)
		{
			int res = this.VerifyName(Name);
			if(res == 0)
			{
				IWDEGroupBoxDef def = WDEGroupBoxDef.Create();
				Add(def);
				def.ControlName = Name;
				return def;
			}
            else if (res == -1)
                  throw new WDEException("API00037", new object[] {Name});
            else
                 throw new WDEException("API00038", new object[] {Name});	
		}

		public IWDEGroupBoxDef AddGroupBox()
		{
			string newName = GetNextDefaultName("GroupBox");
			IWDEGroupBoxDef def = WDEGroupBoxDef.Create();
			Add(def);
			def.ControlName = newName;
			return def;
		}

		public IWDEDetailGridDef AddDetailGrid(string Name)
		{
			int res = this.VerifyName(Name);
			if(res == 0)
			{
				IWDEDetailGridDef def = WDEDetailGridDef.Create();
				Add(def);
				def.ControlName = Name;
				return def;
			}
            else if (res == -1)
                 throw new WDEException("API00037", new object[] {Name});                
            else
                throw new WDEException("API00038", new object[] {Name});	                
		}

		public IWDEDetailGridDef AddDetailGrid()
		{
			string newName = GetNextDefaultName("DetailGrid");
			IWDEDetailGridDef def = WDEDetailGridDef.Create();
			Add(def);
			def.ControlName = newName;
			return def;
		}

		public ArrayList GetKeyorder()
		{
			ArrayList res = (ArrayList) base.List.Clone();
			res.Sort(new KeyOrderSorter());
			return res;
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			Clear();
			XmlReader.MoveToContent();

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ControlDefs"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();
			}

			WDEBaseCollectionItem obj = null;
			m_KeyOrderList = new KeyOrderList();
			try
			{
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					((obj = (WDEBaseCollectionItem) GetControlObj(XmlReader.Name)) != null))
				{
					base.InternalAdd(obj);

					IWDEXmlPersist ipers = (IWDEXmlPersist) obj;
					ipers.ReadFromXml(XmlReader);
					base.RegisterObject(obj);
                    if ((obj is IWDETextBoxDef) || (obj is IWDEDetailGridDef) || (obj is IWDEImageDataDef))
					{
						m_KeyOrderList.Add(obj, ((IWDEControlDef) obj).KeyOrder);
					}
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ControlDefs"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
			finally
			{
				m_KeyOrderList.ReOrder(this);
				m_KeyOrderList = null;
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion

		#region Private Methods

		private IWDEControlDef GetControlObj(string ControlName)
		{
			switch(ControlName)
			{
				case "LabelDef":
					return WDELabelDef.Create();
				case "TextBoxDef":
					return WDETextBoxDef.Create();
				case "GroupBoxDef":
					return WDEGroupBoxDef.Create();
				case "DetailGridDef":
				case "DEGridDef":
					return WDEDetailGridDef.Create();
                case "ImageDataDef":
                    return WDEImageDataDef.Create();
                default:
					return null;
			}
		}

		private IWDEFormDef GetContainingForm()
		{
			IWDEFormDef result = (IWDEFormDef) GetParentInterface("IWDEFormDef");

            if (result != null)
                return result;
            else
                 throw new WDEException("API90014", new object[] {"IWDEControlDef", "IWDEFormDef"});
		}

		private bool CheckChildNames(WDEBaseCollection collection, string name)
		{
			foreach(WDEBaseCollectionItem item in collection)
			{
				if(string.Compare(item.GetNodeName(), name, true) == 0)
				{
					return true;
				}
				
				if(item is IWDEDetailGridDef)
				{
					WDEBaseCollection col = (WDEBaseCollection) ((IWDEDetailGridDef) item).HeaderControlDefs;
					if (CheckChildNames(col, name))
						return true;

					col = (WDEBaseCollection) ((IWDEDetailGridDef) item).ControlDefs;
					if(CheckChildNames(col, name))
						return true;
				}
			}

			return false;
		}

		#endregion

		#region IWDEControlDefsInternal Members

		public void MergeKeyOrderList(object subList, int KeyOrderPosition)
		{
			if(subList is KeyOrderList)
			{
				if(m_KeyOrderList != null)
				{
					m_KeyOrderList.Add(subList, KeyOrderPosition);
				}
			}
		}

		#endregion
	}

	internal class KeyOrderSorter : IComparer
	{
		public KeyOrderSorter()
		{
		}

		#region IComparer Members

		public int Compare(object x, object y)
		{
			if((x is IWDEControlDef) && (y is IWDEControlDef))
			{
				int order_x = ((IWDEControlDef) x).KeyOrder;
				int order_y = ((IWDEControlDef) y).KeyOrder;
				if(order_x < order_y)
					return -1;
				else if(order_x == order_y)
					return 0;
				else
					return 1;
			}
			else
				throw new ArgumentException(string.Format("Cannot compare {0} to {1}", new object[] {x.GetType().FullName, y.GetType().FullName}));
		}

		#endregion

	}


	public class WDEControlDefLinks : WDEBaseCollection, IWDEControlDefLinks, IWDEXmlPersist
	{
		private WDEControlDefLinks(object Parent) : base(Parent)
		{
		}

		public static IWDEControlDefLinks Create(object Parent)
		{
			return new WDEControlDefLinks(Parent) as IWDEControlDefLinks;
		}

		public static IWDEControlDefLinks CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			base.Remove(LinkedItem);
		}

		#region IWDEControlDefLinks Members

		public IWDEControlDef this[int index]
		{
			get
			{
				return (IWDEControlDef) base.InternalGetIndex(index);
			}
			set
			{
				while(Count <= index)
					base.InternalAdd(null);

				WDEBaseCollectionItem obj = base.InternalGetIndex(index);
				if(obj != null)
					obj.RemoveLink(this);

				base.InternalSetIndex(index, (WDEBaseCollectionItem) value, false);

				obj = (WDEBaseCollectionItem) value;
				if(obj != null)
					obj.AddLink(this);
			}
		}

		public void Add(IWDEControlDef ControlDef)
		{
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) ControlDef;
			obj.AddLink(this);

			base.InternalAdd((WDEBaseCollectionItem) ControlDef, false);
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ControlDefLink"))
			{
				Clear();

				IWDEProjectInternal proj = null;
                if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                    proj = (IWDEProjectInternal)base.TopParent();
                else
                   throw new WDEException("API90006", new object[] {"ControlDefLinks"});

				int i = 0;
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "ControlDefLink"))
				{
					proj.Resolver.AddRequest(this, "Item", Utils.GetAttribute(XmlReader, "ControlName", ""), i++);
					XmlReader.Read();
					XmlReader.MoveToContent();

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ZoneDefLink"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				WDEBaseCollectionItem obj = base.InternalGetIndex(i);
				XmlWriter.WriteStartElement("ControlDefLink");
				XmlWriter.WriteAttributeString("ControlName", obj.GetNamePath());
				XmlWriter.WriteEndElement();
			}
		}

		#endregion
	}

	public abstract class WDEControlDef : WDEBaseCollectionItem, IWDEControlDef
	{
		private Color m_BackColor;
		private Color m_ForeColor;
		private string m_Desc;
		private string m_Help;
		private string m_Hint;
		private int m_KeyOrder;
		private bool m_TabStop;
		private Rectangle m_Location;
		private string m_ControlName;
		private string m_ControlFont;

		public WDEControlDef()
		{
			m_BackColor = Color.FromKnownColor(KnownColor.Control);
			m_ForeColor = Color.FromKnownColor(KnownColor.ControlText);
			m_Desc = "";
			m_Help = "";
			m_Hint = "";
			m_Location = Rectangle.Empty;
			m_ControlName = "";
			m_KeyOrder = -1;
            m_ControlFont = "Tahoma, 10pt";
		}

		protected override string InternalGetNodeName()
		{
			return m_ControlName;
		}

		protected void ReadXmlAttributes(XmlTextReader XmlReader)
		{
			m_BackColor = Utils.GetColor(XmlReader, "BackColor", Color.FromKnownColor(KnownColor.Control));
			m_ForeColor = Utils.GetColor(XmlReader, "ForeColor", Color.FromKnownColor(KnownColor.ControlText));
			m_Desc = Utils.GetAttribute(XmlReader, "Description", "");
			m_Help = Utils.GetAttribute(XmlReader, "Help", "");
			m_Hint = Utils.GetAttribute(XmlReader, "Hint", "");
			m_KeyOrder = Utils.GetIntValue(XmlReader, "KeyOrder");
			m_TabStop = Utils.GetBoolValue(XmlReader, "TabStop", false);
			m_Location = Utils.GetRectValue(XmlReader, "Location");
			m_ControlName = Utils.GetAttribute(XmlReader, "ControlName", "");
			m_ControlFont = Utils.GetAttribute(XmlReader, "Font", "Tahoma,10,Regular");
		}

		protected void WriteXmlAttributes(XmlTextWriter XmlWriter)
		{
			if(m_BackColor != Color.FromKnownColor(KnownColor.Control))
				XmlWriter.WriteAttributeString("BackColor", m_BackColor.Name);
			if(m_ForeColor != Color.FromKnownColor(KnownColor.ControlText))
				XmlWriter.WriteAttributeString("ForeColor", m_ForeColor.Name);
			if(m_Desc != "")
				XmlWriter.WriteAttributeString("Description", m_Desc);
			if(m_Help != "")
				XmlWriter.WriteAttributeString("Help", m_Help);
			if(m_Hint != "")
				XmlWriter.WriteAttributeString("Hint", m_Hint);
			if(m_KeyOrder != -1)
				XmlWriter.WriteAttributeString("KeyOrder", m_KeyOrder.ToString());
			if(m_TabStop)
				XmlWriter.WriteAttributeString("TabStop", m_TabStop.ToString());
			if(!m_Location.IsEmpty)
				XmlWriter.WriteAttributeString("Location", Utils.RectToString(m_Location));
			if(m_ControlName != "")
				XmlWriter.WriteAttributeString("ControlName", m_ControlName);
			XmlWriter.WriteAttributeString("Font", m_ControlFont);
		}

		protected void ReadDescription(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
			{
				Description = Utils.GetAttribute(XmlReader, "ENGLISH", "");

				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		protected void ReadFont(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Fonts"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "Font"))
				{
					if(Utils.GetAttribute(XmlReader, "Name", "") == "ENGLISH")
					{
						ForeColor = Utils.GetColor(XmlReader, "Color", Color.FromKnownColor(KnownColor.WindowText));
						string name = Utils.GetAttribute(XmlReader, "FontName", "");
						float height = Utils.ConvertFontHeight(Utils.GetIntValue(XmlReader, "Height"));
						Utils.ConvertBitmapFont(ref name, ref height);
						FontStyle style = Utils.ConvertFontStyle(Utils.GetAttribute(XmlReader, "Style", ""), XmlReader.LineNumber, XmlReader.LinePosition);

                        using (Font temp = new Font(name, height, style))
                            m_ControlFont = Utils.FontToString(temp);
					}

					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Fonts"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		protected void ReadHelp(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Helps"))
			{
				Help = Utils.GetAttribute(XmlReader, "ENGLISH", "");
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		protected void ReadHint(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Hints"))
			{
				Hint = Utils.GetAttribute(XmlReader, "ENGLISH", "");
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		protected void ReadBackColor(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Colors"))
			{
				BackColor = Utils.GetColor(XmlReader, "ENGLISH", Color.FromKnownColor(KnownColor.Control));
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		protected void ReadZoneLinks(XmlTextReader XmlReader, IWDEZoneLinks ZoneLinks)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ZoneLinks"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				IWDEProjectInternal iproj = GetProjectInternal();
                if (iproj == null)
                    throw new WDEException("API90006", new object[] {"ControlDef"});

				int index = 0;
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "ZoneLink"))
				{
					string temp = Utils.GetAttribute(XmlReader, "Zone", "");
					if(temp != "")
					{
						string[] parts = temp.Split('.');
						IWDEFormDef fd = GetParentFormDef();
						if(string.Compare(parts[1], fd.FormName, true) == 0)
						{
							temp = PrependFormName(temp);
							string convertName = temp;
							int i = convertName.LastIndexOf(".");
							convertName = convertName.Substring(0, i);
							iproj.Resolver.AddRequest(ZoneLinks, "Item", temp, index++, convertName);
						}
					}

					XmlReader.Read();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ZoneLinks"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		protected IWDEControlDefs GetTopLevelControlDefs()
		{
			IWDEFormDef res = GetParentFormDef();

			if(res != null)
			{
				return ((IWDEFormDef) res).ControlDefs;
			}
			else
				return null;
		}

		protected IWDEFormDef GetParentFormDef()
		{
			return (IWDEFormDef) GetParentInterface("IWDEFormDef");
		}
		
		protected virtual string PrependFormName(string zoneName)
		{
			IWDEFormDef def = GetParentFormDef();
			if(def != null)
			{
				string[] temp = zoneName.Split('.');
				temp[1] = def.FormName;
				temp[temp.Length - 1] = def.FormName + "_" + temp[temp.Length - 1];
				return string.Join(".", temp);
			}
			else
				return zoneName;
		}

		#region IWDEControlDef_R1 Members

		public Color BackColor
		{
			get
			{
				return m_BackColor;
			}
			set
			{
				m_BackColor = value;
			}
		}

		public Color ForeColor
		{
			get
			{
				return m_ForeColor;
			}
			set
			{
				m_ForeColor = value;
			}
		}

		public string Description
		{
			get
			{
				return m_Desc;
			}
			set
			{
				if(value == null)
					m_Desc = "";
				else
					m_Desc = value;
			}
		}

		public string Help
		{
			get
			{
				return m_Help;
			}
			set
			{
				if(value == null)
					m_Help = "";
				else
					m_Help = value;
			}
		}

		public string Hint
		{
			get
			{
				return m_Hint;
			}
			set
			{
				if(value == null)
					m_Hint = "";
				else
					m_Hint = value;
			}
		}

		public int KeyOrder
		{
			get
			{
				return m_KeyOrder;
			}
			set
			{
				m_KeyOrder = value;
			}
		}

		public bool TabStop
		{
			get
			{
				return m_TabStop;
			}
			set
			{
				m_TabStop = value;
			}
		}

		public Rectangle Location
		{
			get
			{
				return m_Location;
			}
			set
			{
				m_Location = value;
			}
		}

		public string ControlName
		{
			get
			{
				return m_ControlName;
			}
			set
			{
				string newName = value;
				if(newName == null)
					newName = "";

				if(newName != m_ControlName)
				{
					int res = base.Collection.VerifyName(newName);
					if(res == 0)
						m_ControlName = newName;
                    else if (res == -1)
                         throw new WDEException("API00037", new object[] {newName});
                    else if (res == -2)
                        throw new WDEException("API00038", new object[] {newName});
				}
			}
		}

		public Font ControlFont
		{
			get
			{
				return Utils.StringToFont(m_ControlFont);
			}
			set
			{
				m_ControlFont = Utils.FontToString(value);
			}
		}

        public IWDEControlDef Clone()
        {
            return (IWDEControlDef)this.MemberwiseClone();
		}

		#endregion

	}

	public class WDELabelDef : WDEControlDef, IWDELabelDef, IWDEXmlPersist
	{
		private string m_Caption;
		private IWDEFieldDef m_FieldDef;
		private IWDEControlDef m_NextControl;
		private bool m_AutoSize;
		
		private WDELabelDef()
		{
			m_Caption = "";
			KeyOrder = -1;
		}

		public static IWDELabelDef Create()
		{
			return new WDELabelDef() as IWDELabelDef;
		}

		public static IWDELabelDef CreateInstance()
		{
			return Create();
		}

		protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			if(LinkedItem == m_FieldDef)
				m_FieldDef = null;
			else if(LinkedItem == m_NextControl)
				m_NextControl = null;
		}

		#region IWDELabelDef Members

		public string Caption
		{
			get
			{
				return m_Caption;
			}
			set
			{
				if(value == null)
					m_Caption = "";
				else
					m_Caption = value;
			}
		}

		public IWDEFieldDef Field
		{
			get
			{
				return m_FieldDef;
			}
			set
			{
				if(m_FieldDef != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_FieldDef;
					obj.RemoveLink(this);
				}

				m_FieldDef = value;

				if(m_FieldDef != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_FieldDef;
					obj.AddLink(this);
				}
			}
		}

		public IWDEControlDef NextControl
		{
			get
			{
				return m_NextControl;
			}
			set
			{
				if(m_NextControl != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_NextControl;
					obj.RemoveLink(this);
				}

				m_NextControl = value;

				if(m_NextControl != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_NextControl;
					obj.AddLink(this);
				}
			}
		}

		public bool AutoSize
		{
			get
			{
				return m_AutoSize;
			}
			set
			{
				m_AutoSize = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			IWDEProjectInternal proj = null;
            if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                proj = (IWDEProjectInternal)base.TopParent();
            else
                throw new WDEException("API90006", new object[] {"LabelDef"});

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "LabelDef"))
			{
				if(ConvertingOldProject)
				{
					ConvertOldLabel(XmlReader, proj);
				}
				else
				{
					base.ReadXmlAttributes(XmlReader);
					KeyOrder = -1;
					m_Caption = Utils.GetAttribute(XmlReader, "Caption", "");
					m_AutoSize = Utils.GetBoolValue(XmlReader, "AutoSize", false);

					string temp = Utils.GetAttribute(XmlReader, "Field", "");
					if(temp != "")
						proj.Resolver.AddRequest(this, "Field", temp);

					temp = Utils.GetAttribute(XmlReader, "NextControl", "");
					if(temp != "")
						proj.Resolver.AddRequest(this, "NextControl", temp);

					XmlReader.Read();
					XmlReader.MoveToContent();

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "LabelDef"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("LabelDef");
			base.WriteXmlAttributes(XmlWriter);
//			if(Alignment != ContentAlignment.MiddleRight)
//				XmlWriter.WriteAttributeString("Alignment", Alignment.ToString());
			if(Caption != "")
				XmlWriter.WriteAttributeString("Caption", Caption);
			if(AutoSize)
				XmlWriter.WriteAttributeString("AutoSize", AutoSize.ToString());
			
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) Field;
			if(obj != null)
				XmlWriter.WriteAttributeString("Field", obj.GetNamePath());

			obj = (WDEBaseCollectionItem) NextControl;
			if(obj != null)
				XmlWriter.WriteAttributeString("NextControl", obj.GetNamePath());

			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Methods

		private void ConvertOldLabel(XmlTextReader XmlReader, IWDEProjectInternal proj)
		{
			ControlName = Utils.GetAttribute(XmlReader, "Name", "");
			m_AutoSize = Utils.GetBoolValue(XmlReader, "AutoSize", false);
			
			string temp = Utils.GetAttribute(XmlReader, "Field", "");
			if(temp != "")
				proj.Resolver.AddRequest(this, "Field", temp);

			temp = FixFormName(Utils.GetAttribute(XmlReader, "NextControl", ""));
			if(temp != "")
				proj.Resolver.AddRequest(this, "NextControl", temp);

			XmlReader.Read();
			XmlReader.MoveToContent();

			ReadDescription(XmlReader);
			ReadAlignment(XmlReader);
			ReadCaption(XmlReader);
			ReadBackColor(XmlReader);
			ReadFont(XmlReader);
			ReadRect(XmlReader);

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "LabelDef"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		private void ReadAlignment(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Alignments"))
			{
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		private void ReadCaption(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Captions"))
			{
				m_Caption = Utils.GetAttribute(XmlReader, "ENGLISH", "");

				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		private void ReadRect(XmlTextReader XmlReader)
		{
			int l = 0;
			int t = 0;
			int w = 0;
			int h = 0;

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Heights"))
			{
				h = Utils.GetIntValue(XmlReader, "ENGLISH");
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Lefts"))
			{
				l = Utils.GetIntValue(XmlReader, "ENGLISH");
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Tops"))
			{
				t = Utils.GetIntValue(XmlReader, "ENGLISH");
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Widths"))
			{
				w = Utils.GetIntValue(XmlReader, "ENGLISH");
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}

			// increased positioning from GDI positions.
			l = (int) (l * 1.1);
			w = (int) (w * 1.1);

			Location = new Rectangle(l, t, w, h);
		}

		private string FixFormName(string controlLink)
		{
			IWDEFormDef def = (IWDEFormDef) GetParentInterface("IWDEFormDef");

			if(controlLink != "")
			{
				if(def != null)
				{
					string[] temp = controlLink.Split('.');
					if(temp.Length > 1)
					{
						temp[1] = def.FormName;
						return string.Join(".", temp);
					}
					else
						return temp[0];
				}
				else
					return controlLink;
			}
			else
				return controlLink;
		}

		#endregion
	}

    public class WDETextBoxDef : WDEControlDef, IWDETextBoxDef_R3, IWDEXmlPersist
	{
		private string m_CharSet;
        private string m_DisplayLabel;
		private IWDEEditDefs m_EditDefs;
		private IWDEFieldDef m_Field;
		private string m_InputMask;
		private WDEControlOption m_Options;
		private IWDELabelLinks m_LabelLinks;
		private IWDEZoneLinks m_ZoneLinks;
		private IWDEEventScriptDef m_OnValidate;
		private IWDEEventScriptDef m_OnEnter;
		private IWDEEventScriptDef m_OnExit;
        private IWDEEventScriptDef m_OnKeyPress;
        private IWDEErrorOverrides m_ErrorOverrides;

		private WDETextBoxDef()
		{
			m_CharSet = "";
            m_DisplayLabel = "";
			m_InputMask = "";
			m_Options = WDEControlOption.None;

			m_EditDefs = WDEEditDefs.Create(this);
			m_LabelLinks = WDELabelLinks.Create(this);
			m_ZoneLinks = WDEZoneLinks.Create(this);
			m_OnValidate = WDEEventScriptDef.Create(this, "OnValidate");
			m_OnEnter = WDEEventScriptDef.Create(this, "OnEnter");
			m_OnExit = WDEEventScriptDef.Create(this, "OnExit");
            m_OnKeyPress = WDEEventScriptDef.Create(this, "OnKeyPress");
            m_ErrorOverrides = WDEErrorOverrides.Create(this);
		}

		public static IWDETextBoxDef Create()
		{
			return new WDETextBoxDef() as IWDETextBoxDef;
		}

		public static IWDETextBoxDef CreateInstance()
		{
			return Create();
		}

		protected override string PrependFormName(string zoneName)
		{
			IWDEFormDef def = GetParentFormDef();
			if(def != null)
			{
				string[] temp = zoneName.Split('.');
				temp[1] = def.FormName;
				if(Parent is IWDEDetailGridDef && temp.Length > 4)
					temp[temp.Length - 2] = def.FormName + "_" + temp[temp.Length - 2];
				temp[temp.Length - 1] = def.FormName + "_" + temp[temp.Length - 1];
				return string.Join(".", temp);
			}
			else
				return zoneName;
		}

		#region IWDETextBoxDef Members

		public string CharSet
		{
			get
			{
                if (string.IsNullOrEmpty(m_CharSet))
                {
                    if (m_Field != null)
                        m_CharSet = m_Field.CharSet;
                }

				return m_CharSet;
			}
			set
			{
				if(value == null)
					m_CharSet = "";
				else
					m_CharSet = value;
			}
		}        

		public IWDEEditDefs EditDefs
		{
			get
			{
				return m_EditDefs;
			}
		}

		public IWDEFieldDef Field
		{
			get
			{
				return m_Field;
			}
			set
			{
				if(m_Field != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Field;
					obj.RemoveLink(this);
				}

				m_Field = value;

				if(m_Field != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Field;
					obj.AddLink(this);

                    if ((m_Options == WDEControlOption.None) || (base.ConvertingOldProject))
                    {
                        if ((m_Field.Options & WDEFieldOption.MustComplete) == WDEFieldOption.MustComplete)
                            m_Options = m_Options | WDEControlOption.MustComplete;                        
                        if ((m_Field.Options & WDEFieldOption.MustEnter) == WDEFieldOption.MustEnter)
                            m_Options = m_Options | WDEControlOption.MustEnter;                        
                        if ((m_Field.Options & WDEFieldOption.NumericShift) == WDEFieldOption.NumericShift)
                            m_Options = m_Options | WDEControlOption.NumericShift;
                        if ((m_Field.Options & WDEFieldOption.AllowFlag) == WDEFieldOption.AllowFlag)
                            m_Options = m_Options | WDEControlOption.AllowFlag;
                        if ((m_Field.Options & WDEFieldOption.UpperCase) == WDEFieldOption.UpperCase)
                            m_Options = m_Options | WDEControlOption.UpperCase;
                    }
				}
			}
		}

		public string InputMask
		{
			get
			{
				return m_InputMask;
			}
			set
			{
				if(value == null)
					m_InputMask = "";
				else
					m_InputMask = value;
			}
		}

		public WebDX.Api.WDEControlOption Options
		{
			get
			{
				return m_Options;
			}
			set
			{
				m_Options = value;
			}
		}

		public IWDELabelLinks LabelLinks 
		{
			get
			{
				return m_LabelLinks;
			}
		}

		public IWDEZoneLinks ZoneLinks 
		{
			get
			{
				return m_ZoneLinks;
			}
		}

		public IWDEEventScriptDef OnValidate
		{
			get
			{
				return m_OnValidate;
			}
            set
            {
                m_OnValidate = value;
            }
		}

		public IWDEEventScriptDef OnEnter
		{
			get
			{
				return m_OnEnter;
			}
            set
            {
                m_OnEnter = value;
            }
		}

		public IWDEEventScriptDef OnExit
		{
			get
			{
				return m_OnExit;
			}
            set
            {
                m_OnExit = value;
            }
		}

		public ArrayList GetZonesByImageType(string ImageType)
		{
			if (ImageType == null)
				throw new ArgumentNullException("ImageType", "ImageType cannot be null");

			ArrayList result = new ArrayList();

			foreach (WDEBaseCollectionItem item in ZoneLinks)
			{
				if (item is IWDEZoneDef)
				{
					object current = item.Parent;
					while ((current != null) && (!(current is IWDEImageSourceDef)) && (current is WDEBaseCollectionItem))
						current = ((WDEBaseCollectionItem)current).Parent;

					if ((current != null) && (current is IWDEImageSourceDef))
						if (string.Compare(((IWDEImageSourceDef)current).ImageType, ImageType, true) == 0)
							result.Add(item);
				}
			}

			return result;
		}

		public IWDESnippetDef GetSnippet(IWDEZoneDef Zone)
		{
			if(Zone == null)
                throw new ArgumentNullException("Zone","Zone cannot be null");

			if(Zone.ZoneRect.IsEmpty)
				return null;

			object current = ((WDEBaseCollectionItem) Zone).Parent;
			while((current != null) && (!(current is IWDEImageSourceDef)) && (current is WDEBaseCollectionItem))
				current = ((WDEBaseCollectionItem) current).Parent;

			if((current != null) && (current is IWDEImageSourceDef))
			{
				IWDEImageSourceDef def = (IWDEImageSourceDef) current;
				foreach(IWDESnippetDef snip in def.SnippetDefs)
				{
					Rectangle intRect = Rectangle.Intersect(snip.SnippetRect, Zone.ZoneRect);
					if(intRect.Equals(Zone.ZoneRect))
						return snip;
				}

				return null;
			}
			else
				return null;
		}

		#endregion

        #region IWDETextBoxDef_R1 Members

        public IWDEEventScriptDef OnKeyPress
        {
            get 
            { 
                return m_OnKeyPress; 
            }
            set
            {
                m_OnKeyPress = value;
            }
        }

        #endregion

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			IWDEProjectInternal proj = null;
            if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                proj = (IWDEProjectInternal)base.TopParent();
            else
                throw new WDEException("API90006", new object[] {"TextBoxDef"});

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "TextBoxDef"))
			{
				if(ConvertingOldProject)
				{
					ConvertOldTextBox(XmlReader, proj);
				}
				else
				{
					base.ReadXmlAttributes(XmlReader);

					m_CharSet = Utils.GetAttribute(XmlReader, "CharSet", "");
                    m_DisplayLabel = Utils.GetAttribute(XmlReader, "DisplayLabel", "");
					m_InputMask = Utils.GetAttribute(XmlReader, "InputMask", "");
					m_Options = Utils.GetControlOption(XmlReader, "Options");

                    IWDEProject project = (IWDEProject)this.GetProjectInternal();
                    Version projVersion = new Version(project.APIVersion);                    
                    Version optionVersion = new Version("3.0.0.0");
                    if (projVersion < optionVersion)
                        m_Options |= WDEControlOption.SwitchNoZoneImage;

					string temp = Utils.GetAttribute(XmlReader, "Field", "");
					if(temp != "")
						proj.Resolver.AddRequest(this, "Field", temp);

					XmlReader.Read();
					XmlReader.MoveToContent();

					IWDEXmlPersist ipers = (IWDEXmlPersist) m_EditDefs;
					ipers.ReadFromXml(XmlReader);

                    ipers = (IWDEXmlPersist)m_ErrorOverrides;
                    ipers.ReadFromXml(XmlReader);

					ipers = (IWDEXmlPersist) m_LabelLinks;
					ipers.ReadFromXml(XmlReader);

					ipers = (IWDEXmlPersist) m_ZoneLinks;
					ipers.ReadFromXml(XmlReader);

					ipers = (IWDEXmlPersist) m_OnValidate;
					ipers.ReadFromXml(XmlReader);

					ipers = (IWDEXmlPersist) m_OnEnter;
					ipers.ReadFromXml(XmlReader);

					ipers = (IWDEXmlPersist) m_OnExit;
					ipers.ReadFromXml(XmlReader);

                    ipers = (IWDEXmlPersist)m_OnKeyPress;
                    ipers.ReadFromXml(XmlReader);

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "TextBoxDef"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
			else if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Column"))
			{
				ConvertColumn(XmlReader, proj);
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("TextBoxDef");
			base.WriteXmlAttributes(XmlWriter);
			if(CharSet != "")
				XmlWriter.WriteAttributeString("CharSet", CharSet);
            if (DisplayLabel != "")
                XmlWriter.WriteAttributeString("DisplayLabel", DisplayLabel);
			if(InputMask != "")
				XmlWriter.WriteAttributeString("InputMask", InputMask);
			if(m_Options != WDEControlOption.None)
				XmlWriter.WriteAttributeString("Options", m_Options.ToString());

			if(Field != null)
			{
				WDEBaseCollectionItem obj = (WDEBaseCollectionItem) Field;
				XmlWriter.WriteAttributeString("Field", obj.GetNamePath());
			}

			IWDEXmlPersist ipers = (IWDEXmlPersist) m_EditDefs;
			ipers.WriteToXml(XmlWriter);

            ipers = (IWDEXmlPersist)m_ErrorOverrides;
            ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist) m_LabelLinks;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist) m_ZoneLinks;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist) m_OnValidate;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist) m_OnEnter;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist) m_OnExit;
			ipers.WriteToXml(XmlWriter);

            ipers = (IWDEXmlPersist)m_OnKeyPress;
            ipers.WriteToXml(XmlWriter);

			XmlWriter.WriteEndElement();
		}

		#endregion

        #region IWDETextBoxDef_R2 Members

        public string DisplayLabel
        {
            get
            {
                return m_DisplayLabel;
            }
            set
            {
                if (value == null)
                    m_DisplayLabel = "";
                else
                    m_DisplayLabel = value;
            }
        }

        #endregion

        #region IWDETextBoxDef_R3 Members

        public IWDEErrorOverrides ErrorOverrides
        {
            get
            {
                return m_ErrorOverrides;
            }
            set
            {
                m_ErrorOverrides = value;
            }
        }

        #endregion

        #region Private Methods

        private void ConvertOldTextBox(XmlTextReader XmlReader, IWDEProjectInternal iproj)
		{
			ControlName = Utils.GetAttribute(XmlReader, "Name", "");
			CharSet = Utils.GetAttribute(XmlReader, "CharSet", "");
            DisplayLabel = Utils.GetAttribute(XmlReader, "DisplayLabel", "");
			m_Options = WDEControlOption.None;

			if(Utils.GetBoolValue(XmlReader, "AllowFlag", true) == true)
				m_Options |= WDEControlOption.AllowFlag;

			if(Utils.GetBoolValue(XmlReader, "AutoAdvance", true) == true)
				m_Options |= WDEControlOption.AutoAdvance;

			string temp = Utils.GetAttribute(XmlReader, "Field", "");
			if(temp != "")
				iproj.Resolver.AddRequest(this, "Field", temp);

			m_InputMask = Utils.GetAttribute(XmlReader, "InputMask", "");
			KeyOrder = Utils.GetIntValue(XmlReader, "KeyOrder");
			WDEControlOption tempOpt = Utils.GetControlOption(XmlReader, "Options");
			m_Options |= tempOpt;
			
			if(Utils.GetBoolValue(XmlReader, "ShowSnippetImage", false) == true)
				m_Options |= WDEControlOption.ShowSnippetImage;

			TabStop = Utils.GetBoolValue(XmlReader, "TabStop", false);

			XmlReader.Read();
			XmlReader.MoveToContent();

			ReadDescription(XmlReader);
			ReadAccumulatorCheck(XmlReader, iproj);
			ReadAddressCorrection(XmlReader);
			ReadBackColor(XmlReader);
			ReadConditionalGoto(XmlReader);
			ReadDateEdit(XmlReader);
			ReadDiagnosisPtr(XmlReader);
			ReadFont(XmlReader);
			int h = ReadChildInt(XmlReader, "Heights");
			ReadHelp(XmlReader);
			ReadHint(XmlReader);
			ReadLabelLinks(XmlReader);
			int l = ReadChildInt(XmlReader, "Lefts");
			ReadMustEnter(XmlReader);
			ReadShowEntireImage(XmlReader);
			ReadTableLookup(XmlReader);
			int t = ReadChildInt(XmlReader, "Tops");
			ReadValidate(XmlReader);
			int w = ReadChildInt(XmlReader, "Widths");
			
			l = (int) (l * 1.1);
			w = (int) (w * 1.1);
			Location = new Rectangle(l, t, w, h);

			ReadZipLookup(XmlReader);
			ReadZoneLinks(XmlReader, ZoneLinks);
			ReadScripts(XmlReader);

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "TextBoxDef"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		private void ConvertColumn(XmlTextReader XmlReader, IWDEProjectInternal iproj)
		{
			ControlName = Utils.GetAttribute(XmlReader, "Name", "");
			if(Utils.GetBoolValue(XmlReader, "AutoAdvance", true) == true)
				m_Options |= WDEControlOption.AutoAdvance;
			string temp = Utils.GetAttribute(XmlReader, "Field", "");
			if(temp != "")
				iproj.Resolver.AddRequest(this, "Field", temp);
			XmlReader.Read();
			XmlReader.MoveToContent();

			ReadDiagnosisPtr(XmlReader);
			ReadMustEnter(XmlReader);
			ReadTableLookup(XmlReader);
			ReadTitle(XmlReader);
			ReadValidate(XmlReader);
			int w = Utils.StrToIntDef(ReadEnglishValue(XmlReader, "Widths"), 0);
			ReadZipLookup(XmlReader);
			ReadZoneLinks(XmlReader, ZoneLinks);
			w = (int) (w * 1.1);
			Location = new Rectangle(0, 0, w, 24);
			ReadScripts(XmlReader);

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Column"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		private void ReadTitle(XmlTextReader XmlReader)
		{
			string title = ReadEnglishValue(XmlReader, "Titles");
			Help = title;
		}

		private string ReadEnglishValue(XmlTextReader XmlReader, string Key)
		{
			string result = "";
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == Key))
			{
				result = Utils.GetAttribute(XmlReader, "ENGLISH", "");
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
			return result;
		}

		private void ReadAccumulatorCheck(XmlTextReader XmlReader, IWDEProjectInternal iproj)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "AccumulatorCheck"))
			{
				string accum = Utils.GetAttribute(XmlReader, "Accumulator", "");
				bool enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
				if(enabled && (accum != ""))
				{
					Hashtable ht = new Hashtable();
					ht.Add("Enabled", enabled);
					ht.Add("ErrorType", Utils.GetEditErrorType(XmlReader, "ErrorType"));
					ht.Add("SessionMode", Utils.GetSessionType(XmlReader, "ViewModes"));
					
					XmlReader.Read();
					XmlReader.MoveToContent();

					if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
					{
						ht.Add("Description", Utils.GetAttribute(XmlReader, "ENGLISH", ""));
						XmlReader.ReadInnerXml();
						XmlReader.MoveToContent();
					}

					if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ErrorMessages"))
					{
						ht.Add("ErrorMessage", Utils.GetAttribute(XmlReader, "ENGLISH", ""));
						XmlReader.ReadInnerXml();
						XmlReader.MoveToContent();
					}

					iproj.Resolver.AddDupeRequest(EditDefs, accum, ht, "Add");

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "AccumulatorCheck"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
				else
				{
					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ReadAddressCorrection(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "AddressCorrection"))
			{
				IWDEAddressCorrectionEditDef def = WDEAddressCorrectionEditDef.Create();
				IWDEXmlPersist ipers = (IWDEXmlPersist) def;
				EditDefs.Add(def);
				ipers.ReadFromXml(XmlReader);
			}
		}

		private void ReadConditionalGoto(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ConditionalGoto"))
			{
				IWDEConditionalGotoEditDef def = WDEConditionalGotoEditDef.Create();
				IWDEXmlPersist ipers = (IWDEXmlPersist) def;
				EditDefs.Add(def);
				ipers.ReadFromXml(XmlReader);
			}
		}

		private void ReadDateEdit(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DateEdit"))
			{
				IWDEDateEditDef def = WDEDateEditDef.Create();
				IWDEXmlPersist ipers = (IWDEXmlPersist) def;
				EditDefs.Add(def);
				ipers.ReadFromXml(XmlReader);
			}
		}

		private void ReadMustEnter(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "MustEnter"))
			{
				IWDERequiredEditDef def = WDERequiredEditDef.Create();
				IWDEXmlPersist ipers = (IWDEXmlPersist) def;
				EditDefs.Add(def);
				ipers.ReadFromXml(XmlReader);
			}
		}

		private void ReadValidate(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Validate"))
			{
				IWDEValidateEditDef def = WDEValidateEditDef.Create();
				IWDEXmlPersist ipers = (IWDEXmlPersist) def;
				EditDefs.Add(def);
				ipers.ReadFromXml(XmlReader);
			}
		}

		private void ReadDiagnosisPtr(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DiagnosisPtr"))
			{
				IWDEDiagnosisPtrEditDef def = WDEDiagnosisPtrEditDef.Create();
				IWDEXmlPersist ipers = (IWDEXmlPersist) def;
				EditDefs.Add(def);
				ipers.ReadFromXml(XmlReader);
			}
		}

		private void ReadLabelLinks(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "LabelLinks"))
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) LabelLinks;
				ipers.ReadFromXml(XmlReader);
			}
		}

		private void ReadShowEntireImage(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ShowEntireImage"))
			{
				if(Utils.GetBoolValue(XmlReader, "Enabled", false) == true)
					m_Options |= WDEControlOption.ShowEntireImage;
				if(Utils.GetBoolValue(XmlReader, "ShowOverlay", false) == true)
					m_Options |= WDEControlOption.ShowOverlay;

				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		private int ReadChildInt(XmlTextReader XmlReader, string TagName)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == TagName))
			{
				int res = Utils.GetIntValue(XmlReader, "ENGLISH");
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
				return res;
			}
			else
				return 0;
		}

		private void ReadTableLookup(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "TableLookup"))
			{
				IWDETableLookupEditDef def = WDETableLookupEditDef.Create();
				IWDEXmlPersist ipers = (IWDEXmlPersist) def;
				EditDefs.Add(def);
				ipers.ReadFromXml(XmlReader);
			}
		}

		private void ReadZipLookup(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ZipLookup"))
			{
				IWDEZipLookupEditDef def = WDEZipLookupEditDef.Create();
				IWDEXmlPersist ipers = (IWDEXmlPersist) def;
				m_EditDefs.Add(def);
				ipers.ReadFromXml(XmlReader);
			}
		}

		private void ReadScripts(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Scripts"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "EventScript"))
				{
					string name = Utils.GetAttribute(XmlReader, "Name", "");
					IWDEXmlPersist ipers = null;
					if(name == "OnValidate")
					{
						ipers = (IWDEXmlPersist) m_OnValidate;
						ipers.ReadFromXml(XmlReader);
					}
					else if(name == "OnEnter")
					{
						ipers = (IWDEXmlPersist) m_OnEnter;
						ipers.ReadFromXml(XmlReader);
					}
					else if(name == "OnExit")
					{
						ipers = (IWDEXmlPersist) m_OnExit;
						ipers.ReadFromXml(XmlReader);
					}
					else
					{
						XmlReader.ReadInnerXml(); // slurp any invalid events.
						XmlReader.MoveToContent();
					}
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Scripts"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
	}

	public class WDEGroupBoxDef : WDEControlDef, IWDEGroupBoxDef, IWDEXmlPersist
	{
		private string m_Caption;
		private WDEGroupBoxOption m_Options;
		private IWDEZoneLinks m_ZoneLinks;
		private IWDEControlDefLinks m_ControlDefLinks;

		private WDEGroupBoxDef()
		{
			m_Options = WDEGroupBoxOption.None;
			m_Caption = "";
			KeyOrder = -1;
			
			m_ZoneLinks = WDEZoneLinks.Create(this);
			m_ControlDefLinks = WDEControlDefLinks.Create(this);
		}
		
		public static IWDEGroupBoxDef Create()
		{
			return new WDEGroupBoxDef() as IWDEGroupBoxDef;
		}

		public static IWDEGroupBoxDef CreateInstance()
		{
			return Create();
		}

		#region IWDEGroupBoxDef Members

		public string Caption
		{
			get
			{
				return m_Caption;
			}
			set
			{
				if(value == null)
					m_Caption = "";
				else
					m_Caption = value;
			}
		}

		public WebDX.Api.WDEGroupBoxOption Options
		{
			get
			{
				return m_Options;
			}
			set
			{
				m_Options = value;
			}
		}

		public IWDEZoneLinks ZoneLinks
		{
			get
			{
				return m_ZoneLinks;
			}
		}

		public IWDEControlDefLinks ControlDefs
		{
			get
			{
				return m_ControlDefLinks;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if(ConvertingOldProject)
			{
				ReadOldGroupBox(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "GroupBoxDef"))
				{
					base.ReadXmlAttributes(XmlReader);
					KeyOrder = -1;
					m_Caption = Utils.GetAttribute(XmlReader, "Caption", "");

					m_Options = Utils.GetGroupBoxOption(XmlReader, "Options");

					bool isEmpty = XmlReader.IsEmptyElement;
					XmlReader.Read();
					XmlReader.MoveToContent();
					IWDEXmlPersist ipers = null;

					if(!isEmpty)
					{
						ipers = (IWDEXmlPersist) ControlDefs;
						ipers.ReadFromXml(XmlReader);
					}

					ipers = (IWDEXmlPersist) m_ZoneLinks;
					ipers.ReadFromXml(XmlReader);

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "GroupBoxDef"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("GroupBoxDef");
			base.WriteXmlAttributes(XmlWriter);
			if(Caption != "")
				XmlWriter.WriteAttributeString("Caption", Caption);

			if(m_Options != WDEGroupBoxOption.None)
				XmlWriter.WriteAttributeString("Options", m_Options.ToString());

			IWDEXmlPersist ipers = (IWDEXmlPersist) ControlDefs;
			ipers.WriteToXml(XmlWriter);
			ipers = (IWDEXmlPersist) m_ZoneLinks;
			ipers.WriteToXml(XmlWriter);

			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Methods

		private void ReadOldGroupBox(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "GroupBoxDef"))
			{
				ControlName = Utils.GetAttribute(XmlReader, "Name", "");
				Options = Utils.GetGroupBoxOption(XmlReader, "Options");
				TabStop = Utils.GetBoolValue(XmlReader, "TabStop", false);
				KeyOrder = Utils.GetIntValue(XmlReader, "KeyOrder");

				XmlReader.Read();
				XmlReader.MoveToContent();

				Description = ReadEnglishText(XmlReader, "Descriptions");
				Caption = ReadEnglishText(XmlReader, "Captions");
				ReadBackColor(XmlReader);
				
				ReadControlDefs(XmlReader);

				ReadFont(XmlReader);
				int h = ReadHeight(XmlReader);
				ReadHelp(XmlReader);
				ReadHint(XmlReader);
				int l = ReadLeft(XmlReader);
				int t = ReadTop(XmlReader);
				int w = ReadWidth(XmlReader);
				ReadZoneLinks(XmlReader, ZoneLinks);

				l = (int) (l * 1.1);
				w = (int) (w * 1.1);
				Location = new Rectangle(l, t, w, h);
				OffsetControlDefs(l, t);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "GroupBoxDef"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private string ReadEnglishText(XmlTextReader XmlReader, string Key)
		{
			string text = "";
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == Key))
			{
				text = Utils.GetAttribute(XmlReader, "ENGLISH", "");
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
			return text;
		}

		private int ReadHeight(XmlTextReader XmlReader)
		{
			string temp = ReadEnglishText(XmlReader, "Heights");
			return Utils.StrToIntDef(temp, 0);
		}

		private int ReadLeft(XmlTextReader XmlReader)
		{
			string temp = ReadEnglishText(XmlReader, "Lefts");
			return Utils.StrToIntDef(temp, 0);
		}

		private int ReadTop(XmlTextReader XmlReader)
		{
			string temp = ReadEnglishText(XmlReader, "Tops");
			return Utils.StrToIntDef(temp, 0);
		}

		private int ReadWidth(XmlTextReader XmlReader)
		{
			string temp = ReadEnglishText(XmlReader, "Widths");
			return Utils.StrToIntDef(temp, 0);
		}

		private void ReadControlDefs(XmlTextReader XmlReader)
		{
			IWDEControlDefs subDefs = WDEControlDefs.Create(this);
			IWDEXmlPersist ipers = (IWDEXmlPersist) subDefs;
			ipers.ReadFromXml(XmlReader);

			KeyOrderList subOrder = new KeyOrderList();
			IWDEControlDefsInternal formDefs = (IWDEControlDefsInternal) GetTopLevelControlDefs();
			foreach(IWDEControlDef subDef in subDefs)
			{
				formDefs.Add(subDef);
				if(subDef is IWDETextBoxDef)
					subOrder.Add(subDef, subDef.KeyOrder);

				ControlDefs.Add(subDef);
			}

			if(TabStop)
			{
				if(subOrder.List.Count > 0)
					((IWDEControlDef) subOrder.List[0]).TabStop = true;
				TabStop = false;
			}

			((IWDEControlDefsInternal) formDefs).MergeKeyOrderList(subOrder, KeyOrder);
			KeyOrder = -1;
		}

		private void OffsetControlDefs(int x, int y)
		{
			foreach(IWDEControlDef def in ControlDefs)
			{
				Rectangle temp = def.Location;
				temp.Offset(x, y);
				def.Location = temp;
			}
		}

		#endregion
	}

	public class WDEDetailGridDef : WDEControlDef, IWDEDetailGridDef, IWDEXmlPersist, IWDELinkConverter, IWDEXmlPersistFlat
	{
		private const int DEFAULT_FIXED_HEIGHT = 30;
		private Color HEADER_BACK_COLOR = Color.DarkGray;
		private Color HEADER_FORE_COLOR = Color.White;

		private IWDERecordDef m_RecordDef;
		private IWDEControlDefs m_HeaderControlDefs;
		private Color m_HeaderBackColor;
		private Color m_HeaderForeColor;
		private int m_HeaderHeight;
		private string m_HeaderFont;
		private WDEDetailGridOption m_Options;
		private int m_Rows;
		private WDERecordNumberPosition m_RecordNumberPosition;
		private IWDEDetailZoneDef m_DetailZoneDef;
		private IWDEEventScriptDef m_OnEnter;
		private IWDEEventScriptDef m_OnExit;
		private IWDEEventScriptDef m_OnValidate;
		private IWDEControlDefs m_ControlDefs;
        private bool m_WriteFlat;

		private WDEDetailGridDef()
		{
			m_HeaderFont = "Tahoma, 10pt";
			m_HeaderBackColor = HEADER_BACK_COLOR;
			m_HeaderForeColor = HEADER_FORE_COLOR;
			m_Options = WDEDetailGridOption.None;
			m_RecordNumberPosition = WDERecordNumberPosition.Right;

			m_HeaderControlDefs = WDEControlDefs.Create(this);
			m_OnEnter = WDEEventScriptDef.Create(this, "OnEnter");
			m_OnExit = WDEEventScriptDef.Create(this, "OnExit");
			m_OnValidate = WDEEventScriptDef.Create(this, "OnValidate");
			m_ControlDefs = WDEControlDefs.Create(this);
		}

		public static IWDEDetailGridDef Create()
		{
			return new WDEDetailGridDef() as IWDEDetailGridDef;
		}

		public static IWDEDetailGridDef CreateInstance()
		{
			return Create();
		}

		protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			if(LinkedItem == m_RecordDef)
				m_RecordDef = null;
		}

		protected override void InternalClearNotify()
		{
			m_HeaderControlDefs.Clear();
		}

		public override ArrayList GetChildCollections()
		{
			ArrayList al = new ArrayList();
			al.Add(m_HeaderControlDefs);
			al.Add(m_ControlDefs);
			return al;
		}


		#region IWDEDetailGridDef Members

		public IWDERecordDef RecordDef
		{
			get
			{
				return m_RecordDef;
			}
			set
			{
				if(m_RecordDef != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_RecordDef;
					obj.RemoveLink(this);
				}

				m_RecordDef = value;

				if(m_RecordDef != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_RecordDef;
					obj.AddLink(this);
				}
			}
		}

		public IWDEControlDefs HeaderControlDefs
		{
			get
			{
				return m_HeaderControlDefs;
			}
		}

		public Color HeaderBackColor
		{
			get
			{
				return m_HeaderBackColor;
			}
			set
			{
				m_HeaderBackColor = value;
			}
		}

		public Color HeaderForeColor
		{
			get
			{
				return m_HeaderForeColor;
			}
			set
			{
				m_HeaderForeColor = value;
			}
		}

		public int HeaderHeight
		{
			get
			{
				return m_HeaderHeight;
			}
			set
			{
				m_HeaderHeight = value;
			}
		}

		public Font HeaderFont
		{
			get
			{
				return Utils.StringToFont(m_HeaderFont);
			}
			set
			{
				m_HeaderFont = Utils.FontToString(value);
			}
		}

		public WebDX.Api.WDEDetailGridOption Options
		{
			get
			{
				return m_Options;
			}
			set
			{
				m_Options = value;
			}
		}

		public int Rows
		{
			get
			{
				return m_Rows;
			}
			set
			{
				m_Rows = value;
			}
		}

		public WebDX.Api.WDERecordNumberPosition RecordNumberPosition
		{
			get
			{
				return m_RecordNumberPosition;
			}
			set
			{
				m_RecordNumberPosition = value;
			}
		}

		public IWDEDetailZoneDef DetailZoneDef
		{
			get
			{
				return m_DetailZoneDef;
			}
			set
			{
				m_DetailZoneDef = value;
			}
		}

		public IWDEEventScriptDef OnEnter
		{
			get
			{
				return m_OnEnter;
			}
		}

		public IWDEEventScriptDef OnExit
		{
			get
			{
				return m_OnExit;
			}
		}

		public IWDEEventScriptDef OnValidate
		{
			get
			{
				return m_OnValidate;
			}
		}

		public IWDEControlDefs ControlDefs
		{
			get
			{
				return m_ControlDefs;
			}
            set
            {
                m_ControlDefs = value;
            }
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			IWDEProjectInternal proj = null;
            if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                proj = (IWDEProjectInternal)base.TopParent();
            else
                throw new WDEException("API90006", new object[] {"DetailGridDef"});
            

			XmlReader.MoveToContent();
			if(ConvertingOldProject)
			{
				ReadOldDetailGrid(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DetailGridDef"))
				{
					base.ReadXmlAttributes(XmlReader);
					m_HeaderHeight = Utils.GetIntValue(XmlReader, "HeaderHeight");
					m_HeaderFont = Utils.GetAttribute(XmlReader, "HeaderFont", "Tahoma,10,Regular");
					m_HeaderBackColor = Utils.GetColor(XmlReader, "HeaderBackColor", HEADER_BACK_COLOR);
					m_HeaderForeColor = Utils.GetColor(XmlReader, "HeaderForeColor", HEADER_FORE_COLOR);
					m_Options = Utils.GetDetailGridOption(XmlReader, "Options");
					m_Rows = Utils.GetIntValue(XmlReader, "Rows");
					m_RecordNumberPosition = Utils.GetRecordNumberPosition(XmlReader, "RecordNumberPosition");
					string temp = Utils.GetAttribute(XmlReader, "RecordDef", "");
					if(temp != "")
						proj.Resolver.AddRequest(this, "RecordDef", temp);
					temp = Utils.GetAttribute(XmlReader, "DetailZoneDef", "");
					if(temp != "")
						proj.Resolver.AddRequest(this, "DetailZoneDef", temp);

					XmlReader.Read();
					XmlReader.MoveToContent();

					IWDEXmlPersist ipers = null;

					if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Header"))
					{
						XmlReader.Read();
						XmlReader.MoveToContent();
						ipers = (IWDEXmlPersist) m_HeaderControlDefs;
						ipers.ReadFromXml(XmlReader);

						if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Header"))
						{
							XmlReader.ReadEndElement();
							XmlReader.MoveToContent();
						}
					}

					if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Detail"))
					{
						XmlReader.Read();
						XmlReader.MoveToContent();
						ipers = (IWDEXmlPersist) ControlDefs;
						ipers.ReadFromXml(XmlReader);

						if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Detail"))
						{
							XmlReader.ReadEndElement();
							XmlReader.MoveToContent();
						}
					}

					ipers = (IWDEXmlPersist) m_OnEnter;
					ipers.ReadFromXml(XmlReader);

					ipers = (IWDEXmlPersist) m_OnValidate;
					ipers.ReadFromXml(XmlReader);

					ipers = (IWDEXmlPersist) m_OnExit;
					ipers.ReadFromXml(XmlReader);

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "DetailGridDef"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

        public void WriteToXmlFlat(XmlTextWriter xmlWriter)
        {
            m_WriteFlat = true;
            try
            {
                WriteToXml(xmlWriter);
            }
            finally
            {
                m_WriteFlat = false;
            }
        }

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("DetailGridDef");
			base.WriteXmlAttributes(XmlWriter);
			if(m_HeaderHeight != 0)
				XmlWriter.WriteAttributeString("HeaderHeight", m_HeaderHeight.ToString());
			XmlWriter.WriteAttributeString("HeaderFont", m_HeaderFont);
			if(m_HeaderBackColor != HEADER_BACK_COLOR)
				XmlWriter.WriteAttributeString("HeaderBackColor", m_HeaderBackColor.Name);
			if(m_HeaderForeColor != HEADER_FORE_COLOR)
				XmlWriter.WriteAttributeString("HeaderForeColor", m_HeaderForeColor.Name);
			if(m_Options != WDEDetailGridOption.None)
				XmlWriter.WriteAttributeString("Options", Options.ToString());
			if(m_Rows != 0)
				XmlWriter.WriteAttributeString("Rows", m_Rows.ToString());

            //if(RecordNumberPosition != WDERecordNumberPosition.Right) //Commented to fix O-508 
				XmlWriter.WriteAttributeString("RecordNumberPosition", RecordNumberPosition.ToString());

            //Avoid writing the "DetailZoneDef" attribute to the "DetailGridDef" element when there are no ZoneDefs
            if (DetailZoneDef != null && DetailZoneDef.ZoneDefs.Count > 0)
			{
				WDEBaseCollectionItem obj = (WDEBaseCollectionItem) DetailZoneDef;
				XmlWriter.WriteAttributeString("DetailZoneDef", obj.GetNamePath());
			}
			if(RecordDef != null)
			{
				WDEBaseCollectionItem obj = (WDEBaseCollectionItem) RecordDef;
				XmlWriter.WriteAttributeString("RecordDef", obj.GetNamePath());
			}

            IWDEXmlPersist ipers = null;
            if (!m_WriteFlat)
            {
                XmlWriter.WriteStartElement("Header");
                ipers = (IWDEXmlPersist)m_HeaderControlDefs;
                ipers.WriteToXml(XmlWriter);
                XmlWriter.WriteEndElement();

                XmlWriter.WriteStartElement("Detail");
                ipers = (IWDEXmlPersist)ControlDefs;
                ipers.WriteToXml(XmlWriter);
                XmlWriter.WriteEndElement();
            }

			ipers = (IWDEXmlPersist) m_OnEnter;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist) m_OnValidate;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist) m_OnExit;
			ipers.WriteToXml(XmlWriter);

			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Members

		private void ReadOldDetailGrid(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DetailGridDef"))
			{
				IWDEProjectInternal iproj = GetProjectInternal();
                if (iproj == null)
                    throw new WDEException("API90006", new object[] {"DetailGridDef"});

				ControlName = Utils.GetAttribute(XmlReader, "Name", "");
				string temp = Utils.GetAttribute(XmlReader, "DataRecord", "");
				if(temp != "")
					iproj.Resolver.AddRequest(this, "RecordDef", temp);

				KeyOrder = Utils.GetIntValue(XmlReader, "KeyOrder");
				Options = Utils.GetDetailGridOption(XmlReader, "Options");
				Rows = Utils.GetIntValue(XmlReader, "Rows");
				if(!Utils.GetBoolValue(XmlReader, "ShowHeader", false))
					Options |= WDEDetailGridOption.HideHeader;
				TabStop = Utils.GetBoolValue(XmlReader, "TabStop", false);
				RecordNumberPosition = Utils.GetRecordNumberPosition(XmlReader, "ShowRecordNumber");

				XmlReader.Read();
				XmlReader.MoveToContent();

				ReadDescription(XmlReader);
				ReadBackColor(XmlReader);
				ReadFont(XmlReader);
				ReadHeaderColor(XmlReader);
				ReadHeaderFont(XmlReader);
				HeaderHeight = Utils.StrToIntDef(ReadEnglishValue(XmlReader, "HeaderHeights"), 0);
				int h = Utils.StrToIntDef(ReadEnglishValue(XmlReader, "Heights"), 0);
				ReadHelp(XmlReader);
				ReadHint(XmlReader);
				int l = Utils.StrToIntDef(ReadEnglishValue(XmlReader, "Lefts"), 0);
				ReadSelectedColor(XmlReader);
				int t = Utils.StrToIntDef(ReadEnglishValue(XmlReader, "Tops"), 0);
				int w = Utils.StrToIntDef(ReadEnglishValue(XmlReader, "Widths"), 0);
				l = (int) (l * 1.1);
				w = (int) (w * 1.1);
				Location = new Rectangle(l, t, w, h);
				ReadDetailZoneLink(XmlReader);

				ReadScripts(XmlReader);
				ReadHeaderControls(XmlReader);
				ReadDetailControls(XmlReader);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "DetailGridDef"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
			else if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DEGridDef"))
			{
				ReadDEGridDef(XmlReader);
			}
		}

		private void ReadDEGridDef(XmlTextReader XmlReader)
		{
			IWDEProjectInternal iproj = GetProjectInternal();
            if (iproj == null)
                throw new WDEException("API90006", new object[] {"DetailGridDef"});

			HeaderHeight = HeaderFont.Height + 2;
			ControlName = Utils.GetAttribute(XmlReader, "Name", "");
			string temp = Utils.GetAttribute(XmlReader, "DataRecord", "");
			if(temp != "")
				iproj.Resolver.AddRequest(this, "RecordDef", temp);
			KeyOrder = Utils.GetIntValue(XmlReader, "KeyOrder");
			TabStop = Utils.GetBoolValue(XmlReader, "TabStop", false);

			XmlReader.Read();
			XmlReader.MoveToContent();

			ReadBackColor(XmlReader);
			ReadDEColumns(XmlReader);
			ReadDescription(XmlReader);
			ReadFont(XmlReader);
			int h = Utils.StrToIntDef(ReadEnglishValue(XmlReader, "Heights"), 0);
			ReadHelp(XmlReader);
			ReadHint(XmlReader);
			int l = Utils.StrToIntDef(ReadEnglishValue(XmlReader, "Lefts"), 0);
			ReadTitleFont(XmlReader);
			int t = Utils.StrToIntDef(ReadEnglishValue(XmlReader, "Tops"), 0);
			int w = Utils.StrToIntDef(ReadEnglishValue(XmlReader, "Widths"), 0);
			
			l = (int) (l * 1.1);
			w = (int) (w * 1.1);
			Location = new Rectangle(l, t, w, h);
			ReadDetailZoneLink(XmlReader);
			ReadScripts(XmlReader);

			this.Rows = (int) Math.Round((double) (h / 24));

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "DEGridDef"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		private void ReadDEColumns(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Columns"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				int l = 0;
				int keyOrder = 0;
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "Column"))
				{
					IWDETextBoxDef def = WDETextBoxDef.Create();
					((IWDEControlDefsInternal) ControlDefs).Add(def);
					IWDEXmlPersist ipers = (IWDEXmlPersist) def;
					ipers.ReadFromXml(XmlReader);
					Rectangle temp = def.Location;
					temp.Offset(l, 0);
					def.Location = temp;
					def.KeyOrder = keyOrder++;

					string title = def.Help;
					def.Help = "";
					IWDELabelDef ldef = WDELabelDef.Create();
					((IWDEControlDefsInternal) HeaderControlDefs).Add(ldef);
					ldef.ControlName = "Label" + def.ControlName;
					ldef.Caption = title;
					ldef.ControlFont = HeaderFont;
					ldef.ForeColor = HeaderForeColor;
					ldef.Location = new Rectangle(l + 2, 0, def.Location.Width, HeaderFont.Height);

					l += def.Location.Width + 1;
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Columns"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ReadTitleFont(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "TitleFonts"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "Font"))
				{
					if(Utils.GetAttribute(XmlReader, "Name", "") == "ENGLISH")
					{
						HeaderForeColor = Utils.GetColor(XmlReader, "Color", Color.FromKnownColor(KnownColor.WindowText));
						string name = Utils.GetAttribute(XmlReader, "FontName", "");
						float height = Utils.ConvertFontHeight(Utils.GetIntValue(XmlReader, "Height"));
						FontStyle style = Utils.ConvertFontStyle(Utils.GetAttribute(XmlReader, "Style", ""), XmlReader.LineNumber, XmlReader.LinePosition);

                        using (Font temp = new Font(name, height, style))
                            m_HeaderFont = Utils.FontToString(temp);
					}

					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "TitleFonts"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private string ReadEnglishValue(XmlTextReader XmlReader, string Key)
		{
			string result = "";
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == Key))
			{
				result = Utils.GetAttribute(XmlReader, "ENGLISH", "");
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
			return result;
		}

		private void ReadHeaderColor(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "HeaderColors"))
			{
				HeaderBackColor = Utils.GetColor(XmlReader, "ENGLISH", Color.FromKnownColor(KnownColor.Control));
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}	
		}

		private void ReadHeaderFont(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "HeaderFonts"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "Font"))
				{
					if(Utils.GetAttribute(XmlReader, "Name", "") == "ENGLISH")
					{	
						HeaderForeColor = Utils.GetColor(XmlReader, "Color", Color.FromKnownColor(KnownColor.WindowText));
						string name = Utils.GetAttribute(XmlReader, "FontName", "");
						float height = Utils.ConvertFontHeight(Utils.GetIntValue(XmlReader, "Height"));
						FontStyle style = Utils.ConvertFontStyle(Utils.GetAttribute(XmlReader, "Style", ""), XmlReader.LineNumber, XmlReader.LinePosition);

                        using (Font temp = new Font(name, height, style))
                            m_HeaderFont = Utils.FontToString(temp);
					}

					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "HeaderFonts"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ReadSelectedColor(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "SelectedColors"))
			{
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}	
		}

		private void ReadScripts(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Scripts"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "EventScript"))
				{
					string temp = Utils.GetAttribute(XmlReader, "Name", "");
					IWDEXmlPersist ipers = null;
					switch(temp)
					{
						case "OnEnter":
							ipers = (IWDEXmlPersist) m_OnEnter;
							break;
						case "OnExit":
							ipers = (IWDEXmlPersist) m_OnExit;
							break;
						case "OnValidate":
							ipers = (IWDEXmlPersist) m_OnValidate;
							break;
						default:
							throw new XmlException("Invalid script name in DetailGrid", null, XmlReader.LineNumber, XmlReader.LinePosition);
					}
					ipers.ReadFromXml(XmlReader);
				}
			}

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Scripts"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		private void ReadHeaderControls(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Header"))
			{
				string name = Utils.GetAttribute(XmlReader, "Name", "");

				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
				{
					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				IWDEXmlPersist ipers = (IWDEXmlPersist) HeaderControlDefs;
				ipers.ReadFromXml(XmlReader);

				SetNameConversions("Header", name, HeaderControlDefs);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Header"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ReadDetailControls(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Detail"))
			{

				string name = Utils.GetAttribute(XmlReader, "Name", "");

				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
				{
					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				IWDEXmlPersist ipers = (IWDEXmlPersist) ControlDefs;
				ipers.ReadFromXml(XmlReader);

				SetNameConversions("Detail", name, ControlDefs);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Detail"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ReadDetailZoneLink(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ZoneLinks"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				IWDEProjectInternal iproj = GetProjectInternal();
                if (iproj == null)
                        throw new WDEException("API90006", new object[] {"DetailGridDef"});

				bool linkAdded = false;
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "ZoneLink"))
				{
					if(!linkAdded)
					{
						string temp = Utils.GetAttribute(XmlReader, "Zone", "");
						if(temp != "")
						{
							temp = PrependFormName(temp);
							string convertName = temp;
							int i = convertName.LastIndexOf(".");
							convertName = convertName.Substring(0, i);
							iproj.Resolver.AddRequest(this, "DetailZoneDef", temp, -1, convertName);
							linkAdded = true;
						}
					}

					XmlReader.Read();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ZoneLinks"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void SetNameConversions(string InsertName, string xmlName, IWDEControlDefs defs)
		{
			IWDEProjectInternal iproj = GetProjectInternal();

			foreach(WDEBaseCollectionItem item in defs)
			{
				string curName = item.GetNamePath();
				int p = curName.LastIndexOf(".");
				string newName = curName.Substring(0, p) + "." + InsertName + curName.Substring(p);
				iproj.Resolver.AddConvertLink(newName, curName);
				if(xmlName != "")
				{
					newName = curName.Substring(0, p) + "." + xmlName + curName.Substring(p);
					iproj.Resolver.AddConvertLink(newName, curName);
				}
			}
		}

		private IWDEDetailZoneDef GetDetailDef(object parent)
		{
			string name = ((WDEBaseCollectionItem) parent).GetNamePath();

			while((parent != null) && (!(parent is IWDEImageSourceDef)))
				parent = ((WDEBaseCollectionItem) parent).Parent;

            if (parent == null)
                    throw new WDEException("API90013", new object[] {name});
            else
            {
                return ((IWDEImageSourceDef)parent).DetailZoneDefs.Add();
            }
		}

		#endregion

		#region IWDELinkConverter Members

		public object ConvertLinkObject(object linkTarget)
		{
			if(linkTarget is IWDEDetailZoneDef)
			{
				IWDEDetailZoneDef detailDef = (IWDEDetailZoneDef) linkTarget;
				if(detailDef.ZoneDefs.Count == 0)
				{
					IWDEZoneDef newDef = detailDef.ZoneDefs.Add("Zone1");
					Rectangle zoneRect = ((IWDEDetailZoneDefInternal) detailDef).ZoneRect;
					newDef.ZoneRect = new Rectangle(zoneRect.Left, zoneRect.Top, zoneRect.Width, detailDef.LineHeight);
				}

				return detailDef;
			}
			else if((linkTarget is IWDEZoneDef) || (linkTarget is IWDESnippetDef))
			{
				string name;
				Rectangle origRect;
				if(linkTarget is IWDEZoneDef)
				{
					name = ((IWDEZoneDef) linkTarget).Name;
					origRect = ((IWDEZoneDef) linkTarget).ZoneRect;
				}
				else
				{
					name = ((IWDESnippetDef) linkTarget).Name;
					origRect = ((IWDESnippetDef) linkTarget).SnippetRect;
				}

				IWDEDetailZoneDef detailDef = GetDetailDef(((WDEBaseCollectionItem) linkTarget).Parent);
				IWDEZoneDef targetDef = detailDef.ZoneDefs.Add(name);
				targetDef.ZoneRect = origRect;
				detailDef.LineCount = 1;
				detailDef.LineHeight = origRect.Height;

				return detailDef;
			}
			else
				return linkTarget;
		}

		#endregion
	}

    public class WDEImageFormDef : WDEBaseCollectionItem, IWDEImageFormDef, IWDEXmlPersist
    {
        private Color m_BackColor;
        private Color m_ForeColor;
        private IWDEControlDefs m_ControlDefs;
        private IWDERecordDef m_RecordDef;
        private string m_Desc;
        private string m_FormFont;
        private string m_Help;
        private string m_Hint;
        private IWDEImageSourceDef m_ImageType;
        private string m_ImageTypeName;
        private IWDEEventScriptDef m_OnEnter;
        private IWDEEventScriptDef m_OnExit;

        private WDEImageFormDef()
        {
            m_BackColor = Color.FromKnownColor(KnownColor.Control);
            m_ForeColor = Color.FromKnownColor(KnownColor.WindowText);
            m_Desc = "";
            m_Help = "";
            m_Hint = "";
            m_OnEnter = WDEEventScriptDef.Create(this, "OnEnter");
            m_OnExit = WDEEventScriptDef.Create(this, "OnExit");

            m_ImageTypeName = "";

            m_ControlDefs = WDEControlDefs.Create(this);
            m_FormFont = "Tahoma, 10pt";
        }

        public static IWDEImageFormDef Create()
        {
            return new WDEImageFormDef() as IWDEImageFormDef;
        }

        public static IWDEImageFormDef CreateInstance()
        {
            return Create();
        }

        protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
        {
            if (LinkedItem == m_RecordDef)
                m_RecordDef = null;
            if (LinkedItem == m_ImageType)
                m_ImageType = null;
        }

        protected override string InternalGetNodeName()
        {
            return m_ImageTypeName;
        }

        protected override void InternalClearNotify()
        {
            m_ControlDefs.Clear();
        }

        public override ArrayList GetChildCollections()
        {
            ArrayList al = new ArrayList();
            al.Add(m_ControlDefs);
            return al;
        }

        #region IWDEFormDef Members

        public Color BackColor
        {
            get
            {
                return m_BackColor;
            }
            set
            {
                m_BackColor = value;
            }
        }

        public Color ForeColor
        {
            get
            {
                return m_ForeColor;
            }
            set
            {
                m_ForeColor = value;
            }
        }

        public IWDEControlDefs ControlDefs
        {
            get
            {
                return m_ControlDefs;
            }
        }

        public IWDERecordDef RecordDef
        {
            get
            {
                return m_RecordDef;
            }
            set
            {
                WDEBaseCollectionItem def = (WDEBaseCollectionItem)m_RecordDef;
                if (def != null)
                    def.RemoveLink(this);

                m_RecordDef = value;

                if (value != null)
                {
                    def = (WDEBaseCollectionItem)m_RecordDef;
                    def.AddLink(this);
                }
            }
        }

        public string Description
        {
            get
            {
                return m_Desc;
            }
            set
            {
                if (value == null)
                    m_Desc = "";
                else
                    m_Desc = value;
            }
        }

        public Font FormFont
        {
            get
            {
                return Utils.StringToFont(m_FormFont);
            }
            set
            {
                m_FormFont = Utils.FontToString(value);
            }
        }

        public string Help
        {
            get
            {
                return m_Help;
            }
            set
            {
                if (value == null)
                    m_Help = "";
                else
                    m_Help = value;
            }
        }

        public string Hint
        {
            get
            {
                return m_Hint;
            }
            set
            {
                if (value == null)
                    m_Hint = "";
                else
                    m_Hint = value;
            }
        }

        public IWDEImageSourceDef ImageSourceDef
        {
            get
            {
                return m_ImageType;
            }
            set
            {
                m_ImageType = value;
                m_ImageTypeName = value.ImageType;
            }
        }

        #endregion

        public IWDEEventScriptDef OnEnter
        {
            get
            {
                return m_OnEnter;
            }
            set
            {
                m_OnEnter = value;
            }
        }

        public IWDEEventScriptDef OnExit
        {
            get
            {
                return m_OnExit;
            }
            set
            {
                m_OnExit = value;
            }
        }

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
        {
            if (XmlReader == null)
                throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");

            IWDEProjectInternal proj = null;
            if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                proj = (IWDEProjectInternal)base.TopParent();
            else
                throw new WDEException("API90006", new object[] { "ImageFormDef" });


            XmlReader.MoveToContent();
            if ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ImageFormDef"))
            {
                m_BackColor = Utils.GetColor(XmlReader, "BackColor", Color.FromKnownColor(KnownColor.Control));
                m_ForeColor = Utils.GetColor(XmlReader, "ForeColor", Color.FromKnownColor(KnownColor.WindowText));
                string temp = Utils.GetAttribute(XmlReader, "Record", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "RecordDef", temp);

                temp = Utils.GetAttribute(XmlReader, "ImageType", "");
                if (temp != "")
                {
                    string[] imageTypeParts = temp.Split('.');
                    m_ImageTypeName = imageTypeParts[imageTypeParts.Length - 1];
                    proj.Resolver.AddRequest(this, "ImageSourceDef", temp);
                }

                m_Desc = Utils.GetAttribute(XmlReader, "Description", "");
                m_FormFont = Utils.GetAttribute(XmlReader, "Font", "Tahoma,10,Regular");
                m_Help = Utils.GetAttribute(XmlReader, "Help", "");
                m_Hint = Utils.GetAttribute(XmlReader, "Hint", "");

                XmlReader.Read();
                XmlReader.MoveToContent();

                IWDEXmlPersist ipers = (IWDEXmlPersist)ControlDefs;
                ipers.ReadFromXml(XmlReader);

                ipers = (IWDEXmlPersist)m_OnEnter;
                ipers.ReadFromXml(XmlReader);

                ipers = (IWDEXmlPersist)m_OnExit;
                ipers.ReadFromXml(XmlReader);

                if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ImageFormDef"))
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

            XmlWriter.WriteStartElement("ImageFormDef");
            if (BackColor != Color.FromKnownColor(KnownColor.Control))
                XmlWriter.WriteAttributeString("BackColor", BackColor.Name);
            if (ForeColor != Color.FromKnownColor(KnownColor.WindowText))
                XmlWriter.WriteAttributeString("ForeColor", ForeColor.Name);
            if (RecordDef != null)
            {
                WDEBaseCollectionItem obj = (WDEBaseCollectionItem)RecordDef;
                XmlWriter.WriteAttributeString("Record", obj.GetNamePath());
            }

            if (ImageSourceDef != null)
            {
                WDEBaseCollectionItem obj = (WDEBaseCollectionItem)ImageSourceDef;
                XmlWriter.WriteAttributeString("ImageType", obj.GetNamePath());
            }

            if (Description != "")
                XmlWriter.WriteAttributeString("Description", Description);
            XmlWriter.WriteAttributeString("Font", m_FormFont);
            if (Help != "")
                XmlWriter.WriteAttributeString("Help", Help);
            if (Hint != "")
                XmlWriter.WriteAttributeString("Hint", Hint);

            IWDEXmlPersist ipers = (IWDEXmlPersist)ControlDefs;
            ipers.WriteToXml(XmlWriter);

            ipers = (IWDEXmlPersist)m_OnEnter;
            ipers.WriteToXml(XmlWriter);

            ipers = (IWDEXmlPersist)m_OnExit;
            ipers.WriteToXml(XmlWriter);

            XmlWriter.WriteEndElement();
        }

        #endregion
    }

    public class WDEImageFormDefs : WDEBaseCollection, IWDEImageFormDefs, IWDEXmlPersist
    {
        private WDEImageFormDefs(object Parent)
            : base(Parent)
        {
        }

        public static IWDEImageFormDefs Create(IWDEImageDataDef Parent)
        {
            return new WDEImageFormDefs(Parent) as IWDEImageFormDefs;
        }

        public static IWDEImageFormDefs CreateInstance(IWDEImageDataDef Parent)
        {
            return Create(Parent);
        }

        protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
        {
            IWDEImageFormDef def = (IWDEImageFormDef)Item;
            return (string.Compare(def.ImageSourceDef.ImageType, Name, true) == 0);
        }

        // We should only care about uniqueness within the same document. I.E. within this collection.
        //protected override ArrayList GetCollectionList()
        //{
        //    return base.GetSameLevelCollections();
        //}

        #region IWDEImageFormDefs Members

        public IWDEImageFormDef this[int index]
        {
            get
            {
                return (IWDEImageFormDef)base.InternalGetIndex(index);
            }
        }

        public int Find(string ImageType)
        {
            return base.InternalIndexOf(ImageType);
        }

        public IWDEImageFormDef Add(string ImageType)
        {
            int res = base.VerifyName(ImageType);
            if (res == 0)
            {
                IWDEImageFormDef newDef = WDEImageFormDef.Create();
                base.InternalAdd((WDEBaseCollectionItem)newDef);
                IWDEDocumentDef docdef = ((WDEBaseCollectionItem)this.Parent).Parent as IWDEDocumentDef;
                foreach (IWDEImageSourceDef imgdef in docdef.ImageSourceDefs)
                {
                    if (imgdef.ImageType == ImageType)
                    {
                        newDef.ImageSourceDef = imgdef;
                        break;
                    }
                }
                return newDef;
            }
            else if (res == -1)
                throw new WDEException("API00037", new object[] { ImageType });
            else
                throw new WDEException("API00038", new object[] { ImageType });
        }

        #endregion

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
        {
            if (XmlReader == null)
                throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");

            XmlReader.MoveToContent();
            if ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ImageFormDef"))
            {
                Clear();

                while ((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
                    (XmlReader.Name == "ImageFormDef"))
                {
                    IWDEImageFormDef def = WDEImageFormDef.Create();
                    base.InternalAdd((WDEBaseCollectionItem)def);
                    IWDEXmlPersist ipers = (IWDEXmlPersist)def;
                    ipers.ReadFromXml(XmlReader);
                    base.RegisterObject((WDEBaseCollectionItem)def);
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
    }

    public class WDEImageDataDef : WDEControlDef, IWDEImageDataDef, IWDEXmlPersist, IWDEXmlPersistFlat
    {
        private const int DEFAULT_FIXED_HEIGHT = 30;

        private int m_HeaderHeight;
        private string m_HeaderFont;
        private IWDEEventScriptDef m_OnEnter;
        private IWDEEventScriptDef m_OnExit;
        private IWDEImageFormDefs m_FormDefs;
        private bool m_WriteFlat;

        private WDEImageDataDef()
        {
            m_HeaderFont = "Tahoma, 10pt";
            m_OnEnter = WDEEventScriptDef.Create(this, "OnEnter");
            m_OnExit = WDEEventScriptDef.Create(this, "OnExit");
            m_FormDefs = WDEImageFormDefs.Create(this);
        }

        public static IWDEImageDataDef Create()
        {
            return new WDEImageDataDef() as IWDEImageDataDef;
        }

        public static IWDEImageDataDef CreateInstance()
        {
            return Create();
        }

        protected override void InternalClearNotify()
        {
        }

        public override ArrayList GetChildCollections()
        {
            ArrayList al = new ArrayList();
            al.Add(m_FormDefs);
            return al;
        }


        #region IWDEImageDataDef Members

        public int HeaderHeight
        {
            get
            {
                return m_HeaderHeight;
            }
            set
            {
                m_HeaderHeight = value;
            }
        }

        public Font HeaderFont
        {
            get
            {
                return Utils.StringToFont(m_HeaderFont);
            }
            set
            {
                m_HeaderFont = Utils.FontToString(value);
            }
        }

        public IWDEEventScriptDef OnEnter
        {
            get
            {
                return m_OnEnter;
            }
        }

        public IWDEEventScriptDef OnExit
        {
            get
            {
                return m_OnExit;
            }
        }

        public IWDEImageFormDefs FormDefs
        {
            get
            {
                return m_FormDefs;
            }
            set
            {
                m_FormDefs = value;
            }
        }

        #endregion

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
        {
            if (XmlReader == null)
                throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");

            IWDEProjectInternal proj = null;
            if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                proj = (IWDEProjectInternal)base.TopParent();
            else
                throw new WDEException("API90006", new object[] { "ImageDataDef" });


            XmlReader.MoveToContent();
            if ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ImageDataDef"))
            {
                base.ReadXmlAttributes(XmlReader);
                m_HeaderHeight = Utils.GetIntValue(XmlReader, "HeaderHeight");
                m_HeaderFont = Utils.GetAttribute(XmlReader, "HeaderFont", "Tahoma,10,Regular");

                XmlReader.Read();
                XmlReader.MoveToContent();

                IWDEXmlPersist ipers = null;

                ipers = (IWDEXmlPersist)FormDefs;
                ipers.ReadFromXml(XmlReader);

                ipers = (IWDEXmlPersist)m_OnEnter;
                ipers.ReadFromXml(XmlReader);

                ipers = (IWDEXmlPersist)m_OnExit;
                ipers.ReadFromXml(XmlReader);

                if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ImageDataDef"))
                {
                    XmlReader.ReadEndElement();
                    XmlReader.MoveToContent();
                }
            }
        }

        public void WriteToXmlFlat(XmlTextWriter xmlWriter)
        {
            m_WriteFlat = true;
            try
            {
                WriteToXml(xmlWriter);
            }
            finally
            {
                m_WriteFlat = false;
            }
        }

        public void WriteToXml(XmlTextWriter XmlWriter)
        {
            if (XmlWriter == null)
                throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");

            XmlWriter.WriteStartElement("ImageDataDef");
            base.WriteXmlAttributes(XmlWriter);
            if (m_HeaderHeight != 0)
                XmlWriter.WriteAttributeString("HeaderHeight", m_HeaderHeight.ToString());
            XmlWriter.WriteAttributeString("HeaderFont", m_HeaderFont);

            IWDEXmlPersist ipers = null;
            if (!m_WriteFlat)
            {
                ipers = (IWDEXmlPersist)FormDefs;
                ipers.WriteToXml(XmlWriter);
            }

            ipers = (IWDEXmlPersist)m_OnEnter;
            ipers.WriteToXml(XmlWriter);

            ipers = (IWDEXmlPersist)m_OnExit;
            ipers.WriteToXml(XmlWriter);

            XmlWriter.WriteEndElement();
        }

        #endregion
    }

	public class WDEPhotoStitchDef : WDEBaseCollectionItem, IWDEPhotoStitchDef, IWDEXmlPersist
	{
		private int m_ColCount;
		private int m_RowCount;
		private WDEPSOrientation m_Orientation;

		private WDEPhotoStitchDef() : base()
		{
			m_Orientation = WDEPSOrientation.Horizontal;
		}

		public static IWDEPhotoStitchDef Create()
		{
			return new WDEPhotoStitchDef() as IWDEPhotoStitchDef;
		}

		public static IWDEPhotoStitchDef CreateInstance()
		{
			return Create();
		}

		protected override string InternalGetNodeName()
		{
			return "PhotoStitchDef";
		}

		#region IWDEPhotoStitchDef Members

		public int ColCount
		{
			get
			{
				return m_ColCount;
			}
			set
			{
				m_ColCount = value;
			}
		}

		public int RowCount
		{
			get
			{
				return m_RowCount;
			}
			set
			{
				m_RowCount = value;
			}
		}

		public WebDX.Api.WDEPSOrientation Orientation
		{
			get
			{
				return m_Orientation;
			}
			set
			{
				m_Orientation = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			string tag = "PhotoStitchDef";
			if(ConvertingOldProject)
				tag = "PhotoStitch";

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == tag))
			{
				m_ColCount = Utils.GetIntValue(XmlReader, "ColCount");
				m_RowCount = Utils.GetIntValue(XmlReader, "RowCount");
				m_Orientation = Utils.GetPSOrientation(XmlReader, "Orientation", WDEPSOrientation.Horizontal);

				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == tag))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("PhotoStitchDef");
			if(ColCount != 0)
				XmlWriter.WriteAttributeString("ColCount", ColCount.ToString());
			if(RowCount != 0)
				XmlWriter.WriteAttributeString("RowCount", RowCount.ToString());
			if(Orientation != WDEPSOrientation.Horizontal)
				XmlWriter.WriteAttributeString("Orientation", Orientation.ToString());
			XmlWriter.WriteEndElement();
		}

		#endregion
	}

    public class WDEErrorOverrides : WDEBaseCollection, IWDEErrorOverrides, IWDEXmlPersist
    {
        private WDEErrorOverrides(object Parent)
            : base(Parent)
        {
        }

        public static IWDEErrorOverrides Create(object Parent)
        {
            return new WDEErrorOverrides(Parent) as IWDEErrorOverrides;
        }

        public static IWDEErrorOverrides CreateInstance(object Parent)
        {
            return Create(Parent);
        }

        #region IWDEErrorOverrides Members

        public IWDEErrorOverride this[int index]
        {
            get
            {
                return (IWDEErrorOverride)base.InternalGetIndex(index);
            }
        }

        public void Add(IWDEErrorOverride ErrorOverride)
        {
            base.InternalAdd((WDEBaseCollectionItem)ErrorOverride);
        }


        #endregion

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
        {
            if (XmlReader == null)
                throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");

            XmlReader.MoveToContent();
            IWDEErrorOverride def = null;
            if ((XmlReader.NodeType == XmlNodeType.Element) && (ErrorOverrideConsts.IsErrorOverride(XmlReader.Name)))
            {
                Clear();

                while ((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
                    (ErrorOverrideConsts.IsErrorOverride(XmlReader.Name)))
                {
                    def = WDEErrorOverride.Create();
                    IWDEXmlPersist ipers = (IWDEXmlPersist)def;
                    base.InternalAdd((WDEBaseCollectionItem)def);
                    ipers.ReadFromXml(XmlReader);
                    base.RegisterObject((WDEBaseCollectionItem)def);
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
    }

    internal class ErrorOverrideConsts
    {
        public static bool IsErrorOverride(string overrideName)
        {
            switch (overrideName)
            {
                case "MustComplete":
                case "MustEnter":
                case "InputMask":
                case "CharSet":
                case "ErrorOverride":
                    return true;
                default:
                    return false;
            }
        }
    }

    public class WDEErrorOverride : WDEBaseCollectionItem, IWDEErrorOverride, IWDEXmlPersist
    {
        private WDEErrorOverrideType m_ErrorName;
        private string m_ErrorMessage;


        public WDEErrorOverride()
        {
            m_ErrorName = WDEErrorOverrideType.None;
            m_ErrorMessage = "";
        }

        public static IWDEErrorOverride Create()
        {
            return new WDEErrorOverride() as IWDEErrorOverride;
        }

        public static IWDEErrorOverride CreateInstance()
        {
            return Create();
        }

        protected override string InternalGetNodeName()
        {
            string nodeName = string.Empty;
            if (m_ErrorName != WDEErrorOverrideType.None)
                nodeName = m_ErrorName.ToString();
            return nodeName;
        }

        protected void ReadXmlAttributes(XmlTextReader XmlReader)
        {
            m_ErrorName = Utils.GetErrorOverrideType(XmlReader, "ErrorName"); 
            if(m_ErrorName != WDEErrorOverrideType.None)
                m_ErrorMessage = Utils.GetAttribute(XmlReader, "ErrorMessage", "");
        }

        protected void WriteXmlAttributes(XmlTextWriter XmlWriter)
        {
            if (XmlWriter == null)
                throw new ArgumentNullException("XmlWriter", "XmlWriter cannot be null");            

            if ((m_ErrorName != WDEErrorOverrideType.None) && !string.IsNullOrEmpty(m_ErrorMessage))
            {
                XmlWriter.WriteAttributeString("ErrorName", m_ErrorName.ToString());
                XmlWriter.WriteAttributeString("ErrorMessage", m_ErrorMessage);
            }
        }               

        #region IWDEErrorOverride Members

        public WDEErrorOverrideType ErrorName
        {
            get
            {
                return m_ErrorName;
            }
            set
            {
                m_ErrorName = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return m_ErrorMessage;
            }
            set
            {
                if (value == null)
                    m_ErrorMessage = "";
                else
                    m_ErrorMessage = value;
            }
        }       

        #endregion 

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
        {
            if (XmlReader == null)
                throw new ArgumentNullException("XmlReader");

            XmlReader.MoveToContent();
            if ((XmlReader.NodeType == XmlNodeType.Element) && (ErrorOverrideConsts.IsErrorOverride(XmlReader.Name)))
            {
                ReadXmlAttributes(XmlReader);

                if (XmlReader.Name == "ErrorOverride")
                {
                    XmlReader.Read();
                    XmlReader.MoveToContent();                    
                }
                else
                {                    
                    XmlReader.MoveToContent();
                }
            }
        }

        public void WriteToXml(XmlTextWriter XmlWriter)
        {
            if (XmlWriter == null)
                throw new ArgumentNullException("XmlWriter");

            XmlWriter.WriteStartElement("ErrorOverride");
            WriteXmlAttributes(XmlWriter);           

            XmlWriter.WriteEndElement();
        }

        #endregion
    }

	public class WDEEditDefs : WDEBaseCollection, IWDEEditDefs_R1, IWDEXmlPersist
	{
		private WDEEditDefs(object Parent) : base(Parent)
		{
		}

		public static IWDEEditDefs Create(object Parent)
		{
			return new WDEEditDefs(Parent) as IWDEEditDefs;
		}

		public static IWDEEditDefs CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		#region IWDEEditDefs Members

		public IWDEEditDef this[int index]
		{
			get
			{
				return (IWDEEditDef) base.InternalGetIndex(index);
			}
		}

		public void Add(IWDEEditDef EditDef)
		{
			base.InternalAdd((WDEBaseCollectionItem) EditDef);
		}

        public void Delete(IWDEEditDef EditDef)
        {
            base.Remove((WDEBaseCollectionItem)EditDef);
        }

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			IWDEEditDef def = null;
			if((XmlReader.NodeType == XmlNodeType.Element) && (EditDefConsts.IsEditDef(XmlReader.Name)))
			{
				Clear();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(EditDefConsts.IsEditDef(XmlReader.Name)))
				{
                    def = WDEEditDef.Create();
                    IWDEXmlPersist ipers = (IWDEXmlPersist)def;
                    base.InternalAdd((WDEBaseCollectionItem)def);
                    ipers.ReadFromXml(XmlReader);
                    base.RegisterObject((WDEBaseCollectionItem)def);
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion
	}

    internal class EditDefConsts
    {
        public static bool IsEditDef(string defName)
        {
            switch (defName)
            {
                case "AddressCorrectionEditDef":
                case "CheckDigitEditDef":
                case "ConditionalGotoEditDef":
                case "DateEditDef":
                case "DiagnosisPtrEditDef":
                case "RangeEditDef":
                case "RequiredEditDef":
                case "SimpleListEditDef":
                case "TableLookupEditDef":
                case "ValidLengthsEditDef":
                case "BalanceCheckEditDef":
                case "ValidateEditDef":
                case "ZipCodeLookupEditDef":
                case "EditDef":
                    return true;
                default:
                    return false;
            }
        }
    }

	public class WDEEditDef : WDEBaseCollectionItem, IWDEEditDef_R1, IWDEEditDefInternal, IWDEXmlPersist
	{
		private string m_DisplayName;
		private string m_FullName;
		private string m_Desc;
		private bool m_Enabled;
		private string m_ErrorMessage;
		private WDEEditErrorType m_ErrorType;
		private WDESessionType m_SessionType;
        private Scripts.IScriptEdit m_EditObject;
        private string m_EditParams;

		public WDEEditDef()
		{
			m_DisplayName = "";
			m_FullName = "";
			m_Desc = "";
			m_ErrorMessage = "";
            m_EditParams = "";
			m_ErrorType = WDEEditErrorType.Failure;
			m_SessionType = WDESessionType.None;
		}

        public static IWDEEditDef Create()
        {
            return new WDEEditDef() as IWDEEditDef;
        }

        public static IWDEEditDef CreateInstance()
        {
            return Create();
        }

		protected override string InternalGetNodeName()
		{
			return m_DisplayName;
		}

		protected void ReadXmlAttributes(XmlTextReader XmlReader)
		{
            m_DisplayName = Utils.GetAttribute(XmlReader, "DisplayName", "");
			m_Desc = Utils.GetAttribute(XmlReader, "Description", "");
			m_Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
			m_ErrorMessage = Utils.GetAttribute(XmlReader, "ErrorMessage", "");
			m_ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
			m_SessionType = Utils.GetSessionType(XmlReader, "SessionType");
            m_FullName = Utils.GetAttribute(XmlReader, "FullName", "");
		}

		protected void WriteXmlAttributes(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

            if (m_DisplayName != "")
                XmlWriter.WriteAttributeString("DisplayName", m_DisplayName);
			if(m_Desc != "")
				XmlWriter.WriteAttributeString("Description", m_Desc);
			if(m_Enabled)
				XmlWriter.WriteAttributeString("Enabled", m_Enabled.ToString());
			if(m_ErrorMessage != "")
				XmlWriter.WriteAttributeString("ErrorMessage", m_ErrorMessage);
			if(m_ErrorType != WDEEditErrorType.Failure)
				XmlWriter.WriteAttributeString("ErrorType", m_ErrorType.ToString());
			if(m_SessionType != WDESessionType.None)
				XmlWriter.WriteAttributeString("SessionType", m_SessionType.ToString());
            if (m_FullName != "")
                XmlWriter.WriteAttributeString("FullName", m_FullName);
		}

		protected void SetNames(string DisplayName, string FullName)
		{
			m_DisplayName = DisplayName;
			m_FullName = FullName;
		}

		protected void ReadDescription(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
			{
				Description = Utils.GetAttribute(XmlReader, "ENGLISH", "");

				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		protected void ReadErrorMessage(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ErrorMessages"))
			{
				ErrorMessage = Utils.GetAttribute(XmlReader, "ENGLISH", "");

				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		#region IWDEEditDef Members

		public string DisplayName
		{
			get
			{
				return m_DisplayName;
			}
            set
            {
                m_DisplayName = value == null ? "" : value;
            }
		}

		public string FullName
		{
			get
			{
				return m_FullName;
			}
            set
            {
                m_FullName = value == null ? "" : value;
            }
		}

		public string Description
		{
			get
			{
				return m_Desc;
			}
			set
			{
				if(value == null)
					m_Desc = "";
				else
					m_Desc = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return m_Enabled;
			}
			set
			{
				m_Enabled = value;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return m_ErrorMessage;
			}
			set
			{
				if(value == null)
					m_ErrorMessage = "";
				else
					m_ErrorMessage = value;
			}
		}

		public WebDX.Api.WDEEditErrorType ErrorType
		{
			get
			{
				return m_ErrorType;
			}
			set
			{
				m_ErrorType = value;
			}
		}

		public WebDX.Api.WDESessionType SessionMode
		{
			get
			{
				return m_SessionType;
			}
			set
			{
				m_SessionType = value;
			}
		}

		#endregion

        #region IWDEEditDef_R1 Members

        public string EditParams
        {
            get { return m_EditParams; }
            set { m_EditParams = value == null ? "" : value; }
        }

        #endregion

        #region IWDEEditDefInternal Members

        public WebDX.Api.Scripts.IScriptEdit EditObject
        {
            get
            {
                return m_EditObject;
            }
            set
            {
                m_EditObject = value;
                if (m_EditObject != null)
                {
                    m_EditObject.ReadFromXml(m_EditParams);
                }
            }
        }

        #endregion

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
        {
            if (XmlReader == null)
                throw new ArgumentNullException("XmlReader");

            XmlReader.MoveToContent();
            if ((XmlReader.NodeType == XmlNodeType.Element) && (EditDefConsts.IsEditDef(XmlReader.Name)))
            {
                ReadXmlAttributes(XmlReader);

                if (XmlReader.Name == "EditDef")
                {
                    XmlReader.Read();
                    XmlReader.MoveToContent();

                    if (!XmlReader.IsEmptyElement)
                    {
                        m_EditParams = XmlReader.ReadString();
                        XmlReader.ReadEndElement();
                        XmlReader.MoveToContent();
                    }
                }
                else
                {
                    m_EditParams = XmlReader.ReadOuterXml();
                    XmlReader.MoveToContent();
                }
            }
        }

        public void WriteToXml(XmlTextWriter XmlWriter)
        {
            if (XmlWriter == null)
                throw new ArgumentNullException("XmlWriter");

            XmlWriter.WriteStartElement("EditDef");
            WriteXmlAttributes(XmlWriter);

            if (m_EditParams != "")
            {
                XmlWriter.WriteCData(m_EditParams);
            }

            XmlWriter.WriteEndElement();
        }

        #endregion
    }

	public class WDECheckDigitEditDef : WDEEditDef, IWDECheckDigitEditDef, IWDEXmlPersist
	{
		private WDECheckDigitMethods m_Methods;

		private WDECheckDigitEditDef() : base()
		{
			m_Methods = WDECheckDigitMethods.None;
			SetNames("CheckDigit", "WebDX.Api.Scripts.CheckDigitEdit");
		}

		public static IWDECheckDigitEditDef Create()
		{
			return new WDECheckDigitEditDef() as IWDECheckDigitEditDef;
		}

		public static IWDECheckDigitEditDef CreateInstance()
		{
			return Create();
		}

		#region IWDECheckDigitEditDef Members

		public WebDX.Api.WDECheckDigitMethods Methods
		{
			get
			{
				return m_Methods;
			}
			set
			{
				m_Methods = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "CheckDigitEditDef"))
			{
				base.ReadXmlAttributes(XmlReader);
				m_Methods = Utils.GetCheckDigitMethods(XmlReader, "Methods");

				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "CheckDigitEditDef"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("CheckDigitEditDef");
			base.WriteXmlAttributes(XmlWriter);
			if(Methods != WDECheckDigitMethods.None)
				XmlWriter.WriteAttributeString("Methods", m_Methods.ToString());
			XmlWriter.WriteEndElement();
		}

		#endregion
	}

	public class WDERangeEditDef : WDEEditDef, IWDERangeEditDef, IWDEXmlPersist
	{
		private string m_HighRange;
		private string m_LowRange;

		private WDERangeEditDef() : base()
		{
			m_HighRange = "";
			m_LowRange = "";

			SetNames("Range", "WebDX.Api.Scripts.RangeEdit");
		}

		public static IWDERangeEditDef Create()
		{
			return new WDERangeEditDef() as IWDERangeEditDef;
		}

		public static IWDERangeEditDef CreateInstance()
		{
			return Create();
		}

		#region IWDERangeEditDef Members

		public string HighRange
		{
			get
			{
				return m_HighRange;
			}
			set
			{
				if(value == null)
					m_HighRange = "";
				else
					m_HighRange = value;
			}
		}

		public string LowRange
		{
			get
			{
				return m_LowRange;
			}
			set
			{
				if(value == null)
					m_LowRange = "";
				else
					m_LowRange = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "RangeEditDef"))
			{
				base.ReadXmlAttributes(XmlReader);
				m_HighRange = Utils.GetAttribute(XmlReader, "HighRange", "");
				m_LowRange = Utils.GetAttribute(XmlReader, "LowRange", "");

				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "RangeEditDef"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("RangeEditDef");
			base.WriteXmlAttributes(XmlWriter);
			if(HighRange != "")
				XmlWriter.WriteAttributeString("HighRange", HighRange);
			if(LowRange != "")
				XmlWriter.WriteAttributeString("LowRange", LowRange);
			XmlWriter.WriteEndElement();
		}

		#endregion
	}

	public class WDERequiredEditDef : WDEEditDef, IWDERequiredEditDef, IWDEXmlPersist
	{
		private string m_Expression;

		private WDERequiredEditDef() : base()
		{
			m_Expression = "";

			SetNames("Required", "WebDX.Api.Scripts.RequiredEdit");
		}

		public static IWDERequiredEditDef Create()
		{
			return new WDERequiredEditDef() as IWDERequiredEditDef;
		}

		public static IWDERequiredEditDef CreateInstance()
		{
			return Create();
		}

		#region IWDERequiredEditDef Members

		public string Expression
		{
			get
			{
				return m_Expression;
			}
			set
			{
				if(value == null)
					m_Expression = "";
				else
					m_Expression = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if(XmlReader.NodeType == XmlNodeType.Element) 
			{
				if(XmlReader.Name == "RequiredEditDef")
				{
					base.ReadXmlAttributes(XmlReader);
					string temp = Utils.GetAttribute(XmlReader, "Expression", "");
					if(temp != "")
					{
						IWDEProjectInternal iproj = GetProjectInternal();
                        if (iproj == null)
                            throw new WDEException("API90006", new object[] {"RequiredEditDef"});

						iproj.AppendOldExpression(temp, GetNamePath());
						m_Expression = GetNamePath();
					}

					XmlReader.Read();
					XmlReader.MoveToContent();

					if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "RequiredEditDef"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
				else if(XmlReader.Name == "MustEnter")
				{
					ReadOldRequiredEdit(XmlReader);
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("RequiredEditDef");
			base.WriteXmlAttributes(XmlWriter);
			if(Expression != "")
				XmlWriter.WriteAttributeString("Expression", Expression);
			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Methods

		private void ReadOldRequiredEdit(XmlTextReader XmlReader)
		{
			Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
			ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
			SessionMode = Utils.GetSessionType(XmlReader, "ViewModes");
			
			string temp = Utils.GetAttribute(XmlReader, "Expression", "");
			if(temp != "")
			{
				IWDEProjectInternal iproj = GetProjectInternal();
                if (iproj == null)
                    throw new WDEException("API90006", new object[] {"RequiredEditDef"});

				iproj.AppendOldExpression(temp, GetNamePath());
				Expression = GetNamePath();
			}

			XmlReader.Read();
			XmlReader.MoveToContent();

			ReadDescription(XmlReader);
			ReadErrorMessage(XmlReader);

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "MustEnter"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		#endregion
	}

	public class WDESimpleListEditDef : WDEEditDef, IWDESimpleListEditDef, IWDEXmlPersist
	{
		private string[] m_List;

		private WDESimpleListEditDef() : base()
		{
			SetNames("SimpleList", "WebDX.Api.Scripts.SimpleListEdit");
		}

		public static IWDESimpleListEditDef Create()
		{
			return new WDESimpleListEditDef() as IWDESimpleListEditDef;
		}

		public static IWDESimpleListEditDef CreateInstance()
		{
			return Create();
		}

		#region IWDESimpleListEditDef Members

		public string[] List
		{
			get
			{
				return m_List;
			}
			set
			{
				m_List = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "SimpleListEditDef"))
			{
				base.ReadXmlAttributes(XmlReader);
				string temp = Utils.GetAttribute(XmlReader, "List", "");
				if(temp != "")
				{
					m_List = temp.Split(',');
					for(int i = 0; i < m_List.Length; i++)
						m_List[i] = m_List[i].Trim();
				}

				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "SimpleListEditDef"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("SimpleListEditDef");
			base.WriteXmlAttributes(XmlWriter);
			if(m_List != null)
				XmlWriter.WriteAttributeString("List", string.Join(",", m_List));
			XmlWriter.WriteEndElement();
		}

		#endregion
	}

	public class WDEValidLengthsEditDef : WDEEditDef, IWDEValidLengthsEditDef, IWDEXmlPersist
	{
		private int[] m_Lengths;

		private WDEValidLengthsEditDef() : base()
		{
			SetNames("ValidLengths", "WebDX.Api.Scripts.ValidLengthsEdit");
		}

		public static IWDEValidLengthsEditDef Create()
		{
			return new WDEValidLengthsEditDef() as IWDEValidLengthsEditDef;
		}

		public static IWDEValidLengthsEditDef CreateInstance()
		{
			return Create();
		}

		#region IWDEValidLengthsEditDef Members

		public int[] Lengths
		{
			get
			{
				return m_Lengths;
			}
			set
			{
				m_Lengths = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ValidLengthsEditDef"))
			{
				base.ReadXmlAttributes(XmlReader);
				string temp = Utils.GetAttribute(XmlReader, "Lengths", "");
				if(temp != "")
				{
					string[] temps = temp.Split(',');
					m_Lengths = new int[temps.Length];
					try
					{
						for(int i = 0; i < m_Lengths.Length; i++)
							m_Lengths[i] = int.Parse(temps[i].Trim());
					}
					catch(FormatException fe)
					{
						throw new XmlException("Invalid int value", fe, XmlReader.LineNumber, XmlReader.LinePosition);
					}
					catch(OverflowException oe)
					{
						throw new XmlException("Invalid int value", oe, XmlReader.LineNumber, XmlReader.LinePosition);
					}
				}

				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ValidLengthsEditDef"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("ValidLengthsEditDef");
			base.WriteXmlAttributes(XmlWriter);
			if(m_Lengths != null)
			{
				string[] temp = new string[m_Lengths.Length];
				for(int i = 0; i < m_Lengths.Length; i++)
					temp[i] = m_Lengths[i].ToString();

				XmlWriter.WriteAttributeString("Lengths", string.Join(",", temp));
			}
			XmlWriter.WriteEndElement();
		}

		#endregion
	}

	public class WDEDateEditDef : WDEEditDef, IWDEDateEditDef, IWDEXmlPersist
	{
		private bool m_AllowFutureDates;

		private WDEDateEditDef() : base()
		{
			SetNames("ValidDate", "WebDX.Edits.ValidDateEdit");
		}

		public static IWDEDateEditDef Create()
		{
			return new WDEDateEditDef() as IWDEDateEditDef;
		}

		public static IWDEDateEditDef CreateInstance()
		{
			return Create();
		}

		#region IWDEDateEditDef Members

		public bool AllowFutureDates
		{
			get
			{
				return m_AllowFutureDates;
			}
			set
			{
				m_AllowFutureDates = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if(ConvertingOldProject)
			{
				ReadOldDateEdit(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DateEditDef"))
				{
					base.ReadXmlAttributes(XmlReader);
					m_AllowFutureDates = Utils.GetBoolValue(XmlReader, "AllowFutureDates", false);

					XmlReader.Read();
					XmlReader.MoveToContent();

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "DateEditDef"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("DateEditDef");
			base.WriteXmlAttributes(XmlWriter);
			if(AllowFutureDates)
				XmlWriter.WriteAttributeString("AllowFutureDates", AllowFutureDates.ToString());
			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Methods

		private void ReadOldDateEdit(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DateEdit"))
			{
				Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
				AllowFutureDates = Utils.GetBoolValue(XmlReader, "AllowFutureDates", false);
				ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
				SessionMode = Utils.GetSessionType(XmlReader, "ViewModes");

				XmlReader.Read();
				XmlReader.MoveToContent();

				ReadDescription(XmlReader);
				ReadErrorMessage(XmlReader);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "DateEdit"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
	}

	public class WDEDiagnosisPtrEditDef : WDEEditDef, IWDEDiagnosisPtrEditDef, IWDEXmlPersist
	{
		private IWDEFieldDefLinks m_DiagnosisCodes;
		private string m_Database;
		private string m_Query;

		private WDEDiagnosisPtrEditDef() : base()
		{
			m_Database = "";
			m_Query = "";

			m_DiagnosisCodes = WDEFieldDefLinks.Create(this);
			SetNames("ValidDiagnosisCodes", "WebDX.Edits.ValidDiagnosisCodes");
		}

		public static IWDEDiagnosisPtrEditDef Create()
		{
			return new WDEDiagnosisPtrEditDef() as IWDEDiagnosisPtrEditDef;
		}

		public static IWDEDiagnosisPtrEditDef CreateInstance()
		{
			return Create();
		}

		#region IWDEDiagnosisPtrEditDef Members

		public IWDEFieldDefLinks DiagnosisCodes
		{
			get
			{
				return m_DiagnosisCodes;
			}
		}

		public string Database
		{
			get
			{
				return m_Database;
			}
			set
			{
				if(value == null)
					m_Database = "";
				else
					m_Database = value;
			}
		}

		public string Query
		{
			get
			{
				return m_Query;
			}
			set
			{
				if(value == null)
					m_Query = "";
				else
					m_Query = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if(ConvertingOldProject)
			{
				ReadOldDiagnosisPtr(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DiagnosisPtrEditDef"))
				{
					base.ReadXmlAttributes(XmlReader);
					m_Database = Utils.GetAttribute(XmlReader, "Database", "");
					m_Query = Utils.GetAttribute(XmlReader, "Query", "");
				
					XmlReader.Read();
					XmlReader.MoveToContent();

					IWDEXmlPersist ipers = (IWDEXmlPersist) m_DiagnosisCodes;
					ipers.ReadFromXml(XmlReader);

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "DiagnosisPtrEditDef"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("DiagnosisPtrEditDef");
			base.WriteXmlAttributes(XmlWriter);
			if(Database != "")
				XmlWriter.WriteAttributeString("Database", Database);
			if(Query != "")
				XmlWriter.WriteAttributeString("Query", Query);
			IWDEXmlPersist ipers = (IWDEXmlPersist) m_DiagnosisCodes;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Members

		private void ReadOldDiagnosisPtr(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DiagnosisPtr"))
			{
				Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
				ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
				SessionMode = Utils.GetSessionType(XmlReader, "ViewModes");
				Database = Utils.GetAttribute(XmlReader, "Database", "");
				Query = Utils.GetAttribute(XmlReader, "Query", "");

				XmlReader.Read();
				XmlReader.MoveToContent();

				ReadDescription(XmlReader);
				ReadErrorMessage(XmlReader);
				ReadCodes(XmlReader);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "DiagnosisPtr"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		private void ReadCodes(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DiagnosisCodes"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				IWDEProjectInternal iproj = GetProjectInternal();

				int index = 0;
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DiagCodeFields"))
				{
					string temp = Utils.GetAttribute(XmlReader, "Field", "");
					if(temp != "")
						iproj.Resolver.AddRequest(m_DiagnosisCodes, "Item", temp, index++);

					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "DiagnosisCodes"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
	}

	public class WDEBalanceCheckEditDef : WDEEditDef, IWDEBalanceCheckEditDef, IWDEXmlPersist, ICloneable
	{
		private IWDEFieldDefLinks m_SumFields;
		private double m_ErrorMargin;

		private WDEBalanceCheckEditDef() : base()
		{
			m_SumFields = WDEFieldDefLinks.Create(this);
			SetNames("BalanceCheck", "WebDX.Edits.BalanceCheck");
		}

		public static IWDEBalanceCheckEditDef Create()
		{
			return new WDEBalanceCheckEditDef() as IWDEBalanceCheckEditDef;
		}

		public static IWDEBalanceCheckEditDef CreateInstance()
		{
			return Create();
		}

		#region IWDEBalanceCheckEditDef Members

		public IWDEFieldDefLinks SumFields
		{
			get
			{
				return m_SumFields;
			}
		}

		public double ErrorMargin
		{
			get
			{
				return m_ErrorMargin;
			}

			set
			{
				m_ErrorMargin = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "BalanceCheckEditDef"))
			{
				base.ReadXmlAttributes(XmlReader);
				m_ErrorMargin = Utils.GetDoubleValue(XmlReader, "ErrorMargin");
				XmlReader.Read();
				XmlReader.MoveToContent();

				IWDEXmlPersist ipers = (IWDEXmlPersist) m_SumFields;
				ipers.ReadFromXml(XmlReader);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "BalanceCheckEditDef"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("BalanceCheckEditDef");
			base.WriteXmlAttributes(XmlWriter);
			if(m_ErrorMargin != 0)
				XmlWriter.WriteAttributeString("ErrorMargin", m_ErrorMargin.ToString());
			IWDEXmlPersist ipers = (IWDEXmlPersist) m_SumFields;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.WriteEndElement();
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			//Can't use MemberwiseClone because we need a new m_SumFields instance.
			IWDEBalanceCheckEditDef res = Create();
			res.Description = Description;
			res.Enabled = Enabled;
			res.ErrorMargin = ErrorMargin;
			res.ErrorMessage = ErrorMessage;
			res.ErrorType = ErrorType;
			res.SessionMode = SessionMode;
			
			foreach(IWDEFieldDef def in SumFields)
			{
				res.SumFields.Add(def);
			}

			return res;
		}

		#endregion
	}


	public class WDEEventScriptDef : WDEBaseCollectionItem, IWDEEventScriptDef, IWDEXmlPersist
	{
		private string m_Desc;
		private bool m_Enabled;
		private string m_ScriptFullName;
		private string m_ObjName;

		private WDEEventScriptDef(object Parent, string ObjName) : base()
		{
			m_Desc = "";
			m_ScriptFullName = "";
			this.Parent = Parent;
			m_ObjName = ObjName;
		}

		public static IWDEEventScriptDef Create(object Parent, string ObjName)
		{
			return new WDEEventScriptDef(Parent, ObjName) as IWDEEventScriptDef;
		}

		public static IWDEEventScriptDef CreateInstance(object Parent, string ObjName)
		{
			return Create(Parent, ObjName);
		}

		protected override string InternalGetNodeName()
		{
			return m_ObjName;
		}

		#region IWDEEventScriptDef Members

		public string Description
		{
			get
			{
				return m_Desc;
			}
			set
			{
				if(value == null)
					m_Desc = "";
				else
					m_Desc = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return m_Enabled;
			}
			set
			{
				m_Enabled = value;
			}
		}

		public string ScriptFullName
		{
			get
			{
				return m_ScriptFullName;
			}
			set
			{
				if(value == null)
					m_ScriptFullName = "";
				else
					m_ScriptFullName = value;
			}
		}

        public IWDEEventScriptDef Clone()
        {
            return (IWDEEventScriptDef)this.MemberwiseClone(false);
        }

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();

			if(ConvertingOldProject)
			{
				ConvertOldScript(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ScriptDef"))
				{
					m_Desc = Utils.GetAttribute(XmlReader, "Description", "");
					m_Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
					m_ScriptFullName = Utils.GetAttribute(XmlReader, "ScriptFullName", "");

                    if (m_ScriptFullName != string.Empty && !m_ScriptFullName.StartsWith("StudioScripts"))
                    {
                        Regex r = new Regex(m_ScriptFullName.Substring(0, m_ScriptFullName.IndexOf(".")), RegexOptions.IgnoreCase);
                        m_ScriptFullName = r.Replace(m_ScriptFullName, "StudioScripts", 1);
                    }

					XmlReader.Read();
					XmlReader.MoveToContent();
				
					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ScriptDef"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("ScriptDef");
			if(Description != "")
				XmlWriter.WriteAttributeString("Description", Description);
			if(Enabled)
				XmlWriter.WriteAttributeString("Enabled", Enabled.ToString());
			if(ScriptFullName != "")
				XmlWriter.WriteAttributeString("ScriptFullName", ScriptFullName);
			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Members

		private void ConvertOldScript(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "EventScript"))
			{
				Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);

				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Script"))
				{
					XmlReader.Read();
					XmlReader.MoveToContent();

					StringBuilder sb = new StringBuilder();
					while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
						(XmlReader.Name == "String"))
					{
						sb.Append(Utils.GetAttribute(XmlReader, "Value", "") + Environment.NewLine);

						XmlReader.Read();
						XmlReader.MoveToContent();
					}

					IWDEProjectInternal iproj = GetProjectInternal();
					iproj.AppendOldScriptText(sb.ToString(), GetNamePath());
					ScriptFullName = GetNamePath();

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Script"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "EventScript"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
	}

	public class WDESessionDefs : WDEBaseCollection, IWDESessionDefs, IWDEXmlPersist
	{
		private WDESessionDefs(object Parent) : base(Parent)
		{
		}

		public static IWDESessionDefs Create(IWDEProject Parent)
		{
			return new WDESessionDefs(Parent) as IWDESessionDefs;
		}

		public static IWDESessionDefs CreateInstance(IWDEProject Parent)
		{
			return Create(Parent);
		}

		protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			IWDESessionDef def = (IWDESessionDef) Item;
			return (string.Compare(def.SessionDefName, Name, true) == 0);
		}

		protected override ArrayList GetCollectionList()
		{
			return base.GetSameLevelCollections();
		}

		#region IWDESessionDefs Members

		public IWDESessionDef this[int index]
		{
			get
			{
				return (IWDESessionDef) base.InternalGetIndex(index);
			}
		}

		public int Find(string SessionDefName)
		{
			return base.InternalIndexOf(SessionDefName);
		}

		public IWDESessionDef Add(string SessionDefName)
		{
			int res = base.VerifyName(SessionDefName);
			if(res == 0)
			{
				IWDESessionDef newItem = WDESessionDef.Create();
				base.InternalAdd((WDEBaseCollectionItem) newItem);
				newItem.SessionDefName = SessionDefName;
				return newItem;
			}
            else if (res == -1)
                 throw new WDEException("API00037", new object[] {SessionDefName});
            else
                 throw new WDEException("API00038", new object[] {SessionDefName});
		}

		public IWDESessionDef Add()
		{
			string newName = base.GetNextDefaultName( "SessionDef");
			IWDESessionDef newItem = WDESessionDef.Create();
			base.InternalAdd((WDEBaseCollectionItem) newItem);
			newItem.SessionDefName = newName;
			return newItem;
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ViewDefs"))
			{
				ReadOldSessionDefs(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "SessionDef"))
				{
					Clear();
					while(!(XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
						(XmlReader.Name == "SessionDef"))
					{
						IWDESessionDef def = WDESessionDef.Create();
						base.InternalAdd((WDEBaseCollectionItem) def);
						IWDEXmlPersist ipers = (IWDEXmlPersist) def;
						ipers.ReadFromXml(XmlReader);
						base.RegisterObject((WDEBaseCollectionItem) def);                        
                    }
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion

		#region Private Methods

		private void ReadOldSessionDefs(XmlTextReader XmlReader)
		{
			XmlReader.Read();
			XmlReader.MoveToContent();

			while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
				(XmlReader.Name == "ViewDef"))
			{
				IWDESessionDef def = WDESessionDef.Create();
				base.InternalAdd((WDEBaseCollectionItem) def);
				IWDEXmlPersist ipers = (IWDEXmlPersist) def;
				ipers.ReadFromXml(XmlReader);
				base.RegisterObject((WDEBaseCollectionItem) def);
			}

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ViewDefs"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		#endregion
	}

	public class WDESessionDef : WDEBaseCollectionItem, IWDESessionDef_R3, IWDEXmlPersist
	{
		private string m_SessionDefName;
		private int m_DataPanelHeight;
		private int m_FirstImage;
		private IWDEFormLinks m_Forms;
		private WDESessionOption m_Options;
		private IWDEPhotoStitchDef m_PhotoStitchDef;
		private bool m_ShowTicker;
		private int m_TickerCharHeight;
		private WDEImageScale m_ImageScale;
		private int m_ImageScalePercent;
		private WDESessionType m_SessionType;
		private WDESessionStyle m_SessionStyle;
        private WDEImageFormMaps m_ImageFormMaps;
        private string m_PluginName;
        private PluginDocking m_UserPanelDocking;
        private int m_UserPanelWidthHeight;
        private ToolbarDock m_ToolbarDock;
        private ImageDock m_ImageDock;

		private WDESessionDef()
		{
			m_SessionDefName = "";
			m_Options = WDESessionOption.None;
			m_ImageScale = WDEImageScale.FitWidth;
			m_SessionType = WDESessionType.None;
			m_SessionStyle = WDESessionStyle.Horizontal;

			m_Forms = WDEFormLinks.Create(this);
			m_PhotoStitchDef = WDEPhotoStitchDef.Create();
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_PhotoStitchDef;
			obj.Parent = this;
            m_ImageFormMaps = new WDEImageFormMaps(this);
            m_PluginName = "";
            m_UserPanelDocking = PluginDocking.Right;
		}

		public static IWDESessionDef Create()
		{
			return new WDESessionDef() as IWDESessionDef;
		}

		public static IWDESessionDef CreateInstance()
		{
			return Create();
		}

		protected override string InternalGetNodeName()
		{
			return m_SessionDefName;
		}

		#region IWDESessionDef Members

		public string SessionDefName
		{
			get
			{
				return m_SessionDefName;
			}
			set
			{
				string newName = value;
				if(newName == null)
					newName = "";

				if(newName != m_SessionDefName)
				{
					int res = base.Collection.VerifyName(newName);
					if(res == 0)
						m_SessionDefName = newName;
                    else if (res == -1)
                         throw new WDEException("API00037", new object[] {newName});
                    else if (res == -2)
                        throw new WDEException("API00038", new object[] {newName});
				}
			}
		}

		public int DataPanelHeight
		{
			get
			{
				return m_DataPanelHeight;
			}
			set
			{
				m_DataPanelHeight = value;
			}
		}

		public int FirstImage
		{
			get
			{
				return m_FirstImage;
			}
			set
			{
				m_FirstImage = value;
			}
		}

		public IWDEFormLinks Forms
		{
			get
			{
				return m_Forms;
			}
		}

		public WebDX.Api.WDESessionOption Options
		{
			get
			{
				// clear deprecated options
				return m_Options & ~(
					WDESessionOption.AllowFloatingWindow |
					WDESessionOption.ShowWorkZones |
					WDESessionOption.AllowModifyNonKeyables |
					WDESessionOption.AllowImageTypeChange |
					WDESessionOption.AllowFieldVerify |
					WDESessionOption.DisableVerifyInReview |
					WDESessionOption.LowResThumbnails |
					WDESessionOption.QIVerifyValidated |
					WDESessionOption.PutFlaggedDoc |
					WDESessionOption.OnePassLockFields);
			}
			set
			{
				// clear deprecated options
				m_Options = value & ~(
					WDESessionOption.AllowFloatingWindow |
					WDESessionOption.ShowWorkZones |
					WDESessionOption.AllowModifyNonKeyables |
					WDESessionOption.AllowImageTypeChange |
					WDESessionOption.AllowFieldVerify |
					WDESessionOption.DisableVerifyInReview |
					WDESessionOption.LowResThumbnails |
					WDESessionOption.QIVerifyValidated |
					WDESessionOption.PutFlaggedDoc |
					WDESessionOption.OnePassLockFields);
			}
		}

		public IWDEPhotoStitchDef PhotoStitch
		{
			get
			{
				return m_PhotoStitchDef;
			}
		}		

		public bool ShowTicker
		{
			get
			{
				return m_ShowTicker;
			}
			set
			{
				m_ShowTicker = value;
			}
		}

		public int TickerCharHeight
		{
			get
			{
				return m_TickerCharHeight;
			}
			set
			{
				m_TickerCharHeight = value;
			}
		}

		public WebDX.Api.WDEImageScale ImageScale
		{
			get
			{
				return m_ImageScale;
			}
			set
			{
				m_ImageScale = value;
			}
		}

		public int ImageScalePercent
		{
			get
			{
				return m_ImageScalePercent;
			}
			set
			{
				m_ImageScalePercent = value;
			}
		}

		public WebDX.Api.WDESessionType SessionType
		{
			get
			{
				return m_SessionType;
			}
			set
			{
				m_SessionType = value;
			}
		}

		public WebDX.Api.WDESessionStyle SessionStyle
		{
			get
			{
				return m_SessionStyle;
			}
			set
			{
				m_SessionStyle = value;
			}
		}

		public IWDEFormDef GetFormByImageType(string ImageType)
		{
			if(ImageType == null)
				throw new ArgumentNullException("ImageType","ImageType cannot be null");
			if(ImageType == "")
				throw new ArgumentException("ImageType cannot be blank","ImageType");

            if (m_Forms.Count == 0)
                 throw new WDEException("API00036", new object[] { SessionDefName });

			IWDEFormDef result = null;

            //Find the document that ImageType belongs to
            IWDEProject proj = (IWDEProject)this.GetProjectInternal();
            IWDEDocumentDef imageDoc = null;
            bool found = false;
            foreach (IWDEDocumentDef docDef in proj.DocumentDefs)
            {
                foreach (IWDEImageSourceDef def in docDef.ImageSourceDefs)
                {
                    if (string.Compare(def.ImageType, ImageType, true) == 0)
                    {
                        found = true;
                        imageDoc = docDef;
                        break;
                    }
                }
                if (found)
                    break;
            }

            //Match the image document to a default form document
            foreach (IWDEFormDef form in m_Forms)
            {
                IWDEDocumentDef formDoc = null;
                if (form.RecordDef != null)
                {
                    formDoc = form.RecordDef.Document;
                }
                else
                {
                    formDoc = FindFormDoc(form);
                }

                if (formDoc == imageDoc)
                {
                    result = form;
                    break;
                }
            }

            if (m_ImageFormMaps.Count == 0)
            {
                //Use the DefaultImage property (for forms converted from 1.x projects).
                foreach (IWDEFormDef def in m_Forms)
                {
                    if (def.DefaultImage != null)
                    {
                        if (string.Compare(def.DefaultImage.ImageType, ImageType, true) == 0)
                        {
                            result = def;
                            break;
                        }
                    }
                }
            }
            else
            {
                //Use the specific mapping in 2.x
                int i = m_ImageFormMaps.Find(ImageType);
                if (i > -1)
                    result = m_ImageFormMaps[i].FormDef;
            }

            if (result == null)
                throw new WDEException("API00039", new object[] { SessionDefName, imageDoc == null ? "(Not Found)" : imageDoc.DocType, ImageType });
			return result;
		}

		#endregion

        #region IWDESessionDef_R1 Members

        public void SetImageFormMap(string ImageType, IWDEFormDef FormDef)
        {
            if (ImageType == null)
                throw new ArgumentNullException("ImageType");
            if (FormDef == null)
                throw new ArgumentNullException("FormDef");

            bool found = false;
            foreach (IWDEFormDef form in m_Forms)
            {
                if (form == FormDef)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                throw new ArgumentException("FormDef not found in SessionDef form list.","FormDef");

            IWDEProject proj = (IWDEProject)this.GetProjectInternal();

            found = false;
            IWDEImageSourceDef imageDef = null;
            foreach (IWDEDocumentDef docDef in proj.DocumentDefs)
            {
                foreach (IWDEImageSourceDef def in docDef.ImageSourceDefs)
                {
                    if (string.Compare(def.ImageType, ImageType, true) == 0)
                    {
                        found = true;
                        imageDef = def;
                        break;
                    }
                }
                if (found)
                    break;
            }

            if (!found)
                throw new ArgumentException("ImageType not found.","ImageType");

            int index = m_ImageFormMaps.Find(imageDef.ImageType);
            if (index != -1)
                m_ImageFormMaps.RemoveAt(index);

            m_ImageFormMaps.Add(imageDef, FormDef);
        }

        #endregion

        #region IWDESessionDef_R2 Members

        public string PluginName
        {
            get
            {
                return m_PluginName;
            }
            set
            {
                m_PluginName = value == null ? "" : value;
            }
        }

        public PluginDocking UserPanelDocking
        {
            get
            {
                return m_UserPanelDocking;
            }
            set
            {
                m_UserPanelDocking = value;
            }
        }

        public int UserPanelWidthHeight
        {
            get
            {
                return m_UserPanelWidthHeight;
            }
            set
            {
                m_UserPanelWidthHeight = value;
            }
        }

        public ToolbarDock ToolbarDock
        {
            get
            {
                return m_ToolbarDock;
            }
            set
            {
                m_ToolbarDock = value;
            }
        }

        public ImageDock ImageDock
        {
            get
            {
                return m_ImageDock;
            }
            set
            {
                m_ImageDock = value;
            }
        }


        #endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			string tag = "SessionDef";
			if(ConvertingOldProject)
				tag = "ViewDef";

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == tag))
			{
				m_SessionDefName = Utils.GetAttribute(XmlReader, "Name", "");
				m_DataPanelHeight = Utils.GetIntValue(XmlReader, "DataPanelHeight");
				m_FirstImage = Utils.GetIntValue(XmlReader, "FirstImage");
				m_Options = Utils.GetSessionOptions(XmlReader, "Options");

                IWDEProject proj = (IWDEProject)this.GetProjectInternal();
                Version projVersion = new Version(proj.APIVersion);
                Version optionVersion = new Version("2.6.0.0");
				if (projVersion < optionVersion)
				{
					m_Options |= WDESessionOption.OmniPassAutoUnflag | WDESessionOption.VerifyFieldCorrect;

					if ((m_Options & WDESessionOption.OnePassCharRepOnly) != 0)
						m_Options |= WDESessionOption.ShowKeyingWindow;
				}

                Version bovVersion = new Version("3.1.0.0");
                if (projVersion < bovVersion)
                    m_Options |= WDESessionOption.BlankOnValidate;

                Version soVersion = new Version("3.2.1.0");
                if (projVersion < soVersion)
                    m_Options |= WDESessionOption.ScaleOverlay;

                Version rvfVersion = new Version("3.2.0.2");
                if (projVersion < rvfVersion)
                    m_Options |= WDESessionOption.ReValidateForm;

                Version zdcVersion = new Version("3.4.5.10");
                if (projVersion < zdcVersion)
                    m_Options |= WDESessionOption.ZoneDoubleClick;

                m_ShowTicker = Utils.GetBoolValue(XmlReader, "ShowTicker", false);
				m_TickerCharHeight = Utils.GetIntValue(XmlReader, "TickerCharHeight");
				if(ConvertingOldProject)
				{
					m_ImageScale = Utils.GetImageScale(XmlReader, "ScaleMode", WDEImageScale.FitWidth);
					m_ImageScalePercent = Utils.GetIntValue(XmlReader, "ScalePercent");
					m_SessionType = Utils.GetSessionType(XmlReader, "ViewMode");
					m_SessionStyle = Utils.GetSessionStyle(XmlReader, "ViewStyle", WDESessionStyle.Horizontal);
				}
				else
				{
					m_ImageScale = Utils.GetImageScale(XmlReader, "ImageScale", WDEImageScale.FitWidth);
					m_ImageScalePercent = Utils.GetIntValue(XmlReader, "ImageScalePercent");
					m_SessionType = Utils.GetSessionType(XmlReader, "SessionType");
					m_SessionStyle = Utils.GetSessionStyle(XmlReader, "SessionStyle", WDESessionStyle.Horizontal);
                    m_PluginName = Utils.GetAttribute(XmlReader, "PluginName", "");
                    m_UserPanelDocking = Utils.GetPluginDocking(XmlReader, "UserPanelDocking");
                    m_UserPanelWidthHeight = Utils.GetIntValue(XmlReader, "UserPanelWidthHeight");
                    m_ToolbarDock = Utils.GetToolbarDock(XmlReader, "ToolbarDock");
                    m_ImageDock = Utils.GetImageDock(XmlReader, "ImageDock");                    
				}

				XmlReader.Read();
				XmlReader.MoveToContent();

				if(ConvertingOldProject)
				{
					ReadDescription(XmlReader);
				
					IWDEXmlPersist ipers = (IWDEXmlPersist) m_Forms;
					ipers.ReadFromXml(XmlReader);

					ipers = (IWDEXmlPersist) m_PhotoStitchDef;
					ipers.ReadFromXml(XmlReader);

                    while ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "RejectCodeLink"))
                    {
                        XmlReader.Read();
                        XmlReader.MoveToContent();
                    }
                }
                else
				{
					IWDEXmlPersist ipers = (IWDEXmlPersist) m_PhotoStitchDef;
					ipers.ReadFromXml(XmlReader);
				
					ipers = (IWDEXmlPersist) m_Forms;
					ipers.ReadFromXml(XmlReader);

                    while ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "RejectCodeLink"))
                    {
                        XmlReader.Read();
                        XmlReader.MoveToContent();
                    }

                    ipers = (IWDEXmlPersist)m_ImageFormMaps;
                    ipers.ReadFromXml(XmlReader);
				}

                if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == tag))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("SessionDef");
			if(SessionDefName != "")
				XmlWriter.WriteAttributeString("Name", SessionDefName);
			if(DataPanelHeight != 0)
				XmlWriter.WriteAttributeString("DataPanelHeight", DataPanelHeight.ToString());
			if(FirstImage != 0)
				XmlWriter.WriteAttributeString("FirstImage", FirstImage.ToString());
			if(Options != WDESessionOption.None)
				XmlWriter.WriteAttributeString("Options", Options.ToString());
			if(ShowTicker != false)
				XmlWriter.WriteAttributeString("ShowTicker", ShowTicker.ToString());
			if(TickerCharHeight != 0)
				XmlWriter.WriteAttributeString("TickerCharHeight", TickerCharHeight.ToString());
			if(ImageScale != WDEImageScale.FitWidth)
				XmlWriter.WriteAttributeString("ImageScale", ImageScale.ToString());
			if(ImageScalePercent != 0)
				XmlWriter.WriteAttributeString("ImageScalePercent", ImageScalePercent.ToString());
			if(SessionType != WDESessionType.None)
				XmlWriter.WriteAttributeString("SessionType", SessionType.ToString());
			if(SessionStyle != WDESessionStyle.Horizontal)
				XmlWriter.WriteAttributeString("SessionStyle", SessionStyle.ToString());
            if (PluginName != "")
                XmlWriter.WriteAttributeString("PluginName", PluginName);
            if (UserPanelDocking != PluginDocking.Right)
                XmlWriter.WriteAttributeString("UserPanelDocking", UserPanelDocking.ToString());
            if (UserPanelWidthHeight != 0)
                XmlWriter.WriteAttributeString("UserPanelWidthHeight", UserPanelWidthHeight.ToString());
            if (ToolbarDock != ToolbarDock.Right)
                XmlWriter.WriteAttributeString("ToolbarDock", ToolbarDock.ToString());
            if (ImageDock != ImageDock.Top)
                XmlWriter.WriteAttributeString("ImageDock", ImageDock.ToString());
			
			IWDEXmlPersist ipers = (IWDEXmlPersist) m_PhotoStitchDef;
			ipers.WriteToXml(XmlWriter);
			
			ipers = (IWDEXmlPersist) m_Forms;
			ipers.WriteToXml(XmlWriter);

            ipers = (IWDEXmlPersist)m_ImageFormMaps;
            ipers.WriteToXml(XmlWriter);

			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Methods

		private void ReadDescription(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Descriptions"))
			{
				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

        private IWDEDocumentDef FindFormDoc(IWDEFormDef form)
        {
            IWDEProject proj = (IWDEProject)this.GetProjectInternal();

            foreach (IWDEDocumentDef doc in proj.DocumentDefs)
            {
                foreach (IWDEFormDef fDef in doc.FormDefs)
                    if (fDef == form)
                        return doc;
            }

               throw new WDEException("API90015", new object[] { form.FormName });

        }

		#endregion
        
    }

    public class WDEImageFormMaps : WDEBaseCollection, IWDEXmlPersist
    {
        public WDEImageFormMaps(object Parent)
            : base(Parent)
        {
        }

        protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
        {
            if (Item is WDEImageFormMap)
            {
                return (string.Compare(((WDEImageFormMap)Item).ImageSourceDef.ImageType, Name, true) == 0);
            }
            else
                return false;
        }

        public WDEImageFormMap this[int index]
        {
            get { return (WDEImageFormMap)base.InternalGetIndex(index); }
        }

        public void Add(IWDEImageSourceDef ImageDef, IWDEFormDef FormDef)
        {
            WDEImageFormMap map = new WDEImageFormMap();
            map.FormDef = FormDef;
            map.ImageSourceDef = ImageDef;
            base.InternalAdd((WDEBaseCollectionItem)map);
        }

        public int Find(string ImageType)
        {
            return base.InternalIndexOf(ImageType);
        }

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
        {
            if (XmlReader == null)
                throw new ArgumentNullException("XmlReader");

            Clear();
            while ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ImageFormMap"))
            {
                WDEImageFormMap map = new WDEImageFormMap();
                base.InternalAdd((WDEBaseCollectionItem)map);
                IWDEXmlPersist ipers = (IWDEXmlPersist)map;
                ipers.ReadFromXml(XmlReader);
            }
        }

        public void WriteToXml(XmlTextWriter XmlWriter)
        {
            if (XmlWriter == null)
                throw new ArgumentNullException("XmlWriter");

            foreach (WDEImageFormMap item in this)
            {
                IWDEXmlPersist ipers = (IWDEXmlPersist)item;
                ipers.WriteToXml(XmlWriter);
            }
        }

        #endregion
    }

    public class WDEImageFormMap : WDEBaseCollectionItem, IWDEXmlPersist
    {
        private IWDEImageSourceDef m_ImageSourceDef;
        private IWDEFormDef m_FormDef;

        public WDEImageFormMap() : base() { }

        public IWDEImageSourceDef ImageSourceDef
        {
            get { return m_ImageSourceDef; }
            set
            {
                if (m_ImageSourceDef != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_ImageSourceDef;
                    obj.RemoveLink(this);
                }

                m_ImageSourceDef = value;

                if (m_ImageSourceDef != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_ImageSourceDef;
                    obj.AddLink(this);
                }
            }
        }

        public IWDEFormDef FormDef
        {
            get { return m_FormDef; }
            set
            {
                if (m_FormDef != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_FormDef;
                    obj.RemoveLink(this);
                }

                m_FormDef = value;

                if (m_FormDef != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_FormDef;
                    obj.AddLink(this);
                }
            }
        }

        protected override string InternalGetNodeName()
        {
            return "ImageFormMap" + base.Collection.IndexOf(this).ToString();
        }

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
        {
            if (XmlReader == null)
                throw new ArgumentNullException("XmlReader");

            IWDEProjectInternal proj = null;
            if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                proj = (IWDEProjectInternal)base.TopParent();
            else
                throw new WDEException("API90006", new object[] { "ImageFormMap" });

            if ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ImageFormMap"))
            {
                string temp = Utils.GetAttribute(XmlReader, "ImageLink", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "ImageSourceDef", temp);

                temp = Utils.GetAttribute(XmlReader, "FormLink", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "FormDef", temp);

                XmlReader.Read();
                XmlReader.MoveToContent();

                if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ImageFormMap"))
                {
                    XmlReader.ReadEndElement();
                    XmlReader.MoveToContent();
                }
            }
        }

        public void WriteToXml(XmlTextWriter XmlWriter)
        {
            if (XmlWriter == null)
                throw new ArgumentNullException("XmlWriter");

            XmlWriter.WriteStartElement("ImageFormMap");
            WDEBaseCollectionItem obj = null;
            if (m_ImageSourceDef != null)
            {
                obj = (WDEBaseCollectionItem)m_ImageSourceDef;
                XmlWriter.WriteAttributeString("ImageLink", obj.GetNamePath());
            }

            if (m_FormDef != null)
            {
                obj = (WDEBaseCollectionItem)m_FormDef;
                XmlWriter.WriteAttributeString("FormLink", obj.GetNamePath());
            }

            XmlWriter.WriteEndElement();
        }

        #endregion
    }

	public class WDEFormLinks : WDEBaseCollection, IWDEFormLinks_R1, IWDEXmlPersist
	{
		private WDEFormLinks(object Parent) :  base(Parent)
		{
		}

		public static IWDEFormLinks Create(object Parent)
		{
			return new WDEFormLinks(Parent) as IWDEFormLinks;
		}

		public static IWDEFormLinks CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			base.Remove(LinkedItem);
		}

		#region IWDEFormLinks Members

		public IWDEFormDef this[int index]
		{
			get
			{
				return (IWDEFormDef) base.InternalGetIndex(index);
			}
			set
			{
				while(Count <= index)
					base.InternalAdd(null);

				IWDEFormDef def = (IWDEFormDef) base.InternalGetIndex(index);
				if(def != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) def;
					obj.RemoveLink(this);
				}

				base.InternalSetIndex(index, (WDEBaseCollectionItem) value, false);

				def = (IWDEFormDef) base.InternalGetIndex(index);
				if(def != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) def;
					obj.AddLink(this);
				}
			}
		}

		public void Add(IWDEFormDef FormDef)
		{
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) FormDef;
			obj.AddLink(this);
			base.InternalAdd((WDEBaseCollectionItem) FormDef, false);
		}

        public void RemoveForm(IWDEFormDef FormDef)
        {
            WDEBaseCollectionItem obj = (WDEBaseCollectionItem)FormDef;
            obj.RemoveLink(this);
            base.RemoveLink(obj);
        }

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			IWDEProjectInternal proj = null;
            if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                proj = (IWDEProjectInternal)base.TopParent();
            else
                throw new WDEException("API90006", new object[] {"FormLinks"});

			int index = 0;
			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "FormLinks"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();
			}

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "FormLink"))
			{
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "FormLink"))
				{
					string temp = Utils.GetAttribute(XmlReader, "FormName", "");
					if(temp == "")
						temp = Utils.GetAttribute(XmlReader, "Form", "");

					if(temp != "")
						proj.Resolver.AddRequest(this, "Item", proj.Resolver.GetConvertedFormName(temp), index++);

					XmlReader.Read();
					XmlReader.MoveToContent();

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "FormLink"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "FormLinks"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				WDEBaseCollectionItem obj = (WDEBaseCollectionItem) this[i];
				XmlWriter.WriteStartElement("FormLink");
				XmlWriter.WriteAttributeString("FormName", obj.GetNamePath());
				XmlWriter.WriteEndElement();
			}
		}

		#endregion
	}

	public class WDEAddressCorrectionEditDef : WDEEditDef, IWDEAddressCorrectionEditDef, IWDEXmlPersist
	{
		private IWDEZP4LookupFields m_LookupFields;
		private IWDEZP4ResultFields m_ResultFields;
		private bool m_ReviewResults;
		private bool m_UseDPV;

		private WDEAddressCorrectionEditDef() : base()
		{
			m_LookupFields = WDEZP4LookupFields.Create();
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_LookupFields;
			obj.Parent = this;

			m_ResultFields = WDEZP4ResultFields.Create();
			obj = (WDEBaseCollectionItem) m_ResultFields;
			obj.Parent = this;
			SetNames("AddressCorrection", "WebDX.Edits.AddressCorrection");
		}

		public static IWDEAddressCorrectionEditDef Create()
		{
			return new WDEAddressCorrectionEditDef() as IWDEAddressCorrectionEditDef;
		}

		public static IWDEAddressCorrectionEditDef CreateInstance()
		{
			return Create();
		}

		public override ArrayList GetChildCollections()
		{
			ArrayList al = new ArrayList();
			al.Add(m_LookupFields);
			al.Add(m_ResultFields);
			return al;
		}


		#region IWDEAddressCorrectionEditDef Members

		public IWDEZP4LookupFields LookupFields
		{
			get
			{
				return m_LookupFields;
			}
		}

		public IWDEZP4ResultFields ResultFields
		{
			get
			{
				return m_ResultFields;
			}
		}

		public bool ReviewResults
		{
			get
			{
				return m_ReviewResults;
			}
			set
			{
				m_ReviewResults = value;
			}
		}

		public bool UseDPV
		{
			get
			{
				return m_UseDPV;
			}
			set
			{
				m_UseDPV = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if(ConvertingOldProject)
			{
				ReadOldAddressCorrection(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "AddressCorrectionEditDef"))
				{
					base.ReadXmlAttributes(XmlReader);
					m_ReviewResults = Utils.GetBoolValue(XmlReader, "ReviewResults", false);
					m_UseDPV = Utils.GetBoolValue(XmlReader, "UseDPV", false);

					XmlReader.Read();
					XmlReader.MoveToContent();

					IWDEXmlPersist ipers = (IWDEXmlPersist) LookupFields;
					ipers.ReadFromXml(XmlReader);

					ipers = (IWDEXmlPersist) ResultFields;
					ipers.ReadFromXml(XmlReader);

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "AddressCorrectionEditDef"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("AddressCorrectionEditDef");
			base.WriteXmlAttributes(XmlWriter);
			if(ReviewResults != false)
				XmlWriter.WriteAttributeString("ReviewResults", ReviewResults.ToString());
			if(UseDPV != false)
				XmlWriter.WriteAttributeString("UseDPV", UseDPV.ToString());

			IWDEXmlPersist ipers = (IWDEXmlPersist) LookupFields;
			ipers.WriteToXml(XmlWriter);

			ipers = (IWDEXmlPersist) ResultFields;
			ipers.WriteToXml(XmlWriter);

			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Members

		private void ReadOldAddressCorrection(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "AddressCorrection"))
			{
				Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
				ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
				SessionMode = Utils.GetSessionType(XmlReader, "ViewModes");
				ReviewResults = Utils.GetBoolValue(XmlReader, "ReviewResults", false);
				UseDPV = Utils.GetBoolValue(XmlReader, "UseDPV", false);

				XmlReader.Read();
				XmlReader.MoveToContent();

				ReadDescription(XmlReader);
				ReadErrorMessage(XmlReader);

				IWDEXmlPersist ipers = (IWDEXmlPersist) m_LookupFields;
				ipers.ReadFromXml(XmlReader);
				
				ipers = (IWDEXmlPersist) m_ResultFields;
				ipers.ReadFromXml(XmlReader);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "AddressCorrection"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
	}

    public class WDEZP4LookupFields : WDEBaseCollectionItem, IWDEZP4LookupFields, IWDEXmlPersist
    {
        private IWDEFieldDef m_AddressField;
        private IWDEFieldDef m_CityField;
        private IWDEFieldDef m_ZipCodeField;

        private WDEZP4LookupFields()
        {
        }

        public static IWDEZP4LookupFields Create()
        {
            return new WDEZP4LookupFields() as IWDEZP4LookupFields;
        }

        public static IWDEZP4LookupFields CreateInstance()
        {
            return Create();
        }

        protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
        {
            if (LinkedItem == m_AddressField)
                m_AddressField = null;
            if (LinkedItem == m_CityField)
                m_CityField = null;
            if (LinkedItem == m_ZipCodeField)
                m_ZipCodeField = null;
        }

        protected override string InternalGetNodeName()
        {
            return "ZP4LookupFields";
        }

        #region IWDEZP4LookupFields Members

        public IWDEFieldDef AddressField
        {
            get
            {
                return m_AddressField;
            }
            set
            {
                if (m_AddressField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_AddressField;
                    obj.RemoveLink(this);
                }

                m_AddressField = value;

                if (m_AddressField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_AddressField;
                    obj.AddLink(this);
                }
            }
        }

        public IWDEFieldDef CityField
        {
            get
            {
                return m_CityField;
            }
            set
            {
                if (m_CityField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_CityField;
                    obj.RemoveLink(this);
                }

                m_CityField = value;

                if (m_CityField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_CityField;
                    obj.AddLink(this);
                }
            }
        }

        public IWDEFieldDef ZipCodeField
        {
            get
            {
                return m_ZipCodeField;
            }
            set
            {
                if (m_ZipCodeField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_ZipCodeField;
                    obj.RemoveLink(this);
                }

                m_ZipCodeField = value;

                if (m_ZipCodeField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_ZipCodeField;
                    obj.AddLink(this);
                }
            }
        }

        #endregion

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
        {
            if (XmlReader == null)
                throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

            IWDEProjectInternal proj = null;
            if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                proj = (IWDEProjectInternal)base.TopParent();
            else
                throw new WDEException("API90006", new object[] { "ZP4LookupFields" });

            if ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "LookupFields"))
            {
                string temp = Utils.GetAttribute(XmlReader, "AddressField", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "AddressField", temp);

                temp = Utils.GetAttribute(XmlReader, "CityField", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "CityField", temp);

                temp = Utils.GetAttribute(XmlReader, "ZipCodeField", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "ZipCodeField", temp);

                XmlReader.Read();
                XmlReader.MoveToContent();

                if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "LookupFields"))
                {
                    XmlReader.ReadEndElement();
                    XmlReader.MoveToContent();
                }
            }
        }

        public void WriteToXml(XmlTextWriter XmlWriter)
        {
            if (XmlWriter == null)
                throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

            XmlWriter.WriteStartElement("LookupFields");
            WDEBaseCollectionItem obj = null;

            if (AddressField != null)
            {
                obj = (WDEBaseCollectionItem)AddressField;
                XmlWriter.WriteAttributeString("AddressField", obj.GetNamePath());
            }
            if (CityField != null)
            {
                obj = (WDEBaseCollectionItem)CityField;
                XmlWriter.WriteAttributeString("CityField", obj.GetNamePath());
            }
            if (ZipCodeField != null)
            {
                obj = (WDEBaseCollectionItem)ZipCodeField;
                XmlWriter.WriteAttributeString("ZipCodeField", obj.GetNamePath());
            }

            XmlWriter.WriteEndElement();
        }

        #endregion
    }

    public class WDEZP4ResultFields : WDEBaseCollectionItem, IWDEZP4ResultFields, IWDEXmlPersist
    {
        private IWDEFieldDef m_AddressField;
        private IWDEFieldDef m_CityField;
        private IWDEFieldDef m_StateField;
        private IWDEFieldDef m_ZipCodeField;
        private IWDEFieldDef m_ZipPlus4Field;
        private IWDEFieldDef m_HouseNumberField;
        private IWDEFieldDef m_PreDirectionField;
        private IWDEFieldDef m_StreetNameField;
        private IWDEFieldDef m_StreetSuffixField;
        private IWDEFieldDef m_PostDirectionField;
        private IWDEFieldDef m_AptUnitAbbrField;
        private IWDEFieldDef m_AptNumberField;

        private WDEZP4ResultFields()
        {
        }

        public static IWDEZP4ResultFields Create()
        {
            return new WDEZP4ResultFields() as IWDEZP4ResultFields;
        }

        public static IWDEZP4ResultFields CreateInstance()
        {
            return Create();
        }

        protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
        {
            if (LinkedItem == m_AddressField)
                m_AddressField = null;
            if (LinkedItem == m_CityField)
                m_CityField = null;
            if (LinkedItem == m_StateField)
                m_StateField = null;
            if (LinkedItem == m_ZipCodeField)
                m_ZipCodeField = null;
            if (LinkedItem == m_ZipPlus4Field)
                m_ZipCodeField = null;
            if (LinkedItem == m_HouseNumberField)
                m_HouseNumberField = null;
            if (LinkedItem == m_PreDirectionField)
                m_PreDirectionField = null;
            if (LinkedItem == m_StreetNameField)
                m_StreetNameField = null;
            if (LinkedItem == m_StreetSuffixField)
                m_StreetSuffixField = null;
            if (LinkedItem == m_PostDirectionField)
                m_PostDirectionField = null;
            if (LinkedItem == m_AptUnitAbbrField)
                m_AptUnitAbbrField = null;
            if (LinkedItem == m_AptNumberField)
                m_AptNumberField = null;
        }

        protected override string InternalGetNodeName()
        {
            return "ZP4ResultFields";
        }

        #region IWDEZP4ResultFields Members

        public IWDEFieldDef AddressField
        {
            get
            {
                return m_AddressField;
            }
            set
            {
                if (m_AddressField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_AddressField;
                    obj.RemoveLink(this);
                }

                m_AddressField = value;

                if (m_AddressField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_AddressField;
                    obj.AddLink(this);
                }
            }
        }

        public IWDEFieldDef CityField
        {
            get
            {
                return m_CityField;
            }
            set
            {
                if (m_CityField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_CityField;
                    obj.RemoveLink(this);
                }

                m_CityField = value;

                if (m_CityField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_CityField;
                    obj.AddLink(this);
                }
            }
        }

        public IWDEFieldDef StateField
        {
            get
            {
                return m_StateField;
            }
            set
            {
                if (m_StateField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_StateField;
                    obj.RemoveLink(this);
                }

                m_StateField = value;

                if (m_StateField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_StateField;
                    obj.AddLink(this);
                }
            }
        }

        public IWDEFieldDef ZipCodeField
        {
            get
            {
                return m_ZipCodeField;
            }
            set
            {
                if (m_ZipCodeField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_ZipCodeField;
                    obj.RemoveLink(this);
                }

                m_ZipCodeField = value;

                if (m_ZipCodeField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_ZipCodeField;
                    obj.AddLink(this);
                }
            }
        }

        public IWDEFieldDef ZipPlus4Field
        {
            get
            {
                return m_ZipPlus4Field;
            }
            set
            {
                if (m_ZipPlus4Field != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_ZipPlus4Field;
                    obj.RemoveLink(this);
                }

                m_ZipPlus4Field = value;

                if (m_ZipPlus4Field != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_ZipPlus4Field;
                    obj.AddLink(this);
                }
            }
        }

        public IWDEFieldDef HouseNumberField
        {
            get
            {
                return m_HouseNumberField;
            }
            set
            {
                if (m_HouseNumberField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_HouseNumberField;
                    obj.RemoveLink(this);
                }

                m_HouseNumberField = value;

                if (m_HouseNumberField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_HouseNumberField;
                    obj.AddLink(this);
                }
            }
        }

        public IWDEFieldDef PreDirectionField
        {
            get
            {
                return m_PreDirectionField;
            }
            set
            {
                if (m_PreDirectionField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_PreDirectionField;
                    obj.RemoveLink(this);
                }

                m_PreDirectionField = value;

                if (m_PreDirectionField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_PreDirectionField;
                    obj.AddLink(this);
                }
            }
        }

        public IWDEFieldDef StreetNameField
        {
            get
            {
                return m_StreetNameField;
            }
            set
            {
                if (m_StreetNameField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_StreetNameField;
                    obj.RemoveLink(this);
                }

                m_StreetNameField = value;

                if (m_StreetNameField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_StreetNameField;
                    obj.AddLink(this);
                }
            }
        }

        public IWDEFieldDef StreetSuffixField
        {
            get
            {
                return m_StreetSuffixField;
            }
            set
            {
                if (m_StreetSuffixField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_StreetSuffixField;
                    obj.RemoveLink(this);
                }

                m_StreetSuffixField = value;

                if (m_StreetSuffixField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_StreetSuffixField;
                    obj.AddLink(this);
                }
            }
        }

        public IWDEFieldDef PostDirectionField
        {
            get
            {
                return m_PostDirectionField;
            }
            set
            {
                if (m_PostDirectionField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_PostDirectionField;
                    obj.RemoveLink(this);
                }

                m_PostDirectionField = value;

                if (m_PostDirectionField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_PostDirectionField;
                    obj.AddLink(this);
                }
            }
        }

        public IWDEFieldDef AptUnitAbbrField
        {
            get
            {
                return m_AptUnitAbbrField;
            }
            set
            {
                if (m_AptUnitAbbrField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_AptUnitAbbrField;
                    obj.RemoveLink(this);
                }

                m_AptUnitAbbrField = value;

                if (m_AptUnitAbbrField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_AptUnitAbbrField;
                    obj.AddLink(this);
                }
            }
        }

        public IWDEFieldDef AptNumberField
        {
            get
            {
                return m_AptNumberField;
            }
            set
            {
                if (m_AptNumberField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_AptNumberField;
                    obj.RemoveLink(this);
                }

                m_AptNumberField = value;

                if (m_AptNumberField != null)
                {
                    WDEBaseCollectionItem obj = (WDEBaseCollectionItem)m_AptNumberField;
                    obj.AddLink(this);
                }
            }
        }

        #endregion

        #region IWDEXmlPersist Members

        public void ReadFromXml(XmlTextReader XmlReader)
        {
            if (XmlReader == null)
                throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

            IWDEProjectInternal proj = null;
            if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                proj = (IWDEProjectInternal)base.TopParent();
            else
               throw new WDEException("API90006", new object[] { "ZP4ResultFields" });

			XmlReader.MoveToContent();
            if ((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ResultFields"))
            {
                string temp = Utils.GetAttribute(XmlReader, "AddressField", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "AddressField", temp);
                temp = Utils.GetAttribute(XmlReader, "CityField", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "CityField", temp);
                temp = Utils.GetAttribute(XmlReader, "StateField", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "StateField", temp);
                temp = Utils.GetAttribute(XmlReader, "ZipCodeField", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "ZipCodeField", temp);
                temp = Utils.GetAttribute(XmlReader, "ZipPlus4Field", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "ZipPlus4Field", temp);
                temp = Utils.GetAttribute(XmlReader, "HouseNumberField", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "HouseNumberField", temp);
                temp = Utils.GetAttribute(XmlReader, "PreDirectionField", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "PreDirectionField", temp);
                temp = Utils.GetAttribute(XmlReader, "StreetNameField", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "StreetNameField", temp);
                temp = Utils.GetAttribute(XmlReader, "StreetSuffixField", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "StreetSuffixField", temp);
                temp = Utils.GetAttribute(XmlReader, "PostDirectionField", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "PostDirectionField", temp);
                temp = Utils.GetAttribute(XmlReader, "AptUnitAbbrField", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "AptUnitAbbrField", temp);
                temp = Utils.GetAttribute(XmlReader, "AptNumberField", "");
                if (temp != "")
                    proj.Resolver.AddRequest(this, "AptNumberField", temp);

                XmlReader.Read();
                XmlReader.MoveToContent();

                if ((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ResultFields"))
                {
                    XmlReader.ReadEndElement();
                    XmlReader.MoveToContent();
                }
            }
        }

        public void WriteToXml(XmlTextWriter XmlWriter)
        {
            if (XmlWriter == null)
                throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

            XmlWriter.WriteStartElement("ResultFields");

            WDEBaseCollectionItem obj = null;

            if (AddressField != null)
            {
                obj = (WDEBaseCollectionItem)AddressField;
                XmlWriter.WriteAttributeString("AddressField", obj.GetNamePath());
            }
            if (CityField != null)
            {
                obj = (WDEBaseCollectionItem)CityField;
                XmlWriter.WriteAttributeString("CityField", obj.GetNamePath());
            }
            if (StateField != null)
            {
                obj = (WDEBaseCollectionItem)StateField;
                XmlWriter.WriteAttributeString("StateField", obj.GetNamePath());
            }
            if (ZipCodeField != null)
            {
                obj = (WDEBaseCollectionItem)ZipCodeField;
                XmlWriter.WriteAttributeString("ZipCodeField", obj.GetNamePath());
            }
            if (ZipPlus4Field != null)
            {
                obj = (WDEBaseCollectionItem)ZipPlus4Field;
                XmlWriter.WriteAttributeString("ZipPlus4Field", obj.GetNamePath());
            }
            if (HouseNumberField != null)
            {
                obj = (WDEBaseCollectionItem)HouseNumberField;
                XmlWriter.WriteAttributeString("HouseNumberField", obj.GetNamePath());
            }
            if (PreDirectionField != null)
            {
                obj = (WDEBaseCollectionItem)PreDirectionField;
                XmlWriter.WriteAttributeString("PreDirectionField", obj.GetNamePath());
            }
            if (StreetNameField != null)
            {
                obj = (WDEBaseCollectionItem)StreetNameField;
                XmlWriter.WriteAttributeString("StreetNameField", obj.GetNamePath());
            }
            if (StreetSuffixField != null)
            {
                obj = (WDEBaseCollectionItem)StreetSuffixField;
                XmlWriter.WriteAttributeString("StreetSuffixField", obj.GetNamePath());
            }
            if (PostDirectionField != null)
            {
                obj = (WDEBaseCollectionItem)PostDirectionField;
                XmlWriter.WriteAttributeString("PostDirectionField", obj.GetNamePath());
            }
            if (AptUnitAbbrField != null)
            {
                obj = (WDEBaseCollectionItem)AptUnitAbbrField;
                XmlWriter.WriteAttributeString("AptUnitAbbrField", obj.GetNamePath());
            }
            if (AptNumberField != null)
            {
                obj = (WDEBaseCollectionItem)AptNumberField;
                XmlWriter.WriteAttributeString("AptNumberField", obj.GetNamePath());
            }
            XmlWriter.WriteEndElement();
        }

        #endregion
    }

	public class WDEConditionalGotoEditDef : WDEEditDef, IWDEConditionalGotoEditDef, IWDEXmlPersist
	{
		private IWDEConditionalGotos m_Gotos;

		private WDEConditionalGotoEditDef() : base()
		{
			m_Gotos = WDEConditionalGotos.Create(this);
			SetNames("ConditionalGoto", "WebDX.Edits.ConditionalGoto");
		}

		public static IWDEConditionalGotoEditDef Create()
		{
			return new WDEConditionalGotoEditDef() as IWDEConditionalGotoEditDef;
		}

		public static IWDEConditionalGotoEditDef CreateInstance()
		{
			return Create();
		}

		public override ArrayList GetChildCollections()
		{
			ArrayList al = new ArrayList();
			al.Add(m_Gotos);
			return al;
		}


		#region IWDEConditionalGotoEditDef Members

		public IWDEConditionalGotos Gotos
		{
			get
			{
				return m_Gotos;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if(ConvertingOldProject)
			{
				ReadOldConditionalGoto(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ConditionalGotoEditDef"))
				{
					base.ReadXmlAttributes(XmlReader);

					XmlReader.Read();
					XmlReader.MoveToContent();

					IWDEXmlPersist ipers = (IWDEXmlPersist) Gotos;
					ipers.ReadFromXml(XmlReader);

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ConditionalGotoEditDef"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("ConditionalGotoEditDef");
			base.WriteXmlAttributes(XmlWriter);
			IWDEXmlPersist ipers = (IWDEXmlPersist) Gotos;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Members

		private void ReadOldConditionalGoto(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ConditionalGoto"))
			{
				Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
				ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
				SessionMode = Utils.GetSessionType(XmlReader, "ViewModes");

				XmlReader.Read();
				XmlReader.MoveToContent();

				ReadDescription(XmlReader);
				ReadErrorMessage(XmlReader);
				
				IWDEXmlPersist ipers = (IWDEXmlPersist) Gotos;
				ipers.ReadFromXml(XmlReader);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ConditionalGoto"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
	}

	public class WDEConditionalGotos : WDEBaseCollection, IWDEConditionalGotos, IWDEXmlPersist
	{
		private WDEConditionalGotos(object Parent) : base(Parent)
		{
		}

		public static IWDEConditionalGotos Create(object Parent)
		{
			return new WDEConditionalGotos(Parent) as IWDEConditionalGotos;
		}

		public static IWDEConditionalGotos CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		#region IWDEConditionalGotos Members

		public IWDEConditionalGoto this[int index]
		{
			get
			{
				return (IWDEConditionalGoto) base.InternalGetIndex(index);
			}
		}

		public IWDEConditionalGoto Add()
		{
			IWDEConditionalGoto newItem = WDEConditionalGoto.Create(this);
			base.InternalAdd((WDEBaseCollectionItem) newItem);
			return newItem;
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
                throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			IWDEProjectInternal iproj = GetProjectInternal();
			if(iproj.ConvertOldFormat)
			{
				ReadOldGotos(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Goto"))
				{
					Clear();
					while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
						(XmlReader.Name == "Goto"))
					{
						IWDEConditionalGoto def = WDEConditionalGoto.Create(this);
						base.InternalAdd((WDEBaseCollectionItem) def);
						IWDEXmlPersist ipers = (IWDEXmlPersist) def;
						ipers.ReadFromXml(XmlReader);
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion

		#region Private Members

		private void ReadOldGotos(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Gotos"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "ConditionalGoto"))
				{
					IWDEConditionalGoto def = WDEConditionalGoto.Create(this);
					base.InternalAdd((WDEBaseCollectionItem) def);
					IWDEXmlPersist ipers = (IWDEXmlPersist) def;
					ipers.ReadFromXml(XmlReader);
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Gotos"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
	}

	public class WDEConditionalGoto : WDEBaseCollectionItem, IWDEConditionalGoto, IWDEXmlPersist
	{
		private IWDEControlDef m_Control;
		private string m_Expression;
		private bool m_Release;
		private bool m_ClearFields;
		private WDEBaseCollection m_Collection;

		private WDEConditionalGoto(WDEBaseCollection Collection) : base()
		{
			m_Expression = "";
			m_Collection = Collection;
		}

		public static IWDEConditionalGoto Create(WDEBaseCollection Collection)
		{
			return new WDEConditionalGoto(Collection) as IWDEConditionalGoto;
		}

		public static IWDEConditionalGoto CreateInstance(WDEBaseCollection Collection)
		{
			return Create(Collection);
		}

		protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			if(LinkedItem == m_Control)
				m_Control = null;
		}

		protected override string InternalGetNodeName()
		{
			int i = m_Collection.IndexOf(this);
			return "Goto" + i.ToString();
		}

		#region IWDEConditionalGoto Members

		public IWDEControlDef Control
		{
			get
			{
				return m_Control;
			}
			set
			{
				if(m_Control != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Control;
					obj.RemoveLink(this);
				}

				m_Control = value;

				if(m_Control != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Control;
					obj.AddLink(this);
				}
			}
		}

		public string Expression
		{
			get
			{
				return m_Expression;
			}
			set
			{
				if(value == null)
					m_Expression = "";
				else
					m_Expression = value;
			}
		}

		public bool Release
		{
			get
			{
				return m_Release;
			}
			set
			{
				m_Release = value;
				if(m_Release)
					Control = null;
			}
		}

		public bool ClearFields
		{
			get
			{
				return m_ClearFields;
			}
			set
			{
				m_ClearFields = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			IWDEProjectInternal proj = null;
            if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                proj = (IWDEProjectInternal)base.TopParent();
            else
            
               throw new WDEException("API90006", new object[] {"LookupResultField"});

			XmlReader.MoveToContent();
			if(ConvertingOldProject)
			{
				ReadOldGoto(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Goto"))
				{
					string temp = Utils.GetAttribute(XmlReader, "Control", "");
					if(temp != "")
						proj.Resolver.AddRequest(this, "Control", temp);
					m_Expression = Utils.GetAttribute(XmlReader, "Expression", "");
					m_Release = Utils.GetBoolValue(XmlReader, "Release", false);
					m_ClearFields = Utils.GetBoolValue(XmlReader, "ClearFields", false);

					XmlReader.Read();
					XmlReader.MoveToContent();

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Goto"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("Goto");
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) Control;
			if(obj != null)
				XmlWriter.WriteAttributeString("Control", obj.GetNamePath());
			if(Expression != "")
				XmlWriter.WriteAttributeString("Expression", Expression);
			if(Release != false)
				XmlWriter.WriteAttributeString("Release", Release.ToString());
			if(ClearFields != false)
				XmlWriter.WriteAttributeString("ClearFields", ClearFields.ToString());
			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Members

		private void ReadOldGoto(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ConditionalGoto"))
			{
				IWDEProjectInternal iproj = GetProjectInternal();
                if (iproj == null)
                    throw new WDEException("API90006", new object[] {"ConditionalGoto"});

				string control = FixFormName(Utils.GetAttribute(XmlReader, "Control", ""));
				if(control != "")
					iproj.Resolver.AddRequest(this, "Control", control);

				string expression = Utils.GetAttribute(XmlReader, "Expression", "");
				if(expression != "")
				{
					iproj.AppendOldExpression(expression, GetNamePath());
					Expression = GetNamePath();
				}

				Release = Utils.GetBoolValue(XmlReader, "Release", false);
				ClearFields = Utils.GetBoolValue(XmlReader, "ClearFields", false);

				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		private string FixFormName(string controlLink)
		{
			IWDEFormDef def = (IWDEFormDef) GetParentInterface("IWDEFormDef");

			if(controlLink != "")
			{
				if(def != null)
				{
					string[] temp = controlLink.Split('.');
					if(temp.Length > 1)
					{
						temp[1] = def.FormName;
						return string.Join(".", temp);
					}
					else
						return temp[0];
				}
				else
					return controlLink;
			}
			else
				return controlLink;
		}

		#endregion
	}

	public class WDETableLookupEditDef : WDEEditDef, IWDETableLookupEditDef, IWDEXmlPersist
	{
		private WDEExecuteOption m_ExecuteOn;
		private IWDETableLookups m_TableLookups;

		private WDETableLookupEditDef() : base()
		{
			m_ExecuteOn = WDEExecuteOption.Validate;
			
			m_TableLookups = WDETableLookups.Create(this);
			SetNames("TableLookup", "WebDX.Edits.TableLookup");
		}

		public static IWDETableLookupEditDef Create()
		{
			return new WDETableLookupEditDef() as IWDETableLookupEditDef;
		}

		public static IWDETableLookupEditDef CreateInstance()
		{
			return Create();
		}

		#region IWDETableLookupEditDef Members

		public WebDX.Api.WDEExecuteOption ExecuteOn
		{
			get
			{
				return m_ExecuteOn;
			}
			set
			{
				m_ExecuteOn = value;
			}
		}

		public IWDETableLookups TableLookups
		{
			get
			{
				return m_TableLookups;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if(ConvertingOldProject)
			{
				ReadOldTableLookup(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "TableLookupEditDef"))
				{
					base.ReadXmlAttributes(XmlReader);
					m_ExecuteOn = Utils.GetExecuteOptions(XmlReader, "ExecuteOn");

					XmlReader.Read();
					XmlReader.MoveToContent();

					IWDEXmlPersist ipers = (IWDEXmlPersist) TableLookups;
					ipers.ReadFromXml(XmlReader);

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "TableLookupEditDef"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("TableLookupEditDef");
			base.WriteXmlAttributes(XmlWriter);
			if(ExecuteOn != WDEExecuteOption.Validate)
				XmlWriter.WriteAttributeString("ExecuteOn", ExecuteOn.ToString());
			IWDEXmlPersist ipers = (IWDEXmlPersist) TableLookups;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Methods

		private void ReadOldTableLookup(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "TableLookup"))
			{
				Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
				ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
				SessionMode = Utils.GetSessionType(XmlReader, "ViewModes");
				ExecuteOn = Utils.GetExecuteOptions(XmlReader, "ExecuteOn");

				XmlReader.Read();
				XmlReader.MoveToContent();

				ReadDescription(XmlReader);
				ReadErrorMessage(XmlReader);

				IWDEXmlPersist ipers = (IWDEXmlPersist) TableLookups;
				ipers.ReadFromXml(XmlReader);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "TableLookup"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
	}

	public class WDETableLookups : WDEBaseCollection, IWDETableLookups, IWDEXmlPersist
	{
		private WDETableLookups(object Parent) : base(Parent)
		{
		}

		public static IWDETableLookups Create(object Parent)
		{
			return new WDETableLookups(Parent) as IWDETableLookups;
		}

		public static IWDETableLookups CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		#region IWDETableLookups Members

		public IWDETableLookup this[int index]
		{
			get
			{
				return (IWDETableLookup) base.InternalGetIndex(index);
			}
		}

		public IWDETableLookup Add()
		{
			IWDETableLookup newItem = WDETableLookup.Create();
			base.InternalAdd((WDEBaseCollectionItem) newItem);
			return newItem;
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Lookups"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();
			}

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "TableLookup"))
			{
				Clear();
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "TableLookup"))
				{
					IWDETableLookup def = WDETableLookup.Create();
					IWDEXmlPersist ipers = (IWDEXmlPersist) def;
					base.InternalAdd((WDEBaseCollectionItem) def);
					ipers.ReadFromXml(XmlReader);
				}
			}

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Lookups"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion
	}

	public class WDETableLookup : WDEBaseCollectionItem, IWDETableLookup, IWDEXmlPersist
	{
		private string m_Database;
		private string m_Query;
		private IWDELookupResultFields m_ResultFields;
		private WDETableLookupOption m_Options;

		private WDETableLookup()
		{
			m_Database = "";
			m_Query = "";
			m_Options = WDETableLookupOption.None;

			m_ResultFields = WDELookupResultFields.Create(this);
		}

		public static IWDETableLookup Create()
		{
			return new WDETableLookup() as IWDETableLookup;
		}

		public static IWDETableLookup CreateInstance()
		{
			return Create();
		}

		protected override string InternalGetNodeName()
		{
			return "TableLookup";
		}

		#region IWDETableLookup Members

		public string Database
		{
			get
			{
				return m_Database;
			}
			set
			{
				if(value == null)
					m_Database = "";
				else
					m_Database = value;
			}
		}

		public string Query
		{
			get
			{
				return m_Query;
			}
			set
			{
				if(value == null)
					m_Query = "";
				else
					m_Query = value;
			}
		}

		public IWDELookupResultFields ResultFields
		{
			get
			{
				return m_ResultFields;
			}
		}

		public WebDX.Api.WDETableLookupOption Options
		{
			get
			{
				return m_Options;
			}
			set
			{
				m_Options = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "TableLookup"))
			{
				m_Database = Utils.GetAttribute(XmlReader, "Database", "");
				m_Query = Utils.GetAttribute(XmlReader, "Query", "");
				m_Options = Utils.GetTableLookupOptions(XmlReader, "Options");

				XmlReader.Read();
				XmlReader.MoveToContent();

				IWDEXmlPersist ipers = (IWDEXmlPersist) ResultFields;
				ipers.ReadFromXml(XmlReader);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "TableLookup"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("TableLookup");
			if(Database != "")
				XmlWriter.WriteAttributeString("Database", Database);
			if(Query != "")
				XmlWriter.WriteAttributeString("Query", Query);
			if(Options != WDETableLookupOption.None)
				XmlWriter.WriteAttributeString("Options", Options.ToString());

			IWDEXmlPersist ipers = (IWDEXmlPersist) ResultFields;
			ipers.WriteToXml(XmlWriter);

			XmlWriter.WriteEndElement();
		}

		#endregion
	}

	public class WDELookupResultFields : WDEBaseCollection, IWDELookupResultFields, IWDEXmlPersist
	{
		private WDELookupResultFields(object Parent) : base(Parent)
		{
		}

		public static IWDELookupResultFields Create(object Parent)
		{
			return new WDELookupResultFields(Parent) as IWDELookupResultFields;
		}

		public static IWDELookupResultFields CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		#region IWDELookupResultFields Members

		public IWDELookupResultField this[int index]
		{
			get
			{
				return (IWDELookupResultField) base.InternalGetIndex(index);
			}
		}

		public IWDELookupResultField Add()
		{
			IWDELookupResultField newItem = WDELookupResultField.Create();
			base.InternalAdd((WDEBaseCollectionItem) newItem);
			return newItem;
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ResultFields"))
			{
				ReadOldResultFields(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "LookupResultField"))
				{
					Clear();
					while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
						(XmlReader.Name == "LookupResultField"))
					{
						IWDELookupResultField lres = WDELookupResultField.Create();
						IWDEXmlPersist ipers = (IWDEXmlPersist) lres;
						base.InternalAdd((WDEBaseCollectionItem) lres);
						ipers.ReadFromXml(XmlReader);
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion

		#region Private Methods

		private void ReadOldResultFields(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ResultFields"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "ResultFields"))
				{
					IWDELookupResultField lres = WDELookupResultField.Create();
					IWDEXmlPersist ipers = (IWDEXmlPersist) lres;
					base.InternalAdd((WDEBaseCollectionItem) lres);
					ipers.ReadFromXml(XmlReader);
				}

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ResultFields"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
	}

	public class WDELookupResultField : WDEBaseCollectionItem, IWDELookupResultField, IWDEXmlPersist
	{
		private string m_DBFieldName;
		private IWDEFieldDef m_Field;
		private WDELookupResultFieldOption m_Options;

		private WDELookupResultField()
		{
			m_DBFieldName = "";
			m_Options = WDELookupResultFieldOption.None;
		}

		public static IWDELookupResultField Create()
		{
			return new WDELookupResultField() as IWDELookupResultField;
		}

		public static IWDELookupResultField CreateInstance()
		{
			return Create();
		}

		protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			if(LinkedItem == m_Field)
				m_Field = null;
		}

		protected override string InternalGetNodeName()
		{
			return "LookupResultField";
		}

		#region IWDELookupResultField Members

		public string DBFieldName
		{
			get
			{
				return m_DBFieldName;
			}
			set
			{
				if(value == null)
					m_DBFieldName = "";
				else
					m_DBFieldName = value;
			}
		}

		public IWDEFieldDef Field
		{
			get
			{
				return m_Field;
			}
			set
			{
				if(m_Field != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Field;
					obj.RemoveLink(this);
				}

				m_Field = value;

				if(m_Field != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_Field;
					obj.AddLink(this);
				}
			}
		}

		public WebDX.Api.WDELookupResultFieldOption Options
		{
			get
			{
				return m_Options;
			}
			set
			{
				m_Options = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			IWDEProjectInternal proj = null;
            if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                proj = (IWDEProjectInternal)base.TopParent();
            else
                throw new WDEException("API90006", new object[] {"LookupResultField"});

			XmlReader.MoveToContent();
			if(XmlReader.NodeType == XmlNodeType.Element)
			{
				if((XmlReader.Name == "LookupResultField") || (XmlReader.Name == "ResultFields"))
				{
					m_DBFieldName = Utils.GetAttribute(XmlReader, "DBFieldName", "");
					string temp = Utils.GetAttribute(XmlReader, "Field", "");
					if(temp != "")
						proj.Resolver.AddRequest(this, "Field", temp);
					m_Options = Utils.GetLookupResultOptions(XmlReader, "Options");

					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("LookupResultField");
			if(DBFieldName != "")
				XmlWriter.WriteAttributeString("DBFieldName", DBFieldName);
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) Field;
			if(obj != null)
				XmlWriter.WriteAttributeString("Field", obj.GetNamePath());
			if(Options != WDELookupResultFieldOption.None)
				XmlWriter.WriteAttributeString("Options", Options.ToString());
			XmlWriter.WriteEndElement();
		}

		#endregion
	}

	public class WDEZoneDefs : WDEBaseCollection, IWDEZoneDefs, IWDEXmlPersist
	{
		private WDEZoneDefs(object Parent) : base(Parent)
		{
		}

		public static IWDEZoneDefs Create(object Parent)
		{
			return new WDEZoneDefs(Parent) as IWDEZoneDefs;
		}

		public static IWDEZoneDefs CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			IWDEZoneDef def = (IWDEZoneDef) Item;
			return (string.Compare(def.Name, Name, true) == 0);
		}

		#region IWDEZoneDefs Members

		public IWDEZoneDef this[int index]
		{
			get
			{
				return (IWDEZoneDef) base.InternalGetIndex(index);
			}
		}

		public IWDEZoneDef Add(string Name)
		{
			int res = base.VerifyName(Name);
			if(res == 0)
			{
				IWDEZoneDef newItem = WDEZoneDef.Create();
				base.InternalAdd((WDEBaseCollectionItem) newItem);
				newItem.Name = Name;
				return newItem;
			}
            else if (res == -1)
                 throw new WDEException("API00037", new object[] {Name});
            else
                 throw new WDEException("API00038", new object[] {Name});
		}

		public IWDEZoneDef Add()
		{
			string newName = base.GetNextDefaultName( "Zone");
			IWDEZoneDef newItem = WDEZoneDef.Create();
			base.InternalAdd((WDEBaseCollectionItem) newItem);
			newItem.Name = newName;
			return newItem;
		}

        public void Add(IWDEZoneDef newItem)
        {
            base.InternalAdd((WDEBaseCollectionItem)newItem);
        }

        public int Find(string Name)
		{
			return base.InternalIndexOf(Name);
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
                throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ZoneDef"))
			{
				Clear();
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "ZoneDef"))
				{
					IWDEZoneDef def = WDEZoneDef.Create();
					IWDEXmlPersist ipers = (IWDEXmlPersist) def;
					base.InternalAdd((WDEBaseCollectionItem) def);
					ipers.ReadFromXml(XmlReader);
					base.RegisterObject((WDEBaseCollectionItem) def);
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion
	}

	public class WDEZoneDef : WDEBaseCollectionItem, IWDEZoneDef, IWDEXmlPersist
	{
		private string m_ZoneName;
		private Rectangle m_ZoneRect;

		private WDEZoneDef()
		{
			m_ZoneName = "";
			m_ZoneRect = Rectangle.Empty;
		}

		public static IWDEZoneDef Create()
		{
			return new WDEZoneDef() as IWDEZoneDef;
		}

		public static IWDEZoneDef CreateInstance()
		{
			return Create();
		}

		protected override string InternalGetNodeName()
		{
			return m_ZoneName;
		}

		#region IWDEZoneDef Members

		public string Name
		{
			get
			{
				return m_ZoneName;
			}
			set
			{
				string newName = value;
				if(newName == null)
					newName = "";

				if(newName != m_ZoneName)
				{
					int res = base.Collection.VerifyName(newName);
					if(res == 0)
						m_ZoneName = newName;
                    else if (res == -1)
                         throw new WDEException("API00037", new object[] {newName});
                    else if (res == -2)
                         throw new WDEException("API00038", new object[] {newName});
				}
			}
		}

		public Rectangle ZoneRect
		{
			get
			{
				return m_ZoneRect;
			}
			set
			{
				m_ZoneRect = value;
			}
			}

        public IWDEZoneDef Clone()
        {
            return (IWDEZoneDef)this.MemberwiseClone(false);
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ZoneDef"))
			{
				m_ZoneName = Utils.GetAttribute(XmlReader, "ZoneName", "");
				m_ZoneRect = Utils.GetRectValue(XmlReader, "ZoneRect");

				XmlReader.Read();
				XmlReader.MoveToContent();
				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ZoneDef"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("ZoneDef");
			if(Name != "")
				XmlWriter.WriteAttributeString("ZoneName", Name);
			if(!ZoneRect.IsEmpty)
				XmlWriter.WriteAttributeString("ZoneRect", Utils.RectToString(ZoneRect));
			XmlWriter.WriteEndElement();
		}

		#endregion
	}

	public class WDELabelLinks : WDEBaseCollection, IWDELabelLinks, IWDEXmlPersist
	{
		private WDELabelLinks(object Parent) : base(Parent)
		{
		}

		public static IWDELabelLinks Create(object Parent)
		{
			return new WDELabelLinks(Parent) as IWDELabelLinks;
		}

		public static IWDELabelLinks CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			base.Remove(LinkedItem);
		}

		#region IWDELabelLinks Members

		public IWDELabelDef this[int index]
		{
			get
			{
				return (IWDELabelDef) base.InternalGetIndex(index);
			}
			set
			{
				while(Count <= index)
					base.InternalAdd(null);

				WDEBaseCollectionItem obj = base.InternalGetIndex(index);
				if(obj != null)
					obj.RemoveLink(this);

				base.InternalSetIndex(index, (WDEBaseCollectionItem) value, false);

				obj = (WDEBaseCollectionItem) value;
				if(obj != null)
					obj.AddLink(this);
			}
		}

		public void Add(IWDELabelDef LabelDef)
		{
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) LabelDef;
			obj.AddLink(this);

			base.InternalAdd((WDEBaseCollectionItem) LabelDef, false);
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "LabelLinks"))
			{
				XmlReader.Read();
				XmlReader.MoveToContent();
			}

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "LabelLink"))
			{
				Clear();

				IWDEProjectInternal proj = null;
                if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                    proj = (IWDEProjectInternal)base.TopParent();
                else
                    throw new WDEException("API90006", new object[] {"LabelLinks"});

				int i = 0;
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "LabelLink"))
				{
					string temp = Utils.GetAttribute(XmlReader, "LabelName", "");
					if(temp == "")
						temp = Utils.GetAttribute(XmlReader, "LabelDef", "");
                    if (temp == "")
                        throw new WDEException("API00035");

					proj.Resolver.AddRequest(this, "Item", temp, i++);
					XmlReader.Read();
					XmlReader.MoveToContent();

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "LabelLink"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "LabelLinks"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				WDEBaseCollectionItem obj = base.InternalGetIndex(i);
				XmlWriter.WriteStartElement("LabelLink");
				XmlWriter.WriteAttributeString("LabelName", obj.GetNamePath());
				XmlWriter.WriteEndElement();
			}
		}

		#endregion
	}

	public class WDEZoneLinks : WDEBaseCollection, IWDEZoneLinks, IWDEXmlPersist
	{
		private WDEZoneLinks(object Parent) : base(Parent)
		{
		}

		public static IWDEZoneLinks Create(object Parent)
		{
			return new WDEZoneLinks(Parent) as IWDEZoneLinks;
		}

		public static IWDEZoneLinks CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
		{
            base.RemoveLink(LinkedItem);
		}

		#region IWDEZoneLinks Members

		public IWDEBaseZoneDef this[int index]
		{
			get
			{
				return (IWDEBaseZoneDef) base.InternalGetIndex(index);
			}
			set
			{
				while(Count <= index)
					base.InternalAdd(null);

				WDEBaseCollectionItem obj = base.InternalGetIndex(index);
				if(obj != null)
					obj.RemoveLink(this);

				base.InternalSetIndex(index, (WDEBaseCollectionItem) value, false);

				obj = (WDEBaseCollectionItem) value;
				if(obj != null)
					obj.AddLink(this);
			}
		}

		public void Add(IWDEBaseZoneDef ZoneDef)
		{
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) ZoneDef;
			obj.AddLink(this);

			base.InternalAdd((WDEBaseCollectionItem) ZoneDef, false);
		}

        public override void Clear()
        {
            foreach (WDEBaseCollectionItem item in this)
                item.RemoveLink(this);

            m_List.Clear();
        }

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();

			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ZoneDefLink"))
			{
				Clear();

				IWDEProjectInternal proj = null;
                if ((base.TopParent() != null) && (base.TopParent() is IWDEProjectInternal))
                    proj = (IWDEProjectInternal)base.TopParent();
                else
                   throw new WDEException("API90006", new object[] {"ZoneLinks"});

				int i = 0;
				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "ZoneDefLink"))
				{
					string temp = Utils.GetAttribute(XmlReader, "ZoneName", "");
					proj.Resolver.AddRequest(this, "Item", temp, i++);

					XmlReader.Read();
					XmlReader.MoveToContent();

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ZoneDefLink"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				WDEBaseCollectionItem obj = base.InternalGetIndex(i);
				XmlWriter.WriteStartElement("ZoneDefLink");
				XmlWriter.WriteAttributeString("ZoneName", obj.GetNamePath());
				XmlWriter.WriteEndElement();
			}
		}

		#endregion
	}

	public class WDESnippetDefs : WDEBaseCollection, IWDESnippetDefs, IWDEXmlPersist
	{
		private WDESnippetDefs(object Parent) : base(Parent)
		{
		}

		public static IWDESnippetDefs Create(object Parent)
		{
			return new WDESnippetDefs(Parent) as IWDESnippetDefs;
		}
		
		public static IWDESnippetDefs CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			IWDESnippetDef def = (IWDESnippetDef) Item;
			return (string.Compare(def.Name, Name, true) == 0);
		}

		#region IWDESnippetDefs Members

		public IWDESnippetDef this[int index]
		{
			get
			{
				return (IWDESnippetDef) base.InternalGetIndex(index);
			}
		}

		public IWDESnippetDef Add(string SnippetName)
		{
			int res = base.VerifyName(SnippetName);
			if(res == 0)
			{
				IWDESnippetDef def = WDESnippetDef.Create();
				base.InternalAdd((WDEBaseCollectionItem) def);
				def.Name = SnippetName;
				return def;
			}
            else if (res == -1)
                 throw new WDEException("API00037", new object[] {SnippetName});
            else
                 throw new WDEException("API00038", new object[] {SnippetName});
 		}

		public IWDESnippetDef Add()
		{
			string newName = base.GetNextDefaultName("SnippetDef");
			IWDESnippetDef def = WDESnippetDef.Create();
			base.InternalAdd((WDEBaseCollectionItem) def);
			def.Name = newName;
			return def;
		}

        public void Add(IWDESnippetDef newDef)
        {
            base.InternalAdd((WDEBaseCollectionItem)newDef);
        }

		public int Find(string SnippetName)
		{
			return base.InternalIndexOf(SnippetName);
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "SnippetDef"))
			{
				Clear();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
					(XmlReader.Name == "SnippetDef"))
				{
					IWDESnippetDef def = WDESnippetDef.Create();
					base.InternalAdd((WDEBaseCollectionItem) def);
					IWDEXmlPersist ipers = (IWDEXmlPersist) def;
					ipers.ReadFromXml(XmlReader);
					base.RegisterObject((WDEBaseCollectionItem) def);
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion
	}

	public class WDESnippetDef : WDEBaseCollectionItem, IWDESnippetDef, IWDEXmlPersist
	{
		private string m_SnippetName;
		private Rectangle m_SnippetRect;
		private Point m_Location;

		private WDESnippetDef()
		{
			m_SnippetName = "";
		}

		public static IWDESnippetDef Create()
		{
			return new WDESnippetDef() as IWDESnippetDef;
		}

		public static IWDESnippetDef CreateInstance()
		{
			return Create();
		}

		protected override string InternalGetNodeName()
		{
			return m_SnippetName;
		}

		#region IWDESnippetDef Members

		public string Name
		{
			get
			{
				return m_SnippetName;
			}
			set
			{
				string newName = value;
				if(newName == null)
					newName = "";

				if(newName != m_SnippetName)
				{
					int res = base.Collection.VerifyName(newName);
					if(res == 0)
						m_SnippetName = newName;
                    else if (res == -1)
                         throw new WDEException("API00037", new object[] {newName});
                    else if (res == -2)
                         throw new WDEException("API00038", new object[] {newName});
				}
			}
		}

		public Rectangle SnippetRect
		{
			get
			{
				return m_SnippetRect;
			}
			set
			{
				m_SnippetRect = value;
			}
		}

		public Point Location
		{
			get
			{
				return m_Location;
			}
			set
			{
				m_Location = value;
			}
		}

        public IWDESnippetDef Clone()
        {
            return (IWDESnippetDef)this.MemberwiseClone();
        }

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "SnippetDef"))
			{
				m_SnippetName = Utils.GetAttribute(XmlReader, "SnippetName", "");
				m_SnippetRect = Utils.GetRectValue(XmlReader, "SnippetRect");
				m_Location = Utils.GetPointValue(XmlReader, "Location");

				XmlReader.Read();
				XmlReader.MoveToContent();

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "SnippetDef"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("SnippetDef");
			if(m_SnippetName != "")
				XmlWriter.WriteAttributeString("SnippetName", m_SnippetName);
			if(!m_SnippetRect.IsEmpty)
				XmlWriter.WriteAttributeString("SnippetRect", Utils.RectToString(m_SnippetRect));
			if(!m_Location.IsEmpty)
				XmlWriter.WriteAttributeString("Location", string.Format("{0},{1}", new object[] {m_Location.X, m_Location.Y}));

			XmlWriter.WriteEndElement();
		}

		#endregion
	}

	public class WDEDetailZoneDefs : WDEBaseCollection, IWDEDetailZoneDefs, IWDEXmlPersist
	{
		private WDEDetailZoneDefs(object Parent) : base(Parent)
		{
		}

		public static IWDEDetailZoneDefs Create(object Parent)
		{
			return new WDEDetailZoneDefs(Parent) as IWDEDetailZoneDefs;
		}

		public static IWDEDetailZoneDefs CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			IWDEDetailZoneDef def = (IWDEDetailZoneDef) Item;
			return (string.Compare(def.Name, Name, true) == 0);
		}

		#region IWDEDetailZoneDefs Members

		public IWDEDetailZoneDef this[int index]
		{
			get
			{
				return (IWDEDetailZoneDef) base.InternalGetIndex(index);
			}
		}

		public IWDEDetailZoneDef Add(string DetailZoneName)
		{
			int res = base.VerifyName(DetailZoneName);
			if(res == 0)
			{
				IWDEDetailZoneDef def = WDEDetailZoneDef.Create();
				base.InternalAdd((WDEBaseCollectionItem) def);
				def.Name = DetailZoneName;
				return def;
			}
            else if (res == -1)
                 throw new WDEException("API00037", new object[] {DetailZoneName});
            else
                 throw new WDEException("API00038", new object[] {DetailZoneName});
		}

		public IWDEDetailZoneDef Add()
		{
			string newName = base.GetNextDefaultName( "DetailZone");
			IWDEDetailZoneDef def = WDEDetailZoneDef.Create();
			base.InternalAdd((WDEBaseCollectionItem) def);
			def.Name = newName;
			return def;
		}

        public void Add(IWDEDetailZoneDef newDef)
        {
            base.InternalAdd((WDEBaseCollectionItem)newDef);
        }

        public int Find(string DetailZoneName)
		{
			return base.InternalIndexOf(DetailZoneName);
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DetailZoneDef"))
			{
				Clear();

				while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) &&
					(XmlReader.Name == "DetailZoneDef"))
				{
					IWDEDetailZoneDef def = WDEDetailZoneDef.Create();
					base.InternalAdd((WDEBaseCollectionItem) def);
					IWDEXmlPersist ipers = (IWDEXmlPersist) def;
					ipers.ReadFromXml(XmlReader);
					base.RegisterObject((WDEBaseCollectionItem) def);
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion
	}

	public class WDEDetailZoneDef : WDEBaseCollectionItem, IWDEDetailZoneDef_R1, IWDEXmlPersist, IWDEDetailZoneDefInternal
	{
		private string m_DetailZoneName;
		private int m_LineCount;
		private int m_LineHeight;
		private Rectangle m_ZoneRect;
		private IWDEZoneDefs m_ZoneDefs;
        private int m_Width;

		private WDEDetailZoneDef()
		{
			m_DetailZoneName = "";

			m_ZoneDefs = WDEZoneDefs.Create(this);
		}

		public static IWDEDetailZoneDef Create()
		{
			return new WDEDetailZoneDef() as IWDEDetailZoneDef;
		}

		public static IWDEDetailZoneDef CreateInstance()
		{
			return Create();
		}

		protected override string InternalGetNodeName()
		{
			return m_DetailZoneName;
		}

		public override ArrayList GetChildCollections()
		{
			ArrayList al = new ArrayList();
			al.Add(m_ZoneDefs);
			return al;
		}

		#region IWDEDetailZoneDef Members

		public string Name
		{
			get
			{
				return m_DetailZoneName;
			}
			set
			{
				string newName = value;
				if(newName == null)
					newName = "";

				int res = base.Collection.VerifyName(newName);
				if(res == 0)
					m_DetailZoneName = newName;
                else if (res == -1)
                     throw new WDEException("API00037", new object[] {newName});
                else if (res == -2)
                     throw new WDEException("API00038", new object[] {newName});
			}
		}

		public int LineCount
		{
			get
			{
				return m_LineCount;
			}
			set
			{
				m_LineCount = value;
			}
		}

		public int LineHeight
		{
			get
			{
				return m_LineHeight;
			}
			set
			{
				m_LineHeight = value;
			}
		}

		public IWDEZoneDefs ZoneDefs
		{
			get
			{
				return m_ZoneDefs;
			}
		}

        public IWDEDetailZoneDef Clone()
        {
            return (IWDEDetailZoneDef)this.MemberwiseClone();
        }

        public int Width
        {
            get
            {
                return m_Width;
            }

            set
            {
                m_Width = value;
            }
        }

		#endregion

		#region IWDEDetailZoneDefInternal Members

		public Rectangle ZoneRect
		{
			get
			{
				return m_ZoneRect;
			}
			set
			{
				m_ZoneRect = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "DetailZoneDef"))
			{
				m_DetailZoneName = Utils.GetAttribute(XmlReader, "DetailZoneName", "");
				m_LineCount = Utils.GetIntValue(XmlReader, "LineCount");
				m_LineHeight = Utils.GetIntValue(XmlReader, "LineHeight");
                m_Width = Utils.GetIntValue(XmlReader, "Width");

				XmlReader.Read();
				XmlReader.MoveToContent();

				IWDEXmlPersist ipers = (IWDEXmlPersist) m_ZoneDefs;
				ipers.ReadFromXml(XmlReader);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "DetailZoneDef"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("DetailZoneDef");
			if(m_DetailZoneName != "")
				XmlWriter.WriteAttributeString("DetailZoneName", m_DetailZoneName);
			if(m_LineCount != 0)
				XmlWriter.WriteAttributeString("LineCount", m_LineCount.ToString());
			if(m_LineHeight != 0)
				XmlWriter.WriteAttributeString("LineHeight", m_LineHeight.ToString());
            if (m_Width != 0)
                XmlWriter.WriteAttributeString("Width", m_Width.ToString());

			IWDEXmlPersist ipers = (IWDEXmlPersist) m_ZoneDefs;
			ipers.WriteToXml(XmlWriter);

			XmlWriter.WriteEndElement();
		}

		#endregion
	}

	public class WDEZipLookupEditDef : WDEEditDef, IWDEZipLookupEditDef, IWDEXmlPersist
	{
		private IWDEFieldDef m_CityField;
		private IWDEFieldDef m_CityCodeField;
		private WDEZipLookupOption m_Options;
		private IWDEFieldDef m_StateField;
		private IWDEFieldDef m_ZipCodeField;

		private WDEZipLookupEditDef() : base()
		{
			SetNames("ZipLookup", "WebDX.Edits.ZipLookup");
		}

		public static IWDEZipLookupEditDef Create()
		{
			return new WDEZipLookupEditDef() as IWDEZipLookupEditDef;
		}

		public static IWDEZipLookupEditDef CreateInstance()
		{
			return Create();
		}

		protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			if(LinkedItem == m_CityField)
				m_CityField = null;
			else if(LinkedItem == m_CityCodeField)
				m_CityCodeField = null;
			else if(LinkedItem == m_StateField)
				m_StateField = null;
			else if(LinkedItem == m_ZipCodeField)
				m_ZipCodeField = null;
		}

		#region IWDEZipLookupEditDef Members

		public IWDEFieldDef CityField
		{
			get
			{
				return m_CityField;
			}
			set
			{
				if(m_CityField != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_CityField;
					obj.RemoveLink(this);
				}

				m_CityField = value;

				if(m_CityField != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_CityField;
					obj.AddLink(this);
				}
			}
		}

		public IWDEFieldDef CityCodeField
		{
			get
			{
				return m_CityCodeField;
			}
			set
			{
				if(m_CityCodeField != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_CityCodeField;
					obj.RemoveLink(this);
				}

				m_CityCodeField = value;

				if(m_CityCodeField != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_CityCodeField;
					obj.AddLink(this);
				}
			}
		}

		public WebDX.Api.WDEZipLookupOption Options
		{
			get
			{
				return m_Options;
			}
			set
			{
				m_Options = value;
			}
		}

		public IWDEFieldDef StateField
		{
			get
			{
				return m_StateField;
			}
			set
			{
				if(m_StateField != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_StateField;
					obj.RemoveLink(this);
				}

				m_StateField = value;

				if(m_StateField != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_StateField;
					obj.AddLink(this);
				}
			}
		}

		public IWDEFieldDef ZipCodeField
		{
			get
			{
				return m_ZipCodeField;
			}
			set
			{
				if(m_ZipCodeField != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_ZipCodeField;
					obj.RemoveLink(this);
				}

				m_ZipCodeField = value;

				if(m_ZipCodeField != null)
				{
					WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_ZipCodeField;
					obj.AddLink(this);
				}
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if(ConvertingOldProject)
			{
				ReadOldZipLookup(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ZipCodeLookupEditDef"))
				{
					base.ReadXmlAttributes(XmlReader);
					m_Options = Utils.GetZipLookupOptions(XmlReader, "Options");
					IWDEProjectInternal iproj = GetProjectInternal();
                    if (iproj == null)
                        throw new WDEException("API90006", new object[] {"ZipLookupEditDef"});

					string temp = Utils.GetAttribute(XmlReader, "CityField", "");
					if(temp != "")
						iproj.Resolver.AddRequest(this, "CityField", temp);
					temp = Utils.GetAttribute(XmlReader, "CityCodeField", "");
					if(temp != "")
						iproj.Resolver.AddRequest(this, "CityCodeField", temp);
					temp = Utils.GetAttribute(XmlReader, "StateField", "");
					if(temp != "")
						iproj.Resolver.AddRequest(this, "StateField", temp);
					temp = Utils.GetAttribute(XmlReader, "ZipCodeField", "");
					if(temp != "")
						iproj.Resolver.AddRequest(this, "ZipCodeField", temp);

					XmlReader.Read();
					XmlReader.MoveToContent();

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ZipCodeLookupEditDef"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("ZipCodeLookupEditDef");
			base.WriteXmlAttributes(XmlWriter);

			if(m_Options != WDEZipLookupOption.None)
				XmlWriter.WriteAttributeString("Options", m_Options.ToString());
			
			if(m_CityField != null)
			{
				WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_CityField;
				XmlWriter.WriteAttributeString("CityField", obj.GetNamePath());
			}
			if(m_CityCodeField != null)
			{
				WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_CityCodeField;
				XmlWriter.WriteAttributeString("CityCodeField", obj.GetNamePath());
			}
			if(m_StateField != null)
			{
				WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_StateField;
				XmlWriter.WriteAttributeString("StateField", obj.GetNamePath());
			}
			if(m_ZipCodeField != null)
			{
				WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_ZipCodeField;
				XmlWriter.WriteAttributeString("ZipCodeField", obj.GetNamePath());
			}

			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Methods

		private void ReadOldZipLookup(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ZipLookup"))
			{
				Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
				ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
				SessionMode = Utils.GetSessionType(XmlReader, "ViewModes");

				WDEZipLookupOption options = WDEZipLookupOption.None;
				if(Utils.GetBoolValue(XmlReader, "CityCodeOverride", false))
					options |= WDEZipLookupOption.CityCodeOverride;
				if(Utils.GetBoolValue(XmlReader, "OneHitPopup", false))
					options |= WDEZipLookupOption.OneHitPopup;
				if(Utils.GetBoolValue(XmlReader, "ReviewResults", false))
					options |= WDEZipLookupOption.ReviewResults;

				Options = options;

				IWDEProjectInternal iproj = GetProjectInternal();
				string temp = Utils.GetAttribute(XmlReader, "CityField", "");
				if(temp != "")
					iproj.Resolver.AddRequest(this, "CityField", temp);
				temp = Utils.GetAttribute(XmlReader, "CityCodeField", "");
				if(temp != "")
					iproj.Resolver.AddRequest(this, "CityCodeField", temp);
				temp = Utils.GetAttribute(XmlReader, "StateField", "");
				if(temp != "")
					iproj.Resolver.AddRequest(this, "StateField", temp);
                temp = Utils.GetAttribute(XmlReader, "ZipCodeField", "");
                if (temp != "")
                    iproj.Resolver.AddRequest(this, "ZipCodeField", temp);

				XmlReader.Read();
				XmlReader.MoveToContent();

				ReadDescription(XmlReader);
				ReadErrorMessage(XmlReader);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ZipLookup"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
	}

	public class WDEValidateEditDef : WDEEditDef, IWDEValidateEditDef, IWDEXmlPersist
	{
		private IWDEValidations m_Validations;

		private WDEValidateEditDef() : base()
		{
			m_Validations = WDEValidations.Create(this);
			this.SetNames("Validate", "WebDX.Edits.Validate");
		}

		public static IWDEValidateEditDef Create()
		{
			return new WDEValidateEditDef() as IWDEValidateEditDef;
		}

		public static IWDEValidateEditDef CreateInstance()
		{
			return Create();
		}

		#region IWDEValidateEditDef Members

		public IWDEValidations Validations
		{
			get
			{
				return m_Validations;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();

			if(ConvertingOldProject)
			{
				ReadOldValidate(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "ValidateEditDef"))
				{
					base.ReadXmlAttributes(XmlReader);

					XmlReader.Read();
					XmlReader.MoveToContent();

					IWDEXmlPersist ipers = (IWDEXmlPersist) m_Validations;
					ipers.ReadFromXml(XmlReader);

					if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "ValidateEditDef"))
					{
						XmlReader.ReadEndElement();
						XmlReader.MoveToContent();
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("ValidateEditDef");
			base.WriteXmlAttributes(XmlWriter);
			IWDEXmlPersist ipers = (IWDEXmlPersist) m_Validations;
			ipers.WriteToXml(XmlWriter);
			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Methods

		private void ReadOldValidate(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Validate"))
			{
				Enabled = Utils.GetBoolValue(XmlReader, "Enabled", false);
				ErrorType = Utils.GetEditErrorType(XmlReader, "ErrorType");
				SessionMode = Utils.GetSessionType(XmlReader, "ViewModes");

				XmlReader.Read();
				XmlReader.MoveToContent();

				ReadDescription(XmlReader);
				ReadErrorMessage(XmlReader);

				IWDEXmlPersist ipers = (IWDEXmlPersist) m_Validations;
				ipers.ReadFromXml(XmlReader);

				if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Validate"))
				{
					XmlReader.ReadEndElement();
					XmlReader.MoveToContent();
				}
			}
		}

		#endregion
	}

	public class WDEValidations : WDEBaseCollection, IWDEValidations, IWDEXmlPersist
	{
		private WDEValidations(object Parent) : base(Parent)
		{
		}

		public static IWDEValidations Create(object Parent)
		{
			return new WDEValidations(Parent) as IWDEValidations;
		}

		public static IWDEValidations CreateInstance(object Parent)
		{
			return Create(Parent);
		}

		#region IWDEValidations Members

		public IWDEValidation this[int index]
		{
			get
			{
				return (IWDEValidation) base.InternalGetIndex(index);
			}
		}

		public IWDEValidation Add()
		{
			IWDEValidation obj = WDEValidation.Create(this);
			this.InternalAdd((WDEBaseCollectionItem) obj, true);
			return obj;
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Validations"))
			{
				ReadOldValidations(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Validation"))
				{
					Clear();

					while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
						(XmlReader.Name == "Validation"))
					{
						IWDEValidation obj = WDEValidation.Create(this);
						this.InternalAdd((WDEBaseCollectionItem) obj, true);
						IWDEXmlPersist ipers = (IWDEXmlPersist) obj;
						ipers.ReadFromXml(XmlReader);
					}
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			for(int i = 0; i < Count; i++)
			{
				IWDEXmlPersist ipers = (IWDEXmlPersist) this[i];
				ipers.WriteToXml(XmlWriter);
			}
		}

		#endregion

		#region Private Methods

		private void ReadOldValidations(XmlTextReader XmlReader)
		{
			XmlReader.Read();
			XmlReader.MoveToContent();

			Clear();

			while((!XmlReader.EOF) && (XmlReader.NodeType == XmlNodeType.Element) && 
				(XmlReader.Name == "Validations"))
			{
				IWDEValidation obj = WDEValidation.Create(this);
				this.InternalAdd((WDEBaseCollectionItem) obj, true);
				IWDEXmlPersist ipers = (IWDEXmlPersist) obj;
				ipers.ReadFromXml(XmlReader);
			}

			if((XmlReader.NodeType == XmlNodeType.EndElement) && (XmlReader.Name == "Validations"))
			{
				XmlReader.ReadEndElement();
				XmlReader.MoveToContent();
			}
		}

		#endregion
	}

	public class WDEValidation : WDEBaseCollectionItem, IWDEValidation, IWDEXmlPersist
	{
		private string m_Expression;
		private string m_ErrorMessage;
		private WDEBaseCollection m_Collection;

		private WDEValidation(WDEBaseCollection Collection) : base()
		{
			m_Expression = "";
			m_ErrorMessage = "";
			m_Collection = Collection;
		}

		public static IWDEValidation Create(WDEBaseCollection Collection)
		{
			return new WDEValidation(Collection) as IWDEValidation;
		}

		public static IWDEValidation CreateInstance(WDEBaseCollection Collection)
		{
			return Create(Collection);
		}

		protected override string InternalGetNodeName()
		{
			int i = m_Collection.IndexOf(this);
			return "Validation" + i.ToString();
		}

		#region IWDEValidation Members

		public string Expression
		{
			get
			{
				return m_Expression;
			}
			set
			{
				if(value == null)
					m_Expression = "";
				else
					m_Expression = value;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return m_ErrorMessage;
			}
			set
			{
				if(value == null)
					m_ErrorMessage = "";
				else
					m_ErrorMessage = value;
			}
		}

		#endregion

		#region IWDEXmlPersist Members

		public void ReadFromXml(XmlTextReader XmlReader)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader","XmlReader cannot be null");

			XmlReader.MoveToContent();

			if(ConvertingOldProject)
			{
				ReadOldValidation(XmlReader);
			}
			else
			{
				if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Validation"))
				{
					m_Expression = Utils.GetAttribute(XmlReader, "Expression", "");
					m_ErrorMessage = Utils.GetAttribute(XmlReader, "ErrorMessage", "");

					XmlReader.ReadInnerXml();
					XmlReader.MoveToContent();
				}
			}
		}

		public void WriteToXml(XmlTextWriter XmlWriter)
		{
			if(XmlWriter == null)
				throw new ArgumentNullException("XmlWriter","XmlWriter cannot be null");

			XmlWriter.WriteStartElement("Validation");
			if(m_Expression != "")
				XmlWriter.WriteAttributeString("Expression", m_Expression);
			if(m_ErrorMessage != "")
				XmlWriter.WriteAttributeString("ErrorMessage", m_ErrorMessage);
			XmlWriter.WriteEndElement();
		}

		#endregion

		#region Private Methods

		private void ReadOldValidation(XmlTextReader XmlReader)
		{
			if((XmlReader.NodeType == XmlNodeType.Element) && (XmlReader.Name == "Validations"))
			{
				string temp = Utils.GetAttribute(XmlReader, "Expression", "");
				if(temp != "")
				{
					IWDEProjectInternal iproj = GetProjectInternal();
					iproj.AppendOldExpression(temp, GetNamePath());
					Expression = GetNamePath();
				}

				ErrorMessage = Utils.GetAttribute(XmlReader, "ErrorMessage", "");

				XmlReader.ReadInnerXml();
				XmlReader.MoveToContent();
			}
		}

		#endregion
	}

}
