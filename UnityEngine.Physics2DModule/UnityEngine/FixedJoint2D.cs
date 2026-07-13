using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics2D/FixedJoint2D.h")]
	public sealed class FixedJoint2D : AnchoredJoint2D
	{
		public float dampingRatio
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<FixedJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return FixedJoint2D.get_dampingRatio_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<FixedJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				FixedJoint2D.set_dampingRatio_Injected(intPtr, value);
			}
		}

		public float frequency
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<FixedJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return FixedJoint2D.get_frequency_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<FixedJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				FixedJoint2D.set_frequency_Injected(intPtr, value);
			}
		}

		public float referenceAngle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<FixedJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return FixedJoint2D.get_referenceAngle_Injected(intPtr);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_dampingRatio_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_dampingRatio_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_frequency_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_frequency_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_referenceAngle_Injected(IntPtr _unity_self);
	}
}
