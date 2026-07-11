using System;

namespace System.Data
{
	public class DataRowChangeEventArgs : EventArgs
	{
		public DataRowChangeEventArgs(DataRow row, DataRowAction action)
		{
			this.Row = row;
			this.Action = action;
		}

		public DataRow Row { get; }

		public DataRowAction Action { get; }
	}
}
