using System;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.OleDb
{
	[DefaultEvent("RecordsAffected")]
	[Designer("Microsoft.VSDesigner.Data.VS.OleDbCommandDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[ToolboxItem("System.Drawing.Design.ToolboxItem, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public sealed class OleDbCommand : DbCommand, IDbCommand, IDisposable, ICloneable
	{
		public OleDbCommand()
		{
		}

		public OleDbCommand(string cmdText)
		{
		}

		public OleDbCommand(string cmdText, OleDbConnection connection)
		{
		}

		public OleDbCommand(string cmdText, OleDbConnection connection, OleDbTransaction transaction)
		{
		}

		[DefaultValue("")]
		[Editor("Microsoft.VSDesigner.Data.ADO.Design.OleDbCommandTextEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[RefreshProperties(RefreshProperties.All)]
		public override string CommandText
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public override int CommandTimeout
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue("Text")]
		[RefreshProperties(RefreshProperties.All)]
		public override CommandType CommandType
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
		[Editor("Microsoft.VSDesigner.Data.Design.DbConnectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public new OleDbConnection Connection
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get
			{
				throw null;
			}
		}

		protected override DbTransaction DbTransaction
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DefaultValue(true)]
		[DesignOnly(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool DesignTimeVisible
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new OleDbParameterCollection Parameters
		{
			get
			{
				throw null;
			}
		}

		IDbConnection IDbCommand.Connection
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		IDataParameterCollection IDbCommand.Parameters
		{
			get
			{
				throw null;
			}
		}

		IDbTransaction IDbCommand.Transaction
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new OleDbTransaction Transaction
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(UpdateRowSource.Both)]
		[MonoTODO]
		public override UpdateRowSource UpdatedRowSource
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[MonoTODO]
		public override void Cancel()
		{
		}

		public OleDbCommand Clone()
		{
			throw null;
		}

		protected override DbParameter CreateDbParameter()
		{
			throw null;
		}

		public new OleDbParameter CreateParameter()
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			throw null;
		}

		public override int ExecuteNonQuery()
		{
			throw null;
		}

		public new OleDbDataReader ExecuteReader()
		{
			throw null;
		}

		public new OleDbDataReader ExecuteReader(CommandBehavior behavior)
		{
			throw null;
		}

		public override object ExecuteScalar()
		{
			throw null;
		}

		[MonoTODO]
		public override void Prepare()
		{
		}

		public void ResetCommandTimeout()
		{
		}

		IDataReader IDbCommand.ExecuteReader()
		{
			throw null;
		}

		IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
		{
			throw null;
		}

		object ICloneable.Clone()
		{
			throw null;
		}
	}
}
