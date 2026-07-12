using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
	internal static class HttpUtilities
	{
		internal static Version DefaultRequestVersion
		{
			get
			{
				return HttpVersion.Version20;
			}
		}

		internal static Version DefaultResponseVersion
		{
			get
			{
				return HttpVersion.Version11;
			}
		}

		internal static bool IsHttpUri(Uri uri)
		{
			return HttpUtilities.IsSupportedScheme(uri.Scheme);
		}

		internal static bool IsSupportedScheme(string scheme)
		{
			return HttpUtilities.IsSupportedNonSecureScheme(scheme) || HttpUtilities.IsSupportedSecureScheme(scheme);
		}

		internal static bool IsSupportedNonSecureScheme(string scheme)
		{
			return string.Equals(scheme, "http", StringComparison.OrdinalIgnoreCase) || HttpUtilities.IsNonSecureWebSocketScheme(scheme);
		}

		internal static bool IsSupportedSecureScheme(string scheme)
		{
			return string.Equals(scheme, "https", StringComparison.OrdinalIgnoreCase) || HttpUtilities.IsSecureWebSocketScheme(scheme);
		}

		internal static bool IsNonSecureWebSocketScheme(string scheme)
		{
			return string.Equals(scheme, "ws", StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsSecureWebSocketScheme(string scheme)
		{
			return string.Equals(scheme, "wss", StringComparison.OrdinalIgnoreCase);
		}

		internal static Task ContinueWithStandard<T>(this Task<T> task, object state, Action<Task<T>, object> continuation)
		{
			return task.ContinueWith(continuation, state, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		}
	}
}
