using System;

namespace System.Diagnostics
{
	internal class TraceImplSettings
	{
		public TraceImplSettings()
		{
			this.Listeners.Add(new DefaultTraceListener
			{
				IndentSize = this.IndentSize
			});
		}

		public const string Key = ".__TraceInfoSettingsKey__.";

		public bool AutoFlush;

		public int IndentSize = 4;

		public TraceListenerCollection Listeners = new TraceListenerCollection();
	}
}
