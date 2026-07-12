using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	public static class RenderPipelineManager
	{
		public static RenderPipeline currentPipeline { get; private set; }

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<ScriptableRenderContext, Camera[]> beginFrameRendering;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<ScriptableRenderContext, Camera> beginCameraRendering;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<ScriptableRenderContext, Camera[]> endFrameRendering;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<ScriptableRenderContext, Camera> endCameraRendering;

		internal static void BeginFrameRendering(ScriptableRenderContext context, Camera[] cameras)
		{
			Action<ScriptableRenderContext, Camera[]> action = RenderPipelineManager.beginFrameRendering;
			if (action != null)
			{
				action(context, cameras);
			}
		}

		internal static void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			Action<ScriptableRenderContext, Camera> action = RenderPipelineManager.beginCameraRendering;
			if (action != null)
			{
				action(context, camera);
			}
		}

		internal static void EndFrameRendering(ScriptableRenderContext context, Camera[] cameras)
		{
			Action<ScriptableRenderContext, Camera[]> action = RenderPipelineManager.endFrameRendering;
			if (action != null)
			{
				action(context, cameras);
			}
		}

		internal static void EndCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			Action<ScriptableRenderContext, Camera> action = RenderPipelineManager.endCameraRendering;
			if (action != null)
			{
				action(context, camera);
			}
		}

		[RequiredByNativeCode]
		internal static void CleanupRenderPipeline()
		{
			bool flag = RenderPipelineManager.currentPipeline != null && !RenderPipelineManager.currentPipeline.disposed;
			if (flag)
			{
				RenderPipelineManager.currentPipeline.Dispose();
				RenderPipelineManager.s_CurrentPipelineAsset = null;
				RenderPipelineManager.currentPipeline = null;
				SupportedRenderingFeatures.active = new SupportedRenderingFeatures();
			}
		}

		private static void GetCameras(ScriptableRenderContext context)
		{
			int numberOfCameras = context.GetNumberOfCameras();
			bool flag = numberOfCameras != RenderPipelineManager.s_CameraCapacity;
			if (flag)
			{
				Array.Resize<Camera>(ref RenderPipelineManager.s_Cameras, numberOfCameras);
				RenderPipelineManager.s_CameraCapacity = numberOfCameras;
			}
			for (int i = 0; i < numberOfCameras; i++)
			{
				RenderPipelineManager.s_Cameras[i] = context.GetCamera(i);
			}
		}

		[RequiredByNativeCode]
		private static void DoRenderLoop_Internal(RenderPipelineAsset pipe, IntPtr loopPtr, List<Camera.RenderRequest> renderRequests)
		{
			RenderPipelineManager.PrepareRenderPipeline(pipe);
			bool flag = RenderPipelineManager.currentPipeline == null;
			if (!flag)
			{
				ScriptableRenderContext scriptableRenderContext = new ScriptableRenderContext(loopPtr);
				Array.Clear(RenderPipelineManager.s_Cameras, 0, RenderPipelineManager.s_Cameras.Length);
				RenderPipelineManager.GetCameras(scriptableRenderContext);
				bool flag2 = renderRequests == null;
				if (flag2)
				{
					RenderPipelineManager.currentPipeline.InternalRender(scriptableRenderContext, RenderPipelineManager.s_Cameras);
				}
				else
				{
					RenderPipelineManager.currentPipeline.InternalRenderWithRequests(scriptableRenderContext, RenderPipelineManager.s_Cameras, renderRequests);
				}
				Array.Clear(RenderPipelineManager.s_Cameras, 0, RenderPipelineManager.s_Cameras.Length);
			}
		}

		internal static void PrepareRenderPipeline(RenderPipelineAsset pipelineAsset)
		{
			bool flag = RenderPipelineManager.s_CurrentPipelineAsset != pipelineAsset;
			if (flag)
			{
				RenderPipelineManager.CleanupRenderPipeline();
				RenderPipelineManager.s_CurrentPipelineAsset = pipelineAsset;
			}
			bool flag2 = RenderPipelineManager.s_CurrentPipelineAsset != null && (RenderPipelineManager.currentPipeline == null || RenderPipelineManager.currentPipeline.disposed);
			if (flag2)
			{
				RenderPipelineManager.currentPipeline = RenderPipelineManager.s_CurrentPipelineAsset.InternalCreatePipeline();
			}
		}

		internal static RenderPipelineAsset s_CurrentPipelineAsset;

		private static Camera[] s_Cameras = new Camera[0];

		private static int s_CameraCapacity = 0;
	}
}
