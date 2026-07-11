using System;

namespace System.Data
{
	public class DataRowChangeEventArgs : EventArgs
	{
		public DataRowChangeEventArgs(DataRow row, DataRowAction action)
		{
		}

		public DataRowAction Action
		{
			get
			{
				throw null;
			}
		}

		public DataRow Row
		{
			get
			{
				throw null;
			}
		}
	}
}
