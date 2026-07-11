using System;
using System.Runtime.InteropServices;

namespace System.IO
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum FileOptions
	{
		None = 0,
		Encrypted = 16384,
		DeleteOnClose = 67108864,
		SequentialScan = 134217728,
		RandomAccess = 268435456,
		Asynchronous = 1073741824,
		WriteThrough = -2147483648
	}
}
