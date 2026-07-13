using System;

namespace UnityEngine.Rendering
{
	public struct RayTracingSubGeometryDesc
	{
		public RayTracingSubGeometryDesc()
		{
			this.flags = RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.ClosestHitOnly;
			this.id = 0;
			this.indexStart = 0;
			this.indexCount = 0;
			this.vertexStart = 0;
			this.vertexCount = 0;
		}

		public RayTracingSubGeometryDesc(int indexStart, int indexCount, int id = 0, RayTracingSubMeshFlags flags = RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.ClosestHitOnly)
		{
			this.vertexStart = 0;
			this.vertexCount = 0;
			this.indexStart = indexStart;
			this.indexCount = indexCount;
			this.id = id;
			this.flags = flags;
		}

		public RayTracingSubMeshFlags flags { readonly get; set; }

		public int id { readonly get; set; }

		public int indexStart { readonly get; set; }

		public int indexCount { readonly get; set; }

		public int vertexStart { readonly get; set; }

		public int vertexCount { readonly get; set; }
	}
}
