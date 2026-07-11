using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum ParameterAttributes
	{
		None = 0,
		In = 1,
		Out = 2,
		Lcid = 4,
		Retval = 8,
		Optional = 16,
		ReservedMask = 61440,
		HasDefault = 4096,
		HasFieldMarshal = 8192,
		Reserved3 = 16384,
		Reserved4 = 32768
	}
}
