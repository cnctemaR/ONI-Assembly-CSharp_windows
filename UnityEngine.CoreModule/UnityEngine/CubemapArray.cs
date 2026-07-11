using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/CubemapArrayTexture.h")]
	public sealed class CubemapArray : Texture
	{
		[RequiredByNativeCode]
		public CubemapArray(int width, int cubemapCount, GraphicsFormat format, TextureCreationFlags flags)
		{
			if (base.ValidateFormat(format, FormatUsage.Sample))
			{
				CubemapArray.Internal_Create(this, width, cubemapCount, format, flags);
			}
		}

		public CubemapArray(int width, int cubemapCount, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool linear)
		{
			if (base.ValidateFormat(textureFormat))
			{
				GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(textureFormat, !linear);
				TextureCreationFlags textureCreationFlags = TextureCreationFlags.None;
				if (mipChain)
				{
					textureCreationFlags |= TextureCreationFlags.MipChain;
				}
				if (GraphicsFormatUtility.IsCrunchFormat(textureFormat))
				{
					textureCreationFlags |= TextureCreationFlags.Crunch;
				}
				CubemapArray.Internal_Create(this, width, cubemapCount, graphicsFormat, textureCreationFlags);
			}
		}

		public CubemapArray(int width, int cubemapCount, TextureFormat textureFormat, bool mipChain)
			: this(width, cubemapCount, textureFormat, mipChain, false)
		{
		}

		public extern int cubemapCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern TextureFormat format
		{
			[NativeName("GetTextureFormat")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public override extern bool isReadable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("CubemapArrayScripting::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_CreateImpl([Writable] CubemapArray mono, int ext, int count, GraphicsFormat format, TextureCreationFlags flags);

		private static void Internal_Create([Writable] CubemapArray mono, int ext, int count, GraphicsFormat format, TextureCreationFlags flags)
		{
			if (!CubemapArray.Internal_CreateImpl(mono, ext, count, format, flags))
			{
				throw new UnityException("Failed to create cubemap array texture because of invalid parameters.");
			}
		}

		[FreeFunction(Name = "CubemapArrayScripting::Apply", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable);

		[FreeFunction(Name = "CubemapArrayScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels(CubemapFace face, int arrayElement, int miplevel);

		public Color[] GetPixels(CubemapFace face, int arrayElement)
		{
			return this.GetPixels(face, arrayElement, 0);
		}

		[FreeFunction(Name = "CubemapArrayScripting::GetPixels32", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color32[] GetPixels32(CubemapFace face, int arrayElement, int miplevel);

		public Color32[] GetPixels32(CubemapFace face, int arrayElement)
		{
			return this.GetPixels32(face, arrayElement, 0);
		}

		[FreeFunction(Name = "CubemapArrayScripting::SetPixels", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels(Color[] colors, CubemapFace face, int arrayElement, int miplevel);

		public void SetPixels(Color[] colors, CubemapFace face, int arrayElement)
		{
			this.SetPixels(colors, face, arrayElement, 0);
		}

		[FreeFunction(Name = "CubemapArrayScripting::SetPixels32", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels32(Color32[] colors, CubemapFace face, int arrayElement, int miplevel);

		public void SetPixels32(Color32[] colors, CubemapFace face, int arrayElement)
		{
			this.SetPixels32(colors, face, arrayElement, 0);
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
	}
}
