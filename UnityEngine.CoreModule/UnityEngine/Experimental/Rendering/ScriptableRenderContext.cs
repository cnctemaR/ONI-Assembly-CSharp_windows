using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
	[NativeHeader("Runtime/UI/CanvasManager.h")]
	[NativeHeader("Runtime/UI/Canvas.h")]
	[NativeType("Runtime/Graphics/ScriptableRenderLoop/ScriptableRenderContext.h")]
	[NativeHeader("Runtime/Export/ScriptableRenderContext.bindings.h")]
	public struct ScriptableRenderContext
	{
		internal ScriptableRenderContext(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		[FreeFunction("ScriptableRenderContext::BeginRenderPass")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BeginRenderPassInternal(IntPtr _self, int w, int h, int samples, RenderPassAttachment[] colors, RenderPassAttachment depth);

		[FreeFunction("ScriptableRenderContext::BeginSubPass")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BeginSubPassInternal(IntPtr _self, RenderPassAttachment[] colors, RenderPassAttachment[] inputs, bool readOnlyDepth);

		[FreeFunction("ScriptableRenderContext::EndRenderPass")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EndRenderPassInternal(IntPtr _self);

		private void Submit_Internal()
		{
			ScriptableRenderContext.Submit_Internal_Injected(ref this);
		}

		private void DrawRenderers_Internal(FilterResults renderers, ref DrawRendererSettings drawSettings, FilterRenderersSettings filterSettings)
		{
			ScriptableRenderContext.DrawRenderers_Internal_Injected(ref this, ref renderers, ref drawSettings, ref filterSettings);
		}

		private void DrawRenderers_StateBlock_Internal(FilterResults renderers, ref DrawRendererSettings drawSettings, FilterRenderersSettings filterSettings, RenderStateBlock stateBlock)
		{
			ScriptableRenderContext.DrawRenderers_StateBlock_Internal_Injected(ref this, ref renderers, ref drawSettings, ref filterSettings, ref stateBlock);
		}

		private void DrawRenderers_StateMap_Internal(FilterResults renderers, ref DrawRendererSettings drawSettings, FilterRenderersSettings filterSettings, Array stateMap, int stateMapLength)
		{
			ScriptableRenderContext.DrawRenderers_StateMap_Internal_Injected(ref this, ref renderers, ref drawSettings, ref filterSettings, stateMap, stateMapLength);
		}

		private void DrawShadows_Internal(ref DrawShadowsSettings settings)
		{
			ScriptableRenderContext.DrawShadows_Internal_Injected(ref this, ref settings);
		}

		private void ExecuteCommandBuffer_Internal(CommandBuffer commandBuffer)
		{
			ScriptableRenderContext.ExecuteCommandBuffer_Internal_Injected(ref this, commandBuffer);
		}

		private void ExecuteCommandBufferAsync_Internal(CommandBuffer commandBuffer, ComputeQueueType queueType)
		{
			ScriptableRenderContext.ExecuteCommandBufferAsync_Internal_Injected(ref this, commandBuffer, queueType);
		}

		private void SetupCameraProperties_Internal(Camera camera, bool stereoSetup)
		{
			ScriptableRenderContext.SetupCameraProperties_Internal_Injected(ref this, camera, stereoSetup);
		}

		private void StereoEndRender_Internal(Camera camera)
		{
			ScriptableRenderContext.StereoEndRender_Internal_Injected(ref this, camera);
		}

		private void StartMultiEye_Internal(Camera camera)
		{
			ScriptableRenderContext.StartMultiEye_Internal_Injected(ref this, camera);
		}

		private void StopMultiEye_Internal(Camera camera)
		{
			ScriptableRenderContext.StopMultiEye_Internal_Injected(ref this, camera);
		}

		private void DrawSkybox_Internal(Camera camera)
		{
			ScriptableRenderContext.DrawSkybox_Internal_Injected(ref this, camera);
		}

		internal IntPtr Internal_GetPtr()
		{
			return this.m_Ptr;
		}

		public void Submit()
		{
			this.CheckValid();
			this.Submit_Internal();
		}

		public void DrawRenderers(FilterResults renderers, ref DrawRendererSettings drawSettings, FilterRenderersSettings filterSettings)
		{
			this.CheckValid();
			this.DrawRenderers_Internal(renderers, ref drawSettings, filterSettings);
		}

		public void DrawRenderers(FilterResults renderers, ref DrawRendererSettings drawSettings, FilterRenderersSettings filterSettings, RenderStateBlock stateBlock)
		{
			this.CheckValid();
			this.DrawRenderers_StateBlock_Internal(renderers, ref drawSettings, filterSettings, stateBlock);
		}

		public void DrawRenderers(FilterResults renderers, ref DrawRendererSettings drawSettings, FilterRenderersSettings filterSettings, List<RenderStateMapping> stateMap)
		{
			this.CheckValid();
			this.DrawRenderers_StateMap_Internal(renderers, ref drawSettings, filterSettings, NoAllocHelpers.ExtractArrayFromList(stateMap), stateMap.Count);
		}

		public void DrawShadows(ref DrawShadowsSettings settings)
		{
			this.CheckValid();
			this.DrawShadows_Internal(ref settings);
		}

		public void ExecuteCommandBuffer(CommandBuffer commandBuffer)
		{
			if (commandBuffer == null)
			{
				throw new ArgumentNullException("commandBuffer");
			}
			this.CheckValid();
			this.ExecuteCommandBuffer_Internal(commandBuffer);
		}

		public void ExecuteCommandBufferAsync(CommandBuffer commandBuffer, ComputeQueueType queueType)
		{
			if (commandBuffer == null)
			{
				throw new ArgumentNullException("commandBuffer");
			}
			this.CheckValid();
			this.ExecuteCommandBufferAsync_Internal(commandBuffer, queueType);
		}

		public void SetupCameraProperties(Camera camera)
		{
			this.CheckValid();
			this.SetupCameraProperties_Internal(camera, false);
		}

		public void SetupCameraProperties(Camera camera, bool stereoSetup)
		{
			this.CheckValid();
			this.SetupCameraProperties_Internal(camera, stereoSetup);
		}

		public void StereoEndRender(Camera camera)
		{
			this.CheckValid();
			this.StereoEndRender_Internal(camera);
		}

		public void StartMultiEye(Camera camera)
		{
			this.CheckValid();
			this.StartMultiEye_Internal(camera);
		}

		public void StopMultiEye(Camera camera)
		{
			this.CheckValid();
			this.StopMultiEye_Internal(camera);
		}

		public void DrawSkybox(Camera camera)
		{
			this.CheckValid();
			this.DrawSkybox_Internal(camera);
		}

		internal void CheckValid()
		{
			if (this.m_Ptr.ToInt64() == 0L)
			{
				throw new ArgumentException("Invalid ScriptableRenderContext.  This can be caused by allocating a context in user code.");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Submit_Internal_Injected(ref ScriptableRenderContext _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawRenderers_Internal_Injected(ref ScriptableRenderContext _unity_self, ref FilterResults renderers, ref DrawRendererSettings drawSettings, ref FilterRenderersSettings filterSettings);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawRenderers_StateBlock_Internal_Injected(ref ScriptableRenderContext _unity_self, ref FilterResults renderers, ref DrawRendererSettings drawSettings, ref FilterRenderersSettings filterSettings, ref RenderStateBlock stateBlock);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawRenderers_StateMap_Internal_Injected(ref ScriptableRenderContext _unity_self, ref FilterResults renderers, ref DrawRendererSettings drawSettings, ref FilterRenderersSettings filterSettings, Array stateMap, int stateMapLength);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawShadows_Internal_Injected(ref ScriptableRenderContext _unity_self, ref DrawShadowsSettings settings);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExecuteCommandBuffer_Internal_Injected(ref ScriptableRenderContext _unity_self, CommandBuffer commandBuffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExecuteCommandBufferAsync_Internal_Injected(ref ScriptableRenderContext _unity_self, CommandBuffer commandBuffer, ComputeQueueType queueType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetupCameraProperties_Internal_Injected(ref ScriptableRenderContext _unity_self, Camera camera, bool stereoSetup);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StereoEndRender_Internal_Injected(ref ScriptableRenderContext _unity_self, Camera camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StartMultiEye_Internal_Injected(ref ScriptableRenderContext _unity_self, Camera camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopMultiEye_Internal_Injected(ref ScriptableRenderContext _unity_self, Camera camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawSkybox_Internal_Injected(ref ScriptableRenderContext _unity_self, Camera camera);

		private IntPtr m_Ptr;
	}
}
