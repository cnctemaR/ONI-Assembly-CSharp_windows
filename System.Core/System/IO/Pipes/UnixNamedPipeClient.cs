using System;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal class UnixNamedPipeClient : UnixNamedPipe, IPipe, INamedPipeClient
	{
		public UnixNamedPipeClient(NamedPipeClientStream owner, SafePipeHandle safePipeHandle)
		{
			this.owner = owner;
			this.handle = safePipeHandle;
		}

		public UnixNamedPipeClient(NamedPipeClientStream owner, string serverName, string pipeName, PipeAccessRights desiredAccessRights, PipeOptions options, HandleInheritability inheritability)
		{
			UnixNamedPipeClient <>f__this = this;
			this.owner = owner;
			if (serverName != "." && !Dns.GetHostEntry(serverName).AddressList.Contains(IPAddress.Loopback))
			{
				throw new NotImplementedException("Unix fifo does not support remote server connection");
			}
			string name = Path.Combine("/var/tmp/", pipeName);
			base.EnsureTargetFile(name);
			string text = base.RightsToAccess(desiredAccessRights);
			base.ValidateOptions(options, owner.TransmissionMode);
			this.opener = delegate
			{
				FileStream fileStream = new FileStream(name, FileMode.Open, <>f__this.RightsToFileAccess(desiredAccessRights), FileShare.ReadWrite);
				owner.Stream = fileStream;
				<>f__this.handle = new SafePipeHandle(fileStream.Handle, false);
			};
		}

		public override SafePipeHandle Handle
		{
			get
			{
				return this.handle;
			}
		}

		public void Connect()
		{
			if (this.owner.IsConnected)
			{
				throw new InvalidOperationException("The named pipe is already connected");
			}
			this.opener();
		}

		public void Connect(int timeout)
		{
			AutoResetEvent waitHandle = new AutoResetEvent(false);
			this.opener.BeginInvoke(delegate(IAsyncResult result)
			{
				this.opener.EndInvoke(result);
				waitHandle.Set();
			}, null);
			if (!waitHandle.WaitOne(TimeSpan.FromMilliseconds((double)timeout)))
			{
				throw new TimeoutException();
			}
		}

		public bool IsAsync
		{
			get
			{
				return this.is_async;
			}
		}

		public int NumberOfServerInstances
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		private NamedPipeClientStream owner;

		private bool is_async;

		private SafePipeHandle handle;

		private Action opener;
	}
}
