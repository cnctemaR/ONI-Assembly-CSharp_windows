using System;
using System.Security.Permissions;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	[MonoTODO("working only on win32 right now")]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
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
			}
			else
			{
				this.impl = new UnixNamedPipeClient(this, serverName, pipeName, desiredAccessRights, options, inheritability);
			}
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
