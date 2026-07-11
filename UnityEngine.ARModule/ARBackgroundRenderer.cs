using System;
using System.Diagnostics;
using UnityEngine.Rendering;

namespace UnityEngine.XR
{
	public class ARBackgroundRenderer
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action backgroundRendererChanged = null;

		public Material backgroundMaterial
		{
			get
			{
				return this.m_BackgroundMaterial;
			}
			set
			{
				if (!(this.m_BackgroundMaterial == value))
				{
					this.RemoveCommandBuffersIfNeeded();
					this.m_BackgroundMaterial = value;
					if (this.backgroundRendererChanged != null)
					{
						this.backgroundRendererChanged();
					}
					this.ReapplyCommandBuffersIfNeeded();
				}
			}
		}

		public Texture backgroundTexture
		{
			get
			{
				return this.m_BackgroundTexture;
			}
			set
			{
				this.m_BackgroundTexture = value;
				if (!value)
				{
					this.RemoveCommandBuffersIfNeeded();
					this.m_BackgroundTexture = value;
					if (this.backgroundRendererChanged != null)
					{
						this.backgroundRendererChanged();
					}
					this.ReapplyCommandBuffersIfNeeded();
				}
			}
		}

		public Camera camera
		{
			get
			{
				return (!(this.m_Camera != null)) ? Camera.main : this.m_Camera;
			}
			set
			{
				if (!(this.m_Camera == value))
				{
					this.RemoveCommandBuffersIfNeeded();
					this.m_Camera = value;
					if (this.backgroundRendererChanged != null)
					{
						this.backgroundRendererChanged();
					}
					this.ReapplyCommandBuffersIfNeeded();
				}
			}
		}

		public ARRenderMode mode
		{
			get
			{
				return this.m_RenderMode;
			}
			set
			{
				if (value != this.m_RenderMode)
				{
					this.m_RenderMode = value;
					ARRenderMode renderMode = this.m_RenderMode;
					if (renderMode != ARRenderMode.StandardBackground)
					{
						if (renderMode != ARRenderMode.MaterialAsBackground)
						{
							throw new Exception("Unhandled render mode.");
						}
						this.EnableARBackgroundRendering();
					}
					else
					{
						this.DisableARBackgroundRendering();
					}
					if (this.backgroundRendererChanged != null)
					{
						this.backgroundRendererChanged();
					}
				}
			}
		}

		protected bool EnableARBackgroundRendering()
		{
			bool flag;
			if (this.m_BackgroundMaterial == null)
			{
				flag = false;
			}
			else
			{
				Camera camera;
				if (this.m_Camera != null)
				{
					camera = this.m_Camera;
				}
				else
				{
					camera = Camera.main;
				}
				if (camera == null)
				{
					flag = false;
				}
				else
				{
					this.m_CameraClearFlags = camera.clearFlags;
					camera.clearFlags = CameraClearFlags.Depth;
					this.m_CommandBuffer = new CommandBuffer();
					Texture texture = this.m_BackgroundTexture;
					if (texture == null)
					{
						if (this.m_BackgroundMaterial.HasProperty("_MainTex"))
						{
							texture = this.m_BackgroundMaterial.GetTexture("_MainTex");
						}
					}
					this.m_CommandBuffer.Blit(texture, BuiltinRenderTextureType.CameraTarget, this.m_BackgroundMaterial);
					camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, this.m_CommandBuffer);
					camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, this.m_CommandBuffer);
					flag = true;
				}
			}
			return flag;
		}

		protected void DisableARBackgroundRendering()
		{
			if (this.m_CommandBuffer != null)
			{
				Camera camera = this.m_Camera ?? Camera.main;
				if (!(camera == null))
				{
					camera.clearFlags = this.m_CameraClearFlags;
					camera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, this.m_CommandBuffer);
					camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, this.m_CommandBuffer);
				}
			}
		}

		private bool ReapplyCommandBuffersIfNeeded()
		{
			bool flag;
			if (this.m_RenderMode != ARRenderMode.MaterialAsBackground)
			{
				flag = false;
			}
			else
			{
				this.EnableARBackgroundRendering();
				flag = true;
			}
			return flag;
		}

		private bool RemoveCommandBuffersIfNeeded()
		{
			bool flag;
			if (this.m_RenderMode != ARRenderMode.MaterialAsBackground)
			{
				flag = false;
			}
			else
			{
				this.DisableARBackgroundRendering();
				flag = true;
			}
			return flag;
		}

		protected Camera m_Camera = null;

		protected Material m_BackgroundMaterial = null;

		protected Texture m_BackgroundTexture = null;

		private ARRenderMode m_RenderMode = ARRenderMode.StandardBackground;

		private CommandBuffer m_CommandBuffer = null;

		private CameraClearFlags m_CameraClearFlags = CameraClearFlags.Skybox;
	}
}
