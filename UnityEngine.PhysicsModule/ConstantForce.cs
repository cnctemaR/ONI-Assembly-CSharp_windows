using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics/ConstantForce.h")]
	[RequireComponent(typeof(Rigidbody))]
	public class ConstantForce : Behaviour
	{
		public Vector3 force
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConstantForce>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ConstantForce.get_force_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConstantForce>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConstantForce.set_force_Injected(intPtr, ref value);
			}
		}

		public Vector3 torque
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConstantForce>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ConstantForce.get_torque_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConstantForce>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConstantForce.set_torque_Injected(intPtr, ref value);
			}
		}

		public Vector3 relativeForce
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConstantForce>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ConstantForce.get_relativeForce_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConstantForce>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConstantForce.set_relativeForce_Injected(intPtr, ref value);
			}
		}

		public Vector3 relativeTorque
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConstantForce>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ConstantForce.get_relativeTorque_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConstantForce>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConstantForce.set_relativeTorque_Injected(intPtr, ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_force_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_force_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_torque_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_torque_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_relativeForce_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_relativeForce_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_relativeTorque_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_relativeTorque_Injected(IntPtr _unity_self, [In] ref Vector3 value);
	}
}
