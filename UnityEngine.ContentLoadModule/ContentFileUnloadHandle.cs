using System;
using Unity.Jobs;

namespace Unity.Loading
{
	public struct ContentFileUnloadHandle
	{
		public bool IsCompleted
		{
			get
			{
				return this.jobHandle.IsCompleted;
			}
		}

		public bool WaitForCompletion(int timeoutMs)
		{
			return ContentLoadInterface.WaitForJobCompletion(this.jobHandle, timeoutMs);
		}

		internal JobHandle jobHandle;
	}
}
