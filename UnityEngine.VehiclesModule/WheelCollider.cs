using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>A special collider for vehicle wheels.</para>
	/// </summary>
	[NativeHeader("Runtime/Vehicles/WheelCollider.h")]
	[NativeHeader("PhysicsScriptingClasses.h")]
	public class WheelCollider : Collider
	{
		/// <summary>
		///   <para>The center of the wheel, measured in the object's local space.</para>
		/// </summary>
		public Vector3 center
		{
			get
			{
				Vector3 vector;
				this.get_center_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_center_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The radius of the wheel, measured in local space.</para>
		/// </summary>
		public extern float radius
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Maximum extension distance of wheel suspension, measured in local space.</para>
		/// </summary>
		public extern float suspensionDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The parameters of wheel's suspension. The suspension attempts to reach a target position by applying a linear force and a damping force.</para>
		/// </summary>
		public JointSpring suspensionSpring
		{
			get
			{
				JointSpring jointSpring;
				this.get_suspensionSpring_Injected(out jointSpring);
				return jointSpring;
			}
			set
			{
				this.set_suspensionSpring_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Application point of the suspension and tire forces measured from the base of the resting wheel.</para>
		/// </summary>
		public extern float forceAppPointDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The mass of the wheel, expressed in kilograms. Must be larger than zero. Typical values would be in range (20,80).</para>
		/// </summary>
		public extern float mass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The damping rate of the wheel. Must be larger than zero.</para>
		/// </summary>
		public extern float wheelDampingRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Properties of tire friction in the direction the wheel is pointing in.</para>
		/// </summary>
		public WheelFrictionCurve forwardFriction
		{
			get
			{
				WheelFrictionCurve wheelFrictionCurve;
				this.get_forwardFriction_Injected(out wheelFrictionCurve);
				return wheelFrictionCurve;
			}
			set
			{
				this.set_forwardFriction_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Properties of tire friction in the sideways direction.</para>
		/// </summary>
		public WheelFrictionCurve sidewaysFriction
		{
			get
			{
				WheelFrictionCurve wheelFrictionCurve;
				this.get_sidewaysFriction_Injected(out wheelFrictionCurve);
				return wheelFrictionCurve;
			}
			set
			{
				this.set_sidewaysFriction_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Motor torque on the wheel axle expressed in Newton metres. Positive or negative depending on direction.</para>
		/// </summary>
		public extern float motorTorque
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Brake torque expressed in Newton metres.</para>
		/// </summary>
		public extern float brakeTorque
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Steering angle in degrees, always around the local y-axis.</para>
		/// </summary>
		public extern float steerAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Indicates whether the wheel currently collides with something (Read Only).</para>
		/// </summary>
		public extern bool isGrounded
		{
			[NativeName("IsGrounded")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Current wheel axle rotation speed, in rotations per minute (Read Only).</para>
		/// </summary>
		public extern float rpm
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The mass supported by this WheelCollider.</para>
		/// </summary>
		public extern float sprungMass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Configure vehicle sub-stepping parameters.</para>
		/// </summary>
		/// <param name="speedThreshold">The speed threshold of the sub-stepping algorithm.</param>
		/// <param name="stepsBelowThreshold">Amount of simulation sub-steps when vehicle's speed is below speedThreshold.</param>
		/// <param name="stepsAboveThreshold">Amount of simulation sub-steps when vehicle's speed is above speedThreshold.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ConfigureVehicleSubsteps(float speedThreshold, int stepsBelowThreshold, int stepsAboveThreshold);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetWorldPose(out Vector3 pos, out Quaternion quat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetGroundHit(out WheelHit hit);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_center_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_center_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_suspensionSpring_Injected(out JointSpring ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_suspensionSpring_Injected(ref JointSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_forwardFriction_Injected(out WheelFrictionCurve ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_forwardFriction_Injected(ref WheelFrictionCurve value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_sidewaysFriction_Injected(out WheelFrictionCurve ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_sidewaysFriction_Injected(ref WheelFrictionCurve value);
	}
}
