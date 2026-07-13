using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompile]
	internal struct UnsafeHashMapDisposeJob : IJob
	{
		public void Execute()
		{
			UnsafeHashMapData.DeallocateHashMap(this.Data, this.Allocator);
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe UnsafeHashMapData* Data;

		public AllocatorManager.AllocatorHandle Allocator;
	}
}
