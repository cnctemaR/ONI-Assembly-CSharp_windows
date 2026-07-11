using System;

namespace System.Data
{
	public sealed class DataTableClearEventArgs : EventArgs
	{
		public DataTableClearEventArgs(DataTable dataTable)
		{
			this.Table = dataTable;
		}

		public DataTable Table { get; }

		public string TableName
		{
			get
			{
				return this.Table.TableName;
			}
		}

		public string TableNamespace
		{
			get
			{
				return this.Table.Namespace;
			}
		}
	}
}
