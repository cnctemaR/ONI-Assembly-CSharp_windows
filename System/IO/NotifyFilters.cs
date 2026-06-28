using System;

namespace System.IO
{
	[Flags]
	public enum NotifyFilters
	{
		Attributes = 4,
		CreationTime = 64,
		DirectoryName = 2,
		FileName = 1,
		LastAccess = 32,
		LastWrite = 16,
		Security = 256,
		Size = 8
	}
}
