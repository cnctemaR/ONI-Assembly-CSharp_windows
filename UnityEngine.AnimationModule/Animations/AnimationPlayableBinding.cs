using System;
using UnityEngine.Playables;

namespace UnityEngine.Animations
{
	/// <summary>
	///   <para>A PlayableBinding that contains information representing an AnimationPlayableOutput.</para>
	/// </summary>
	public static class AnimationPlayableBinding
	{
		/// <summary>
		///   <para>Creates a PlayableBinding that contains information representing an AnimationPlayableOutput.</para>
		/// </summary>
		/// <param name="name">The name of the AnimationPlayableOutput.</param>
		/// <param name="key">A reference to a UnityEngine.Object that acts as a key for this binding.</param>
		/// <returns>
		///   <para>Returns a PlayableBinding that contains information that is used to create an AnimationPlayableOutput.</para>
		/// </returns>
		public static PlayableBinding Create(string name, Object key)
		{
			return PlayableBinding.CreateInternal(name, key, typeof(Animator), new PlayableBinding.CreateOutputMethod(AnimationPlayableBinding.CreateAnimationOutput));
		}

		private static PlayableOutput CreateAnimationOutput(PlayableGraph graph, string name)
		{
			return AnimationPlayableOutput.Create(graph, name, null);
		}
	}
}
