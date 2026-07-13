using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[NativeContainer]
	[GenerateTestsForBurstCompatibility]
	internal struct NativeRingQueueDispose
	{
		public void Dispose()
		{
			UnsafeRingQueue<int>.Free(this.m_QueueData);
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe UnsafeRingQueue<int>* m_QueueData;
	}
}
