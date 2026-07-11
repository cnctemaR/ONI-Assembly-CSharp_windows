using System;
using UnityEngine.Playables;

namespace UnityEngine.Experimental.Playables
{
	/// <summary>
	///   <para>A PlayableBinding that contains information representing a TexturePlayableOutput.</para>
	/// </summary>
	public static class TexturePlayableBinding
	{
		/// <summary>
		///   <para>Creates a PlayableBinding that contains information representing a TexturePlayableOutput.</para>
		/// </summary>
		/// <param name="key">A reference to a UnityEngine.Object that acts as a key for this binding.</param>
		/// <param name="name">The name of the TexturePlayableOutput.</param>
		/// <returns>
		///   <para>Returns a PlayableBinding that contains information that is used to create a TexturePlayableOutput.</para>
		/// </returns>
		public static PlayableBinding Create(string name, Object key)
		{
			return PlayableBinding.CreateInternal(name, key, typeof(RenderTexture), new PlayableBinding.CreateOutputMethod(TexturePlayableBinding.CreateTextureOutput));
		}

		private static PlayableOutput CreateTextureOutput(PlayableGraph graph, string name)
		{
			return TexturePlayableOutput.Create(graph, name, null);
		}
	}
}
