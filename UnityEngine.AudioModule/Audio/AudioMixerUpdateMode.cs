using System;

namespace UnityEngine.Audio
{
	/// <summary>
	///   <para>The mode in which an AudioMixer should update its time.</para>
	/// </summary>
	public enum AudioMixerUpdateMode
	{
		/// <summary>
		///   <para>Update the AudioMixer with scaled game time.</para>
		/// </summary>
		Normal,
		/// <summary>
		///   <para>Update the AudioMixer with unscaled realtime.</para>
		/// </summary>
		UnscaledTime
	}
}
