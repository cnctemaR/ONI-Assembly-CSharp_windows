using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections
{
	[BurstCompile]
	internal struct NativeQueueDisposeJob : IJob
	{
		public void Execute()
		{
			this.Data.Dispose();
		}

		public NativeQueueDispose Data;
	}
}
