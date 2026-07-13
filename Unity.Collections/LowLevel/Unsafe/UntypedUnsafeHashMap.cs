using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	public struct UntypedUnsafeHashMap
	{
		[NativeDisableUnsafePtrRestriction]
		private unsafe UnsafeHashMapData* m_Buffer;

		private AllocatorManager.AllocatorHandle m_AllocatorLabel;
	}
}
