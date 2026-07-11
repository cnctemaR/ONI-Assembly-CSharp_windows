using System;

namespace Mono.Posix
{
	[Flags]
	[CLSCompliant(false)]
	[Obsolete("Use Mono.Unix.Native.FilePermissions")]
	public enum FileMode
	{
		S_ISUID = 2048,
		S_ISGID = 1024,
		S_ISVTX = 512,
		S_IRUSR = 256,
		S_IWUSR = 128,
		S_IXUSR = 64,
		S_IRGRP = 32,
		S_IWGRP = 16,
		S_IXGRP = 8,
		S_IROTH = 4,
		S_IWOTH = 2,
		S_IXOTH = 1
	}
}
