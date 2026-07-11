using System;
using UnityEngine.Rendering;

namespace UnityEngine
{
	/// <summary>
	///   <para>This struct contains all the information required to create a RenderTexture. It can be copied, cached, and reused to easily create RenderTextures that all share the same properties.</para>
	/// </summary>
	public struct RenderTextureDescriptor
	{
		/// <summary>
		///   <para>Create a RenderTextureDescriptor with default values, or a certain width, height, and format.</para>
		/// </summary>
		/// <param name="width">Width of the RenderTexture in pixels.</param>
		/// <param name="height">Height of the RenderTexture in pixels.</param>
		/// <param name="colorFormat">The color format for the RenderTexture.</param>
		/// <param name="depthBufferBits">The number of bits to use for the depth buffer.</param>
		public RenderTextureDescriptor(int width, int height)
		{
			this = new RenderTextureDescriptor(width, height, RenderTextureFormat.Default, 0);
		}

		/// <summary>
		///   <para>Create a RenderTextureDescriptor with default values, or a certain width, height, and format.</para>
		/// </summary>
		/// <param name="width">Width of the RenderTexture in pixels.</param>
		/// <param name="height">Height of the RenderTexture in pixels.</param>
		/// <param name="colorFormat">The color format for the RenderTexture.</param>
		/// <param name="depthBufferBits">The number of bits to use for the depth buffer.</param>
		public RenderTextureDescriptor(int width, int height, RenderTextureFormat colorFormat)
		{
			this = new RenderTextureDescriptor(width, height, colorFormat, 0);
		}

		/// <summary>
		///   <para>Create a RenderTextureDescriptor with default values, or a certain width, height, and format.</para>
		/// </summary>
		/// <param name="width">Width of the RenderTexture in pixels.</param>
		/// <param name="height">Height of the RenderTexture in pixels.</param>
		/// <param name="colorFormat">The color format for the RenderTexture.</param>
		/// <param name="depthBufferBits">The number of bits to use for the depth buffer.</param>
		public RenderTextureDescriptor(int width, int height, RenderTextureFormat colorFormat, int depthBufferBits)
		{
			this = default(RenderTextureDescriptor);
			this.width = width;
			this.height = height;
			this.volumeDepth = 1;
			this.msaaSamples = 1;
			this.colorFormat = colorFormat;
			this.depthBufferBits = depthBufferBits;
			this.dimension = TextureDimension.Tex2D;
			this.shadowSamplingMode = ShadowSamplingMode.None;
			this.vrUsage = VRTextureUsage.None;
			this._flags = RenderTextureCreationFlags.AutoGenerateMips | RenderTextureCreationFlags.AllowVerticalFlip;
			this.memoryless = RenderTextureMemoryless.None;
		}

		/// <summary>
		///   <para>The width of the render texture in pixels.</para>
		/// </summary>
		public int width { get; set; }

		/// <summary>
		///   <para>The height of the render texture in pixels.</para>
		/// </summary>
		public int height { get; set; }

		/// <summary>
		///   <para>The multisample antialiasing level for the RenderTexture.
		///
		/// See RenderTexture.antiAliasing.</para>
		/// </summary>
		public int msaaSamples { get; set; }

		/// <summary>
		///   <para>Volume extent of a 3D render texture.</para>
		/// </summary>
		public int volumeDepth { get; set; }

		/// <summary>
		///   <para>The color format for the RenderTexture.</para>
		/// </summary>
		public RenderTextureFormat colorFormat { get; set; }

		/// <summary>
		///   <para>The precision of the render texture's depth buffer in bits (0, 16, 24/32 are supported).
		///
		/// See RenderTexture.depth.</para>
		/// </summary>
		public int depthBufferBits
		{
			get
			{
				return RenderTextureDescriptor.depthFormatBits[this._depthBufferBits];
			}
			set
			{
				if (value <= 0)
				{
					this._depthBufferBits = 0;
				}
				else if (value <= 16)
				{
					this._depthBufferBits = 1;
				}
				else
				{
					this._depthBufferBits = 2;
				}
			}
		}

		/// <summary>
		///   <para>Dimensionality (type) of the render texture.
		///
		/// See RenderTexture.dimension.</para>
		/// </summary>
		public TextureDimension dimension { get; set; }

		/// <summary>
		///   <para>Determines how the RenderTexture is sampled if it is used as a shadow map.
		///
		/// See ShadowSamplingMode for more details.</para>
		/// </summary>
		public ShadowSamplingMode shadowSamplingMode { get; set; }

		/// <summary>
		///   <para>If this RenderTexture is a VR eye texture used in stereoscopic rendering, this property decides what special rendering occurs, if any. Instead of setting this manually, use the value returned by XR.XRSettings.eyeTextureDesc|eyeTextureDesc or other VR functions returning a RenderTextureDescriptor.</para>
		/// </summary>
		public VRTextureUsage vrUsage { get; set; }

		/// <summary>
		///   <para>A set of RenderTextureCreationFlags that control how the texture is created.</para>
		/// </summary>
		public RenderTextureCreationFlags flags
		{
			get
			{
				return this._flags;
			}
		}

		/// <summary>
		///   <para>The render texture memoryless mode property.</para>
		/// </summary>
		public RenderTextureMemoryless memoryless { get; set; }

		private void SetOrClearRenderTextureCreationFlag(bool value, RenderTextureCreationFlags flag)
		{
			if (value)
			{
				this._flags |= flag;
			}
			else
			{
				this._flags &= ~flag;
			}
		}

		/// <summary>
		///   <para>This flag causes the render texture uses sRGB read/write conversions.</para>
		/// </summary>
		public bool sRGB
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.SRGB) != (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.SRGB);
			}
		}

		/// <summary>
		///   <para>Render texture has mipmaps when this flag is set.
		///
		/// See RenderTexture.useMipMap.</para>
		/// </summary>
		public bool useMipMap
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.MipMap) != (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.MipMap);
			}
		}

		/// <summary>
		///   <para>Mipmap levels are generated automatically when this flag is set.</para>
		/// </summary>
		public bool autoGenerateMips
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.AutoGenerateMips) != (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.AutoGenerateMips);
			}
		}

		/// <summary>
		///   <para>Enable random access write into this render texture on Shader Model 5.0 level shaders.
		///
		/// See RenderTexture.enableRandomWrite.</para>
		/// </summary>
		public bool enableRandomWrite
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.EnableRandomWrite) != (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.EnableRandomWrite);
			}
		}

		/// <summary>
		///   <para>If true and msaaSamples is greater than 1, the render texture will not be resolved by default.  Use this if the render texture needs to be bound as a multisampled texture in a shader.</para>
		/// </summary>
		public bool bindMS
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.BindMS) != (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.BindMS);
			}
		}

		internal bool createdFromScript
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.CreatedFromScript) != (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.CreatedFromScript);
			}
		}

		internal bool useDynamicScale
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.DynamicallyScalable) != (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.DynamicallyScalable);
			}
		}

		private int _depthBufferBits;

		private static int[] depthFormatBits = new int[] { 0, 16, 24 };

		private RenderTextureCreationFlags _flags;
	}
}
