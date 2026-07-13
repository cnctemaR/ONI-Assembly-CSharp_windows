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
	[ExcludeFromPreset]
	[NativeHeader("Runtime/Graphics/CubemapTexture.h")]
	public sealed class Cubemap : Texture
	{
		public TextureFormat format
		{
			[NativeName("GetTextureFormat")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cubemap.get_format_Injected(intPtr);
			}
		}

		[FreeFunction("CubemapScripting::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_CreateImpl([Writable] Cubemap mono, int ext, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex);

		private static void Internal_Create([Writable] Cubemap mono, int ext, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex)
		{
			bool flag = !Cubemap.Internal_CreateImpl(mono, ext, mipCount, format, colorSpace, flags, nativeTex);
			if (flag)
			{
				throw new UnityException("Failed to create texture because of invalid parameters.");
			}
		}

		[FreeFunction(Name = "CubemapScripting::Apply", HasExplicitThis = true)]
		private void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Cubemap.ApplyImpl_Injected(intPtr, updateMipmaps, makeNoLongerReadable);
		}

		[FreeFunction("CubemapScripting::UpdateExternalTexture", HasExplicitThis = true)]
		public void UpdateExternalTexture(IntPtr nativeTexture)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Cubemap.UpdateExternalTexture_Injected(intPtr, nativeTexture);
		}

		public override bool isReadable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cubemap.get_isReadable_Injected(intPtr);
			}
		}

		[NativeName("SetPixel")]
		private void SetPixelImpl(int image, int mip, int x, int y, Color color)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Cubemap.SetPixelImpl_Injected(intPtr, image, mip, x, y, ref color);
		}

		[NativeName("GetPixel")]
		private Color GetPixelImpl(int image, int mip, int x, int y)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Color color;
			Cubemap.GetPixelImpl_Injected(intPtr, image, mip, x, y, out color);
			return color;
		}

		[NativeName("FixupEdges")]
		public void SmoothEdges([DefaultValue("1")] int smoothRegionWidthInPixels)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Cubemap.SmoothEdges_Injected(intPtr, smoothRegionWidthInPixels);
		}

		public void SmoothEdges()
		{
			this.SmoothEdges(1);
		}

		[FreeFunction(Name = "CubemapScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public Color[] GetPixels(CubemapFace face, int miplevel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Cubemap.GetPixels_Injected(intPtr, face, miplevel);
		}

		public Color[] GetPixels(CubemapFace face)
		{
			return this.GetPixels(face, 0);
		}

		[FreeFunction(Name = "CubemapScripting::SetPixels", HasExplicitThis = true, ThrowsException = true)]
		public unsafe void SetPixels(Color[] colors, CubemapFace face, int miplevel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Color> span = new Span<Color>(colors);
			fixed (Color* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Cubemap.SetPixels_Injected(intPtr, ref managedSpanWrapper, face, miplevel);
			}
		}

		[FreeFunction(Name = "CubemapScripting::SetPixelDataArray", HasExplicitThis = true, ThrowsException = true)]
		private bool SetPixelDataImplArray(Array data, int mipLevel, int face, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Cubemap.SetPixelDataImplArray_Injected(intPtr, data, mipLevel, face, elementSize, dataArraySize, sourceDataStartIndex);
		}

		[FreeFunction(Name = "CubemapScripting::SetPixelData", HasExplicitThis = true, ThrowsException = true)]
		private bool SetPixelDataImpl(IntPtr data, int mipLevel, int face, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Cubemap.SetPixelDataImpl_Injected(intPtr, data, mipLevel, face, elementSize, dataArraySize, sourceDataStartIndex);
		}

		public void SetPixels(Color[] colors, CubemapFace face)
		{
			this.SetPixels(colors, face, 0);
		}

		[FreeFunction(Name = "CubemapScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
		private void CopyPixels_Full(Texture src)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Cubemap.CopyPixels_Full_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(src));
		}

		[FreeFunction(Name = "CubemapScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
		private void CopyPixels_Slice(Texture src, int srcElement, int srcMip, int dstFace, int dstMip)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Cubemap.CopyPixels_Slice_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(src), srcElement, srcMip, dstFace, dstMip);
		}

		[FreeFunction(Name = "CubemapScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
		private void CopyPixels_Region(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstFace, int dstMip, int dstX, int dstY)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Cubemap.CopyPixels_Region_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(src), srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dstFace, dstMip, dstX, dstY);
		}

		private IntPtr GetWritableImageData(int frame)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Cubemap.GetWritableImageData_Injected(intPtr, frame);
		}

		internal bool isPreProcessed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cubemap.get_isPreProcessed_Injected(intPtr);
			}
		}

		public bool streamingMipmaps
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cubemap.get_streamingMipmaps_Injected(intPtr);
			}
		}

		public int streamingMipmapsPriority
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cubemap.get_streamingMipmapsPriority_Injected(intPtr);
			}
		}

		public int requestedMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetRequestedMipmapLevel", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cubemap.get_requestedMipmapLevel_Injected(intPtr);
			}
			[FreeFunction(Name = "GetTextureStreamingManager().SetRequestedMipmapLevel", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cubemap.set_requestedMipmapLevel_Injected(intPtr, value);
			}
		}

		internal bool loadAllMips
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetLoadAllMips", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cubemap.get_loadAllMips_Injected(intPtr);
			}
			[FreeFunction(Name = "GetTextureStreamingManager().SetLoadAllMips", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Cubemap.set_loadAllMips_Injected(intPtr, value);
			}
		}

		public int desiredMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetDesiredMipmapLevel", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cubemap.get_desiredMipmapLevel_Injected(intPtr);
			}
		}

		public int loadingMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetLoadingMipmapLevel", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cubemap.get_loadingMipmapLevel_Injected(intPtr);
			}
		}

		public int loadedMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetLoadedMipmapLevel", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Cubemap.get_loadedMipmapLevel_Injected(intPtr);
			}
		}

		[FreeFunction(Name = "GetTextureStreamingManager().ClearRequestedMipmapLevel", HasExplicitThis = true)]
		public void ClearRequestedMipmapLevel()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Cubemap.ClearRequestedMipmapLevel_Injected(intPtr);
		}

		[FreeFunction(Name = "GetTextureStreamingManager().IsRequestedMipmapLevelLoaded", HasExplicitThis = true)]
		public bool IsRequestedMipmapLevelLoaded()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Cubemap>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Cubemap.IsRequestedMipmapLevelLoaded_Injected(intPtr);
		}

		internal bool ValidateFormat(TextureFormat format, int width)
		{
			bool flag = base.ValidateFormat(format);
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = TextureFormat.PVRTC_RGB2 <= format && format <= TextureFormat.PVRTC_RGBA4;
				bool flag4 = flag3 && !Mathf.IsPowerOfTwo(width);
				if (flag4)
				{
					throw new UnityException(string.Format("'{0}' demands texture to have power-of-two dimensions", format.ToString()));
				}
			}
			return flag;
		}

		internal bool ValidateFormat(GraphicsFormat format, int width)
		{
			bool flag = base.ValidateFormat(format, GraphicsFormatUsage.Sample);
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = GraphicsFormatUtility.IsPVRTCFormat(format);
				bool flag4 = flag3 && !Mathf.IsPowerOfTwo(width);
				if (flag4)
				{
					throw new UnityException(string.Format("'{0}' demands texture to have power-of-two dimensions", format.ToString()));
				}
			}
			return flag;
		}

		[ExcludeFromDocs]
		public Cubemap(int width, DefaultFormat format, TextureCreationFlags flags)
			: this(width, SystemInfo.GetGraphicsFormat(format), flags)
		{
		}

		[ExcludeFromDocs]
		public Cubemap(int width, DefaultFormat format, TextureCreationFlags flags, int mipCount)
			: this(width, SystemInfo.GetGraphicsFormat(format), flags, mipCount)
		{
		}

		[ExcludeFromDocs]
		[RequiredByNativeCode]
		public Cubemap(int width, GraphicsFormat format, TextureCreationFlags flags)
			: this(width, format, flags, Texture.GenerateAllMips)
		{
		}

		[ExcludeFromDocs]
		public Cubemap(int width, GraphicsFormat format, TextureCreationFlags flags, int mipCount)
		{
			bool flag = !this.ValidateFormat(format, width);
			if (!flag)
			{
				Cubemap.ValidateIsNotCrunched(flags);
				Cubemap.Internal_Create(this, width, mipCount, format, base.GetTextureColorSpace(format), flags, IntPtr.Zero);
			}
		}

		internal Cubemap(int width, TextureFormat textureFormat, int mipCount, IntPtr nativeTex, bool createUninitialized)
		{
			bool flag = !this.ValidateFormat(textureFormat, width);
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
				Cubemap.ValidateIsNotCrunched(textureCreationFlags);
				Cubemap.Internal_Create(this, width, mipCount, graphicsFormat, base.GetTextureColorSpace(true), textureCreationFlags, nativeTex);
			}
		}

		public Cubemap(int width, TextureFormat textureFormat, bool mipChain)
			: this(width, textureFormat, mipChain ? Texture.GenerateAllMips : 1, IntPtr.Zero, false)
		{
		}

		public Cubemap(int width, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool createUninitialized)
			: this(width, textureFormat, mipChain ? Texture.GenerateAllMips : 1, IntPtr.Zero, createUninitialized)
		{
		}

		public Cubemap(int width, TextureFormat format, int mipCount)
			: this(width, format, mipCount, IntPtr.Zero, false)
		{
		}

		public Cubemap(int width, TextureFormat format, int mipCount, [DefaultValue("false")] bool createUninitialized)
			: this(width, format, mipCount, IntPtr.Zero, createUninitialized)
		{
		}

		public static Cubemap CreateExternalTexture(int width, TextureFormat format, bool mipmap, IntPtr nativeTex)
		{
			bool flag = nativeTex == IntPtr.Zero;
			if (flag)
			{
				throw new ArgumentException("nativeTex can not be null");
			}
			return new Cubemap(width, format, mipmap ? Texture.GenerateAllMips : 1, nativeTex, false);
		}

		public void SetPixelData<T>(T[] data, int mipLevel, CubemapFace face, [DefaultValue("0")] int sourceDataStartIndex = 0)
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
			this.SetPixelDataImplArray(data, mipLevel, (int)face, Marshal.SizeOf<T>(data[0]), data.Length, sourceDataStartIndex);
		}

		public void SetPixelData<T>(NativeArray<T> data, int mipLevel, CubemapFace face, [DefaultValue("0")] int sourceDataStartIndex = 0) where T : struct
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
			this.SetPixelDataImpl((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), mipLevel, (int)face, UnsafeUtility.SizeOf<T>(), data.Length, sourceDataStartIndex);
		}

		public unsafe NativeArray<T> GetPixelData<T>(int mipLevel, CubemapFace face) where T : struct
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
				throw new ArgumentException("The passed in face " + face.ToString() + " is invalid. The valid range is 0 through 5.");
			}
			bool flag4 = this.GetWritableImageData(0).ToInt64() == 0L;
			if (flag4)
			{
				throw new UnityException("Texture '" + base.name + "' has no data.");
			}
			ulong pixelDataOffset = base.GetPixelDataOffset(base.mipmapCount, (int)face);
			ulong pixelDataOffset2 = base.GetPixelDataOffset(mipLevel, (int)face);
			ulong pixelDataSize = base.GetPixelDataSize(mipLevel, (int)face);
			int num = UnsafeUtility.SizeOf<T>();
			ulong num2 = pixelDataSize / (ulong)((long)num);
			bool flag5 = num2 > 2147483647UL;
			if (flag5)
			{
				throw base.CreateNativeArrayLengthOverflowException();
			}
			IntPtr intPtr = new IntPtr((long)this.GetWritableImageData(0) + (long)(pixelDataOffset * (ulong)((long)face) + pixelDataOffset2));
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)intPtr, (int)num2, Allocator.None);
		}

		[ExcludeFromDocs]
		public void SetPixel(CubemapFace face, int x, int y, Color color)
		{
			this.SetPixel(face, x, y, color, 0);
		}

		public void SetPixel(CubemapFace face, int x, int y, Color color, [DefaultValue("0")] int mip)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelImpl((int)face, mip, x, y, color);
		}

		[ExcludeFromDocs]
		public Color GetPixel(CubemapFace face, int x, int y)
		{
			return this.GetPixel(face, x, y, 0);
		}

		public Color GetPixel(CubemapFace face, int x, int y, [DefaultValue("0")] int mip)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelImpl((int)face, mip, x, y);
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

		public void CopyPixels(Texture src, int srcElement, int srcMip, CubemapFace dstFace, int dstMip)
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
			this.CopyPixels_Slice(src, srcElement, srcMip, (int)dstFace, dstMip);
		}

		public void CopyPixels(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, CubemapFace dstFace, int dstMip, int dstX, int dstY)
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
			this.CopyPixels_Region(src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, (int)dstFace, dstMip, dstX, dstY);
		}

		private static void ValidateIsNotCrunched(TextureCreationFlags flags)
		{
			bool flag = (flags &= TextureCreationFlags.Crunch) > TextureCreationFlags.None;
			if (flag)
			{
				throw new ArgumentException("Crunched Cubemap is not supported for textures created from script.");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureFormat get_format_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ApplyImpl_Injected(IntPtr _unity_self, bool updateMipmaps, bool makeNoLongerReadable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateExternalTexture_Injected(IntPtr _unity_self, IntPtr nativeTexture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isReadable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPixelImpl_Injected(IntPtr _unity_self, int image, int mip, int x, int y, [In] ref Color color);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPixelImpl_Injected(IntPtr _unity_self, int image, int mip, int x, int y, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SmoothEdges_Injected(IntPtr _unity_self, [DefaultValue("1")] int smoothRegionWidthInPixels);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Color[] GetPixels_Injected(IntPtr _unity_self, CubemapFace face, int miplevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPixels_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, CubemapFace face, int miplevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetPixelDataImplArray_Injected(IntPtr _unity_self, Array data, int mipLevel, int face, int elementSize, int dataArraySize, int sourceDataStartIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetPixelDataImpl_Injected(IntPtr _unity_self, IntPtr data, int mipLevel, int face, int elementSize, int dataArraySize, int sourceDataStartIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyPixels_Full_Injected(IntPtr _unity_self, IntPtr src);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyPixels_Slice_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int dstFace, int dstMip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyPixels_Region_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstFace, int dstMip, int dstX, int dstY);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetWritableImageData_Injected(IntPtr _unity_self, int frame);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isPreProcessed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_streamingMipmaps_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_streamingMipmapsPriority_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_requestedMipmapLevel_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_requestedMipmapLevel_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_loadAllMips_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_loadAllMips_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_desiredMipmapLevel_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_loadingMipmapLevel_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_loadedMipmapLevel_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearRequestedMipmapLevel_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsRequestedMipmapLevelLoaded_Injected(IntPtr _unity_self);
	}
}
