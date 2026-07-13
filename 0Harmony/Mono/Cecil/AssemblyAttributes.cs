using System;

namespace Mono.Cecil
{
	[Flags]
	internal enum AssemblyAttributes : uint
	{
		PublicKey = 1U,
		SideBySideCompatible = 0U,
		Retargetable = 256U,
		WindowsRuntime = 512U,
		DisableJITCompileOptimizer = 16384U,
		EnableJITCompileTracking = 32768U
	}
}
