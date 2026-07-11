using System;
using UnityEngine.Scripting;

namespace UnityEngine.Video
{
	/// <summary>
	///   <para>Places where the audio embedded in a video can be sent.</para>
	/// </summary>
	[RequiredByNativeCode]
	public enum VideoAudioOutputMode
	{
		/// <summary>
		///   <para>Disable the embedded audio.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>Send the embedded audio into a specified AudioSource.</para>
		/// </summary>
		AudioSource,
		/// <summary>
		///   <para>Send the embedded audio direct to the platform's audio hardware.</para>
		/// </summary>
		Direct,
		/// <summary>
		///   <para>Send the embedded audio to the associated AudioSampleProvider.</para>
		/// </summary>
		APIOnly
	}
}
