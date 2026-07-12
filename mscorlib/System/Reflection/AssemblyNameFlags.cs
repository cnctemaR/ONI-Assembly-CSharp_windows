using System;

namespace System.Reflection
{
	[Flags]
	public enum AssemblyNameFlags
	{
		None = 0,
		PublicKey = 1,
		EnableJITcompileOptimizer = 16384,
		EnableJITcompileTracking = 32768,
		Retargetable = 256
	}
}
