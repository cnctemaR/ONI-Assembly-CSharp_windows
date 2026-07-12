using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	public sealed class NamedPipeServerStream : PipeStream
	{
		private void Create(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, HandleInheritability inheritability)
		{
			this.Create(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, null, inheritability, (PipeAccessRights)0);
		}

		private void Create(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity, HandleInheritability inheritability, PipeAccessRights additionalAccessRights)
		{
			string fullPath = Path.GetFullPath("\\\\.\\pipe\\" + pipeName);
			if (string.Equals(fullPath, "\\\\.\\pipe\\anonymous", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentOutOfRangeException("pipeName", "The pipeName \\\"anonymous\\\" is reserved.");
			}
			if (base.IsCurrentUserOnly)
			{
				using (WindowsIdentity current = WindowsIdentity.GetCurrent())
				{
					SecurityIdentifier owner = current.Owner;
					PipeAccessRule pipeAccessRule = new PipeAccessRule(owner, PipeAccessRights.FullControl, AccessControlType.Allow);
					pipeSecurity = new PipeSecurity();
					pipeSecurity.AddAccessRule(pipeAccessRule);
					pipeSecurity.SetOwner(owner);
				}
				options &= ~PipeOptions.CurrentUserOnly;
			}
			int num = (int)(direction | ((maxNumberOfServerInstances == 1) ? ((PipeDirection)524288) : ((PipeDirection)0)) | (PipeDirection)options | (PipeDirection)additionalAccessRights);
			int num2 = (int)(((int)transmissionMode << 2) | ((int)transmissionMode << 1));
			if (maxNumberOfServerInstances == -1)
			{
				maxNumberOfServerInstances = 255;
			}
			GCHandle gchandle = default(GCHandle);
			try
			{
				global::Interop.Kernel32.SECURITY_ATTRIBUTES secAttrs = PipeStream.GetSecAttrs(inheritability, pipeSecurity, ref gchandle);
				SafePipeHandle safePipeHandle = global::Interop.Kernel32.CreateNamedPipe(fullPath, num, num2, maxNumberOfServerInstances, outBufferSize, inBufferSize, 0, ref secAttrs);
				if (safePipeHandle.IsInvalid)
				{
					throw Win32Marshal.GetExceptionForLastWin32Error("");
				}
				base.InitializeHandle(safePipeHandle, false, (options & PipeOptions.Asynchronous) > PipeOptions.None);
			}
			finally
			{
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
			}
		}

		public void WaitForConnection()
		{
			this.CheckConnectOperationsServerWithHandle();
			if (base.IsAsync)
			{
				this.WaitForConnectionCoreAsync(CancellationToken.None).GetAwaiter().GetResult();
				return;
			}
			if (!global::Interop.Kernel32.ConnectNamedPipe(base.InternalHandle, IntPtr.Zero))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 535)
				{
					throw Win32Marshal.GetExceptionForWin32Error(lastWin32Error, "");
				}
				if (lastWin32Error == 535 && base.State == PipeState.Connected)
				{
					throw new InvalidOperationException("Already in a connected state.");
				}
			}
			base.State = PipeState.Connected;
		}

		public Task WaitForConnectionAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			if (!base.IsAsync)
			{
				return Task.Factory.StartNew(delegate(object s)
				{
					((NamedPipeServerStream)s).WaitForConnection();
				}, this, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
			}
			return this.WaitForConnectionCoreAsync(cancellationToken);
		}

		public void Disconnect()
		{
			this.CheckDisconnectOperations();
			if (!global::Interop.Kernel32.DisconnectNamedPipe(base.InternalHandle))
			{
				throw Win32Marshal.GetExceptionForLastWin32Error("");
			}
			base.State = PipeState.Disconnected;
		}

		public string GetImpersonationUserName()
		{
			base.CheckWriteOperations();
			StringBuilder stringBuilder = new StringBuilder(514);
			if (!global::Interop.Kernel32.GetNamedPipeHandleState(base.InternalHandle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, stringBuilder, stringBuilder.Capacity))
			{
				throw base.WinIOError(Marshal.GetLastWin32Error());
			}
			return stringBuilder.ToString();
		}

		public void RunAsClient(PipeStreamImpersonationWorker impersonationWorker)
		{
			base.CheckWriteOperations();
			NamedPipeServerStream.ExecuteHelper executeHelper = new NamedPipeServerStream.ExecuteHelper(impersonationWorker, base.InternalHandle);
			RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(NamedPipeServerStream.tryCode, NamedPipeServerStream.cleanupCode, executeHelper);
			if (executeHelper._impersonateErrorCode != 0)
			{
				throw base.WinIOError(executeHelper._impersonateErrorCode);
			}
			if (executeHelper._revertImpersonateErrorCode != 0)
			{
				throw base.WinIOError(executeHelper._revertImpersonateErrorCode);
			}
		}

		private static void ImpersonateAndTryCode(object helper)
		{
			NamedPipeServerStream.ExecuteHelper executeHelper = (NamedPipeServerStream.ExecuteHelper)helper;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				if (global::Interop.Advapi32.ImpersonateNamedPipeClient(executeHelper._handle))
				{
					executeHelper._mustRevert = true;
				}
				else
				{
					executeHelper._impersonateErrorCode = Marshal.GetLastWin32Error();
				}
			}
			if (executeHelper._mustRevert)
			{
				executeHelper._userCode();
			}
		}

		private static void RevertImpersonationOnBackout(object helper, bool exceptionThrown)
		{
			NamedPipeServerStream.ExecuteHelper executeHelper = (NamedPipeServerStream.ExecuteHelper)helper;
			if (executeHelper._mustRevert && !global::Interop.Advapi32.RevertToSelf())
			{
				executeHelper._revertImpersonateErrorCode = Marshal.GetLastWin32Error();
			}
		}

		private Task WaitForConnectionCoreAsync(CancellationToken cancellationToken)
		{
			this.CheckConnectOperationsServerWithHandle();
			if (!base.IsAsync)
			{
				throw new InvalidOperationException("Pipe is not opened in asynchronous mode.");
			}
			ConnectionCompletionSource connectionCompletionSource = new ConnectionCompletionSource(this);
			if (!global::Interop.Kernel32.ConnectNamedPipe(base.InternalHandle, connectionCompletionSource.Overlapped))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 535)
				{
					if (lastWin32Error != 997)
					{
						connectionCompletionSource.ReleaseResources();
						throw Win32Marshal.GetExceptionForWin32Error(lastWin32Error, "");
					}
				}
				else
				{
					connectionCompletionSource.ReleaseResources();
					if (base.State == PipeState.Connected)
					{
						throw new InvalidOperationException("Already in a connected state.");
					}
					connectionCompletionSource.SetCompletedSynchronously();
					return Task.CompletedTask;
				}
			}
			connectionCompletionSource.RegisterForCancellation(cancellationToken);
			return connectionCompletionSource.Task;
		}

		private void CheckConnectOperationsServerWithHandle()
		{
			if (base.InternalHandle == null)
			{
				throw new InvalidOperationException("Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?");
			}
			this.CheckConnectOperationsServer();
		}

		public NamedPipeServerStream(string pipeName)
			: this(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, HandleInheritability.None)
		{
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction)
			: this(pipeName, direction, 1, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, HandleInheritability.None)
		{
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances)
			: this(pipeName, direction, maxNumberOfServerInstances, PipeTransmissionMode.Byte, PipeOptions.None, 0, 0, HandleInheritability.None)
		{
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode)
			: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, PipeOptions.None, 0, 0, HandleInheritability.None)
		{
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options)
			: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, 0, 0, HandleInheritability.None)
		{
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize)
			: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, HandleInheritability.None)
		{
		}

		private NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, HandleInheritability inheritability)
			: base(direction, transmissionMode, outBufferSize)
		{
			if (pipeName == null)
			{
				throw new ArgumentNullException("pipeName");
			}
			if (pipeName.Length == 0)
			{
				throw new ArgumentException("pipeName cannot be an empty string.");
			}
			if ((options & (PipeOptions)536870911) != PipeOptions.None)
			{
				throw new ArgumentOutOfRangeException("options", "options contains an invalid flag.");
			}
			if (inBufferSize < 0)
			{
				throw new ArgumentOutOfRangeException("inBufferSize", "Non negative number is required.");
			}
			if ((maxNumberOfServerInstances < 1 || maxNumberOfServerInstances > 254) && maxNumberOfServerInstances != -1)
			{
				throw new ArgumentOutOfRangeException("maxNumberOfServerInstances", "maxNumberOfServerInstances must either be a value between 1 and 254, or NamedPipeServerStream.MaxAllowedServerInstances (to obtain the maximum number allowed by system resources).");
			}
			if (inheritability < HandleInheritability.None || inheritability > HandleInheritability.Inheritable)
			{
				throw new ArgumentOutOfRangeException("inheritability", "HandleInheritability.None or HandleInheritability.Inheritable required.");
			}
			if ((options & PipeOptions.CurrentUserOnly) != PipeOptions.None)
			{
				base.IsCurrentUserOnly = true;
			}
			this.Create(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, inheritability);
		}

		public NamedPipeServerStream(PipeDirection direction, bool isAsync, bool isConnected, SafePipeHandle safePipeHandle)
			: base(direction, PipeTransmissionMode.Byte, 0)
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

		~NamedPipeServerStream()
		{
			this.Dispose(false);
		}

		public Task WaitForConnectionAsync()
		{
			return this.WaitForConnectionAsync(CancellationToken.None);
		}

		public IAsyncResult BeginWaitForConnection(AsyncCallback callback, object state)
		{
			return TaskToApm.Begin(this.WaitForConnectionAsync(), callback, state);
		}

		public void EndWaitForConnection(IAsyncResult asyncResult)
		{
			TaskToApm.End(asyncResult);
		}

		private void CheckConnectOperationsServer()
		{
			if (base.State == PipeState.Closed)
			{
				throw Error.GetPipeNotOpen();
			}
			if (base.InternalHandle != null && base.InternalHandle.IsClosed)
			{
				throw Error.GetPipeNotOpen();
			}
			if (base.State == PipeState.Broken)
			{
				throw new IOException("Pipe is broken.");
			}
		}

		private void CheckDisconnectOperations()
		{
			if (base.State == PipeState.WaitingToConnect)
			{
				throw new InvalidOperationException("Pipe hasn't been connected yet.");
			}
			if (base.State == PipeState.Disconnected)
			{
				throw new InvalidOperationException("Already in a disconnected state.");
			}
			if (base.InternalHandle == null)
			{
				throw new InvalidOperationException("Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?");
			}
			if (base.State == PipeState.Closed || (base.InternalHandle != null && base.InternalHandle.IsClosed))
			{
				throw Error.GetPipeNotOpen();
			}
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity)
			: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, HandleInheritability.None)
		{
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity, HandleInheritability inheritability)
			: this(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, inheritability)
		{
		}

		public NamedPipeServerStream(string pipeName, PipeDirection direction, int maxNumberOfServerInstances, PipeTransmissionMode transmissionMode, PipeOptions options, int inBufferSize, int outBufferSize, PipeSecurity pipeSecurity, HandleInheritability inheritability, PipeAccessRights additionalAccessRights)
			: base(direction, transmissionMode, outBufferSize)
		{
			if (pipeName == null)
			{
				throw new ArgumentNullException("pipeName");
			}
			if (pipeName.Length == 0)
			{
				throw new ArgumentException("pipeName cannot be an empty string.");
			}
			if ((options & (PipeOptions)1073741823) != PipeOptions.None)
			{
				throw new ArgumentOutOfRangeException("options", "options contains an invalid flag.");
			}
			if (inBufferSize < 0)
			{
				throw new ArgumentOutOfRangeException("inBufferSize", "Non negative number is required.");
			}
			if ((maxNumberOfServerInstances < 1 || maxNumberOfServerInstances > 254) && maxNumberOfServerInstances != -1)
			{
				throw new ArgumentOutOfRangeException("maxNumberOfServerInstances", "maxNumberOfServerInstances must either be a value between 1 and 254, or NamedPipeServerStream.MaxAllowedServerInstances (to obtain the maximum number allowed by system resources).");
			}
			if (inheritability < HandleInheritability.None || inheritability > HandleInheritability.Inheritable)
			{
				throw new ArgumentOutOfRangeException("inheritability", "HandleInheritability.None or HandleInheritability.Inheritable required.");
			}
			if ((additionalAccessRights & ~(PipeAccessRights.ChangePermissions | PipeAccessRights.TakeOwnership | PipeAccessRights.AccessSystemSecurity)) != (PipeAccessRights)0)
			{
				throw new ArgumentOutOfRangeException("additionalAccessRights", "additionalAccessRights is limited to the PipeAccessRights.ChangePermissions, PipeAccessRights.TakeOwnership, and PipeAccessRights.AccessSystemSecurity flags when creating NamedPipeServerStreams.");
			}
			this.Create(pipeName, direction, maxNumberOfServerInstances, transmissionMode, options, inBufferSize, outBufferSize, pipeSecurity, inheritability, additionalAccessRights);
		}

		private static RuntimeHelpers.TryCode tryCode = new RuntimeHelpers.TryCode(NamedPipeServerStream.ImpersonateAndTryCode);

		private static RuntimeHelpers.CleanupCode cleanupCode = new RuntimeHelpers.CleanupCode(NamedPipeServerStream.RevertImpersonationOnBackout);

		public const int MaxAllowedServerInstances = -1;

		internal class ExecuteHelper
		{
			internal ExecuteHelper(PipeStreamImpersonationWorker userCode, SafePipeHandle handle)
			{
				this._userCode = userCode;
				this._handle = handle;
			}

			internal PipeStreamImpersonationWorker _userCode;

			internal SafePipeHandle _handle;

			internal bool _mustRevert;

			internal int _impersonateErrorCode;

			internal int _revertImpersonateErrorCode;
		}
	}
}
