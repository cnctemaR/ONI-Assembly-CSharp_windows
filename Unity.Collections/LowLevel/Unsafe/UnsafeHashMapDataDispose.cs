using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[NativeContainer]
	[BurstCompatible]
	internal struct UnsafeHashMapDataDispose
	{
		public void Dispose()
		{
			UnsafeHashMapData.DeallocateHashMap(this.m_Buffer, this.m_AllocatorLabel);
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeHashMapData* m_Buffer;

		internal AllocatorManager.AllocatorHandle m_AllocatorLabel;
	}
}
