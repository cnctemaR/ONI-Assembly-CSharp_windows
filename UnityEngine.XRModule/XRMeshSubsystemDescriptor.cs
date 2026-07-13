using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeType(Header = "Modules/XR/Subsystems/Planes/XRMeshSubsystemDescriptor.h")]
	[UsedByNativeCode]
	public class XRMeshSubsystemDescriptor : IntegratedSubsystemDescriptor<XRMeshSubsystem>
	{
		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(XRMeshSubsystemDescriptor descriptor)
			{
				return descriptor.m_Ptr;
			}
		}
	}
}
