using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace System.Data.Common
{
	public abstract class DbDataAdapter : DataAdapter, IDataAdapter, IDbDataAdapter, ICloneable
	{
		protected DbDataAdapter()
		{
		}

		protected DbDataAdapter(DbDataAdapter adapter)
			: base(adapter)
		{
		}

		IDbCommand IDbDataAdapter.SelectCommand
		{
			get
			{
				return this.SelectCommand;
			}
			set
			{
				this.SelectCommand = (DbCommand)value;
			}
		}

		IDbCommand IDbDataAdapter.UpdateCommand
		{
			get
			{
				return this.UpdateCommand;
			}
			set
			{
				this.UpdateCommand = (DbCommand)value;
			}
		}

		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get
			{
				return this.DeleteCommand;
			}
			set
			{
				this.DeleteCommand = (DbCommand)value;
			}
		}

		IDbCommand IDbDataAdapter.InsertCommand
		{
			get
			{
				return this.InsertCommand;
			}
			set
			{
				this.InsertCommand = (DbCommand)value;
			}
		}

		[MonoTODO]
		[Obsolete("use 'protected DbDataAdapter(DbDataAdapter)' ctor")]
		object ICloneable.Clone()
		{
			throw new NotImplementedException();
		}

		protected internal CommandBehavior FillCommandBehavior
		{
			get
			{
				return this._behavior;
			}
			set
			{
				this._behavior = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbCommand SelectCommand
		{
			get
			{
				return (DbCommand)this._selectCommand;
			}
			set
			{
				if (this._selectCommand != value)
				{
					this._selectCommand = value;
					((IDbDataAdapter)this).SelectCommand = value;
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DbCommand DeleteCommand
		{
			get
			{
				return (DbCommand)this._deleteCommand;
			}
			set
			{
				if (this._deleteCommand != value)
				{
					this._deleteCommand = value;
					((IDbDataAdapter)this).DeleteCommand = value;
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbCommand InsertCommand
		{
			get
			{
				return (DbCommand)this._insertCommand;
			}
			set
			{
				if (this._insertCommand != value)
				{
					this._insertCommand = value;
					((IDbDataAdapter)this).InsertCommand = value;
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbCommand UpdateCommand
		{
			get
			{
				return (DbCommand)this._updateCommand;
			}
			set
			{
				if (this._updateCommand != value)
				{
					this._updateCommand = value;
					((IDbDataAdapter)this).UpdateCommand = value;
				}
			}
		}

		[DefaultValue(1)]
		public virtual int UpdateBatchSize
		{
			get
			{
				return 1;
			}
			set
			{
				if (value != 1)
				{
					throw new NotSupportedException();
				}
			}
		}

		protected virtual RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new RowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
		}

		protected virtual RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new RowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
		}

		protected virtual void OnRowUpdated(RowUpdatedEventArgs value)
		{
			if (base.Events["RowUpdated"] != null)
			{
				Delegate[] invocationList = base.Events["RowUpdated"].GetInvocationList();
				foreach (Delegate @delegate in invocationList)
				{
					MethodInfo method = @delegate.Method;
					method.Invoke(value, null);
				}
			}
		}

		protected virtual void OnRowUpdating(RowUpdatingEventArgs value)
		{
			if (base.Events["RowUpdating"] != null)
			{
				Delegate[] invocationList = base.Events["RowUpdating"].GetInvocationList();
				foreach (Delegate @delegate in invocationList)
				{
					MethodInfo method = @delegate.Method;
					method.Invoke(value, null);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (((IDbDataAdapter)this).SelectCommand != null)
				{
					((IDbDataAdapter)this).SelectCommand.Dispose();
					((IDbDataAdapter)this).SelectCommand = null;
				}
				if (((IDbDataAdapter)this).InsertCommand != null)
				{
					((IDbDataAdapter)this).InsertCommand.Dispose();
					((IDbDataAdapter)this).InsertCommand = null;
				}
				if (((IDbDataAdapter)this).UpdateCommand != null)
				{
					((IDbDataAdapter)this).UpdateCommand.Dispose();
					((IDbDataAdapter)this).UpdateCommand = null;
				}
				if (((IDbDataAdapter)this).DeleteCommand != null)
				{
					((IDbDataAdapter)this).DeleteCommand.Dispose();
					((IDbDataAdapter)this).DeleteCommand = null;
				}
			}
		}

		public override int Fill(DataSet dataSet)
		{
			return this.Fill(dataSet, 0, 0, "Table", ((IDbDataAdapter)this).SelectCommand, this._behavior);
		}

		public int Fill(DataTable dataTable)
		{
			if (dataTable == null)
			{
				throw new ArgumentNullException("DataTable");
			}
			return this.Fill(dataTable, ((IDbDataAdapter)this).SelectCommand, this._behavior);
		}

		public int Fill(DataSet dataSet, string srcTable)
		{
			return this.Fill(dataSet, 0, 0, srcTable, ((IDbDataAdapter)this).SelectCommand, this._behavior);
		}

		protected virtual int Fill(DataTable dataTable, IDbCommand command, CommandBehavior behavior)
		{
			CommandBehavior commandBehavior = behavior;
			if (command.Connection.State == ConnectionState.Closed)
			{
				command.Connection.Open();
				commandBehavior |= CommandBehavior.CloseConnection;
			}
			return this.Fill(dataTable, command.ExecuteReader(commandBehavior));
		}

		public int Fill(DataSet dataSet, int startRecord, int maxRecords, string srcTable)
		{
			return this.Fill(dataSet, startRecord, maxRecords, srcTable, ((IDbDataAdapter)this).SelectCommand, this._behavior);
		}

		[MonoTODO]
		public int Fill(int startRecord, int maxRecords, params DataTable[] dataTables)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected virtual int Fill(DataTable[] dataTables, int startRecord, int maxRecords, IDbCommand command, CommandBehavior behavior)
		{
			throw new NotImplementedException();
		}

		protected virtual int Fill(DataSet dataSet, int startRecord, int maxRecords, string srcTable, IDbCommand command, CommandBehavior behavior)
		{
			if (command.Connection == null)
			{
				throw new InvalidOperationException("Connection state is closed");
			}
			if (this.MissingSchemaAction == MissingSchemaAction.AddWithKey)
			{
				behavior |= CommandBehavior.KeyInfo;
			}
			CommandBehavior commandBehavior = behavior;
			if (command.Connection.State == ConnectionState.Closed)
			{
				command.Connection.Open();
				commandBehavior |= CommandBehavior.CloseConnection;
			}
			return this.Fill(dataSet, srcTable, command.ExecuteReader(commandBehavior), startRecord, maxRecords);
		}

		internal static int FillFromReader(DataTable table, IDataReader reader, int start, int length, int[] mapping, LoadOption loadOption)
		{
			if (reader.FieldCount == 0)
			{
				return 0;
			}
			for (int i = 0; i < start; i++)
			{
				reader.Read();
			}
			int num = 0;
			object[] array = new object[mapping.Length];
			while (reader.Read() && (length == 0 || num < length))
			{
				for (int j = 0; j < mapping.Length; j++)
				{
					array[j] = ((mapping[j] >= 0) ? reader[mapping[j]] : null);
				}
				table.BeginLoadData();
				table.LoadDataRow(array, loadOption);
				table.EndLoadData();
				num++;
			}
			return num;
		}

		internal static int FillFromReader(DataTable table, IDataReader reader, int start, int length, int[] mapping, LoadOption loadOption, FillErrorEventHandler errorHandler)
		{
			if (reader.FieldCount == 0)
			{
				return 0;
			}
			for (int i = 0; i < start; i++)
			{
				reader.Read();
			}
			int num = 0;
			object[] array = new object[mapping.Length];
			while (reader.Read() && (length == 0 || num < length))
			{
				for (int j = 0; j < mapping.Length; j++)
				{
					array[j] = ((mapping[j] >= 0) ? reader[mapping[j]] : null);
				}
				table.BeginLoadData();
				try
				{
					table.LoadDataRow(array, loadOption);
				}
				catch (Exception ex)
				{
					FillErrorEventArgs e = new FillErrorEventArgs(table, array);
					e.Errors = ex;
					e.Continue = false;
					errorHandler(table, e);
					if (!e.Continue)
					{
						throw ex;
					}
				}
				table.EndLoadData();
				num++;
			}
			return num;
		}

		public override DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
		{
			return this.FillSchema(dataSet, schemaType, ((IDbDataAdapter)this).SelectCommand, "Table", this._behavior);
		}

		public DataTable FillSchema(DataTable dataTable, SchemaType schemaType)
		{
			return this.FillSchema(dataTable, schemaType, ((IDbDataAdapter)this).SelectCommand, this._behavior);
		}

		public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType, string srcTable)
		{
			return this.FillSchema(dataSet, schemaType, ((IDbDataAdapter)this).SelectCommand, srcTable, this._behavior);
		}

		protected virtual DataTable FillSchema(DataTable dataTable, SchemaType schemaType, IDbCommand command, CommandBehavior behavior)
		{
			if (dataTable == null)
			{
				throw new ArgumentNullException("DataTable");
			}
			behavior |= CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo;
			if (command.Connection.State == ConnectionState.Closed)
			{
				command.Connection.Open();
				behavior |= CommandBehavior.CloseConnection;
			}
			IDataReader dataReader = command.ExecuteReader(behavior);
			try
			{
				string text = base.SetupSchema(schemaType, dataTable.TableName);
				if (text != null)
				{
					MissingSchemaAction missingSchemaAction = this.MissingSchemaAction;
					if (missingSchemaAction != MissingSchemaAction.Ignore && missingSchemaAction != MissingSchemaAction.Error)
					{
						missingSchemaAction = MissingSchemaAction.AddWithKey;
					}
					DataAdapter.BuildSchema(dataReader, dataTable, schemaType, missingSchemaAction, this.MissingMappingAction, base.TableMappings);
				}
			}
			finally
			{
				dataReader.Close();
			}
			return dataTable;
		}

		protected virtual DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType, IDbCommand command, string srcTable, CommandBehavior behavior)
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException("DataSet");
			}
			behavior |= CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo;
			if (command.Connection.State == ConnectionState.Closed)
			{
				command.Connection.Open();
				behavior |= CommandBehavior.CloseConnection;
			}
			IDataReader dataReader = command.ExecuteReader(behavior);
			ArrayList arrayList = new ArrayList();
			string text = srcTable;
			int num = 0;
			try
			{
				MissingSchemaAction missingSchemaAction = this.MissingSchemaAction;
				if (this.MissingSchemaAction != MissingSchemaAction.Ignore && this.MissingSchemaAction != MissingSchemaAction.Error)
				{
					missingSchemaAction = MissingSchemaAction.AddWithKey;
				}
				do
				{
					text = base.SetupSchema(schemaType, text);
					if (text != null)
					{
						DataTable dataTable;
						if (dataSet.Tables.Contains(text))
						{
							dataTable = dataSet.Tables[text];
						}
						else
						{
							if (this.MissingSchemaAction == MissingSchemaAction.Ignore)
							{
								goto IL_00FA;
							}
							dataTable = dataSet.Tables.Add(text);
						}
						DataAdapter.BuildSchema(dataReader, dataTable, schemaType, missingSchemaAction, this.MissingMappingAction, base.TableMappings);
						arrayList.Add(dataTable);
						text = string.Format("{0}{1}", srcTable, ++num);
					}
					IL_00FA:;
				}
				while (dataReader.NextResult());
			}
			finally
			{
				dataReader.Close();
			}
			return (DataTable[])arrayList.ToArray(typeof(DataTable));
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public override IDataParameter[] GetFillParameters()
		{
			IDbCommand selectCommand = ((IDbDataAdapter)this).SelectCommand;
			IDataParameter[] array = new IDataParameter[selectCommand.Parameters.Count];
			selectCommand.Parameters.CopyTo(array, 0);
			return array;
		}

		public int Update(DataRow[] dataRows)
		{
			if (dataRows == null)
			{
				throw new ArgumentNullException("dataRows");
			}
			if (dataRows.Length == 0)
			{
				return 0;
			}
			if (dataRows[0] == null)
			{
				throw new ArgumentException("dataRows[0].");
			}
			DataTable table = dataRows[0].Table;
			if (table == null)
			{
				throw new ArgumentException("table is null reference.");
			}
			for (int i = 0; i < dataRows.Length; i++)
			{
				if (dataRows[i] == null)
				{
					throw new ArgumentException("dataRows[" + i + "].");
				}
				if (dataRows[i].Table != table)
				{
					throw new ArgumentException(" DataRow[" + i + "] is from a different DataTable than DataRow[0].");
				}
			}
			DataTableMapping dataTableMapping = base.TableMappings.GetByDataSetTable(table.TableName);
			if (dataTableMapping == null)
			{
				dataTableMapping = DataTableMappingCollection.GetTableMappingBySchemaAction(base.TableMappings, table.TableName, table.TableName, this.MissingMappingAction);
				if (dataTableMapping != null)
				{
					foreach (object obj in table.Columns)
					{
						DataColumn dataColumn = (DataColumn)obj;
						if (dataTableMapping.ColumnMappings.IndexOf(dataColumn.ColumnName) < 0)
						{
							DataColumnMapping dataColumnMapping = DataColumnMappingCollection.GetColumnMappingBySchemaAction(dataTableMapping.ColumnMappings, dataColumn.ColumnName, this.MissingMappingAction);
							if (dataColumnMapping == null)
							{
								dataColumnMapping = new DataColumnMapping(dataColumn.ColumnName, dataColumn.ColumnName);
							}
							dataTableMapping.ColumnMappings.Add(dataColumnMapping);
						}
					}
				}
				else
				{
					ArrayList arrayList = new ArrayList();
					foreach (object obj2 in table.Columns)
					{
						DataColumn dataColumn2 = (DataColumn)obj2;
						arrayList.Add(new DataColumnMapping(dataColumn2.ColumnName, dataColumn2.ColumnName));
					}
					dataTableMapping = new DataTableMapping(table.TableName, table.TableName, arrayList.ToArray(typeof(DataColumnMapping)) as DataColumnMapping[]);
				}
			}
			DataRow[] array = table.NewRowArray(dataRows.Length);
			Array.Copy(dataRows, 0, array, 0, dataRows.Length);
			return this.Update(array, dataTableMapping);
		}

		public override int Update(DataSet dataSet)
		{
			return this.Update(dataSet, "Table");
		}

		public int Update(DataTable dataTable)
		{
			DataTableMapping dataTableMapping = base.TableMappings.GetByDataSetTable(dataTable.TableName);
			if (dataTableMapping == null)
			{
				dataTableMapping = DataTableMappingCollection.GetTableMappingBySchemaAction(base.TableMappings, dataTable.TableName, dataTable.TableName, this.MissingMappingAction);
				if (dataTableMapping != null)
				{
					foreach (object obj in dataTable.Columns)
					{
						DataColumn dataColumn = (DataColumn)obj;
						if (dataTableMapping.ColumnMappings.IndexOf(dataColumn.ColumnName) < 0)
						{
							DataColumnMapping dataColumnMapping = DataColumnMappingCollection.GetColumnMappingBySchemaAction(dataTableMapping.ColumnMappings, dataColumn.ColumnName, this.MissingMappingAction);
							if (dataColumnMapping == null)
							{
								dataColumnMapping = new DataColumnMapping(dataColumn.ColumnName, dataColumn.ColumnName);
							}
							dataTableMapping.ColumnMappings.Add(dataColumnMapping);
						}
					}
				}
				else
				{
					ArrayList arrayList = new ArrayList();
					foreach (object obj2 in dataTable.Columns)
					{
						DataColumn dataColumn2 = (DataColumn)obj2;
						arrayList.Add(new DataColumnMapping(dataColumn2.ColumnName, dataColumn2.ColumnName));
					}
					dataTableMapping = new DataTableMapping(dataTable.TableName, dataTable.TableName, arrayList.ToArray(typeof(DataColumnMapping)) as DataColumnMapping[]);
				}
			}
			return this.Update(dataTable, dataTableMapping);
		}

		private int Update(DataTable dataTable, DataTableMapping tableMapping)
		{
			DataRow[] array = dataTable.NewRowArray(dataTable.Rows.Count);
			dataTable.Rows.CopyTo(array, 0);
			return this.Update(array, tableMapping);
		}

		protected virtual int Update(DataRow[] dataRows, DataTableMapping tableMapping)
		{
			int num = 0;
			int i = 0;
			while (i < dataRows.Length)
			{
				DataRow dataRow = dataRows[i];
				StatementType statementType = StatementType.Update;
				IDbCommand dbCommand = null;
				string text = string.Empty;
				DataRowState rowState = dataRow.RowState;
				switch (rowState)
				{
				case DataRowState.Detached:
				case DataRowState.Unchanged:
					break;
				default:
					if (rowState != DataRowState.Modified)
					{
						goto IL_00A5;
					}
					statementType = StatementType.Update;
					dbCommand = ((IDbDataAdapter)this).UpdateCommand;
					text = "Update";
					goto IL_00A5;
				case DataRowState.Added:
					statementType = StatementType.Insert;
					dbCommand = ((IDbDataAdapter)this).InsertCommand;
					text = "Insert";
					goto IL_00A5;
				case DataRowState.Deleted:
					statementType = StatementType.Delete;
					dbCommand = ((IDbDataAdapter)this).DeleteCommand;
					text = "Delete";
					goto IL_00A5;
				}
				IL_066A:
				i++;
				continue;
				IL_00A5:
				RowUpdatingEventArgs e = this.CreateRowUpdatingEvent(dataRow, dbCommand, statementType, tableMapping);
				dataRow.RowError = null;
				this.OnRowUpdating(e);
				switch (e.Status)
				{
				case UpdateStatus.Continue:
				{
					dbCommand = e.Command;
					try
					{
						if (dbCommand != null)
						{
							DataColumnMappingCollection columnMappings = tableMapping.ColumnMappings;
							foreach (object obj in dbCommand.Parameters)
							{
								IDataParameter dataParameter = (IDataParameter)obj;
								if ((dataParameter.Direction & ParameterDirection.Input) != (ParameterDirection)0)
								{
									DataRowVersion dataRowVersion = dataParameter.SourceVersion;
									if (statementType == StatementType.Delete)
									{
										dataRowVersion = DataRowVersion.Original;
									}
									string text2 = dataParameter.SourceColumn;
									if (columnMappings.Contains(text2))
									{
										text2 = columnMappings[text2].DataSetColumn;
										dataParameter.Value = dataRow[text2, dataRowVersion];
									}
									else
									{
										dataParameter.Value = null;
									}
									DbParameter dbParameter = dataParameter as DbParameter;
									if (dbParameter != null && dbParameter.SourceColumnNullMapping)
									{
										if (dataParameter.Value != null && dataParameter.Value != DBNull.Value)
										{
											dbParameter.Value = 0;
										}
										else
										{
											dbParameter.Value = 1;
										}
									}
								}
							}
						}
					}
					catch (Exception ex)
					{
						e.Errors = ex;
						e.Status = UpdateStatus.ErrorsOccurred;
					}
					IDataReader dataReader = null;
					try
					{
						if (dbCommand == null)
						{
							throw ExceptionHelper.UpdateRequiresCommand(text);
						}
						CommandBehavior commandBehavior = CommandBehavior.Default;
						if (dbCommand.Connection.State == ConnectionState.Closed)
						{
							dbCommand.Connection.Open();
							commandBehavior |= CommandBehavior.CloseConnection;
						}
						dataReader = dbCommand.ExecuteReader(commandBehavior);
						DataColumnMappingCollection columnMappings2 = tableMapping.ColumnMappings;
						if ((dbCommand.UpdatedRowSource == UpdateRowSource.Both || dbCommand.UpdatedRowSource == UpdateRowSource.FirstReturnedRecord) && dataReader.Read())
						{
							DataTable schemaTable = dataReader.GetSchemaTable();
							foreach (object obj2 in schemaTable.Rows)
							{
								DataRow dataRow2 = (DataRow)obj2;
								string text3 = dataRow2["ColumnName"].ToString();
								string text4 = text3;
								if (columnMappings2 != null && columnMappings2.Contains(text3))
								{
									text4 = columnMappings2[text4].DataSetColumn;
								}
								DataColumn dataColumn = dataRow.Table.Columns[text4];
								if (dataColumn != null && (dataColumn.Expression == null || dataColumn.Expression.Length <= 0))
								{
									bool readOnly = dataColumn.ReadOnly;
									dataColumn.ReadOnly = false;
									try
									{
										dataRow[text4] = dataReader[text3];
									}
									finally
									{
										dataColumn.ReadOnly = readOnly;
									}
								}
							}
						}
						dataReader.Close();
						int recordsAffected = dataReader.RecordsAffected;
						if (recordsAffected == 0)
						{
							throw new DBConcurrencyException("Concurrency violation: the " + text + "Command affected 0 records.", null, new DataRow[] { dataRow });
						}
						num += recordsAffected;
						if (dbCommand.UpdatedRowSource == UpdateRowSource.Both || dbCommand.UpdatedRowSource == UpdateRowSource.OutputParameters)
						{
							foreach (object obj3 in dbCommand.Parameters)
							{
								IDataParameter dataParameter2 = (IDataParameter)obj3;
								if (dataParameter2.Direction == ParameterDirection.InputOutput || dataParameter2.Direction == ParameterDirection.Output || dataParameter2.Direction == ParameterDirection.ReturnValue)
								{
									string text5 = dataParameter2.SourceColumn;
									if (columnMappings2 != null && columnMappings2.Contains(dataParameter2.SourceColumn))
									{
										text5 = columnMappings2[dataParameter2.SourceColumn].DataSetColumn;
									}
									DataColumn dataColumn2 = dataRow.Table.Columns[text5];
									if (dataColumn2 != null && (dataColumn2.Expression == null || dataColumn2.Expression.Length <= 0))
									{
										bool readOnly2 = dataColumn2.ReadOnly;
										dataColumn2.ReadOnly = false;
										try
										{
											dataRow[text5] = dataParameter2.Value;
										}
										finally
										{
											dataColumn2.ReadOnly = readOnly2;
										}
									}
								}
							}
						}
						RowUpdatedEventArgs e2 = this.CreateRowUpdatedEvent(dataRow, dbCommand, statementType, tableMapping);
						this.OnRowUpdated(e2);
						switch (e2.Status)
						{
						case UpdateStatus.ErrorsOccurred:
						{
							if (e2.Errors == null)
							{
								e2.Errors = ExceptionHelper.RowUpdatedError();
							}
							DataRow dataRow3 = dataRow;
							dataRow3.RowError += e2.Errors.Message;
							if (!base.ContinueUpdateOnError)
							{
								throw e2.Errors;
							}
							break;
						}
						case UpdateStatus.SkipCurrentRow:
							goto IL_066A;
						case UpdateStatus.SkipAllRemainingRows:
							return num;
						}
						if (base.AcceptChangesDuringUpdate)
						{
							dataRow.AcceptChanges();
						}
					}
					catch (Exception ex2)
					{
						dataRow.RowError = ex2.Message;
						if (!base.ContinueUpdateOnError)
						{
							throw ex2;
						}
					}
					finally
					{
						if (dataReader != null && !dataReader.IsClosed)
						{
							dataReader.Close();
						}
					}
					goto IL_066A;
				}
				case UpdateStatus.ErrorsOccurred:
				{
					if (e.Errors == null)
					{
						e.Errors = ExceptionHelper.RowUpdatedError();
					}
					DataRow dataRow4 = dataRow;
					dataRow4.RowError += e.Errors.Message;
					if (!base.ContinueUpdateOnError)
					{
						throw e.Errors;
					}
					goto IL_066A;
				}
				case UpdateStatus.SkipCurrentRow:
					num++;
					goto IL_066A;
				case UpdateStatus.SkipAllRemainingRows:
					return num;
				default:
					throw ExceptionHelper.InvalidUpdateStatus(e.Status);
				}
			}
			return num;
		}

		public int Update(DataSet dataSet, string srcTable)
		{
			MissingMappingAction missingMappingAction = this.MissingMappingAction;
			if (missingMappingAction == MissingMappingAction.Ignore)
			{
				missingMappingAction = MissingMappingAction.Error;
			}
			DataTableMapping tableMappingBySchemaAction = DataTableMappingCollection.GetTableMappingBySchemaAction(base.TableMappings, srcTable, srcTable, missingMappingAction);
			DataTable dataTable = dataSet.Tables[tableMappingBySchemaAction.DataSetTable];
			if (dataTable == null)
			{
				throw new ArgumentException(string.Format("Missing table {0}", srcTable));
			}
			return this.Update(dataTable, tableMappingBySchemaAction);
		}

		protected virtual int AddToBatch(IDbCommand command)
		{
			throw this.CreateMethodNotSupportedException();
		}

		protected virtual void ClearBatch()
		{
			throw this.CreateMethodNotSupportedException();
		}

		protected virtual int ExecuteBatch()
		{
			throw this.CreateMethodNotSupportedException();
		}

		protected virtual IDataParameter GetBatchedParameter(int commandIdentifier, int parameterIndex)
		{
			throw this.CreateMethodNotSupportedException();
		}

		protected virtual bool GetBatchedRecordsAffected(int commandIdentifier, out int recordsAffected, out Exception error)
		{
			recordsAffected = 1;
			error = null;
			return true;
		}

		protected virtual void InitializeBatching()
		{
			throw this.CreateMethodNotSupportedException();
		}

		protected virtual void TerminateBatching()
		{
			throw this.CreateMethodNotSupportedException();
		}

		private Exception CreateMethodNotSupportedException()
		{
			return new NotSupportedException("Method is not supported.");
		}

		public const string DefaultSourceTableName = "Table";

		private const string DefaultSourceColumnName = "Column";

		private CommandBehavior _behavior;

		private IDbCommand _selectCommand;

		private IDbCommand _updateCommand;

		private IDbCommand _deleteCommand;

		private IDbCommand _insertCommand;
	}
}
