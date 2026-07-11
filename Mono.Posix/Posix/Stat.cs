using System;

namespace Mono.Posix
{
	[Obsolete("Use Mono.Unix.Native.Stat")]
	public struct Stat
	{
		[Obsolete("Use Mono.Unix.Native.NativeConvert.ToDateTime")]
		public static DateTime UnixToDateTime(long unix)
		{
			return Stat.UnixEpoch.Add(TimeSpan.FromSeconds((double)unix)).ToLocalTime();
		}

		internal Stat(int device, int inode, int mode, int nlinks, int uid, int gid, int rdev, long size, long blksize, long blocks, long atime, long mtime, long ctime)
		{
			this.Device = device;
			this.INode = inode;
			this.Mode = (StatMode)mode;
			this.NLinks = nlinks;
			this.Uid = uid;
			this.Gid = gid;
			this.DeviceType = (long)rdev;
			this.Size = size;
			this.BlockSize = blksize;
			this.Blocks = blocks;
			if (atime != 0L)
			{
				this.ATime = Stat.UnixToDateTime(atime);
			}
			else
			{
				this.ATime = default(DateTime);
			}
			if (mtime != 0L)
			{
				this.MTime = Stat.UnixToDateTime(mtime);
			}
			else
			{
				this.MTime = default(DateTime);
			}
			if (ctime != 0L)
			{
				this.CTime = Stat.UnixToDateTime(ctime);
				return;
			}
			this.CTime = default(DateTime);
		}

		[Obsolete("Use Mono.Unix.Native.Stat.st_dev")]
		public readonly int Device;

		[Obsolete("Use Mono.Unix.Native.Stat.st_ino")]
		public readonly int INode;

		[Obsolete("Use Mono.Unix.Native.Stat.st_mode")]
		public readonly StatMode Mode;

		[Obsolete("Use Mono.Unix.Native.Stat.st_nlink")]
		public readonly int NLinks;

		[Obsolete("Use Mono.Unix.Native.Stat.st_uid")]
		public readonly int Uid;

		[Obsolete("Use Mono.Unix.Native.Stat.st_gid")]
		public readonly int Gid;

		[Obsolete("Use Mono.Unix.Native.Stat.st_rdev")]
		public readonly long DeviceType;

		[Obsolete("Use Mono.Unix.Native.Stat.st_size")]
		public readonly long Size;

		[Obsolete("Use Mono.Unix.Native.Stat.st_blksize")]
		public readonly long BlockSize;

		[Obsolete("Use Mono.Unix.Native.Stat.st_blocks")]
		public readonly long Blocks;

		[Obsolete("Use Mono.Unix.Native.Stat.st_atime")]
		public readonly DateTime ATime;

		[Obsolete("Use Mono.Unix.Native.Stat.st_mtime")]
		public readonly DateTime MTime;

		[Obsolete("Use Mono.Unix.Native.Stat.st_ctime")]
		public readonly DateTime CTime;

		[Obsolete("Use Mono.Unix.Native.NativeConvert.LocalUnixEpoch")]
		public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);
	}
}
