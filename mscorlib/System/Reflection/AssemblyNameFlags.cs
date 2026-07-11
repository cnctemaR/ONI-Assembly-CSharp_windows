using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum AssemblyNameFlags
	{
		None = 0,
		PublicKey = 1,
		EnableJITcompileOptimizer = 16384,
		EnableJITcompileTracking = 32768,
		Retargetable = 256
	}
}
