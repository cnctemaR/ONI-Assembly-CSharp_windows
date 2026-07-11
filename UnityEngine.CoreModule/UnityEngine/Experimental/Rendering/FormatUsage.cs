using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Use this format usages to figure out the capabilities of specific GraphicsFormat</para>
	/// </summary>
	public enum FormatUsage
	{
		/// <summary>
		///   <para>To create and sample textures.</para>
		/// </summary>
		Sample,
		/// <summary>
		///   <para>To sample textures with a linear filter</para>
		/// </summary>
		Linear,
		/// <summary>
		///   <para>To create and render to a rendertexture.</para>
		/// </summary>
		Render = 3,
		/// <summary>
		///   <para>To blend on a rendertexture.</para>
		/// </summary>
		Blend,
		/// <summary>
		///   <para>To perform resource load and store on a texture</para>
		/// </summary>
		LoadStore = 8,
		/// <summary>
		///   <para>To create and render to a MSAA 2X rendertexture.</para>
		/// </summary>
		MSAA2x,
		/// <summary>
		///   <para>To create and render to a MSAA 4X rendertexture.</para>
		/// </summary>
		MSAA4x,
		/// <summary>
		///   <para>To create and render to a MSAA 8X rendertexture.</para>
		/// </summary>
		MSAA8x
	}
}
