using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Class for handling cube maps, Use this to create or modify existing.</para>
	/// </summary>
	[ExcludeFromPreset]
	[NativeHeader("Runtime/Graphics/CubemapTexture.h")]
	public sealed class Cubemap : Texture
	{
		[RequiredByNativeCode]
		public Cubemap(int width, GraphicsFormat format, TextureCreationFlags flags)
		{
			if (base.ValidateFormat(format, FormatUsage.Sample))
			{
				Cubemap.Internal_Create(this, width, format, flags, IntPtr.Zero);
			}
		}

		internal Cubemap(int width, TextureFormat textureFormat, bool mipChain, IntPtr nativeTex)
		{
			GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(textureFormat, false);
			TextureCreationFlags textureCreationFlags = TextureCreationFlags.None;
			if (mipChain)
			{
				textureCreationFlags |= TextureCreationFlags.MipChain;
			}
			if (GraphicsFormatUtility.IsCrunchFormat(textureFormat))
			{
				textureCreationFlags |= TextureCreationFlags.Crunch;
			}
			Cubemap.Internal_Create(this, width, graphicsFormat, textureCreationFlags, nativeTex);
		}

		/// <summary>
		///   <para>Create a new empty cubemap texture.</para>
		/// </summary>
		/// <param name="size">Width/height of a cube face in pixels.</param>
		/// <param name="format">Pixel data format to be used for the Cubemap.</param>
		/// <param name="mipmap">Should mipmaps be created?</param>
		/// <param name="width"></param>
		/// <param name="textureFormat"></param>
		/// <param name="mipChain"></param>
		public Cubemap(int width, TextureFormat textureFormat, bool mipChain)
			: this(width, textureFormat, mipChain, IntPtr.Zero)
		{
		}

		/// <summary>
		///   <para>Returns pixel colors of a cubemap face.</para>
		/// </summary>
		/// <param name="face">The face from which pixel data is taken.</param>
		/// <param name="miplevel">Mipmap level for the chosen face.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels(CubemapFace face, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color[] GetPixels(CubemapFace face)
		{
			int num = 0;
			return this.GetPixels(face, num);
		}

		/// <summary>
		///   <para>Sets pixel colors of a cubemap face.</para>
		/// </summary>
		/// <param name="colors">Pixel data for the Cubemap face.</param>
		/// <param name="face">The face to which the new data should be applied.</param>
		/// <param name="miplevel">The mipmap level for the face.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels(Color[] colors, CubemapFace face, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels(Color[] colors, CubemapFace face)
		{
			int num = 0;
			this.SetPixels(colors, face, num);
		}

		/// <summary>
		///   <para>How many mipmap levels are in this texture (Read Only).</para>
		/// </summary>
		public extern int mipmapCount
		{
			[NativeName("CountDataMipmaps")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The format of the pixel data in the texture (Read Only).</para>
		/// </summary>
		public extern TextureFormat format
		{
			[NativeName("GetTextureFormat")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("CubemapScripting::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_CreateImpl([Writable] Cubemap mono, int ext, GraphicsFormat format, TextureCreationFlags flags, IntPtr nativeTex);

		private static void Internal_Create([Writable] Cubemap mono, int ext, GraphicsFormat format, TextureCreationFlags flags, IntPtr nativeTex)
		{
			if (!Cubemap.Internal_CreateImpl(mono, ext, format, flags, nativeTex))
			{
				throw new UnityException("Failed to create texture because of invalid parameters.");
			}
		}

		[FreeFunction(Name = "CubemapScripting::Apply", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable);

		[NativeName("GetIsReadable")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsReadable();

		[NativeName("SetPixel")]
		private void SetPixelImpl(int image, int x, int y, Color color)
		{
			this.SetPixelImpl_Injected(image, x, y, ref color);
		}

		[NativeName("GetPixel")]
		private Color GetPixelImpl(int image, int x, int y)
		{
			Color color;
			this.GetPixelImpl_Injected(image, x, y, out color);
			return color;
		}

		/// <summary>
		///   <para>Performs smoothing of near edge regions.</para>
		/// </summary>
		/// <param name="smoothRegionWidthInPixels">Pixel distance at edges over which to apply smoothing.</param>
		[NativeName("FixupEdges")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SmoothEdges([DefaultValue("1")] int smoothRegionWidthInPixels);

		public void SmoothEdges()
		{
			this.SmoothEdges(1);
		}

		/// <summary>
		///   <para>Creates a Unity cubemap out of externally created native cubemap object.</para>
		/// </summary>
		/// <param name="size">The width and height of each face of the cubemap should be the same.</param>
		/// <param name="format">Format of underlying cubemap object.</param>
		/// <param name="mipmap">Does the cubemap have mipmaps?</param>
		/// <param name="nativeTex">Native cubemap texture object.</param>
		/// <param name="width"></param>
		public static Cubemap CreateExternalTexture(int width, TextureFormat format, bool mipmap, IntPtr nativeTex)
		{
			if (nativeTex == IntPtr.Zero)
			{
				throw new ArgumentException("nativeTex can not be null");
			}
			return new Cubemap(width, format, mipmap, nativeTex);
		}

		/// <summary>
		///   <para>Sets pixel color at coordinates (face, x, y).</para>
		/// </summary>
		/// <param name="face"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="color"></param>
		public void SetPixel(CubemapFace face, int x, int y, Color color)
		{
			if (!this.IsReadable())
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelImpl((int)face, x, y, color);
		}

		/// <summary>
		///   <para>Returns pixel color at coordinates (face, x, y).</para>
		/// </summary>
		/// <param name="face"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Color GetPixel(CubemapFace face, int x, int y)
		{
			if (!this.IsReadable())
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelImpl((int)face, x, y);
		}

		/// <summary>
		///   <para>Actually apply all previous SetPixel and SetPixels changes.</para>
		/// </summary>
		/// <param name="updateMipmaps">When set to true, mipmap levels are recalculated.</param>
		/// <param name="makeNoLongerReadable">When set to true, system memory copy of a texture is released.</param>
		public void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable)
		{
			if (!this.IsReadable())
			{
				throw base.CreateNonReadableException(this);
			}
			this.ApplyImpl(updateMipmaps, makeNoLongerReadable);
		}

		public void Apply(bool updateMipmaps)
		{
			this.Apply(updateMipmaps, false);
		}

		public void Apply()
		{
			this.Apply(true, false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetPixelImpl_Injected(int image, int x, int y, ref Color color);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPixelImpl_Injected(int image, int x, int y, out Color ret);
	}
}
