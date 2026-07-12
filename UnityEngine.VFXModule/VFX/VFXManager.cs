using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.VFX
{
	[StaticAccessor("GetVFXManager()", StaticAccessorType.Dot)]
	[NativeHeader("Modules/VFX/Public/ScriptBindings/VFXManagerBindings.h")]
	[NativeHeader("Modules/VFX/Public/VFXManager.h")]
	[RequiredByNativeCode]
	public static class VFXManager
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern VisualEffect[] GetComponents();

		internal static extern ScriptableObject runtimeResources
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
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

		internal static extern float maxScrubTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern string renderPipeSettingsPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
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

		public static VFXBatchedEffectInfo GetBatchedEffectInfo([NotNull("NullExceptionObject")] VisualEffectAsset vfx)
		{
			VFXBatchedEffectInfo vfxbatchedEffectInfo;
			VFXManager.GetBatchedEffectInfo_Injected(vfx, out vfxbatchedEffectInfo);
			return vfxbatchedEffectInfo;
		}

		[FreeFunction(Name = "VFXManagerBindings::GetBatchedEffectInfos", HasExplicitThis = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetBatchedEffectInfos([NotNull("NullExceptionObject")] List<VFXBatchedEffectInfo> infos);

		internal static VFXBatchInfo GetBatchInfo(VisualEffectAsset vfx, uint batchIndex)
		{
			VFXBatchInfo vfxbatchInfo;
			VFXManager.GetBatchInfo_Injected(vfx, batchIndex, out vfxbatchInfo);
			return vfxbatchInfo;
		}

		[Obsolete("Use explicit PrepareCamera and ProcessCameraCommand instead")]
		public static void ProcessCamera(Camera cam)
		{
			VFXManager.PrepareCamera(cam, VFXManager.kDefaultCameraXRSettings);
			VFXManager.Internal_ProcessCameraCommand(cam, null, VFXManager.kDefaultCameraXRSettings, IntPtr.Zero);
		}

		public static void PrepareCamera(Camera cam)
		{
			VFXManager.PrepareCamera(cam, VFXManager.kDefaultCameraXRSettings);
		}

		public static void PrepareCamera([NotNull("NullExceptionObject")] Camera cam, VFXCameraXRSettings camXRSettings)
		{
			VFXManager.PrepareCamera_Injected(cam, ref camXRSettings);
		}

		[Obsolete("Use ProcessCameraCommand with CullingResults to allow culling of VFX per camera")]
		public static void ProcessCameraCommand(Camera cam, CommandBuffer cmd)
		{
			VFXManager.Internal_ProcessCameraCommand(cam, cmd, VFXManager.kDefaultCameraXRSettings, IntPtr.Zero);
		}

		[Obsolete("Use ProcessCameraCommand with CullingResults to allow culling of VFX per camera")]
		public static void ProcessCameraCommand(Camera cam, CommandBuffer cmd, VFXCameraXRSettings camXRSettings)
		{
			VFXManager.Internal_ProcessCameraCommand(cam, cmd, camXRSettings, IntPtr.Zero);
		}

		public static void ProcessCameraCommand(Camera cam, CommandBuffer cmd, VFXCameraXRSettings camXRSettings, CullingResults results)
		{
			VFXManager.Internal_ProcessCameraCommand(cam, cmd, camXRSettings, results.ptr);
		}

		private static void Internal_ProcessCameraCommand([NotNull("NullExceptionObject")] Camera cam, CommandBuffer cmd, VFXCameraXRSettings camXRSettings, IntPtr cullResults)
		{
			VFXManager.Internal_ProcessCameraCommand_Injected(cam, cmd, ref camXRSettings, cullResults);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern VFXCameraBufferTypes IsCameraBufferNeeded([NotNull("NullExceptionObject")] Camera cam);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetCameraBuffer([NotNull("NullExceptionObject")] Camera cam, VFXCameraBufferTypes type, Texture buffer, int x, int y, int width, int height);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBatchedEffectInfo_Injected(VisualEffectAsset vfx, out VFXBatchedEffectInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBatchInfo_Injected(VisualEffectAsset vfx, uint batchIndex, out VFXBatchInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PrepareCamera_Injected(Camera cam, ref VFXCameraXRSettings camXRSettings);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ProcessCameraCommand_Injected(Camera cam, CommandBuffer cmd, ref VFXCameraXRSettings camXRSettings, IntPtr cullResults);

		private static readonly VFXCameraXRSettings kDefaultCameraXRSettings = new VFXCameraXRSettings
		{
			viewTotal = 1U,
			viewCount = 1U,
			viewOffset = 0U
		};
	}
}
