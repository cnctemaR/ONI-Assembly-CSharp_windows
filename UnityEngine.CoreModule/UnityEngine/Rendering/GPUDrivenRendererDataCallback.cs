using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	internal delegate void GPUDrivenRendererDataCallback(in GPUDrivenRendererGroupData rendererData, IList<Mesh> meshes, IList<Material> materials);
}
