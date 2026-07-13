using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[NativeContainer]
	[GenerateTestsForBurstCompatibility]
	internal struct NativeStreamDispose
	{
		public void Dispose()
		{
			this.m_StreamData.Dispose();
		}

		public UnsafeStream m_StreamData;
	}
}
