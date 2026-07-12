using System;
using System.Threading;

namespace System.Net.Security
{
	internal static class SSPIHandleCache
	{
		internal static void CacheCredential(SafeFreeCredentials newHandle)
		{
			try
			{
				SafeCredentialReference safeCredentialReference = SafeCredentialReference.CreateReference(newHandle);
				if (safeCredentialReference != null)
				{
					int num = Interlocked.Increment(ref SSPIHandleCache.s_current) & 31;
					safeCredentialReference = Interlocked.Exchange<SafeCredentialReference>(ref SSPIHandleCache.s_cacheSlots[num], safeCredentialReference);
					if (safeCredentialReference != null)
					{
						safeCredentialReference.Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				if (!ExceptionCheck.IsFatal(ex))
				{
					NetEventSource.Fail(null, "Attempted to throw: {e}", "CacheCredential");
				}
			}
		}

		private const int c_MaxCacheSize = 31;

		private static SafeCredentialReference[] s_cacheSlots = new SafeCredentialReference[32];

		private static int s_current = -1;
	}
}
