using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace WebDX.Api
{
	/// <summary>
	/// Base class for all collections in the Project API. Supports linking objects.
	/// </summary>
	public abstract class WDEBaseCollection : MarshalByRefObject, ILinkNotify, IEnumerable
	{
		private const string NAME_MATCH = "^[a-zA-Z_][0-9a-zA-Z_]*$"; // enforces naming rules
		internal ArrayList m_List;
		private object m_Parent;
		private bool m_Unlinking;

		public WDEBaseCollection(object parent)
		{
			m_List = new ArrayList();
			m_Parent = parent;
		}

        public override object InitializeLifetimeService()
        {
            return null;
        }

		protected virtual bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			// do nothing in the base implementation
			return false;
		}

		protected virtual void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			// do nothing in the base implementation
		}

		protected object InternalFind(string Name)
		{
			for(int i = 0; i < m_List.Count; i++)
			{
				if(MatchesName((WDEBaseCollectionItem) m_List[i], Name))
					return m_List[i];
			}
			return null;
		}

		protected int InternalIndexOf(string Name)
		{
			for(int i = 0; i < m_List.Count; i++)
			{
				if(MatchesName((WDEBaseCollectionItem) m_List[i], Name))
					return i;
			}
			return -1;
		}

		protected void RegisterObject(WDEBaseCollectionItem obj)
		{
			IWDEProjectInternal proj = GetProjectInternal();
			if(proj.Resolver != null)
			{
				proj.Resolver.AddObject(obj.GetNamePath(), obj);
			}
		}

		internal IWDEProjectInternal GetProjectInternal()
		{
			object topParent = TopParent();
			if((topParent != null) && (topParent is IWDEProjectInternal))
				return (IWDEProjectInternal) topParent;
			else
				return null;
		}

		protected void InternalAdd(WDEBaseCollectionItem obj, bool Owner)
		{
			if(obj != null)
			{
				if(Owner)
					obj.Parent = m_Parent;
				obj.Collection = this;
			}
			m_List.Add(obj);
		}

		protected void InternalAdd(WDEBaseCollectionItem obj)
		{
			InternalAdd(obj, true);
		}

		protected WDEBaseCollectionItem InternalGetIndex(int index)
		{
			return (WDEBaseCollectionItem) m_List[index];
		}

		protected void InternalSetIndex(int index, WDEBaseCollectionItem item)
		{
			InternalSetIndex(index, item, true);
		}

		protected void InternalSetIndex(int index, WDEBaseCollectionItem item, bool Owner)
		{
			if(item != null && Owner)
				item.Parent = m_Parent;
			m_List[index] = item;
		}

		protected object Parent
		{
			get
			{
				return m_Parent;
			}
		}

		protected object TopParent()
		{
			if((Parent != null) && (Parent is WDEBaseCollectionItem))
			{
				WDEBaseCollectionItem current = (WDEBaseCollectionItem) Parent;
				while(current.Parent != null)
				{
					if(current.Parent is WDEBaseCollectionItem)
						current = (WDEBaseCollectionItem) current.Parent;
					else
						return current.Parent;
				}
				return current;
			}
			else
				return Parent;
		}

		protected ArrayList List
		{
			get
			{
				return m_List;
			}
		}

		public int Count
		{
			get
			{
				return m_List.Count;
			}
		}

        public virtual void Insert(WDEBaseCollectionItem obj)
        {
            InternalAdd(obj, true);
        }

		public virtual void Remove(WDEBaseCollectionItem obj)
		{
			if(!m_Unlinking)
                obj.NotifyLinks();
			obj.Parent = null;
			obj.Collection = null;
			m_List.Remove(obj);
		}

        public void RemoveLink(WDEBaseCollectionItem obj)
        {
            m_List.Remove(obj);
        }

		public void RemoveAt(int index)
		{
			WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_List[index];
			Remove(obj);
		}

		public int IndexOf(WDEBaseCollectionItem obj)
		{
			return m_List.IndexOf(obj);
		}

		public virtual void Clear()
		{
			for(int i = 0; i < m_List.Count; i++)
			{
				WDEBaseCollectionItem obj = (WDEBaseCollectionItem) m_List[i];
				if(!m_Unlinking)
					obj.NotifyLinks();
				obj.Parent = null;
				obj.Collection = null;
				obj.ClearNotify();
			}

			m_List.Clear();
		}

		public virtual int VerifyName(string newName)
		{
            if(Regex.Match(newName, NAME_MATCH).Success)
            {
                if (NameExists(newName))
                    return -2;
                else
                    return 0;
            }
            else
                return -1; // invalid characters in name
		}

		public virtual bool NameExists(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name", "name cannot be null");
			if(name == "")
                throw new ArgumentException("name cannot be blank",  "name");

			ArrayList collections = GetCollectionList();
			foreach(WDEBaseCollection collection in collections)
			{
				foreach(WDEBaseCollectionItem item in collection)
				{
					if(string.Compare(item.GetNodeName(), name, true) == 0)
					{
						return true;
					}
				}
			}

			return false;
		}

		public virtual string RepairNameCollsion(string duplicateName)
		{
			if(duplicateName == null)
				throw new ArgumentNullException("duplicateName", "duplicateName cannot be null");
			if(duplicateName == "")
                throw new ArgumentException("duplicateName cannot be null", "duplicateName");

			int suffix = 1;
			duplicateName += "_";
			ArrayList collections = GetCollectionList();
			foreach(WDEBaseCollection collection in collections)
			{
				foreach(WDEBaseCollectionItem item in collection)
				{
					string testName = item.GetNodeName();
					if(testName.Length > duplicateName.Length)
						testName = testName.Substring(0, duplicateName.Length);

					if(string.Compare(testName, duplicateName, true) == 0)
						suffix++;
				}
			}

			return duplicateName + suffix.ToString();
		}

		public virtual string GetNextDefaultName(string nameRoot)
		{
			if(nameRoot == null)
				throw new ArgumentNullException("nameRoot", "nameRoot cannot be null");
			if(nameRoot == "")
                throw new ArgumentException("nameRoot cannot be blank", "nameRoot");

			int suffix = 0;
			foreach(WDEBaseCollectionItem item in m_List)
			{
				string testName = item.GetNodeName();
                Match m = Regex.Match(testName, "^" + nameRoot + "([0-9]+)");
                if (m.Success)
                {
                    int index = 0;
                    if (int.TryParse(m.Groups[1].Value, out index))
                    {
                        if (suffix < index)
                            suffix = index;
                    }
                }
			}

            suffix++;
			return nameRoot + suffix.ToString();
		}

		public int Level
		{
			get
			{
				int result = 1;
				if(Parent is WDEBaseCollectionItem)
				{
					WDEBaseCollectionItem cur = (WDEBaseCollectionItem) Parent;
					object top = TopParent();
					while((cur != null) && (cur != top))
					{
						result++;
						if(cur.Parent is WDEBaseCollectionItem)
							cur = (WDEBaseCollectionItem) cur.Parent;
						else
							break;
					}
				}
				return result;
			}
		}

		public object GetParentInterface(string parentInterface)
		{
			object current = Parent;
			while((current != null) && (current.GetType().GetInterface(parentInterface) == null) && (current is WDEBaseCollectionItem))
				current = ((WDEBaseCollectionItem) current).Parent;

			if((current != null) && (current.GetType().GetInterface(parentInterface) != null))
				return current;
			else
				return null;
		}

		protected virtual ArrayList GetCollectionList()
		{
			ArrayList result = new ArrayList();
			result.Add(this);
			return result;
		}

		protected ArrayList GetSameLevelCollections()
		{
			int level = Level;
			ArrayList result = new ArrayList();

			object top = TopParent();
			if(top is IWDEProjectInternal)
			{
				ArrayList topCollections = ((IWDEProjectInternal) top).GetTopLevelCollections();

				foreach(WDEBaseCollection topCollection in topCollections)
				{
					AddCollectionsForLevel(level, result, topCollection);
				}
			}

			return result;
		}

		private void AddCollectionsForLevel(int level, ArrayList targetList, WDEBaseCollection collection)
		{
			if(collection.Level == level)
				targetList.Add(collection);
			else
			{
				foreach(WDEBaseCollectionItem item in collection)
				{
					ArrayList childCollections = item.GetChildCollections();
					if(childCollections != null)
					{
						foreach(WDEBaseCollection child in childCollections)
							AddCollectionsForLevel(level, targetList, child);
					}
				}
			}
		}

		#region ILinkNotify Members

		public void LinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			m_Unlinking = true;
			try
			{
				InternalLinkNotify(LinkedItem);
			}
			finally
			{
				m_Unlinking = false;
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			IEnumerable ienum = (IEnumerable) m_List;
			return ienum.GetEnumerator();
		}

		#endregion
	}

	/// <summary>
	/// Base class for all collection items in the API. Supports linking.
	/// </summary>
	public abstract class WDEBaseCollectionItem : MarshalByRefObject, ILinkNotify
	{
		private ArrayList m_LinkedObjects;
		private object m_Parent;
		private WDEBaseCollection m_Collection;

		public WDEBaseCollectionItem()
		{
			m_LinkedObjects = new ArrayList();
		}

        public override object InitializeLifetimeService()
        {
            return null;
        }

#if DEBUG
		public void AddLink(ILinkNotify linkedObject)
#else
		internal void AddLink(ILinkNotify linkedObject)
#endif
		{
			m_LinkedObjects.Add(linkedObject);
		}

#if DEBUG
		public void RemoveLink(ILinkNotify linkedObject)
#else
		internal void RemoveLink(ILinkNotify linkedObject)
#endif
		{
			m_LinkedObjects.Remove(linkedObject);
		}

		protected virtual void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			// do nothing in the base implementation
		}

		protected virtual void InternalClearNotify()
		{
			// do nothing in the base implementation
		}

        protected ArrayList GetLinkedObjects()
        {
            return m_LinkedObjects;
        }

		internal IWDEProjectInternal GetProjectInternal()
		{
			object topParent = TopParent();
			if((topParent != null) && (topParent is IWDEProjectInternal))
				return (IWDEProjectInternal) topParent;
			else
				return null;
		}

		protected bool ConvertingOldProject
		{
			get
			{
				IWDEProjectInternal proj = GetProjectInternal();
				if(proj != null)
					return proj.ConvertOldFormat;
				else
					return false;
			}
		}

		protected abstract string InternalGetNodeName();

		public object GetParentInterface(string parentInterface)
		{
			object current = Parent;
			while((current != null) && (current.GetType().GetInterface(parentInterface) == null) && (current is WDEBaseCollectionItem))
				current = ((WDEBaseCollectionItem) current).Parent;

			if((current != null) && (current.GetType().GetInterface(parentInterface) != null))
				return current;
			else
				return null;
		}

		public string GetNodeName()
		{
			return InternalGetNodeName();
		}

		public virtual ArrayList GetChildCollections()
		{
			return null;
		}

		public string GetNamePath()
		{
			string result = InternalGetNodeName();
			if((Parent != null) && (Parent is WDEBaseCollectionItem))
			{
				WDEBaseCollectionItem parent = (WDEBaseCollectionItem) Parent;
				result = parent.GetNamePath() + "." + result;
			}
			return result;
		}

		protected object TopParent()
		{
			WDEBaseCollectionItem current = this;
			while(current.Parent != null)
			{
				if(current.Parent is WDEBaseCollectionItem)
					current = (WDEBaseCollectionItem) current.Parent;
				else
					return current.Parent;
			}
			return current;
		}

		public void NotifyLinks()
		{
			InternalLinkNotify(null);

			for(int i = 0; i < m_LinkedObjects.Count; i++)
			{
				ILinkNotify obj = (ILinkNotify) m_LinkedObjects[i];
				obj.LinkNotify(this);
			}

			m_LinkedObjects.Clear();
		}

		public void ClearNotify()
		{
			InternalClearNotify();
		}

		public object Parent
		{
			get
			{
				return m_Parent;
			}

			set
			{
				m_Parent = value;
			}
		}

		public WDEBaseCollection Collection
		{
			get
			{
				return m_Collection;
			}
			set
			{
				m_Collection = value;
			}
		}

		#region ILinkNotify Members

		public void LinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			InternalLinkNotify(LinkedItem);
		}

		#endregion
	}

	/// <summary>
	/// Used during conversion of old projects to preserve the keyorder when controls are moved to the form collection
	/// </summary>
#if DEBUG
	public class KeyOrderList
#else
	internal class KeyOrderList
#endif
	{
		private ArrayList m_List;

		public KeyOrderList()
		{
			Init();
		}

		public void Add(object def, int keyOrder)
		{
			while(m_List.Count < keyOrder + 1)
				m_List.Add(null);
			if(m_List[keyOrder] == null)
				m_List[keyOrder] = def;
			else
				m_List.Insert(keyOrder, def);
		}

		public void ReOrder(IWDEControlDefs defs)
		{
			Flatten();

			Hashtable ht = new Hashtable();
			for(int i = 0; i < m_List.Count; i++)
			{
				ht.Add(m_List[i], i);
			}

			foreach(IWDEControlDef def in defs)
			{
				object res = ht[def];
				if(res != null)
					def.KeyOrder = (int) res;
			}
		}

		public ArrayList List
		{
			get
			{
				return m_List;
			}
		}

		private void Init()
		{
			m_List = new ArrayList();
		}
		

		private void Flatten()
		{
			int i = 0;
			while(i < m_List.Count)
			{
				if(m_List[i] == null)
					m_List.RemoveAt(i);
				else if(m_List[i] is KeyOrderList)
				{
					KeyOrderList subList = ((KeyOrderList) m_List[i]);
					m_List.RemoveAt(i);
					i += subList.JoinTo(this, i);
				}
				else
					i++;
			}
		}

		private int JoinTo(KeyOrderList newList, int index)
		{
			Flatten();
			int result = 0;
			foreach(IWDEControlDef def in m_List)
			{
				newList.List.Insert(index++, def);
				result++;
			}
			return result;
		}

	}

#if DEBUG
	public interface ILinkNotify
#else
	internal interface ILinkNotify
#endif
	{
		void LinkNotify(WDEBaseCollectionItem LinkedItem);
	}
}
