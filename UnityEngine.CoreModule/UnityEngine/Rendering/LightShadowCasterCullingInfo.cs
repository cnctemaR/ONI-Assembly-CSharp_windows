using System;

namespace UnityEngine.Rendering
{
	public struct LightShadowCasterCullingInfo
	{
		public RangeInt splitRange;

		public BatchCullingProjectionType projectionType;

		public ushort splitExclusionMask;
	}
}
