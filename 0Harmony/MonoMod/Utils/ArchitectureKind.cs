using System;

namespace MonoMod.Utils
{
	internal enum ArchitectureKind
	{
		Unknown,
		Bits64,
		x86,
		x86_64,
		AMD64 = 3,
		Arm,
		Arm64
	}
}
