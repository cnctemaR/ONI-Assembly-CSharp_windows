using System;

namespace System.IO
{
	[Flags]
	internal enum EventFlags : ushort
	{
		Add = 1,
		Delete = 2,
		Enable = 4,
		Disable = 8,
		OneShot = 16,
		Clear = 32,
		Receipt = 64,
		Dispatch = 128,
		Flag0 = 4096,
		Flag1 = 8192,
		SystemFlags = 61440,
		EOF = 32768,
		Error = 16384
	}
}
