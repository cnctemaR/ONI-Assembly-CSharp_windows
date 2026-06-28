using System;

namespace Mono.Unix
{
	[Flags]
	public enum FileAccessPermissions
	{
		UserReadWriteExecute = 448,
		UserRead = 256,
		UserWrite = 128,
		UserExecute = 64,
		GroupReadWriteExecute = 56,
		GroupRead = 32,
		GroupWrite = 16,
		GroupExecute = 8,
		OtherReadWriteExecute = 7,
		OtherRead = 4,
		OtherWrite = 2,
		OtherExecute = 1,
		DefaultPermissions = 438,
		AllPermissions = 511
	}
}
