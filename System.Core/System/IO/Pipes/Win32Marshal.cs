using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal static class Win32Marshal
	{
		internal static bool IsWindows
		{
			get
			{
				PlatformID platform = Environment.OSVersion.Platform;
				return platform <= PlatformID.WinCE;
			}
		}

		[DllImport("kernel32", SetLastError = true)]
		internal static extern bool CreatePipe(out IntPtr readHandle, out IntPtr writeHandle, ref SecurityAttributes pipeAtts, int size);

		[DllImport("kernel32", SetLastError = true)]
		internal static extern IntPtr CreateNamedPipe(string name, uint openMode, int pipeMode, int maxInstances, int outBufferSize, int inBufferSize, int defaultTimeout, ref SecurityAttributes securityAttributes, IntPtr atts);

		[DllImport("kernel32", SetLastError = true)]
		internal static extern bool ConnectNamedPipe(SafePipeHandle handle, IntPtr overlapped);

		[DllImport("kernel32", SetLastError = true)]
		internal static extern bool DisconnectNamedPipe(SafePipeHandle handle);

		[DllImport("kernel32", SetLastError = true)]
		internal static extern bool GetNamedPipeHandleState(SafePipeHandle handle, out int state, out int curInstances, out int maxCollectionCount, out int collectDateTimeout, byte[] userName, int maxUserNameSize);

		[DllImport("kernel32", SetLastError = true)]
		internal static extern bool WaitNamedPipe(string name, int timeout);

		[DllImport("kernel32", SetLastError = true)]
		internal static extern IntPtr CreateFile(string name, PipeAccessRights desiredAccess, FileShare fileShare, ref SecurityAttributes atts, int creationDisposition, int flags, IntPtr templateHandle);
	}
}
