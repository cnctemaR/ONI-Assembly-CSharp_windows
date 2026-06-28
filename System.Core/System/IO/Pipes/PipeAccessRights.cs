using System;

namespace System.IO.Pipes
{
	[Flags]
	public enum PipeAccessRights
	{
		ReadData = 1,
		WriteData = 2,
		ReadAttributes = 4,
		WriteAttributes = 8,
		ReadExtendedAttributes = 16,
		WriteExtendedAttributes = 32,
		CreateNewInstance = 64,
		Delete = 128,
		ReadPermissions = 256,
		ChangePermissions = 512,
		TakeOwnership = 1024,
		Synchronize = 2048,
		FullControl = 1855,
		Read = 277,
		Write = 554,
		ReadWrite = 831,
		AccessSystemSecurity = 1792
	}
}
