using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/Texture2DArray.h")]
	public sealed class Texture2DArray : Texture
	{
		[RequiredByNativeCode]
		public Texture2DArray(int width, int height, int depth, GraphicsFormat format, TextureCreationFlags flags)
		{
			if (base.ValidateFormat(format, FormatUsage.Sample))
			{
				Texture2DArray.Internal_Create(this, width, height, depth, format, flags);
			}
		}

		public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool linear)
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
				Texture2DArray.Internal_Create(this, width, height, depth, graphicsFormat, textureCreationFlags);
			}
		}

		public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, bool mipChain)
			: this(width, height, depth, textureFormat, mipChain, false)
		{
		}

		public extern int depth
		{
			[NativeName("GetTextureLayerCount")]
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

		[FreeFunction("Texture2DArrayScripting::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_CreateImpl([Writable] Texture2DArray mono, int w, int h, int d, GraphicsFormat format, TextureCreationFlags flags);

		private static void Internal_Create([Writable] Texture2DArray mono, int w, int h, int d, GraphicsFormat format, TextureCreationFlags flags)
		{
			if (!Texture2DArray.Internal_CreateImpl(mono, w, h, d, format, flags))
			{
				throw new UnityException("Failed to create 2D array texture because of invalid parameters.");
			}
		}

		[FreeFunction(Name = "Texture2DArrayScripting::Apply", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable);

		[FreeFunction(Name = "Texture2DArrayScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels(int arrayElement, int miplevel);

		public Color[] GetPixels(int arrayElement)
		{
			return this.GetPixels(arrayElement, 0);
		}

		[FreeFunction(Name = "Texture2DArrayScripting::GetPixels32", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color32[] GetPixels32(int arrayElement, int miplevel);

		public Color32[] GetPixels32(int arrayElement)
		{
			return this.GetPixels32(arrayElement, 0);
		}

		[FreeFunction(Name = "Texture2DArrayScripting::SetPixels", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels(Color[] colors, int arrayElement, int miplevel);

		public void SetPixels(Color[] colors, int arrayElement)
		{
			this.SetPixels(colors, arrayElement, 0);
		}

		[FreeFunction(Name = "Texture2DArrayScripting::SetPixels32", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels32(Color32[] colors, int arrayElement, int miplevel);

		public void SetPixels32(Color32[] colors, int arrayElement)
		{
			this.SetPixels32(colors, arrayElement, 0);
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
