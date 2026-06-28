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
			this.timeout = 30;
			this.commandType = CommandType.Text;
			this._parameters = new OdbcParameterCollection();
			this.designTimeVisible = true;
			this.updateRowSource = UpdateRowSource.Both;
		}

		public OdbcCommand(string cmdText)
			: this()
		{
			this.commandText = cmdText;
		}

		public OdbcCommand(string cmdText, OdbcConnection connection)
			: this(cmdText)
		{
			this.Connection = connection;
		}

		public OdbcCommand(string cmdText, OdbcConnection connection, OdbcTransaction transaction)
			: this(cmdText, connection)
		{
			this.Transaction = transaction;
		}

		object ICloneable.Clone()
		{
			OdbcCommand odbcCommand = new OdbcCommand();
			odbcCommand.CommandText = this.CommandText;
			odbcCommand.CommandTimeout = this.CommandTimeout;
			odbcCommand.CommandType = this.CommandType;
			odbcCommand.Connection = this.Connection;
			odbcCommand.DesignTimeVisible = this.DesignTimeVisible;
			foreach (object obj in this.Parameters)
			{
				OdbcParameter odbcParameter = (OdbcParameter)obj;
				odbcCommand.Parameters.Add(odbcParameter);
			}
			odbcCommand.Transaction = this.Transaction;
			return odbcCommand;
		}

		internal IntPtr hStmt
		{
			get
			{
				return this.hstmt;
			}
		}

		[DefaultValue("")]
		[OdbcDescription("Command text to execute")]
		[RefreshProperties(RefreshProperties.All)]
		[Editor("Microsoft.VSDesigner.Data.Odbc.Design.OdbcCommandTextEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[OdbcCategory("Data")]
		public override string CommandText
		{
			get
			{
				if (this.commandText == null)
				{
					return string.Empty;
				}
				return this.commandText;
			}
			set
			{
				this.prepared = false;
				this.commandText = value;
			}
		}

		[OdbcDescription("Time to wait for command to execute")]
		public override int CommandTimeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("The property value assigned is less than 0.", "CommandTimeout");
				}
				this.timeout = value;
			}
		}

		[OdbcCategory("Data")]
		[RefreshProperties(RefreshProperties.All)]
		[OdbcDescription("How to interpret the CommandText")]
		[DefaultValue("Text")]
		public override CommandType CommandType
		{
			get
			{
				return this.commandType;
			}
			set
			{
				ExceptionHelper.CheckEnumValue(typeof(CommandType), value);
				this.commandType = value;
			}
		}

		[Editor("Microsoft.VSDesigner.Data.Design.DbConnectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[DefaultValue(null)]
		public new OdbcConnection Connection
		{
			get
			{
				return this.DbConnection as OdbcConnection;
			}
			set
			{
				this.DbConnection = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(true)]
		[DesignOnly(true)]
		public override bool DesignTimeVisible
		{
			get
			{
				return this.designTimeVisible;
			}
			set
			{
				this.designTimeVisible = value;
			}
		}

		[OdbcCategory("Data")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[OdbcDescription("The parameters collection")]
		public new OdbcParameterCollection Parameters
		{
			get
			{
				return base.Parameters as OdbcParameterCollection;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[OdbcDescription("The transaction used by the command")]
		[Browsable(false)]
		public new OdbcTransaction Transaction
		{
			get
			{
				return this.transaction;
			}
			set
			{
				this.transaction = value;
			}
		}

		[DefaultValue(UpdateRowSource.Both)]
		[OdbcDescription("When used by a DataAdapter.Update, how command results are applied to the current DataRow")]
		[OdbcCategory("Behavior")]
		public override UpdateRowSource UpdatedRowSource
		{
			get
			{
				return this.updateRowSource;
			}
			set
			{
				ExceptionHelper.CheckEnumValue(typeof(UpdateRowSource), value);
				this.updateRowSource = value;
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return this.connection;
			}
			set
			{
				this.connection = (OdbcConnection)value;
			}
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get
			{
				return this._parameters;
			}
		}

		protected override DbTransaction DbTransaction
		{
			get
			{
				return this.transaction;
			}
			set
			{
				this.transaction = (OdbcTransaction)value;
			}
		}

		public override void Cancel()
		{
			if (!(this.hstmt != IntPtr.Zero))
			{
				throw new InvalidOperationException();
			}
			OdbcReturn odbcReturn = libodbc.SQLCancel(this.hstmt);
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw this.connection.CreateOdbcException(OdbcHandleType.Stmt, this.hstmt);
			}
		}

		protected override DbParameter CreateDbParameter()
		{
			return this.CreateParameter();
		}

		public new OdbcParameter CreateParameter()
		{
			return new OdbcParameter();
		}

		internal void Unlink()
		{
			if (this.disposed)
			{
				return;
			}
			this.FreeStatement(false);
		}

		protected override void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			this.FreeStatement();
			this.CommandText = null;
			this.Connection = null;
			this.Transaction = null;
			this.Parameters.Clear();
			this.disposed = true;
		}

		private IntPtr ReAllocStatment()
		{
			if (this.hstmt != IntPtr.Zero)
			{
				this.FreeStatement();
			}
			else
			{
				this.Connection.Link(this);
			}
			OdbcReturn odbcReturn = libodbc.SQLAllocHandle(OdbcHandleType.Stmt, this.Connection.hDbc, ref this.hstmt);
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw this.connection.CreateOdbcException(OdbcHandleType.Dbc, this.Connection.hDbc);
			}
			this.disposed = false;
			return this.hstmt;
		}

		private void FreeStatement()
		{
			this.FreeStatement(true);
		}

		private void FreeStatement(bool unlink)
		{
			this.prepared = false;
			if (this.hstmt == IntPtr.Zero)
			{
				return;
			}
			if (unlink)
			{
				this.Connection.Unlink(this);
			}
			OdbcReturn odbcReturn = libodbc.SQLFreeStmt(this.hstmt, libodbc.SQLFreeStmtOptions.Close);
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw this.connection.CreateOdbcException(OdbcHandleType.Stmt, this.hstmt);
			}
			odbcReturn = libodbc.SQLFreeHandle(3, this.hstmt);
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw this.connection.CreateOdbcException(OdbcHandleType.Stmt, this.hstmt);
			}
			this.hstmt = IntPtr.Zero;
		}

		private void ExecSQL(CommandBehavior behavior, bool createReader, string sql)
		{
			if (!this.prepared && this.Parameters.Count == 0)
			{
				this.ReAllocStatment();
				OdbcReturn odbcReturn = libodbc.SQLExecDirect(this.hstmt, sql, -3);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo && odbcReturn != OdbcReturn.NoData)
				{
					throw this.connection.CreateOdbcException(OdbcHandleType.Stmt, this.hstmt);
				}
				return;
			}
			else
			{
				if (!this.prepared)
				{
					this.Prepare();
				}
				this.BindParameters();
				OdbcReturn odbcReturn = libodbc.SQLExecute(this.hstmt);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.connection.CreateOdbcException(OdbcHandleType.Stmt, this.hstmt);
				}
				return;
			}
		}

		internal void FreeIfNotPrepared()
		{
			if (!this.prepared)
			{
				this.FreeStatement();
			}
		}

		public override int ExecuteNonQuery()
		{
			return this.ExecuteNonQuery("ExecuteNonQuery", CommandBehavior.Default, false);
		}

		private int ExecuteNonQuery(string method, CommandBehavior behavior, bool createReader)
		{
			if (this.Connection == null)
			{
				throw new InvalidOperationException(string.Format("{0}: Connection is not set.", method));
			}
			if (this.Connection.State == ConnectionState.Closed)
			{
				throw new InvalidOperationException(string.Format("{0}: Connection state is closed", method));
			}
			if (this.CommandText.Length == 0)
			{
				throw new InvalidOperationException(string.Format("{0}: CommandText is not set.", method));
			}
			this.ExecSQL(behavior, createReader, this.CommandText);
			int num2;
			if (this.CommandText.ToUpper().IndexOf("UPDATE") != -1 || this.CommandText.ToUpper().IndexOf("INSERT") != -1 || this.CommandText.ToUpper().IndexOf("DELETE") != -1)
			{
				int num = 0;
				OdbcReturn odbcReturn = libodbc.SQLRowCount(this.hstmt, ref num);
				num2 = num;
			}
			else
			{
				num2 = -1;
			}
			if (!createReader && !this.prepared)
			{
				this.FreeStatement();
			}
			return num2;
		}

		public override void Prepare()
		{
			this.ReAllocStatment();
			OdbcReturn odbcReturn = libodbc.SQLPrepare(this.hstmt, this.CommandText, this.CommandText.Length);
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw this.connection.CreateOdbcException(OdbcHandleType.Stmt, this.hstmt);
			}
			this.prepared = true;
		}

		private void BindParameters()
		{
			int num = 1;
			foreach (object obj in this.Parameters)
			{
				OdbcParameter odbcParameter = (OdbcParameter)obj;
				odbcParameter.Bind(this, this.hstmt, num);
				odbcParameter.CopyValue();
				num++;
			}
		}

		public new OdbcDataReader ExecuteReader()
		{
			return this.ExecuteReader(CommandBehavior.Default);
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return this.ExecuteReader(behavior);
		}

		public new OdbcDataReader ExecuteReader(CommandBehavior behavior)
		{
			return this.ExecuteReader("ExecuteReader", behavior);
		}

		private OdbcDataReader ExecuteReader(string method, CommandBehavior behavior)
		{
			int num = this.ExecuteNonQuery(method, behavior, true);
			return new OdbcDataReader(this, behavior, num);
		}

		public override object ExecuteScalar()
		{
			object obj = null;
			OdbcDataReader odbcDataReader = this.ExecuteReader("ExecuteScalar", CommandBehavior.Default);
			try
			{
				if (odbcDataReader.Read())
				{
					obj = odbcDataReader[0];
				}
			}
			finally
			{
				odbcDataReader.Close();
			}
			return obj;
		}

		public void ResetCommandTimeout()
		{
			this.CommandTimeout = 30;
		}

		private const int DEFAULT_COMMAND_TIMEOUT = 30;

		private string commandText;

		private int timeout;

		private CommandType commandType;

		private UpdateRowSource updateRowSource;

		private OdbcConnection connection;

		private OdbcTransaction transaction;

		private OdbcParameterCollection _parameters;

		private bool designTimeVisible;

		private bool prepared;

		private IntPtr hstmt = IntPtr.Zero;

		private bool disposed;
	}
}
