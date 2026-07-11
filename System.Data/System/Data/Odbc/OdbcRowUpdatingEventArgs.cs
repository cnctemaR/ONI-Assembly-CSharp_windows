using System;
using System.Data.Common;

namespace System.Data.Odbc
{
	public sealed class OdbcRowUpdatingEventArgs : RowUpdatingEventArgs
	{
		public OdbcRowUpdatingEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(row, command, statementType, tableMapping)
		{
		}

		public new OdbcCommand Command
		{
			get
			{
				return base.Command as OdbcCommand;
			}
			set
			{
				base.Command = value;
			}
		}

		protected override IDbCommand BaseCommand
		{
			get
			{
				return base.BaseCommand;
			}
			set
			{
				base.BaseCommand = value as OdbcCommand;
			}
		}
	}
}
