using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Diagnostics
{
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	public sealed class EventLogTraceListener : TraceListener
	{
		public EventLogTraceListener()
		{
		}

		public EventLogTraceListener(EventLog eventLog)
		{
			if (eventLog == null)
			{
				throw new ArgumentNullException("eventLog");
			}
			this.event_log = eventLog;
		}

		public EventLogTraceListener(string source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			this.event_log = new EventLog();
			this.event_log.Source = source;
		}

		public EventLog EventLog
		{
			get
			{
				return this.event_log;
			}
			set
			{
				this.event_log = value;
			}
		}

		public override string Name
		{
			get
			{
				if (this.name == null)
				{
					return this.event_log.Source;
				}
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public override void Close()
		{
			this.event_log.Close();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.event_log.Dispose();
			}
		}

		public override void Write(string message)
		{
			this.TraceData(new TraceEventCache(), this.event_log.Source, TraceEventType.Information, 0, message);
		}

		public override void WriteLine(string message)
		{
			this.Write(message);
		}

		[ComVisible(false)]
		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType severity, int id, object data)
		{
			EventLogEntryType eventLogEntryType;
			if (severity - TraceEventType.Critical > 1)
			{
				if (severity != TraceEventType.Warning)
				{
					eventLogEntryType = EventLogEntryType.Information;
				}
				else
				{
					eventLogEntryType = EventLogEntryType.Warning;
				}
			}
			else
			{
				eventLogEntryType = EventLogEntryType.Error;
			}
			this.event_log.WriteEntry((data != null) ? data.ToString() : string.Empty, eventLogEntryType, id, 0);
		}

		[ComVisible(false)]
		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType severity, int id, params object[] data)
		{
			string text = string.Empty;
			if (data != null)
			{
				string[] array = new string[data.Length];
				for (int i = 0; i < data.Length; i++)
				{
					array[i] = ((data[i] != null) ? data[i].ToString() : string.Empty);
				}
				text = string.Join(", ", array);
			}
			this.TraceData(eventCache, source, severity, id, text);
		}

		[ComVisible(false)]
		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType severity, int id, string message)
		{
			this.TraceData(eventCache, source, severity, id, message);
		}

		[ComVisible(false)]
		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType severity, int id, string format, params object[] args)
		{
			this.TraceEvent(eventCache, source, severity, id, (format != null) ? string.Format(format, args) : null);
		}

		private EventLog event_log;

		private string name;
	}
}
