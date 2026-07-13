using System;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.AdaptivePerformance.Provider
{
	public abstract class AdaptivePerformanceSubsystemBase<TSubsystem, TSubsystemDescriptor, TProvider> : SubsystemWithProvider<TSubsystem, TSubsystemDescriptor, TProvider> where TSubsystem : SubsystemWithProvider, new() where TSubsystemDescriptor : SubsystemDescriptorWithProvider where TProvider : SubsystemProvider<TSubsystem>
	{
		public abstract Feature Capabilities { get; protected set; }

		public abstract PerformanceDataRecord Update();

		public abstract IApplicationLifecycle ApplicationLifecycle { get; }

		public abstract IDevicePerformanceLevelControl PerformanceLevelControl { get; }

		public abstract Version Version { get; }

		public abstract string Stats { get; }

		public abstract bool Initialized { get; protected set; }
	}
}
