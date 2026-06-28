using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class WheelJoint2D : AnchoredJoint2D
	{
		public JointSuspension2D suspension
		{
			get
			{
				JointSuspension2D jointSuspension2D;
				this.INTERNAL_get_suspension(out jointSuspension2D);
				return jointSuspension2D;
			}
			set
			{
				this.INTERNAL_set_suspension(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_suspension(out JointSuspension2D value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_suspension(ref JointSuspension2D value);

		public extern bool useMotor
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

		public extern float jointTranslation
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float jointLinearSpeed
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

		public extern float jointAngle
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public float GetMotorTorque(float timeStep)
		{
			return WheelJoint2D.INTERNAL_CALL_GetMotorTorque(this, timeStep);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_GetMotorTorque(WheelJoint2D self, float timeStep);
	}
}
