using System;
using System.ComponentModel;

namespace FileHelpers.Events
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class WriteEventArgs<T> : FileHelpersEventArgs<T> where T : class
	{
		internal WriteEventArgs(EventEngineBase<T> engine, T record, int lineNumber)
			: base(engine, lineNumber)
		{
			this.Record = record;
		}

		public T Record { get; private set; }
	}
}
