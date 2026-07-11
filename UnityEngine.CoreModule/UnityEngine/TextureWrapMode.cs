using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Wrap mode for textures.</para>
	/// </summary>
	public enum TextureWrapMode
	{
		/// <summary>
		///   <para>Tiles the texture, creating a repeating pattern.</para>
		/// </summary>
		Repeat,
		/// <summary>
		///   <para>Clamps the texture to the last pixel at the edge.</para>
		/// </summary>
		Clamp,
		/// <summary>
		///   <para>Tiles the texture, creating a repeating pattern by mirroring it at every integer boundary.</para>
		/// </summary>
		Mirror,
		/// <summary>
		///   <para>Mirrors the texture once, then clamps to edge pixels.</para>
		/// </summary>
		MirrorOnce
	}
}
