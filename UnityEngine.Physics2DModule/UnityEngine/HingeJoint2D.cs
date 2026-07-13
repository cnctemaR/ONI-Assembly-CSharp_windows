using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics2D/HingeJoint2D.h")]
	public sealed class HingeJoint2D : AnchoredJoint2D
	{
		public bool useMotor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HingeJoint2D.get_useMotor_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HingeJoint2D.set_useMotor_Injected(intPtr, value);
			}
		}

		public bool useLimits
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HingeJoint2D.get_useLimits_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HingeJoint2D.set_useLimits_Injected(intPtr, value);
			}
		}

		public bool useConnectedAnchor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HingeJoint2D.get_useConnectedAnchor_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HingeJoint2D.set_useConnectedAnchor_Injected(intPtr, value);
			}
		}

		public JointMotor2D motor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointMotor2D jointMotor2D;
				HingeJoint2D.get_motor_Injected(intPtr, out jointMotor2D);
				return jointMotor2D;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HingeJoint2D.set_motor_Injected(intPtr, ref value);
			}
		}

		public JointAngleLimits2D limits
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointAngleLimits2D jointAngleLimits2D;
				HingeJoint2D.get_limits_Injected(intPtr, out jointAngleLimits2D);
				return jointAngleLimits2D;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HingeJoint2D.set_limits_Injected(intPtr, ref value);
			}
		}

		public JointLimitState2D limitState
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HingeJoint2D.get_limitState_Injected(intPtr);
			}
		}

		public float referenceAngle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HingeJoint2D.get_referenceAngle_Injected(intPtr);
			}
		}

		public float jointAngle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HingeJoint2D.get_jointAngle_Injected(intPtr);
			}
		}

		public float jointSpeed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HingeJoint2D.get_jointSpeed_Injected(intPtr);
			}
		}

		public float GetMotorTorque(float timeStep)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<HingeJoint2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HingeJoint2D.GetMotorTorque_Injected(intPtr, timeStep);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useMotor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useMotor_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useLimits_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useLimits_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useConnectedAnchor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useConnectedAnchor_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_motor_Injected(IntPtr _unity_self, out JointMotor2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_motor_Injected(IntPtr _unity_self, [In] ref JointMotor2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_limits_Injected(IntPtr _unity_self, out JointAngleLimits2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_limits_Injected(IntPtr _unity_self, [In] ref JointAngleLimits2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern JointLimitState2D get_limitState_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_referenceAngle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_jointAngle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_jointSpeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetMotorTorque_Injected(IntPtr _unity_self, float timeStep);
	}
}
