using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	/// <summary>
	///   <para>A IPlayableOutput implementation that connects the PlayableGraph to an Animator in the scene.</para>
	/// </summary>
	[NativeHeader("Runtime/Animation/ScriptBindings/AnimationPlayableOutput.bindings.h")]
	[NativeHeader("Runtime/Animation/Director/AnimationPlayableOutput.h")]
	[NativeHeader("Runtime/Animation/Animator.h")]
	[NativeHeader("Runtime/Director/Core/HPlayableGraph.h")]
	[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
	[StaticAccessor("AnimationPlayableOutputBindings", StaticAccessorType.DoubleColon)]
	[RequiredByNativeCode]
	public struct AnimationPlayableOutput : IPlayableOutput
	{
		internal AnimationPlayableOutput(PlayableOutputHandle handle)
		{
			if (handle.IsValid())
			{
				if (!handle.IsPlayableOutputOfType<AnimationPlayableOutput>())
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationPlayableOutput.");
				}
			}
			this.m_Handle = handle;
		}

		/// <summary>
		///   <para>Creates an AnimationPlayableOutput in the PlayableGraph.</para>
		/// </summary>
		/// <param name="graph">The PlayableGraph that will contain the AnimationPlayableOutput.</param>
		/// <param name="name">The name of the output.</param>
		/// <param name="target">The Animator that will process the PlayableGraph.</param>
		/// <returns>
		///   <para>A new AnimationPlayableOutput attached to the PlayableGraph.</para>
		/// </returns>
		public static AnimationPlayableOutput Create(PlayableGraph graph, string name, Animator target)
		{
			PlayableOutputHandle playableOutputHandle;
			AnimationPlayableOutput animationPlayableOutput;
			if (!AnimationPlayableGraphExtensions.InternalCreateAnimationOutput(ref graph, name, out playableOutputHandle))
			{
				animationPlayableOutput = AnimationPlayableOutput.Null;
			}
			else
			{
				AnimationPlayableOutput animationPlayableOutput2 = new AnimationPlayableOutput(playableOutputHandle);
				animationPlayableOutput2.SetTarget(target);
				animationPlayableOutput = animationPlayableOutput2;
			}
			return animationPlayableOutput;
		}

		public static AnimationPlayableOutput Null
		{
			get
			{
				return new AnimationPlayableOutput(PlayableOutputHandle.Null);
			}
		}

		public PlayableOutputHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator PlayableOutput(AnimationPlayableOutput output)
		{
			return new PlayableOutput(output.GetHandle());
		}

		public static explicit operator AnimationPlayableOutput(PlayableOutput output)
		{
			return new AnimationPlayableOutput(output.GetHandle());
		}

		/// <summary>
		///   <para>Returns the Animator that plays the animation graph.</para>
		/// </summary>
		/// <returns>
		///   <para>The targeted Animator.</para>
		/// </returns>
		public Animator GetTarget()
		{
			return AnimationPlayableOutput.InternalGetTarget(ref this.m_Handle);
		}

		/// <summary>
		///   <para>Sets the Animator that plays the animation graph.</para>
		/// </summary>
		/// <param name="value">The targeted Animator.</param>
		public void SetTarget(Animator value)
		{
			AnimationPlayableOutput.InternalSetTarget(ref this.m_Handle, value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Animator InternalGetTarget(ref PlayableOutputHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetTarget(ref PlayableOutputHandle handle, Animator target);

		private PlayableOutputHandle m_Handle;
	}
}
