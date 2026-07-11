using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Describes a render target with one or more color buffers, a depthstencil buffer and the associated loadstore-actions that are applied when the render target is active.</para>
	/// </summary>
	public struct RenderTargetBinding
	{
		/// <summary>
		///   <para>Constructs RenderTargetBinding.</para>
		/// </summary>
		/// <param name="color">Color buffers to use as render targets.</param>
		/// <param name="depth">Depth buffer to use as render target.</param>
		/// <param name="colorLoadAction">Load actions for color buffers.</param>
		/// <param name="colorStoreAction">Store actions for color buffers.</param>
		/// <param name="depthLoadAction">Load action for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action for the depth/stencil buffer.</param>
		/// <param name="colorRenderTarget"></param>
		/// <param name="depthRenderTarget"></param>
		/// <param name="colorRenderTargets"></param>
		/// <param name="colorLoadActions"></param>
		/// <param name="colorStoreActions"></param>
		/// <param name="setup"></param>
		public RenderTargetBinding(RenderTargetIdentifier[] colorRenderTargets, RenderBufferLoadAction[] colorLoadActions, RenderBufferStoreAction[] colorStoreActions, RenderTargetIdentifier depthRenderTarget, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			this.m_ColorRenderTargets = colorRenderTargets;
			this.m_DepthRenderTarget = depthRenderTarget;
			this.m_ColorLoadActions = colorLoadActions;
			this.m_ColorStoreActions = colorStoreActions;
			this.m_DepthLoadAction = depthLoadAction;
			this.m_DepthStoreAction = depthStoreAction;
		}

		/// <summary>
		///   <para>Constructs RenderTargetBinding.</para>
		/// </summary>
		/// <param name="color">Color buffers to use as render targets.</param>
		/// <param name="depth">Depth buffer to use as render target.</param>
		/// <param name="colorLoadAction">Load actions for color buffers.</param>
		/// <param name="colorStoreAction">Store actions for color buffers.</param>
		/// <param name="depthLoadAction">Load action for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action for the depth/stencil buffer.</param>
		/// <param name="colorRenderTarget"></param>
		/// <param name="depthRenderTarget"></param>
		/// <param name="colorRenderTargets"></param>
		/// <param name="colorLoadActions"></param>
		/// <param name="colorStoreActions"></param>
		/// <param name="setup"></param>
		public RenderTargetBinding(RenderTargetIdentifier colorRenderTarget, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depthRenderTarget, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			this = new RenderTargetBinding(new RenderTargetIdentifier[] { colorRenderTarget }, new RenderBufferLoadAction[] { colorLoadAction }, new RenderBufferStoreAction[] { colorStoreAction }, depthRenderTarget, depthLoadAction, depthStoreAction);
		}

		/// <summary>
		///   <para>Constructs RenderTargetBinding.</para>
		/// </summary>
		/// <param name="color">Color buffers to use as render targets.</param>
		/// <param name="depth">Depth buffer to use as render target.</param>
		/// <param name="colorLoadAction">Load actions for color buffers.</param>
		/// <param name="colorStoreAction">Store actions for color buffers.</param>
		/// <param name="depthLoadAction">Load action for the depth/stencil buffer.</param>
		/// <param name="depthStoreAction">Store action for the depth/stencil buffer.</param>
		/// <param name="colorRenderTarget"></param>
		/// <param name="depthRenderTarget"></param>
		/// <param name="colorRenderTargets"></param>
		/// <param name="colorLoadActions"></param>
		/// <param name="colorStoreActions"></param>
		/// <param name="setup"></param>
		public RenderTargetBinding(RenderTargetSetup setup)
		{
			this.m_ColorRenderTargets = new RenderTargetIdentifier[setup.color.Length];
			for (int i = 0; i < this.m_ColorRenderTargets.Length; i++)
			{
				this.m_ColorRenderTargets[i] = new RenderTargetIdentifier(setup.color[i], setup.mipLevel, setup.cubemapFace, setup.depthSlice);
			}
			this.m_DepthRenderTarget = setup.depth;
			this.m_ColorLoadActions = (RenderBufferLoadAction[])setup.colorLoad.Clone();
			this.m_ColorStoreActions = (RenderBufferStoreAction[])setup.colorStore.Clone();
			this.m_DepthLoadAction = setup.depthLoad;
			this.m_DepthStoreAction = setup.depthStore;
		}

		/// <summary>
		///   <para>Color buffers to use as render targets.</para>
		/// </summary>
		public RenderTargetIdentifier[] colorRenderTargets
		{
			get
			{
				return this.m_ColorRenderTargets;
			}
			set
			{
				this.m_ColorRenderTargets = value;
			}
		}

		/// <summary>
		///   <para>Depth/stencil buffer to use as render target.</para>
		/// </summary>
		public RenderTargetIdentifier depthRenderTarget
		{
			get
			{
				return this.m_DepthRenderTarget;
			}
			set
			{
				this.m_DepthRenderTarget = value;
			}
		}

		/// <summary>
		///   <para>Load actions for color buffers.</para>
		/// </summary>
		public RenderBufferLoadAction[] colorLoadActions
		{
			get
			{
				return this.m_ColorLoadActions;
			}
			set
			{
				this.m_ColorLoadActions = value;
			}
		}

		/// <summary>
		///   <para>Store actions for color buffers.</para>
		/// </summary>
		public RenderBufferStoreAction[] colorStoreActions
		{
			get
			{
				return this.m_ColorStoreActions;
			}
			set
			{
				this.m_ColorStoreActions = value;
			}
		}

		/// <summary>
		///   <para>Load action for the depth/stencil buffer.</para>
		/// </summary>
		public RenderBufferLoadAction depthLoadAction
		{
			get
			{
				return this.m_DepthLoadAction;
			}
			set
			{
				this.m_DepthLoadAction = value;
			}
		}

		/// <summary>
		///   <para>Store action for the depth/stencil buffer.</para>
		/// </summary>
		public RenderBufferStoreAction depthStoreAction
		{
			get
			{
				return this.m_DepthStoreAction;
			}
			set
			{
				this.m_DepthStoreAction = value;
			}
		}

		private RenderTargetIdentifier[] m_ColorRenderTargets;

		private RenderTargetIdentifier m_DepthRenderTarget;

		private RenderBufferLoadAction[] m_ColorLoadActions;

		private RenderBufferStoreAction[] m_ColorStoreActions;

		private RenderBufferLoadAction m_DepthLoadAction;

		private RenderBufferStoreAction m_DepthStoreAction;
	}
}
