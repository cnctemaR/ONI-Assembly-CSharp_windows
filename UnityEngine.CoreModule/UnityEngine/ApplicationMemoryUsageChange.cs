using System;

namespace UnityEngine
{
	public struct ApplicationMemoryUsageChange
	{
		public ApplicationMemoryUsage memoryUsage { readonly get; private set; }

		public ApplicationMemoryUsageChange(ApplicationMemoryUsage usage)
		{
			this.memoryUsage = usage;
		}
	}
}
