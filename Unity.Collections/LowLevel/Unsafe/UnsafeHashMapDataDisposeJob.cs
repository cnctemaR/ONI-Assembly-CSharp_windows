using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompile]
	internal struct UnsafeHashMapDataDisposeJob : IJob
	{
		public void Execute()
		{
			this.Data.Dispose();
		}

		internal UnsafeHashMapDataDispose Data;
	}
}
