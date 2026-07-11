using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.SqlClient
{
	public sealed class SqlClientFactory : DbProviderFactory
	{
		internal SqlClientFactory()
		{
		}

		public override bool CanCreateDataSourceEnumerator
		{
			get
			{
				throw null;
			}
		}

		public override DbCommand CreateCommand()
		{
			throw null;
		}

		public override DbCommandBuilder CreateCommandBuilder()
		{
			throw null;
		}

		public override DbConnection CreateConnection()
		{
			throw null;
		}

		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			throw null;
		}

		public override DbDataAdapter CreateDataAdapter()
		{
			throw null;
		}

		public override DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			throw null;
		}

		public override DbParameter CreateParameter()
		{
			throw null;
		}

		public override CodeAccessPermission CreatePermission(PermissionState state)
		{
			throw null;
		}

		public static readonly SqlClientFactory Instance;
	}
}
