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
	[NativeHeader("Runtime/Graphics/CubemapArrayTexture.h")]
	[ExcludeFromPreset]
	public sealed class CubemapArray : Texture
	{
		public int cubemapCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CubemapArray>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CubemapArray.get_cubemapCount_Injected(intPtr);
			}
		}

		public TextureFormat format
		{
			[NativeName("GetTextureFormat")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CubemapArray>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CubemapArray.get_format_Injected(intPtr);
			}
		}

		public override bool isReadable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CubemapArray>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CubemapArray.get_isReadable_Injected(intPtr);
			}
		}

		[FreeFunction("CubemapArrayScripting::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_CreateImpl([Writable] CubemapArray mono, int ext, int count, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags);

		private static void Internal_Create([Writable] CubemapArray mono, int ext, int count, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags)
		{
			bool flag = !CubemapArray.Internal_CreateImpl(mono, ext, count, mipCount, format, colorSpace, flags);
			if (flag)
			{
				throw new UnityException("Failed to create cubemap array texture because of invalid parameters.");
			}
		}

		[FreeFunction(Name = "CubemapArrayScripting::Apply", HasExplicitThis = true)]
		private void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CubemapArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CubemapArray.ApplyImpl_Injected(intPtr, updateMipmaps, makeNoLongerReadable);
		}

		[FreeFunction(Name = "CubemapArrayScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public Color[] GetPixels(CubemapFace face, int arrayElement, int miplevel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CubemapArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return CubemapArray.GetPixels_Injected(intPtr, face, arrayElement, miplevel);
		}

		public Color[] GetPixels(CubemapFace face, int arrayElement)
		{
			return this.GetPixels(face, arrayElement, 0);
		}

		[FreeFunction(Name = "CubemapArrayScripting::GetPixels32", HasExplicitThis = true, ThrowsException = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public Color32[] GetPixels32(CubemapFace face, int arrayElement, int miplevel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CubemapArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return CubemapArray.GetPixels32_Injected(intPtr, face, arrayElement, miplevel);
		}

		public Color32[] GetPixels32(CubemapFace face, int arrayElement)
		{
			return this.GetPixels32(face, arrayElement, 0);
		}

		[FreeFunction(Name = "CubemapArrayScripting::SetPixels", HasExplicitThis = true, ThrowsException = true)]
		public unsafe void SetPixels(Color[] colors, CubemapFace face, int arrayElement, int miplevel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CubemapArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Color> span = new Span<Color>(colors);
			fixed (Color* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CubemapArray.SetPixels_Injected(intPtr, ref managedSpanWrapper, face, arrayElement, miplevel);
			}
		}

		public void SetPixels(Color[] colors, CubemapFace face, int arrayElement)
		{
			this.SetPixels(colors, face, arrayElement, 0);
		}

		[FreeFunction(Name = "CubemapArrayScripting::SetPixels32", HasExplicitThis = true, ThrowsException = true)]
		public unsafe void SetPixels32(Color32[] colors, CubemapFace face, int arrayElement, int miplevel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CubemapArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Color32> span = new Span<Color32>(colors);
			fixed (Color32* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CubemapArray.SetPixels32_Injected(intPtr, ref managedSpanWrapper, face, arrayElement, miplevel);
			}
		}

		public void SetPixels32(Color32[] colors, CubemapFace face, int arrayElement)
		{
			this.SetPixels32(colors, face, arrayElement, 0);
		}

		[FreeFunction(Name = "CubemapArrayScripting::SetPixelDataArray", HasExplicitThis = true, ThrowsException = true)]
		private bool SetPixelDataImplArray(Array data, int mipLevel, int face, int element, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CubemapArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return CubemapArray.SetPixelDataImplArray_Injected(intPtr, data, mipLevel, face, element, elementSize, dataArraySize, sourceDataStartIndex);
		}

		[FreeFunction(Name = "CubemapArrayScripting::SetPixelData", HasExplicitThis = true, ThrowsException = true)]
		private bool SetPixelDataImpl(IntPtr data, int mipLevel, int face, int element, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CubemapArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return CubemapArray.SetPixelDataImpl_Injected(intPtr, data, mipLevel, face, element, elementSize, dataArraySize, sourceDataStartIndex);
		}

		[FreeFunction(Name = "CubemapArrayScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
		private void CopyPixels_Full(Texture src)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CubemapArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CubemapArray.CopyPixels_Full_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(src));
		}

		[FreeFunction(Name = "CubemapArrayScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
		private void CopyPixels_Slice(Texture src, int srcElement, int srcMip, int dstElement, int dstMip)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CubemapArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CubemapArray.CopyPixels_Slice_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(src), srcElement, srcMip, dstElement, dstMip);
		}

		[FreeFunction(Name = "CubemapArrayScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
		private void CopyPixels_Region(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstElement, int dstMip, int dstX, int dstY)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CubemapArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CubemapArray.CopyPixels_Region_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(src), srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dstElement, dstMip, dstX, dstY);
		}

		private IntPtr GetImageData()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CubemapArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return CubemapArray.GetImageData_Injected(intPtr);
		}

		[ExcludeFromDocs]
		public CubemapArray(int width, int cubemapCount, DefaultFormat format, TextureCreationFlags flags)
			: this(width, cubemapCount, SystemInfo.GetGraphicsFormat(format), flags)
		{
		}

		[ExcludeFromDocs]
		public CubemapArray(int width, int cubemapCount, DefaultFormat format, TextureCreationFlags flags, [DefaultValue("Texture.GenerateAllMips")] int mipCount)
			: this(width, cubemapCount, SystemInfo.GetGraphicsFormat(format), flags, mipCount)
		{
		}

		[RequiredByNativeCode]
		public CubemapArray(int width, int cubemapCount, GraphicsFormat format, TextureCreationFlags flags)
			: this(width, cubemapCount, format, flags, Texture.GenerateAllMips)
		{
		}

		[ExcludeFromDocs]
		public CubemapArray(int width, int cubemapCount, GraphicsFormat format, TextureCreationFlags flags, [DefaultValue("Texture.GenerateAllMips")] int mipCount)
		{
			bool flag = !base.ValidateFormat(format, GraphicsFormatUsage.Sample);
			if (!flag)
			{
				CubemapArray.ValidateIsNotCrunched(flags);
				CubemapArray.Internal_Create(this, width, cubemapCount, mipCount, format, base.GetTextureColorSpace(format), flags);
			}
		}

		public CubemapArray(int width, int cubemapCount, TextureFormat textureFormat, int mipCount, bool linear, [DefaultValue("false")] bool createUninitialized)
		{
			bool flag = !base.ValidateFormat(textureFormat);
			if (!flag)
			{
				GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(textureFormat, !linear);
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
				CubemapArray.ValidateIsNotCrunched(textureCreationFlags);
				CubemapArray.Internal_Create(this, width, cubemapCount, mipCount, graphicsFormat, base.GetTextureColorSpace(linear), textureCreationFlags);
			}
		}

		public CubemapArray(int width, int cubemapCount, TextureFormat textureFormat, int mipCount, bool linear)
			: this(width, cubemapCount, textureFormat, mipCount, linear, false)
		{
		}

		public CubemapArray(int width, int cubemapCount, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized)
			: this(width, cubemapCount, textureFormat, mipChain ? Texture.GenerateAllMips : 1, linear, createUninitialized)
		{
		}

		[ExcludeFromDocs]
		public CubemapArray(int width, int cubemapCount, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool linear)
			: this(width, cubemapCount, textureFormat, mipChain ? Texture.GenerateAllMips : 1, linear)
		{
		}

		public CubemapArray(int width, int cubemapCount, TextureFormat textureFormat, bool mipChain)
			: this(width, cubemapCount, textureFormat, mipChain ? Texture.GenerateAllMips : 1, false)
		{
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

		public void SetPixelData<T>(T[] data, int mipLevel, CubemapFace face, int element, [DefaultValue("0")] int sourceDataStartIndex = 0)
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
			this.SetPixelDataImplArray(data, mipLevel, (int)face, element, Marshal.SizeOf<T>(data[0]), data.Length, sourceDataStartIndex);
		}

		public void SetPixelData<T>(NativeArray<T> data, int mipLevel, CubemapFace face, int element, [DefaultValue("0")] int sourceDataStartIndex = 0) where T : struct
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
			this.SetPixelDataImpl((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), mipLevel, (int)face, element, UnsafeUtility.SizeOf<T>(), data.Length, sourceDataStartIndex);
		}

		public unsafe NativeArray<T> GetPixelData<T>(int mipLevel, CubemapFace face, int element) where T : struct
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag2 = mipLevel < 0 || mipLevel >= base.mipmapCount;
			if (flag2)
			{
				throw new ArgumentException("The passed in miplevel " + mipLevel.ToString() + " is invalid. The valid range is 0 through " + (base.mipmapCount - 1).ToString());
			}
			bool flag3 = face < CubemapFace.PositiveX || face >= (CubemapFace)6;
			if (flag3)
			{
				throw new ArgumentException("The passed in face " + face.ToString() + " is invalid.  The valid range is 0 through 5");
			}
			bool flag4 = element < 0 || element >= this.cubemapCount;
			if (flag4)
			{
				throw new ArgumentException("The passed in element " + element.ToString() + " is invalid. The valid range is 0 through " + (this.cubemapCount - 1).ToString());
			}
			int num = (int)(element * 6 + face);
			ulong pixelDataOffset = base.GetPixelDataOffset(base.mipmapCount, num);
			ulong pixelDataOffset2 = base.GetPixelDataOffset(mipLevel, num);
			ulong pixelDataSize = base.GetPixelDataSize(mipLevel, num);
			int num2 = UnsafeUtility.SizeOf<T>();
			ulong num3 = pixelDataSize / (ulong)((long)num2);
			bool flag5 = num3 > 2147483647UL;
			if (flag5)
			{
				throw base.CreateNativeArrayLengthOverflowException();
			}
			IntPtr intPtr = new IntPtr((long)this.GetImageData() + (long)(pixelDataOffset * (ulong)((long)num) + pixelDataOffset2));
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)intPtr, (int)num3, Allocator.None);
		}

		public void CopyPixels(Texture src)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag2 = !src.isReadable;
			if (flag2)
			{
				throw base.CreateNonReadableException(src);
			}
			this.CopyPixels_Full(src);
		}

		public void CopyPixels(Texture src, int srcElement, int srcMip, int dstElement, int dstMip)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag2 = !src.isReadable;
			if (flag2)
			{
				throw base.CreateNonReadableException(src);
			}
			this.CopyPixels_Slice(src, srcElement, srcMip, dstElement, dstMip);
		}

		public void CopyPixels(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstElement, int dstMip, int dstX, int dstY)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag2 = !src.isReadable;
			if (flag2)
			{
				throw base.CreateNonReadableException(src);
			}
			this.CopyPixels_Region(src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dstElement, dstMip, dstX, dstY);
		}

		private static void ValidateIsNotCrunched(TextureCreationFlags flags)
		{
			bool flag = (flags &= TextureCreationFlags.Crunch) > TextureCreationFlags.None;
			if (flag)
			{
				throw new ArgumentException("Crunched TextureCubeArray is not supported.");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_cubemapCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureFormat get_format_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isReadable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ApplyImpl_Injected(IntPtr _unity_self, bool updateMipmaps, bool makeNoLongerReadable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Color[] GetPixels_Injected(IntPtr _unity_self, CubemapFace face, int arrayElement, int miplevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Color32[] GetPixels32_Injected(IntPtr _unity_self, CubemapFace face, int arrayElement, int miplevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPixels_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, CubemapFace face, int arrayElement, int miplevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPixels32_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, CubemapFace face, int arrayElement, int miplevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetPixelDataImplArray_Injected(IntPtr _unity_self, Array data, int mipLevel, int face, int element, int elementSize, int dataArraySize, int sourceDataStartIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetPixelDataImpl_Injected(IntPtr _unity_self, IntPtr data, int mipLevel, int face, int element, int elementSize, int dataArraySize, int sourceDataStartIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyPixels_Full_Injected(IntPtr _unity_self, IntPtr src);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyPixels_Slice_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int dstElement, int dstMip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyPixels_Region_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstElement, int dstMip, int dstX, int dstY);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetImageData_Injected(IntPtr _unity_self);
	}
}
