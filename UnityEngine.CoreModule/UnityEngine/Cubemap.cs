using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
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
			if (base.ValidateFormat(textureFormat))
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
		}

		public Cubemap(int width, TextureFormat textureFormat, bool mipChain)
			: this(width, textureFormat, mipChain, IntPtr.Zero)
		{
		}

		public extern int mipmapCount
		{
			[NativeName("CountDataMipmaps")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

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

		public override extern bool isReadable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

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

		[NativeName("FixupEdges")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SmoothEdges([DefaultValue("1")] int smoothRegionWidthInPixels);

		public void SmoothEdges()
		{
			this.SmoothEdges(1);
		}

		[FreeFunction(Name = "CubemapScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels(CubemapFace face, int miplevel);

		public Color[] GetPixels(CubemapFace face)
		{
			return this.GetPixels(face, 0);
		}

		[FreeFunction(Name = "CubemapScripting::SetPixels", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels(Color[] colors, CubemapFace face, int miplevel);

		public void SetPixels(Color[] colors, CubemapFace face)
		{
			this.SetPixels(colors, face, 0);
		}

		public static Cubemap CreateExternalTexture(int width, TextureFormat format, bool mipmap, IntPtr nativeTex)
		{
			if (nativeTex == IntPtr.Zero)
			{
				throw new ArgumentException("nativeTex can not be null");
			}
			return new Cubemap(width, format, mipmap, nativeTex);
		}

		public void SetPixel(CubemapFace face, int x, int y, Color color)
		{
			if (!this.isReadable)
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelImpl((int)face, x, y, color);
		}

		public Color GetPixel(CubemapFace face, int x, int y)
		{
			if (!this.isReadable)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelImpl((int)face, x, y);
		}

		public void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable)
		{
			if (!this.isReadable)
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
