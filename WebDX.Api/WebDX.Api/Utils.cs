using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Serialization;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Globalization;

namespace WebDX.Api
{
	/// <summary>
	/// Miscellaneous helper methods.
	/// </summary>
#if DEBUG
	public class Utils
#else
	internal class Utils
#endif
	{
		private const int DEFAULT_BUFFER_SIZE = 2048;

		public Utils()
		{
		}

		private static string TrimPrefix(string Data, string Prefix, string ErrorType, int LineNumber, int LinePosition)
		{
			if(Data.StartsWith(Prefix))
			{
				string[] Datas = Data.Split(',');
				for(int i = 0; i < Datas.Length; i++)
				{
					string t = Datas[i].Trim();
					if((t.Length > Prefix.Length) && (t.StartsWith(Prefix)))
					{
						t = t.Substring(Prefix.Length);
					}
					else if(t.Length != Prefix.Length)
						throw new XmlException("Invalid " + ErrorType + " in Xml: " + t, null, LineNumber, LinePosition);
					Datas[i] = t;
				}
				Data = string.Join(", ", Datas);
			}
			
			return Data;
		}

		private static string ConvertDelphiKnownColor(string ColorName)
		{
			// assumes that the "cl" prefix has already been removed using TrimPrefix.
			switch(ColorName)
			{
				case "Background":
					return "Desktop";
				case "CaptionText":
					return "ControlText";
				case "BtnFace":
					return "Control";
				case "BtnShadow":
					return "ControlDark";
				case "BtnText":
					return "ControlText";
				case "BtnHighLight":
					return "ControlLight";
				case "InfoBk":
					return "Info";
				case "HotLight":
					return "HotTrack";
				case "GradientActiveCaption":
					return "ActiveCaption";
				case "GradientInactiveCaption":
					return "InactiveCaption";
				case "LtGray":
					return "LightGray";
				case "DkGray":
					return "DarkGray";
				case "MoneyGreen":
					return "0x00C0DCC0";
				case "Cream":
					return "0x00F0FBFF";
				case "MedGray":
					return "0x00A4A0A0";
				default:
					return ColorName;
			}
		}

		public static void UCOMIStreamToStream(IStream InStream, Stream OutStream)
		{
			UCOMIStreamToStream(InStream, OutStream, DEFAULT_BUFFER_SIZE);
		}

		public static void UCOMIStreamToStream(IStream InStream, Stream OutStream, int BufferSize)
		{
			if(InStream == null)
				throw new ArgumentNullException("InStream", "InStream cannot be null");

			if(OutStream == null)
				throw new ArgumentNullException("OutStream", "OutStream cannot be null");

			byte[] buf = new byte[BufferSize];
			int len = 0;
			IntPtr lenptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(long)));
			try
			{
				InStream.Read(buf, BufferSize, lenptr);
				len = Convert.ToInt32(Marshal.ReadInt32(lenptr));
				while(len > 0)
				{
					OutStream.Write(buf, 0, len);
					InStream.Read(buf, BufferSize, lenptr);
					len = Convert.ToInt32(Marshal.ReadInt32(lenptr));
				}
				OutStream.Flush();
			}
			finally
			{
				Marshal.FreeHGlobal(lenptr);
			}
		}

		public static void StreamToUCOMIStream(Stream InStream, IStream OutStream)
		{
			StreamToUCOMIStream(InStream, OutStream, DEFAULT_BUFFER_SIZE);
		}

		public static void StreamToUCOMIStream(Stream InStream, IStream OutStream, int BufferSize)
		{
			if(InStream == null)
                throw new ArgumentNullException("InStream", "InStream cannot be null");

			if(OutStream == null)
                throw new ArgumentNullException("OutStream", "OutStream cannot be null");

			byte[] buf = new byte[BufferSize];
			int len = 0;
			IntPtr lenptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(long)));
			try
			{
				while((len = InStream.Read(buf, 0, BufferSize)) > 0)
				{
					OutStream.Write(buf, len, lenptr);
					int written = Convert.ToInt32(Marshal.ReadInt32(lenptr));
					if(written != len)
					{
						if(written == 0)
						{
							OutStream.SetSize(InStream.Length);
							InStream.Seek(-len, SeekOrigin.Current);
						}
						else
							throw new IOException("Stream write error");
					}
				}
			}
			finally
			{
				Marshal.FreeHGlobal(lenptr);
			}
		}

        public static TimeSpan GetTimeSpanValue(XmlTextReader XmlReader, string AttributeName)
        {
            string value = GetAttribute(XmlReader, AttributeName, "");
            if (value != "")
            {
                if (value.StartsWith("P"))
                {
                    try
                    {
                        return SoapDuration.Parse(value);
                    }
                    catch
                    {
                        return TimeSpan.Zero;
                    }
                }
                else
                {
                    TimeSpan result = TimeSpan.Zero;
                    if (TimeSpan.TryParse(value, out result))
                        return result;
                    else
                        return TimeSpan.Zero;
                }
            }
            else
                return TimeSpan.Zero;
        }

		public static string GetAttribute(XmlTextReader XmlReader, string AttributeName, string DefaultValue)
		{
			if(XmlReader == null)
				throw new ArgumentNullException("XmlReader", "XmlReader cannot be null");
			if(AttributeName == null)
				throw new ArgumentNullException("AttributeName", "AttributeName cannot be null");
			if(DefaultValue == null)
				throw new ArgumentNullException("DefaultValue", "DefaultValue cannot be null");

			if(XmlReader.NodeType != XmlNodeType.Element)
				throw new XmlException(
					string.Format("Invalid element reading xml attribute: \"{0}\"", new object[] { XmlReader.NodeType }),
					null,
					XmlReader.LineNumber,
					XmlReader.LinePosition);

			string result = XmlReader.GetAttribute(AttributeName);
			if (result == null)
				return DefaultValue;
			else
				return result;
		}

		public static bool GetBoolValue(XmlTextReader XmlReader, string AttributeName, bool DefalutValue)
		{
			string val = GetAttribute(XmlReader, AttributeName, DefalutValue.ToString());
			if(string.Compare(val, bool.TrueString, true) == 0)
				return true;
			else
				return false;
		}

		public static Point GetPointValue(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = Utils.GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				string[] xy = temp.Split(',');
				if(xy.Length != 2)
					throw new XmlException(
						string.Format("Invalid point value in Xml file \"{0}\"", new object[] { temp }),
						null,
						XmlReader.LineNumber,
						XmlReader.LinePosition);
				int x = 0;
				int y = 0;
				x = StrToIntDef(xy[0].Trim(), 0);
				y = StrToIntDef(xy[1].Trim(), 0);
				return new Point(x, y);
			}
			else
				return new Point(0, 0);
		}

		public static Rectangle GetRectValue(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = Utils.GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				string[] bounds = temp.Split(',');
				if(bounds.Length != 4)
					throw new XmlException(
						string.Format("Invalid rectangle value in Xml file \"{0}\"", new object[] { temp }),
						null,
						XmlReader.LineNumber,
						XmlReader.LinePosition);

				int[] intbounds = new int[4];
				for(int i = 0; i < 4; i++)
				{
					intbounds[i] = StrToIntDef(bounds[i].Trim(), 0);
				}
				return new Rectangle(intbounds[0], intbounds[1], intbounds[2] - intbounds[0], intbounds[3] - intbounds[1]);
			}
			else
				return new Rectangle(0, 0, 0, 0);
		}

		public static string RectToString(Rectangle Rect)
		{
			return string.Format("{0},{1},{2},{3}", new object[] {Rect.Left, Rect.Top, Rect.Right, Rect.Bottom});
		}

		public static WDEOpenMode GetMode(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "ds", "Mode",
					XmlReader.LineNumber, XmlReader.LinePosition);

				if(temp == "QC")
					temp = "QI";

				try
				{
					return (WDEOpenMode) Enum.Parse(typeof(WDEOpenMode), temp, true);
				}
				catch
				{
					throw new XmlException("Invalid Mode in Xml: " + temp, null, XmlReader.LineNumber, XmlReader.LinePosition);
				}
			}
			else
				throw new XmlException("Invalid Mode in Xml: " + temp, null, XmlReader.LineNumber, XmlReader.LinePosition);
		}

		public static WDESessionStatus GetStatus(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "ss", "SessionStatus",
					XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDESessionStatus) Enum.Parse(typeof(WDESessionStatus), temp, true);
				}
				catch
				{
					throw new XmlException("Invalid SessionStatus in Xml: " + temp, null, XmlReader.LineNumber, XmlReader.LinePosition);
				}
			}
			else
				throw new XmlException("Blank SessionStatus in Xml", null, XmlReader.LineNumber, XmlReader.LinePosition);
		}

		public static WDEDataType GetDataType(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = Utils.GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "dt", "DataType",
					XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDEDataType) Enum.Parse(typeof(WDEDataType), temp, true);
				}
				catch
				{
					return WDEDataType.Text;
				}
			}
			else
				return WDEDataType.Text;
		}

		public static WDEFieldFlags GetFieldFlags(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = Utils.GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "ff", "FieldFlags",
					XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDEFieldFlags) Enum.Parse(typeof(WDEFieldFlags), temp, true);
				}
				catch
				{
					return WDEFieldFlags.None;
				}
			}
			else
				return WDEFieldFlags.None;
		}

		public static WDEMiscFlags GetMiscFlags(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = Utils.GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "mf", "MiscFlags",
					XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDEMiscFlags) Enum.Parse(typeof(WDEMiscFlags), temp, true);
				}
				catch
				{
					return WDEMiscFlags.None;
				}
			}
			else
				return WDEMiscFlags.None;	
		}

		

		public static WDEProjectOption GetProjectOption(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = Utils.GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "po", "ProjectOptions",
					XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDEProjectOption) Enum.Parse(typeof(WDEProjectOption), temp, true);
				}
				catch
				{
					return WDEProjectOption.None;
				}
			}
			else
				return WDEProjectOption.None;
		}

		public static WDEFieldStatus GetFieldStatus(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "fs", "Status", 
					XmlReader.LineNumber, XmlReader.LinePosition);

				if(temp == "QIVerified")
					temp = "Verified";
				if(temp == "QCVerified")
					temp = "Verified";
				if(temp == "Rejected")
					temp = "Flagged";

				try
				{
					return (WDEFieldStatus) Enum.Parse(typeof(WDEFieldStatus), temp, true);
				}
				catch
				{
					return WDEFieldStatus.None;
				}
			}
			else
				return WDEFieldStatus.None;
		}

		public static int GetIntValue(XmlTextReader XmlReader, string AttributeName, int defaultValue)
		{
			string temp = Utils.GetAttribute(XmlReader, AttributeName, "");
			return StrToIntDef(temp, defaultValue);
		}

        public static int GetIntValue(XmlTextReader XmlReader, string AttributeName)
        {
            return GetIntValue(XmlReader, AttributeName, 0);
        }

		public static void SetStatusByDataOwner(string DataOwner, ref WDEFieldStatus Status)
		{
			if(DataOwner == null)
				throw new ArgumentNullException("DataOwner", "DataOwner cannot be null");

			switch(DataOwner)
			{
				case "doPgm":
					Status = WDEFieldStatus.None;
					break;
				case "doEdit":
					Status = WDEFieldStatus.Plugged;
					break;
				case "doKeyer":
					if (Status == WDEFieldStatus.None)
						Status = WDEFieldStatus.Keyed;
					break;
				case "doScript":
					Status = WDEFieldStatus.Plugged;
					break;
				case "doSys":
					Status = WDEFieldStatus.None;
					break;
			}
		}

		public static Color GetColor(XmlTextReader XmlReader, string AttributeName, Color Default)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "cl", "Color",
					XmlReader.LineNumber, XmlReader.LinePosition);

				temp = ConvertDelphiKnownColor(temp);

				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
				try
				{
					return (Color) converter.ConvertFromString(temp);
				}
				catch
				{
					return Default;
				}
			}
			else
				return Default;
		}

		public static double GetDoubleValue(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = Utils.GetAttribute(XmlReader, AttributeName, "");
			double result = 0;
			if(double.TryParse(temp, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out result))
				return result;
			else
				return 0;
		}

		public static char GetCharValue(XmlTextReader XmlReader, string AttributeName, char DefaultValue)
		{
			string temp = Utils.GetAttribute(XmlReader, AttributeName, "");
			if(temp == "")
				return DefaultValue;
			else
				return char.Parse(temp);
		}

		public static string DateTimeToISO(DateTime Date)
		{
			return Date.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
		}

		public static DateTime ISOToDateTime(string ISOTime)
		{
			if(ISOTime == null)
				throw new ArgumentNullException("ISOTime","ISOTime cannot be null");

			int p = ISOTime.IndexOf(".");
			if(p == -1)
				throw new FormatException("Invalid date time format: " + ISOTime);

			p += 4;
			if(p >= ISOTime.Length)
				throw new FormatException("Invalid date time format: " + ISOTime);

			string tz = ISOTime.Substring(p);
			if(string.Compare(tz, "GMT", true) == 0)
				tz = "+00:00";
			ISOTime = ISOTime.Substring(0, p);
			return DateTime.Parse(ISOTime + tz, CultureInfo.InvariantCulture);
		}

		public static DateTime USStrToDateTime(string USTime)
		{
			if(USTime == null)
				throw new ArgumentNullException("USTime", "USTime cannot be null");

			IFormatProvider prov = new System.Globalization.CultureInfo("en-US");
			return DateTime.Parse(USTime, prov);
		}

		public static void CopyStream(Stream InStream, Stream OutStream)
		{
			byte[] buf = new byte[DEFAULT_BUFFER_SIZE];
			int len = 0;
			while((len = InStream.Read(buf, 0, DEFAULT_BUFFER_SIZE)) != 0)
				OutStream.Write(buf, 0, len);
            OutStream.Flush();
		}

		public static int StrToIntDef(string val, int def)
		{
			double res;
			if(Double.TryParse(val, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out res))
				return (int) res;
			else
				return def;
		}

		public static bool CheckEnumFlag(int val, int flag)
		{
			return (val & flag) == flag;
		}

		public static WDEEditErrorType GetEditErrorType(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "et", "EditErrorType",
					XmlReader.LineNumber, XmlReader.LinePosition);
				
				try
				{
					return (WDEEditErrorType) Enum.Parse(typeof(WDEEditErrorType), temp, true);
				}
				catch
				{
					return WDEEditErrorType.Failure;
				}
			}
			else
				return WDEEditErrorType.Failure;
		}

		public static WDESessionOption GetSessionOption(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "vo", "SessionOption", 
					XmlReader.LineNumber, XmlReader.LinePosition);
				
				try
				{
					return (WDESessionOption) Enum.Parse(typeof(WDESessionOption), temp, true);
				}
				catch
				{
					return WDESessionOption.None;
				}
			}
			else
				return WDESessionOption.None;
		}

		public static WDEExecuteOption GetExecuteOptions(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "ex", "ExecuteOption", 
					XmlReader.LineNumber, XmlReader.LinePosition);
				
				try
				{
					return (WDEExecuteOption) Enum.Parse(typeof(WDEExecuteOption), temp, true);
				}
				catch
				{
					return WDEExecuteOption.Validate;
				}
			}
			else
				return WDEExecuteOption.Validate;
		}

		public static WDETableLookupOption GetTableLookupOptions(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "lo", "TableLookupOption", 
					XmlReader.LineNumber, XmlReader.LinePosition);
				
				try
				{
					return (WDETableLookupOption) Enum.Parse(typeof(WDETableLookupOption), temp, true);
				}
				catch
				{
					return WDETableLookupOption.None;
				}
			}
			else
				return WDETableLookupOption.None;
		}

		public static WDELookupResultFieldOption GetLookupResultOptions(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "rf", "LookupResultFieldOptions", 
					XmlReader.LineNumber, XmlReader.LinePosition);
				
				try
				{
					return (WDELookupResultFieldOption) Enum.Parse(typeof(WDELookupResultFieldOption), temp, true);
				}
				catch
				{
					return WDELookupResultFieldOption.None;
				}
			}
			else
				return WDELookupResultFieldOption.None;
		}

		public static WDESessionOption GetSessionOptions(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "vo", "SessionOptions", XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDESessionOption) Enum.Parse(typeof(WDESessionOption), temp, true);
				}
				catch
				{
					return WDESessionOption.None;
				}
			}
			else
				return WDESessionOption.None;
		}

		public static WDEImageScale GetImageScale(XmlTextReader XmlReader, string AttributeName, WDEImageScale Default)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "sm", "ImageScale", XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDEImageScale) Enum.Parse(typeof(WDEImageScale), temp, true);
				}
				catch
				{
					return Default;
				}
			}
			else
				return Default;
		}

		public static WDESessionType GetSessionType(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "vm", "SessionType", XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDESessionType) Enum.Parse(typeof(WDESessionType), temp, true);
				}
				catch
				{
					return WDESessionType.None;
				}
			}
			else
				return WDESessionType.None;
		}

		public static WDESessionStyle GetSessionStyle(XmlTextReader XmlReader, string AttributeName, WDESessionStyle Default)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "vs", "SessionStyle",
					XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDESessionStyle) Enum.Parse(typeof(WDESessionStyle), temp, true);
				}
				catch
				{
					return Default;
				}
			}
			else
				return Default;
		}

		public static WDEPSOrientation GetPSOrientation(XmlTextReader XmlReader, string AttributeName, WDEPSOrientation Default)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "ps", "Orientation", 
					XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDEPSOrientation) Enum.Parse(typeof(WDEPSOrientation), temp, true);
				}
				catch
				{
					return Default;
				}
			}
			else
				return Default;
		}

		public static WDECheckDigitMethods GetCheckDigitMethods(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "cd", "CheckDigitMethod", XmlReader.LineNumber, 
					XmlReader.LinePosition);

				string[] t = temp.Split(',');
				ArrayList al = new ArrayList();
				foreach(string val in t)
				{
					string s = val.Trim();
					if((string.Compare("Mod7", s, true) != 0) &&
						(string.Compare("SIN", s, true) != 0))
						al.Add(s);
				}
				t = new string[al.Count];
				al.CopyTo(t);
				temp = string.Join(", ", t);

				try
				{
					return (WDECheckDigitMethods) Enum.Parse(typeof(WDECheckDigitMethods), temp, true);
				}
				catch
				{
					return WDECheckDigitMethods.None;
				}
			}
			else
				return WDECheckDigitMethods.None;
		}

		public static Font GetFont(XmlTextReader XmlReader, string AttributeName, string Default)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp == "")
				temp = Default;

            return StringToFont(temp);
		}

        public static Font StringToFont(string fontString)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
            return (Font)converter.ConvertFromInvariantString(fontString);  
        }

		public static string FontToString(Font aFont)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
            return converter.ConvertToInvariantString(aFont);   
		}

		public static ContentAlignment GetContentAlignment(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "al", "Alignment", XmlReader.LineNumber, XmlReader.LinePosition);
				if(temp.EndsWith("Justify"))
				{
					temp = temp.Replace("Justify", "Center");
					if(temp.StartsWith("Center"))
					{
						if(temp.Length > 6)
							temp = temp.Substring(6);
						else
							temp = "";

						temp = "Middle" + temp;
					}
				}

				try
				{
					return (ContentAlignment) Enum.Parse(typeof(ContentAlignment), temp, true);
				}
				catch
				{
					return ContentAlignment.MiddleRight;
				}
			}
			else
				return ContentAlignment.MiddleRight;
		}

		public static WDEControlOption GetControlOption(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "op", "ControlOption", XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDEControlOption) Enum.Parse(typeof(WDEControlOption), temp, true);
				}
				catch
				{
					return WDEControlOption.None;
				}
			}
			else
				return WDEControlOption.None;
		}

		public static WDEGroupBoxOption GetGroupBoxOption(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "gp", "GroupBoxOption", XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDEGroupBoxOption) Enum.Parse(typeof(WDEGroupBoxOption), temp, true);
				}
				catch
				{
					return WDEGroupBoxOption.None;
				}
			}
			else
				return WDEGroupBoxOption.None;
		}

		public static WDEDetailGridOption GetDetailGridOption(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "dg", "DetailGridOption", XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDEDetailGridOption) Enum.Parse(typeof(WDEDetailGridOption), temp, true);
				}
				catch
				{
					return WDEDetailGridOption.None;
				}
			}
			else
				return WDEDetailGridOption.None;
		}

		public static WDERecordNumberPosition GetRecordNumberPosition(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "sr", "RecordNumberPosition", XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDERecordNumberPosition) Enum.Parse(typeof(WDERecordNumberPosition), temp, true);
				}
				catch
				{
					return WDERecordNumberPosition.Right;
				}
			}
			else
				return WDERecordNumberPosition.Right;
		}

		public static WDEOCRRepairMode GetRepairMode(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "rm", "OCRRepairMode", XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDEOCRRepairMode) Enum.Parse(typeof(WDEOCRRepairMode), temp, true);
				}
				catch
				{
					return WDEOCRRepairMode.None;
				}
			}
			else
				return WDEOCRRepairMode.None;
		}

		public static WDEFieldOption GetFieldOption(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				// no prefix on FieldOption

				try
				{
					return (WDEFieldOption) Enum.Parse(typeof(WDEFieldOption), temp, true);
				}
				catch
				{
					return WDEFieldOption.None;
				}
			}
			else
				return WDEFieldOption.None;
		}

		public static WDEScriptLanguage GetScriptLanguage(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "lt", "ScriptLanguage", XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDEScriptLanguage) Enum.Parse(typeof(WDEScriptLanguage), temp, true);
				}
				catch
				{
					return WDEScriptLanguage.CSharpNet;
				}
			}
			else
				return WDEScriptLanguage.CSharpNet;
		}

        public static WDEKeyMode GetKeyMode(XmlTextReader XmlReader, string AttributeName)
        {
            string temp = GetAttribute(XmlReader, AttributeName, "");
            if (temp != "")
            {
                try
                {
                    return (WDEKeyMode)Enum.Parse(typeof(WDEKeyMode), temp, true);
                }
                catch
                {
                    return WDEKeyMode.Normal;
                }
            }
            else
                return WDEKeyMode.Normal;
        }

		public static WDEQIOption GetQIOption(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				temp = TrimPrefix(temp, "qi", "QIOption", XmlReader.LineNumber, XmlReader.LinePosition);

				try
				{
					return (WDEQIOption) Enum.Parse(typeof(WDEQIOption), temp, true);
				}
				catch
				{
					return WDEQIOption.NonCritical;
				}
			}
			else
				return WDEQIOption.NonCritical;
		}

        public static PluginDocking GetPluginDocking(XmlTextReader XmlReader, string AttributeName)
        {
            string temp = GetAttribute(XmlReader, AttributeName, "");
            if (temp != "")
            {
                try
                {
                    return (PluginDocking)Enum.Parse(typeof(PluginDocking), temp, true);
                }
                catch
                {
                    return PluginDocking.Right;
                }
            }
            else
                return PluginDocking.Right;
        }

		public static float ConvertFontHeight(int OldHeight)
		{
			using(Control c = new System.Windows.Forms.Control())
			{
				using(Graphics g = c.CreateGraphics())
				{
					float logpixelsy = g.DpiY;

					float res = (OldHeight * 72) / logpixelsy;
					return (float) -Math.Round(res);
				}
			}
		}

		public static FontStyle ConvertFontStyle(string OldStyle, int ErrLine, int ErrPos)
		{
			if(OldStyle == null)
				throw new ArgumentNullException("OldStyle", "OldStyle cannot be null");

			//fsBold,fsItalic,fsUnderline,fsStrikeOut
			if(OldStyle != "")
			{
				OldStyle = TrimPrefix(OldStyle, "fs", "FontStyle", ErrLine, ErrPos);

				try
				{
					return (FontStyle) Enum.Parse(typeof(FontStyle), OldStyle, true);
				}
				catch
				{
					return FontStyle.Regular;
				}
			}
			else
				return FontStyle.Regular;
		}

		public static WDEZipLookupOption GetZipLookupOptions(XmlTextReader XmlReader, string AttributeName)
		{
			string temp = GetAttribute(XmlReader, AttributeName, "");
			if(temp != "")
			{
				try
				{
					return (WDEZipLookupOption) Enum.Parse(typeof(WDEZipLookupOption), temp, true);
				}
				catch
				{
					return WDEZipLookupOption.None;
				}
			}
			else
				return WDEZipLookupOption.None;
		}

		public static void ConvertBitmapFont(ref string name, ref float height)
		{
			switch(name.ToUpper())
			{
				case "FIXEDSYS":
					name = "Lucida Console";
					height = 10f;
					break;
				case "COURIER":
					name = "Courier New";
					height = 10f;
					break;
				case "TERMINAL":
					name = "Lucida Console";
					height = height % 2 == 0 ? height : height - 1;
					break;
				case "MS SERIF":
				case "MS SANS SERIF":
					name = "Microsoft Sans Serif";
					height = height % 2 == 0 ? height : height - 1;
					break;
				case "MODERN":
				case "ROMAN":
				case "SCRIPT":
				case "SMALL FONTS":
				case "SYSTEM":
					name = "Lucida Console";
					height = 9f;
					break;
			}
		}

        public static WDEErrorOverrideType GetErrorOverrideType(XmlTextReader XmlReader, string AttributeName)
        {
            string temp = GetAttribute(XmlReader, AttributeName, "");
            if (temp != "")
            {
                try
                {
                    return (WDEErrorOverrideType)Enum.Parse(typeof(WDEErrorOverrideType), temp, true);
                }
                catch
                {
                    return WDEErrorOverrideType.None;
                }
            }
            else
                return WDEErrorOverrideType.None;
        }

        public static ToolbarDock GetToolbarDock(XmlTextReader XmlReader, string AttributeName)
        {
            string temp = GetAttribute(XmlReader, AttributeName, "");

            if (temp != "")
            {
                try
                {
                    return (ToolbarDock)Enum.Parse(typeof(ToolbarDock), temp, true);
                }
                catch
                {
                    return ToolbarDock.Right;
                }
            }
            else
                return ToolbarDock.Right;
        }

        public static ImageDock GetImageDock(XmlTextReader XmlReader, string AttributeName)
        {
            string temp = GetAttribute(XmlReader, AttributeName, "");

            if (temp != "")
            {
                try
                {
                    return (ImageDock)Enum.Parse(typeof(ImageDock), temp, true);
                }
                catch
                {
                    return ImageDock.Top;
                }
            }
            else
                return ImageDock.Top;
        }

	}
}
