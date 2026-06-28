using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace System.Data.Common
{
	public abstract class DbCommandBuilder : Component
	{
		private void BuildCache(bool closeConnection)
		{
			DbCommand sourceCommand = this.SourceCommand;
			if (sourceCommand == null)
			{
				throw new InvalidOperationException("The DataAdapter.SelectCommand property needs to be initialized.");
			}
			DbConnection connection = sourceCommand.Connection;
			if (connection == null)
			{
				throw new InvalidOperationException("The DataAdapter.SelectCommand.Connection property needs to be initialized.");
			}
			if (this._dbSchemaTable == null)
			{
				if (connection.State == ConnectionState.Open)
				{
					closeConnection = false;
				}
				else
				{
					connection.Open();
				}
				DbDataReader dbDataReader = sourceCommand.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
				this._dbSchemaTable = dbDataReader.GetSchemaTable();
				dbDataReader.Close();
				if (closeConnection)
				{
					connection.Close();
				}
				this.BuildInformation(this._dbSchemaTable);
			}
		}

		private string QuotedTableName
		{
			get
			{
				return this.GetQuotedString(this._tableName);
			}
		}

		private bool IsCommandGenerated
		{
			get
			{
				return this._insertCommand != null || this._updateCommand != null || this._deleteCommand != null;
			}
		}

		private string GetQuotedString(string value)
		{
			if (value == string.Empty || value == null)
			{
				return value;
			}
			string quotePrefix = this.QuotePrefix;
			string quoteSuffix = this.QuoteSuffix;
			if (quotePrefix.Length == 0 && quoteSuffix.Length == 0)
			{
				return value;
			}
			return string.Format("{0}{1}{2}", quotePrefix, value, quoteSuffix);
		}

		private void BuildInformation(DataTable schemaTable)
		{
			this._tableName = string.Empty;
			foreach (object obj in schemaTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (!dataRow.IsNull("BaseTableName") && !((string)dataRow["BaseTableName"] == string.Empty))
				{
					if (this._tableName == string.Empty)
					{
						this._tableName = (string)dataRow["BaseTableName"];
					}
					else if (this._tableName != (string)dataRow["BaseTableName"])
					{
						throw new InvalidOperationException("Dynamic SQL generation is not supported against multiple base tables.");
					}
				}
			}
			if (this._tableName == string.Empty)
			{
				throw new InvalidOperationException("Dynamic SQL generation is not supported with no base table.");
			}
			this._dbSchemaTable = schemaTable;
		}

		private bool IncludedInInsert(DataRow schemaRow)
		{
			return (schemaRow.IsNull("IsAutoIncrement") || !(bool)schemaRow["IsAutoIncrement"]) && (schemaRow.IsNull("IsExpression") || !(bool)schemaRow["IsExpression"]) && (schemaRow.IsNull("IsRowVersion") || !(bool)schemaRow["IsRowVersion"]) && (schemaRow.IsNull("IsReadOnly") || !(bool)schemaRow["IsReadOnly"]);
		}

		private bool IncludedInUpdate(DataRow schemaRow)
		{
			return (schemaRow.IsNull("IsAutoIncrement") || !(bool)schemaRow["IsAutoIncrement"]) && (schemaRow.IsNull("IsRowVersion") || !(bool)schemaRow["IsRowVersion"]) && (schemaRow.IsNull("IsExpression") || !(bool)schemaRow["IsExpression"]) && (schemaRow.IsNull("IsReadOnly") || !(bool)schemaRow["IsReadOnly"]);
		}

		private bool IncludedInWhereClause(DataRow schemaRow)
		{
			return !(bool)schemaRow["IsLong"];
		}

		private DbCommand CreateDeleteCommand(bool option)
		{
			if (this.QuotedTableName == string.Empty)
			{
				return null;
			}
			this.CreateNewCommand(ref this._deleteCommand);
			string text = string.Format("DELETE FROM {0}", this.QuotedTableName);
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			int num = 1;
			foreach (object obj in this._dbSchemaTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (dataRow.IsNull("IsExpression") || !(bool)dataRow["IsExpression"])
				{
					if (this.IncludedInWhereClause(dataRow))
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(" AND ");
						}
						bool flag2 = (bool)dataRow["IsKey"];
						if (flag2)
						{
							flag = true;
						}
						bool flag3 = (bool)dataRow["AllowDBNull"];
						DbParameter dbParameter;
						if (!flag2 && flag3)
						{
							dbParameter = this._deleteCommand.CreateParameter();
							if (option)
							{
								dbParameter.ParameterName = string.Format("@IsNull_{0}", dataRow["BaseColumnName"]);
							}
							else
							{
								dbParameter.ParameterName = string.Format("@p{0}", num++);
							}
							dbParameter.Value = 1;
							dbParameter.DbType = DbType.Int32;
							string text2 = (string)dataRow["BaseColumnName"];
							dbParameter.SourceColumn = text2;
							dbParameter.SourceColumnNullMapping = true;
							dbParameter.SourceVersion = DataRowVersion.Original;
							this._deleteCommand.Parameters.Add(dbParameter);
							stringBuilder.Append("(");
							stringBuilder.Append(string.Format(DbCommandBuilder.clause1, dbParameter.ParameterName, this.GetQuotedString(text2)));
							stringBuilder.Append(" OR ");
						}
						if (option)
						{
							dbParameter = this.CreateParameter(this._deleteCommand, dataRow, true);
						}
						else
						{
							dbParameter = this.CreateParameter(this._deleteCommand, num++, dataRow);
						}
						dbParameter.SourceVersion = DataRowVersion.Original;
						this.ApplyParameterInfo(dbParameter, dataRow, StatementType.Delete, true);
						stringBuilder.Append(string.Format(DbCommandBuilder.clause2, this.GetQuotedString(dbParameter.SourceColumn), dbParameter.ParameterName));
						if (!flag2 && flag3)
						{
							stringBuilder.Append(")");
						}
					}
				}
			}
			if (!flag)
			{
				throw new InvalidOperationException("Dynamic SQL generation for the DeleteCommand is not supported against a SelectCommand that does not return any key column information.");
			}
			string text3 = string.Format("{0} WHERE ({1})", text, stringBuilder.ToString());
			this._deleteCommand.CommandText = text3;
			this._dbCommand = this._deleteCommand;
			return this._deleteCommand;
		}

		private DbCommand CreateInsertCommand(bool option, DataRow row)
		{
			if (this.QuotedTableName == string.Empty)
			{
				return null;
			}
			this.CreateNewCommand(ref this._insertCommand);
			string text = string.Format("INSERT INTO {0}", this.QuotedTableName);
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			int num = 1;
			foreach (object obj in this._dbSchemaTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (this.IncludedInInsert(dataRow))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
						stringBuilder2.Append(", ");
					}
					DbParameter dbParameter;
					if (option)
					{
						dbParameter = this.CreateParameter(this._insertCommand, dataRow, false);
					}
					else
					{
						dbParameter = this.CreateParameter(this._insertCommand, num++, dataRow);
					}
					dbParameter.SourceVersion = DataRowVersion.Current;
					this.ApplyParameterInfo(dbParameter, dataRow, StatementType.Insert, false);
					stringBuilder.Append(this.GetQuotedString(dbParameter.SourceColumn));
					string text2 = dataRow["ColumnName"] as string;
					if (!(!dataRow.IsNull("AllowDBNull") & (bool)dataRow["AllowDBNull"]) && row != null && (row[text2] == DBNull.Value || row[text2] == null))
					{
						stringBuilder2.Append("DEFAULT");
					}
					else
					{
						stringBuilder2.Append(dbParameter.ParameterName);
					}
				}
			}
			string text3 = string.Format("{0} ({1}) VALUES ({2})", text, stringBuilder.ToString(), stringBuilder2.ToString());
			this._insertCommand.CommandText = text3;
			this._dbCommand = this._insertCommand;
			return this._insertCommand;
		}

		private void CreateNewCommand(ref DbCommand command)
		{
			DbCommand sourceCommand = this.SourceCommand;
			if (command == null)
			{
				command = sourceCommand.Connection.CreateCommand();
				command.CommandTimeout = sourceCommand.CommandTimeout;
				command.Transaction = sourceCommand.Transaction;
			}
			command.CommandType = CommandType.Text;
			command.UpdatedRowSource = UpdateRowSource.None;
			command.Parameters.Clear();
		}

		private DbCommand CreateUpdateCommand(bool option)
		{
			if (this.QuotedTableName == string.Empty)
			{
				return null;
			}
			this.CreateNewCommand(ref this._updateCommand);
			string text = string.Format("UPDATE {0} SET ", this.QuotedTableName);
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			int num = 1;
			bool flag = false;
			foreach (object obj in this._dbSchemaTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (this.IncludedInUpdate(dataRow))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					DbParameter dbParameter;
					if (option)
					{
						dbParameter = this.CreateParameter(this._updateCommand, dataRow, false);
					}
					else
					{
						dbParameter = this.CreateParameter(this._updateCommand, num++, dataRow);
					}
					dbParameter.SourceVersion = DataRowVersion.Current;
					this.ApplyParameterInfo(dbParameter, dataRow, StatementType.Update, false);
					stringBuilder.Append(string.Format("{0} = {1}", this.GetQuotedString(dbParameter.SourceColumn), dbParameter.ParameterName));
				}
			}
			foreach (object obj2 in this._dbSchemaTable.Rows)
			{
				DataRow dataRow2 = (DataRow)obj2;
				if (dataRow2.IsNull("IsExpression") || !(bool)dataRow2["IsExpression"])
				{
					if (this.IncludedInWhereClause(dataRow2))
					{
						if (stringBuilder2.Length > 0)
						{
							stringBuilder2.Append(" AND ");
						}
						bool flag2 = (bool)dataRow2["IsKey"];
						if (flag2)
						{
							flag = true;
						}
						bool flag3 = (bool)dataRow2["AllowDBNull"];
						DbParameter dbParameter;
						if (!flag2 && flag3)
						{
							dbParameter = this._updateCommand.CreateParameter();
							if (option)
							{
								dbParameter.ParameterName = string.Format("@IsNull_{0}", dataRow2["BaseColumnName"]);
							}
							else
							{
								dbParameter.ParameterName = string.Format("@p{0}", num++);
							}
							dbParameter.DbType = DbType.Int32;
							dbParameter.Value = 1;
							dbParameter.SourceColumn = (string)dataRow2["BaseColumnName"];
							dbParameter.SourceColumnNullMapping = true;
							dbParameter.SourceVersion = DataRowVersion.Original;
							stringBuilder2.Append("(");
							stringBuilder2.Append(string.Format(DbCommandBuilder.clause1, dbParameter.ParameterName, this.GetQuotedString((string)dataRow2["BaseColumnName"])));
							stringBuilder2.Append(" OR ");
							this._updateCommand.Parameters.Add(dbParameter);
						}
						if (option)
						{
							dbParameter = this.CreateParameter(this._updateCommand, dataRow2, true);
						}
						else
						{
							dbParameter = this.CreateParameter(this._updateCommand, num++, dataRow2);
						}
						dbParameter.SourceVersion = DataRowVersion.Original;
						this.ApplyParameterInfo(dbParameter, dataRow2, StatementType.Update, true);
						stringBuilder2.Append(string.Format(DbCommandBuilder.clause2, this.GetQuotedString(dbParameter.SourceColumn), dbParameter.ParameterName));
						if (!flag2 && flag3)
						{
							stringBuilder2.Append(")");
						}
					}
				}
			}
			if (!flag)
			{
				throw new InvalidOperationException("Dynamic SQL generation for the UpdateCommand is not supported against a SelectCommand that does not return any key column information.");
			}
			string text2 = string.Format("{0}{1} WHERE ({2})", text, stringBuilder.ToString(), stringBuilder2.ToString());
			this._updateCommand.CommandText = text2;
			this._dbCommand = this._updateCommand;
			return this._updateCommand;
		}

		private DbParameter CreateParameter(DbCommand _dbCommand, DataRow schemaRow, bool whereClause)
		{
			string text = (string)schemaRow["BaseColumnName"];
			DbParameter dbParameter = _dbCommand.CreateParameter();
			if (whereClause)
			{
				dbParameter.ParameterName = this.GetParameterName("Original_" + text);
			}
			else
			{
				dbParameter.ParameterName = this.GetParameterName(text);
			}
			dbParameter.SourceColumn = text;
			_dbCommand.Parameters.Add(dbParameter);
			return dbParameter;
		}

		private DbParameter CreateParameter(DbCommand _dbCommand, int paramIndex, DataRow schemaRow)
		{
			string text = (string)schemaRow["BaseColumnName"];
			DbParameter dbParameter = _dbCommand.CreateParameter();
			dbParameter.ParameterName = this.GetParameterName(paramIndex);
			dbParameter.SourceColumn = text;
			_dbCommand.Parameters.Add(dbParameter);
			return dbParameter;
		}

		[DefaultValue(CatalogLocation.Start)]
		public virtual CatalogLocation CatalogLocation
		{
			get
			{
				return this._catalogLocation;
			}
			set
			{
				DbCommandBuilder.CheckEnumValue(typeof(CatalogLocation), (int)value);
				this._catalogLocation = value;
			}
		}

		[DefaultValue(".")]
		public virtual string CatalogSeparator
		{
			get
			{
				if (this._catalogSeparator == null || this._catalogSeparator.Length == 0)
				{
					return DbCommandBuilder.SEPARATOR_DEFAULT;
				}
				return this._catalogSeparator;
			}
			set
			{
				this._catalogSeparator = value;
			}
		}

		[DefaultValue(ConflictOption.CompareAllSearchableValues)]
		public virtual ConflictOption ConflictOption
		{
			get
			{
				return this._conflictOption;
			}
			set
			{
				DbCommandBuilder.CheckEnumValue(typeof(ConflictOption), (int)value);
				this._conflictOption = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DbDataAdapter DataAdapter
		{
			get
			{
				return this._dbDataAdapter;
			}
			set
			{
				if (value != null)
				{
					this.SetRowUpdatingHandler(value);
				}
				this._dbDataAdapter = value;
			}
		}

		[DefaultValue("")]
		public virtual string QuotePrefix
		{
			get
			{
				if (this._quotePrefix == null)
				{
					return string.Empty;
				}
				return this._quotePrefix;
			}
			set
			{
				if (this.IsCommandGenerated)
				{
					throw new InvalidOperationException("QuotePrefix cannot be set after an Insert, Update or Delete command has been generated.");
				}
				this._quotePrefix = value;
			}
		}

		[DefaultValue("")]
		public virtual string QuoteSuffix
		{
			get
			{
				if (this._quoteSuffix == null)
				{
					return string.Empty;
				}
				return this._quoteSuffix;
			}
			set
			{
				if (this.IsCommandGenerated)
				{
					throw new InvalidOperationException("QuoteSuffix cannot be set after an Insert, Update or Delete command has been generated.");
				}
				this._quoteSuffix = value;
			}
		}

		[DefaultValue(".")]
		public virtual string SchemaSeparator
		{
			get
			{
				if (this._schemaSeparator == null || this._schemaSeparator.Length == 0)
				{
					return DbCommandBuilder.SEPARATOR_DEFAULT;
				}
				return this._schemaSeparator;
			}
			set
			{
				this._schemaSeparator = value;
			}
		}

		[DefaultValue(false)]
		public bool SetAllValues
		{
			get
			{
				return this._setAllValues;
			}
			set
			{
				this._setAllValues = value;
			}
		}

		private DbCommand SourceCommand
		{
			get
			{
				if (this._dbDataAdapter != null)
				{
					return this._dbDataAdapter.SelectCommand;
				}
				return null;
			}
		}

		protected abstract void ApplyParameterInfo(DbParameter parameter, DataRow row, StatementType statementType, bool whereClause);

		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					if (this._insertCommand != null)
					{
						this._insertCommand.Dispose();
					}
					if (this._deleteCommand != null)
					{
						this._deleteCommand.Dispose();
					}
					if (this._updateCommand != null)
					{
						this._updateCommand.Dispose();
					}
					if (this._dbSchemaTable != null)
					{
						this._dbSchemaTable.Dispose();
					}
				}
				this._disposed = true;
			}
		}

		public DbCommand GetDeleteCommand()
		{
			return this.GetDeleteCommand(false);
		}

		public DbCommand GetDeleteCommand(bool option)
		{
			this.BuildCache(true);
			if (this._deleteCommand == null || option)
			{
				return this.CreateDeleteCommand(option);
			}
			return this._deleteCommand;
		}

		public DbCommand GetInsertCommand()
		{
			return this.GetInsertCommand(false, null);
		}

		public DbCommand GetInsertCommand(bool option)
		{
			return this.GetInsertCommand(option, null);
		}

		internal DbCommand GetInsertCommand(bool option, DataRow row)
		{
			this.BuildCache(true);
			if (this._insertCommand == null || option)
			{
				return this.CreateInsertCommand(option, row);
			}
			return this._insertCommand;
		}

		public DbCommand GetUpdateCommand()
		{
			return this.GetUpdateCommand(false);
		}

		public DbCommand GetUpdateCommand(bool option)
		{
			this.BuildCache(true);
			if (this._updateCommand == null || option)
			{
				return this.CreateUpdateCommand(option);
			}
			return this._updateCommand;
		}

		protected virtual DbCommand InitializeCommand(DbCommand command)
		{
			if (this._dbCommand == null)
			{
				this._dbCommand = this.SourceCommand;
			}
			else
			{
				this._dbCommand.CommandTimeout = 30;
				this._dbCommand.Transaction = null;
				this._dbCommand.CommandType = CommandType.Text;
				this._dbCommand.UpdatedRowSource = UpdateRowSource.None;
			}
			return this._dbCommand;
		}

		public virtual string QuoteIdentifier(string unquotedIdentifier)
		{
			throw new NotSupportedException();
		}

		public virtual string UnquoteIdentifier(string quotedIdentifier)
		{
			if (quotedIdentifier == null)
			{
				throw new ArgumentNullException("Quoted identifier parameter cannot be null");
			}
			string text = quotedIdentifier.Trim();
			if (text.StartsWith(this.QuotePrefix))
			{
				text = text.Remove(0, 1);
			}
			if (text.EndsWith(this.QuoteSuffix))
			{
				text = text.Remove(text.Length - 1, 1);
			}
			return text;
		}

		public virtual void RefreshSchema()
		{
			this._tableName = string.Empty;
			this._dbSchemaTable = null;
			this._deleteCommand = null;
			this._updateCommand = null;
			this._insertCommand = null;
		}

		protected void RowUpdatingHandler(RowUpdatingEventArgs args)
		{
			if (args.Command != null)
			{
				return;
			}
			try
			{
				switch (args.StatementType)
				{
				case StatementType.Insert:
					args.Command = this.GetInsertCommand(false, args.Row);
					break;
				case StatementType.Update:
					args.Command = this.GetUpdateCommand();
					break;
				case StatementType.Delete:
					args.Command = this.GetDeleteCommand();
					break;
				}
			}
			catch (Exception ex)
			{
				args.Errors = ex;
				args.Status = UpdateStatus.ErrorsOccurred;
			}
		}

		protected abstract string GetParameterName(int parameterOrdinal);

		protected abstract string GetParameterName(string parameterName);

		protected abstract string GetParameterPlaceholder(int parameterOrdinal);

		protected abstract void SetRowUpdatingHandler(DbDataAdapter adapter);

		protected virtual DataTable GetSchemaTable(DbCommand cmd)
		{
			DataTable schemaTable;
			using (DbDataReader dbDataReader = cmd.ExecuteReader())
			{
				schemaTable = dbDataReader.GetSchemaTable();
			}
			return schemaTable;
		}

		private static void CheckEnumValue(Type type, int value)
		{
			if (Enum.IsDefined(type, value))
			{
				return;
			}
			string name = type.Name;
			string text = string.Format(CultureInfo.CurrentCulture, "Value {0} is not valid for {1}.", new object[] { value, name });
			throw new ArgumentOutOfRangeException(name, text);
		}

		private bool _setAllValues;

		private bool _disposed;

		private DataTable _dbSchemaTable;

		private DbDataAdapter _dbDataAdapter;

		private CatalogLocation _catalogLocation = CatalogLocation.Start;

		private ConflictOption _conflictOption = ConflictOption.CompareAllSearchableValues;

		private string _tableName;

		private string _catalogSeparator;

		private string _quotePrefix;

		private string _quoteSuffix;

		private string _schemaSeparator;

		private DbCommand _dbCommand;

		private DbCommand _deleteCommand;

		private DbCommand _insertCommand;

		private DbCommand _updateCommand;

		private static readonly string SEPARATOR_DEFAULT = ".";

		private static readonly string clause1 = "({0} = 1 AND {1} IS NULL)";

		private static readonly string clause2 = "({0} = {1})";
	}
}
