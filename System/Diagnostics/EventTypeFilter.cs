using System;

namespace System.Diagnostics
{
	public class EventTypeFilter : TraceFilter
	{
		public EventTypeFilter(SourceLevels level)
		{
			this.level = level;
		}

		public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
		{
			return (eventType & (TraceEventType)this.level) > (TraceEventType)0;
		}

		public SourceLevels EventType
		{
			get
			{
				return this.level;
			}
			set
			{
				this.level = value;
			}
		}

		private SourceLevels level;
	}
}
