using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>The wheel joint allows the simulation of wheels by providing a constraining suspension motion with an optional motor.</para>
	/// </summary>
	[NativeHeader("Modules/Physics2D/WheelJoint2D.h")]
	public sealed class WheelJoint2D : AnchoredJoint2D
	{
		/// <summary>
		///   <para>Set the joint suspension configuration.</para>
		/// </summary>
		public JointSuspension2D suspension
		{
			get
			{
				JointSuspension2D jointSuspension2D;
				this.get_suspension_Injected(out jointSuspension2D);
				return jointSuspension2D;
			}
			set
			{
				this.set_suspension_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Should a motor force be applied automatically to the Rigidbody2D?</para>
		/// </summary>
		public extern bool useMotor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Parameters for a motor force that is applied automatically to the Rigibody2D along the line.</para>
		/// </summary>
		public JointMotor2D motor
		{
			get
			{
				JointMotor2D jointMotor2D;
				this.get_motor_Injected(out jointMotor2D);
				return jointMotor2D;
			}
			set
			{
				this.set_motor_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The current joint translation.</para>
		/// </summary>
		public extern float jointTranslation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The current joint linear speed in meters/sec.</para>
		/// </summary>
		public extern float jointLinearSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The current joint rotational speed in degrees/sec.</para>
		/// </summary>
		public extern float jointSpeed
		{
			[NativeMethod("GetJointAngularSpeed")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The current joint angle (in degrees) defined as the relative angle between the two Rigidbody2D that the joint connects to.</para>
		/// </summary>
		public extern float jointAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Gets the motor torque of the joint given the specified timestep.</para>
		/// </summary>
		/// <param name="timeStep">The time to calculate the motor torque for.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetMotorTorque(float timeStep);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_suspension_Injected(out JointSuspension2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_suspension_Injected(ref JointSuspension2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_motor_Injected(out JointMotor2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_motor_Injected(ref JointMotor2D value);
	}
}
