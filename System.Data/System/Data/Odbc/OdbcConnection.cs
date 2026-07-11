using System;
using System.ComponentModel;
using System.Data.Common;
using System.EnterpriseServices;
using System.Transactions;

namespace System.Data.Odbc
{
	[DefaultEvent("InfoMessage")]
	public sealed class OdbcConnection : DbConnection, ICloneable
	{
		public OdbcConnection()
		{
		}

		public OdbcConnection(string connectionString)
		{
		}

		[DefaultValue("")]
		[Editor("Microsoft.VSDesigner.Data.Odbc.Design.OdbcConnectionStringEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[RecommendedAsConfigurable(true)]
		[RefreshProperties(RefreshProperties.All)]
		public override string ConnectionString
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(15)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int ConnectionTimeout
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string DataSource
		{
			get
			{
				throw null;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Driver
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

		public event OdbcInfoMessageEventHandler InfoMessage
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

		public new OdbcTransaction BeginTransaction()
		{
			throw null;
		}

		public new OdbcTransaction BeginTransaction(IsolationLevel isolevel)
		{
			throw null;
		}

		public override void ChangeDatabase(string value)
		{
		}

		public override void Close()
		{
		}

		public new OdbcCommand CreateCommand()
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

		[MonoTODO]
		public void EnlistDistributedTransaction(ITransaction transaction)
		{
		}

		[MonoTODO]
		public override void EnlistTransaction(Transaction transaction)
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

		[MonoTODO]
		public static void ReleaseObjectPool()
		{
		}

		[MonoTODO]
		object ICloneable.Clone()
		{
			throw null;
		}
	}
}
