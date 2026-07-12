using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
	public struct RayTracingInstanceCullingConfig
	{
		public RayTracingInstanceCullingFlags flags;

		public Vector3 sphereCenter;

		public float sphereRadius;

		public Plane[] planes;

		public RayTracingInstanceCullingTest[] instanceTests;

		public RayTracingInstanceCullingMaterialTest materialTest;

		public RayTracingInstanceMaterialConfig transparentMaterialConfig;

		public RayTracingInstanceMaterialConfig alphaTestedMaterialConfig;

		public RayTracingSubMeshFlagsConfig subMeshFlagsConfig;

		public RayTracingInstanceTriangleCullingConfig triangleCullingConfig;

		public LODParameters lodParameters;
	}
}
