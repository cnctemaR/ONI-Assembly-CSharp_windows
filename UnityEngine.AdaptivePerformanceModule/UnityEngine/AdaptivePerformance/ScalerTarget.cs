using System;

namespace UnityEngine.AdaptivePerformance
{
	[Flags]
	public enum ScalerTarget
	{
		CPU = 1,
		GPU = 2,
		FillRate = 4
	}
}
