using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public struct RenderTextureDescriptor
	{
		public int width { readonly get; set; }

		public int height { readonly get; set; }

		public int msaaSamples { readonly get; set; }

		public int volumeDepth { readonly get; set; }

		public int mipCount { readonly get; set; }

		public GraphicsFormat graphicsFormat
		{
			get
			{
				return this._graphicsFormat;
			}
			set
			{
				this._graphicsFormat = value;
				this.SetOrClearRenderTextureCreationFlag(GraphicsFormatUtility.IsSRGBFormat(value), RenderTextureCreationFlags.SRGB);
			}
		}

		public GraphicsFormat stencilFormat { readonly get; set; }

		public GraphicsFormat depthStencilFormat { readonly get; set; }

		public RenderTextureFormat colorFormat
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
					renderTextureFormat = ((this.shadowSamplingMode != ShadowSamplingMode.None) ? RenderTextureFormat.Shadowmap : RenderTextureFormat.Depth);
				}
				return renderTextureFormat;
			}
			set
			{
				this.shadowSamplingMode = RenderTexture.GetShadowSamplingModeForFormat(value);
				GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(value, this.sRGB);
				this.graphicsFormat = SystemInfo.GetCompatibleFormat(graphicsFormat, GraphicsFormatUsage.Render);
				this.depthStencilFormat = RenderTexture.GetDepthStencilFormatLegacy(this.depthBufferBits, this.shadowSamplingMode);
			}
		}

		public bool sRGB
		{
			get
			{
				return GraphicsFormatUtility.IsSRGBFormat(this.graphicsFormat);
			}
			set
			{
				this.graphicsFormat = ((value && QualitySettings.activeColorSpace == ColorSpace.Linear) ? GraphicsFormatUtility.GetSRGBFormat(this.graphicsFormat) : GraphicsFormatUtility.GetLinearFormat(this.graphicsFormat));
			}
		}

		public int depthBufferBits
		{
			get
			{
				return GraphicsFormatUtility.GetDepthBits(this.depthStencilFormat);
			}
			set
			{
				this.depthStencilFormat = RenderTexture.GetDepthStencilFormatLegacy(value, this.shadowSamplingMode);
			}
		}

		public TextureDimension dimension { readonly get; set; }

		public ShadowSamplingMode shadowSamplingMode { readonly get; set; }

		public VRTextureUsage vrUsage { readonly get; set; }

		public RenderTextureCreationFlags flags
		{
			get
			{
				return this._flags;
			}
		}

		public RenderTextureMemoryless memoryless { readonly get; set; }

		[ExcludeFromDocs]
		public RenderTextureDescriptor(int width, int height)
		{
			this = new RenderTextureDescriptor(width, height, RenderTextureFormat.Default);
		}

		[ExcludeFromDocs]
		public RenderTextureDescriptor(int width, int height, RenderTextureFormat colorFormat)
		{
			this = new RenderTextureDescriptor(width, height, colorFormat, 0);
		}

		[ExcludeFromDocs]
		public RenderTextureDescriptor(int width, int height, RenderTextureFormat colorFormat, int depthBufferBits)
		{
			this = new RenderTextureDescriptor(width, height, colorFormat, depthBufferBits, Texture.GenerateAllMips);
		}

		[ExcludeFromDocs]
		public RenderTextureDescriptor(int width, int height, GraphicsFormat colorFormat, int depthBufferBits)
		{
			this = new RenderTextureDescriptor(width, height, colorFormat, depthBufferBits, Texture.GenerateAllMips);
		}

		[ExcludeFromDocs]
		public RenderTextureDescriptor(int width, int height, RenderTextureFormat colorFormat, int depthBufferBits, int mipCount)
		{
			this = new RenderTextureDescriptor(width, height, colorFormat, depthBufferBits, mipCount, RenderTextureReadWrite.Linear);
		}

		public RenderTextureDescriptor(int width, int height, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat colorFormat, [DefaultValue("0")] int depthBufferBits, [DefaultValue("Texture.GenerateAllMips")] int mipCount, [DefaultValue("RenderTextureReadWrite.Linear")] RenderTextureReadWrite readWrite)
		{
			GraphicsFormat compatibleFormat = RenderTexture.GetCompatibleFormat(colorFormat, readWrite);
			this = new RenderTextureDescriptor(width, height, compatibleFormat, RenderTexture.GetDepthStencilFormatLegacy(depthBufferBits, colorFormat, false), mipCount);
			this.shadowSamplingMode = RenderTexture.GetShadowSamplingModeForFormat(colorFormat);
		}

		[ExcludeFromDocs]
		public RenderTextureDescriptor(int width, int height, GraphicsFormat colorFormat, int depthBufferBits, int mipCount)
		{
			this = default(RenderTextureDescriptor);
			this._flags = RenderTextureCreationFlags.AutoGenerateMips | RenderTextureCreationFlags.AllowVerticalFlip;
			this.width = width;
			this.height = height;
			this.volumeDepth = 1;
			this.msaaSamples = 1;
			this.graphicsFormat = colorFormat;
			this.depthStencilFormat = RenderTexture.GetDepthStencilFormatLegacy(depthBufferBits, colorFormat);
			this.mipCount = mipCount;
			this.dimension = TextureDimension.Tex2D;
			this.shadowSamplingMode = ShadowSamplingMode.None;
			this.vrUsage = VRTextureUsage.None;
			this.memoryless = RenderTextureMemoryless.None;
		}

		[ExcludeFromDocs]
		public RenderTextureDescriptor(int width, int height, GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat)
		{
			this = new RenderTextureDescriptor(width, height, colorFormat, depthStencilFormat, Texture.GenerateAllMips);
		}

		[ExcludeFromDocs]
		public RenderTextureDescriptor(int width, int height, GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat, int mipCount)
		{
			this = default(RenderTextureDescriptor);
			this._flags = RenderTextureCreationFlags.AutoGenerateMips | RenderTextureCreationFlags.AllowVerticalFlip;
			this.width = width;
			this.height = height;
			this.volumeDepth = 1;
			this.msaaSamples = 1;
			this.graphicsFormat = colorFormat;
			this.depthStencilFormat = depthStencilFormat;
			this.mipCount = mipCount;
			this.dimension = TextureDimension.Tex2D;
			this.shadowSamplingMode = ShadowSamplingMode.None;
			this.vrUsage = VRTextureUsage.None;
			this.memoryless = RenderTextureMemoryless.None;
		}

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

		public bool useMipMap
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.MipMap) > (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.MipMap);
			}
		}

		public bool autoGenerateMips
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.AutoGenerateMips) > (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.AutoGenerateMips);
			}
		}

		public bool enableRandomWrite
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.EnableRandomWrite) > (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.EnableRandomWrite);
			}
		}

		public bool bindMS
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.BindMS) > (RenderTextureCreationFlags)0;
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
				return (this._flags & RenderTextureCreationFlags.CreatedFromScript) > (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.CreatedFromScript);
			}
		}

		public bool useDynamicScale
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.DynamicallyScalable) > (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.DynamicallyScalable);
			}
		}

		public bool useDynamicScaleExplicit
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.DynamicallyScalableExplicit) > (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.DynamicallyScalableExplicit);
			}
		}

		public bool enableShadingRate
		{
			get
			{
				return (this._flags & RenderTextureCreationFlags.ShadingRate) > (RenderTextureCreationFlags)0;
			}
			set
			{
				this.SetOrClearRenderTextureCreationFlag(value, RenderTextureCreationFlags.ShadingRate);
			}
		}

		private GraphicsFormat _graphicsFormat;

		private RenderTextureCreationFlags _flags;
	}
}
