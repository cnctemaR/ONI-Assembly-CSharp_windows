using System;

namespace FileHelpers.Events
{
	public sealed class BeforeReadEventArgs<T> : ReadEventArgs<T> where T : class
	{
		internal BeforeReadEventArgs(EventEngineBase<T> engine, T record, string line, int lineNumber)
			: base(engine, line, lineNumber)
		{
			this.Record = record;
			base.SkipThisRecord = false;
		}

		public T Record { get; private set; }
	}
}
