using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	internal delegate void GPUDrivenRendererDataNativeCallback(in GPUDrivenRendererGroupDataNative rendererDataNative, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback callback);
}
