using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	internal struct Spinner
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Acquire()
		{
			while (Interlocked.CompareExchange(ref this.m_Lock, 1, 0) != 0)
			{
				while (Volatile.Read(ref this.m_Lock) == 1)
				{
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool TryAcquire()
		{
			return Volatile.Read(ref this.m_Lock) == 0 && Interlocked.CompareExchange(ref this.m_Lock, 1, 0) == 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool TryAcquire(bool spin)
		{
			if (spin)
			{
				this.Acquire();
				return true;
			}
			return this.TryAcquire();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Release()
		{
			Volatile.Write(ref this.m_Lock, 0);
		}

		private int m_Lock;
	}
}
