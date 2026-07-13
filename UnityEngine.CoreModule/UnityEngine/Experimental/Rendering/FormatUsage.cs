using System;

namespace UnityEngine.Experimental.Rendering
{
	[Obsolete("Use GraphicsFormatUsage instead", false)]
	public enum FormatUsage
	{
		Sample,
		Linear,
		Sparse,
		Render = 4,
		Blend,
		GetPixels,
		SetPixels,
		SetPixels32,
		ReadPixels,
		LoadStore,
		MSAA2x,
		MSAA4x,
		MSAA8x,
		StencilSampling = 16
	}
}
