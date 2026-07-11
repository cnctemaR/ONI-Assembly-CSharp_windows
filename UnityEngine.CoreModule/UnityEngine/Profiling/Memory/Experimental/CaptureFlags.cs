using System;

namespace UnityEngine.Profiling.Memory.Experimental
{
	[Flags]
	public enum CaptureFlags : uint
	{
		ManagedObjects = 1U,
		NativeObjects = 2U,
		NativeAllocations = 4U,
		NativeAllocationSites = 8U,
		NativeStackTraces = 16U
	}
}
