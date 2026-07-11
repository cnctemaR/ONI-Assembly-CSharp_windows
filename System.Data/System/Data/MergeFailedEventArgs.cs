using System;

namespace System.Data
{
	public class MergeFailedEventArgs : EventArgs
	{
		public MergeFailedEventArgs(DataTable table, string conflict)
		{
		}

		public string Conflict
		{
			get
			{
				throw null;
			}
		}

		public DataTable Table
		{
			get
			{
				throw null;
			}
		}
	}
}
