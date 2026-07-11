using System;

namespace System.Data.ProviderBase
{
	internal class DbConnectionPoolGroupProviderInfo
	{
		internal DbConnectionPoolGroup PoolGroup
		{
			get
			{
				return this._poolGroup;
			}
			set
			{
				this._poolGroup = value;
			}
		}

		private DbConnectionPoolGroup _poolGroup;
	}
}
