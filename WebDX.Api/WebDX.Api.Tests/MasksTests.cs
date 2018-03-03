using System;
using System.Text;

using WebDX.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebDX.Api.Tests
{
	/// <summary>
	/// Tests for Masks objects.
	/// </summary>
	[TestClass]
	public class MasksTests
	{
		public MasksTests()
		{
		}

		[TestCleanup]
		public void TearDown()
		{
			GC.Collect();
		}

		[TestMethod]
		public void CharSetBasic()
		{
			WDECharSet cs = new WDECharSet("A,B,C,D");
			Assert.IsTrue(cs.IsValidChar('A'), "'A' is not valid. Expected valid.");
			Assert.IsTrue(cs.IsValidChar('B'), "'B' is not valid. Expected valid.");
			Assert.IsTrue(cs.IsValidChar('C'), "'C' is not valid. Expected valid.");
			Assert.IsTrue(cs.IsValidChar('D'), "'D' is not valid. Expected valid.");
			Assert.IsFalse(cs.IsValidChar('E'), "'E' is valid. Expected not valid.");
		}

		[TestMethod]
		public void CharSetRange()
		{
			WDECharSet cs = new WDECharSet("A-Z");
			string test = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			foreach(char testChar in test.ToCharArray())
			{
				Assert.IsTrue(cs.IsValidChar(testChar), testChar.ToString() + " is invalid. Expected valid.");
			}
			Assert.IsFalse(cs.IsValidChar('9'), "'9' is valid. Expected invalid.");
		}

		[TestMethod]
		public void CharSetCombo()
		{
			WDECharSet cs = new WDECharSet("A-Z,1,3,5-9");
			string test = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1356789";
			foreach(char testChar in test.ToCharArray())
			{
				Assert.IsTrue(cs.IsValidChar(testChar), testChar.ToString() + " is invalid. Expected valid.");
			}
			Assert.IsFalse(cs.IsValidChar('2'), "'2' is valid. Expected invalid.");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CharSetNullInit()
		{
			WDECharSet cs = new WDECharSet(null);
		}

		[TestMethod]
		public void CharSetProp()
		{
			WDECharSet cs = new WDECharSet("A-Z");
			Assert.AreEqual("A-Z", cs.CharSet, "CharSet = {0}, Expected \"A-Z\"", new object[] {cs.CharSet});
			cs.CharSet = "A,B,C";
			Assert.AreEqual("A,B,C", cs.CharSet, "Charset = {0}, Expected \"ABC\"", new object[] {cs.CharSet});
		}

		[TestMethod]
		public void CharSetFilter()
		{
			WDECharSet cs = new WDECharSet("A,B,D");
			Assert.AreEqual("ABD",cs.Filter("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"));
		}

		[TestMethod]
		public void CharSetValidString()
		{
			WDECharSet cs = new WDECharSet("A,B,C,D");
			Assert.IsTrue(cs.IsValidString("ABC"), "'ABC' is not valid. Expected valid.");
			Assert.IsFalse(cs.IsValidString("ABCDE"), "'ABCDE' is valid. Expected not valid.");
			Assert.IsFalse(cs.IsValidString("ABA2E"), "'ABA2E' is valid. Expected not valid.");
			Assert.IsTrue(cs.IsValidString("ABCDEFG", 4), "'ABCDEFG' length 4 is invalid. Expected valid.");
			Assert.IsTrue(cs.IsValidString("ABCD", 6), "'ABCD' length 6 is invalid. Expected valid.");
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException))]
		public void CharSetInvalidRangeOrder()
		{
			WDECharSet cs = new WDECharSet("Z-A");
		}

		[TestMethod]
		public void CharSetSameRange()
		{
			WDECharSet cs = new WDECharSet("A-A");
			Assert.IsTrue(cs.IsValidChar('A'), "'A' is invalid. Expected valid.");
		}

		[TestMethod]
		public void CharSetCommaTests()
		{
			WDECharSet cs = new WDECharSet("A,");
			Assert.IsTrue(cs.IsValidChar('A'), "'A' is invalid. Expected valid.");
			cs.CharSet = ",A,";
			cs.CharSet = ",,";
			Assert.IsTrue(cs.IsValidChar(','), "',' is invalid. Expected valid.");
			cs.CharSet = ",";
			Assert.IsTrue(cs.IsValidChar(','), "',' (single) is invalid. Expected valid.");
		}

		[TestMethod]
		public void CharSetDashTests()
		{
			WDECharSet cs = new WDECharSet("A,B,--,C");
			Assert.IsTrue(cs.IsValidChar('-'), "'-' is invalid. Expected valid.");
			cs.CharSet = "-";
			Assert.IsTrue(cs.IsValidChar('-'), "'-' is invalid. Expected valid.");
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException), "Unterminated range in CharSet")]
		public void CharSetUnterminatedRange()
		{
			WDECharSet cs = new WDECharSet("A-");
		}

		[TestMethod]
		[ExpectedException(typeof(FormatException), "Range without begin value in CharSet")]
		public void CharSetRangeWithoutBegin()
		{
			WDECharSet cs = new WDECharSet("-Z");
		}

		[TestMethod]
		public void CharSetEmpty()
		{
			WDECharSet cs = new WDECharSet("");
			StringBuilder sb = new StringBuilder();
			for(char c = char.MinValue; c < char.MaxValue; c++)
			{
				sb.Append(c);
			}
			sb.Append(char.MaxValue);
			Assert.IsTrue(cs.IsValidString(sb.ToString()));
		}
	}
}
