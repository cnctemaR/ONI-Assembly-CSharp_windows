using System;
using UnityEngine.Scripting;

namespace UnityEngine.Video
{
	/// <summary>
	///   <para>Type of destination for the images read by a VideoPlayer.</para>
	/// </summary>
	[RequiredByNativeCode]
	public enum VideoRenderMode
	{
		/// <summary>
		///   <para>Draw video content behind a camera's scene.</para>
		/// </summary>
		CameraFarPlane,
		/// <summary>
		///   <para>Draw video content in front of a camera's scene.</para>
		/// </summary>
		CameraNearPlane,
		/// <summary>
		///   <para>Draw video content into a RenderTexture.</para>
		/// </summary>
		RenderTexture,
		/// <summary>
		///   <para>Draw the video content into a user-specified property of the current GameObject's material.</para>
		/// </summary>
		MaterialOverride,
		/// <summary>
		///   <para>Don't draw the video content anywhere, but still make it available via the VideoPlayer's texture property in the API.</para>
		/// </summary>
		APIOnly
	}
}
