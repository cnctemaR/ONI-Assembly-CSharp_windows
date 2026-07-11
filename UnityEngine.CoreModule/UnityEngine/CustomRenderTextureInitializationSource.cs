using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Specify the source of a Custom Render Texture initialization.</para>
	/// </summary>
	public enum CustomRenderTextureInitializationSource
	{
		/// <summary>
		///   <para>Custom Render Texture is initialized by a Texture multiplied by a Color.</para>
		/// </summary>
		TextureAndColor,
		/// <summary>
		///   <para>Custom Render Texture is initalized with a Material.</para>
		/// </summary>
		Material
	}
}
