using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;

namespace System.Data.ProviderBase
{
	internal sealed class DbConnectionPoolGroup
	{
		internal DbConnectionPoolGroup(DbConnectionOptions connectionOptions, DbConnectionPoolKey key, DbConnectionPoolGroupOptions poolGroupOptions)
		{
			this._connectionOptions = connectionOptions;
			this._poolKey = key;
			this._poolGroupOptions = poolGroupOptions;
			this._poolCollection = new ConcurrentDictionary<DbConnectionPoolIdentity, DbConnectionPool>();
			this._state = 1;
		}

		internal DbConnectionOptions ConnectionOptions
		{
			get
			{
				return this._connectionOptions;
			}
		}

		internal DbConnectionPoolKey PoolKey
		{
			get
			{
				return this._poolKey;
			}
		}

		internal DbConnectionPoolGroupProviderInfo ProviderInfo
		{
			get
			{
				return this._providerInfo;
			}
			set
			{
				this._providerInfo = value;
				if (value != null)
				{
					this._providerInfo.PoolGroup = this;
				}
			}
		}

		internal bool IsDisabled
		{
			get
			{
				return 4 == this._state;
			}
		}

		internal DbConnectionPoolGroupOptions PoolGroupOptions
		{
			get
			{
				return this._poolGroupOptions;
			}
		}

		internal DbMetaDataFactory MetaDataFactory
		{
			get
			{
				return this._metaDataFactory;
			}
			set
			{
				this._metaDataFactory = value;
			}
		}

		internal int Clear()
		{
			ConcurrentDictionary<DbConnectionPoolIdentity, DbConnectionPool> concurrentDictionary = null;
			lock (this)
			{
				if (this._poolCollection.Count > 0)
				{
					concurrentDictionary = this._poolCollection;
					this._poolCollection = new ConcurrentDictionary<DbConnectionPoolIdentity, DbConnectionPool>();
				}
			}
			if (concurrentDictionary != null)
			{
				foreach (KeyValuePair<DbConnectionPoolIdentity, DbConnectionPool> keyValuePair in concurrentDictionary)
				{
					DbConnectionPool value = keyValuePair.Value;
					if (value != null)
					{
						value.ConnectionFactory.QueuePoolForRelease(value, true);
					}
				}
			}
			return this._poolCollection.Count;
		}

		internal DbConnectionPool GetConnectionPool(DbConnectionFactory connectionFactory)
		{
			DbConnectionPool dbConnectionPool = null;
			if (this._poolGroupOptions != null)
			{
				DbConnectionPoolIdentity dbConnectionPoolIdentity = DbConnectionPoolIdentity.NoIdentity;
				if (this._poolGroupOptions.PoolByIdentity)
				{
					dbConnectionPoolIdentity = DbConnectionPoolIdentity.GetCurrent();
					if (dbConnectionPoolIdentity.IsRestricted)
					{
						dbConnectionPoolIdentity = null;
					}
				}
				if (dbConnectionPoolIdentity != null && !this._poolCollection.TryGetValue(dbConnectionPoolIdentity, out dbConnectionPool))
				{
					DbConnectionPoolGroup dbConnectionPoolGroup = this;
					lock (dbConnectionPoolGroup)
					{
						if (!this._poolCollection.TryGetValue(dbConnectionPoolIdentity, out dbConnectionPool))
						{
							DbConnectionPoolProviderInfo dbConnectionPoolProviderInfo = connectionFactory.CreateConnectionPoolProviderInfo(this.ConnectionOptions);
							DbConnectionPool dbConnectionPool2 = new DbConnectionPool(connectionFactory, this, dbConnectionPoolIdentity, dbConnectionPoolProviderInfo);
							if (this.MarkPoolGroupAsActive())
							{
								dbConnectionPool2.Startup();
								this._poolCollection.TryAdd(dbConnectionPoolIdentity, dbConnectionPool2);
								dbConnectionPool = dbConnectionPool2;
							}
							else
							{
								dbConnectionPool2.Shutdown();
							}
						}
					}
				}
			}
			if (dbConnectionPool == null)
			{
				DbConnectionPoolGroup dbConnectionPoolGroup = this;
				lock (dbConnectionPoolGroup)
				{
					this.MarkPoolGroupAsActive();
				}
			}
			return dbConnectionPool;
		}

		private bool MarkPoolGroupAsActive()
		{
			if (2 == this._state)
			{
				this._state = 1;
			}
			return 1 == this._state;
		}

		internal bool Prune()
		{
			bool flag2;
			lock (this)
			{
				if (this._poolCollection.Count > 0)
				{
					ConcurrentDictionary<DbConnectionPoolIdentity, DbConnectionPool> concurrentDictionary = new ConcurrentDictionary<DbConnectionPoolIdentity, DbConnectionPool>();
					foreach (KeyValuePair<DbConnectionPoolIdentity, DbConnectionPool> keyValuePair in this._poolCollection)
					{
						DbConnectionPool value = keyValuePair.Value;
						if (value != null)
						{
							if (!value.ErrorOccurred && value.Count == 0)
							{
								value.ConnectionFactory.QueuePoolForRelease(value, false);
							}
							else
							{
								concurrentDictionary.TryAdd(keyValuePair.Key, keyValuePair.Value);
							}
						}
					}
					this._poolCollection = concurrentDictionary;
				}
				if (this._poolCollection.Count == 0)
				{
					if (1 == this._state)
					{
						this._state = 2;
					}
					else if (2 == this._state)
					{
						this._state = 4;
					}
				}
				flag2 = 4 == this._state;
			}
			return flag2;
		}

		private readonly DbConnectionOptions _connectionOptions;

		private readonly DbConnectionPoolKey _poolKey;

		private readonly DbConnectionPoolGroupOptions _poolGroupOptions;

		private ConcurrentDictionary<DbConnectionPoolIdentity, DbConnectionPool> _poolCollection;

		private int _state;

		private DbConnectionPoolGroupProviderInfo _providerInfo;

		private DbMetaDataFactory _metaDataFactory;

		private const int PoolGroupStateActive = 1;

		private const int PoolGroupStateIdle = 2;

		private const int PoolGroupStateDisabled = 4;
	}
}
