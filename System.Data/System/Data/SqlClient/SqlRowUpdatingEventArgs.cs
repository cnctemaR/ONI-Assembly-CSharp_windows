using System;
using System.Data.Common;

namespace System.Data.SqlClient
{
	public sealed class SqlRowUpdatingEventArgs : RowUpdatingEventArgs
	{
		public SqlRowUpdatingEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
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

		public new SqlCommand Command
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
