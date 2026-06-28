using System;

namespace FileHelpers.Events
{
	public sealed class AfterReadEventArgs<T> : ReadEventArgs<T> where T : class
	{
		internal AfterReadEventArgs(EventEngineBase<T> engine, string line, bool lineChanged, T newRecord, int lineNumber)
			: base(engine, line, lineNumber)
		{
			base.SkipThisRecord = false;
			this.Record = newRecord;
			base.RecordLineChanged = lineChanged;
		}

		public T Record { get; set; }
	}
}
