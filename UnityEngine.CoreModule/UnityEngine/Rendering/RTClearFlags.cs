using System;

namespace UnityEngine.Rendering
{
	[Flags]
	public enum RTClearFlags
	{
		None = 0,
		Color = 1,
		Depth = 2,
		Stencil = 4,
		All = 7,
		DepthStencil = 6,
		ColorDepth = 3,
		ColorStencil = 5,
		Color0 = 8,
		Color1 = 16,
		Color2 = 32,
		Color3 = 64,
		Color4 = 128,
		Color5 = 256,
		Color6 = 512,
		Color7 = 1024
	}
}
