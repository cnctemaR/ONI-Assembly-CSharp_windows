using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal class Win32NamedPipeServer : Win32NamedPipe, IPipe, INamedPipeServer
	{
		public Win32NamedPipeServer(NamedPipeServerStream owner, SafePipeHandle safePipeHandle)
		{
			this.handle = safePipeHandle;
		}

		public Win32NamedPipeServer(NamedPipeServerStream owner, string pipeName, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeAccessRights rights, PipeOptions options, int inBufferSize, int outBufferSize, HandleInheritability inheritability)
		{
			string text = string.Format("\\\\.\\pipe\\{0}", pipeName);
			uint num = 0U;
			if ((rights & PipeAccessRights.ReadData) != (PipeAccessRights)0)
			{
				num |= 1U;
			}
			if ((rights & PipeAccessRights.WriteData) != (PipeAccessRights)0)
			{
				num |= 2U;
			}
			if ((options & PipeOptions.WriteThrough) != PipeOptions.None)
			{
				num |= 2147483648U;
			}
			int num2 = 0;
			if ((owner.TransmissionMode & PipeTransmissionMode.Message) != PipeTransmissionMode.Byte)
			{
				num2 |= 4;
			}
			if ((options & PipeOptions.Asynchronous) != PipeOptions.None)
			{
				num2 |= 1;
			}
			SecurityAttributesHack securityAttributesHack = new SecurityAttributesHack(inheritability == HandleInheritability.Inheritable);
			IntPtr intPtr = Win32Marshal.CreateNamedPipe(text, num, num2, maxNumberOfServerInstances, outBufferSize, inBufferSize, 0, ref securityAttributesHack, IntPtr.Zero);
			if (intPtr == new IntPtr(-1L))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			this.handle = new SafePipeHandle(intPtr, true);
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
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}

		private SafePipeHandle handle;
	}
}
