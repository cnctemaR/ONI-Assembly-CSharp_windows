using System;

namespace UnityEngine.UIElements.Experimental
{
	internal class EventDebuggerCallTrace : EventDebuggerTrace
	{
		public int callbackHashCode { get; }

		public string callbackName { get; }

		public bool propagationHasStopped { get; }

		public bool immediatePropagationHasStopped { get; }

		public EventDebuggerCallTrace(IPanel panel, EventBase evt, int cbHashCode, string cbName, bool propagationHasStopped, bool immediatePropagationHasStopped, long duration, IEventHandler mouseCapture)
			: base(panel, evt, duration, mouseCapture)
		{
			this.callbackHashCode = cbHashCode;
			this.callbackName = cbName;
			this.propagationHasStopped = propagationHasStopped;
			this.immediatePropagationHasStopped = immediatePropagationHasStopped;
		}
	}
}
