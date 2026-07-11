using System;

namespace UnityEngine.Playables
{
	/// <summary>
	///   <para>Status of a Playable.</para>
	/// </summary>
	public enum PlayState
	{
		/// <summary>
		///   <para>The Playable has been paused. Its local time will not advance.</para>
		/// </summary>
		Paused,
		/// <summary>
		///   <para>The Playable is currently Playing.</para>
		/// </summary>
		Playing,
		/// <summary>
		///   <para>The Playable has been delayed, using PlayableExtensions.SetDelay. It will not start until the delay is entirely consumed.</para>
		/// </summary>
		Delayed
	}
}
