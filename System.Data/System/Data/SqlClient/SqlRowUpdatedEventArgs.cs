using System;
using System.Data.Common;

namespace System.Data.SqlClient
{
	public sealed class SqlRowUpdatedEventArgs : RowUpdatedEventArgs
	{
		public SqlRowUpdatedEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(row, command, statementType, tableMapping)
		{
		}

		public new SqlCommand Command
		{
			get
			{
				return (SqlCommand)base.Command;
			}
		}
	}
}
