using System;

namespace UnityEngine.NVIDIA
{
	[Flags]
	public enum DLSSFeatureFlags
	{
		None = 0,
		IsHDR = 1,
		MVLowRes = 2,
		MVJittered = 4,
		DepthInverted = 8,
		DoSharpening = 16
	}
}
