using System;

namespace System.Data
{
	public class DataRowChangeEventArgs : EventArgs
	{
		public DataRowChangeEventArgs(DataRow row, DataRowAction action)
		{
			this.row = row;
			this.action = action;
		}

		public DataRowAction Action
		{
			get
			{
				return this.action;
			}
		}

		public DataRow Row
		{
			get
			{
				return this.row;
			}
		}

		private DataRow row;

		private DataRowAction action;
	}
}
