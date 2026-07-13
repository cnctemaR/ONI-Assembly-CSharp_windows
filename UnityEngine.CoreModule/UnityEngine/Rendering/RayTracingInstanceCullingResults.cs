using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering
{
	[MovedFrom("UnityEngine.Experimental.Rendering")]
	public struct RayTracingInstanceCullingResults
	{
		public RayTracingInstanceMaterialCRC[] materialsCRC;

		public bool transformsChanged;
	}
}
