using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental
{
	/// <summary>
	///   <para>Information about a subsystem that can be queried before creating a subsystem instance.</para>
	/// </summary>
	[NativeType(Header = "Modules/XR/XRSubsystemDescriptor.h")]
	[UsedByNativeCode("XRSubsystemDescriptor")]
	[StructLayout(LayoutKind.Sequential)]
	public class SubsystemDescriptor<TSubsystem> : SubsystemDescriptorBase where TSubsystem : Subsystem
	{
		public TSubsystem Create()
		{
			IntPtr intPtr = Internal_SubsystemDescriptors.Create(this.m_Ptr);
			TSubsystem tsubsystem = (TSubsystem)((object)Internal_SubsystemInstances.Internal_GetInstanceByPtr(intPtr));
			if (tsubsystem != null)
			{
				tsubsystem.m_subsystemDescriptor = this;
			}
			return tsubsystem;
		}
	}
}
