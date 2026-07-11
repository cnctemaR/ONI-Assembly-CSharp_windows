using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>The configurable joint is an extremely flexible joint giving you complete control over rotation and linear motion.</para>
	/// </summary>
	[NativeClass("Unity::ConfigurableJoint")]
	[NativeHeader("Runtime/Dynamics/ConfigurableJoint.h")]
	public class ConfigurableJoint : Joint
	{
		/// <summary>
		///   <para>The joint's secondary axis.</para>
		/// </summary>
		public Vector3 secondaryAxis
		{
			get
			{
				Vector3 vector;
				this.get_secondaryAxis_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_secondaryAxis_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Allow movement along the X axis to be Free, completely Locked, or Limited according to Linear Limit.</para>
		/// </summary>
		public extern ConfigurableJointMotion xMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Allow movement along the Y axis to be Free, completely Locked, or Limited according to Linear Limit.</para>
		/// </summary>
		public extern ConfigurableJointMotion yMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Allow movement along the Z axis to be Free, completely Locked, or Limited according to Linear Limit.</para>
		/// </summary>
		public extern ConfigurableJointMotion zMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Allow rotation around the X axis to be Free, completely Locked, or Limited according to Low and High Angular XLimit.</para>
		/// </summary>
		public extern ConfigurableJointMotion angularXMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Allow rotation around the Y axis to be Free, completely Locked, or Limited according to Angular YLimit.</para>
		/// </summary>
		public extern ConfigurableJointMotion angularYMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Allow rotation around the Z axis to be Free, completely Locked, or Limited according to Angular ZLimit.</para>
		/// </summary>
		public extern ConfigurableJointMotion angularZMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The configuration of the spring attached to the linear limit of the joint.</para>
		/// </summary>
		public SoftJointLimitSpring linearLimitSpring
		{
			get
			{
				SoftJointLimitSpring softJointLimitSpring;
				this.get_linearLimitSpring_Injected(out softJointLimitSpring);
				return softJointLimitSpring;
			}
			set
			{
				this.set_linearLimitSpring_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The configuration of the spring attached to the angular X limit of the joint.</para>
		/// </summary>
		public SoftJointLimitSpring angularXLimitSpring
		{
			get
			{
				SoftJointLimitSpring softJointLimitSpring;
				this.get_angularXLimitSpring_Injected(out softJointLimitSpring);
				return softJointLimitSpring;
			}
			set
			{
				this.set_angularXLimitSpring_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The configuration of the spring attached to the angular Y and angular Z limits of the joint.</para>
		/// </summary>
		public SoftJointLimitSpring angularYZLimitSpring
		{
			get
			{
				SoftJointLimitSpring softJointLimitSpring;
				this.get_angularYZLimitSpring_Injected(out softJointLimitSpring);
				return softJointLimitSpring;
			}
			set
			{
				this.set_angularYZLimitSpring_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Boundary defining movement restriction, based on distance from the joint's origin.</para>
		/// </summary>
		public SoftJointLimit linearLimit
		{
			get
			{
				SoftJointLimit softJointLimit;
				this.get_linearLimit_Injected(out softJointLimit);
				return softJointLimit;
			}
			set
			{
				this.set_linearLimit_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Boundary defining lower rotation restriction, based on delta from original rotation.</para>
		/// </summary>
		public SoftJointLimit lowAngularXLimit
		{
			get
			{
				SoftJointLimit softJointLimit;
				this.get_lowAngularXLimit_Injected(out softJointLimit);
				return softJointLimit;
			}
			set
			{
				this.set_lowAngularXLimit_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Boundary defining upper rotation restriction, based on delta from original rotation.</para>
		/// </summary>
		public SoftJointLimit highAngularXLimit
		{
			get
			{
				SoftJointLimit softJointLimit;
				this.get_highAngularXLimit_Injected(out softJointLimit);
				return softJointLimit;
			}
			set
			{
				this.set_highAngularXLimit_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Boundary defining rotation restriction, based on delta from original rotation.</para>
		/// </summary>
		public SoftJointLimit angularYLimit
		{
			get
			{
				SoftJointLimit softJointLimit;
				this.get_angularYLimit_Injected(out softJointLimit);
				return softJointLimit;
			}
			set
			{
				this.set_angularYLimit_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Boundary defining rotation restriction, based on delta from original rotation.</para>
		/// </summary>
		public SoftJointLimit angularZLimit
		{
			get
			{
				SoftJointLimit softJointLimit;
				this.get_angularZLimit_Injected(out softJointLimit);
				return softJointLimit;
			}
			set
			{
				this.set_angularZLimit_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The desired position that the joint should move into.</para>
		/// </summary>
		public Vector3 targetPosition
		{
			get
			{
				Vector3 vector;
				this.get_targetPosition_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_targetPosition_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The desired velocity that the joint should move along.</para>
		/// </summary>
		public Vector3 targetVelocity
		{
			get
			{
				Vector3 vector;
				this.get_targetVelocity_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_targetVelocity_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Definition of how the joint's movement will behave along its local X axis.</para>
		/// </summary>
		public JointDrive xDrive
		{
			get
			{
				JointDrive jointDrive;
				this.get_xDrive_Injected(out jointDrive);
				return jointDrive;
			}
			set
			{
				this.set_xDrive_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Definition of how the joint's movement will behave along its local Y axis.</para>
		/// </summary>
		public JointDrive yDrive
		{
			get
			{
				JointDrive jointDrive;
				this.get_yDrive_Injected(out jointDrive);
				return jointDrive;
			}
			set
			{
				this.set_yDrive_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Definition of how the joint's movement will behave along its local Z axis.</para>
		/// </summary>
		public JointDrive zDrive
		{
			get
			{
				JointDrive jointDrive;
				this.get_zDrive_Injected(out jointDrive);
				return jointDrive;
			}
			set
			{
				this.set_zDrive_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>This is a Quaternion. It defines the desired rotation that the joint should rotate into.</para>
		/// </summary>
		public Quaternion targetRotation
		{
			get
			{
				Quaternion quaternion;
				this.get_targetRotation_Injected(out quaternion);
				return quaternion;
			}
			set
			{
				this.set_targetRotation_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>This is a Vector3. It defines the desired angular velocity that the joint should rotate into.</para>
		/// </summary>
		public Vector3 targetAngularVelocity
		{
			get
			{
				Vector3 vector;
				this.get_targetAngularVelocity_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_targetAngularVelocity_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Control the object's rotation with either X &amp; YZ or Slerp Drive by itself.</para>
		/// </summary>
		public extern RotationDriveMode rotationDriveMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Definition of how the joint's rotation will behave around its local X axis. Only used if Rotation Drive Mode is Swing &amp; Twist.</para>
		/// </summary>
		public JointDrive angularXDrive
		{
			get
			{
				JointDrive jointDrive;
				this.get_angularXDrive_Injected(out jointDrive);
				return jointDrive;
			}
			set
			{
				this.set_angularXDrive_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Definition of how the joint's rotation will behave around its local Y and Z axes. Only used if Rotation Drive Mode is Swing &amp; Twist.</para>
		/// </summary>
		public JointDrive angularYZDrive
		{
			get
			{
				JointDrive jointDrive;
				this.get_angularYZDrive_Injected(out jointDrive);
				return jointDrive;
			}
			set
			{
				this.set_angularYZDrive_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Definition of how the joint's rotation will behave around all local axes. Only used if Rotation Drive Mode is Slerp Only.</para>
		/// </summary>
		public JointDrive slerpDrive
		{
			get
			{
				JointDrive jointDrive;
				this.get_slerpDrive_Injected(out jointDrive);
				return jointDrive;
			}
			set
			{
				this.set_slerpDrive_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Brings violated constraints back into alignment even when the solver fails. Projection is not a physical process and does not preserve momentum or respect collision geometry. It is best avoided if practical, but can be useful in improving simulation quality where joint separation results in unacceptable artifacts.</para>
		/// </summary>
		public extern JointProjectionMode projectionMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Set the linear tolerance threshold for projection.
		///
		/// If the joint separates by more than this distance along its locked degrees of freedom, the solver
		/// will move the bodies to close the distance.
		///
		/// Setting a very small tolerance may result in simulation jitter or other artifacts.
		///
		/// Sometimes it is not possible to project (for example when the joints form a cycle).</para>
		/// </summary>
		public extern float projectionDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Set the angular tolerance threshold (in degrees) for projection.
		///
		/// If the joint deviates by more than this angle around its locked angular degrees of freedom,
		/// the solver will move the bodies to close the angle.
		///
		/// Setting a very small tolerance may result in simulation jitter or other artifacts.
		///
		/// Sometimes it is not possible to project (for example when the joints form a cycle).</para>
		/// </summary>
		public extern float projectionAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>If enabled, all Target values will be calculated in world space instead of the object's local space.</para>
		/// </summary>
		public extern bool configuredInWorldSpace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>If enabled, the two connected rigidbodies will be swapped, as if the joint was attached to the other body.</para>
		/// </summary>
		public extern bool swapBodies
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_secondaryAxis_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_secondaryAxis_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_linearLimitSpring_Injected(out SoftJointLimitSpring ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_linearLimitSpring_Injected(ref SoftJointLimitSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_angularXLimitSpring_Injected(out SoftJointLimitSpring ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_angularXLimitSpring_Injected(ref SoftJointLimitSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_angularYZLimitSpring_Injected(out SoftJointLimitSpring ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_angularYZLimitSpring_Injected(ref SoftJointLimitSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_linearLimit_Injected(out SoftJointLimit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_linearLimit_Injected(ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_lowAngularXLimit_Injected(out SoftJointLimit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_lowAngularXLimit_Injected(ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_highAngularXLimit_Injected(out SoftJointLimit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_highAngularXLimit_Injected(ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_angularYLimit_Injected(out SoftJointLimit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_angularYLimit_Injected(ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_angularZLimit_Injected(out SoftJointLimit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_angularZLimit_Injected(ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_targetPosition_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_targetPosition_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_targetVelocity_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_targetVelocity_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_xDrive_Injected(out JointDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_xDrive_Injected(ref JointDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_yDrive_Injected(out JointDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_yDrive_Injected(ref JointDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_zDrive_Injected(out JointDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_zDrive_Injected(ref JointDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_targetRotation_Injected(out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_targetRotation_Injected(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_targetAngularVelocity_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_targetAngularVelocity_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_angularXDrive_Injected(out JointDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_angularXDrive_Injected(ref JointDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_angularYZDrive_Injected(out JointDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_angularYZDrive_Injected(ref JointDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_slerpDrive_Injected(out JointDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_slerpDrive_Injected(ref JointDrive value);
	}
}
