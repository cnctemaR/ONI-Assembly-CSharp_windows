using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeType(Header = "Modules/Subsystems/SubsystemManager.h")]
	public static class SubsystemManager
	{
		static SubsystemManager()
		{
			SubsystemManager.StaticConstructScriptingClassMap();
		}

		public static void GetAllSubsystemDescriptors(List<ISubsystemDescriptor> descriptors)
		{
			descriptors.Clear();
			foreach (ISubsystemDescriptorImpl subsystemDescriptorImpl in Internal_SubsystemDescriptors.s_IntegratedSubsystemDescriptors)
			{
				descriptors.Add(subsystemDescriptorImpl);
			}
			foreach (ISubsystemDescriptor subsystemDescriptor in Internal_SubsystemDescriptors.s_StandaloneSubsystemDescriptors)
			{
				descriptors.Add(subsystemDescriptor);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ReportSingleSubsystemAnalytics(string id);

		public static void GetSubsystemDescriptors<T>(List<T> descriptors) where T : ISubsystemDescriptor
		{
			descriptors.Clear();
			foreach (ISubsystemDescriptorImpl subsystemDescriptorImpl in Internal_SubsystemDescriptors.s_IntegratedSubsystemDescriptors)
			{
				bool flag = subsystemDescriptorImpl is T;
				if (flag)
				{
					descriptors.Add((T)((object)subsystemDescriptorImpl));
				}
			}
			foreach (ISubsystemDescriptor subsystemDescriptor in Internal_SubsystemDescriptors.s_StandaloneSubsystemDescriptors)
			{
				bool flag2 = subsystemDescriptor is T;
				if (flag2)
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
				bool flag = subsystem is T;
				if (flag)
				{
					instances.Add((T)((object)subsystem));
				}
			}
			foreach (ISubsystem subsystem2 in Internal_SubsystemInstances.s_StandaloneSubsystemInstances)
			{
				bool flag2 = subsystem2 is T;
				if (flag2)
				{
					instances.Add((T)((object)subsystem2));
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DestroyInstance_Internal(IntPtr instancePtr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StaticConstructScriptingClassMap();

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action reloadSubsytemsStarted;

		[RequiredByNativeCode]
		private static void Internal_ReloadSubsystemsStarted()
		{
			bool flag = SubsystemManager.reloadSubsytemsStarted != null;
			if (flag)
			{
				SubsystemManager.reloadSubsytemsStarted();
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action reloadSubsytemsCompleted;

		[RequiredByNativeCode]
		private static void Internal_ReloadSubsystemsCompleted()
		{
			bool flag = SubsystemManager.reloadSubsytemsCompleted != null;
			if (flag)
			{
				SubsystemManager.reloadSubsytemsCompleted();
			}
		}
	}
}
