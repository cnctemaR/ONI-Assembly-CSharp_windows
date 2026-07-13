using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics2D/WheelJoint2D.h")]
	public sealed class WheelJoint2D : AnchoredJoint2D
	{
		public JointSuspension2D suspension
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointSuspension2D jointSuspension2D;
				WheelJoint2D.get_suspension_Injected(intPtr, out jointSuspension2D);
				return jointSuspension2D;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelJoint2D.set_suspension_Injected(intPtr, ref value);
			}
		}

		public bool useMotor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelJoint2D.get_useMotor_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelJoint2D.set_useMotor_Injected(intPtr, value);
			}
		}

		public JointMotor2D motor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointMotor2D jointMotor2D;
				WheelJoint2D.get_motor_Injected(intPtr, out jointMotor2D);
				return jointMotor2D;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WheelJoint2D.set_motor_Injected(intPtr, ref value);
			}
		}

		public float jointTranslation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelJoint2D.get_jointTranslation_Injected(intPtr);
			}
		}

		public float jointLinearSpeed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelJoint2D.get_jointLinearSpeed_Injected(intPtr);
			}
		}

		public float jointSpeed
		{
			[NativeMethod("GetJointAngularSpeed")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelJoint2D.get_jointSpeed_Injected(intPtr);
			}
		}

		public float jointAngle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelJoint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WheelJoint2D.get_jointAngle_Injected(intPtr);
			}
		}

		public float GetMotorTorque(float timeStep)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WheelJoint2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return WheelJoint2D.GetMotorTorque_Injected(intPtr, timeStep);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_suspension_Injected(IntPtr _unity_self, out JointSuspension2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_suspension_Injected(IntPtr _unity_self, [In] ref JointSuspension2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useMotor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useMotor_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_motor_Injected(IntPtr _unity_self, out JointMotor2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_motor_Injected(IntPtr _unity_self, [In] ref JointMotor2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_jointTranslation_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_jointLinearSpeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_jointSpeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_jointAngle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetMotorTorque_Injected(IntPtr _unity_self, float timeStep);
	}
}
