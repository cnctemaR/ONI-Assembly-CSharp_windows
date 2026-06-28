using System;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.Odbc
{
	[DefaultEvent("RowUpdated")]
	[Designer("Microsoft.VSDesigner.Data.VS.OdbcDataAdapterDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[ToolboxItem("Microsoft.VSDesigner.Data.VS.OdbcDataAdapterToolboxItem, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public sealed class OdbcDataAdapter : DbDataAdapter, IDataAdapter, IDbDataAdapter, ICloneable
	{
		public OdbcDataAdapter()
			: this(null)
		{
		}

		public OdbcDataAdapter(OdbcCommand selectCommand)
		{
			this.SelectCommand = selectCommand;
		}

		public OdbcDataAdapter(string selectCommandText, OdbcConnection selectConnection)
			: this(new OdbcCommand(selectCommandText, selectConnection))
		{
		}

		public OdbcDataAdapter(string selectCommandText, string selectConnectionString)
			: this(selectCommandText, new OdbcConnection(selectConnectionString))
		{
		}

		public event OdbcRowUpdatedEventHandler RowUpdated;

		public event OdbcRowUpdatingEventHandler RowUpdating;

		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get
			{
				return this.DeleteCommand;
			}
			set
			{
				this.DeleteCommand = (OdbcCommand)value;
			}
		}

		IDbCommand IDbDataAdapter.InsertCommand
		{
			get
			{
				return this.InsertCommand;
			}
			set
			{
				this.InsertCommand = (OdbcCommand)value;
			}
		}

		IDbCommand IDbDataAdapter.SelectCommand
		{
			get
			{
				return this.SelectCommand;
			}
			set
			{
				this.SelectCommand = (OdbcCommand)value;
			}
		}

		IDbCommand IDbDataAdapter.UpdateCommand
		{
			get
			{
				return this.UpdateCommand;
			}
			set
			{
				this.UpdateCommand = (OdbcCommand)value;
			}
		}

		[MonoTODO]
		object ICloneable.Clone()
		{
			throw new NotImplementedException();
		}

		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[OdbcDescription("Used during Update for deleted rows in DataSet.")]
		[OdbcCategory("Update")]
		[DefaultValue(null)]
		public new OdbcCommand DeleteCommand
		{
			get
			{
				return this.deleteCommand;
			}
			set
			{
				this.deleteCommand = value;
			}
		}

		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[OdbcDescription("Used during Update for new rows in DataSet.")]
		[OdbcCategory("Update")]
		[DefaultValue(null)]
		public new OdbcCommand InsertCommand
		{
			get
			{
				return this.insertCommand;
			}
			set
			{
				this.insertCommand = value;
			}
		}

		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[OdbcCategory("Fill")]
		[OdbcDescription("Used during Fill/FillSchema.")]
		[DefaultValue(null)]
		public new OdbcCommand SelectCommand
		{
			get
			{
				return this.selectCommand;
			}
			set
			{
				this.selectCommand = value;
			}
		}

		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[OdbcCategory("Update")]
		[OdbcDescription("Used during Update for modified rows in DataSet.")]
		[DefaultValue(null)]
		public new OdbcCommand UpdateCommand
		{
			get
			{
				return this.updateCommand;
			}
			set
			{
				this.updateCommand = value;
			}
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
			if (this.RowUpdated != null)
			{
				this.RowUpdated(this, (OdbcRowUpdatedEventArgs)value);
			}
		}

		protected override void OnRowUpdating(RowUpdatingEventArgs value)
		{
			if (this.RowUpdating != null)
			{
				this.RowUpdating(this, (OdbcRowUpdatingEventArgs)value);
			}
		}

		private OdbcCommand deleteCommand;

		private OdbcCommand insertCommand;

		private OdbcCommand selectCommand;

		private OdbcCommand updateCommand;
	}
}
