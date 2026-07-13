using System;
using System.Threading;

namespace Unity.Collections
{
	internal struct Spinner
	{
		public void Lock()
		{
			while (Interlocked.CompareExchange(ref this.m_value, 1, 0) != 0)
			{
			}
			Interlocked.MemoryBarrier();
		}

		public void Unlock()
		{
			Interlocked.MemoryBarrier();
			while (1 != Interlocked.CompareExchange(ref this.m_value, 0, 1))
			{
			}
		}

		private int m_value;
	}
}
