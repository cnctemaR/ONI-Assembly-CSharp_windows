using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[HelpURL("AnimatorOverrideController")]
	[NativeHeader("Modules/Animation/ScriptBindings/Animation.bindings.h")]
	[NativeHeader("Modules/Animation/AnimatorOverrideController.h")]
	public class AnimatorOverrideController : RuntimeAnimatorController
	{
		public AnimatorOverrideController()
		{
			AnimatorOverrideController.Internal_Create(this, null);
			this.OnOverrideControllerDirty = null;
		}

		public AnimatorOverrideController(RuntimeAnimatorController controller)
		{
			AnimatorOverrideController.Internal_Create(this, controller);
			this.OnOverrideControllerDirty = null;
		}

		[FreeFunction("AnimationBindings::CreateAnimatorOverrideController")]
		private static void Internal_Create([Writable] AnimatorOverrideController self, RuntimeAnimatorController controller)
		{
			AnimatorOverrideController.Internal_Create_Injected(self, Object.MarshalledUnityObject.Marshal<RuntimeAnimatorController>(controller));
		}

		public RuntimeAnimatorController runtimeAnimatorController
		{
			[NativeMethod("GetAnimatorController")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimatorOverrideController>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<RuntimeAnimatorController>(AnimatorOverrideController.get_runtimeAnimatorController_Injected(intPtr));
			}
			[NativeMethod("SetAnimatorController")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimatorOverrideController>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimatorOverrideController.set_runtimeAnimatorController_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RuntimeAnimatorController>(value));
			}
		}

		public AnimationClip this[string name]
		{
			get
			{
				return this.Internal_GetClipByName(name, true);
			}
			set
			{
				this.Internal_SetClipByName(name, value);
			}
		}

		[NativeMethod("GetClip")]
		private unsafe AnimationClip Internal_GetClipByName(string name, bool returnEffectiveClip)
		{
			AnimationClip animationClip;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimatorOverrideController>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr2 = AnimatorOverrideController.Internal_GetClipByName_Injected(intPtr, ref managedSpanWrapper, returnEffectiveClip);
			}
			finally
			{
				IntPtr intPtr2;
				animationClip = Unmarshal.UnmarshalUnityObject<AnimationClip>(intPtr2);
				char* ptr = null;
			}
			return animationClip;
		}

		[NativeMethod("SetClip")]
		private unsafe void Internal_SetClipByName(string name, AnimationClip clip)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimatorOverrideController>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				AnimatorOverrideController.Internal_SetClipByName_Injected(intPtr, ref managedSpanWrapper, Object.MarshalledUnityObject.Marshal<AnimationClip>(clip));
			}
			finally
			{
				char* ptr = null;
			}
		}

		public AnimationClip this[AnimationClip clip]
		{
			get
			{
				return this.GetClip(clip, true);
			}
			set
			{
				this.SetClip(clip, value, true);
			}
		}

		private AnimationClip GetClip(AnimationClip originalClip, bool returnEffectiveClip)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimatorOverrideController>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<AnimationClip>(AnimatorOverrideController.GetClip_Injected(intPtr, Object.MarshalledUnityObject.Marshal<AnimationClip>(originalClip), returnEffectiveClip));
		}

		private void SetClip(AnimationClip originalClip, AnimationClip overrideClip, bool notify)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimatorOverrideController>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AnimatorOverrideController.SetClip_Injected(intPtr, Object.MarshalledUnityObject.Marshal<AnimationClip>(originalClip), Object.MarshalledUnityObject.Marshal<AnimationClip>(overrideClip), notify);
		}

		private void SendNotification()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimatorOverrideController>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AnimatorOverrideController.SendNotification_Injected(intPtr);
		}

		private AnimationClip GetOriginalClip(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimatorOverrideController>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<AnimationClip>(AnimatorOverrideController.GetOriginalClip_Injected(intPtr, index));
		}

		private AnimationClip GetOverrideClip(AnimationClip originalClip)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimatorOverrideController>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<AnimationClip>(AnimatorOverrideController.GetOverrideClip_Injected(intPtr, Object.MarshalledUnityObject.Marshal<AnimationClip>(originalClip)));
		}

		public int overridesCount
		{
			[NativeMethod("GetOriginalClipsCount")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimatorOverrideController>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimatorOverrideController.get_overridesCount_Injected(intPtr);
			}
		}

		public void GetOverrides(List<KeyValuePair<AnimationClip, AnimationClip>> overrides)
		{
			bool flag = overrides == null;
			if (flag)
			{
				throw new ArgumentNullException("overrides");
			}
			int overridesCount = this.overridesCount;
			bool flag2 = overrides.Capacity < overridesCount;
			if (flag2)
			{
				overrides.Capacity = overridesCount;
			}
			overrides.Clear();
			for (int i = 0; i < overridesCount; i++)
			{
				AnimationClip originalClip = this.GetOriginalClip(i);
				overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(originalClip, this.GetOverrideClip(originalClip)));
			}
		}

		public void ApplyOverrides(IList<KeyValuePair<AnimationClip, AnimationClip>> overrides)
		{
			bool flag = overrides == null;
			if (flag)
			{
				throw new ArgumentNullException("overrides");
			}
			for (int i = 0; i < overrides.Count; i++)
			{
				this.SetClip(overrides[i].Key, overrides[i].Value, false);
			}
			this.SendNotification();
		}

		[Obsolete("AnimatorOverrideController.clips property is deprecated. Use AnimatorOverrideController.GetOverrides and AnimatorOverrideController.ApplyOverrides instead.")]
		public AnimationClipPair[] clips
		{
			get
			{
				int overridesCount = this.overridesCount;
				AnimationClipPair[] array = new AnimationClipPair[overridesCount];
				for (int i = 0; i < overridesCount; i++)
				{
					array[i] = new AnimationClipPair();
					array[i].originalClip = this.GetOriginalClip(i);
					array[i].overrideClip = this.GetOverrideClip(array[i].originalClip);
				}
				return array;
			}
			set
			{
				for (int i = 0; i < value.Length; i++)
				{
					this.SetClip(value[i].originalClip, value[i].overrideClip, false);
				}
				this.SendNotification();
			}
		}

		[NativeConditional("UNITY_EDITOR")]
		internal void PerformOverrideClipListCleanup()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimatorOverrideController>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AnimatorOverrideController.PerformOverrideClipListCleanup_Injected(intPtr);
		}

		[NativeConditional("UNITY_EDITOR")]
		[RequiredByNativeCode]
		internal static void OnInvalidateOverrideController(AnimatorOverrideController controller)
		{
			bool flag = controller.OnOverrideControllerDirty != null;
			if (flag)
			{
				controller.OnOverrideControllerDirty();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create_Injected([Writable] AnimatorOverrideController self, IntPtr controller);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_runtimeAnimatorController_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_runtimeAnimatorController_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_GetClipByName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, bool returnEffectiveClip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetClipByName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, IntPtr clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetClip_Injected(IntPtr _unity_self, IntPtr originalClip, bool returnEffectiveClip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetClip_Injected(IntPtr _unity_self, IntPtr originalClip, IntPtr overrideClip, bool notify);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendNotification_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetOriginalClip_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetOverrideClip_Injected(IntPtr _unity_self, IntPtr originalClip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_overridesCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PerformOverrideClipListCleanup_Injected(IntPtr _unity_self);

		internal AnimatorOverrideController.OnOverrideControllerDirtyCallback OnOverrideControllerDirty;

		internal delegate void OnOverrideControllerDirtyCallback();
	}
}
