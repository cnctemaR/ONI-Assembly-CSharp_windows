using System;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.Odbc
{
	[DefaultEvent("RecordsAffected")]
	[Designer("Microsoft.VSDesigner.Data.VS.OdbcCommandDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[ToolboxItem("System.Drawing.Design.ToolboxItem, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public sealed class OdbcCommand : DbCommand, ICloneable
	{
		public OdbcCommand()
		{
		}

		public OdbcCommand(string cmdText)
		{
		}

		public OdbcCommand(string cmdText, OdbcConnection connection)
		{
		}

		public OdbcCommand(string cmdText, OdbcConnection connection, OdbcTransaction transaction)
		{
		}

		[DefaultValue("")]
		[Editor("Microsoft.VSDesigner.Data.Odbc.Design.OdbcCommandTextEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
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
		public new OdbcConnection Connection
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
		public new OdbcParameterCollection Parameters
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new OdbcTransaction Transaction
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

		public override void Cancel()
		{
		}

		protected override DbParameter CreateDbParameter()
		{
			throw null;
		}

		public new OdbcParameter CreateParameter()
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

		public new OdbcDataReader ExecuteReader()
		{
			throw null;
		}

		public new OdbcDataReader ExecuteReader(CommandBehavior behavior)
		{
			throw null;
		}

		public override object ExecuteScalar()
		{
			throw null;
		}

		public override void Prepare()
		{
		}

		public void ResetCommandTimeout()
		{
		}

		object ICloneable.Clone()
		{
			throw null;
		}
	}
}
