using System;

namespace System.Data
{
	public sealed class DataRowBuilder
	{
		internal DataRowBuilder(DataTable table, int rowID, int y)
		{
			this.table = table;
			this._rowId = rowID;
		}

		internal DataTable Table
		{
			get
			{
				return this.table;
			}
		}

		private DataTable table;

		internal int _rowId;
	}
}
