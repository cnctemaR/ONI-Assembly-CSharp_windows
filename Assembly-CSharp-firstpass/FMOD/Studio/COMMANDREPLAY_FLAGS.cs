using System;

namespace FMOD.Studio
{
	[Flags]
	public enum COMMANDREPLAY_FLAGS : uint
	{
		NORMAL = 0U,
		SKIP_CLEANUP = 1U,
		FAST_FORWARD = 2U
	}
}
