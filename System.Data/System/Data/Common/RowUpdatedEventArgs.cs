using System;

namespace System.Data.Common
{
	public class RowUpdatedEventArgs : EventArgs
	{
		public RowUpdatedEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			if (statementType > StatementType.Batch)
			{
				throw ADP.InvalidStatementType(statementType);
			}
			this._dataRow = dataRow;
			this._command = command;
			this._statementType = statementType;
			this._tableMapping = tableMapping;
		}

		public IDbCommand Command
		{
			get
			{
				return this._command;
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

		public int RecordsAffected
		{
			get
			{
				return this._recordsAffected;
			}
		}

		public DataRow Row
		{
			get
			{
				return this._dataRow;
			}
		}

		internal DataRow[] Rows
		{
			get
			{
				return this._dataRows;
			}
		}

		public int RowCount
		{
			get
			{
				DataRow[] dataRows = this._dataRows;
				if (dataRows != null)
				{
					return dataRows.Length;
				}
				if (this._dataRow == null)
				{
					return 0;
				}
				return 1;
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

		internal void AdapterInit(DataRow[] dataRows)
		{
			this._statementType = StatementType.Batch;
			this._dataRows = dataRows;
			if (dataRows != null && 1 == dataRows.Length)
			{
				this._dataRow = dataRows[0];
			}
		}

		internal void AdapterInit(int recordsAffected)
		{
			this._recordsAffected = recordsAffected;
		}

		public void CopyToRows(DataRow[] array)
		{
			this.CopyToRows(array, 0);
		}

		public void CopyToRows(DataRow[] array, int arrayIndex)
		{
			DataRow[] dataRows = this._dataRows;
			if (dataRows != null)
			{
				dataRows.CopyTo(array, arrayIndex);
				return;
			}
			if (array == null)
			{
				throw ADP.ArgumentNull("array");
			}
			array[arrayIndex] = this.Row;
		}

		private IDbCommand _command;

		private StatementType _statementType;

		private DataTableMapping _tableMapping;

		private Exception _errors;

		private DataRow _dataRow;

		private DataRow[] _dataRows;

		private UpdateStatus _status;

		private int _recordsAffected;
	}
}
