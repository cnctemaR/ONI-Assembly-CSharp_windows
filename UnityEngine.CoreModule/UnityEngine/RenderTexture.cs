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
	[NativeHeader("Runtime/Graphics/RenderTexture.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Camera/Camera.h")]
	[NativeHeader("Runtime/Graphics/RenderBufferManager.h")]
	[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
	public class RenderTexture : Texture
	{
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
			if (textureToCopy == null)
			{
				throw new ArgumentNullException("textureToCopy");
			}
			RenderTexture.ValidateRenderTextureDesc(textureToCopy.descriptor);
			RenderTexture.Internal_Create(this);
			this.SetRenderTextureDescriptor(textureToCopy.descriptor);
		}

		public RenderTexture(int width, int height, int depth, GraphicsFormat format)
		{
			if (base.ValidateFormat(format, FormatUsage.Render))
			{
				RenderTexture.Internal_Create(this);
				this.width = width;
				this.height = height;
				this.depth = depth;
				this.format = GraphicsFormatUtility.GetRenderTextureFormat(format);
				this.SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(format));
			}
		}

		public RenderTexture(int width, int height, int depth, [UnityEngine.Internal.DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [UnityEngine.Internal.DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite)
		{
			if (base.ValidateFormat(format))
			{
				RenderTexture.Internal_Create(this);
				this.width = width;
				this.height = height;
				this.depth = depth;
				this.format = format;
				bool flag = QualitySettings.activeColorSpace == ColorSpace.Linear;
				this.SetSRGBReadWrite((readWrite != RenderTextureReadWrite.Default) ? (readWrite == RenderTextureReadWrite.sRGB) : flag);
			}
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, int depth, RenderTextureFormat format)
			: this(width, height, depth, format, RenderTextureReadWrite.Default)
		{
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, int depth)
			: this(width, height, depth, RenderTextureFormat.Default, RenderTextureReadWrite.Default)
		{
		}

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

		[NativeProperty("ColorFormat")]
		public extern RenderTextureFormat format
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
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
			if (desc.width <= 0)
			{
				throw new ArgumentException("RenderTextureDesc width must be greater than zero.", "desc.width");
			}
			if (desc.height <= 0)
			{
				throw new ArgumentException("RenderTextureDesc height must be greater than zero.", "desc.height");
			}
			if (desc.volumeDepth <= 0)
			{
				throw new ArgumentException("RenderTextureDesc volumeDepth must be greater than zero.", "desc.volumeDepth");
			}
			if (desc.msaaSamples != 1 && desc.msaaSamples != 2 && desc.msaaSamples != 4 && desc.msaaSamples != 8)
			{
				throw new ArgumentException("RenderTextureDesc msaaSamples must be 1, 2, 4, or 8.", "desc.msaaSamples");
			}
			if (desc.depthBufferBits != 0 && desc.depthBufferBits != 16 && desc.depthBufferBits != 24)
			{
				throw new ArgumentException("RenderTextureDesc depthBufferBits must be 0, 16, or 24.", "desc.depthBufferBits");
			}
		}

		public static RenderTexture GetTemporary(RenderTextureDescriptor desc)
		{
			RenderTexture.ValidateRenderTextureDesc(desc);
			desc.createdFromScript = true;
			return RenderTexture.GetTemporary_Internal(desc);
		}

		private static RenderTexture GetTemporaryImpl(int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, int antiAliasing = 1, RenderTextureMemoryless memorylessMode = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, bool useDynamicScale = false)
		{
			return RenderTexture.GetTemporary(new RenderTextureDescriptor(width, height, format, depthBuffer)
			{
				sRGB = (readWrite != RenderTextureReadWrite.Linear),
				msaaSamples = antiAliasing,
				memoryless = memorylessMode,
				vrUsage = vrUsage,
				useDynamicScale = useDynamicScale
			});
		}

		public static RenderTexture GetTemporary(int width, int height, [UnityEngine.Internal.DefaultValue("0")] int depthBuffer, [UnityEngine.Internal.DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [UnityEngine.Internal.DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [UnityEngine.Internal.DefaultValue("1")] int antiAliasing, [UnityEngine.Internal.DefaultValue("RenderTextureMemoryless.None")] RenderTextureMemoryless memorylessMode, [UnityEngine.Internal.DefaultValue("VRTextureUsage.None")] VRTextureUsage vrUsage, [UnityEngine.Internal.DefaultValue("false")] bool useDynamicScale)
		{
			return RenderTexture.GetTemporaryImpl(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode, vrUsage, useDynamicScale);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, RenderTextureMemoryless memorylessMode, VRTextureUsage vrUsage)
		{
			return RenderTexture.GetTemporaryImpl(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode, vrUsage, false);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, RenderTextureMemoryless memorylessMode)
		{
			return RenderTexture.GetTemporaryImpl(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode, VRTextureUsage.None, false);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
		{
			return RenderTexture.GetTemporaryImpl(width, height, depthBuffer, format, readWrite, antiAliasing, RenderTextureMemoryless.None, VRTextureUsage.None, false);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			return RenderTexture.GetTemporaryImpl(width, height, depthBuffer, format, readWrite, 1, RenderTextureMemoryless.None, VRTextureUsage.None, false);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format)
		{
			return RenderTexture.GetTemporaryImpl(width, height, depthBuffer, format, RenderTextureReadWrite.Default, 1, RenderTextureMemoryless.None, VRTextureUsage.None, false);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer)
		{
			return RenderTexture.GetTemporaryImpl(width, height, depthBuffer, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 1, RenderTextureMemoryless.None, VRTextureUsage.None, false);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height)
		{
			return RenderTexture.GetTemporaryImpl(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 1, RenderTextureMemoryless.None, VRTextureUsage.None, false);
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
				this.dimension = ((!value) ? TextureDimension.Tex2D : TextureDimension.Cube);
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
				this.dimension = ((!value) ? TextureDimension.Tex2D : TextureDimension.Tex3D);
			}
		}

		[Obsolete("RenderTexture.enabled is always now, no need to use it.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("GetTexelOffset always returns zero now, no point in using it.", false)]
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
