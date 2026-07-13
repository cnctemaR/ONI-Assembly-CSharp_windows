using System;

namespace UnityEngine.AdaptivePerformance.Provider
{
	[Flags]
	public enum Feature
	{
		None = 0,
		WarningLevel = 1,
		TemperatureLevel = 2,
		TemperatureTrend = 4,
		CpuPerformanceLevel = 8,
		GpuPerformanceLevel = 16,
		PerformanceLevelControl = 32,
		GpuFrameTime = 64,
		CpuFrameTime = 128,
		OverallFrameTime = 256,
		CpuPerformanceBoost = 512,
		GpuPerformanceBoost = 1024,
		ClusterInfo = 2048,
		PerformanceMode = 4096
	}
}
