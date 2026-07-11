using System;

namespace UnityEngine.Playables
{
	/// <summary>
	///   <para>Wrap mode for Playables.</para>
	/// </summary>
	public enum DirectorWrapMode
	{
		/// <summary>
		///   <para>Hold the last frame when the playable time reaches it's duration.</para>
		/// </summary>
		Hold,
		/// <summary>
		///   <para>Loop back to zero time and continue playing.</para>
		/// </summary>
		Loop,
		/// <summary>
		///   <para>Do not keep playing when the time reaches the duration.</para>
		/// </summary>
		None
	}
}
