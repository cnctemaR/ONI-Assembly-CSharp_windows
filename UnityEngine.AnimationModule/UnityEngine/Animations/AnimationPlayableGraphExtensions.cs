using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;

namespace UnityEngine.Animations
{
	[NativeHeader("Modules/Animation/Animator.h")]
	[NativeHeader("Modules/Animation/ScriptBindings/AnimationPlayableGraphExtensions.bindings.h")]
	[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[StaticAccessor("AnimationPlayableGraphExtensionsBindings", StaticAccessorType.DoubleColon)]
	internal static class AnimationPlayableGraphExtensions
	{
		internal static void SyncUpdateAndTimeMode(this PlayableGraph graph, Animator animator)
		{
			AnimationPlayableGraphExtensions.InternalSyncUpdateAndTimeMode(ref graph, animator);
		}

		internal static void DestroyOutput(this PlayableGraph graph, PlayableOutputHandle handle)
		{
			AnimationPlayableGraphExtensions.InternalDestroyOutput(ref graph, ref handle);
		}

		[NativeThrows]
		internal unsafe static bool InternalCreateAnimationOutput(ref PlayableGraph graph, string name, out PlayableOutputHandle handle)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = AnimationPlayableGraphExtensions.InternalCreateAnimationOutput_Injected(ref graph, ref managedSpanWrapper, out handle);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[NativeThrows]
		internal static void InternalSyncUpdateAndTimeMode(ref PlayableGraph graph, [NotNull] Animator animator)
		{
			if (animator == null)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(animator);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			AnimationPlayableGraphExtensions.InternalSyncUpdateAndTimeMode_Injected(ref graph, intPtr);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalDestroyOutput(ref PlayableGraph graph, ref PlayableOutputHandle handle);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int InternalAnimationOutputCount(ref PlayableGraph graph);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalGetAnimationOutput(ref PlayableGraph graph, int index, out PlayableOutputHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalCreateAnimationOutput_Injected(ref PlayableGraph graph, ref ManagedSpanWrapper name, out PlayableOutputHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSyncUpdateAndTimeMode_Injected(ref PlayableGraph graph, IntPtr animator);
	}
}
