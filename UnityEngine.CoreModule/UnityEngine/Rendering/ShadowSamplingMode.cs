using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Used by CommandBuffer.SetShadowSamplingMode.</para>
	/// </summary>
	public enum ShadowSamplingMode
	{
		/// <summary>
		///   <para>Default shadow sampling mode: sampling with a comparison filter.</para>
		/// </summary>
		CompareDepths,
		/// <summary>
		///   <para>Shadow sampling mode for sampling the depth value.</para>
		/// </summary>
		RawDepth,
		/// <summary>
		///   <para>In ShadowSamplingMode.None, depths are not compared. Use this value if a Texture is not a shadowmap.</para>
		/// </summary>
		None
	}
}
