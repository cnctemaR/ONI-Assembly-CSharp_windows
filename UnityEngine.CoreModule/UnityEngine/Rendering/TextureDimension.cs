using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Texture "dimension" (type).</para>
	/// </summary>
	public enum TextureDimension
	{
		/// <summary>
		///   <para>Texture type is not initialized or unknown.</para>
		/// </summary>
		Unknown = -1,
		/// <summary>
		///   <para>No texture is assigned.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>Any texture type.</para>
		/// </summary>
		Any,
		/// <summary>
		///   <para>2D texture (Texture2D).</para>
		/// </summary>
		Tex2D,
		/// <summary>
		///   <para>3D volume texture (Texture3D).</para>
		/// </summary>
		Tex3D,
		/// <summary>
		///   <para>Cubemap texture.</para>
		/// </summary>
		Cube,
		/// <summary>
		///   <para>2D array texture (Texture2DArray).</para>
		/// </summary>
		Tex2DArray,
		/// <summary>
		///   <para>Cubemap array texture (CubemapArray).</para>
		/// </summary>
		CubeArray
	}
}
