using System;
using System.Resources;
using System.Text;

namespace WebDX.Api
{
	/// <summary>
	/// An exception that originates in the WebDX API.
	/// </summary>
	[Serializable]
	public class WDEException : System.ApplicationException
	{
		private string m_Message;
		private string m_ExtraInfo;
		private string m_ErrorCode;

		public WDEException(string errorCode) : base(string.Empty)
		{
			Init(errorCode, null);
		}

		public WDEException(string errorCode, object[] messageParams) : base(string.Empty)
		{
			Init(errorCode, messageParams);
		}

		public WDEException(string errorCode, Exception innerException) : base(string.Empty, innerException)
		{
			Init(errorCode, null);
		}

		public WDEException(string errorCode, object[] messageParams, Exception innerException) : base(string.Empty, innerException)
		{
			Init(errorCode, messageParams);
		}

        public WDEException(System.Runtime.Serialization.SerializationInfo info, 
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            m_Message = info.GetString("__Message");
            m_ExtraInfo = info.GetString("ExtraInfo");
            m_ErrorCode = info.GetString("ErrorCode");
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("__Message", m_Message);
            info.AddValue("ExtraInfo", m_ExtraInfo);
            info.AddValue("ErrorCode", m_ErrorCode);
        }

		public override string Message
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(m_ErrorCode + ": " + m_Message + Environment.NewLine);
				sb.Append(m_ExtraInfo);
				return sb.ToString();
			}
		}

		public string ShortMessage
		{
			get
			{
				return m_Message;
			}
		}

		public string ExtraInfo
		{
			get
			{
				return m_ExtraInfo;
			}
		}

		public string ErrorCode
		{
			get
			{
				return m_ErrorCode;
			}
		}

		public override string ToString()
		{
			return Message + Environment.NewLine + Environment.NewLine +
				"Stack Trace:" + Environment.NewLine +
				base.StackTrace;
		}

		private void Init(string errorCode, object[] messageParams)
		{
			m_ErrorCode = errorCode;

            ResourceManager resMan = WDEExceptionResources.ResourceManager;            
			string temp = resMan.GetString(errorCode);
			string[] tempList = null;
			if(temp == null)
			{
                temp = resMan.GetString("UNKNOWN");
				tempList = temp.Split('|');
				m_Message = tempList[0];
				m_ExtraInfo = tempList[1];
			}
			else
			{
				tempList = temp.Split('|');
				if(tempList.Length > 0)
					m_ExtraInfo = tempList[1];
				else
					m_ExtraInfo = string.Empty;

				m_Message = tempList[0];
			}

			if(m_ExtraInfo != string.Empty)
			{
				m_ExtraInfo = m_ExtraInfo.Replace(@"\n", Environment.NewLine);
			}

			if(messageParams != null)
			{
				m_Message = string.Format(m_Message, messageParams);
				m_ExtraInfo = string.Format(m_ExtraInfo, messageParams);
			}
		}
	}
}
