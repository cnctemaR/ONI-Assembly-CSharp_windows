using System;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Common
{
	public abstract class DbProviderFactory
	{
		public virtual bool CanCreateDataSourceEnumerator
		{
			get
			{
				throw null;
			}
		}

		public virtual DbCommand CreateCommand()
		{
			throw null;
		}

		public virtual DbCommandBuilder CreateCommandBuilder()
		{
			throw null;
		}

		public virtual DbConnection CreateConnection()
		{
			throw null;
		}

		public virtual DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			throw null;
		}

		public virtual DbDataAdapter CreateDataAdapter()
		{
			throw null;
		}

		public virtual DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			throw null;
		}

		public virtual DbParameter CreateParameter()
		{
			throw null;
		}

		public virtual CodeAccessPermission CreatePermission(PermissionState state)
		{
			throw null;
		}
	}
}
