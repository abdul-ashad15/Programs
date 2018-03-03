using System;
using System.Drawing;
using System.Xml;
using System.Text;
using System.IO;
using System.Globalization;

using WebDX.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;

namespace WebDX.Api.Tests
{
	/// <summary>
	/// Tests for Utils class.
	/// </summary>
	[TestClass]
	public class UtilsTests
	{
		public UtilsTests()
		{
		}

		[TestCleanup]
		public void TearDown()
		{
			GC.Collect();
		}

		[TestMethod]
		public void UtilsUCOMIStreamToStream()
		{
			MemoryStream instream = new MemoryStream();
			StreamWriter sw = new StreamWriter(instream);
			sw.Write("TESTING");
			sw.Flush();
			TestUcomIstream InStream = new TestUcomIstream(instream.ToArray());
			MemoryStream OutStream = new MemoryStream();
			WebDX.Api.Utils.UCOMIStreamToStream(InStream, OutStream, 2048);
			OutStream.Seek(0, SeekOrigin.Begin);
			StreamReader sr = new StreamReader(OutStream);
			string test = sr.ReadToEnd();
			Assert.AreEqual("TESTING", test);
			sw.Close();
			sr.Close();
		}

		[TestMethod]
		public void UtilsUCOMIStreamToStream_Long()
		{
			Random r = new Random();
			byte[] bytes = new byte[r.Next(3000000, 5000000)];
			MemoryStream instream = new MemoryStream(bytes);
			TestUcomIstream InStream = new TestUcomIstream(instream.ToArray());
			MemoryStream OutStream = new MemoryStream();
			WebDX.Api.Utils.UCOMIStreamToStream(InStream, OutStream, 2048);
			byte[] res = OutStream.ToArray();
			Assert.AreEqual(bytes.Length, res.Length, "Lengths are different");
			for(int i = 0; i < res.Length; i++)
				Assert.AreEqual(bytes[i], res[i], "Contents are different");
		}

		[TestMethod]
		public void UtilsStreamToUCOMIStream()
		{
			MemoryStream InStream = new MemoryStream();
			StreamWriter sw = new StreamWriter(InStream);
			sw.Write("TESTING");
			sw.Flush();
			InStream.Seek(0, SeekOrigin.Begin);
			TestUcomIstream OutStream = new TestUcomIstream();
			WebDX.Api.Utils.StreamToUCOMIStream(InStream, OutStream, 2048);
			MemoryStream outstream = new MemoryStream(OutStream.GetBytes());
			StreamReader sr = new StreamReader(outstream);
			string test = sr.ReadToEnd();
			Assert.AreEqual("TESTING", test);
			sw.Close();
			sr.Close();
		}

		[TestMethod]
		public void UtilsStreamToUCOMIStream_Long()
		{
			Random r = new Random();
			byte[] bytes = new byte[r.Next(3000000, 5000000)];
			MemoryStream InStream = new MemoryStream(bytes);
			TestUcomIstream OutStream = new TestUcomIstream();
			WebDX.Api.Utils.StreamToUCOMIStream(InStream, OutStream, 2048);
			byte[] res = OutStream.GetBytes();
			Assert.AreEqual(bytes.Length, res.Length, "Lengths are different");
			for(int i = 0; i < res.Length; i++)
				Assert.AreEqual(bytes[i], res[i], "Contents are different");
		}

		[TestMethod]
		public void UtilsGetAttribute()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xw = new XmlTextWriter(ms, UnicodeEncoding.UTF8);
			xw.WriteStartElement("Test");
			xw.WriteAttributeString("Test1", "Value1");
			xw.WriteAttributeString("Test2", "Value2");
			xw.WriteEndElement();
			xw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			XmlTextReader xr = new XmlTextReader(ms);
			xr.Read();
			string Test1 = Utils.GetAttribute(xr, "Test1", "");
			string Test2 = Utils.GetAttribute(xr, "Test2", "");
			string Test3 = Utils.GetAttribute(xr, "Test3", "DefaultValue");
			Assert.AreEqual("Value1", Test1);
			Assert.AreEqual("Value2", Test2);
			Assert.AreEqual("DefaultValue", Test3);
			xw.Close();
			xr.Close();
		}

		[TestMethod]
		public void UtilsGetBoolValue()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xw = new XmlTextWriter(ms, UnicodeEncoding.UTF8);
			xw.WriteStartElement("Test");
			xw.WriteAttributeString("Bool1", bool.TrueString);
			xw.WriteAttributeString("Bool2", "");
			xw.WriteAttributeString("Bool3", "TRUE");
			xw.WriteEndElement();
			xw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			XmlTextReader xr = new XmlTextReader(ms);
			xr.Read();
			bool bool1 = Utils.GetBoolValue(xr, "Bool1", false);
			bool bool2 = Utils.GetBoolValue(xr, "Bool2", false);
			bool bool3 = Utils.GetBoolValue(xr, "Bool3", false);
			bool bool4 = Utils.GetBoolValue(xr, "Bool4", true);
			Assert.AreEqual(true, bool1);
			Assert.AreEqual(false, bool2);
			Assert.AreEqual(true, bool3);
			Assert.AreEqual(true, bool4);
			xw.Close();
			xr.Close();
		}

		[TestMethod]
		[ExpectedException(typeof(XmlException))]
		public void UtilsGetPointValue()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xw = new XmlTextWriter(ms, UnicodeEncoding.UTF8);
			xw.WriteStartElement("Test");
			xw.WriteAttributeString("Point1", "3, 5");
			xw.WriteAttributeString("Point2", "");
			xw.WriteAttributeString("Point3", "Invalid");
			xw.WriteEndElement();
			xw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			XmlTextReader xr = new XmlTextReader(ms);
			xr.Read();
			Point point1 = Utils.GetPointValue(xr, "Point1");
			Point point2 = Utils.GetPointValue(xr, "Point2");
			Assert.AreEqual(new Point(3, 5), point1);
			Assert.AreEqual(new Point(0, 0), point2);
			Point point3 = Utils.GetPointValue(xr, "Point3"); // throws exception
			xw.Close();
			xr.Close();
		}

		[TestMethod]
		[ExpectedException(typeof(XmlException))]
		public void UtilsGetRectValue()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xw = new XmlTextWriter(ms, UnicodeEncoding.UTF8);
			xw.WriteStartElement("Test");
			xw.WriteAttributeString("Rect1", "1, 2, 3, 4");
			xw.WriteAttributeString("Rect2", "");
			xw.WriteAttributeString("Rect3", "Invalid");
			xw.WriteEndElement();
			xw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			XmlTextReader xr = new XmlTextReader(ms);
			xr.Read();
			Rectangle rect1 = Utils.GetRectValue(xr, "Rect1");
			Rectangle rect2 = Utils.GetRectValue(xr, "Rect2");
			Assert.AreEqual(new Rectangle(1, 2, 2, 2), rect1);
			Assert.AreEqual(new Rectangle(0, 0, 0, 0), rect2);
			Rectangle rect3 = Utils.GetRectValue(xr, "Rect3"); // throws exception
			xw.Close();
			xr.Close();
		}

		[TestMethod]
		public void UtilsRectToString()
		{
			Rectangle r = new Rectangle(1, 2, 3, 4);
			Assert.AreEqual("1,2,4,6", Utils.RectToString(r));
		}

		[TestMethod]
		[ExpectedException(typeof(XmlException))]
		public void UtilsGetMode()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xw = new XmlTextWriter(ms, UnicodeEncoding.UTF8);
			xw.WriteStartElement("Test");
			xw.WriteAttributeString("Mode1", "Create");
			xw.WriteAttributeString("Mode2", "dsEntry");
			xw.WriteAttributeString("Mode3", "FocusAudit");
			xw.WriteAttributeString("Mode4", "Invalid");
			xw.WriteEndElement();
			xw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			XmlTextReader xr = new XmlTextReader(ms);
			xr.Read();
			WDEOpenMode mode1 = Utils.GetMode(xr, "Mode1");
			WDEOpenMode mode2 = Utils.GetMode(xr, "Mode2");
			WDEOpenMode mode3 = Utils.GetMode(xr, "Mode3");
			Assert.AreEqual(WDEOpenMode.Create, mode1);
			Assert.AreEqual(WDEOpenMode.Entry, mode2);
			Assert.AreEqual(WDEOpenMode.FocusAudit, mode3);
			WDEOpenMode mode4 = Utils.GetMode(xr, "Mode4");
			xw.Close();
			xr.Close();
		}

		[TestMethod]
		[ExpectedException(typeof(XmlException))]
		public void UtilsGetStatus()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xw = new XmlTextWriter(ms, UnicodeEncoding.UTF8);
			xw.WriteStartElement("Test");
			xw.WriteAttributeString("Session1", "None");
			xw.WriteAttributeString("Session2", "ssCompleted");
			xw.WriteAttributeString("Session3", "Rejected");
			xw.WriteAttributeString("Session4", "Invalid");
			xw.WriteEndElement();
			xw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			XmlTextReader xr = new XmlTextReader(ms);
			xr.Read();
			WDESessionStatus session1 = Utils.GetStatus(xr, "Session1");
			WDESessionStatus session2 = Utils.GetStatus(xr, "Session2");
			WDESessionStatus session3 = Utils.GetStatus(xr, "Session3");
			Assert.AreEqual(WDESessionStatus.None, session1);
			Assert.AreEqual(WDESessionStatus.Completed, session2);
			Assert.AreEqual(WDESessionStatus.Rejected, session3);
			WDESessionStatus session4 = Utils.GetStatus(xr, "Session4");
			xw.Close();
			xr.Close();
		}

		[TestMethod]
		public void UtilsGetDataType()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xw = new XmlTextWriter(ms, UnicodeEncoding.UTF8);
			xw.WriteStartElement("Test");
			xw.WriteAttributeString("Dt1", "Text");
			xw.WriteAttributeString("Dt2", "dtNumber");
			xw.WriteAttributeString("Dt3", "YesNo");
			xw.WriteAttributeString("Dt4", "Invalid");
			xw.WriteAttributeString("Dt5", "dt");
			xw.WriteEndElement();
			xw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			XmlTextReader xr = new XmlTextReader(ms);
			xr.Read();
			WDEDataType dt1 = Utils.GetDataType(xr, "Dt1");
			WDEDataType dt2 = Utils.GetDataType(xr, "Dt2");
			WDEDataType dt3 = Utils.GetDataType(xr, "Dt3");
			WDEDataType dt4 = Utils.GetDataType(xr, "Dt4");
			WDEDataType dt5 = Utils.GetDataType(xr, "Dt5");
			Assert.AreEqual(WDEDataType.Text, dt1);
			Assert.AreEqual(WDEDataType.Number, dt2);
			Assert.AreEqual(WDEDataType.YesNo, dt3);
			Assert.AreEqual(WDEDataType.Text, dt4);
			Assert.AreEqual(WDEDataType.Text, dt5);
			xw.Close();
			xr.Close();
		}

		[TestMethod]
		public void UtilsGetFieldFlags()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xw = new XmlTextWriter(ms, UnicodeEncoding.UTF8);
			xw.WriteStartElement("Test");
			xw.WriteAttributeString("Ff1", "None");
			xw.WriteAttributeString("Ff2", "ffActive");
			xw.WriteAttributeString("Ff3", "Completed");
			xw.WriteAttributeString("Ff4", "Invalid");
			xw.WriteAttributeString("Ff5", "ff");
			xw.WriteEndElement();
			xw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			XmlTextReader xr = new XmlTextReader(ms);
			xr.Read();
			WDEFieldFlags ff1 = Utils.GetFieldFlags(xr, "Ff1");
			WDEFieldFlags ff2 = Utils.GetFieldFlags(xr, "Ff2");
			WDEFieldFlags ff3 = Utils.GetFieldFlags(xr, "Ff3");
			WDEFieldFlags ff4 = Utils.GetFieldFlags(xr, "Ff4");
			WDEFieldFlags ff5 = Utils.GetFieldFlags(xr, "Ff5");
			Assert.AreEqual(WDEFieldFlags.None, ff1);
			Assert.AreEqual(WDEFieldFlags.Active, ff2);
			Assert.AreEqual(WDEFieldFlags.Completed, ff3);
			Assert.AreEqual(WDEFieldFlags.None, ff4);
			Assert.AreEqual(WDEFieldFlags.None, ff5);
			xw.Close();
			xr.Close();
		}

		[TestMethod]
		public void UtilsGetMiscFlags()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xw = new XmlTextWriter(ms, UnicodeEncoding.UTF8);
			xw.WriteStartElement("Test");
			xw.WriteAttributeString("Ff1", "None");
			xw.WriteAttributeString("Ff2", "mfKeyable, mfVerify");
			xw.WriteAttributeString("Ff3", "Verify");
			xw.WriteAttributeString("Ff4", "Invalid");
			xw.WriteAttributeString("Ff5", "mf");
			xw.WriteEndElement();
			xw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			XmlTextReader xr = new XmlTextReader(ms);
			xr.Read();
			WDEMiscFlags ff1 = Utils.GetMiscFlags(xr, "Ff1");
			WDEMiscFlags ff2 = Utils.GetMiscFlags(xr, "Ff2");
			WDEMiscFlags ff3 = Utils.GetMiscFlags(xr, "Ff3");
			WDEMiscFlags ff4 = Utils.GetMiscFlags(xr, "Ff4");
			Assert.AreEqual(WDEMiscFlags.None, ff1);
			Assert.AreEqual(WDEMiscFlags.Keyable | WDEMiscFlags.Verify, ff2);
			Assert.AreEqual(WDEMiscFlags.Verify, ff3);
			Assert.AreEqual(WDEMiscFlags.None, ff4);
			WDEMiscFlags ff5 = Utils.GetMiscFlags(xr, "Ff5");
			xw.Close();
			xr.Close();
		}

		[TestMethod]
		[ExpectedException(typeof(XmlException))]
		public void UtilsGetMiscFlags_nomf()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xw = new XmlTextWriter(ms, UnicodeEncoding.UTF8);
			xw.WriteStartElement("Test");
			xw.WriteAttributeString("Ff2", "mfKeyable, Verify");
			xw.WriteEndElement();
			xw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			XmlTextReader xr = new XmlTextReader(ms);
			xr.Read();
			WDEMiscFlags ff2 = Utils.GetMiscFlags(xr, "Ff2");
			xw.Close();
			xr.Close();
		}

		[TestMethod]
		public void UtilsGetFieldStatus()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xw = new XmlTextWriter(ms, UnicodeEncoding.UTF8);
			xw.WriteStartElement("Test");
			xw.WriteAttributeString("Ff1", "None");
			xw.WriteAttributeString("Ff2", "fsKeyed");
			xw.WriteAttributeString("Ff3", "Compared");
			xw.WriteAttributeString("Ff4", "Invalid");
			xw.WriteAttributeString("Ff5", "fs");
			xw.WriteEndElement();
			xw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			XmlTextReader xr = new XmlTextReader(ms);
			xr.Read();
			WDEFieldStatus ff1 = Utils.GetFieldStatus(xr, "Ff1");
			WDEFieldStatus ff2 = Utils.GetFieldStatus(xr, "Ff2");
			WDEFieldStatus ff3 = Utils.GetFieldStatus(xr, "Ff3");
			WDEFieldStatus ff4 = Utils.GetFieldStatus(xr, "Ff4");
			Assert.AreEqual(WDEFieldStatus.None, ff1);
			Assert.AreEqual(WDEFieldStatus.Keyed, ff2);
			Assert.AreEqual(WDEFieldStatus.Compared, ff3);
			Assert.AreEqual(WDEFieldStatus.None, ff4);
			WDEFieldStatus ff5 = Utils.GetFieldStatus(xr, "Ff5");
			xw.Close();
			xr.Close();
		}

		[TestMethod]
		public void UtilsGetFieldStatus_Compat()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xw = new XmlTextWriter(ms, UnicodeEncoding.UTF8);
			xw.WriteStartElement("Test");
			xw.WriteAttributeString("Ff1", "fsQIVerified");
			xw.WriteAttributeString("Ff2", "fsRejected");
			xw.WriteEndElement();
			xw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			XmlTextReader xr = new XmlTextReader(ms);
			xr.Read();
			WDEFieldStatus ff1 = Utils.GetFieldStatus(xr, "Ff1");
			WDEFieldStatus ff2 = Utils.GetFieldStatus(xr, "Ff2");
			Assert.AreEqual(WDEFieldStatus.Verified, ff1);
			Assert.AreEqual(WDEFieldStatus.Flagged, ff2);
			xw.Close();
			xr.Close();
		}

		[TestMethod]
		public void UtilsGetIntValue()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xw = new XmlTextWriter(ms, UnicodeEncoding.UTF8);
			xw.WriteStartElement("Test");
			xw.WriteAttributeString("Int1", "432");
			xw.WriteAttributeString("Int2", "Invalid");
			xw.WriteEndElement();
			xw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			XmlTextReader xr = new XmlTextReader(ms);
			xr.Read();
			int int1 = Utils.GetIntValue(xr, "Int1");
			int int2 = Utils.GetIntValue(xr, "Int2");
			Assert.AreEqual(432, int1);
			Assert.AreEqual(0, int2);
			xw.Close();
			xr.Close();
		}

		[TestMethod]
		public void UtilsSetStatusByDataOwner()
		{
			//pgm,edit,keyer,script,sys
			WDEFieldStatus status = WDEFieldStatus.DblEntered;
			Utils.SetStatusByDataOwner("doPgm", ref status);
			Assert.AreEqual(WDEFieldStatus.None, status);

			status = WDEFieldStatus.DblEntered;
			Utils.SetStatusByDataOwner("doEdit", ref status);
			Assert.AreEqual(WDEFieldStatus.Plugged, status);

			status = WDEFieldStatus.DblEntered;
			Utils.SetStatusByDataOwner("doKeyer", ref status);
			Assert.AreEqual(WDEFieldStatus.DblEntered, status);

			status = WDEFieldStatus.None;
			Utils.SetStatusByDataOwner("doKeyer", ref status);
			Assert.AreEqual(WDEFieldStatus.Keyed, status);

			status = WDEFieldStatus.DblEntered;
			Utils.SetStatusByDataOwner("doScript", ref status);
			Assert.AreEqual(WDEFieldStatus.Plugged, status);

			status = WDEFieldStatus.DblEntered;
			Utils.SetStatusByDataOwner("doSys", ref status);
			Assert.AreEqual(WDEFieldStatus.None, status);

			status = WDEFieldStatus.DblEntered;
			Utils.SetStatusByDataOwner("Invalid", ref status);
			Assert.AreEqual(WDEFieldStatus.DblEntered, status);
		}

		[TestMethod]
		public void UtilsGetDoubleValue()
		{
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xw = new XmlTextWriter(ms, UnicodeEncoding.UTF8);
			xw.WriteStartElement("Test");
			xw.WriteAttributeString("Dbl1", "432.56");
			xw.WriteAttributeString("Dbl2", "Invalid");
			xw.WriteEndElement();
			xw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			XmlTextReader xr = new XmlTextReader(ms);
			xr.Read();
			double dbl1 = Utils.GetDoubleValue(xr, "Dbl1");
			double dbl2 = Utils.GetDoubleValue(xr, "Dbl2");
			Assert.AreEqual(432.56, dbl1);
			Assert.AreEqual(0, dbl2);
			xw.Close();
			xr.Close();
		}

		[TestMethod]
		public void UtilsDateTimeToISO()
		{
			DateTime testdate = new DateTime(2003, 05, 15, 13, 25, 35, 465);
			string teststring = Utils.DateTimeToISO(testdate);

			CultureInfo defCulture = CultureInfo.CurrentCulture;
			
			try
			{
				CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
				int i = 0;
				foreach(CultureInfo curCulture in cultures)
				{
					if(!curCulture.IsNeutralCulture)
					{
						System.Threading.Thread.CurrentThread.CurrentCulture = curCulture;
						DateTime parsedate = DateTime.Parse(teststring);
						Assert.AreEqual(testdate.ToUniversalTime(), parsedate.ToUniversalTime(), 
							"Culture {0} doesn't parse correctly {1}, {2}",
							new object[] {curCulture.DisplayName, testdate.ToUniversalTime(), parsedate.ToUniversalTime()});
						i++;
					}
				}
				Console.WriteLine("ISO Date tested in {0} cultures", new object[] {i});
			}
			finally
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = defCulture;
			}
		}

		[TestMethod]
		public void UtilsISOToDateTime()
		{
			DateTime testdate = new DateTime(2003, 05, 15, 13, 25, 35, 465);
			string teststring = Utils.DateTimeToISO(testdate);
			DateTime parsedate = Utils.ISOToDateTime(teststring);
			Assert.AreEqual(testdate, parsedate, "Direct parse is not equal");

			teststring = testdate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff");
			teststring += "GMT";
			parsedate = Utils.ISOToDateTime(teststring);
			Assert.AreEqual(testdate, parsedate, "GMT parse is not equal");

			teststring = testdate.ToString("yyyy-MM-ddTHH:mm:ss.fff");
			teststring += "-05:30";
			parsedate = Utils.ISOToDateTime(teststring);
			Assert.AreEqual(new DateTime(2003, 05, 15, 12, 55, 35, 465), parsedate, "India time is not equal");
		}

		[TestMethod]
		public void UtilsUSStrToDateTime()
		{
			DateTime testdate = new DateTime(2003, 05, 09, 13, 25, 35, 465);
			string teststring = testdate.ToString("MM/dd/yyyy HH:mm:ss.fff");

			CultureInfo defCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
			try
			{
				CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
				int i = 0;
				foreach(CultureInfo curCulture in cultures)
				{
					if(!curCulture.IsNeutralCulture)
					{
						System.Threading.Thread.CurrentThread.CurrentCulture = curCulture;
						DateTime parsedate = Utils.USStrToDateTime(teststring);
						Assert.AreEqual(testdate, parsedate, "Parsing failed for culture: {0}", new object[] {curCulture.Name});
						i++;
					}
				}
				Console.WriteLine(string.Format("USStrtoDateTime Tested in {0} cultures", new object[] {i}));
			}
			finally
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = defCulture;
			}
		}

		[TestMethod]
		public void FontToString()
		{
			Font aFont = new Font("Tahoma", 10, FontStyle.Italic);
			string temp = Utils.FontToString(aFont);
			Assert.AreEqual("Tahoma, 10pt, style=Italic", temp);
		}

		[TestMethod]
		public void GetFont()
		{
			StringReader sr = new StringReader("<Font Data=\"Tahoma, 10pt, style=Italic\" />");
			XmlTextReader XmlReader = new XmlTextReader(sr);
			XmlReader.MoveToContent();
			Font defaultFont = new Font("Tahoma", 10);
			Font aFont = Utils.GetFont(XmlReader, "Data", Utils.FontToString(defaultFont));
			defaultFont.Dispose();
			XmlReader.Close();
			Assert.AreEqual("Tahoma", aFont.Name, "Name");
			Assert.AreEqual(10, aFont.Size, "Size");
			Assert.AreEqual(FontStyle.Italic, aFont.Style, "Style");
			aFont.Dispose();
		}

		[TestMethod]
		public void ConvertFontHeight()
		{
			Graphics gr = Graphics.FromHwnd(IntPtr.Zero);
			float screendpi = gr.DpiY;
			gr.Dispose();

			float res = Utils.ConvertFontHeight(-13);

			Assert.AreEqual(-Math.Round(-13 * 72 / screendpi), res, "-13");
			res = Utils.ConvertFontHeight(-17);
			Assert.AreEqual(-Math.Round(-17 * 72 / screendpi), res, "-17");
		}

		[TestMethod]
		public void ConvertFontStyle()
		{
			FontStyle style = Utils.ConvertFontStyle("fsBold,fsItalic,fsUnderline,fsStrikeOut", 0, 0);
			Assert.AreEqual(FontStyle.Bold | FontStyle.Italic | 
				FontStyle.Strikeout | FontStyle.Underline, style, "Style");
		}

		[TestMethod]
		public void ConvertBitmapFont()
		{
			string name = "Fixedsys";
			float height = 14f;
			Utils.ConvertBitmapFont(ref name, ref height);
			Font f = new Font(name, height);
			Assert.AreEqual("Lucida Console, 10pt", Utils.FontToString(f), "Fixedsys");

			name = "System";
			height = 22f;
			Utils.ConvertBitmapFont(ref name, ref height);
			f = new Font(name, height);
			Assert.AreEqual("Lucida Console, 9pt", Utils.FontToString(f), "System");

			name = "Courier";
			height = 90f;
			Utils.ConvertBitmapFont(ref name, ref height);
			f = new Font(name, height);
			Assert.AreEqual("Courier New, 10pt", Utils.FontToString(f), "Courier");

			name = "Terminal";
			height = 6f;
			Utils.ConvertBitmapFont(ref name, ref height);
			f = new Font(name, height);
			Assert.AreEqual("Lucida Console, 6pt", Utils.FontToString(f), "Terminal");

			name = "Unknown";
			height = 14f;
			Utils.ConvertBitmapFont(ref name, ref height);
			f = new Font(name, height);
			Assert.AreEqual("Microsoft Sans Serif, 14pt", Utils.FontToString(f), "Unknown");

			name = "Lucida Console";
			height = 14f;
			Utils.ConvertBitmapFont(ref name, ref height);
			f = new Font(name, height);
			Assert.AreEqual("Lucida Console, 14pt", Utils.FontToString(f), "Lucida Console");
		}
	}
}
