using System;

namespace UnityEngine.Rendering
{
	public struct RayTracingAABBsInstanceConfig
	{
		public RayTracingAABBsInstanceConfig()
		{
			this.aabbBuffer = null;
			this.aabbCount = 0;
			this.material = null;
			this.dynamicGeometry = false;
			this.opaqueMaterial = true;
			this.aabbOffset = 0U;
			this.materialProperties = null;
			this.layer = 0;
			this.mask = 255U;
			this.accelerationStructureBuildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
			this.accelerationStructureBuildFlagsOverride = false;
		}

		public RayTracingAABBsInstanceConfig(GraphicsBuffer aabbBuffer, int aabbCount, bool dynamicGeometry, Material material)
		{
			this.aabbBuffer = aabbBuffer;
			this.aabbCount = aabbCount;
			this.material = material;
			this.dynamicGeometry = dynamicGeometry;
			this.opaqueMaterial = true;
			this.aabbOffset = 0U;
			this.materialProperties = null;
			this.layer = 0;
			this.mask = 255U;
			this.accelerationStructureBuildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
			this.accelerationStructureBuildFlagsOverride = false;
		}

		public GraphicsBuffer aabbBuffer { readonly get; set; }

		public int aabbCount { readonly get; set; }

		public uint aabbOffset { readonly get; set; }

		public bool dynamicGeometry { readonly get; set; }

		public bool opaqueMaterial { readonly get; set; }

		public Material material { readonly get; set; }

		public MaterialPropertyBlock materialProperties { readonly get; set; }

		public int layer { readonly get; set; }

		public uint mask { readonly get; set; }

		public RayTracingAccelerationStructureBuildFlags accelerationStructureBuildFlags { readonly get; set; }

		public bool accelerationStructureBuildFlagsOverride { readonly get; set; }
	}
}
