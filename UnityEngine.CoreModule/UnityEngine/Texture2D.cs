using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Class for texture handling.</para>
	/// </summary>
	[UsedByNativeCode]
	[NativeHeader("Runtime/Graphics/Texture2D.h")]
	[NativeHeader("Runtime/Graphics/GeneratedTextures.h")]
	public sealed class Texture2D : Texture
	{
		internal Texture2D(int width, int height, GraphicsFormat format, TextureCreationFlags flags, IntPtr nativeTex)
		{
			if (base.ValidateFormat(format, FormatUsage.Sample))
			{
				Texture2D.Internal_Create(this, width, height, format, flags, nativeTex);
			}
		}

		public Texture2D(int width, int height, GraphicsFormat format, TextureCreationFlags flags)
			: this(width, height, format, flags, IntPtr.Zero)
		{
		}

		internal Texture2D(int width, int height, TextureFormat textureFormat, bool mipChain, bool linear, IntPtr nativeTex)
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
			Texture2D.Internal_Create(this, width, height, graphicsFormat, textureCreationFlags, nativeTex);
		}

		/// <summary>
		///   <para>Create a new empty texture.</para>
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="textureFormat"></param>
		/// <param name="mipChain"></param>
		/// <param name="linear"></param>
		public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("true")] bool mipChain, [DefaultValue("false")] bool linear)
			: this(width, height, textureFormat, mipChain, linear, IntPtr.Zero)
		{
		}

		/// <summary>
		///   <para>Create a new empty texture.</para>
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="textureFormat"></param>
		/// <param name="mipChain"></param>
		public Texture2D(int width, int height, TextureFormat textureFormat, bool mipChain)
			: this(width, height, textureFormat, mipChain, false, IntPtr.Zero)
		{
		}

		/// <summary>
		///   <para>Create a new empty texture.</para>
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public Texture2D(int width, int height)
			: this(width, height, TextureFormat.RGBA32, true, false, IntPtr.Zero)
		{
		}

		/// <summary>
		///   <para>Updates Unity texture to use different native texture object.</para>
		/// </summary>
		/// <param name="nativeTex">Native 2D texture object.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateExternalTexture(IntPtr nativeTex);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetAllPixels32(Color32[] colors, int miplevel);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetBlockOfPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, int miplevel);

		[ExcludeFromDocs]
		public void SetPixels32(Color32[] colors)
		{
			int num = 0;
			this.SetPixels32(colors, num);
		}

		/// <summary>
		///   <para>Set a block of pixel colors.</para>
		/// </summary>
		/// <param name="colors"></param>
		/// <param name="miplevel"></param>
		public void SetPixels32(Color32[] colors, [DefaultValue("0")] int miplevel)
		{
			this.SetAllPixels32(colors, miplevel);
		}

		[ExcludeFromDocs]
		public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors)
		{
			int num = 0;
			this.SetPixels32(x, y, blockWidth, blockHeight, colors, num);
		}

		/// <summary>
		///   <para>Set a block of pixel colors.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="blockWidth"></param>
		/// <param name="blockHeight"></param>
		/// <param name="colors"></param>
		/// <param name="miplevel"></param>
		public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, [DefaultValue("0")] int miplevel)
		{
			this.SetBlockOfPixels32(x, y, blockWidth, blockHeight, colors, miplevel);
		}

		/// <summary>
		///   <para>Get raw data from a texture.</para>
		/// </summary>
		/// <returns>
		///   <para>Raw texture data as a byte array.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern byte[] GetRawTextureData();

		[ExcludeFromDocs]
		public Color[] GetPixels()
		{
			int num = 0;
			return this.GetPixels(num);
		}

		/// <summary>
		///   <para>Get the pixel colors from the texture.</para>
		/// </summary>
		/// <param name="miplevel">The mipmap level to fetch the pixels from. Defaults to zero.</param>
		/// <returns>
		///   <para>The array of all pixels in the mipmap level of the texture.</para>
		/// </returns>
		public Color[] GetPixels([DefaultValue("0")] int miplevel)
		{
			int num = this.width >> miplevel;
			if (num < 1)
			{
				num = 1;
			}
			int num2 = this.height >> miplevel;
			if (num2 < 1)
			{
				num2 = 1;
			}
			return this.GetPixels(0, 0, num, num2, miplevel);
		}

		/// <summary>
		///   <para>Get a block of pixel colors.</para>
		/// </summary>
		/// <param name="x">The x position of the pixel array to fetch.</param>
		/// <param name="y">The y position of the pixel array to fetch.</param>
		/// <param name="blockWidth">The width length of the pixel array to fetch.</param>
		/// <param name="blockHeight">The height length of the pixel array to fetch.</param>
		/// <param name="miplevel">The mipmap level to fetch the pixels. Defaults to zero, and is
		///   optional.</param>
		/// <returns>
		///   <para>The array of pixels in the texture that have been selected.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels(int x, int y, int blockWidth, int blockHeight, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight)
		{
			int num = 0;
			return this.GetPixels(x, y, blockWidth, blockHeight, num);
		}

		/// <summary>
		///   <para>Get a block of pixel colors in Color32 format.</para>
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
		///   <para>Packs multiple Textures into a texture atlas.</para>
		/// </summary>
		/// <param name="textures">Array of textures to pack into the atlas.</param>
		/// <param name="padding">Padding in pixels between the packed textures.</param>
		/// <param name="maximumAtlasSize">Maximum size of the resulting texture.</param>
		/// <param name="makeNoLongerReadable">Should the texture be marked as no longer readable?</param>
		/// <returns>
		///   <para>An array of rectangles containing the UV coordinates in the atlas for each input texture, or null if packing fails.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Rect[] PackTextures(Texture2D[] textures, int padding, [DefaultValue("2048")] int maximumAtlasSize, [DefaultValue("false")] bool makeNoLongerReadable);

		[ExcludeFromDocs]
		public Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize)
		{
			bool flag = false;
			return this.PackTextures(textures, padding, maximumAtlasSize, flag);
		}

		[ExcludeFromDocs]
		public Rect[] PackTextures(Texture2D[] textures, int padding)
		{
			bool flag = false;
			int num = 2048;
			return this.PackTextures(textures, padding, num, flag);
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

		/// <summary>
		///   <para>Get a small texture with all white pixels.</para>
		/// </summary>
		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static extern Texture2D whiteTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Get a small texture with all black pixels.</para>
		/// </summary>
		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static extern Texture2D blackTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Compress texture into DXT format.</para>
		/// </summary>
		/// <param name="highQuality"></param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Compress(bool highQuality);

		[FreeFunction("Texture2DScripting::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_CreateImpl([Writable] Texture2D mono, int w, int h, GraphicsFormat format, TextureCreationFlags flags, IntPtr nativeTex);

		private static void Internal_Create([Writable] Texture2D mono, int w, int h, GraphicsFormat format, TextureCreationFlags flags, IntPtr nativeTex)
		{
			if (!Texture2D.Internal_CreateImpl(mono, w, h, format, flags, nativeTex))
			{
				throw new UnityException("Failed to create texture because of invalid parameters.");
			}
		}

		[NativeName("GetIsReadable")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsReadable();

		[NativeName("Apply")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable);

		[NativeName("Resize")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool ResizeImpl(int width, int height);

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

		[NativeName("GetPixelBilinear")]
		private Color GetPixelBilinearImpl(int image, float x, float y)
		{
			Color color;
			this.GetPixelBilinearImpl_Injected(image, x, y, out color);
			return color;
		}

		[FreeFunction(Name = "Texture2DScripting::ResizeWithFormat", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool ResizeWithFormatImpl(int width, int height, TextureFormat format, bool hasMipMap);

		[FreeFunction(Name = "Texture2DScripting::ReadPixels", HasExplicitThis = true)]
		private void ReadPixelsImpl(Rect source, int destX, int destY, bool recalculateMipMaps)
		{
			this.ReadPixelsImpl_Injected(ref source, destX, destY, recalculateMipMaps);
		}

		[FreeFunction(Name = "Texture2DScripting::SetPixels", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetPixelsImpl(int x, int y, int w, int h, Color[] pixel, int miplevel, int frame);

		[FreeFunction(Name = "Texture2DScripting::LoadRawData", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool LoadRawTextureDataImpl(IntPtr data, int size);

		[FreeFunction(Name = "Texture2DScripting::LoadRawData", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool LoadRawTextureDataImplArray(byte[] data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr GetWritableImageData(int frame);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern long GetRawImageDataSize();

		[FreeFunction("Texture2DScripting::GenerateAtlas")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateAtlasImpl(Vector2[] sizes, int padding, int atlasSize, [Out] Rect[] rect);

		/// <summary>
		///   <para>Has mipmap streaming been enabled for this texture.</para>
		/// </summary>
		public extern bool streamingMipmaps
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Relative priority for this texture when reducing memory size in order to hit the memory budget.</para>
		/// </summary>
		public extern int streamingMipmapsPriority
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The mipmap level to load.</para>
		/// </summary>
		public extern int requestedMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetRequestedMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction(Name = "GetTextureStreamingManager().SetRequestedMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The mipmap level which would have been loaded by the streaming system before memory budgets are applied.</para>
		/// </summary>
		public extern int desiredMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetDesiredMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Which mipmap level is in the process of being loaded by the mipmap streaming system.</para>
		/// </summary>
		public extern int loadingMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetLoadingMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Which mipmap level is currently loaded by the streaming system.</para>
		/// </summary>
		public extern int loadedMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetLoadedMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Resets the requestedMipmapLevel field.</para>
		/// </summary>
		[FreeFunction(Name = "GetTextureStreamingManager().ClearRequestedMipmapLevel", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearRequestedMipmapLevel();

		/// <summary>
		///   <para>Has the mipmap level requested by setting requestedMipmapLevel finished loading?</para>
		/// </summary>
		/// <returns>
		///   <para>True if the mipmap level requested by setting requestedMipmapLevel has finished loading.</para>
		/// </returns>
		[FreeFunction(Name = "GetTextureStreamingManager().IsRequestedMipmapLevelLoaded", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsRequestedMipmapLevelLoaded();

		/// <summary>
		///   <para>Creates Unity Texture out of externally created native texture object.</para>
		/// </summary>
		/// <param name="nativeTex">Native 2D texture object.</param>
		/// <param name="width">Width of texture in pixels.</param>
		/// <param name="height">Height of texture in pixels.</param>
		/// <param name="format">Format of underlying texture object.</param>
		/// <param name="mipmap">Does the texture have mipmaps?</param>
		/// <param name="linear">Is texture using linear color space?</param>
		/// <param name="mipChain"></param>
		public static Texture2D CreateExternalTexture(int width, int height, TextureFormat format, bool mipChain, bool linear, IntPtr nativeTex)
		{
			if (nativeTex == IntPtr.Zero)
			{
				throw new ArgumentException("nativeTex can not be null");
			}
			return new Texture2D(width, height, format, mipChain, linear, nativeTex);
		}

		/// <summary>
		///   <para>Sets pixel color at coordinates (x,y).</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="color"></param>
		public void SetPixel(int x, int y, Color color)
		{
			if (!this.IsReadable())
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelImpl(0, x, y, color);
		}

		/// <summary>
		///   <para>Set a block of pixel colors.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="blockWidth"></param>
		/// <param name="blockHeight"></param>
		/// <param name="colors"></param>
		/// <param name="miplevel"></param>
		public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, [DefaultValue("0")] int miplevel)
		{
			if (!this.IsReadable())
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelsImpl(x, y, blockWidth, blockHeight, colors, miplevel, 0);
		}

		public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors)
		{
			this.SetPixels(x, y, blockWidth, blockHeight, colors, 0);
		}

		/// <summary>
		///   <para>Set a block of pixel colors.</para>
		/// </summary>
		/// <param name="colors">The array of pixel colours to assign (a 2D image flattened to a 1D array).</param>
		/// <param name="miplevel">The mip level of the texture to write to.</param>
		public void SetPixels(Color[] colors, [DefaultValue("0")] int miplevel)
		{
			int num = this.width >> miplevel;
			if (num < 1)
			{
				num = 1;
			}
			int num2 = this.height >> miplevel;
			if (num2 < 1)
			{
				num2 = 1;
			}
			this.SetPixels(0, 0, num, num2, colors, miplevel);
		}

		public void SetPixels(Color[] colors)
		{
			this.SetPixels(0, 0, this.width, this.height, colors, 0);
		}

		/// <summary>
		///   <para>Returns pixel color at coordinates (x, y).</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Color GetPixel(int x, int y)
		{
			if (!this.IsReadable())
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelImpl(0, x, y);
		}

		/// <summary>
		///   <para>Returns filtered pixel color at normalized coordinates (u, v).</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Color GetPixelBilinear(float x, float y)
		{
			if (!this.IsReadable())
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelBilinearImpl(0, x, y);
		}

		/// <summary>
		///   <para>Fills texture pixels with raw preformatted data.</para>
		/// </summary>
		/// <param name="data">Raw data array to initialize texture pixels with.</param>
		/// <param name="size">Size of data in bytes.</param>
		public void LoadRawTextureData(IntPtr data, int size)
		{
			if (!this.IsReadable())
			{
				throw base.CreateNonReadableException(this);
			}
			if (data == IntPtr.Zero || size == 0)
			{
				Debug.LogError("No texture data provided to LoadRawTextureData", this);
			}
			else if (!this.LoadRawTextureDataImpl(data, size))
			{
				throw new UnityException("LoadRawTextureData: not enough data provided (will result in overread).");
			}
		}

		/// <summary>
		///   <para>Fills texture pixels with raw preformatted data.</para>
		/// </summary>
		/// <param name="data">Raw data array to initialize texture pixels with.</param>
		/// <param name="size">Size of data in bytes.</param>
		public void LoadRawTextureData(byte[] data)
		{
			if (!this.IsReadable())
			{
				throw base.CreateNonReadableException(this);
			}
			if (data == null || data.Length == 0)
			{
				Debug.LogError("No texture data provided to LoadRawTextureData", this);
			}
			else if (!this.LoadRawTextureDataImplArray(data))
			{
				throw new UnityException("LoadRawTextureData: not enough data provided (will result in overread).");
			}
		}

		public void LoadRawTextureData<T>(NativeArray<T> data) where T : struct
		{
			if (!this.IsReadable())
			{
				throw base.CreateNonReadableException(this);
			}
			if (!data.IsCreated || data.Length == 0)
			{
				throw new UnityException("No texture data provided to LoadRawTextureData");
			}
			if (!this.LoadRawTextureDataImpl((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), data.Length * UnsafeUtility.SizeOf<T>()))
			{
				throw new UnityException("LoadRawTextureData: not enough data provided (will result in overread).");
			}
		}

		public unsafe NativeArray<T> GetRawTextureData<T>() where T : struct
		{
			if (!this.IsReadable())
			{
				throw base.CreateNonReadableException(this);
			}
			int num = UnsafeUtility.SizeOf<T>();
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)this.GetWritableImageData(0), (int)(this.GetRawImageDataSize() / (long)num), Allocator.None);
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

		/// <summary>
		///   <para>Resizes the texture.</para>
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public bool Resize(int width, int height)
		{
			if (!this.IsReadable())
			{
				throw base.CreateNonReadableException(this);
			}
			return this.ResizeImpl(width, height);
		}

		/// <summary>
		///   <para>Resizes the texture.</para>
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="format"></param>
		/// <param name="hasMipMap"></param>
		public bool Resize(int width, int height, TextureFormat format, bool hasMipMap)
		{
			if (!this.IsReadable())
			{
				throw base.CreateNonReadableException(this);
			}
			return this.ResizeWithFormatImpl(width, height, format, hasMipMap);
		}

		/// <summary>
		///   <para>Read pixels from screen into the saved texture data.</para>
		/// </summary>
		/// <param name="source">Rectangular region of the view to read from. Pixels are read from current render target.</param>
		/// <param name="destX">Horizontal pixel position in the texture to place the pixels that are read.</param>
		/// <param name="destY">Vertical pixel position in the texture to place the pixels that are read.</param>
		/// <param name="recalculateMipMaps">Should the texture's mipmaps be recalculated after reading?</param>
		public void ReadPixels(Rect source, int destX, int destY, [DefaultValue("true")] bool recalculateMipMaps)
		{
			if (!this.IsReadable())
			{
				throw base.CreateNonReadableException(this);
			}
			this.ReadPixelsImpl(source, destX, destY, recalculateMipMaps);
		}

		[ExcludeFromDocs]
		public void ReadPixels(Rect source, int destX, int destY)
		{
			this.ReadPixels(source, destX, destY, true);
		}

		public static bool GenerateAtlas(Vector2[] sizes, int padding, int atlasSize, List<Rect> results)
		{
			if (sizes == null)
			{
				throw new ArgumentException("sizes array can not be null");
			}
			if (results == null)
			{
				throw new ArgumentException("results list cannot be null");
			}
			if (padding < 0)
			{
				throw new ArgumentException("padding can not be negative");
			}
			if (atlasSize <= 0)
			{
				throw new ArgumentException("atlas size must be positive");
			}
			results.Clear();
			bool flag;
			if (sizes.Length == 0)
			{
				flag = true;
			}
			else
			{
				NoAllocHelpers.EnsureListElemCount<Rect>(results, sizes.Length);
				Texture2D.GenerateAtlasImpl(sizes, padding, atlasSize, NoAllocHelpers.ExtractArrayFromListT<Rect>(results));
				flag = results.Count != 0;
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetPixelImpl_Injected(int image, int x, int y, ref Color color);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPixelImpl_Injected(int image, int x, int y, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPixelBilinearImpl_Injected(int image, float x, float y, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ReadPixelsImpl_Injected(ref Rect source, int destX, int destY, bool recalculateMipMaps);

		/// <summary>
		///   <para>Flags used to control the encoding to an EXR file.</para>
		/// </summary>
		[Flags]
		public enum EXRFlags
		{
			/// <summary>
			///   <para>No flag. This will result in an uncompressed 16-bit float EXR file.</para>
			/// </summary>
			None = 0,
			/// <summary>
			///   <para>The texture will be exported as a 32-bit float EXR file (default is 16-bit).</para>
			/// </summary>
			OutputAsFloat = 1,
			/// <summary>
			///   <para>The texture will use the EXR ZIP compression format.</para>
			/// </summary>
			CompressZIP = 2,
			/// <summary>
			///   <para>The texture will use RLE (Run Length Encoding) EXR compression format (similar to Targa RLE compression).</para>
			/// </summary>
			CompressRLE = 4,
			/// <summary>
			///   <para>This texture will use Wavelet compression. This is best used for grainy images.</para>
			/// </summary>
			CompressPIZ = 8
		}
	}
}
