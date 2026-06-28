using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public class RenderTexture : Texture
	{
		public RenderTexture(int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			RenderTexture.Internal_CreateRenderTexture(this);
			this.width = width;
			this.height = height;
			this.depth = depth;
			this.format = format;
			bool flag = readWrite == RenderTextureReadWrite.sRGB;
			if (readWrite == RenderTextureReadWrite.Default)
			{
				flag = QualitySettings.activeColorSpace == ColorSpace.Linear;
			}
			RenderTexture.Internal_SetSRGBReadWrite(this, flag);
		}

		public RenderTexture(int width, int height, int depth, RenderTextureFormat format)
		{
			RenderTexture.Internal_CreateRenderTexture(this);
			this.width = width;
			this.height = height;
			this.depth = depth;
			this.format = format;
			RenderTexture.Internal_SetSRGBReadWrite(this, QualitySettings.activeColorSpace == ColorSpace.Linear);
		}

		public RenderTexture(int width, int height, int depth)
		{
			RenderTexture.Internal_CreateRenderTexture(this);
			this.width = width;
			this.height = height;
			this.depth = depth;
			this.format = RenderTextureFormat.Default;
			RenderTexture.Internal_SetSRGBReadWrite(this, QualitySettings.activeColorSpace == ColorSpace.Linear);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateRenderTexture([Writable] RenderTexture rt);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern RenderTexture GetTemporary(int width, int height, [DefaultValue("0")] int depthBuffer, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [DefaultValue("1")] int antiAliasing, [DefaultValue("VRTextureUsage.None")] VRTextureUsage vrUsage);

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
		{
			VRTextureUsage vrtextureUsage = VRTextureUsage.None;
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, vrtextureUsage);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			VRTextureUsage vrtextureUsage = VRTextureUsage.None;
			int num = 1;
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, num, vrtextureUsage);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format)
		{
			VRTextureUsage vrtextureUsage = VRTextureUsage.None;
			int num = 1;
			RenderTextureReadWrite renderTextureReadWrite = RenderTextureReadWrite.Default;
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, renderTextureReadWrite, num, vrtextureUsage);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer)
		{
			VRTextureUsage vrtextureUsage = VRTextureUsage.None;
			int num = 1;
			RenderTextureReadWrite renderTextureReadWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat renderTextureFormat = RenderTextureFormat.Default;
			return RenderTexture.GetTemporary(width, height, depthBuffer, renderTextureFormat, renderTextureReadWrite, num, vrtextureUsage);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height)
		{
			VRTextureUsage vrtextureUsage = VRTextureUsage.None;
			int num = 1;
			RenderTextureReadWrite renderTextureReadWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat renderTextureFormat = RenderTextureFormat.Default;
			int num2 = 0;
			return RenderTexture.GetTemporary(width, height, num2, renderTextureFormat, renderTextureReadWrite, num, vrtextureUsage);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ReleaseTemporary(RenderTexture temp);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetWidth(RenderTexture mono);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetWidth(RenderTexture mono, int width);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetHeight(RenderTexture mono);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetHeight(RenderTexture mono, int width);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VRTextureUsage Internal_GetVRUsage(RenderTexture mono);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetVRUsage(RenderTexture mono, VRTextureUsage vrUsage);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetSRGBReadWrite(RenderTexture mono, bool sRGB);

		public override int width
		{
			get
			{
				return RenderTexture.Internal_GetWidth(this);
			}
			set
			{
				RenderTexture.Internal_SetWidth(this, value);
			}
		}

		public override int height
		{
			get
			{
				return RenderTexture.Internal_GetHeight(this);
			}
			set
			{
				RenderTexture.Internal_SetHeight(this, value);
			}
		}

		public VRTextureUsage vrUsage
		{
			get
			{
				return RenderTexture.Internal_GetVRUsage(this);
			}
			set
			{
				RenderTexture.Internal_SetVRUsage(this, value);
			}
		}

		public extern int depth
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isPowerOfTwo
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool sRGB
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern RenderTextureFormat format
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useMipMap
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool autoGenerateMips
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureDimension Internal_GetDimension(RenderTexture rt);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetDimension(RenderTexture rt, TextureDimension dim);

		public override TextureDimension dimension
		{
			get
			{
				return RenderTexture.Internal_GetDimension(this);
			}
			set
			{
				RenderTexture.Internal_SetDimension(this, value);
			}
		}

		[Obsolete("Use RenderTexture.dimension instead.")]
		public extern bool isCubemap
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use RenderTexture.dimension instead.")]
		public extern bool isVolume
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int volumeDepth
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int antiAliasing
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enableRandomWrite
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public bool Create()
		{
			return RenderTexture.INTERNAL_CALL_Create(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Create(RenderTexture self);

		public void Release()
		{
			RenderTexture.INTERNAL_CALL_Release(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Release(RenderTexture self);

		public bool IsCreated()
		{
			return RenderTexture.INTERNAL_CALL_IsCreated(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_IsCreated(RenderTexture self);

		public void DiscardContents()
		{
			RenderTexture.INTERNAL_CALL_DiscardContents(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DiscardContents(RenderTexture self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DiscardContents(bool discardColor, bool discardDepth);

		public void MarkRestoreExpected()
		{
			RenderTexture.INTERNAL_CALL_MarkRestoreExpected(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MarkRestoreExpected(RenderTexture self);

		public void GenerateMips()
		{
			RenderTexture.INTERNAL_CALL_GenerateMips(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GenerateMips(RenderTexture self);

		public RenderBuffer colorBuffer
		{
			get
			{
				RenderBuffer renderBuffer;
				this.GetColorBuffer(out renderBuffer);
				return renderBuffer;
			}
		}

		public RenderBuffer depthBuffer
		{
			get
			{
				RenderBuffer renderBuffer;
				this.GetDepthBuffer(out renderBuffer);
				return renderBuffer;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetColorBuffer(out RenderBuffer res);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetDepthBuffer(out RenderBuffer res);

		public IntPtr GetNativeDepthBufferPtr()
		{
			IntPtr intPtr;
			RenderTexture.INTERNAL_CALL_GetNativeDepthBufferPtr(this, out intPtr);
			return intPtr;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetNativeDepthBufferPtr(RenderTexture self, out IntPtr value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalShaderProperty(string propertyName);

		public static extern RenderTexture active
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("RenderTexture.enabled is always now, no need to use it")]
		public static extern bool enabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("GetTexelOffset always returns zero now, no point in using it.")]
		public Vector2 GetTexelOffset()
		{
			return Vector2.zero;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SupportsStencil(RenderTexture rt);

		[Obsolete("SetBorderColor is no longer supported.", true)]
		public void SetBorderColor(Color color)
		{
		}
	}
}
