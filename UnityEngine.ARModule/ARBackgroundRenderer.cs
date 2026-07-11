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
				bool flag = this.m_BackgroundMaterial == value;
				if (!flag)
				{
					this.RemoveCommandBuffersIfNeeded();
					this.m_BackgroundMaterial = value;
					bool flag2 = this.backgroundRendererChanged != null;
					if (flag2)
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
				bool flag = value;
				if (!flag)
				{
					this.RemoveCommandBuffersIfNeeded();
					this.m_BackgroundTexture = value;
					bool flag2 = this.backgroundRendererChanged != null;
					if (flag2)
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
				return (this.m_Camera != null) ? this.m_Camera : Camera.main;
			}
			set
			{
				bool flag = this.m_Camera == value;
				if (!flag)
				{
					this.RemoveCommandBuffersIfNeeded();
					this.m_Camera = value;
					bool flag2 = this.backgroundRendererChanged != null;
					if (flag2)
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
				bool flag = value == this.m_RenderMode;
				if (!flag)
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
					bool flag2 = this.backgroundRendererChanged != null;
					if (flag2)
					{
						this.backgroundRendererChanged();
					}
				}
			}
		}

		protected bool EnableARBackgroundRendering()
		{
			bool flag = this.m_BackgroundMaterial == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this.m_Camera != null;
				Camera camera;
				if (flag3)
				{
					camera = this.m_Camera;
				}
				else
				{
					camera = Camera.main;
				}
				bool flag4 = camera == null;
				if (flag4)
				{
					flag2 = false;
				}
				else
				{
					this.m_CameraClearFlags = camera.clearFlags;
					camera.clearFlags = CameraClearFlags.Depth;
					this.m_CommandBuffer = new CommandBuffer();
					Texture texture = this.m_BackgroundTexture;
					bool flag5 = texture == null;
					if (flag5)
					{
						bool flag6 = this.m_BackgroundMaterial.HasProperty("_MainTex");
						if (flag6)
						{
							texture = this.m_BackgroundMaterial.GetTexture("_MainTex");
						}
					}
					this.m_CommandBuffer.Blit(texture, BuiltinRenderTextureType.CameraTarget, this.m_BackgroundMaterial);
					camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, this.m_CommandBuffer);
					camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, this.m_CommandBuffer);
					flag2 = true;
				}
			}
			return flag2;
		}

		protected void DisableARBackgroundRendering()
		{
			bool flag = this.m_CommandBuffer == null;
			if (!flag)
			{
				Camera camera = this.m_Camera ?? Camera.main;
				bool flag2 = camera == null;
				if (!flag2)
				{
					camera.clearFlags = this.m_CameraClearFlags;
					camera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, this.m_CommandBuffer);
					camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, this.m_CommandBuffer);
				}
			}
		}

		private bool ReapplyCommandBuffersIfNeeded()
		{
			bool flag = this.m_RenderMode != ARRenderMode.MaterialAsBackground;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this.EnableARBackgroundRendering();
				flag2 = true;
			}
			return flag2;
		}

		private bool RemoveCommandBuffersIfNeeded()
		{
			bool flag = this.m_RenderMode != ARRenderMode.MaterialAsBackground;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this.DisableARBackgroundRendering();
				flag2 = true;
			}
			return flag2;
		}

		protected Camera m_Camera = null;

		protected Material m_BackgroundMaterial = null;

		protected Texture m_BackgroundTexture = null;

		private ARRenderMode m_RenderMode = ARRenderMode.StandardBackground;

		private CommandBuffer m_CommandBuffer = null;

		private CameraClearFlags m_CameraClearFlags = CameraClearFlags.Skybox;
	}
}
