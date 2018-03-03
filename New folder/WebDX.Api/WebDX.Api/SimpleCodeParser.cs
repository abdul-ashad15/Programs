using System;
using System.Collections;
using System.Text;

namespace WebDX.Api
{
	public enum SCPCommentType {Single, Multi, Borland};
	public enum SCPTokenType {MethodDecl, Comment, Identifier, Whitespace, Symbol, String};

	/// <summary>
	/// Parse source code to replace method names
	/// </summary>
	public abstract class SimpleCodeParser
	{
		private SCPCommentType m_CommentType;
		private SCPTokenType m_TokenType;
		private bool m_EOF;
		private string m_ScriptText;
		private int m_LinePos;
		private int m_CharPos;
		private int m_LineNum;
		private int m_TokenPos;
		private int m_TokenLine;
		private int m_AbsTokenPos;
		private string[] m_MethodDeclNames;
		private string[] m_StringTerminators;
		private char m_StringBeginChar;

		public SimpleCodeParser()
		{
			InitParser();
		}

		public string ScriptText
		{
			get
			{
				return m_ScriptText;
			}
			set
			{
				if(value == null)
					throw new Exception("Property ScriptText cannot be null");
				m_ScriptText = value;
				ResetParser();
			}
		}

		public bool EOF
		{
			get
			{
				return m_EOF;
			}
		}

		public SCPTokenType TokenType
		{
			get
			{
				return m_TokenType;
			}
		}

		public int TokenPos
		{
			get
			{
				return m_TokenPos;
			}
		}

		public int TokenLine
		{
			get
			{
				return m_TokenLine;
			}
		}

		public void ReplaceMethodDecl(string replaceText)
		{
			if(m_ScriptText != "")
			{
				ResetParser();

				bool methodFound = false;
				bool replaced = false;
				while (!EOF && !replaced)
				{
					string tokString = GetNextToken();
					
					switch(TokenType)
					{
						case SCPTokenType.MethodDecl:
							methodFound = true;
							break;
						case SCPTokenType.Identifier:
							if(methodFound)
							{
								m_ScriptText = m_ScriptText.Substring(0, m_AbsTokenPos) + replaceText + 
									m_ScriptText.Substring(m_AbsTokenPos + tokString.Length);
								replaced = true;
							}
							break;
					}
				}
			}
		}

		public string GetNextToken()
		{
			StringBuilder result = new StringBuilder();
			bool tokenFound = false;

			while (!m_EOF && !tokenFound)
			{
				char curChar = GetNextChar();
				result.Append(curChar);
				if(char.IsWhiteSpace(curChar))
				{
					m_AbsTokenPos = m_CharPos;
					m_TokenPos = m_LinePos;
					m_TokenLine = m_LineNum;
					result.Append(SlurpWhiteSpace());
					m_TokenType = SCPTokenType.Whitespace;
					tokenFound = true;
				}
				else
				{
					switch(curChar)
					{
						case '`':
						case '~':
						case '@':
						case '#':
						case '$':
						case '%':
						case '^':
						case '&':
						case '*':
						case '-':
						case '+':
						case '[':
						case ']':
						case '|':
						case '\\':
						case ':':
						case ';':
						case '\"':
						case '<':
						case '>':
						case ',':
						case '.':
						case '?':
						case '}':
						case ')':
						case '\'':
						case '/':
						case '{':
						case '(':
						case '=':
							if(CheckComment(curChar))
							{
								m_AbsTokenPos = m_CharPos;
								m_TokenPos = m_LinePos;
								m_TokenLine = m_LineNum;
								result.Append(SlurpComment());
								m_TokenType = SCPTokenType.Comment;
								tokenFound = true;
							}
							else if(CheckString(curChar))
							{
								m_StringBeginChar = curChar;
								m_AbsTokenPos = m_CharPos;
								m_TokenPos = m_LinePos;
								m_TokenLine = m_LineNum;
								result.Remove(0, result.Length);
								result.Append(SlurpString());
								m_TokenType = SCPTokenType.String;
								tokenFound = true;
							}
							else
							{
								m_AbsTokenPos = m_CharPos;
								m_TokenPos = m_LinePos;
								m_TokenLine = m_LineNum;
								m_TokenType = SCPTokenType.Symbol;
								tokenFound = true;
							}
							break;
						default:
							if(CheckComment(curChar))
							{
								m_AbsTokenPos = m_CharPos;
								m_TokenPos = m_LinePos;
								m_TokenLine = m_LineNum;
								result.Append(SlurpComment());
								m_TokenType = SCPTokenType.Comment;
								tokenFound = true;
							}
							else
							{
								m_AbsTokenPos = m_CharPos;
								m_TokenPos = m_LinePos;
								m_TokenLine = m_LineNum;
								result.Append(SlurpIdent(curChar));
								tokenFound = true;
							}
							break;
					}
				}
			}
			return result.ToString();
		}

		protected abstract void InitParser();
		protected abstract bool CheckComment(char CurChar);
		protected abstract string SlurpComment();

		protected virtual bool CheckString(char CurChar)
		{
			foreach(string Term in m_StringTerminators)
			{
				if(Term.StartsWith(char.ToString(CurChar)))
					return true;
			}
			return false;
		}

		protected virtual void ResetParser()
		{
			m_LinePos = 0;
			m_LineNum = 0;
			m_CharPos = -1;
			m_AbsTokenPos = -1;
			m_TokenPos = 0;
			m_TokenLine = 0;
			m_EOF = false;
		}

		protected virtual bool StringTerminate()
		{
			return m_EOF;
		}

		protected string[] MethodDeclNames
		{
			get
			{
				return m_MethodDeclNames;
			}
			set
			{
				m_MethodDeclNames = value;
			}
		}

		protected string[] StringTerminators
		{
			get
			{
				return m_StringTerminators;
			}
			set
			{
				m_StringTerminators = value;
			}
		}

		protected SCPCommentType CommentType
		{
			get
			{
				return m_CommentType;
			}
			set
			{
				m_CommentType = value;
			}
		}

		protected int LineNum
		{
			get
			{
				return m_LineNum;
			}
		}

		protected int CharPos
		{
			get
			{
				return m_CharPos;
			}
		}

		protected char PeekNextChar()
		{
			int tempPos = m_CharPos + 1;
			if(tempPos < m_ScriptText.Length)
			{
				return m_ScriptText[tempPos];
			}
			else
				return Char.MinValue;
		}

		protected char GetNextChar()
		{
			m_CharPos++;
			m_LinePos++;
			if(m_CharPos < m_ScriptText.Length)
			{
				if(AtEol())
				{
					if(m_ScriptText[m_CharPos] == Environment.NewLine[0])
					{
						m_LineNum++;
						m_LinePos = 0;
					}
				}
				return m_ScriptText[m_CharPos];
			}
			else
			{
				m_EOF = true;
				return Char.MinValue;
			}
		}

		protected bool AtEol()
		{
			if((m_CharPos < m_ScriptText.Length) && (Environment.NewLine.IndexOf(m_ScriptText[m_CharPos]) > -1))
				return true;
			else
				return false;
		}

		private bool IsMethodDecl(string Ident)
		{
			foreach(string Decl in m_MethodDeclNames)
			{
				if(string.Compare(Decl, Ident, true) == 0)
					return true;
			}
			return false;
		}

		private string SlurpWhiteSpace()
		{
			StringBuilder sb = new StringBuilder();
			while((!m_EOF) && (Char.IsWhiteSpace(PeekNextChar())))
				sb.Append(GetNextChar());
			m_TokenType = SCPTokenType.Whitespace;
			return sb.ToString();
		}

		private string SlurpIdent(char startChar)
		{
			StringBuilder sb = new StringBuilder();
			char next = PeekNextChar();
			while((!m_EOF) && (Char.IsLetterOrDigit(next) ||
				(next == '_')))
			{
				sb.Append(GetNextChar());
				next = PeekNextChar();
			}

			if(IsMethodDecl(startChar.ToString() + sb.ToString()))
				m_TokenType = SCPTokenType.MethodDecl;
			else
				m_TokenType = SCPTokenType.Identifier;
			return sb.ToString();
		}

		private string SlurpString()
		{
			StringBuilder sb = new StringBuilder();
			while((!StringTerminate()) && (PeekNextChar() != m_StringBeginChar))
				sb.Append(GetNextChar());

			if(PeekNextChar() == m_StringBeginChar)
				sb.Append(GetNextChar());

			m_TokenType = SCPTokenType.String;
			return sb.ToString();
		}
	}

	public class SimpleDelphiParser : SimpleCodeParser
	{
		public SimpleDelphiParser() : base()
		{
		}

		protected override void InitParser()
		{
			MethodDeclNames = new string[] {"function", "procedure"};
			StringTerminators = new string[] {"'"};
		}


		protected override bool CheckComment(char CurChar)
		{
			char nextChar = this.PeekNextChar();
			switch(CurChar)
			{
				case '/':
					if(nextChar == '/')
					{
						CommentType = SCPCommentType.Single;
						return true;
					}
					else
						return false;
				case '(':
					if(nextChar == '*')
					{
						CommentType = SCPCommentType.Borland;
						return true;
					}
					else
						return false;
				case '{':
					CommentType = SCPCommentType.Multi;
					return true;
				default:
					return false;
			}
		}

		protected override string SlurpComment()
		{
			StringBuilder result = new StringBuilder();

			int lastLine = LineNum;
			bool foundEnd = false;
			while (!EOF && !foundEnd)
			{
				char curChar = GetNextChar();
				if(lastLine != LineNum)
				{
					if(CommentType != SCPCommentType.Single)
					{
						result.Append(Environment.NewLine);
						lastLine = LineNum;
					}
					else
						break;
				}

				result.Append(curChar);
				switch(CommentType)
				{
					case SCPCommentType.Single:
						if(AtEol())
							foundEnd = true;
						break;
					case SCPCommentType.Multi:
						if(curChar == '}')
							foundEnd = true;
						break;
					case SCPCommentType.Borland:
						if((curChar == '*') && (PeekNextChar() == ')'))
						{
							result.Append(GetNextChar());
							foundEnd = true;
						}
						break;
				}
			}
			return result.ToString();
		}
	}

	public class SimpleVBParser : SimpleCodeParser
	{
		public SimpleVBParser() : base()
		{
		}

		protected override void InitParser()
		{
			MethodDeclNames = new string[] {"sub", "function"};
			StringTerminators = new string[] {"\""};
		}

		protected override bool CheckComment(char CurChar)
		{
			if(char.ToUpper(CurChar) == 'R')
			{
				if(CharPos < (ScriptText.Length - 2))
				{
					if(string.Compare(ScriptText.Substring(CharPos, 3), "REM", true) == 0)
					{
						CommentType = SCPCommentType.Single;
						return true;
					}
					else
						return false;
				}
				else
					return false;
			}
			else if(char.ToUpper(CurChar) == '\'')
			{
				CommentType = SCPCommentType.Single;
				return false;
			}
			else
				return false;
		}

		protected override string SlurpComment()
		{
			StringBuilder result = new StringBuilder();
			while(!AtEol())
				result.Append(GetNextChar());
			return result.ToString();
		}

		protected override bool StringTerminate()
		{
			return AtEol();
		}
	}

	public class SimpleJScriptParser : SimpleCodeParser
	{
		public SimpleJScriptParser() : base()
		{
		}

		protected override void InitParser()
		{
			MethodDeclNames = new string[] {"function"};
			StringTerminators = new string[] {"'", "\""};
		}

		protected override bool CheckComment(char CurChar)
		{
			char nextChar = PeekNextChar();
			if(CurChar == '/')
			{
				if(nextChar == '/')
				{
					CommentType = SCPCommentType.Single;
					return true;
				}
				else if(nextChar == '*')
				{
					CommentType = SCPCommentType.Multi;
					return true;
				}
				else
					return false;
			}
			else
				return false;
		}

		protected override string SlurpComment()
		{
			StringBuilder result = new StringBuilder();
			int lastLine = LineNum;

			bool foundEnd = false;
			while(!EOF && !foundEnd)
			{
				char curChar = GetNextChar();
				if(lastLine != LineNum)
				{
					result.Append(Environment.NewLine);
					lastLine = LineNum;
				}
				result.Append(curChar);

				switch(CommentType)
				{
					case SCPCommentType.Single:
						if(AtEol())
							foundEnd = true;
						break;
					case SCPCommentType.Multi:
						if((curChar == '*') && (PeekNextChar() == '/'))
						{
							result.Append(GetNextChar());
							foundEnd = true;
						}
						break;
				}
			}
			return result.ToString();
		}
	}
}
