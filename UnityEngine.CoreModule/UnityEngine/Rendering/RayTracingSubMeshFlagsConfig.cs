using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering
{
	[MovedFrom("UnityEngine.Experimental.Rendering")]
	public struct RayTracingSubMeshFlagsConfig
	{
		public RayTracingSubMeshFlags opaqueMaterials;

		public RayTracingSubMeshFlags transparentMaterials;

		public RayTracingSubMeshFlags alphaTestedMaterials;
	}
}
