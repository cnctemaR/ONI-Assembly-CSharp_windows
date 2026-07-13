using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
	public class Behaviour : Component
	{
		[NativeProperty]
		[RequiredByNativeCode]
		public bool enabled
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Behaviour>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Behaviour.get_enabled_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Behaviour>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Behaviour.set_enabled_Injected(intPtr, value);
			}
		}

		[NativeProperty]
		public bool isActiveAndEnabled
		{
			[NativeMethod("IsAddedToManager")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Behaviour>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Behaviour.get_isActiveAndEnabled_Injected(intPtr);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enabled_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enabled_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isActiveAndEnabled_Injected(IntPtr _unity_self);
	}
}
