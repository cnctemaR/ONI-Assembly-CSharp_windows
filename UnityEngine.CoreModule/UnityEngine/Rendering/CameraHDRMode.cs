using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>The HDR mode to use for rendering.</para>
	/// </summary>
	public enum CameraHDRMode
	{
		/// <summary>
		///   <para>Uses RenderTextureFormat.ARGBHalf.</para>
		/// </summary>
		FP16 = 1,
		/// <summary>
		///   <para>Uses RenderTextureFormat.RGB111110Float.</para>
		/// </summary>
		R11G11B10
	}
}
