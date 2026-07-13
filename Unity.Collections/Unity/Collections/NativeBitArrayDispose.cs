using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[NativeContainer]
	[GenerateTestsForBurstCompatibility]
	internal struct NativeBitArrayDispose
	{
		public void Dispose()
		{
			UnsafeBitArray.Free(this.m_BitArrayData, this.m_Allocator);
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe UnsafeBitArray* m_BitArrayData;

		public AllocatorManager.AllocatorHandle m_Allocator;
	}
}
