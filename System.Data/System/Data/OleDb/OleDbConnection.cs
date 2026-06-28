using System;
using System.ComponentModel;
using System.Data.Common;
using System.EnterpriseServices;
using System.Transactions;

namespace System.Data.OleDb
{
	[DefaultEvent("InfoMessage")]
	public sealed class OleDbConnection : DbConnection, ICloneable
	{
		public OleDbConnection()
		{
			this.gdaConnection = IntPtr.Zero;
			this.connectionTimeout = 15;
		}

		public OleDbConnection(string connectionString)
			: this()
		{
			this.connectionString = connectionString;
		}

		[DataCategory("DataCategory_InfoMessage")]
		public event OleDbInfoMessageEventHandler InfoMessage;

		[MonoTODO]
		object ICloneable.Clone()
		{
			throw new NotImplementedException();
		}

		[DataCategory("Data")]
		[DefaultValue("")]
		[RefreshProperties(RefreshProperties.All)]
		[RecommendedAsConfigurable(true)]
		[Editor("Microsoft.VSDesigner.Data.ADO.Design.OleDbConnectionStringEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ConnectionTimeout
		{
			get
			{
				return this.connectionTimeout;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Database
		{
			get
			{
				if (this.gdaConnection != IntPtr.Zero && libgda.gda_connection_is_open(this.gdaConnection))
				{
					return libgda.gda_connection_get_database(this.gdaConnection);
				}
				return string.Empty;
			}
		}

		[Browsable(true)]
		public override string DataSource
		{
			get
			{
				if (this.gdaConnection != IntPtr.Zero && libgda.gda_connection_is_open(this.gdaConnection))
				{
					return libgda.gda_connection_get_dsn(this.gdaConnection);
				}
				return string.Empty;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(true)]
		public string Provider
		{
			get
			{
				if (this.gdaConnection != IntPtr.Zero && libgda.gda_connection_is_open(this.gdaConnection))
				{
					return libgda.gda_connection_get_provider(this.gdaConnection);
				}
				return string.Empty;
			}
		}

		public override string ServerVersion
		{
			get
			{
				if (this.State == ConnectionState.Closed)
				{
					throw ExceptionHelper.ConnectionClosed();
				}
				return libgda.gda_connection_get_server_version(this.gdaConnection);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ConnectionState State
		{
			get
			{
				if (this.gdaConnection != IntPtr.Zero && libgda.gda_connection_is_open(this.gdaConnection))
				{
					return ConnectionState.Open;
				}
				return ConnectionState.Closed;
			}
		}

		internal IntPtr GdaConnection
		{
			get
			{
				return this.gdaConnection;
			}
		}

		public new OleDbTransaction BeginTransaction()
		{
			if (this.State == ConnectionState.Closed)
			{
				throw ExceptionHelper.ConnectionClosed();
			}
			return new OleDbTransaction(this);
		}

		public new OleDbTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			if (this.State == ConnectionState.Closed)
			{
				throw ExceptionHelper.ConnectionClosed();
			}
			return new OleDbTransaction(this, isolationLevel);
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			return this.BeginTransaction(isolationLevel);
		}

		protected override DbCommand CreateDbCommand()
		{
			return this.CreateCommand();
		}

		public override void ChangeDatabase(string value)
		{
			if (this.State != ConnectionState.Open)
			{
				throw new InvalidOperationException();
			}
			if (!libgda.gda_connection_change_database(this.gdaConnection, value))
			{
				throw new OleDbException(this);
			}
		}

		public override void Close()
		{
			if (this.State == ConnectionState.Open)
			{
				libgda.gda_connection_close(this.gdaConnection);
				this.gdaConnection = IntPtr.Zero;
			}
		}

		public new OleDbCommand CreateCommand()
		{
			if (this.State == ConnectionState.Open)
			{
				return new OleDbCommand(null, this);
			}
			return null;
		}

		[MonoTODO]
		protected override void Dispose(bool disposing)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public DataTable GetOleDbSchemaTable(Guid schema, object[] restrictions)
		{
			throw new NotImplementedException();
		}

		public override void Open()
		{
			if (this.State == ConnectionState.Open)
			{
				throw new InvalidOperationException();
			}
			libgda.gda_init("System.Data.OleDb", "1.0", 0, new string[0]);
			this.gdaConnection = libgda.gda_client_open_connection(libgda.GdaClient, this.ConnectionString, string.Empty, string.Empty, (GdaConnectionOptions)0);
			if (this.gdaConnection == IntPtr.Zero)
			{
				throw new OleDbException(this);
			}
		}

		[MonoTODO]
		public static void ReleaseObjectPool()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void EnlistDistributedTransaction(ITransaction transaction)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public override void EnlistTransaction(Transaction transaction)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public override DataTable GetSchema()
		{
			if (this.State == ConnectionState.Closed)
			{
				throw ExceptionHelper.ConnectionClosed();
			}
			throw new NotImplementedException();
		}

		[MonoTODO]
		public override DataTable GetSchema(string collectionName)
		{
			return this.GetSchema(collectionName, null);
		}

		[MonoTODO]
		public override DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			if (this.State == ConnectionState.Closed)
			{
				throw ExceptionHelper.ConnectionClosed();
			}
			throw new NotImplementedException();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[MonoTODO]
		public void ResetState()
		{
			throw new NotImplementedException();
		}

		private string connectionString;

		private int connectionTimeout;

		private IntPtr gdaConnection;
	}
}
