using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal static class Internal_SubsystemDescriptors
	{
		[RequiredByNativeCode]
		internal static bool Internal_AddDescriptor(SubsystemDescriptor descriptor)
		{
			foreach (ISubsystemDescriptor subsystemDescriptor in Internal_SubsystemDescriptors.s_StandaloneSubsystemDescriptors)
			{
				bool flag = subsystemDescriptor == descriptor;
				if (flag)
				{
					return false;
				}
			}
			Internal_SubsystemDescriptors.s_StandaloneSubsystemDescriptors.Add(descriptor);
			SubsystemManager.ReportSingleSubsystemAnalytics(descriptor.id);
			return true;
		}

		[RequiredByNativeCode]
		internal static void Internal_InitializeManagedDescriptor(IntPtr ptr, ISubsystemDescriptorImpl desc)
		{
			desc.ptr = ptr;
			Internal_SubsystemDescriptors.s_IntegratedSubsystemDescriptors.Add(desc);
		}

		[RequiredByNativeCode]
		internal static void Internal_ClearManagedDescriptors()
		{
			foreach (ISubsystemDescriptorImpl subsystemDescriptorImpl in Internal_SubsystemDescriptors.s_IntegratedSubsystemDescriptors)
			{
				subsystemDescriptorImpl.ptr = IntPtr.Zero;
			}
			Internal_SubsystemDescriptors.s_IntegratedSubsystemDescriptors.Clear();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr Create(IntPtr descriptorPtr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetId(IntPtr descriptorPtr);

		internal static List<ISubsystemDescriptorImpl> s_IntegratedSubsystemDescriptors = new List<ISubsystemDescriptorImpl>();

		internal static List<ISubsystemDescriptor> s_StandaloneSubsystemDescriptors = new List<ISubsystemDescriptor>();
	}
}
