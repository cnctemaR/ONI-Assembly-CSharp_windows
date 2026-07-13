using System;

namespace UnityEngine.Rendering
{
	[Flags]
	public enum GraphicsTextureDescriptorFlags
	{
		None = 0,
		RenderTarget = 1,
		RandomWriteTarget = 2
	}
}
