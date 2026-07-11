using System;
using System.ComponentModel;
using System.Data.Common;
using System.EnterpriseServices;

namespace System.Data.SqlClient
{
	[DefaultEvent("InfoMessage")]
	public sealed class SqlConnection : DbConnection, IDbConnection, IDisposable, ICloneable
	{
		public SqlConnection()
		{
		}

		public SqlConnection(string connectionString)
		{
		}

		public SqlConnection(string connectionString, SqlCredential cred)
		{
		}

		[DefaultValue("")]
		[Editor("Microsoft.VSDesigner.Data.SQL.Design.SqlConnectionStringEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[RecommendedAsConfigurable(true)]
		[RefreshProperties(RefreshProperties.All)]
		public override string ConnectionString
		{
			get
			{
				throw null;
			}
			[MonoTODO("persist security info, encrypt, enlist keyword not implemented")]
			set
			{
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ConnectionTimeout
		{
			get
			{
				throw null;
			}
		}

		public SqlCredential Credentials
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Database
		{
			get
			{
				throw null;
			}
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string DataSource
		{
			get
			{
				throw null;
			}
		}

		protected internal override DbProviderFactory DbProviderFactory
		{
			get
			{
				throw null;
			}
		}

		public bool FireInfoMessageEventOnUserErrors
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int PacketSize
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string ServerVersion
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ConnectionState State
		{
			get
			{
				throw null;
			}
		}

		[DefaultValue(false)]
		public bool StatisticsEnabled
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string WorkstationId
		{
			get
			{
				throw null;
			}
		}

		public event SqlInfoMessageEventHandler InfoMessage
		{
			add
			{
			}
			remove
			{
			}
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			throw null;
		}

		public new SqlTransaction BeginTransaction()
		{
			throw null;
		}

		public new SqlTransaction BeginTransaction(IsolationLevel iso)
		{
			throw null;
		}

		public SqlTransaction BeginTransaction(IsolationLevel iso, string transactionName)
		{
			throw null;
		}

		public SqlTransaction BeginTransaction(string transactionName)
		{
			throw null;
		}

		public override void ChangeDatabase(string database)
		{
		}

		public static void ChangePassword(string connectionString, string newPassword)
		{
		}

		public static void ClearAllPools()
		{
		}

		public static void ClearPool(SqlConnection connection)
		{
		}

		public override void Close()
		{
		}

		public new SqlCommand CreateCommand()
		{
			throw null;
		}

		protected override DbCommand CreateDbCommand()
		{
			throw null;
		}

		protected override void Dispose(bool disposing)
		{
		}

		[MonoTODO("Not sure what this means at present.")]
		public void EnlistDistributedTransaction(ITransaction transaction)
		{
		}

		public override DataTable GetSchema()
		{
			throw null;
		}

		public override DataTable GetSchema(string collectionName)
		{
			throw null;
		}

		public override DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			throw null;
		}

		public override void Open()
		{
		}

		object ICloneable.Clone()
		{
			throw null;
		}
	}
}
