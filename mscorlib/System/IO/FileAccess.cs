using System;
using System.Runtime.InteropServices;

namespace System.IO
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum FileAccess
	{
		Read = 1,
		Write = 2,
		ReadWrite = 3
	}
}
