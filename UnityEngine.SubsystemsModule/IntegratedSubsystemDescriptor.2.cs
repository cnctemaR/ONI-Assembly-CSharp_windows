using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeType(Header = "Modules/Subsystems/SubsystemDescriptor.h")]
	[UsedByNativeCode("SubsystemDescriptor")]
	[StructLayout(LayoutKind.Sequential)]
	public class IntegratedSubsystemDescriptor<TSubsystem> : IntegratedSubsystemDescriptor where TSubsystem : IntegratedSubsystem
	{
		internal override ISubsystem CreateImpl()
		{
			return this.Create();
		}

		public TSubsystem Create()
		{
			IntPtr intPtr = Internal_SubsystemDescriptors.Create(this.m_Ptr);
			TSubsystem tsubsystem = (TSubsystem)((object)Internal_SubsystemInstances.Internal_GetInstanceByPtr(intPtr));
			bool flag = tsubsystem != null;
			if (flag)
			{
				tsubsystem.m_subsystemDescriptor = this;
			}
			return tsubsystem;
		}
	}
}
