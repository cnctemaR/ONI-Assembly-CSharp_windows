using System;

namespace System.Diagnostics
{
	internal class TraceSourceInfo
	{
		public TraceSourceInfo(string name, SourceLevels levels)
		{
			this.name = name;
			this.levels = levels;
			this.listeners = new TraceListenerCollection();
		}

		internal TraceSourceInfo(string name, SourceLevels levels, TraceImplSettings settings)
		{
			this.name = name;
			this.levels = levels;
			this.listeners = new TraceListenerCollection();
			this.listeners.Add(new DefaultTraceListener
			{
				IndentSize = settings.IndentSize
			});
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public SourceLevels Levels
		{
			get
			{
				return this.levels;
			}
		}

		public TraceListenerCollection Listeners
		{
			get
			{
				return this.listeners;
			}
		}

		private string name;

		private SourceLevels levels;

		private TraceListenerCollection listeners;
	}
}
