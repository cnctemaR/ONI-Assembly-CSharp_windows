using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>The HingeJoint groups together 2 rigid bodies, constraining them to move like connected by a hinge.</para>
	/// </summary>
	[NativeHeader("Runtime/Dynamics/HingeJoint.h")]
	[NativeClass("Unity::HingeJoint")]
	public class HingeJoint : Joint
	{
		/// <summary>
		///   <para>The motor will apply a force up to a maximum force to achieve the target velocity in degrees per second.</para>
		/// </summary>
		public JointMotor motor
		{
			get
			{
				JointMotor jointMotor;
				this.get_motor_Injected(out jointMotor);
				return jointMotor;
			}
			set
			{
				this.set_motor_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Limit of angular rotation (in degrees) on the hinge joint.</para>
		/// </summary>
		public JointLimits limits
		{
			get
			{
				JointLimits jointLimits;
				this.get_limits_Injected(out jointLimits);
				return jointLimits;
			}
			set
			{
				this.set_limits_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The spring attempts to reach a target angle by adding spring and damping forces.</para>
		/// </summary>
		public JointSpring spring
		{
			get
			{
				JointSpring jointSpring;
				this.get_spring_Injected(out jointSpring);
				return jointSpring;
			}
			set
			{
				this.set_spring_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Enables the joint's motor. Disabled by default.</para>
		/// </summary>
		public extern bool useMotor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Enables the joint's limits. Disabled by default.</para>
		/// </summary>
		public extern bool useLimits
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Enables the joint's spring. Disabled by default.</para>
		/// </summary>
		public extern bool useSpring
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The angular velocity of the joint in degrees per second. (Read Only)</para>
		/// </summary>
		public extern float velocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The current angle in degrees of the joint relative to its rest position. (Read Only)</para>
		/// </summary>
		public extern float angle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_motor_Injected(out JointMotor ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_motor_Injected(ref JointMotor value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_limits_Injected(out JointLimits ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_limits_Injected(ref JointLimits value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_spring_Injected(out JointSpring ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_spring_Injected(ref JointSpring value);
	}
}
