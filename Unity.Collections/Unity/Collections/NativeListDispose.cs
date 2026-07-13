using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[NativeContainer]
	[GenerateTestsForBurstCompatibility]
	internal struct NativeListDispose
	{
		public unsafe void Dispose()
		{
			UnsafeList<int>* listData = (UnsafeList<int>*)this.m_ListData;
			UnsafeList<int>.Destroy(listData);
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe UntypedUnsafeList* m_ListData;
	}
}
