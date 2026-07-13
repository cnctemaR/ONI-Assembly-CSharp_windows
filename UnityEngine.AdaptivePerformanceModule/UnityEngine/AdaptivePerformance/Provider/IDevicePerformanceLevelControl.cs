using System;

namespace UnityEngine.AdaptivePerformance.Provider
{
	public interface IDevicePerformanceLevelControl
	{
		int MaxCpuPerformanceLevel { get; }

		int MaxGpuPerformanceLevel { get; }

		bool SetPerformanceLevel(ref int cpu, ref int gpu);

		bool EnableCpuBoost();

		bool EnableGpuBoost();
	}
}
