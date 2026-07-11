using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal class Win32NamedPipeClient : Win32NamedPipe, IPipe, INamedPipeClient
	{
		public Win32NamedPipeClient(NamedPipeClientStream owner, SafePipeHandle safePipeHandle)
		{
			this.handle = safePipeHandle;
			this.owner = owner;
		}

		public Win32NamedPipeClient(NamedPipeClientStream owner, string serverName, string pipeName, PipeAccessRights desiredAccessRights, PipeOptions options, HandleInheritability inheritability)
		{
			Win32NamedPipeClient <>f__this = this;
			this.name = string.Format("\\\\{0}\\pipe\\{1}", serverName, pipeName);
			SecurityAttributesHack att = new SecurityAttributesHack(inheritability == HandleInheritability.Inheritable);
			this.is_async = (options & PipeOptions.Asynchronous) != PipeOptions.None;
			this.opener = delegate
			{
				IntPtr intPtr = Win32Marshal.CreateFile(<>f__this.name, desiredAccessRights, FileShare.None, ref att, 3, 0, IntPtr.Zero);
				if (intPtr == new IntPtr(-1L))
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
				return new SafePipeHandle(intPtr, true);
			};
			this.owner = owner;
		}

		public override SafePipeHandle Handle
		{
			get
			{
				return this.handle;
			}
		}

		public bool IsAsync
		{
			get
			{
				return this.is_async;
			}
		}

		public void Connect()
		{
			if (this.owner.IsConnected)
			{
				throw new InvalidOperationException("The named pipe is already connected");
			}
			this.handle = this.opener();
		}

		public void Connect(int timeout)
		{
			if (this.owner.IsConnected)
			{
				throw new InvalidOperationException("The named pipe is already connected");
			}
			if (!Win32Marshal.WaitNamedPipe(this.name, timeout))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			this.Connect();
		}

		public int NumberOfServerInstances
		{
			get
			{
				byte[] array = null;
				int num;
				int num2;
				int num3;
				int num4;
				if (!Win32Marshal.GetNamedPipeHandleState(this.Handle, out num, out num2, out num3, out num4, array, 0))
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
				return num2;
			}
		}

		private NamedPipeClientStream owner;

		private Func<SafePipeHandle> opener;

		private bool is_async;

		private string name;

		private SafePipeHandle handle;
	}
}
