using System;

namespace FileHelpers.Events
{
	public sealed class BeforeWriteEventArgs<T> : WriteEventArgs<T> where T : class
	{
		internal BeforeWriteEventArgs(EventEngineBase<T> engine, T record, int lineNumber)
			: base(engine, record, lineNumber)
		{
			this.SkipThisRecord = false;
		}

		public bool SkipThisRecord { get; set; }
	}
}
