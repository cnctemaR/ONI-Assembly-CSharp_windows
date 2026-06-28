using System;
using System.ComponentModel;
using System.Data.Common;
using System.Text;

namespace System.Data.Odbc
{
	public sealed class OdbcCommandBuilder : DbCommandBuilder
	{
		public OdbcCommandBuilder()
		{
		}

		public OdbcCommandBuilder(OdbcDataAdapter adapter)
			: this()
		{
			this.DataAdapter = adapter;
		}

		[DefaultValue(null)]
		[OdbcDescription("The DataAdapter for which to automatically generate OdbcCommands")]
		public new OdbcDataAdapter DataAdapter
		{
			get
			{
				return this._adapter;
			}
			set
			{
				if (this._adapter == value)
				{
					return;
				}
				if (this.rowUpdatingHandler != null)
				{
					this.rowUpdatingHandler = new OdbcRowUpdatingEventHandler(this.OnRowUpdating);
				}
				if (this._adapter != null)
				{
					this._adapter.RowUpdating -= this.rowUpdatingHandler;
				}
				this._adapter = value;
				if (this._adapter != null)
				{
					this._adapter.RowUpdating += this.rowUpdatingHandler;
				}
			}
		}

		private OdbcCommand SelectCommand
		{
			get
			{
				if (this.DataAdapter == null)
				{
					return null;
				}
				return this.DataAdapter.SelectCommand;
			}
		}

		private DataTable Schema
		{
			get
			{
				if (this._schema == null)
				{
					this.RefreshSchema();
				}
				return this._schema;
			}
		}

		private string TableName
		{
			get
			{
				if (this._tableName != string.Empty)
				{
					return this._tableName;
				}
				DataRow[] array = this.Schema.Select("BaseTableName is not null and BaseTableName <> ''");
				if (array.Length > 1)
				{
					string text = (string)array[0]["BaseTableName"];
					foreach (DataRow dataRow in array)
					{
						if ((string)dataRow["BaseTableName"] != text)
						{
							throw new InvalidOperationException("Dynamic SQL generation is not supported against multiple base tables.");
						}
					}
				}
				if (array.Length == 0)
				{
					throw new InvalidOperationException("Cannot determine the base table name. Cannot proceed");
				}
				this._tableName = array[0]["BaseTableName"].ToString();
				return this._tableName;
			}
		}

		[MonoTODO]
		public static void DeriveParameters(OdbcCommand command)
		{
			throw new NotImplementedException();
		}

		private new void Dispose(bool disposing)
		{
			if (this._disposed)
			{
				return;
			}
			if (disposing)
			{
				if (this._insertCommand != null)
				{
					this._insertCommand.Dispose();
				}
				if (this._updateCommand != null)
				{
					this._updateCommand.Dispose();
				}
				if (this._deleteCommand != null)
				{
					this._deleteCommand.Dispose();
				}
				if (this._schema != null)
				{
					this._schema.Dispose();
				}
				this._insertCommand = null;
				this._updateCommand = null;
				this._deleteCommand = null;
				this._schema = null;
			}
			this._disposed = true;
		}

		private bool IsUpdatable(DataRow schemaRow)
		{
			return (schemaRow.IsNull("IsAutoIncrement") || !(bool)schemaRow["IsAutoIncrement"]) && (schemaRow.IsNull("IsRowVersion") || !(bool)schemaRow["IsRowVersion"]) && (schemaRow.IsNull("IsReadOnly") || !(bool)schemaRow["IsReadOnly"]) && !schemaRow.IsNull("BaseTableName") && ((string)schemaRow["BaseTableName"]).Length != 0;
		}

		private string GetColumnName(DataRow schemaRow)
		{
			string text = ((!schemaRow.IsNull("BaseColumnName")) ? ((string)schemaRow["BaseColumnName"]) : string.Empty);
			if (text == string.Empty)
			{
				text = ((!schemaRow.IsNull("ColumnName")) ? ((string)schemaRow["ColumnName"]) : string.Empty);
			}
			return text;
		}

		private OdbcParameter AddParameter(OdbcCommand cmd, string paramName, OdbcType odbcType, int length, string sourceColumnName, DataRowVersion rowVersion)
		{
			OdbcParameter odbcParameter;
			if (length >= 0 && sourceColumnName != string.Empty)
			{
				odbcParameter = cmd.Parameters.Add(paramName, odbcType, length, sourceColumnName);
			}
			else
			{
				odbcParameter = cmd.Parameters.Add(paramName, odbcType);
			}
			odbcParameter.SourceVersion = rowVersion;
			return odbcParameter;
		}

		private string CreateOptWhereClause(OdbcCommand command, int paramCount)
		{
			string[] array = new string[this.Schema.Rows.Count];
			int num = 0;
			foreach (object obj in this.Schema.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (this.IsUpdatable(dataRow))
				{
					string columnName = this.GetColumnName(dataRow);
					if (columnName == string.Empty)
					{
						throw new InvalidOperationException("Cannot form delete command. Column name is missing!");
					}
					bool flag = dataRow.IsNull("AllowDBNull") || (bool)dataRow["AllowDBNull"];
					OdbcType odbcType = (OdbcType)((!dataRow.IsNull("ProviderType")) ? ((int)dataRow["ProviderType"]) : 22);
					int num2 = ((!dataRow.IsNull("ColumnSize")) ? ((int)dataRow["ColumnSize"]) : (-1));
					if (flag)
					{
						array[num++] = string.Format("((? = 1 AND {0} IS NULL) OR ({0} = ?))", this.GetQuotedString(columnName));
						OdbcParameter odbcParameter = this.AddParameter(command, this.GetParameterName(++paramCount), OdbcType.Int, num2, columnName, DataRowVersion.Original);
						odbcParameter.Value = 1;
						this.AddParameter(command, this.GetParameterName(++paramCount), odbcType, num2, columnName, DataRowVersion.Original);
					}
					else
					{
						array[num++] = string.Format("({0} = ?)", this.GetQuotedString(columnName));
						this.AddParameter(command, this.GetParameterName(++paramCount), odbcType, num2, columnName, DataRowVersion.Original);
					}
				}
			}
			return string.Join(" AND ", array, 0, num);
		}

		private void CreateNewCommand(ref OdbcCommand command)
		{
			OdbcCommand selectCommand = this.SelectCommand;
			if (command == null)
			{
				command = new OdbcCommand();
				command.Connection = selectCommand.Connection;
				command.CommandTimeout = selectCommand.CommandTimeout;
				command.Transaction = selectCommand.Transaction;
			}
			command.CommandType = CommandType.Text;
			command.UpdatedRowSource = UpdateRowSource.None;
			command.Parameters.Clear();
		}

		private OdbcCommand CreateInsertCommand(bool option)
		{
			this.CreateNewCommand(ref this._insertCommand);
			string text = string.Format("INSERT INTO {0}", this.GetQuotedString(this.TableName));
			string[] array = new string[this.Schema.Rows.Count];
			string[] array2 = new string[this.Schema.Rows.Count];
			int num = 0;
			foreach (object obj in this.Schema.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (this.IsUpdatable(dataRow))
				{
					string columnName = this.GetColumnName(dataRow);
					if (columnName == string.Empty)
					{
						throw new InvalidOperationException("Cannot form insert command. Column name is missing!");
					}
					array[num] = this.GetQuotedString(columnName);
					array2[num++] = "?";
					OdbcType odbcType = (OdbcType)((!dataRow.IsNull("ProviderType")) ? ((int)dataRow["ProviderType"]) : 22);
					int num2 = ((!dataRow.IsNull("ColumnSize")) ? ((int)dataRow["ColumnSize"]) : (-1));
					this.AddParameter(this._insertCommand, this.GetParameterName(num), odbcType, num2, columnName, DataRowVersion.Current);
				}
			}
			text = string.Format("{0} ({1}) VALUES ({2})", text, string.Join(", ", array, 0, num), string.Join(", ", array2, 0, num));
			this._insertCommand.CommandText = text;
			return this._insertCommand;
		}

		public new OdbcCommand GetInsertCommand()
		{
			if (this._insertCommand != null)
			{
				return this._insertCommand;
			}
			if (this._schema == null)
			{
				this.RefreshSchema();
			}
			return this.CreateInsertCommand(false);
		}

		public new OdbcCommand GetInsertCommand(bool useColumnsForParameterNames)
		{
			if (this._insertCommand != null)
			{
				return this._insertCommand;
			}
			if (this._schema == null)
			{
				this.RefreshSchema();
			}
			return this.CreateInsertCommand(useColumnsForParameterNames);
		}

		private OdbcCommand CreateUpdateCommand(bool option)
		{
			this.CreateNewCommand(ref this._updateCommand);
			string text = string.Format("UPDATE {0} SET", this.GetQuotedString(this.TableName));
			string[] array = new string[this.Schema.Rows.Count];
			int num = 0;
			foreach (object obj in this.Schema.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (this.IsUpdatable(dataRow))
				{
					string columnName = this.GetColumnName(dataRow);
					if (columnName == string.Empty)
					{
						throw new InvalidOperationException("Cannot form update command. Column name is missing!");
					}
					OdbcType odbcType = (OdbcType)((!dataRow.IsNull("ProviderType")) ? ((int)dataRow["ProviderType"]) : 22);
					int num2 = ((!dataRow.IsNull("ColumnSize")) ? ((int)dataRow["ColumnSize"]) : (-1));
					array[num++] = string.Format("{0} = ?", this.GetQuotedString(columnName));
					this.AddParameter(this._updateCommand, this.GetParameterName(num), odbcType, num2, columnName, DataRowVersion.Current);
				}
			}
			string text2 = this.CreateOptWhereClause(this._updateCommand, num);
			text = string.Format("{0} {1} WHERE ({2})", text, string.Join(", ", array, 0, num), text2);
			this._updateCommand.CommandText = text;
			return this._updateCommand;
		}

		public new OdbcCommand GetUpdateCommand()
		{
			if (this._updateCommand != null)
			{
				return this._updateCommand;
			}
			if (this._schema == null)
			{
				this.RefreshSchema();
			}
			return this.CreateUpdateCommand(false);
		}

		public new OdbcCommand GetUpdateCommand(bool useColumnsForParameterNames)
		{
			if (this._updateCommand != null)
			{
				return this._updateCommand;
			}
			if (this._schema == null)
			{
				this.RefreshSchema();
			}
			return this.CreateUpdateCommand(useColumnsForParameterNames);
		}

		private OdbcCommand CreateDeleteCommand(bool option)
		{
			this.CreateNewCommand(ref this._deleteCommand);
			string text = string.Format("DELETE FROM {0}", this.GetQuotedString(this.TableName));
			string text2 = this.CreateOptWhereClause(this._deleteCommand, 0);
			text = string.Format("{0} WHERE ({1})", text, text2);
			this._deleteCommand.CommandText = text;
			return this._deleteCommand;
		}

		public new OdbcCommand GetDeleteCommand()
		{
			if (this._deleteCommand != null)
			{
				return this._deleteCommand;
			}
			if (this._schema == null)
			{
				this.RefreshSchema();
			}
			return this.CreateDeleteCommand(false);
		}

		public new OdbcCommand GetDeleteCommand(bool useColumnsForParameterNames)
		{
			if (this._deleteCommand != null)
			{
				return this._deleteCommand;
			}
			if (this._schema == null)
			{
				this.RefreshSchema();
			}
			return this.CreateDeleteCommand(useColumnsForParameterNames);
		}

		private new void RefreshSchema()
		{
			if (this.SelectCommand == null)
			{
				throw new InvalidOperationException("SelectCommand should be valid");
			}
			if (this.SelectCommand.Connection == null)
			{
				throw new InvalidOperationException("SelectCommand's Connection should be valid");
			}
			CommandBehavior commandBehavior = CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo;
			if (this.SelectCommand.Connection.State != ConnectionState.Open)
			{
				this.SelectCommand.Connection.Open();
				commandBehavior |= CommandBehavior.CloseConnection;
			}
			OdbcDataReader odbcDataReader = this.SelectCommand.ExecuteReader(commandBehavior);
			this._schema = odbcDataReader.GetSchemaTable();
			odbcDataReader.Close();
			this._insertCommand = null;
			this._updateCommand = null;
			this._deleteCommand = null;
			this._tableName = string.Empty;
		}

		protected override string GetParameterName(int parameterOrdinal)
		{
			return string.Format("p{0}", parameterOrdinal);
		}

		protected override void ApplyParameterInfo(DbParameter parameter, DataRow row, StatementType statementType, bool whereClause)
		{
			OdbcParameter odbcParameter = (OdbcParameter)parameter;
			odbcParameter.Size = int.Parse(row["ColumnSize"].ToString());
			if (row["NumericPrecision"] != DBNull.Value)
			{
				odbcParameter.Precision = byte.Parse(row["NumericPrecision"].ToString());
			}
			if (row["NumericScale"] != DBNull.Value)
			{
				odbcParameter.Scale = byte.Parse(row["NumericScale"].ToString());
			}
			odbcParameter.DbType = (DbType)((int)row["ProviderType"]);
		}

		protected override string GetParameterName(string parameterName)
		{
			return string.Format("@{0}", parameterName);
		}

		protected override string GetParameterPlaceholder(int parameterOrdinal)
		{
			return this.GetParameterName(parameterOrdinal);
		}

		protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
		{
			if (!(adapter is OdbcDataAdapter))
			{
				throw new InvalidOperationException("Adapter needs to be a SqlDataAdapter");
			}
			if (this.rowUpdatingHandler == null)
			{
				this.rowUpdatingHandler = new OdbcRowUpdatingEventHandler(this.OnRowUpdating);
			}
			((OdbcDataAdapter)adapter).RowUpdating += this.rowUpdatingHandler;
		}

		public override string QuoteIdentifier(string unquotedIdentifier)
		{
			return this.QuoteIdentifier(unquotedIdentifier, null);
		}

		public string QuoteIdentifier(string unquotedIdentifier, OdbcConnection connection)
		{
			if (unquotedIdentifier == null)
			{
				throw new ArgumentNullException("unquotedIdentifier");
			}
			string text = this.QuotePrefix;
			string text2 = this.QuoteSuffix;
			if (this.QuotePrefix.Length == 0)
			{
				if (connection == null)
				{
					throw new InvalidOperationException("An open connection is required if QuotePrefix is not set.");
				}
				text2 = (text = this.GetQuoteCharacter(connection));
			}
			if (text.Length > 0 && text != " ")
			{
				string text3;
				if (text2.Length > 0)
				{
					text3 = unquotedIdentifier.Replace(text2, text2 + text2);
				}
				else
				{
					text3 = unquotedIdentifier;
				}
				return text + text3 + text2;
			}
			return unquotedIdentifier;
		}

		public string UnquoteIdentifier(string quotedIdentifier, OdbcConnection connection)
		{
			return this.UnquoteIdentifier(quotedIdentifier);
		}

		public override string UnquoteIdentifier(string quotedIdentifier)
		{
			if (quotedIdentifier == null || quotedIdentifier.Length == 0)
			{
				return quotedIdentifier;
			}
			StringBuilder stringBuilder = new StringBuilder(quotedIdentifier.Length);
			stringBuilder.Append(quotedIdentifier);
			if (quotedIdentifier.StartsWith(this.QuotePrefix))
			{
				stringBuilder.Remove(0, this.QuotePrefix.Length);
			}
			if (quotedIdentifier.EndsWith(this.QuoteSuffix))
			{
				stringBuilder.Remove(stringBuilder.Length - this.QuoteSuffix.Length, this.QuoteSuffix.Length);
			}
			return stringBuilder.ToString();
		}

		private void OnRowUpdating(object sender, OdbcRowUpdatingEventArgs args)
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
					args.Command = this.GetInsertCommand();
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

		private string GetQuotedString(string unquotedIdentifier)
		{
			string quotePrefix = this.QuotePrefix;
			string quoteSuffix = this.QuoteSuffix;
			if (quotePrefix.Length == 0 && quoteSuffix.Length == 0)
			{
				return unquotedIdentifier;
			}
			return string.Format("{0}{1}{2}", quotePrefix, unquotedIdentifier, quoteSuffix);
		}

		private bool IsCommandGenerated
		{
			get
			{
				return this._insertCommand != null || this._updateCommand != null || this._deleteCommand != null;
			}
		}

		private string GetQuoteCharacter(OdbcConnection conn)
		{
			return conn.GetInfo(OdbcInfo.IdentifierQuoteChar);
		}

		private OdbcDataAdapter _adapter;

		private DataTable _schema;

		private string _tableName;

		private OdbcCommand _insertCommand;

		private OdbcCommand _updateCommand;

		private OdbcCommand _deleteCommand;

		private bool _disposed;

		private OdbcRowUpdatingEventHandler rowUpdatingHandler;
	}
}
