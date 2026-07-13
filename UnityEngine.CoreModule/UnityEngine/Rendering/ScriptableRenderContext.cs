using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Rendering.RendererUtils;

namespace UnityEngine.Rendering
{
	[NativeHeader("Modules/UI/CanvasManager.h")]
	[NativeHeader("Runtime/Graphics/ScriptableRenderLoop/ScriptableDrawRenderersUtility.h")]
	[NativeHeader("Runtime/Export/RenderPipeline/ScriptableRenderContext.bindings.h")]
	[NativeHeader("Runtime/Export/RenderPipeline/ScriptableRenderPipeline.bindings.h")]
	[NativeHeader("Modules/UI/Canvas.h")]
	[NativeType("Runtime/Graphics/ScriptableRenderLoop/ScriptableRenderContext.h")]
	public struct ScriptableRenderContext : IEquatable<ScriptableRenderContext>
	{
		[FreeFunction("ScriptableRenderContext::BeginRenderPass")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginRenderPass_Internal(IntPtr self, int width, int height, int volumeDepth, int samples, IntPtr colors, int colorCount, int depthAttachmentIndex, int shadingRateImageAttachmentIndex);

		[FreeFunction("ScriptableRenderContext::BeginSubPass")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginSubPass_Internal(IntPtr self, IntPtr colors, int colorCount, IntPtr inputs, int inputCount, bool isDepthReadOnly, bool isStencilReadOnly);

		[FreeFunction("ScriptableRenderContext::EndSubPass")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EndSubPass_Internal(IntPtr self);

		[FreeFunction("ScriptableRenderContext::EndRenderPass")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EndRenderPass_Internal(IntPtr self);

		[FreeFunction("ScriptableRenderContext::HasInvokeOnRenderObjectCallbacks")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasInvokeOnRenderObjectCallbacks_Internal();

		[FreeFunction("ScriptableRenderPipeline_Bindings::Internal_Cull")]
		private static void Internal_Cull(ref ScriptableCullingParameters parameters, ScriptableRenderContext renderLoop, IntPtr results)
		{
			ScriptableRenderContext.Internal_Cull_Injected(ref parameters, ref renderLoop, results);
		}

		[FreeFunction("ScriptableRenderPipeline_Bindings::Internal_CullShadowCasters")]
		private static void Internal_CullShadowCasters(ScriptableRenderContext renderLoop, IntPtr context)
		{
			ScriptableRenderContext.Internal_CullShadowCasters_Injected(ref renderLoop, context);
		}

		[FreeFunction("InitializeSortSettings")]
		internal static void InitializeSortSettings(Camera camera, out SortingSettings sortingSettings)
		{
			ScriptableRenderContext.InitializeSortSettings_Injected(Object.MarshalledUnityObject.Marshal<Camera>(camera), out sortingSettings);
		}

		[FreeFunction("ScriptableRenderContext::PushDisableApiRenderers")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PushDisableApiRenderers();

		[FreeFunction("ScriptableRenderContext::PopDisableApiRenderers")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PopDisableApiRenderers();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Submit_Internal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool SubmitForRenderPassValidation_Internal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetCameras_Internal(Type listType, object resultList);

		private void DrawRenderers_Internal(IntPtr cullResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, ShaderTagId tagName, bool isPassTagName, IntPtr tagValues, IntPtr stateBlocks, int stateCount)
		{
			ScriptableRenderContext.DrawRenderers_Internal_Injected(ref this, cullResults, ref drawingSettings, ref filteringSettings, ref tagName, isPassTagName, tagValues, stateBlocks, stateCount);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DrawShadows_Internal(IntPtr shadowDrawingSettings);

		[FreeFunction("PlayerEmitCanvasGeometryForCamera")]
		public static void EmitGeometryForCamera(Camera camera)
		{
			ScriptableRenderContext.EmitGeometryForCamera_Injected(Object.MarshalledUnityObject.Marshal<Camera>(camera));
		}

		[NativeThrows]
		private void ExecuteCommandBuffer_Internal(CommandBuffer commandBuffer)
		{
			ScriptableRenderContext.ExecuteCommandBuffer_Internal_Injected(ref this, (commandBuffer == null) ? ((IntPtr)0) : CommandBuffer.BindingsMarshaller.ConvertToNative(commandBuffer));
		}

		[NativeThrows]
		private void ExecuteCommandBufferAsync_Internal(CommandBuffer commandBuffer, ComputeQueueType queueType)
		{
			ScriptableRenderContext.ExecuteCommandBufferAsync_Internal_Injected(ref this, (commandBuffer == null) ? ((IntPtr)0) : CommandBuffer.BindingsMarshaller.ConvertToNative(commandBuffer), queueType);
		}

		private void SetupCameraProperties_Internal([NotNull] Camera camera, bool stereoSetup, int eye)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			ScriptableRenderContext.SetupCameraProperties_Internal_Injected(ref this, intPtr, stereoSetup, eye);
		}

		private void StereoEndRender_Internal([NotNull] Camera camera, int eye, bool isFinalPass)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			ScriptableRenderContext.StereoEndRender_Internal_Injected(ref this, intPtr, eye, isFinalPass);
		}

		private void StartMultiEye_Internal([NotNull] Camera camera, int eye)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			ScriptableRenderContext.StartMultiEye_Internal_Injected(ref this, intPtr, eye);
		}

		private void StopMultiEye_Internal([NotNull] Camera camera)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			ScriptableRenderContext.StopMultiEye_Internal_Injected(ref this, intPtr);
		}

		private void DrawSkybox_Internal([NotNull] Camera camera)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			ScriptableRenderContext.DrawSkybox_Internal_Injected(ref this, intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InvokeOnRenderObjectCallback_Internal();

		private void DrawGizmos_Internal([NotNull] Camera camera, GizmoSubset gizmoSubset)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			ScriptableRenderContext.DrawGizmos_Internal_Injected(ref this, intPtr, gizmoSubset);
		}

		private void DrawWireOverlay_Impl([NotNull] Camera camera)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			ScriptableRenderContext.DrawWireOverlay_Impl_Injected(ref this, intPtr);
		}

		private void DrawUIOverlay_Internal([NotNull] Camera camera)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			ScriptableRenderContext.DrawUIOverlay_Internal_Injected(ref this, intPtr);
		}

		internal IntPtr Internal_GetPtr()
		{
			return this.m_Ptr;
		}

		private RendererList CreateRendererList_Internal(IntPtr cullResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, ShaderTagId tagName, bool isPassTagName, IntPtr tagValues, IntPtr stateBlocks, int stateCount)
		{
			RendererList rendererList;
			ScriptableRenderContext.CreateRendererList_Internal_Injected(ref this, cullResults, ref drawingSettings, ref filteringSettings, ref tagName, isPassTagName, tagValues, stateBlocks, stateCount, out rendererList);
			return rendererList;
		}

		private RendererList CreateShadowRendererList_Internal(IntPtr shadowDrawinSettings)
		{
			RendererList rendererList;
			ScriptableRenderContext.CreateShadowRendererList_Internal_Injected(ref this, shadowDrawinSettings, out rendererList);
			return rendererList;
		}

		private RendererList CreateSkyboxRendererList_Internal([NotNull] Camera camera, int mode, Matrix4x4 proj, Matrix4x4 view, Matrix4x4 projR, Matrix4x4 viewR)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			RendererList rendererList;
			ScriptableRenderContext.CreateSkyboxRendererList_Internal_Injected(ref this, intPtr, mode, ref proj, ref view, ref projR, ref viewR, out rendererList);
			return rendererList;
		}

		private RendererList CreateGizmoRendererList_Internal([NotNull] Camera camera, GizmoSubset gizmoSubset)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			RendererList rendererList;
			ScriptableRenderContext.CreateGizmoRendererList_Internal_Injected(ref this, intPtr, gizmoSubset, out rendererList);
			return rendererList;
		}

		private RendererList CreateUIOverlayRendererList_Internal([NotNull] Camera camera, UISubset uiSubset)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			RendererList rendererList;
			ScriptableRenderContext.CreateUIOverlayRendererList_Internal_Injected(ref this, intPtr, uiSubset, out rendererList);
			return rendererList;
		}

		private RendererList CreateWireOverlayRendererList_Internal([NotNull] Camera camera)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			RendererList rendererList;
			ScriptableRenderContext.CreateWireOverlayRendererList_Internal_Injected(ref this, intPtr, out rendererList);
			return rendererList;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void PrepareRendererListsAsync_Internal(object rendererLists);

		private RendererListStatus QueryRendererListStatus_Internal(RendererList handle)
		{
			return ScriptableRenderContext.QueryRendererListStatus_Internal_Injected(ref this, ref handle);
		}

		internal ScriptableRenderContext(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		public void BeginRenderPass(int width, int height, int volumeDepth, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex, int shadingRateImageAttachmentIndex)
		{
			ScriptableRenderContext.BeginRenderPass_Internal(this.m_Ptr, width, height, volumeDepth, samples, (IntPtr)attachments.GetUnsafeReadOnlyPtr<AttachmentDescriptor>(), attachments.Length, depthAttachmentIndex, shadingRateImageAttachmentIndex);
		}

		public void BeginRenderPass(int width, int height, int volumeDepth, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex = -1)
		{
			ScriptableRenderContext.BeginRenderPass_Internal(this.m_Ptr, width, height, volumeDepth, samples, (IntPtr)attachments.GetUnsafeReadOnlyPtr<AttachmentDescriptor>(), attachments.Length, depthAttachmentIndex, -1);
		}

		public void BeginRenderPass(int width, int height, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex, int shadingRateImageAttachmentIndex)
		{
			ScriptableRenderContext.BeginRenderPass_Internal(this.m_Ptr, width, height, 1, samples, (IntPtr)attachments.GetUnsafeReadOnlyPtr<AttachmentDescriptor>(), attachments.Length, depthAttachmentIndex, shadingRateImageAttachmentIndex);
		}

		public void BeginRenderPass(int width, int height, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex = -1)
		{
			ScriptableRenderContext.BeginRenderPass_Internal(this.m_Ptr, width, height, 1, samples, (IntPtr)attachments.GetUnsafeReadOnlyPtr<AttachmentDescriptor>(), attachments.Length, depthAttachmentIndex, -1);
		}

		public ScopedRenderPass BeginScopedRenderPass(int width, int height, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex, int shadingRateImageAttachmentIndex)
		{
			this.BeginRenderPass(width, height, samples, attachments, depthAttachmentIndex, shadingRateImageAttachmentIndex);
			return new ScopedRenderPass(this);
		}

		public ScopedRenderPass BeginScopedRenderPass(int width, int height, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex = -1)
		{
			this.BeginRenderPass(width, height, samples, attachments, depthAttachmentIndex, -1);
			return new ScopedRenderPass(this);
		}

		public void BeginSubPass(NativeArray<int> colors, NativeArray<int> inputs, bool isDepthReadOnly, bool isStencilReadOnly)
		{
			ScriptableRenderContext.BeginSubPass_Internal(this.m_Ptr, (IntPtr)colors.GetUnsafeReadOnlyPtr<int>(), colors.Length, (IntPtr)inputs.GetUnsafeReadOnlyPtr<int>(), inputs.Length, isDepthReadOnly, isStencilReadOnly);
		}

		public void BeginSubPass(NativeArray<int> colors, NativeArray<int> inputs, bool isDepthStencilReadOnly = false)
		{
			ScriptableRenderContext.BeginSubPass_Internal(this.m_Ptr, (IntPtr)colors.GetUnsafeReadOnlyPtr<int>(), colors.Length, (IntPtr)inputs.GetUnsafeReadOnlyPtr<int>(), inputs.Length, isDepthStencilReadOnly, isDepthStencilReadOnly);
		}

		public void BeginSubPass(NativeArray<int> colors, bool isDepthReadOnly, bool isStencilReadOnly)
		{
			ScriptableRenderContext.BeginSubPass_Internal(this.m_Ptr, (IntPtr)colors.GetUnsafeReadOnlyPtr<int>(), colors.Length, IntPtr.Zero, 0, isDepthReadOnly, isStencilReadOnly);
		}

		public void BeginSubPass(NativeArray<int> colors, bool isDepthStencilReadOnly = false)
		{
			ScriptableRenderContext.BeginSubPass_Internal(this.m_Ptr, (IntPtr)colors.GetUnsafeReadOnlyPtr<int>(), colors.Length, IntPtr.Zero, 0, isDepthStencilReadOnly, isDepthStencilReadOnly);
		}

		public ScopedSubPass BeginScopedSubPass(NativeArray<int> colors, NativeArray<int> inputs, bool isDepthReadOnly, bool isStencilReadOnly)
		{
			this.BeginSubPass(colors, inputs, isDepthReadOnly, isStencilReadOnly);
			return new ScopedSubPass(this);
		}

		public ScopedSubPass BeginScopedSubPass(NativeArray<int> colors, NativeArray<int> inputs, bool isDepthStencilReadOnly = false)
		{
			this.BeginSubPass(colors, inputs, isDepthStencilReadOnly);
			return new ScopedSubPass(this);
		}

		public ScopedSubPass BeginScopedSubPass(NativeArray<int> colors, bool isDepthReadOnly, bool isStencilReadOnly)
		{
			this.BeginSubPass(colors, isDepthReadOnly, isStencilReadOnly);
			return new ScopedSubPass(this);
		}

		public ScopedSubPass BeginScopedSubPass(NativeArray<int> colors, bool isDepthStencilReadOnly = false)
		{
			this.BeginSubPass(colors, isDepthStencilReadOnly);
			return new ScopedSubPass(this);
		}

		public void EndSubPass()
		{
			ScriptableRenderContext.EndSubPass_Internal(this.m_Ptr);
		}

		public void EndRenderPass()
		{
			ScriptableRenderContext.EndRenderPass_Internal(this.m_Ptr);
		}

		public void Submit()
		{
			this.Submit_Internal();
		}

		public bool SubmitForRenderPassValidation()
		{
			return this.SubmitForRenderPassValidation_Internal();
		}

		public bool HasInvokeOnRenderObjectCallbacks()
		{
			return ScriptableRenderContext.HasInvokeOnRenderObjectCallbacks_Internal();
		}

		internal void GetCameras(List<Camera> results)
		{
			this.GetCameras_Internal(typeof(Camera), results);
		}

		[Obsolete("DrawRenderers is obsolete and replaced with the RendererList API: construct a RendererList using ScriptableRenderContext.CreateRendererList and execture it using CommandBuffer.DrawRendererList.", false)]
		public void DrawRenderers(CullingResults cullingResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings)
		{
			this.DrawRenderers_Internal(cullingResults.ptr, ref drawingSettings, ref filteringSettings, ShaderTagId.none, false, IntPtr.Zero, IntPtr.Zero, 0);
		}

		[Obsolete("DrawRenderers is obsolete and replaced with the RendererList API: construct a RendererList using ScriptableRenderContext.CreateRendererList and execture it using CommandBuffer.DrawRendererList.", false)]
		public unsafe void DrawRenderers(CullingResults cullingResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, ref RenderStateBlock stateBlock)
		{
			ShaderTagId shaderTagId = default(ShaderTagId);
			fixed (RenderStateBlock* ptr = &stateBlock)
			{
				RenderStateBlock* ptr2 = ptr;
				this.DrawRenderers_Internal(cullingResults.ptr, ref drawingSettings, ref filteringSettings, ShaderTagId.none, false, (IntPtr)((void*)(&shaderTagId)), (IntPtr)((void*)ptr2), 1);
			}
		}

		[Obsolete("DrawRenderers is obsolete and replaced with the RendererList API: construct a RendererList using ScriptableRenderContext.CreateRendererList and execture it using CommandBuffer.DrawRendererList.", false)]
		public void DrawRenderers(CullingResults cullingResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, NativeArray<ShaderTagId> renderTypes, NativeArray<RenderStateBlock> stateBlocks)
		{
			bool flag = renderTypes.Length != stateBlocks.Length;
			if (flag)
			{
				throw new ArgumentException(string.Format("Arrays {0} and {1} should have same length, but {2} had length {3} while {4} had length {5}.", new object[] { "renderTypes", "stateBlocks", "renderTypes", renderTypes.Length, "stateBlocks", stateBlocks.Length }));
			}
			this.DrawRenderers_Internal(cullingResults.ptr, ref drawingSettings, ref filteringSettings, ScriptableRenderContext.kRenderTypeTag, false, (IntPtr)renderTypes.GetUnsafeReadOnlyPtr<ShaderTagId>(), (IntPtr)stateBlocks.GetUnsafeReadOnlyPtr<RenderStateBlock>(), renderTypes.Length);
		}

		[Obsolete("DrawRenderers is obsolete and replaced with the RendererList API: construct a RendererList using ScriptableRenderContext.CreateRendererList and execture it using CommandBuffer.DrawRendererList.", false)]
		public void DrawRenderers(CullingResults cullingResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, ShaderTagId tagName, bool isPassTagName, NativeArray<ShaderTagId> tagValues, NativeArray<RenderStateBlock> stateBlocks)
		{
			bool flag = tagValues.Length != stateBlocks.Length;
			if (flag)
			{
				throw new ArgumentException(string.Format("Arrays {0} and {1} should have same length, but {2} had length {3} while {4} had length {5}.", new object[] { "tagValues", "stateBlocks", "tagValues", tagValues.Length, "stateBlocks", stateBlocks.Length }));
			}
			this.DrawRenderers_Internal(cullingResults.ptr, ref drawingSettings, ref filteringSettings, tagName, isPassTagName, (IntPtr)tagValues.GetUnsafeReadOnlyPtr<ShaderTagId>(), (IntPtr)stateBlocks.GetUnsafeReadOnlyPtr<RenderStateBlock>(), tagValues.Length);
		}

		[Obsolete("DrawShadows is obsolete and replaced with the RendererList API: construct a RendererList using ScriptableRenderContext.CreateShadowRendererList and execture it using CommandBuffer.DrawRendererList.", false)]
		public unsafe void DrawShadows(ref ShadowDrawingSettings settings)
		{
			fixed (ShadowDrawingSettings* ptr = &settings)
			{
				ShadowDrawingSettings* ptr2 = ptr;
				this.DrawShadows_Internal((IntPtr)((void*)ptr2));
			}
		}

		public void ExecuteCommandBuffer(CommandBuffer commandBuffer)
		{
			bool flag = commandBuffer == null;
			if (flag)
			{
				throw new ArgumentNullException("commandBuffer");
			}
			bool flag2 = commandBuffer.m_Ptr == IntPtr.Zero;
			if (flag2)
			{
				throw new ObjectDisposedException("commandBuffer");
			}
			this.ExecuteCommandBuffer_Internal(commandBuffer);
		}

		public void ExecuteCommandBufferAsync(CommandBuffer commandBuffer, ComputeQueueType queueType)
		{
			bool flag = commandBuffer == null;
			if (flag)
			{
				throw new ArgumentNullException("commandBuffer");
			}
			bool flag2 = commandBuffer.m_Ptr == IntPtr.Zero;
			if (flag2)
			{
				throw new ObjectDisposedException("commandBuffer");
			}
			this.ExecuteCommandBufferAsync_Internal(commandBuffer, queueType);
		}

		public void SetupCameraProperties(Camera camera, bool stereoSetup = false)
		{
			this.SetupCameraProperties(camera, stereoSetup, 0);
		}

		public void SetupCameraProperties(Camera camera, bool stereoSetup, int eye)
		{
			this.SetupCameraProperties_Internal(camera, stereoSetup, eye);
		}

		public void StereoEndRender(Camera camera)
		{
			this.StereoEndRender(camera, 0, true);
		}

		public void StereoEndRender(Camera camera, int eye)
		{
			this.StereoEndRender(camera, eye, true);
		}

		public void StereoEndRender(Camera camera, int eye, bool isFinalPass)
		{
			this.StereoEndRender_Internal(camera, eye, isFinalPass);
		}

		public void StartMultiEye(Camera camera)
		{
			this.StartMultiEye(camera, 0);
		}

		public void StartMultiEye(Camera camera, int eye)
		{
			this.StartMultiEye_Internal(camera, eye);
		}

		public void StopMultiEye(Camera camera)
		{
			this.StopMultiEye_Internal(camera);
		}

		[Obsolete("DrawSkybox is obsolete and replaced with the RendererList API: construct a RendererList using ScriptableRenderContext.CreateSkyboxRendererList and execture it using CommandBuffer.DrawRendererList.", false)]
		public void DrawSkybox(Camera camera)
		{
			this.DrawSkybox_Internal(camera);
		}

		public void InvokeOnRenderObjectCallback()
		{
			this.InvokeOnRenderObjectCallback_Internal();
		}

		public void DrawGizmos(Camera camera, GizmoSubset gizmoSubset)
		{
			this.DrawGizmos_Internal(camera, gizmoSubset);
		}

		public void DrawWireOverlay(Camera camera)
		{
			this.DrawWireOverlay_Impl(camera);
		}

		public void DrawUIOverlay(Camera camera)
		{
			this.DrawUIOverlay_Internal(camera);
		}

		public unsafe CullingResults Cull(ref ScriptableCullingParameters parameters)
		{
			CullingResults cullingResults = default(CullingResults);
			ScriptableRenderContext.Internal_Cull(ref parameters, this, (IntPtr)((void*)(&cullingResults)));
			return cullingResults;
		}

		private unsafe void ValidateCullShadowCastersParameters(in CullingResults cullingResults, in ShadowCastersCullingInfos cullingInfos)
		{
			bool flag = false;
			if (flag)
			{
				throw new UnityException("CullingResults is null");
			}
			NativeArray<LightShadowCasterCullingInfo> nativeArray = cullingInfos.perLightInfos;
			bool flag2 = nativeArray.Length == 0;
			if (!flag2)
			{
				CullingResults cullingResults2 = cullingResults;
				int length = cullingResults2.visibleLights.Length;
				nativeArray = cullingInfos.perLightInfos;
				bool flag3 = length != nativeArray.Length;
				if (flag3)
				{
					string text = "CullingResults.visibleLights.Length ({0}) != ShadowCastersCullingInfos.perLightInfos.Length ({1}). ";
					cullingResults2 = cullingResults;
					object obj = cullingResults2.visibleLights.Length;
					nativeArray = cullingInfos.perLightInfos;
					throw new UnityException(string.Format(text, obj, nativeArray.Length) + "ShadowCastersCullingInfos.perLightInfos must have one entry per visible light.");
				}
				LightShadowCasterCullingInfo* unsafeReadOnlyPtr = (LightShadowCasterCullingInfo*)cullingInfos.perLightInfos.GetUnsafeReadOnlyPtr<LightShadowCasterCullingInfo>();
				int num = 0;
				ref LightShadowCasterCullingInfo ptr;
				RangeInt splitRange;
				NativeArray<ShadowSplitData> nativeArray2;
				for (;;)
				{
					int num2 = num;
					nativeArray = cullingInfos.perLightInfos;
					if (num2 >= nativeArray.Length)
					{
						return;
					}
					ptr = ref unsafeReadOnlyPtr[num];
					splitRange = ptr.splitRange;
					int start = splitRange.start;
					int length2 = splitRange.length;
					int num3 = start + length2;
					bool flag4 = start == 0 && length2 == 0;
					if (!flag4)
					{
						bool flag5;
						if (start >= 0)
						{
							int num4 = start;
							nativeArray2 = cullingInfos.splitBuffer;
							flag5 = num4 <= nativeArray2.Length;
						}
						else
						{
							flag5 = false;
						}
						bool flag6 = flag5;
						bool flag7 = length2 >= 0 && length2 <= 6;
						bool flag8;
						if (num3 >= start)
						{
							int num5 = num3;
							nativeArray2 = cullingInfos.splitBuffer;
							flag8 = num5 <= nativeArray2.Length;
						}
						else
						{
							flag8 = false;
						}
						bool flag9 = flag8;
						bool flag10 = flag6 && flag7 && flag9;
						bool flag11 = !flag10;
						if (flag11)
						{
							break;
						}
						bool flag12 = length2 > 0 && ptr.projectionType == BatchCullingProjectionType.Unknown;
						if (flag12)
						{
							goto Block_11;
						}
						bool flag13 = ptr.splitExclusionMask >> length2 != 0;
						if (flag13)
						{
							goto Block_12;
						}
					}
					num++;
				}
				string text2 = string.Format("ShadowCastersCullingInfos.perLightInfos[{0}] is referring to an invalid memory location. ", num);
				string text3 = string.Format("splitRange.start ({0}) splitRange.length ({1}) ", splitRange.start, splitRange.length);
				string text4 = "ShadowCastersCullingInfos.splitBuffer.Length ({0}).";
				nativeArray2 = cullingInfos.splitBuffer;
				throw new UnityException(text2 + text3 + string.Format(text4, nativeArray2.Length));
				Block_11:
				throw new UnityException(string.Format("ShadowCastersCullingInfos.perLightInfos[{0}].projectionType == {1}. ", num, ptr.projectionType) + string.Format("The range however appears to be valid. splitRange.start ({0}) splitRange.length ({1}).", splitRange.start, splitRange.length));
				Block_12:
				string text5 = Convert.ToString((int)ptr.splitExclusionMask, 2);
				throw new UnityException(string.Format("ShadowCastersCullingInfos.perLightInfos[{0}].splitExclusionMask == 0b{1}. ", num, text5) + string.Format("The highest bit set must be less than the split count. splitRange.start ({0}) splitRange.length ({1}).", splitRange.start, splitRange.length));
			}
		}

		public unsafe void CullShadowCasters(CullingResults cullingResults, ShadowCastersCullingInfos infos)
		{
			ScriptableRenderContext.CullShadowCastersContext cullShadowCastersContext = default(ScriptableRenderContext.CullShadowCastersContext);
			cullShadowCastersContext.cullResults = cullingResults.ptr;
			cullShadowCastersContext.splitBuffer = (ShadowSplitData*)infos.splitBuffer.GetUnsafePtr<ShadowSplitData>();
			cullShadowCastersContext.splitBufferLength = infos.splitBuffer.Length;
			cullShadowCastersContext.perLightInfos = (LightShadowCasterCullingInfo*)infos.perLightInfos.GetUnsafePtr<LightShadowCasterCullingInfo>();
			cullShadowCastersContext.perLightInfoCount = infos.perLightInfos.Length;
			ScriptableRenderContext.Internal_CullShadowCasters(this, (IntPtr)((void*)(&cullShadowCastersContext)));
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		internal void Validate()
		{
		}

		public bool Equals(ScriptableRenderContext other)
		{
			return this.m_Ptr.Equals(other.m_Ptr);
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			return !flag && obj is ScriptableRenderContext && this.Equals((ScriptableRenderContext)obj);
		}

		public override int GetHashCode()
		{
			return this.m_Ptr.GetHashCode();
		}

		public static bool operator ==(ScriptableRenderContext left, ScriptableRenderContext right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ScriptableRenderContext left, ScriptableRenderContext right)
		{
			return !left.Equals(right);
		}

		public RendererList CreateRendererList(RendererListDesc desc)
		{
			RendererListParams rendererListParams = RendererListDesc.ConvertToParameters(in desc);
			RendererList rendererList = this.CreateRendererList(ref rendererListParams);
			rendererListParams.Dispose();
			return rendererList;
		}

		public RendererList CreateRendererList(ref RendererListParams param)
		{
			param.Validate();
			return this.CreateRendererList_Internal(param.cullingResults.ptr, ref param.drawSettings, ref param.filteringSettings, param.tagName, param.isPassTagName, param.tagsValuePtr, param.stateBlocksPtr, param.numStateBlocks);
		}

		public unsafe RendererList CreateShadowRendererList(ref ShadowDrawingSettings settings)
		{
			fixed (ShadowDrawingSettings* ptr = &settings)
			{
				ShadowDrawingSettings* ptr2 = ptr;
				return this.CreateShadowRendererList_Internal((IntPtr)((void*)ptr2));
			}
		}

		public RendererList CreateSkyboxRendererList(Camera camera, Matrix4x4 projectionMatrixL, Matrix4x4 viewMatrixL, Matrix4x4 projectionMatrixR, Matrix4x4 viewMatrixR)
		{
			return this.CreateSkyboxRendererList_Internal(camera, 2, projectionMatrixL, viewMatrixL, projectionMatrixR, viewMatrixR);
		}

		public RendererList CreateSkyboxRendererList(Camera camera, Matrix4x4 projectionMatrix, Matrix4x4 viewMatrix)
		{
			return this.CreateSkyboxRendererList_Internal(camera, 1, projectionMatrix, viewMatrix, Matrix4x4.identity, Matrix4x4.identity);
		}

		public RendererList CreateSkyboxRendererList(Camera camera)
		{
			return this.CreateSkyboxRendererList_Internal(camera, 0, Matrix4x4.identity, Matrix4x4.identity, Matrix4x4.identity, Matrix4x4.identity);
		}

		public RendererList CreateGizmoRendererList(Camera camera, GizmoSubset gizmoSubset)
		{
			return this.CreateGizmoRendererList_Internal(camera, gizmoSubset);
		}

		public RendererList CreateUIOverlayRendererList(Camera camera)
		{
			return this.CreateUIOverlayRendererList_Internal(camera, UISubset.All);
		}

		public RendererList CreateUIOverlayRendererList(Camera camera, UISubset uiSubset)
		{
			return this.CreateUIOverlayRendererList_Internal(camera, uiSubset);
		}

		public RendererList CreateWireOverlayRendererList(Camera camera)
		{
			return this.CreateWireOverlayRendererList_Internal(camera);
		}

		public void PrepareRendererListsAsync(List<RendererList> rendererLists)
		{
			this.PrepareRendererListsAsync_Internal(rendererLists);
		}

		public RendererListStatus QueryRendererListStatus(RendererList rendererList)
		{
			return this.QueryRendererListStatus_Internal(rendererList);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Cull_Injected(ref ScriptableCullingParameters parameters, [In] ref ScriptableRenderContext renderLoop, IntPtr results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CullShadowCasters_Injected([In] ref ScriptableRenderContext renderLoop, IntPtr context);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitializeSortSettings_Injected(IntPtr camera, out SortingSettings sortingSettings);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawRenderers_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr cullResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, [In] ref ShaderTagId tagName, bool isPassTagName, IntPtr tagValues, IntPtr stateBlocks, int stateCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EmitGeometryForCamera_Injected(IntPtr camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExecuteCommandBuffer_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr commandBuffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExecuteCommandBufferAsync_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr commandBuffer, ComputeQueueType queueType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetupCameraProperties_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, bool stereoSetup, int eye);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StereoEndRender_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, int eye, bool isFinalPass);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StartMultiEye_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, int eye);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StopMultiEye_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawSkybox_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawGizmos_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, GizmoSubset gizmoSubset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawWireOverlay_Impl_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawUIOverlay_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateRendererList_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr cullResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, [In] ref ShaderTagId tagName, bool isPassTagName, IntPtr tagValues, IntPtr stateBlocks, int stateCount, out RendererList ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateShadowRendererList_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr shadowDrawinSettings, out RendererList ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateSkyboxRendererList_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, int mode, [In] ref Matrix4x4 proj, [In] ref Matrix4x4 view, [In] ref Matrix4x4 projR, [In] ref Matrix4x4 viewR, out RendererList ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateGizmoRendererList_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, GizmoSubset gizmoSubset, out RendererList ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateUIOverlayRendererList_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, UISubset uiSubset, out RendererList ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateWireOverlayRendererList_Internal_Injected(ref ScriptableRenderContext _unity_self, IntPtr camera, out RendererList ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RendererListStatus QueryRendererListStatus_Internal_Injected(ref ScriptableRenderContext _unity_self, [In] ref RendererList handle);

		private static readonly ShaderTagId kRenderTypeTag = new ShaderTagId("RenderType");

		private IntPtr m_Ptr;

		private const bool deprecateDrawXmethods = false;

		internal enum SkyboxXRMode
		{
			Off,
			Enabled,
			LegacySinglePass
		}

		private struct CullShadowCastersContext
		{
			public IntPtr cullResults;

			public unsafe ShadowSplitData* splitBuffer;

			public int splitBufferLength;

			public unsafe LightShadowCasterCullingInfo* perLightInfos;

			public int perLightInfoCount;
		}
	}
}
