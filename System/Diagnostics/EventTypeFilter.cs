using System;

namespace System.Diagnostics
{
	public class EventTypeFilter : TraceFilter
	{
		public EventTypeFilter(SourceLevels eventType)
		{
			this.event_type = eventType;
		}

		public SourceLevels EventType
		{
			get
			{
				return this.event_type;
			}
			set
			{
				this.event_type = value;
			}
		}

		public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
		{
			switch (eventType)
			{
			case TraceEventType.Critical:
				return (this.event_type & SourceLevels.Critical) != SourceLevels.Off;
			case TraceEventType.Error:
				return (this.event_type & SourceLevels.Error) != SourceLevels.Off;
			default:
				if (eventType == TraceEventType.Verbose)
				{
					return (this.event_type & SourceLevels.Verbose) != SourceLevels.Off;
				}
				if (eventType != TraceEventType.Start && eventType != TraceEventType.Stop && eventType != TraceEventType.Suspend && eventType != TraceEventType.Resume && eventType != TraceEventType.Transfer)
				{
					return this.event_type != SourceLevels.Off;
				}
				return (this.event_type & SourceLevels.ActivityTracing) != SourceLevels.Off;
			case TraceEventType.Warning:
				return (this.event_type & SourceLevels.Warning) != SourceLevels.Off;
			case TraceEventType.Information:
				return (this.event_type & SourceLevels.Information) != SourceLevels.Off;
			}
		}

		private SourceLevels event_type;
	}
}
