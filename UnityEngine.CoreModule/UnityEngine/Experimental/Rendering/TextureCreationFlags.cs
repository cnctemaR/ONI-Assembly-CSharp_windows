using System;
using UnityEngine.Internal;

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
		[Obsolete("IgnoreMipmapLimit flag is no longer used since this is now the default behavior for all Texture shapes. Please provide mipmap limit information using a MipmapLimitDescriptor argument.", false)]
		[ExcludeFromDocs]
		IgnoreMipmapLimit = 2048
	}
}
