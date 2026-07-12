using System;

namespace UnityEngine.Experimental.Rendering
{
	[Flags]
	public enum TextureCreationFlags
	{
		None = 0,
		MipChain = 1,
		DontInitializePixels = 4,
		Crunch = 64,
		DontUploadUponCreate = 1024,
		IgnoreMipmapLimit = 2048
	}
}
