using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.VFX
{
	[StaticAccessor("GetVFXManager()", StaticAccessorType.Dot)]
	[NativeHeader("Modules/VFX/Public/VFXManager.h")]
	[RequiredByNativeCode]
	public static class VFXManager
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern VisualEffect[] GetComponents();

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

		internal static extern string renderPipeSettingsPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ProcessCamera(Camera cam);

		[RequiredByNativeCode]
		internal static void RegisterPerCameraCallback()
		{
			RenderPipeline.beginCameraRendering += VFXManager.ProcessCamera;
		}

		[RequiredByNativeCode]
		internal static void UnregisterPerCameraCallback()
		{
			RenderPipeline.beginCameraRendering -= VFXManager.ProcessCamera;
		}
	}
}
