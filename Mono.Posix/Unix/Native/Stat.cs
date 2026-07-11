using System;

namespace Mono.Unix.Native
{
	public struct Stat : IEquatable<Stat>
	{
		public Timespec st_atim
		{
			get
			{
				return new Timespec
				{
					tv_sec = this.st_atime,
					tv_nsec = this.st_atime_nsec
				};
			}
			set
			{
				this.st_atime = value.tv_sec;
				this.st_atime_nsec = value.tv_nsec;
			}
		}

		public Timespec st_mtim
		{
			get
			{
				return new Timespec
				{
					tv_sec = this.st_mtime,
					tv_nsec = this.st_mtime_nsec
				};
			}
			set
			{
				this.st_mtime = value.tv_sec;
				this.st_mtime_nsec = value.tv_nsec;
			}
		}

		public Timespec st_ctim
		{
			get
			{
				return new Timespec
				{
					tv_sec = this.st_ctime,
					tv_nsec = this.st_ctime_nsec
				};
			}
			set
			{
				this.st_ctime = value.tv_sec;
				this.st_ctime_nsec = value.tv_nsec;
			}
		}

		public override int GetHashCode()
		{
			return this.st_dev.GetHashCode() ^ this.st_ino.GetHashCode() ^ this.st_mode.GetHashCode() ^ this.st_nlink.GetHashCode() ^ this.st_uid.GetHashCode() ^ this.st_gid.GetHashCode() ^ this.st_rdev.GetHashCode() ^ this.st_size.GetHashCode() ^ this.st_blksize.GetHashCode() ^ this.st_blocks.GetHashCode() ^ this.st_atime.GetHashCode() ^ this.st_mtime.GetHashCode() ^ this.st_ctime.GetHashCode() ^ this.st_atime_nsec.GetHashCode() ^ this.st_mtime_nsec.GetHashCode() ^ this.st_ctime_nsec.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != base.GetType())
			{
				return false;
			}
			Stat stat = (Stat)obj;
			return stat.st_dev == this.st_dev && stat.st_ino == this.st_ino && stat.st_mode == this.st_mode && stat.st_nlink == this.st_nlink && stat.st_uid == this.st_uid && stat.st_gid == this.st_gid && stat.st_rdev == this.st_rdev && stat.st_size == this.st_size && stat.st_blksize == this.st_blksize && stat.st_blocks == this.st_blocks && stat.st_atime == this.st_atime && stat.st_mtime == this.st_mtime && stat.st_ctime == this.st_ctime && stat.st_atime_nsec == this.st_atime_nsec && stat.st_mtime_nsec == this.st_mtime_nsec && stat.st_ctime_nsec == this.st_ctime_nsec;
		}

		public bool Equals(Stat value)
		{
			return value.st_dev == this.st_dev && value.st_ino == this.st_ino && value.st_mode == this.st_mode && value.st_nlink == this.st_nlink && value.st_uid == this.st_uid && value.st_gid == this.st_gid && value.st_rdev == this.st_rdev && value.st_size == this.st_size && value.st_blksize == this.st_blksize && value.st_blocks == this.st_blocks && value.st_atime == this.st_atime && value.st_mtime == this.st_mtime && value.st_ctime == this.st_ctime && value.st_atime_nsec == this.st_atime_nsec && value.st_mtime_nsec == this.st_mtime_nsec && value.st_ctime_nsec == this.st_ctime_nsec;
		}

		public static bool operator ==(Stat lhs, Stat rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Stat lhs, Stat rhs)
		{
			return !lhs.Equals(rhs);
		}

		[CLSCompliant(false)]
		[dev_t]
		public ulong st_dev;

		[CLSCompliant(false)]
		[ino_t]
		public ulong st_ino;

		[CLSCompliant(false)]
		public FilePermissions st_mode;

		[NonSerialized]
		private uint _padding_;

		[CLSCompliant(false)]
		[nlink_t]
		public ulong st_nlink;

		[CLSCompliant(false)]
		[uid_t]
		public uint st_uid;

		[CLSCompliant(false)]
		[gid_t]
		public uint st_gid;

		[CLSCompliant(false)]
		[dev_t]
		public ulong st_rdev;

		[off_t]
		public long st_size;

		[blksize_t]
		public long st_blksize;

		[blkcnt_t]
		public long st_blocks;

		[time_t]
		public long st_atime;

		[time_t]
		public long st_mtime;

		[time_t]
		public long st_ctime;

		public long st_atime_nsec;

		public long st_mtime_nsec;

		public long st_ctime_nsec;
	}
}
