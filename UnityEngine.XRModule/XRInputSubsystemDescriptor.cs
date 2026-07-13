using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeType(Header = "Modules/XR/Subsystems/Input/XRInputSubsystemDescriptor.h")]
	[UsedByNativeCode]
	[NativeConditional("ENABLE_XR")]
	public class XRInputSubsystemDescriptor : IntegratedSubsystemDescriptor<XRInputSubsystem>
	{
		[NativeConditional("ENABLE_XR")]
		public bool disablesLegacyInput
		{
			get
			{
				IntPtr intPtr = XRInputSubsystemDescriptor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRInputSubsystemDescriptor.get_disablesLegacyInput_Injected(intPtr);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_disablesLegacyInput_Injected(IntPtr _unity_self);

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(XRInputSubsystemDescriptor descriptor)
			{
				return descriptor.m_Ptr;
			}
		}
	}
}
