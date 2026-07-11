using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/Texture3D.h")]
	[ExcludeFromPreset]
	public sealed class Texture3D : Texture
	{
		[RequiredByNativeCode]
		public Texture3D(int width, int height, int depth, GraphicsFormat format, TextureCreationFlags flags)
		{
			if (base.ValidateFormat(format, FormatUsage.Sample))
			{
				Texture3D.Internal_Create(this, width, height, depth, format, flags);
			}
		}

		public Texture3D(int width, int height, int depth, TextureFormat textureFormat, bool mipChain)
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
				Texture3D.Internal_Create(this, width, height, depth, graphicsFormat, textureCreationFlags);
			}
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

		[FreeFunction("Texture3DScripting::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_CreateImpl([Writable] Texture3D mono, int w, int h, int d, GraphicsFormat format, TextureCreationFlags flags);

		private static void Internal_Create([Writable] Texture3D mono, int w, int h, int d, GraphicsFormat format, TextureCreationFlags flags)
		{
			if (!Texture3D.Internal_CreateImpl(mono, w, h, d, format, flags))
			{
				throw new UnityException("Failed to create texture because of invalid parameters.");
			}
		}

		[FreeFunction(Name = "Texture3DScripting::Apply", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable);

		[FreeFunction(Name = "Texture3DScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels(int miplevel);

		public Color[] GetPixels()
		{
			return this.GetPixels(0);
		}

		[FreeFunction(Name = "Texture3DScripting::GetPixels32", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color32[] GetPixels32(int miplevel);

		public Color32[] GetPixels32()
		{
			return this.GetPixels32(0);
		}

		[FreeFunction(Name = "Texture3DScripting::SetPixels", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels(Color[] colors, int miplevel);

		public void SetPixels(Color[] colors)
		{
			this.SetPixels(colors, 0);
		}

		[FreeFunction(Name = "Texture3DScripting::SetPixels32", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels32(Color32[] colors, int miplevel);

		public void SetPixels32(Color32[] colors)
		{
			this.SetPixels32(colors, 0);
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
