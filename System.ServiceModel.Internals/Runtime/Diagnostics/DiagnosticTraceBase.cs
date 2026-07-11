using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Xml;

namespace System.Runtime.Diagnostics
{
	internal abstract class DiagnosticTraceBase
	{
		public DiagnosticTraceBase(string traceSourceName)
		{
			this.thisLock = new object();
			this.TraceSourceName = traceSourceName;
			this.LastFailure = DateTime.MinValue;
		}

		protected DateTime LastFailure { get; set; }

		[SecurityCritical]
		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		private static void UnsafeRemoveDefaultTraceListener(TraceSource traceSource)
		{
			traceSource.Listeners.Remove("Default");
		}

		public TraceSource TraceSource
		{
			get
			{
				return this.traceSource;
			}
			set
			{
				this.SetTraceSource(value);
			}
		}

		[SecuritySafeCritical]
		protected void SetTraceSource(TraceSource traceSource)
		{
			if (traceSource != null)
			{
				DiagnosticTraceBase.UnsafeRemoveDefaultTraceListener(traceSource);
				this.traceSource = traceSource;
				this.haveListeners = this.traceSource.Listeners.Count > 0;
			}
		}

		public bool HaveListeners
		{
			get
			{
				return this.haveListeners;
			}
		}

		private SourceLevels FixLevel(SourceLevels level)
		{
			if ((level & (SourceLevels)(-16) & SourceLevels.Verbose) != SourceLevels.Off)
			{
				level |= SourceLevels.Verbose;
			}
			else if ((level & (SourceLevels)(-8) & SourceLevels.Information) != SourceLevels.Off)
			{
				level |= SourceLevels.Information;
			}
			else if ((level & (SourceLevels)(-4) & SourceLevels.Warning) != SourceLevels.Off)
			{
				level |= SourceLevels.Warning;
			}
			if ((level & ~SourceLevels.Critical & SourceLevels.Error) != SourceLevels.Off)
			{
				level |= SourceLevels.Error;
			}
			if ((level & SourceLevels.Critical) != SourceLevels.Off)
			{
				level |= SourceLevels.Critical;
			}
			if (level == SourceLevels.ActivityTracing)
			{
				level = SourceLevels.Off;
			}
			return level;
		}

		protected virtual void OnSetLevel(SourceLevels level)
		{
		}

		[SecurityCritical]
		private void SetLevel(SourceLevels level)
		{
			SourceLevels sourceLevels = this.FixLevel(level);
			this.level = sourceLevels;
			if (this.TraceSource != null)
			{
				this.haveListeners = this.TraceSource.Listeners.Count > 0;
				this.OnSetLevel(level);
				this.tracingEnabled = this.HaveListeners && level > SourceLevels.Off;
				this.TraceSource.Switch.Level = level;
			}
		}

		[SecurityCritical]
		private void SetLevelThreadSafe(SourceLevels level)
		{
			object obj = this.thisLock;
			lock (obj)
			{
				this.SetLevel(level);
			}
		}

		public SourceLevels Level
		{
			get
			{
				if (this.TraceSource != null && this.TraceSource.Switch.Level != this.level)
				{
					this.level = this.TraceSource.Switch.Level;
				}
				return this.level;
			}
			[SecurityCritical]
			set
			{
				this.SetLevelThreadSafe(value);
			}
		}

		protected string EventSourceName
		{
			[SecuritySafeCritical]
			get
			{
				return this.eventSourceName;
			}
			[SecurityCritical]
			set
			{
				this.eventSourceName = value;
			}
		}

		public bool TracingEnabled
		{
			get
			{
				return this.tracingEnabled && this.traceSource != null;
			}
		}

		protected static string ProcessName
		{
			[SecuritySafeCritical]
			get
			{
				string text = null;
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					text = currentProcess.ProcessName;
				}
				return text;
			}
		}

		protected static int ProcessId
		{
			[SecuritySafeCritical]
			get
			{
				int num = -1;
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					num = currentProcess.Id;
				}
				return num;
			}
		}

		public virtual bool ShouldTrace(TraceEventLevel level)
		{
			return this.ShouldTraceToTraceSource(level);
		}

		public bool ShouldTrace(TraceEventType type)
		{
			return this.TracingEnabled && this.HaveListeners && this.TraceSource != null && (type & (TraceEventType)this.Level) > (TraceEventType)0;
		}

		public bool ShouldTraceToTraceSource(TraceEventLevel level)
		{
			return this.ShouldTrace(TraceLevelHelper.GetTraceEventType(level));
		}

		public static string XmlEncode(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			int length = text.Length;
			StringBuilder stringBuilder = new StringBuilder(length + 8);
			for (int i = 0; i < length; i++)
			{
				char c = text[i];
				if (c != '&')
				{
					if (c != '<')
					{
						if (c != '>')
						{
							stringBuilder.Append(c);
						}
						else
						{
							stringBuilder.Append("&gt;");
						}
					}
					else
					{
						stringBuilder.Append("&lt;");
					}
				}
				else
				{
					stringBuilder.Append("&amp;");
				}
			}
			return stringBuilder.ToString();
		}

		[SecuritySafeCritical]
		protected void AddDomainEventHandlersForCleanup()
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			if (this.TraceSource != null)
			{
				this.haveListeners = this.TraceSource.Listeners.Count > 0;
			}
			this.tracingEnabled = this.haveListeners;
			if (this.TracingEnabled)
			{
				currentDomain.UnhandledException += this.UnhandledExceptionHandler;
				this.SetLevel(this.TraceSource.Switch.Level);
				currentDomain.DomainUnload += this.ExitOrUnloadEventHandler;
				currentDomain.ProcessExit += this.ExitOrUnloadEventHandler;
			}
		}

		private void ExitOrUnloadEventHandler(object sender, EventArgs e)
		{
			this.ShutdownTracing();
		}

		protected abstract void OnUnhandledException(Exception exception);

		protected void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
		{
			Exception ex = (Exception)args.ExceptionObject;
			this.OnUnhandledException(ex);
			this.ShutdownTracing();
		}

		protected static string CreateSourceString(object source)
		{
			ITraceSourceStringProvider traceSourceStringProvider = source as ITraceSourceStringProvider;
			if (traceSourceStringProvider != null)
			{
				return traceSourceStringProvider.GetSourceString();
			}
			return DiagnosticTraceBase.CreateDefaultSourceString(source);
		}

		internal static string CreateDefaultSourceString(object source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return string.Format(CultureInfo.CurrentCulture, "{0}/{1}", source.GetType().ToString(), source.GetHashCode());
		}

		protected static void AddExceptionToTraceString(XmlWriter xml, Exception exception)
		{
			xml.WriteElementString("ExceptionType", DiagnosticTraceBase.XmlEncode(exception.GetType().AssemblyQualifiedName));
			xml.WriteElementString("Message", DiagnosticTraceBase.XmlEncode(exception.Message));
			xml.WriteElementString("StackTrace", DiagnosticTraceBase.XmlEncode(DiagnosticTraceBase.StackTraceString(exception)));
			xml.WriteElementString("ExceptionString", DiagnosticTraceBase.XmlEncode(exception.ToString()));
			Win32Exception ex = exception as Win32Exception;
			if (ex != null)
			{
				xml.WriteElementString("NativeErrorCode", ex.NativeErrorCode.ToString("X", CultureInfo.InvariantCulture));
			}
			if (exception.Data != null && exception.Data.Count > 0)
			{
				xml.WriteStartElement("DataItems");
				foreach (object obj in exception.Data.Keys)
				{
					xml.WriteStartElement("Data");
					xml.WriteElementString("Key", DiagnosticTraceBase.XmlEncode(obj.ToString()));
					xml.WriteElementString("Value", DiagnosticTraceBase.XmlEncode(exception.Data[obj].ToString()));
					xml.WriteEndElement();
				}
				xml.WriteEndElement();
			}
			if (exception.InnerException != null)
			{
				xml.WriteStartElement("InnerException");
				DiagnosticTraceBase.AddExceptionToTraceString(xml, exception.InnerException);
				xml.WriteEndElement();
			}
		}

		protected static string StackTraceString(Exception exception)
		{
			string text = exception.StackTrace;
			if (string.IsNullOrEmpty(text))
			{
				StackFrame[] frames = new StackTrace(false).GetFrames();
				int num = 0;
				bool flag = false;
				StackFrame[] array = frames;
				for (int i = 0; i < array.Length; i++)
				{
					string name = array[i].GetMethod().Name;
					if (name == "StackTraceString" || name == "AddExceptionToTraceString" || name == "BuildTrace" || name == "TraceEvent" || name == "TraceException" || name == "GetAdditionalPayload")
					{
						num++;
					}
					else if (name.StartsWith("ThrowHelper", StringComparison.Ordinal))
					{
						num++;
					}
					else
					{
						flag = true;
					}
					if (flag)
					{
						break;
					}
				}
				text = new StackTrace(num, false).ToString();
			}
			return text;
		}

		[SecuritySafeCritical]
		protected void LogTraceFailure(string traceString, Exception exception)
		{
			TimeSpan timeSpan = TimeSpan.FromMinutes(10.0);
			try
			{
				object obj = this.thisLock;
				lock (obj)
				{
					if (DateTime.UtcNow.Subtract(this.LastFailure) >= timeSpan)
					{
						this.LastFailure = DateTime.UtcNow;
						EventLogger eventLogger = EventLogger.UnsafeCreateEventLogger(this.eventSourceName, this);
						if (exception == null)
						{
							eventLogger.UnsafeLogEvent(TraceEventType.Error, 4, 3221291112U, false, new string[] { traceString });
						}
						else
						{
							eventLogger.UnsafeLogEvent(TraceEventType.Error, 4, 3221291113U, false, new string[]
							{
								traceString,
								exception.ToString()
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
			}
		}

		protected abstract void OnShutdownTracing();

		private void ShutdownTracing()
		{
			if (!this.calledShutdown)
			{
				this.calledShutdown = true;
				try
				{
					this.OnShutdownTracing();
				}
				catch (Exception ex)
				{
					if (Fx.IsFatal(ex))
					{
						throw;
					}
					this.LogTraceFailure(null, ex);
				}
			}
		}

		protected bool CalledShutdown
		{
			get
			{
				return this.calledShutdown;
			}
		}

		public static Guid ActivityId
		{
			[SecuritySafeCritical]
			get
			{
				object obj = Trace.CorrelationManager.ActivityId;
				if (obj != null)
				{
					return (Guid)obj;
				}
				return Guid.Empty;
			}
			[SecuritySafeCritical]
			set
			{
				Trace.CorrelationManager.ActivityId = value;
			}
		}

		protected static string LookupSeverity(TraceEventType type)
		{
			if (type <= TraceEventType.Verbose)
			{
				switch (type)
				{
				case TraceEventType.Critical:
					return "Critical";
				case TraceEventType.Error:
					return "Error";
				case (TraceEventType)3:
					break;
				case TraceEventType.Warning:
					return "Warning";
				default:
					if (type == TraceEventType.Information)
					{
						return "Information";
					}
					if (type == TraceEventType.Verbose)
					{
						return "Verbose";
					}
					break;
				}
			}
			else if (type <= TraceEventType.Stop)
			{
				if (type == TraceEventType.Start)
				{
					return "Start";
				}
				if (type == TraceEventType.Stop)
				{
					return "Stop";
				}
			}
			else
			{
				if (type == TraceEventType.Suspend)
				{
					return "Suspend";
				}
				if (type == TraceEventType.Transfer)
				{
					return "Transfer";
				}
			}
			return type.ToString();
		}

		public abstract bool IsEnabled();

		public abstract void TraceEventLogEvent(TraceEventType type, TraceRecord traceRecord);

		protected const string DefaultTraceListenerName = "Default";

		protected const string TraceRecordVersion = "http://schemas.microsoft.com/2004/10/E2ETraceEvent/TraceRecord";

		protected static string AppDomainFriendlyName = AppDomain.CurrentDomain.FriendlyName;

		private const ushort TracingEventLogCategory = 4;

		private object thisLock;

		private bool tracingEnabled = true;

		private bool calledShutdown;

		private bool haveListeners;

		private SourceLevels level;

		protected string TraceSourceName;

		private TraceSource traceSource;

		[SecurityCritical]
		private string eventSourceName;
	}
}
