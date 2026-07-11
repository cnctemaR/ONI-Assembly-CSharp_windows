using System;
using UnityEngine.Scripting;

namespace UnityEngine.Video
{
	/// <summary>
	///   <para>Types of 3D content layout within a video.</para>
	/// </summary>
	[RequiredByNativeCode]
	public enum Video3DLayout
	{
		/// <summary>
		///   <para>Video does not have any 3D content.</para>
		/// </summary>
		No3D,
		/// <summary>
		///   <para>Video contains 3D content where the left eye occupies the left half and right eye occupies the right half of video frames.</para>
		/// </summary>
		SideBySide3D,
		/// <summary>
		///   <para>Video contains 3D content where the left eye occupies the upper half and right eye occupies the lower half of video frames.</para>
		/// </summary>
		OverUnder3D
	}
}
