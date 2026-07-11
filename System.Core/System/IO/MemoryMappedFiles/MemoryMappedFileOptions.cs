using System;

namespace System.IO.MemoryMappedFiles
{
	[Flags]
	[Serializable]
	public enum MemoryMappedFileOptions
	{
		None = 0,
		DelayAllocatePages = 67108864
	}
}
