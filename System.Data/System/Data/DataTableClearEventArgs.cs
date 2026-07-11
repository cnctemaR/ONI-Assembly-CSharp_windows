using System;

namespace System.Data
{
	public sealed class DataTableClearEventArgs : EventArgs
	{
		public DataTableClearEventArgs(DataTable dataTable)
		{
		}

		public DataTable Table
		{
			get
			{
				throw null;
			}
		}

		public string TableName
		{
			get
			{
				throw null;
			}
		}

		public string TableNamespace
		{
			get
			{
				throw null;
			}
		}
	}
}
