using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/Texture.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Streaming/TextureStreamingManager.h")]
	public class Texture : Object
	{
		protected Texture()
		{
		}

		[Obsolete("masterTextureLimit has been deprecated. Use globalMipmapLimit instead (UnityUpgradable) -> globalMipmapLimit", false)]
		[NativeProperty("ActiveGlobalMipmapLimit")]
		public static extern int masterTextureLimit
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("ActiveGlobalMipmapLimit")]
		[Obsolete("globalMipmapLimit is not supported. Use QualitySettings.globalTextureMipmapLimit or Mipmap Limit Groups instead.", false)]
		public static extern int globalMipmapLimit
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public int mipmapCount
		{
			[NativeName("GetMipmapCount")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture.get_mipmapCount_Injected(intPtr);
			}
		}

		[NativeProperty("AnisoLimit")]
		public static extern AnisotropicFiltering anisotropicFiltering
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeName("SetGlobalAnisoLimits")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetGlobalAnisotropicFilteringLimits(int forcedMin, int globalMax);

		public virtual GraphicsFormat graphicsFormat
		{
			get
			{
				return GraphicsFormatUtility.GetFormat(this);
			}
		}

		[ThreadSafe]
		private int GetDataWidth()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture.GetDataWidth_Injected(intPtr);
		}

		[ThreadSafe]
		private int GetDataHeight()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture.GetDataHeight_Injected(intPtr);
		}

		[ThreadSafe]
		private TextureDimension GetDimension()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture.GetDimension_Injected(intPtr);
		}

		public virtual int width
		{
			get
			{
				return this.GetDataWidth();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual int height
		{
			get
			{
				return this.GetDataHeight();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual TextureDimension dimension
		{
			get
			{
				return this.GetDimension();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		internal bool isNativeTexture
		{
			[NativeName("IsNativeTexture")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture.get_isNativeTexture_Injected(intPtr);
			}
		}

		public virtual bool isReadable
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture.get_isReadable_Injected(intPtr);
			}
		}

		public TextureWrapMode wrapMode
		{
			[NativeName("GetWrapModeU")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture.get_wrapMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Texture.set_wrapMode_Injected(intPtr, value);
			}
		}

		public TextureWrapMode wrapModeU
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture.get_wrapModeU_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Texture.set_wrapModeU_Injected(intPtr, value);
			}
		}

		public TextureWrapMode wrapModeV
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture.get_wrapModeV_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Texture.set_wrapModeV_Injected(intPtr, value);
			}
		}

		public TextureWrapMode wrapModeW
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture.get_wrapModeW_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Texture.set_wrapModeW_Injected(intPtr, value);
			}
		}

		public FilterMode filterMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture.get_filterMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Texture.set_filterMode_Injected(intPtr, value);
			}
		}

		public int anisoLevel
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture.get_anisoLevel_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Texture.set_anisoLevel_Injected(intPtr, value);
			}
		}

		public float mipMapBias
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture.get_mipMapBias_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Texture.set_mipMapBias_Injected(intPtr, value);
			}
		}

		public Vector2 texelSize
		{
			[NativeName("GetTexelSize")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Texture.get_texelSize_Injected(intPtr, out vector);
				return vector;
			}
		}

		public IntPtr GetNativeTexturePtr()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture.GetNativeTexturePtr_Injected(intPtr);
		}

		[Obsolete("Use GetNativeTexturePtr instead.", false)]
		public int GetNativeTextureID()
		{
			return (int)this.GetNativeTexturePtr();
		}

		public uint updateCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Texture.get_updateCount_Injected(intPtr);
			}
		}

		public void IncrementUpdateCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Texture.IncrementUpdateCount_Injected(intPtr);
		}

		[NativeMethod("GetActiveTextureColorSpace")]
		private int Internal_GetActiveTextureColorSpace()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture.Internal_GetActiveTextureColorSpace_Injected(intPtr);
		}

		internal ColorSpace activeTextureColorSpace
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "Unity.UIElements" })]
			get
			{
				return (this.Internal_GetActiveTextureColorSpace() == 0) ? ColorSpace.Linear : ColorSpace.Gamma;
			}
		}

		[NativeMethod("GetStoredColorSpace")]
		private TextureColorSpace Internal_GetStoredColorSpace()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture.Internal_GetStoredColorSpace_Injected(intPtr);
		}

		public bool isDataSRGB
		{
			get
			{
				return this.Internal_GetStoredColorSpace() == TextureColorSpace.sRGB;
			}
		}

		public static extern ulong totalTextureMemory
		{
			[FreeFunction("GetTextureStreamingManager().GetTotalTextureMemory")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ulong desiredTextureMemory
		{
			[FreeFunction("GetTextureStreamingManager().GetDesiredTextureMemory")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ulong targetTextureMemory
		{
			[FreeFunction("GetTextureStreamingManager().GetTargetTextureMemory")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ulong currentTextureMemory
		{
			[FreeFunction("GetTextureStreamingManager().GetCurrentTextureMemory")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ulong nonStreamingTextureMemory
		{
			[FreeFunction("GetTextureStreamingManager().GetNonStreamingTextureMemory")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ulong streamingMipmapUploadCount
		{
			[FreeFunction("GetTextureStreamingManager().GetStreamingMipmapUploadCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ulong streamingRendererCount
		{
			[FreeFunction("GetTextureStreamingManager().GetStreamingRendererCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ulong streamingTextureCount
		{
			[FreeFunction("GetTextureStreamingManager().GetStreamingTextureCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ulong nonStreamingTextureCount
		{
			[FreeFunction("GetTextureStreamingManager().GetNonStreamingTextureCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ulong streamingTexturePendingLoadCount
		{
			[FreeFunction("GetTextureStreamingManager().GetStreamingTexturePendingLoadCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ulong streamingTextureLoadingCount
		{
			[FreeFunction("GetTextureStreamingManager().GetStreamingTextureLoadingCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("GetTextureStreamingManager().SetStreamingTextureMaterialDebugProperties")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStreamingTextureMaterialDebugProperties();

		[FreeFunction("GetTextureStreamingManager().SetStreamingTextureMaterialDebugPropertiesWithSlot")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetStreamingTextureMaterialDebugPropertiesWithSlot(int materialTextureSlot);

		public static void SetStreamingTextureMaterialDebugProperties(int materialTextureSlot)
		{
			Texture.SetStreamingTextureMaterialDebugPropertiesWithSlot(materialTextureSlot);
		}

		public static extern bool streamingTextureForceLoadAll
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetForceLoadAll")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction(Name = "GetTextureStreamingManager().SetForceLoadAll")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool streamingTextureDiscardUnusedMips
		{
			[FreeFunction(Name = "GetTextureStreamingManager().GetDiscardUnusedMips")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction(Name = "GetTextureStreamingManager().SetDiscardUnusedMips")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool allowThreadedTextureCreation
		{
			[FreeFunction(Name = "Texture2DScripting::IsCreateTextureThreadedEnabled")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction(Name = "Texture2DScripting::EnableCreateTextureThreaded")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal ulong GetPixelDataSize(int mipLevel, int element = 0)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture.GetPixelDataSize_Injected(intPtr, mipLevel, element);
		}

		internal ulong GetPixelDataOffset(int mipLevel, int element = 0)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Texture.GetPixelDataOffset_Injected(intPtr, mipLevel, element);
		}

		public GraphicsTexture graphicsTexture
		{
			[FreeFunction(Name = "Texture2DScripting::GetCurrentGraphicsTexture", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				IntPtr intPtr2 = Texture.get_graphicsTexture_Injected(intPtr);
				return (intPtr2 == 0) ? null : GraphicsTexture.BindingsMarshaller.ConvertToManaged(intPtr2);
			}
		}

		internal TextureColorSpace GetTextureColorSpace(bool linear)
		{
			return linear ? TextureColorSpace.Linear : TextureColorSpace.sRGB;
		}

		internal TextureColorSpace GetTextureColorSpace(GraphicsFormat format)
		{
			return this.GetTextureColorSpace(!GraphicsFormatUtility.IsSRGBFormat(format));
		}

		internal bool ValidateFormat(RenderTextureFormat format)
		{
			bool flag = SystemInfo.SupportsRenderTextureFormat(format);
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				Debug.LogError(string.Format("RenderTexture creation failed. '{0}' is not supported on this platform. Use 'SystemInfo.SupportsRenderTextureFormat' C# API to check format support.", format.ToString()), this);
				flag2 = false;
			}
			return flag2;
		}

		internal bool ValidateFormat(TextureFormat format)
		{
			bool flag = SystemInfo.SupportsTextureFormat(format);
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = GraphicsFormatUtility.IsCompressedFormat(format) && GraphicsFormatUtility.CanDecompressFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
				if (flag3)
				{
					Debug.LogWarning(string.Format("'{0}' is not supported on this platform. Decompressing texture. Use 'SystemInfo.SupportsTextureFormat' C# API to check format support.", format.ToString()), this);
					flag2 = true;
				}
				else
				{
					Debug.LogError(string.Format("Texture creation failed. '{0}' is not supported on this platform. Use 'SystemInfo.SupportsTextureFormat' C# API to check format support.", format.ToString()), this);
					flag2 = false;
				}
			}
			return flag2;
		}

		internal bool ValidateFormat(GraphicsFormat format, GraphicsFormatUsage usage)
		{
			bool flag = SystemInfo.IsFormatSupported(format, usage);
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				Debug.LogError(string.Format("Texture creation failed. '{0}' is not supported for {1} usage on this platform. Use 'SystemInfo.IsFormatSupported' C# API to check format support.", format.ToString(), usage.ToString()), this);
				flag2 = false;
			}
			return flag2;
		}

		internal UnityException CreateNonReadableException(Texture t)
		{
			return new UnityException(string.Format("Texture '{0}' is not readable, the texture memory can not be accessed from scripts. You can make the texture readable in the Texture Import Settings.", t.name));
		}

		internal UnityException IgnoreMipmapLimitCannotBeToggledException(Texture t)
		{
			return new UnityException(string.Format("Failed to toggle ignoreMipmapLimit, Texture '{0}' is not readable. You can make the texture readable in the Texture Import Settings.", t.name));
		}

		internal UnityException CreateNativeArrayLengthOverflowException()
		{
			return new UnityException("Failed to create NativeArray, length exceeds the allowed maximum of Int32.MaxValue. Use a larger type as template argument to reduce the array length.");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_mipmapCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDataWidth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDataHeight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureDimension GetDimension_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isNativeTexture_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isReadable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureWrapMode get_wrapMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wrapMode_Injected(IntPtr _unity_self, TextureWrapMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureWrapMode get_wrapModeU_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wrapModeU_Injected(IntPtr _unity_self, TextureWrapMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureWrapMode get_wrapModeV_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wrapModeV_Injected(IntPtr _unity_self, TextureWrapMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureWrapMode get_wrapModeW_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wrapModeW_Injected(IntPtr _unity_self, TextureWrapMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern FilterMode get_filterMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_filterMode_Injected(IntPtr _unity_self, FilterMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_anisoLevel_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_anisoLevel_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_mipMapBias_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_mipMapBias_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_texelSize_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetNativeTexturePtr_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_updateCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IncrementUpdateCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetActiveTextureColorSpace_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureColorSpace Internal_GetStoredColorSpace_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ulong GetPixelDataSize_Injected(IntPtr _unity_self, int mipLevel, int element);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ulong GetPixelDataOffset_Injected(IntPtr _unity_self, int mipLevel, int element);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_graphicsTexture_Injected(IntPtr _unity_self);

		public static readonly int GenerateAllMips = -1;
	}
}
