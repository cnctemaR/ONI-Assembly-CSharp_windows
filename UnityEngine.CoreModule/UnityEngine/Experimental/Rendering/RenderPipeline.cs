using System;
using System.Diagnostics;

namespace UnityEngine.Experimental.Rendering
{
	public abstract class RenderPipeline : IRenderPipeline, IDisposable
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<Camera[]> beginFrameRendering;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<Camera> beginCameraRendering;

		public virtual void Render(ScriptableRenderContext renderContext, Camera[] cameras)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(string.Format("{0} has been disposed. Do not call Render on disposed RenderLoops.", this));
			}
		}

		public bool disposed { get; private set; }

		public virtual void Dispose()
		{
			this.disposed = true;
		}

		public static void BeginFrameRendering(Camera[] cameras)
		{
			if (RenderPipeline.beginFrameRendering != null)
			{
				RenderPipeline.beginFrameRendering(cameras);
			}
		}

		public static void BeginCameraRendering(Camera camera)
		{
			if (RenderPipeline.beginCameraRendering != null)
			{
				RenderPipeline.beginCameraRendering(camera);
			}
		}
	}
}
