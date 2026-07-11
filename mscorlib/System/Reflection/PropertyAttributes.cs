using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum PropertyAttributes
	{
		None = 0,
		SpecialName = 512,
		ReservedMask = 62464,
		RTSpecialName = 1024,
		HasDefault = 4096,
		Reserved2 = 8192,
		Reserved3 = 16384,
		Reserved4 = 32768
	}
}
