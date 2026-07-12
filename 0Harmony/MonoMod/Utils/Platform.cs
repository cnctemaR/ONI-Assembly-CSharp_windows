using System;

namespace MonoMod.Utils
{
	[Flags]
	internal enum Platform
	{
		OS = 1,
		Bits64 = 2,
		NT = 4,
		Unix = 8,
		ARM = 65536,
		Wine = 131072,
		Unknown = 17,
		Windows = 37,
		MacOS = 73,
		Linux = 137,
		Android = 393,
		iOS = 585
	}
}
