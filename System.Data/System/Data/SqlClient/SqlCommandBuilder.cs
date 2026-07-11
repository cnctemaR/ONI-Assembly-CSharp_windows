using System;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.SqlClient
{
	public sealed class SqlCommandBuilder : DbCommandBuilder
	{
		public SqlCommandBuilder()
		{
		}

		public SqlCommandBuilder(SqlDataAdapter adapter)
		{
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override CatalogLocation CatalogLocation
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
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string CatalogSeparator
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(null)]
		public new SqlDataAdapter DataAdapter
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
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string QuotePrefix
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
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string QuoteSuffix
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
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string SchemaSeparator
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		protected override void ApplyParameterInfo(DbParameter parameter, DataRow datarow, StatementType statementType, bool whereClause)
		{
		}

		public static void DeriveParameters(SqlCommand command)
		{
		}

		public new SqlCommand GetDeleteCommand()
		{
			throw null;
		}

		public new SqlCommand GetDeleteCommand(bool useColumnsForParameterNames)
		{
			throw null;
		}

		public new SqlCommand GetInsertCommand()
		{
			throw null;
		}

		public new SqlCommand GetInsertCommand(bool useColumnsForParameterNames)
		{
			throw null;
		}

		protected override string GetParameterName(int parameterOrdinal)
		{
			throw null;
		}

		protected override string GetParameterName(string parameterName)
		{
			throw null;
		}

		protected override string GetParameterPlaceholder(int parameterOrdinal)
		{
			throw null;
		}

		protected override DataTable GetSchemaTable(DbCommand srcCommand)
		{
			throw null;
		}

		public new SqlCommand GetUpdateCommand()
		{
			throw null;
		}

		public new SqlCommand GetUpdateCommand(bool useColumnsForParameterNames)
		{
			throw null;
		}

		protected override DbCommand InitializeCommand(DbCommand command)
		{
			throw null;
		}

		public override string QuoteIdentifier(string unquotedIdentifier)
		{
			throw null;
		}

		protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
		{
		}

		public override string UnquoteIdentifier(string quotedIdentifier)
		{
			throw null;
		}
	}
}
