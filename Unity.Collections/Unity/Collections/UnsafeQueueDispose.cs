using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	internal struct UnsafeQueueDispose
	{
		public void Dispose()
		{
			UnsafeQueueData.DeallocateQueue(this.m_Buffer, this.m_AllocatorLabel);
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeQueueData* m_Buffer;

		internal AllocatorManager.AllocatorHandle m_AllocatorLabel;
	}
}
