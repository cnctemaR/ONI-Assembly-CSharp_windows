using System;

namespace System.Data.Common
{
	public class RowUpdatingEventArgs : EventArgs
	{
		public RowUpdatingEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			ADP.CheckArgumentNull(dataRow, "dataRow");
			ADP.CheckArgumentNull(tableMapping, "tableMapping");
			if (statementType <= StatementType.Delete)
			{
				this._dataRow = dataRow;
				this._command = command;
				this._statementType = statementType;
				this._tableMapping = tableMapping;
				return;
			}
			if (statementType == StatementType.Batch)
			{
				throw ADP.NotSupportedStatementType(statementType, "RowUpdatingEventArgs");
			}
			throw ADP.InvalidStatementType(statementType);
		}

		protected virtual IDbCommand BaseCommand
		{
			get
			{
				return this._command;
			}
			set
			{
				this._command = value;
			}
		}

		public IDbCommand Command
		{
			get
			{
				return this.BaseCommand;
			}
			set
			{
				this.BaseCommand = value;
			}
		}

		public Exception Errors
		{
			get
			{
				return this._errors;
			}
			set
			{
				this._errors = value;
			}
		}

		public DataRow Row
		{
			get
			{
				return this._dataRow;
			}
		}

		public StatementType StatementType
		{
			get
			{
				return this._statementType;
			}
		}

		public UpdateStatus Status
		{
			get
			{
				return this._status;
			}
			set
			{
				if (value <= UpdateStatus.SkipAllRemainingRows)
				{
					this._status = value;
					return;
				}
				throw ADP.InvalidUpdateStatus(value);
			}
		}

		public DataTableMapping TableMapping
		{
			get
			{
				return this._tableMapping;
			}
		}

		private IDbCommand _command;

		private StatementType _statementType;

		private DataTableMapping _tableMapping;

		private Exception _errors;

		private DataRow _dataRow;

		private UpdateStatus _status;
	}
}
