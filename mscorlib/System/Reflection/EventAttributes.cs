using System;

namespace System.Reflection
{
	[Flags]
	public enum EventAttributes
	{
		None = 0,
		SpecialName = 512,
		RTSpecialName = 1024,
		ReservedMask = 1024
	}
}
