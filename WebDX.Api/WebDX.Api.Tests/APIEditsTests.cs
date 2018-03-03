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
using NUnit.Framework;
using TypeMock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebDX.Api.Tests
{
	[TestClass]
	public class APIEditsTests
	{
		ResourceManager m_ResMan;

		private MockObject m_CSDef;
		private MockObject m_RADef;
		private MockObject m_VLDef;
		private MockObject m_SLDef;
		private MockObject m_REDef;

		public APIEditsTests()
		{
		}

		[SetUp]
		public void Init()
		{
			m_ResMan = new ResourceManager("WebDX.Api.Tests.DataExpectedResults", Assembly.GetExecutingAssembly());
			MockManager.Init();
			m_CSDef = MockManager.MockObject(typeof(IWDECheckDigitEditDef));
			m_CSDef.ExpectGetAlways("Enabled", true);
			m_CSDef.ExpectGetAlways("ErrorType", WDEEditErrorType.Ignore);
			
			m_RADef = MockManager.MockObject(typeof(IWDERangeEditDef));
			m_RADef.ExpectGetAlways("DisplayName", "RangeEdit");
			m_RADef.ExpectGetAlways("Enabled", true);
			m_RADef.ExpectGetAlways("ErrorType", WDEEditErrorType.Failure);

			m_VLDef = MockManager.MockObject(typeof(IWDEValidLengthsEditDef));
			m_VLDef.ExpectGetAlways("DisplayName", "ValidLengthsEdit");
			m_VLDef.ExpectGetAlways("Enabled", true);
			m_VLDef.ExpectGetAlways("ErrorType", WDEEditErrorType.Failure);

			m_SLDef = MockManager.MockObject(typeof(IWDESimpleListEditDef));
			m_SLDef.ExpectGetAlways("DisplayName", "SimpleListEdit");
			m_SLDef.ExpectGetAlways("Enabled", true);
			m_SLDef.ExpectGetAlways("ErrorType", WDEEditErrorType.Failure);

			m_REDef = MockManager.MockObject(typeof(IWDERequiredEditDef));
			m_REDef.ExpectGetAlways("DisplayName", "RequiredEdit");
			m_REDef.ExpectGetAlways("Enabled", true);
			m_REDef.ExpectGetAlways("ErrorType", WDEEditErrorType.Failure);
			m_REDef.ExpectGetAlways("Expression", "");
		}

		[TearDown]
		public void Cleanup()
		{
			m_CSDef = null;
			m_RADef = null;
			m_VLDef = null;
			m_SLDef = null;
			m_REDef = null;

			MockManager.Verify();
			GC.Collect();
		}

		[Test]
		public void Visa()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.Visa);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDECheckDigitEditDef) m_CSDef.Object, "4222222222222");
			edit.Execute((IWDECheckDigitEditDef) m_CSDef.Object, "4111111111111111");
			edit.Execute((IWDECheckDigitEditDef) m_CSDef.Object, "4012888888881881");
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void VisaNotValid()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.Visa);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "4222222222223");
		}

		[Test]
		public void UPSTracking()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.UPSTracking);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "1Z2221E92210023360");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "1Z2X39082210007990");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "1ZF1532F2210065635");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "1ZV319912110112707");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "1Z2791872210024177");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "1Z2X39082210009238");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "1ZF1532F2210065626");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "J1866610589");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "K0020231705");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "F0495241137");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "U5027976386");
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void UPSTrackingInv()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.UPSTracking);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "1Z2221E92210123360");
		}

		[Test]
		public void UPSPBN()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.UPSPBN);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "001M222198185");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "0029660414114");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "0029624716214");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "0029592028115");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "001K899538890");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "001K899538886");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "001M152775873");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "001M133263852");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "001M151565761");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "001M151565750");
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void UPSPBNInv()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.UPSPBN);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "001M223198185");
		}

		[Test]
		public void MasterC()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.MasterCard);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "5105105105105100");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "5555555555554444");
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void MasterCNotValid()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.MasterCard);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "5105105105105140");
		}

		[Test]
		public void Amex()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.AMEX);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "378282246310005");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "371449635398431");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "378734493671000");
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void AmexNotValid()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.AMEX);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "378282246310015");
		}

		[Test]
		public void Diners()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.Diners);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "38520000023237");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "30569309025904");
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void DinersNotValid()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.Diners);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "38520000023227");
		}

		[Test]
		public void Discover()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.Discover);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "6011111111111117");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "6011000990139424");
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void DiscoverNotValid()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.Discover);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "6011111111111107");
		}

		[Test]
		public void AllCC()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.AMEX | WDECheckDigitMethods.Diners |
				WDECheckDigitMethods.Discover | WDECheckDigitMethods.MasterCard | WDECheckDigitMethods.Visa);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "4222222222222");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "4111111111111111");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "4012888888881881");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "5105105105105100");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "5555555555554444");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "378282246310005");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "371449635398431");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "378734493671000");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "38520000023237");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "30569309025904");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "6011111111111117");
			edit.Execute((IWDEEditDef) m_CSDef.Object, "6011000990139424");
		}

		[Test]
		public void ISBN()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.ISBN);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "0201353415");
		}

		[Test]
		public void UPC()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.UPC);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "726412175425");
		}

		[Test]
		public void Mod10()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.UPC);

			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "08137919805");
		}

		[Test]
		public void CSNull()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.AMEX | WDECheckDigitMethods.ISBN | WDECheckDigitMethods.Mod10 | 
				WDECheckDigitMethods.UPSPBN | WDECheckDigitMethods.UPSTracking);
			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, null);
		}

		[Test]
		public void CSBlank()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.AMEX | WDECheckDigitMethods.ISBN | WDECheckDigitMethods.Mod10 | 
				WDECheckDigitMethods.UPSPBN | WDECheckDigitMethods.UPSTracking);
			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "");
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void CSRand()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.AMEX | WDECheckDigitMethods.ISBN | WDECheckDigitMethods.Mod10 | 
				WDECheckDigitMethods.UPSPBN | WDECheckDigitMethods.UPSTracking);
			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "12312$##ssadfa");
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void CSOne()
		{
			m_CSDef.ExpectGetAlways("Methods", WDECheckDigitMethods.AMEX | WDECheckDigitMethods.ISBN | WDECheckDigitMethods.Mod10 | 
				WDECheckDigitMethods.UPSPBN | WDECheckDigitMethods.UPSTracking);
			IWDEEdit edit = new CheckDigitEdit() as IWDEEdit;
			edit.Execute((IWDEEditDef) m_CSDef.Object, "1");
		}

		[Test]
		[ExpectedException(typeof(WDEException))]
		public void RangeInvalids()
		{
			m_RADef.ExpectGetAlways("LowRange", null);
			m_RADef.ExpectGetAlways("HighRange", "");
			IWDEEdit edit = new RangeEdit();
			edit.Execute((IWDEEditDef) m_RADef.Object, "10");
		}

		[Test]
		public void RangeNumeric()
		{
			m_RADef.ExpectGetAlways("LowRange", "15");
			m_RADef.ExpectGetAlways("HighRange", "20");

			IWDEEdit edit = new RangeEdit();
			edit.Execute((IWDEEditDef) m_RADef.Object, "15");
			edit.Execute((IWDEEditDef) m_RADef.Object, "20");
			edit.Execute((IWDEEditDef) m_RADef.Object, "17");
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void RangeNumericOut()
		{
			m_RADef.ExpectGetAlways("LowRange", "15");
			m_RADef.ExpectGetAlways("HighRange", "20");

			IWDEEdit edit = new RangeEdit();
			edit.Execute((IWDEEditDef) m_RADef.Object, "14");
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void RangeNumericOutHigh()
		{
			m_RADef.ExpectGetAlways("LowRange", "15");
			m_RADef.ExpectGetAlways("HighRange", "20");

			IWDEEdit edit = new RangeEdit();
			edit.Execute((IWDEEditDef) m_RADef.Object, "21");
		}

		[Test]
		public void RangeOneOnly()
		{
			m_RADef.ExpectGetAlways("LowRange", "15");
			m_RADef.ExpectGetAlways("HighRange", "");

			IWDEEdit edit = new RangeEdit();
			edit.Execute((IWDEEditDef) m_RADef.Object, "140");

			m_RADef.ExpectGetAlways("LowRange", "");
			m_RADef.ExpectGetAlways("HighRange", "20");
			edit.Execute((IWDEEditDef) m_RADef.Object, "1");
		}

		[Test]
		public void RangeNumOrder()
		{
			m_RADef.ExpectGetAlways("LowRange", "80");
			m_RADef.ExpectGetAlways("HighRange", "110");

			IWDEEdit edit = new RangeEdit();
			edit.Execute((IWDEEditDef) m_RADef.Object, "91");
		}

		[Test]
		public void RangeAlphaOrder()
		{
			m_RADef.ExpectGetAlways("LowRange", "C");
			m_RADef.ExpectGetAlways("HighRange", "F");

			IWDEEdit edit = new RangeEdit();
			edit.Execute((IWDEEditDef) m_RADef.Object, "D");
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void RangeNumAlpha()
		{
			m_RADef.ExpectGetAlways("LowRange", "C");
			m_RADef.ExpectGetAlways("HighRange", "F");

			IWDEEdit edit = new RangeEdit();
			edit.Execute((IWDEEditDef) m_RADef.Object, "9");
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void RangeAlphaNum()
		{
			m_RADef.ExpectGetAlways("LowRange", "10");
			m_RADef.ExpectGetAlways("HighRange", "20");

			IWDEEdit edit = new RangeEdit();
			edit.Execute((IWDEEditDef) m_RADef.Object, "A");
		}

		[Test]
		public void RangeANRange()
		{
			m_RADef.ExpectGetAlways("LowRange", "1");
			m_RADef.ExpectGetAlways("HighRange", "A");

			IWDEEdit edit = new RangeEdit();
			edit.Execute((IWDEEditDef) m_RADef.Object, "21");
		}

		[Test]
		[ExpectedException(typeof(WDEException))]
		public void RangeAlphaInv()
		{
			m_RADef.ExpectGetAlways("LowRange", "A");
			m_RADef.ExpectGetAlways("HighRange", "1");

			IWDEEdit edit = new RangeEdit();
			edit.Execute((IWDEEditDef) m_RADef.Object, "21");
		}

		[Test]
		[ExpectedException(typeof(WDEException))]
		public void RangeAlphaEq()
		{
			m_RADef.ExpectGetAlways("LowRange", "A");
			m_RADef.ExpectGetAlways("HighRange", "A");

			IWDEEdit edit = new RangeEdit();
			edit.Execute((IWDEEditDef) m_RADef.Object, "21");
		}

		[Test]
		[ExpectedException(typeof(WDEException))]
		public void RangeNumEq()
		{
			m_RADef.ExpectGetAlways("LowRange", "1");
			m_RADef.ExpectGetAlways("HighRange", "1");

			IWDEEdit edit = new RangeEdit();
			edit.Execute((IWDEEditDef) m_RADef.Object, "21");
		}

		[Test]
		[ExpectedException(typeof(WDEException))]
		public void RangeNumInv()
		{
			m_RADef.ExpectGetAlways("LowRange", "5");
			m_RADef.ExpectGetAlways("HighRange", "1");

			IWDEEdit edit = new RangeEdit();
			edit.Execute((IWDEEditDef) m_RADef.Object, "21");
		}

		[Test]
		public void ValidLengths()
		{
			m_VLDef.ExpectGetAlways("Lengths", new int[] {5, 10});

			IWDEEdit edit = new ValidLengthsEdit();
			edit.Execute((IWDEEditDef) m_VLDef.Object, "12345");
			edit.Execute((IWDEEditDef) m_VLDef.Object, "0123456789");
			edit.Execute((IWDEEditDef) m_VLDef.Object, "");
			edit.Execute((IWDEEditDef) m_VLDef.Object, null);
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void ValidLengthsInv()
		{
			m_VLDef.ExpectGetAlways("Lengths", new int[] {5, 10});
			IWDEEdit edit = new ValidLengthsEdit();
			edit.Execute((IWDEEditDef) m_VLDef.Object, "1234");
		}

		[Test]
		public void SimpleList()
		{
			m_SLDef.ExpectGetAlways("List", new string[] {"ONE", "TWO"});

			IWDEEdit edit = new SimpleListEdit();
			edit.Execute((IWDEEditDef) m_SLDef.Object, "ONE");
			edit.Execute((IWDEEditDef) m_SLDef.Object, "TWO");
			edit.Execute((IWDEEditDef) m_SLDef.Object, "");
			edit.Execute((IWDEEditDef) m_SLDef.Object, null);
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void SimpleListInv()
		{
			m_SLDef.ExpectGetAlways("List", new string[] {"ONE", "TWO"});

			IWDEEdit edit = new SimpleListEdit();
			edit.Execute((IWDEEditDef) m_SLDef.Object, "THREE");
		}

		[Test]
		public void Required()
		{
			IWDEEdit edit = new RequiredEdit();
			edit.Execute((IWDEEditDef) m_REDef.Object, "NOTBLANK");
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void RequiredBlank()
		{
			IWDEDataSet DataSet = WDEDataSet.Create();

			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
			sw.Write(m_ResMan.GetString("DataSetFixSessNoGaps"));
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			DataSet.LoadFromStream("User1", "ScriptTest", WDEOpenMode.Verify, "SomeLocation", ms);

			IWDEWebDEInternal iwde = (IWDEWebDEInternal) Host.WebDE;
			iwde.CurrentField = (IWDEScriptField) DataSet.Documents[0].Records[0].Fields[0];

			IWDEEdit edit = new RequiredEdit();
			edit.Execute((IWDEEditDef) m_REDef.Object, "");
			sw.Close();
		}

		[Test]
		[ExpectedException(typeof(WDEEditException))]
		public void RequiredNull()
		{
			IWDEDataSet DataSet = WDEDataSet.Create();

			MemoryStream ms = new MemoryStream();
			StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
			sw.Write(m_ResMan.GetString("DataSetFixSessNoGaps"));
			sw.Flush();
			ms.Seek(0, SeekOrigin.Begin);
			DataSet.LoadFromStream("User1", "ScriptTest", WDEOpenMode.Verify, "SomeLocation", ms);

			IWDEWebDEInternal iwde = (IWDEWebDEInternal) Host.WebDE;
			iwde.CurrentField = (IWDEScriptField) DataSet.Documents[0].Records[0].Fields[0];

			IWDEEdit edit = new RequiredEdit();
			edit.Execute((IWDEEditDef) m_REDef.Object, null);
			sw.Close();
		}
	}
}
