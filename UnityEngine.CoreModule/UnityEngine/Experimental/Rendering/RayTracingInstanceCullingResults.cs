using System;

namespace UnityEngine.Experimental.Rendering
{
	public struct RayTracingInstanceCullingResults
	{
		public RayTracingInstanceMaterialCRC[] materialsCRC;

		public bool transformsChanged;
	}
}
