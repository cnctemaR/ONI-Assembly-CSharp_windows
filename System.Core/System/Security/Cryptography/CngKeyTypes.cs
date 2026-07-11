using System;

namespace System.Security.Cryptography
{
	[Flags]
	internal enum CngKeyTypes
	{
		None = 0,
		MachineKey = 32
	}
}
