using System;
using System.Data.ProviderBase;

namespace System.Data.SqlClient
{
	internal sealed class SqlConnectionPoolGroupProviderInfo : DbConnectionPoolGroupProviderInfo
	{
		internal SqlConnectionPoolGroupProviderInfo(SqlConnectionString connectionOptions)
		{
			this._failoverPartner = connectionOptions.FailoverPartner;
			if (string.IsNullOrEmpty(this._failoverPartner))
			{
				this._failoverPartner = null;
			}
		}

		internal string FailoverPartner
		{
			get
			{
				return this._failoverPartner;
			}
		}

		internal bool UseFailoverPartner
		{
			get
			{
				return this._useFailoverPartner;
			}
		}

		internal void AliasCheck(string server)
		{
			if (this._alias != server)
			{
				lock (this)
				{
					if (this._alias == null)
					{
						this._alias = server;
					}
					else if (this._alias != server)
					{
						base.PoolGroup.Clear();
						this._alias = server;
					}
				}
			}
		}

		internal void FailoverCheck(SqlInternalConnection connection, bool actualUseFailoverPartner, SqlConnectionString userConnectionOptions, string actualFailoverPartner)
		{
			if (this.UseFailoverPartner != actualUseFailoverPartner)
			{
				base.PoolGroup.Clear();
				this._useFailoverPartner = actualUseFailoverPartner;
			}
			if (!this._useFailoverPartner && this._failoverPartner != actualFailoverPartner)
			{
				lock (this)
				{
					if (this._failoverPartner != actualFailoverPartner)
					{
						this._failoverPartner = actualFailoverPartner;
					}
				}
			}
		}

		private string _alias;

		private string _failoverPartner;

		private bool _useFailoverPartner;
	}
}
