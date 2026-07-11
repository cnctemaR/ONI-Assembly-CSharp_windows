using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	public sealed class OleDbRowUpdatedEventArgs : RowUpdatedEventArgs
	{
		public OleDbRowUpdatedEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(null, null, StatementType.Select, null)
		{
		}

		public new OleDbCommand Command
		{
			get
			{
				throw null;
			}
		}
	}
}
