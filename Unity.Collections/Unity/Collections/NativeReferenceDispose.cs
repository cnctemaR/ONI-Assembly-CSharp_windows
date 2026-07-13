using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[NativeContainer]
	internal struct NativeReferenceDispose
	{
		public void Dispose()
		{
			Memory.Unmanaged.Free(this.m_Data, this.m_AllocatorLabel);
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe void* m_Data;

		internal AllocatorManager.AllocatorHandle m_AllocatorLabel;
	}
}
