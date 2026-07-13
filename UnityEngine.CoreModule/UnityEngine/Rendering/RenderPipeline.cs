using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	public abstract class RenderPipeline
	{
		[Obsolete("Render with an array parameter is deprecated. Use Render with a list parameter instead. If you're extending the RenderPipeline class, override the Render method with a List parameter to perform rendering in order to avoid unnecessary allocations and copies. #from 6000.1", false)]
		protected virtual void Render(ScriptableRenderContext context, Camera[] cameras)
		{
		}

		protected virtual void ProcessRenderRequests<RequestData>(ScriptableRenderContext context, Camera camera, RequestData renderRequest)
		{
		}

		protected internal virtual bool IsRenderRequestSupported<RequestData>(Camera camera, RequestData data)
		{
			return false;
		}

		[Obsolete("BeginFrameRendering is deprecated. Use BeginContextRendering instead. #from 6000.1", false)]
		protected static void BeginFrameRendering(ScriptableRenderContext context, Camera[] cameras)
		{
			RenderPipelineManager.BeginContextRendering(context, new List<Camera>(cameras));
		}

		protected static void BeginContextRendering(ScriptableRenderContext context, List<Camera> cameras)
		{
			RenderPipelineManager.BeginContextRendering(context, cameras);
		}

		protected static void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			RenderPipelineManager.BeginCameraRendering(context, camera);
		}

		protected static void EndContextRendering(ScriptableRenderContext context, List<Camera> cameras)
		{
			RenderPipelineManager.EndContextRendering(context, cameras);
		}

		[Obsolete("EndFrameRendering is deprecated. Use EndContextRendering instead. #from 6000.1", false)]
		protected static void EndFrameRendering(ScriptableRenderContext context, Camera[] cameras)
		{
			RenderPipelineManager.EndContextRendering(context, new List<Camera>(cameras));
		}

		protected static void EndCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			RenderPipelineManager.EndCameraRendering(context, camera);
		}

		protected virtual void Render(ScriptableRenderContext context, List<Camera> cameras)
		{
			this.Render(context, cameras.ToArray());
		}

		internal void InternalRender(ScriptableRenderContext context, List<Camera> cameras)
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				throw new ObjectDisposedException(string.Format("{0} has been disposed. Do not call Render on disposed a RenderPipeline.", this));
			}
			this.Render(context, cameras);
		}

		internal void InternalProcessRenderRequests<RequestData>(ScriptableRenderContext context, Camera camera, RequestData renderRequest)
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				throw new ObjectDisposedException(string.Format("{0} has been disposed. Do not call Render on disposed a RenderPipeline.", this));
			}
			this.ProcessRenderRequests<RequestData>(context, camera, renderRequest);
		}

		public static bool SupportsRenderRequest<RequestData>(Camera camera, RequestData data)
		{
			bool flag = false;
			bool flag2 = GraphicsSettings.currentRenderPipeline != null;
			if (flag2)
			{
				bool flag3 = RenderPipelineManager.currentPipeline == null;
				if (flag3)
				{
					bool flag4 = RenderPipelineManager.TryPrepareRenderPipeline(GraphicsSettings.currentRenderPipeline);
					Debug.Assert(flag4);
				}
				bool flag5 = RenderPipelineManager.currentPipeline != null;
				if (flag5)
				{
					flag = RenderPipelineManager.currentPipeline.IsRenderRequestSupported<RequestData>(camera, data);
				}
			}
			return flag;
		}

		public static void SubmitRenderRequest<RequestData>(Camera camera, RequestData data)
		{
			camera.SubmitRenderRequest<RequestData>(data);
		}

		public bool disposed { get; private set; }

		internal void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
			this.disposed = true;
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public virtual RenderPipelineGlobalSettings defaultSettings
		{
			get
			{
				return null;
			}
		}

		public class StandardRequest
		{
			public RenderTexture destination = null;

			public int mipLevel = 0;

			public CubemapFace face = CubemapFace.Unknown;

			public int slice = 0;
		}
	}
}
