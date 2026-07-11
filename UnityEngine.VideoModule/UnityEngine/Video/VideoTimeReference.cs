using System;
using UnityEngine.Scripting;

namespace UnityEngine.Video
{
	/// <summary>
	///   <para>The clock that the Video.VideoPlayer observes to detect and correct drift.</para>
	/// </summary>
	[RequiredByNativeCode]
	public enum VideoTimeReference
	{
		/// <summary>
		///   <para>Disables the drift detection.</para>
		/// </summary>
		Freerun,
		/// <summary>
		///   <para>Internal reference clock the Video.VideoPlayer observes to detect and correct drift.</para>
		/// </summary>
		InternalTime,
		/// <summary>
		///   <para>External reference clock the Video.VideoPlayer observes to detect and correct drift.</para>
		/// </summary>
		ExternalTime
	}
}
