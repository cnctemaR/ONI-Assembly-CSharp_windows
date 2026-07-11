using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbCommandBuilder : DbCommandBuilder
	{
		public OleDbCommandBuilder()
		{
			throw ADP.OleDb();
		}

		public OleDbCommandBuilder(OleDbDataAdapter adapter)
		{
			throw ADP.OleDb();
		}

		public new OleDbDataAdapter DataAdapter
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		protected override void ApplyParameterInfo(DbParameter parameter, DataRow datarow, StatementType statementType, bool whereClause)
		{
			throw ADP.OleDb();
		}

		public static void DeriveParameters(OleDbCommand command)
		{
			throw ADP.OleDb();
		}

		public new OleDbCommand GetDeleteCommand()
		{
			throw ADP.OleDb();
		}

		public new OleDbCommand GetDeleteCommand(bool useColumnsForParameterNames)
		{
			throw ADP.OleDb();
		}

		public new OleDbCommand GetInsertCommand()
		{
			throw ADP.OleDb();
		}

		public new OleDbCommand GetInsertCommand(bool useColumnsForParameterNames)
		{
			throw ADP.OleDb();
		}

		protected override string GetParameterName(int parameterOrdinal)
		{
			throw ADP.OleDb();
		}

		protected override string GetParameterName(string parameterName)
		{
			throw ADP.OleDb();
		}

		protected override string GetParameterPlaceholder(int parameterOrdinal)
		{
			throw ADP.OleDb();
		}

		public new OleDbCommand GetUpdateCommand()
		{
			throw ADP.OleDb();
		}

		public new OleDbCommand GetUpdateCommand(bool useColumnsForParameterNames)
		{
			throw ADP.OleDb();
		}

		public override string QuoteIdentifier(string unquotedIdentifier)
		{
			throw ADP.OleDb();
		}

		public string QuoteIdentifier(string unquotedIdentifier, OleDbConnection connection)
		{
			throw ADP.OleDb();
		}

		protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
		{
			throw ADP.OleDb();
		}

		public override string UnquoteIdentifier(string quotedIdentifier)
		{
			throw ADP.OleDb();
		}

		public string UnquoteIdentifier(string quotedIdentifier, OleDbConnection connection)
		{
			throw ADP.OleDb();
		}
	}
}
