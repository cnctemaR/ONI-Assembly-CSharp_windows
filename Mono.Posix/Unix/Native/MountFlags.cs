using System;

namespace Mono.Unix.Native
{
	[Map]
	[Flags]
	[CLSCompliant(false)]
	public enum MountFlags : ulong
	{
		ST_RDONLY = 1UL,
		ST_NOSUID = 2UL,
		ST_NODEV = 4UL,
		ST_NOEXEC = 8UL,
		ST_SYNCHRONOUS = 16UL,
		ST_REMOUNT = 32UL,
		ST_MANDLOCK = 64UL,
		ST_WRITE = 128UL,
		ST_APPEND = 256UL,
		ST_IMMUTABLE = 512UL,
		ST_NOATIME = 1024UL,
		ST_NODIRATIME = 2048UL,
		ST_BIND = 4096UL
	}
}
