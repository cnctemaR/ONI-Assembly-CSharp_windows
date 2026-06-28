using System;

namespace System.Data
{
	public class FillErrorEventArgs : EventArgs
	{
		public FillErrorEventArgs(DataTable dataTable, object[] values)
		{
			this.data_table = dataTable;
			this.values = values;
		}

		public bool Continue
		{
			get
			{
				return this.f_continue;
			}
			set
			{
				this.f_continue = value;
			}
		}

		public DataTable DataTable
		{
			get
			{
				return this.data_table;
			}
		}

		public Exception Errors
		{
			get
			{
				return this.errors;
			}
			set
			{
				this.errors = value;
			}
		}

		public object[] Values
		{
			get
			{
				return this.values;
			}
		}

		private DataTable data_table;

		private object[] values;

		private Exception errors;

		private bool f_continue;
	}
}
