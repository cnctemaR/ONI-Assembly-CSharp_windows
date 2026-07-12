using System;
using System.Collections.ObjectModel;
using Unity;

namespace System.Diagnostics.Tracing
{
	public class EventWrittenEventArgs : EventArgs
	{
		internal EventWrittenEventArgs(EventSource eventSource)
		{
			this.EventSource = eventSource;
		}

		public Guid ActivityId
		{
			get
			{
				return EventSource.CurrentThreadActivityId;
			}
		}

		public EventChannel Channel
		{
			get
			{
				return EventChannel.None;
			}
		}

		public int EventId { get; internal set; }

		public long OSThreadId { get; internal set; }

		public DateTime TimeStamp { get; internal set; }

		public string EventName { get; internal set; }

		public EventSource EventSource { get; private set; }

		public EventKeywords Keywords
		{
			get
			{
				return EventKeywords.None;
			}
		}

		public EventLevel Level
		{
			get
			{
				return EventLevel.LogAlways;
			}
		}

		public string Message { get; internal set; }

		public EventOpcode Opcode
		{
			get
			{
				return EventOpcode.Info;
			}
		}

		public ReadOnlyCollection<object> Payload { get; internal set; }

		public ReadOnlyCollection<string> PayloadNames { get; internal set; }

		public Guid RelatedActivityId { get; internal set; }

		public EventTags Tags
		{
			get
			{
				return EventTags.None;
			}
		}

		public EventTask Task
		{
			get
			{
				return EventTask.None;
			}
		}

		public byte Version
		{
			get
			{
				return 0;
			}
		}

		internal EventWrittenEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
