using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering
{
	[MovedFrom("UnityEngine.Experimental.Rendering")]
	public struct RayTracingMeshInstanceConfig
	{
		public RayTracingMeshInstanceConfig()
		{
			this.mesh = null;
			this.subMeshIndex = 0U;
			this.material = null;
			this.subMeshFlags = RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.ClosestHitOnly;
			this.rayTracingMode = RayTracingMode.Static;
			this.dynamicGeometry = false;
			this.materialProperties = null;
			this.enableTriangleCulling = true;
			this.frontTriangleCounterClockwise = false;
			this.layer = 0;
			this.renderingLayerMask = RenderingLayerMask.defaultRenderingLayerMask;
			this.mask = 255U;
			this.motionVectorMode = MotionVectorGenerationMode.Camera;
			this.lightProbeUsage = LightProbeUsage.Off;
			this.lightProbeProxyVolume = null;
			this.accelerationStructureBuildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
			this.accelerationStructureBuildFlagsOverride = false;
			this.meshLod = 0;
		}

		public RayTracingMeshInstanceConfig(Mesh mesh, uint subMeshIndex, Material material)
		{
			this.mesh = mesh;
			this.subMeshIndex = subMeshIndex;
			this.material = material;
			this.subMeshFlags = RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.ClosestHitOnly;
			this.rayTracingMode = RayTracingMode.Static;
			this.dynamicGeometry = false;
			this.materialProperties = null;
			this.enableTriangleCulling = true;
			this.frontTriangleCounterClockwise = false;
			this.layer = 0;
			this.renderingLayerMask = RenderingLayerMask.defaultRenderingLayerMask;
			this.mask = 255U;
			this.motionVectorMode = MotionVectorGenerationMode.Camera;
			this.lightProbeUsage = LightProbeUsage.Off;
			this.lightProbeProxyVolume = null;
			this.accelerationStructureBuildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
			this.accelerationStructureBuildFlagsOverride = false;
			this.meshLod = 0;
		}

		public RayTracingMode rayTracingMode { readonly get; set; }

		[Obsolete("dynamicGeometry has been deprecated and will be removed in the future. Use rayTracingMode instead.", false)]
		public bool dynamicGeometry { readonly get; set; }

		public RayTracingAccelerationStructureBuildFlags accelerationStructureBuildFlags { readonly get; set; }

		public bool accelerationStructureBuildFlagsOverride { readonly get; set; }

		public Mesh mesh;

		public uint subMeshIndex;

		public RayTracingSubMeshFlags subMeshFlags;

		public Material material;

		public MaterialPropertyBlock materialProperties;

		public bool enableTriangleCulling;

		public bool frontTriangleCounterClockwise;

		public int layer;

		public uint renderingLayerMask;

		public uint mask;

		public MotionVectorGenerationMode motionVectorMode;

		public LightProbeUsage lightProbeUsage;

		public LightProbeProxyVolume lightProbeProxyVolume;

		public int meshLod;
	}
}
