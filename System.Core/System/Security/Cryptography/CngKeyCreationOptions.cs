using System;

namespace System.Security.Cryptography
{
	[Flags]
	public enum CngKeyCreationOptions
	{
		None = 0,
		MachineKey = 32,
		OverwriteExistingKey = 128
	}
}
