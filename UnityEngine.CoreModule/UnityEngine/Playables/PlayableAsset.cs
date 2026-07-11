using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	/// <summary>
	///   <para>An base class for assets that can be used to instatiate a Playable at runtime.</para>
	/// </summary>
	[RequiredByNativeCode]
	[AssetFileNameExtension("playable", new string[] { })]
	[Serializable]
	public abstract class PlayableAsset : ScriptableObject, IPlayableAsset
	{
		/// <summary>
		///   <para>Implement this method to have your asset inject playables into the given graph.</para>
		/// </summary>
		/// <param name="graph">The graph to inject playables into.</param>
		/// <param name="owner">The game object which initiated the build.</param>
		/// <returns>
		///   <para>The playable injected into the graph, or the root playable if multiple playables are injected.</para>
		/// </returns>
		public abstract Playable CreatePlayable(PlayableGraph graph, GameObject owner);

		/// <summary>
		///   <para>The playback duration in seconds of the instantiated Playable.</para>
		/// </summary>
		public virtual double duration
		{
			get
			{
				return PlayableBinding.DefaultDuration;
			}
		}

		/// <summary>
		///   <para>A description of the outputs of the instantiated Playable.</para>
		/// </summary>
		public virtual IEnumerable<PlayableBinding> outputs
		{
			get
			{
				return PlayableBinding.None;
			}
		}

		[RequiredByNativeCode]
		internal unsafe static void Internal_CreatePlayable(PlayableAsset asset, PlayableGraph graph, GameObject go, IntPtr ptr)
		{
			Playable playable;
			if (asset == null)
			{
				playable = Playable.Null;
			}
			else
			{
				playable = asset.CreatePlayable(graph, go);
			}
			Playable* ptr2 = (Playable*)ptr.ToPointer();
			*ptr2 = playable;
		}

		[RequiredByNativeCode]
		internal unsafe static void Internal_GetPlayableAssetDuration(PlayableAsset asset, IntPtr ptrToDouble)
		{
			double duration = asset.duration;
			double* ptr = (double*)ptrToDouble.ToPointer();
			*ptr = duration;
		}
	}
}
