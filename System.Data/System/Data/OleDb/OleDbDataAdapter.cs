using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbDataAdapter : DbDataAdapter, IDataAdapter, IDbDataAdapter, ICloneable
	{
		public new OleDbCommand DeleteCommand
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public new OleDbCommand InsertCommand
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public new OleDbCommand SelectCommand
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		IDbCommand IDbDataAdapter.InsertCommand
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		IDbCommand IDbDataAdapter.SelectCommand
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		IDbCommand IDbDataAdapter.UpdateCommand
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public new OleDbCommand UpdateCommand
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public OleDbDataAdapter()
		{
		}

		public OleDbDataAdapter(OleDbCommand selectCommand)
		{
			throw ADP.OleDb();
		}

		public OleDbDataAdapter(string selectCommandText, OleDbConnection selectConnection)
		{
			throw ADP.OleDb();
		}

		public OleDbDataAdapter(string selectCommandText, string selectConnectionString)
		{
			throw ADP.OleDb();
		}

		protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			throw ADP.OleDb();
		}

		protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			throw ADP.OleDb();
		}

		public int Fill(DataSet dataSet, object ADODBRecordSet, string srcTable)
		{
			throw ADP.OleDb();
		}

		public int Fill(DataTable dataTable, object ADODBRecordSet)
		{
			throw ADP.OleDb();
		}

		protected override void OnRowUpdated(RowUpdatedEventArgs value)
		{
			throw ADP.OleDb();
		}

		protected override void OnRowUpdating(RowUpdatingEventArgs value)
		{
			throw ADP.OleDb();
		}

		public event OleDbRowUpdatedEventHandler RowUpdated;

		public event OleDbRowUpdatingEventHandler RowUpdating;

		object ICloneable.Clone()
		{
			throw ADP.OleDb();
		}
	}
}
