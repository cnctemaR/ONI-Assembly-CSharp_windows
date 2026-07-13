using System;
using System.Collections.Generic;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.AdaptivePerformance.Provider
{
	internal static class AdaptivePerformanceSubsystemRegistry
	{
		public static AdaptivePerformanceSubsystemDescriptor RegisterDescriptor(AdaptivePerformanceSubsystemDescriptor.Cinfo cinfo)
		{
			AdaptivePerformanceSubsystemDescriptor adaptivePerformanceSubsystemDescriptor = new AdaptivePerformanceSubsystemDescriptor(cinfo);
			SubsystemDescriptorStore.RegisterDescriptor(adaptivePerformanceSubsystemDescriptor);
			return adaptivePerformanceSubsystemDescriptor;
		}

		public static List<AdaptivePerformanceSubsystemDescriptor> GetRegisteredDescriptors()
		{
			List<AdaptivePerformanceSubsystemDescriptor> list = new List<AdaptivePerformanceSubsystemDescriptor>();
			SubsystemManager.GetSubsystemDescriptors<AdaptivePerformanceSubsystemDescriptor>(list);
			return list;
		}
	}
}
