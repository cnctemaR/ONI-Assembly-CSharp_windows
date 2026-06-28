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
			: this()
		{
			this.adapter = adapter;
		}

		[DefaultValue(null)]
		public new OleDbDataAdapter DataAdapter
		{
			get
			{
				return this.adapter;
			}
			set
			{
				this.adapter = value;
			}
		}

		protected override void ApplyParameterInfo(DbParameter dbParameter, DataRow row, StatementType statementType, bool whereClause)
		{
			OleDbParameter oleDbParameter = (OleDbParameter)dbParameter;
			oleDbParameter.Size = int.Parse(row["ColumnSize"].ToString());
			if (row["NumericPrecision"] != DBNull.Value)
			{
				oleDbParameter.Precision = byte.Parse(row["NumericPrecision"].ToString());
			}
			if (row["NumericScale"] != DBNull.Value)
			{
				oleDbParameter.Scale = byte.Parse(row["NumericScale"].ToString());
			}
			oleDbParameter.DbType = (DbType)((int)row["ProviderType"]);
		}

		[MonoTODO]
		public static void DeriveParameters(OleDbCommand command)
		{
			if (command.CommandType != CommandType.StoredProcedure)
			{
				throw new InvalidOperationException("You can perform this operation only on CommandTye StoredProcedure");
			}
			throw new NotImplementedException();
		}

		[MonoTODO]
		public new OleDbCommand GetDeleteCommand()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public new OleDbCommand GetDeleteCommand(bool useColumnsForParameterNames)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public new OleDbCommand GetInsertCommand()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public new OleDbCommand GetInsertCommand(bool useColumnsForParameterNames)
		{
			throw new NotImplementedException();
		}

		protected override string GetParameterName(int position)
		{
			return string.Format("@p{0}", position);
		}

		protected override string GetParameterName(string parameterName)
		{
			return string.Format("@{0}", parameterName);
		}

		protected override string GetParameterPlaceholder(int position)
		{
			return this.GetParameterName(position);
		}

		[MonoTODO]
		public new OleDbCommand GetUpdateCommand()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public new OleDbCommand GetUpdateCommand(bool useColumnsForParameterNames)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public override string QuoteIdentifier(string unquotedIdentifier)
		{
			return base.QuoteIdentifier(unquotedIdentifier);
		}

		[MonoTODO]
		public string QuoteIdentifier(string unquotedIdentifier, OleDbConnection connection)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public override string UnquoteIdentifier(string quotedIdentifier)
		{
			return base.UnquoteIdentifier(quotedIdentifier);
		}

		[MonoTODO]
		public string UnquoteIdentifier(string quotedIdentifier, OleDbConnection connection)
		{
			throw new NotImplementedException();
		}

		private OleDbDataAdapter adapter;
	}
}
