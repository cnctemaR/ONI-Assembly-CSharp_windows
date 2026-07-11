using System;

namespace System.Net.Cache
{
	public class HttpRequestCachePolicy : RequestCachePolicy
	{
		public HttpRequestCachePolicy()
		{
		}

		public HttpRequestCachePolicy(DateTime cacheSyncDate)
		{
			this.cacheSyncDate = cacheSyncDate;
		}

		public HttpRequestCachePolicy(HttpRequestCacheLevel level)
		{
			this.level = level;
		}

		public HttpRequestCachePolicy(HttpCacheAgeControl cacheAgeControl, TimeSpan ageOrFreshOrStale)
		{
			switch (cacheAgeControl)
			{
			case HttpCacheAgeControl.MinFresh:
				this.minFresh = ageOrFreshOrStale;
				return;
			case HttpCacheAgeControl.MaxAge:
				this.maxAge = ageOrFreshOrStale;
				return;
			case HttpCacheAgeControl.MaxStale:
				this.maxStale = ageOrFreshOrStale;
				return;
			}
			throw new ArgumentException("ageOrFreshOrStale");
		}

		public HttpRequestCachePolicy(HttpCacheAgeControl cacheAgeControl, TimeSpan maxAge, TimeSpan freshOrStale)
		{
			this.maxAge = maxAge;
			switch (cacheAgeControl)
			{
			case HttpCacheAgeControl.MinFresh:
				this.minFresh = freshOrStale;
				return;
			case HttpCacheAgeControl.MaxStale:
				this.maxStale = freshOrStale;
				return;
			}
			throw new ArgumentException("freshOrStale");
		}

		public HttpRequestCachePolicy(HttpCacheAgeControl cacheAgeControl, TimeSpan maxAge, TimeSpan freshOrStale, DateTime cacheSyncDate)
			: this(cacheAgeControl, maxAge, freshOrStale)
		{
			this.cacheSyncDate = cacheSyncDate;
		}

		public DateTime CacheSyncDate
		{
			get
			{
				return this.cacheSyncDate;
			}
		}

		public new HttpRequestCacheLevel Level
		{
			get
			{
				return this.level;
			}
		}

		public TimeSpan MaxAge
		{
			get
			{
				return this.maxAge;
			}
		}

		public TimeSpan MaxStale
		{
			get
			{
				return this.maxStale;
			}
		}

		public TimeSpan MinFresh
		{
			get
			{
				return this.minFresh;
			}
		}

		[global::System.MonoTODO]
		public override string ToString()
		{
			throw new NotImplementedException();
		}

		private DateTime cacheSyncDate;

		private HttpRequestCacheLevel level;

		private TimeSpan maxAge;

		private TimeSpan maxStale;

		private TimeSpan minFresh;
	}
}
