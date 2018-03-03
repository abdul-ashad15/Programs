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
	public class SimpleCodeParserTests
	{
		public SimpleCodeParserTests()
		{
		}

		[TestCleanup]
		public void TearDown()
		{
			GC.Collect();
		}

		[TestMethod]
		public void BasicDelphi()
		{
			SimpleDelphiParser p = new SimpleDelphiParser();
			p.ScriptText = "\r\n(*borland\r\nmulti\r\nline\r\ncomment function*)\r\n\r\nfunction\r\nTestDelphi(Input: string): string;\r\nvar\r\n\tx: Integer;\r\nbegin\r\nx := 25;\r\nend;";
			p.ReplaceMethodDecl("Replaced");
			Assert.AreEqual("\r\n(*borland\r\nmulti\r\nline\r\ncomment function*)\r\n\r\nfunction\r\nReplaced(Input: string): string;\r\nvar\r\n\tx: Integer;\r\nbegin\r\nx := 25;\r\nend;",
				p.ScriptText);
		}

		[TestMethod]
		public void BasicVB()
		{
			SimpleVBParser p = new SimpleVBParser();
			p.ScriptText = "\r\nrem comment function\r\n\r\nfunction\r\nTestVB(Input: string): string\r\nvar\r\n\tx: Integer\r\nbegin\r\nx := 25\r\nend";
			p.ReplaceMethodDecl("Replaced");
			Assert.AreEqual("\r\nrem comment function\r\n\r\nfunction\r\nReplaced(Input: string): string\r\nvar\r\n\tx: Integer\r\nbegin\r\nx := 25\r\nend",
				p.ScriptText);
		}

		[TestMethod]
		public void BasicJScript()
		{
			SimpleJScriptParser p = new SimpleJScriptParser();
			p.ScriptText = "\r\n/*jscript\r\nmulti\r\nline\r\ncomment function*/\r\n\r\nfunction\r\nTestJScript(Input: string): string;\r\nvar\r\n\tx: Integer;\r\nbegin\r\nx = 25;\r\nend;";
			p.ReplaceMethodDecl("Replaced");
			Assert.AreEqual("\r\n/*jscript\r\nmulti\r\nline\r\ncomment function*/\r\n\r\nfunction\r\nReplaced(Input: string): string;\r\nvar\r\n\tx: Integer;\r\nbegin\r\nx = 25;\r\nend;",
				p.ScriptText);
		}
	}
}
