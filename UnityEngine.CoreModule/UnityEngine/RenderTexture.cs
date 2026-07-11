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
	/// <summary>
	///   <para>Render textures are textures that can be rendered to.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Graphics/RenderTexture.h")]
	[NativeHeader("Runtime/Camera/Camera.h")]
	public class RenderTexture : Texture
	{
		[RequiredByNativeCode]
		protected internal RenderTexture()
		{
		}

		/// <summary>
		///   <para>Creates a new RenderTexture object.</para>
		/// </summary>
		/// <param name="width">Texture width in pixels.</param>
		/// <param name="height">Texture height in pixels.</param>
		/// <param name="depth">Number of bits in depth buffer (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
		/// <param name="format">Texture color format.</param>
		/// <param name="readWrite">How or if color space conversions should be done on texture read/write.</param>
		/// <param name="desc">Create the RenderTexture with the settings in the RenderTextureDescriptor.</param>
		/// <param name="textureToCopy">Copy the settings from another RenderTexture.</param>
		public RenderTexture(RenderTextureDescriptor desc)
		{
			RenderTexture.ValidateRenderTextureDesc(desc);
			RenderTexture.Internal_Create(this);
			this.SetRenderTextureDescriptor(desc);
		}

		/// <summary>
		///   <para>Creates a new RenderTexture object.</para>
		/// </summary>
		/// <param name="width">Texture width in pixels.</param>
		/// <param name="height">Texture height in pixels.</param>
		/// <param name="depth">Number of bits in depth buffer (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
		/// <param name="format">Texture color format.</param>
		/// <param name="readWrite">How or if color space conversions should be done on texture read/write.</param>
		/// <param name="desc">Create the RenderTexture with the settings in the RenderTextureDescriptor.</param>
		/// <param name="textureToCopy">Copy the settings from another RenderTexture.</param>
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

		/// <summary>
		///   <para>Creates a new RenderTexture object.</para>
		/// </summary>
		/// <param name="width">Texture width in pixels.</param>
		/// <param name="height">Texture height in pixels.</param>
		/// <param name="depth">Number of bits in depth buffer (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
		/// <param name="format">Texture color format.</param>
		/// <param name="readWrite">How or if color space conversions should be done on texture read/write.</param>
		/// <param name="desc">Create the RenderTexture with the settings in the RenderTextureDescriptor.</param>
		/// <param name="textureToCopy">Copy the settings from another RenderTexture.</param>
		public RenderTexture(int width, int height, int depth, [UnityEngine.Internal.DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [UnityEngine.Internal.DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite)
		{
			RenderTexture.Internal_Create(this);
			this.width = width;
			this.height = height;
			this.depth = depth;
			this.format = format;
			bool flag = QualitySettings.activeColorSpace == ColorSpace.Linear;
			this.SetSRGBReadWrite((readWrite != RenderTextureReadWrite.Default) ? (readWrite == RenderTextureReadWrite.sRGB) : flag);
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

		private void SetRenderTextureDescriptor(RenderTextureDescriptor desc)
		{
			RenderTexture.INTERNAL_CALL_SetRenderTextureDescriptor(this, ref desc);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetRenderTextureDescriptor(RenderTexture self, ref RenderTextureDescriptor desc);

		private RenderTextureDescriptor GetDescriptor()
		{
			RenderTextureDescriptor renderTextureDescriptor;
			RenderTexture.INTERNAL_CALL_GetDescriptor(this, out renderTextureDescriptor);
			return renderTextureDescriptor;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetDescriptor(RenderTexture self, out RenderTextureDescriptor value);

		private static RenderTexture GetTemporary_Internal(RenderTextureDescriptor desc)
		{
			return RenderTexture.INTERNAL_CALL_GetTemporary_Internal(ref desc);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RenderTexture INTERNAL_CALL_GetTemporary_Internal(ref RenderTextureDescriptor desc);

		/// <summary>
		///   <para>Release a temporary texture allocated with GetTemporary.</para>
		/// </summary>
		/// <param name="temp"></param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ReleaseTemporary(RenderTexture temp);

		/// <summary>
		///   <para>The precision of the render texture's depth buffer in bits (0, 16, 24/32 are supported).</para>
		/// </summary>
		public extern int depth
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The width of the render texture in pixels.</para>
		/// </summary>
		public override extern int width
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The height of the render texture in pixels.</para>
		/// </summary>
		public override extern int height
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Dimensionality (type) of the render texture.</para>
		/// </summary>
		public override extern TextureDimension dimension
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Render texture has mipmaps when this flag is set.</para>
		/// </summary>
		[NativeProperty("MipMap")]
		public extern bool useMipMap
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Does this render texture use sRGB read/write conversions? (Read Only).</para>
		/// </summary>
		[NativeProperty("SRGBReadWrite")]
		public extern bool sRGB
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The color format of the render texture.</para>
		/// </summary>
		[NativeProperty("ColorFormat")]
		public extern RenderTextureFormat format
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>If this RenderTexture is a VR eye texture used in stereoscopic rendering, this property decides what special rendering occurs, if any.</para>
		/// </summary>
		[NativeProperty("VRUsage")]
		public extern VRTextureUsage vrUsage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The render texture memoryless mode property.</para>
		/// </summary>
		[NativeProperty("Memoryless")]
		public extern RenderTextureMemoryless memorylessMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Mipmap levels are generated automatically when this flag is set.</para>
		/// </summary>
		public extern bool autoGenerateMips
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Volume extent of a 3D render texture or number of slices of array texture.</para>
		/// </summary>
		public extern int volumeDepth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The antialiasing level for the RenderTexture.</para>
		/// </summary>
		public extern int antiAliasing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>If true and antiAliasing is greater than 1, the render texture will not be resolved by default.  Use this if the render texture needs to be bound as a multisampled texture in a shader.</para>
		/// </summary>
		public extern bool bindTextureMS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Enable random access write into this render texture on Shader Model 5.0 level shaders.</para>
		/// </summary>
		public extern bool enableRandomWrite
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Is the render texture marked to be scaled by the Dynamic Resolution system.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Currently active render texture.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Color buffer of the render texture (Read Only).</para>
		/// </summary>
		public RenderBuffer colorBuffer
		{
			get
			{
				return this.GetColorBuffer();
			}
		}

		/// <summary>
		///   <para>Depth/stencil buffer of the render texture (Read Only).</para>
		/// </summary>
		public RenderBuffer depthBuffer
		{
			get
			{
				return this.GetDepthBuffer();
			}
		}

		/// <summary>
		///   <para>Retrieve a native (underlying graphics API) pointer to the depth buffer resource.</para>
		/// </summary>
		/// <returns>
		///   <para>Pointer to an underlying graphics API depth buffer resource.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern IntPtr GetNativeDepthBufferPtr();

		/// <summary>
		///   <para>Hint the GPU driver that the contents of the RenderTexture will not be used.</para>
		/// </summary>
		/// <param name="discardColor">Should the colour buffer be discarded?</param>
		/// <param name="discardDepth">Should the depth buffer be discarded?</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DiscardContents(bool discardColor, bool discardDepth);

		/// <summary>
		///   <para>Indicate that there's a RenderTexture restore operation expected.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void MarkRestoreExpected();

		/// <summary>
		///   <para>Hint the GPU driver that the contents of the RenderTexture will not be used.</para>
		/// </summary>
		/// <param name="discardColor">Should the colour buffer be discarded?</param>
		/// <param name="discardDepth">Should the depth buffer be discarded?</param>
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

		/// <summary>
		///   <para>Force an antialiased render texture to be resolved.</para>
		/// </summary>
		/// <param name="target">The render texture to resolve into.  If set, the target render texture must have the same dimensions and format as the source.</param>
		public void ResolveAntiAliasedSurface()
		{
			this.ResolveAA();
		}

		/// <summary>
		///   <para>Force an antialiased render texture to be resolved.</para>
		/// </summary>
		/// <param name="target">The render texture to resolve into.  If set, the target render texture must have the same dimensions and format as the source.</param>
		public void ResolveAntiAliasedSurface(RenderTexture target)
		{
			this.ResolveAATo(target);
		}

		/// <summary>
		///   <para>Assigns this RenderTexture as a global shader property named propertyName.</para>
		/// </summary>
		/// <param name="propertyName"></param>
		[FreeFunction(Name = "RenderTextureScripting::SetGlobalShaderProperty", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetGlobalShaderProperty(string propertyName);

		/// <summary>
		///   <para>Actually creates the RenderTexture.</para>
		/// </summary>
		/// <returns>
		///   <para>True if the texture is created, else false.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool Create();

		/// <summary>
		///   <para>Releases the RenderTexture.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Release();

		/// <summary>
		///   <para>Is the render texture actually created?</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsCreated();

		/// <summary>
		///   <para>Generate mipmap levels of a render texture.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GenerateMips();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ConvertToEquirect(RenderTexture equirect, Camera.MonoOrStereoscopicEye eye = Camera.MonoOrStereoscopicEye.Mono);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetSRGBReadWrite(bool srgb);

		[FreeFunction("RenderTextureScripting::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] RenderTexture rt);

		/// <summary>
		///   <para>Does a RenderTexture have stencil buffer?</para>
		/// </summary>
		/// <param name="rt">Render texture, or null for main screen.</param>
		[FreeFunction("RenderTextureSupportsStencil")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool SupportsStencil(RenderTexture rt);

		/// <summary>
		///   <para>This struct contains all the information required to create a RenderTexture. It can be copied, cached, and reused to easily create RenderTextures that all share the same properties.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Allocate a temporary render texture.</para>
		/// </summary>
		/// <param name="width">Width in pixels.</param>
		/// <param name="height">Height in pixels.</param>
		/// <param name="depthBuffer">Depth buffer bits (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
		/// <param name="format">Render texture format.</param>
		/// <param name="readWrite">Color space conversion mode.</param>
		/// <param name="antiAliasing">Number of antialiasing samples to store in the texture. Valid values are 1, 2, 4, and 8. Throws an exception if any other value is passed.</param>
		/// <param name="memorylessMode">Render texture memoryless mode.</param>
		/// <param name="desc">Use this RenderTextureDesc for the settings when creating the temporary RenderTexture.</param>
		/// <param name="vrUsage"></param>
		/// <param name="useDynamicScale"></param>
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

		/// <summary>
		///   <para>Allocate a temporary render texture.</para>
		/// </summary>
		/// <param name="width">Width in pixels.</param>
		/// <param name="height">Height in pixels.</param>
		/// <param name="depthBuffer">Depth buffer bits (0, 16 or 24). Note that only 24 bit depth has stencil buffer.</param>
		/// <param name="format">Render texture format.</param>
		/// <param name="readWrite">Color space conversion mode.</param>
		/// <param name="antiAliasing">Number of antialiasing samples to store in the texture. Valid values are 1, 2, 4, and 8. Throws an exception if any other value is passed.</param>
		/// <param name="memorylessMode">Render texture memoryless mode.</param>
		/// <param name="desc">Use this RenderTextureDesc for the settings when creating the temporary RenderTexture.</param>
		/// <param name="vrUsage"></param>
		/// <param name="useDynamicScale"></param>
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

		/// <summary>
		///   <para>If enabled, this Render Texture will be used as a Texture3D.</para>
		/// </summary>
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
	}
}
