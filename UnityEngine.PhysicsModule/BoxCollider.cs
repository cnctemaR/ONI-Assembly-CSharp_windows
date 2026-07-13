using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics/BoxCollider.h")]
	[RequireComponent(typeof(Transform))]
	public class BoxCollider : Collider
	{
		public Vector3 center
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BoxCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				BoxCollider.get_center_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BoxCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BoxCollider.set_center_Injected(intPtr, ref value);
			}
		}

		public Vector3 size
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BoxCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				BoxCollider.get_size_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BoxCollider>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BoxCollider.set_size_Injected(intPtr, ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_center_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_center_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_size_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_size_Injected(IntPtr _unity_self, [In] ref Vector3 value);
	}
}
