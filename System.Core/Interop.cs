using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

internal static class Interop
{
	internal enum BOOL
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
	}

	internal static class Advapi32
	{
		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ImpersonateNamedPipeClient(SafePipeHandle hNamedPipe);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal static extern bool RevertToSelf();
	}

	internal class Kernel32
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		internal unsafe static extern bool CancelIoEx(SafeHandle handle, NativeOverlapped* lpOverlapped);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal unsafe static extern bool ConnectNamedPipe(SafePipeHandle handle, NativeOverlapped* overlapped);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ConnectNamedPipe(SafePipeHandle handle, IntPtr overlapped);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "CreateNamedPipeW", SetLastError = true)]
		internal static extern SafePipeHandle CreateNamedPipe(string pipeName, int openMode, int pipeMode, int maxInstances, int outBufferSize, int inBufferSize, int defaultTimeout, ref global::Interop.Kernel32.SECURITY_ATTRIBUTES securityAttributes);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "CreateFileW", SetLastError = true)]
		internal static extern SafePipeHandle CreateNamedPipeClient(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, ref global::Interop.Kernel32.SECURITY_ATTRIBUTES secAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool CreatePipe(out SafePipeHandle hReadPipe, out SafePipeHandle hWritePipe, ref global::Interop.Kernel32.SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DisconnectNamedPipe(SafePipeHandle hNamedPipe);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, SafePipeHandle hSourceHandle, IntPtr hTargetProcessHandle, out SafePipeHandle lpTargetHandle, uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwOptions);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FlushFileBuffers(SafeHandle hHandle);

		[DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, EntryPoint = "FormatMessageW", SetLastError = true)]
		private unsafe static extern int FormatMessage(int dwFlags, IntPtr lpSource, uint dwMessageId, int dwLanguageId, char* lpBuffer, int nSize, IntPtr[] arguments);

		internal static string GetMessage(int errorCode)
		{
			return global::Interop.Kernel32.GetMessage(IntPtr.Zero, errorCode);
		}

		internal unsafe static string GetMessage(IntPtr moduleHandle, int errorCode)
		{
			Span<char> span = new Span<char>(stackalloc byte[(UIntPtr)512], 256);
			string text;
			while (!global::Interop.Kernel32.TryGetErrorMessage(moduleHandle, errorCode, span, out text))
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
				num2 = global::Interop.Kernel32.FormatMessage(num, moduleHandle, (uint)errorCode, 0, ptr, buffer.Length, null);
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

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern int GetFileType(SafeHandle hFile);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetNamedPipeHandleStateW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetNamedPipeHandleState(SafePipeHandle hNamedPipe, out int lpState, IntPtr lpCurInstances, IntPtr lpMaxCollectionCount, IntPtr lpCollectDataTimeout, IntPtr lpUserName, int nMaxUserNameSize);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "GetNamedPipeHandleStateW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetNamedPipeHandleState(SafePipeHandle hNamedPipe, IntPtr lpState, IntPtr lpCurInstances, IntPtr lpMaxCollectionCount, IntPtr lpCollectDataTimeout, [Out] StringBuilder lpUserName, int nMaxUserNameSize);

		[DllImport("kernel32.dll", EntryPoint = "GetNamedPipeHandleStateW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetNamedPipeHandleState(SafePipeHandle hNamedPipe, IntPtr lpState, out int lpCurInstances, IntPtr lpMaxCollectionCount, IntPtr lpCollectDataTimeout, IntPtr lpUserName, int nMaxUserNameSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetNamedPipeInfo(SafePipeHandle hNamedPipe, out int lpFlags, IntPtr lpOutBufferSize, IntPtr lpInBufferSize, IntPtr lpMaxInstances);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetNamedPipeInfo(SafePipeHandle hNamedPipe, IntPtr lpFlags, out int lpOutBufferSize, IntPtr lpInBufferSize, IntPtr lpMaxInstances);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetNamedPipeInfo(SafePipeHandle hNamedPipe, IntPtr lpFlags, IntPtr lpOutBufferSize, out int lpInBufferSize, IntPtr lpMaxInstances);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal unsafe static extern int ReadFile(SafeHandle handle, byte* bytes, int numBytesToRead, out int numBytesRead, IntPtr mustBeZero);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal unsafe static extern int ReadFile(SafeHandle handle, byte* bytes, int numBytesToRead, IntPtr numBytesRead_mustBeZero, NativeOverlapped* overlapped);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal unsafe static extern bool SetNamedPipeHandleState(SafePipeHandle hNamedPipe, int* lpMode, IntPtr lpMaxCollectionCount, IntPtr lpCollectDataTimeout);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "WaitNamedPipeW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WaitNamedPipe(string name, int timeout);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal unsafe static extern int WriteFile(SafeHandle handle, byte* bytes, int numBytesToWrite, out int numBytesWritten, IntPtr mustBeZero);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal unsafe static extern int WriteFile(SafeHandle handle, byte* bytes, int numBytesToWrite, IntPtr numBytesWritten_mustBeZero, NativeOverlapped* lpOverlapped);

		internal const uint SEM_FAILCRITICALERRORS = 1U;

		private const int FORMAT_MESSAGE_IGNORE_INSERTS = 512;

		private const int FORMAT_MESSAGE_FROM_HMODULE = 2048;

		private const int FORMAT_MESSAGE_FROM_SYSTEM = 4096;

		private const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 8192;

		private const int ERROR_INSUFFICIENT_BUFFER = 122;

		private const int InitialBufferSize = 256;

		private const int BufferSizeIncreaseFactor = 4;

		private const int MaxAllowedBufferSize = 66560;

		internal const int MAX_PATH = 260;

		internal const int CREDUI_MAX_USERNAME_LENGTH = 513;

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

		internal class FileTypes
		{
			internal const int FILE_TYPE_UNKNOWN = 0;

			internal const int FILE_TYPE_DISK = 1;

			internal const int FILE_TYPE_CHAR = 2;

			internal const int FILE_TYPE_PIPE = 3;
		}

		internal class GenericOperations
		{
			internal const int GENERIC_READ = -2147483648;

			internal const int GENERIC_WRITE = 1073741824;
		}

		internal class HandleOptions
		{
			internal const int DUPLICATE_SAME_ACCESS = 2;

			internal const int STILL_ACTIVE = 259;

			internal const int TOKEN_ADJUST_PRIVILEGES = 32;
		}

		internal class PipeOptions
		{
			internal const int PIPE_ACCESS_INBOUND = 1;

			internal const int PIPE_ACCESS_OUTBOUND = 2;

			internal const int PIPE_ACCESS_DUPLEX = 3;

			internal const int PIPE_TYPE_BYTE = 0;

			internal const int PIPE_TYPE_MESSAGE = 4;

			internal const int PIPE_READMODE_BYTE = 0;

			internal const int PIPE_READMODE_MESSAGE = 2;

			internal const int PIPE_UNLIMITED_INSTANCES = 255;
		}

		internal struct SECURITY_ATTRIBUTES
		{
			internal uint nLength;

			internal IntPtr lpSecurityDescriptor;

			internal global::Interop.BOOL bInheritHandle;
		}

		internal class SecurityOptions
		{
			internal const int SECURITY_SQOS_PRESENT = 1048576;

			internal const int SECURITY_ANONYMOUS = 0;

			internal const int SECURITY_IDENTIFICATION = 65536;

			internal const int SECURITY_IMPERSONATION = 131072;

			internal const int SECURITY_DELEGATION = 196608;
		}
	}
}
