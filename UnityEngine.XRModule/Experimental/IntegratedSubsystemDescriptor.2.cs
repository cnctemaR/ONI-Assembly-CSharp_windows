using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental
{
	[UsedByNativeCode("XRSubsystemDescriptor")]
	[NativeType(Header = "Modules/XR/XRSubsystemDescriptor.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class IntegratedSubsystemDescriptor<TSubsystem> : IntegratedSubsystemDescriptor where TSubsystem : IntegratedSubsystem
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
