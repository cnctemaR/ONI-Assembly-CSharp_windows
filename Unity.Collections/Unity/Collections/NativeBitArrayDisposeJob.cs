using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections
{
	[BurstCompile]
	internal struct NativeBitArrayDisposeJob : IJob
	{
		public void Execute()
		{
			this.Data.Dispose();
		}

		public NativeBitArrayDispose Data;
	}
}
