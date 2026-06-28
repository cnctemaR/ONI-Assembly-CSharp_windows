using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.EnterpriseServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Transactions;

namespace System.Data.Odbc
{
	[DefaultEvent("InfoMessage")]
	public sealed class OdbcConnection : DbConnection, ICloneable
	{
		public OdbcConnection()
			: this(string.Empty)
		{
		}

		public OdbcConnection(string connectionString)
		{
			this.connectionTimeout = 15;
			this.ConnectionString = connectionString;
		}

		[OdbcCategory("DataCategory_InfoMessage")]
		[OdbcDescription("DbConnection_InfoMessage")]
		public event OdbcInfoMessageEventHandler InfoMessage;

		[MonoTODO]
		object ICloneable.Clone()
		{
			throw new NotImplementedException();
		}

		internal IntPtr hDbc
		{
			get
			{
				return this.hdbc;
			}
		}

		[RecommendedAsConfigurable(true)]
		[Editor("Microsoft.VSDesigner.Data.Odbc.Design.OdbcConnectionStringEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[RefreshProperties(RefreshProperties.All)]
		[OdbcDescription("Information used to connect to a Data Source")]
		[DefaultValue("")]
		[OdbcCategory("DataCategory_Data")]
		public override string ConnectionString
		{
			get
			{
				if (this.connectionString == null)
				{
					return string.Empty;
				}
				return this.connectionString;
			}
			set
			{
				this.connectionString = value;
			}
		}

		[OdbcDescription("Current connection timeout value, not settable  in the ConnectionString")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(15)]
		public new int ConnectionTimeout
		{
			get
			{
				return this.connectionTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("Timout should not be less than zero.");
				}
				this.connectionTimeout = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[OdbcDescription("Current data source Catlog value, 'Database=X' in the ConnectionString")]
		public override string Database
		{
			get
			{
				if (this.State == ConnectionState.Closed)
				{
					return string.Empty;
				}
				return this.GetInfo(OdbcInfo.DatabaseName);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[OdbcDescription("The ConnectionState indicating whether the connection is open or closed")]
		[Browsable(false)]
		public override ConnectionState State
		{
			get
			{
				if (this.hdbc != IntPtr.Zero)
				{
					return ConnectionState.Open;
				}
				return ConnectionState.Closed;
			}
		}

		[OdbcDescription("Current data source, 'Server=X' in the ConnectionString")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public override string DataSource
		{
			get
			{
				if (this.State == ConnectionState.Closed)
				{
					return string.Empty;
				}
				return this.GetInfo(OdbcInfo.DataSourceName);
			}
		}

		[Browsable(false)]
		[OdbcDescription("Current ODBC Driver")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Driver
		{
			get
			{
				if (this.State == ConnectionState.Closed)
				{
					return string.Empty;
				}
				return this.GetInfo(OdbcInfo.DriverName);
			}
		}

		[OdbcDescription("Version of the product accessed by the ODBC Driver")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string ServerVersion
		{
			get
			{
				return this.GetInfo(OdbcInfo.DbmsVersion);
			}
		}

		internal string SafeDriver
		{
			get
			{
				string safeInfo = this.GetSafeInfo(OdbcInfo.DriverName);
				if (safeInfo == null)
				{
					return string.Empty;
				}
				return safeInfo;
			}
		}

		public new OdbcTransaction BeginTransaction()
		{
			return this.BeginTransaction(IsolationLevel.Unspecified);
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			return this.BeginTransaction(isolationLevel);
		}

		public new OdbcTransaction BeginTransaction(IsolationLevel isolevel)
		{
			if (this.State == ConnectionState.Closed)
			{
				throw ExceptionHelper.ConnectionClosed();
			}
			if (this.transaction == null)
			{
				this.transaction = new OdbcTransaction(this, isolevel);
				return this.transaction;
			}
			throw new InvalidOperationException();
		}

		public override void Close()
		{
			if (this.State == ConnectionState.Open)
			{
				if (this.linkedCommands != null)
				{
					for (int i = 0; i < this.linkedCommands.Count; i++)
					{
						WeakReference weakReference = (WeakReference)this.linkedCommands[i];
						if (weakReference != null)
						{
							OdbcCommand odbcCommand = (OdbcCommand)weakReference.Target;
							if (odbcCommand != null)
							{
								odbcCommand.Unlink();
							}
						}
					}
					this.linkedCommands = null;
				}
				OdbcReturn odbcReturn = libodbc.SQLDisconnect(this.hdbc);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.CreateOdbcException(OdbcHandleType.Dbc, this.hdbc);
				}
				this.FreeHandles();
				this.transaction = null;
				this.RaiseStateChange(ConnectionState.Open, ConnectionState.Closed);
			}
		}

		public new OdbcCommand CreateCommand()
		{
			return new OdbcCommand(string.Empty, this, this.transaction);
		}

		public override void ChangeDatabase(string value)
		{
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = Marshal.StringToHGlobalUni(value);
				OdbcReturn odbcReturn = libodbc.SQLSetConnectAttr(this.hdbc, OdbcConnectionAttribute.CurrentCatalog, intPtr, value.Length * 2);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.CreateOdbcException(OdbcHandleType.Dbc, this.hdbc);
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeCoTaskMem(intPtr);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				try
				{
					this.Close();
					this.disposed = true;
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		protected override DbCommand CreateDbCommand()
		{
			return this.CreateCommand();
		}

		public override void Open()
		{
			if (this.State == ConnectionState.Open)
			{
				throw new InvalidOperationException();
			}
			try
			{
				OdbcReturn odbcReturn = libodbc.SQLAllocHandle(OdbcHandleType.Env, IntPtr.Zero, ref this.henv);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					OdbcException ex = new OdbcException(new OdbcErrorCollection
					{
						new OdbcError(this)
					});
					this.MessageHandler(ex);
					throw ex;
				}
				odbcReturn = libodbc.SQLSetEnvAttr(this.henv, OdbcEnv.OdbcVersion, (IntPtr)3, 0);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.CreateOdbcException(OdbcHandleType.Env, this.henv);
				}
				odbcReturn = libodbc.SQLAllocHandle(OdbcHandleType.Dbc, this.henv, ref this.hdbc);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.CreateOdbcException(OdbcHandleType.Env, this.henv);
				}
				if (this.ConnectionString.ToLower().IndexOf("dsn=") >= 0)
				{
					string text = string.Empty;
					string text2 = string.Empty;
					string text3 = string.Empty;
					string[] array = this.ConnectionString.Split(new char[] { ';' });
					foreach (string text4 in array)
					{
						string[] array3 = text4.Split(new char[] { '=' });
						string text5 = array3[0].Trim().ToLower();
						switch (text5)
						{
						case "dsn":
							text3 = array3[1].Trim();
							break;
						case "uid":
							text = array3[1].Trim();
							break;
						case "pwd":
							text2 = array3[1].Trim();
							break;
						}
					}
					odbcReturn = libodbc.SQLConnect(this.hdbc, text3, -3, text, -3, text2, -3);
					if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
					{
						throw this.CreateOdbcException(OdbcHandleType.Dbc, this.hdbc);
					}
				}
				else
				{
					string text6 = new string(' ', 1024);
					short num2 = 0;
					odbcReturn = libodbc.SQLDriverConnect(this.hdbc, IntPtr.Zero, this.ConnectionString, -3, text6, (short)text6.Length, ref num2, 0);
					if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
					{
						throw this.CreateOdbcException(OdbcHandleType.Dbc, this.hdbc);
					}
				}
				this.RaiseStateChange(ConnectionState.Closed, ConnectionState.Open);
			}
			catch
			{
				this.FreeHandles();
				throw;
			}
			this.disposed = false;
		}

		[MonoTODO]
		public static void ReleaseObjectPool()
		{
			throw new NotImplementedException();
		}

		private void FreeHandles()
		{
			if (this.hdbc != IntPtr.Zero)
			{
				OdbcReturn odbcReturn = libodbc.SQLFreeHandle(2, this.hdbc);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.CreateOdbcException(OdbcHandleType.Dbc, this.hdbc);
				}
			}
			this.hdbc = IntPtr.Zero;
			if (this.henv != IntPtr.Zero)
			{
				OdbcReturn odbcReturn = libodbc.SQLFreeHandle(1, this.henv);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.CreateOdbcException(OdbcHandleType.Env, this.henv);
				}
			}
			this.henv = IntPtr.Zero;
		}

		public override DataTable GetSchema()
		{
			if (this.State == ConnectionState.Closed)
			{
				throw ExceptionHelper.ConnectionClosed();
			}
			return DbConnection.MetaDataCollections.Instance;
		}

		public override DataTable GetSchema(string collectionName)
		{
			return this.GetSchema(collectionName, null);
		}

		public override DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			if (this.State == ConnectionState.Closed)
			{
				throw ExceptionHelper.ConnectionClosed();
			}
			return this.GetSchema(collectionName, null);
		}

		[MonoTODO]
		public override void EnlistTransaction(Transaction transaction)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void EnlistDistributedTransaction(ITransaction transaction)
		{
			throw new NotImplementedException();
		}

		internal string GetInfo(OdbcInfo info)
		{
			if (this.State == ConnectionState.Closed)
			{
				throw new InvalidOperationException("The connection is closed.");
			}
			short num = 512;
			byte[] array = new byte[512];
			short num2 = 0;
			OdbcReturn odbcReturn = libodbc.SQLGetInfo(this.hdbc, info, array, num, ref num2);
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw this.CreateOdbcException(OdbcHandleType.Dbc, this.hdbc);
			}
			return Encoding.Unicode.GetString(array, 0, (int)num2);
		}

		private string GetSafeInfo(OdbcInfo info)
		{
			if (this.State == ConnectionState.Closed)
			{
				return null;
			}
			short num = 512;
			byte[] array = new byte[512];
			short num2 = 0;
			OdbcReturn odbcReturn = libodbc.SQLGetInfo(this.hdbc, info, array, num, ref num2);
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				return null;
			}
			return Encoding.Unicode.GetString(array, 0, (int)num2);
		}

		private void RaiseStateChange(ConnectionState from, ConnectionState to)
		{
			base.OnStateChange(new StateChangeEventArgs(from, to));
		}

		private OdbcInfoMessageEventArgs CreateOdbcInfoMessageEvent(OdbcErrorCollection errors)
		{
			return new OdbcInfoMessageEventArgs(errors);
		}

		private void OnOdbcInfoMessage(OdbcInfoMessageEventArgs e)
		{
			if (this.InfoMessage != null)
			{
				this.InfoMessage(this, e);
			}
		}

		internal OdbcException CreateOdbcException(OdbcHandleType HandleType, IntPtr Handle)
		{
			short num = 256;
			short num2 = 0;
			int num3 = 0;
			OdbcReturn odbcReturn = OdbcReturn.Success;
			OdbcErrorCollection odbcErrorCollection = new OdbcErrorCollection();
			for (;;)
			{
				byte[] array = new byte[(int)(num * 2)];
				byte[] array2 = new byte[(int)(num * 2)];
				switch (HandleType)
				{
				case OdbcHandleType.Env:
					odbcReturn = libodbc.SQLError(Handle, IntPtr.Zero, IntPtr.Zero, array2, ref num3, array, num, ref num2);
					break;
				case OdbcHandleType.Dbc:
					odbcReturn = libodbc.SQLError(IntPtr.Zero, Handle, IntPtr.Zero, array2, ref num3, array, num, ref num2);
					break;
				case OdbcHandleType.Stmt:
					odbcReturn = libodbc.SQLError(IntPtr.Zero, IntPtr.Zero, Handle, array2, ref num3, array, num, ref num2);
					break;
				}
				if (odbcReturn != OdbcReturn.Success)
				{
					break;
				}
				string text = OdbcConnection.RemoveTrailingNullChar(Encoding.Unicode.GetString(array2));
				string @string = Encoding.Unicode.GetString(array, 0, (int)(num2 * 2));
				odbcErrorCollection.Add(new OdbcError(@string, text, num3));
			}
			string safeDriver = this.SafeDriver;
			foreach (object obj in odbcErrorCollection)
			{
				OdbcError odbcError = (OdbcError)obj;
				odbcError.SetSource(safeDriver);
			}
			return new OdbcException(odbcErrorCollection);
		}

		private static string RemoveTrailingNullChar(string value)
		{
			return value.TrimEnd(new char[1]);
		}

		internal void Link(OdbcCommand cmd)
		{
			if (this.linkedCommands == null)
			{
				this.linkedCommands = new ArrayList();
			}
			this.linkedCommands.Add(new WeakReference(cmd));
		}

		internal void Unlink(OdbcCommand cmd)
		{
			if (this.linkedCommands == null)
			{
				return;
			}
			for (int i = 0; i < this.linkedCommands.Count; i++)
			{
				WeakReference weakReference = (WeakReference)this.linkedCommands[i];
				if (weakReference != null)
				{
					OdbcCommand odbcCommand = (OdbcCommand)weakReference.Target;
					if (odbcCommand == cmd)
					{
						this.linkedCommands[i] = null;
						break;
					}
				}
			}
		}

		private void MessageHandler(OdbcException e)
		{
			this.OnOdbcInfoMessage(this.CreateOdbcInfoMessageEvent(e.Errors));
		}

		private string connectionString;

		private int connectionTimeout;

		internal OdbcTransaction transaction;

		private IntPtr henv = IntPtr.Zero;

		private IntPtr hdbc = IntPtr.Zero;

		private bool disposed;

		private ArrayList linkedCommands;
	}
}
