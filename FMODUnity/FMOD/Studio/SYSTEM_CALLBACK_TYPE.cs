using System;

namespace FMOD.Studio
{
	[Flags]
	public enum SYSTEM_CALLBACK_TYPE : uint
	{
		PREUPDATE = 1U,
		POSTUPDATE = 2U,
		BANK_UNLOAD = 4U,
		ALL = 4294967295U
	}
}
