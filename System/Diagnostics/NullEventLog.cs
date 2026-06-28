using System;

namespace System.Diagnostics
{
	internal class NullEventLog : EventLogImpl
	{
		public NullEventLog(EventLog coreEventLog)
			: base(coreEventLog)
		{
		}

		public override void BeginInit()
		{
		}

		public override void Clear()
		{
		}

		public override void Close()
		{
		}

		public override void CreateEventSource(EventSourceCreationData sourceData)
		{
		}

		public override void Delete(string logName, string machineName)
		{
		}

		public override void DeleteEventSource(string source, string machineName)
		{
		}

		public override void Dispose(bool disposing)
		{
		}

		public override void DisableNotification()
		{
		}

		public override void EnableNotification()
		{
		}

		public override void EndInit()
		{
		}

		public override bool Exists(string logName, string machineName)
		{
			return true;
		}

		protected override string FormatMessage(string source, uint messageID, string[] replacementStrings)
		{
			return string.Join(", ", replacementStrings);
		}

		protected override int GetEntryCount()
		{
			return 0;
		}

		protected override EventLogEntry GetEntry(int index)
		{
			return null;
		}

		protected override string GetLogDisplayName()
		{
			return base.CoreEventLog.Log;
		}

		protected override string[] GetLogNames(string machineName)
		{
			return new string[0];
		}

		public override string LogNameFromSourceName(string source, string machineName)
		{
			return null;
		}

		public override bool SourceExists(string source, string machineName)
		{
			return false;
		}

		public override void WriteEntry(string[] replacementStrings, EventLogEntryType type, uint instanceID, short category, byte[] rawData)
		{
		}

		public override OverflowAction OverflowAction
		{
			get
			{
				return OverflowAction.DoNotOverwrite;
			}
		}

		public override int MinimumRetentionDays
		{
			get
			{
				return int.MaxValue;
			}
		}

		public override long MaximumKilobytes
		{
			get
			{
				return long.MaxValue;
			}
			set
			{
				throw new NotSupportedException("This EventLog implementation does not support setting max kilobytes policy");
			}
		}

		public override void ModifyOverflowPolicy(OverflowAction action, int retentionDays)
		{
			throw new NotSupportedException("This EventLog implementation does not support modifying overflow policy");
		}

		public override void RegisterDisplayName(string resourceFile, long resourceId)
		{
			throw new NotSupportedException("This EventLog implementation does not support registering display name");
		}
	}
}
