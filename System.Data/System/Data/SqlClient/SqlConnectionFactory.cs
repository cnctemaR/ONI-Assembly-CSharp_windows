using System;
using System.Data.Common;
using System.Data.ProviderBase;
using System.IO;
using System.Reflection;

namespace System.Data.SqlClient
{
	internal sealed class SqlConnectionFactory : DbConnectionFactory
	{
		private SqlConnectionFactory()
		{
		}

		public override DbProviderFactory ProviderFactory
		{
			get
			{
				return SqlClientFactory.Instance;
			}
		}

		protected override DbConnectionInternal CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningConnection)
		{
			return this.CreateConnection(options, poolKey, poolGroupProviderInfo, pool, owningConnection, null);
		}

		protected override DbConnectionInternal CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningConnection, DbConnectionOptions userOptions)
		{
			SqlConnectionString sqlConnectionString = (SqlConnectionString)options;
			SqlConnectionPoolKey sqlConnectionPoolKey = (SqlConnectionPoolKey)poolKey;
			SessionData sessionData = null;
			SqlConnection sqlConnection = (SqlConnection)owningConnection;
			bool flag = sqlConnection != null && sqlConnection._applyTransientFaultHandling;
			SqlConnectionString sqlConnectionString2 = null;
			if (userOptions != null)
			{
				sqlConnectionString2 = (SqlConnectionString)userOptions;
			}
			else if (sqlConnection != null)
			{
				sqlConnectionString2 = (SqlConnectionString)sqlConnection.UserConnectionOptions;
			}
			if (sqlConnection != null)
			{
				sessionData = sqlConnection._recoverySessionData;
			}
			bool flag2 = false;
			DbConnectionPoolIdentity dbConnectionPoolIdentity = null;
			if (sqlConnectionString.IntegratedSecurity)
			{
				if (pool != null)
				{
					dbConnectionPoolIdentity = pool.Identity;
				}
				else
				{
					dbConnectionPoolIdentity = DbConnectionPoolIdentity.GetCurrent();
				}
			}
			if (sqlConnectionString.UserInstance)
			{
				flag2 = true;
				string text;
				if (pool == null || (pool != null && pool.Count <= 0))
				{
					SqlInternalConnectionTds sqlInternalConnectionTds = null;
					try
					{
						SqlConnectionString sqlConnectionString3 = new SqlConnectionString(sqlConnectionString, sqlConnectionString.DataSource, true, new bool?(false));
						sqlInternalConnectionTds = new SqlInternalConnectionTds(dbConnectionPoolIdentity, sqlConnectionString3, null, false, null, null, flag);
						text = sqlInternalConnectionTds.InstanceName;
						if (!text.StartsWith("\\\\.\\", StringComparison.Ordinal))
						{
							throw SQL.NonLocalSSEInstance();
						}
						if (pool != null)
						{
							((SqlConnectionPoolProviderInfo)pool.ProviderInfo).InstanceName = text;
						}
						goto IL_0113;
					}
					finally
					{
						if (sqlInternalConnectionTds != null)
						{
							sqlInternalConnectionTds.Dispose();
						}
					}
				}
				text = ((SqlConnectionPoolProviderInfo)pool.ProviderInfo).InstanceName;
				IL_0113:
				sqlConnectionString = new SqlConnectionString(sqlConnectionString, text, false, null);
				poolGroupProviderInfo = null;
			}
			return new SqlInternalConnectionTds(dbConnectionPoolIdentity, sqlConnectionString, poolGroupProviderInfo, flag2, sqlConnectionString2, sessionData, flag);
		}

		protected override DbConnectionOptions CreateConnectionOptions(string connectionString, DbConnectionOptions previous)
		{
			return new SqlConnectionString(connectionString);
		}

		internal override DbConnectionPoolProviderInfo CreateConnectionPoolProviderInfo(DbConnectionOptions connectionOptions)
		{
			DbConnectionPoolProviderInfo dbConnectionPoolProviderInfo = null;
			if (((SqlConnectionString)connectionOptions).UserInstance)
			{
				dbConnectionPoolProviderInfo = new SqlConnectionPoolProviderInfo();
			}
			return dbConnectionPoolProviderInfo;
		}

		protected override DbConnectionPoolGroupOptions CreateConnectionPoolGroupOptions(DbConnectionOptions connectionOptions)
		{
			SqlConnectionString sqlConnectionString = (SqlConnectionString)connectionOptions;
			DbConnectionPoolGroupOptions dbConnectionPoolGroupOptions = null;
			if (sqlConnectionString.Pooling)
			{
				int num = sqlConnectionString.ConnectTimeout;
				if (0 < num && num < 2147483)
				{
					num *= 1000;
				}
				else if (num >= 2147483)
				{
					num = int.MaxValue;
				}
				dbConnectionPoolGroupOptions = new DbConnectionPoolGroupOptions(sqlConnectionString.IntegratedSecurity, sqlConnectionString.MinPoolSize, sqlConnectionString.MaxPoolSize, num, sqlConnectionString.LoadBalanceTimeout, sqlConnectionString.Enlist);
			}
			return dbConnectionPoolGroupOptions;
		}

		internal override DbConnectionPoolGroupProviderInfo CreateConnectionPoolGroupProviderInfo(DbConnectionOptions connectionOptions)
		{
			return new SqlConnectionPoolGroupProviderInfo((SqlConnectionString)connectionOptions);
		}

		internal static SqlConnectionString FindSqlConnectionOptions(SqlConnectionPoolKey key)
		{
			SqlConnectionString sqlConnectionString = (SqlConnectionString)SqlConnectionFactory.SingletonInstance.FindConnectionOptions(key);
			if (sqlConnectionString == null)
			{
				sqlConnectionString = new SqlConnectionString(key.ConnectionString);
			}
			if (sqlConnectionString.IsEmpty)
			{
				throw ADP.NoConnectionString();
			}
			return sqlConnectionString;
		}

		internal override DbConnectionPoolGroup GetConnectionPoolGroup(DbConnection connection)
		{
			SqlConnection sqlConnection = connection as SqlConnection;
			if (sqlConnection != null)
			{
				return sqlConnection.PoolGroup;
			}
			return null;
		}

		internal override DbConnectionInternal GetInnerConnection(DbConnection connection)
		{
			SqlConnection sqlConnection = connection as SqlConnection;
			if (sqlConnection != null)
			{
				return sqlConnection.InnerConnection;
			}
			return null;
		}

		internal override void PermissionDemand(DbConnection outerConnection)
		{
			SqlConnection sqlConnection = outerConnection as SqlConnection;
			if (sqlConnection != null)
			{
				sqlConnection.PermissionDemand();
			}
		}

		internal override void SetConnectionPoolGroup(DbConnection outerConnection, DbConnectionPoolGroup poolGroup)
		{
			SqlConnection sqlConnection = outerConnection as SqlConnection;
			if (sqlConnection != null)
			{
				sqlConnection.PoolGroup = poolGroup;
			}
		}

		internal override void SetInnerConnectionEvent(DbConnection owningObject, DbConnectionInternal to)
		{
			SqlConnection sqlConnection = owningObject as SqlConnection;
			if (sqlConnection != null)
			{
				sqlConnection.SetInnerConnectionEvent(to);
			}
		}

		internal override bool SetInnerConnectionFrom(DbConnection owningObject, DbConnectionInternal to, DbConnectionInternal from)
		{
			SqlConnection sqlConnection = owningObject as SqlConnection;
			return sqlConnection != null && sqlConnection.SetInnerConnectionFrom(to, from);
		}

		internal override void SetInnerConnectionTo(DbConnection owningObject, DbConnectionInternal to)
		{
			SqlConnection sqlConnection = owningObject as SqlConnection;
			if (sqlConnection != null)
			{
				sqlConnection.SetInnerConnectionTo(to);
			}
		}

		protected override DbMetaDataFactory CreateMetaDataFactory(DbConnectionInternal internalConnection, out bool cacheMetaDataFactory)
		{
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("System.Data.SqlClient.SqlMetaData.xml");
			cacheMetaDataFactory = true;
			return new SqlMetaDataFactory(manifestResourceStream, internalConnection.ServerVersion, internalConnection.ServerVersion);
		}

		private const string _metaDataXml = "MetaDataXml";

		public static readonly SqlConnectionFactory SingletonInstance = new SqlConnectionFactory();
	}
}
