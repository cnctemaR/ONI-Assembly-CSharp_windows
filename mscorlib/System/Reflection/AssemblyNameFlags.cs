using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum AssemblyNameFlags
	{
		None = 0,
		PublicKey = 1,
		Retargetable = 256,
		EnableJITcompileOptimizer = 16384,
		EnableJITcompileTracking = 32768
	}
}
