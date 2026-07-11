using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum EventAttributes
	{
		None = 0,
		SpecialName = 512,
		ReservedMask = 1024,
		RTSpecialName = 1024
	}
}
