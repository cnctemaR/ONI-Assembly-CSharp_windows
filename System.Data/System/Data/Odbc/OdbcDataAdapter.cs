using System;
using System.Data.Common;

namespace System.Data.Odbc
{
	public sealed class OdbcDataAdapter : DbDataAdapter, IDbDataAdapter, IDataAdapter, ICloneable
	{
		public OdbcDataAdapter()
		{
			GC.SuppressFinalize(this);
		}

		public OdbcDataAdapter(OdbcCommand selectCommand)
			: this()
		{
			this.SelectCommand = selectCommand;
		}

		public OdbcDataAdapter(string selectCommandText, OdbcConnection selectConnection)
			: this()
		{
			this.SelectCommand = new OdbcCommand(selectCommandText, selectConnection);
		}

		public OdbcDataAdapter(string selectCommandText, string selectConnectionString)
			: this()
		{
			OdbcConnection odbcConnection = new OdbcConnection(selectConnectionString);
			this.SelectCommand = new OdbcCommand(selectCommandText, odbcConnection);
		}

		private OdbcDataAdapter(OdbcDataAdapter from)
			: base(from)
		{
			GC.SuppressFinalize(this);
		}

		public new OdbcCommand DeleteCommand
		{
			get
			{
				return this._deleteCommand;
			}
			set
			{
				this._deleteCommand = value;
			}
		}

		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get
			{
				return this._deleteCommand;
			}
			set
			{
				this._deleteCommand = (OdbcCommand)value;
			}
		}

		public new OdbcCommand InsertCommand
		{
			get
			{
				return this._insertCommand;
			}
			set
			{
				this._insertCommand = value;
			}
		}

		IDbCommand IDbDataAdapter.InsertCommand
		{
			get
			{
				return this._insertCommand;
			}
			set
			{
				this._insertCommand = (OdbcCommand)value;
			}
		}

		public new OdbcCommand SelectCommand
		{
			get
			{
				return this._selectCommand;
			}
			set
			{
				this._selectCommand = value;
			}
		}

		IDbCommand IDbDataAdapter.SelectCommand
		{
			get
			{
				return this._selectCommand;
			}
			set
			{
				this._selectCommand = (OdbcCommand)value;
			}
		}

		public new OdbcCommand UpdateCommand
		{
			get
			{
				return this._updateCommand;
			}
			set
			{
				this._updateCommand = value;
			}
		}

		IDbCommand IDbDataAdapter.UpdateCommand
		{
			get
			{
				return this._updateCommand;
			}
			set
			{
				this._updateCommand = (OdbcCommand)value;
			}
		}

		public event OdbcRowUpdatedEventHandler RowUpdated
		{
			add
			{
				base.Events.AddHandler(OdbcDataAdapter.s_eventRowUpdated, value);
			}
			remove
			{
				base.Events.RemoveHandler(OdbcDataAdapter.s_eventRowUpdated, value);
			}
		}

		public event OdbcRowUpdatingEventHandler RowUpdating
		{
			add
			{
				OdbcRowUpdatingEventHandler odbcRowUpdatingEventHandler = (OdbcRowUpdatingEventHandler)base.Events[OdbcDataAdapter.s_eventRowUpdating];
				if (odbcRowUpdatingEventHandler != null && value.Target is OdbcCommandBuilder)
				{
					OdbcRowUpdatingEventHandler odbcRowUpdatingEventHandler2 = (OdbcRowUpdatingEventHandler)ADP.FindBuilder(odbcRowUpdatingEventHandler);
					if (odbcRowUpdatingEventHandler2 != null)
					{
						base.Events.RemoveHandler(OdbcDataAdapter.s_eventRowUpdating, odbcRowUpdatingEventHandler2);
					}
				}
				base.Events.AddHandler(OdbcDataAdapter.s_eventRowUpdating, value);
			}
			remove
			{
				base.Events.RemoveHandler(OdbcDataAdapter.s_eventRowUpdating, value);
			}
		}

		object ICloneable.Clone()
		{
			return new OdbcDataAdapter(this);
		}

		protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new OdbcRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
		}

		protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new OdbcRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
		}

		protected override void OnRowUpdated(RowUpdatedEventArgs value)
		{
			OdbcRowUpdatedEventHandler odbcRowUpdatedEventHandler = (OdbcRowUpdatedEventHandler)base.Events[OdbcDataAdapter.s_eventRowUpdated];
			if (odbcRowUpdatedEventHandler != null && value is OdbcRowUpdatedEventArgs)
			{
				odbcRowUpdatedEventHandler(this, (OdbcRowUpdatedEventArgs)value);
			}
			base.OnRowUpdated(value);
		}

		protected override void OnRowUpdating(RowUpdatingEventArgs value)
		{
			OdbcRowUpdatingEventHandler odbcRowUpdatingEventHandler = (OdbcRowUpdatingEventHandler)base.Events[OdbcDataAdapter.s_eventRowUpdating];
			if (odbcRowUpdatingEventHandler != null && value is OdbcRowUpdatingEventArgs)
			{
				odbcRowUpdatingEventHandler(this, (OdbcRowUpdatingEventArgs)value);
			}
			base.OnRowUpdating(value);
		}

		private static readonly object s_eventRowUpdated = new object();

		private static readonly object s_eventRowUpdating = new object();

		private OdbcCommand _deleteCommand;

		private OdbcCommand _insertCommand;

		private OdbcCommand _selectCommand;

		private OdbcCommand _updateCommand;
	}
}
