using System;

namespace System.Configuration
{
	internal class ConfigurationSaveEventArgs : EventArgs
	{
		public string StreamPath { get; private set; }

		public bool Start { get; private set; }

		public object Context { get; private set; }

		public bool Failed { get; private set; }

		public Exception Exception { get; private set; }

		public ConfigurationSaveEventArgs(string streamPath, bool start, Exception ex, object context)
		{
			this.StreamPath = streamPath;
			this.Start = start;
			this.Failed = ex != null;
			this.Exception = ex;
			this.Context = context;
		}
	}
}
