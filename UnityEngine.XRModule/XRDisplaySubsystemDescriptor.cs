using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[NativeType(Header = "Modules/XR/Subsystems/Display/XRDisplaySubsystemDescriptor.h")]
	[UsedByNativeCode]
	public class XRDisplaySubsystemDescriptor : IntegratedSubsystemDescriptor<XRDisplaySubsystem>
	{
		[NativeConditional("ENABLE_XR")]
		public bool disablesLegacyVr
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystemDescriptor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystemDescriptor.get_disablesLegacyVr_Injected(intPtr);
			}
		}

		[NativeConditional("ENABLE_XR")]
		public bool enableBackBufferMSAA
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystemDescriptor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystemDescriptor.get_enableBackBufferMSAA_Injected(intPtr);
			}
		}

		[NativeConditional("ENABLE_XR")]
		[NativeMethod("TryGetAvailableMirrorModeCount")]
		public int GetAvailableMirrorBlitModeCount()
		{
			IntPtr intPtr = XRDisplaySubsystemDescriptor.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystemDescriptor.GetAvailableMirrorBlitModeCount_Injected(intPtr);
		}

		[NativeConditional("ENABLE_XR")]
		[NativeMethod("TryGetMirrorModeByIndex")]
		public void GetMirrorBlitModeByIndex(int index, out XRMirrorViewBlitModeDesc mode)
		{
			IntPtr intPtr = XRDisplaySubsystemDescriptor.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			XRDisplaySubsystemDescriptor.GetMirrorBlitModeByIndex_Injected(intPtr, index, out mode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_disablesLegacyVr_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enableBackBufferMSAA_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAvailableMirrorBlitModeCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMirrorBlitModeByIndex_Injected(IntPtr _unity_self, int index, out XRMirrorViewBlitModeDesc mode);

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(XRDisplaySubsystemDescriptor descriptor)
			{
				return descriptor.m_Ptr;
			}
		}
	}
}
