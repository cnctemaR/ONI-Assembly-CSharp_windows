using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompile]
	internal struct UnsafeParallelHashMapDataDisposeJob : IJob
	{
		public void Execute()
		{
			this.Data.Dispose();
		}

		internal UnsafeParallelHashMapDataDispose Data;
	}
}
