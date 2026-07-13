using System;
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
	[NativeHeader("Runtime/Graphics/Texture3D.h")]
	[ExcludeFromPreset]
	public sealed class Texture3D : Texture
	{
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

		[NativeName("SetPixel")]
		private void SetPixelImpl(int mip, int x, int y, int z, Color color)
		{
			this.SetPixelImpl_Injected(mip, x, y, z, ref color);
		}

		[NativeName("GetPixel")]
		private Color GetPixelImpl(int mip, int x, int y, int z)
		{
			Color color;
			this.GetPixelImpl_Injected(mip, x, y, z, out color);
			return color;
		}

		[NativeName("GetPixelBilinear")]
		private Color GetPixelBilinearImpl(int mip, float u, float v, float w)
		{
			Color color;
			this.GetPixelBilinearImpl_Injected(mip, u, v, w, out color);
			return color;
		}

		[FreeFunction("Texture3DScripting::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_CreateImpl([Writable] Texture3D mono, int w, int h, int d, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex);

		private static void Internal_Create([Writable] Texture3D mono, int w, int h, int d, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex)
		{
			bool flag = !Texture3D.Internal_CreateImpl(mono, w, h, d, mipCount, format, colorSpace, flags, nativeTex);
			if (flag)
			{
				throw new UnityException("Failed to create texture because of invalid parameters.");
			}
		}

		[FreeFunction("Texture3DScripting::UpdateExternalTexture", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UpdateExternalTexture(IntPtr nativeTex);

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
		public extern void SetPixels([Unmarshalled] Color[] colors, int miplevel);

		public void SetPixels(Color[] colors)
		{
			this.SetPixels(colors, 0);
		}

		[FreeFunction(Name = "Texture3DScripting::SetPixels32", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPixels32([Unmarshalled] Color32[] colors, int miplevel);

		public void SetPixels32(Color32[] colors)
		{
			this.SetPixels32(colors, 0);
		}

		[FreeFunction(Name = "Texture3DScripting::SetPixelDataArray", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool SetPixelDataImplArray(Array data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex = 0);

		[FreeFunction(Name = "Texture3DScripting::SetPixelData", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool SetPixelDataImpl(IntPtr data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex = 0);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr GetImageDataPointer();

		[ExcludeFromDocs]
		public Texture3D(int width, int height, int depth, DefaultFormat format, TextureCreationFlags flags)
			: this(width, height, depth, SystemInfo.GetGraphicsFormat(format), flags)
		{
		}

		[ExcludeFromDocs]
		public Texture3D(int width, int height, int depth, DefaultFormat format, TextureCreationFlags flags, int mipCount)
			: this(width, height, depth, SystemInfo.GetGraphicsFormat(format), flags, mipCount)
		{
		}

		[RequiredByNativeCode]
		[ExcludeFromDocs]
		public Texture3D(int width, int height, int depth, GraphicsFormat format, TextureCreationFlags flags)
			: this(width, height, depth, format, flags, Texture.GenerateAllMips)
		{
		}

		[ExcludeFromDocs]
		public Texture3D(int width, int height, int depth, GraphicsFormat format, TextureCreationFlags flags, [DefaultValue("Texture.GenerateAllMips")] int mipCount)
		{
			bool flag = !base.ValidateFormat(format, FormatUsage.Sample);
			if (!flag)
			{
				Texture3D.ValidateIsNotCrunched(flags);
				Texture3D.Internal_Create(this, width, height, depth, mipCount, format, base.GetTextureColorSpace(format), flags, IntPtr.Zero);
			}
		}

		[ExcludeFromDocs]
		public Texture3D(int width, int height, int depth, TextureFormat textureFormat, int mipCount)
			: this(width, height, depth, textureFormat, mipCount, IntPtr.Zero)
		{
		}

		public Texture3D(int width, int height, int depth, TextureFormat textureFormat, int mipCount, [DefaultValue("IntPtr.Zero")] IntPtr nativeTex)
			: this(width, height, depth, textureFormat, mipCount, nativeTex, false)
		{
		}

		public Texture3D(int width, int height, int depth, TextureFormat textureFormat, int mipCount, [DefaultValue("IntPtr.Zero")] IntPtr nativeTex, [DefaultValue("false")] bool createUninitialized)
		{
			bool flag = !base.ValidateFormat(textureFormat);
			if (!flag)
			{
				GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(textureFormat, false);
				TextureCreationFlags textureCreationFlags = ((mipCount != 1) ? TextureCreationFlags.MipChain : TextureCreationFlags.None);
				bool flag2 = GraphicsFormatUtility.IsCrunchFormat(textureFormat);
				if (flag2)
				{
					textureCreationFlags |= TextureCreationFlags.Crunch;
				}
				if (createUninitialized)
				{
					textureCreationFlags |= TextureCreationFlags.DontInitializePixels | TextureCreationFlags.DontUploadUponCreate;
				}
				Texture3D.ValidateIsNotCrunched(textureCreationFlags);
				Texture3D.Internal_Create(this, width, height, depth, mipCount, graphicsFormat, base.GetTextureColorSpace(true), textureCreationFlags, nativeTex);
			}
		}

		[ExcludeFromDocs]
		public Texture3D(int width, int height, int depth, TextureFormat textureFormat, bool mipChain)
			: this(width, height, depth, textureFormat, mipChain ? Texture.GenerateAllMips : 1)
		{
		}

		public Texture3D(int width, int height, int depth, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool createUninitialized)
			: this(width, height, depth, textureFormat, mipChain ? Texture.GenerateAllMips : 1, IntPtr.Zero, createUninitialized)
		{
		}

		public Texture3D(int width, int height, int depth, TextureFormat textureFormat, bool mipChain, [DefaultValue("IntPtr.Zero")] IntPtr nativeTex)
			: this(width, height, depth, textureFormat, mipChain ? Texture.GenerateAllMips : 1, nativeTex)
		{
		}

		public static Texture3D CreateExternalTexture(int width, int height, int depth, TextureFormat format, bool mipChain, IntPtr nativeTex)
		{
			bool flag = nativeTex == IntPtr.Zero;
			if (flag)
			{
				throw new ArgumentException("nativeTex may not be zero");
			}
			return new Texture3D(width, height, depth, format, mipChain ? (-1) : 1, nativeTex, false);
		}

		public void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			this.ApplyImpl(updateMipmaps, makeNoLongerReadable);
		}

		[ExcludeFromDocs]
		public void Apply(bool updateMipmaps)
		{
			this.Apply(updateMipmaps, false);
		}

		[ExcludeFromDocs]
		public void Apply()
		{
			this.Apply(true, false);
		}

		[ExcludeFromDocs]
		public void SetPixel(int x, int y, int z, Color color)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelImpl(0, x, y, z, color);
		}

		public void SetPixel(int x, int y, int z, Color color, [DefaultValue("0")] int mipLevel)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelImpl(mipLevel, x, y, z, color);
		}

		[ExcludeFromDocs]
		public Color GetPixel(int x, int y, int z)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelImpl(0, x, y, z);
		}

		public Color GetPixel(int x, int y, int z, [DefaultValue("0")] int mipLevel)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelImpl(mipLevel, x, y, z);
		}

		[ExcludeFromDocs]
		public Color GetPixelBilinear(float u, float v, float w)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelBilinearImpl(0, u, v, w);
		}

		public Color GetPixelBilinear(float u, float v, float w, [DefaultValue("0")] int mipLevel)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelBilinearImpl(mipLevel, u, v, w);
		}

		public void SetPixelData<T>(T[] data, int mipLevel, [DefaultValue("0")] int sourceDataStartIndex = 0)
		{
			bool flag = sourceDataStartIndex < 0;
			if (flag)
			{
				throw new UnityException("SetPixelData: sourceDataStartIndex cannot be less than 0.");
			}
			bool flag2 = !this.isReadable;
			if (flag2)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag3 = data == null || data.Length == 0;
			if (flag3)
			{
				throw new UnityException("No texture data provided to SetPixelData.");
			}
			this.SetPixelDataImplArray(data, mipLevel, Marshal.SizeOf<T>(data[0]), data.Length, sourceDataStartIndex);
		}

		public void SetPixelData<T>(NativeArray<T> data, int mipLevel, [DefaultValue("0")] int sourceDataStartIndex = 0) where T : struct
		{
			bool flag = sourceDataStartIndex < 0;
			if (flag)
			{
				throw new UnityException("SetPixelData: sourceDataStartIndex cannot be less than 0.");
			}
			bool flag2 = !this.isReadable;
			if (flag2)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag3 = !data.IsCreated || data.Length == 0;
			if (flag3)
			{
				throw new UnityException("No texture data provided to SetPixelData.");
			}
			this.SetPixelDataImpl((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), mipLevel, UnsafeUtility.SizeOf<T>(), data.Length, sourceDataStartIndex);
		}

		public unsafe NativeArray<T> GetPixelData<T>(int mipLevel) where T : struct
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag2 = mipLevel < 0 || mipLevel >= base.mipmapCount;
			if (flag2)
			{
				throw new ArgumentException("The passed in miplevel " + mipLevel.ToString() + " is invalid. The valid range is 0 through  " + (base.mipmapCount - 1).ToString());
			}
			bool flag3 = this.GetImageDataPointer().ToInt64() == 0L;
			if (flag3)
			{
				throw new UnityException("Texture '" + base.name + "' has no data.");
			}
			ulong pixelDataOffset = base.GetPixelDataOffset(mipLevel, 0);
			ulong pixelDataSize = base.GetPixelDataSize(mipLevel, 0);
			int num = UnsafeUtility.SizeOf<T>();
			ulong num2 = pixelDataSize / (ulong)((long)num);
			bool flag4 = num2 > 2147483647UL;
			if (flag4)
			{
				throw base.CreateNativeArrayLengthOverflowException();
			}
			IntPtr intPtr = new IntPtr((long)this.GetImageDataPointer() + (long)pixelDataOffset);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)intPtr, (int)num2, Allocator.None);
		}

		private static void ValidateIsNotCrunched(TextureCreationFlags flags)
		{
			bool flag = (flags &= TextureCreationFlags.Crunch) > TextureCreationFlags.None;
			if (flag)
			{
				throw new ArgumentException("Crunched Texture3D is not supported.");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetPixelImpl_Injected(int mip, int x, int y, int z, ref Color color);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPixelImpl_Injected(int mip, int x, int y, int z, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPixelBilinearImpl_Injected(int mip, float u, float v, float w, out Color ret);
	}
}
