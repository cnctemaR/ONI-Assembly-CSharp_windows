using System;

namespace System.Data.ProviderBase
{
	internal sealed class DbConnectionPoolGroupOptions
	{
		public DbConnectionPoolGroupOptions(bool poolByIdentity, int minPoolSize, int maxPoolSize, int creationTimeout, int loadBalanceTimeout, bool hasTransactionAffinity)
		{
			this._poolByIdentity = poolByIdentity;
			this._minPoolSize = minPoolSize;
			this._maxPoolSize = maxPoolSize;
			this._creationTimeout = creationTimeout;
			if (loadBalanceTimeout != 0)
			{
				this._loadBalanceTimeout = new TimeSpan(0, 0, loadBalanceTimeout);
				this._useLoadBalancing = true;
			}
			this._hasTransactionAffinity = hasTransactionAffinity;
		}

		public int CreationTimeout
		{
			get
			{
				return this._creationTimeout;
			}
		}

		public bool HasTransactionAffinity
		{
			get
			{
				return this._hasTransactionAffinity;
			}
		}

		public TimeSpan LoadBalanceTimeout
		{
			get
			{
				return this._loadBalanceTimeout;
			}
		}

		public int MaxPoolSize
		{
			get
			{
				return this._maxPoolSize;
			}
		}

		public int MinPoolSize
		{
			get
			{
				return this._minPoolSize;
			}
		}

		public bool PoolByIdentity
		{
			get
			{
				return this._poolByIdentity;
			}
		}

		public bool UseLoadBalancing
		{
			get
			{
				return this._useLoadBalancing;
			}
		}

		private readonly bool _poolByIdentity;

		private readonly int _minPoolSize;

		private readonly int _maxPoolSize;

		private readonly int _creationTimeout;

		private readonly TimeSpan _loadBalanceTimeout;

		private readonly bool _hasTransactionAffinity;

		private readonly bool _useLoadBalancing;
	}
}
