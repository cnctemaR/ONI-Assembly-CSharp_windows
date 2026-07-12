using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
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
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] AnimatorOverrideController self, RuntimeAnimatorController controller);

		public extern RuntimeAnimatorController runtimeAnimatorController
		{
			[NativeMethod("GetAnimatorController")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetAnimatorController")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
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
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AnimationClip Internal_GetClipByName(string name, bool returnEffectiveClip);

		[NativeMethod("SetClip")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_SetClipByName(string name, AnimationClip clip);

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AnimationClip GetClip(AnimationClip originalClip, bool returnEffectiveClip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetClip(AnimationClip originalClip, AnimationClip overrideClip, bool notify);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SendNotification();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AnimationClip GetOriginalClip(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AnimationClip GetOverrideClip(AnimationClip originalClip);

		public extern int overridesCount
		{
			[NativeMethod("GetOriginalClipsCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
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
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void PerformOverrideClipListCleanup();

		[RequiredByNativeCode]
		[NativeConditional("UNITY_EDITOR")]
		internal static void OnInvalidateOverrideController(AnimatorOverrideController controller)
		{
			bool flag = controller.OnOverrideControllerDirty != null;
			if (flag)
			{
				controller.OnOverrideControllerDirty();
			}
		}

		internal AnimatorOverrideController.OnOverrideControllerDirtyCallback OnOverrideControllerDirty;

		internal delegate void OnOverrideControllerDirtyCallback();
	}
}
