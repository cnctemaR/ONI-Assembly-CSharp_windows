using System;
using System.Globalization;

namespace System.Diagnostics
{
	internal abstract class EventLogImpl
	{
		protected EventLogImpl(EventLog coreEventLog)
		{
			this._coreEventLog = coreEventLog;
		}

		protected EventLog CoreEventLog
		{
			get
			{
				return this._coreEventLog;
			}
		}

		public int EntryCount
		{
			get
			{
				if (this._coreEventLog.Log == null || this._coreEventLog.Log.Length == 0)
				{
					throw new ArgumentException("Log property is not set.");
				}
				if (!EventLog.Exists(this._coreEventLog.Log, this._coreEventLog.MachineName))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The event log '{0}' on  computer '{1}' does not exist.", this._coreEventLog.Log, this._coreEventLog.MachineName));
				}
				return this.GetEntryCount();
			}
		}

		public EventLogEntry this[int index]
		{
			get
			{
				if (this._coreEventLog.Log == null || this._coreEventLog.Log.Length == 0)
				{
					throw new ArgumentException("Log property is not set.");
				}
				if (!EventLog.Exists(this._coreEventLog.Log, this._coreEventLog.MachineName))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The event log '{0}' on  computer '{1}' does not exist.", this._coreEventLog.Log, this._coreEventLog.MachineName));
				}
				if (index < 0 || index >= this.EntryCount)
				{
					throw new ArgumentException("Index out of range");
				}
				return this.GetEntry(index);
			}
		}

		public string LogDisplayName
		{
			get
			{
				if (this._coreEventLog.Log != null && this._coreEventLog.Log.Length == 0)
				{
					throw new InvalidOperationException("Event log names must consist of printable characters and cannot contain \\, *, ?, or spaces.");
				}
				if (this._coreEventLog.Log != null)
				{
					if (this._coreEventLog.Log.Length == 0)
					{
						return string.Empty;
					}
					if (!EventLog.Exists(this._coreEventLog.Log, this._coreEventLog.MachineName))
					{
						throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot find Log {0} on computer {1}.", this._coreEventLog.Log, this._coreEventLog.MachineName));
					}
				}
				return this.GetLogDisplayName();
			}
		}

		public EventLogEntry[] GetEntries()
		{
			string log = this.CoreEventLog.Log;
			if (log == null || log.Length == 0)
			{
				throw new ArgumentException("Log property value has not been specified.");
			}
			if (!EventLog.Exists(log))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The event log '{0}' on  computer '{1}' does not exist.", log, this._coreEventLog.MachineName));
			}
			int entryCount = this.GetEntryCount();
			EventLogEntry[] array = new EventLogEntry[entryCount];
			for (int i = 0; i < entryCount; i++)
			{
				array[i] = this.GetEntry(i);
			}
			return array;
		}

		public abstract void DisableNotification();

		public abstract void EnableNotification();

		public abstract void BeginInit();

		public abstract void Clear();

		public abstract void Close();

		public abstract void CreateEventSource(EventSourceCreationData sourceData);

		public abstract void Delete(string logName, string machineName);

		public abstract void DeleteEventSource(string source, string machineName);

		public abstract void Dispose(bool disposing);

		public abstract void EndInit();

		public abstract bool Exists(string logName, string machineName);

		protected abstract int GetEntryCount();

		protected abstract EventLogEntry GetEntry(int index);

		public EventLog[] GetEventLogs(string machineName)
		{
			string[] logNames = this.GetLogNames(machineName);
			EventLog[] array = new EventLog[logNames.Length];
			for (int i = 0; i < logNames.Length; i++)
			{
				EventLog eventLog = new EventLog(logNames[i], machineName);
				array[i] = eventLog;
			}
			return array;
		}

		protected abstract string GetLogDisplayName();

		public abstract string LogNameFromSourceName(string source, string machineName);

		public abstract bool SourceExists(string source, string machineName);

		public abstract void WriteEntry(string[] replacementStrings, EventLogEntryType type, uint instanceID, short category, byte[] rawData);

		protected abstract string FormatMessage(string source, uint messageID, string[] replacementStrings);

		protected abstract string[] GetLogNames(string machineName);

		protected void ValidateCustomerLogName(string logName, string machineName)
		{
			if (logName.Length >= 8)
			{
				string text = logName.Substring(0, 8);
				if (string.Compare(text, "AppEvent", true) == 0 || string.Compare(text, "SysEvent", true) == 0 || string.Compare(text, "SecEvent", true) == 0)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The log name: '{0}' is invalid for customer log creation.", logName));
				}
				foreach (string text2 in this.GetLogNames(machineName))
				{
					if (text2.Length >= 8 && string.Compare(text2, 0, text, 0, 8, true) == 0)
					{
						throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Only the first eight characters of a custom log name are significant, and there is already another log on the system using the first eight characters of the name given. Name given: '{0}', name of existing log: '{1}'.", logName, text2));
					}
				}
			}
			if (!this.SourceExists(logName, machineName))
			{
				return;
			}
			if (machineName == ".")
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Log {0} has already been registered as a source on the local computer.", logName));
			}
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Log {0} has already been registered as a source on the computer {1}.", logName, machineName));
		}

		public abstract OverflowAction OverflowAction { get; }

		public abstract int MinimumRetentionDays { get; }

		public abstract long MaximumKilobytes { get; set; }

		public abstract void ModifyOverflowPolicy(OverflowAction action, int retentionDays);

		public abstract void RegisterDisplayName(string resourceFile, long resourceId);

		private readonly EventLog _coreEventLog;
	}
}
