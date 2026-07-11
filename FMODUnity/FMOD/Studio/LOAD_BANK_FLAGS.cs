using System;

namespace FMOD.Studio
{
	[Flags]
	public enum LOAD_BANK_FLAGS : uint
	{
		NORMAL = 0U,
		NONBLOCKING = 1U,
		DECOMPRESS_SAMPLES = 2U,
		UNENCRYPTED = 4U
	}
}
