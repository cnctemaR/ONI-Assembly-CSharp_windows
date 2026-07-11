using System;

namespace System.Data
{
	public class MergeFailedEventArgs : EventArgs
	{
		public MergeFailedEventArgs(DataTable table, string conflict)
		{
			this.Table = table;
			this.Conflict = conflict;
		}

		public DataTable Table { get; }

		public string Conflict { get; }
	}
}
