using System;
using Microsoft.Win32.SafeHandles;
using Mono.Unix.Native;

namespace System.IO.Pipes
{
	internal class UnixNamedPipeServer : UnixNamedPipe, INamedPipeServer, IPipe
	{
		public UnixNamedPipeServer(NamedPipeServerStream owner, SafePipeHandle safePipeHandle)
		{
			this.handle = safePipeHandle;
		}

		public UnixNamedPipeServer(NamedPipeServerStream owner, string pipeName, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeAccessRights rights, PipeOptions options, int inBufferSize, int outBufferSize, HandleInheritability inheritability)
		{
			string text = Path.Combine("/var/tmp/", pipeName);
			base.EnsureTargetFile(text);
			base.RightsToAccess(rights);
			base.ValidateOptions(options, owner.TransmissionMode);
			FileStream fileStream = new FileStream(text, FileMode.Open, base.RightsToFileAccess(rights), FileShare.ReadWrite);
			this.handle = new SafePipeHandle(fileStream.SafeFileHandle.DangerousGetHandle(), false);
			owner.Stream = fileStream;
			this.should_close_handle = true;
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
			if (this.should_close_handle)
			{
				Stdlib.fclose(this.handle.DangerousGetHandle());
			}
		}

		public void WaitForConnection()
		{
		}

		private SafePipeHandle handle;

		private bool should_close_handle;
	}
}
