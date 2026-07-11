using System;
using System.Data.Common;

namespace System.Data.SqlClient
{
	public sealed class SqlRowUpdatedEventArgs : RowUpdatedEventArgs
	{
		public SqlRowUpdatedEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(null, null, StatementType.Select, null)
		{
		}

		public new SqlCommand Command
		{
			get
			{
				throw null;
			}
		}
	}
}
