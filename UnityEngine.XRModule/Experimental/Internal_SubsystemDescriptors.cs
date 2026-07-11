using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental
{
	internal static class Internal_SubsystemDescriptors
	{
		[RequiredByNativeCode]
		internal static void Internal_InitializeManagedDescriptor(IntPtr ptr, ISubsystemDescriptorImpl desc)
		{
			desc.ptr = ptr;
			Internal_SubsystemDescriptors.s_SubsystemDescriptors.Add(desc);
		}

		[RequiredByNativeCode]
		internal static void Internal_ClearManagedDescriptors()
		{
			foreach (ISubsystemDescriptorImpl subsystemDescriptorImpl in Internal_SubsystemDescriptors.s_SubsystemDescriptors)
			{
				subsystemDescriptorImpl.ptr = IntPtr.Zero;
			}
			Internal_SubsystemDescriptors.s_SubsystemDescriptors.Clear();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr Create(IntPtr descriptorPtr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetId(IntPtr descriptorPtr);

		internal static List<ISubsystemDescriptorImpl> s_SubsystemDescriptors = new List<ISubsystemDescriptorImpl>();
	}
}
