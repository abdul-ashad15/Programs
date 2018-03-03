using System;
using System.Collections;
using System.Text;

namespace WebDX.Api
{
	public class WDECharSet
	{
		private enum ParserState {Normal, Range, Comma};

		private	ArrayList m_Def;
		private string m_DefString;

		public WDECharSet(string CharSetDef)
		{
			if(CharSetDef == null)
				throw new ArgumentNullException("CharSetDef","CharSetDef cannot be null");

			m_DefString = "";
			m_Def = new ArrayList();
			SetDef(CharSetDef);
		}

		public string CharSet
		{
			get
			{
				return m_DefString;
			}

			set
			{
				SetDef(value);
			}
		}

		public bool IsValidChar(char Value)
		{
			if(m_DefString == "")
				return true;
			else
				return m_Def.Contains(Value);
		}

		public string Filter(string Value)
		{
			if(Value == null)
				throw new ArgumentNullException("Value", "Value cannot be null");

			if(m_DefString == "")
				return Value;

			StringBuilder sb = new StringBuilder(Value.Length);
			for(int i = 0; i < Value.Length; i++)
				if(IsValidChar(Value[i]))
					sb.Append(Value[i]);

			return sb.ToString();
		}

		public bool IsValidString(string Value)
		{
			if(Value == null)
                throw new ArgumentNullException("Value", "Value cannot be null");

			return IsValidString(Value, Value.Length);	
		}

		public bool IsValidString(string Value, int MaxLen)
		{
			if(Value == null)
				throw new ArgumentNullException("Value", "Value cannot be null");

			if(m_DefString == "")
				return true;

			int l = Math.Min(Value.Length, MaxLen);
			for(int i = 0; i < l; i++)
				if(!IsValidChar(Value[i]))
					return false;
			return true;
		}

		private void SetDef(string Value)
		{
			if(Value == null)
				throw new ArgumentNullException("Value", "Value cannot be null");

			if(Value != m_DefString)
			{
				m_Def.Clear();
				Stack s = new Stack();
				ParserState state = ParserState.Normal;
				foreach(char c in Value)
				{
					switch(c)
					{
						case ',':
							if(state == ParserState.Comma)
							{
								m_Def.Add(c);
								if(s.Count > 0)
									s.Pop();
								state = ParserState.Normal;
							}
							else
								state = ParserState.Comma;
							break;
						case '-':
							if((state == ParserState.Comma) || (state == ParserState.Range))
							{
								m_Def.Add(c);
								if(s.Count > 0)
									s.Pop();
								state = ParserState.Normal;
							}
							else
							{
								state = ParserState.Range;
							}
							break;
						default:
							if(state == ParserState.Range)
							{
								s.Pop();
								if(s.Count == 0)
									throw new FormatException("Range without begin value in CharSet");

								char RangeChar = (char) s.Pop();
								int low = Convert.ToInt32(RangeChar);
								int high = Convert.ToInt32(c);
								if(low > high)
									throw new FormatException(string.Format("Invalid range in CharSet \"{0}\"", new object[] { RangeChar.ToString() + "-" + c.ToString() }));

								for(int i = Convert.ToInt32(RangeChar); i <= Convert.ToInt32(c); i++)
									m_Def.Add(Convert.ToChar(i));
								state = ParserState.Normal;
							}
							else
							{
								if((s.Count != 0) && (state != ParserState.Comma))
									throw new FormatException(string.Format("Invalid CharSet \"{0}\"", new object[] { Value }));

								m_Def.Add(c);
								if(s.Count > 0)
									s.Pop();
								state = ParserState.Normal;
							}
							break;
					}
					s.Push(c);
				}
				if(state == ParserState.Range)
				{
					if(Value.Length == 1)
						m_Def.Add('-');
					else
						throw new FormatException("Unterminated range in CharSet");
				}

				if((state == ParserState.Comma) && (Value.Length == 1))
					m_Def.Add(',');

				m_DefString = Value;
			}
		}
	}
}
