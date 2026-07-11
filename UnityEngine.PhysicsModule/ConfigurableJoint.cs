using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Dynamics/ConfigurableJoint.h")]
	[NativeClass("Unity::ConfigurableJoint")]
	public class ConfigurableJoint : Joint
	{
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

		public extern ConfigurableJointMotion xMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion yMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion zMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion angularXMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion angularYMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ConfigurableJointMotion angularZMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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

		public extern RotationDriveMode rotationDriveMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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

		public extern JointProjectionMode projectionMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float projectionDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float projectionAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool configuredInWorldSpace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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
