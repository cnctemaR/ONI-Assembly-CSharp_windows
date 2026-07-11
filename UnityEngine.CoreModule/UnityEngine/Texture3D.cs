using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Class for handling 3D Textures, Use this to create.</para>
	/// </summary>
	[ExcludeFromPreset]
	[NativeHeader("Runtime/Graphics/Texture3D.h")]
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

		/// <summary>
		///   <para>Create a new empty 3D Texture.</para>
		/// </summary>
		/// <param name="width">Width of texture in pixels.</param>
		/// <param name="height">Height of texture in pixels.</param>
		/// <param name="depth">Depth of texture in pixels.</param>
		/// <param name="format">Texture data format.</param>
		/// <param name="mipmap">Should the texture have mipmaps?</param>
		/// <param name="textureFormat"></param>
		/// <param name="mipChain"></param>
		public Texture3D(int width, int height, int depth, TextureFormat textureFormat, bool mipChain)
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

		/// <summary>
		///   <para>Returns an array of pixel colors representing one mip level of the 3D texture.</para>
		/// </summary>
		/// <param name="miplevel"></param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels([DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color[] GetPixels()
		{
			int num = 0;
			return this.GetPixels(num);
		}

		/// <summary>
		///   <para>Returns an array of pixel colors representing one mip level of the 3D texture.</para>
		/// </summary>
		/// <param name="miplevel"></param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color32[] GetPixels32([DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color32[] GetPixels32()
		{
			int num = 0;
			return this.GetPixels32(num);
		}

		/// <summary>
		///   <para>Sets pixel colors of a 3D texture.</para>
		/// </summary>
		/// <param name="colors">The colors to set the pixels to.</param>
		/// <param name="miplevel">The mipmap level to be affected by the new colors.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels(Color[] colors, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels(Color[] colors)
		{
			int num = 0;
			this.SetPixels(colors, num);
		}

		/// <summary>
		///   <para>Sets pixel colors of a 3D texture.</para>
		/// </summary>
		/// <param name="colors">The colors to set the pixels to.</param>
		/// <param name="miplevel">The mipmap level to be affected by the new colors.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels32(Color32[] colors, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels32(Color32[] colors)
		{
			int num = 0;
			this.SetPixels32(colors, num);
		}

		/// <summary>
		///   <para>The depth of the texture (Read Only).</para>
		/// </summary>
		public extern int depth
		{
			[NativeName("GetTextureLayerCount")]
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

		[NativeName("GetIsReadable")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsReadable();

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

		/// <summary>
		///   <para>Actually apply all previous SetPixels changes.</para>
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
	}
}
