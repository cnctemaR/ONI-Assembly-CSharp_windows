using System;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	internal class UnixNamedPipeClient : UnixNamedPipe, INamedPipeClient, IPipe
	{
		public UnixNamedPipeClient(NamedPipeClientStream owner, SafePipeHandle safePipeHandle)
		{
			this.owner = owner;
			this.handle = safePipeHandle;
		}

		public UnixNamedPipeClient(NamedPipeClientStream owner, string serverName, string pipeName, PipeAccessRights desiredAccessRights, PipeOptions options, HandleInheritability inheritability)
		{
			UnixNamedPipeClient.<>c__DisplayClass1_0 CS$<>8__locals1 = new UnixNamedPipeClient.<>c__DisplayClass1_0();
			CS$<>8__locals1.desiredAccessRights = desiredAccessRights;
			CS$<>8__locals1.owner = owner;
			base..ctor();
			CS$<>8__locals1.<>4__this = this;
			this.owner = CS$<>8__locals1.owner;
			if (serverName != "." && !Dns.GetHostEntry(serverName).AddressList.Contains(IPAddress.Loopback))
			{
				throw new NotImplementedException("Unix fifo does not support remote server connection");
			}
			string name = Path.Combine("/var/tmp/", pipeName);
			base.EnsureTargetFile(name);
			base.RightsToAccess(CS$<>8__locals1.desiredAccessRights);
			base.ValidateOptions(options, CS$<>8__locals1.owner.TransmissionMode);
			this.opener = delegate
			{
				FileStream fileStream = new FileStream(name, FileMode.Open, CS$<>8__locals1.<>4__this.RightsToFileAccess(CS$<>8__locals1.desiredAccessRights), FileShare.ReadWrite);
				CS$<>8__locals1.owner.Stream = fileStream;
				CS$<>8__locals1.<>4__this.handle = new SafePipeHandle(fileStream.SafeFileHandle.DangerousGetHandle(), false);
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
				return false;
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

		private SafePipeHandle handle;

		private Action opener;
	}
}
