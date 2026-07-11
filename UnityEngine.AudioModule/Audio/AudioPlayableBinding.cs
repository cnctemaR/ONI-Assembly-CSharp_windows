using System;
using UnityEngine.Playables;

namespace UnityEngine.Audio
{
	/// <summary>
	///   <para>A PlayableBinding that contains information representing an AudioPlayableOutput.</para>
	/// </summary>
	public static class AudioPlayableBinding
	{
		/// <summary>
		///   <para>Creates a PlayableBinding that contains information representing an AudioPlayableOutput.</para>
		/// </summary>
		/// <param name="key">A reference to a UnityEngine.Object that acts as a key for this binding.</param>
		/// <param name="name">The name of the AudioPlayableOutput.</param>
		/// <returns>
		///   <para>Returns a PlayableBinding that contains information that is used to create an AudioPlayableOutput.</para>
		/// </returns>
		public static PlayableBinding Create(string name, Object key)
		{
			return PlayableBinding.CreateInternal(name, key, typeof(AudioSource), new PlayableBinding.CreateOutputMethod(AudioPlayableBinding.CreateAudioOutput));
		}

		private static PlayableOutput CreateAudioOutput(PlayableGraph graph, string name)
		{
			return AudioPlayableOutput.Create(graph, name, null);
		}
	}
}
