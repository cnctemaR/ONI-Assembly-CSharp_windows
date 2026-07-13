using System;

namespace UnityEngine.AdaptivePerformance
{
	public struct PerformanceLevelChangeEventArgs
	{
		public int CpuLevel { readonly get; set; }

		public int CpuLevelDelta { readonly get; set; }

		public int GpuLevel { readonly get; set; }

		public int GpuLevelDelta { readonly get; set; }

		public PerformanceControlMode PerformanceControlMode { readonly get; set; }

		public bool ManualOverride { readonly get; set; }
	}
}
