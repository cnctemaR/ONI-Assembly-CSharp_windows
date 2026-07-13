using System;

namespace UnityEngine.VFX
{
	internal enum VFXSystemFlag
	{
		SystemDefault,
		SystemHasKill,
		SystemHasIndirectBuffer,
		SystemReceivedEventGPU = 4,
		SystemHasStrips = 8,
		SystemNeedsComputeBounds = 16,
		SystemAutomaticBounds = 32,
		SystemInWorldSpace = 64,
		SystemHasDirectLink = 128,
		SystemHasAttributeBuffer = 256,
		SystemUsesInstancedRendering = 512,
		SystemIsRayTraced = 1024
	}
}
