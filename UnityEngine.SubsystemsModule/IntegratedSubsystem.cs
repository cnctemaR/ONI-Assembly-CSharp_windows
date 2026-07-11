using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeType(Header = "Modules/Subsystems/Subsystem.h")]
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
			this.m_Ptr = IntPtr.Zero;
		}

		public bool running
		{
			get
			{
				return this.valid && this.Internal_IsRunning();
			}
		}

		internal bool valid
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool Internal_IsRunning();

		internal IntPtr m_Ptr;

		internal ISubsystemDescriptor m_subsystemDescriptor;
	}
}
