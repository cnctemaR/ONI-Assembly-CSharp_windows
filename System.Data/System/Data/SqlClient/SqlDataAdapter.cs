using System;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.SqlClient
{
	[DefaultEvent("RowUpdated")]
	[Designer("Microsoft.VSDesigner.Data.VS.SqlDataAdapterDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[ToolboxItem("Microsoft.VSDesigner.Data.VS.SqlDataAdapterToolboxItem, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public sealed class SqlDataAdapter : DbDataAdapter, IDataAdapter, IDbDataAdapter, ICloneable
	{
		public SqlDataAdapter()
		{
		}

		public SqlDataAdapter(SqlCommand selectCommand)
		{
		}

		public SqlDataAdapter(string selectCommandText, SqlConnection selectConnection)
		{
		}

		public SqlDataAdapter(string selectCommandText, string selectConnectionString)
		{
		}

		[DefaultValue(null)]
		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public new SqlCommand DeleteCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(null)]
		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public new SqlCommand InsertCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(null)]
		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public new SqlCommand SelectCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		IDbCommand IDbDataAdapter.InsertCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		IDbCommand IDbDataAdapter.SelectCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		IDbCommand IDbDataAdapter.UpdateCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override int UpdateBatchSize
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(null)]
		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public new SqlCommand UpdateCommand
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public event SqlRowUpdatedEventHandler RowUpdated
		{
			add
			{
			}
			remove
			{
			}
		}

		public event SqlRowUpdatingEventHandler RowUpdating
		{
			add
			{
			}
			remove
			{
			}
		}

		[MonoTODO]
		protected override int AddToBatch(IDbCommand command)
		{
			throw null;
		}

		[MonoTODO]
		protected override void ClearBatch()
		{
		}

		protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			throw null;
		}

		protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			throw null;
		}

		[MonoTODO]
		protected override int ExecuteBatch()
		{
			throw null;
		}

		[MonoTODO]
		protected override IDataParameter GetBatchedParameter(int commandIdentifier, int parameterIndex)
		{
			throw null;
		}

		[MonoTODO]
		protected override void InitializeBatching()
		{
		}

		protected override void OnRowUpdated(RowUpdatedEventArgs value)
		{
		}

		protected override void OnRowUpdating(RowUpdatingEventArgs value)
		{
		}

		[MonoTODO]
		object ICloneable.Clone()
		{
			throw null;
		}

		[MonoTODO]
		protected override void TerminateBatching()
		{
		}
	}
}
