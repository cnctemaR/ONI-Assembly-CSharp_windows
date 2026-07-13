using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeHeader("Modules/Subsystems/Subsystem.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class IntegratedSubsystem : ISubsystem
	{
		internal void SetHandle([UnityMarshalAs(NativeType.ScriptingObjectPtr)] IntegratedSubsystem subsystem)
		{
			IntPtr intPtr = IntegratedSubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntegratedSubsystem.SetHandle_Injected(intPtr, subsystem);
		}

		public void Start()
		{
			IntPtr intPtr = IntegratedSubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntegratedSubsystem.Start_Injected(intPtr);
		}

		public void Stop()
		{
			IntPtr intPtr = IntegratedSubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntegratedSubsystem.Stop_Injected(intPtr);
		}

		public void Destroy()
		{
			IntPtr ptr = this.m_Ptr;
			SubsystemManager.RemoveIntegratedSubsystemByPtr(this.m_Ptr);
			SubsystemBindings.DestroySubsystem(ptr);
			this.m_Ptr = IntPtr.Zero;
		}

		public bool running
		{
			get
			{
				return this.valid && this.IsRunning();
			}
		}

		internal bool valid
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		internal bool IsRunning()
		{
			IntPtr intPtr = IntegratedSubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return IntegratedSubsystem.IsRunning_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetHandle_Injected(IntPtr _unity_self, IntegratedSubsystem subsystem);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Start_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsRunning_Injected(IntPtr _unity_self);

		[VisibleToOtherModules(new string[] { "UnityEngine.XRModule" })]
		internal IntPtr m_Ptr;

		internal ISubsystemDescriptor m_SubsystemDescriptor;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(IntegratedSubsystem integratedSubsystem)
			{
				return integratedSubsystem.m_Ptr;
			}
		}
	}
}
