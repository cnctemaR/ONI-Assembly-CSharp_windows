using System;
using System.Data.Common;

namespace System.Data.Odbc
{
	public sealed class OdbcRowUpdatedEventArgs : RowUpdatedEventArgs
	{
		public OdbcRowUpdatedEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(row, command, statementType, tableMapping)
		{
		}

		public new OdbcCommand Command
		{
			get
			{
				return (OdbcCommand)base.Command;
			}
		}
	}
}
