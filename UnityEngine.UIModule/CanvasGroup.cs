using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/UI/CanvasGroup.h")]
	[NativeClass("UI::CanvasGroup")]
	public sealed class CanvasGroup : Behaviour, ICanvasRaycastFilter
	{
		[NativeProperty("Alpha", false, TargetType.Function)]
		public float alpha
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CanvasGroup.get_alpha_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CanvasGroup.set_alpha_Injected(intPtr, value);
			}
		}

		[NativeProperty("Interactable", false, TargetType.Function)]
		public bool interactable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CanvasGroup.get_interactable_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CanvasGroup.set_interactable_Injected(intPtr, value);
			}
		}

		[NativeProperty("BlocksRaycasts", false, TargetType.Function)]
		public bool blocksRaycasts
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CanvasGroup.get_blocksRaycasts_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CanvasGroup.set_blocksRaycasts_Injected(intPtr, value);
			}
		}

		[NativeProperty("IgnoreParentGroups", false, TargetType.Function)]
		public bool ignoreParentGroups
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CanvasGroup.get_ignoreParentGroups_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CanvasGroup.set_ignoreParentGroups_Injected(intPtr, value);
			}
		}

		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return this.blocksRaycasts;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_alpha_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_alpha_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_interactable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_interactable_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_blocksRaycasts_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_blocksRaycasts_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_ignoreParentGroups_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_ignoreParentGroups_Injected(IntPtr _unity_self, bool value);
	}
}
