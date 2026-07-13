using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompile]
	internal struct UnsafeParallelHashMapDisposeJob : IJob
	{
		public void Execute()
		{
			UnsafeParallelHashMapData.DeallocateHashMap(this.Data, this.Allocator);
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe UnsafeParallelHashMapData* Data;

		public AllocatorManager.AllocatorHandle Allocator;
	}
}
