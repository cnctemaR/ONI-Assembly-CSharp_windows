using System;
using System.Data.ProviderBase;

namespace System.Data.SqlClient
{
	internal sealed class SqlConnectionPoolProviderInfo : DbConnectionPoolProviderInfo
	{
		internal string InstanceName
		{
			get
			{
				return this._instanceName;
			}
			set
			{
				this._instanceName = value;
			}
		}

		private string _instanceName;
	}
}
