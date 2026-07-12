using System;

namespace System.Diagnostics.Tracing
{
	public abstract class DiagnosticCounter : IDisposable
	{
		internal DiagnosticCounter(string name, EventSource eventSource)
		{
		}

		internal DiagnosticCounter()
		{
		}

		public string DisplayName { get; set; }

		public string DisplayUnits { get; set; }

		public EventSource EventSource { get; }

		public string Name { get; }

		public void AddMetadata(string key, string value)
		{
		}

		public void Dispose()
		{
		}
	}
}
