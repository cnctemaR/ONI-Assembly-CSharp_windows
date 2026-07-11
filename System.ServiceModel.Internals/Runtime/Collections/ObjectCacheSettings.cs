using System;

namespace System.Runtime.Collections
{
	internal class ObjectCacheSettings
	{
		public ObjectCacheSettings()
		{
			this.CacheLimit = 64;
			this.IdleTimeout = ObjectCacheSettings.DefaultIdleTimeout;
			this.LeaseTimeout = ObjectCacheSettings.DefaultLeaseTimeout;
			this.PurgeFrequency = 32;
		}

		private ObjectCacheSettings(ObjectCacheSettings other)
		{
			this.CacheLimit = other.CacheLimit;
			this.IdleTimeout = other.IdleTimeout;
			this.LeaseTimeout = other.LeaseTimeout;
			this.PurgeFrequency = other.PurgeFrequency;
		}

		internal ObjectCacheSettings Clone()
		{
			return new ObjectCacheSettings(this);
		}

		public int CacheLimit
		{
			get
			{
				return this.cacheLimit;
			}
			set
			{
				this.cacheLimit = value;
			}
		}

		public TimeSpan IdleTimeout
		{
			get
			{
				return this.idleTimeout;
			}
			set
			{
				this.idleTimeout = value;
			}
		}

		public TimeSpan LeaseTimeout
		{
			get
			{
				return this.leaseTimeout;
			}
			set
			{
				this.leaseTimeout = value;
			}
		}

		public int PurgeFrequency
		{
			get
			{
				return this.purgeFrequency;
			}
			set
			{
				this.purgeFrequency = value;
			}
		}

		private int cacheLimit;

		private TimeSpan idleTimeout;

		private TimeSpan leaseTimeout;

		private int purgeFrequency;

		private const int DefaultCacheLimit = 64;

		private const int DefaultPurgeFrequency = 32;

		private static TimeSpan DefaultIdleTimeout = TimeSpan.FromMinutes(2.0);

		private static TimeSpan DefaultLeaseTimeout = TimeSpan.FromMinutes(5.0);
	}
}
