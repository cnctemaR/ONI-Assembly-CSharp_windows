using System;
using System.Runtime.CompilerServices;

namespace System.Threading
{
	[ReflectionBlocked]
	public struct LockHolder : IDisposable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static LockHolder Hold(Lock l)
		{
			l.Acquire();
			LockHolder lockHolder;
			lockHolder._lock = l;
			return lockHolder;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			this._lock.Release();
		}

		private Lock _lock;
	}
}
