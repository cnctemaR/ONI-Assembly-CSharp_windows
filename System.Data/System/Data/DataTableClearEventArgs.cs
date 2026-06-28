using System;

namespace System.Data
{
	public sealed class DataTableClearEventArgs : EventArgs
	{
		public DataTableClearEventArgs(DataTable table)
		{
			this._table = table;
		}

		public DataTable Table
		{
			get
			{
				return this._table;
			}
		}

		public string TableName
		{
			get
			{
				return this._table.TableName;
			}
		}

		public string TableNamespace
		{
			get
			{
				return this._table.Namespace;
			}
		}

		private readonly DataTable _table;
	}
}
