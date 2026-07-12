using System;

namespace UnityEngine.Experimental.Rendering
{
	public struct RayTracingSubMeshFlagsConfig
	{
		public RayTracingSubMeshFlags opaqueMaterials;

		public RayTracingSubMeshFlags transparentMaterials;

		public RayTracingSubMeshFlags alphaTestedMaterials;
	}
}
