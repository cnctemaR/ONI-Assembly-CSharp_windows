using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Diagnostics
{
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
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
				return (this.name == null) ? this.event_log.Source : this.name;
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
		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			EventLogEntryType eventLogEntryType;
			switch (eventType)
			{
			case TraceEventType.Critical:
			case TraceEventType.Error:
				eventLogEntryType = EventLogEntryType.Error;
				goto IL_0034;
			case TraceEventType.Warning:
				eventLogEntryType = EventLogEntryType.Warning;
				goto IL_0034;
			}
			eventLogEntryType = EventLogEntryType.Information;
			IL_0034:
			this.event_log.WriteEntry((data == null) ? string.Empty : data.ToString(), eventLogEntryType, id, 0);
		}

		[ComVisible(false)]
		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			string text = string.Empty;
			if (data != null)
			{
				string[] array = new string[data.Length];
				for (int i = 0; i < data.Length; i++)
				{
					array[i] = ((data[i] == null) ? string.Empty : data[i].ToString());
				}
				text = string.Join(", ", array);
			}
			this.TraceData(eventCache, source, eventType, id, text);
		}

		[ComVisible(false)]
		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			this.TraceData(eventCache, source, eventType, id, message);
		}

		[ComVisible(false)]
		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			this.TraceEvent(eventCache, source, eventType, id, (format == null) ? null : string.Format(format, args));
		}

		private EventLog event_log;

		private string name;
	}
}
