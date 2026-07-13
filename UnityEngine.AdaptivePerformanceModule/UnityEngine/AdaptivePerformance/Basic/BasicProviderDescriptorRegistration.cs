using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.AdaptivePerformance.Provider;
using UnityEngine.Scripting;

namespace UnityEngine.AdaptivePerformance.Basic
{
	internal class BasicProviderDescriptorRegistration
	{
		[RequiredByNativeCode(false)]
		[DynamicDependency("#ctor()", typeof(BasicAdaptivePerformanceSubsystem))]
		[DynamicDependency("#ctor()", typeof(BasicAdaptivePerformanceSubsystem.BasicProvider))]
		private static AdaptivePerformanceSubsystemDescriptor RegisterDescriptor()
		{
			return AdaptivePerformanceSubsystemDescriptor.RegisterDescriptor(new AdaptivePerformanceSubsystemDescriptor.Cinfo
			{
				id = "BasicAdaptivePerformanceSubsystem",
				providerType = typeof(BasicAdaptivePerformanceSubsystem.BasicProvider),
				subsystemTypeOverride = typeof(BasicAdaptivePerformanceSubsystem)
			});
		}
	}
}
