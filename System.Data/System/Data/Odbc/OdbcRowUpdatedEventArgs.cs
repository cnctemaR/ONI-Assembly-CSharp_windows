using System;
using System.Data.Common;

namespace System.Data.Odbc
{
	public sealed class OdbcRowUpdatedEventArgs : RowUpdatedEventArgs
	{
		public OdbcRowUpdatedEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(null, null, StatementType.Select, null)
		{
		}

		public new OdbcCommand Command
		{
			get
			{
				throw null;
			}
		}
	}
}
