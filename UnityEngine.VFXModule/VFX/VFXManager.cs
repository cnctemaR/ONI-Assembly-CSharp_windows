using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.VFX
{
	[StaticAccessor("GetVFXManager()", StaticAccessorType.Dot)]
	[NativeHeader("Modules/VFX/Public/ScriptBindings/VFXManagerBindings.h")]
	[RequiredByNativeCode]
	[NativeHeader("Modules/VFX/Public/VFXManager.h")]
	public static class VFXManager
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern VisualEffect[] GetComponents();

		internal static ScriptableObject runtimeResources
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<ScriptableObject>(VFXManager.get_runtimeResources_Injected());
			}
		}

		public static extern float fixedTimeStep
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern float maxDeltaTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern uint maxCapacity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern float maxScrubTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static string renderPipeSettingsPath
		{
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					VFXManager.get_renderPipeSettingsPath_Injected(out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		internal static extern uint batchEmptyLifetime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CleanupEmptyBatches(bool force = false);

		public static void FlushEmptyBatches()
		{
			VFXManager.CleanupEmptyBatches(true);
		}

		public static VFXBatchedEffectInfo GetBatchedEffectInfo([NotNull] VisualEffectAsset vfx)
		{
			if (vfx == null)
			{
				ThrowHelper.ThrowArgumentNullException(vfx, "vfx");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<VisualEffectAsset>(vfx);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(vfx, "vfx");
			}
			VFXBatchedEffectInfo vfxbatchedEffectInfo;
			VFXManager.GetBatchedEffectInfo_Injected(intPtr, out vfxbatchedEffectInfo);
			return vfxbatchedEffectInfo;
		}

		[FreeFunction(Name = "VFXManagerBindings::GetBatchedEffectInfos", HasExplicitThis = false)]
		public static void GetBatchedEffectInfos([NotNull] List<VFXBatchedEffectInfo> infos)
		{
			if (infos == null)
			{
				ThrowHelper.ThrowArgumentNullException(infos, "infos");
			}
			VFXManager.GetBatchedEffectInfos_Injected(infos);
		}

		internal static VFXBatchInfo GetBatchInfo(VisualEffectAsset vfx, uint batchIndex)
		{
			VFXBatchInfo vfxbatchInfo;
			VFXManager.GetBatchInfo_Injected(Object.MarshalledUnityObject.Marshal<VisualEffectAsset>(vfx), batchIndex, out vfxbatchInfo);
			return vfxbatchInfo;
		}

		[Obsolete("Use explicit PrepareCamera and ProcessCameraCommand instead")]
		public static void ProcessCamera(Camera cam)
		{
			VFXManager.PrepareCamera(cam, VFXManager.kDefaultCameraXRSettings);
			VFXManager.Internal_ProcessCameraCommand(cam, null, VFXManager.kDefaultCameraXRSettings, IntPtr.Zero, IntPtr.Zero);
		}

		public static void PrepareCamera(Camera cam)
		{
			VFXManager.PrepareCamera(cam, VFXManager.kDefaultCameraXRSettings);
		}

		public static void PrepareCamera([NotNull] Camera cam, VFXCameraXRSettings camXRSettings)
		{
			if (cam == null)
			{
				ThrowHelper.ThrowArgumentNullException(cam, "cam");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(cam);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(cam, "cam");
			}
			VFXManager.PrepareCamera_Injected(intPtr, ref camXRSettings);
		}

		[Obsolete("Use ProcessCameraCommand with CullingResults to allow culling of VFX per camera")]
		public static void ProcessCameraCommand(Camera cam, CommandBuffer cmd)
		{
			VFXManager.Internal_ProcessCameraCommand(cam, cmd, VFXManager.kDefaultCameraXRSettings, IntPtr.Zero, IntPtr.Zero);
		}

		[Obsolete("Use ProcessCameraCommand with CullingResults to allow culling of VFX per camera")]
		public static void ProcessCameraCommand(Camera cam, CommandBuffer cmd, VFXCameraXRSettings camXRSettings)
		{
			VFXManager.Internal_ProcessCameraCommand(cam, cmd, camXRSettings, IntPtr.Zero, IntPtr.Zero);
		}

		public static void ProcessCameraCommand(Camera cam, CommandBuffer cmd, VFXCameraXRSettings camXRSettings, CullingResults results)
		{
			VFXManager.Internal_ProcessCameraCommand(cam, cmd, camXRSettings, results.ptr, IntPtr.Zero);
		}

		public static void ProcessCameraCommand(Camera cam, CommandBuffer cmd, VFXCameraXRSettings camXRSettings, CullingResults results, CullingResults customPassResults)
		{
			VFXManager.Internal_ProcessCameraCommand(cam, cmd, camXRSettings, results.ptr, customPassResults.ptr);
		}

		private static void Internal_ProcessCameraCommand([NotNull] Camera cam, CommandBuffer cmd, VFXCameraXRSettings camXRSettings, IntPtr cullResults, IntPtr customPassCullResults)
		{
			if (cam == null)
			{
				ThrowHelper.ThrowArgumentNullException(cam, "cam");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(cam);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(cam, "cam");
			}
			VFXManager.Internal_ProcessCameraCommand_Injected(intPtr, (cmd == null) ? ((IntPtr)0) : CommandBuffer.BindingsMarshaller.ConvertToNative(cmd), ref camXRSettings, cullResults, customPassCullResults);
		}

		public static VFXCameraBufferTypes IsCameraBufferNeeded([NotNull] Camera cam)
		{
			if (cam == null)
			{
				ThrowHelper.ThrowArgumentNullException(cam, "cam");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(cam);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(cam, "cam");
			}
			return VFXManager.IsCameraBufferNeeded_Injected(intPtr);
		}

		public static void SetCameraBuffer([NotNull] Camera cam, VFXCameraBufferTypes type, Texture buffer, int x, int y, int width, int height)
		{
			if (cam == null)
			{
				ThrowHelper.ThrowArgumentNullException(cam, "cam");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Camera>(cam);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(cam, "cam");
			}
			VFXManager.SetCameraBuffer_Injected(intPtr, type, Object.MarshalledUnityObject.Marshal<Texture>(buffer), x, y, width, height);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetRayTracingEnabled(bool enabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RequestRtasAabbConstruction();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_runtimeResources_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_renderPipeSettingsPath_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBatchedEffectInfo_Injected(IntPtr vfx, out VFXBatchedEffectInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBatchedEffectInfos_Injected(List<VFXBatchedEffectInfo> infos);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBatchInfo_Injected(IntPtr vfx, uint batchIndex, out VFXBatchInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PrepareCamera_Injected(IntPtr cam, [In] ref VFXCameraXRSettings camXRSettings);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ProcessCameraCommand_Injected(IntPtr cam, IntPtr cmd, [In] ref VFXCameraXRSettings camXRSettings, IntPtr cullResults, IntPtr customPassCullResults);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VFXCameraBufferTypes IsCameraBufferNeeded_Injected(IntPtr cam);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetCameraBuffer_Injected(IntPtr cam, VFXCameraBufferTypes type, IntPtr buffer, int x, int y, int width, int height);

		private static readonly VFXCameraXRSettings kDefaultCameraXRSettings = new VFXCameraXRSettings
		{
			viewTotal = 1U,
			viewCount = 1U,
			viewOffset = 0U
		};
	}
}
