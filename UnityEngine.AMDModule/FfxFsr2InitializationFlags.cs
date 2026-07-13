using System;

namespace UnityEngine.AMD
{
	[Flags]
	public enum FfxFsr2InitializationFlags
	{
		EnableHighDynamicRange = 1,
		EnableDisplayResolutionMotionVectors = 2,
		EnableMotionVectorsJitterCancellation = 4,
		DepthInverted = 8,
		EnableDepthInfinite = 16,
		EnableAutoExposure = 32,
		EnableDynamicResolution = 64,
		EnableTexture1DUsage = 128
	}
}
