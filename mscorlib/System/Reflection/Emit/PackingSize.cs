using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public enum PackingSize
	{
		Unspecified,
		Size1,
		Size2,
		Size4 = 4,
		Size8 = 8,
		Size16 = 16,
		Size32 = 32,
		Size64 = 64,
		Size128 = 128
	}
}
