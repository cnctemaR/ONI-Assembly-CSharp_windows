using System;

namespace System.Data
{
	public sealed class StatementCompletedEventArgs : EventArgs
	{
		public StatementCompletedEventArgs(int recordCount)
		{
			this.RecordCount = recordCount;
		}

		public int RecordCount { get; }
	}
}
