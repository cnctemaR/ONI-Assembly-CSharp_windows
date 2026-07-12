using System;

namespace FMODUnity
{
	[Flags]
	public enum ThreadAffinity : uint
	{
		Any = 0U,
		Core0 = 1U,
		Core1 = 2U,
		Core2 = 4U,
		Core3 = 8U,
		Core4 = 16U,
		Core5 = 32U,
		Core6 = 64U,
		Core7 = 128U,
		Core8 = 256U,
		Core9 = 512U,
		Core10 = 1024U,
		Core11 = 2048U,
		Core12 = 4096U,
		Core13 = 8192U,
		Core14 = 16384U,
		Core15 = 32768U
	}
}
