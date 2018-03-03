using System;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Xml;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;

using WebDX.Api.Scripts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebDX.Api.Tests
{
	[TestClass]
	public class LinkResolverTests
	{
		private LinkResolver m_LinkResolver;

		public LinkResolverTests()
		{
		}

		[TestInitialize]
		public void Init()
		{
			m_LinkResolver = new LinkResolver();
		}

		[TestCleanup]
		public void TearDown()
		{
			m_LinkResolver = null;
			GC.Collect();
		}

		[TestMethod]
		public void ResolveLink()
		{
			TestInt1 t = new Test1() as TestInt1;
			Req1 r = new Req1();
			
			m_LinkResolver.AddObject("Test1", t);
			m_LinkResolver.AddObject("Req1", r);
			m_LinkResolver.AddRequest(r, "TestInt", "Test1");
			m_LinkResolver.ResolveLinks();
			Assert.AreSame(t, r.TestInt, "t and TestInt are not the same. Expected same.");
		}

		[TestMethod]
		public void ResolveIndexed()
		{
			TestInt1 t = new Test1() as TestInt1;
			TestInt1 t1 = new Test1() as TestInt1;
			TestInt1 t2 = new Test1() as TestInt1;
			Req1 r = new Req1();
			
			m_LinkResolver.AddObject("Test1", t);
			m_LinkResolver.AddObject("Test2", t1);
			m_LinkResolver.AddObject("Test3", t2);
			m_LinkResolver.AddObject("Req1", r);
			m_LinkResolver.AddRequest(r, "Item", "Test1", 0);
			m_LinkResolver.AddRequest(r, "Item", "Test2", 1);
			m_LinkResolver.AddRequest(r, "Item", "Test3", 2);
			m_LinkResolver.ResolveLinks();
			Assert.AreSame(t, r[0], "Item[0] and TestInt are not the same. Expected same.");
			Assert.AreSame(t1, r[1], "Item[1] and TestInt are not the same. Expected same.");
			Assert.AreSame(t2, r[2], "Item[2] and TestInt are not the same. Expected same.");
		}

		[TestMethod]
		[ExpectedException(typeof(WDEException))]
		public void ResolveBad()
		{
			TestInt1 t = new Test1() as TestInt1;
			Req1 r = new Req1();
			
			m_LinkResolver.AddObject("Test1", t);
			m_LinkResolver.AddObject("Req1", r);
			m_LinkResolver.AddRequest(r, "TestInt", "Test2");
			m_LinkResolver.ResolveLinks();
		}

		[TestMethod]
		public void DupeTest()
		{
			IWDEFieldDef def = WDEFieldDef.Create();
			IWDEBalanceCheckEditDef balDef = WDEBalanceCheckEditDef.Create();
			balDef.SumFields.Add(def);
			balDef.Description = "DESC";
			balDef.Enabled = true;
			balDef.ErrorMargin = 1.11;
			balDef.ErrorMessage = "ErrorMessage";
			balDef.ErrorType = WDEEditErrorType.Warning;
			balDef.SessionMode = WDESessionType.FullForm;
			
			IWDEEditDefs defs = WDEEditDefs.Create(null);

			LinkResolver lr = new LinkResolver();
			lr.AddObject("balDef", balDef);

			Hashtable ht = new Hashtable();
			ht.Add("ErrorType", WDEEditErrorType.Ignore);

			lr.AddDupeRequest(defs, "balDef", ht, "Add");
			lr.ResolveLinks();

			Assert.AreEqual(1, defs.Count, "Count");
			Assert.AreNotSame(balDef, defs[0], "balDef and defs[0] are the same. Expected not same.");
			IWDEBalanceCheckEditDef cloneDef = (IWDEBalanceCheckEditDef) defs[0];
			Assert.AreEqual(balDef.SumFields.Count, cloneDef.SumFields.Count, "balDef.Count");
			Assert.AreSame(balDef.SumFields[0], cloneDef.SumFields[0], "balDef.SumFields[0] and cloneDef.SumFields[0] are not the same. Expected same.");
			Assert.AreEqual(balDef.Description, cloneDef.Description, "Description");
			Assert.AreEqual(balDef.Enabled, cloneDef.Enabled, "Enabled");
			Assert.AreEqual(balDef.ErrorMargin, cloneDef.ErrorMargin, "ErrorMargin");
			Assert.AreEqual(balDef.ErrorMessage, cloneDef.ErrorMessage, "ErrorMessage");
			Assert.AreEqual(WDEEditErrorType.Ignore, cloneDef.ErrorType, "ErrorType");
			Assert.AreEqual(balDef.SessionMode, cloneDef.SessionMode, "SessionMode");
		}

		[TestMethod]
		public void NameConversion()
		{
			TestInt1 t = new Test1() as TestInt1;
			Req1 r = new Req1();
			
			m_LinkResolver.AddObject("Test2.Image2.Zone1", t);
			m_LinkResolver.AddObject("Req1", r);
			m_LinkResolver.AddRequest(r, "TestInt", "Test2.Form1.ImageLink2.Zone1", -1, "Test2.Form1.ImageLink2");
			m_LinkResolver.AddConvertName("Test2.Form1.ImageLink2", "Test2.Image2");
			m_LinkResolver.ResolveLinks();
			Assert.AreSame(t, r.TestInt, "t and r.TestInt are not the same. Expected same.");

			m_LinkResolver.AddObject("Test2.Image2.Zone1", t);
			m_LinkResolver.AddObject("Req1", r);
			m_LinkResolver.AddConvertName("Test2.Form1.ImageLink2", "Test2.Image2");
			m_LinkResolver.AddRequest(r, "TestInt", "Test2.Form1.ImageLink2.Zone1", -1, "Test2.Form1.ImageLink2");
			m_LinkResolver.ResolveLinks();
			Assert.AreSame(t, r.TestInt, "2 - t and r.TestInt are not the same. Expected same.");
		}
	}

	public interface TestInt1
	{
		string Value {get;}
	}

	public class Req1
	{
		private TestInt1 m_TestInt;
		private ArrayList m_TestIndex;

		public Req1()
		{
			m_TestIndex = new ArrayList();
		}

		public TestInt1 TestInt
		{
			get{ return m_TestInt; }
			set{ m_TestInt = value; }
		}

		public TestInt1 this[int index]
		{
			get
			{
				return (TestInt1) m_TestIndex[index];
			}

			set
			{
				while(index >= m_TestIndex.Count)
					m_TestIndex.Add(null);
				m_TestIndex[index] = value;
			}
		}
	}

	public class Test1 : TestInt1
	{
		public Test1()
		{
		}

		#region TestInt1 Members

		public string Value
		{
			get
			{
				return "TESTDATA";
			}
		}

		#endregion
	}

}
