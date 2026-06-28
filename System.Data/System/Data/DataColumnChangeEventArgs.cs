using System;

namespace System.Data
{
	public class DataColumnChangeEventArgs : EventArgs
	{
		public DataColumnChangeEventArgs(DataRow row, DataColumn column, object value)
		{
			this.Initialize(row, column, value);
		}

		internal DataColumnChangeEventArgs()
		{
		}

		public DataColumn Column
		{
			get
			{
				return this._column;
			}
		}

		public object ProposedValue
		{
			get
			{
				return this._proposedValue;
			}
			set
			{
				this._proposedValue = value;
			}
		}

		public DataRow Row
		{
			get
			{
				return this._row;
			}
		}

		internal void Initialize(DataRow row, DataColumn column, object value)
		{
			this._column = column;
			this._row = row;
			this._proposedValue = value;
		}

		private DataColumn _column;

		private DataRow _row;

		private object _proposedValue;
	}
}
