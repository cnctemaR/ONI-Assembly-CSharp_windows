using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[global::System.ComponentModel.InstallerType(typeof(EventLogInstaller))]
	[MonitoringDescription("Represents an event log")]
	[global::System.ComponentModel.DefaultEvent("EntryWritten")]
	public class EventLog : global::System.ComponentModel.Component, global::System.ComponentModel.ISupportInitialize
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
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value '{0}' for parameter 'machineName'.", new object[] { machineName }));
			}
			this.source = source;
			this.machineName = machineName;
			this.logName = logName;
			this.Impl = EventLog.CreateEventLogImpl(this);
		}

		[MonitoringDescription("Raised for each EventLog entry written.")]
		public event EntryWrittenEventHandler EntryWritten;

		[global::System.ComponentModel.DefaultValue(false)]
		[MonitoringDescription("If enabled raises event when a log is written.")]
		[global::System.ComponentModel.Browsable(false)]
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

		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[global::System.ComponentModel.Browsable(false)]
		[MonitoringDescription("The entries in the log.")]
		public EventLogEntryCollection Entries
		{
			get
			{
				return new EventLogEntryCollection(this.Impl);
			}
		}

		[global::System.ComponentModel.DefaultValue("")]
		[MonitoringDescription("Name of the log that is read and written.")]
		[global::System.ComponentModel.TypeConverter("System.Diagnostics.Design.LogConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[global::System.ComponentModel.RecommendedAsConfigurable(true)]
		[global::System.ComponentModel.ReadOnly(true)]
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

		[global::System.ComponentModel.Browsable(false)]
		public string LogDisplayName
		{
			get
			{
				return this.Impl.LogDisplayName;
			}
		}

		[global::System.ComponentModel.ReadOnly(true)]
		[MonitoringDescription("Name of the machine that this log get written to.")]
		[global::System.ComponentModel.RecommendedAsConfigurable(true)]
		[global::System.ComponentModel.DefaultValue(".")]
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
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value {0} for property MachineName.", new object[] { value }));
				}
				if (string.Compare(this.machineName, value, true) != 0)
				{
					this.Close();
					this.machineName = value;
				}
			}
		}

		[global::System.ComponentModel.ReadOnly(true)]
		[MonitoringDescription("The application name that writes the log.")]
		[global::System.ComponentModel.TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[global::System.ComponentModel.RecommendedAsConfigurable(true)]
		[global::System.ComponentModel.DefaultValue("")]
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
				}
				else if (string.Compare(this.source, value, true) != 0)
				{
					this.source = value;
					this.Reset();
				}
			}
		}

		[MonitoringDescription("An object that synchronizes event handler calls.")]
		[global::System.ComponentModel.Browsable(false)]
		[global::System.ComponentModel.DefaultValue(null)]
		public global::System.ComponentModel.ISynchronizeInvoke SynchronizingObject
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

		[global::System.MonoTODO]
		[ComVisible(false)]
		[global::System.ComponentModel.Browsable(false)]
		public OverflowAction OverflowAction
		{
			get
			{
				return this.Impl.OverflowAction;
			}
		}

		[global::System.MonoTODO]
		[global::System.ComponentModel.Browsable(false)]
		[ComVisible(false)]
		public int MinimumRetentionDays
		{
			get
			{
				return this.Impl.MinimumRetentionDays;
			}
		}

		[global::System.MonoTODO]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		[global::System.ComponentModel.Browsable(false)]
		[ComVisible(false)]
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
		[global::System.MonoTODO]
		public void ModifyOverflowPolicy(OverflowAction action, int retentionDays)
		{
			this.Impl.ModifyOverflowPolicy(action, retentionDays);
		}

		[global::System.MonoTODO]
		[ComVisible(false)]
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
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Event Log '{0}' does not exist on computer '{1}'.", new object[] { log, this.machineName }));
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

		[global::System.MonoNotSupported("remote machine is not supported")]
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
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Source '{0}' already exists on '{1}'.", new object[] { sourceData.Source, sourceData.MachineName }));
			}
			EventLogImpl eventLogImpl = EventLog.CreateEventLogImpl(sourceData.LogName, sourceData.MachineName, sourceData.Source);
			eventLogImpl.CreateEventSource(sourceData);
		}

		public static void Delete(string logName)
		{
			EventLog.Delete(logName, ".");
		}

		[global::System.MonoNotSupported("remote machine is not supported")]
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
			EventLogImpl eventLogImpl = EventLog.CreateEventLogImpl(logName, machineName, string.Empty);
			eventLogImpl.Delete(logName, machineName);
		}

		public static void DeleteEventSource(string source)
		{
			EventLog.DeleteEventSource(source, ".");
		}

		[global::System.MonoNotSupported("remote machine is not supported")]
		public static void DeleteEventSource(string source, string machineName)
		{
			if (machineName == null || machineName.Trim().Length == 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value '{0}' for parameter 'machineName'.", new object[] { machineName }));
			}
			EventLogImpl eventLogImpl = EventLog.CreateEventLogImpl(string.Empty, machineName, source);
			eventLogImpl.DeleteEventSource(source, machineName);
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

		[global::System.MonoNotSupported("remote machine is not supported")]
		public static bool Exists(string logName, string machineName)
		{
			if (machineName == null || machineName.Trim().Length == 0)
			{
				throw new ArgumentException("Invalid format for argument machineName.");
			}
			if (logName == null || logName.Length == 0)
			{
				return false;
			}
			EventLogImpl eventLogImpl = EventLog.CreateEventLogImpl(logName, machineName, string.Empty);
			return eventLogImpl.Exists(logName, machineName);
		}

		public static EventLog[] GetEventLogs()
		{
			return EventLog.GetEventLogs(".");
		}

		[global::System.MonoNotSupported("remote machine is not supported")]
		public static EventLog[] GetEventLogs(string machineName)
		{
			EventLogImpl eventLogImpl = EventLog.CreateEventLogImpl(new EventLog());
			return eventLogImpl.GetEventLogs(machineName);
		}

		[global::System.MonoNotSupported("remote machine is not supported")]
		public static string LogNameFromSourceName(string source, string machineName)
		{
			if (machineName == null || machineName.Trim().Length == 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value '{0}' for parameter 'MachineName'.", new object[] { machineName }));
			}
			EventLogImpl eventLogImpl = EventLog.CreateEventLogImpl(string.Empty, machineName, source);
			return eventLogImpl.LogNameFromSourceName(source, machineName);
		}

		public static bool SourceExists(string source)
		{
			return EventLog.SourceExists(source, ".");
		}

		[global::System.MonoNotSupported("remote machine is not supported")]
		public static bool SourceExists(string source, string machineName)
		{
			if (machineName == null || machineName.Trim().Length == 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value '{0}' for parameter 'machineName'.", new object[] { machineName }));
			}
			EventLogImpl eventLogImpl = EventLog.CreateEventLogImpl(string.Empty, machineName, source);
			return eventLogImpl.SourceExists(source, machineName);
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
			EventLog eventLog = new EventLog(logName, machineName, source);
			return EventLog.CreateEventLogImpl(eventLog);
		}

		private static EventLogImpl CreateEventLogImpl(EventLog eventLog)
		{
			string eventLogImplType = EventLog.EventLogImplType;
			switch (eventLogImplType)
			{
			case "local":
				return new LocalFileEventLog(eventLog);
			case "win32":
				return new Win32EventLog(eventLog);
			case "null":
				return new NullEventLog(eventLog);
			}
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Eventlog implementation '{0}' is not supported.", new object[] { EventLog.EventLogImplType }));
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
						throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Eventlog implementation '{0}' is not supported.", new object[] { text }));
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
				throw new global::System.ComponentModel.InvalidEnumArgumentException("type", (int)type, typeof(EventLogEntryType));
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
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The source '{0}' is not registered in log '{1}' (it is registered in log '{2}'). The Source and Log properties must be matched, or you may set Log to the empty string, and it will automatically be matched to the Source property.", new object[] { this.Source, this.logName, text }));
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
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid eventID value '{0}'. It must be in the range between '{1}' and '{2}'.", new object[] { instanceID, 0, ushort.MaxValue }));
			}
		}

		internal static int GetEventID(long instanceID)
		{
			long num = ((instanceID >= 0L) ? instanceID : (-instanceID));
			int num2 = (int)(num & 1073741823L);
			return (instanceID >= 0L) ? num2 : (-num2);
		}

		internal const string LOCAL_FILE_IMPL = "local";

		private const string WIN32_IMPL = "win32";

		private const string NULL_IMPL = "null";

		internal const string EVENTLOG_TYPE_VAR = "MONO_EVENTLOG_TYPE";

		private string source;

		private string logName;

		private string machineName;

		private bool doRaiseEvents;

		private global::System.ComponentModel.ISynchronizeInvoke synchronizingObject;

		private EventLogImpl Impl;
	}
}
