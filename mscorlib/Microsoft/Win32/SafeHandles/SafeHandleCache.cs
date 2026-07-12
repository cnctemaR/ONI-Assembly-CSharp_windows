using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.Win32.SafeHandles
{
	internal static class SafeHandleCache<T> where T : SafeHandle
	{
		internal static T GetInvalidHandle(Func<T> invalidHandleFactory)
		{
			T t = Volatile.Read<T>(ref SafeHandleCache<T>.s_invalidHandle);
			if (t == null)
			{
				T t2 = invalidHandleFactory();
				t = Interlocked.CompareExchange<T>(ref SafeHandleCache<T>.s_invalidHandle, t2, default(T));
				if (t == null)
				{
					GC.SuppressFinalize(t2);
					t = t2;
				}
				else
				{
					t2.Dispose();
				}
			}
			return t;
		}

		internal static bool IsCachedInvalidHandle(SafeHandle handle)
		{
			return handle == Volatile.Read<T>(ref SafeHandleCache<T>.s_invalidHandle);
		}

		private static T s_invalidHandle;
	}
}
