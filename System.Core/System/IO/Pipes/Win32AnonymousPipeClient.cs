using System;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal class Win32AnonymousPipeClient : Win32AnonymousPipe, IAnonymousPipeClient, IPipe
	{
		public Win32AnonymousPipeClient(AnonymousPipeClientStream owner, SafePipeHandle handle)
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
