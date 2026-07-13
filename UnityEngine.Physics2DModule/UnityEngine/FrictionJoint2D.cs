using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics2D/FrictionJoint2D.h")]
	public sealed class FrictionJoint2D : AnchoredJoint2D
	{
		public float maxForce
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<FrictionJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return FrictionJoint2D.get_maxForce_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<FrictionJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				FrictionJoint2D.set_maxForce_Injected(intPtr, value);
			}
		}

		public float maxTorque
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<FrictionJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return FrictionJoint2D.get_maxTorque_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<FrictionJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				FrictionJoint2D.set_maxTorque_Injected(intPtr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_maxForce_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maxForce_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_maxTorque_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maxTorque_Injected(IntPtr _unity_self, float value);
	}
}
