using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Class for handling 2D texture arrays.</para>
	/// </summary>
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

		/// <summary>
		///   <para>Create a new texture array.</para>
		/// </summary>
		/// <param name="width">Width of texture array in pixels.</param>
		/// <param name="height">Height of texture array in pixels.</param>
		/// <param name="depth">Number of elements in the texture array.</param>
		/// <param name="format">Format of the texture.</param>
		/// <param name="mipmap">Should mipmaps be created?</param>
		/// <param name="linear">Does the texture contain non-color data (i.e. don't do any color space conversions when sampling)? Default is false.</param>
		/// <param name="textureFormat"></param>
		/// <param name="mipChain"></param>
		public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool linear)
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

		/// <summary>
		///   <para>Create a new texture array.</para>
		/// </summary>
		/// <param name="width">Width of texture array in pixels.</param>
		/// <param name="height">Height of texture array in pixels.</param>
		/// <param name="depth">Number of elements in the texture array.</param>
		/// <param name="format">Format of the texture.</param>
		/// <param name="mipmap">Should mipmaps be created?</param>
		/// <param name="linear">Does the texture contain non-color data (i.e. don't do any color space conversions when sampling)? Default is false.</param>
		/// <param name="textureFormat"></param>
		/// <param name="mipChain"></param>
		public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, bool mipChain)
			: this(width, height, depth, textureFormat, mipChain, false)
		{
		}

		/// <summary>
		///   <para>Set pixel colors for the whole mip level.</para>
		/// </summary>
		/// <param name="colors">An array of pixel colors.</param>
		/// <param name="arrayElement">The texture array element index.</param>
		/// <param name="miplevel">The mip level.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels(Color[] colors, int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels(Color[] colors, int arrayElement)
		{
			int num = 0;
			this.SetPixels(colors, arrayElement, num);
		}

		/// <summary>
		///   <para>Set pixel colors for the whole mip level.</para>
		/// </summary>
		/// <param name="colors">An array of pixel colors.</param>
		/// <param name="arrayElement">The texture array element index.</param>
		/// <param name="miplevel">The mip level.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels32(Color32[] colors, int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels32(Color32[] colors, int arrayElement)
		{
			int num = 0;
			this.SetPixels32(colors, arrayElement, num);
		}

		/// <summary>
		///   <para>Returns pixel colors of a single array slice.</para>
		/// </summary>
		/// <param name="arrayElement">Array slice to read pixels from.</param>
		/// <param name="miplevel">Mipmap level to read pixels from.</param>
		/// <returns>
		///   <para>Array of pixel colors.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels(int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color[] GetPixels(int arrayElement)
		{
			int num = 0;
			return this.GetPixels(arrayElement, num);
		}

		/// <summary>
		///   <para>Returns pixel colors of a single array slice.</para>
		/// </summary>
		/// <param name="arrayElement">Array slice to read pixels from.</param>
		/// <param name="miplevel">Mipmap level to read pixels from.</param>
		/// <returns>
		///   <para>Array of pixel colors in low precision (8 bits/channel) format.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color32[] GetPixels32(int arrayElement, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color32[] GetPixels32(int arrayElement)
		{
			int num = 0;
			return this.GetPixels32(arrayElement, num);
		}

		/// <summary>
		///   <para>Number of elements in a texture array (Read Only).</para>
		/// </summary>
		public extern int depth
		{
			[NativeName("GetTextureLayerCount")]
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
