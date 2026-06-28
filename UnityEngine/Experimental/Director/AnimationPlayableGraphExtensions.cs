using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	public static class AnimationPlayableGraphExtensions
	{
		public static AnimationPlayableOutput CreateAnimationOutput(this PlayableGraph graph, string name, Animator target)
		{
			AnimationPlayableOutput animationPlayableOutput = default(AnimationPlayableOutput);
			AnimationPlayableOutput animationPlayableOutput2;
			if (!AnimationPlayableGraphExtensions.InternalCreateAnimationOutput(ref graph, name, out animationPlayableOutput.m_Output))
			{
				animationPlayableOutput2 = AnimationPlayableOutput.Null;
			}
			else
			{
				animationPlayableOutput.target = target;
				animationPlayableOutput2 = animationPlayableOutput;
			}
			return animationPlayableOutput2;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalCreateAnimationOutput(ref PlayableGraph graph, string name, out PlayableOutput output);

		internal static void SyncUpdateAndTimeMode(this PlayableGraph graph, Animator animator)
		{
			AnimationPlayableGraphExtensions.InternalSyncUpdateAndTimeMode(ref graph, animator);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalSyncUpdateAndTimeMode(ref PlayableGraph graph, Animator animator);

		public static PlayableHandle CreateAnimationClipPlayable(this PlayableGraph graph, AnimationClip clip)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle playableHandle;
			if (!AnimationPlayableGraphExtensions.InternalCreateAnimationClipPlayable(ref graph, clip, ref @null))
			{
				playableHandle = PlayableHandle.Null;
			}
			else
			{
				playableHandle = @null;
			}
			return playableHandle;
		}

		private static bool InternalCreateAnimationClipPlayable(ref PlayableGraph graph, AnimationClip clip, ref PlayableHandle handle)
		{
			return AnimationPlayableGraphExtensions.INTERNAL_CALL_InternalCreateAnimationClipPlayable(ref graph, clip, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_InternalCreateAnimationClipPlayable(ref PlayableGraph graph, AnimationClip clip, ref PlayableHandle handle);

		[ExcludeFromDocs]
		public static PlayableHandle CreateAnimationMixerPlayable(this PlayableGraph graph, int inputCount)
		{
			bool flag = false;
			return graph.CreateAnimationMixerPlayable(inputCount, flag);
		}

		[ExcludeFromDocs]
		public static PlayableHandle CreateAnimationMixerPlayable(this PlayableGraph graph)
		{
			bool flag = false;
			int num = 0;
			return graph.CreateAnimationMixerPlayable(num, flag);
		}

		public static PlayableHandle CreateAnimationMixerPlayable(this PlayableGraph graph, [DefaultValue("0")] int inputCount, [DefaultValue("false")] bool normalizeWeights)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle playableHandle;
			if (!AnimationPlayableGraphExtensions.InternalCreateAnimationMixerPlayable(ref graph, inputCount, normalizeWeights, ref @null))
			{
				playableHandle = PlayableHandle.Null;
			}
			else
			{
				@null.inputCount = inputCount;
				playableHandle = @null;
			}
			return playableHandle;
		}

		private static bool InternalCreateAnimationMixerPlayable(ref PlayableGraph graph, int inputCount, bool normalizeWeights, ref PlayableHandle handle)
		{
			return AnimationPlayableGraphExtensions.INTERNAL_CALL_InternalCreateAnimationMixerPlayable(ref graph, inputCount, normalizeWeights, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_InternalCreateAnimationMixerPlayable(ref PlayableGraph graph, int inputCount, bool normalizeWeights, ref PlayableHandle handle);

		public static PlayableHandle CreateAnimatorControllerPlayable(this PlayableGraph graph, RuntimeAnimatorController controller)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle playableHandle;
			if (!AnimationPlayableGraphExtensions.InternalCreateAnimatorControllerPlayable(ref graph, controller, ref @null))
			{
				playableHandle = PlayableHandle.Null;
			}
			else
			{
				playableHandle = @null;
			}
			return playableHandle;
		}

		private static bool InternalCreateAnimatorControllerPlayable(ref PlayableGraph graph, RuntimeAnimatorController controller, ref PlayableHandle handle)
		{
			return AnimationPlayableGraphExtensions.INTERNAL_CALL_InternalCreateAnimatorControllerPlayable(ref graph, controller, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_InternalCreateAnimatorControllerPlayable(ref PlayableGraph graph, RuntimeAnimatorController controller, ref PlayableHandle handle);

		internal static PlayableHandle CreateAnimationOffsetPlayable(this PlayableGraph graph, Vector3 position, Quaternion rotation, int inputCount)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle playableHandle;
			if (!AnimationPlayableGraphExtensions.InternalCreateAnimationOffsetPlayable(ref graph, position, rotation, ref @null))
			{
				playableHandle = PlayableHandle.Null;
			}
			else
			{
				@null.inputCount = inputCount;
				playableHandle = @null;
			}
			return playableHandle;
		}

		private static bool InternalCreateAnimationOffsetPlayable(ref PlayableGraph graph, Vector3 position, Quaternion rotation, ref PlayableHandle handle)
		{
			return AnimationPlayableGraphExtensions.INTERNAL_CALL_InternalCreateAnimationOffsetPlayable(ref graph, ref position, ref rotation, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_InternalCreateAnimationOffsetPlayable(ref PlayableGraph graph, ref Vector3 position, ref Quaternion rotation, ref PlayableHandle handle);

		internal static PlayableHandle CreateAnimationMotionXToDeltaPlayable(this PlayableGraph graph)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle playableHandle;
			if (!AnimationPlayableGraphExtensions.InternalCreateAnimationMotionXToDeltaPlayable(ref graph, ref @null))
			{
				playableHandle = PlayableHandle.Null;
			}
			else
			{
				@null.inputCount = 1;
				playableHandle = @null;
			}
			return playableHandle;
		}

		private static bool InternalCreateAnimationMotionXToDeltaPlayable(ref PlayableGraph graph, ref PlayableHandle handle)
		{
			return AnimationPlayableGraphExtensions.INTERNAL_CALL_InternalCreateAnimationMotionXToDeltaPlayable(ref graph, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_InternalCreateAnimationMotionXToDeltaPlayable(ref PlayableGraph graph, ref PlayableHandle handle);

		[ExcludeFromDocs]
		internal static PlayableHandle CreateAnimationLayerMixerPlayable(this PlayableGraph graph)
		{
			int num = 0;
			return graph.CreateAnimationLayerMixerPlayable(num);
		}

		internal static PlayableHandle CreateAnimationLayerMixerPlayable(this PlayableGraph graph, [DefaultValue("0")] int inputCount)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle playableHandle;
			if (!AnimationPlayableGraphExtensions.InternalCreateAnimationLayerMixerPlayable(ref graph, ref @null))
			{
				playableHandle = PlayableHandle.Null;
			}
			else
			{
				@null.inputCount = inputCount;
				playableHandle = @null;
			}
			return playableHandle;
		}

		private static bool InternalCreateAnimationLayerMixerPlayable(ref PlayableGraph graph, ref PlayableHandle handle)
		{
			return AnimationPlayableGraphExtensions.INTERNAL_CALL_InternalCreateAnimationLayerMixerPlayable(ref graph, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_InternalCreateAnimationLayerMixerPlayable(ref PlayableGraph graph, ref PlayableHandle handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalDestroyOutput(ref PlayableGraph graph, ref PlayableOutput output);

		public static void DestroyOutput(this PlayableGraph graph, AnimationPlayableOutput output)
		{
			AnimationPlayableGraphExtensions.InternalDestroyOutput(ref graph, ref output.m_Output);
		}

		public static int GetAnimationOutputCount(this PlayableGraph graph)
		{
			return AnimationPlayableGraphExtensions.InternalAnimationOutputCount(ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int InternalAnimationOutputCount(ref PlayableGraph graph);

		public static AnimationPlayableOutput GetAnimationOutput(this PlayableGraph graph, int index)
		{
			AnimationPlayableOutput animationPlayableOutput = default(AnimationPlayableOutput);
			AnimationPlayableOutput animationPlayableOutput2;
			if (!AnimationPlayableGraphExtensions.InternalGetAnimationOutput(ref graph, index, out animationPlayableOutput.m_Output))
			{
				animationPlayableOutput2 = AnimationPlayableOutput.Null;
			}
			else
			{
				animationPlayableOutput2 = animationPlayableOutput;
			}
			return animationPlayableOutput2;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalGetAnimationOutput(ref PlayableGraph graph, int index, out PlayableOutput output);
	}
}
