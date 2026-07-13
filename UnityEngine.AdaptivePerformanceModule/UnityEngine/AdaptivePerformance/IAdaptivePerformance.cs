using System;
using UnityEngine.AdaptivePerformance.Provider;

namespace UnityEngine.AdaptivePerformance
{
	public interface IAdaptivePerformance
	{
		bool Initialized { get; }

		bool Active { get; }

		IThermalStatus ThermalStatus { get; }

		IPerformanceStatus PerformanceStatus { get; }

		IDevicePerformanceControl DevicePerformanceControl { get; }

		IPerformanceModeStatus PerformanceModeStatus { get; }

		IDevelopmentSettings DevelopmentSettings { get; }

		AdaptivePerformanceIndexer Indexer { get; }

		IAdaptivePerformanceSettings Settings { get; }

		AdaptivePerformanceSubsystem Subsystem { get; }

		bool SupportedFeature(Feature feature);

		void InitializeAdaptivePerformance();

		void StartAdaptivePerformance();

		void StopAdaptivePerformance();

		void DeinitializeAdaptivePerformance();
	}
}
