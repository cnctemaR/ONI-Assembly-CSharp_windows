using System;
using System.IO;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixFileInfo : UnixFileSystemInfo
	{
		public UnixFileInfo(string path)
			: base(path)
		{
		}

		internal UnixFileInfo(string path, Stat stat)
			: base(path, stat)
		{
		}

		public override string Name
		{
			get
			{
				return UnixPath.GetFileName(base.FullPath);
			}
		}

		public string DirectoryName
		{
			get
			{
				return UnixPath.GetDirectoryName(base.FullPath);
			}
		}

		public UnixDirectoryInfo Directory
		{
			get
			{
				return new UnixDirectoryInfo(this.DirectoryName);
			}
		}

		public override void Delete()
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.unlink(base.FullPath));
			base.Refresh();
		}

		public UnixStream Create()
		{
			FilePermissions filePermissions = FilePermissions.S_IRUSR | FilePermissions.S_IWUSR | FilePermissions.S_IRGRP | FilePermissions.S_IROTH;
			return this.Create(filePermissions);
		}

		[CLSCompliant(false)]
		public UnixStream Create(FilePermissions mode)
		{
			int num = Syscall.creat(base.FullPath, mode);
			if (num < 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			base.Refresh();
			return new UnixStream(num);
		}

		public UnixStream Create(FileAccessPermissions mode)
		{
			return this.Create((FilePermissions)mode);
		}

		[CLSCompliant(false)]
		public UnixStream Open(OpenFlags flags)
		{
			if ((flags & OpenFlags.O_CREAT) != OpenFlags.O_RDONLY)
			{
				throw new ArgumentException("Cannot specify OpenFlags.O_CREAT without providing FilePermissions.  Use the Open(OpenFlags, FilePermissions) method instead");
			}
			int num = Syscall.open(base.FullPath, flags);
			if (num < 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return new UnixStream(num);
		}

		[CLSCompliant(false)]
		public UnixStream Open(OpenFlags flags, FilePermissions mode)
		{
			int num = Syscall.open(base.FullPath, flags, mode);
			if (num < 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return new UnixStream(num);
		}

		public UnixStream Open(FileMode mode)
		{
			OpenFlags openFlags = NativeConvert.ToOpenFlags(mode, FileAccess.ReadWrite);
			return this.Open(openFlags);
		}

		public UnixStream Open(FileMode mode, FileAccess access)
		{
			OpenFlags openFlags = NativeConvert.ToOpenFlags(mode, access);
			return this.Open(openFlags);
		}

		[CLSCompliant(false)]
		public UnixStream Open(FileMode mode, FileAccess access, FilePermissions perms)
		{
			OpenFlags openFlags = NativeConvert.ToOpenFlags(mode, access);
			int num = Syscall.open(base.FullPath, openFlags, perms);
			if (num < 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return new UnixStream(num);
		}

		public UnixStream OpenRead()
		{
			return this.Open(FileMode.Open, FileAccess.Read);
		}

		public UnixStream OpenWrite()
		{
			return this.Open(FileMode.OpenOrCreate, FileAccess.Write);
		}
	}
}
