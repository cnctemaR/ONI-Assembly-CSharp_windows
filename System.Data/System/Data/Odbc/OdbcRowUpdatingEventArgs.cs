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
				return (OdbcCommand)base.Command;
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
				return base.Command;
			}
			set
			{
				base.Command = value;
			}
		}
	}
}
