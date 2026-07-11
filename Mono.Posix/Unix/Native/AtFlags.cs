using System;

namespace Mono.Unix.Native
{
	[Map]
	[Flags]
	[CLSCompliant(false)]
	public enum AtFlags
	{
		AT_SYMLINK_NOFOLLOW = 256,
		AT_REMOVEDIR = 512,
		AT_SYMLINK_FOLLOW = 1024,
		AT_NO_AUTOMOUNT = 2048,
		AT_EMPTY_PATH = 4096
	}
}
