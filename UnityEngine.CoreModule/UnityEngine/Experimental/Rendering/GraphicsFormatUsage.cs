using System;

namespace UnityEngine.Experimental.Rendering
{
	[Flags]
	public enum GraphicsFormatUsage
	{
		None = 0,
		Sample = 1,
		Linear = 2,
		Sparse = 4,
		Render = 16,
		Blend = 32,
		GetPixels = 64,
		SetPixels = 128,
		SetPixels32 = 256,
		ReadPixels = 512,
		LoadStore = 1024,
		MSAA2x = 2048,
		MSAA4x = 4096,
		MSAA8x = 8192,
		StencilSampling = 65536
	}
}
