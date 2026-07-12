using System;

namespace Mono.Cecil
{
	[Flags]
	public enum ModuleAttributes
	{
		ILOnly = 1,
		Required32Bit = 2,
		ILLibrary = 4,
		StrongNameSigned = 8,
		Preferred32Bit = 131072
	}
}
