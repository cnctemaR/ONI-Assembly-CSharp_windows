using System;
using System.Data.Common;

namespace System.Data.Odbc
{
	public sealed class OdbcRowUpdatingEventArgs : RowUpdatingEventArgs
	{
		public OdbcRowUpdatingEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(null, null, StatementType.Select, null)
		{
		}

		protected override IDbCommand BaseCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public new OdbcCommand Command
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}
	}
}
