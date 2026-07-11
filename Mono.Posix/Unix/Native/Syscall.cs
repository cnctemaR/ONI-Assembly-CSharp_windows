using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	public sealed class Syscall : Stdlib
	{
		private Syscall()
		{
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setxattr", SetLastError = true)]
		public static extern int setxattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string name, byte[] value, ulong size, XattrFlags flags);

		public static int setxattr(string path, string name, byte[] value, ulong size)
		{
			return Syscall.setxattr(path, name, value, size, XattrFlags.XATTR_AUTO);
		}

		public static int setxattr(string path, string name, byte[] value, XattrFlags flags)
		{
			return Syscall.setxattr(path, name, value, (ulong)((long)value.Length), flags);
		}

		public static int setxattr(string path, string name, byte[] value)
		{
			return Syscall.setxattr(path, name, value, (ulong)((long)value.Length));
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_lsetxattr", SetLastError = true)]
		public static extern int lsetxattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string name, byte[] value, ulong size, XattrFlags flags);

		public static int lsetxattr(string path, string name, byte[] value, ulong size)
		{
			return Syscall.lsetxattr(path, name, value, size, XattrFlags.XATTR_AUTO);
		}

		public static int lsetxattr(string path, string name, byte[] value, XattrFlags flags)
		{
			return Syscall.lsetxattr(path, name, value, (ulong)((long)value.Length), flags);
		}

		public static int lsetxattr(string path, string name, byte[] value)
		{
			return Syscall.lsetxattr(path, name, value, (ulong)((long)value.Length));
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fsetxattr", SetLastError = true)]
		public static extern int fsetxattr(int fd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string name, byte[] value, ulong size, XattrFlags flags);

		public static int fsetxattr(int fd, string name, byte[] value, ulong size)
		{
			return Syscall.fsetxattr(fd, name, value, size, XattrFlags.XATTR_AUTO);
		}

		public static int fsetxattr(int fd, string name, byte[] value, XattrFlags flags)
		{
			return Syscall.fsetxattr(fd, name, value, (ulong)((long)value.Length), flags);
		}

		public static int fsetxattr(int fd, string name, byte[] value)
		{
			return Syscall.fsetxattr(fd, name, value, (ulong)((long)value.Length));
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getxattr", SetLastError = true)]
		public static extern long getxattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string name, byte[] value, ulong size);

		public static long getxattr(string path, string name, byte[] value)
		{
			return Syscall.getxattr(path, name, value, (ulong)((long)value.Length));
		}

		public static long getxattr(string path, string name, out byte[] value)
		{
			value = null;
			long num = Syscall.getxattr(path, name, value, 0UL);
			if (num <= 0L)
			{
				return num;
			}
			value = new byte[num];
			return Syscall.getxattr(path, name, value, (ulong)num);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_lgetxattr", SetLastError = true)]
		public static extern long lgetxattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string name, byte[] value, ulong size);

		public static long lgetxattr(string path, string name, byte[] value)
		{
			return Syscall.lgetxattr(path, name, value, (ulong)((long)value.Length));
		}

		public static long lgetxattr(string path, string name, out byte[] value)
		{
			value = null;
			long num = Syscall.lgetxattr(path, name, value, 0UL);
			if (num <= 0L)
			{
				return num;
			}
			value = new byte[num];
			return Syscall.lgetxattr(path, name, value, (ulong)num);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fgetxattr", SetLastError = true)]
		public static extern long fgetxattr(int fd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string name, byte[] value, ulong size);

		public static long fgetxattr(int fd, string name, byte[] value)
		{
			return Syscall.fgetxattr(fd, name, value, (ulong)((long)value.Length));
		}

		public static long fgetxattr(int fd, string name, out byte[] value)
		{
			value = null;
			long num = Syscall.fgetxattr(fd, name, value, 0UL);
			if (num <= 0L)
			{
				return num;
			}
			value = new byte[num];
			return Syscall.fgetxattr(fd, name, value, (ulong)num);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_listxattr", SetLastError = true)]
		public static extern long listxattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, byte[] list, ulong size);

		public static long listxattr(string path, Encoding encoding, out string[] values)
		{
			values = null;
			long num = Syscall.listxattr(path, null, 0UL);
			if (num == 0L)
			{
				values = new string[0];
			}
			if (num <= 0L)
			{
				return (long)((int)num);
			}
			byte[] array = new byte[num];
			long num2 = Syscall.listxattr(path, array, (ulong)num);
			if (num2 < 0L)
			{
				return (long)((int)num2);
			}
			Syscall.GetValues(array, encoding, out values);
			return 0L;
		}

		public static long listxattr(string path, out string[] values)
		{
			return Syscall.listxattr(path, UnixEncoding.Instance, out values);
		}

		private static void GetValues(byte[] list, Encoding encoding, out string[] values)
		{
			int num = 0;
			for (int i = 0; i < list.Length; i++)
			{
				if (list[i] == 0)
				{
					num++;
				}
			}
			values = new string[num];
			num = 0;
			int num2 = 0;
			for (int j = 0; j < list.Length; j++)
			{
				if (list[j] == 0)
				{
					values[num++] = encoding.GetString(list, num2, j - num2);
					num2 = j + 1;
				}
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_llistxattr", SetLastError = true)]
		public static extern long llistxattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, byte[] list, ulong size);

		public static long llistxattr(string path, Encoding encoding, out string[] values)
		{
			values = null;
			long num = Syscall.llistxattr(path, null, 0UL);
			if (num == 0L)
			{
				values = new string[0];
			}
			if (num <= 0L)
			{
				return (long)((int)num);
			}
			byte[] array = new byte[num];
			long num2 = Syscall.llistxattr(path, array, (ulong)num);
			if (num2 < 0L)
			{
				return (long)((int)num2);
			}
			Syscall.GetValues(array, encoding, out values);
			return 0L;
		}

		public static long llistxattr(string path, out string[] values)
		{
			return Syscall.llistxattr(path, UnixEncoding.Instance, out values);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_flistxattr", SetLastError = true)]
		public static extern long flistxattr(int fd, byte[] list, ulong size);

		public static long flistxattr(int fd, Encoding encoding, out string[] values)
		{
			values = null;
			long num = Syscall.flistxattr(fd, null, 0UL);
			if (num == 0L)
			{
				values = new string[0];
			}
			if (num <= 0L)
			{
				return (long)((int)num);
			}
			byte[] array = new byte[num];
			long num2 = Syscall.flistxattr(fd, array, (ulong)num);
			if (num2 < 0L)
			{
				return (long)((int)num2);
			}
			Syscall.GetValues(array, encoding, out values);
			return 0L;
		}

		public static long flistxattr(int fd, out string[] values)
		{
			return Syscall.flistxattr(fd, UnixEncoding.Instance, out values);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_removexattr", SetLastError = true)]
		public static extern int removexattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string name);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_lremovexattr", SetLastError = true)]
		public static extern int lremovexattr([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string name);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fremovexattr", SetLastError = true)]
		public static extern int fremovexattr(int fd, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string name);

		[DllImport("libc", SetLastError = true)]
		public static extern IntPtr opendir([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string name);

		[DllImport("libc", SetLastError = true)]
		public static extern int closedir(IntPtr dir);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_seekdir", SetLastError = true)]
		public static extern int seekdir(IntPtr dir, long offset);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_telldir", SetLastError = true)]
		public static extern long telldir(IntPtr dir);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_rewinddir", SetLastError = true)]
		public static extern int rewinddir(IntPtr dir);

		private static void CopyDirent(Dirent to, ref Syscall._Dirent from)
		{
			try
			{
				to.d_ino = from.d_ino;
				to.d_off = from.d_off;
				to.d_reclen = from.d_reclen;
				to.d_type = from.d_type;
				to.d_name = UnixMarshal.PtrToString(from.d_name);
			}
			finally
			{
				Stdlib.free(from.d_name);
				from.d_name = IntPtr.Zero;
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_readdir", SetLastError = true)]
		private static extern int sys_readdir(IntPtr dir, out Syscall._Dirent dentry);

		public static Dirent readdir(IntPtr dir)
		{
			object obj = Syscall.readdir_lock;
			Syscall._Dirent dirent;
			int num;
			lock (obj)
			{
				num = Syscall.sys_readdir(dir, out dirent);
			}
			if (num != 0)
			{
				return null;
			}
			Dirent dirent2 = new Dirent();
			Syscall.CopyDirent(dirent2, ref dirent);
			return dirent2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_readdir_r", SetLastError = true)]
		private static extern int sys_readdir_r(IntPtr dirp, out Syscall._Dirent entry, out IntPtr result);

		public static int readdir_r(IntPtr dirp, Dirent entry, out IntPtr result)
		{
			entry.d_ino = 0UL;
			entry.d_off = 0L;
			entry.d_reclen = 0;
			entry.d_type = 0;
			entry.d_name = null;
			Syscall._Dirent dirent;
			int num = Syscall.sys_readdir_r(dirp, out dirent, out result);
			if (num == 0 && result != IntPtr.Zero)
			{
				Syscall.CopyDirent(entry, ref dirent);
			}
			return num;
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int dirfd(IntPtr dir);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fcntl", SetLastError = true)]
		public static extern int fcntl(int fd, FcntlCommand cmd);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fcntl_arg", SetLastError = true)]
		public static extern int fcntl(int fd, FcntlCommand cmd, long arg);

		public static int fcntl(int fd, FcntlCommand cmd, DirectoryNotifyFlags arg)
		{
			if (cmd != FcntlCommand.F_NOTIFY)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				return -1;
			}
			long num = (long)NativeConvert.FromDirectoryNotifyFlags(arg);
			return Syscall.fcntl(fd, FcntlCommand.F_NOTIFY, num);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fcntl_lock", SetLastError = true)]
		public static extern int fcntl(int fd, FcntlCommand cmd, ref Flock @lock);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_open", SetLastError = true)]
		public static extern int open([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string pathname, OpenFlags flags);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_open_mode", SetLastError = true)]
		public static extern int open([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string pathname, OpenFlags flags, FilePermissions mode);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_creat", SetLastError = true)]
		public static extern int creat([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string pathname, FilePermissions mode);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_posix_fadvise", SetLastError = true)]
		public static extern int posix_fadvise(int fd, long offset, long len, PosixFadviseAdvice advice);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_posix_fallocate", SetLastError = true)]
		public static extern int posix_fallocate(int fd, long offset, ulong len);

		private static void CopyFstab(Fstab to, ref Syscall._Fstab from)
		{
			try
			{
				to.fs_spec = UnixMarshal.PtrToString(from.fs_spec);
				to.fs_file = UnixMarshal.PtrToString(from.fs_file);
				to.fs_vfstype = UnixMarshal.PtrToString(from.fs_vfstype);
				to.fs_mntops = UnixMarshal.PtrToString(from.fs_mntops);
				to.fs_type = UnixMarshal.PtrToString(from.fs_type);
				to.fs_freq = from.fs_freq;
				to.fs_passno = from.fs_passno;
			}
			finally
			{
				Stdlib.free(from._fs_buf_);
				from._fs_buf_ = IntPtr.Zero;
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_endfsent", SetLastError = true)]
		private static extern int sys_endfsent();

		public static int endfsent()
		{
			object obj = Syscall.fstab_lock;
			int num;
			lock (obj)
			{
				num = Syscall.sys_endfsent();
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getfsent", SetLastError = true)]
		private static extern int sys_getfsent(out Syscall._Fstab fs);

		public static Fstab getfsent()
		{
			object obj = Syscall.fstab_lock;
			Syscall._Fstab fstab;
			int num;
			lock (obj)
			{
				num = Syscall.sys_getfsent(out fstab);
			}
			if (num != 0)
			{
				return null;
			}
			Fstab fstab2 = new Fstab();
			Syscall.CopyFstab(fstab2, ref fstab);
			return fstab2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getfsfile", SetLastError = true)]
		private static extern int sys_getfsfile([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string mount_point, out Syscall._Fstab fs);

		public static Fstab getfsfile(string mount_point)
		{
			object obj = Syscall.fstab_lock;
			Syscall._Fstab fstab;
			int num;
			lock (obj)
			{
				num = Syscall.sys_getfsfile(mount_point, out fstab);
			}
			if (num != 0)
			{
				return null;
			}
			Fstab fstab2 = new Fstab();
			Syscall.CopyFstab(fstab2, ref fstab);
			return fstab2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getfsspec", SetLastError = true)]
		private static extern int sys_getfsspec([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string special_file, out Syscall._Fstab fs);

		public static Fstab getfsspec(string special_file)
		{
			object obj = Syscall.fstab_lock;
			Syscall._Fstab fstab;
			int num;
			lock (obj)
			{
				num = Syscall.sys_getfsspec(special_file, out fstab);
			}
			if (num != 0)
			{
				return null;
			}
			Fstab fstab2 = new Fstab();
			Syscall.CopyFstab(fstab2, ref fstab);
			return fstab2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setfsent", SetLastError = true)]
		private static extern int sys_setfsent();

		public static int setfsent()
		{
			object obj = Syscall.fstab_lock;
			int num;
			lock (obj)
			{
				num = Syscall.sys_setfsent();
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setgroups", SetLastError = true)]
		public static extern int setgroups(ulong size, uint[] list);

		public static int setgroups(uint[] list)
		{
			return Syscall.setgroups((ulong)((long)list.Length), list);
		}

		private static void CopyGroup(Group to, ref Syscall._Group from)
		{
			try
			{
				to.gr_gid = from.gr_gid;
				to.gr_name = UnixMarshal.PtrToString(from.gr_name);
				to.gr_passwd = UnixMarshal.PtrToString(from.gr_passwd);
				to.gr_mem = UnixMarshal.PtrToStringArray(from._gr_nmem_, from.gr_mem);
			}
			finally
			{
				Stdlib.free(from.gr_mem);
				Stdlib.free(from._gr_buf_);
				from.gr_mem = IntPtr.Zero;
				from._gr_buf_ = IntPtr.Zero;
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getgrnam", SetLastError = true)]
		private static extern int sys_getgrnam(string name, out Syscall._Group group);

		public static Group getgrnam(string name)
		{
			object obj = Syscall.grp_lock;
			Syscall._Group group;
			int num;
			lock (obj)
			{
				num = Syscall.sys_getgrnam(name, out group);
			}
			if (num != 0)
			{
				return null;
			}
			Group group2 = new Group();
			Syscall.CopyGroup(group2, ref group);
			return group2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getgrgid", SetLastError = true)]
		private static extern int sys_getgrgid(uint uid, out Syscall._Group group);

		public static Group getgrgid(uint uid)
		{
			object obj = Syscall.grp_lock;
			Syscall._Group group;
			int num;
			lock (obj)
			{
				num = Syscall.sys_getgrgid(uid, out group);
			}
			if (num != 0)
			{
				return null;
			}
			Group group2 = new Group();
			Syscall.CopyGroup(group2, ref group);
			return group2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getgrnam_r", SetLastError = true)]
		private static extern int sys_getgrnam_r([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string name, out Syscall._Group grbuf, out IntPtr grbufp);

		public static int getgrnam_r(string name, Group grbuf, out Group grbufp)
		{
			grbufp = null;
			Syscall._Group group;
			IntPtr intPtr;
			int num = Syscall.sys_getgrnam_r(name, out group, out intPtr);
			if (num == 0 && intPtr != IntPtr.Zero)
			{
				Syscall.CopyGroup(grbuf, ref group);
				grbufp = grbuf;
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getgrgid_r", SetLastError = true)]
		private static extern int sys_getgrgid_r(uint uid, out Syscall._Group grbuf, out IntPtr grbufp);

		public static int getgrgid_r(uint uid, Group grbuf, out Group grbufp)
		{
			grbufp = null;
			Syscall._Group group;
			IntPtr intPtr;
			int num = Syscall.sys_getgrgid_r(uid, out group, out intPtr);
			if (num == 0 && intPtr != IntPtr.Zero)
			{
				Syscall.CopyGroup(grbuf, ref group);
				grbufp = grbuf;
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getgrent", SetLastError = true)]
		private static extern int sys_getgrent(out Syscall._Group grbuf);

		public static Group getgrent()
		{
			object obj = Syscall.grp_lock;
			Syscall._Group group;
			int num;
			lock (obj)
			{
				num = Syscall.sys_getgrent(out group);
			}
			if (num != 0)
			{
				return null;
			}
			Group group2 = new Group();
			Syscall.CopyGroup(group2, ref group);
			return group2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setgrent", SetLastError = true)]
		private static extern int sys_setgrent();

		public static int setgrent()
		{
			object obj = Syscall.grp_lock;
			int num;
			lock (obj)
			{
				num = Syscall.sys_setgrent();
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_endgrent", SetLastError = true)]
		private static extern int sys_endgrent();

		public static int endgrent()
		{
			object obj = Syscall.grp_lock;
			int num;
			lock (obj)
			{
				num = Syscall.sys_endgrent();
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fgetgrent", SetLastError = true)]
		private static extern int sys_fgetgrent(IntPtr stream, out Syscall._Group grbuf);

		public static Group fgetgrent(IntPtr stream)
		{
			object obj = Syscall.grp_lock;
			Syscall._Group group;
			int num;
			lock (obj)
			{
				num = Syscall.sys_fgetgrent(stream, out group);
			}
			if (num != 0)
			{
				return null;
			}
			Group group2 = new Group();
			Syscall.CopyGroup(group2, ref group);
			return group2;
		}

		private static void CopyPasswd(Passwd to, ref Syscall._Passwd from)
		{
			try
			{
				to.pw_name = UnixMarshal.PtrToString(from.pw_name);
				to.pw_passwd = UnixMarshal.PtrToString(from.pw_passwd);
				to.pw_uid = from.pw_uid;
				to.pw_gid = from.pw_gid;
				to.pw_gecos = UnixMarshal.PtrToString(from.pw_gecos);
				to.pw_dir = UnixMarshal.PtrToString(from.pw_dir);
				to.pw_shell = UnixMarshal.PtrToString(from.pw_shell);
			}
			finally
			{
				Stdlib.free(from._pw_buf_);
				from._pw_buf_ = IntPtr.Zero;
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getpwnam", SetLastError = true)]
		private static extern int sys_getpwnam(string name, out Syscall._Passwd passwd);

		public static Passwd getpwnam(string name)
		{
			object obj = Syscall.pwd_lock;
			Syscall._Passwd passwd;
			int num;
			lock (obj)
			{
				num = Syscall.sys_getpwnam(name, out passwd);
			}
			if (num != 0)
			{
				return null;
			}
			Passwd passwd2 = new Passwd();
			Syscall.CopyPasswd(passwd2, ref passwd);
			return passwd2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getpwuid", SetLastError = true)]
		private static extern int sys_getpwuid(uint uid, out Syscall._Passwd passwd);

		public static Passwd getpwuid(uint uid)
		{
			object obj = Syscall.pwd_lock;
			Syscall._Passwd passwd;
			int num;
			lock (obj)
			{
				num = Syscall.sys_getpwuid(uid, out passwd);
			}
			if (num != 0)
			{
				return null;
			}
			Passwd passwd2 = new Passwd();
			Syscall.CopyPasswd(passwd2, ref passwd);
			return passwd2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getpwnam_r", SetLastError = true)]
		private static extern int sys_getpwnam_r([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string name, out Syscall._Passwd pwbuf, out IntPtr pwbufp);

		public static int getpwnam_r(string name, Passwd pwbuf, out Passwd pwbufp)
		{
			pwbufp = null;
			Syscall._Passwd passwd;
			IntPtr intPtr;
			int num = Syscall.sys_getpwnam_r(name, out passwd, out intPtr);
			if (num == 0 && intPtr != IntPtr.Zero)
			{
				Syscall.CopyPasswd(pwbuf, ref passwd);
				pwbufp = pwbuf;
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getpwuid_r", SetLastError = true)]
		private static extern int sys_getpwuid_r(uint uid, out Syscall._Passwd pwbuf, out IntPtr pwbufp);

		public static int getpwuid_r(uint uid, Passwd pwbuf, out Passwd pwbufp)
		{
			pwbufp = null;
			Syscall._Passwd passwd;
			IntPtr intPtr;
			int num = Syscall.sys_getpwuid_r(uid, out passwd, out intPtr);
			if (num == 0 && intPtr != IntPtr.Zero)
			{
				Syscall.CopyPasswd(pwbuf, ref passwd);
				pwbufp = pwbuf;
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getpwent", SetLastError = true)]
		private static extern int sys_getpwent(out Syscall._Passwd pwbuf);

		public static Passwd getpwent()
		{
			object obj = Syscall.pwd_lock;
			Syscall._Passwd passwd;
			int num;
			lock (obj)
			{
				num = Syscall.sys_getpwent(out passwd);
			}
			if (num != 0)
			{
				return null;
			}
			Passwd passwd2 = new Passwd();
			Syscall.CopyPasswd(passwd2, ref passwd);
			return passwd2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setpwent", SetLastError = true)]
		private static extern int sys_setpwent();

		public static int setpwent()
		{
			object obj = Syscall.pwd_lock;
			int num;
			lock (obj)
			{
				num = Syscall.sys_setpwent();
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_endpwent", SetLastError = true)]
		private static extern int sys_endpwent();

		public static int endpwent()
		{
			object obj = Syscall.pwd_lock;
			int num;
			lock (obj)
			{
				num = Syscall.sys_endpwent();
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fgetpwent", SetLastError = true)]
		private static extern int sys_fgetpwent(IntPtr stream, out Syscall._Passwd pwbuf);

		public static Passwd fgetpwent(IntPtr stream)
		{
			object obj = Syscall.pwd_lock;
			Syscall._Passwd passwd;
			int num;
			lock (obj)
			{
				num = Syscall.sys_fgetpwent(stream, out passwd);
			}
			if (num != 0)
			{
				return null;
			}
			Passwd passwd2 = new Passwd();
			Syscall.CopyPasswd(passwd2, ref passwd);
			return passwd2;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_psignal", SetLastError = true)]
		private static extern int psignal(int sig, string s);

		public static int psignal(Signum sig, string s)
		{
			int num = NativeConvert.FromSignum(sig);
			return Syscall.psignal(num, s);
		}

		[DllImport("libc", EntryPoint = "kill", SetLastError = true)]
		private static extern int sys_kill(int pid, int sig);

		public static int kill(int pid, Signum sig)
		{
			int num = NativeConvert.FromSignum(sig);
			return Syscall.sys_kill(pid, num);
		}

		[DllImport("libc", EntryPoint = "strsignal", SetLastError = true)]
		private static extern IntPtr sys_strsignal(int sig);

		public static string strsignal(Signum sig)
		{
			int num = NativeConvert.FromSignum(sig);
			object obj = Syscall.signal_lock;
			string text;
			lock (obj)
			{
				IntPtr intPtr = Syscall.sys_strsignal(num);
				text = UnixMarshal.PtrToString(intPtr);
			}
			return text;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_L_ctermid")]
		private static extern int _L_ctermid();

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_L_cuserid")]
		private static extern int _L_cuserid();

		[DllImport("libc", EntryPoint = "cuserid", SetLastError = true)]
		private static extern IntPtr sys_cuserid([Out] StringBuilder @string);

		[Obsolete("\"Nobody knows precisely what cuserid() does... DO NOT USE cuserid().\n`string' must hold L_cuserid characters.  Use getlogin_r instead.")]
		public static string cuserid(StringBuilder @string)
		{
			if (@string.Capacity < Syscall.L_cuserid)
			{
				throw new ArgumentOutOfRangeException("string", "string.Capacity < L_cuserid");
			}
			object obj = Syscall.getlogin_lock;
			string text;
			lock (obj)
			{
				IntPtr intPtr = Syscall.sys_cuserid(@string);
				text = UnixMarshal.PtrToString(intPtr);
			}
			return text;
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int mkstemp(StringBuilder template);

		[DllImport("libc", SetLastError = true)]
		public static extern int ttyslot();

		[Obsolete("This is insecure and should not be used", true)]
		public static int setkey(string key)
		{
			throw new SecurityException("crypt(3) has been broken.  Use something more secure.");
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_strerror_r", SetLastError = true)]
		private static extern int sys_strerror_r(int errnum, [Out] StringBuilder buf, ulong n);

		public static int strerror_r(Errno errnum, StringBuilder buf, ulong n)
		{
			int num = NativeConvert.FromErrno(errnum);
			return Syscall.sys_strerror_r(num, buf, n);
		}

		public static int strerror_r(Errno errnum, StringBuilder buf)
		{
			return Syscall.strerror_r(errnum, buf, (ulong)((long)buf.Capacity));
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_posix_madvise", SetLastError = true)]
		public static extern int posix_madvise(IntPtr addr, ulong len, PosixMadviseAdvice advice);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_mmap", SetLastError = true)]
		public static extern IntPtr mmap(IntPtr start, ulong length, MmapProts prot, MmapFlags flags, int fd, long offset);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_munmap", SetLastError = true)]
		public static extern int munmap(IntPtr start, ulong length);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_mprotect", SetLastError = true)]
		public static extern int mprotect(IntPtr start, ulong len, MmapProts prot);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_msync", SetLastError = true)]
		public static extern int msync(IntPtr start, ulong len, MsyncFlags flags);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_mlock", SetLastError = true)]
		public static extern int mlock(IntPtr start, ulong len);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_munlock", SetLastError = true)]
		public static extern int munlock(IntPtr start, ulong len);

		[DllImport("libc", EntryPoint = "mlockall", SetLastError = true)]
		private static extern int sys_mlockall(int flags);

		public static int mlockall(MlockallFlags flags)
		{
			int num = NativeConvert.FromMlockallFlags(flags);
			return Syscall.sys_mlockall(num);
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int munlockall();

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_mremap", SetLastError = true)]
		public static extern IntPtr mremap(IntPtr old_address, ulong old_size, ulong new_size, MremapFlags flags);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_mincore", SetLastError = true)]
		public static extern int mincore(IntPtr start, ulong length, byte[] vec);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_remap_file_pages", SetLastError = true)]
		public static extern int remap_file_pages(IntPtr start, ulong size, MmapProts prot, long pgoff, MmapFlags flags);

		[DllImport("libc", EntryPoint = "poll", SetLastError = true)]
		private static extern int sys_poll(Syscall._pollfd[] ufds, uint nfds, int timeout);

		public static int poll(Pollfd[] fds, uint nfds, int timeout)
		{
			if ((long)fds.Length < (long)((ulong)nfds))
			{
				throw new ArgumentOutOfRangeException("fds", "Must refer to at least `nfds' elements");
			}
			Syscall._pollfd[] array = new Syscall._pollfd[nfds];
			for (int i = 0; i < array.Length; i++)
			{
				array[i].fd = fds[i].fd;
				array[i].events = NativeConvert.FromPollEvents(fds[i].events);
			}
			int num = Syscall.sys_poll(array, nfds, timeout);
			for (int j = 0; j < array.Length; j++)
			{
				fds[j].revents = NativeConvert.ToPollEvents(array[j].revents);
			}
			return num;
		}

		public static int poll(Pollfd[] fds, int timeout)
		{
			return Syscall.poll(fds, (uint)fds.Length, timeout);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_sendfile", SetLastError = true)]
		public static extern long sendfile(int out_fd, int in_fd, ref long offset, ulong count);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_stat", SetLastError = true)]
		public static extern int stat([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string file_name, out Stat buf);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fstat", SetLastError = true)]
		public static extern int fstat(int filedes, out Stat buf);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_lstat", SetLastError = true)]
		public static extern int lstat([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string file_name, out Stat buf);

		[DllImport("libc", EntryPoint = "chmod", SetLastError = true)]
		private static extern int sys_chmod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, uint mode);

		public static int chmod(string path, FilePermissions mode)
		{
			uint num = NativeConvert.FromFilePermissions(mode);
			return Syscall.sys_chmod(path, num);
		}

		[DllImport("libc", EntryPoint = "fchmod", SetLastError = true)]
		private static extern int sys_fchmod(int filedes, uint mode);

		public static int fchmod(int filedes, FilePermissions mode)
		{
			uint num = NativeConvert.FromFilePermissions(mode);
			return Syscall.sys_fchmod(filedes, num);
		}

		[DllImport("libc", EntryPoint = "umask", SetLastError = true)]
		private static extern uint sys_umask(uint mask);

		public static FilePermissions umask(FilePermissions mask)
		{
			uint num = NativeConvert.FromFilePermissions(mask);
			uint num2 = Syscall.sys_umask(num);
			return NativeConvert.ToFilePermissions(num2);
		}

		[DllImport("libc", EntryPoint = "mkdir", SetLastError = true)]
		private static extern int sys_mkdir([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string oldpath, uint mode);

		public static int mkdir(string oldpath, FilePermissions mode)
		{
			uint num = NativeConvert.FromFilePermissions(mode);
			return Syscall.sys_mkdir(oldpath, num);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_mknod", SetLastError = true)]
		public static extern int mknod([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string pathname, FilePermissions mode, ulong dev);

		[DllImport("libc", EntryPoint = "mkfifo", SetLastError = true)]
		private static extern int sys_mkfifo([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string pathname, uint mode);

		public static int mkfifo(string pathname, FilePermissions mode)
		{
			uint num = NativeConvert.FromFilePermissions(mode);
			return Syscall.sys_mkfifo(pathname, num);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_statvfs", SetLastError = true)]
		public static extern int statvfs([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, out Statvfs buf);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fstatvfs", SetLastError = true)]
		public static extern int fstatvfs(int fd, out Statvfs buf);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_gettimeofday", SetLastError = true)]
		public static extern int gettimeofday(out Timeval tv, out Timezone tz);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_gettimeofday", SetLastError = true)]
		private static extern int gettimeofday(out Timeval tv, IntPtr ignore);

		public static int gettimeofday(out Timeval tv)
		{
			return Syscall.gettimeofday(out tv, IntPtr.Zero);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_gettimeofday", SetLastError = true)]
		private static extern int gettimeofday(IntPtr ignore, out Timezone tz);

		public static int gettimeofday(out Timezone tz)
		{
			return Syscall.gettimeofday(IntPtr.Zero, out tz);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_settimeofday", SetLastError = true)]
		public static extern int settimeofday(ref Timeval tv, ref Timezone tz);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_gettimeofday", SetLastError = true)]
		private static extern int settimeofday(ref Timeval tv, IntPtr ignore);

		public static int settimeofday(ref Timeval tv)
		{
			return Syscall.settimeofday(ref tv, IntPtr.Zero);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_utimes", SetLastError = true)]
		private static extern int sys_utimes([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string filename, Timeval[] tvp);

		public static int utimes(string filename, Timeval[] tvp)
		{
			if (tvp != null && tvp.Length != 2)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				return -1;
			}
			return Syscall.sys_utimes(filename, tvp);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_lutimes", SetLastError = true)]
		private static extern int sys_lutimes([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string filename, Timeval[] tvp);

		public static int lutimes(string filename, Timeval[] tvp)
		{
			if (tvp != null && tvp.Length != 2)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				return -1;
			}
			return Syscall.sys_lutimes(filename, tvp);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_futimes", SetLastError = true)]
		private static extern int sys_futimes(int fd, Timeval[] tvp);

		public static int futimes(int fd, Timeval[] tvp)
		{
			if (tvp != null && tvp.Length != 2)
			{
				Stdlib.SetLastError(Errno.EINVAL);
				return -1;
			}
			return Syscall.sys_futimes(fd, tvp);
		}

		private static void CopyUtsname(ref Utsname to, ref Syscall._Utsname from)
		{
			try
			{
				to = new Utsname();
				to.sysname = UnixMarshal.PtrToString(from.sysname);
				to.nodename = UnixMarshal.PtrToString(from.nodename);
				to.release = UnixMarshal.PtrToString(from.release);
				to.version = UnixMarshal.PtrToString(from.version);
				to.machine = UnixMarshal.PtrToString(from.machine);
				to.domainname = UnixMarshal.PtrToString(from.domainname);
			}
			finally
			{
				Stdlib.free(from._buf_);
				from._buf_ = IntPtr.Zero;
			}
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_uname", SetLastError = true)]
		private static extern int sys_uname(out Syscall._Utsname buf);

		public static int uname(out Utsname buf)
		{
			Syscall._Utsname utsname;
			int num = Syscall.sys_uname(out utsname);
			buf = new Utsname();
			if (num == 0)
			{
				Syscall.CopyUtsname(ref buf, ref utsname);
			}
			return num;
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int wait(out int status);

		[DllImport("libc", SetLastError = true)]
		private static extern int waitpid(int pid, out int status, int options);

		public static int waitpid(int pid, out int status, WaitOptions options)
		{
			int num = NativeConvert.FromWaitOptions(options);
			return Syscall.waitpid(pid, out status, num);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_WIFEXITED")]
		private static extern int _WIFEXITED(int status);

		public static bool WIFEXITED(int status)
		{
			return Syscall._WIFEXITED(status) != 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_WEXITSTATUS")]
		public static extern int WEXITSTATUS(int status);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_WIFSIGNALED")]
		private static extern int _WIFSIGNALED(int status);

		public static bool WIFSIGNALED(int status)
		{
			return Syscall._WIFSIGNALED(status) != 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_WTERMSIG")]
		private static extern int _WTERMSIG(int status);

		public static Signum WTERMSIG(int status)
		{
			int num = Syscall._WTERMSIG(status);
			return NativeConvert.ToSignum(num);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_WIFSTOPPED")]
		private static extern int _WIFSTOPPED(int status);

		public static bool WIFSTOPPED(int status)
		{
			return Syscall._WIFSTOPPED(status) != 0;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_WSTOPSIG")]
		private static extern int _WSTOPSIG(int status);

		public static Signum WSTOPSIG(int status)
		{
			int num = Syscall._WSTOPSIG(status);
			return NativeConvert.ToSignum(num);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_openlog", SetLastError = true)]
		private static extern int sys_openlog(IntPtr ident, int option, int facility);

		public static int openlog(IntPtr ident, SyslogOptions option, SyslogFacility defaultFacility)
		{
			int num = NativeConvert.FromSyslogOptions(option);
			int num2 = NativeConvert.FromSyslogFacility(defaultFacility);
			return Syscall.sys_openlog(ident, num, num2);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_syslog", SetLastError = true)]
		private static extern int sys_syslog(int priority, string message);

		public static int syslog(SyslogFacility facility, SyslogLevel level, string message)
		{
			int num = NativeConvert.FromSyslogFacility(facility);
			int num2 = NativeConvert.FromSyslogLevel(level);
			return Syscall.sys_syslog(num | num2, Syscall.GetSyslogMessage(message));
		}

		public static int syslog(SyslogLevel level, string message)
		{
			int num = NativeConvert.FromSyslogLevel(level);
			return Syscall.sys_syslog(num, Syscall.GetSyslogMessage(message));
		}

		private static string GetSyslogMessage(string message)
		{
			return UnixMarshal.EscapeFormatString(message, new char[] { 'm' });
		}

		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse syslog(SyslogFacility, SyslogLevel, string) instead.")]
		public static int syslog(SyslogFacility facility, SyslogLevel level, string format, params object[] parameters)
		{
			int num = NativeConvert.FromSyslogFacility(facility);
			int num2 = NativeConvert.FromSyslogLevel(level);
			object[] array = new object[checked(parameters.Length + 2)];
			array[0] = num | num2;
			array[1] = format;
			Array.Copy(parameters, 0, array, 2, parameters.Length);
			return (int)XPrintfFunctions.syslog(array);
		}

		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse syslog(SyslogLevel, string) instead.")]
		public static int syslog(SyslogLevel level, string format, params object[] parameters)
		{
			int num = NativeConvert.FromSyslogLevel(level);
			object[] array = new object[checked(parameters.Length + 2)];
			array[0] = num;
			array[1] = format;
			Array.Copy(parameters, 0, array, 2, parameters.Length);
			return (int)XPrintfFunctions.syslog(array);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_closelog", SetLastError = true)]
		public static extern int closelog();

		[DllImport("libc", EntryPoint = "setlogmask", SetLastError = true)]
		private static extern int sys_setlogmask(int mask);

		public static int setlogmask(SyslogLevel mask)
		{
			int num = NativeConvert.FromSyslogLevel(mask);
			return Syscall.sys_setlogmask(num);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_nanosleep", SetLastError = true)]
		public static extern int nanosleep(ref Timespec req, ref Timespec rem);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_stime", SetLastError = true)]
		public static extern int stime(ref long t);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_time", SetLastError = true)]
		public static extern long time(out long t);

		[DllImport("libc", EntryPoint = "access", SetLastError = true)]
		private static extern int sys_access([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string pathname, int mode);

		public static int access(string pathname, AccessModes mode)
		{
			int num = NativeConvert.FromAccessModes(mode);
			return Syscall.sys_access(pathname, num);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_lseek", SetLastError = true)]
		private static extern long sys_lseek(int fd, long offset, int whence);

		public static long lseek(int fd, long offset, SeekFlags whence)
		{
			short num = NativeConvert.FromSeekFlags(whence);
			return Syscall.sys_lseek(fd, offset, (int)num);
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int close(int fd);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_read", SetLastError = true)]
		public static extern long read(int fd, IntPtr buf, ulong count);

		public unsafe static long read(int fd, void* buf, ulong count)
		{
			return Syscall.read(fd, (IntPtr)buf, count);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_write", SetLastError = true)]
		public static extern long write(int fd, IntPtr buf, ulong count);

		public unsafe static long write(int fd, void* buf, ulong count)
		{
			return Syscall.write(fd, (IntPtr)buf, count);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_pread", SetLastError = true)]
		public static extern long pread(int fd, IntPtr buf, ulong count, long offset);

		public unsafe static long pread(int fd, void* buf, ulong count, long offset)
		{
			return Syscall.pread(fd, (IntPtr)buf, count, offset);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_pwrite", SetLastError = true)]
		public static extern long pwrite(int fd, IntPtr buf, ulong count, long offset);

		public unsafe static long pwrite(int fd, void* buf, ulong count, long offset)
		{
			return Syscall.pwrite(fd, (IntPtr)buf, count, offset);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_pipe", SetLastError = true)]
		public static extern int pipe(out int reading, out int writing);

		public static int pipe(int[] filedes)
		{
			if (filedes == null || filedes.Length != 2)
			{
				return -1;
			}
			int num2;
			int num3;
			int num = Syscall.pipe(out num2, out num3);
			filedes[0] = num2;
			filedes[1] = num3;
			return num;
		}

		[DllImport("libc", SetLastError = true)]
		public static extern uint alarm(uint seconds);

		[DllImport("libc", SetLastError = true)]
		public static extern uint sleep(uint seconds);

		[DllImport("libc", SetLastError = true)]
		public static extern uint ualarm(uint usecs, uint interval);

		[DllImport("libc", SetLastError = true)]
		public static extern int pause();

		[DllImport("libc", SetLastError = true)]
		public static extern int chown([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, uint owner, uint group);

		[DllImport("libc", SetLastError = true)]
		public static extern int fchown(int fd, uint owner, uint group);

		[DllImport("libc", SetLastError = true)]
		public static extern int lchown([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, uint owner, uint group);

		[DllImport("libc", SetLastError = true)]
		public static extern int chdir([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path);

		[DllImport("libc", SetLastError = true)]
		public static extern int fchdir(int fd);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getcwd", SetLastError = true)]
		public static extern IntPtr getcwd([Out] StringBuilder buf, ulong size);

		public static StringBuilder getcwd(StringBuilder buf)
		{
			Syscall.getcwd(buf, (ulong)((long)buf.Capacity));
			return buf;
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int dup(int fd);

		[DllImport("libc", SetLastError = true)]
		public static extern int dup2(int fd, int fd2);

		[DllImport("libc", SetLastError = true)]
		public static extern int execve([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, string[] argv, string[] envp);

		[DllImport("libc", SetLastError = true)]
		public static extern int fexecve(int fd, string[] argv, string[] envp);

		[DllImport("libc", SetLastError = true)]
		public static extern int execv([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, string[] argv);

		[DllImport("libc", SetLastError = true)]
		public static extern int execvp([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, string[] argv);

		[DllImport("libc", SetLastError = true)]
		public static extern int nice(int inc);

		[CLSCompliant(false)]
		[DllImport("libc", SetLastError = true)]
		public static extern int _exit(int status);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_fpathconf", SetLastError = true)]
		public static extern long fpathconf(int filedes, PathconfName name, Errno defaultError);

		public static long fpathconf(int filedes, PathconfName name)
		{
			return Syscall.fpathconf(filedes, name, (Errno)0);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_pathconf", SetLastError = true)]
		public static extern long pathconf([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, PathconfName name, Errno defaultError);

		public static long pathconf(string path, PathconfName name)
		{
			return Syscall.pathconf(path, name, (Errno)0);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_sysconf", SetLastError = true)]
		public static extern long sysconf(SysconfName name, Errno defaultError);

		public static long sysconf(SysconfName name)
		{
			return Syscall.sysconf(name, (Errno)0);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_confstr", SetLastError = true)]
		public static extern ulong confstr(ConfstrName name, [Out] StringBuilder buf, ulong len);

		[DllImport("libc", SetLastError = true)]
		public static extern int getpid();

		[DllImport("libc", SetLastError = true)]
		public static extern int getppid();

		[DllImport("libc", SetLastError = true)]
		public static extern int setpgid(int pid, int pgid);

		[DllImport("libc", SetLastError = true)]
		public static extern int getpgid(int pid);

		[DllImport("libc", SetLastError = true)]
		public static extern int setpgrp();

		[DllImport("libc", SetLastError = true)]
		public static extern int getpgrp();

		[DllImport("libc", SetLastError = true)]
		public static extern int setsid();

		[DllImport("libc", SetLastError = true)]
		public static extern int getsid(int pid);

		[DllImport("libc", SetLastError = true)]
		public static extern uint getuid();

		[DllImport("libc", SetLastError = true)]
		public static extern uint geteuid();

		[DllImport("libc", SetLastError = true)]
		public static extern uint getgid();

		[DllImport("libc", SetLastError = true)]
		public static extern uint getegid();

		[DllImport("libc", SetLastError = true)]
		public static extern int getgroups(int size, uint[] list);

		public static int getgroups(uint[] list)
		{
			return Syscall.getgroups(list.Length, list);
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int setuid(uint uid);

		[DllImport("libc", SetLastError = true)]
		public static extern int setreuid(uint ruid, uint euid);

		[DllImport("libc", SetLastError = true)]
		public static extern int setregid(uint rgid, uint egid);

		[DllImport("libc", SetLastError = true)]
		public static extern int seteuid(uint euid);

		[DllImport("libc", SetLastError = true)]
		public static extern int setegid(uint uid);

		[DllImport("libc", SetLastError = true)]
		public static extern int setgid(uint gid);

		[DllImport("libc", SetLastError = true)]
		public static extern int getresuid(out uint ruid, out uint euid, out uint suid);

		[DllImport("libc", SetLastError = true)]
		public static extern int getresgid(out uint rgid, out uint egid, out uint sgid);

		[DllImport("libc", SetLastError = true)]
		public static extern int setresuid(uint ruid, uint euid, uint suid);

		[DllImport("libc", SetLastError = true)]
		public static extern int setresgid(uint rgid, uint egid, uint sgid);

		[DllImport("libc", EntryPoint = "ttyname", SetLastError = true)]
		private static extern IntPtr sys_ttyname(int fd);

		public static string ttyname(int fd)
		{
			object obj = Syscall.tty_lock;
			string text;
			lock (obj)
			{
				IntPtr intPtr = Syscall.sys_ttyname(fd);
				text = UnixMarshal.PtrToString(intPtr);
			}
			return text;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_ttyname_r", SetLastError = true)]
		public static extern int ttyname_r(int fd, [Out] StringBuilder buf, ulong buflen);

		public static int ttyname_r(int fd, StringBuilder buf)
		{
			return Syscall.ttyname_r(fd, buf, (ulong)((long)buf.Capacity));
		}

		[DllImport("libc", EntryPoint = "isatty")]
		private static extern int sys_isatty(int fd);

		public static bool isatty(int fd)
		{
			return Syscall.sys_isatty(fd) == 1;
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int link([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string oldpath, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string newpath);

		[DllImport("libc", SetLastError = true)]
		public static extern int symlink([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string oldpath, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string newpath);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_readlink", SetLastError = true)]
		public static extern int readlink([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, [Out] StringBuilder buf, ulong bufsiz);

		public static int readlink(string path, [Out] StringBuilder buf)
		{
			return Syscall.readlink(path, buf, (ulong)((long)buf.Capacity));
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int unlink([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string pathname);

		[DllImport("libc", SetLastError = true)]
		public static extern int rmdir([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string pathname);

		[DllImport("libc", SetLastError = true)]
		public static extern int tcgetpgrp(int fd);

		[DllImport("libc", SetLastError = true)]
		public static extern int tcsetpgrp(int fd, int pgrp);

		[DllImport("libc", EntryPoint = "getlogin", SetLastError = true)]
		private static extern IntPtr sys_getlogin();

		public static string getlogin()
		{
			object obj = Syscall.getlogin_lock;
			string text;
			lock (obj)
			{
				IntPtr intPtr = Syscall.sys_getlogin();
				text = UnixMarshal.PtrToString(intPtr);
			}
			return text;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getlogin_r", SetLastError = true)]
		public static extern int getlogin_r([Out] StringBuilder name, ulong bufsize);

		public static int getlogin_r(StringBuilder name)
		{
			return Syscall.getlogin_r(name, (ulong)((long)name.Capacity));
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int setlogin(string name);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_gethostname", SetLastError = true)]
		public static extern int gethostname([Out] StringBuilder name, ulong len);

		public static int gethostname(StringBuilder name)
		{
			return Syscall.gethostname(name, (ulong)((long)name.Capacity));
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_sethostname", SetLastError = true)]
		public static extern int sethostname(string name, ulong len);

		public static int sethostname(string name)
		{
			return Syscall.sethostname(name, (ulong)((long)name.Length));
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_gethostid", SetLastError = true)]
		public static extern long gethostid();

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_sethostid", SetLastError = true)]
		public static extern int sethostid(long hostid);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_getdomainname", SetLastError = true)]
		public static extern int getdomainname([Out] StringBuilder name, ulong len);

		public static int getdomainname(StringBuilder name)
		{
			return Syscall.getdomainname(name, (ulong)((long)name.Capacity));
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setdomainname", SetLastError = true)]
		public static extern int setdomainname(string name, ulong len);

		public static int setdomainname(string name)
		{
			return Syscall.setdomainname(name, (ulong)((long)name.Length));
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int vhangup();

		[DllImport("libc", SetLastError = true)]
		public static extern int revoke([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string file);

		[DllImport("libc", SetLastError = true)]
		public static extern int acct([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string filename);

		[DllImport("libc", EntryPoint = "getusershell", SetLastError = true)]
		private static extern IntPtr sys_getusershell();

		public static string getusershell()
		{
			object obj = Syscall.usershell_lock;
			string text;
			lock (obj)
			{
				IntPtr intPtr = Syscall.sys_getusershell();
				text = UnixMarshal.PtrToString(intPtr);
			}
			return text;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_setusershell", SetLastError = true)]
		private static extern int sys_setusershell();

		public static int setusershell()
		{
			object obj = Syscall.usershell_lock;
			int num;
			lock (obj)
			{
				num = Syscall.sys_setusershell();
			}
			return num;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_endusershell", SetLastError = true)]
		private static extern int sys_endusershell();

		public static int endusershell()
		{
			object obj = Syscall.usershell_lock;
			int num;
			lock (obj)
			{
				num = Syscall.sys_endusershell();
			}
			return num;
		}

		[DllImport("libc", SetLastError = true)]
		public static extern int chroot([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path);

		[DllImport("libc", SetLastError = true)]
		public static extern int fsync(int fd);

		[DllImport("libc", SetLastError = true)]
		public static extern int fdatasync(int fd);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_sync", SetLastError = true)]
		public static extern int sync();

		[Obsolete("Dropped in POSIX 1003.1-2001.  Use Syscall.sysconf (SysconfName._SC_PAGESIZE).")]
		[DllImport("libc", SetLastError = true)]
		public static extern int getpagesize();

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_truncate", SetLastError = true)]
		public static extern int truncate([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string path, long length);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_ftruncate", SetLastError = true)]
		public static extern int ftruncate(int fd, long length);

		[DllImport("libc", SetLastError = true)]
		public static extern int getdtablesize();

		[DllImport("libc", SetLastError = true)]
		public static extern int brk(IntPtr end_data_segment);

		[DllImport("libc", SetLastError = true)]
		public static extern IntPtr sbrk(IntPtr increment);

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_lockf", SetLastError = true)]
		public static extern int lockf(int fd, LockfCommand cmd, long len);

		[Obsolete("This is insecure and should not be used", true)]
		public static string crypt(string key, string salt)
		{
			throw new SecurityException("crypt(3) has been broken.  Use something more secure.");
		}

		[Obsolete("This is insecure and should not be used", true)]
		public static int encrypt(byte[] block, bool decode)
		{
			throw new SecurityException("crypt(3) has been broken.  Use something more secure.");
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_swab", SetLastError = true)]
		public static extern int swab(IntPtr from, IntPtr to, long n);

		public unsafe static void swab(void* from, void* to, long n)
		{
			Syscall.swab((IntPtr)from, (IntPtr)to, n);
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Syscall_utime", SetLastError = true)]
		private static extern int sys_utime([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = Mono.Unix.Native.FileNameMarshaler)] string filename, ref Utimbuf buf, int use_buf);

		public static int utime(string filename, ref Utimbuf buf)
		{
			return Syscall.sys_utime(filename, ref buf, 1);
		}

		public static int utime(string filename)
		{
			Utimbuf utimbuf = default(Utimbuf);
			return Syscall.sys_utime(filename, ref utimbuf, 0);
		}

		internal new const string LIBC = "libc";

		internal static object readdir_lock = new object();

		internal static object fstab_lock = new object();

		internal static object grp_lock = new object();

		internal static object pwd_lock = new object();

		private static object signal_lock = new object();

		public static readonly int L_ctermid = Syscall._L_ctermid();

		public static readonly int L_cuserid = Syscall._L_cuserid();

		internal static object getlogin_lock = new object();

		public static readonly IntPtr MAP_FAILED = (IntPtr)(-1);

		private static object tty_lock = new object();

		internal static object usershell_lock = new object();

		private struct _Dirent
		{
			[ino_t]
			public ulong d_ino;

			[off_t]
			public long d_off;

			public ushort d_reclen;

			public byte d_type;

			public IntPtr d_name;
		}

		[Map]
		private struct _Fstab
		{
			public IntPtr fs_spec;

			public IntPtr fs_file;

			public IntPtr fs_vfstype;

			public IntPtr fs_mntops;

			public IntPtr fs_type;

			public int fs_freq;

			public int fs_passno;

			public IntPtr _fs_buf_;
		}

		[Map]
		private struct _Group
		{
			public IntPtr gr_name;

			public IntPtr gr_passwd;

			[gid_t]
			public uint gr_gid;

			public int _gr_nmem_;

			public IntPtr gr_mem;

			public IntPtr _gr_buf_;
		}

		[Map]
		private struct _Passwd
		{
			public IntPtr pw_name;

			public IntPtr pw_passwd;

			[uid_t]
			public uint pw_uid;

			[gid_t]
			public uint pw_gid;

			public IntPtr pw_gecos;

			public IntPtr pw_dir;

			public IntPtr pw_shell;

			public IntPtr _pw_buf_;
		}

		private struct _pollfd
		{
			public int fd;

			public short events;

			public short revents;
		}

		[Map]
		private struct _Utsname
		{
			public IntPtr sysname;

			public IntPtr nodename;

			public IntPtr release;

			public IntPtr version;

			public IntPtr machine;

			public IntPtr domainname;

			public IntPtr _buf_;
		}
	}
}
