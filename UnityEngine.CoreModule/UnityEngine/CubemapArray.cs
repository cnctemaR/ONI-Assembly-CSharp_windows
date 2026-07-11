using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Class for handling Cubemap arrays.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/CubemapArrayTexture.h")]
	public sealed class CubemapArray : Texture
	{
		public CubemapArray(int width, int cubemapCount, GraphicsFormat format, TextureCreationFlags flags)
		{
			if (base.ValidateFormat(format, FormatUsage.Sample))
			{
				CubemapArray.Internal_Create(this, width, cubemapCount, format, flags);
			}
		}

		/// <summary>
		///   <para>Create a new cubemap array.</para>
		/// </summary>
		/// <param name="faceSize">Cubemap face size in pixels.</param>
		/// <param name="cubemapCount">Number of elements in the cubemap array.</param>
		/// <param name="format">Format of the pixel data.</param>
		/// <param name="mipmap">Should mipmaps be created?</param>
		/// <param name="linear">Does the texture contain non-color data (i.e. don't do any color space conversions when sampling)? Default is false.</param>
		/// <param name="width"></param>
		/// <param name="textureFormat"></param>
		/// <param name="mipChain"></param>
		public CubemapArray(int width, int cubemapCount, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool linear)
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

		/// <summary>
		///   <para>Create a new cubemap array.</para>
		/// </summary>
		/// <param name="faceSize">Cubemap face size in pixels.</param>
		/// <param name="cubemapCount">Number of elements in the cubemap array.</param>
		/// <param name="format">Format of the pixel data.</param>
		/// <param name="mipmap">Should mipmaps be created?</param>
		/// <param name="linear">Does the texture contain non-color data (i.e. don't do any color space conversions when sampling)? Default is false.</param>
		/// <param name="width"></param>
		/// <param name="textureFormat"></param>
		/// <param name="mipChain"></param>
		public CubemapArray(int width, int cubemapCount, TextureFormat textureFormat, bool mipChain)
			: this(width, cubemapCount, textureFormat, mipChain, false)
		{
		}

		/// <summary>
		///   <para>Set pixel colors for a single array slice/face.</para>
		/// </summary>
		/// <param name="colors">An array of pixel colors.</param>
		/// <param name="face">Cubemap face to set pixels for.</param>
		/// <param name="arrayElement">Array element index to set pixels for.</param>
		/// <param name="miplevel">Mipmap level to set pixels for.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels(Color[] colors, CubemapFace face, int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels(Color[] colors, CubemapFace face, int arrayElement)
		{
			int num = 0;
			this.SetPixels(colors, face, arrayElement, num);
		}

		/// <summary>
		///   <para>Set pixel colors for a single array slice/face.</para>
		/// </summary>
		/// <param name="colors">An array of pixel colors in low precision (8 bits/channel) format.</param>
		/// <param name="face">Cubemap face to set pixels for.</param>
		/// <param name="arrayElement">Array element index to set pixels for.</param>
		/// <param name="miplevel">Mipmap level to set pixels for.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels32(Color32[] colors, CubemapFace face, int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels32(Color32[] colors, CubemapFace face, int arrayElement)
		{
			int num = 0;
			this.SetPixels32(colors, face, arrayElement, num);
		}

		/// <summary>
		///   <para>Returns pixel colors of a single array slice/face.</para>
		/// </summary>
		/// <param name="face">Cubemap face to read pixels from.</param>
		/// <param name="arrayElement">Array slice to read pixels from.</param>
		/// <param name="miplevel">Mipmap level to read pixels from.</param>
		/// <returns>
		///   <para>Array of pixel colors.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels(CubemapFace face, int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color[] GetPixels(CubemapFace face, int arrayElement)
		{
			int num = 0;
			return this.GetPixels(face, arrayElement, num);
		}

		/// <summary>
		///   <para>Returns pixel colors of a single array slice/face.</para>
		/// </summary>
		/// <param name="face">Cubemap face to read pixels from.</param>
		/// <param name="arrayElement">Array slice to read pixels from.</param>
		/// <param name="miplevel">Mipmap level to read pixels from.</param>
		/// <returns>
		///   <para>Array of pixel colors in low precision (8 bits/channel) format.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color32[] GetPixels32(CubemapFace face, int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color32[] GetPixels32(CubemapFace face, int arrayElement)
		{
			int num = 0;
			return this.GetPixels32(face, arrayElement, num);
		}

		/// <summary>
		///   <para>Number of cubemaps in the array (Read Only).</para>
		/// </summary>
		public extern int cubemapCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Texture format (Read Only).</para>
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
