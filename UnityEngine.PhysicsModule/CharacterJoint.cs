using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeClass("Unity::CharacterJoint")]
	[RequireComponent(typeof(Rigidbody))]
	[NativeHeader("Modules/Physics/CharacterJoint.h")]
	public class CharacterJoint : Joint
	{
		public Vector3 swingAxis
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				CharacterJoint.get_swingAxis_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CharacterJoint.set_swingAxis_Injected(intPtr, ref value);
			}
		}

		public SoftJointLimitSpring twistLimitSpring
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SoftJointLimitSpring softJointLimitSpring;
				CharacterJoint.get_twistLimitSpring_Injected(intPtr, out softJointLimitSpring);
				return softJointLimitSpring;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CharacterJoint.set_twistLimitSpring_Injected(intPtr, ref value);
			}
		}

		public SoftJointLimitSpring swingLimitSpring
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SoftJointLimitSpring softJointLimitSpring;
				CharacterJoint.get_swingLimitSpring_Injected(intPtr, out softJointLimitSpring);
				return softJointLimitSpring;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CharacterJoint.set_swingLimitSpring_Injected(intPtr, ref value);
			}
		}

		public SoftJointLimit lowTwistLimit
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SoftJointLimit softJointLimit;
				CharacterJoint.get_lowTwistLimit_Injected(intPtr, out softJointLimit);
				return softJointLimit;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CharacterJoint.set_lowTwistLimit_Injected(intPtr, ref value);
			}
		}

		public SoftJointLimit highTwistLimit
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SoftJointLimit softJointLimit;
				CharacterJoint.get_highTwistLimit_Injected(intPtr, out softJointLimit);
				return softJointLimit;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CharacterJoint.set_highTwistLimit_Injected(intPtr, ref value);
			}
		}

		public SoftJointLimit swing1Limit
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SoftJointLimit softJointLimit;
				CharacterJoint.get_swing1Limit_Injected(intPtr, out softJointLimit);
				return softJointLimit;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CharacterJoint.set_swing1Limit_Injected(intPtr, ref value);
			}
		}

		public SoftJointLimit swing2Limit
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SoftJointLimit softJointLimit;
				CharacterJoint.get_swing2Limit_Injected(intPtr, out softJointLimit);
				return softJointLimit;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CharacterJoint.set_swing2Limit_Injected(intPtr, ref value);
			}
		}

		public bool enableProjection
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CharacterJoint.get_enableProjection_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CharacterJoint.set_enableProjection_Injected(intPtr, value);
			}
		}

		public float projectionDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CharacterJoint.get_projectionDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CharacterJoint.set_projectionDistance_Injected(intPtr, value);
			}
		}

		public float projectionAngle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CharacterJoint.get_projectionAngle_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CharacterJoint>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CharacterJoint.set_projectionAngle_Injected(intPtr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_swingAxis_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_swingAxis_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_twistLimitSpring_Injected(IntPtr _unity_self, out SoftJointLimitSpring ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_twistLimitSpring_Injected(IntPtr _unity_self, [In] ref SoftJointLimitSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_swingLimitSpring_Injected(IntPtr _unity_self, out SoftJointLimitSpring ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_swingLimitSpring_Injected(IntPtr _unity_self, [In] ref SoftJointLimitSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_lowTwistLimit_Injected(IntPtr _unity_self, out SoftJointLimit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_lowTwistLimit_Injected(IntPtr _unity_self, [In] ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_highTwistLimit_Injected(IntPtr _unity_self, out SoftJointLimit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_highTwistLimit_Injected(IntPtr _unity_self, [In] ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_swing1Limit_Injected(IntPtr _unity_self, out SoftJointLimit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_swing1Limit_Injected(IntPtr _unity_self, [In] ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_swing2Limit_Injected(IntPtr _unity_self, out SoftJointLimit ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_swing2Limit_Injected(IntPtr _unity_self, [In] ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enableProjection_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enableProjection_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_projectionDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_projectionDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_projectionAngle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_projectionAngle_Injected(IntPtr _unity_self, float value);
	}
}
