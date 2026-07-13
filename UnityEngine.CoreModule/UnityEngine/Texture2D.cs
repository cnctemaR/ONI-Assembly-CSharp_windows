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
	[HelpURL("texture-type-default")]
	[NativeHeader("Runtime/Graphics/Texture2D.h")]
	[UsedByNativeCode]
	[ExcludeFromPreset]
	[NativeHeader("Runtime/Graphics/GeneratedTextures.h")]
	public sealed class Texture2D : Texture
	{
		public TextureFormat format
		{
			[NativeName("GetTextureFormat")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2D.get_format_Injected(intPtr);
			}
		}

		private bool IgnoreMipmapLimit()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2D.IgnoreMipmapLimit_Injected(intPtr);
		}

		private void SetIgnoreMipmapLimitAndReload(bool value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2D.SetIgnoreMipmapLimitAndReload_Injected(intPtr, value);
		}

		public string mipmapLimitGroup
		{
			[NativeName("GetMipmapLimitGroupName")]
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					Texture2D.get_mipmapLimitGroup_Injected(intPtr, out managedSpanWrapper);
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
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2D.get_activeMipmapLimit_Injected(intPtr);
			}
		}

		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static Texture2D whiteTexture
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Texture2D>(Texture2D.get_whiteTexture_Injected());
			}
		}

		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static Texture2D blackTexture
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Texture2D>(Texture2D.get_blackTexture_Injected());
			}
		}

		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static Texture2D redTexture
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Texture2D>(Texture2D.get_redTexture_Injected());
			}
		}

		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static Texture2D grayTexture
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Texture2D>(Texture2D.get_grayTexture_Injected());
			}
		}

		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static Texture2D linearGrayTexture
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Texture2D>(Texture2D.get_linearGrayTexture_Injected());
			}
		}

		[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
		public static Texture2D normalTexture
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Texture2D>(Texture2D.get_normalTexture_Injected());
			}
		}

		public void Compress(bool highQuality)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2D.Compress_Injected(intPtr, highQuality);
		}

		[FreeFunction("Texture2DScripting::CreateEmpty")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_CreateEmptyImpl([Writable] Texture2D mono);

		[FreeFunction("Texture2DScripting::Create")]
		private unsafe static bool Internal_CreateImpl([Writable] Texture2D mono, int w, int h, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex, bool ignoreMipmapLimit, string mipmapLimitGroupName)
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
				flag = Texture2D.Internal_CreateImpl_Injected(mono, w, h, mipCount, format, colorSpace, flags, nativeTex, ignoreMipmapLimit, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		private static void Internal_Create([Writable] Texture2D mono, int w, int h, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex, bool ignoreMipmapLimit, string mipmapLimitGroupName)
		{
			bool flag = !Texture2D.Internal_CreateImpl(mono, w, h, mipCount, format, colorSpace, flags, nativeTex, ignoreMipmapLimit, mipmapLimitGroupName);
			if (flag)
			{
				throw new UnityException("Failed to create texture because of invalid parameters.");
			}
		}

		public override bool isReadable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2D.get_isReadable_Injected(intPtr);
			}
		}

		[NativeConditional("ENABLE_VIRTUALTEXTURING && UNITY_EDITOR")]
		[NativeName("VTOnly")]
		public bool vtOnly
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2D.get_vtOnly_Injected(intPtr);
			}
		}

		[NativeName("Apply")]
		private void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2D.ApplyImpl_Injected(intPtr, updateMipmaps, makeNoLongerReadable);
		}

		[NativeName("Reinitialize")]
		private bool ReinitializeImpl(int width, int height)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2D.ReinitializeImpl_Injected(intPtr, width, height);
		}

		[NativeName("SetPixel")]
		private void SetPixelImpl(int image, int mip, int x, int y, Color color)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2D.SetPixelImpl_Injected(intPtr, image, mip, x, y, ref color);
		}

		[NativeName("GetPixel")]
		private Color GetPixelImpl(int image, int mip, int x, int y)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Color color;
			Texture2D.GetPixelImpl_Injected(intPtr, image, mip, x, y, out color);
			return color;
		}

		[NativeName("GetPixelBilinear")]
		private Color GetPixelBilinearImpl(int image, int mip, float u, float v)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Color color;
			Texture2D.GetPixelBilinearImpl_Injected(intPtr, image, mip, u, v, out color);
			return color;
		}

		[FreeFunction(Name = "Texture2DScripting::ReinitializeWithFormat", HasExplicitThis = true)]
		private bool ReinitializeWithFormatImpl(int width, int height, GraphicsFormat format, bool hasMipMap)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2D.ReinitializeWithFormatImpl_Injected(intPtr, width, height, format, hasMipMap);
		}

		[FreeFunction(Name = "Texture2DScripting::ReinitializeWithTextureFormat", HasExplicitThis = true)]
		private bool ReinitializeWithTextureFormatImpl(int width, int height, TextureFormat textureFormat, bool hasMipMap)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2D.ReinitializeWithTextureFormatImpl_Injected(intPtr, width, height, textureFormat, hasMipMap);
		}

		[FreeFunction(Name = "Texture2DScripting::ReadPixels", HasExplicitThis = true)]
		private void ReadPixelsImpl(Rect source, int destX, int destY, bool recalculateMipMaps)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2D.ReadPixelsImpl_Injected(intPtr, ref source, destX, destY, recalculateMipMaps);
		}

		[FreeFunction(Name = "Texture2DScripting::SetPixels", HasExplicitThis = true, ThrowsException = true)]
		private unsafe void SetPixelsImpl(int x, int y, int w, int h, Color[] pixel, int miplevel, int frame)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Color> span = new Span<Color>(pixel);
			fixed (Color* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Texture2D.SetPixelsImpl_Injected(intPtr, x, y, w, h, ref managedSpanWrapper, miplevel, frame);
			}
		}

		[FreeFunction(Name = "Texture2DScripting::LoadRawData", HasExplicitThis = true)]
		private bool LoadRawTextureDataImpl(IntPtr data, ulong size)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2D.LoadRawTextureDataImpl_Injected(intPtr, data, size);
		}

		[FreeFunction(Name = "Texture2DScripting::LoadRawData", HasExplicitThis = true)]
		private unsafe bool LoadRawTextureDataImplArray(byte[] data)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<byte> span = new Span<byte>(data);
			bool flag;
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				flag = Texture2D.LoadRawTextureDataImplArray_Injected(intPtr, ref managedSpanWrapper);
			}
			return flag;
		}

		[FreeFunction(Name = "Texture2DScripting::SetPixelDataArray", HasExplicitThis = true, ThrowsException = true)]
		private bool SetPixelDataImplArray(Array data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2D.SetPixelDataImplArray_Injected(intPtr, data, mipLevel, elementSize, dataArraySize, sourceDataStartIndex);
		}

		[FreeFunction(Name = "Texture2DScripting::SetPixelData", HasExplicitThis = true, ThrowsException = true)]
		private bool SetPixelDataImpl(IntPtr data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2D.SetPixelDataImpl_Injected(intPtr, data, mipLevel, elementSize, dataArraySize, sourceDataStartIndex);
		}

		private IntPtr GetWritableImageData(int frame)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2D.GetWritableImageData_Injected(intPtr, frame);
		}

		private ulong GetImageDataSize()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2D.GetImageDataSize_Injected(intPtr);
		}

		[FreeFunction("Texture2DScripting::GenerateAtlas")]
		private unsafe static void GenerateAtlasImpl(Vector2[] sizes, int padding, int atlasSize, [Out] Rect[] rect)
		{
			try
			{
				Span<Vector2> span = new Span<Vector2>(sizes);
				fixed (Vector2* ptr = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
					BlittableArrayWrapper blittableArrayWrapper;
					if (rect != null)
					{
						fixed (Rect[] array = rect)
						{
							if (array.Length != 0)
							{
								blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
							}
						}
					}
					Texture2D.GenerateAtlasImpl_Injected(ref managedSpanWrapper, padding, atlasSize, out blittableArrayWrapper);
				}
			}
			finally
			{
				Vector2* ptr = null;
				Rect[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<Rect>(ref array);
			}
		}

		internal bool isPreProcessed
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2D.get_isPreProcessed_Injected(intPtr);
			}
		}

		public bool streamingMipmaps
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2D.get_streamingMipmaps_Injected(intPtr);
			}
		}

		public int streamingMipmapsPriority
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2D.get_streamingMipmapsPriority_Injected(intPtr);
			}
		}

		public int requestedMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetRequestedMipmapLevel", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2D.get_requestedMipmapLevel_Injected(intPtr);
			}
			[FreeFunction(Name = "GetTextureStreamingManager().SetRequestedMipmapLevel", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Texture2D.set_requestedMipmapLevel_Injected(intPtr, value);
			}
		}

		public int minimumMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetMinimumMipmapLevel", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2D.get_minimumMipmapLevel_Injected(intPtr);
			}
			[FreeFunction(Name = "GetTextureStreamingManager().SetMinimumMipmapLevel", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Texture2D.set_minimumMipmapLevel_Injected(intPtr, value);
			}
		}

		internal bool loadAllMips
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetLoadAllMips", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2D.get_loadAllMips_Injected(intPtr);
			}
			[FreeFunction(Name = "GetTextureStreamingManager().SetLoadAllMips", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Texture2D.set_loadAllMips_Injected(intPtr, value);
			}
		}

		public int calculatedMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetCalculatedMipmapLevel", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2D.get_calculatedMipmapLevel_Injected(intPtr);
			}
		}

		public int desiredMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetDesiredMipmapLevel", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2D.get_desiredMipmapLevel_Injected(intPtr);
			}
		}

		public int loadingMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetLoadingMipmapLevel", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2D.get_loadingMipmapLevel_Injected(intPtr);
			}
		}

		public int loadedMipmapLevel
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetLoadedMipmapLevel", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture2D.get_loadedMipmapLevel_Injected(intPtr);
			}
		}

		[FreeFunction(Name = "GetTextureStreamingManager().ClearRequestedMipmapLevel", HasExplicitThis = true)]
		public void ClearRequestedMipmapLevel()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2D.ClearRequestedMipmapLevel_Injected(intPtr);
		}

		[FreeFunction(Name = "GetTextureStreamingManager().IsRequestedMipmapLevelLoaded", HasExplicitThis = true)]
		public bool IsRequestedMipmapLevelLoaded()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2D.IsRequestedMipmapLevelLoaded_Injected(intPtr);
		}

		[FreeFunction(Name = "GetTextureStreamingManager().ClearMinimumMipmapLevel", HasExplicitThis = true)]
		public void ClearMinimumMipmapLevel()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2D.ClearMinimumMipmapLevel_Injected(intPtr);
		}

		[FreeFunction("Texture2DScripting::UpdateExternalTexture", HasExplicitThis = true)]
		public void UpdateExternalTexture(IntPtr nativeTex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2D.UpdateExternalTexture_Injected(intPtr, nativeTex);
		}

		[FreeFunction("Texture2DScripting::SetAllPixels32", HasExplicitThis = true, ThrowsException = true)]
		private unsafe void SetAllPixels32(Color32[] colors, int miplevel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Color32> span = new Span<Color32>(colors);
			fixed (Color32* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Texture2D.SetAllPixels32_Injected(intPtr, ref managedSpanWrapper, miplevel);
			}
		}

		[FreeFunction("Texture2DScripting::SetBlockOfPixels32", HasExplicitThis = true, ThrowsException = true)]
		private unsafe void SetBlockOfPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, int miplevel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Color32> span = new Span<Color32>(colors);
			fixed (Color32* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Texture2D.SetBlockOfPixels32_Injected(intPtr, x, y, blockWidth, blockHeight, ref managedSpanWrapper, miplevel);
			}
		}

		[FreeFunction("Texture2DScripting::GetRawTextureData", HasExplicitThis = true, ThrowsException = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public byte[] GetRawTextureData()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2D.GetRawTextureData_Injected(intPtr);
		}

		[FreeFunction("Texture2DScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight, [DefaultValue("0")] int miplevel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2D.GetPixels_Injected(intPtr, x, y, blockWidth, blockHeight, miplevel);
		}

		[ExcludeFromDocs]
		public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight)
		{
			return this.GetPixels(x, y, blockWidth, blockHeight, 0);
		}

		[FreeFunction("Texture2DScripting::GetPixels32", HasExplicitThis = true, ThrowsException = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public Color32[] GetPixels32([DefaultValue("0")] int miplevel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2D.GetPixels32_Injected(intPtr, miplevel);
		}

		[ExcludeFromDocs]
		public Color32[] GetPixels32()
		{
			return this.GetPixels32(0);
		}

		[FreeFunction("Texture2DScripting::PackTextures", HasExplicitThis = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize, bool makeNoLongerReadable)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture2D.PackTextures_Injected(intPtr, textures, padding, maximumAtlasSize, makeNoLongerReadable);
		}

		public Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize)
		{
			return this.PackTextures(textures, padding, maximumAtlasSize, false);
		}

		public Rect[] PackTextures(Texture2D[] textures, int padding)
		{
			return this.PackTextures(textures, padding, 2048);
		}

		[FreeFunction(Name = "Texture2DScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
		private void CopyPixels_Full(Texture src)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2D.CopyPixels_Full_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(src));
		}

		[FreeFunction(Name = "Texture2DScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
		private void CopyPixels_Slice(Texture src, int srcElement, int srcMip, int dstMip)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2D.CopyPixels_Slice_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(src), srcElement, srcMip, dstMip);
		}

		[FreeFunction(Name = "Texture2DScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
		private void CopyPixels_Region(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstMip, int dstX, int dstY)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture2D>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture2D.CopyPixels_Region_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(src), srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dstMip, dstX, dstY);
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

		internal Texture2D(int width, int height, GraphicsFormat format, TextureCreationFlags flags, int mipCount, IntPtr nativeTex, MipmapLimitDescriptor mipmapLimitDescriptor)
		{
			bool flag = mipmapLimitDescriptor.useMipmapLimit;
			string groupName = mipmapLimitDescriptor.groupName;
			bool flag2 = (flags & TextureCreationFlags.IgnoreMipmapLimit) > TextureCreationFlags.None;
			bool flag3 = flag2;
			if (flag3)
			{
				flag = false;
			}
			bool flag4 = this.ValidateFormat(format, width, height);
			if (flag4)
			{
				Texture2D.Internal_Create(this, width, height, mipCount, format, base.GetTextureColorSpace(format), flags, nativeTex, !flag, groupName);
			}
		}

		[ExcludeFromDocs]
		public Texture2D(int width, int height, DefaultFormat format, TextureCreationFlags flags)
			: this(width, height, SystemInfo.GetGraphicsFormat(format), flags)
		{
		}

		[ExcludeFromDocs]
		public Texture2D(int width, int height, DefaultFormat format, int mipCount, TextureCreationFlags flags)
			: this(width, height, SystemInfo.GetGraphicsFormat(format), flags, mipCount, IntPtr.Zero, default(MipmapLimitDescriptor))
		{
		}

		[ExcludeFromDocs]
		[Obsolete("Please provide mipmap limit information using a MipmapLimitDescriptor argument", false)]
		public Texture2D(int width, int height, DefaultFormat format, int mipCount, string mipmapLimitGroupName, TextureCreationFlags flags)
			: this(width, height, SystemInfo.GetGraphicsFormat(format), flags, mipCount, IntPtr.Zero, new MipmapLimitDescriptor(true, mipmapLimitGroupName))
		{
		}

		[ExcludeFromDocs]
		public Texture2D(int width, int height, DefaultFormat format, int mipCount, TextureCreationFlags flags, MipmapLimitDescriptor mipmapLimitDescriptor)
			: this(width, height, SystemInfo.GetGraphicsFormat(format), flags, mipCount, IntPtr.Zero, mipmapLimitDescriptor)
		{
		}

		[ExcludeFromDocs]
		public Texture2D(int width, int height, GraphicsFormat format, TextureCreationFlags flags)
			: this(width, height, format, flags, Texture.GenerateAllMips, IntPtr.Zero, default(MipmapLimitDescriptor))
		{
		}

		[ExcludeFromDocs]
		public Texture2D(int width, int height, GraphicsFormat format, int mipCount, TextureCreationFlags flags)
			: this(width, height, format, flags, mipCount, IntPtr.Zero, default(MipmapLimitDescriptor))
		{
		}

		[ExcludeFromDocs]
		[Obsolete("Please provide mipmap limit information using a MipmapLimitDescriptor argument", false)]
		public Texture2D(int width, int height, GraphicsFormat format, int mipCount, string mipmapLimitGroupName, TextureCreationFlags flags)
			: this(width, height, format, flags, mipCount, IntPtr.Zero, new MipmapLimitDescriptor(true, mipmapLimitGroupName))
		{
		}

		[ExcludeFromDocs]
		public Texture2D(int width, int height, GraphicsFormat format, int mipCount, TextureCreationFlags flags, MipmapLimitDescriptor mipmapLimitDescriptor)
			: this(width, height, format, flags, mipCount, IntPtr.Zero, mipmapLimitDescriptor)
		{
		}

		internal Texture2D(int width, int height, TextureFormat textureFormat, int mipCount, bool linear, IntPtr nativeTex, bool createUninitialized, MipmapLimitDescriptor mipmapLimitDescriptor)
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
				Texture2D.Internal_Create(this, width, height, mipCount, graphicsFormat, base.GetTextureColorSpace(linear), textureCreationFlags, nativeTex, !mipmapLimitDescriptor.useMipmapLimit, mipmapLimitDescriptor.groupName);
			}
		}

		public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("-1")] int mipCount, [DefaultValue("false")] bool linear)
			: this(width, height, textureFormat, mipCount, linear, IntPtr.Zero, false, default(MipmapLimitDescriptor))
		{
		}

		public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("-1")] int mipCount, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized)
			: this(width, height, textureFormat, mipCount, linear, IntPtr.Zero, createUninitialized, default(MipmapLimitDescriptor))
		{
		}

		public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("-1")] int mipCount, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized, MipmapLimitDescriptor mipmapLimitDescriptor)
			: this(width, height, textureFormat, mipCount, linear, IntPtr.Zero, createUninitialized, mipmapLimitDescriptor)
		{
		}

		[Obsolete("Please provide mipmap limit information using a MipmapLimitDescriptor argument", false)]
		public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("-1")] int mipCount, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized, [DefaultValue("true")] bool ignoreMipmapLimit, [DefaultValue("null")] string mipmapLimitGroupName)
			: this(width, height, textureFormat, mipCount, linear, IntPtr.Zero, createUninitialized, new MipmapLimitDescriptor(!ignoreMipmapLimit, mipmapLimitGroupName))
		{
		}

		public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("true")] bool mipChain, [DefaultValue("false")] bool linear)
			: this(width, height, textureFormat, mipChain ? Texture.GenerateAllMips : 1, linear, IntPtr.Zero, false, default(MipmapLimitDescriptor))
		{
		}

		public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("true")] bool mipChain, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized)
			: this(width, height, textureFormat, mipChain ? Texture.GenerateAllMips : 1, linear, IntPtr.Zero, createUninitialized, default(MipmapLimitDescriptor))
		{
		}

		public Texture2D(int width, int height, TextureFormat textureFormat, bool mipChain)
			: this(width, height, textureFormat, mipChain ? Texture.GenerateAllMips : 1, false, IntPtr.Zero, false, default(MipmapLimitDescriptor))
		{
		}

		public Texture2D(int width, int height)
		{
			TextureFormat textureFormat = TextureFormat.RGBA32;
			bool flag = width == 0 && height == 0;
			if (flag)
			{
				Texture2D.Internal_CreateEmptyImpl(this);
			}
			else
			{
				bool flag2 = this.ValidateFormat(textureFormat, width, height);
				if (flag2)
				{
					Texture2D.Internal_Create(this, width, height, Texture.GenerateAllMips, GraphicsFormatUtility.GetGraphicsFormat(textureFormat, true), base.GetTextureColorSpace(false), TextureCreationFlags.MipChain, IntPtr.Zero, true, null);
				}
			}
		}

		public static Texture2D CreateExternalTexture(int width, int height, TextureFormat format, bool mipChain, bool linear, IntPtr nativeTex)
		{
			bool flag = nativeTex == IntPtr.Zero;
			if (flag)
			{
				throw new ArgumentException("nativeTex can not be null");
			}
			return new Texture2D(width, height, format, mipChain ? (-1) : 1, linear, nativeTex, false, default(MipmapLimitDescriptor));
		}

		[ExcludeFromDocs]
		public void SetPixel(int x, int y, Color color)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelImpl(0, 0, x, y, color);
		}

		public void SetPixel(int x, int y, Color color, [DefaultValue("0")] int mipLevel)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelImpl(0, mipLevel, x, y, color);
		}

		public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, [DefaultValue("0")] int miplevel)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			this.SetPixelsImpl(x, y, blockWidth, blockHeight, colors, miplevel, 0);
		}

		[ExcludeFromDocs]
		public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors)
		{
			this.SetPixels(x, y, blockWidth, blockHeight, colors, 0);
		}

		public void SetPixels(Color[] colors, [DefaultValue("0")] int miplevel)
		{
			int num = this.width >> miplevel;
			bool flag = num < 1;
			if (flag)
			{
				num = 1;
			}
			int num2 = this.height >> miplevel;
			bool flag2 = num2 < 1;
			if (flag2)
			{
				num2 = 1;
			}
			this.SetPixels(0, 0, num, num2, colors, miplevel);
		}

		[ExcludeFromDocs]
		public void SetPixels(Color[] colors)
		{
			this.SetPixels(0, 0, this.width, this.height, colors, 0);
		}

		[ExcludeFromDocs]
		public Color GetPixel(int x, int y)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelImpl(0, 0, x, y);
		}

		public Color GetPixel(int x, int y, [DefaultValue("0")] int mipLevel)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelImpl(0, mipLevel, x, y);
		}

		[ExcludeFromDocs]
		public Color GetPixelBilinear(float u, float v)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelBilinearImpl(0, 0, u, v);
		}

		public Color GetPixelBilinear(float u, float v, [DefaultValue("0")] int mipLevel)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.GetPixelBilinearImpl(0, mipLevel, u, v);
		}

		public void LoadRawTextureData(IntPtr data, int size)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag2 = data == IntPtr.Zero || size == 0;
			if (flag2)
			{
				Debug.LogError("No texture data provided to LoadRawTextureData", this);
			}
			else
			{
				bool flag3 = !this.LoadRawTextureDataImpl(data, (ulong)((long)size));
				if (flag3)
				{
					throw new UnityException("LoadRawTextureData: not enough data provided (will result in overread).");
				}
			}
		}

		public void LoadRawTextureData(byte[] data)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag2 = data == null || data.Length == 0;
			if (flag2)
			{
				Debug.LogError("No texture data provided to LoadRawTextureData", this);
			}
			else
			{
				bool flag3 = !this.LoadRawTextureDataImplArray(data);
				if (flag3)
				{
					throw new UnityException("LoadRawTextureData: not enough data provided (will result in overread).");
				}
			}
		}

		public void LoadRawTextureData<T>(NativeArray<T> data) where T : struct
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			bool flag2 = !data.IsCreated || data.Length == 0;
			if (flag2)
			{
				throw new UnityException("No texture data provided to LoadRawTextureData");
			}
			bool flag3 = !this.LoadRawTextureDataImpl((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), (ulong)((long)data.Length * (long)UnsafeUtility.SizeOf<T>()));
			if (flag3)
			{
				throw new UnityException("LoadRawTextureData: not enough data provided (will result in overread).");
			}
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
				throw new ArgumentException("The passed in miplevel " + mipLevel.ToString() + " is invalid. It needs to be in the range 0 and " + (base.mipmapCount - 1).ToString());
			}
			bool flag3 = this.GetWritableImageData(0).ToInt64() == 0L;
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
			IntPtr intPtr = new IntPtr((long)this.GetWritableImageData(0) + (long)pixelDataOffset);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)intPtr, (int)num2, Allocator.None);
		}

		public unsafe NativeArray<T> GetRawTextureData<T>() where T : struct
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			int num = UnsafeUtility.SizeOf<T>();
			ulong num2 = this.GetImageDataSize() / (ulong)((long)num);
			bool flag2 = num2 > 2147483647UL;
			if (flag2)
			{
				throw base.CreateNativeArrayLengthOverflowException();
			}
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)this.GetWritableImageData(0), (int)num2, Allocator.None);
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

		public bool Reinitialize(int width, int height)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.ReinitializeImpl(width, height);
		}

		public bool Reinitialize(int width, int height, TextureFormat format, bool hasMipMap)
		{
			return this.ReinitializeWithTextureFormatImpl(width, height, format, hasMipMap);
		}

		public bool Reinitialize(int width, int height, GraphicsFormat format, bool hasMipMap)
		{
			bool flag = !this.isReadable;
			if (flag)
			{
				throw base.CreateNonReadableException(this);
			}
			return this.ReinitializeWithFormatImpl(width, height, format, hasMipMap);
		}

		[Obsolete("Texture2D.Resize(int, int) has been deprecated because it actually reinitializes the texture. Use Texture2D.Reinitialize(int, int) instead (UnityUpgradable) -> Reinitialize([*] System.Int32, [*] System.Int32)", false)]
		public bool Resize(int width, int height)
		{
			return this.Reinitialize(width, height);
		}

		[Obsolete("Texture2D.Resize(int, int, TextureFormat, bool) has been deprecated because it actually reinitializes the texture. Use Texture2D.Reinitialize(int, int, TextureFormat, bool) instead (UnityUpgradable) -> Reinitialize([*] System.Int32, [*] System.Int32, UnityEngine.TextureFormat, [*] System.Boolean)", false)]
		public bool Resize(int width, int height, TextureFormat format, bool hasMipMap)
		{
			return this.Reinitialize(width, height, format, hasMipMap);
		}

		[Obsolete("Texture2D.Resize(int, int, GraphicsFormat, bool) has been deprecated because it actually reinitializes the texture. Use Texture2D.Reinitialize(int, int, GraphicsFormat, bool) instead (UnityUpgradable) -> Reinitialize([*] System.Int32, [*] System.Int32, UnityEngine.Experimental.Rendering.GraphicsFormat, [*] System.Boolean)", false)]
		public bool Resize(int width, int height, GraphicsFormat format, bool hasMipMap)
		{
			return this.Reinitialize(width, height, format, hasMipMap);
		}

		public void ReadPixels(Rect source, int destX, int destY, [DefaultValue("true")] bool recalculateMipMaps)
		{
			bool flag = !this.isReadable;
			if (flag)
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
			bool flag = sizes == null;
			if (flag)
			{
				throw new ArgumentException("sizes array can not be null");
			}
			bool flag2 = results == null;
			if (flag2)
			{
				throw new ArgumentException("results list cannot be null");
			}
			bool flag3 = padding < 0;
			if (flag3)
			{
				throw new ArgumentException("padding can not be negative");
			}
			bool flag4 = atlasSize <= 0;
			if (flag4)
			{
				throw new ArgumentException("atlas size must be positive");
			}
			results.Clear();
			bool flag5 = sizes.Length == 0;
			bool flag6;
			if (flag5)
			{
				flag6 = true;
			}
			else
			{
				NoAllocHelpers.EnsureListElemCount<Rect>(results, sizes.Length);
				Texture2D.GenerateAtlasImpl(sizes, padding, atlasSize, NoAllocHelpers.ExtractArrayFromList<Rect>(results));
				flag6 = results.Count != 0;
			}
			return flag6;
		}

		public void SetPixels32(Color32[] colors, [DefaultValue("0")] int miplevel)
		{
			this.SetAllPixels32(colors, miplevel);
		}

		[ExcludeFromDocs]
		public void SetPixels32(Color32[] colors)
		{
			this.SetPixels32(colors, 0);
		}

		public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, [DefaultValue("0")] int miplevel)
		{
			this.SetBlockOfPixels32(x, y, blockWidth, blockHeight, colors, miplevel);
		}

		[ExcludeFromDocs]
		public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors)
		{
			this.SetPixels32(x, y, blockWidth, blockHeight, colors, 0);
		}

		public Color[] GetPixels([DefaultValue("0")] int miplevel)
		{
			int num = this.width >> miplevel;
			bool flag = num < 1;
			if (flag)
			{
				num = 1;
			}
			int num2 = this.height >> miplevel;
			bool flag2 = num2 < 1;
			if (flag2)
			{
				num2 = 1;
			}
			return this.GetPixels(0, 0, num, num2, miplevel);
		}

		[ExcludeFromDocs]
		public Color[] GetPixels()
		{
			return this.GetPixels(0);
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

		public void CopyPixels(Texture src, int srcElement, int srcMip, int dstMip)
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
			this.CopyPixels_Slice(src, srcElement, srcMip, dstMip);
		}

		public void CopyPixels(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstMip, int dstX, int dstY)
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
			this.CopyPixels_Region(src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dstMip, dstX, dstY);
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
		private static extern IntPtr get_whiteTexture_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_blackTexture_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_redTexture_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_grayTexture_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_linearGrayTexture_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_normalTexture_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Compress_Injected(IntPtr _unity_self, bool highQuality);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_CreateImpl_Injected([Writable] Texture2D mono, int w, int h, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex, bool ignoreMipmapLimit, ref ManagedSpanWrapper mipmapLimitGroupName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isReadable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_vtOnly_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ApplyImpl_Injected(IntPtr _unity_self, bool updateMipmaps, bool makeNoLongerReadable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ReinitializeImpl_Injected(IntPtr _unity_self, int width, int height);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPixelImpl_Injected(IntPtr _unity_self, int image, int mip, int x, int y, [In] ref Color color);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPixelImpl_Injected(IntPtr _unity_self, int image, int mip, int x, int y, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPixelBilinearImpl_Injected(IntPtr _unity_self, int image, int mip, float u, float v, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ReinitializeWithFormatImpl_Injected(IntPtr _unity_self, int width, int height, GraphicsFormat format, bool hasMipMap);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ReinitializeWithTextureFormatImpl_Injected(IntPtr _unity_self, int width, int height, TextureFormat textureFormat, bool hasMipMap);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReadPixelsImpl_Injected(IntPtr _unity_self, [In] ref Rect source, int destX, int destY, bool recalculateMipMaps);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPixelsImpl_Injected(IntPtr _unity_self, int x, int y, int w, int h, ref ManagedSpanWrapper pixel, int miplevel, int frame);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool LoadRawTextureDataImpl_Injected(IntPtr _unity_self, IntPtr data, ulong size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool LoadRawTextureDataImplArray_Injected(IntPtr _unity_self, ref ManagedSpanWrapper data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetPixelDataImplArray_Injected(IntPtr _unity_self, Array data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetPixelDataImpl_Injected(IntPtr _unity_self, IntPtr data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetWritableImageData_Injected(IntPtr _unity_self, int frame);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ulong GetImageDataSize_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateAtlasImpl_Injected(ref ManagedSpanWrapper sizes, int padding, int atlasSize, out BlittableArrayWrapper rect);

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
		private static extern int get_minimumMipmapLevel_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_minimumMipmapLevel_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_loadAllMips_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_loadAllMips_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_calculatedMipmapLevel_Injected(IntPtr _unity_self);

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearMinimumMipmapLevel_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateExternalTexture_Injected(IntPtr _unity_self, IntPtr nativeTex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAllPixels32_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, int miplevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBlockOfPixels32_Injected(IntPtr _unity_self, int x, int y, int blockWidth, int blockHeight, ref ManagedSpanWrapper colors, int miplevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern byte[] GetRawTextureData_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Color[] GetPixels_Injected(IntPtr _unity_self, int x, int y, int blockWidth, int blockHeight, [DefaultValue("0")] int miplevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Color32[] GetPixels32_Injected(IntPtr _unity_self, [DefaultValue("0")] int miplevel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Rect[] PackTextures_Injected(IntPtr _unity_self, Texture2D[] textures, int padding, int maximumAtlasSize, bool makeNoLongerReadable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyPixels_Full_Injected(IntPtr _unity_self, IntPtr src);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyPixels_Slice_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int dstMip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyPixels_Region_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstMip, int dstX, int dstY);

		internal const int streamingMipmapsPriorityMin = -128;

		internal const int streamingMipmapsPriorityMax = 127;

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
