using System;

namespace Unity.Burst
{
	[Flags]
	internal enum NativeDumpFlags
	{
		None = 0,
		IL = 1,
		Unused = 2,
		IR = 4,
		IROptimized = 8,
		Asm = 16,
		Function = 32,
		Analysis = 64,
		IRPassAnalysis = 128,
		ILPre = 256,
		IRPerEntryPoint = 512,
		All = 1021
	}
}
