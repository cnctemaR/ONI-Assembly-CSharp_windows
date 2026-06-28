using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	internal static class RenderPipelineManager
	{
		public static IRenderPipeline currentPipeline { get; private set; }

		[RequiredByNativeCode]
		internal static void CleanupRenderPipeline()
		{
			if (RenderPipelineManager.s_CurrentPipelineAsset != null)
			{
				RenderPipelineManager.s_CurrentPipelineAsset.DestroyCreatedInstances();
			}
			RenderPipelineManager.s_CurrentPipelineAsset = null;
			RenderPipelineManager.currentPipeline = null;
		}

		[RequiredByNativeCode]
		private static bool DoRenderLoop_Internal(IRenderPipelineAsset pipe, Camera[] cameras, IntPtr loopPtr)
		{
			bool flag;
			if (!RenderPipelineManager.PrepareRenderPipeline(pipe))
			{
				flag = false;
			}
			else
			{
				ScriptableRenderContext scriptableRenderContext = default(ScriptableRenderContext);
				scriptableRenderContext.Initialize(loopPtr);
				RenderPipelineManager.currentPipeline.Render(scriptableRenderContext, cameras);
				flag = true;
			}
			return flag;
		}

		private static bool PrepareRenderPipeline(IRenderPipelineAsset pipe)
		{
			if (RenderPipelineManager.s_CurrentPipelineAsset != pipe)
			{
				if (RenderPipelineManager.s_CurrentPipelineAsset != null)
				{
					RenderPipelineManager.CleanupRenderPipeline();
				}
				RenderPipelineManager.s_CurrentPipelineAsset = pipe;
			}
			if (RenderPipelineManager.s_CurrentPipelineAsset != null && (RenderPipelineManager.currentPipeline == null || RenderPipelineManager.currentPipeline.disposed))
			{
				RenderPipelineManager.currentPipeline = RenderPipelineManager.s_CurrentPipelineAsset.CreatePipeline();
			}
			return RenderPipelineManager.s_CurrentPipelineAsset != null;
		}

		private static IRenderPipelineAsset s_CurrentPipelineAsset;
	}
}
