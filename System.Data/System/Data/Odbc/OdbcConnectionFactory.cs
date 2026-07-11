using System;
using System.Data.Common;
using System.Data.ProviderBase;

namespace System.Data.Odbc
{
	internal sealed class OdbcConnectionFactory : DbConnectionFactory
	{
		private OdbcConnectionFactory()
		{
		}

		public override DbProviderFactory ProviderFactory
		{
			get
			{
				return OdbcFactory.Instance;
			}
		}

		protected override DbConnectionInternal CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningObject)
		{
			return new OdbcConnectionOpen(owningObject as OdbcConnection, options as OdbcConnectionString);
		}

		protected override DbConnectionOptions CreateConnectionOptions(string connectionString, DbConnectionOptions previous)
		{
			return new OdbcConnectionString(connectionString, previous != null);
		}

		protected override DbConnectionPoolGroupOptions CreateConnectionPoolGroupOptions(DbConnectionOptions connectionOptions)
		{
			return null;
		}

		internal override DbConnectionPoolGroupProviderInfo CreateConnectionPoolGroupProviderInfo(DbConnectionOptions connectionOptions)
		{
			return new OdbcConnectionPoolGroupProviderInfo();
		}

		internal override DbConnectionPoolGroup GetConnectionPoolGroup(DbConnection connection)
		{
			OdbcConnection odbcConnection = connection as OdbcConnection;
			if (odbcConnection != null)
			{
				return odbcConnection.PoolGroup;
			}
			return null;
		}

		internal override DbConnectionInternal GetInnerConnection(DbConnection connection)
		{
			OdbcConnection odbcConnection = connection as OdbcConnection;
			if (odbcConnection != null)
			{
				return odbcConnection.InnerConnection;
			}
			return null;
		}

		internal override void PermissionDemand(DbConnection outerConnection)
		{
			OdbcConnection odbcConnection = outerConnection as OdbcConnection;
			if (odbcConnection != null)
			{
				odbcConnection.PermissionDemand();
			}
		}

		internal override void SetConnectionPoolGroup(DbConnection outerConnection, DbConnectionPoolGroup poolGroup)
		{
			OdbcConnection odbcConnection = outerConnection as OdbcConnection;
			if (odbcConnection != null)
			{
				odbcConnection.PoolGroup = poolGroup;
			}
		}

		internal override void SetInnerConnectionEvent(DbConnection owningObject, DbConnectionInternal to)
		{
			OdbcConnection odbcConnection = owningObject as OdbcConnection;
			if (odbcConnection != null)
			{
				odbcConnection.SetInnerConnectionEvent(to);
			}
		}

		internal override bool SetInnerConnectionFrom(DbConnection owningObject, DbConnectionInternal to, DbConnectionInternal from)
		{
			OdbcConnection odbcConnection = owningObject as OdbcConnection;
			return odbcConnection != null && odbcConnection.SetInnerConnectionFrom(to, from);
		}

		internal override void SetInnerConnectionTo(DbConnection owningObject, DbConnectionInternal to)
		{
			OdbcConnection odbcConnection = owningObject as OdbcConnection;
			if (odbcConnection != null)
			{
				odbcConnection.SetInnerConnectionTo(to);
			}
		}

		public static readonly OdbcConnectionFactory SingletonInstance = new OdbcConnectionFactory();
	}
}
