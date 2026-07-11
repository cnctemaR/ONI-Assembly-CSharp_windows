using System;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal class UnixAnonymousPipeClient : UnixAnonymousPipe, IPipe, IAnonymousPipeClient
	{
		public UnixAnonymousPipeClient(AnonymousPipeClientStream owner, SafePipeHandle handle)
		{
			this.handle = handle;
		}

		public override SafePipeHandle Handle
		{
			get
			{
				return this.handle;
			}
		}

		private SafePipeHandle handle;
	}
}
