using System;

namespace System.Data.Common
{
	public class RowUpdatingEventArgs : EventArgs
	{
		public RowUpdatingEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			this.dataRow = dataRow;
			this.command = command;
			this.statementType = statementType;
			this.tableMapping = tableMapping;
			this.status = UpdateStatus.Continue;
			this.errors = null;
		}

		public IDbCommand Command
		{
			get
			{
				return this.command;
			}
			set
			{
				this.command = value;
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

		protected virtual IDbCommand BaseCommand
		{
			get
			{
				return this.command;
			}
			set
			{
				this.command = value;
			}
		}

		private DataRow dataRow;

		private IDbCommand command;

		private StatementType statementType;

		private DataTableMapping tableMapping;

		private UpdateStatus status;

		private Exception errors;
	}
}
