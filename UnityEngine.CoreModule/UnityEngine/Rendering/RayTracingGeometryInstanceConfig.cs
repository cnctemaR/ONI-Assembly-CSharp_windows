using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	public struct RayTracingGeometryInstanceConfig
	{
		public RayTracingGeometryInstanceConfig()
		{
			this.material = null;
			this.vertexBuffer = null;
			this.indexBuffer = null;
			this.vertexAttributes = null;
			this.vertexStart = 0U;
			this.indexStart = 0U;
			this.vertexCount = -1;
			this.indexCount = -1;
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
			this.accelerationStructureBuildFlagsOverride = false;
			this.accelerationStructureBuildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
		}

		public RayTracingGeometryInstanceConfig(GraphicsBuffer vertexBuffer, VertexAttributeDescriptor[] vertexAttributes, GraphicsBuffer indexBuffer, Material material)
		{
			this.material = material;
			this.vertexBuffer = vertexBuffer;
			this.indexBuffer = indexBuffer;
			this.vertexAttributes = vertexAttributes;
			this.vertexStart = 0U;
			this.indexStart = 0U;
			this.vertexCount = -1;
			this.indexCount = -1;
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
			this.accelerationStructureBuildFlagsOverride = false;
			this.accelerationStructureBuildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
		}

		public GraphicsBuffer vertexBuffer { readonly get; set; }

		public VertexAttributeDescriptor[] vertexAttributes { readonly get; set; }

		public uint vertexStart { readonly get; set; }

		public int vertexCount { readonly get; set; }

		public GraphicsBuffer indexBuffer { readonly get; set; }

		public uint indexStart { readonly get; set; }

		public int indexCount { readonly get; set; }

		public RayTracingSubMeshFlags subMeshFlags { readonly get; set; }

		public RayTracingMode rayTracingMode { readonly get; set; }

		[Obsolete("dynamicGeometry has been deprecated and will be removed in the future. Use rayTracingMode instead.", false)]
		public bool dynamicGeometry { readonly get; set; }

		public Material material { readonly get; set; }

		public MaterialPropertyBlock materialProperties { readonly get; set; }

		public bool enableTriangleCulling { readonly get; set; }

		public bool frontTriangleCounterClockwise { readonly get; set; }

		public int layer { readonly get; set; }

		public uint renderingLayerMask { readonly get; set; }

		public uint mask { readonly get; set; }

		public MotionVectorGenerationMode motionVectorMode { readonly get; set; }

		public LightProbeUsage lightProbeUsage { readonly get; set; }

		public LightProbeProxyVolume lightProbeProxyVolume { readonly get; set; }

		public RayTracingAccelerationStructureBuildFlags accelerationStructureBuildFlags { readonly get; set; }

		public bool accelerationStructureBuildFlagsOverride { readonly get; set; }
	}
}
