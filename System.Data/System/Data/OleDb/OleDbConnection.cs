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
		}

		public OleDbConnection(string connectionString)
		{
		}

		[DefaultValue("")]
		[Editor("Microsoft.VSDesigner.Data.ADO.Design.OleDbConnectionStringEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ConnectionTimeout
		{
			get
			{
				throw null;
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
		public override string DataSource
		{
			get
			{
				throw null;
			}
		}

		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Provider
		{
			get
			{
				throw null;
			}
		}

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

		public event OleDbInfoMessageEventHandler InfoMessage
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

		public new OleDbTransaction BeginTransaction()
		{
			throw null;
		}

		public new OleDbTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			throw null;
		}

		public override void ChangeDatabase(string value)
		{
		}

		public override void Close()
		{
		}

		public new OleDbCommand CreateCommand()
		{
			throw null;
		}

		protected override DbCommand CreateDbCommand()
		{
			throw null;
		}

		[MonoTODO]
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

		[MonoTODO]
		public DataTable GetOleDbSchemaTable(Guid schema, object[] restrictions)
		{
			throw null;
		}

		[MonoTODO]
		public override DataTable GetSchema()
		{
			throw null;
		}

		[MonoTODO]
		public override DataTable GetSchema(string collectionName)
		{
			throw null;
		}

		[MonoTODO]
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

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[MonoTODO]
		public void ResetState()
		{
		}

		[MonoTODO]
		object ICloneable.Clone()
		{
			throw null;
		}
	}
}
