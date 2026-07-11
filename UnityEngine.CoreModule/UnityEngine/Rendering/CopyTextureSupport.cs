using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Support for various Graphics.CopyTexture cases.</para>
	/// </summary>
	[Flags]
	public enum CopyTextureSupport
	{
		/// <summary>
		///   <para>No support for Graphics.CopyTexture.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>Basic Graphics.CopyTexture support.</para>
		/// </summary>
		Basic = 1,
		/// <summary>
		///   <para>Support for Texture3D in Graphics.CopyTexture.</para>
		/// </summary>
		Copy3D = 2,
		/// <summary>
		///   <para>Support for Graphics.CopyTexture between different texture types.</para>
		/// </summary>
		DifferentTypes = 4,
		/// <summary>
		///   <para>Support for Texture to RenderTexture copies in Graphics.CopyTexture.</para>
		/// </summary>
		TextureToRT = 8,
		/// <summary>
		///   <para>Support for RenderTexture to Texture copies in Graphics.CopyTexture.</para>
		/// </summary>
		RTToTexture = 16
	}
}
