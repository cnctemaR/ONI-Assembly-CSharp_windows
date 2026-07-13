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
	[NativeHeader("Runtime/Graphics/Texture2DArray.h")]
	public sealed class Texture2DArray : Texture
	{
		public static extern int allSlices
		{
			[NativeName("GetAllTextureLayersIdentifier")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public int depth
		{
			[NativeName("GetTextureLayerCount")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2DArray.get_depth_Injected(intPtr);
			}
		}

		public TextureFormat format
		{
			[NativeName("GetTextureFormat")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2DArray.get_format_Injected(intPtr);
			}
		}

		private bool IgnoreMipmapLimit()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2DArray.IgnoreMipmapLimit_Injected(intPtr);
		}

		private void SetIgnoreMipmapLimitAndReload(bool value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2DArray.SetIgnoreMipmapLimitAndReload_Injected(intPtr, value);
		}

		public string mipmapLimitGroup
		{
			[NativeName("GetMipmapLimitGroupName")]
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					Texture2DArray.get_mipmapLimitGroup_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public int activeMipmapLimit
		{
			[NativeName("GetMipmapLimit")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2DArray.get_activeMipmapLimit_Injected(intPtr);
			}
		}

		public override bool isReadable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2DArray.get_isReadable_Injected(intPtr);
			}
		}

		[FreeFunction("Texture2DArrayScripting::Create")]
		private unsafe static bool Internal_CreateImpl([Writable] Texture2DArray mono, int w, int h, int d, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, bool ignoreMipmapLimit, string mipmapLimitGroupName)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(mipmapLimitGroupName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = mipmapLimitGroupName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = Texture2DArray.Internal_CreateImpl_Injected(mono, w, h, d, mipCount, format, colorSpace, flags, ignoreMipmapLimit, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		private static void Internal_Create([Writable] Texture2DArray mono, int w, int h, int d, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, bool ignoreMipmapLimit, string mipmapLimitGroupName)
		{
			bool flag = !Texture2DArray.Internal_CreateImpl(mono, w, h, d, mipCount, format, colorSpace, flags, ignoreMipmapLimit, mipmapLimitGroupName);
			if (flag)
			{
				throw new UnityException("Failed to create 2D array texture because of invalid parameters.");
			}
		}

		[FreeFunction(Name = "Texture2DArrayScripting::Apply", HasExplicitThis = true)]
		private void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2DArray.ApplyImpl_Injected(intPtr, updateMipmaps, makeNoLongerReadable);
		}

		[FreeFunction(Name = "Texture2DArrayScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public Color[] GetPixels(int arrayElement, int miplevel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2DArray.GetPixels_Injected(intPtr, arrayElement, miplevel);
		}

		public Color[] GetPixels(int arrayElement)
		{
			return this.GetPixels(arrayElement, 0);
		}

		[FreeFunction(Name = "Texture2DArrayScripting::SetPixelDataArray", HasExplicitThis = true, ThrowsException = true)]
		private bool SetPixelDataImplArray(Array data, int mipLevel, int element, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2DArray.SetPixelDataImplArray_Injected(intPtr, data, mipLevel, element, elementSize, dataArraySize, sourceDataStartIndex);
		}

		[FreeFunction(Name = "Texture2DArrayScripting::SetPixelData", HasExplicitThis = true, ThrowsException = true)]
		private bool SetPixelDataImpl(IntPtr data, int mipLevel, int element, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2DArray.SetPixelDataImpl_Injected(intPtr, data, mipLevel, element, elementSize, dataArraySize, sourceDataStartIndex);
		}

		[FreeFunction(Name = "Texture2DArrayScripting::GetPixels32", HasExplicitThis = true, ThrowsException = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public Color32[] GetPixels32(int arrayElement, int miplevel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2DArray.GetPixels32_Injected(intPtr, arrayElement, miplevel);
		}

		public Color32[] GetPixels32(int arrayElement)
		{
			return this.GetPixels32(arrayElement, 0);
		}

		[FreeFunction(Name = "Texture2DArrayScripting::SetPixels", HasExplicitThis = true, ThrowsException = true)]
		public unsafe void SetPixels(Color[] colors, int arrayElement, int miplevel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Color> span = new Span<Color>(colors);
			fixed (Color* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Texture2DArray.SetPixels_Injected(intPtr, ref managedSpanWrapper, arrayElement, miplevel);
			}
		}

		public void SetPixels(Color[] colors, int arrayElement)
		{
			this.SetPixels(colors, arrayElement, 0);
		}

		[FreeFunction(Name = "Texture2DArrayScripting::SetPixels32", HasExplicitThis = true, ThrowsException = true)]
		public unsafe void SetPixels32(Color32[] colors, int arrayElement, int miplevel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Color32> span = new Span<Color32>(colors);
			fixed (Color32* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Texture2DArray.SetPixels32_Injected(intPtr, ref managedSpanWrapper, arrayElement, miplevel);
			}
		}

		public void SetPixels32(Color32[] colors, int arrayElement)
		{
			this.SetPixels32(colors, arrayElement, 0);
		}

		[FreeFunction(Name = "Texture2DArrayScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
		private void CopyPixels_Full(Texture src)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2DArray.CopyPixels_Full_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(src));
		}

		[FreeFunction(Name = "Texture2DArrayScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
		private void CopyPixels_Slice(Texture src, int srcElement, int srcMip, int dstElement, int dstMip)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2DArray.CopyPixels_Slice_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(src), srcElement, srcMip, dstElement, dstMip);
		}

		[FreeFunction(Name = "Texture2DArrayScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
		private void CopyPixels_Region(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstElement, int dstMip, int dstX, int dstY)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2DArray.CopyPixels_Region_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(src), srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dstElement, dstMip, dstX, dstY);
		}

		private IntPtr GetImageData()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2DArray>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2DArray.GetImageData_Injected(intPtr);
		}

		internal bool ValidateFormat(TextureFormat format, int width, int height)
		{
			bool flag = base.ValidateFormat(format);
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = TextureFormat.PVRTC_RGB2 <= format && format <= TextureFormat.PVRTC_RGBA4;
				bool flag4 = flag3 && (width != height || !Mathf.IsPowerOfTwo(width));
				if (flag4)
				{
					throw new UnityException(string.Format("'{0}' demands texture to be square and have power-of-two dimensions", format.ToString()));
				}
			}
			return flag;
		}

		internal bool ValidateFormat(GraphicsFormat format, int width, int height)
		{
			bool flag = base.ValidateFormat(format, GraphicsFormatUsage.Sample);
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = GraphicsFormatUtility.IsPVRTCFormat(format);
				bool flag4 = flag3 && (width != height || !Mathf.IsPowerOfTwo(width));
				if (flag4)
				{
					throw new UnityException(string.Format("'{0}' demands texture to be square and have power-of-two dimensions", format.ToString()));
				}
			}
			return flag;
		}

		[ExcludeFromDocs]
		public Texture2DArray(int width, int height, int depth, DefaultFormat format, TextureCreationFlags flags)
			: this(width, height, depth, SystemInfo.GetGraphicsFormat(format), flags)
		{
		}

		[ExcludeFromDocs]
		public Texture2DArray(int width, int height, int depth, DefaultFormat format, TextureCreationFlags flags, int mipCount)
			: this(width, height, depth, SystemInfo.GetGraphicsFormat(format), flags, mipCount)
		{
		}

		[ExcludeFromDocs]
		public Texture2DArray(int width, int height, int depth, DefaultFormat format, TextureCreationFlags flags, int mipCount, MipmapLimitDescriptor mipmapLimitDescriptor)
			: this(width, height, depth, SystemInfo.GetGraphicsFormat(format), flags, mipCount, mipmapLimitDescriptor)
		{
		}

		[RequiredByNativeCode]
		public Texture2DArray(int width, int height, int depth, GraphicsFormat format, TextureCreationFlags flags)
			: this(width, height, depth, format, flags, Texture.GenerateAllMips, default(MipmapLimitDescriptor))
		{
		}

		[ExcludeFromDocs]
		public Texture2DArray(int width, int height, int depth, GraphicsFormat format, TextureCreationFlags flags, int mipCount)
			: this(width, height, depth, format, flags, mipCount, default(MipmapLimitDescriptor))
		{
		}

		[ExcludeFromDocs]
		public Texture2DArray(int width, int height, int depth, GraphicsFormat format, TextureCreationFlags flags, int mipCount, MipmapLimitDescriptor mipmapLimitDescriptor)
		{
			bool flag = !this.ValidateFormat(format, width, height);
			if (!flag)
			{
				Texture2DArray.ValidateIsNotCrunched(flags);
				Texture2DArray.Internal_Create(this, width, height, depth, mipCount, format, base.GetTextureColorSpace(format), flags, !mipmapLimitDescriptor.useMipmapLimit, mipmapLimitDescriptor.groupName);
			}
		}

		public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, int mipCount, bool linear, bool createUninitialized, MipmapLimitDescriptor mipmapLimitDescriptor)
		{
			bool flag = !this.ValidateFormat(textureFormat, width, height);
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
				Texture2DArray.ValidateIsNotCrunched(textureCreationFlags);
				Texture2DArray.Internal_Create(this, width, height, depth, mipCount, graphicsFormat, base.GetTextureColorSpace(linear), textureCreationFlags, !mipmapLimitDescriptor.useMipmapLimit, mipmapLimitDescriptor.groupName);
			}
		}

		public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, int mipCount, bool linear, bool createUninitialized)
			: this(width, height, depth, textureFormat, mipCount, linear, createUninitialized, default(MipmapLimitDescriptor))
		{
		}

		public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, int mipCount, bool linear)
			: this(width, height, depth, textureFormat, mipCount, linear, false, default(MipmapLimitDescriptor))
		{
		}

		public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized)
			: this(width, height, depth, textureFormat, mipChain ? Texture.GenerateAllMips : 1, linear, createUninitialized, default(MipmapLimitDescriptor))
		{
		}

		public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool linear)
			: this(width, height, depth, textureFormat, mipChain ? Texture.GenerateAllMips : 1, linear)
		{
		}

		[ExcludeFromDocs]
		public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, bool mipChain)
			: this(width, height, depth, textureFormat, mipChain ? Texture.GenerateAllMips : 1, false)
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

		public void SetPixelData<T>(T[] data, int mipLevel, int element, [DefaultValue("0")] int sourceDataStartIndex = 0)
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
			this.SetPixelDataImplArray(data, mipLevel, element, Marshal.SizeOf<T>(data[0]), data.Length, sourceDataStartIndex);
		}

		public void SetPixelData<T>(NativeArray<T> data, int mipLevel, int element, [DefaultValue("0")] int sourceDataStartIndex = 0) where T : struct
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
			this.SetPixelDataImpl((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), mipLevel, element, UnsafeUtility.SizeOf<T>(), data.Length, sourceDataStartIndex);
		}

		public unsafe NativeArray<T> GetPixelData<T>(int mipLevel, int element) where T : struct
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
			bool flag3 = element < 0 || element >= this.depth;
			if (flag3)
			{
				throw new ArgumentException("The passed in element " + element.ToString() + " is invalid. The valid range is 0 through " + (this.depth - 1).ToString());
			}
			ulong pixelDataOffset = base.GetPixelDataOffset(base.mipmapCount, element);
			ulong pixelDataOffset2 = base.GetPixelDataOffset(mipLevel, element);
			ulong pixelDataSize = base.GetPixelDataSize(mipLevel, element);
			int num = UnsafeUtility.SizeOf<T>();
			ulong num2 = pixelDataSize / (ulong)((long)num);
			bool flag4 = num2 > 2147483647UL;
			if (flag4)
			{
				throw base.CreateNativeArrayLengthOverflowException();
			}
			IntPtr intPtr = new IntPtr((long)this.GetImageData() + (long)(pixelDataOffset * (ulong)((long)element) + pixelDataOffset2));
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)intPtr, (int)num2, Allocator.None);
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

		public bool ignoreMipmapLimit
		{
			get
			{
				return this.IgnoreMipmapLimit();
			}
			set
			{
				bool flag = !this.isReadable;
				if (flag)
				{
					throw base.IgnoreMipmapLimitCannotBeToggledException(this);
				}
				this.SetIgnoreMipmapLimitAndReload(value);
			}
		}

		private static void ValidateIsNotCrunched(TextureCreationFlags flags)
		{
			bool flag = (flags &= TextureCreationFlags.Crunch) > TextureCreationFlags.None;
			if (flag)
			{
				throw new ArgumentException("Crunched Texture2DArray is not supported.");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_depth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureFormat get_format_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IgnoreMipmapLimit_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIgnoreMipmapLimitAndReload_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_mipmapLimitGroup_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_activeMipmapLimit_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isReadable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_CreateImpl_Injected([Writable] Texture2DArray mono, int w, int h, int d, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, bool ignoreMipmapLimit, ref ManagedSpanWrapper mipmapLimitGroupName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ApplyImpl_Injected(IntPtr _unity_self, bool updateMipmaps, bool makeNoLongerReadable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Color[] GetPixels_Injected(IntPtr _unity_self, int arrayElement, int miplevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetPixelDataImplArray_Injected(IntPtr _unity_self, Array data, int mipLevel, int element, int elementSize, int dataArraySize, int sourceDataStartIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetPixelDataImpl_Injected(IntPtr _unity_self, IntPtr data, int mipLevel, int element, int elementSize, int dataArraySize, int sourceDataStartIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Color32[] GetPixels32_Injected(IntPtr _unity_self, int arrayElement, int miplevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPixels_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, int arrayElement, int miplevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPixels32_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, int arrayElement, int miplevel);

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
