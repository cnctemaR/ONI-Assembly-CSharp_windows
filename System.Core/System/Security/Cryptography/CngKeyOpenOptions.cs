using System;

namespace System.Security.Cryptography
{
	[Flags]
	public enum CngKeyOpenOptions
	{
		None = 0,
		UserKey = 0,
		MachineKey = 32,
		Silent = 64
	}
}
