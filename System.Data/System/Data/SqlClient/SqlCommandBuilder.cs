using System;
using System.Data.Common;
using System.Data.Sql;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Data.SqlClient
{
	public sealed class SqlCommandBuilder : DbCommandBuilder
	{
		public SqlCommandBuilder()
		{
			GC.SuppressFinalize(this);
			base.QuotePrefix = "[";
			base.QuoteSuffix = "]";
		}

		public SqlCommandBuilder(SqlDataAdapter adapter)
			: this()
		{
			this.DataAdapter = adapter;
		}

		public override CatalogLocation CatalogLocation
		{
			get
			{
				return CatalogLocation.Start;
			}
			set
			{
				if (CatalogLocation.Start != value)
				{
					throw ADP.SingleValuedProperty("CatalogLocation", "Start");
				}
			}
		}

		public override string CatalogSeparator
		{
			get
			{
				return ".";
			}
			set
			{
				if ("." != value)
				{
					throw ADP.SingleValuedProperty("CatalogSeparator", ".");
				}
			}
		}

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

		public override string QuotePrefix
		{
			get
			{
				return base.QuotePrefix;
			}
			set
			{
				if ("[" != value && "\"" != value)
				{
					throw ADP.DoubleValuedProperty("QuotePrefix", "[", "\"");
				}
				base.QuotePrefix = value;
			}
		}

		public override string QuoteSuffix
		{
			get
			{
				return base.QuoteSuffix;
			}
			set
			{
				if ("]" != value && "\"" != value)
				{
					throw ADP.DoubleValuedProperty("QuoteSuffix", "]", "\"");
				}
				base.QuoteSuffix = value;
			}
		}

		public override string SchemaSeparator
		{
			get
			{
				return ".";
			}
			set
			{
				if ("." != value)
				{
					throw ADP.SingleValuedProperty("SchemaSeparator", ".");
				}
			}
		}

		private void SqlRowUpdatingHandler(object sender, SqlRowUpdatingEventArgs ruevent)
		{
			base.RowUpdatingHandler(ruevent);
		}

		public new SqlCommand GetInsertCommand()
		{
			return (SqlCommand)base.GetInsertCommand();
		}

		public new SqlCommand GetInsertCommand(bool useColumnsForParameterNames)
		{
			return (SqlCommand)base.GetInsertCommand(useColumnsForParameterNames);
		}

		public new SqlCommand GetUpdateCommand()
		{
			return (SqlCommand)base.GetUpdateCommand();
		}

		public new SqlCommand GetUpdateCommand(bool useColumnsForParameterNames)
		{
			return (SqlCommand)base.GetUpdateCommand(useColumnsForParameterNames);
		}

		public new SqlCommand GetDeleteCommand()
		{
			return (SqlCommand)base.GetDeleteCommand();
		}

		public new SqlCommand GetDeleteCommand(bool useColumnsForParameterNames)
		{
			return (SqlCommand)base.GetDeleteCommand(useColumnsForParameterNames);
		}

		protected override void ApplyParameterInfo(DbParameter parameter, DataRow datarow, StatementType statementType, bool whereClause)
		{
			SqlParameter sqlParameter = (SqlParameter)parameter;
			object obj = datarow[SchemaTableColumn.ProviderType];
			sqlParameter.SqlDbType = (SqlDbType)obj;
			sqlParameter.Offset = 0;
			object obj2 = datarow[SchemaTableColumn.NumericPrecision];
			if (DBNull.Value != obj2)
			{
				byte b = (byte)((short)obj2);
				sqlParameter.PrecisionInternal = ((byte.MaxValue != b) ? b : 0);
			}
			obj2 = datarow[SchemaTableColumn.NumericScale];
			if (DBNull.Value != obj2)
			{
				byte b2 = (byte)((short)obj2);
				sqlParameter.ScaleInternal = ((byte.MaxValue != b2) ? b2 : 0);
			}
		}

		protected override string GetParameterName(int parameterOrdinal)
		{
			return "@p" + parameterOrdinal.ToString(CultureInfo.InvariantCulture);
		}

		protected override string GetParameterName(string parameterName)
		{
			return "@" + parameterName;
		}

		protected override string GetParameterPlaceholder(int parameterOrdinal)
		{
			return "@p" + parameterOrdinal.ToString(CultureInfo.InvariantCulture);
		}

		private void ConsistentQuoteDelimiters(string quotePrefix, string quoteSuffix)
		{
			if (("\"" == quotePrefix && "\"" != quoteSuffix) || ("[" == quotePrefix && "]" != quoteSuffix))
			{
				throw ADP.InvalidPrefixSuffix();
			}
		}

		public static void DeriveParameters(SqlCommand command)
		{
			if (command == null)
			{
				throw ADP.ArgumentNull("command");
			}
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				command.DeriveParameters();
			}
			catch (OutOfMemoryException ex)
			{
				if (command != null)
				{
					SqlConnection connection = command.Connection;
					if (connection != null)
					{
						connection.Abort(ex);
					}
				}
				throw;
			}
			catch (StackOverflowException ex2)
			{
				if (command != null)
				{
					SqlConnection connection2 = command.Connection;
					if (connection2 != null)
					{
						connection2.Abort(ex2);
					}
				}
				throw;
			}
			catch (ThreadAbortException ex3)
			{
				if (command != null)
				{
					SqlConnection connection3 = command.Connection;
					if (connection3 != null)
					{
						connection3.Abort(ex3);
					}
				}
				throw;
			}
		}

		protected override DataTable GetSchemaTable(DbCommand srcCommand)
		{
			SqlCommand sqlCommand = srcCommand as SqlCommand;
			SqlNotificationRequest notification = sqlCommand.Notification;
			sqlCommand.Notification = null;
			DataTable schemaTable;
			try
			{
				using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
				{
					schemaTable = sqlDataReader.GetSchemaTable();
				}
			}
			finally
			{
				sqlCommand.Notification = notification;
			}
			return schemaTable;
		}

		protected override DbCommand InitializeCommand(DbCommand command)
		{
			return (SqlCommand)base.InitializeCommand(command);
		}

		public override string QuoteIdentifier(string unquotedIdentifier)
		{
			ADP.CheckArgumentNull(unquotedIdentifier, "unquotedIdentifier");
			string quoteSuffix = this.QuoteSuffix;
			string quotePrefix = this.QuotePrefix;
			this.ConsistentQuoteDelimiters(quotePrefix, quoteSuffix);
			return ADP.BuildQuotedString(quotePrefix, quoteSuffix, unquotedIdentifier);
		}

		protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
		{
			if (adapter == base.DataAdapter)
			{
				((SqlDataAdapter)adapter).RowUpdating -= this.SqlRowUpdatingHandler;
				return;
			}
			((SqlDataAdapter)adapter).RowUpdating += this.SqlRowUpdatingHandler;
		}

		public override string UnquoteIdentifier(string quotedIdentifier)
		{
			ADP.CheckArgumentNull(quotedIdentifier, "quotedIdentifier");
			string quoteSuffix = this.QuoteSuffix;
			string quotePrefix = this.QuotePrefix;
			this.ConsistentQuoteDelimiters(quotePrefix, quoteSuffix);
			string text;
			ADP.RemoveStringQuotes(quotePrefix, quoteSuffix, quotedIdentifier, out text);
			return text;
		}
	}
}
