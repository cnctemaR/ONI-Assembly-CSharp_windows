using System;

namespace Mono.Cecil
{
	[Flags]
	internal enum MethodSemanticsAttributes : ushort
	{
		None = 0,
		Setter = 1,
		Getter = 2,
		Other = 4,
		AddOn = 8,
		RemoveOn = 16,
		Fire = 32
	}
}
