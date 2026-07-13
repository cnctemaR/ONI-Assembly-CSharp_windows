using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering
{
	[MovedFrom("UnityEngine.Experimental.Rendering")]
	public struct RayTracingInstanceTriangleCullingConfig
	{
		public string[] optionalDoubleSidedShaderKeywords;

		public bool frontTriangleCounterClockwise;

		public bool checkDoubleSidedGIMaterial;

		public bool forceDoubleSided;
	}
}
