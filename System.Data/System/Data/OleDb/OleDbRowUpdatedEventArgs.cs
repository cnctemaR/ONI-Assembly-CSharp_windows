using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	public sealed class OleDbRowUpdatedEventArgs : RowUpdatedEventArgs
	{
		public OleDbRowUpdatedEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(dataRow, command, statementType, tableMapping)
		{
		}

		public new OleDbCommand Command
		{
			get
			{
				return (OleDbCommand)base.Command;
			}
		}
	}
}
