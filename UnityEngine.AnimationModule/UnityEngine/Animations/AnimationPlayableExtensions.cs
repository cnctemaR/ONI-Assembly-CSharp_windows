using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;

namespace UnityEngine.Animations
{
	[NativeHeader("Modules/Animation/AnimationClip.h")]
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[NativeHeader("Modules/Animation/Director/AnimationPlayableExtensions.h")]
	public static class AnimationPlayableExtensions
	{
		public static void SetAnimatedProperties<U>(this U playable, AnimationClip clip) where U : struct, IPlayable
		{
			PlayableHandle handle = playable.GetHandle();
			AnimationPlayableExtensions.SetAnimatedPropertiesInternal(ref handle, clip);
		}

		[NativeThrows]
		internal static void SetAnimatedPropertiesInternal(ref PlayableHandle playable, AnimationClip animatedProperties)
		{
			AnimationPlayableExtensions.SetAnimatedPropertiesInternal_Injected(ref playable, Object.MarshalledUnityObject.Marshal<AnimationClip>(animatedProperties));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAnimatedPropertiesInternal_Injected(ref PlayableHandle playable, IntPtr animatedProperties);
	}
}
