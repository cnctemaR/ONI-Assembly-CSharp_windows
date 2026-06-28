using System;

namespace FileHelpers.Events
{
	public abstract class FileHelpersEventArgs<T> : EventArgs where T : class
	{
		protected FileHelpersEventArgs(EventEngineBase<T> engine, int lineNumber)
		{
			this.Engine = engine;
			this.LineNumber = lineNumber;
		}

		public EventEngineBase<T> Engine { get; set; }

		public int LineNumber { get; private set; }
	}
}
