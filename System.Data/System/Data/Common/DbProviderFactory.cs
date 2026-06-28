using System;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Common
{
	public abstract class DbProviderFactory
	{
		private NotImplementedException CreateNotImplementedException()
		{
			return new NotImplementedException();
		}

		public virtual bool CanCreateDataSourceEnumerator
		{
			get
			{
				throw this.CreateNotImplementedException();
			}
		}

		public virtual DbCommand CreateCommand()
		{
			throw this.CreateNotImplementedException();
		}

		public virtual DbCommandBuilder CreateCommandBuilder()
		{
			throw this.CreateNotImplementedException();
		}

		public virtual DbConnection CreateConnection()
		{
			throw this.CreateNotImplementedException();
		}

		public virtual DbDataAdapter CreateDataAdapter()
		{
			throw this.CreateNotImplementedException();
		}

		public virtual DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			throw this.CreateNotImplementedException();
		}

		public virtual DbParameter CreateParameter()
		{
			throw this.CreateNotImplementedException();
		}

		public virtual CodeAccessPermission CreatePermission(PermissionState state)
		{
			throw this.CreateNotImplementedException();
		}

		public virtual DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			throw this.CreateNotImplementedException();
		}
	}
}
