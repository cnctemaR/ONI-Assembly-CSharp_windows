using System;
using System.Collections.Generic;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.AdaptivePerformance.Provider
{
	public sealed class AdaptivePerformanceSubsystemDescriptor : SubsystemDescriptorWithProvider<AdaptivePerformanceSubsystem, AdaptivePerformanceSubsystem.APProvider>
	{
		public AdaptivePerformanceSubsystemDescriptor(AdaptivePerformanceSubsystemDescriptor.Cinfo cinfo)
		{
			base.id = cinfo.id;
			base.providerType = cinfo.providerType;
			base.subsystemTypeOverride = cinfo.subsystemTypeOverride;
		}

		public static AdaptivePerformanceSubsystemDescriptor RegisterDescriptor(AdaptivePerformanceSubsystemDescriptor.Cinfo cinfo)
		{
			List<AdaptivePerformanceSubsystemDescriptor> registeredDescriptors = AdaptivePerformanceSubsystemRegistry.GetRegisteredDescriptors();
			foreach (AdaptivePerformanceSubsystemDescriptor adaptivePerformanceSubsystemDescriptor in registeredDescriptors)
			{
				bool flag = adaptivePerformanceSubsystemDescriptor.id == cinfo.id;
				if (flag)
				{
					return adaptivePerformanceSubsystemDescriptor;
				}
			}
			return AdaptivePerformanceSubsystemRegistry.RegisterDescriptor(cinfo);
		}

		public struct Cinfo
		{
			public string id { readonly get; set; }

			public Type providerType { readonly get; set; }

			public Type subsystemTypeOverride { readonly get; set; }

			[Obsolete("AdaptivePerformanceSubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2023.1. Use providerType and, optionally, subsystemTypeOverride instead.", true)]
			public Type subsystemImplementationType { readonly get; set; }
		}
	}
}
