using System;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Common
{
	public abstract class DbProviderFactory
	{
		public virtual CodeAccessPermission CreatePermission(PermissionState state)
		{
			return null;
		}

		public virtual bool CanCreateDataSourceEnumerator
		{
			get
			{
				return false;
			}
		}

		public virtual bool CanCreateDataAdapter
		{
			get
			{
				if (this._canCreateDataAdapter == null)
				{
					using (DbDataAdapter dbDataAdapter = this.CreateDataAdapter())
					{
						this._canCreateDataAdapter = new bool?(dbDataAdapter != null);
					}
				}
				return this._canCreateDataAdapter.Value;
			}
		}

		public virtual bool CanCreateCommandBuilder
		{
			get
			{
				if (this._canCreateCommandBuilder == null)
				{
					using (DbCommandBuilder dbCommandBuilder = this.CreateCommandBuilder())
					{
						this._canCreateCommandBuilder = new bool?(dbCommandBuilder != null);
					}
				}
				return this._canCreateCommandBuilder.Value;
			}
		}

		public virtual DbCommand CreateCommand()
		{
			return null;
		}

		public virtual DbCommandBuilder CreateCommandBuilder()
		{
			return null;
		}

		public virtual DbConnection CreateConnection()
		{
			return null;
		}

		public virtual DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return null;
		}

		public virtual DbDataAdapter CreateDataAdapter()
		{
			return null;
		}

		public virtual DbParameter CreateParameter()
		{
			return null;
		}

		public virtual DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			return null;
		}

		private bool? _canCreateDataAdapter;

		private bool? _canCreateCommandBuilder;
	}
}
