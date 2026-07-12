using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	public sealed class NamedPipeClientStream : PipeStream
	{
		private bool TryConnect(int timeout, CancellationToken cancellationToken)
		{
			global::Interop.Kernel32.SECURITY_ATTRIBUTES secAttrs = PipeStream.GetSecAttrs(this._inheritability);
			int num = (int)(this._pipeOptions & ~PipeOptions.CurrentUserOnly);
			if (this._impersonationLevel != TokenImpersonationLevel.None)
			{
				num |= 1048576;
				num |= this._impersonationLevel - TokenImpersonationLevel.Anonymous << 16;
			}
			int num2 = this._access;
			if ((PipeDirection.In & this._direction) != (PipeDirection)0)
			{
				num2 |= int.MinValue;
			}
			if ((PipeDirection.Out & this._direction) != (PipeDirection)0)
			{
				num2 |= 1073741824;
			}
			SafePipeHandle safePipeHandle = global::Interop.Kernel32.CreateNamedPipeClient(this._normalizedPipePath, num2, FileShare.None, ref secAttrs, FileMode.Open, num, IntPtr.Zero);
			if (safePipeHandle.IsInvalid)
			{
				int num3 = Marshal.GetLastWin32Error();
				if (num3 != 231 && num3 != 2)
				{
					throw Win32Marshal.GetExceptionForWin32Error(num3, "");
				}
				if (!global::Interop.Kernel32.WaitNamedPipe(this._normalizedPipePath, timeout))
				{
					num3 = Marshal.GetLastWin32Error();
					if (num3 == 2 || num3 == 121)
					{
						return false;
					}
					throw Win32Marshal.GetExceptionForWin32Error(num3, "");
				}
				else
				{
					safePipeHandle = global::Interop.Kernel32.CreateNamedPipeClient(this._normalizedPipePath, num2, FileShare.None, ref secAttrs, FileMode.Open, num, IntPtr.Zero);
					if (safePipeHandle.IsInvalid)
					{
						num3 = Marshal.GetLastWin32Error();
						if (num3 == 231)
						{
							return false;
						}
						throw Win32Marshal.GetExceptionForWin32Error(num3, "");
					}
				}
			}
			base.InitializeHandle(safePipeHandle, false, (this._pipeOptions & PipeOptions.Asynchronous) > PipeOptions.None);
			base.State = PipeState.Connected;
			this.ValidateRemotePipeUser();
			return true;
		}

		public int NumberOfServerInstances
		{
			get
			{
				this.CheckPipePropertyOperations();
				int num;
				if (!global::Interop.Kernel32.GetNamedPipeHandleState(base.InternalHandle, IntPtr.Zero, out num, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0))
				{
					throw base.WinIOError(Marshal.GetLastWin32Error());
				}
				return num;
			}
		}

		private void ValidateRemotePipeUser()
		{
			if (!base.IsCurrentUserOnly)
			{
				return;
			}
			IdentityReference owner = base.GetAccessControl().GetOwner(typeof(SecurityIdentifier));
			using (WindowsIdentity current = WindowsIdentity.GetCurrent())
			{
				SecurityIdentifier owner2 = current.Owner;
				if (owner != owner2)
				{
					base.State = PipeState.Closed;
					throw new UnauthorizedAccessException("Could not connect to the pipe because it was not owned by the current user.");
				}
			}
		}

		public NamedPipeClientStream(string pipeName)
			: this(".", pipeName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None, HandleInheritability.None)
		{
		}

		public NamedPipeClientStream(string serverName, string pipeName)
			: this(serverName, pipeName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None, HandleInheritability.None)
		{
		}

		public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction)
			: this(serverName, pipeName, direction, PipeOptions.None, TokenImpersonationLevel.None, HandleInheritability.None)
		{
		}

		public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options)
			: this(serverName, pipeName, direction, options, TokenImpersonationLevel.None, HandleInheritability.None)
		{
		}

		public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options, TokenImpersonationLevel impersonationLevel)
			: this(serverName, pipeName, direction, options, impersonationLevel, HandleInheritability.None)
		{
		}

		public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options, TokenImpersonationLevel impersonationLevel, HandleInheritability inheritability)
			: base(direction, 0)
		{
			if (pipeName == null)
			{
				throw new ArgumentNullException("pipeName");
			}
			if (serverName == null)
			{
				throw new ArgumentNullException("serverName", "serverName cannot be null. Use \\\".\\\" for current machine.");
			}
			if (pipeName.Length == 0)
			{
				throw new ArgumentException("pipeName cannot be an empty string.");
			}
			if (serverName.Length == 0)
			{
				throw new ArgumentException("serverName cannot be an empty string.  Use \\\\\\\".\\\\\\\" for current machine.");
			}
			if ((options & (PipeOptions)536870911) != PipeOptions.None)
			{
				throw new ArgumentOutOfRangeException("options", "options contains an invalid flag.");
			}
			if (impersonationLevel < TokenImpersonationLevel.None || impersonationLevel > TokenImpersonationLevel.Delegation)
			{
				throw new ArgumentOutOfRangeException("impersonationLevel", "TokenImpersonationLevel.None, TokenImpersonationLevel.Anonymous, TokenImpersonationLevel.Identification, TokenImpersonationLevel.Impersonation or TokenImpersonationLevel.Delegation required.");
			}
			if (inheritability < HandleInheritability.None || inheritability > HandleInheritability.Inheritable)
			{
				throw new ArgumentOutOfRangeException("inheritability", "HandleInheritability.None or HandleInheritability.Inheritable required.");
			}
			if ((options & PipeOptions.CurrentUserOnly) != PipeOptions.None)
			{
				base.IsCurrentUserOnly = true;
			}
			this._normalizedPipePath = PipeStream.GetPipePath(serverName, pipeName);
			this._direction = direction;
			this._inheritability = inheritability;
			this._impersonationLevel = impersonationLevel;
			this._pipeOptions = options;
		}

		public NamedPipeClientStream(PipeDirection direction, bool isAsync, bool isConnected, SafePipeHandle safePipeHandle)
			: base(direction, 0)
		{
			if (safePipeHandle == null)
			{
				throw new ArgumentNullException("safePipeHandle");
			}
			if (safePipeHandle.IsInvalid)
			{
				throw new ArgumentException("Invalid handle.", "safePipeHandle");
			}
			base.ValidateHandleIsPipe(safePipeHandle);
			base.InitializeHandle(safePipeHandle, true, isAsync);
			if (isConnected)
			{
				base.State = PipeState.Connected;
			}
		}

		~NamedPipeClientStream()
		{
			this.Dispose(false);
		}

		public void Connect()
		{
			this.Connect(-1);
		}

		public void Connect(int timeout)
		{
			this.CheckConnectOperationsClient();
			if (timeout < 0 && timeout != -1)
			{
				throw new ArgumentOutOfRangeException("timeout", "Timeout must be non-negative or equal to -1 (Timeout.Infinite)");
			}
			this.ConnectInternal(timeout, CancellationToken.None, Environment.TickCount);
		}

		private void ConnectInternal(int timeout, CancellationToken cancellationToken, int startTime)
		{
			int num = 0;
			SpinWait spinWait = default(SpinWait);
			for (;;)
			{
				cancellationToken.ThrowIfCancellationRequested();
				int num2 = timeout - num;
				if (cancellationToken.CanBeCanceled && num2 > 50)
				{
					num2 = 50;
				}
				if (this.TryConnect(num2, cancellationToken))
				{
					break;
				}
				spinWait.SpinOnce();
				if (timeout != -1 && (num = Environment.TickCount - startTime) >= timeout)
				{
					goto Block_5;
				}
			}
			return;
			Block_5:
			throw new TimeoutException();
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
			this.CheckConnectOperationsClient();
			if (timeout < 0 && timeout != -1)
			{
				throw new ArgumentOutOfRangeException("timeout", "Timeout must be non-negative or equal to -1 (Timeout.Infinite)");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			int startTime = Environment.TickCount;
			return Task.Run(delegate
			{
				this.ConnectInternal(timeout, cancellationToken, startTime);
			}, cancellationToken);
		}

		protected internal override void CheckPipePropertyOperations()
		{
			base.CheckPipePropertyOperations();
			if (base.State == PipeState.WaitingToConnect)
			{
				throw new InvalidOperationException("Pipe hasn't been connected yet.");
			}
			if (base.State == PipeState.Broken)
			{
				throw new IOException("Pipe is broken.");
			}
		}

		private void CheckConnectOperationsClient()
		{
			if (base.State == PipeState.Connected)
			{
				throw new InvalidOperationException("Already in a connected state.");
			}
			if (base.State == PipeState.Closed)
			{
				throw Error.GetPipeNotOpen();
			}
		}

		public NamedPipeClientStream(string serverName, string pipeName, PipeAccessRights desiredAccessRights, PipeOptions options, TokenImpersonationLevel impersonationLevel, HandleInheritability inheritability)
			: this(serverName, pipeName, (PipeDirection)(desiredAccessRights & (PipeAccessRights.ReadData | PipeAccessRights.WriteData)), options, impersonationLevel, inheritability)
		{
			if ((desiredAccessRights & ~(PipeAccessRights.ReadData | PipeAccessRights.WriteData | PipeAccessRights.ReadAttributes | PipeAccessRights.WriteAttributes | PipeAccessRights.ReadExtendedAttributes | PipeAccessRights.WriteExtendedAttributes | PipeAccessRights.CreateNewInstance | PipeAccessRights.Delete | PipeAccessRights.ReadPermissions | PipeAccessRights.ChangePermissions | PipeAccessRights.TakeOwnership | PipeAccessRights.Synchronize | PipeAccessRights.AccessSystemSecurity)) != (PipeAccessRights)0)
			{
				throw new ArgumentOutOfRangeException("desiredAccessRights", "Invalid PipeAccessRights flag.");
			}
			this._access = (int)desiredAccessRights;
		}

		private const int CancellationCheckInterval = 50;

		private readonly string _normalizedPipePath;

		private readonly TokenImpersonationLevel _impersonationLevel;

		private readonly PipeOptions _pipeOptions;

		private readonly HandleInheritability _inheritability;

		private readonly PipeDirection _direction;

		private int _access;
	}
}
