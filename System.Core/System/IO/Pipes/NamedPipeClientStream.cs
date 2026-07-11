using System;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	[global::System.MonoTODO("working only on win32 right now")]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class NamedPipeClientStream : PipeStream
	{
		public NamedPipeClientStream(string pipeName)
			: this(".", pipeName)
		{
		}

		public NamedPipeClientStream(string serverName, string pipeName)
			: this(serverName, pipeName, PipeDirection.InOut)
		{
		}

		public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction)
			: this(serverName, pipeName, direction, PipeOptions.None)
		{
		}

		public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options)
			: this(serverName, pipeName, direction, options, TokenImpersonationLevel.None)
		{
		}

		public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options, TokenImpersonationLevel impersonationLevel)
			: this(serverName, pipeName, direction, options, impersonationLevel, HandleInheritability.None)
		{
		}

		public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options, TokenImpersonationLevel impersonationLevel, HandleInheritability inheritability)
			: this(serverName, pipeName, PipeStream.ToAccessRights(direction), options, impersonationLevel, inheritability)
		{
		}

		public NamedPipeClientStream(PipeDirection direction, bool isAsync, bool isConnected, SafePipeHandle safePipeHandle)
			: base(direction, 1024)
		{
			if (PipeStream.IsWindows)
			{
				this.impl = new Win32NamedPipeClient(this, safePipeHandle);
			}
			else
			{
				this.impl = new UnixNamedPipeClient(this, safePipeHandle);
			}
			base.IsConnected = isConnected;
			base.InitializeHandle(safePipeHandle, true, isAsync);
		}

		public NamedPipeClientStream(string serverName, string pipeName, PipeAccessRights desiredAccessRights, PipeOptions options, TokenImpersonationLevel impersonationLevel, HandleInheritability inheritability)
			: base(PipeStream.ToDirection(desiredAccessRights), 1024)
		{
			if (impersonationLevel != TokenImpersonationLevel.None || inheritability != HandleInheritability.None)
			{
				throw base.ThrowACLException();
			}
			if (PipeStream.IsWindows)
			{
				this.impl = new Win32NamedPipeClient(this, serverName, pipeName, desiredAccessRights, options, inheritability);
				return;
			}
			this.impl = new UnixNamedPipeClient(this, serverName, pipeName, desiredAccessRights, options, inheritability);
		}

		~NamedPipeClientStream()
		{
			this.Dispose(false);
		}

		public void Connect()
		{
			this.impl.Connect();
			base.InitializeHandle(this.impl.Handle, false, this.impl.IsAsync);
			base.IsConnected = true;
		}

		public void Connect(int timeout)
		{
			this.impl.Connect(timeout);
			base.InitializeHandle(this.impl.Handle, false, this.impl.IsAsync);
			base.IsConnected = true;
		}

		public Task ConnectAsync()
		{
			return this.ConnectAsync(-1, CancellationToken.None);
		}

		public Task ConnectAsync(int timeout)
		{
			return this.ConnectAsync(timeout, CancellationToken.None);
		}

		public Task ConnectAsync(CancellationToken cancellationToken)
		{
			return this.ConnectAsync(-1, cancellationToken);
		}

		public Task ConnectAsync(int timeout, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		protected internal override void CheckPipePropertyOperations()
		{
			base.CheckPipePropertyOperations();
		}

		public int NumberOfServerInstances
		{
			get
			{
				this.CheckPipePropertyOperations();
				return this.impl.NumberOfServerInstances;
			}
		}

		private INamedPipeClient impl;
	}
}
