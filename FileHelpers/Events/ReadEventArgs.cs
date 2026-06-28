using System;
using System.ComponentModel;

namespace FileHelpers.Events
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class ReadEventArgs<T> : FileHelpersEventArgs<T> where T : class
	{
		internal ReadEventArgs(EventEngineBase<T> engine, string line, int lineNumber)
			: base(engine, lineNumber)
		{
			this.RecordLineChanged = false;
			this.mRecordLine = line;
		}

		public string RecordLine
		{
			get
			{
				return this.mRecordLine;
			}
			set
			{
				if (this.mRecordLine == value)
				{
					return;
				}
				this.mRecordLine = value;
				this.RecordLineChanged = true;
			}
		}

		public bool RecordLineChanged { get; protected set; }

		public bool SkipThisRecord { get; set; }

		private string mRecordLine;
	}
}
