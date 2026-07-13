using System;

namespace Mono.Cecil
{
	[Flags]
	internal enum EventAttributes : ushort
	{
		None = 0,
		SpecialName = 512,
		RTSpecialName = 1024
	}
}
