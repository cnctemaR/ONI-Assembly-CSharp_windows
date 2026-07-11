using System;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Sql;
using System.Xml;

namespace System.Data.SqlClient
{
	[DefaultEvent("RecordsAffected")]
	[Designer("Microsoft.VSDesigner.Data.VS.SqlCommandDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[ToolboxItem("System.Drawing.Design.ToolboxItem, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public sealed class SqlCommand : DbCommand, IDbCommand, IDisposable, ICloneable
	{
		public SqlCommand()
		{
		}

		public SqlCommand(string cmdText)
		{
		}

		public SqlCommand(string cmdText, SqlConnection connection)
		{
		}

		public SqlCommand(string cmdText, SqlConnection connection, SqlTransaction transaction)
		{
		}

		[DefaultValue("")]
		[Editor("Microsoft.VSDesigner.Data.SQL.Design.SqlCommandTextEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
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

		[DefaultValue(CommandType.Text)]
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
		public new SqlConnection Connection
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SqlNotificationRequest Notification
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(true)]
		public bool NotificationAutoEnlist
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
		public new SqlParameterCollection Parameters
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new SqlTransaction Transaction
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

		public event StatementCompletedEventHandler StatementCompleted
		{
			add
			{
			}
			remove
			{
			}
		}

		public IAsyncResult BeginExecuteNonQuery()
		{
			throw null;
		}

		public IAsyncResult BeginExecuteNonQuery(AsyncCallback callback, object stateObject)
		{
			throw null;
		}

		public IAsyncResult BeginExecuteReader()
		{
			throw null;
		}

		public IAsyncResult BeginExecuteReader(AsyncCallback callback, object stateObject)
		{
			throw null;
		}

		public IAsyncResult BeginExecuteReader(AsyncCallback callback, object stateObject, CommandBehavior behavior)
		{
			throw null;
		}

		public IAsyncResult BeginExecuteReader(CommandBehavior behavior)
		{
			throw null;
		}

		public IAsyncResult BeginExecuteXmlReader()
		{
			throw null;
		}

		public IAsyncResult BeginExecuteXmlReader(AsyncCallback callback, object stateObject)
		{
			throw null;
		}

		public override void Cancel()
		{
		}

		public SqlCommand Clone()
		{
			throw null;
		}

		protected override DbParameter CreateDbParameter()
		{
			throw null;
		}

		public new SqlParameter CreateParameter()
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
		}

		public int EndExecuteNonQuery(IAsyncResult asyncResult)
		{
			throw null;
		}

		public SqlDataReader EndExecuteReader(IAsyncResult asyncResult)
		{
			throw null;
		}

		public XmlReader EndExecuteXmlReader(IAsyncResult asyncResult)
		{
			throw null;
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			throw null;
		}

		public override int ExecuteNonQuery()
		{
			throw null;
		}

		public new SqlDataReader ExecuteReader()
		{
			throw null;
		}

		public new SqlDataReader ExecuteReader(CommandBehavior behavior)
		{
			throw null;
		}

		public override object ExecuteScalar()
		{
			throw null;
		}

		public XmlReader ExecuteXmlReader()
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
