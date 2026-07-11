using System;

namespace Mono.Unix.Native
{
	[Flags]
	[CLSCompliant(false)]
	[Map]
	public enum FilePermissions : uint
	{
		S_ISUID = 2048U,
		S_ISGID = 1024U,
		S_ISVTX = 512U,
		S_IRUSR = 256U,
		S_IWUSR = 128U,
		S_IXUSR = 64U,
		S_IRGRP = 32U,
		S_IWGRP = 16U,
		S_IXGRP = 8U,
		S_IROTH = 4U,
		S_IWOTH = 2U,
		S_IXOTH = 1U,
		S_IRWXG = 56U,
		S_IRWXU = 448U,
		S_IRWXO = 7U,
		ACCESSPERMS = 511U,
		ALLPERMS = 4095U,
		DEFFILEMODE = 438U,
		S_IFMT = 61440U,
		[Map(SuppressFlags = "S_IFMT")]
		S_IFDIR = 16384U,
		[Map(SuppressFlags = "S_IFMT")]
		S_IFCHR = 8192U,
		[Map(SuppressFlags = "S_IFMT")]
		S_IFBLK = 24576U,
		[Map(SuppressFlags = "S_IFMT")]
		S_IFREG = 32768U,
		[Map(SuppressFlags = "S_IFMT")]
		S_IFIFO = 4096U,
		[Map(SuppressFlags = "S_IFMT")]
		S_IFLNK = 40960U,
		[Map(SuppressFlags = "S_IFMT")]
		S_IFSOCK = 49152U
	}
}
