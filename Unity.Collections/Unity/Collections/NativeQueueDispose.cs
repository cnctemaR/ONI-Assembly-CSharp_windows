using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[NativeContainer]
	[GenerateTestsForBurstCompatibility]
	internal struct NativeQueueDispose
	{
		public void Dispose()
		{
			UnsafeQueue<int>.Free(this.m_QueueData);
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe UnsafeQueue<int>* m_QueueData;
	}
}
