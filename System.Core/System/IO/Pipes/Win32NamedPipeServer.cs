using System;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal class Win32NamedPipeServer : Win32NamedPipe, INamedPipeServer, IPipe
	{
		public Win32NamedPipeServer(NamedPipeServerStream owner, SafePipeHandle safePipeHandle)
		{
			this.handle = safePipeHandle;
		}

		public unsafe Win32NamedPipeServer(NamedPipeServerStream owner, string pipeName, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeAccessRights rights, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity, HandleInheritability inheritability)
		{
			string text = string.Format("\\\\.\\pipe\\{0}", pipeName);
			uint num = (uint)(rights | (PipeAccessRights)options);
			int num2 = 0;
			if ((owner.TransmissionMode & PipeTransmissionMode.Message) != PipeTransmissionMode.Byte)
			{
				num2 |= 4;
			}
			if ((options & PipeOptions.Asynchronous) != PipeOptions.None)
			{
				num2 |= 1;
			}
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
			IntPtr intPtr = Win32Marshal.CreateNamedPipe(text, num, num2, maxNumberOfServerInstances, outBufferSize, inBufferSize, 0, ref securityAttributes, IntPtr.Zero);
			if (intPtr == new IntPtr(-1L))
			{
				throw Win32PipeError.GetException();
			}
			this.handle = new SafePipeHandle(intPtr, true);
			array2 = null;
		}

		public override SafePipeHandle Handle
		{
			get
			{
				return this.handle;
			}
		}

		public void Disconnect()
		{
			Win32Marshal.DisconnectNamedPipe(this.Handle);
		}

		public void WaitForConnection()
		{
			if (!Win32Marshal.ConnectNamedPipe(this.Handle, IntPtr.Zero))
			{
				throw Win32PipeError.GetException();
			}
		}

		private SafePipeHandle handle;
	}
}
