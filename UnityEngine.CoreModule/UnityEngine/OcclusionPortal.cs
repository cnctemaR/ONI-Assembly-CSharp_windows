using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Camera/OcclusionPortal.h")]
	public sealed class OcclusionPortal : Component
	{
		[NativeProperty("IsOpen")]
		public bool open
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OcclusionPortal>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return OcclusionPortal.get_open_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<OcclusionPortal>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				OcclusionPortal.set_open_Injected(intPtr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_open_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_open_Injected(IntPtr _unity_self, bool value);
	}
}
