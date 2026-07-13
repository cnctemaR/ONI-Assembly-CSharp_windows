using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering
{
	[MovedFrom("UnityEngine.Experimental.Rendering")]
	public struct RayTracingInstanceCullingMaterialTest
	{
		public string[] deniedShaderPasses;

		public RayTracingInstanceCullingShaderTagConfig[] requiredShaderTags;
	}
}
