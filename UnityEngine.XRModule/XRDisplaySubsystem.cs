using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[NativeType(Header = "Modules/XR/Subsystems/Display/XRDisplaySubsystem.h")]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[UsedByNativeCode]
	[NativeConditional("ENABLE_XR")]
	public class XRDisplaySubsystem : IntegratedSubsystem<XRDisplaySubsystemDescriptor>
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<bool> displayFocusChanged;

		[RequiredByNativeCode]
		private void InvokeDisplayFocusChanged(bool focus)
		{
			bool flag = this.displayFocusChanged != null;
			if (flag)
			{
				this.displayFocusChanged(focus);
			}
		}

		[Obsolete("singlePassRenderingDisabled{get;set;} is deprecated. Use textureLayout and supportedTextureLayouts instead.", false)]
		public bool singlePassRenderingDisabled
		{
			get
			{
				return (this.textureLayout & XRDisplaySubsystem.TextureLayout.Texture2DArray) == (XRDisplaySubsystem.TextureLayout)0;
			}
			set
			{
				if (value)
				{
					this.textureLayout = XRDisplaySubsystem.TextureLayout.SeparateTexture2Ds;
				}
				else
				{
					bool flag = (this.supportedTextureLayouts & XRDisplaySubsystem.TextureLayout.Texture2DArray) > (XRDisplaySubsystem.TextureLayout)0;
					if (flag)
					{
						this.textureLayout = XRDisplaySubsystem.TextureLayout.Texture2DArray;
					}
				}
			}
		}

		public bool displayOpaque
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_displayOpaque_Injected(intPtr);
			}
		}

		public bool contentProtectionEnabled
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_contentProtectionEnabled_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				XRDisplaySubsystem.set_contentProtectionEnabled_Injected(intPtr, value);
			}
		}

		public float appliedViewportScale
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_appliedViewportScale_Injected(intPtr);
			}
		}

		public float scaleOfAllViewports
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_scaleOfAllViewports_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				XRDisplaySubsystem.set_scaleOfAllViewports_Injected(intPtr, value);
			}
		}

		public float scaleOfAllRenderTargets
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_scaleOfAllRenderTargets_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				XRDisplaySubsystem.set_scaleOfAllRenderTargets_Injected(intPtr, value);
			}
		}

		public float globalDynamicScale
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_globalDynamicScale_Injected(intPtr);
			}
		}

		public float zNear
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_zNear_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				XRDisplaySubsystem.set_zNear_Injected(intPtr, value);
			}
		}

		public float zFar
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_zFar_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				XRDisplaySubsystem.set_zFar_Injected(intPtr, value);
			}
		}

		public bool sRGB
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_sRGB_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				XRDisplaySubsystem.set_sRGB_Injected(intPtr, value);
			}
		}

		public float occlusionMaskScale
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_occlusionMaskScale_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				XRDisplaySubsystem.set_occlusionMaskScale_Injected(intPtr, value);
			}
		}

		public float foveatedRenderingLevel
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_foveatedRenderingLevel_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				XRDisplaySubsystem.set_foveatedRenderingLevel_Injected(intPtr, value);
			}
		}

		public XRDisplaySubsystem.FoveatedRenderingFlags foveatedRenderingFlags
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_foveatedRenderingFlags_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				XRDisplaySubsystem.set_foveatedRenderingFlags_Injected(intPtr, value);
			}
		}

		public void MarkTransformLateLatched(Transform transform, XRDisplaySubsystem.LateLatchNode nodeType)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			XRDisplaySubsystem.MarkTransformLateLatched_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Transform>(transform), nodeType);
		}

		public XRDisplaySubsystem.TextureLayout textureLayout
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_textureLayout_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				XRDisplaySubsystem.set_textureLayout_Injected(intPtr, value);
			}
		}

		public XRDisplaySubsystem.TextureLayout supportedTextureLayouts
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_supportedTextureLayouts_Injected(intPtr);
			}
		}

		public int ScaledTextureWidth(RenderTexture renderTexture)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.ScaledTextureWidth_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RenderTexture>(renderTexture));
		}

		public int ScaledTextureHeight(RenderTexture renderTexture)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.ScaledTextureHeight_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RenderTexture>(renderTexture));
		}

		public XRDisplaySubsystem.ReprojectionMode reprojectionMode
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_reprojectionMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				XRDisplaySubsystem.set_reprojectionMode_Injected(intPtr, value);
			}
		}

		public void SetFocusPlane(Vector3 point, Vector3 normal, Vector3 velocity)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			XRDisplaySubsystem.SetFocusPlane_Injected(intPtr, ref point, ref normal, ref velocity);
		}

		public void SetMSAALevel(int level)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			XRDisplaySubsystem.SetMSAALevel_Injected(intPtr, level);
		}

		public bool disableLegacyRenderer
		{
			get
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return XRDisplaySubsystem.get_disableLegacyRenderer_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				XRDisplaySubsystem.set_disableLegacyRenderer_Injected(intPtr, value);
			}
		}

		public int GetRenderPassCount()
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.GetRenderPassCount_Injected(intPtr);
		}

		public void GetRenderPass(int renderPassIndex, out XRDisplaySubsystem.XRRenderPass renderPass)
		{
			bool flag = !this.Internal_TryGetRenderPass(renderPassIndex, out renderPass);
			if (flag)
			{
				throw new IndexOutOfRangeException("renderPassIndex");
			}
		}

		[NativeMethod("TryGetRenderPass")]
		private bool Internal_TryGetRenderPass(int renderPassIndex, out XRDisplaySubsystem.XRRenderPass renderPass)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.Internal_TryGetRenderPass_Injected(intPtr, renderPassIndex, out renderPass);
		}

		public void EndRecordingIfLateLatched(Camera camera)
		{
			bool flag = !this.Internal_TryEndRecordingIfLateLatched(camera);
			if (flag)
			{
				bool flag2 = camera == null;
				if (flag2)
				{
					throw new ArgumentNullException("camera");
				}
			}
		}

		[NativeMethod("TryEndRecordingIfLateLatched")]
		private bool Internal_TryEndRecordingIfLateLatched(Camera camera)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.Internal_TryEndRecordingIfLateLatched_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Camera>(camera));
		}

		public void BeginRecordingIfLateLatched(Camera camera)
		{
			bool flag = !this.Internal_TryBeginRecordingIfLateLatched(camera);
			if (flag)
			{
				bool flag2 = camera == null;
				if (flag2)
				{
					throw new ArgumentNullException("camera");
				}
			}
		}

		[NativeMethod("TryBeginRecordingIfLateLatched")]
		private bool Internal_TryBeginRecordingIfLateLatched(Camera camera)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.Internal_TryBeginRecordingIfLateLatched_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Camera>(camera));
		}

		public void GetCullingParameters(Camera camera, int cullingPassIndex, out ScriptableCullingParameters scriptableCullingParameters)
		{
			bool flag = !this.Internal_TryGetCullingParams(camera, cullingPassIndex, out scriptableCullingParameters);
			if (!flag)
			{
				return;
			}
			bool flag2 = camera == null;
			if (flag2)
			{
				throw new ArgumentNullException("camera");
			}
			throw new IndexOutOfRangeException("cullingPassIndex");
		}

		[NativeMethod("TryGetCullingParams")]
		[NativeHeader("Runtime/Graphics/ScriptableRenderLoop/ScriptableCulling.h")]
		private bool Internal_TryGetCullingParams(Camera camera, int cullingPassIndex, out ScriptableCullingParameters scriptableCullingParameters)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.Internal_TryGetCullingParams_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Camera>(camera), cullingPassIndex, out scriptableCullingParameters);
		}

		[NativeMethod("TryGetAppGPUTimeLastFrame")]
		public bool TryGetAppGPUTimeLastFrame(out float gpuTimeLastFrame)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.TryGetAppGPUTimeLastFrame_Injected(intPtr, out gpuTimeLastFrame);
		}

		[NativeMethod("TryGetCompositorGPUTimeLastFrame")]
		public bool TryGetCompositorGPUTimeLastFrame(out float gpuTimeLastFrameCompositor)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.TryGetCompositorGPUTimeLastFrame_Injected(intPtr, out gpuTimeLastFrameCompositor);
		}

		[NativeMethod("TryGetDroppedFrameCount")]
		public bool TryGetDroppedFrameCount(out int droppedFrameCount)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.TryGetDroppedFrameCount_Injected(intPtr, out droppedFrameCount);
		}

		[NativeMethod("TryGetFramePresentCount")]
		public bool TryGetFramePresentCount(out int framePresentCount)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.TryGetFramePresentCount_Injected(intPtr, out framePresentCount);
		}

		[NativeMethod("TryGetDisplayRefreshRate")]
		public bool TryGetDisplayRefreshRate(out float displayRefreshRate)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.TryGetDisplayRefreshRate_Injected(intPtr, out displayRefreshRate);
		}

		[NativeMethod("TryGetMotionToPhoton")]
		public bool TryGetMotionToPhoton(out float motionToPhoton)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.TryGetMotionToPhoton_Injected(intPtr, out motionToPhoton);
		}

		[NativeConditional("ENABLE_XR")]
		[NativeMethod(Name = "UnityXRRenderTextureIdToRenderTexture", IsThreadSafe = false)]
		public RenderTexture GetRenderTexture(uint unityXrRenderTextureId)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<RenderTexture>(XRDisplaySubsystem.GetRenderTexture_Injected(intPtr, unityXrRenderTextureId));
		}

		[NativeConditional("ENABLE_XR")]
		[NativeMethod(Name = "GetTextureForRenderPass", IsThreadSafe = false)]
		public RenderTexture GetRenderTextureForRenderPass(int renderPass)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<RenderTexture>(XRDisplaySubsystem.GetRenderTextureForRenderPass_Injected(intPtr, renderPass));
		}

		[NativeMethod(Name = "GetSharedDepthTextureForRenderPass", IsThreadSafe = false)]
		[NativeConditional("ENABLE_XR")]
		public RenderTexture GetSharedDepthTextureForRenderPass(int renderPass)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<RenderTexture>(XRDisplaySubsystem.GetSharedDepthTextureForRenderPass_Injected(intPtr, renderPass));
		}

		[NativeMethod(Name = "GetPreferredMirrorViewBlitMode", IsThreadSafe = false)]
		[NativeConditional("ENABLE_XR")]
		public int GetPreferredMirrorBlitMode()
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.GetPreferredMirrorBlitMode_Injected(intPtr);
		}

		[NativeConditional("ENABLE_XR")]
		[NativeMethod(Name = "SetPreferredMirrorViewBlitMode", IsThreadSafe = false)]
		public void SetPreferredMirrorBlitMode(int blitMode)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			XRDisplaySubsystem.SetPreferredMirrorBlitMode_Injected(intPtr, blitMode);
		}

		[Obsolete("GetMirrorViewBlitDesc(RenderTexture, out XRMirrorViewBlitDesc) is deprecated. Use GetMirrorViewBlitDesc(RenderTexture, out XRMirrorViewBlitDesc, int) instead.", false)]
		public bool GetMirrorViewBlitDesc(RenderTexture mirrorRt, out XRDisplaySubsystem.XRMirrorViewBlitDesc outDesc)
		{
			return this.GetMirrorViewBlitDesc(mirrorRt, out outDesc, -1);
		}

		[NativeConditional("ENABLE_XR")]
		[NativeMethod(Name = "QueryMirrorViewBlitDesc", IsThreadSafe = false)]
		public bool GetMirrorViewBlitDesc(RenderTexture mirrorRt, out XRDisplaySubsystem.XRMirrorViewBlitDesc outDesc, int mode)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.GetMirrorViewBlitDesc_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RenderTexture>(mirrorRt), out outDesc, mode);
		}

		[Obsolete("AddGraphicsThreadMirrorViewBlit(CommandBuffer, bool) is deprecated. Use AddGraphicsThreadMirrorViewBlit(CommandBuffer, bool, int) instead.", false)]
		public bool AddGraphicsThreadMirrorViewBlit(CommandBuffer cmd, bool allowGraphicsStateInvalidate)
		{
			return this.AddGraphicsThreadMirrorViewBlit(cmd, allowGraphicsStateInvalidate, -1);
		}

		[NativeMethod(Name = "AddGraphicsThreadMirrorViewBlit", IsThreadSafe = false)]
		[NativeHeader("Runtime/Graphics/CommandBuffer/RenderingCommandBuffer.h")]
		[NativeConditional("ENABLE_XR")]
		public bool AddGraphicsThreadMirrorViewBlit(CommandBuffer cmd, bool allowGraphicsStateInvalidate, int mode)
		{
			IntPtr intPtr = XRDisplaySubsystem.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return XRDisplaySubsystem.AddGraphicsThreadMirrorViewBlit_Injected(intPtr, (cmd == null) ? ((IntPtr)0) : CommandBuffer.BindingsMarshaller.ConvertToNative(cmd), allowGraphicsStateInvalidate, mode);
		}

		public HDROutputSettings hdrOutputSettings
		{
			get
			{
				bool flag = this.m_HDROutputSettings == null;
				if (flag)
				{
					this.m_HDROutputSettings = new HDROutputSettings(-1);
				}
				return this.m_HDROutputSettings;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_displayOpaque_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_contentProtectionEnabled_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_contentProtectionEnabled_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_appliedViewportScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_scaleOfAllViewports_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_scaleOfAllViewports_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_scaleOfAllRenderTargets_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_scaleOfAllRenderTargets_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_globalDynamicScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_zNear_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_zNear_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_zFar_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_zFar_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_sRGB_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sRGB_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_occlusionMaskScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_occlusionMaskScale_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_foveatedRenderingLevel_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_foveatedRenderingLevel_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern XRDisplaySubsystem.FoveatedRenderingFlags get_foveatedRenderingFlags_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_foveatedRenderingFlags_Injected(IntPtr _unity_self, XRDisplaySubsystem.FoveatedRenderingFlags value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MarkTransformLateLatched_Injected(IntPtr _unity_self, IntPtr transform, XRDisplaySubsystem.LateLatchNode nodeType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern XRDisplaySubsystem.TextureLayout get_textureLayout_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_textureLayout_Injected(IntPtr _unity_self, XRDisplaySubsystem.TextureLayout value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern XRDisplaySubsystem.TextureLayout get_supportedTextureLayouts_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ScaledTextureWidth_Injected(IntPtr _unity_self, IntPtr renderTexture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ScaledTextureHeight_Injected(IntPtr _unity_self, IntPtr renderTexture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern XRDisplaySubsystem.ReprojectionMode get_reprojectionMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_reprojectionMode_Injected(IntPtr _unity_self, XRDisplaySubsystem.ReprojectionMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFocusPlane_Injected(IntPtr _unity_self, [In] ref Vector3 point, [In] ref Vector3 normal, [In] ref Vector3 velocity);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMSAALevel_Injected(IntPtr _unity_self, int level);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_disableLegacyRenderer_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_disableLegacyRenderer_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRenderPassCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_TryGetRenderPass_Injected(IntPtr _unity_self, int renderPassIndex, out XRDisplaySubsystem.XRRenderPass renderPass);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_TryEndRecordingIfLateLatched_Injected(IntPtr _unity_self, IntPtr camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_TryBeginRecordingIfLateLatched_Injected(IntPtr _unity_self, IntPtr camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_TryGetCullingParams_Injected(IntPtr _unity_self, IntPtr camera, int cullingPassIndex, out ScriptableCullingParameters scriptableCullingParameters);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetAppGPUTimeLastFrame_Injected(IntPtr _unity_self, out float gpuTimeLastFrame);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetCompositorGPUTimeLastFrame_Injected(IntPtr _unity_self, out float gpuTimeLastFrameCompositor);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetDroppedFrameCount_Injected(IntPtr _unity_self, out int droppedFrameCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetFramePresentCount_Injected(IntPtr _unity_self, out int framePresentCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetDisplayRefreshRate_Injected(IntPtr _unity_self, out float displayRefreshRate);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryGetMotionToPhoton_Injected(IntPtr _unity_self, out float motionToPhoton);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetRenderTexture_Injected(IntPtr _unity_self, uint unityXrRenderTextureId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetRenderTextureForRenderPass_Injected(IntPtr _unity_self, int renderPass);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSharedDepthTextureForRenderPass_Injected(IntPtr _unity_self, int renderPass);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPreferredMirrorBlitMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPreferredMirrorBlitMode_Injected(IntPtr _unity_self, int blitMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetMirrorViewBlitDesc_Injected(IntPtr _unity_self, IntPtr mirrorRt, out XRDisplaySubsystem.XRMirrorViewBlitDesc outDesc, int mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddGraphicsThreadMirrorViewBlit_Injected(IntPtr _unity_self, IntPtr cmd, bool allowGraphicsStateInvalidate, int mode);

		private HDROutputSettings m_HDROutputSettings;

		[Flags]
		public enum FoveatedRenderingFlags
		{
			None = 0,
			GazeAllowed = 1
		}

		public enum LateLatchNode
		{
			Head,
			LeftHand,
			RightHand
		}

		[Flags]
		public enum TextureLayout
		{
			Texture2DArray = 1,
			SingleTexture2D = 2,
			SeparateTexture2Ds = 4
		}

		public enum ReprojectionMode
		{
			Unspecified,
			PositionAndOrientation,
			OrientationOnly,
			None
		}

		[NativeHeader("Modules/XR/Subsystems/Display/XRDisplaySubsystem.bindings.h")]
		public struct XRRenderParameter
		{
			public Matrix4x4 view;

			public Matrix4x4 projection;

			public Rect viewport;

			public Mesh occlusionMesh;

			public Mesh visibleMesh;

			public int textureArraySlice;

			public Matrix4x4 previousView;

			public bool isPreviousViewValid;
		}

		[NativeHeader("Modules/XR/Subsystems/Display/XRDisplaySubsystem.bindings.h")]
		[NativeHeader("Runtime/Graphics/CommandBuffer/RenderingCommandBuffer.h")]
		[NativeHeader("Runtime/Graphics/RenderTextureDesc.h")]
		public struct XRRenderPass
		{
			[NativeMethod(Name = "XRRenderPassScriptApi::GetRenderParameter", IsFreeFunction = true, HasExplicitThis = true, ThrowsException = true)]
			[NativeConditional("ENABLE_XR")]
			public void GetRenderParameter(Camera camera, int renderParameterIndex, out XRDisplaySubsystem.XRRenderParameter renderParameter)
			{
				XRDisplaySubsystem.XRRenderPass.GetRenderParameter_Injected(ref this, Object.MarshalledUnityObject.Marshal<Camera>(camera), renderParameterIndex, out renderParameter);
			}

			[NativeMethod(Name = "XRRenderPassScriptApi::GetRenderParameterCount", IsFreeFunction = true, HasExplicitThis = true)]
			[NativeConditional("ENABLE_XR")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern int GetRenderParameterCount();

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetRenderParameter_Injected(ref XRDisplaySubsystem.XRRenderPass _unity_self, IntPtr camera, int renderParameterIndex, out XRDisplaySubsystem.XRRenderParameter renderParameter);

			private IntPtr displaySubsystemInstance;

			public int renderPassIndex;

			public RenderTargetIdentifier renderTarget;

			public RenderTextureDescriptor renderTargetDesc;

			public int renderTargetScaledWidth;

			public int renderTargetScaledHeight;

			public bool hasMotionVectorPass;

			public RenderTargetIdentifier motionVectorRenderTarget;

			public RenderTextureDescriptor motionVectorRenderTargetDesc;

			public bool shouldFillOutDepth;

			public bool spaceWarpRightHandedNDC;

			public int cullingPassIndex;

			public IntPtr foveatedRenderingInfo;
		}

		[NativeHeader("Runtime/Graphics/RenderTexture.h")]
		[NativeHeader("Modules/XR/Subsystems/Display/XRDisplaySubsystem.bindings.h")]
		public struct XRBlitParams
		{
			public RenderTexture srcTex;

			public int srcTexArraySlice;

			public Rect srcRect;

			public Rect destRect;

			public IntPtr foveatedRenderingInfo;

			public bool srcHdrEncoded;

			public ColorGamut srcHdrColorGamut;

			public int srcHdrMaxLuminance;
		}

		[NativeHeader("Modules/XR/Subsystems/Display/XRDisplaySubsystem.bindings.h")]
		public struct XRMirrorViewBlitDesc
		{
			[NativeConditional("ENABLE_XR")]
			[NativeMethod(Name = "XRMirrorViewBlitDescScriptApi::GetBlitParameter", IsFreeFunction = true, HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern void GetBlitParameter(int blitParameterIndex, out XRDisplaySubsystem.XRBlitParams blitParameter);

			private IntPtr displaySubsystemInstance;

			public bool nativeBlitAvailable;

			public bool nativeBlitInvalidStates;

			public int blitParamsCount;
		}

		internal new static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(XRDisplaySubsystem xrDisplaySubsystem)
			{
				return xrDisplaySubsystem.m_Ptr;
			}
		}
	}
}
