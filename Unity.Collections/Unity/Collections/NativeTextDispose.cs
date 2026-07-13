using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[NativeContainer]
	[GenerateTestsForBurstCompatibility]
	internal struct NativeTextDispose
	{
		public void Dispose()
		{
			UnsafeText.Free(this.m_TextData);
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe UnsafeText* m_TextData;
	}
}
