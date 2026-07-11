using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental
{
	/// <summary>
	///   <para>Gives access to subsystems which provide additional functionality through plugins.</para>
	/// </summary>
	[NativeType(Header = "Modules/XR/XRSubsystemManager.h")]
	public static class SubsystemManager
	{
		static SubsystemManager()
		{
			SubsystemManager.StaticConstructScriptingClassMap();
		}

		public static void GetSubsystemDescriptors<T>(List<T> descriptors) where T : ISubsystemDescriptor
		{
			descriptors.Clear();
			foreach (ISubsystemDescriptorImpl subsystemDescriptorImpl in Internal_SubsystemDescriptors.s_SubsystemDescriptors)
			{
				if (subsystemDescriptorImpl is T)
				{
					descriptors.Add((T)((object)subsystemDescriptorImpl));
				}
			}
		}

		public static void GetInstances<T>(List<T> instances) where T : Subsystem
		{
			instances.Clear();
			foreach (Subsystem subsystem in Internal_SubsystemInstances.s_SubsystemInstances)
			{
				if (subsystem is T)
				{
					instances.Add((T)((object)subsystem));
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DestroyInstance_Internal(IntPtr instancePtr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void StaticConstructScriptingClassMap();
	}
}
