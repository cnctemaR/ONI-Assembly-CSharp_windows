using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections
{
	[BurstCompile]
	internal struct NativeReferenceDisposeJob : IJob
	{
		public void Execute()
		{
			this.Data.Dispose();
		}

		internal NativeReferenceDispose Data;
	}
}
