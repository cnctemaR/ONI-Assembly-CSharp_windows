using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.VFX
{
	[RequiredByNativeCode]
	[NativeHeader("Modules/VFX/Public/VFXManager.h")]
	[StaticAccessor("GetVFXManager()", StaticAccessorType.Dot)]
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

		public static void ProcessCamera(Camera cam)
		{
			VFXManager.PrepareCamera(cam);
			VFXManager.ProcessCameraCommand(cam, null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PrepareCamera(Camera cam);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ProcessCameraCommand(Camera cam, CommandBuffer cmd);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern VFXCameraBufferTypes IsCameraBufferNeeded(Camera cam);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetCameraBuffer(Camera cam, VFXCameraBufferTypes type, Texture buffer, int x, int y, int width, int height);
	}
}
