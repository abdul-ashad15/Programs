using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Xml;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using WebDX.Api.Scripts;
//using NUnit.Framework;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mscoree;

namespace WebDX.Api.Tests
{
	[TestClass]
	public class ScriptEngineTests
	{
		/*ResourceManager m_ResMan;
		ScriptEngine m_Engine;
		IWDEDataSet m_DataSet;
		
		public ScriptEngineTests()
		{
		}

		[ClassInitialize]
		public void Setup()
		{
			m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());
		}

		[TestInitialize]
		public void Init()
		{
			m_Engine = new ScriptEngine();
			File.Copy(@"..\..\..\TestScriptOne\bin\Debug\TestScriptOne.dll", "TestScriptOne.dll", true);
			m_DataSet = WDEDataSet.Create();

			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
			sw.Write(m_ResMan.GetString("DataSetFixSessNoGaps"));
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			m_DataSet.LoadFromStream("User1", "ScriptTest", WDEOpenMode.Verify, "SomeLocation", ms);
			sw.Close();
		}

		[TestCleanup]
		public void Teardown()
		{
			m_Engine.UnloadAll();
			File.Delete("TestScriptOne.dll");
			m_Engine = null;
			m_DataSet = null;
			GC.Collect();
		}

		[TestMethod]
		public void LoadAssembly()
		{
			m_Engine.LoadScriptAssembly("TestScriptOne.dll");
			string domains = GetLoadedDomains();
			Assert.IsTrue(domains.IndexOf("TestScriptOne.dll") > -1, "TestScriptOne.dll domain not loaded");
			m_Engine.UnloadAll();
			domains = GetLoadedDomains();
			Assert.AreEqual(-1, domains.IndexOf("TestScriptOne.dll"), "Unloaded");
		}

		[TestMethod]
		public void UnloadAssembly()
		{
			m_Engine.LoadScriptAssembly("TestScriptOne.dll");
			string domains = GetLoadedDomains();
			Assert.IsTrue(domains.IndexOf("TestScriptOne.dll") > -1, "TestScriptOne.dll domain not loaded");
			m_Engine.UnloadScriptAssembly("TestScriptOne.dll");
			domains = GetLoadedDomains();
			Assert.AreEqual(-1, domains.IndexOf("TestScriptOne.dll"), "Unloaded");
		}

		[TestMethod]
		public void EvalExpression()
		{
			m_Engine.LoadScriptAssembly("TestScriptOne.dll");
			Assert.IsTrue(m_Engine.EvaluateExpression("TestScriptOne.TestExpression1", (IWDEScriptField) m_DataSet.Documents[0].Records[0].Fields[0]), "TestExpression1 evaluated false. Expected true.");
		}

		[TestMethod]
		public void Reset()
		{
			m_Engine.LoadScriptAssembly("TestScriptOne.dll");
			m_Engine.Reset();
		}

		[TestMethod]
		public void LoadTwice()
		{
			m_Engine.LoadScriptAssembly("TestScriptOne.dll");
			m_Engine.LoadScriptAssembly("TestScriptOne.dll");
			string domains = GetLoadedDomains();
			int count = 0;
			foreach(string domain in domains.Split(Environment.NewLine.ToCharArray()))
				if(domain == "TestScriptOne.dll")
					count++;

			Assert.AreEqual(1, count);
		}

		[TestMethod]
		public void LoadFrom()
		{
			File.Delete("TestScriptOne.dll");
			m_Engine.LoadScriptAssembly(@"..\..\..\TestScriptOne\bin\Debug\TestScriptOne.dll");
			Assert.IsTrue(m_Engine.EvaluateExpression("TestScriptOne.TestExpression1", (IWDEScriptField) m_DataSet.Documents[0].Records[0].Fields[0]), "TestExpression1 evaluated false. Expected true.");
		}

		[TestMethod]
		[ExpectedException(typeof(System.IO.FileNotFoundException))]
		public void LoadNoFile()
		{
			m_Engine.LoadScriptAssembly("NotThere.dll");
		}

		[TestMethod]
		public void ClearInterfaces()
		{
			CodeCompileUnit res = GetTestExpression(true);
			Microsoft.CSharp.CSharpCodeProvider cp = new Microsoft.CSharp.CSharpCodeProvider();

			CompilerParameters options = new CompilerParameters();
			options.GenerateInMemory = true;
			options.ReferencedAssemblies.Add("System.dll");
			options.ReferencedAssemblies.Add("WebDX.Api.dll");
			CompilerResults cres = cp.CompileAssemblyFromDom(options, res);

			m_Engine.LoadInterfaces(cres.CompiledAssembly, "ApiTester");
			Assert.IsTrue(m_Engine.EvaluateExpression("ApiTester.Expr1", (IWDEScriptField) m_DataSet.Documents[0].Records[0].Fields[0]), "First attempt is false. Expected true.");

			res = GetTestExpression(false);
			cres = cp.CompileAssemblyFromDom(options, res);
			m_Engine.ClearInterfaces("ApiTester");
			m_Engine.LoadInterfaces(cres.CompiledAssembly, "ApiTester");

			Assert.IsFalse(m_Engine.EvaluateExpression("ApiTester.Expr1", (IWDEScriptField) m_DataSet.Documents[0].Records[0].Fields[0]), "Second attempt is true. Expected false.");	

			// non explicit
			res = GetTestExpression(true);
			cres = cp.CompileAssemblyFromDom(options, res);
			m_Engine.LoadInterfaces(cres.CompiledAssembly, "ApiTester");

			Assert.IsTrue(m_Engine.EvaluateExpression("ApiTester.Expr1", (IWDEScriptField) m_DataSet.Documents[0].Records[0].Fields[0]), "Third attempt is false. Expected true.");	
		}

		private string GetLoadedDomains()
		{
			StringBuilder sb = new StringBuilder();
			CorRuntimeHostClass host = new CorRuntimeHostClass();
			try
			{
				IntPtr enumHandle;
				host.EnumDomains(out enumHandle);
				while (true)
				{
					object domain = null;
					host.NextDomain(enumHandle, out domain);
					if(domain == null)
						break;
					sb.Append(((AppDomain) domain).FriendlyName + Environment.NewLine);
				}
				return sb.ToString();
			}
			finally
			{
				Marshal.ReleaseComObject(host);
			}
		}

		private CodeCompileUnit GetTestExpression(bool result)
		{
			CodeCompileUnit res = new CodeCompileUnit();
			CodeNamespace ns = new CodeNamespace("ApiTester");
			ns.Imports.Add(new CodeNamespaceImport("System"));
			ns.Imports.Add(new CodeNamespaceImport("WebDX.Api.Scripts"));
			res.Namespaces.Add(ns);
			res.ReferencedAssemblies.Add("System.dll");
			res.ReferencedAssemblies.Add("WebDX.Api.dll");

			CodeTypeDeclaration class1 = new CodeTypeDeclaration("Expr1");
			class1.BaseTypes.Add(typeof(MarshalByRefObject));
			class1.BaseTypes.Add(typeof(IWDEExpression));
			CodeConstructor const1 = new CodeConstructor();
			const1.Name = "Expr1";
			const1.Attributes = MemberAttributes.Public;
			class1.Members.Add(const1);
			
			CodeMemberMethod calc1 = new CodeMemberMethod();
			calc1.Name = "Calculate";
			calc1.ReturnType = new CodeTypeReference("System.Boolean");
			calc1.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			calc1.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(result)));
			class1.Members.Add(calc1);
			
			ns.Types.Add(class1);

			return res;
		}*/
	}
}
