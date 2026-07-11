using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public sealed class DirectoryInfo : FileSystemInfo
	{
		public DirectoryInfo(string path)
			: this(path, false)
		{
		}

		internal DirectoryInfo(string path, bool simpleOriginalPath)
		{
			this.CheckPath(path);
			this.FullPath = Path.GetFullPath(path);
			if (simpleOriginalPath)
			{
				this.OriginalPath = Path.GetFileName(this.FullPath);
			}
			else
			{
				this.OriginalPath = path;
			}
			this.Initialize();
		}

		private DirectoryInfo(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Initialize();
		}

		private void Initialize()
		{
			int num = this.FullPath.Length - 1;
			if (num > 1 && this.FullPath[num] == Path.DirectorySeparatorChar)
			{
				num--;
			}
			int num2 = this.FullPath.LastIndexOf(Path.DirectorySeparatorChar, num);
			if (num2 == -1 || (num2 == 0 && num == 0))
			{
				this.current = this.FullPath;
				this.parent = null;
				return;
			}
			this.current = this.FullPath.Substring(num2 + 1, num - num2);
			if (num2 == 0 && !Environment.IsRunningOnWindows)
			{
				this.parent = Path.DirectorySeparatorStr;
			}
			else
			{
				this.parent = this.FullPath.Substring(0, num2);
			}
			if (Environment.IsRunningOnWindows && this.parent.Length == 2 && this.parent[1] == ':' && char.IsLetter(this.parent[0]))
			{
				this.parent += Path.DirectorySeparatorChar.ToString();
			}
		}

		public override bool Exists
		{
			get
			{
				if (this._dataInitialised == -1)
				{
					base.Refresh();
				}
				return this._data.fileAttributes != (FileAttributes)(-1) && (this._data.fileAttributes & FileAttributes.Directory) != (FileAttributes)0;
			}
		}

		public override string Name
		{
			get
			{
				return this.current;
			}
		}

		public DirectoryInfo Parent
		{
			get
			{
				if (this.parent == null || this.parent.Length == 0)
				{
					return null;
				}
				return new DirectoryInfo(this.parent);
			}
		}

		public DirectoryInfo Root
		{
			get
			{
				string pathRoot = Path.GetPathRoot(this.FullPath);
				if (pathRoot == null)
				{
					return null;
				}
				return new DirectoryInfo(pathRoot);
			}
		}

		public void Create()
		{
			Directory.CreateDirectory(this.FullPath);
		}

		public DirectoryInfo CreateSubdirectory(string path)
		{
			this.CheckPath(path);
			path = Path.Combine(this.FullPath, path);
			Directory.CreateDirectory(path);
			return new DirectoryInfo(path);
		}

		public FileInfo[] GetFiles()
		{
			return this.GetFiles("*");
		}

		public FileInfo[] GetFiles(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			string[] files = Directory.GetFiles(this.FullPath, searchPattern);
			FileInfo[] array = new FileInfo[files.Length];
			int num = 0;
			foreach (string text in files)
			{
				array[num++] = new FileInfo(text);
			}
			return array;
		}

		public DirectoryInfo[] GetDirectories()
		{
			return this.GetDirectories("*");
		}

		public DirectoryInfo[] GetDirectories(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			string[] directories = Directory.GetDirectories(this.FullPath, searchPattern);
			DirectoryInfo[] array = new DirectoryInfo[directories.Length];
			int num = 0;
			foreach (string text in directories)
			{
				array[num++] = new DirectoryInfo(text);
			}
			return array;
		}

		public FileSystemInfo[] GetFileSystemInfos()
		{
			return this.GetFileSystemInfos("*");
		}

		public FileSystemInfo[] GetFileSystemInfos(string searchPattern)
		{
			return this.GetFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly);
		}

		public FileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", "Must be TopDirectoryOnly or AllDirectories");
			}
			if (!Directory.Exists(this.FullPath))
			{
				throw new IOException("Invalid directory");
			}
			List<FileSystemInfo> list = new List<FileSystemInfo>();
			this.InternalGetFileSystemInfos(searchPattern, searchOption, list);
			return list.ToArray();
		}

		private void InternalGetFileSystemInfos(string searchPattern, SearchOption searchOption, List<FileSystemInfo> infos)
		{
			string[] directories = Directory.GetDirectories(this.FullPath, searchPattern);
			string[] files = Directory.GetFiles(this.FullPath, searchPattern);
			Array.ForEach<string>(directories, delegate(string dir)
			{
				infos.Add(new DirectoryInfo(dir));
			});
			Array.ForEach<string>(files, delegate(string file)
			{
				infos.Add(new FileInfo(file));
			});
			if (directories.Length == 0 || searchOption == SearchOption.TopDirectoryOnly)
			{
				return;
			}
			string[] array = directories;
			for (int i = 0; i < array.Length; i++)
			{
				new DirectoryInfo(array[i]).InternalGetFileSystemInfos(searchPattern, searchOption, infos);
			}
		}

		public override void Delete()
		{
			this.Delete(false);
		}

		public void Delete(bool recursive)
		{
			Directory.Delete(this.FullPath, recursive);
		}

		public void MoveTo(string destDirName)
		{
			if (destDirName == null)
			{
				throw new ArgumentNullException("destDirName");
			}
			if (destDirName.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.", "destDirName");
			}
			Directory.Move(this.FullPath, Path.GetFullPath(destDirName));
			this.OriginalPath = destDirName;
			this.FullPath = destDirName;
			this.Initialize();
		}

		public override string ToString()
		{
			return this.OriginalPath;
		}

		public DirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
		{
			string[] directories = Directory.GetDirectories(this.FullPath, searchPattern, searchOption);
			DirectoryInfo[] array = new DirectoryInfo[directories.Length];
			for (int i = 0; i < directories.Length; i++)
			{
				string text = directories[i];
				array[i] = new DirectoryInfo(text);
			}
			return array;
		}

		internal int GetFilesSubdirs(ArrayList l, string pattern)
		{
			FileInfo[] array = null;
			try
			{
				array = this.GetFiles(pattern);
			}
			catch (UnauthorizedAccessException)
			{
				return 0;
			}
			int num = array.Length;
			l.Add(array);
			foreach (DirectoryInfo directoryInfo in this.GetDirectories())
			{
				num += directoryInfo.GetFilesSubdirs(l, pattern);
			}
			return num;
		}

		public FileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
		{
			if (searchOption == SearchOption.TopDirectoryOnly)
			{
				return this.GetFiles(searchPattern);
			}
			if (searchOption != SearchOption.AllDirectories)
			{
				string text = Locale.GetText("Invalid enum value '{0}' for '{1}'.", new object[] { searchOption, "SearchOption" });
				throw new ArgumentOutOfRangeException("searchOption", text);
			}
			ArrayList arrayList = new ArrayList();
			int filesSubdirs = this.GetFilesSubdirs(arrayList, searchPattern);
			int num = 0;
			FileInfo[] array = new FileInfo[filesSubdirs];
			foreach (object obj in arrayList)
			{
				FileInfo[] array2 = (FileInfo[])obj;
				array2.CopyTo(array, num);
				num += array2.Length;
			}
			return array;
		}

		[MonoLimitation("DirectorySecurity isn't implemented")]
		public void Create(DirectorySecurity directorySecurity)
		{
			if (directorySecurity != null)
			{
				throw new UnauthorizedAccessException();
			}
			this.Create();
		}

		[MonoLimitation("DirectorySecurity isn't implemented")]
		public DirectoryInfo CreateSubdirectory(string path, DirectorySecurity directorySecurity)
		{
			if (directorySecurity != null)
			{
				throw new UnauthorizedAccessException();
			}
			return this.CreateSubdirectory(path);
		}

		public DirectorySecurity GetAccessControl()
		{
			return Directory.GetAccessControl(this.FullPath);
		}

		public DirectorySecurity GetAccessControl(AccessControlSections includeSections)
		{
			return Directory.GetAccessControl(this.FullPath, includeSections);
		}

		public void SetAccessControl(DirectorySecurity directorySecurity)
		{
			Directory.SetAccessControl(this.FullPath, directorySecurity);
		}

		public IEnumerable<DirectoryInfo> EnumerateDirectories()
		{
			return this.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
		}

		public IEnumerable<DirectoryInfo> EnumerateDirectories(string searchPattern)
		{
			return this.EnumerateDirectories(searchPattern, SearchOption.TopDirectoryOnly);
		}

		public IEnumerable<DirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return this.CreateEnumerateDirectoriesIterator(searchPattern, searchOption);
		}

		private IEnumerable<DirectoryInfo> CreateEnumerateDirectoriesIterator(string searchPattern, SearchOption searchOption)
		{
			foreach (string text in Directory.EnumerateDirectories(this.FullPath, searchPattern, searchOption))
			{
				yield return new DirectoryInfo(text);
			}
			IEnumerator<string> enumerator = null;
			yield break;
			yield break;
		}

		public IEnumerable<FileInfo> EnumerateFiles()
		{
			return this.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
		}

		public IEnumerable<FileInfo> EnumerateFiles(string searchPattern)
		{
			return this.EnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly);
		}

		public IEnumerable<FileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return this.CreateEnumerateFilesIterator(searchPattern, searchOption);
		}

		private IEnumerable<FileInfo> CreateEnumerateFilesIterator(string searchPattern, SearchOption searchOption)
		{
			foreach (string text in Directory.EnumerateFiles(this.FullPath, searchPattern, searchOption))
			{
				yield return new FileInfo(text);
			}
			IEnumerator<string> enumerator = null;
			yield break;
			yield break;
		}

		public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos()
		{
			return this.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly);
		}

		public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
		{
			return this.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly);
		}

		public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchoption");
			}
			return DirectoryInfo.EnumerateFileSystemInfos(this.FullPath, searchPattern, searchOption);
		}

		internal static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string basePath, string searchPattern, SearchOption searchOption)
		{
			Path.Validate(basePath);
			SafeFindHandle findHandle = null;
			try
			{
				string text = Path.Combine(basePath, searchPattern);
				string text2;
				int num;
				int num2;
				try
				{
				}
				finally
				{
					findHandle = new SafeFindHandle(MonoIO.FindFirstFile(text, out text2, out num, out num2));
				}
				if (!findHandle.IsInvalid)
				{
					while (text2 != null)
					{
						if (!(text2 == ".") && !(text2 == ".."))
						{
							FileAttributes attrs = (FileAttributes)num;
							string fullPath = Path.Combine(basePath, text2);
							if ((attrs & FileAttributes.ReparsePoint) == (FileAttributes)0)
							{
								if ((attrs & FileAttributes.Directory) != (FileAttributes)0)
								{
									yield return new DirectoryInfo(fullPath);
								}
								else
								{
									yield return new FileInfo(fullPath);
								}
							}
							if ((attrs & FileAttributes.Directory) != (FileAttributes)0 && searchOption == SearchOption.AllDirectories)
							{
								foreach (FileSystemInfo fileSystemInfo in DirectoryInfo.EnumerateFileSystemInfos(fullPath, searchPattern, searchOption))
								{
									yield return fileSystemInfo;
								}
								IEnumerator<FileSystemInfo> enumerator = null;
							}
							fullPath = null;
						}
						int num3;
						if (!MonoIO.FindNextFile(findHandle.DangerousGetHandle(), out text2, out num, out num3))
						{
							goto JumpOutOfTryFinally-3;
						}
					}
					yield break;
				}
				MonoIOError monoIOError = (MonoIOError)num2;
				if (monoIOError != MonoIOError.ERROR_FILE_NOT_FOUND)
				{
					throw MonoIO.GetException(Path.GetDirectoryName(text), monoIOError);
				}
				yield break;
			}
			finally
			{
				if (findHandle != null)
				{
					findHandle.Dispose();
				}
			}
			JumpOutOfTryFinally-3:
			yield break;
			yield break;
		}

		internal void CheckPath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.");
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path.");
			}
			if (Environment.IsRunningOnWindows)
			{
				int num = path.IndexOf(':');
				if (num >= 0 && num != 1)
				{
					throw new ArgumentException("path");
				}
			}
		}

		private string current;

		private string parent;
	}
}
