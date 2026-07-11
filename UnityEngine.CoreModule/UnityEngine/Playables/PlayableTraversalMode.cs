using System;

namespace UnityEngine.Playables
{
	/// <summary>
	///   <para>Traversal mode for Playables.</para>
	/// </summary>
	public enum PlayableTraversalMode
	{
		/// <summary>
		///   <para>Causes the Playable to prepare and process it's inputs when demanded by an output.</para>
		/// </summary>
		Mix,
		/// <summary>
		///   <para>Causes the Playable to act as a passthrough for PrepareFrame and ProcessFrame. If the PlayableOutput being processed is connected to the n-th input port of the Playable, the Playable only propagates the n-th output port. Use this enum value in conjunction with PlayableOutput SetSourceOutputPort.</para>
		/// </summary>
		Passthrough
	}
}
