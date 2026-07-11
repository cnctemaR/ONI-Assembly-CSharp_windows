using System;

namespace System.Data
{
	public class DataColumnChangeEventArgs : EventArgs
	{
		internal DataColumnChangeEventArgs(DataRow row)
		{
			this.Row = row;
		}

		public DataColumnChangeEventArgs(DataRow row, DataColumn column, object value)
		{
			this.Row = row;
			this._column = column;
			this.ProposedValue = value;
		}

		public DataColumn Column
		{
			get
			{
				return this._column;
			}
		}

		public DataRow Row { get; }

		public object ProposedValue { get; set; }

		internal void InitializeColumnChangeEvent(DataColumn column, object value)
		{
			this._column = column;
			this.ProposedValue = value;
		}

		private DataColumn _column;
	}
}
