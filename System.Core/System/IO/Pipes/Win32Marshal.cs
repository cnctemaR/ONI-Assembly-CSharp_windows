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
				switch (Environment.OSVersion.Platform)
				{
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.Win32NT:
				case PlatformID.WinCE:
					return true;
				default:
					return false;
				}
			}
		}

		[DllImport("kernel32")]
		internal static extern bool CreatePipe(out IntPtr readHandle, out IntPtr writeHandle, ref SecurityAttributesHack pipeAtts, int size);

		[DllImport("kernel32")]
		internal static extern IntPtr CreateNamedPipe(string name, uint openMode, int pipeMode, int maxInstances, int outBufferSize, int inBufferSize, int defaultTimeout, ref SecurityAttributesHack securityAttributes, IntPtr atts);

		[DllImport("kernel32")]
		internal static extern bool ConnectNamedPipe(SafePipeHandle handle, IntPtr overlapped);

		[DllImport("kernel32")]
		internal static extern bool DisconnectNamedPipe(SafePipeHandle handle);

		[DllImport("kernel32")]
		internal static extern bool GetNamedPipeHandleState(SafePipeHandle handle, out int state, out int curInstances, out int maxCollectionCount, out int collectDateTimeout, byte[] userName, int maxUserNameSize);

		[DllImport("kernel32")]
		internal static extern bool WaitNamedPipe(string name, int timeout);

		[DllImport("kernel32")]
		internal static extern IntPtr CreateFile(string name, PipeAccessRights desiredAccess, FileShare fileShare, ref SecurityAttributesHack atts, int creationDisposition, int flags, IntPtr templateHandle);
	}
}
