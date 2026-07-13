using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics2D/SliderJoint2D.h")]
	public sealed class SliderJoint2D : AnchoredJoint2D
	{
		public bool autoConfigureAngle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SliderJoint2D.get_autoConfigureAngle_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SliderJoint2D.set_autoConfigureAngle_Injected(intPtr, value);
			}
		}

		public float angle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SliderJoint2D.get_angle_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SliderJoint2D.set_angle_Injected(intPtr, value);
			}
		}

		public bool useMotor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SliderJoint2D.get_useMotor_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SliderJoint2D.set_useMotor_Injected(intPtr, value);
			}
		}

		public bool useLimits
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SliderJoint2D.get_useLimits_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SliderJoint2D.set_useLimits_Injected(intPtr, value);
			}
		}

		public JointMotor2D motor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointMotor2D jointMotor2D;
				SliderJoint2D.get_motor_Injected(intPtr, out jointMotor2D);
				return jointMotor2D;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SliderJoint2D.set_motor_Injected(intPtr, ref value);
			}
		}

		public JointTranslationLimits2D limits
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointTranslationLimits2D jointTranslationLimits2D;
				SliderJoint2D.get_limits_Injected(intPtr, out jointTranslationLimits2D);
				return jointTranslationLimits2D;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SliderJoint2D.set_limits_Injected(intPtr, ref value);
			}
		}

		public JointLimitState2D limitState
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SliderJoint2D.get_limitState_Injected(intPtr);
			}
		}

		public float referenceAngle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SliderJoint2D.get_referenceAngle_Injected(intPtr);
			}
		}

		public float jointTranslation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SliderJoint2D.get_jointTranslation_Injected(intPtr);
			}
		}

		public float jointSpeed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SliderJoint2D.get_jointSpeed_Injected(intPtr);
			}
		}

		public float GetMotorForce(float timeStep)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SliderJoint2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return SliderJoint2D.GetMotorForce_Injected(intPtr, timeStep);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_autoConfigureAngle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_autoConfigureAngle_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_angle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angle_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useMotor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useMotor_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useLimits_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useLimits_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_motor_Injected(IntPtr _unity_self, out JointMotor2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_motor_Injected(IntPtr _unity_self, [In] ref JointMotor2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_limits_Injected(IntPtr _unity_self, out JointTranslationLimits2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_limits_Injected(IntPtr _unity_self, [In] ref JointTranslationLimits2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern JointLimitState2D get_limitState_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_referenceAngle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_jointTranslation_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_jointSpeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetMotorForce_Injected(IntPtr _unity_self, float timeStep);
	}
}
