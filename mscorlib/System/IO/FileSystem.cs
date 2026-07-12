using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO
{
	internal static class FileSystem
	{
		public static void CopyFile(string sourceFullPath, string destFullPath, bool overwrite)
		{
			int num = FileSystem.UnityCopyFile(sourceFullPath, destFullPath, !overwrite);
			if (num != 0)
			{
				string text = destFullPath;
				if (num != 80)
				{
					using (SafeFileHandle safeFileHandle = Interop.Kernel32.CreateFile(sourceFullPath, int.MinValue, FileShare.Read, FileMode.Open, 0))
					{
						if (safeFileHandle.IsInvalid)
						{
							text = sourceFullPath;
						}
					}
					if (num == 5 && FileSystem.DirectoryExists(destFullPath))
					{
						throw new IOException(SR.Format("The target file '{0}' is a directory, not a file.", destFullPath), 5);
					}
				}
				throw Win32Marshal.GetExceptionForWin32Error(num, text);
			}
		}

		public static void ReplaceFile(string sourceFullPath, string destFullPath, string destBackupFullPath, bool ignoreMetadataErrors)
		{
			int num = (ignoreMetadataErrors ? 2 : 0);
			if (!Interop.Kernel32.ReplaceFile(destFullPath, sourceFullPath, destBackupFullPath, num, IntPtr.Zero, IntPtr.Zero))
			{
				throw Win32Marshal.GetExceptionForWin32Error(Marshal.GetLastWin32Error(), "");
			}
		}

		public static void CreateDirectory(string fullPath)
		{
			if (FileSystem.DirectoryExists(fullPath))
			{
				return;
			}
			List<string> list = new List<string>();
			bool flag = false;
			int num = fullPath.Length;
			if (num >= 2 && PathInternal.EndsInDirectorySeparator(fullPath))
			{
				num--;
			}
			int rootLength = PathInternal.GetRootLength(fullPath);
			if (num > rootLength)
			{
				int num2 = num - 1;
				while (num2 >= rootLength && !flag)
				{
					string text = fullPath.Substring(0, num2 + 1);
					if (!FileSystem.DirectoryExists(text))
					{
						list.Add(text);
					}
					else
					{
						flag = true;
					}
					while (num2 > rootLength && !PathInternal.IsDirectorySeparator(fullPath[num2]))
					{
						num2--;
					}
					num2--;
				}
			}
			int count = list.Count;
			bool flag2 = true;
			int num3 = 0;
			string text2 = fullPath;
			while (list.Count > 0)
			{
				string text3 = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
				flag2 = FileSystem.UnityCreateDirectory(text3);
				if (!flag2 && num3 == 0)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error != 183)
					{
						num3 = lastWin32Error;
					}
					else if (FileSystem.FileExists(text3) || (!FileSystem.DirectoryExists(text3, out lastWin32Error) && lastWin32Error == 5))
					{
						num3 = lastWin32Error;
						text2 = text3;
					}
				}
			}
			if (count == 0 && !flag)
			{
				string text4 = Directory.InternalGetDirectoryRoot(fullPath);
				if (!FileSystem.DirectoryExists(text4))
				{
					throw Win32Marshal.GetExceptionForWin32Error(3, text4);
				}
				return;
			}
			else
			{
				if (!flag2 && num3 != 0)
				{
					throw Win32Marshal.GetExceptionForWin32Error(num3, text2);
				}
				return;
			}
		}

		public static void DeleteFile(string fullPath)
		{
			if (FileSystem.UnityDeleteFile(fullPath))
			{
				return;
			}
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error == 2)
			{
				return;
			}
			throw Win32Marshal.GetExceptionForWin32Error(lastWin32Error, fullPath);
		}

		public static bool DirectoryExists(string fullPath)
		{
			int num;
			return FileSystem.DirectoryExists(fullPath, out num);
		}

		private static bool DirectoryExists(string path, out int lastError)
		{
			Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA);
			lastError = FileSystem.FillAttributeInfo(path, ref win32_FILE_ATTRIBUTE_DATA, true);
			return lastError == 0 && win32_FILE_ATTRIBUTE_DATA.dwFileAttributes != -1 && (win32_FILE_ATTRIBUTE_DATA.dwFileAttributes & 16) != 0;
		}

		internal static int FillAttributeInfo(string path, ref Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA data, bool returnErrorOnNotFound)
		{
			int num = 0;
			path = PathInternal.TrimEndingDirectorySeparator(path);
			using (DisableMediaInsertionPrompt.Create())
			{
				if (!FileSystem.UnityGetFileAttributesEx(path, ref data))
				{
					num = Marshal.GetLastWin32Error();
					if (num != 2 && num != 3 && num != 21 && num != 123 && num != 161 && num != 53 && num != 67 && num != 87 && num != 1231)
					{
						Interop.Kernel32.WIN32_FIND_DATA win32_FIND_DATA = default(Interop.Kernel32.WIN32_FIND_DATA);
						using (SafeFindHandle safeFindHandle = FileSystem.UnityFindFirstFile(path, ref win32_FIND_DATA))
						{
							if (safeFindHandle.IsInvalid)
							{
								num = Marshal.GetLastWin32Error();
							}
							else
							{
								num = 0;
								data.PopulateFrom(ref win32_FIND_DATA);
							}
						}
					}
				}
			}
			if (num != 0 && !returnErrorOnNotFound && (num - 2 <= 1 || num == 21))
			{
				data.dwFileAttributes = -1;
				return 0;
			}
			return num;
		}

		public static bool FileExists(string fullPath)
		{
			Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA);
			return FileSystem.FillAttributeInfo(fullPath, ref win32_FILE_ATTRIBUTE_DATA, true) == 0 && win32_FILE_ATTRIBUTE_DATA.dwFileAttributes != -1 && (win32_FILE_ATTRIBUTE_DATA.dwFileAttributes & 16) == 0;
		}

		public static FileAttributes GetAttributes(string fullPath)
		{
			Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA);
			int num = FileSystem.FillAttributeInfo(fullPath, ref win32_FILE_ATTRIBUTE_DATA, true);
			if (num != 0)
			{
				throw Win32Marshal.GetExceptionForWin32Error(num, fullPath);
			}
			return (FileAttributes)win32_FILE_ATTRIBUTE_DATA.dwFileAttributes;
		}

		public static DateTimeOffset GetCreationTime(string fullPath)
		{
			Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA);
			int num = FileSystem.FillAttributeInfo(fullPath, ref win32_FILE_ATTRIBUTE_DATA, false);
			if (num != 0)
			{
				throw Win32Marshal.GetExceptionForWin32Error(num, fullPath);
			}
			return win32_FILE_ATTRIBUTE_DATA.ftCreationTime.ToDateTimeOffset();
		}

		public static FileSystemInfo GetFileSystemInfo(string fullPath, bool asDirectory)
		{
			if (!asDirectory)
			{
				return new FileInfo(fullPath, null, null, false);
			}
			return new DirectoryInfo(fullPath, null, null, false);
		}

		public static DateTimeOffset GetLastAccessTime(string fullPath)
		{
			Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA);
			int num = FileSystem.FillAttributeInfo(fullPath, ref win32_FILE_ATTRIBUTE_DATA, false);
			if (num != 0)
			{
				throw Win32Marshal.GetExceptionForWin32Error(num, fullPath);
			}
			return win32_FILE_ATTRIBUTE_DATA.ftLastAccessTime.ToDateTimeOffset();
		}

		public static DateTimeOffset GetLastWriteTime(string fullPath)
		{
			Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA);
			int num = FileSystem.FillAttributeInfo(fullPath, ref win32_FILE_ATTRIBUTE_DATA, false);
			if (num != 0)
			{
				throw Win32Marshal.GetExceptionForWin32Error(num, fullPath);
			}
			return win32_FILE_ATTRIBUTE_DATA.ftLastWriteTime.ToDateTimeOffset();
		}

		public static void MoveDirectory(string sourceFullPath, string destFullPath)
		{
			if (FileSystem.UnityMoveFile(sourceFullPath, destFullPath))
			{
				return;
			}
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error == 2)
			{
				throw Win32Marshal.GetExceptionForWin32Error(3, sourceFullPath);
			}
			if (lastWin32Error == 5)
			{
				throw new IOException(SR.Format("Access to the path '{0}' is denied.", sourceFullPath), Win32Marshal.MakeHRFromErrorCode(lastWin32Error));
			}
			throw Win32Marshal.GetExceptionForWin32Error(lastWin32Error, "");
		}

		public static void MoveFile(string sourceFullPath, string destFullPath)
		{
			if (!FileSystem.UnityMoveFile(sourceFullPath, destFullPath))
			{
				throw Win32Marshal.GetExceptionForLastWin32Error("");
			}
		}

		private static SafeFileHandle OpenHandle(string fullPath, bool asDirectory)
		{
			string text = fullPath.Substring(0, PathInternal.GetRootLength(fullPath));
			if (text == fullPath && text[1] == Path.VolumeSeparatorChar)
			{
				throw new ArgumentException("Path must not be a drive.", "path");
			}
			SafeFileHandle safeFileHandle = Interop.Kernel32.CreateFile(fullPath, 1073741824, FileShare.Read | FileShare.Write | FileShare.Delete, FileMode.Open, asDirectory ? 33554432 : 0);
			if (safeFileHandle.IsInvalid)
			{
				int num = Marshal.GetLastWin32Error();
				if (!asDirectory && num == 3 && fullPath.Equals(Directory.GetDirectoryRoot(fullPath)))
				{
					num = 5;
				}
				throw Win32Marshal.GetExceptionForWin32Error(num, fullPath);
			}
			return safeFileHandle;
		}

		public static void RemoveDirectory(string fullPath, bool recursive)
		{
			if (!recursive)
			{
				FileSystem.RemoveDirectoryInternal(fullPath, true, false);
				return;
			}
			Interop.Kernel32.WIN32_FIND_DATA win32_FIND_DATA = default(Interop.Kernel32.WIN32_FIND_DATA);
			FileSystem.GetFindData(fullPath, ref win32_FIND_DATA);
			if (FileSystem.IsNameSurrogateReparsePoint(ref win32_FIND_DATA))
			{
				FileSystem.RemoveDirectoryInternal(fullPath, true, false);
				return;
			}
			fullPath = PathInternal.EnsureExtendedPrefix(fullPath);
			FileSystem.RemoveDirectoryRecursive(fullPath, ref win32_FIND_DATA, true);
		}

		private static void GetFindData(string fullPath, ref Interop.Kernel32.WIN32_FIND_DATA findData)
		{
			using (SafeFindHandle safeFindHandle = FileSystem.UnityFindFirstFile(PathInternal.TrimEndingDirectorySeparator(fullPath), ref findData))
			{
				if (safeFindHandle.IsInvalid)
				{
					int num = Marshal.GetLastWin32Error();
					if (num == 2)
					{
						num = 3;
					}
					throw Win32Marshal.GetExceptionForWin32Error(num, fullPath);
				}
			}
		}

		private static bool IsNameSurrogateReparsePoint(ref Interop.Kernel32.WIN32_FIND_DATA data)
		{
			return (data.dwFileAttributes & 1024U) != 0U && (data.dwReserved0 & 536870912U) > 0U;
		}

		private static void RemoveDirectoryRecursive(string fullPath, ref Interop.Kernel32.WIN32_FIND_DATA findData, bool topLevel)
		{
			Exception ex = null;
			using (SafeFindHandle safeFindHandle = FileSystem.UnityFindFirstFile(Path.Join(fullPath, "*"), ref findData))
			{
				if (safeFindHandle.IsInvalid)
				{
					throw Win32Marshal.GetExceptionForLastWin32Error(fullPath);
				}
				int num;
				do
				{
					if ((findData.dwFileAttributes & 16U) == 0U)
					{
						string stringFromFixedBuffer = findData.cFileName.GetStringFromFixedBuffer();
						if (!FileSystem.UnityDeleteFile(Path.Combine(fullPath, stringFromFixedBuffer)) && ex == null)
						{
							num = Marshal.GetLastWin32Error();
							if (num != 2)
							{
								ex = Win32Marshal.GetExceptionForWin32Error(num, stringFromFixedBuffer);
							}
						}
					}
					else if (!findData.cFileName.FixedBufferEqualsString(".") && !findData.cFileName.FixedBufferEqualsString(".."))
					{
						string stringFromFixedBuffer2 = findData.cFileName.GetStringFromFixedBuffer();
						if (!FileSystem.IsNameSurrogateReparsePoint(ref findData))
						{
							try
							{
								FileSystem.RemoveDirectoryRecursive(Path.Combine(fullPath, stringFromFixedBuffer2), ref findData, false);
								goto IL_013D;
							}
							catch (Exception ex2)
							{
								if (ex == null)
								{
									ex = ex2;
								}
								goto IL_013D;
							}
						}
						if (findData.dwReserved0 == 2684354563U && !Interop.Kernel32.DeleteVolumeMountPoint(Path.Join(fullPath, stringFromFixedBuffer2, "\\")) && ex == null)
						{
							num = Marshal.GetLastWin32Error();
							if (num != 0 && num != 3)
							{
								ex = Win32Marshal.GetExceptionForWin32Error(num, stringFromFixedBuffer2);
							}
						}
						if (!FileSystem.UnityRemoveDirectory(Path.Combine(fullPath, stringFromFixedBuffer2)) && ex == null)
						{
							num = Marshal.GetLastWin32Error();
							if (num != 3)
							{
								ex = Win32Marshal.GetExceptionForWin32Error(num, stringFromFixedBuffer2);
							}
						}
					}
					IL_013D:;
				}
				while (FileSystem.UnityFindNextFile(safeFindHandle, ref findData));
				if (ex != null)
				{
					throw ex;
				}
				num = Marshal.GetLastWin32Error();
				if (num != 0 && num != 18)
				{
					throw Win32Marshal.GetExceptionForWin32Error(num, fullPath);
				}
			}
			FileSystem.RemoveDirectoryInternal(fullPath, topLevel, true);
		}

		private static void RemoveDirectoryInternal(string fullPath, bool topLevel, bool allowDirectoryNotEmpty = false)
		{
			if (!FileSystem.UnityRemoveDirectory(fullPath))
			{
				int num = Marshal.GetLastWin32Error();
				switch (num)
				{
				case 2:
					num = 3;
					break;
				case 3:
					break;
				case 4:
					goto IL_004B;
				case 5:
					throw new IOException(SR.Format("Access to the path '{0}' is denied.", fullPath));
				default:
					if (num != 145)
					{
						goto IL_004B;
					}
					if (allowDirectoryNotEmpty)
					{
						return;
					}
					goto IL_004B;
				}
				if (!topLevel)
				{
					return;
				}
				IL_004B:
				throw Win32Marshal.GetExceptionForWin32Error(num, fullPath);
			}
		}

		public static void SetAttributes(string fullPath, FileAttributes attributes)
		{
			if (FileSystem.UnitySetFileAttributes(fullPath, attributes))
			{
				return;
			}
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error == 87)
			{
				throw new ArgumentException("Invalid File or Directory attributes value.", "attributes");
			}
			throw Win32Marshal.GetExceptionForWin32Error(lastWin32Error, fullPath);
		}

		public static void SetCreationTime(string fullPath, DateTimeOffset time, bool asDirectory)
		{
			using (SafeFileHandle safeFileHandle = FileSystem.OpenHandle(fullPath, asDirectory))
			{
				if (!Interop.Kernel32.SetFileTime(safeFileHandle, time.ToFileTime(), -1L, -1L, -1L, 0U))
				{
					throw Win32Marshal.GetExceptionForLastWin32Error(fullPath);
				}
			}
		}

		public static void SetLastAccessTime(string fullPath, DateTimeOffset time, bool asDirectory)
		{
			using (SafeFileHandle safeFileHandle = FileSystem.OpenHandle(fullPath, asDirectory))
			{
				if (!Interop.Kernel32.SetFileTime(safeFileHandle, -1L, time.ToFileTime(), -1L, -1L, 0U))
				{
					throw Win32Marshal.GetExceptionForLastWin32Error(fullPath);
				}
			}
		}

		public static void SetLastWriteTime(string fullPath, DateTimeOffset time, bool asDirectory)
		{
			using (SafeFileHandle safeFileHandle = FileSystem.OpenHandle(fullPath, asDirectory))
			{
				if (!Interop.Kernel32.SetFileTime(safeFileHandle, -1L, -1L, time.ToFileTime(), -1L, 0U))
				{
					throw Win32Marshal.GetExceptionForLastWin32Error(fullPath);
				}
			}
		}

		public static string[] GetLogicalDrives()
		{
			return DriveInfoInternal.GetLogicalDrives();
		}

		private static bool UnityCreateDirectory(string name)
		{
			Interop.Kernel32.SECURITY_ATTRIBUTES security_ATTRIBUTES = default(Interop.Kernel32.SECURITY_ATTRIBUTES);
			return Interop.Kernel32.CreateDirectory(name, ref security_ATTRIBUTES);
		}

		private static bool UnityRemoveDirectory(string fullPath)
		{
			return Interop.Kernel32.RemoveDirectory(fullPath);
		}

		private static bool UnityGetFileAttributesEx(string path, ref Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA data)
		{
			if ((path.StartsWith("\\?\\") || path.StartsWith("\\\\?\\")) && path.Contains("GLOBALROOT\\Device\\Harddisk") && path.Length - path.IndexOf("Partition") <= 11 && path[path.Length - 1] != '\\')
			{
				path += "\\";
			}
			return Interop.Kernel32.GetFileAttributesEx(path, Interop.Kernel32.GET_FILEEX_INFO_LEVELS.GetFileExInfoStandard, ref data);
		}

		private static bool UnitySetFileAttributes(string fullPath, FileAttributes attributes)
		{
			return Interop.Kernel32.SetFileAttributes(fullPath, (int)attributes);
		}

		internal static IntPtr UnityCreateFile_IntPtr(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, FileMode dwCreationDisposition, int dwFlagsAndAttributes)
		{
			return Interop.Kernel32.CreateFile_IntPtr(lpFileName, dwDesiredAccess, dwShareMode, dwCreationDisposition, dwFlagsAndAttributes);
		}

		private static int UnityCopyFile(string sourceFullPath, string destFullPath, bool failIfExists)
		{
			return Interop.Kernel32.CopyFile(sourceFullPath, destFullPath, failIfExists);
		}

		private static bool UnityDeleteFile(string path)
		{
			return Interop.Kernel32.DeleteFile(path);
		}

		private static bool UnityMoveFile(string sourceFullPath, string destFullPath)
		{
			return Interop.Kernel32.MoveFile(sourceFullPath, destFullPath);
		}

		private static SafeFindHandle UnityFindFirstFile(string path, ref Interop.Kernel32.WIN32_FIND_DATA findData)
		{
			return Interop.Kernel32.FindFirstFile(path, ref findData);
		}

		private static bool UnityFindNextFile(SafeFindHandle handle, ref Interop.Kernel32.WIN32_FIND_DATA findData)
		{
			bool flag = false;
			bool flag2 = false;
			if (!flag)
			{
				flag2 = Interop.Kernel32.FindNextFile(handle, ref findData);
			}
			return flag2;
		}

		internal const int GENERIC_READ = -2147483648;
	}
}
