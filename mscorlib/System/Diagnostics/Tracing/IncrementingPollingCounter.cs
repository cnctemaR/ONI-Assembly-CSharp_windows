using System;

namespace System.Diagnostics.Tracing
{
	public class IncrementingPollingCounter : DiagnosticCounter
	{
		public IncrementingPollingCounter(string name, EventSource eventSource, Func<double> totalValueProvider)
			: base(name, eventSource)
		{
		}

		public TimeSpan DisplayRateTimeScale { get; set; }
	}
}
