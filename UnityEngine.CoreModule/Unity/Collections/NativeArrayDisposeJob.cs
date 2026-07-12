using System;
using Unity.Jobs;

namespace Unity.Collections
{
	internal struct NativeArrayDisposeJob : IJob
	{
		public void Execute()
		{
			this.Data.Dispose();
		}

		internal NativeArrayDispose Data;
	}
}
