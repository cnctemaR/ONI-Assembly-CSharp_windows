using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/Texture.h")]
	[NativeHeader("Runtime/Streaming/TextureStreamingManager.h")]
	[UsedByNativeCode]
	public class Texture : Object
	{
		protected Texture()
		{
		}

		public static extern int masterTextureLimit
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetDataWidth();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetDataHeight();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern TextureDimension GetDimension();

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

		public virtual extern bool isReadable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern TextureWrapMode wrapMode
		{
			[NativeName("GetWrapModeU")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureWrapMode wrapModeU
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureWrapMode wrapModeV
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern TextureWrapMode wrapModeW
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern FilterMode filterMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int anisoLevel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float mipMapBias
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector2 texelSize
		{
			[NativeName("GetNpotTexelSize")]
			get
			{
				Vector2 vector;
				this.get_texelSize_Injected(out vector);
				return vector;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern IntPtr GetNativeTexturePtr();

		[Obsolete("Use GetNativeTexturePtr instead.", false)]
		public int GetNativeTextureID()
		{
			return (int)this.GetNativeTexturePtr();
		}

		public extern uint updateCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void IncrementUpdateCount();

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

		internal bool ValidateFormat(RenderTextureFormat format)
		{
			bool flag;
			if (SystemInfo.SupportsRenderTextureFormat(format))
			{
				flag = true;
			}
			else
			{
				Debug.LogError(string.Format("RenderTexture creation failed. '{0}' is not supported on this platform. Use 'SystemInfo.SupportsRenderTextureFormat' C# API to check format support.", format.ToString()), this);
				flag = false;
			}
			return flag;
		}

		internal bool ValidateFormat(TextureFormat format)
		{
			bool flag;
			if (SystemInfo.SupportsTextureFormat(format))
			{
				flag = true;
			}
			else if (GraphicsFormatUtility.IsCompressedTextureFormat(format))
			{
				Debug.LogWarning(string.Format("'{0}' is not supported on this platform. Decompressing texture. Use 'SystemInfo.SupportsTextureFormat' C# API to check format support.", format.ToString()), this);
				flag = true;
			}
			else
			{
				Debug.LogError(string.Format("Texture creation failed. '{0}' is not supported on this platform. Use 'SystemInfo.SupportsTextureFormat' C# API to check format support.", format.ToString()), this);
				flag = false;
			}
			return flag;
		}

		internal bool ValidateFormat(GraphicsFormat format, FormatUsage usage)
		{
			bool flag;
			if (SystemInfo.IsFormatSupported(format, usage))
			{
				flag = true;
			}
			else
			{
				Debug.LogError(string.Format("Texture creation failed. '{0}' is not supported for {1} usage on this platform. Use 'SystemInfo.IsFormatSupported' C# API to check format support.", format.ToString(), usage.ToString()), this);
				flag = false;
			}
			return flag;
		}

		internal UnityException CreateNonReadableException(Texture t)
		{
			return new UnityException(string.Format("Texture '{0}' is not readable, the texture memory can not be accessed from scripts. You can make the texture readable in the Texture Import Settings.", t.name));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_texelSize_Injected(out Vector2 ret);
	}
}
