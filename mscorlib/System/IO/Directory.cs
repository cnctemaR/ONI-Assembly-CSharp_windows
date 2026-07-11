using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;

namespace System.IO
{
	[ComVisible(true)]
	public static class Directory
	{
		public static string[] GetFiles(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return Directory.InternalGetFiles(path, "*", SearchOption.TopDirectoryOnly);
		}

		public static string[] GetFiles(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return Directory.InternalGetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("Enum value was out of legal range."));
			}
			return Directory.InternalGetFiles(path, searchPattern, searchOption);
		}

		private static string[] InternalGetFiles(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.InternalGetFileDirectoryNames(path, path, searchPattern, true, false, searchOption, true);
		}

		[SecurityCritical]
		internal static string[] UnsafeGetFiles(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.InternalGetFileDirectoryNames(path, path, searchPattern, true, false, searchOption, false);
		}

		public static string[] GetDirectories(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return Directory.InternalGetDirectories(path, "*", SearchOption.TopDirectoryOnly);
		}

		public static string[] GetDirectories(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return Directory.InternalGetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("Enum value was out of legal range."));
			}
			return Directory.InternalGetDirectories(path, searchPattern, searchOption);
		}

		private static string[] InternalGetDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.InternalGetFileDirectoryNames(path, path, searchPattern, false, true, searchOption, true);
		}

		[SecurityCritical]
		internal static string[] UnsafeGetDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.InternalGetFileDirectoryNames(path, path, searchPattern, false, true, searchOption, false);
		}

		public static string[] GetFileSystemEntries(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return Directory.InternalGetFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);
		}

		public static string[] GetFileSystemEntries(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return Directory.InternalGetFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		public static string[] GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("Enum value was out of legal range."));
			}
			return Directory.InternalGetFileSystemEntries(path, searchPattern, searchOption);
		}

		private static string[] InternalGetFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.InternalGetFileDirectoryNames(path, path, searchPattern, true, true, searchOption, true);
		}

		internal static string[] InternalGetFileDirectoryNames(string path, string userPathOriginal, string searchPattern, bool includeFiles, bool includeDirs, SearchOption searchOption, bool checkHost)
		{
			return new List<string>(FileSystemEnumerableFactory.CreateFileNameIterator(path, userPathOriginal, searchPattern, includeFiles, includeDirs, searchOption, checkHost)).ToArray();
		}

		public static IEnumerable<string> EnumerateDirectories(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return Directory.InternalEnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
		}

		public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return Directory.InternalEnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("Enum value was out of legal range."));
			}
			return Directory.InternalEnumerateDirectories(path, searchPattern, searchOption);
		}

		private static IEnumerable<string> InternalEnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.EnumerateFileSystemNames(path, searchPattern, searchOption, false, true);
		}

		public static IEnumerable<string> EnumerateFiles(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return Directory.InternalEnumerateFiles(path, "*", SearchOption.TopDirectoryOnly);
		}

		public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return Directory.InternalEnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("Enum value was out of legal range."));
			}
			return Directory.InternalEnumerateFiles(path, searchPattern, searchOption);
		}

		private static IEnumerable<string> InternalEnumerateFiles(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.EnumerateFileSystemNames(path, searchPattern, searchOption, true, false);
		}

		public static IEnumerable<string> EnumerateFileSystemEntries(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return Directory.InternalEnumerateFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);
		}

		public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return Directory.InternalEnumerateFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("Enum value was out of legal range."));
			}
			return Directory.InternalEnumerateFileSystemEntries(path, searchPattern, searchOption);
		}

		private static IEnumerable<string> InternalEnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.EnumerateFileSystemNames(path, searchPattern, searchOption, true, true);
		}

		private static IEnumerable<string> EnumerateFileSystemNames(string path, string searchPattern, SearchOption searchOption, bool includeFiles, bool includeDirs)
		{
			return FileSystemEnumerableFactory.CreateFileNameIterator(path, path, searchPattern, includeFiles, includeDirs, searchOption, true);
		}

		public static string GetDirectoryRoot(string path)
		{
			Path.Validate(path);
			return new string(Path.DirectorySeparatorChar, 1);
		}

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
			if (Environment.IsRunningOnWindows && path == ":")
			{
				throw new ArgumentException("Only ':' In path");
			}
			return Directory.CreateDirectoriesInternal(path);
		}

		[MonoLimitation("DirectorySecurity not implemented")]
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
			if (!MonoIO.CreateDirectory(directoryInfo.FullName, out monoIOError) && monoIOError != MonoIOError.ERROR_ALREADY_EXISTS && monoIOError != MonoIOError.ERROR_FILE_EXISTS)
			{
				throw MonoIO.GetException(path, monoIOError);
			}
			return directoryInfo;
		}

		public static void Delete(string path)
		{
			Path.Validate(path);
			if (Environment.IsRunningOnWindows && path == ":")
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
				throw new IOException("Directory does not exist, but a file of the same name exists.");
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
			string[] array = Directory.GetFiles(path);
			for (int i = 0; i < array.Length; i++)
			{
				File.Delete(array[i]);
			}
			Directory.Delete(path);
		}

		public static void Delete(string path, bool recursive)
		{
			Path.Validate(path);
			if (recursive)
			{
				Directory.RecursiveDelete(path);
				return;
			}
			Directory.Delete(path);
		}

		public static bool Exists(string path)
		{
			if (path == null)
			{
				return false;
			}
			if (!SecurityManager.CheckElevatedPermissions())
			{
				return false;
			}
			string fullPath;
			try
			{
				fullPath = Path.GetFullPath(path);
			}
			catch
			{
				return false;
			}
			MonoIOError monoIOError;
			return MonoIO.ExistsDirectory(fullPath, out monoIOError);
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
			string text = Directory.InsecureGetCurrentDirectory();
			if (text != null && text.Length > 0 && SecurityManager.SecurityEnabled)
			{
				new FileIOPermission(FileIOPermissionAccess.PathDiscovery, text).Demand();
			}
			return text;
		}

		internal static string InsecureGetCurrentDirectory()
		{
			MonoIOError monoIOError;
			string currentDirectory = MonoIO.GetCurrentDirectory(out monoIOError);
			if (monoIOError != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(monoIOError);
			}
			return currentDirectory;
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
			Path.Validate(path);
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
			if (directorySecurity == null)
			{
				throw new ArgumentNullException("directorySecurity");
			}
			directorySecurity.PersistModifications(path);
		}

		public static void SetCreationTime(string path, DateTime creationTime)
		{
			File.SetCreationTime(path, creationTime);
		}

		public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
		{
			Directory.SetCreationTime(path, creationTimeUtc.ToLocalTime());
		}

		[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
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

		public static DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)
		{
			return new DirectorySecurity(path, includeSections);
		}

		public static DirectorySecurity GetAccessControl(string path)
		{
			return Directory.GetAccessControl(path, AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
		}

		internal static string GetDemandDir(string fullPath, bool thisDirOnly)
		{
			string text;
			if (thisDirOnly)
			{
				if (fullPath.EndsWith(Path.DirectorySeparatorChar) || fullPath.EndsWith(Path.AltDirectorySeparatorChar))
				{
					text = fullPath + ".";
				}
				else
				{
					text = fullPath + Path.DirectorySeparatorCharAsString + ".";
				}
			}
			else if (!fullPath.EndsWith(Path.DirectorySeparatorChar) && !fullPath.EndsWith(Path.AltDirectorySeparatorChar))
			{
				text = fullPath + Path.DirectorySeparatorCharAsString;
			}
			else
			{
				text = fullPath;
			}
			return text;
		}

		internal sealed class SearchData
		{
			public SearchData(string fullPath, string userPath, SearchOption searchOption)
			{
				this.fullPath = fullPath;
				this.userPath = userPath;
				this.searchOption = searchOption;
			}

			public readonly string fullPath;

			public readonly string userPath;

			public readonly SearchOption searchOption;
		}
	}
}
