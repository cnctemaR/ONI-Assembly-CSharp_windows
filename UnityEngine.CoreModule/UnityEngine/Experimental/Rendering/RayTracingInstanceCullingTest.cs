using System;

namespace UnityEngine.Experimental.Rendering
{
	public struct RayTracingInstanceCullingTest
	{
		public uint instanceMask;

		public int layerMask;

		public int shadowCastingModeMask;

		public bool allowOpaqueMaterials;

		public bool allowTransparentMaterials;

		public bool allowAlphaTestedMaterials;
	}
}
