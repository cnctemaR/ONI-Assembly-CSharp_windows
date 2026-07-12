using System;

namespace UnityEngine
{
	[Flags]
	public enum TerrainQualityOverrides
	{
		None = 0,
		PixelError = 1,
		BasemapDistance = 2,
		DetailDensity = 4,
		DetailDistance = 8,
		TreeDistance = 16,
		BillboardStart = 32,
		FadeLength = 64,
		MaxTrees = 128
	}
}
