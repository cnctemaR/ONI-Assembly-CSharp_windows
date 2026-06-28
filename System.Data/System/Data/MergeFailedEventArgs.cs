using System;

namespace System.Data
{
	public class MergeFailedEventArgs : EventArgs
	{
		public MergeFailedEventArgs(DataTable table, string conflict)
		{
			this.data_table = table;
			this.conflict = conflict;
		}

		public DataTable Table
		{
			get
			{
				return this.data_table;
			}
		}

		public string Conflict
		{
			get
			{
				return this.conflict;
			}
		}

		private readonly DataTable data_table;

		private readonly string conflict;
	}
}
