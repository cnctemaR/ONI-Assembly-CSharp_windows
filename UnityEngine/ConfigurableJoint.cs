using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class ConfigurableJoint : Joint
	{
		public Vector3 secondaryAxis
		{
			get
			{
				Vector3 vector;
				this.INTERNAL_get_secondaryAxis(out vector);
				return vector;
			}
			set
			{
				this.INTERNAL_set_secondaryAxis(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_secondaryAxis(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_secondaryAxis(ref Vector3 value);

		public extern ConfigurableJointMotion xMotion
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion yMotion
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion zMotion
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion angularXMotion
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion angularYMotion
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion angularZMotion
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public SoftJointLimitSpring linearLimitSpring
		{
			get
			{
				SoftJointLimitSpring softJointLimitSpring;
				this.INTERNAL_get_linearLimitSpring(out softJointLimitSpring);
				return softJointLimitSpring;
			}
			set
			{
				this.INTERNAL_set_linearLimitSpring(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_linearLimitSpring(out SoftJointLimitSpring value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_linearLimitSpring(ref SoftJointLimitSpring value);

		public SoftJointLimitSpring angularXLimitSpring
		{
			get
			{
				SoftJointLimitSpring softJointLimitSpring;
				this.INTERNAL_get_angularXLimitSpring(out softJointLimitSpring);
				return softJointLimitSpring;
			}
			set
			{
				this.INTERNAL_set_angularXLimitSpring(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_angularXLimitSpring(out SoftJointLimitSpring value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_angularXLimitSpring(ref SoftJointLimitSpring value);

		public SoftJointLimitSpring angularYZLimitSpring
		{
			get
			{
				SoftJointLimitSpring softJointLimitSpring;
				this.INTERNAL_get_angularYZLimitSpring(out softJointLimitSpring);
				return softJointLimitSpring;
			}
			set
			{
				this.INTERNAL_set_angularYZLimitSpring(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_angularYZLimitSpring(out SoftJointLimitSpring value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_angularYZLimitSpring(ref SoftJointLimitSpring value);

		public SoftJointLimit linearLimit
		{
			get
			{
				SoftJointLimit softJointLimit;
				this.INTERNAL_get_linearLimit(out softJointLimit);
				return softJointLimit;
			}
			set
			{
				this.INTERNAL_set_linearLimit(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_linearLimit(out SoftJointLimit value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_linearLimit(ref SoftJointLimit value);

		public SoftJointLimit lowAngularXLimit
		{
			get
			{
				SoftJointLimit softJointLimit;
				this.INTERNAL_get_lowAngularXLimit(out softJointLimit);
				return softJointLimit;
			}
			set
			{
				this.INTERNAL_set_lowAngularXLimit(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_lowAngularXLimit(out SoftJointLimit value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_lowAngularXLimit(ref SoftJointLimit value);

		public SoftJointLimit highAngularXLimit
		{
			get
			{
				SoftJointLimit softJointLimit;
				this.INTERNAL_get_highAngularXLimit(out softJointLimit);
				return softJointLimit;
			}
			set
			{
				this.INTERNAL_set_highAngularXLimit(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_highAngularXLimit(out SoftJointLimit value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_highAngularXLimit(ref SoftJointLimit value);

		public SoftJointLimit angularYLimit
		{
			get
			{
				SoftJointLimit softJointLimit;
				this.INTERNAL_get_angularYLimit(out softJointLimit);
				return softJointLimit;
			}
			set
			{
				this.INTERNAL_set_angularYLimit(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_angularYLimit(out SoftJointLimit value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_angularYLimit(ref SoftJointLimit value);

		public SoftJointLimit angularZLimit
		{
			get
			{
				SoftJointLimit softJointLimit;
				this.INTERNAL_get_angularZLimit(out softJointLimit);
				return softJointLimit;
			}
			set
			{
				this.INTERNAL_set_angularZLimit(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_angularZLimit(out SoftJointLimit value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_angularZLimit(ref SoftJointLimit value);

		public Vector3 targetPosition
		{
			get
			{
				Vector3 vector;
				this.INTERNAL_get_targetPosition(out vector);
				return vector;
			}
			set
			{
				this.INTERNAL_set_targetPosition(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_targetPosition(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_targetPosition(ref Vector3 value);

		public Vector3 targetVelocity
		{
			get
			{
				Vector3 vector;
				this.INTERNAL_get_targetVelocity(out vector);
				return vector;
			}
			set
			{
				this.INTERNAL_set_targetVelocity(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_targetVelocity(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_targetVelocity(ref Vector3 value);

		public JointDrive xDrive
		{
			get
			{
				JointDrive jointDrive;
				this.INTERNAL_get_xDrive(out jointDrive);
				return jointDrive;
			}
			set
			{
				this.INTERNAL_set_xDrive(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_xDrive(out JointDrive value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_xDrive(ref JointDrive value);

		public JointDrive yDrive
		{
			get
			{
				JointDrive jointDrive;
				this.INTERNAL_get_yDrive(out jointDrive);
				return jointDrive;
			}
			set
			{
				this.INTERNAL_set_yDrive(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_yDrive(out JointDrive value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_yDrive(ref JointDrive value);

		public JointDrive zDrive
		{
			get
			{
				JointDrive jointDrive;
				this.INTERNAL_get_zDrive(out jointDrive);
				return jointDrive;
			}
			set
			{
				this.INTERNAL_set_zDrive(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_zDrive(out JointDrive value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_zDrive(ref JointDrive value);

		public Quaternion targetRotation
		{
			get
			{
				Quaternion quaternion;
				this.INTERNAL_get_targetRotation(out quaternion);
				return quaternion;
			}
			set
			{
				this.INTERNAL_set_targetRotation(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_targetRotation(out Quaternion value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_targetRotation(ref Quaternion value);

		public Vector3 targetAngularVelocity
		{
			get
			{
				Vector3 vector;
				this.INTERNAL_get_targetAngularVelocity(out vector);
				return vector;
			}
			set
			{
				this.INTERNAL_set_targetAngularVelocity(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_targetAngularVelocity(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_targetAngularVelocity(ref Vector3 value);

		public extern RotationDriveMode rotationDriveMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public JointDrive angularXDrive
		{
			get
			{
				JointDrive jointDrive;
				this.INTERNAL_get_angularXDrive(out jointDrive);
				return jointDrive;
			}
			set
			{
				this.INTERNAL_set_angularXDrive(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_angularXDrive(out JointDrive value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_angularXDrive(ref JointDrive value);

		public JointDrive angularYZDrive
		{
			get
			{
				JointDrive jointDrive;
				this.INTERNAL_get_angularYZDrive(out jointDrive);
				return jointDrive;
			}
			set
			{
				this.INTERNAL_set_angularYZDrive(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_angularYZDrive(out JointDrive value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_angularYZDrive(ref JointDrive value);

		public JointDrive slerpDrive
		{
			get
			{
				JointDrive jointDrive;
				this.INTERNAL_get_slerpDrive(out jointDrive);
				return jointDrive;
			}
			set
			{
				this.INTERNAL_set_slerpDrive(ref value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_slerpDrive(out JointDrive value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_slerpDrive(ref JointDrive value);

		public extern JointProjectionMode projectionMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float projectionDistance
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float projectionAngle
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool configuredInWorldSpace
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool swapBodies
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
