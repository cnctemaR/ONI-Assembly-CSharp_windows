using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.OleDb
{
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbFactory : DbProviderFactory
	{
		internal OleDbFactory()
		{
		}

		public override DbCommand CreateCommand()
		{
			throw ADP.OleDb();
		}

		public override DbCommandBuilder CreateCommandBuilder()
		{
			throw ADP.OleDb();
		}

		public override DbConnection CreateConnection()
		{
			throw ADP.OleDb();
		}

		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			throw ADP.OleDb();
		}

		public override DbDataAdapter CreateDataAdapter()
		{
			throw ADP.OleDb();
		}

		public override DbParameter CreateParameter()
		{
			throw ADP.OleDb();
		}

		public override CodeAccessPermission CreatePermission(PermissionState state)
		{
			throw ADP.OleDb();
		}

		public static readonly OleDbFactory Instance;
	}
}
