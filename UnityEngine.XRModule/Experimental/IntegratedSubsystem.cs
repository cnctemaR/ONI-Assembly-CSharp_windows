using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental
{
	[NativeType(Header = "Modules/XR/XRSubsystem.h")]
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class IntegratedSubsystem : ISubsystem
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetHandle(IntegratedSubsystem inst);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Start();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Stop();

		public void Destroy()
		{
			IntPtr ptr = this.m_Ptr;
			Internal_SubsystemInstances.Internal_RemoveInstanceByPtr(this.m_Ptr);
			SubsystemManager.DestroyInstance_Internal(ptr);
		}

		internal IntPtr m_Ptr;

		internal ISubsystemDescriptor m_subsystemDescriptor;
	}
}
