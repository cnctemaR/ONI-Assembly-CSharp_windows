using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	public struct RayTracingMultiGeometryInstanceConfig
	{
		public RayTracingMultiGeometryInstanceConfig()
		{
			this.vertexBuffer = null;
			this.indexBuffer = null;
			this.vertexAttributes = null;
			this.rayTracingMode = RayTracingMode.Static;
			this.materials = null;
			this.subGeometries = null;
			this.subGeometriesValidation = true;
			this.materialProperties = null;
			this.enableTriangleCulling = false;
			this.frontTriangleCounterClockwise = false;
			this.layer = 0;
			this.renderingLayerMask = RenderingLayerMask.defaultRenderingLayerMask;
			this.mask = 255U;
			this.motionVectorMode = MotionVectorGenerationMode.Camera;
			this.accelerationStructureBuildFlagsOverride = false;
			this.accelerationStructureBuildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
		}

		public GraphicsBuffer vertexBuffer { readonly get; set; }

		public VertexAttributeDescriptor[] vertexAttributes { readonly get; set; }

		public GraphicsBuffer indexBuffer { readonly get; set; }

		public RayTracingMode rayTracingMode { readonly get; set; }

		public Material[] materials { readonly get; set; }

		public RayTracingSubGeometryDesc[] subGeometries { readonly get; set; }

		public bool subGeometriesValidation { readonly get; set; }

		public MaterialPropertyBlock materialProperties { readonly get; set; }

		public bool enableTriangleCulling { readonly get; set; }

		public bool frontTriangleCounterClockwise { readonly get; set; }

		public int layer { readonly get; set; }

		public uint renderingLayerMask { readonly get; set; }

		public uint mask { readonly get; set; }

		public MotionVectorGenerationMode motionVectorMode { readonly get; set; }

		public RayTracingAccelerationStructureBuildFlags accelerationStructureBuildFlags { readonly get; set; }

		public bool accelerationStructureBuildFlagsOverride { readonly get; set; }
	}
}
