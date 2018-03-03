//#define FORCE_LOGGING

using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Permissions;
using System.IO;
using System.Text;

namespace WebDX.Api
{
	/// <summary>
	/// Manages application logging for WebDX
	/// </summary>
	internal class TraceLogger
	{
		/*
		 * Log columns:
		 * 
		 * TimeStamp,ThreadID,EventType,Message
		 */
		private static TraceLogger m_Log;
		private bool m_ReflectionAllowed;

		private TraceLogger()
		{
			try
			{
				ReflectionPermission perm = new ReflectionPermission(ReflectionPermissionFlag.TypeInformation);
				perm.Demand();
				m_ReflectionAllowed = true;
			}
			catch
			{
				m_ReflectionAllowed = false;
				if(GetEnabled())
					Trace.WriteLine("Trace: Reflection not allowed. Only error and message information will be included.");
			}
		}

		/// <summary>
		/// Gets the system logger object
		/// </summary>
		public static TraceLogger Log
		{
			get
			{
				if(m_Log == null)
					m_Log = new TraceLogger();
				return m_Log;
			}
		}

		/// <summary>
		/// Traces the begining of method execution
		/// </summary>
		/// <param name="args">The list of arguments passed to the method except out parameters</param>
		public void BeginMethod(params object[] args)
		{
			if(GetEnabled())
			{
				if(m_ReflectionAllowed)
				{
					StackTrace st = new StackTrace();
					StackFrame sf = new StackFrame(1);
					MethodBase mb = sf.GetMethod();

					string message = "Begin: " + FormatMethodInfo(mb, args, false);
				
					if(mb.MemberType == MemberTypes.Method)
						LogMessage(TraceEventType.Method, message);
					else if(mb.MemberType == MemberTypes.Event)
						LogMessage(TraceEventType.Event, message);
					else if(mb.MemberType == MemberTypes.Property)
						LogMessage(TraceEventType.Property, message);
				}
			}
		}

		/// <summary>
		/// Traces the end of method execution
		/// </summary>
		/// <param name="args">The first parameter is the return value if applicable. All others are the arguments passed to the method including out parameters.</param>
		public void EndMethod(params object[] args)
		{
			if(GetEnabled())
			{
				if(m_ReflectionAllowed)
				{
					StackTrace st = new StackTrace();
					StackFrame sf = new StackFrame(1);
					MethodBase mb = sf.GetMethod();
					MethodInfo mi = (MethodInfo) mb;

					object[] args2 = null;

					StringBuilder sb = new StringBuilder();
					if(mi.ReturnType.UnderlyingSystemType != typeof(void))
					{
						string rv = "";
						object returnValue = null;
						if((args == null) || (args.Length == 0))
							returnValue = "<Not Logged>";
						else
							returnValue = args[0];
						if(mi.ReturnType.UnderlyingSystemType == typeof(string))
							rv = returnValue == null ? "<null>" : "\"" + returnValue.ToString() + "\"";
						else
							rv = returnValue == null ? "<null>" : returnValue.ToString();
					
						sb.Append(" Return: " + rv);
						
						if(args != null && args.Length > 1)
						{
							args2 = new object[args.Length - 1];
							for(int i = 1; i < args.Length; i++)
								args2[i - 1] = args[i];
						}
					}
					else
						args2 = args;

					string message = "End: " + FormatMethodInfo(mb, args2, true) + 
						sb.ToString();
				
					if(mb.MemberType == MemberTypes.Method)
						LogMessage(TraceEventType.Method, message);
					else if(mb.MemberType == MemberTypes.Event)
						LogMessage(TraceEventType.Event, message);
					else if(mb.MemberType == MemberTypes.Property)
						LogMessage(TraceEventType.Property, message);
				}
			}
		}

		/// <summary>
		/// Logs a message
		/// </summary>
		/// <param name="message">The message to log</param>
		public void Message(string message)
		{
			if(GetEnabled())
			{
				LogMessage(TraceEventType.Message, message);
			}
		}

		/// <summary>
		/// Logs a message if the condition is true.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="message"></param>
		public void MessageIf(bool condition, string message)
		{
			if(GetEnabled())
			{
				if(condition)
					LogMessage(TraceEventType.Conditional, message);
			}
		}

		/// <summary>
		/// Logs an error
		/// </summary>
		/// <param name="exception">The error to log</param>
		public void Error(Exception exception)
		{
			if(GetEnabled())
			{
				LogMessage(TraceEventType.Error, exception.ToString());
			}
		}

		private void LogMessage(TraceEventType eventType, string message)
		{
			const string SEPARATOR = "|";

			StringBuilder sb = new StringBuilder();
			sb.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fffffff") + SEPARATOR);
			sb.Append(System.Threading.Thread.CurrentThread.GetHashCode().ToString() + SEPARATOR);
			sb.Append(eventType.ToString() + SEPARATOR);
			sb.Append(message);
			Trace.WriteLine(sb.ToString().Replace(Environment.NewLine, ""));
		}

		private string FormatMethodInfo(MethodBase mb, object[] args, bool showOutArgs)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(mb.DeclaringType.Name + "." + mb.Name + "(");
			ParameterInfo[] p = mb.GetParameters();
			int j = 0;
			for(int i = 0; i < p.Length; i++)
			{
				sb.Append("[" + p[i].ParameterType.FullName + "] ");
				object obj = null;
				if(p[i].IsOut && !showOutArgs)
					sb.Append("<out>");
				else
				{
					if((args != null) && (j < args.Length))
					{
						obj = args[j++];

						if(obj == null)
							sb.Append("<null>");
						else if(obj is string)
							sb.Append("\"" + obj.ToString() + "\"");
						else
							sb.Append(obj.ToString());
					}
					else
						sb.Append("<not logged>");
				}

				if(i < p.Length - 1)
					sb.Append(", ");
			}

			sb.Append(")");

			return sb.ToString();
		}

		private bool GetEnabled()
		{
			TraceSwitch traceSwitch = new TraceSwitch("WebDXLog", "Tracing for WebDX");
#if FORCE_LOGGING
			traceSwitch.Level = TraceLevel.Verbose;
#endif
			return (traceSwitch.Level != TraceLevel.Off);
		}
	}

	public enum TraceEventType
	{
		Conditional, Error, Event, Message, Method, Property
	}
}
