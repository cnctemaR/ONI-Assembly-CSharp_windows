using System;

namespace System.Diagnostics.Tracing
{
	internal class EventListenerHelper : EventListener
	{
		public new static int EventSourceIndex(EventSource eventSource)
		{
			return EventListener.EventSourceIndex(eventSource);
		}

		protected internal override void OnEventWritten(EventWrittenEventArgs eventData)
		{
		}
	}
}
