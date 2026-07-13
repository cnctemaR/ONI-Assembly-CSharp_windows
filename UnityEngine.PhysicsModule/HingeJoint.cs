using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics/HingeJoint.h")]
	[NativeClass("Unity::HingeJoint")]
	[RequireComponent(typeof(Rigidbody))]
	public class HingeJoint : Joint
	{
		public JointMotor motor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointMotor jointMotor;
				HingeJoint.get_motor_Injected(intPtr, out jointMotor);
				return jointMotor;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HingeJoint.set_motor_Injected(intPtr, ref value);
			}
		}

		public JointLimits limits
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointLimits jointLimits;
				HingeJoint.get_limits_Injected(intPtr, out jointLimits);
				return jointLimits;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HingeJoint.set_limits_Injected(intPtr, ref value);
			}
		}

		public JointSpring spring
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointSpring jointSpring;
				HingeJoint.get_spring_Injected(intPtr, out jointSpring);
				return jointSpring;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HingeJoint.set_spring_Injected(intPtr, ref value);
			}
		}

		public bool useMotor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HingeJoint.get_useMotor_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HingeJoint.set_useMotor_Injected(intPtr, value);
			}
		}

		public bool useLimits
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HingeJoint.get_useLimits_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HingeJoint.set_useLimits_Injected(intPtr, value);
			}
		}

		public bool extendedLimits
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HingeJoint.get_extendedLimits_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HingeJoint.set_extendedLimits_Injected(intPtr, value);
			}
		}

		public bool useSpring
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HingeJoint.get_useSpring_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HingeJoint.set_useSpring_Injected(intPtr, value);
			}
		}

		public float velocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HingeJoint.get_velocity_Injected(intPtr);
			}
		}

		public float angle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HingeJoint.get_angle_Injected(intPtr);
			}
		}

		public bool useAcceleration
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HingeJoint.get_useAcceleration_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HingeJoint.set_useAcceleration_Injected(intPtr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_motor_Injected(IntPtr _unity_self, out JointMotor ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_motor_Injected(IntPtr _unity_self, [In] ref JointMotor value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_limits_Injected(IntPtr _unity_self, out JointLimits ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_limits_Injected(IntPtr _unity_self, [In] ref JointLimits value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_spring_Injected(IntPtr _unity_self, out JointSpring ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_spring_Injected(IntPtr _unity_self, [In] ref JointSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useMotor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useMotor_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useLimits_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useLimits_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_extendedLimits_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_extendedLimits_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useSpring_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useSpring_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_velocity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_angle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useAcceleration_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useAcceleration_Injected(IntPtr _unity_self, bool value);
	}
}
