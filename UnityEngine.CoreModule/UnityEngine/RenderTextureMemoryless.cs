using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Flags enumeration of the render texture memoryless modes.</para>
	/// </summary>
	[Flags]
	public enum RenderTextureMemoryless
	{
		/// <summary>
		///   <para>The render texture is not memoryless.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>Render texture color pixels are memoryless when RenderTexture.antiAliasing is set to 1.</para>
		/// </summary>
		Color = 1,
		/// <summary>
		///   <para>Render texture depth pixels are memoryless.</para>
		/// </summary>
		Depth = 2,
		/// <summary>
		///   <para>Render texture color pixels are memoryless when RenderTexture.antiAliasing is set to 2, 4 or 8.</para>
		/// </summary>
		MSAA = 4
	}
}
