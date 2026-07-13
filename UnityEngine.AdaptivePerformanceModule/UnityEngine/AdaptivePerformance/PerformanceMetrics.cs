using System;

namespace UnityEngine.AdaptivePerformance
{
	public struct PerformanceMetrics
	{
		public int CurrentCpuLevel { readonly get; set; }

		public int CurrentGpuLevel { readonly get; set; }

		public PerformanceBottleneck PerformanceBottleneck { readonly get; set; }

		public bool CpuPerformanceBoost { readonly get; set; }

		public bool GpuPerformanceBoost { readonly get; set; }

		public ClusterInfo ClusterInfo { readonly get; set; }
	}
}
