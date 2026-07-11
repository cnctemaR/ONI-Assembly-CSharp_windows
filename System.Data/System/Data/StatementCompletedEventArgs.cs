using System;

namespace System.Data
{
	public sealed class StatementCompletedEventArgs : EventArgs
	{
		public StatementCompletedEventArgs(int recordCount)
		{
		}

		public int RecordCount
		{
			get
			{
				throw null;
			}
		}
	}
}
