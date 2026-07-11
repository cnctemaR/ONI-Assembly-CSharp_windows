using System;
using UnityEngine.Scripting;

namespace UnityEngine.Video
{
	/// <summary>
	///   <para>Source of the video content for a VideoPlayer.</para>
	/// </summary>
	[RequiredByNativeCode]
	public enum VideoSource
	{
		/// <summary>
		///   <para>Use the current clip as the video content source.</para>
		/// </summary>
		VideoClip,
		/// <summary>
		///   <para>Use the current URL as the video content source.</para>
		/// </summary>
		Url
	}
}
