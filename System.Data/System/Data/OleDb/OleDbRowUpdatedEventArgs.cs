using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbRowUpdatedEventArgs : RowUpdatedEventArgs
	{
		public new OleDbCommand Command
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public OleDbRowUpdatedEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(null, null, StatementType.Select, null)
		{
			throw ADP.OleDb();
		}
	}
}
