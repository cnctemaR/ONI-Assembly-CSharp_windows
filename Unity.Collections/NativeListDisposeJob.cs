using System;
using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections
{
	[BurstCompile]
	[BurstCompatible]
	internal struct NativeListDisposeJob : IJob
	{
		public void Execute()
		{
			this.Data.Dispose();
		}

		internal NativeListDispose Data;
	}
}
