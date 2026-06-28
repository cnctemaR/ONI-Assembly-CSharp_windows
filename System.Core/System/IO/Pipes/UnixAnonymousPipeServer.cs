using System;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal class UnixAnonymousPipeServer : UnixAnonymousPipe, IPipe, IAnonymousPipeServer
	{
		public UnixAnonymousPipeServer(AnonymousPipeServerStream owner, PipeDirection direction, HandleInheritability inheritability, int bufferSize)
		{
			throw new NotImplementedException();
		}

		public UnixAnonymousPipeServer(AnonymousPipeServerStream owner, SafePipeHandle serverHandle, SafePipeHandle clientHandle)
		{
			this.server_handle = serverHandle;
			this.client_handle = clientHandle;
			throw new NotImplementedException();
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
