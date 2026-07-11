using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Joint that restricts the motion of a Rigidbody2D object to a single line.</para>
	/// </summary>
	[NativeHeader("Modules/Physics2D/SliderJoint2D.h")]
	public sealed class SliderJoint2D : AnchoredJoint2D
	{
		/// <summary>
		///   <para>Should the angle be calculated automatically?</para>
		/// </summary>
		public extern bool autoConfigureAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The angle of the line in space (in degrees).</para>
		/// </summary>
		public extern float angle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
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
		///   <para>Should motion limits be used?</para>
		/// </summary>
		public extern bool useLimits
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
		///   <para>Restrictions on how far the joint can slide in each direction along the line.</para>
		/// </summary>
		public JointTranslationLimits2D limits
		{
			get
			{
				JointTranslationLimits2D jointTranslationLimits2D;
				this.get_limits_Injected(out jointTranslationLimits2D);
				return jointTranslationLimits2D;
			}
			set
			{
				this.set_limits_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Gets the state of the joint limit.</para>
		/// </summary>
		public extern JointLimitState2D limitState
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The angle (in degrees) referenced between the two bodies used as the constraint for the joint.</para>
		/// </summary>
		public extern float referenceAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
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
		///   <para>The current joint speed.</para>
		/// </summary>
		public extern float jointSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Gets the motor force of the joint given the specified timestep.</para>
		/// </summary>
		/// <param name="timeStep">The time to calculate the motor force for.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetMotorForce(float timeStep);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_motor_Injected(out JointMotor2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_motor_Injected(ref JointMotor2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_limits_Injected(out JointTranslationLimits2D ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_limits_Injected(ref JointTranslationLimits2D value);
	}
}
