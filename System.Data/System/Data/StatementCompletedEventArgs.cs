using System;

namespace System.Data
{
	public sealed class StatementCompletedEventArgs : EventArgs
	{
		public StatementCompletedEventArgs(int recordCount)
		{
			this.recordCount = recordCount;
		}

		public int RecordCount
		{
			get
			{
				return this.recordCount;
			}
		}

		private int recordCount;
	}
}
