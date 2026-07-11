using System;

namespace UnityEngine.UIElements
{
	internal class EventDebuggerTrace
	{
		public EventDebuggerEventRecord eventBase { get; }

		public IEventHandler focusedElement { get; }

		public IEventHandler mouseCapture { get; }

		public long duration { get; set; }

		public EventDebuggerTrace(IPanel panel, EventBase evt, long duration, IEventHandler mouseCapture)
		{
			this.eventBase = new EventDebuggerEventRecord(evt);
			IEventHandler eventHandler;
			if (panel == null)
			{
				eventHandler = null;
			}
			else
			{
				FocusController focusController = panel.focusController;
				eventHandler = ((focusController != null) ? focusController.focusedElement : null);
			}
			this.focusedElement = eventHandler;
			this.mouseCapture = mouseCapture;
			this.duration = duration;
		}
	}
}
