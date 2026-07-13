using System;

namespace UnityEngine.AdaptivePerformance.Provider
{
	public struct PerformanceDataRecord
	{
		public Feature ChangeFlags { readonly get; set; }

		public float TemperatureLevel { readonly get; set; }

		public float TemperatureTrend { readonly get; set; }

		public WarningLevel WarningLevel { readonly get; set; }

		public int CpuPerformanceLevel { readonly get; set; }

		public int GpuPerformanceLevel { readonly get; set; }

		public bool PerformanceLevelControlAvailable { readonly get; set; }

		public float CpuFrameTime { readonly get; set; }

		public float GpuFrameTime { readonly get; set; }

		public float OverallFrameTime { readonly get; set; }

		public bool CpuPerformanceBoost { readonly get; set; }

		public bool GpuPerformanceBoost { readonly get; set; }

		public ClusterInfo ClusterInfo { readonly get; set; }

		public PerformanceMode PerformanceMode { readonly get; set; }
	}
}
