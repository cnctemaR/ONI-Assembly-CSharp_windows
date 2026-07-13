using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Animation/Motion.h")]
	public class Motion : Object
	{
		protected Motion()
		{
		}

		public float averageDuration
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Motion>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Motion.get_averageDuration_Injected(intPtr);
			}
		}

		public float averageAngularSpeed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Motion>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Motion.get_averageAngularSpeed_Injected(intPtr);
			}
		}

		public Vector3 averageSpeed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Motion>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Motion.get_averageSpeed_Injected(intPtr, out vector);
				return vector;
			}
		}

		public float apparentSpeed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Motion>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Motion.get_apparentSpeed_Injected(intPtr);
			}
		}

		public bool isLooping
		{
			[NativeMethod("IsLooping")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Motion>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Motion.get_isLooping_Injected(intPtr);
			}
		}

		public bool legacy
		{
			[NativeMethod("IsLegacy")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Motion>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Motion.get_legacy_Injected(intPtr);
			}
		}

		public bool isHumanMotion
		{
			[NativeMethod("IsHumanMotion")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Motion>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Motion.get_isHumanMotion_Injected(intPtr);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("ValidateIfRetargetable is not supported anymore, please use isHumanMotion instead.", true)]
		public bool ValidateIfRetargetable(bool val)
		{
			return false;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("isAnimatorMotion is not supported anymore, please use !legacy instead.", true)]
		public bool isAnimatorMotion { get; }

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_averageDuration_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_averageAngularSpeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_averageSpeed_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_apparentSpeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isLooping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_legacy_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isHumanMotion_Injected(IntPtr _unity_self);
	}
}
