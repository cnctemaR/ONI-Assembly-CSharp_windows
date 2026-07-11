using System;
using System.Data.Common;
using System.Data.ProviderBase;
using System.Transactions;

namespace System.Data.Odbc
{
	internal sealed class OdbcConnectionOpen : DbConnectionInternal
	{
		internal OdbcConnectionOpen(OdbcConnection outerConnection, OdbcConnectionString connectionOptions)
		{
			OdbcEnvironmentHandle globalEnvironmentHandle = OdbcEnvironment.GetGlobalEnvironmentHandle();
			outerConnection.ConnectionHandle = new OdbcConnectionHandle(outerConnection, connectionOptions, globalEnvironmentHandle);
		}

		internal OdbcConnection OuterConnection
		{
			get
			{
				OdbcConnection odbcConnection = (OdbcConnection)base.Owner;
				if (odbcConnection == null)
				{
					throw ODBC.OpenConnectionNoOwner();
				}
				return odbcConnection;
			}
		}

		public override string ServerVersion
		{
			get
			{
				return this.OuterConnection.Open_GetServerVersion();
			}
		}

		protected override void Activate(Transaction transaction)
		{
		}

		public override DbTransaction BeginTransaction(IsolationLevel isolevel)
		{
			return this.BeginOdbcTransaction(isolevel);
		}

		internal OdbcTransaction BeginOdbcTransaction(IsolationLevel isolevel)
		{
			return this.OuterConnection.Open_BeginTransaction(isolevel);
		}

		public override void ChangeDatabase(string value)
		{
			this.OuterConnection.Open_ChangeDatabase(value);
		}

		protected override DbReferenceCollection CreateReferenceCollection()
		{
			return new OdbcReferenceCollection();
		}

		protected override void Deactivate()
		{
			base.NotifyWeakReference(0);
		}

		public override void EnlistTransaction(Transaction transaction)
		{
		}
	}
}
