using System;

namespace UnityEngine.Experimental.Rendering
{
	public struct RayTracingInstanceTriangleCullingConfig
	{
		public string[] optionalDoubleSidedShaderKeywords;

		public bool frontTriangleCounterClockwise;

		public bool checkDoubleSidedGIMaterial;

		public bool forceDoubleSided;
	}
}
