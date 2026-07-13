using System;

namespace UnityEngine.Networking
{
	[Flags]
	public enum DownloadedTextureFlags : uint
	{
		None = 0U,
		Readable = 1U,
		MipmapChain = 2U,
		LinearColorSpace = 4U
	}
}
