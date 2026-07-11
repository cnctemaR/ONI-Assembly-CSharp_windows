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
	[NativeHeader("Runtime/Graphics/GeneratedTextures.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Graphics/Texture2D.h")]
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
				Texture2D.Internal_Create(this, width, height, graphicsFormat, textureCreationFlags, nativeTex);
			}
		}

		public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("true")] bool mipChain, [DefaultValue("false")] bool linear)
			: this(width, height, textureFormat, mipChain, linear, IntPtr.Zero)
		{
		}

		public Texture2D(int width, int height, TextureFormat textureFormat, bool mipChain)
			: this(width, height, textureFormat, mipChain, false, IntPtr.Zero)
		{
		}

		public Texture2D(int width, int height)
			: this(width, height, TextureFormat.RGBA32, true, false, IntPtr.Zero)
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

		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static extern Texture2D whiteTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static extern Texture2D blackTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

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

		public override extern bool isReadable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

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

		public extern bool streamingMipmaps
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int streamingMipmapsPriority
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int requestedMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetRequestedMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction(Name = "GetTextureStreamingManager().SetRequestedMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int desiredMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetDesiredMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int loadingMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetLoadingMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int loadedMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetLoadedMipmapLevel", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction(Name = "GetTextureStreamingManager().ClearRequestedMipmapLevel", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearRequestedMipmapLevel();

		[FreeFunction(Name = "GetTextureStreamingManager().IsRequestedMipmapLevelLoaded", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsRequestedMipmapLevelLoaded();

		[FreeFunction("Texture2DScripting::UpdateExternalTexture", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateExternalTexture(IntPtr nativeTex);

		[FreeFunction("Texture2DScripting::SetAllPixels32", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetAllPixels32(Color32[] colors, int miplevel);

		[FreeFunction("Texture2DScripting::SetBlockOfPixels32", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetBlockOfPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, int miplevel);

		[FreeFunction("Texture2DScripting::GetRawTextureData", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern byte[] GetRawTextureData();

		[FreeFunction("Texture2DScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color[] GetPixels(int x, int y, int blockWidth, int blockHeight, int miplevel);

		public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight)
		{
			return this.GetPixels(x, y, blockWidth, blockHeight, 0);
		}

		[FreeFunction("Texture2DScripting::GetPixels32", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Color32[] GetPixels32(int miplevel);

		public Color32[] GetPixels32()
		{
			return this.GetPixels32(0);
		}

		[FreeFunction("Texture2DScripting::PackTextures", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize, bool makeNoLongerReadable);

		public Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize)
		{
			return this.PackTextures(textures, padding, maximumAtlasSize, false);
		}

		public Rect[] PackTextures(Texture2D[] textures, int padding)
		{
			return this.PackTextures(textures, padding, 2048);
		}

		public static Texture2D CreateExternalTexture(int width, int height, TextureFormat format, bool mipChain, bool linear, IntPtr nativeTex)
		{
			if (nativeTex == IntPtr.Zero)
			{
				throw new ArgumentException("nativeTex can not be null");
			}
			return new Texture2D(width, height, format, mipChain, linear, nativeTex);
		}

		public void SetPixel(int x, int y, Color color)
		{
			if (!this.isReadable)
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelImpl(0, x, y, color);
		}

		public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, [DefaultValue("0")] int miplevel)
		{
			if (!this.isReadable)
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelsImpl(x, y, blockWidth, blockHeight, colors, miplevel, 0);
		}

		public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors)
		{
			this.SetPixels(x, y, blockWidth, blockHeight, colors, 0);
		}

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

		public Color GetPixel(int x, int y)
		{
			if (!this.isReadable)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelImpl(0, x, y);
		}

		public Color GetPixelBilinear(float x, float y)
		{
			if (!this.isReadable)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelBilinearImpl(0, x, y);
		}

		public void LoadRawTextureData(IntPtr data, int size)
		{
			if (!this.isReadable)
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

		public void LoadRawTextureData(byte[] data)
		{
			if (!this.isReadable)
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
			if (!this.isReadable)
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
			if (!this.isReadable)
			{
				throw base.CreateNonReadableException(this);
			}
			int num = UnsafeUtility.SizeOf<T>();
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)this.GetWritableImageData(0), (int)(this.GetRawImageDataSize() / (long)num), Allocator.None);
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

		public bool Resize(int width, int height)
		{
			if (!this.isReadable)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.ResizeImpl(width, height);
		}

		public bool Resize(int width, int height, TextureFormat format, bool hasMipMap)
		{
			if (!this.isReadable)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.ResizeWithFormatImpl(width, height, format, hasMipMap);
		}

		public void ReadPixels(Rect source, int destX, int destY, [DefaultValue("true")] bool recalculateMipMaps)
		{
			if (!this.isReadable)
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

		public void SetPixels32(Color32[] colors, int miplevel)
		{
			this.SetAllPixels32(colors, miplevel);
		}

		public void SetPixels32(Color32[] colors)
		{
			this.SetPixels32(colors, 0);
		}

		public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, int miplevel)
		{
			this.SetBlockOfPixels32(x, y, blockWidth, blockHeight, colors, miplevel);
		}

		public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors)
		{
			this.SetPixels32(x, y, blockWidth, blockHeight, colors, 0);
		}

		public Color[] GetPixels(int miplevel)
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

		public Color[] GetPixels()
		{
			return this.GetPixels(0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetPixelImpl_Injected(int image, int x, int y, ref Color color);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPixelImpl_Injected(int image, int x, int y, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPixelBilinearImpl_Injected(int image, float x, float y, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ReadPixelsImpl_Injected(ref Rect source, int destX, int destY, bool recalculateMipMaps);

		[Flags]
		public enum EXRFlags
		{
			None = 0,
			OutputAsFloat = 1,
			CompressZIP = 2,
			CompressRLE = 4,
			CompressPIZ = 8
		}
	}
}
