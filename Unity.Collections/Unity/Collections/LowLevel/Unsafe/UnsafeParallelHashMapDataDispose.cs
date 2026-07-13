using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[NativeContainer]
	[GenerateTestsForBurstCompatibility]
	internal struct UnsafeParallelHashMapDataDispose
	{
		public void Dispose()
		{
			UnsafeParallelHashMapData.DeallocateHashMap(this.m_Buffer, this.m_AllocatorLabel);
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeParallelHashMapData* m_Buffer;

		internal AllocatorManager.AllocatorHandle m_AllocatorLabel;
	}
}
