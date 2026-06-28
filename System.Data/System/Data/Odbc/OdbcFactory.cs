using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Odbc
{
	public sealed class OdbcFactory : DbProviderFactory
	{
		private OdbcFactory()
		{
		}

		static OdbcFactory()
		{
			object obj = OdbcFactory.lockStatic;
			lock (obj)
			{
				if (OdbcFactory.Instance == null)
				{
					OdbcFactory.Instance = new OdbcFactory();
				}
			}
		}

		public override DbConnection CreateConnection()
		{
			return new OdbcConnection();
		}

		public override DbCommand CreateCommand()
		{
			return new OdbcCommand();
		}

		public override DbCommandBuilder CreateCommandBuilder()
		{
			return new OdbcCommandBuilder();
		}

		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return new OdbcConnectionStringBuilder();
		}

		public override DbDataAdapter CreateDataAdapter()
		{
			return new OdbcDataAdapter();
		}

		public override DbParameter CreateParameter()
		{
			return new OdbcParameter();
		}

		public override CodeAccessPermission CreatePermission(PermissionState state)
		{
			return new OdbcPermission(state);
		}

		public static readonly OdbcFactory Instance;

		private static readonly object lockStatic = new object();
	}
}
