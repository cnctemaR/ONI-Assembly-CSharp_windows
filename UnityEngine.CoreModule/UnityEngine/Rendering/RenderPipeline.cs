using System;

namespace UnityEngine.Rendering
{
	public abstract class RenderPipeline
	{
		protected abstract void Render(ScriptableRenderContext context, Camera[] cameras);

		protected static void BeginFrameRendering(ScriptableRenderContext context, Camera[] cameras)
		{
			RenderPipelineManager.BeginFrameRendering(context, cameras);
		}

		protected static void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			RenderPipelineManager.BeginCameraRendering(context, camera);
		}

		protected static void EndFrameRendering(ScriptableRenderContext context, Camera[] cameras)
		{
			RenderPipelineManager.EndFrameRendering(context, cameras);
		}

		protected static void EndCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			RenderPipelineManager.EndCameraRendering(context, camera);
		}

		internal void InternalRender(ScriptableRenderContext context, Camera[] cameras)
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				throw new ObjectDisposedException(string.Format("{0} has been disposed. Do not call Render on disposed a RenderPipeline.", this));
			}
			this.Render(context, cameras);
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
	}
}
