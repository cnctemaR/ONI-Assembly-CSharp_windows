using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace System
{
	[ComDefaultInterface(typeof(_Exception))]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[Serializable]
	public class Exception : ISerializable, _Exception
	{
		public Exception()
		{
		}

		public Exception(string message)
		{
			this.message = message;
		}

		protected Exception(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.class_name = info.GetString("ClassName");
			this.message = info.GetString("Message");
			this.help_link = info.GetString("HelpURL");
			this.stack_trace = info.GetString("StackTraceString");
			this._remoteStackTraceString = info.GetString("RemoteStackTraceString");
			this.remote_stack_index = info.GetInt32("RemoteStackIndex");
			this.hresult = info.GetInt32("HResult");
			this.source = info.GetString("Source");
			this.inner_exception = (Exception)info.GetValue("InnerException", typeof(Exception));
			try
			{
				this._data = (IDictionary)info.GetValue("Data", typeof(IDictionary));
			}
			catch (SerializationException)
			{
			}
		}

		public Exception(string message, Exception innerException)
		{
			this.inner_exception = innerException;
			this.message = message;
		}

		public Exception InnerException
		{
			get
			{
				return this.inner_exception;
			}
		}

		public virtual string HelpLink
		{
			get
			{
				return this.help_link;
			}
			set
			{
				this.help_link = value;
			}
		}

		protected int HResult
		{
			get
			{
				return this.hresult;
			}
			set
			{
				this.hresult = value;
			}
		}

		internal void SetMessage(string s)
		{
			this.message = s;
		}

		internal void SetStackTrace(string s)
		{
			this.stack_trace = s;
		}

		private string ClassName
		{
			get
			{
				if (this.class_name == null)
				{
					this.class_name = this.GetType().ToString();
				}
				return this.class_name;
			}
		}

		public virtual string Message
		{
			get
			{
				if (this.message == null)
				{
					this.message = string.Format(Locale.GetText("Exception of type '{0}' was thrown."), this.ClassName);
				}
				return this.message;
			}
		}

		public virtual string Source
		{
			get
			{
				if (this.source == null)
				{
					StackTrace stackTrace = new StackTrace(this, true);
					if (stackTrace.FrameCount > 0)
					{
						StackFrame frame = stackTrace.GetFrame(0);
						if (stackTrace != null)
						{
							MethodBase method = frame.GetMethod();
							if (method != null)
							{
								this.source = method.DeclaringType.Assembly.UnprotectedGetName().Name;
							}
						}
					}
				}
				return this.source;
			}
			set
			{
				this.source = value;
			}
		}

		public virtual string StackTrace
		{
			get
			{
				if (this.stack_trace == null)
				{
					if (this.trace_ips == null)
					{
						return null;
					}
					StackTrace stackTrace = new StackTrace(this, 0, true, true);
					StringBuilder stringBuilder = new StringBuilder();
					string text = string.Format("{0}  {1} ", Environment.NewLine, Locale.GetText("at"));
					string text2 = Locale.GetText("<unknown method>");
					for (int i = 0; i < stackTrace.FrameCount; i++)
					{
						StackFrame frame = stackTrace.GetFrame(i);
						if (i == 0)
						{
							stringBuilder.AppendFormat("  {0} ", Locale.GetText("at"));
						}
						else
						{
							stringBuilder.Append(text);
						}
						if (frame.GetMethod() == null)
						{
							string internalMethodName = frame.GetInternalMethodName();
							if (internalMethodName != null)
							{
								stringBuilder.Append(internalMethodName);
							}
							else
							{
								stringBuilder.AppendFormat("<0x{0:x5}> {1}", frame.GetNativeOffset(), text2);
							}
						}
						else
						{
							this.GetFullNameForStackTrace(stringBuilder, frame.GetMethod());
							if (frame.GetILOffset() == -1)
							{
								stringBuilder.AppendFormat(" <0x{0:x5}> ", frame.GetNativeOffset());
							}
							else
							{
								stringBuilder.AppendFormat(" [0x{0:x5}] ", frame.GetILOffset());
							}
							stringBuilder.AppendFormat("in {0}:{1} ", frame.GetSecureFileName(), frame.GetFileLineNumber());
						}
					}
					this.stack_trace = stringBuilder.ToString();
				}
				return this.stack_trace;
			}
		}

		public MethodBase TargetSite
		{
			get
			{
				StackTrace stackTrace = new StackTrace(this, true);
				if (stackTrace.FrameCount > 0)
				{
					return stackTrace.GetFrame(0).GetMethod();
				}
				return null;
			}
		}

		public virtual IDictionary Data
		{
			get
			{
				if (this._data == null)
				{
					this._data = new Hashtable();
				}
				return this._data;
			}
		}

		public virtual Exception GetBaseException()
		{
			for (Exception innerException = this.inner_exception; innerException != null; innerException = innerException.InnerException)
			{
				if (innerException.InnerException == null)
				{
					return innerException;
				}
			}
			return this;
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("ClassName", this.ClassName);
			info.AddValue("Message", this.message);
			info.AddValue("InnerException", this.inner_exception);
			info.AddValue("HelpURL", this.help_link);
			info.AddValue("StackTraceString", this.StackTrace);
			info.AddValue("RemoteStackTraceString", this._remoteStackTraceString);
			info.AddValue("RemoteStackIndex", this.remote_stack_index);
			info.AddValue("HResult", this.hresult);
			info.AddValue("Source", this.Source);
			info.AddValue("ExceptionMethod", null);
			info.AddValue("Data", this._data, typeof(IDictionary));
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(this.ClassName);
			stringBuilder.Append(": ").Append(this.Message);
			if (this._remoteStackTraceString != null)
			{
				stringBuilder.Append(this._remoteStackTraceString);
			}
			if (this.inner_exception != null)
			{
				stringBuilder.Append(" ---> ").Append(this.inner_exception.ToString());
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(Locale.GetText("  --- End of inner exception stack trace ---"));
			}
			if (this.StackTrace != null)
			{
				stringBuilder.Append(Environment.NewLine).Append(this.StackTrace);
			}
			return stringBuilder.ToString();
		}

		internal Exception FixRemotingException()
		{
			string text = ((this.remote_stack_index != 0) ? Locale.GetText("{1}{0}{0}Exception rethrown at [{2}]: {0}") : Locale.GetText("{0}{0}Server stack trace: {0}{1}{0}{0}Exception rethrown at [{2}]: {0}"));
			string text2 = string.Format(text, Environment.NewLine, this.StackTrace, this.remote_stack_index);
			this._remoteStackTraceString = text2;
			this.remote_stack_index++;
			this.stack_trace = null;
			return this;
		}

		internal void GetFullNameForStackTrace(StringBuilder sb, MethodBase mi)
		{
			ParameterInfo[] parameters = mi.GetParameters();
			sb.Append(mi.DeclaringType.ToString());
			sb.Append(".");
			sb.Append(mi.Name);
			if (mi.IsGenericMethod)
			{
				Type[] genericArguments = mi.GetGenericArguments();
				sb.Append("[");
				for (int i = 0; i < genericArguments.Length; i++)
				{
					if (i > 0)
					{
						sb.Append(",");
					}
					sb.Append(genericArguments[i].Name);
				}
				sb.Append("]");
			}
			sb.Append(" (");
			for (int j = 0; j < parameters.Length; j++)
			{
				if (j > 0)
				{
					sb.Append(", ");
				}
				Type parameterType = parameters[j].ParameterType;
				if (parameterType.IsClass && parameterType.Namespace != string.Empty)
				{
					sb.Append(parameterType.Namespace);
					sb.Append(".");
				}
				sb.Append(parameterType.Name);
				if (parameters[j].Name != null)
				{
					sb.Append(" ");
					sb.Append(parameters[j].Name);
				}
			}
			sb.Append(")");
		}

		public new Type GetType()
		{
			return base.GetType();
		}

		private IntPtr[] trace_ips;

		private Exception inner_exception;

		internal string message;

		private string help_link;

		private string class_name;

		private string stack_trace;

		private string _remoteStackTraceString;

		private int remote_stack_index;

		internal int hresult = -2146233088;

		private string source;

		private IDictionary _data;
	}
}
