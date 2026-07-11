using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.OleDb
{
	public sealed class OleDbFactory : DbProviderFactory
	{
		internal OleDbFactory()
		{
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

		public override DbParameter CreateParameter()
		{
			throw null;
		}

		public override CodeAccessPermission CreatePermission(PermissionState state)
		{
			throw null;
		}

		public static readonly OleDbFactory Instance;
	}
}
