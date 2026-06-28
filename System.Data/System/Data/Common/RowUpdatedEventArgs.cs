using System;

namespace System.Data.Common
{
	public class RowUpdatedEventArgs : EventArgs
	{
		public RowUpdatedEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			this.dataRow = dataRow;
			this.command = command;
			this.statementType = statementType;
			this.tableMapping = tableMapping;
			this.status = UpdateStatus.Continue;
		}

		public IDbCommand Command
		{
			get
			{
				return this.command;
			}
		}

		public Exception Errors
		{
			get
			{
				if (this.errors == null)
				{
					this.errors = new DataException("RowUpdatedEvent: No additional information is available!");
				}
				return this.errors;
			}
			set
			{
				this.errors = value;
			}
		}

		public int RecordsAffected
		{
			get
			{
				return this.recordsAffected;
			}
		}

		public DataRow Row
		{
			get
			{
				return this.dataRow;
			}
		}

		public StatementType StatementType
		{
			get
			{
				return this.statementType;
			}
		}

		public UpdateStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		public DataTableMapping TableMapping
		{
			get
			{
				return this.tableMapping;
			}
		}

		public int RowCount
		{
			get
			{
				return 0;
			}
		}

		public void CopyToRows(DataRow[] array)
		{
		}

		public void CopyToRows(DataRow[] array, int arrayIndex)
		{
		}

		private DataRow dataRow;

		private IDbCommand command;

		private StatementType statementType;

		private DataTableMapping tableMapping;

		private Exception errors;

		private UpdateStatus status;

		private int recordsAffected;
	}
}
