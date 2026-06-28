using System;

namespace System.Diagnostics
{
	public class EntryWrittenEventArgs : EventArgs
	{
		public EntryWrittenEventArgs()
			: this(null)
		{
		}

		public EntryWrittenEventArgs(EventLogEntry entry)
		{
			this.entry = entry;
		}

		public EventLogEntry Entry
		{
			get
			{
				return this.entry;
			}
		}

		private EventLogEntry entry;
	}
}
