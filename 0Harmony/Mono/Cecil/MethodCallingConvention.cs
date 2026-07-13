using System;

namespace Mono.Cecil
{
	internal enum MethodCallingConvention : byte
	{
		Default,
		C,
		StdCall,
		ThisCall,
		FastCall,
		VarArg,
		Unmanaged = 9,
		Generic = 16
	}
}
