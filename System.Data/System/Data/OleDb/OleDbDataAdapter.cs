using System;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.OleDb
{
	[ToolboxItem("Microsoft.VSDesigner.Data.VS.OleDbDataAdapterToolboxItem, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[Designer("Microsoft.VSDesigner.Data.VS.OleDbDataAdapterDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[DefaultEvent("RowUpdated")]
	public sealed class OleDbDataAdapter : DbDataAdapter, IDataAdapter, IDbDataAdapter, ICloneable
	{
		public OleDbDataAdapter()
			: this(null)
		{
		}

		public OleDbDataAdapter(OleDbCommand selectCommand)
		{
			this.SelectCommand = selectCommand;
		}

		public OleDbDataAdapter(string selectCommandText, OleDbConnection selectConnection)
			: this(new OleDbCommand(selectCommandText, selectConnection))
		{
		}

		public OleDbDataAdapter(string selectCommandText, string selectConnectionString)
			: this(selectCommandText, new OleDbConnection(selectConnectionString))
		{
		}

		[DataCategory("DataCategory_Update")]
		public event OleDbRowUpdatedEventHandler RowUpdated;

		[DataCategory("DataCategory_Update")]
		public event OleDbRowUpdatingEventHandler RowUpdating;

		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get
			{
				return this.DeleteCommand;
			}
			set
			{
				this.DeleteCommand = (OleDbCommand)value;
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
				this.InsertCommand = (OleDbCommand)value;
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
				this.SelectCommand = (OleDbCommand)value;
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
				this.UpdateCommand = (OleDbCommand)value;
			}
		}

		[MonoTODO]
		object ICloneable.Clone()
		{
			throw new NotImplementedException();
		}

		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[DataCategory("Update")]
		[DefaultValue(null)]
		public new OleDbCommand DeleteCommand
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
		[DataCategory("Update")]
		[DefaultValue(null)]
		public new OleDbCommand InsertCommand
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
		[DefaultValue(null)]
		[DataCategory("Fill")]
		public new OleDbCommand SelectCommand
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

		[DataCategory("Update")]
		[DefaultValue(null)]
		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public new OleDbCommand UpdateCommand
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
			return new OleDbRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
		}

		protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new OleDbRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
		}

		protected override void OnRowUpdated(RowUpdatedEventArgs value)
		{
			if (this.RowUpdated != null)
			{
				this.RowUpdated(this, (OleDbRowUpdatedEventArgs)value);
			}
		}

		protected override void OnRowUpdating(RowUpdatingEventArgs value)
		{
			if (this.RowUpdating != null)
			{
				this.RowUpdating(this, (OleDbRowUpdatingEventArgs)value);
			}
		}

		[MonoTODO]
		public int Fill(DataTable dataTable, object ADODBRecordSet)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public int Fill(DataSet dataSet, object ADODBRecordSet, string srcTable)
		{
			throw new NotImplementedException();
		}

		private OleDbCommand deleteCommand;

		private OleDbCommand insertCommand;

		private OleDbCommand selectCommand;

		private OleDbCommand updateCommand;
	}
}
