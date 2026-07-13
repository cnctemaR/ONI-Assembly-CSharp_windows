using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[RequireComponent(typeof(Transform), typeof(Rigidbody2D))]
	[NativeHeader("Modules/Physics2D/Joint2D.h")]
	public class Joint2D : Behaviour
	{
		public Rigidbody2D attachedRigidbody
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Rigidbody2D>(Joint2D.get_attachedRigidbody_Injected(intPtr));
			}
		}

		public Rigidbody2D connectedBody
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Rigidbody2D>(Joint2D.get_connectedBody_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Joint2D.set_connectedBody_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Rigidbody2D>(value));
			}
		}

		public bool enableCollision
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Joint2D.get_enableCollision_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Joint2D.set_enableCollision_Injected(intPtr, value);
			}
		}

		public float breakForce
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Joint2D.get_breakForce_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Joint2D.set_breakForce_Injected(intPtr, value);
			}
		}

		public float breakTorque
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Joint2D.get_breakTorque_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Joint2D.set_breakTorque_Injected(intPtr, value);
			}
		}

		public JointBreakAction2D breakAction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Joint2D.get_breakAction_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Joint2D.set_breakAction_Injected(intPtr, value);
			}
		}

		public Vector2 reactionForce
		{
			[NativeMethod("GetReactionForceFixedTime")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Joint2D.get_reactionForce_Injected(intPtr, out vector);
				return vector;
			}
		}

		public float reactionTorque
		{
			[NativeMethod("GetReactionTorqueFixedTime")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Joint2D.get_reactionTorque_Injected(intPtr);
			}
		}

		public Vector2 GetReactionForce(float timeStep)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 vector;
			Joint2D.GetReactionForce_Injected(intPtr, timeStep, out vector);
			return vector;
		}

		public float GetReactionTorque(float timeStep)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Joint2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Joint2D.GetReactionTorque_Injected(intPtr, timeStep);
		}

		[Obsolete("Joint2D.collideConnected has been obsolete. Use Joint2D.enableCollision instead (UnityUpgradable) -> enableCollision", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool collideConnected
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_attachedRigidbody_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_connectedBody_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_connectedBody_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enableCollision_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enableCollision_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_breakForce_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_breakForce_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_breakTorque_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_breakTorque_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern JointBreakAction2D get_breakAction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_breakAction_Injected(IntPtr _unity_self, JointBreakAction2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_reactionForce_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_reactionTorque_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetReactionForce_Injected(IntPtr _unity_self, float timeStep, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetReactionTorque_Injected(IntPtr _unity_self, float timeStep);
	}
}
