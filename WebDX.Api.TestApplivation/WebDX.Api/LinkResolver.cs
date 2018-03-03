using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace WebDX.Api
{
	/// <summary>
	/// Quickly resolves links in Project files.
	/// </summary>
	public class LinkResolver
	{
		private Hashtable m_Objects;
		private ArrayList m_MethodReqList;
		private ArrayList m_MethodList;
		private ArrayList m_MethodArgsList;

		private ArrayList m_DupeReqList;
		private ArrayList m_DupeParamList;
		private ArrayList m_DupeTargetList;
		private ArrayList m_DupeReqMethodList;

		private ArrayList m_ReqList;
		private ArrayList m_PropList;
		private ArrayList m_NameList;
		private Hashtable m_NameConvertList;
		private Hashtable m_ConvertedNamesList;
		private ArrayList m_IndexList;
		private Hashtable m_ConvertedLinksList;
		private Hashtable m_SuppressedRequestList;

		private Hashtable m_FormNameConvertList;

		public LinkResolver()
		{
			m_Objects = new Hashtable(10000);
			m_NameConvertList = new Hashtable();
			m_ConvertedNamesList = new Hashtable();
			m_ConvertedLinksList = new Hashtable();
			m_SuppressedRequestList = new Hashtable();
			
			m_MethodReqList = new ArrayList();
			m_MethodList = new ArrayList();
			m_MethodArgsList = new ArrayList();

			m_DupeReqList = new ArrayList();
			m_DupeParamList = new ArrayList();
			m_DupeTargetList = new ArrayList();
			m_DupeReqMethodList = new ArrayList();

			m_ReqList = new ArrayList(10000);
			m_PropList = new ArrayList(10000);
			m_NameList = new ArrayList(10000);
			m_IndexList = new ArrayList(10000);

			m_FormNameConvertList = new Hashtable();
		}

		/// <summary>
		/// Adds an object to the resolver list
		/// </summary>
		/// <param name="fullName">A unique name identifying the object</param>
		/// <param name="newObject">The object to add</param>
		public void AddObject(string fullName, object newObject)
		{
			if(fullName == null)
				throw new ArgumentNullException("fullName","fullName cannot be null");
			if(newObject == null)
				throw new ArgumentNullException("newObject", "newObject cannot be null");

			if(!m_Objects.ContainsKey(fullName))
				m_Objects.Add(fullName, newObject);
		}

		public void Clear()
		{
			m_Objects.Clear();
			m_NameConvertList.Clear();
			m_ConvertedNamesList.Clear();
			m_ConvertedLinksList.Clear();
			m_SuppressedRequestList.Clear();
			
			m_MethodReqList.Clear();
			m_MethodList.Clear();
			m_MethodArgsList.Clear();

			m_DupeReqList.Clear();
			m_DupeParamList.Clear();
			m_DupeTargetList.Clear();
			m_DupeReqMethodList.Clear();

			m_ReqList.Clear();
			m_PropList.Clear();
			m_NameList.Clear();
			m_IndexList.Clear();

			m_FormNameConvertList.Clear();
		}

		/// <summary>
		/// Fills the object list with all objects currently in the project
		/// </summary>
		/// <param name="project">The project to iterate when filling the list</param>
		public void FillObjectsFromProject(IWDEProject project)
		{
			Clear();

			foreach(IWDEDocumentDef docDef in project.DocumentDefs)
			{
				WDEBaseCollectionItem item = (WDEBaseCollectionItem) docDef;
				AddObject(item.GetNamePath(), docDef);
				FillRecords(docDef.RecordDefs);
				FillImages(docDef.ImageSourceDefs);
				FillForms(docDef.FormDefs);
			}			

			foreach(IWDESessionDef def in project.SessionDefs)
			{
				WDEBaseCollectionItem item = (WDEBaseCollectionItem) def;
				AddObject(item.GetNamePath(), def);
			}
		}

		/// <summary>
		/// Gets an object from the resolver list
		/// </summary>
		/// <param name="fullName">The unique name identifying the object</param>
		/// <returns>The object if it exists in the list or null if it does not</returns>
		public object GetObject(string fullName)
		{
			if(fullName == null)
                throw new ArgumentNullException("fullName", "fullName cannot be null");

			return m_Objects[fullName];
		}

		/// <summary>
		/// Adds a link request to the link request list
		/// </summary>
		/// <param name="requestor">The object requesting a link</param>
		/// <param name="propertyName">The property name to assign the linked object to</param>
		/// <param name="requestedObjectName">The unique name of the object to link</param>
		public void AddRequest(object requestor, string propertyName, string requestedObjectName)
		{
			AddRequest(requestor, propertyName, requestedObjectName, -1, "");
		}

		/// <summary>
		/// Adds a link request to the link request list
		/// </summary>
		/// <param name="requestor">The object requesting a link</param>
		/// <param name="propertyName">The property name to assign the linked object to</param>
		/// <param name="requestedObjectName">The unique name of the object to link</param>
		/// <param name="propertyIndex">The index in the propertyName property to assign the object to</param>
		public void AddRequest(object requestor, string propertyName, string requestedObjectName, int propertyIndex)
		{
			AddRequest(requestor, propertyName, requestedObjectName, propertyIndex, "");
		}

		/// <summary>
		/// Adds a link request to the link request list
		/// </summary>
		/// <param name="requestor">The object requesting a link</param>
		/// <param name="propertyName">The property name to assign the linked object to</param>
		/// <param name="requestedObjectName">The unique name of the object to link</param>
		/// <param name="propertyIndex">The index in the propertyName property to assign the object to</param>
		/// <param name="convertName">The converted requested object name</param>
		public void AddRequest(object requestor, string propertyName, string requestedObjectName, int propertyIndex, string convertName)
		{
			if(requestor == null)
				throw new ArgumentNullException("requestor", "requestor cannot be null");
			if(propertyName == null)
				throw new ArgumentNullException("propertyName", "propertyName cannot be null");
			if(requestedObjectName == null)
				throw new ArgumentNullException("requestedObjectName", "requestedObjectName cannot be null");
			if(convertName == null)
				throw new ArgumentNullException("convertName", "convertName cannot be null");

			PropertyInfo pi = requestor.GetType().GetProperty(propertyName);
			if(pi == null)
				throw new ArgumentException(requestor.GetType().FullName + " does not contain a property called " + propertyName, "propertyName");

			m_ReqList.Add(requestor);
			m_PropList.Add(pi);
			int nameIndex = m_NameList.Add(requestedObjectName);
			if(convertName != "")
			{
				ArrayList indexList = (ArrayList) m_NameConvertList[convertName];
				if(indexList == null)
				{
					indexList = new ArrayList();
					m_NameConvertList.Add(convertName, indexList);
				}

				indexList.Add(nameIndex);
			}
			m_IndexList.Add(propertyIndex);
		}

		/// <summary>
		/// Adds a link to the link request list, ignoring errors encountered during linking.
		/// </summary>
		/// <param name="requestor">The object requesting a link</param>
		/// <param name="propertyName">The property name to assign the linked object to</param>
		/// <param name="requestedObjectName">The unique name of the object to link</param>
		/// <param name="propertyIndex">The index in the propertyName property to assign the object to</param>
		/// <param name="convertName">The converted requested object name</param>
		public void AddSuppressedRequest(object requestor, string propertyName, string requestedObjectName, int propertyIndex, string convertName)
		{
			AddRequest(requestor, propertyName, requestedObjectName, propertyIndex, convertName);
			m_SuppressedRequestList.Add(m_ReqList.Count - 1, true);
		}

		/// <summary>
		/// Adds a name conversion to the conversion list
		/// </summary>
		/// <param name="OldName">The original name</param>
		/// <param name="NewName">The converted name</param>
		public void AddConvertName(string OldName, string NewName)
		{
			if(!m_ConvertedNamesList.ContainsKey(OldName))
				m_ConvertedNamesList.Add(OldName, NewName);
		}

		/// <summary>
		/// Adds a link name conversion to the conversion list
		/// </summary>
		/// <param name="OldName">The original name</param>
		/// <param name="NewName">The converted name</param>
		public void AddConvertLink(string OldName, string NewName)
		{
			if(!m_ConvertedLinksList.ContainsKey(OldName))
				m_ConvertedLinksList.Add(OldName, NewName);
		}

		/// <summary>
		/// Adds a request to link an object by calling a method
		/// </summary>
		/// <param name="requestor">The requesting object</param>
		/// <param name="methodName">The method to call</param>
		/// <param name="args">The arguments to be passed when calling methodName</param>
		public void AddMethodRequest(string requestor, string methodName, object[] args)
		{
			if(requestor == null)
                throw new ArgumentNullException("requestor",  "requestor cannot be null");
			if(methodName == null)
				throw new ArgumentNullException( "methodName",  "methodName cannot be null");

			m_MethodReqList.Add(requestor);
			m_MethodList.Add(methodName);
			m_MethodArgsList.Add(args);
		}

		/// <summary>
		/// Adds a request to link a clone of the object, with propertyOverrides to the link list
		/// </summary>
		/// <param name="requestor">The requesting object</param>
		/// <param name="dupeTarget">The unique name of the object to dupe</param>
		/// <param name="propOverrides">Propety overrides for the dupe target</param>
		/// <param name="reqMethodName">The method to call for linking the duped object</param>
		public void AddDupeRequest(object requestor, string dupeTarget, Hashtable propOverrides, string reqMethodName)
		{
			if(requestor == null)
                throw new ArgumentNullException("requestor", "requestor cannot be null");
			if(dupeTarget == null)
				throw new ArgumentNullException("dupeTarget", "dupeTarget cannot be null");
			if(reqMethodName == null)
				throw new ArgumentNullException("reqMethodName", "reqMethodName cannot be null");

			m_DupeReqList.Add(requestor);
			m_DupeTargetList.Add(dupeTarget);
			m_DupeParamList.Add(propOverrides);
			m_DupeReqMethodList.Add(reqMethodName);
		}

		/// <summary>
		/// Adds a form name conversion request to the convert list. Used to ensure form renaming does not break links during load.
		/// </summary>
		/// <param name="oldName">The original form name.</param>
		/// <param name="newName">The new form name.</param>
		public void AddFormNameConversion(string oldName, string newName)
		{
			m_FormNameConvertList.Add(oldName, newName);
		}

		/// <summary>
		/// Gets the converted form name stored using AddFormNameConversion.
		/// </summary>
		/// <param name="oldName">The linked form name.</param>
		/// <returns>The converted form name.</returns>
		public string GetConvertedFormName(string oldName)
		{
			string result = (string) m_FormNameConvertList[oldName];
			if(result == null)
				result = oldName;
			return result;
		}

		/// <summary>
		/// Perform name conversions, resolve links and dupe requests.
		/// </summary>
		public void ResolveLinks()
		{
			ResolveNameConversions();
			ResolveLinkConversions();

			for(int i = 0; i < m_ReqList.Count; i++)
			{
				object req = m_ReqList[i];
				PropertyInfo pi = (PropertyInfo) m_PropList[i];
				object link = m_Objects[m_NameList[i]];

                if (link == null)
                    throw new WDEException("API00028", new object[] { m_NameList[i], req });

				int propertyIndex = (int) m_IndexList[i];
				
				//Allow the object to be converted by the requestor if the requestor implements IWDELinkConverter
				if(req is IWDELinkConverter)
				{
					IWDELinkConverter converter = (IWDELinkConverter) req;
					link = converter.ConvertLinkObject(link);
				}

				try
				{
					if(propertyIndex != -1)
						pi.SetValue(req, link, new object[] {propertyIndex});
					else
						pi.SetValue(req, link, null);
				}
				catch(Exception ex)
				{
                    if (!m_SuppressedRequestList.ContainsKey(i))
                        throw new WDEException("API90009", new object[] {req.ToString(), pi.Name, m_NameList[i], ex.Message}, ex);
				}
				
			}

			for(int i = 0; i < m_MethodReqList.Count; i++)
			{
				object req = m_Objects[m_MethodReqList[i]];
                if (req == null)
                     throw new WDEException("API00028", new object[] { m_MethodReqList[i] });
				MethodInfo mi = (MethodInfo) req.GetType().GetMethod((string) m_MethodList[i]);

                if (mi == null)
                     throw new WDEException("API00031", new object[] { m_MethodReqList[i], m_MethodList[i] });
				object[] args = (object[]) m_MethodArgsList[i];

				try
				{
					mi.Invoke(req, args);
				}
				catch(Exception ex)
				{
                       throw new WDEException("API90010", new object[] {req.ToString(), mi.Name, ex.Message}, ex);

				}
			}

			for(int i = 0; i < m_DupeReqList.Count; i++)
			{
				object req = m_DupeReqList[i];
				MethodInfo mi = (MethodInfo) req.GetType().GetMethod((string) m_DupeReqMethodList[i]);
                if (mi == null)
                    throw new WDEException("API00031", new object[] { m_DupeReqList[i].GetType().FullName, m_DupeReqMethodList[i] });

				object targ = m_Objects[m_DupeTargetList[i]];
                if (targ == null)
                       throw new WDEException("API00028", new object[] {m_DupeTargetList[i]});

                if (targ is ICloneable)
                    targ = ((ICloneable)targ).Clone();
                else
                    throw new WDEException("API90007", new object[] {m_DupeTargetList[i]});

				Hashtable ov = (Hashtable) m_DupeParamList[i];
				foreach(string key in ov.Keys)
				{
					PropertyInfo pi = (PropertyInfo) targ.GetType().GetProperty(key);
                    if (pi == null)
                       throw new WDEException("API00033", new object[] { m_DupeTargetList[i], key });
					try
					{
						pi.SetValue(targ, ov[key], null);
					}
					catch(Exception ex)
					{
                           throw new WDEException("API90011", new object[] {targ.ToString(), pi.Name, ov[key], ex.Message}, ex);
					}
				}

				try
				{
					mi.Invoke(req, new object[] {targ});
				}
				catch(Exception ex)
				{
                        throw new WDEException("API90012", new object[] {req.ToString(), mi.Name, targ.ToString(), ex.Message}, ex);
				}
			}

			m_Objects.Clear();
			m_ReqList.Clear();
			m_PropList.Clear();
			m_NameList.Clear();
			m_IndexList.Clear();
			m_MethodReqList.Clear();
			m_MethodList.Clear();
			m_MethodArgsList.Clear();
			m_DupeReqList.Clear();
			m_DupeTargetList.Clear();
			m_DupeParamList.Clear();
			m_DupeReqMethodList.Clear();
			m_SuppressedRequestList.Clear();
		}

		#region Private Methods

		/// <summary>
		/// Resolve all name conversions
		/// </summary>
		private void ResolveNameConversions()
		{
			foreach(string Key in m_ConvertedNamesList.Keys)
			{
				string newName = (string) m_ConvertedNamesList[Key];
				ArrayList nameList = (ArrayList) m_NameConvertList[Key];
				if(nameList != null)
				{
					foreach(int index in nameList)
					{
						string buf = (string) m_NameList[index];
						buf = buf.Replace(Key, newName);
						m_NameList[index] = buf;
					}

					nameList.Clear();
				}
			}

			m_ConvertedNamesList.Clear();
			m_NameConvertList.Clear();
		}

		/// <summary>
		/// Resolve all link name conversions
		/// </summary>
		private void ResolveLinkConversions()
		{
			for(int i = 0; i < m_NameList.Count; i++)
			{
				if(m_ConvertedLinksList.ContainsKey((string) m_NameList[i]))
				{
					m_NameList[i] = m_ConvertedLinksList[(string) m_NameList[i]];
				}
			}

			m_ConvertedLinksList.Clear();
		}

		private void FillRecords(IWDERecordDefs defs)
		{
			foreach(IWDERecordDef def in defs)
			{
				WDEBaseCollectionItem item = (WDEBaseCollectionItem) def;
				AddObject(item.GetNamePath(), def);

				foreach(IWDEFieldDef fieldDef in def.FieldDefs)
				{
					item = (WDEBaseCollectionItem) fieldDef;
					AddObject(item.GetNamePath(), fieldDef);
				}

				FillRecords(def.RecordDefs);
			}
		}

		private void FillImages(IWDEImageSourceDefs defs)
		{
			foreach(IWDEImageSourceDef def in defs)
			{
				WDEBaseCollectionItem item = (WDEBaseCollectionItem) def;
				AddObject(item.GetNamePath(), def);

				foreach(IWDEZoneDef zoneDef in def.ZoneDefs)
				{
					item = (WDEBaseCollectionItem) zoneDef;
					AddObject(item.GetNamePath(), zoneDef);
				}

				foreach(IWDEDetailZoneDef detailDef in def.DetailZoneDefs)
				{
					item = (WDEBaseCollectionItem) detailDef;
					AddObject(item.GetNamePath(), detailDef);

					foreach(IWDEZoneDef subZone in detailDef.ZoneDefs)
					{
						item = (WDEBaseCollectionItem) subZone;
						AddObject(item.GetNamePath(), subZone);
					}
				}

				foreach(IWDESnippetDef snipDef in def.SnippetDefs)
				{
					item = (WDEBaseCollectionItem) snipDef;
					AddObject(item.GetNamePath(), snipDef);
				}
			}
		}

		private void FillForms(IWDEFormDefs defs)
		{
			foreach(IWDEFormDef formDef in defs)
			{
				WDEBaseCollectionItem item = (WDEBaseCollectionItem) formDef;
				AddObject(item.GetNamePath(), formDef);

				FillControlDefs(formDef.ControlDefs);
			}
		}

		private void FillControlDefs(IWDEControlDefs defs)
		{
			foreach(IWDEControlDef controlDef in defs)
			{
				WDEBaseCollectionItem item = (WDEBaseCollectionItem) controlDef;
				AddObject(item.GetNamePath(), controlDef);

				if(controlDef is IWDETextBoxDef)
				{
					IWDETextBoxDef textDef = (IWDETextBoxDef) controlDef;
					FillEditDefs(textDef.EditDefs);
				}
				else if(controlDef is IWDEDetailGridDef)
				{
					IWDEDetailGridDef detailDef = (IWDEDetailGridDef) controlDef;
					FillControlDefs(detailDef.HeaderControlDefs);
				}
			}
		}

		private void FillEditDefs(IWDEEditDefs defs)
		{
			foreach(IWDEEditDef def in defs)
			{
				WDEBaseCollectionItem item = (WDEBaseCollectionItem) def;
				AddObject(item.GetNamePath(), def);
			}
		}

		#endregion
	}
}
