using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbRowUpdatingEventArgs : RowUpdatingEventArgs
	{
		protected override IDbCommand BaseCommand
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public new OleDbCommand Command
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public OleDbRowUpdatingEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(null, null, StatementType.Select, null)
		{
			throw ADP.OleDb();
		}
	}
}
