using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	public sealed class OleDbRowUpdatingEventArgs : RowUpdatingEventArgs
	{
		public OleDbRowUpdatingEventArgs(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
			: base(dataRow, command, statementType, tableMapping)
		{
		}

		public new OleDbCommand Command
		{
			get
			{
				return (OleDbCommand)base.Command;
			}
			set
			{
				base.Command = value;
			}
		}

		protected override IDbCommand BaseCommand
		{
			get
			{
				return base.BaseCommand;
			}
			set
			{
				base.BaseCommand = value;
			}
		}
	}
}
