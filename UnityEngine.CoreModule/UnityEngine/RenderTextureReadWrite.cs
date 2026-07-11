using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Color space conversion mode of a RenderTexture.</para>
	/// </summary>
	public enum RenderTextureReadWrite
	{
		/// <summary>
		///   <para>Default color space conversion based on project settings.</para>
		/// </summary>
		Default,
		/// <summary>
		///   <para>Render texture contains linear (non-color) data; don't perform color conversions on it.</para>
		/// </summary>
		Linear,
		/// <summary>
		///   <para>Render texture contains sRGB (color) data, perform Linear&lt;-&gt;sRGB conversions on it.</para>
		/// </summary>
		sRGB
	}
}
