using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[NativeContainer]
	[BurstCompatible]
	internal struct NativeQueueDispose
	{
		public void Dispose()
		{
			NativeQueueData.DeallocateQueue(this.m_Buffer, this.m_QueuePool, this.m_AllocatorLabel);
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe NativeQueueData* m_Buffer;

		[NativeDisableUnsafePtrRestriction]
		internal unsafe NativeQueueBlockPoolData* m_QueuePool;

		internal AllocatorManager.AllocatorHandle m_AllocatorLabel;
	}
}
