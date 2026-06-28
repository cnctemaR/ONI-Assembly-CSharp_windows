using System;
using System.Data.Common;
using System.Data.Sql;
using System.Security;
using System.Security.Permissions;

namespace System.Data.SqlClient
{
	public sealed class SqlClientFactory : DbProviderFactory
	{
		private SqlClientFactory()
		{
		}

		public override bool CanCreateDataSourceEnumerator
		{
			get
			{
				return true;
			}
		}

		public override DbCommand CreateCommand()
		{
			return new SqlCommand();
		}

		public override DbCommandBuilder CreateCommandBuilder()
		{
			return new SqlCommandBuilder();
		}

		public override DbConnection CreateConnection()
		{
			return new SqlConnection();
		}

		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return new SqlConnectionStringBuilder();
		}

		public override DbDataAdapter CreateDataAdapter()
		{
			return new SqlDataAdapter();
		}

		public override DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			return SqlDataSourceEnumerator.Instance;
		}

		public override DbParameter CreateParameter()
		{
			return new SqlParameter();
		}

		public override CodeAccessPermission CreatePermission(PermissionState state)
		{
			return new SqlClientPermission(state);
		}

		public static readonly SqlClientFactory Instance = new SqlClientFactory();
	}
}
