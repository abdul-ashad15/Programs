using System;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Xml;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;

using WebDX.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock;

namespace WebDX.Api.Tests
{
	[TestClass]
	public class ProjectBaseObjectTests
	{
		MockObject m_Project;

		public ProjectBaseObjectTests()
		{
		}

		[TestInitialize]
		public void Init()
		{
			MockManager.Init();

			m_Project = MockManager.MockObject(typeof(IWDEProjectInternal));
			
		}

		[TestCleanup]
		public void Cleanup()
		{
			m_Project = null;

			MockManager.Verify();
			GC.Collect();
		}

		[TestMethod]
		public void LinkTest()
		{
			TestCollection t1 = new TestCollection(null);
			TestCollection2 t2 = new TestCollection2(null);
			t1.Add("Item1");
			t2.Add("Item2");
			Assert.AreEqual("Item1", t1[0].ItemName, "Item1");
			Assert.AreEqual("Item2", t2[0].ItemName, "Item2");

			t1[0].Test2 = t2[0];
			Assert.AreEqual("Item2", t1[0].Test2.ItemName, "Item1.Test2");
			t2.Clear();
			Assert.IsNull(t1[0].Test2, "Test2 is not null (Clear). Expected null");

			t2.Add("Item2");
			t1[0].Test2 = t2[0];
			t2.RemoveAt(0);
			Assert.IsNull(t1[0].Test2, "Test2 is not null (RemoveAt). Expected null");
			
			t2.Add("Item2");
			t1[0].Test2 = t2[0];
			t2.Remove(t2[0]);
			Assert.IsNull(t1[0].Test2, "Test2 is not null (Remove). Expected null");

			t2.Clear();
			t2.Add("Item2");
			t1[0].Test2 = t2[0];
			t1[0].Test2 = null;
			t2.Clear();
			Assert.IsNull(t1[0].Test2, "Test2 is not null (set to null). Expected null");

			t2.Clear();
			t2.Add("Item2");
			t1[0].Test2 = t2[0];
			t1.Clear();
			t2.Clear();
		}

		[TestMethod]
		public void LinkListTest()
		{
			TestCollection t1 = new TestCollection(null);
			TestLinkList t2 = new TestLinkList(null);

			t1.Add("Item1");
			t1.Add("Item2");
			t1.Add("Item3");

			t2.Add(t1[0]);
			t2.Add(t1[1]);
			t2.Add(t1[2]);

			t1.Clear();
			Assert.AreEqual(0, t2.Count, "t2.Count");

			t1.Add("Item1");
			t1.Add("Item2");
			t1.Add("Item3");

			t2.Add(t1[0]);
			t2.Add(t1[1]);
			t2.Add(t1[2]);

			t1.RemoveAt(0);
			Assert.AreEqual("Item2", t2[0].ItemName, "t2[0].ItemName");
			t1.RemoveAt(1);
			Assert.AreEqual(1, t2.Count, "t2.Count (2)");
			Assert.AreEqual("Item2", t2[0].ItemName, "t2[0].ItemName (2)");
			t1.RemoveAt(0);
			Assert.AreEqual(0, t2.Count, "t2.Count final");
		}

		[TestMethod]
		public void AddRemoveClear()
		{
			TestCollection t1 = new TestCollection(null);
			t1.Add("ItemName");
			t1.Add("ItemName1");
			t1.Add("ItemName2");
			Assert.AreEqual(3, t1.Count, "Count");
			
			t1.Clear();
			Assert.AreEqual(0, t1.Count, "Count1");

			t1.Add("ItemName");
			t1.Add("ItemName1");
			t1.Add("ItemName2");
			t1.RemoveAt(1);
			Assert.AreEqual(2, t1.Count, "Count2");
			Assert.AreEqual("ItemName", t1[0].ItemName, "ItemName");
			Assert.AreEqual("ItemName2", t1[1].ItemName, "ItemName2");

			t1.Remove(t1[1]);
			Assert.AreEqual(1, t1.Count, "Count3");
			Assert.AreEqual("ItemName", t1[0].ItemName, "ItemName(1)");
		}

		[TestMethod]
		public void Find()
		{
			TestCollection t1 = new TestCollection(null);
			t1.Add("ItemName1");
			t1.Add("ItemName2");

			int index = t1.Find("ItemName1");
			Assert.AreEqual(0, index, "index (0)");
			index = t1.Find("ItemName2");
			Assert.AreEqual(1, index, "index (1)");
			index = t1.Find("NotThere");
			Assert.AreEqual(-1, index, "NotThere");
		}

		[TestMethod]
		public void Parent()
		{
			TestCollection t1 = new TestCollection(null);
			t1.Add("TopParent");
			TestCollection t2 = new TestCollection(t1[0]);
			t2.Add("MiddleObject");
			TestCollection t3 = new TestCollection(t2[0]);
			t3.Add("BottomObject");

			Assert.AreSame(t2[0], t3[0].Parent, "MiddleObject");
			Assert.AreSame(t1[0], t2[0].Parent, "TopParent");
			Assert.AreSame(t1[0], t3[0].GetTopParent(), "T3 TopParent");
			Assert.AreSame(t1[0], t2[0].GetTopParent(), "T2 TopParent");
			Assert.AreSame(t1[0], t1[0].GetTopParent(), "T1 TopParent");
		}

		[TestMethod]
		public void Level()
		{
			TestCollection t1 = new TestCollection(null);
			t1.Add("Item1");
			t1[0].ChildCollection.Add("Item2");
			t1[0].ChildCollection[0].ChildCollection.Add("Item3");
			Assert.AreEqual(1, t1[0].ChildCollection.Level, "Level1");
			Assert.AreEqual(2, t1[0].ChildCollection[0].ChildCollection.Level, "Level2");
			Assert.AreEqual(3, t1[0].ChildCollection[0].ChildCollection[0].ChildCollection.Level, "Level3");
		}

		[TestMethod]
		public void VerifyName()
		{
			ArrayList al = new ArrayList();
			al.Add(new TestCollection(null));
			m_Project.AlwaysReturn("GetTopLevelCollections", al, null);
			TestCollection t1 = new TestCollection(m_Project.Object);
			Assert.AreEqual(0, t1.VerifyName("abc_123"));
			Assert.AreEqual(0, t1.VerifyName("_abc_123"));
		}

		[TestMethod]
		public void VerifyNameDupSameCollection()
		{
			ArrayList al = new ArrayList();
			al.Add(new TestCollection(null));
			m_Project.AlwaysReturn("GetTopLevelCollections", al, null);
			TestCollection t1 = new TestCollection(m_Project.Object);
			al.Add(t1);
			t1.Add("abc_123");
			Assert.AreEqual(-2, t1.VerifyName("abc_123"));
		}

		[TestMethod]
		public void VerifyNameDupDifferentCollection()
		{
			ArrayList al = new ArrayList();
			TestCollection3 t2 = new TestCollection3(null);
			al.Add(t2);
			m_Project.AlwaysReturn("GetTopLevelCollections", al, null);
			TestCollection3 t1 = new TestCollection3(m_Project.Object);
			al.Add(t1);
			t2.Add("abc_123");
			Assert.AreEqual(-2, t1.VerifyName("abc_123"));
		}

		[TestMethod]
		public void VerifyNameInvalidChars()
		{
			TestCollection t1 = new TestCollection(null);
			Assert.AreEqual(-1, t1.VerifyName("91abc"));
			Assert.AreEqual(-1, t1.VerifyName(""));
			Assert.AreEqual(-1, t1.VerifyName("twenty one"));
			Assert.AreEqual(-1, t1.VerifyName(" space"));
			Assert.AreEqual(-1, t1.VerifyName("space "));
			Assert.AreEqual(-1, t1.VerifyName("dash-dash"));
		}

		[TestMethod]
		public void GetNextDefault()
		{
			ArrayList al = new ArrayList();
			m_Project.AlwaysReturn("GetTopLevelCollections", al, null);
			TestCollection t1 = new TestCollection(m_Project.Object);
			al.Add(t1);
			Assert.AreEqual("TextBox1", t1.GetNextDefaultName("TextBox"));
			t1.Add("TextBox1");
			Assert.AreEqual("TextBox2", t1.GetNextDefaultName("TextBox"));
		}

		[TestMethod]
		public void RepairNameCollision()
		{
			ArrayList al = new ArrayList();
			TestCollection t2 = new TestCollection(null);
			al.Add(t2);
			m_Project.AlwaysReturn("GetTopLevelCollections", al, null);
			TestCollection t1 = new TestCollection(m_Project.Object);
			al.Add(t1);
			t2.Add("TextBox23");
			t1.Add("TextBox1");
			Assert.AreEqual("TextBox1_1", t1.RepairNameCollsion("TextBox1"));
			t1.Add("TextBox1_1");
			Assert.AreEqual("TextBox1_2", t1.RepairNameCollsion("TextBox1"));
			Assert.AreEqual("TextBox23_1", t1.RepairNameCollsion("TextBox23"));
		}

		[TestMethod]
		public void KeyOrderListTest()
		{
			IWDEProject proj = WDEProject.Create();
			proj.DocumentDefs.Add();
			proj.DocumentDefs[0].FormDefs.Add();
			IWDEControlDefs defs = proj.DocumentDefs[0].FormDefs[0].ControlDefs;
			KeyOrderList list = new KeyOrderList();
			for(int i = 0; i < 3; i++)
			{
				int o = 3 - i;
				IWDETextBoxDef def = defs.AddTextBox("TextBox" + o.ToString());
				def.KeyOrder = 0; // adds them as 3, 2, 1
				list.Add(def, def.KeyOrder);
			}
			list.ReOrder(defs);
			Assert.AreEqual(2, defs[0].KeyOrder);
			Assert.AreEqual(1, defs[1].KeyOrder);
			Assert.AreEqual(0, defs[2].KeyOrder);
		}

		[TestMethod]
		public void KeyOrderListSubs()
		{
			IWDEProject proj = WDEProject.Create();
			proj.DocumentDefs.Add();
			proj.DocumentDefs[0].FormDefs.Add();
			IWDEControlDefs defs = proj.DocumentDefs[0].FormDefs[0].ControlDefs;
			KeyOrderList list = new KeyOrderList();
			
			AddTextBox(defs, 3, list, "TextBox");
			AddTextBox(defs, 2, list, "TextBox");

			KeyOrderList sub1 = new KeyOrderList();
			list.Add(sub1, 4);
			AddTextBox(defs, 0, sub1, "sub1_");
			AddTextBox(defs, 1, sub1, "sub1_");
			AddTextBox(defs, 2, sub1, "sub1_");

			AddTextBox(defs, 0, list, "TextBox");
			AddTextBox(defs, 5, list, "TextBox");
			
			KeyOrderList sub2 = new KeyOrderList();
			list.Add(sub2, 6);
			AddTextBox(defs, 0, sub2, "sub2_");
			AddTextBox(defs, 1, sub2, "sub2_");
			AddTextBox(defs, 2, sub2, "sub2_");

			list.ReOrder(defs);

			Assert.AreEqual(2, defs[0].KeyOrder, "KeyOrder0");
			Assert.AreEqual(1, defs[1].KeyOrder, "KeyOrder1");
			Assert.AreEqual(3, defs[2].KeyOrder, "KeyOrder2");
			Assert.AreEqual(4, defs[3].KeyOrder, "KeyOrder3");
			Assert.AreEqual(5, defs[4].KeyOrder, "KeyOrder4");
			Assert.AreEqual(0, defs[5].KeyOrder, "KeyOrder5");
			Assert.AreEqual(6, defs[6].KeyOrder, "KeyOrder6");
			Assert.AreEqual(7, defs[7].KeyOrder, "KeyOrder7");
			Assert.AreEqual(8, defs[8].KeyOrder, "KeyOrder8");
			Assert.AreEqual(9, defs[9].KeyOrder, "KeyOrder9");
		}

		[TestMethod]
		public void KeyOrderListSubSub()
		{
			IWDEProject proj = WDEProject.Create();
			proj.DocumentDefs.Add();
			proj.DocumentDefs[0].FormDefs.Add();
			IWDEControlDefs defs = proj.DocumentDefs[0].FormDefs[0].ControlDefs;
			KeyOrderList list = new KeyOrderList();
			
			AddTextBox(defs, 3, list, "TextBox");
			AddTextBox(defs, 2, list, "TextBox");

			KeyOrderList sub1 = new KeyOrderList();
			list.Add(sub1, 4);
			AddTextBox(defs, 0, sub1, "sub1_");
			KeyOrderList sub2 = new KeyOrderList();
			sub1.Add(sub2, 1);
			AddTextBox(defs, 0, sub2, "sub2_");
			AddTextBox(defs, 1, sub2, "sub2_");
			AddTextBox(defs, 1, list, "TextBox");
			AddTextBox(defs, 0, list, "TextBox");
			AddTextBox(defs, 5, list, "TextBox");

			list.ReOrder(defs);

			Assert.AreEqual(3, defs[0].KeyOrder, "KeyOrder0");
			Assert.AreEqual(2, defs[1].KeyOrder, "KeyOrder1");
			Assert.AreEqual(4, defs[2].KeyOrder, "KeyOrder2");
			Assert.AreEqual(5, defs[3].KeyOrder, "KeyOrder3");
			Assert.AreEqual(6, defs[4].KeyOrder, "KeyOrder4");
			Assert.AreEqual(1, defs[5].KeyOrder, "KeyOrder5");
			Assert.AreEqual(0, defs[6].KeyOrder, "KeyOrder6");
			Assert.AreEqual(7, defs[7].KeyOrder, "KeyOrder7");
		}

		[TestMethod]
		public void GetParentInterface()
		{
			TestItem item = new TestItem();
			item.ItemName = "ItemName1";
			item.ChildCollection.Add("SubItem1");
			TestInterface ti = (TestInterface) item.ChildCollection[0].GetParentInterface("TestInterface");
			Assert.IsNotNull(ti, "ti is null. Expected not null.");
			Assert.AreEqual("ItemName1", ti.ItemName, "ItemName");

			TestInterface ti2 = (TestInterface) item.ChildCollection.GetParentInterface("TestInterface");
			Assert.AreSame(ti, ti2, "ti and ti2 are not the same. Expected same.");
		}

		private IWDETextBoxDef AddTextBox(IWDEControlDefs collection, int keyOrder, KeyOrderList list, string rootName)
		{
			IWDETextBoxDef def = collection.AddTextBox(rootName + keyOrder.ToString());
			list.Add(def, keyOrder);
			return def;
		}
	}

	public class TestCollection : WDEBaseCollection
	{
		public TestCollection(object Parent) : base(Parent)
		{
		}

		protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			TestItem item = (TestItem) Item;
			return (item.ItemName == Name);
		}

		public void Add(string ItemName)
		{
			TestItem newItem = new TestItem();
			newItem.ItemName = ItemName;
			this.InternalAdd(newItem);
		}

		public TestItem this[int index]
		{
			get {return (TestItem) this.InternalGetIndex(index);}
			set {this.InternalSetIndex(index, (WDEBaseCollectionItem) value);}
		}

		public int Find(string ItemName)
		{
			return this.InternalIndexOf(ItemName);
		}
	}

	public interface TestInterface
	{
		string ItemName {get; set;}
	}

	public class TestItem : WDEBaseCollectionItem, TestInterface
	{
		private string m_ItemName;
		private TestItem2 m_TestItem2;
		private TestCollection m_TestCollection;

		public TestItem()
		{
			m_ItemName = "";
			m_TestItem2 = null;
			m_TestCollection = new TestCollection(this);
		}

		protected override void InternalLinkNotify(WDEBaseCollectionItem linkedObject)
		{
			if(linkedObject == null)
			{
				if(m_TestItem2 != null)
				{
					m_TestItem2.RemoveLink(this);
					m_TestItem2 = null;
				}
			}
			else if (linkedObject == m_TestItem2)
			{
				m_TestItem2.RemoveLink(this);
				m_TestItem2 = null;
			}
		}

		public string ItemName
		{
			get {return m_ItemName;}
			set {m_ItemName = value;}
		}

		public TestItem2 Test2
		{
			get {return m_TestItem2;}
			set 
			{
				if(m_TestItem2 != null)
					m_TestItem2.RemoveLink(this);

				m_TestItem2 = value;

				if(m_TestItem2 != null)
					m_TestItem2.AddLink(this);
			}
		}

		protected override string InternalGetNodeName()
		{
			return m_ItemName;
		}


		public object GetTopParent()
		{
			return this.TopParent();
		}

		public TestCollection ChildCollection
		{
			get
			{
				return m_TestCollection;
			}
		}
	}

	public class TestCollection2 : WDEBaseCollection
	{
		public TestCollection2(object Parent) : base(Parent)
		{
		}

		protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			return false;
		}

		public void Add(string ItemName)
		{
			TestItem2 newItem = new TestItem2();
			newItem.ItemName = ItemName;
			this.InternalAdd(newItem);
		}

		public TestItem2 this[int index]
		{
			get {return (TestItem2) this.InternalGetIndex(index);}
			set {this.InternalSetIndex(index, (WDEBaseCollectionItem) value);}
		}
	}

	public class TestItem2 : WDEBaseCollectionItem
	{
		private string m_ItemName;
		public TestItem2()
		{
			m_ItemName = "";
		}

		public string ItemName
		{
			get {return m_ItemName;}
			set {m_ItemName = value;}
		}

		protected override string InternalGetNodeName()
		{
			return null;
		}

	}	

	public class TestCollection3 : WDEBaseCollection
	{
		public TestCollection3(object Parent) : base(Parent)
		{
		}

		protected override bool MatchesName(WDEBaseCollectionItem Item, string Name)
		{
			return false;
		}

		protected override ArrayList GetCollectionList()
		{
			return base.GetSameLevelCollections();
		}

		public void Add(string ItemName)
		{
			TestItem3 newItem = new TestItem3();
			newItem.ItemName = ItemName;
			this.InternalAdd(newItem);
		}

		public TestItem3 this[int index]
		{
			get {return (TestItem3) this.InternalGetIndex(index);}
			set {this.InternalSetIndex(index, (WDEBaseCollectionItem) value);}
		}
	}

	public class TestItem3 : WDEBaseCollectionItem
	{
		private string m_ItemName;
		public TestItem3()
		{
			m_ItemName = "";
		}

		public string ItemName
		{
			get {return m_ItemName;}
			set {m_ItemName = value;}
		}

		protected override string InternalGetNodeName()
		{
			return m_ItemName;
		}
	}


	public class TestLinkList : WDEBaseCollection
	{
		public TestLinkList(object Parent) : base(Parent)
		{
		}

		protected override void InternalLinkNotify(WDEBaseCollectionItem LinkedItem)
		{
			base.Remove(LinkedItem);
		}


		public TestItem this[int index]
		{
			get
			{
				return (TestItem) base.InternalGetIndex(index);
			}
			set
			{
				while(index <= Count)
					base.InternalAdd(null);

				TestItem obj = (TestItem) base.InternalGetIndex(index);
				if(obj != null)
					obj.RemoveLink(this);

				obj = value;
				base.InternalSetIndex(index, (WDEBaseCollectionItem) value);
				obj.AddLink(this);
			}
		}

		public void Add(TestItem LinkedItem)
		{
			LinkedItem.AddLink(this);
			base.InternalAdd((WDEBaseCollectionItem) LinkedItem);
		}
	}
}
