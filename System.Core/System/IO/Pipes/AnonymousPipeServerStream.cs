using System;
using System.Globalization;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	[MonoTODO("Anonymous pipes are not working even on win32, due to some access authorization issue")]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
	public sealed class AnonymousPipeServerStream : PipeStream
	{
		public AnonymousPipeServerStream()
			: this(PipeDirection.Out)
		{
		}

		public AnonymousPipeServerStream(PipeDirection direction)
			: this(direction, HandleInheritability.None)
		{
		}

		public AnonymousPipeServerStream(PipeDirection direction, HandleInheritability inheritability)
			: this(direction, inheritability, 1024)
		{
		}

		public AnonymousPipeServerStream(PipeDirection direction, HandleInheritability inheritability, int bufferSize)
			: this(direction, inheritability, bufferSize, null)
		{
		}

		public AnonymousPipeServerStream(PipeDirection direction, HandleInheritability inheritability, int bufferSize, PipeSecurity pipeSecurity)
			: base(direction, bufferSize)
		{
			if (pipeSecurity != null)
			{
				throw base.ThrowACLException();
			}
			if (direction == PipeDirection.InOut)
			{
				throw new NotSupportedException("Anonymous pipe direction can only be either in or out.");
			}
			if (PipeStream.IsWindows)
			{
				this.impl = new Win32AnonymousPipeServer(this, direction, inheritability, bufferSize);
			}
			else
			{
				this.impl = new UnixAnonymousPipeServer(this, direction, inheritability, bufferSize);
			}
			base.InitializeHandle(this.impl.Handle, false, false);
			base.IsConnected = true;
		}

		[MonoTODO]
		public AnonymousPipeServerStream(PipeDirection direction, SafePipeHandle serverSafePipeHandle, SafePipeHandle clientSafePipeHandle)
			: base(direction, 1024)
		{
			if (serverSafePipeHandle == null)
			{
				throw new ArgumentNullException("serverSafePipeHandle");
			}
			if (clientSafePipeHandle == null)
			{
				throw new ArgumentNullException("clientSafePipeHandle");
			}
			if (direction == PipeDirection.InOut)
			{
				throw new NotSupportedException("Anonymous pipe direction can only be either in or out.");
			}
			if (PipeStream.IsWindows)
			{
				this.impl = new Win32AnonymousPipeServer(this, serverSafePipeHandle, clientSafePipeHandle);
			}
			else
			{
				this.impl = new UnixAnonymousPipeServer(this, serverSafePipeHandle, clientSafePipeHandle);
			}
			base.InitializeHandle(serverSafePipeHandle, true, false);
			base.IsConnected = true;
			this.ClientSafePipeHandle = clientSafePipeHandle;
		}

		[MonoTODO]
		public SafePipeHandle ClientSafePipeHandle { get; private set; }

		public override PipeTransmissionMode ReadMode
		{
			set
			{
				if (value == PipeTransmissionMode.Message)
				{
					throw new NotSupportedException();
				}
			}
		}

		public override PipeTransmissionMode TransmissionMode
		{
			get
			{
				return PipeTransmissionMode.Byte;
			}
		}

		[MonoTODO]
		public void DisposeLocalCopyOfClientHandle()
		{
			this.impl.DisposeLocalCopyOfClientHandle();
		}

		public string GetClientHandleAsString()
		{
			return this.impl.Handle.DangerousGetHandle().ToInt64().ToString(NumberFormatInfo.InvariantInfo);
		}

		private IAnonymousPipeServer impl;
	}
}
