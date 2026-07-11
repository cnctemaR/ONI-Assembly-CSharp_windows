using System;

namespace FMOD.Studio
{
	[Flags]
	public enum PARAMETER_FLAGS : uint
	{
		READONLY = 1U,
		AUTOMATIC = 2U,
		GLOBAL = 4U,
		DISCRETE = 8U
	}
}
