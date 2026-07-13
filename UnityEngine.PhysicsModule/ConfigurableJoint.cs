using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[RequireComponent(typeof(Rigidbody))]
	[NativeHeader("Modules/Physics/ConfigurableJoint.h")]
	[NativeClass("Unity::ConfigurableJoint")]
	public class ConfigurableJoint : Joint
	{
		public Vector3 secondaryAxis
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ConfigurableJoint.get_secondaryAxis_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_secondaryAxis_Injected(intPtr, ref value);
			}
		}

		public ConfigurableJointMotion xMotion
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ConfigurableJoint.get_xMotion_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_xMotion_Injected(intPtr, value);
			}
		}

		public ConfigurableJointMotion yMotion
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ConfigurableJoint.get_yMotion_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_yMotion_Injected(intPtr, value);
			}
		}

		public ConfigurableJointMotion zMotion
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ConfigurableJoint.get_zMotion_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_zMotion_Injected(intPtr, value);
			}
		}

		public ConfigurableJointMotion angularXMotion
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ConfigurableJoint.get_angularXMotion_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_angularXMotion_Injected(intPtr, value);
			}
		}

		public ConfigurableJointMotion angularYMotion
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ConfigurableJoint.get_angularYMotion_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_angularYMotion_Injected(intPtr, value);
			}
		}

		public ConfigurableJointMotion angularZMotion
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ConfigurableJoint.get_angularZMotion_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_angularZMotion_Injected(intPtr, value);
			}
		}

		public SoftJointLimitSpring linearLimitSpring
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SoftJointLimitSpring softJointLimitSpring;
				ConfigurableJoint.get_linearLimitSpring_Injected(intPtr, out softJointLimitSpring);
				return softJointLimitSpring;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_linearLimitSpring_Injected(intPtr, ref value);
			}
		}

		public SoftJointLimitSpring angularXLimitSpring
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SoftJointLimitSpring softJointLimitSpring;
				ConfigurableJoint.get_angularXLimitSpring_Injected(intPtr, out softJointLimitSpring);
				return softJointLimitSpring;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_angularXLimitSpring_Injected(intPtr, ref value);
			}
		}

		public SoftJointLimitSpring angularYZLimitSpring
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SoftJointLimitSpring softJointLimitSpring;
				ConfigurableJoint.get_angularYZLimitSpring_Injected(intPtr, out softJointLimitSpring);
				return softJointLimitSpring;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_angularYZLimitSpring_Injected(intPtr, ref value);
			}
		}

		public SoftJointLimit linearLimit
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SoftJointLimit softJointLimit;
				ConfigurableJoint.get_linearLimit_Injected(intPtr, out softJointLimit);
				return softJointLimit;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_linearLimit_Injected(intPtr, ref value);
			}
		}

		public SoftJointLimit lowAngularXLimit
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SoftJointLimit softJointLimit;
				ConfigurableJoint.get_lowAngularXLimit_Injected(intPtr, out softJointLimit);
				return softJointLimit;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_lowAngularXLimit_Injected(intPtr, ref value);
			}
		}

		public SoftJointLimit highAngularXLimit
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SoftJointLimit softJointLimit;
				ConfigurableJoint.get_highAngularXLimit_Injected(intPtr, out softJointLimit);
				return softJointLimit;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_highAngularXLimit_Injected(intPtr, ref value);
			}
		}

		public SoftJointLimit angularYLimit
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SoftJointLimit softJointLimit;
				ConfigurableJoint.get_angularYLimit_Injected(intPtr, out softJointLimit);
				return softJointLimit;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_angularYLimit_Injected(intPtr, ref value);
			}
		}

		public SoftJointLimit angularZLimit
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SoftJointLimit softJointLimit;
				ConfigurableJoint.get_angularZLimit_Injected(intPtr, out softJointLimit);
				return softJointLimit;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_angularZLimit_Injected(intPtr, ref value);
			}
		}

		public Vector3 targetPosition
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ConfigurableJoint.get_targetPosition_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_targetPosition_Injected(intPtr, ref value);
			}
		}

		public Vector3 targetVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ConfigurableJoint.get_targetVelocity_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_targetVelocity_Injected(intPtr, ref value);
			}
		}

		public JointDrive xDrive
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointDrive jointDrive;
				ConfigurableJoint.get_xDrive_Injected(intPtr, out jointDrive);
				return jointDrive;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_xDrive_Injected(intPtr, ref value);
			}
		}

		public JointDrive yDrive
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointDrive jointDrive;
				ConfigurableJoint.get_yDrive_Injected(intPtr, out jointDrive);
				return jointDrive;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_yDrive_Injected(intPtr, ref value);
			}
		}

		public JointDrive zDrive
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointDrive jointDrive;
				ConfigurableJoint.get_zDrive_Injected(intPtr, out jointDrive);
				return jointDrive;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_zDrive_Injected(intPtr, ref value);
			}
		}

		public Quaternion targetRotation
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Quaternion quaternion;
				ConfigurableJoint.get_targetRotation_Injected(intPtr, out quaternion);
				return quaternion;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_targetRotation_Injected(intPtr, ref value);
			}
		}

		public Vector3 targetAngularVelocity
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ConfigurableJoint.get_targetAngularVelocity_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_targetAngularVelocity_Injected(intPtr, ref value);
			}
		}

		public RotationDriveMode rotationDriveMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ConfigurableJoint.get_rotationDriveMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_rotationDriveMode_Injected(intPtr, value);
			}
		}

		public JointDrive angularXDrive
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointDrive jointDrive;
				ConfigurableJoint.get_angularXDrive_Injected(intPtr, out jointDrive);
				return jointDrive;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_angularXDrive_Injected(intPtr, ref value);
			}
		}

		public JointDrive angularYZDrive
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointDrive jointDrive;
				ConfigurableJoint.get_angularYZDrive_Injected(intPtr, out jointDrive);
				return jointDrive;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_angularYZDrive_Injected(intPtr, ref value);
			}
		}

		public JointDrive slerpDrive
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				JointDrive jointDrive;
				ConfigurableJoint.get_slerpDrive_Injected(intPtr, out jointDrive);
				return jointDrive;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_slerpDrive_Injected(intPtr, ref value);
			}
		}

		public JointProjectionMode projectionMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ConfigurableJoint.get_projectionMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_projectionMode_Injected(intPtr, value);
			}
		}

		public float projectionDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ConfigurableJoint.get_projectionDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_projectionDistance_Injected(intPtr, value);
			}
		}

		public float projectionAngle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ConfigurableJoint.get_projectionAngle_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_projectionAngle_Injected(intPtr, value);
			}
		}

		public bool configuredInWorldSpace
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ConfigurableJoint.get_configuredInWorldSpace_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_configuredInWorldSpace_Injected(intPtr, value);
			}
		}

		public bool swapBodies
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ConfigurableJoint.get_swapBodies_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ConfigurableJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ConfigurableJoint.set_swapBodies_Injected(intPtr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_secondaryAxis_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_secondaryAxis_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ConfigurableJointMotion get_xMotion_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_xMotion_Injected(IntPtr _unity_self, ConfigurableJointMotion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ConfigurableJointMotion get_yMotion_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_yMotion_Injected(IntPtr _unity_self, ConfigurableJointMotion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ConfigurableJointMotion get_zMotion_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_zMotion_Injected(IntPtr _unity_self, ConfigurableJointMotion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ConfigurableJointMotion get_angularXMotion_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularXMotion_Injected(IntPtr _unity_self, ConfigurableJointMotion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ConfigurableJointMotion get_angularYMotion_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularYMotion_Injected(IntPtr _unity_self, ConfigurableJointMotion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ConfigurableJointMotion get_angularZMotion_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularZMotion_Injected(IntPtr _unity_self, ConfigurableJointMotion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_linearLimitSpring_Injected(IntPtr _unity_self, out SoftJointLimitSpring ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearLimitSpring_Injected(IntPtr _unity_self, [In] ref SoftJointLimitSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_angularXLimitSpring_Injected(IntPtr _unity_self, out SoftJointLimitSpring ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularXLimitSpring_Injected(IntPtr _unity_self, [In] ref SoftJointLimitSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_angularYZLimitSpring_Injected(IntPtr _unity_self, out SoftJointLimitSpring ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularYZLimitSpring_Injected(IntPtr _unity_self, [In] ref SoftJointLimitSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_linearLimit_Injected(IntPtr _unity_self, out SoftJointLimit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_linearLimit_Injected(IntPtr _unity_self, [In] ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_lowAngularXLimit_Injected(IntPtr _unity_self, out SoftJointLimit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_lowAngularXLimit_Injected(IntPtr _unity_self, [In] ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_highAngularXLimit_Injected(IntPtr _unity_self, out SoftJointLimit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_highAngularXLimit_Injected(IntPtr _unity_self, [In] ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_angularYLimit_Injected(IntPtr _unity_self, out SoftJointLimit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularYLimit_Injected(IntPtr _unity_self, [In] ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_angularZLimit_Injected(IntPtr _unity_self, out SoftJointLimit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularZLimit_Injected(IntPtr _unity_self, [In] ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_targetPosition_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_targetPosition_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_targetVelocity_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_targetVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_xDrive_Injected(IntPtr _unity_self, out JointDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_xDrive_Injected(IntPtr _unity_self, [In] ref JointDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_yDrive_Injected(IntPtr _unity_self, out JointDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_yDrive_Injected(IntPtr _unity_self, [In] ref JointDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_zDrive_Injected(IntPtr _unity_self, out JointDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_zDrive_Injected(IntPtr _unity_self, [In] ref JointDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_targetRotation_Injected(IntPtr _unity_self, out Quaternion ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_targetRotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_targetAngularVelocity_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_targetAngularVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RotationDriveMode get_rotationDriveMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationDriveMode_Injected(IntPtr _unity_self, RotationDriveMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_angularXDrive_Injected(IntPtr _unity_self, out JointDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularXDrive_Injected(IntPtr _unity_self, [In] ref JointDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_angularYZDrive_Injected(IntPtr _unity_self, out JointDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_angularYZDrive_Injected(IntPtr _unity_self, [In] ref JointDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_slerpDrive_Injected(IntPtr _unity_self, out JointDrive ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_slerpDrive_Injected(IntPtr _unity_self, [In] ref JointDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern JointProjectionMode get_projectionMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_projectionMode_Injected(IntPtr _unity_self, JointProjectionMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_projectionDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_projectionDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_projectionAngle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_projectionAngle_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_configuredInWorldSpace_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_configuredInWorldSpace_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_swapBodies_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_swapBodies_Injected(IntPtr _unity_self, bool value);
	}
}
