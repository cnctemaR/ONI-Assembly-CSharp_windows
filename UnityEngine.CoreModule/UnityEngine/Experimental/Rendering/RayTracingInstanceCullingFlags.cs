using System;

namespace UnityEngine.Experimental.Rendering
{
	[Flags]
	public enum RayTracingInstanceCullingFlags
	{
		None = 0,
		EnableSphereCulling = 1,
		EnablePlaneCulling = 2,
		EnableLODCulling = 4,
		ComputeMaterialsCRC = 8,
		IgnoreReflectionProbes = 16
	}
}
