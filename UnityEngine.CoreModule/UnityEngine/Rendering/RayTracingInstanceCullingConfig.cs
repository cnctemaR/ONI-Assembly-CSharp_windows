using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering
{
	[MovedFrom("UnityEngine.Experimental.Rendering")]
	public struct RayTracingInstanceCullingConfig
	{
		public RayTracingInstanceCullingFlags flags;

		public Vector3 sphereCenter;

		public float sphereRadius;

		public float minSolidAngle;

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
