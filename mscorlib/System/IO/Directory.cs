using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;

namespace System.IO
{
	[ComVisible(true)]
	public static class Directory
	{
		public static DirectoryInfo CreateDirectory(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("Path is empty");
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Path contains invalid chars");
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException("Only blank characters in path");
			}
			if (File.Exists(path))
			{
				throw new IOException("Cannot create " + path + " because a file with the same name already exists.");
			}
			if (path == ":")
			{
				throw new ArgumentException("Only ':' In path");
			}
			return Directory.CreateDirectoriesInternal(path);
		}

		[MonoTODO("DirectorySecurity not implemented")]
		public static DirectoryInfo CreateDirectory(string path, DirectorySecurity directorySecurity)
		{
			return Directory.CreateDirectory(path);
		}

		private static DirectoryInfo CreateDirectoriesInternal(string path)
		{
			if (SecurityManager.SecurityEnabled)
			{
				new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, path).Demand();
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(path, true);
			if (directoryInfo.Parent != null && !directoryInfo.Parent.Exists)
			{
				directoryInfo.Parent.Create();
			}
			MonoIOError monoIOError;
			if (!MonoIO.CreateDirectory(path, out monoIOError) && monoIOError != MonoIOError.ERROR_ALREADY_EXISTS && monoIOError != MonoIOError.ERROR_FILE_EXISTS)
			{
				throw MonoIO.GetException(path, monoIOError);
			}
			return directoryInfo;
		}

		public static void Delete(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("Path is empty");
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Path contains invalid chars");
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException("Only blank characters in path");
			}
			if (path == ":")
			{
				throw new NotSupportedException("Only ':' In path");
			}
			MonoIOError monoIOError;
			bool flag;
			if (MonoIO.ExistsSymlink(path, out monoIOError))
			{
				flag = MonoIO.DeleteFile(path, out monoIOError);
			}
			else
			{
				flag = MonoIO.RemoveDirectory(path, out monoIOError);
			}
			if (flag)
			{
				return;
			}
			if (monoIOError != MonoIOError.ERROR_FILE_NOT_FOUND)
			{
				throw MonoIO.GetException(path, monoIOError);
			}
			if (File.Exists(path))
			{
				throw new IOException("Directory does not exist, but a file of the same name exist.");
			}
			throw new DirectoryNotFoundException("Directory does not exist.");
		}

		private static void RecursiveDelete(string path)
		{
			foreach (string text in Directory.GetDirectories(path))
			{
				MonoIOError monoIOError;
				if (MonoIO.ExistsSymlink(text, out monoIOError))
				{
					MonoIO.DeleteFile(text, out monoIOError);
				}
				else
				{
					Directory.RecursiveDelete(text);
				}
			}
			foreach (string text2 in Directory.GetFiles(path))
			{
				File.Delete(text2);
			}
			Directory.Delete(path);
		}

		public static void Delete(string path, bool recursive)
		{
			Directory.CheckPathExceptions(path);
			if (recursive)
			{
				Directory.RecursiveDelete(path);
			}
			else
			{
				Directory.Delete(path);
			}
		}

		public static bool Exists(string path)
		{
			MonoIOError monoIOError;
			return path != null && MonoIO.ExistsDirectory(path, out monoIOError);
		}

		public static DateTime GetLastAccessTime(string path)
		{
			return File.GetLastAccessTime(path);
		}

		public static DateTime GetLastAccessTimeUtc(string path)
		{
			return Directory.GetLastAccessTime(path).ToUniversalTime();
		}

		public static DateTime GetLastWriteTime(string path)
		{
			return File.GetLastWriteTime(path);
		}

		public static DateTime GetLastWriteTimeUtc(string path)
		{
			return Directory.GetLastWriteTime(path).ToUniversalTime();
		}

		public static DateTime GetCreationTime(string path)
		{
			return File.GetCreationTime(path);
		}

		public static DateTime GetCreationTimeUtc(string path)
		{
			return Directory.GetCreationTime(path).ToUniversalTime();
		}

		public static string GetCurrentDirectory()
		{
			MonoIOError monoIOError;
			string currentDirectory = MonoIO.GetCurrentDirectory(out monoIOError);
			if (monoIOError != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(monoIOError);
			}
			if (currentDirectory != null && currentDirectory.Length > 0 && SecurityManager.SecurityEnabled)
			{
				new FileIOPermission(FileIOPermissionAccess.PathDiscovery, currentDirectory).Demand();
			}
			return currentDirectory;
		}

		public static string[] GetDirectories(string path)
		{
			return Directory.GetDirectories(path, "*");
		}

		public static string[] GetDirectories(string path, string searchPattern)
		{
			return Directory.GetFileSystemEntries(path, searchPattern, FileAttributes.Directory, FileAttributes.Directory);
		}

		public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			if (searchOption == SearchOption.TopDirectoryOnly)
			{
				return Directory.GetDirectories(path, searchPattern);
			}
			ArrayList arrayList = new ArrayList();
			Directory.GetDirectoriesRecurse(path, searchPattern, arrayList);
			return (string[])arrayList.ToArray(typeof(string));
		}

		private static void GetDirectoriesRecurse(string path, string searchPattern, ArrayList all)
		{
			all.AddRange(Directory.GetDirectories(path, searchPattern));
			foreach (string text in Directory.GetDirectories(path))
			{
				Directory.GetDirectoriesRecurse(text, searchPattern, all);
			}
		}

		public static string GetDirectoryRoot(string path)
		{
			return new string(Path.DirectorySeparatorChar, 1);
		}

		public static string[] GetFiles(string path)
		{
			return Directory.GetFiles(path, "*");
		}

		public static string[] GetFiles(string path, string searchPattern)
		{
			return Directory.GetFileSystemEntries(path, searchPattern, FileAttributes.Directory, (FileAttributes)0);
		}

		public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
		{
			if (searchOption == SearchOption.TopDirectoryOnly)
			{
				return Directory.GetFiles(path, searchPattern);
			}
			ArrayList arrayList = new ArrayList();
			Directory.GetFilesRecurse(path, searchPattern, arrayList);
			return (string[])arrayList.ToArray(typeof(string));
		}

		private static void GetFilesRecurse(string path, string searchPattern, ArrayList all)
		{
			all.AddRange(Directory.GetFiles(path, searchPattern));
			foreach (string text in Directory.GetDirectories(path))
			{
				Directory.GetFilesRecurse(text, searchPattern, all);
			}
		}

		public static string[] GetFileSystemEntries(string path)
		{
			return Directory.GetFileSystemEntries(path, "*");
		}

		public static string[] GetFileSystemEntries(string path, string searchPattern)
		{
			return Directory.GetFileSystemEntries(path, searchPattern, (FileAttributes)0, (FileAttributes)0);
		}

		public static string[] GetLogicalDrives()
		{
			return Environment.GetLogicalDrives();
		}

		private static bool IsRootDirectory(string path)
		{
			return (Path.DirectorySeparatorChar == '/' && path == "/") || (Path.DirectorySeparatorChar == '\\' && path.Length == 3 && path.EndsWith(":\\"));
		}

		public static DirectoryInfo GetParent(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Path contains invalid characters");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("The Path do not have a valid format");
			}
			if (Directory.IsRootDirectory(path))
			{
				return null;
			}
			string text = Path.GetDirectoryName(path);
			if (text.Length == 0)
			{
				text = Directory.GetCurrentDirectory();
			}
			return new DirectoryInfo(text);
		}

		public static void Move(string sourceDirName, string destDirName)
		{
			if (sourceDirName == null)
			{
				throw new ArgumentNullException("sourceDirName");
			}
			if (destDirName == null)
			{
				throw new ArgumentNullException("destDirName");
			}
			if (sourceDirName.Trim().Length == 0 || sourceDirName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Invalid source directory name: " + sourceDirName, "sourceDirName");
			}
			if (destDirName.Trim().Length == 0 || destDirName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Invalid target directory name: " + destDirName, "destDirName");
			}
			if (sourceDirName == destDirName)
			{
				throw new IOException("Source and destination path must be different.");
			}
			if (Directory.Exists(destDirName))
			{
				throw new IOException(destDirName + " already exists.");
			}
			if (!Directory.Exists(sourceDirName) && !File.Exists(sourceDirName))
			{
				throw new DirectoryNotFoundException(sourceDirName + " does not exist");
			}
			MonoIOError monoIOError;
			if (!MonoIO.MoveFile(sourceDirName, destDirName, out monoIOError))
			{
				throw MonoIO.GetException(monoIOError);
			}
		}

		public static void SetAccessControl(string path, DirectorySecurity directorySecurity)
		{
			throw new NotImplementedException();
		}

		public static void SetCreationTime(string path, DateTime creationTime)
		{
			File.SetCreationTime(path, creationTime);
		}

		public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
		{
			Directory.SetCreationTime(path, creationTimeUtc.ToLocalTime());
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public static void SetCurrentDirectory(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException("path string must not be an empty string or whitespace string");
			}
			if (!Directory.Exists(path))
			{
				throw new DirectoryNotFoundException("Directory \"" + path + "\" not found.");
			}
			MonoIOError monoIOError;
			MonoIO.SetCurrentDirectory(path, out monoIOError);
			if (monoIOError != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(path, monoIOError);
			}
		}

		public static void SetLastAccessTime(string path, DateTime lastAccessTime)
		{
			File.SetLastAccessTime(path, lastAccessTime);
		}

		public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
		{
			Directory.SetLastAccessTime(path, lastAccessTimeUtc.ToLocalTime());
		}

		public static void SetLastWriteTime(string path, DateTime lastWriteTime)
		{
			File.SetLastWriteTime(path, lastWriteTime);
		}

		public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
		{
			Directory.SetLastWriteTime(path, lastWriteTimeUtc.ToLocalTime());
		}

		private static void CheckPathExceptions(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("Path is Empty");
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException("Only blank characters in path");
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Path contains invalid chars");
			}
		}

		private static string[] GetFileSystemEntries(string path, string searchPattern, FileAttributes mask, FileAttributes attrs)
		{
			if (path == null || searchPattern == null)
			{
				throw new ArgumentNullException();
			}
			if (searchPattern.Length == 0)
			{
				return new string[0];
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException("The Path does not have a valid format");
			}
			string text = Path.Combine(path, searchPattern);
			string directoryName = Path.GetDirectoryName(text);
			if (directoryName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Path contains invalid characters");
			}
			MonoIOError monoIOError;
			if (directoryName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				if (path.IndexOfAny(SearchPattern.InvalidChars) == -1)
				{
					throw new ArgumentException("Path contains invalid characters", "path");
				}
				throw new ArgumentException("Pattern contains invalid characters", "pattern");
			}
			else if (!MonoIO.ExistsDirectory(directoryName, out monoIOError))
			{
				MonoIOError monoIOError2;
				if (monoIOError == MonoIOError.ERROR_SUCCESS && MonoIO.ExistsFile(directoryName, out monoIOError2))
				{
					return new string[] { directoryName };
				}
				if (monoIOError != MonoIOError.ERROR_PATH_NOT_FOUND)
				{
					throw MonoIO.GetException(directoryName, monoIOError);
				}
				if (directoryName.IndexOfAny(SearchPattern.WildcardChars) == -1)
				{
					throw new DirectoryNotFoundException("Directory '" + directoryName + "' not found.");
				}
				if (path.IndexOfAny(SearchPattern.WildcardChars) == -1)
				{
					throw new ArgumentException("Pattern is invalid", "searchPattern");
				}
				throw new ArgumentException("Path is invalid", "path");
			}
			else
			{
				string text2 = Path.Combine(directoryName, searchPattern);
				string[] fileSystemEntries = MonoIO.GetFileSystemEntries(path, text2, (int)attrs, (int)mask, out monoIOError);
				if (monoIOError != MonoIOError.ERROR_SUCCESS)
				{
					throw MonoIO.GetException(directoryName, monoIOError);
				}
				return fileSystemEntries;
			}
		}

		[MonoNotSupported("DirectorySecurity isn't implemented")]
		public static DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)
		{
			throw new PlatformNotSupportedException();
		}

		[MonoNotSupported("DirectorySecurity isn't implemented")]
		public static DirectorySecurity GetAccessControl(string path)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
