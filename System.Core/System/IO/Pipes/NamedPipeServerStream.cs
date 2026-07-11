using System;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	[global::System.MonoTODO("working only on win32 right now")]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class NamedPipeServerStream : PipeStream
	{
		public NamedPipeServerStream(string pipeName)
			: this(pipeName, PipeDirection.InOut)
		{
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction)
			: this(pipeName, direction, 1)
		{
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances)
			: this(pipeName, direction, maxNumberOfServerInstances, PipeTransmissionMode.Byte)
		{
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode)
			: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, PipeOptions.None)
		{
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options)
			: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, 1024, 1024)
		{
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize)
			: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, null)
		{
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity)
			: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, pipeSecurity, HandleInheritability.None)
		{
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity, HandleInheritability inheritability)
			: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, pipeSecurity, inheritability, PipeAccessRights.ReadData | PipeAccessRights.WriteData)
		{
		}

		[global::System.MonoTODO]
		public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity, HandleInheritability inheritability, PipeAccessRights additionalAccessRights)
			: base(direction, transmissionMode, outBufferSize)
		{
			PipeAccessRights pipeAccessRights = PipeStream.ToAccessRights(direction) | additionalAccessRights;
			if (PipeStream.IsWindows)
			{
				this.impl = new Win32NamedPipeServer(this, pipeName, maxNumberOfServerInstances, transmissionMode, pipeAccessRights, options, inBufferSize, outBufferSize, pipeSecurity, inheritability);
			}
			else
			{
				this.impl = new UnixNamedPipeServer(this, pipeName, maxNumberOfServerInstances, transmissionMode, pipeAccessRights, options, inBufferSize, outBufferSize, inheritability);
			}
			base.InitializeHandle(this.impl.Handle, false, (options & PipeOptions.Asynchronous) > PipeOptions.None);
		}

		public NamedPipeServerStream(PipeDirection direction, bool isAsync, bool isConnected, SafePipeHandle safePipeHandle)
			: base(direction, 1024)
		{
			if (PipeStream.IsWindows)
			{
				this.impl = new Win32NamedPipeServer(this, safePipeHandle);
			}
			else
			{
				this.impl = new UnixNamedPipeServer(this, safePipeHandle);
			}
			base.IsConnected = isConnected;
			base.InitializeHandle(safePipeHandle, true, isAsync);
		}

		~NamedPipeServerStream()
		{
		}

		public void Disconnect()
		{
			this.impl.Disconnect();
		}

		[global::System.MonoTODO]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlPrincipal)]
		public void RunAsClient(PipeStreamImpersonationWorker impersonationWorker)
		{
			throw new NotImplementedException();
		}

		public void WaitForConnection()
		{
			this.impl.WaitForConnection();
			base.IsConnected = true;
		}

		public Task WaitForConnectionAsync()
		{
			return this.WaitForConnectionAsync(CancellationToken.None);
		}

		[global::System.MonoTODO]
		public Task WaitForConnectionAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlPrincipal)]
		public string GetImpersonationUserName()
		{
			throw new NotImplementedException();
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public IAsyncResult BeginWaitForConnection(AsyncCallback callback, object state)
		{
			if (this.wait_connect_delegate == null)
			{
				this.wait_connect_delegate = new Action(this.WaitForConnection);
			}
			return this.wait_connect_delegate.BeginInvoke(callback, state);
		}

		public void EndWaitForConnection(IAsyncResult asyncResult)
		{
			this.wait_connect_delegate.EndInvoke(asyncResult);
		}

		public const int MaxAllowedServerInstances = -1;

		private INamedPipeServer impl;

		private Action wait_connect_delegate;
	}
}
