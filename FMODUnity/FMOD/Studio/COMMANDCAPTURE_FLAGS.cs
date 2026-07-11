using System;

namespace FMOD.Studio
{
	[Flags]
	public enum COMMANDCAPTURE_FLAGS : uint
	{
		NORMAL = 0U,
		FILEFLUSH = 1U,
		SKIP_INITIAL_STATE = 2U
	}
}
