using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace System.Data.OleDb
{
	[Designer("Microsoft.VSDesigner.Data.VS.OleDbCommandDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[DefaultEvent("RecordsAffected")]
	[ToolboxItem("System.Drawing.Design.ToolboxItem, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public sealed class OleDbCommand : DbCommand, IDisposable, IDbCommand, ICloneable
	{
		public OleDbCommand()
		{
			this.timeout = 30;
			this.commandType = CommandType.Text;
			this.parameters = new OleDbParameterCollection();
			this.behavior = CommandBehavior.Default;
			this.gdaCommand = IntPtr.Zero;
			this.designTimeVisible = true;
			this.updatedRowSource = UpdateRowSource.Both;
		}

		public OleDbCommand(string cmdText)
			: this()
		{
			this.CommandText = cmdText;
		}

		public OleDbCommand(string cmdText, OleDbConnection connection)
			: this(cmdText)
		{
			this.Connection = connection;
		}

		public OleDbCommand(string cmdText, OleDbConnection connection, OleDbTransaction transaction)
			: this(cmdText, connection)
		{
			this.transaction = transaction;
		}

		IDbConnection IDbCommand.Connection
		{
			get
			{
				return this.Connection;
			}
			set
			{
				this.Connection = (OleDbConnection)value;
			}
		}

		IDataParameterCollection IDbCommand.Parameters
		{
			get
			{
				return this.Parameters;
			}
		}

		IDbTransaction IDbCommand.Transaction
		{
			get
			{
				return this.Transaction;
			}
			set
			{
				this.Transaction = (OleDbTransaction)value;
			}
		}

		IDataReader IDbCommand.ExecuteReader()
		{
			return this.ExecuteReader();
		}

		IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
		{
			return this.ExecuteReader(behavior);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		[DataCategory("Data")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("")]
		[Editor("Microsoft.VSDesigner.Data.ADO.Design.OleDbCommandTextEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
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
				this.commandText = value;
			}
		}

		public override int CommandTimeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		[DefaultValue("Text")]
		[DataCategory("Data")]
		[RefreshProperties(RefreshProperties.All)]
		public override CommandType CommandType
		{
			get
			{
				return this.commandType;
			}
			set
			{
				this.commandType = value;
			}
		}

		[Editor("Microsoft.VSDesigner.Data.Design.DbConnectionEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[DataCategory("Behavior")]
		[DefaultValue(null)]
		public new OleDbConnection Connection
		{
			get
			{
				return this.connection;
			}
			set
			{
				this.connection = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignOnly(true)]
		[DefaultValue(true)]
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

		[DataCategory("Data")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new OleDbParameterCollection Parameters
		{
			get
			{
				return this.parameters;
			}
			internal set
			{
				this.parameters = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public new OleDbTransaction Transaction
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

		[DataCategory("Behavior")]
		[MonoTODO]
		[DefaultValue(UpdateRowSource.Both)]
		public override UpdateRowSource UpdatedRowSource
		{
			get
			{
				return this.updatedRowSource;
			}
			set
			{
				ExceptionHelper.CheckEnumValue(typeof(UpdateRowSource), value);
				this.updatedRowSource = value;
			}
		}

		[MonoTODO]
		public override void Cancel()
		{
			throw new NotImplementedException();
		}

		public new OleDbParameter CreateParameter()
		{
			return new OleDbParameter();
		}

		protected override void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			this.Connection = null;
			this.Transaction = null;
			this.disposed = true;
		}

		private void SetupGdaCommand()
		{
			CommandType commandType = this.commandType;
			GdaCommandType gdaCommandType;
			switch (commandType)
			{
			case CommandType.Text:
				break;
			default:
				if (commandType == CommandType.TableDirect)
				{
					gdaCommandType = GdaCommandType.Table;
					goto IL_0044;
				}
				break;
			case CommandType.StoredProcedure:
				gdaCommandType = GdaCommandType.Procedure;
				goto IL_0044;
			}
			gdaCommandType = GdaCommandType.Sql;
			IL_0044:
			if (this.gdaCommand != IntPtr.Zero)
			{
				libgda.gda_command_set_text(this.gdaCommand, this.CommandText);
				libgda.gda_command_set_command_type(this.gdaCommand, gdaCommandType);
			}
			else
			{
				this.gdaCommand = libgda.gda_command_new(this.CommandText, gdaCommandType, (GdaCommandOptions)0);
			}
		}

		public override int ExecuteNonQuery()
		{
			if (this.connection == null)
			{
				throw new InvalidOperationException("connection == null");
			}
			if (this.connection.State == ConnectionState.Closed)
			{
				throw new InvalidOperationException("State == Closed");
			}
			IntPtr gdaConnection = this.connection.GdaConnection;
			IntPtr gdaParameterList = this.parameters.GdaParameterList;
			this.SetupGdaCommand();
			return libgda.gda_connection_execute_non_query(gdaConnection, this.gdaCommand, gdaParameterList);
		}

		public new OleDbDataReader ExecuteReader()
		{
			return this.ExecuteReader(this.behavior);
		}

		public new OleDbDataReader ExecuteReader(CommandBehavior behavior)
		{
			ArrayList arrayList = new ArrayList();
			if (this.connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException("State != Open");
			}
			this.behavior = behavior;
			IntPtr gdaConnection = this.connection.GdaConnection;
			IntPtr gdaParameterList = this.parameters.GdaParameterList;
			this.SetupGdaCommand();
			IntPtr intPtr = libgda.gda_connection_execute_command(gdaConnection, this.gdaCommand, gdaParameterList);
			if (intPtr != IntPtr.Zero)
			{
				for (GdaList gdaList = (GdaList)Marshal.PtrToStructure(intPtr, typeof(GdaList)); gdaList != null; gdaList = (GdaList)Marshal.PtrToStructure(gdaList.next, typeof(GdaList)))
				{
					arrayList.Add(gdaList.data);
					if (gdaList.next == IntPtr.Zero)
					{
						break;
					}
				}
				this.dataReader = new OleDbDataReader(this, arrayList);
				this.dataReader.NextResult();
			}
			return this.dataReader;
		}

		public override object ExecuteScalar()
		{
			this.SetupGdaCommand();
			OleDbDataReader oleDbDataReader = this.ExecuteReader();
			if (oleDbDataReader == null)
			{
				return null;
			}
			if (!oleDbDataReader.Read())
			{
				oleDbDataReader.Close();
				return null;
			}
			object value = oleDbDataReader.GetValue(0);
			oleDbDataReader.Close();
			return value;
		}

		public OleDbCommand Clone()
		{
			return new OleDbCommand
			{
				CommandText = this.CommandText,
				CommandTimeout = this.CommandTimeout,
				CommandType = this.CommandType,
				Connection = this.Connection,
				DesignTimeVisible = this.DesignTimeVisible,
				Parameters = this.Parameters,
				Transaction = this.Transaction
			};
		}

		[MonoTODO]
		public override void Prepare()
		{
			throw new NotImplementedException();
		}

		public void ResetCommandTimeout()
		{
			this.timeout = 30;
		}

		protected override DbParameter CreateDbParameter()
		{
			return this.CreateParameter();
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return this.ExecuteReader(behavior);
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return this.Connection;
			}
			set
			{
				this.Connection = (OleDbConnection)value;
			}
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get
			{
				return this.Parameters;
			}
		}

		protected override DbTransaction DbTransaction
		{
			get
			{
				return this.Transaction;
			}
			set
			{
				this.Transaction = (OleDbTransaction)value;
			}
		}

		private const int DEFAULT_COMMAND_TIMEOUT = 30;

		private string commandText;

		private int timeout;

		private CommandType commandType;

		private OleDbConnection connection;

		private OleDbParameterCollection parameters;

		private OleDbTransaction transaction;

		private bool designTimeVisible;

		private OleDbDataReader dataReader;

		private CommandBehavior behavior;

		private IntPtr gdaCommand;

		private UpdateRowSource updatedRowSource;

		private bool disposed;
	}
}
