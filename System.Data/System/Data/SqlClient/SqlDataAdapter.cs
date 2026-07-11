using System;
using System.Data.Common;

namespace System.Data.SqlClient
{
	public sealed class SqlDataAdapter : DbDataAdapter, IDbDataAdapter, IDataAdapter, ICloneable
	{
		public SqlDataAdapter()
		{
			GC.SuppressFinalize(this);
		}

		public SqlDataAdapter(SqlCommand selectCommand)
			: this()
		{
			this.SelectCommand = selectCommand;
		}

		public SqlDataAdapter(string selectCommandText, string selectConnectionString)
			: this()
		{
			SqlConnection sqlConnection = new SqlConnection(selectConnectionString);
			this.SelectCommand = new SqlCommand(selectCommandText, sqlConnection);
		}

		public SqlDataAdapter(string selectCommandText, SqlConnection selectConnection)
			: this()
		{
			this.SelectCommand = new SqlCommand(selectCommandText, selectConnection);
		}

		private SqlDataAdapter(SqlDataAdapter from)
			: base(from)
		{
			GC.SuppressFinalize(this);
		}

		public new SqlCommand DeleteCommand
		{
			get
			{
				return this._deleteCommand;
			}
			set
			{
				this._deleteCommand = value;
			}
		}

		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get
			{
				return this._deleteCommand;
			}
			set
			{
				this._deleteCommand = (SqlCommand)value;
			}
		}

		public new SqlCommand InsertCommand
		{
			get
			{
				return this._insertCommand;
			}
			set
			{
				this._insertCommand = value;
			}
		}

		IDbCommand IDbDataAdapter.InsertCommand
		{
			get
			{
				return this._insertCommand;
			}
			set
			{
				this._insertCommand = (SqlCommand)value;
			}
		}

		public new SqlCommand SelectCommand
		{
			get
			{
				return this._selectCommand;
			}
			set
			{
				this._selectCommand = value;
			}
		}

		IDbCommand IDbDataAdapter.SelectCommand
		{
			get
			{
				return this._selectCommand;
			}
			set
			{
				this._selectCommand = (SqlCommand)value;
			}
		}

		public new SqlCommand UpdateCommand
		{
			get
			{
				return this._updateCommand;
			}
			set
			{
				this._updateCommand = value;
			}
		}

		IDbCommand IDbDataAdapter.UpdateCommand
		{
			get
			{
				return this._updateCommand;
			}
			set
			{
				this._updateCommand = (SqlCommand)value;
			}
		}

		public override int UpdateBatchSize
		{
			get
			{
				return this._updateBatchSize;
			}
			set
			{
				if (0 > value)
				{
					throw ADP.ArgumentOutOfRange("UpdateBatchSize");
				}
				this._updateBatchSize = value;
			}
		}

		protected override int AddToBatch(IDbCommand command)
		{
			int commandCount = this._commandSet.CommandCount;
			this._commandSet.Append((SqlCommand)command);
			return commandCount;
		}

		protected override void ClearBatch()
		{
			this._commandSet.Clear();
		}

		protected override int ExecuteBatch()
		{
			return this._commandSet.ExecuteNonQuery();
		}

		protected override IDataParameter GetBatchedParameter(int commandIdentifier, int parameterIndex)
		{
			return this._commandSet.GetParameter(commandIdentifier, parameterIndex);
		}

		protected override bool GetBatchedRecordsAffected(int commandIdentifier, out int recordsAffected, out Exception error)
		{
			return this._commandSet.GetBatchedAffected(commandIdentifier, out recordsAffected, out error);
		}

		protected override void InitializeBatching()
		{
			this._commandSet = new SqlCommandSet();
			SqlCommand sqlCommand = this.SelectCommand;
			if (sqlCommand == null)
			{
				sqlCommand = this.InsertCommand;
				if (sqlCommand == null)
				{
					sqlCommand = this.UpdateCommand;
					if (sqlCommand == null)
					{
						sqlCommand = this.DeleteCommand;
					}
				}
			}
			if (sqlCommand != null)
			{
				this._commandSet.Connection = sqlCommand.Connection;
				this._commandSet.Transaction = sqlCommand.Transaction;
				this._commandSet.CommandTimeout = sqlCommand.CommandTimeout;
			}
		}

		protected override void TerminateBatching()
		{
			if (this._commandSet != null)
			{
				this._commandSet.Dispose();
				this._commandSet = null;
			}
		}

		object ICloneable.Clone()
		{
			return new SqlDataAdapter(this);
		}

		protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new SqlRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
		}

		protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new SqlRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
		}

		public event SqlRowUpdatedEventHandler RowUpdated
		{
			add
			{
				base.Events.AddHandler(SqlDataAdapter.EventRowUpdated, value);
			}
			remove
			{
				base.Events.RemoveHandler(SqlDataAdapter.EventRowUpdated, value);
			}
		}

		public event SqlRowUpdatingEventHandler RowUpdating
		{
			add
			{
				SqlRowUpdatingEventHandler sqlRowUpdatingEventHandler = (SqlRowUpdatingEventHandler)base.Events[SqlDataAdapter.EventRowUpdating];
				if (sqlRowUpdatingEventHandler != null && value.Target is DbCommandBuilder)
				{
					SqlRowUpdatingEventHandler sqlRowUpdatingEventHandler2 = (SqlRowUpdatingEventHandler)ADP.FindBuilder(sqlRowUpdatingEventHandler);
					if (sqlRowUpdatingEventHandler2 != null)
					{
						base.Events.RemoveHandler(SqlDataAdapter.EventRowUpdating, sqlRowUpdatingEventHandler2);
					}
				}
				base.Events.AddHandler(SqlDataAdapter.EventRowUpdating, value);
			}
			remove
			{
				base.Events.RemoveHandler(SqlDataAdapter.EventRowUpdating, value);
			}
		}

		protected override void OnRowUpdated(RowUpdatedEventArgs value)
		{
			SqlRowUpdatedEventHandler sqlRowUpdatedEventHandler = (SqlRowUpdatedEventHandler)base.Events[SqlDataAdapter.EventRowUpdated];
			if (sqlRowUpdatedEventHandler != null && value is SqlRowUpdatedEventArgs)
			{
				sqlRowUpdatedEventHandler(this, (SqlRowUpdatedEventArgs)value);
			}
			base.OnRowUpdated(value);
		}

		protected override void OnRowUpdating(RowUpdatingEventArgs value)
		{
			SqlRowUpdatingEventHandler sqlRowUpdatingEventHandler = (SqlRowUpdatingEventHandler)base.Events[SqlDataAdapter.EventRowUpdating];
			if (sqlRowUpdatingEventHandler != null && value is SqlRowUpdatingEventArgs)
			{
				sqlRowUpdatingEventHandler(this, (SqlRowUpdatingEventArgs)value);
			}
			base.OnRowUpdating(value);
		}

		private static readonly object EventRowUpdated = new object();

		private static readonly object EventRowUpdating = new object();

		private SqlCommand _deleteCommand;

		private SqlCommand _insertCommand;

		private SqlCommand _selectCommand;

		private SqlCommand _updateCommand;

		private SqlCommandSet _commandSet;

		private int _updateBatchSize = 1;
	}
}
