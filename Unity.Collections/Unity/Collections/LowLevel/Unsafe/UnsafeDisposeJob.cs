using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompile]
	internal struct UnsafeDisposeJob : IJob
	{
		public void Execute()
		{
			AllocatorManager.Free(this.Allocator, this.Ptr);
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe void* Ptr;

		public AllocatorManager.AllocatorHandle Allocator;
	}
}
