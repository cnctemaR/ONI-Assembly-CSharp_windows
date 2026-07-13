using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections
{
	[BurstCompile]
	internal struct NativeStreamDisposeJob : IJob
	{
		public void Execute()
		{
			this.Data.Dispose();
		}

		public NativeStreamDispose Data;
	}
}
