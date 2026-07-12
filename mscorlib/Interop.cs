using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

internal static class Interop
{
	internal unsafe static void GetRandomBytes(byte* buffer, int length)
	{
		Interop.BCrypt.NTSTATUS ntstatus = Interop.BCrypt.BCryptGenRandom(IntPtr.Zero, buffer, length, 2);
		if (ntstatus == Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
		{
			return;
		}
		if (ntstatus == (Interop.BCrypt.NTSTATUS)3221225495U)
		{
			throw new OutOfMemoryException();
		}
		throw new InvalidOperationException();
	}

	internal static IntPtr MemAlloc(UIntPtr sizeInBytes)
	{
		IntPtr intPtr = Interop.mincore.HeapAlloc(Interop.mincore.GetProcessHeap(), 0U, sizeInBytes);
		if (intPtr == IntPtr.Zero)
		{
			throw new OutOfMemoryException();
		}
		return intPtr;
	}

	internal static void MemFree(IntPtr allocatedMemory)
	{
		Interop.mincore.HeapFree(Interop.mincore.GetProcessHeap(), 0U, allocatedMemory);
	}

	internal static class Kernel32
	{
		internal static int CopyFile(string src, string dst, bool failIfExists)
		{
			int num = (failIfExists ? 1 : 0);
			int num2 = 0;
			if (!Interop.Kernel32.CopyFileEx(src, dst, IntPtr.Zero, IntPtr.Zero, ref num2, num))
			{
				return Marshal.GetLastWin32Error();
			}
			return 0;
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "DeleteVolumeMountPointW", SetLastError = true)]
		internal static extern bool DeleteVolumeMountPointPrivate(string mountPoint);

		internal static bool DeleteVolumeMountPoint(string mountPoint)
		{
			mountPoint = PathInternal.EnsureExtendedPrefixIfNeeded(mountPoint);
			return Interop.Kernel32.DeleteVolumeMountPointPrivate(mountPoint);
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryExW", SetLastError = true)]
		internal static extern SafeLibraryHandle LoadLibraryEx(string libFilename, IntPtr reserved, int flags);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal static extern bool GetFileMUIPath(uint flags, string filePath, [Out] StringBuilder language, ref int languageLength, [Out] StringBuilder fileMuiPath, ref int fileMuiPathLength, ref long enumerator);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal static extern uint GetDynamicTimeZoneInformation(out Interop.Kernel32.TIME_DYNAMIC_ZONE_INFORMATION pTimeZoneInformation);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal static extern uint GetTimeZoneInformation(out Interop.Kernel32.TIME_ZONE_INFORMATION lpTimeZoneInformation);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "CopyFileExW", SetLastError = true)]
		private static extern bool CopyFileExPrivate(string src, string dst, IntPtr progressRoutine, IntPtr progressData, ref int cancel, int flags);

		internal static bool CopyFileEx(string src, string dst, IntPtr progressRoutine, IntPtr progressData, ref int cancel, int flags)
		{
			src = PathInternal.EnsureExtendedPrefixIfNeeded(src);
			dst = PathInternal.EnsureExtendedPrefixIfNeeded(dst);
			return Interop.Kernel32.CopyFileExPrivate(src, dst, progressRoutine, progressData, ref cancel, flags);
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "CreateDirectoryW", SetLastError = true)]
		private static extern bool CreateDirectoryPrivate(string path, ref Interop.Kernel32.SECURITY_ATTRIBUTES lpSecurityAttributes);

		internal static bool CreateDirectory(string path, ref Interop.Kernel32.SECURITY_ATTRIBUTES lpSecurityAttributes)
		{
			path = PathInternal.EnsureExtendedPrefix(path);
			return Interop.Kernel32.CreateDirectoryPrivate(path, ref lpSecurityAttributes);
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "CreateFileW", ExactSpelling = true, SetLastError = true)]
		private unsafe static extern IntPtr CreateFilePrivate(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, Interop.Kernel32.SECURITY_ATTRIBUTES* securityAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

		internal unsafe static SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, ref Interop.Kernel32.SECURITY_ATTRIBUTES securityAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile)
		{
			lpFileName = PathInternal.EnsureExtendedPrefixIfNeeded(lpFileName);
			fixed (Interop.Kernel32.SECURITY_ATTRIBUTES* ptr = &securityAttrs)
			{
				Interop.Kernel32.SECURITY_ATTRIBUTES* ptr2 = ptr;
				IntPtr intPtr = Interop.Kernel32.CreateFilePrivate(lpFileName, dwDesiredAccess, dwShareMode, ptr2, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
				SafeFileHandle safeFileHandle;
				try
				{
					safeFileHandle = new SafeFileHandle(intPtr, true);
				}
				catch
				{
					Interop.Kernel32.CloseHandle(intPtr);
					throw;
				}
				return safeFileHandle;
			}
		}

		internal static SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, FileMode dwCreationDisposition, int dwFlagsAndAttributes)
		{
			IntPtr intPtr = Interop.Kernel32.CreateFile_IntPtr(lpFileName, dwDesiredAccess, dwShareMode, dwCreationDisposition, dwFlagsAndAttributes);
			SafeFileHandle safeFileHandle;
			try
			{
				safeFileHandle = new SafeFileHandle(intPtr, true);
			}
			catch
			{
				Interop.Kernel32.CloseHandle(intPtr);
				throw;
			}
			return safeFileHandle;
		}

		internal static IntPtr CreateFile_IntPtr(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, FileMode dwCreationDisposition, int dwFlagsAndAttributes)
		{
			lpFileName = PathInternal.EnsureExtendedPrefixIfNeeded(lpFileName);
			return Interop.Kernel32.CreateFilePrivate(lpFileName, dwDesiredAccess, dwShareMode, null, dwCreationDisposition, dwFlagsAndAttributes, IntPtr.Zero);
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "DeleteFileW", SetLastError = true)]
		private static extern bool DeleteFilePrivate(string path);

		internal static bool DeleteFile(string path)
		{
			path = PathInternal.EnsureExtendedPrefixIfNeeded(path);
			return Interop.Kernel32.DeleteFilePrivate(path);
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "FindFirstFileExW", SetLastError = true)]
		private static extern SafeFindHandle FindFirstFileExPrivate(string lpFileName, Interop.Kernel32.FINDEX_INFO_LEVELS fInfoLevelId, ref Interop.Kernel32.WIN32_FIND_DATA lpFindFileData, Interop.Kernel32.FINDEX_SEARCH_OPS fSearchOp, IntPtr lpSearchFilter, int dwAdditionalFlags);

		internal static SafeFindHandle FindFirstFile(string fileName, ref Interop.Kernel32.WIN32_FIND_DATA data)
		{
			fileName = PathInternal.EnsureExtendedPrefixIfNeeded(fileName);
			return Interop.Kernel32.FindFirstFileExPrivate(fileName, Interop.Kernel32.FINDEX_INFO_LEVELS.FindExInfoBasic, ref data, Interop.Kernel32.FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, 0);
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "FindNextFileW", SetLastError = true)]
		internal static extern bool FindNextFile(SafeFindHandle hndFindFile, ref Interop.Kernel32.WIN32_FIND_DATA lpFindFileData);

		[DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, EntryPoint = "FormatMessageW", SetLastError = true)]
		private unsafe static extern int FormatMessage(int dwFlags, IntPtr lpSource, uint dwMessageId, int dwLanguageId, char* lpBuffer, int nSize, IntPtr[] arguments);

		internal static string GetMessage(int errorCode)
		{
			return Interop.Kernel32.GetMessage(IntPtr.Zero, errorCode);
		}

		internal unsafe static string GetMessage(IntPtr moduleHandle, int errorCode)
		{
			Span<char> span = new Span<char>(stackalloc byte[(UIntPtr)512], 256);
			string text;
			while (!Interop.Kernel32.TryGetErrorMessage(moduleHandle, errorCode, span, out text))
			{
				span = new char[span.Length * 4];
				if (span.Length >= 66560)
				{
					return string.Format("Unknown error (0x{0:x})", errorCode);
				}
			}
			return text;
		}

		private unsafe static bool TryGetErrorMessage(IntPtr moduleHandle, int errorCode, Span<char> buffer, out string errorMsg)
		{
			int num = 12800;
			if (moduleHandle != IntPtr.Zero)
			{
				num |= 2048;
			}
			int num2;
			fixed (char* reference = MemoryMarshal.GetReference<char>(buffer))
			{
				char* ptr = reference;
				num2 = Interop.Kernel32.FormatMessage(num, moduleHandle, (uint)errorCode, 0, ptr, buffer.Length, null);
			}
			if (num2 != 0)
			{
				int i;
				for (i = num2; i > 0; i--)
				{
					char c = *buffer[i - 1];
					if (c > ' ' && c != '.')
					{
						break;
					}
				}
				errorMsg = buffer.Slice(0, i).ToString();
			}
			else
			{
				if (Marshal.GetLastWin32Error() == 122)
				{
					errorMsg = "";
					return false;
				}
				errorMsg = string.Format("Unknown error (0x{0:x})", errorCode);
			}
			return true;
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetFileAttributesExW", SetLastError = true)]
		private static extern bool GetFileAttributesExPrivate(string name, Interop.Kernel32.GET_FILEEX_INFO_LEVELS fileInfoLevel, ref Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA lpFileInformation);

		internal static bool GetFileAttributesEx(string name, Interop.Kernel32.GET_FILEEX_INFO_LEVELS fileInfoLevel, ref Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA lpFileInformation)
		{
			name = PathInternal.EnsureExtendedPrefixIfNeeded(name);
			return Interop.Kernel32.GetFileAttributesExPrivate(name, fileInfoLevel, ref lpFileInformation);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern int GetLogicalDrives();

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "MoveFileExW", SetLastError = true)]
		private static extern bool MoveFileExPrivate(string src, string dst, uint flags);

		internal static bool MoveFile(string src, string dst)
		{
			src = PathInternal.EnsureExtendedPrefixIfNeeded(src);
			dst = PathInternal.EnsureExtendedPrefixIfNeeded(dst);
			return Interop.Kernel32.MoveFileExPrivate(src, dst, 2U);
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RemoveDirectoryW", SetLastError = true)]
		private static extern bool RemoveDirectoryPrivate(string path);

		internal static bool RemoveDirectory(string path)
		{
			path = PathInternal.EnsureExtendedPrefixIfNeeded(path);
			return Interop.Kernel32.RemoveDirectoryPrivate(path);
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "ReplaceFileW", SetLastError = true)]
		private static extern bool ReplaceFilePrivate(string replacedFileName, string replacementFileName, string backupFileName, int dwReplaceFlags, IntPtr lpExclude, IntPtr lpReserved);

		internal static bool ReplaceFile(string replacedFileName, string replacementFileName, string backupFileName, int dwReplaceFlags, IntPtr lpExclude, IntPtr lpReserved)
		{
			replacedFileName = PathInternal.EnsureExtendedPrefixIfNeeded(replacedFileName);
			replacementFileName = PathInternal.EnsureExtendedPrefixIfNeeded(replacementFileName);
			backupFileName = PathInternal.EnsureExtendedPrefixIfNeeded(backupFileName);
			return Interop.Kernel32.ReplaceFilePrivate(replacedFileName, replacementFileName, backupFileName, dwReplaceFlags, lpExclude, lpReserved);
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "SetFileAttributesW", SetLastError = true)]
		private static extern bool SetFileAttributesPrivate(string name, int attr);

		internal static bool SetFileAttributes(string name, int attr)
		{
			name = PathInternal.EnsureExtendedPrefixIfNeeded(name);
			return Interop.Kernel32.SetFileAttributesPrivate(name, attr);
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern bool SetFileInformationByHandle(SafeFileHandle hFile, Interop.Kernel32.FILE_INFO_BY_HANDLE_CLASS FileInformationClass, ref Interop.Kernel32.FILE_BASIC_INFO lpFileInformation, uint dwBufferSize);

		internal unsafe static bool SetFileTime(SafeFileHandle hFile, long creationTime = -1L, long lastAccessTime = -1L, long lastWriteTime = -1L, long changeTime = -1L, uint fileAttributes = 0U)
		{
			Interop.Kernel32.FILE_BASIC_INFO file_BASIC_INFO = new Interop.Kernel32.FILE_BASIC_INFO
			{
				CreationTime = creationTime,
				LastAccessTime = lastAccessTime,
				LastWriteTime = lastWriteTime,
				ChangeTime = changeTime,
				FileAttributes = fileAttributes
			};
			return Interop.Kernel32.SetFileInformationByHandle(hFile, Interop.Kernel32.FILE_INFO_BY_HANDLE_CLASS.FileBasicInfo, ref file_BASIC_INFO, (uint)sizeof(Interop.Kernel32.FILE_BASIC_INFO));
		}

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern bool SetThreadErrorMode(uint dwNewMode, out uint lpOldMode);

		internal const int LOAD_LIBRARY_AS_DATAFILE = 2;

		internal const int MAX_PATH = 260;

		internal const uint MUI_PREFERRED_UI_LANGUAGES = 16U;

		internal const uint TIME_ZONE_ID_INVALID = 4294967295U;

		internal const uint SEM_FAILCRITICALERRORS = 1U;

		private const int FORMAT_MESSAGE_IGNORE_INSERTS = 512;

		private const int FORMAT_MESSAGE_FROM_HMODULE = 2048;

		private const int FORMAT_MESSAGE_FROM_SYSTEM = 4096;

		private const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 8192;

		private const int ERROR_INSUFFICIENT_BUFFER = 122;

		private const int InitialBufferSize = 256;

		private const int BufferSizeIncreaseFactor = 4;

		private const int MaxAllowedBufferSize = 66560;

		internal const int REPLACEFILE_IGNORE_MERGE_ERRORS = 2;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct WIN32_FIND_DATA
		{
			internal unsafe ReadOnlySpan<char> cFileName
			{
				get
				{
					fixed (char* ptr = &this._cFileName.FixedElementField)
					{
						return new ReadOnlySpan<char>((void*)ptr, 260);
					}
				}
			}

			internal uint dwFileAttributes;

			internal Interop.Kernel32.FILE_TIME ftCreationTime;

			internal Interop.Kernel32.FILE_TIME ftLastAccessTime;

			internal Interop.Kernel32.FILE_TIME ftLastWriteTime;

			internal uint nFileSizeHigh;

			internal uint nFileSizeLow;

			internal uint dwReserved0;

			internal uint dwReserved1;

			[FixedBuffer(typeof(char), 260)]
			private Interop.Kernel32.WIN32_FIND_DATA.<_cFileName>e__FixedBuffer _cFileName;

			[FixedBuffer(typeof(char), 14)]
			private Interop.Kernel32.WIN32_FIND_DATA.<_cAlternateFileName>e__FixedBuffer _cAlternateFileName;

			[UnsafeValueType]
			[CompilerGenerated]
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 520)]
			public struct <_cFileName>e__FixedBuffer
			{
				public char FixedElementField;
			}

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 28)]
			public struct <_cAlternateFileName>e__FixedBuffer
			{
				public char FixedElementField;
			}
		}

		internal struct REG_TZI_FORMAT
		{
			internal REG_TZI_FORMAT(in Interop.Kernel32.TIME_ZONE_INFORMATION tzi)
			{
				this.Bias = tzi.Bias;
				this.StandardDate = tzi.StandardDate;
				this.StandardBias = tzi.StandardBias;
				this.DaylightDate = tzi.DaylightDate;
				this.DaylightBias = tzi.DaylightBias;
			}

			internal int Bias;

			internal int StandardBias;

			internal int DaylightBias;

			internal Interop.Kernel32.SYSTEMTIME StandardDate;

			internal Interop.Kernel32.SYSTEMTIME DaylightDate;
		}

		internal struct SYSTEMTIME
		{
			internal bool Equals(in Interop.Kernel32.SYSTEMTIME other)
			{
				return this.Year == other.Year && this.Month == other.Month && this.DayOfWeek == other.DayOfWeek && this.Day == other.Day && this.Hour == other.Hour && this.Minute == other.Minute && this.Second == other.Second && this.Milliseconds == other.Milliseconds;
			}

			internal ushort Year;

			internal ushort Month;

			internal ushort DayOfWeek;

			internal ushort Day;

			internal ushort Hour;

			internal ushort Minute;

			internal ushort Second;

			internal ushort Milliseconds;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct TIME_DYNAMIC_ZONE_INFORMATION
		{
			internal unsafe string GetTimeZoneKeyName()
			{
				fixed (char* ptr = &this.TimeZoneKeyName.FixedElementField)
				{
					return new string(ptr);
				}
			}

			internal int Bias;

			[FixedBuffer(typeof(char), 32)]
			internal Interop.Kernel32.TIME_DYNAMIC_ZONE_INFORMATION.<StandardName>e__FixedBuffer StandardName;

			internal Interop.Kernel32.SYSTEMTIME StandardDate;

			internal int StandardBias;

			[FixedBuffer(typeof(char), 32)]
			internal Interop.Kernel32.TIME_DYNAMIC_ZONE_INFORMATION.<DaylightName>e__FixedBuffer DaylightName;

			internal Interop.Kernel32.SYSTEMTIME DaylightDate;

			internal int DaylightBias;

			[FixedBuffer(typeof(char), 128)]
			internal Interop.Kernel32.TIME_DYNAMIC_ZONE_INFORMATION.<TimeZoneKeyName>e__FixedBuffer TimeZoneKeyName;

			internal byte DynamicDaylightTimeDisabled;

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 64)]
			public struct <StandardName>e__FixedBuffer
			{
				public char FixedElementField;
			}

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 64)]
			public struct <DaylightName>e__FixedBuffer
			{
				public char FixedElementField;
			}

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 256)]
			public struct <TimeZoneKeyName>e__FixedBuffer
			{
				public char FixedElementField;
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct TIME_ZONE_INFORMATION
		{
			internal unsafe TIME_ZONE_INFORMATION(in Interop.Kernel32.TIME_DYNAMIC_ZONE_INFORMATION dtzi)
			{
				fixed (Interop.Kernel32.TIME_ZONE_INFORMATION* ptr = &this)
				{
					ref Interop.Kernel32.TIME_ZONE_INFORMATION ptr2 = ref *ptr;
					fixed (Interop.Kernel32.TIME_DYNAMIC_ZONE_INFORMATION* ptr3 = &dtzi)
					{
						Interop.Kernel32.TIME_DYNAMIC_ZONE_INFORMATION* ptr4 = ptr3;
						ptr2 = *(Interop.Kernel32.TIME_ZONE_INFORMATION*)ptr4;
					}
				}
			}

			internal unsafe string GetStandardName()
			{
				fixed (char* ptr = &this.StandardName.FixedElementField)
				{
					return new string(ptr);
				}
			}

			internal unsafe string GetDaylightName()
			{
				fixed (char* ptr = &this.DaylightName.FixedElementField)
				{
					return new string(ptr);
				}
			}

			internal int Bias;

			[FixedBuffer(typeof(char), 32)]
			internal Interop.Kernel32.TIME_ZONE_INFORMATION.<StandardName>e__FixedBuffer StandardName;

			internal Interop.Kernel32.SYSTEMTIME StandardDate;

			internal int StandardBias;

			[FixedBuffer(typeof(char), 32)]
			internal Interop.Kernel32.TIME_ZONE_INFORMATION.<DaylightName>e__FixedBuffer DaylightName;

			internal Interop.Kernel32.SYSTEMTIME DaylightDate;

			internal int DaylightBias;

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 64)]
			public struct <StandardName>e__FixedBuffer
			{
				public char FixedElementField;
			}

			[UnsafeValueType]
			[CompilerGenerated]
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 64)]
			public struct <DaylightName>e__FixedBuffer
			{
				public char FixedElementField;
			}
		}

		internal enum FILE_INFO_BY_HANDLE_CLASS : uint
		{
			FileBasicInfo,
			FileStandardInfo,
			FileNameInfo,
			FileRenameInfo,
			FileDispositionInfo,
			FileAllocationInfo,
			FileEndOfFileInfo,
			FileStreamInfo,
			FileCompressionInfo,
			FileAttributeTagInfo,
			FileIdBothDirectoryInfo,
			FileIdBothDirectoryRestartInfo,
			FileIoPriorityHintInfo,
			FileRemoteProtocolInfo,
			FileFullDirectoryInfo,
			FileFullDirectoryRestartInfo
		}

		internal struct FILE_TIME
		{
			internal FILE_TIME(long fileTime)
			{
				this.dwLowDateTime = (uint)fileTime;
				this.dwHighDateTime = (uint)(fileTime >> 32);
			}

			internal long ToTicks()
			{
				return (long)(((ulong)this.dwHighDateTime << 32) + (ulong)this.dwLowDateTime);
			}

			internal DateTime ToDateTimeUtc()
			{
				return DateTime.FromFileTimeUtc(this.ToTicks());
			}

			internal DateTimeOffset ToDateTimeOffset()
			{
				return DateTimeOffset.FromFileTime(this.ToTicks());
			}

			internal uint dwLowDateTime;

			internal uint dwHighDateTime;
		}

		internal enum FINDEX_INFO_LEVELS : uint
		{
			FindExInfoStandard,
			FindExInfoBasic,
			FindExInfoMaxInfoLevel
		}

		internal enum FINDEX_SEARCH_OPS : uint
		{
			FindExSearchNameMatch,
			FindExSearchLimitToDirectories,
			FindExSearchLimitToDevices,
			FindExSearchMaxSearchOp
		}

		internal class FileAttributes
		{
			internal const int FILE_ATTRIBUTE_NORMAL = 128;

			internal const int FILE_ATTRIBUTE_READONLY = 1;

			internal const int FILE_ATTRIBUTE_DIRECTORY = 16;

			internal const int FILE_ATTRIBUTE_REPARSE_POINT = 1024;
		}

		internal class IOReparseOptions
		{
			internal const uint IO_REPARSE_TAG_FILE_PLACEHOLDER = 2147483669U;

			internal const uint IO_REPARSE_TAG_MOUNT_POINT = 2684354563U;
		}

		internal class FileOperations
		{
			internal const int OPEN_EXISTING = 3;

			internal const int COPY_FILE_FAIL_IF_EXISTS = 1;

			internal const int FILE_ACTION_ADDED = 1;

			internal const int FILE_ACTION_REMOVED = 2;

			internal const int FILE_ACTION_MODIFIED = 3;

			internal const int FILE_ACTION_RENAMED_OLD_NAME = 4;

			internal const int FILE_ACTION_RENAMED_NEW_NAME = 5;

			internal const int FILE_FLAG_BACKUP_SEMANTICS = 33554432;

			internal const int FILE_FLAG_FIRST_PIPE_INSTANCE = 524288;

			internal const int FILE_FLAG_OVERLAPPED = 1073741824;

			internal const int FILE_LIST_DIRECTORY = 1;
		}

		internal enum GET_FILEEX_INFO_LEVELS : uint
		{
			GetFileExInfoStandard,
			GetFileExMaxInfoLevel
		}

		internal class GenericOperations
		{
			internal const int GENERIC_READ = -2147483648;

			internal const int GENERIC_WRITE = 1073741824;
		}

		internal struct SECURITY_ATTRIBUTES
		{
			internal uint nLength;

			internal IntPtr lpSecurityDescriptor;

			internal Interop.BOOL bInheritHandle;
		}

		internal struct FILE_BASIC_INFO
		{
			internal long CreationTime;

			internal long LastAccessTime;

			internal long LastWriteTime;

			internal long ChangeTime;

			internal uint FileAttributes;
		}

		internal struct WIN32_FILE_ATTRIBUTE_DATA
		{
			internal void PopulateFrom(ref Interop.Kernel32.WIN32_FIND_DATA findData)
			{
				this.dwFileAttributes = (int)findData.dwFileAttributes;
				this.ftCreationTime = findData.ftCreationTime;
				this.ftLastAccessTime = findData.ftLastAccessTime;
				this.ftLastWriteTime = findData.ftLastWriteTime;
				this.nFileSizeHigh = findData.nFileSizeHigh;
				this.nFileSizeLow = findData.nFileSizeLow;
			}

			internal int dwFileAttributes;

			internal Interop.Kernel32.FILE_TIME ftCreationTime;

			internal Interop.Kernel32.FILE_TIME ftLastAccessTime;

			internal Interop.Kernel32.FILE_TIME ftLastWriteTime;

			internal uint nFileSizeHigh;

			internal uint nFileSizeLow;
		}
	}

	internal class BCrypt
	{
		[DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
		internal unsafe static extern Interop.BCrypt.NTSTATUS BCryptGenRandom(IntPtr hAlgorithm, byte* pbBuffer, int cbBuffer, int dwFlags);

		internal const int BCRYPT_USE_SYSTEM_PREFERRED_RNG = 2;

		internal enum NTSTATUS : uint
		{
			STATUS_SUCCESS,
			STATUS_NOT_FOUND = 3221226021U,
			STATUS_INVALID_PARAMETER = 3221225485U,
			STATUS_NO_MEMORY = 3221225495U
		}
	}

	internal class User32
	{
		[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadStringW", SetLastError = true)]
		internal static extern int LoadString(SafeLibraryHandle handle, int id, [Out] StringBuilder buffer, int bufferLength);
	}

	internal enum BOOL
	{
		FALSE,
		TRUE
	}

	internal enum BOOLEAN : byte
	{
		FALSE,
		TRUE
	}

	internal class Errors
	{
		internal const int ERROR_SUCCESS = 0;

		internal const int ERROR_INVALID_FUNCTION = 1;

		internal const int ERROR_FILE_NOT_FOUND = 2;

		internal const int ERROR_PATH_NOT_FOUND = 3;

		internal const int ERROR_ACCESS_DENIED = 5;

		internal const int ERROR_INVALID_HANDLE = 6;

		internal const int ERROR_NOT_ENOUGH_MEMORY = 8;

		internal const int ERROR_INVALID_DATA = 13;

		internal const int ERROR_INVALID_DRIVE = 15;

		internal const int ERROR_NO_MORE_FILES = 18;

		internal const int ERROR_NOT_READY = 21;

		internal const int ERROR_BAD_COMMAND = 22;

		internal const int ERROR_BAD_LENGTH = 24;

		internal const int ERROR_SHARING_VIOLATION = 32;

		internal const int ERROR_LOCK_VIOLATION = 33;

		internal const int ERROR_HANDLE_EOF = 38;

		internal const int ERROR_BAD_NETPATH = 53;

		internal const int ERROR_BAD_NET_NAME = 67;

		internal const int ERROR_FILE_EXISTS = 80;

		internal const int ERROR_INVALID_PARAMETER = 87;

		internal const int ERROR_BROKEN_PIPE = 109;

		internal const int ERROR_SEM_TIMEOUT = 121;

		internal const int ERROR_CALL_NOT_IMPLEMENTED = 120;

		internal const int ERROR_INSUFFICIENT_BUFFER = 122;

		internal const int ERROR_INVALID_NAME = 123;

		internal const int ERROR_NEGATIVE_SEEK = 131;

		internal const int ERROR_DIR_NOT_EMPTY = 145;

		internal const int ERROR_BAD_PATHNAME = 161;

		internal const int ERROR_LOCK_FAILED = 167;

		internal const int ERROR_BUSY = 170;

		internal const int ERROR_ALREADY_EXISTS = 183;

		internal const int ERROR_BAD_EXE_FORMAT = 193;

		internal const int ERROR_ENVVAR_NOT_FOUND = 203;

		internal const int ERROR_FILENAME_EXCED_RANGE = 206;

		internal const int ERROR_EXE_MACHINE_TYPE_MISMATCH = 216;

		internal const int ERROR_PIPE_BUSY = 231;

		internal const int ERROR_NO_DATA = 232;

		internal const int ERROR_PIPE_NOT_CONNECTED = 233;

		internal const int ERROR_MORE_DATA = 234;

		internal const int ERROR_NO_MORE_ITEMS = 259;

		internal const int ERROR_DIRECTORY = 267;

		internal const int ERROR_PARTIAL_COPY = 299;

		internal const int ERROR_ARITHMETIC_OVERFLOW = 534;

		internal const int ERROR_PIPE_CONNECTED = 535;

		internal const int ERROR_PIPE_LISTENING = 536;

		internal const int ERROR_OPERATION_ABORTED = 995;

		internal const int ERROR_IO_INCOMPLETE = 996;

		internal const int ERROR_IO_PENDING = 997;

		internal const int ERROR_NO_TOKEN = 1008;

		internal const int ERROR_DLL_INIT_FAILED = 1114;

		internal const int ERROR_COUNTER_TIMEOUT = 1121;

		internal const int ERROR_NO_ASSOCIATION = 1155;

		internal const int ERROR_DDE_FAIL = 1156;

		internal const int ERROR_DLL_NOT_FOUND = 1157;

		internal const int ERROR_NOT_FOUND = 1168;

		internal const int ERROR_NETWORK_UNREACHABLE = 1231;

		internal const int ERROR_NON_ACCOUNT_SID = 1257;

		internal const int ERROR_NOT_ALL_ASSIGNED = 1300;

		internal const int ERROR_UNKNOWN_REVISION = 1305;

		internal const int ERROR_INVALID_OWNER = 1307;

		internal const int ERROR_INVALID_PRIMARY_GROUP = 1308;

		internal const int ERROR_NO_SUCH_PRIVILEGE = 1313;

		internal const int ERROR_PRIVILEGE_NOT_HELD = 1314;

		internal const int ERROR_INVALID_ACL = 1336;

		internal const int ERROR_INVALID_SECURITY_DESCR = 1338;

		internal const int ERROR_INVALID_SID = 1337;

		internal const int ERROR_BAD_IMPERSONATION_LEVEL = 1346;

		internal const int ERROR_CANT_OPEN_ANONYMOUS = 1347;

		internal const int ERROR_NO_SECURITY_ON_OBJECT = 1350;

		internal const int ERROR_CLASS_ALREADY_EXISTS = 1410;

		internal const int ERROR_TRUSTED_RELATIONSHIP_FAILURE = 1789;

		internal const int ERROR_RESOURCE_LANG_NOT_FOUND = 1815;

		internal const int EFail = -2147467259;

		internal const int E_FILENOTFOUND = -2147024894;
	}

	internal static class Libraries
	{
		internal const string Advapi32 = "advapi32.dll";

		internal const string BCrypt = "BCrypt.dll";

		internal const string CoreComm_L1_1_1 = "api-ms-win-core-comm-l1-1-1.dll";

		internal const string Crypt32 = "crypt32.dll";

		internal const string Error_L1 = "api-ms-win-core-winrt-error-l1-1-0.dll";

		internal const string HttpApi = "httpapi.dll";

		internal const string IpHlpApi = "iphlpapi.dll";

		internal const string Kernel32 = "kernel32.dll";

		internal const string Memory_L1_3 = "api-ms-win-core-memory-l1-1-3.dll";

		internal const string Mswsock = "mswsock.dll";

		internal const string NCrypt = "ncrypt.dll";

		internal const string NtDll = "ntdll.dll";

		internal const string Odbc32 = "odbc32.dll";

		internal const string OleAut32 = "oleaut32.dll";

		internal const string PerfCounter = "perfcounter.dll";

		internal const string RoBuffer = "api-ms-win-core-winrt-robuffer-l1-1-0.dll";

		internal const string Secur32 = "secur32.dll";

		internal const string Shell32 = "shell32.dll";

		internal const string SspiCli = "sspicli.dll";

		internal const string User32 = "user32.dll";

		internal const string Version = "version.dll";

		internal const string WebSocket = "websocket.dll";

		internal const string WinHttp = "winhttp.dll";

		internal const string Ws2_32 = "ws2_32.dll";

		internal const string Wtsapi32 = "wtsapi32.dll";

		internal const string CompressionNative = "clrcompression.dll";

		internal const string ErrorHandling = "api-ms-win-core-errorhandling-l1-1-0.dll";

		internal const string Handle = "api-ms-win-core-handle-l1-1-0.dll";

		internal const string IO = "api-ms-win-core-io-l1-1-0.dll";

		internal const string Memory = "api-ms-win-core-memory-l1-1-0.dll";

		internal const string ProcessEnvironment = "api-ms-win-core-processenvironment-l1-1-0.dll";

		internal const string ProcessThreads = "api-ms-win-core-processthreads-l1-1-0.dll";

		internal const string RealTime = "api-ms-win-core-realtime-l1-1-0.dll";

		internal const string SysInfo = "api-ms-win-core-sysinfo-l1-2-0.dll";

		internal const string ThreadPool = "api-ms-win-core-threadpool-l1-2-0.dll";

		internal const string Localization = "api-ms-win-core-localization-l1-2-1.dll";
	}

	internal struct LongFileTime
	{
		internal DateTimeOffset ToDateTimeOffset()
		{
			return new DateTimeOffset(DateTime.FromFileTimeUtc(this.TicksSince1601));
		}

		internal long TicksSince1601;
	}

	internal struct UNICODE_STRING
	{
		internal ushort Length;

		internal ushort MaximumLength;

		internal IntPtr Buffer;
	}

	internal class NtDll
	{
		[DllImport("ntdll.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		private unsafe static extern int NtCreateFile(out IntPtr FileHandle, Interop.NtDll.DesiredAccess DesiredAccess, ref Interop.NtDll.OBJECT_ATTRIBUTES ObjectAttributes, out Interop.NtDll.IO_STATUS_BLOCK IoStatusBlock, long* AllocationSize, global::System.IO.FileAttributes FileAttributes, FileShare ShareAccess, Interop.NtDll.CreateDisposition CreateDisposition, Interop.NtDll.CreateOptions CreateOptions, void* EaBuffer, uint EaLength);

		[return: TupleElementNames(new string[] { "status", "handle" })]
		internal unsafe static ValueTuple<int, IntPtr> CreateFile(ReadOnlySpan<char> path, IntPtr rootDirectory, Interop.NtDll.CreateDisposition createDisposition, Interop.NtDll.DesiredAccess desiredAccess = Interop.NtDll.DesiredAccess.SYNCHRONIZE | Interop.NtDll.DesiredAccess.FILE_GENERIC_READ, FileShare shareAccess = FileShare.Read | FileShare.Write | FileShare.Delete, global::System.IO.FileAttributes fileAttributes = (global::System.IO.FileAttributes)0, Interop.NtDll.CreateOptions createOptions = Interop.NtDll.CreateOptions.FILE_SYNCHRONOUS_IO_NONALERT, Interop.NtDll.ObjectAttributes objectAttributes = Interop.NtDll.ObjectAttributes.OBJ_CASE_INSENSITIVE)
		{
			fixed (char* reference = MemoryMarshal.GetReference<char>(path))
			{
				char* ptr = reference;
				Interop.UNICODE_STRING unicode_STRING = checked(new Interop.UNICODE_STRING
				{
					Length = (ushort)(path.Length * 2),
					MaximumLength = (ushort)(path.Length * 2),
					Buffer = (IntPtr)((void*)ptr)
				});
				Interop.NtDll.OBJECT_ATTRIBUTES object_ATTRIBUTES = new Interop.NtDll.OBJECT_ATTRIBUTES(&unicode_STRING, objectAttributes, rootDirectory);
				IntPtr intPtr;
				Interop.NtDll.IO_STATUS_BLOCK io_STATUS_BLOCK;
				return new ValueTuple<int, IntPtr>(Interop.NtDll.NtCreateFile(out intPtr, desiredAccess, ref object_ATTRIBUTES, out io_STATUS_BLOCK, null, fileAttributes, shareAccess, createDisposition, createOptions, null, 0U), intPtr);
			}
		}

		[DllImport("ntdll.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public unsafe static extern int NtQueryDirectoryFile(IntPtr FileHandle, IntPtr Event, IntPtr ApcRoutine, IntPtr ApcContext, out Interop.NtDll.IO_STATUS_BLOCK IoStatusBlock, IntPtr FileInformation, uint Length, Interop.NtDll.FILE_INFORMATION_CLASS FileInformationClass, Interop.BOOLEAN ReturnSingleEntry, Interop.UNICODE_STRING* FileName, Interop.BOOLEAN RestartScan);

		[DllImport("ntdll.dll", ExactSpelling = true)]
		public static extern uint RtlNtStatusToDosError(int Status);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct FILE_FULL_DIR_INFORMATION
		{
			public unsafe ReadOnlySpan<char> FileName
			{
				get
				{
					fixed (char* ptr = &this._fileName)
					{
						return new ReadOnlySpan<char>((void*)ptr, (int)(this.FileNameLength / 2U));
					}
				}
			}

			public unsafe static Interop.NtDll.FILE_FULL_DIR_INFORMATION* GetNextInfo(Interop.NtDll.FILE_FULL_DIR_INFORMATION* info)
			{
				if (info == null)
				{
					return null;
				}
				uint nextEntryOffset = info->NextEntryOffset;
				if (nextEntryOffset == 0U)
				{
					return null;
				}
				return info + nextEntryOffset / (uint)sizeof(Interop.NtDll.FILE_FULL_DIR_INFORMATION);
			}

			public uint NextEntryOffset;

			public uint FileIndex;

			public Interop.LongFileTime CreationTime;

			public Interop.LongFileTime LastAccessTime;

			public Interop.LongFileTime LastWriteTime;

			public Interop.LongFileTime ChangeTime;

			public long EndOfFile;

			public long AllocationSize;

			public global::System.IO.FileAttributes FileAttributes;

			public uint FileNameLength;

			public uint EaSize;

			private char _fileName;
		}

		public enum FILE_INFORMATION_CLASS : uint
		{
			FileDirectoryInformation = 1U,
			FileFullDirectoryInformation,
			FileBothDirectoryInformation,
			FileBasicInformation,
			FileStandardInformation,
			FileInternalInformation,
			FileEaInformation,
			FileAccessInformation,
			FileNameInformation,
			FileRenameInformation,
			FileLinkInformation,
			FileNamesInformation,
			FileDispositionInformation,
			FilePositionInformation,
			FileFullEaInformation,
			FileModeInformation,
			FileAlignmentInformation,
			FileAllInformation,
			FileAllocationInformation,
			FileEndOfFileInformation,
			FileAlternateNameInformation,
			FileStreamInformation,
			FilePipeInformation,
			FilePipeLocalInformation,
			FilePipeRemoteInformation,
			FileMailslotQueryInformation,
			FileMailslotSetInformation,
			FileCompressionInformation,
			FileObjectIdInformation,
			FileCompletionInformation,
			FileMoveClusterInformation,
			FileQuotaInformation,
			FileReparsePointInformation,
			FileNetworkOpenInformation,
			FileAttributeTagInformation,
			FileTrackingInformation,
			FileIdBothDirectoryInformation,
			FileIdFullDirectoryInformation,
			FileValidDataLengthInformation,
			FileShortNameInformation,
			FileIoCompletionNotificationInformation,
			FileIoStatusBlockRangeInformation,
			FileIoPriorityHintInformation,
			FileSfioReserveInformation,
			FileSfioVolumeInformation,
			FileHardLinkInformation,
			FileProcessIdsUsingFileInformation,
			FileNormalizedNameInformation,
			FileNetworkPhysicalNameInformation,
			FileIdGlobalTxDirectoryInformation,
			FileIsRemoteDeviceInformation,
			FileUnusedInformation,
			FileNumaNodeInformation,
			FileStandardLinkInformation,
			FileRemoteProtocolInformation,
			FileRenameInformationBypassAccessCheck,
			FileLinkInformationBypassAccessCheck,
			FileVolumeNameInformation,
			FileIdInformation,
			FileIdExtdDirectoryInformation,
			FileReplaceCompletionInformation,
			FileHardLinkFullIdInformation,
			FileIdExtdBothDirectoryInformation,
			FileDispositionInformationEx,
			FileRenameInformationEx,
			FileRenameInformationExBypassAccessCheck,
			FileDesiredStorageClassInformation,
			FileStatInformation
		}

		public struct IO_STATUS_BLOCK
		{
			public Interop.NtDll.IO_STATUS_BLOCK.IO_STATUS Status;

			public IntPtr Information;

			[StructLayout(LayoutKind.Explicit)]
			public struct IO_STATUS
			{
				[FieldOffset(0)]
				public uint Status;

				[FieldOffset(0)]
				public IntPtr Pointer;
			}
		}

		public struct OBJECT_ATTRIBUTES
		{
			public unsafe OBJECT_ATTRIBUTES(Interop.UNICODE_STRING* objectName, Interop.NtDll.ObjectAttributes attributes, IntPtr rootDirectory)
			{
				this.Length = (uint)sizeof(Interop.NtDll.OBJECT_ATTRIBUTES);
				this.RootDirectory = rootDirectory;
				this.ObjectName = objectName;
				this.Attributes = attributes;
				this.SecurityDescriptor = null;
				this.SecurityQualityOfService = null;
			}

			public uint Length;

			public IntPtr RootDirectory;

			public unsafe Interop.UNICODE_STRING* ObjectName;

			public Interop.NtDll.ObjectAttributes Attributes;

			public unsafe void* SecurityDescriptor;

			public unsafe void* SecurityQualityOfService;
		}

		[Flags]
		public enum ObjectAttributes : uint
		{
			OBJ_INHERIT = 2U,
			OBJ_PERMANENT = 16U,
			OBJ_EXCLUSIVE = 32U,
			OBJ_CASE_INSENSITIVE = 64U,
			OBJ_OPENIF = 128U,
			OBJ_OPENLINK = 256U
		}

		public enum CreateDisposition : uint
		{
			FILE_SUPERSEDE,
			FILE_OPEN,
			FILE_CREATE,
			FILE_OPEN_IF,
			FILE_OVERWRITE,
			FILE_OVERWRITE_IF
		}

		public enum CreateOptions : uint
		{
			FILE_DIRECTORY_FILE = 1U,
			FILE_WRITE_THROUGH,
			FILE_SEQUENTIAL_ONLY = 4U,
			FILE_NO_INTERMEDIATE_BUFFERING = 8U,
			FILE_SYNCHRONOUS_IO_ALERT = 16U,
			FILE_SYNCHRONOUS_IO_NONALERT = 32U,
			FILE_NON_DIRECTORY_FILE = 64U,
			FILE_CREATE_TREE_CONNECTION = 128U,
			FILE_COMPLETE_IF_OPLOCKED = 256U,
			FILE_NO_EA_KNOWLEDGE = 512U,
			FILE_RANDOM_ACCESS = 2048U,
			FILE_DELETE_ON_CLOSE = 4096U,
			FILE_OPEN_BY_FILE_ID = 8192U,
			FILE_OPEN_FOR_BACKUP_INTENT = 16384U,
			FILE_NO_COMPRESSION = 32768U,
			FILE_OPEN_REQUIRING_OPLOCK = 65536U,
			FILE_DISALLOW_EXCLUSIVE = 131072U,
			FILE_SESSION_AWARE = 262144U,
			FILE_RESERVE_OPFILTER = 1048576U,
			FILE_OPEN_REPARSE_POINT = 2097152U,
			FILE_OPEN_NO_RECALL = 4194304U
		}

		[Flags]
		public enum DesiredAccess : uint
		{
			FILE_READ_DATA = 1U,
			FILE_LIST_DIRECTORY = 1U,
			FILE_WRITE_DATA = 2U,
			FILE_ADD_FILE = 2U,
			FILE_APPEND_DATA = 4U,
			FILE_ADD_SUBDIRECTORY = 4U,
			FILE_CREATE_PIPE_INSTANCE = 4U,
			FILE_READ_EA = 8U,
			FILE_WRITE_EA = 16U,
			FILE_EXECUTE = 32U,
			FILE_TRAVERSE = 32U,
			FILE_DELETE_CHILD = 64U,
			FILE_READ_ATTRIBUTES = 128U,
			FILE_WRITE_ATTRIBUTES = 256U,
			FILE_ALL_ACCESS = 983551U,
			DELETE = 65536U,
			READ_CONTROL = 131072U,
			WRITE_DAC = 262144U,
			WRITE_OWNER = 524288U,
			SYNCHRONIZE = 1048576U,
			STANDARD_RIGHTS_READ = 131072U,
			STANDARD_RIGHTS_WRITE = 131072U,
			STANDARD_RIGHTS_EXECUTE = 131072U,
			FILE_GENERIC_READ = 2147483648U,
			FILE_GENERIC_WRITE = 1073741824U,
			FILE_GENERIC_EXECUTE = 536870912U
		}
	}

	internal class StatusOptions
	{
		internal const uint STATUS_SUCCESS = 0U;

		internal const uint STATUS_SOME_NOT_MAPPED = 263U;

		internal const uint STATUS_NO_MORE_FILES = 2147483654U;

		internal const uint STATUS_INVALID_PARAMETER = 3221225485U;

		internal const uint STATUS_NO_MEMORY = 3221225495U;

		internal const uint STATUS_OBJECT_NAME_NOT_FOUND = 3221225524U;

		internal const uint STATUS_NONE_MAPPED = 3221225587U;

		internal const uint STATUS_INSUFFICIENT_RESOURCES = 3221225626U;

		internal const uint STATUS_ACCESS_DENIED = 3221225506U;

		internal const uint STATUS_ACCOUNT_RESTRICTION = 3221225582U;
	}

	internal class Advapi32
	{
		[DllImport("advapi32.dll")]
		internal static extern int RegCloseKey(IntPtr hKey);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegConnectRegistryW")]
		internal static extern int RegConnectRegistry(string machineName, SafeRegistryHandle key, out SafeRegistryHandle result);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegCreateKeyExW")]
		internal static extern int RegCreateKeyEx(SafeRegistryHandle hKey, string lpSubKey, int Reserved, string lpClass, int dwOptions, int samDesired, ref Interop.Kernel32.SECURITY_ATTRIBUTES secAttrs, out SafeRegistryHandle hkResult, out int lpdwDisposition);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegDeleteKeyExW")]
		internal static extern int RegDeleteKeyEx(SafeRegistryHandle hKey, string lpSubKey, int samDesired, int Reserved);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegDeleteValueW")]
		internal static extern int RegDeleteValue(SafeRegistryHandle hKey, string lpValueName);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegEnumKeyExW")]
		internal static extern int RegEnumKeyEx(SafeRegistryHandle hKey, int dwIndex, char[] lpName, ref int lpcbName, int[] lpReserved, [Out] StringBuilder lpClass, int[] lpcbClass, long[] lpftLastWriteTime);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegEnumValueW")]
		internal static extern int RegEnumValue(SafeRegistryHandle hKey, int dwIndex, char[] lpValueName, ref int lpcbValueName, IntPtr lpReserved_MustBeZero, int[] lpType, byte[] lpData, int[] lpcbData);

		[DllImport("advapi32.dll")]
		internal static extern int RegFlushKey(SafeRegistryHandle hKey);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegOpenKeyExW")]
		internal static extern int RegOpenKeyEx(SafeRegistryHandle hKey, string lpSubKey, int ulOptions, int samDesired, out SafeRegistryHandle hkResult);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegOpenKeyExW")]
		internal static extern int RegOpenKeyEx(IntPtr hKey, string lpSubKey, int ulOptions, int samDesired, out SafeRegistryHandle hkResult);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegQueryInfoKeyW")]
		internal static extern int RegQueryInfoKey(SafeRegistryHandle hKey, [Out] StringBuilder lpClass, int[] lpcbClass, IntPtr lpReserved_MustBeZero, ref int lpcSubKeys, int[] lpcbMaxSubKeyLen, int[] lpcbMaxClassLen, ref int lpcValues, int[] lpcbMaxValueNameLen, int[] lpcbMaxValueLen, int[] lpcbSecurityDescriptor, int[] lpftLastWriteTime);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW")]
		internal static extern int RegQueryValueEx(SafeRegistryHandle hKey, string lpValueName, int[] lpReserved, ref int lpType, [Out] byte[] lpData, ref int lpcbData);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW")]
		internal static extern int RegQueryValueEx(SafeRegistryHandle hKey, string lpValueName, int[] lpReserved, ref int lpType, ref int lpData, ref int lpcbData);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW")]
		internal static extern int RegQueryValueEx(SafeRegistryHandle hKey, string lpValueName, int[] lpReserved, ref int lpType, ref long lpData, ref int lpcbData);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW")]
		internal static extern int RegQueryValueEx(SafeRegistryHandle hKey, string lpValueName, int[] lpReserved, ref int lpType, [Out] char[] lpData, ref int lpcbData);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW")]
		internal static extern int RegQueryValueEx(SafeRegistryHandle hKey, string lpValueName, int[] lpReserved, ref int lpType, [Out] StringBuilder lpData, ref int lpcbData);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegSetValueExW")]
		internal static extern int RegSetValueEx(SafeRegistryHandle hKey, string lpValueName, int Reserved, RegistryValueKind dwType, byte[] lpData, int cbData);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegSetValueExW")]
		internal static extern int RegSetValueEx(SafeRegistryHandle hKey, string lpValueName, int Reserved, RegistryValueKind dwType, char[] lpData, int cbData);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegSetValueExW")]
		internal static extern int RegSetValueEx(SafeRegistryHandle hKey, string lpValueName, int Reserved, RegistryValueKind dwType, ref int lpData, int cbData);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegSetValueExW")]
		internal static extern int RegSetValueEx(SafeRegistryHandle hKey, string lpValueName, int Reserved, RegistryValueKind dwType, ref long lpData, int cbData);

		[DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "RegSetValueExW")]
		internal static extern int RegSetValueEx(SafeRegistryHandle hKey, string lpValueName, int Reserved, RegistryValueKind dwType, string lpData, int cbData);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr RegisterServiceCtrlHandler(string serviceName, Delegate callback);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr RegisterServiceCtrlHandlerEx(string serviceName, Delegate callback, IntPtr userData);

		internal class RegistryOptions
		{
			internal const int REG_OPTION_NON_VOLATILE = 0;

			internal const int REG_OPTION_VOLATILE = 1;

			internal const int REG_OPTION_CREATE_LINK = 2;

			internal const int REG_OPTION_BACKUP_RESTORE = 4;
		}

		internal class RegistryView
		{
			internal const int KEY_WOW64_64KEY = 256;

			internal const int KEY_WOW64_32KEY = 512;
		}

		internal class RegistryOperations
		{
			internal const int KEY_QUERY_VALUE = 1;

			internal const int KEY_SET_VALUE = 2;

			internal const int KEY_CREATE_SUB_KEY = 4;

			internal const int KEY_ENUMERATE_SUB_KEYS = 8;

			internal const int KEY_NOTIFY = 16;

			internal const int KEY_CREATE_LINK = 32;

			internal const int KEY_READ = 131097;

			internal const int KEY_WRITE = 131078;

			internal const int SYNCHRONIZE = 1048576;

			internal const int READ_CONTROL = 131072;

			internal const int STANDARD_RIGHTS_READ = 131072;

			internal const int STANDARD_RIGHTS_WRITE = 131072;
		}

		internal class RegistryValues
		{
			internal const int REG_NONE = 0;

			internal const int REG_SZ = 1;

			internal const int REG_EXPAND_SZ = 2;

			internal const int REG_BINARY = 3;

			internal const int REG_DWORD = 4;

			internal const int REG_DWORD_LITTLE_ENDIAN = 4;

			internal const int REG_DWORD_BIG_ENDIAN = 5;

			internal const int REG_LINK = 6;

			internal const int REG_MULTI_SZ = 7;

			internal const int REG_QWORD = 11;
		}
	}

	internal static class mincore
	{
		[DllImport("api-ms-win-core-heap-l1-1-0.dll")]
		internal static extern IntPtr GetProcessHeap();

		[DllImport("api-ms-win-core-heap-l1-1-0.dll")]
		internal static extern IntPtr HeapAlloc(IntPtr hHeap, uint dwFlags, UIntPtr dwBytes);

		[DllImport("api-ms-win-core-heap-l1-1-0.dll")]
		internal static extern int HeapFree(IntPtr hHeap, uint dwFlags, IntPtr lpMem);

		[DllImport("api-ms-win-core-threadpool-l1-2-0.dll", SetLastError = true)]
		internal static extern SafeThreadPoolIOHandle CreateThreadpoolIo(SafeHandle fl, IntPtr pfnio, IntPtr context, IntPtr pcbe);

		[DllImport("api-ms-win-core-threadpool-l1-2-0.dll")]
		internal static extern void CloseThreadpoolIo(IntPtr pio);

		[DllImport("api-ms-win-core-threadpool-l1-2-0.dll")]
		internal static extern void StartThreadpoolIo(SafeThreadPoolIOHandle pio);

		[DllImport("api-ms-win-core-threadpool-l1-2-0.dll")]
		internal static extern void CancelThreadpoolIo(SafeThreadPoolIOHandle pio);
	}

	internal delegate void NativeIoCompletionCallback(IntPtr instance, IntPtr context, IntPtr overlapped, uint ioResult, UIntPtr numberOfBytesTransferred, IntPtr io);
}
