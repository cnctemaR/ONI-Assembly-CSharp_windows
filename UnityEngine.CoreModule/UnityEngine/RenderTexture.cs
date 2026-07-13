using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Camera/Camera.h")]
	[NativeHeader("Runtime/Graphics/RenderTexture.h")]
	[NativeHeader("Runtime/Graphics/RenderBufferManager.h")]
	[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
	public class RenderTexture : Texture
	{
		public override extern int width
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public override extern int height
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public override extern TextureDimension dimension
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeName("GetColorFormat")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern GraphicsFormat GetColorFormat(bool suppressWarnings);

		[NativeName("SetColorFormat")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetColorFormat(GraphicsFormat format);

		public new GraphicsFormat graphicsFormat
		{
			get
			{
				return this.GetColorFormat(true);
			}
			set
			{
				this.SetColorFormat(value);
			}
		}

		[NativeProperty("MipMap")]
		public extern bool useMipMap
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("SRGBReadWrite")]
		public extern bool sRGB
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeProperty("VRUsage")]
		public extern VRTextureUsage vrUsage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("Memoryless")]
		public extern RenderTextureMemoryless memorylessMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public RenderTextureFormat format
		{
			get
			{
				bool flag = this.graphicsFormat > GraphicsFormat.None;
				RenderTextureFormat renderTextureFormat;
				if (flag)
				{
					renderTextureFormat = GraphicsFormatUtility.GetRenderTextureFormat(this.graphicsFormat);
				}
				else
				{
					renderTextureFormat = ((this.GetDescriptor().shadowSamplingMode != ShadowSamplingMode.None) ? RenderTextureFormat.Shadowmap : RenderTextureFormat.Depth);
				}
				return renderTextureFormat;
			}
			set
			{
				this.graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(value, this.sRGB);
			}
		}

		public extern GraphicsFormat stencilFormat
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern GraphicsFormat depthStencilFormat
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool autoGenerateMips
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int volumeDepth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int antiAliasing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool bindTextureMS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enableRandomWrite
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool useDynamicScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetIsPowerOfTwo();

		public bool isPowerOfTwo
		{
			get
			{
				return this.GetIsPowerOfTwo();
			}
			set
			{
			}
		}

		[FreeFunction("RenderTexture::GetActive")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RenderTexture GetActive();

		[FreeFunction("RenderTextureScripting::SetActive")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetActive(RenderTexture rt);

		public static RenderTexture active
		{
			get
			{
				return RenderTexture.GetActive();
			}
			set
			{
				RenderTexture.SetActive(value);
			}
		}

		[FreeFunction(Name = "RenderTextureScripting::GetColorBuffer", HasExplicitThis = true)]
		private RenderBuffer GetColorBuffer()
		{
			RenderBuffer renderBuffer;
			this.GetColorBuffer_Injected(out renderBuffer);
			return renderBuffer;
		}

		[FreeFunction(Name = "RenderTextureScripting::GetDepthBuffer", HasExplicitThis = true)]
		private RenderBuffer GetDepthBuffer()
		{
			RenderBuffer renderBuffer;
			this.GetDepthBuffer_Injected(out renderBuffer);
			return renderBuffer;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetMipMapCount(int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetShadowSamplingMode(ShadowSamplingMode samplingMode);

		public RenderBuffer colorBuffer
		{
			get
			{
				return this.GetColorBuffer();
			}
		}

		public RenderBuffer depthBuffer
		{
			get
			{
				return this.GetDepthBuffer();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern IntPtr GetNativeDepthBufferPtr();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DiscardContents(bool discardColor, bool discardDepth);

		[Obsolete("This function has no effect.", false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void MarkRestoreExpected();

		public void DiscardContents()
		{
			this.DiscardContents(true, true);
		}

		[NativeName("ResolveAntiAliasedSurface")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ResolveAA();

		[NativeName("ResolveAntiAliasedSurface")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ResolveAATo(RenderTexture rt);

		public void ResolveAntiAliasedSurface()
		{
			this.ResolveAA();
		}

		public void ResolveAntiAliasedSurface(RenderTexture target)
		{
			this.ResolveAATo(target);
		}

		[FreeFunction(Name = "RenderTextureScripting::SetGlobalShaderProperty", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalShaderProperty(string propertyName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool Create();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Release();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsCreated();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GenerateMips();

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ConvertToEquirect(RenderTexture equirect, Camera.MonoOrStereoscopicEye eye = Camera.MonoOrStereoscopicEye.Mono);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetSRGBReadWrite(bool srgb);

		[FreeFunction("RenderTextureScripting::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] RenderTexture rt);

		[FreeFunction("RenderTextureSupportsStencil")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SupportsStencil(RenderTexture rt);

		[NativeName("SetRenderTextureDescFromScript")]
		private void SetRenderTextureDescriptor(RenderTextureDescriptor desc)
		{
			this.SetRenderTextureDescriptor_Injected(ref desc);
		}

		[NativeName("GetRenderTextureDesc")]
		private RenderTextureDescriptor GetDescriptor()
		{
			RenderTextureDescriptor renderTextureDescriptor;
			this.GetDescriptor_Injected(out renderTextureDescriptor);
			return renderTextureDescriptor;
		}

		[FreeFunction("GetRenderBufferManager().GetTextures().GetTempBuffer")]
		private static RenderTexture GetTemporary_Internal(RenderTextureDescriptor desc)
		{
			return RenderTexture.GetTemporary_Internal_Injected(ref desc);
		}

		[FreeFunction("GetRenderBufferManager().GetTextures().ReleaseTempBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ReleaseTemporary(RenderTexture temp);

		public extern int depth
		{
			[FreeFunction("RenderTextureScripting::GetDepth", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("RenderTextureScripting::SetDepth", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[RequiredByNativeCode]
		protected internal RenderTexture()
		{
		}

		public RenderTexture(RenderTextureDescriptor desc)
		{
			RenderTexture.ValidateRenderTextureDesc(desc);
			RenderTexture.Internal_Create(this);
			this.SetRenderTextureDescriptor(desc);
		}

		public RenderTexture(RenderTexture textureToCopy)
		{
			bool flag = textureToCopy == null;
			if (flag)
			{
				throw new ArgumentNullException("textureToCopy");
			}
			RenderTexture.ValidateRenderTextureDesc(textureToCopy.descriptor);
			RenderTexture.Internal_Create(this);
			this.SetRenderTextureDescriptor(textureToCopy.descriptor);
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, int depth, DefaultFormat format)
			: this(width, height, RenderTexture.GetDefaultColorFormat(format), RenderTexture.GetDefaultDepthStencilFormat(format, depth), Texture.GenerateAllMips)
		{
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, int depth, GraphicsFormat format)
			: this(width, height, depth, format, Texture.GenerateAllMips)
		{
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, int depth, GraphicsFormat format, int mipCount)
		{
			bool flag = format != GraphicsFormat.None && !base.ValidateFormat(format, FormatUsage.Render);
			if (!flag)
			{
				RenderTexture.Internal_Create(this);
				this.depthStencilFormat = RenderTexture.GetDepthStencilFormatLegacy(depth, format);
				this.width = width;
				this.height = height;
				this.graphicsFormat = format;
				this.SetMipMapCount(mipCount);
				this.SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(format));
			}
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat, int mipCount)
		{
			bool flag = colorFormat != GraphicsFormat.None && !base.ValidateFormat(colorFormat, FormatUsage.Render);
			if (!flag)
			{
				RenderTexture.Internal_Create(this);
				this.width = width;
				this.height = height;
				this.depthStencilFormat = depthStencilFormat;
				this.graphicsFormat = colorFormat;
				this.SetMipMapCount(mipCount);
				this.SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(colorFormat));
			}
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat)
			: this(width, height, colorFormat, depthStencilFormat, Texture.GenerateAllMips)
		{
		}

		public RenderTexture(int width, int height, int depth, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite)
		{
			this.Initialize(width, height, depth, format, readWrite, Texture.GenerateAllMips);
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, int depth, RenderTextureFormat format)
			: this(width, height, depth, format, Texture.GenerateAllMips)
		{
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, int depth)
			: this(width, height, depth, RenderTextureFormat.Default)
		{
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, int depth, RenderTextureFormat format, int mipCount)
		{
			this.Initialize(width, height, depth, format, RenderTextureReadWrite.Default, mipCount);
		}

		private void Initialize(int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readWrite, int mipCount)
		{
			GraphicsFormat compatibleFormat = RenderTexture.GetCompatibleFormat(format, readWrite);
			GraphicsFormat depthStencilFormatLegacy = RenderTexture.GetDepthStencilFormatLegacy(depth, format);
			bool flag = compatibleFormat > GraphicsFormat.None;
			if (flag)
			{
				bool flag2 = !base.ValidateFormat(compatibleFormat, FormatUsage.Render);
				if (flag2)
				{
					return;
				}
			}
			RenderTexture.Internal_Create(this);
			this.width = width;
			this.height = height;
			this.depthStencilFormat = depthStencilFormatLegacy;
			this.graphicsFormat = compatibleFormat;
			this.SetMipMapCount(mipCount);
			this.SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(compatibleFormat));
		}

		internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, GraphicsFormat colorFormat)
		{
			return (colorFormat == GraphicsFormat.ShadowAuto) ? GraphicsFormatUtility.GetDepthStencilFormat(depthBits, 0) : GraphicsFormatUtility.GetDepthStencilFormat(depthBits);
		}

		internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, RenderTextureFormat format)
		{
			return RenderTexture.GetDepthStencilFormatLegacy(depthBits, format == RenderTextureFormat.Shadowmap);
		}

		internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, DefaultFormat format)
		{
			return RenderTexture.GetDepthStencilFormatLegacy(depthBits, format == DefaultFormat.Shadow);
		}

		internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, bool requestedShadowMap)
		{
			return requestedShadowMap ? GraphicsFormatUtility.GetDepthStencilFormat(depthBits, 0) : GraphicsFormatUtility.GetDepthStencilFormat(depthBits);
		}

		public RenderTextureDescriptor descriptor
		{
			get
			{
				return this.GetDescriptor();
			}
			set
			{
				RenderTexture.ValidateRenderTextureDesc(value);
				this.SetRenderTextureDescriptor(value);
			}
		}

		private static void ValidateRenderTextureDesc(RenderTextureDescriptor desc)
		{
			bool flag = desc.graphicsFormat == GraphicsFormat.None && desc.depthStencilFormat == GraphicsFormat.None;
			if (flag)
			{
				throw new ArgumentException("RenderTextureDesc graphicsFormat and depthStencilFormat cannot both be None.");
			}
			bool flag2 = desc.graphicsFormat != GraphicsFormat.None && !SystemInfo.IsFormatSupported(desc.graphicsFormat, FormatUsage.Render);
			if (flag2)
			{
				throw new ArgumentException("RenderTextureDesc graphicsFormat must be a supported GraphicsFormat. " + desc.graphicsFormat.ToString() + " is not supported on this platform.", "desc.graphicsFormat");
			}
			bool flag3 = desc.depthStencilFormat != GraphicsFormat.None && !GraphicsFormatUtility.IsDepthStencilFormat(desc.depthStencilFormat);
			if (flag3)
			{
				throw new ArgumentException("RenderTextureDesc depthStencilFormat must be a supported depth/stencil GraphicsFormat. " + desc.depthStencilFormat.ToString() + " is not supported on this platform.", "desc.depthStencilFormat");
			}
			bool flag4 = desc.width <= 0;
			if (flag4)
			{
				throw new ArgumentException("RenderTextureDesc width must be greater than zero.", "desc.width");
			}
			bool flag5 = desc.height <= 0;
			if (flag5)
			{
				throw new ArgumentException("RenderTextureDesc height must be greater than zero.", "desc.height");
			}
			bool flag6 = desc.volumeDepth <= 0;
			if (flag6)
			{
				throw new ArgumentException("RenderTextureDesc volumeDepth must be greater than zero.", "desc.volumeDepth");
			}
			bool flag7 = desc.msaaSamples != 1 && desc.msaaSamples != 2 && desc.msaaSamples != 4 && desc.msaaSamples != 8;
			if (flag7)
			{
				throw new ArgumentException("RenderTextureDesc msaaSamples must be 1, 2, 4, or 8.", "desc.msaaSamples");
			}
			bool flag8 = desc.dimension == TextureDimension.CubeArray && desc.volumeDepth % 6 != 0;
			if (flag8)
			{
				throw new ArgumentException("RenderTextureDesc volumeDepth must be a multiple of 6 when dimension is CubeArray", "desc.volumeDepth");
			}
			bool flag9 = desc.graphicsFormat != GraphicsFormat.ShadowAuto && desc.graphicsFormat != GraphicsFormat.DepthAuto && GraphicsFormatUtility.IsDepthStencilFormat(desc.graphicsFormat);
			if (flag9)
			{
				throw new ArgumentException("RenderTextureDesc graphicsFormat must not be a depth/stencil format. " + desc.graphicsFormat.ToString() + " is not supported.", "desc.graphicsFormat");
			}
		}

		internal static GraphicsFormat GetDefaultColorFormat(DefaultFormat format)
		{
			GraphicsFormat graphicsFormat;
			if (format != DefaultFormat.DepthStencil)
			{
				if (format != DefaultFormat.Shadow)
				{
					graphicsFormat = SystemInfo.GetGraphicsFormat(format);
				}
				else
				{
					graphicsFormat = GraphicsFormat.ShadowAuto;
				}
			}
			else
			{
				graphicsFormat = GraphicsFormat.DepthAuto;
			}
			return graphicsFormat;
		}

		internal static GraphicsFormat GetDefaultDepthStencilFormat(DefaultFormat format, int depth)
		{
			GraphicsFormat graphicsFormat;
			if (format - DefaultFormat.DepthStencil > 1)
			{
				graphicsFormat = RenderTexture.GetDepthStencilFormatLegacy(depth, format);
			}
			else
			{
				graphicsFormat = SystemInfo.GetGraphicsFormat(format);
			}
			return graphicsFormat;
		}

		internal static GraphicsFormat GetCompatibleFormat(RenderTextureFormat renderTextureFormat, RenderTextureReadWrite readWrite)
		{
			GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(renderTextureFormat, readWrite);
			GraphicsFormat compatibleFormat = SystemInfo.GetCompatibleFormat(graphicsFormat, FormatUsage.Render);
			bool flag = graphicsFormat == compatibleFormat;
			GraphicsFormat graphicsFormat2;
			if (flag)
			{
				graphicsFormat2 = graphicsFormat;
			}
			else
			{
				Debug.LogWarning(string.Format("'{0}' is not supported. RenderTexture::GetTemporary fallbacks to {1} format on this platform. Use 'SystemInfo.IsFormatSupported' C# API to check format support.", graphicsFormat.ToString(), compatibleFormat.ToString()));
				graphicsFormat2 = compatibleFormat;
			}
			return graphicsFormat2;
		}

		public static RenderTexture GetTemporary(RenderTextureDescriptor desc)
		{
			RenderTexture.ValidateRenderTextureDesc(desc);
			desc.createdFromScript = true;
			return RenderTexture.GetTemporary_Internal(desc);
		}

		private static RenderTexture GetTemporaryImpl(int width, int height, GraphicsFormat depthStencilFormat, GraphicsFormat colorFormat, int antiAliasing = 1, RenderTextureMemoryless memorylessMode = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, bool useDynamicScale = false)
		{
			return RenderTexture.GetTemporary(new RenderTextureDescriptor(width, height, colorFormat, depthStencilFormat)
			{
				msaaSamples = antiAliasing,
				memoryless = memorylessMode,
				vrUsage = vrUsage,
				useDynamicScale = useDynamicScale
			});
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, [DefaultValue("1")] int antiAliasing, [DefaultValue("RenderTextureMemoryless.None")] RenderTextureMemoryless memorylessMode, [DefaultValue("VRTextureUsage.None")] VRTextureUsage vrUsage, [DefaultValue("false")] bool useDynamicScale)
		{
			return RenderTexture.GetTemporaryImpl(width, height, RenderTexture.GetDepthStencilFormatLegacy(depthBuffer, format), format, antiAliasing, memorylessMode, vrUsage, useDynamicScale);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, int antiAliasing, RenderTextureMemoryless memorylessMode, VRTextureUsage vrUsage)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, antiAliasing, memorylessMode, vrUsage, false);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, int antiAliasing, RenderTextureMemoryless memorylessMode)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, antiAliasing, memorylessMode, VRTextureUsage.None);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, int antiAliasing)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, antiAliasing, RenderTextureMemoryless.None);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, 1);
		}

		public static RenderTexture GetTemporary(int width, int height, [DefaultValue("0")] int depthBuffer, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [DefaultValue("1")] int antiAliasing, [DefaultValue("RenderTextureMemoryless.None")] RenderTextureMemoryless memorylessMode, [DefaultValue("VRTextureUsage.None")] VRTextureUsage vrUsage, [DefaultValue("false")] bool useDynamicScale)
		{
			GraphicsFormat compatibleFormat = RenderTexture.GetCompatibleFormat(format, readWrite);
			GraphicsFormat depthStencilFormatLegacy = RenderTexture.GetDepthStencilFormatLegacy(depthBuffer, format);
			return RenderTexture.GetTemporaryImpl(width, height, depthStencilFormatLegacy, compatibleFormat, antiAliasing, memorylessMode, vrUsage, useDynamicScale);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, RenderTextureMemoryless memorylessMode, VRTextureUsage vrUsage)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode, vrUsage, false);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, RenderTextureMemoryless memorylessMode)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode, VRTextureUsage.None);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, RenderTextureMemoryless.None);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, 1);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, RenderTextureReadWrite.Default);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, RenderTextureFormat.Default);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height)
		{
			return RenderTexture.GetTemporary(width, height, 0);
		}

		[Obsolete("Use RenderTexture.dimension instead.", false)]
		public bool isCubemap
		{
			get
			{
				return this.dimension == TextureDimension.Cube;
			}
			set
			{
				this.dimension = (value ? TextureDimension.Cube : TextureDimension.Tex2D);
			}
		}

		[Obsolete("Use RenderTexture.dimension instead.", false)]
		public bool isVolume
		{
			get
			{
				return this.dimension == TextureDimension.Tex3D;
			}
			set
			{
				this.dimension = (value ? TextureDimension.Tex3D : TextureDimension.Tex2D);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("RenderTexture.enabled is always now, no need to use it.", false)]
		public static bool enabled
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		[Obsolete("GetTexelOffset always returns zero now, no point in using it.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Vector2 GetTexelOffset()
		{
			return Vector2.zero;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetColorBuffer_Injected(out RenderBuffer ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetDepthBuffer_Injected(out RenderBuffer ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetRenderTextureDescriptor_Injected(ref RenderTextureDescriptor desc);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetDescriptor_Injected(out RenderTextureDescriptor ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RenderTexture GetTemporary_Internal_Injected(ref RenderTextureDescriptor desc);
	}
}
