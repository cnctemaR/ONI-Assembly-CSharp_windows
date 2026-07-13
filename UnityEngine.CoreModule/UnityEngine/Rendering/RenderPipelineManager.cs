using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Pool;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	public static class RenderPipelineManager
	{
		internal static RenderPipelineAsset currentPipelineAsset
		{
			get
			{
				return RenderPipelineManager.s_CurrentPipelineAsset;
			}
		}

		public static RenderPipeline currentPipeline
		{
			get
			{
				return RenderPipelineManager.s_CurrentPipeline;
			}
			private set
			{
				RenderPipelineManager.s_CurrentPipeline = value;
				bool flag = RenderPipelineManager.s_PendingRPAssignationToRaise;
				if (flag)
				{
					RenderPipelineManager.s_PendingRPAssignationToRaise = false;
					Action action = RenderPipelineManager.activeRenderPipelineTypeChanged;
					if (action != null)
					{
						action();
					}
				}
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<ScriptableRenderContext, List<Camera>> beginContextRendering;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<ScriptableRenderContext, List<Camera>> endContextRendering;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<ScriptableRenderContext, Camera> beginCameraRendering;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<ScriptableRenderContext, Camera> endCameraRendering;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action activeRenderPipelineTypeChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<RenderPipelineAsset, RenderPipelineAsset> activeRenderPipelineAssetChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action activeRenderPipelineCreated;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action activeRenderPipelineDisposed;

		public static bool pipelineSwitchCompleted
		{
			get
			{
				return RenderPipelineManager.s_CurrentPipelineAsset == GraphicsSettings.currentRenderPipeline && !RenderPipelineManager.IsPipelineRequireCreation();
			}
		}

		internal static void BeginContextRendering(ScriptableRenderContext context, List<Camera> cameras)
		{
			Action<ScriptableRenderContext, List<Camera>> action = RenderPipelineManager.beginContextRendering;
			if (action != null)
			{
				action(context, cameras);
			}
			Action<ScriptableRenderContext, Camera[]> action2 = RenderPipelineManager.beginFrameRendering;
			if (action2 != null)
			{
				action2(context, cameras.ToArray());
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

		internal static void EndContextRendering(ScriptableRenderContext context, List<Camera> cameras)
		{
			Action<ScriptableRenderContext, Camera[]> action = RenderPipelineManager.endFrameRendering;
			if (action != null)
			{
				action(context, cameras.ToArray());
			}
			Action<ScriptableRenderContext, List<Camera>> action2 = RenderPipelineManager.endContextRendering;
			if (action2 != null)
			{
				action2(context, cameras);
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
		private static void OnActiveRenderPipelineAssetChanged(ScriptableObject from, ScriptableObject to, bool raiseTypeChanged)
		{
			RenderPipelineAsset renderPipelineAsset = from as RenderPipelineAsset;
			RenderPipelineAsset renderPipelineAsset2 = to as RenderPipelineAsset;
			Action<RenderPipelineAsset, RenderPipelineAsset> action = RenderPipelineManager.activeRenderPipelineAssetChanged;
			if (action != null)
			{
				action(renderPipelineAsset, renderPipelineAsset2);
			}
			if (raiseTypeChanged)
			{
				Type type = ((renderPipelineAsset2 == null) ? null : renderPipelineAsset2.pipelineType);
				RenderPipeline currentPipeline = RenderPipelineManager.currentPipeline;
				bool flag = ((currentPipeline != null) ? currentPipeline.GetType() : null) != type;
				if (flag)
				{
					RenderPipelineManager.s_PendingRPAssignationToRaise = true;
				}
				else
				{
					Action action2 = RenderPipelineManager.activeRenderPipelineTypeChanged;
					if (action2 != null)
					{
						action2();
					}
				}
			}
		}

		[RequiredByNativeCode]
		internal static void HandleRenderPipelineChange(RenderPipelineAsset pipelineAsset)
		{
			bool flag = RenderPipelineManager.s_CurrentPipelineAsset != pipelineAsset;
			bool flag2 = RenderPipelineManager.s_CleanUpPipeline || flag;
			if (flag2)
			{
				RenderPipelineManager.CleanupRenderPipeline();
				RenderPipelineManager.s_CurrentPipelineAsset = pipelineAsset;
			}
		}

		[RequiredByNativeCode]
		internal static void RecreateCurrentPipeline(RenderPipelineAsset pipelineAsset)
		{
			bool flag = RenderPipelineManager.s_CurrentPipelineAsset == pipelineAsset;
			if (flag)
			{
				RenderPipelineManager.s_CleanUpPipeline = true;
			}
		}

		[RequiredByNativeCode]
		internal static void CleanupRenderPipeline()
		{
			bool flag = !RenderPipelineManager.isCurrentPipelineValid;
			if (!flag)
			{
				bool flag2 = GraphicsSettings.currentRenderPipeline == null;
				if (flag2)
				{
					Shader.globalRenderPipeline = string.Empty;
				}
				Action action = RenderPipelineManager.activeRenderPipelineDisposed;
				if (action != null)
				{
					action();
				}
				RenderPipelineManager.currentPipeline.Dispose();
				RenderPipelineManager.currentPipeline = null;
				RenderPipelineManager.s_CleanUpPipeline = false;
				RenderPipelineManager.s_CurrentPipelineAsset = null;
				SupportedRenderingFeatures.active = null;
			}
		}

		[RequiredByNativeCode]
		private static void DoRenderLoop_Internal(RenderPipelineAsset pipelineAsset, IntPtr loopPtr, Object renderRequest)
		{
			bool flag = !RenderPipelineManager.TryPrepareRenderPipeline(pipelineAsset);
			if (!flag)
			{
				ScriptableRenderContext scriptableRenderContext = new ScriptableRenderContext(loopPtr);
				List<Camera> list;
				using (CollectionPool<List<Camera>, Camera>.Get(out list))
				{
					scriptableRenderContext.GetCameras(list);
					bool flag2 = renderRequest == null;
					if (flag2)
					{
						RenderPipelineManager.currentPipeline.InternalRender(scriptableRenderContext, list);
					}
					else
					{
						RenderPipelineManager.currentPipeline.InternalProcessRenderRequests<Object>(scriptableRenderContext, list[0], renderRequest);
					}
				}
			}
		}

		internal static bool TryPrepareRenderPipeline(RenderPipelineAsset pipelineAsset)
		{
			RenderPipelineManager.HandleRenderPipelineChange(pipelineAsset);
			bool flag = !RenderPipelineManager.IsPipelineRequireCreation();
			bool flag2;
			if (flag)
			{
				flag2 = RenderPipelineManager.currentPipeline != null;
			}
			else
			{
				RenderPipelineManager.currentPipeline = RenderPipelineManager.s_CurrentPipelineAsset.InternalCreatePipeline();
				Shader.globalRenderPipeline = RenderPipelineManager.s_CurrentPipelineAsset.renderPipelineShaderTag;
				Action action = RenderPipelineManager.activeRenderPipelineCreated;
				if (action != null)
				{
					action();
				}
				flag2 = RenderPipelineManager.currentPipeline != null;
			}
			return flag2;
		}

		internal static bool isCurrentPipelineValid
		{
			get
			{
				RenderPipeline currentPipeline = RenderPipelineManager.currentPipeline;
				return currentPipeline != null && !currentPipeline.disposed;
			}
		}

		private static bool IsPipelineRequireCreation()
		{
			return RenderPipelineManager.s_CurrentPipelineAsset != null && (RenderPipelineManager.currentPipeline == null || RenderPipelineManager.currentPipeline.disposed);
		}

		[Obsolete("beginFrameRendering is deprecated. Use beginContextRendering instead. #from 2023.3", false)]
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<ScriptableRenderContext, Camera[]> beginFrameRendering;

		[Obsolete("endFrameRendering is deprecated. Use endContextRendering instead. #from 2023.3", false)]
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<ScriptableRenderContext, Camera[]> endFrameRendering;

		private static bool s_CleanUpPipeline;

		private static RenderPipelineAsset s_CurrentPipelineAsset;

		private static RenderPipeline s_CurrentPipeline;

		private static bool s_PendingRPAssignationToRaise;
	}
}
