using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections
{
	[BurstCompile]
	internal struct NativeRingQueueDisposeJob : IJob
	{
		public void Execute()
		{
			this.Data.Dispose();
		}

		public NativeRingQueueDispose Data;
	}
}
