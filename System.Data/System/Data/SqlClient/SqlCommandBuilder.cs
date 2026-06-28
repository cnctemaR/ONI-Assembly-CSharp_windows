using System;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.SqlClient
{
	public sealed class SqlCommandBuilder : DbCommandBuilder
	{
		public SqlCommandBuilder()
		{
			this.QuoteSuffix = "]";
			this.QuotePrefix = "[";
		}

		public SqlCommandBuilder(SqlDataAdapter adapter)
			: this()
		{
			this.DataAdapter = adapter;
		}

		[DefaultValue(null)]
		public new SqlDataAdapter DataAdapter
		{
			get
			{
				return (SqlDataAdapter)base.DataAdapter;
			}
			set
			{
				base.DataAdapter = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string QuotePrefix
		{
			get
			{
				return base.QuotePrefix;
			}
			set
			{
				if (value != "[" && value != "\"")
				{
					throw new ArgumentException("Only '[' and '\"' are allowed as value for the 'QuoteSuffix' property.");
				}
				base.QuotePrefix = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string QuoteSuffix
		{
			get
			{
				return base.QuoteSuffix;
			}
			set
			{
				if (value != "]" && value != "\"")
				{
					throw new ArgumentException("Only ']' and '\"' are allowed as value for the 'QuoteSuffix' property.");
				}
				base.QuoteSuffix = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CatalogSeparator
		{
			get
			{
				return this._catalogSeparator;
			}
			set
			{
				if (value != this._catalogSeparator)
				{
					throw new ArgumentException("Only '.' is allowed as value for the 'CatalogSeparator' property.");
				}
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string SchemaSeparator
		{
			get
			{
				return this._schemaSeparator;
			}
			set
			{
				if (value != this._schemaSeparator)
				{
					throw new ArgumentException("Only '.' is allowed as value for the 'SchemaSeparator' property.");
				}
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override CatalogLocation CatalogLocation
		{
			get
			{
				return this._catalogLocation;
			}
			set
			{
				if (value != CatalogLocation.Start)
				{
					throw new ArgumentException("Only 'Start' is allowed as value for the 'CatalogLocation' property.");
				}
			}
		}

		public static void DeriveParameters(SqlCommand command)
		{
			command.DeriveParameters();
		}

		public new SqlCommand GetDeleteCommand()
		{
			return (SqlCommand)base.GetDeleteCommand(false);
		}

		public new SqlCommand GetInsertCommand()
		{
			return (SqlCommand)base.GetInsertCommand(false);
		}

		public new SqlCommand GetUpdateCommand()
		{
			return (SqlCommand)base.GetUpdateCommand(false);
		}

		public new SqlCommand GetUpdateCommand(bool useColumnsForParameterNames)
		{
			return (SqlCommand)base.GetUpdateCommand(useColumnsForParameterNames);
		}

		public new SqlCommand GetDeleteCommand(bool useColumnsForParameterNames)
		{
			return (SqlCommand)base.GetDeleteCommand(useColumnsForParameterNames);
		}

		public new SqlCommand GetInsertCommand(bool useColumnsForParameterNames)
		{
			return (SqlCommand)base.GetInsertCommand(useColumnsForParameterNames);
		}

		public override string QuoteIdentifier(string unquotedIdentifier)
		{
			if (unquotedIdentifier == null)
			{
				throw new ArgumentNullException("unquotedIdentifier");
			}
			string quotePrefix = this.QuotePrefix;
			string quoteSuffix = this.QuoteSuffix;
			if ((quotePrefix == "[" && quoteSuffix != "]") || (quotePrefix == "\"" && quoteSuffix != "\""))
			{
				throw new ArgumentException("The QuotePrefix and QuoteSuffix properties do not match.");
			}
			string text = unquotedIdentifier.Replace(quoteSuffix, quoteSuffix + quoteSuffix);
			return quotePrefix + text + quoteSuffix;
		}

		public override string UnquoteIdentifier(string quotedIdentifier)
		{
			return base.UnquoteIdentifier(quotedIdentifier);
		}

		private bool IncludedInInsert(DataRow schemaRow)
		{
			return (schemaRow.IsNull("IsAutoIncrement") || !(bool)schemaRow["IsAutoIncrement"]) && (schemaRow.IsNull("IsHidden") || !(bool)schemaRow["IsHidden"]) && (schemaRow.IsNull("IsExpression") || !(bool)schemaRow["IsExpression"]) && (schemaRow.IsNull("IsRowVersion") || !(bool)schemaRow["IsRowVersion"]) && (schemaRow.IsNull("IsReadOnly") || !(bool)schemaRow["IsReadOnly"]);
		}

		private bool IncludedInUpdate(DataRow schemaRow)
		{
			return (schemaRow.IsNull("IsAutoIncrement") || !(bool)schemaRow["IsAutoIncrement"]) && (schemaRow.IsNull("IsHidden") || !(bool)schemaRow["IsHidden"]) && (schemaRow.IsNull("IsRowVersion") || !(bool)schemaRow["IsRowVersion"]) && (schemaRow.IsNull("IsExpression") || !(bool)schemaRow["IsExpression"]) && (schemaRow.IsNull("IsReadOnly") || !(bool)schemaRow["IsReadOnly"]);
		}

		private bool IncludedInWhereClause(DataRow schemaRow)
		{
			return !(bool)schemaRow["IsLong"];
		}

		protected override void ApplyParameterInfo(DbParameter parameter, DataRow datarow, StatementType statementType, bool whereClause)
		{
			SqlParameter sqlParameter = (SqlParameter)parameter;
			sqlParameter.SqlDbType = (SqlDbType)((int)datarow["ProviderType"]);
			object obj = datarow["NumericPrecision"];
			if (obj != DBNull.Value)
			{
				short num = (short)obj;
				if (num < 255 && num >= 0)
				{
					sqlParameter.Precision = (byte)num;
				}
			}
			object obj2 = datarow["NumericScale"];
			if (obj2 != DBNull.Value)
			{
				short num2 = (short)obj2;
				if (num2 < 255 && num2 >= 0)
				{
					sqlParameter.Scale = (byte)num2;
				}
			}
		}

		protected override string GetParameterName(int parameterOrdinal)
		{
			return string.Format("@p{0}", parameterOrdinal);
		}

		protected override string GetParameterName(string parameterName)
		{
			return string.Format("@{0}", parameterName);
		}

		protected override string GetParameterPlaceholder(int parameterOrdinal)
		{
			return this.GetParameterName(parameterOrdinal);
		}

		private void RowUpdatingHandler(object sender, SqlRowUpdatingEventArgs args)
		{
			base.RowUpdatingHandler(args);
		}

		protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
		{
			SqlDataAdapter sqlDataAdapter = adapter as SqlDataAdapter;
			if (sqlDataAdapter == null)
			{
				throw new InvalidOperationException("Adapter needs to be a SqlDataAdapter");
			}
			if (sqlDataAdapter != base.DataAdapter)
			{
				sqlDataAdapter.RowUpdating += this.RowUpdatingHandler;
			}
			else
			{
				sqlDataAdapter.RowUpdating -= this.RowUpdatingHandler;
			}
		}

		protected override DataTable GetSchemaTable(DbCommand srcCommand)
		{
			DataTable schemaTable;
			using (SqlDataReader sqlDataReader = (SqlDataReader)srcCommand.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
			{
				schemaTable = sqlDataReader.GetSchemaTable();
			}
			return schemaTable;
		}

		protected override DbCommand InitializeCommand(DbCommand command)
		{
			if (command == null)
			{
				command = new SqlCommand();
			}
			else
			{
				command.CommandTimeout = 30;
				command.Transaction = null;
				command.CommandType = CommandType.Text;
				command.UpdatedRowSource = UpdateRowSource.None;
			}
			return command;
		}

		private readonly string _catalogSeparator = ".";

		private readonly string _schemaSeparator = ".";

		private readonly CatalogLocation _catalogLocation = CatalogLocation.Start;
	}
}
