using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections
{
	[BurstCompile]
	internal struct NativeHashMapDisposeJob : IJob
	{
		public void Execute()
		{
			this.Data.Dispose();
		}

		internal NativeHashMapDispose Data;
	}
}
