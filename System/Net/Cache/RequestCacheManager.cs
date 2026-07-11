using System;

namespace System.Net.Cache
{
	internal sealed class RequestCacheManager
	{
		private RequestCacheManager()
		{
		}

		internal static RequestCacheBinding GetBinding(string internedScheme)
		{
			if (internedScheme == null)
			{
				throw new ArgumentNullException("uriScheme");
			}
			if (RequestCacheManager.s_CacheConfigSettings == null)
			{
				RequestCacheManager.LoadConfigSettings();
			}
			if (RequestCacheManager.s_CacheConfigSettings.DisableAllCaching)
			{
				return RequestCacheManager.s_BypassCacheBinding;
			}
			if (internedScheme.Length == 0)
			{
				return RequestCacheManager.s_DefaultGlobalBinding;
			}
			if (internedScheme == Uri.UriSchemeHttp || internedScheme == Uri.UriSchemeHttps)
			{
				return RequestCacheManager.s_DefaultHttpBinding;
			}
			if (internedScheme == Uri.UriSchemeFtp)
			{
				return RequestCacheManager.s_DefaultFtpBinding;
			}
			return RequestCacheManager.s_BypassCacheBinding;
		}

		internal static bool IsCachingEnabled
		{
			get
			{
				if (RequestCacheManager.s_CacheConfigSettings == null)
				{
					RequestCacheManager.LoadConfigSettings();
				}
				return !RequestCacheManager.s_CacheConfigSettings.DisableAllCaching;
			}
		}

		internal static void SetBinding(string uriScheme, RequestCacheBinding binding)
		{
			if (uriScheme == null)
			{
				throw new ArgumentNullException("uriScheme");
			}
			if (RequestCacheManager.s_CacheConfigSettings == null)
			{
				RequestCacheManager.LoadConfigSettings();
			}
			if (RequestCacheManager.s_CacheConfigSettings.DisableAllCaching)
			{
				return;
			}
			if (uriScheme.Length == 0)
			{
				RequestCacheManager.s_DefaultGlobalBinding = binding;
				return;
			}
			if (uriScheme == Uri.UriSchemeHttp || uriScheme == Uri.UriSchemeHttps)
			{
				RequestCacheManager.s_DefaultHttpBinding = binding;
				return;
			}
			if (uriScheme == Uri.UriSchemeFtp)
			{
				RequestCacheManager.s_DefaultFtpBinding = binding;
			}
		}

		private static void LoadConfigSettings()
		{
			RequestCacheBinding requestCacheBinding = RequestCacheManager.s_BypassCacheBinding;
			lock (requestCacheBinding)
			{
				if (RequestCacheManager.s_CacheConfigSettings == null)
				{
					RequestCacheManager.s_CacheConfigSettings = new RequestCachingSectionInternal();
				}
			}
		}

		private static volatile RequestCachingSectionInternal s_CacheConfigSettings;

		private static readonly RequestCacheBinding s_BypassCacheBinding = new RequestCacheBinding(null, null, new RequestCachePolicy(RequestCacheLevel.BypassCache));

		private static volatile RequestCacheBinding s_DefaultGlobalBinding;

		private static volatile RequestCacheBinding s_DefaultHttpBinding;

		private static volatile RequestCacheBinding s_DefaultFtpBinding;
	}
}
