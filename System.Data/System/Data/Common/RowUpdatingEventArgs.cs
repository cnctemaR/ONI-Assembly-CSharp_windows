using System;

namespace System.Data.Common
{
	public class RowUpdatingEventArgs : EventArgs
	{
		public RowUpdatingEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
		}

		protected virtual IDbCommand BaseCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public IDbCommand Command
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public Exception Errors
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public DataRow Row
		{
			get
			{
				throw null;
			}
		}

		public StatementType StatementType
		{
			get
			{
				throw null;
			}
		}

		public UpdateStatus Status
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public DataTableMapping TableMapping
		{
			get
			{
				throw null;
			}
		}
	}
}
