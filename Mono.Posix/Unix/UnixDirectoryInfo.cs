using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Unix.Native;

namespace Mono.Unix
{
	public sealed class UnixDirectoryInfo : UnixFileSystemInfo
	{
		public UnixDirectoryInfo(string path)
			: base(path)
		{
		}

		internal UnixDirectoryInfo(string path, Stat stat)
			: base(path, stat)
		{
		}

		public override string Name
		{
			get
			{
				string fileName = UnixPath.GetFileName(base.FullPath);
				if (fileName == null || fileName.Length == 0)
				{
					return base.FullPath;
				}
				return fileName;
			}
		}

		public UnixDirectoryInfo Parent
		{
			get
			{
				if (base.FullPath == "/")
				{
					return this;
				}
				string directoryName = UnixPath.GetDirectoryName(base.FullPath);
				if (directoryName == "")
				{
					throw new InvalidOperationException("Do not know parent directory for path `" + base.FullPath + "'");
				}
				return new UnixDirectoryInfo(directoryName);
			}
		}

		public UnixDirectoryInfo Root
		{
			get
			{
				string pathRoot = UnixPath.GetPathRoot(base.FullPath);
				if (pathRoot == null)
				{
					return null;
				}
				return new UnixDirectoryInfo(pathRoot);
			}
		}

		[CLSCompliant(false)]
		public void Create(FilePermissions mode)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.mkdir(base.FullPath, mode));
			base.Refresh();
		}

		public void Create(FileAccessPermissions mode)
		{
			this.Create((FilePermissions)mode);
		}

		public void Create()
		{
			FilePermissions filePermissions = FilePermissions.ACCESSPERMS;
			this.Create(filePermissions);
		}

		public override void Delete()
		{
			this.Delete(false);
		}

		public void Delete(bool recursive)
		{
			if (recursive)
			{
				foreach (UnixFileSystemInfo unixFileSystemInfo in this.GetFileSystemEntries())
				{
					UnixDirectoryInfo unixDirectoryInfo = unixFileSystemInfo as UnixDirectoryInfo;
					if (unixDirectoryInfo != null)
					{
						unixDirectoryInfo.Delete(true);
					}
					else
					{
						unixFileSystemInfo.Delete();
					}
				}
			}
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.rmdir(base.FullPath));
			base.Refresh();
		}

		public Dirent[] GetEntries()
		{
			IntPtr intPtr = Syscall.opendir(base.FullPath);
			if (intPtr == IntPtr.Zero)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			bool flag = false;
			Dirent[] array;
			try
			{
				Dirent[] entries = UnixDirectoryInfo.GetEntries(intPtr);
				flag = true;
				array = entries;
			}
			finally
			{
				int num = Syscall.closedir(intPtr);
				if (flag)
				{
					UnixMarshal.ThrowExceptionForLastErrorIf(num);
				}
			}
			return array;
		}

		private static Dirent[] GetEntries(IntPtr dirp)
		{
			ArrayList arrayList = new ArrayList();
			IntPtr intPtr;
			int num;
			do
			{
				Dirent dirent = new Dirent();
				num = Syscall.readdir_r(dirp, dirent, out intPtr);
				if (num == 0 && intPtr != IntPtr.Zero && dirent.d_name != "." && dirent.d_name != "..")
				{
					arrayList.Add(dirent);
				}
			}
			while (num == 0 && intPtr != IntPtr.Zero);
			if (num != 0)
			{
				UnixMarshal.ThrowExceptionForLastErrorIf(num);
			}
			return (Dirent[])arrayList.ToArray(typeof(Dirent));
		}

		public Dirent[] GetEntries(Regex regex)
		{
			IntPtr intPtr = Syscall.opendir(base.FullPath);
			if (intPtr == IntPtr.Zero)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			Dirent[] entries;
			try
			{
				entries = UnixDirectoryInfo.GetEntries(intPtr, regex);
			}
			finally
			{
				UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.closedir(intPtr));
			}
			return entries;
		}

		private static Dirent[] GetEntries(IntPtr dirp, Regex regex)
		{
			ArrayList arrayList = new ArrayList();
			IntPtr intPtr;
			int num;
			do
			{
				Dirent dirent = new Dirent();
				num = Syscall.readdir_r(dirp, dirent, out intPtr);
				if (num == 0 && intPtr != IntPtr.Zero && regex.Match(dirent.d_name).Success && dirent.d_name != "." && dirent.d_name != "..")
				{
					arrayList.Add(dirent);
				}
			}
			while (num == 0 && intPtr != IntPtr.Zero);
			if (num != 0)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return (Dirent[])arrayList.ToArray(typeof(Dirent));
		}

		public Dirent[] GetEntries(string regex)
		{
			Regex regex2 = new Regex(regex);
			return this.GetEntries(regex2);
		}

		public UnixFileSystemInfo[] GetFileSystemEntries()
		{
			Dirent[] entries = this.GetEntries();
			return this.GetFileSystemEntries(entries);
		}

		private UnixFileSystemInfo[] GetFileSystemEntries(Dirent[] dentries)
		{
			UnixFileSystemInfo[] array = new UnixFileSystemInfo[dentries.Length];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = UnixFileSystemInfo.GetFileSystemEntry(UnixPath.Combine(base.FullPath, new string[] { dentries[num].d_name }));
			}
			return array;
		}

		public UnixFileSystemInfo[] GetFileSystemEntries(Regex regex)
		{
			Dirent[] entries = this.GetEntries(regex);
			return this.GetFileSystemEntries(entries);
		}

		public UnixFileSystemInfo[] GetFileSystemEntries(string regex)
		{
			Regex regex2 = new Regex(regex);
			return this.GetFileSystemEntries(regex2);
		}

		public static string GetCurrentDirectory()
		{
			StringBuilder stringBuilder = new StringBuilder(16);
			IntPtr intPtr = IntPtr.Zero;
			do
			{
				stringBuilder.Capacity *= 2;
				intPtr = Syscall.getcwd(stringBuilder, (ulong)((long)stringBuilder.Capacity));
			}
			while (intPtr == IntPtr.Zero && Stdlib.GetLastError() == Errno.ERANGE);
			if (intPtr == IntPtr.Zero)
			{
				UnixMarshal.ThrowExceptionForLastError();
			}
			return stringBuilder.ToString();
		}

		public static void SetCurrentDirectory(string path)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.chdir(path));
		}
	}
}
