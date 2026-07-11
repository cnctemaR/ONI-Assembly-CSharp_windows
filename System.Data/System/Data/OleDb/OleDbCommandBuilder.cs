using System;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.OleDb
{
	public sealed class OleDbCommandBuilder : DbCommandBuilder
	{
		public OleDbCommandBuilder()
		{
		}

		public OleDbCommandBuilder(OleDbDataAdapter adapter)
		{
		}

		[DefaultValue(null)]
		public new OleDbDataAdapter DataAdapter
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

		[MonoTODO]
		public static void DeriveParameters(OleDbCommand command)
		{
		}

		[MonoTODO]
		public new OleDbCommand GetDeleteCommand()
		{
			throw null;
		}

		[MonoTODO]
		public new OleDbCommand GetDeleteCommand(bool useColumnsForParameterNames)
		{
			throw null;
		}

		[MonoTODO]
		public new OleDbCommand GetInsertCommand()
		{
			throw null;
		}

		[MonoTODO]
		public new OleDbCommand GetInsertCommand(bool useColumnsForParameterNames)
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

		[MonoTODO]
		public new OleDbCommand GetUpdateCommand()
		{
			throw null;
		}

		[MonoTODO]
		public new OleDbCommand GetUpdateCommand(bool useColumnsForParameterNames)
		{
			throw null;
		}

		[MonoTODO]
		public override string QuoteIdentifier(string unquotedIdentifier)
		{
			throw null;
		}

		[MonoTODO]
		public string QuoteIdentifier(string unquotedIdentifier, OleDbConnection connection)
		{
			throw null;
		}

		[MonoTODO]
		protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
		{
		}

		[MonoTODO]
		public override string UnquoteIdentifier(string quotedIdentifier)
		{
			throw null;
		}

		[MonoTODO]
		public string UnquoteIdentifier(string quotedIdentifier, OleDbConnection connection)
		{
			throw null;
		}
	}
}
