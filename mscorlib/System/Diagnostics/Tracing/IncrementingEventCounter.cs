using System;

namespace System.Diagnostics.Tracing
{
	public class IncrementingEventCounter : DiagnosticCounter
	{
		public IncrementingEventCounter(string name, EventSource eventSource)
			: base(name, eventSource)
		{
		}

		public void Increment(double increment = 1.0)
		{
		}

		public TimeSpan DisplayRateTimeScale { get; set; }
	}
}
