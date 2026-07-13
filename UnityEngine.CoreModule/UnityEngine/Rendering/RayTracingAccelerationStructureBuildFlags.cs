using System;

namespace UnityEngine.Rendering
{
	[Flags]
	public enum RayTracingAccelerationStructureBuildFlags
	{
		None = 0,
		PreferFastTrace = 1,
		PreferFastBuild = 2,
		MinimizeMemory = 4
	}
}
