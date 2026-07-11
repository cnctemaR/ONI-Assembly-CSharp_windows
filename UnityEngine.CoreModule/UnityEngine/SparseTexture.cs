using System;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Class for handling Sparse Textures.</para>
	/// </summary>
	public sealed class SparseTexture : Texture
	{
		public SparseTexture(int width, int height, GraphicsFormat format, int mipCount)
		{
			if (base.ValidateFormat(format, FormatUsage.Sample))
			{
				SparseTexture.Internal_Create(this, width, height, GraphicsFormatUtility.GetTextureFormat(format), GraphicsFormatUtility.IsSRGBFormat(format), mipCount);
			}
		}

		/// <summary>
		///   <para>Create a sparse texture.</para>
		/// </summary>
		/// <param name="width">Texture width in pixels.</param>
		/// <param name="height">Texture height in pixels.</param>
		/// <param name="format">Texture format.</param>
		/// <param name="mipCount">Mipmap count. Pass -1 to create full mipmap chain.</param>
		/// <param name="linear">Whether texture data will be in linear or sRGB color space (default is sRGB).</param>
		public SparseTexture(int width, int height, TextureFormat format, int mipCount)
		{
			SparseTexture.Internal_Create(this, width, height, format, false, mipCount);
		}

		/// <summary>
		///   <para>Create a sparse texture.</para>
		/// </summary>
		/// <param name="width">Texture width in pixels.</param>
		/// <param name="height">Texture height in pixels.</param>
		/// <param name="format">Texture format.</param>
		/// <param name="mipCount">Mipmap count. Pass -1 to create full mipmap chain.</param>
		/// <param name="linear">Whether texture data will be in linear or sRGB color space (default is sRGB).</param>
		public SparseTexture(int width, int height, TextureFormat format, int mipCount, bool linear)
		{
			SparseTexture.Internal_Create(this, width, height, format, linear, mipCount);
		}

		/// <summary>
		///   <para>Get sparse texture tile width (Read Only).</para>
		/// </summary>
		public extern int tileWidth
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Get sparse texture tile height (Read Only).</para>
		/// </summary>
		public extern int tileHeight
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Is the sparse texture actually created? (Read Only)</para>
		/// </summary>
		public extern bool isCreated
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] SparseTexture mono, int width, int height, TextureFormat format, bool linear, int mipCount);

		/// <summary>
		///   <para>Update sparse texture tile with color values.</para>
		/// </summary>
		/// <param name="tileX">Tile X coordinate.</param>
		/// <param name="tileY">Tile Y coordinate.</param>
		/// <param name="miplevel">Mipmap level of the texture.</param>
		/// <param name="data">Tile color data.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateTile(int tileX, int tileY, int miplevel, Color32[] data);

		/// <summary>
		///   <para>Update sparse texture tile with raw pixel values.</para>
		/// </summary>
		/// <param name="tileX">Tile X coordinate.</param>
		/// <param name="tileY">Tile Y coordinate.</param>
		/// <param name="miplevel">Mipmap level of the texture.</param>
		/// <param name="data">Tile raw pixel data.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateTileRaw(int tileX, int tileY, int miplevel, byte[] data);

		/// <summary>
		///   <para>Unload sparse texture tile.</para>
		/// </summary>
		/// <param name="tileX">Tile X coordinate.</param>
		/// <param name="tileY">Tile Y coordinate.</param>
		/// <param name="miplevel">Mipmap level of the texture.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UnloadTile(int tileX, int tileY, int miplevel);
	}
}
