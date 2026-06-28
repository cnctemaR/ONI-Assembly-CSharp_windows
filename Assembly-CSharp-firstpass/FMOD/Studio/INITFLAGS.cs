using System;

namespace FMOD.Studio
{
	[Flags]
	public enum INITFLAGS : uint
	{
		NORMAL = 0U,
		LIVEUPDATE = 1U,
		ALLOW_MISSING_PLUGINS = 2U,
		SYNCHRONOUS_UPDATE = 4U,
		DEFERRED_CALLBACKS = 8U
	}
}
