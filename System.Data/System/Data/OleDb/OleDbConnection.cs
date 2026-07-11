using System;
using System.Data.Common;
using System.EnterpriseServices;
using System.Transactions;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbConnection : DbConnection, IDbConnection, IDisposable, ICloneable
	{
		public OleDbConnection()
		{
			throw ADP.OleDb();
		}

		public OleDbConnection(string connectionString)
		{
			throw ADP.OleDb();
		}

		public override string ConnectionString
		{
			get
			{
				throw ADP.OleDb();
			}
			set
			{
			}
		}

		public override int ConnectionTimeout
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override string Database
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override string DataSource
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public string Provider
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override string ServerVersion
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		public override ConnectionState State
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			throw ADP.OleDb();
		}

		public new OleDbTransaction BeginTransaction()
		{
			throw ADP.OleDb();
		}

		public new OleDbTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			throw ADP.OleDb();
		}

		public override void ChangeDatabase(string value)
		{
			throw ADP.OleDb();
		}

		public override void Close()
		{
			throw ADP.OleDb();
		}

		public new OleDbCommand CreateCommand()
		{
			throw ADP.OleDb();
		}

		protected override DbCommand CreateDbCommand()
		{
			throw ADP.OleDb();
		}

		protected override void Dispose(bool disposing)
		{
			throw ADP.OleDb();
		}

		public void EnlistDistributedTransaction(ITransaction transaction)
		{
			throw ADP.OleDb();
		}

		public override void EnlistTransaction(Transaction transaction)
		{
			throw ADP.OleDb();
		}

		public DataTable GetOleDbSchemaTable(Guid schema, object[] restrictions)
		{
			throw ADP.OleDb();
		}

		public override DataTable GetSchema()
		{
			throw ADP.OleDb();
		}

		public override DataTable GetSchema(string collectionName)
		{
			throw ADP.OleDb();
		}

		public override DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			throw ADP.OleDb();
		}

		public override void Open()
		{
			throw ADP.OleDb();
		}

		public static void ReleaseObjectPool()
		{
			throw ADP.OleDb();
		}

		public void ResetState()
		{
			throw ADP.OleDb();
		}

		public event OleDbInfoMessageEventHandler InfoMessage;

		object ICloneable.Clone()
		{
			throw ADP.OleDb();
		}
	}
}
