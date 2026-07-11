using System;
using System.Collections;
using System.IO;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixDriveInfo
	{
		public UnixDriveInfo(string mountPoint)
		{
			if (mountPoint == null)
			{
				throw new ArgumentNullException("mountPoint");
			}
			Fstab fstab = Syscall.getfsfile(mountPoint);
			if (fstab != null)
			{
				this.FromFstab(fstab);
				return;
			}
			this.mount_point = mountPoint;
			this.block_device = "";
			this.fstype = "Unknown";
		}

		private void FromFstab(Fstab fstab)
		{
			this.fstype = fstab.fs_vfstype;
			this.mount_point = fstab.fs_file;
			this.block_device = fstab.fs_spec;
		}

		public static UnixDriveInfo GetForSpecialFile(string specialFile)
		{
			if (specialFile == null)
			{
				throw new ArgumentNullException("specialFile");
			}
			Fstab fstab = Syscall.getfsspec(specialFile);
			if (fstab == null)
			{
				throw new ArgumentException("specialFile isn't valid: " + specialFile);
			}
			return new UnixDriveInfo(fstab);
		}

		private UnixDriveInfo(Fstab fstab)
		{
			this.FromFstab(fstab);
		}

		public long AvailableFreeSpace
		{
			get
			{
				this.Refresh();
				return Convert.ToInt64(this.stat.f_bavail * this.stat.f_frsize);
			}
		}

		public string DriveFormat
		{
			get
			{
				return this.fstype;
			}
		}

		public UnixDriveType DriveType
		{
			get
			{
				return UnixDriveType.Unknown;
			}
		}

		public bool IsReady
		{
			get
			{
				bool flag = this.Refresh(false);
				if (this.mount_point == "/" || !flag)
				{
					return flag;
				}
				Statvfs statvfs;
				return Syscall.statvfs(this.RootDirectory.Parent.FullName, out statvfs) == 0 && statvfs.f_fsid != this.stat.f_fsid;
			}
		}

		public string Name
		{
			get
			{
				return this.mount_point;
			}
		}

		public UnixDirectoryInfo RootDirectory
		{
			get
			{
				return new UnixDirectoryInfo(this.mount_point);
			}
		}

		public long TotalFreeSpace
		{
			get
			{
				this.Refresh();
				return (long)(this.stat.f_bfree * this.stat.f_frsize);
			}
		}

		public long TotalSize
		{
			get
			{
				this.Refresh();
				return (long)(this.stat.f_frsize * this.stat.f_blocks);
			}
		}

		public string VolumeLabel
		{
			get
			{
				return this.block_device;
			}
		}

		public long MaximumFilenameLength
		{
			get
			{
				this.Refresh();
				return Convert.ToInt64(this.stat.f_namemax);
			}
		}

		public static UnixDriveInfo[] GetDrives()
		{
			ArrayList arrayList = new ArrayList();
			object fstab_lock = Syscall.fstab_lock;
			lock (fstab_lock)
			{
				if (Syscall.setfsent() != 1)
				{
					throw new IOException("Error calling setfsent(3)", new UnixIOException());
				}
				try
				{
					Fstab fstab;
					while ((fstab = Syscall.getfsent()) != null)
					{
						if (fstab.fs_file != null && fstab.fs_file.StartsWith("/"))
						{
							arrayList.Add(new UnixDriveInfo(fstab));
						}
					}
				}
				finally
				{
					Syscall.endfsent();
				}
			}
			return (UnixDriveInfo[])arrayList.ToArray(typeof(UnixDriveInfo));
		}

		public override string ToString()
		{
			return this.VolumeLabel;
		}

		private void Refresh()
		{
			this.Refresh(true);
		}

		private bool Refresh(bool throwException)
		{
			int num = Syscall.statvfs(this.mount_point, out this.stat);
			if (num == -1 && throwException)
			{
				Errno lastError = Stdlib.GetLastError();
				throw new InvalidOperationException(UnixMarshal.GetErrorDescription(lastError), new UnixIOException(lastError));
			}
			return num != -1;
		}

		private Statvfs stat;

		private string fstype;

		private string mount_point;

		private string block_device;
	}
}
