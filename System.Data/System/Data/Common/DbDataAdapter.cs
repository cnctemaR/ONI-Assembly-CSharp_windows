using System;
using System.ComponentModel;

namespace System.Data.Common
{
	public abstract class DbDataAdapter : DataAdapter, IDataAdapter, IDbDataAdapter, ICloneable
	{
		protected DbDataAdapter()
		{
		}

		protected DbDataAdapter(DbDataAdapter adapter)
		{
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbCommand DeleteCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		protected internal CommandBehavior FillCommandBehavior
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbCommand InsertCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbCommand SelectCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		IDbCommand IDbDataAdapter.InsertCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		IDbCommand IDbDataAdapter.SelectCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		IDbCommand IDbDataAdapter.UpdateCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(1)]
		public virtual int UpdateBatchSize
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbCommand UpdateCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		protected virtual int AddToBatch(IDbCommand command)
		{
			throw null;
		}

		protected virtual void ClearBatch()
		{
		}

		protected virtual RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			throw null;
		}

		protected virtual RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
		}

		protected virtual int ExecuteBatch()
		{
			throw null;
		}

		public override int Fill(DataSet dataSet)
		{
			throw null;
		}

		public int Fill(DataSet dataSet, int startRecord, int maxRecords, string srcTable)
		{
			throw null;
		}

		protected virtual int Fill(DataSet dataSet, int startRecord, int maxRecords, string srcTable, IDbCommand command, CommandBehavior behavior)
		{
			throw null;
		}

		public int Fill(DataSet dataSet, string srcTable)
		{
			throw null;
		}

		public int Fill(DataTable dataTable)
		{
			throw null;
		}

		protected virtual int Fill(DataTable dataTable, IDbCommand command, CommandBehavior behavior)
		{
			throw null;
		}

		[MonoTODO]
		protected virtual int Fill(DataTable[] dataTables, int startRecord, int maxRecords, IDbCommand command, CommandBehavior behavior)
		{
			throw null;
		}

		[MonoTODO]
		public int Fill(int startRecord, int maxRecords, params DataTable[] dataTables)
		{
			throw null;
		}

		public override DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
		{
			throw null;
		}

		protected virtual DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType, IDbCommand command, string srcTable, CommandBehavior behavior)
		{
			throw null;
		}

		public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType, string srcTable)
		{
			throw null;
		}

		public DataTable FillSchema(DataTable dataTable, SchemaType schemaType)
		{
			throw null;
		}

		protected virtual DataTable FillSchema(DataTable dataTable, SchemaType schemaType, IDbCommand command, CommandBehavior behavior)
		{
			throw null;
		}

		protected virtual IDataParameter GetBatchedParameter(int commandIdentifier, int parameterIndex)
		{
			throw null;
		}

		protected virtual bool GetBatchedRecordsAffected(int commandIdentifier, out int recordsAffected, out Exception error)
		{
			recordsAffected = 0;
			error = null;
			throw null;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public override IDataParameter[] GetFillParameters()
		{
			throw null;
		}

		protected virtual void InitializeBatching()
		{
		}

		protected virtual void OnRowUpdated(RowUpdatedEventArgs value)
		{
		}

		protected virtual void OnRowUpdating(RowUpdatingEventArgs value)
		{
		}

		[MonoTODO]
		[Obsolete("use 'protected DbDataAdapter(DbDataAdapter)' ctor")]
		object ICloneable.Clone()
		{
			throw null;
		}

		protected virtual void TerminateBatching()
		{
		}

		public int Update(DataRow[] dataRows)
		{
			throw null;
		}

		protected virtual int Update(DataRow[] dataRows, DataTableMapping tableMapping)
		{
			throw null;
		}

		public override int Update(DataSet dataSet)
		{
			throw null;
		}

		public int Update(DataSet dataSet, string srcTable)
		{
			throw null;
		}

		public int Update(DataTable dataTable)
		{
			throw null;
		}

		public const string DefaultSourceTableName = "Table";
	}
}
