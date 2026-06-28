using System;

namespace FileHelpers.Events
{
	public sealed class AfterWriteEventArgs<T> : WriteEventArgs<T> where T : class
	{
		internal AfterWriteEventArgs(EventEngineBase<T> engine, T record, int lineNumber, string line)
			: base(engine, record, lineNumber)
		{
			this.RecordLine = line;
		}

		public string RecordLine { get; set; }
	}
}
