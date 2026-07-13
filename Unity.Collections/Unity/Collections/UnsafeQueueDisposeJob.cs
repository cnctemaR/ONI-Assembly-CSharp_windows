using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections
{
	[BurstCompile]
	internal struct UnsafeQueueDisposeJob : IJob
	{
		public void Execute()
		{
			this.Data.Dispose();
		}

		public UnsafeQueueDispose Data;
	}
}
