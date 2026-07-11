using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental
{
	[NativeType(Header = "Modules/XR/XRSubsystemManager.h")]
	public static class SubsystemManager
	{
		static SubsystemManager()
		{
			SubsystemManager.StaticConstructScriptingClassMap();
		}

		[NativeConditional("ENABLE_XR")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ReportSingleSubsystemAnalytics(string id);

		public static void GetSubsystemDescriptors<T>(List<T> descriptors) where T : ISubsystemDescriptor
		{
			descriptors.Clear();
			foreach (ISubsystemDescriptorImpl subsystemDescriptorImpl in Internal_SubsystemDescriptors.s_IntegratedSubsystemDescriptors)
			{
				if (subsystemDescriptorImpl is T)
				{
					descriptors.Add((T)((object)subsystemDescriptorImpl));
				}
			}
			foreach (ISubsystemDescriptor subsystemDescriptor in Internal_SubsystemDescriptors.s_StandaloneSubsystemDescriptors)
			{
				if (subsystemDescriptor is T)
				{
					descriptors.Add((T)((object)subsystemDescriptor));
				}
			}
		}

		public static void GetInstances<T>(List<T> instances) where T : ISubsystem
		{
			instances.Clear();
			foreach (ISubsystem subsystem in Internal_SubsystemInstances.s_IntegratedSubsystemInstances)
			{
				if (subsystem is T)
				{
					instances.Add((T)((object)subsystem));
				}
			}
			foreach (ISubsystem subsystem2 in Internal_SubsystemInstances.s_StandaloneSubsystemInstances)
			{
				if (subsystem2 is T)
				{
					instances.Add((T)((object)subsystem2));
				}
			}
		}

		[NativeConditional("ENABLE_XR")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DestroyInstance_Internal(IntPtr instancePtr);

		[NativeConditional("ENABLE_XR")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StaticConstructScriptingClassMap();
	}
}
