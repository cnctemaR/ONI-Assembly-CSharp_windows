using System;

namespace Epic.OnlineServices.Platform
{
	[Flags]
	public enum PlatformFlags : ulong
	{
		None = 0UL,
		LoadingInEditor = 1UL,
		DisableOverlay = 2UL,
		DisableSocialOverlay = 4UL
	}
}
