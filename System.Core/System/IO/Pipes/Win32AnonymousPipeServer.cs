using System;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal class Win32AnonymousPipeServer : Win32AnonymousPipe, IAnonymousPipeServer, IPipe
	{
		public unsafe Win32AnonymousPipeServer(AnonymousPipeServerStream owner, PipeDirection direction, HandleInheritability inheritability, int bufferSize, PipeSecurity pipeSecurity)
		{
			byte[] array = null;
			if (pipeSecurity != null)
			{
				array = pipeSecurity.GetSecurityDescriptorBinaryForm();
			}
			byte[] array2;
			byte* ptr;
			if ((array2 = array) == null || array2.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array2[0];
			}
			SecurityAttributes securityAttributes = new SecurityAttributes(inheritability, (IntPtr)((void*)ptr));
			IntPtr intPtr;
			IntPtr intPtr2;
			if (!Win32Marshal.CreatePipe(out intPtr, out intPtr2, ref securityAttributes, bufferSize))
			{
				throw Win32PipeError.GetException();
			}
			array2 = null;
			SafePipeHandle safePipeHandle = new SafePipeHandle(intPtr, true);
			SafePipeHandle safePipeHandle2 = new SafePipeHandle(intPtr2, true);
			if (direction == PipeDirection.Out)
			{
				this.server_handle = safePipeHandle2;
				this.client_handle = safePipeHandle;
				return;
			}
			this.server_handle = safePipeHandle;
			this.client_handle = safePipeHandle2;
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
