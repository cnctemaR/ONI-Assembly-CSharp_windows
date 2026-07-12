using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[NativeContainer]
	internal struct NativeArrayDispose
	{
		public void Dispose()
		{
			UnsafeUtility.FreeTracked(this.m_Buffer, this.m_AllocatorLabel);
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe void* m_Buffer;

		internal Allocator m_AllocatorLabel;
	}
}
