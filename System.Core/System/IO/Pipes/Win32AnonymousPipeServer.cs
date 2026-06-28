using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal class Win32AnonymousPipeServer : Win32AnonymousPipe, IPipe, IAnonymousPipeServer
	{
		public Win32AnonymousPipeServer(AnonymousPipeServerStream owner, PipeDirection direction, HandleInheritability inheritability, int bufferSize)
		{
			SecurityAttributesHack securityAttributesHack = new SecurityAttributesHack(inheritability == HandleInheritability.Inheritable);
			IntPtr intPtr;
			IntPtr intPtr2;
			if (!Win32Marshal.CreatePipe(out intPtr, out intPtr2, ref securityAttributesHack, bufferSize))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			SafePipeHandle safePipeHandle = new SafePipeHandle(intPtr, true);
			SafePipeHandle safePipeHandle2 = new SafePipeHandle(intPtr2, true);
			if (direction == PipeDirection.Out)
			{
				this.server_handle = safePipeHandle2;
				this.client_handle = safePipeHandle;
			}
			else
			{
				this.server_handle = safePipeHandle;
				this.client_handle = safePipeHandle2;
			}
		}

		public Win32AnonymousPipeServer(AnonymousPipeServerStream owner, SafePipeHandle serverHandle, SafePipeHandle clientHandle)
		{
			this.server_handle = serverHandle;
			this.client_handle = clientHandle;
		}

		public override SafePipeHandle Handle
		{
			get
			{
				return this.server_handle;
			}
		}

		public SafePipeHandle ClientHandle
		{
			get
			{
				return this.client_handle;
			}
		}

		public void DisposeLocalCopyOfClientHandle()
		{
			throw new NotImplementedException();
		}

		private SafePipeHandle server_handle;

		private SafePipeHandle client_handle;
	}
}
