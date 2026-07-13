using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering
{
	[MovedFrom("UnityEngine.Experimental.Rendering")]
	public struct RayTracingInstanceMaterialConfig
	{
		public int renderQueueLowerBound;

		public int renderQueueUpperBound;

		public RayTracingInstanceCullingShaderTagConfig[] optionalShaderTags;

		public string[] optionalShaderKeywords;
	}
}
