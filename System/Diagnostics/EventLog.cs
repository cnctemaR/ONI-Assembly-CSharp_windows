using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[DefaultEvent("EntryWritten")]
	[MonitoringDescription("Represents an event log")]
	[InstallerType(typeof(EventLogInstaller))]
	public class EventLog : Component, ISupportInitialize
	{
		public EventLog()
			: this(string.Empty)
		{
		}

		public EventLog(string logName)
			: this(logName, ".")
		{
		}

		public EventLog(string logName, string machineName)
			: this(logName, machineName, string.Empty)
		{
		}

		public EventLog(string logName, string machineName, string source)
		{
			if (logName == null)
			{
				throw new ArgumentNullException("logName");
			}
			if (machineName == null || machineName.Trim().Length == 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value '{0}' for parameter 'machineName'.", machineName));
			}
			this.source = source;
			this.machineName = machineName;
			this.logName = logName;
			this.Impl = EventLog.CreateEventLogImpl(this);
		}

		[Browsable(false)]
		[DefaultValue(false)]
		[MonitoringDescription("If enabled raises event when a log is written.")]
		public bool EnableRaisingEvents
		{
			get
			{
				return this.doRaiseEvents;
			}
			set
			{
				if (value == this.doRaiseEvents)
				{
					return;
				}
				if (value)
				{
					this.Impl.EnableNotification();
				}
				else
				{
					this.Impl.DisableNotification();
				}
				this.doRaiseEvents = value;
			}
		}

		[Browsable(false)]
		[MonitoringDescription("The entries in the log.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public EventLogEntryCollection Entries
		{
			get
			{
				return new EventLogEntryCollection(this.Impl);
			}
		}

		[ReadOnly(true)]
		[DefaultValue("")]
		[TypeConverter("System.Diagnostics.Design.LogConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[MonitoringDescription("Name of the log that is read and written.")]
		[RecommendedAsConfigurable(true)]
		public string Log
		{
			get
			{
				if (this.source != null && this.source.Length > 0)
				{
					return this.GetLogName();
				}
				return this.logName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (string.Compare(this.logName, value, true) != 0)
				{
					this.logName = value;
					this.Reset();
				}
			}
		}

		[Browsable(false)]
		public string LogDisplayName
		{
			get
			{
				return this.Impl.LogDisplayName;
			}
		}

		[DefaultValue(".")]
		[RecommendedAsConfigurable(true)]
		[ReadOnly(true)]
		[MonitoringDescription("Name of the machine that this log get written to.")]
		public string MachineName
		{
			get
			{
				return this.machineName;
			}
			set
			{
				if (value == null || value.Trim().Length == 0)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value {0} for property MachineName.", value));
				}
				if (string.Compare(this.machineName, value, true) != 0)
				{
					this.Close();
					this.machineName = value;
				}
			}
		}

		[MonitoringDescription("The application name that writes the log.")]
		[DefaultValue("")]
		[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[ReadOnly(true)]
		[RecommendedAsConfigurable(true)]
		public string Source
		{
			get
			{
				return this.source;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (this.source == null || (this.source.Length == 0 && (this.logName == null || this.logName.Length == 0)))
				{
					this.source = value;
					return;
				}
				if (string.Compare(this.source, value, true) != 0)
				{
					this.source = value;
					this.Reset();
				}
			}
		}

		[MonitoringDescription("An object that synchronizes event handler calls.")]
		[DefaultValue(null)]
		[Browsable(false)]
		public ISynchronizeInvoke SynchronizingObject
		{
			get
			{
				return this.synchronizingObject;
			}
			set
			{
				this.synchronizingObject = value;
			}
		}

		[Browsable(false)]
		[ComVisible(false)]
		[MonoTODO]
		public OverflowAction OverflowAction
		{
			get
			{
				return this.Impl.OverflowAction;
			}
		}

		[ComVisible(false)]
		[Browsable(false)]
		[MonoTODO]
		public int MinimumRetentionDays
		{
			get
			{
				return this.Impl.MinimumRetentionDays;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[ComVisible(false)]
		[MonoTODO]
		public long MaximumKilobytes
		{
			get
			{
				return this.Impl.MaximumKilobytes;
			}
			set
			{
				this.Impl.MaximumKilobytes = value;
			}
		}

		[ComVisible(false)]
		[MonoTODO]
		public void ModifyOverflowPolicy(OverflowAction action, int retentionDays)
		{
			this.Impl.ModifyOverflowPolicy(action, retentionDays);
		}

		[ComVisible(false)]
		[MonoTODO]
		public void RegisterDisplayName(string resourceFile, long resourceId)
		{
			this.Impl.RegisterDisplayName(resourceFile, resourceId);
		}

		public void BeginInit()
		{
			this.Impl.BeginInit();
		}

		public void Clear()
		{
			string log = this.Log;
			if (log == null || log.Length == 0)
			{
				throw new ArgumentException("Log property value has not been specified.");
			}
			if (!EventLog.Exists(log, this.MachineName))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Event Log '{0}' does not exist on computer '{1}'.", log, this.machineName));
			}
			this.Impl.Clear();
			this.Reset();
		}

		public void Close()
		{
			this.Impl.Close();
			this.EnableRaisingEvents = false;
		}

		internal void Reset()
		{
			bool enableRaisingEvents = this.EnableRaisingEvents;
			this.Close();
			this.EnableRaisingEvents = enableRaisingEvents;
		}

		public static void CreateEventSource(string source, string logName)
		{
			EventLog.CreateEventSource(source, logName, ".");
		}

		[Obsolete("use CreateEventSource(EventSourceCreationData) instead")]
		public static void CreateEventSource(string source, string logName, string machineName)
		{
			EventLog.CreateEventSource(new EventSourceCreationData(source, logName, machineName));
		}

		[MonoNotSupported("remote machine is not supported")]
		public static void CreateEventSource(EventSourceCreationData sourceData)
		{
			if (sourceData.Source == null || sourceData.Source.Length == 0)
			{
				throw new ArgumentException("Source property value has not been specified.");
			}
			if (sourceData.LogName == null || sourceData.LogName.Length == 0)
			{
				throw new ArgumentException("Log property value has not been specified.");
			}
			if (EventLog.SourceExists(sourceData.Source, sourceData.MachineName))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Source '{0}' already exists on '{1}'.", sourceData.Source, sourceData.MachineName));
			}
			EventLog.CreateEventLogImpl(sourceData.LogName, sourceData.MachineName, sourceData.Source).CreateEventSource(sourceData);
		}

		public static void Delete(string logName)
		{
			EventLog.Delete(logName, ".");
		}

		[MonoNotSupported("remote machine is not supported")]
		public static void Delete(string logName, string machineName)
		{
			if (machineName == null || machineName.Trim().Length == 0)
			{
				throw new ArgumentException("Invalid format for argument machineName.");
			}
			if (logName == null || logName.Length == 0)
			{
				throw new ArgumentException("Log to delete was not specified.");
			}
			EventLog.CreateEventLogImpl(logName, machineName, string.Empty).Delete(logName, machineName);
		}

		public static void DeleteEventSource(string source)
		{
			EventLog.DeleteEventSource(source, ".");
		}

		[MonoNotSupported("remote machine is not supported")]
		public static void DeleteEventSource(string source, string machineName)
		{
			if (machineName == null || machineName.Trim().Length == 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value '{0}' for parameter 'machineName'.", machineName));
			}
			EventLog.CreateEventLogImpl(string.Empty, machineName, source).DeleteEventSource(source, machineName);
		}

		protected override void Dispose(bool disposing)
		{
			if (this.Impl != null)
			{
				this.Impl.Dispose(disposing);
			}
		}

		public void EndInit()
		{
			this.Impl.EndInit();
		}

		public static bool Exists(string logName)
		{
			return EventLog.Exists(logName, ".");
		}

		[MonoNotSupported("remote machine is not supported")]
		public static bool Exists(string logName, string machineName)
		{
			if (machineName == null || machineName.Trim().Length == 0)
			{
				throw new ArgumentException("Invalid format for argument machineName.");
			}
			return logName != null && logName.Length != 0 && EventLog.CreateEventLogImpl(logName, machineName, string.Empty).Exists(logName, machineName);
		}

		public static EventLog[] GetEventLogs()
		{
			return EventLog.GetEventLogs(".");
		}

		[MonoNotSupported("remote machine is not supported")]
		public static EventLog[] GetEventLogs(string machineName)
		{
			return EventLog.CreateEventLogImpl(new EventLog()).GetEventLogs(machineName);
		}

		[MonoNotSupported("remote machine is not supported")]
		public static string LogNameFromSourceName(string source, string machineName)
		{
			if (machineName == null || machineName.Trim().Length == 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value '{0}' for parameter 'MachineName'.", machineName));
			}
			return EventLog.CreateEventLogImpl(string.Empty, machineName, source).LogNameFromSourceName(source, machineName);
		}

		public static bool SourceExists(string source)
		{
			return EventLog.SourceExists(source, ".");
		}

		[MonoNotSupported("remote machine is not supported")]
		public static bool SourceExists(string source, string machineName)
		{
			if (machineName == null || machineName.Trim().Length == 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value '{0}' for parameter 'machineName'.", machineName));
			}
			return EventLog.CreateEventLogImpl(string.Empty, machineName, source).SourceExists(source, machineName);
		}

		public void WriteEntry(string message)
		{
			this.WriteEntry(message, EventLogEntryType.Information);
		}

		public void WriteEntry(string message, EventLogEntryType type)
		{
			this.WriteEntry(message, type, 0);
		}

		public void WriteEntry(string message, EventLogEntryType type, int eventID)
		{
			this.WriteEntry(message, type, eventID, 0);
		}

		public void WriteEntry(string message, EventLogEntryType type, int eventID, short category)
		{
			this.WriteEntry(message, type, eventID, category, null);
		}

		public void WriteEntry(string message, EventLogEntryType type, int eventID, short category, byte[] rawData)
		{
			this.WriteEntry(new string[] { message }, type, (long)eventID, category, rawData);
		}

		public static void WriteEntry(string source, string message)
		{
			EventLog.WriteEntry(source, message, EventLogEntryType.Information);
		}

		public static void WriteEntry(string source, string message, EventLogEntryType type)
		{
			EventLog.WriteEntry(source, message, type, 0);
		}

		public static void WriteEntry(string source, string message, EventLogEntryType type, int eventID)
		{
			EventLog.WriteEntry(source, message, type, eventID, 0);
		}

		public static void WriteEntry(string source, string message, EventLogEntryType type, int eventID, short category)
		{
			EventLog.WriteEntry(source, message, type, eventID, category, null);
		}

		public static void WriteEntry(string source, string message, EventLogEntryType type, int eventID, short category, byte[] rawData)
		{
			using (EventLog eventLog = new EventLog())
			{
				eventLog.Source = source;
				eventLog.WriteEntry(message, type, eventID, category, rawData);
			}
		}

		[ComVisible(false)]
		public void WriteEvent(EventInstance instance, params object[] values)
		{
			this.WriteEvent(instance, null, values);
		}

		[ComVisible(false)]
		public void WriteEvent(EventInstance instance, byte[] data, params object[] values)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			string[] array;
			if (values != null)
			{
				array = new string[values.Length];
				for (int i = 0; i < values.Length; i++)
				{
					if (values[i] == null)
					{
						array[i] = string.Empty;
					}
					else
					{
						array[i] = values[i].ToString();
					}
				}
			}
			else
			{
				array = new string[0];
			}
			this.WriteEntry(array, instance.EntryType, instance.InstanceId, (short)instance.CategoryId, data);
		}

		public static void WriteEvent(string source, EventInstance instance, params object[] values)
		{
			EventLog.WriteEvent(source, instance, null, values);
		}

		public static void WriteEvent(string source, EventInstance instance, byte[] data, params object[] values)
		{
			using (EventLog eventLog = new EventLog())
			{
				eventLog.Source = source;
				eventLog.WriteEvent(instance, data, values);
			}
		}

		internal void OnEntryWritten(EventLogEntry newEntry)
		{
			if (this.doRaiseEvents && this.EntryWritten != null)
			{
				this.EntryWritten(this, new EntryWrittenEventArgs(newEntry));
			}
		}

		[MonitoringDescription("Raised for each EventLog entry written.")]
		public event EntryWrittenEventHandler EntryWritten;

		internal string GetLogName()
		{
			if (this.logName != null && this.logName.Length > 0)
			{
				return this.logName;
			}
			this.logName = EventLog.LogNameFromSourceName(this.source, this.machineName);
			return this.logName;
		}

		private static EventLogImpl CreateEventLogImpl(string logName, string machineName, string source)
		{
			return EventLog.CreateEventLogImpl(new EventLog(logName, machineName, source));
		}

		private static EventLogImpl CreateEventLogImpl(EventLog eventLog)
		{
			string eventLogImplType = EventLog.EventLogImplType;
			if (eventLogImplType == "local")
			{
				return new LocalFileEventLog(eventLog);
			}
			if (eventLogImplType == "win32")
			{
				return new Win32EventLog(eventLog);
			}
			if (!(eventLogImplType == "null"))
			{
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Eventlog implementation '{0}' is not supported.", EventLog.EventLogImplType));
			}
			return new NullEventLog(eventLog);
		}

		private static bool Win32EventLogEnabled
		{
			get
			{
				return Environment.OSVersion.Platform == PlatformID.Win32NT;
			}
		}

		private static string EventLogImplType
		{
			get
			{
				string text = Environment.GetEnvironmentVariable("MONO_EVENTLOG_TYPE");
				if (text == null)
				{
					if (EventLog.Win32EventLogEnabled)
					{
						return "win32";
					}
					text = "null";
				}
				else if (EventLog.Win32EventLogEnabled && string.Compare(text, "win32", true) == 0)
				{
					text = "win32";
				}
				else if (string.Compare(text, "null", true) == 0)
				{
					text = "null";
				}
				else
				{
					if (string.Compare(text, 0, "local", 0, "local".Length, true) != 0)
					{
						throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Eventlog implementation '{0}' is not supported.", text));
					}
					text = "local";
				}
				return text;
			}
		}

		private void WriteEntry(string[] replacementStrings, EventLogEntryType type, long instanceID, short category, byte[] rawData)
		{
			if (this.Source.Length == 0)
			{
				throw new ArgumentException("Source property was not setbefore writing to the event log.");
			}
			if (!Enum.IsDefined(typeof(EventLogEntryType), type))
			{
				throw new InvalidEnumArgumentException("type", (int)type, typeof(EventLogEntryType));
			}
			this.ValidateEventID(instanceID);
			if (!EventLog.SourceExists(this.Source, this.MachineName))
			{
				if (this.Log == null || this.Log.Length == 0)
				{
					this.Log = "Application";
				}
				EventLog.CreateEventSource(this.Source, this.Log, this.MachineName);
			}
			else if (this.logName != null && this.logName.Length != 0)
			{
				string text = EventLog.LogNameFromSourceName(this.Source, this.MachineName);
				if (string.Compare(this.logName, text, true, CultureInfo.InvariantCulture) != 0)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The source '{0}' is not registered in log '{1}' (it is registered in log '{2}'). The Source and Log properties must be matched, or you may set Log to the empty string, and it will automatically be matched to the Source property.", this.Source, this.logName, text));
				}
			}
			if (rawData == null)
			{
				rawData = new byte[0];
			}
			this.Impl.WriteEntry(replacementStrings, type, (uint)instanceID, category, rawData);
		}

		private void ValidateEventID(long instanceID)
		{
			int eventID = EventLog.GetEventID(instanceID);
			if (eventID < 0 || eventID > 65535)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid eventID value '{0}'. It must be in the range between '{1}' and '{2}'.", instanceID, 0, ushort.MaxValue));
			}
		}

		internal static int GetEventID(long instanceID)
		{
			int num = (int)(((instanceID < 0L) ? (-instanceID) : instanceID) & 1073741823L);
			if (instanceID >= 0L)
			{
				return num;
			}
			return -num;
		}

		private string source;

		private string logName;

		private string machineName;

		private bool doRaiseEvents;

		private ISynchronizeInvoke synchronizingObject;

		internal const string LOCAL_FILE_IMPL = "local";

		private const string WIN32_IMPL = "win32";

		private const string NULL_IMPL = "null";

		internal const string EVENTLOG_TYPE_VAR = "MONO_EVENTLOG_TYPE";

		private EventLogImpl Impl;
	}
}
