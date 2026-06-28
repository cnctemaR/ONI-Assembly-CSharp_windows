using System;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public abstract class UnixFileSystemInfo
	{
		protected UnixFileSystemInfo(string path)
		{
			UnixPath.CheckPath(path);
			this.originalPath = path;
			this.fullPath = UnixPath.GetFullPath(path);
			this.Refresh(true);
		}

		internal UnixFileSystemInfo(string path, Stat stat)
		{
			this.originalPath = path;
			this.fullPath = UnixPath.GetFullPath(path);
			this.stat = stat;
			this.valid = true;
		}

		protected string FullPath
		{
			get
			{
				return this.fullPath;
			}
			set
			{
				if (this.fullPath != value)
				{
					UnixPath.CheckPath(value);
					this.valid = false;
					this.fullPath = value;
				}
			}
		}

		protected string OriginalPath
		{
			get
			{
				return this.originalPath;
			}
			set
			{
				this.originalPath = value;
			}
		}

		private void AssertValid()
		{
			this.Refresh(false);
			if (!this.valid)
			{
				throw new InvalidOperationException("Path doesn't exist!");
			}
		}

		public virtual string FullName
		{
			get
			{
				return this.FullPath;
			}
		}

		public abstract string Name { get; }

		public bool Exists
		{
			get
			{
				this.Refresh(true);
				return this.valid;
			}
		}

		public long Device
		{
			get
			{
				this.AssertValid();
				return Convert.ToInt64(this.stat.st_dev);
			}
		}

		public long Inode
		{
			get
			{
				this.AssertValid();
				return Convert.ToInt64(this.stat.st_ino);
			}
		}

		[CLSCompliant(false)]
		public FilePermissions Protection
		{
			get
			{
				this.AssertValid();
				return this.stat.st_mode;
			}
			set
			{
				int num = Syscall.chmod(this.FullPath, value);
				UnixMarshal.ThrowExceptionForLastErrorIf(num);
			}
		}

		public FileTypes FileType
		{
			get
			{
				this.AssertValid();
				return (FileTypes)(this.stat.st_mode & FilePermissions.S_IFMT);
			}
		}

		public FileAccessPermissions FileAccessPermissions
		{
			get
			{
				this.AssertValid();
				int st_mode = (int)this.stat.st_mode;
				return (FileAccessPermissions)(st_mode & 511);
			}
			set
			{
				this.AssertValid();
				int num = (int)this.stat.st_mode;
				num &= -512;
				num |= (int)value;
				this.Protection = (FilePermissions)num;
			}
		}

		public FileSpecialAttributes FileSpecialAttributes
		{
			get
			{
				this.AssertValid();
				int st_mode = (int)this.stat.st_mode;
				return (FileSpecialAttributes)(st_mode & 3584);
			}
			set
			{
				this.AssertValid();
				int num = (int)this.stat.st_mode;
				num &= -3585;
				num |= (int)value;
				this.Protection = (FilePermissions)num;
			}
		}

		public long LinkCount
		{
			get
			{
				this.AssertValid();
				return Convert.ToInt64(this.stat.st_nlink);
			}
		}

		public UnixUserInfo OwnerUser
		{
			get
			{
				this.AssertValid();
				return new UnixUserInfo(this.stat.st_uid);
			}
		}

		public long OwnerUserId
		{
			get
			{
				this.AssertValid();
				return (long)((ulong)this.stat.st_uid);
			}
		}

		public UnixGroupInfo OwnerGroup
		{
			get
			{
				this.AssertValid();
				return new UnixGroupInfo((long)((ulong)this.stat.st_gid));
			}
		}

		public long OwnerGroupId
		{
			get
			{
				this.AssertValid();
				return (long)((ulong)this.stat.st_gid);
			}
		}

		public long DeviceType
		{
			get
			{
				this.AssertValid();
				return Convert.ToInt64(this.stat.st_rdev);
			}
		}

		public long Length
		{
			get
			{
				this.AssertValid();
				return this.stat.st_size;
			}
		}

		public long BlockSize
		{
			get
			{
				this.AssertValid();
				return this.stat.st_blksize;
			}
		}

		public long BlocksAllocated
		{
			get
			{
				this.AssertValid();
				return this.stat.st_blocks;
			}
		}

		public DateTime LastAccessTime
		{
			get
			{
				this.AssertValid();
				return NativeConvert.ToDateTime(this.stat.st_atime);
			}
		}

		public DateTime LastAccessTimeUtc
		{
			get
			{
				return this.LastAccessTime.ToUniversalTime();
			}
		}

		public DateTime LastWriteTime
		{
			get
			{
				this.AssertValid();
				return NativeConvert.ToDateTime(this.stat.st_mtime);
			}
		}

		public DateTime LastWriteTimeUtc
		{
			get
			{
				return this.LastWriteTime.ToUniversalTime();
			}
		}

		public DateTime LastStatusChangeTime
		{
			get
			{
				this.AssertValid();
				return NativeConvert.ToDateTime(this.stat.st_ctime);
			}
		}

		public DateTime LastStatusChangeTimeUtc
		{
			get
			{
				return this.LastStatusChangeTime.ToUniversalTime();
			}
		}

		public bool IsDirectory
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsFileType(this.stat.st_mode, FilePermissions.S_IFDIR);
			}
		}

		public bool IsCharacterDevice
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsFileType(this.stat.st_mode, FilePermissions.S_IFCHR);
			}
		}

		public bool IsBlockDevice
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsFileType(this.stat.st_mode, FilePermissions.S_IFBLK);
			}
		}

		public bool IsRegularFile
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsFileType(this.stat.st_mode, FilePermissions.S_IFREG);
			}
		}

		public bool IsFifo
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsFileType(this.stat.st_mode, FilePermissions.S_IFIFO);
			}
		}

		public bool IsSymbolicLink
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsFileType(this.stat.st_mode, FilePermissions.S_IFLNK);
			}
		}

		public bool IsSocket
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsFileType(this.stat.st_mode, FilePermissions.S_IFSOCK);
			}
		}

		public bool IsSetUser
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsSet(this.stat.st_mode, FilePermissions.S_ISUID);
			}
		}

		public bool IsSetGroup
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsSet(this.stat.st_mode, FilePermissions.S_ISGID);
			}
		}

		public bool IsSticky
		{
			get
			{
				this.AssertValid();
				return UnixFileSystemInfo.IsSet(this.stat.st_mode, FilePermissions.S_ISVTX);
			}
		}

		internal static bool IsFileType(FilePermissions mode, FilePermissions type)
		{
			return (mode & FilePermissions.S_IFMT) == type;
		}

		internal static bool IsSet(FilePermissions mode, FilePermissions type)
		{
			return (mode & type) == type;
		}

		[CLSCompliant(false)]
		public bool CanAccess(AccessModes mode)
		{
			int num = Syscall.access(this.FullPath, mode);
			return num == 0;
		}

		public UnixFileSystemInfo CreateLink(string path)
		{
			int num = Syscall.link(this.FullName, path);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			return UnixFileSystemInfo.GetFileSystemEntry(path);
		}

		public UnixSymbolicLinkInfo CreateSymbolicLink(string path)
		{
			int num = Syscall.symlink(this.FullName, path);
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			return new UnixSymbolicLinkInfo(path);
		}

		public abstract void Delete();

		[CLSCompliant(false)]
		public long GetConfigurationValue(PathconfName name)
		{
			long num = Syscall.pathconf(this.FullPath, name);
			if (num == -1L && Stdlib.GetLastError() != (Errno)0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return num;
		}

		public void Refresh()
		{
			this.Refresh(true);
		}

		internal void Refresh(bool force)
		{
			if (this.valid && !force)
			{
				return;
			}
			this.valid = this.GetFileStatus(this.FullPath, out this.stat);
		}

		protected virtual bool GetFileStatus(string path, out Stat stat)
		{
			return Syscall.stat(path, out stat) == 0;
		}

		public void SetLength(long length)
		{
			int num;
			do
			{
				num = Syscall.truncate(this.FullPath, length);
			}
			while (UnixMarshal.ShouldRetrySyscall(num));
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
		}

		public virtual void SetOwner(long owner, long group)
		{
			uint num = Convert.ToUInt32(owner);
			uint num2 = Convert.ToUInt32(group);
			int num3 = Syscall.chown(this.FullPath, num, num2);
			UnixMarshal.ThrowExceptionForLastErrorIf(num3);
		}

		public void SetOwner(string owner)
		{
			Passwd passwd = Syscall.getpwnam(owner);
			if (passwd == null)
			{
				throw new ArgumentException(Locale.GetText("invalid username"), "owner");
			}
			uint pw_uid = passwd.pw_uid;
			uint pw_gid = passwd.pw_gid;
			this.SetOwner((long)((ulong)pw_uid), (long)((ulong)pw_gid));
		}

		public void SetOwner(string owner, string group)
		{
			long num = -1L;
			if (owner != null)
			{
				num = new UnixUserInfo(owner).UserId;
			}
			long num2 = -1L;
			if (group != null)
			{
				num2 = new UnixGroupInfo(group).GroupId;
			}
			this.SetOwner(num, num2);
		}

		public void SetOwner(UnixUserInfo owner)
		{
			long num2;
			long num = (num2 = -1L);
			if (owner != null)
			{
				num2 = owner.UserId;
				num = owner.GroupId;
			}
			this.SetOwner(num2, num);
		}

		public void SetOwner(UnixUserInfo owner, UnixGroupInfo group)
		{
			long num2;
			long num = (num2 = -1L);
			if (owner != null)
			{
				num2 = owner.UserId;
			}
			if (group != null)
			{
				num = owner.GroupId;
			}
			this.SetOwner(num2, num);
		}

		public override string ToString()
		{
			return this.FullPath;
		}

		public Stat ToStat()
		{
			this.AssertValid();
			return this.stat;
		}

		public static UnixFileSystemInfo GetFileSystemEntry(string path)
		{
			Stat stat;
			int num = Syscall.lstat(path, out stat);
			if (num == -1 && Stdlib.GetLastError() == Errno.ENOENT)
			{
				return new UnixFileInfo(path);
			}
			UnixMarshal.ThrowExceptionForLastErrorIf(num);
			if (UnixFileSystemInfo.IsFileType(stat.st_mode, FilePermissions.S_IFDIR))
			{
				return new UnixDirectoryInfo(path, stat);
			}
			if (UnixFileSystemInfo.IsFileType(stat.st_mode, FilePermissions.S_IFLNK))
			{
				return new UnixSymbolicLinkInfo(path, stat);
			}
			return new UnixFileInfo(path, stat);
		}

		internal const FileSpecialAttributes AllSpecialAttributes = FileSpecialAttributes.SetUserId | FileSpecialAttributes.SetGroupId | FileSpecialAttributes.Sticky;

		internal const FileTypes AllFileTypes = (FileTypes)61440;

		private Stat stat;

		private string fullPath;

		private string originalPath;

		private bool valid;
	}
}
