using System;

namespace UnityEngine.Experimental.Rendering
{
	public struct RayTracingInstanceMaterialConfig
	{
		public int renderQueueLowerBound;

		public int renderQueueUpperBound;

		public RayTracingInstanceCullingShaderTagConfig[] optionalShaderTags;

		public string[] optionalShaderKeywords;
	}
}
