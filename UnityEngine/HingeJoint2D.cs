using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class HingeJoint2D : AnchoredJoint2D
	{
		public extern bool useMotor
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useLimits
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public JointMotor2D motor
		{
			get
			{
				JointMotor2D jointMotor2D;
				this.INTERNAL_get_motor(out jointMotor2D);
				return jointMotor2D;
			}
			set
			{
				this.INTERNAL_set_motor(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_motor(out JointMotor2D value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_motor(ref JointMotor2D value);

		public JointAngleLimits2D limits
		{
			get
			{
				JointAngleLimits2D jointAngleLimits2D;
				this.INTERNAL_get_limits(out jointAngleLimits2D);
				return jointAngleLimits2D;
			}
			set
			{
				this.INTERNAL_set_limits(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_limits(out JointAngleLimits2D value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_limits(ref JointAngleLimits2D value);

		public extern JointLimitState2D limitState
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float referenceAngle
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float jointAngle
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float jointSpeed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public float GetMotorTorque(float timeStep)
		{
			return HingeJoint2D.INTERNAL_CALL_GetMotorTorque(this, timeStep);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_GetMotorTorque(HingeJoint2D self, float timeStep);
	}
}
