using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections
{
	[BurstCompile]
	internal struct NativeTextDisposeJob : IJob
	{
		public void Execute()
		{
			this.Data.Dispose();
		}

		public NativeTextDispose Data;
	}
}
