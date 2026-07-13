using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	internal struct UntypedUnsafeList
	{
		[NativeDisableUnsafePtrRestriction]
		public unsafe void* Ptr;

		public int m_length;

		public int m_capacity;

		public AllocatorManager.AllocatorHandle Allocator;

		internal int obsolete_length;

		internal int obsolete_capacity;
	}
}
