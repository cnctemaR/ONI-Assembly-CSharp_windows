using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[RequiredByNativeCode]
	internal static class GPUDrivenCallbacks
	{
		[RequiredByNativeCode(GenerateProxy = true)]
		public static void InvokeGPUDrivenLODGroupDataNativeCallback(GPUDrivenLODGroupDataNativeCallback callback, in GPUDrivenLODGroupDataNative lodGroupDataNative, GPUDrivenLODGroupDataCallback target)
		{
			callback(in lodGroupDataNative, target);
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		public static void InvokeGPUDrivenRendererDataNativeCallback(GPUDrivenRendererDataNativeCallback callback, in GPUDrivenRendererGroupDataNative rendererDataNative, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback target)
		{
			callback(in rendererDataNative, meshes, materials, target);
		}
	}
}
