using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.AccessControl;

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
			base.CheckPath(path);
			this.FullPath = Path.GetFullPath(path);
			if (simpleOriginalPath)
			{
				this.OriginalPath = Path.GetFileName(path);
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
			}
			else
			{
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
					this.parent += Path.DirectorySeparatorChar;
				}
			}
		}

		public override bool Exists
		{
			get
			{
				base.Refresh(false);
				return this.stat.Attributes != MonoIO.InvalidFileAttributes && (this.stat.Attributes & FileAttributes.Directory) != (FileAttributes)0;
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
			base.CheckPath(path);
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
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (!Directory.Exists(this.FullPath))
			{
				throw new IOException("Invalid directory");
			}
			string[] directories = Directory.GetDirectories(this.FullPath, searchPattern);
			string[] files = Directory.GetFiles(this.FullPath, searchPattern);
			FileSystemInfo[] array = new FileSystemInfo[directories.Length + files.Length];
			int num = 0;
			foreach (string text in directories)
			{
				array[num++] = new DirectoryInfo(text);
			}
			foreach (string text2 in files)
			{
				array[num++] = new FileInfo(text2);
			}
			return array;
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
		}

		public override string ToString()
		{
			return this.OriginalPath;
		}

		public DirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
		{
			if (searchOption == SearchOption.TopDirectoryOnly)
			{
				return this.GetDirectories(searchPattern);
			}
			if (searchOption != SearchOption.AllDirectories)
			{
				string text = Locale.GetText("Invalid enum value '{0}' for '{1}'.", new object[] { searchOption, "SearchOption" });
				throw new ArgumentOutOfRangeException("searchOption", text);
			}
			Queue queue = new Queue(this.GetDirectories(searchPattern));
			Queue queue2 = new Queue();
			while (queue.Count > 0)
			{
				DirectoryInfo directoryInfo = (DirectoryInfo)queue.Dequeue();
				DirectoryInfo[] directories = directoryInfo.GetDirectories(searchPattern);
				foreach (DirectoryInfo directoryInfo2 in directories)
				{
					queue.Enqueue(directoryInfo2);
				}
				queue2.Enqueue(directoryInfo);
			}
			DirectoryInfo[] array2 = new DirectoryInfo[queue2.Count];
			queue2.CopyTo(array2, 0);
			return array2;
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

		[MonoNotSupported("DirectorySecurity isn't implemented")]
		public DirectorySecurity GetAccessControl()
		{
			throw new UnauthorizedAccessException();
		}

		[MonoNotSupported("DirectorySecurity isn't implemented")]
		public DirectorySecurity GetAccessControl(AccessControlSections includeSections)
		{
			throw new UnauthorizedAccessException();
		}

		[MonoLimitation("DirectorySecurity isn't implemented")]
		public void SetAccessControl(DirectorySecurity directorySecurity)
		{
			if (directorySecurity != null)
			{
				throw new ArgumentNullException("directorySecurity");
			}
			throw new UnauthorizedAccessException();
		}

		private string current;

		private string parent;
	}
}
