using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	public abstract class PipeStream : Stream
	{
		internal static string GetPipePath(string serverName, string pipeName)
		{
			string fullPath = Path.GetFullPath("\\\\" + serverName + "\\pipe\\" + pipeName);
			if (string.Equals(fullPath, "\\\\.\\pipe\\anonymous", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentOutOfRangeException("pipeName", "The pipeName \\\"anonymous\\\" is reserved.");
			}
			return fullPath;
		}

		internal void ValidateHandleIsPipe(SafePipeHandle safePipeHandle)
		{
			if (global::Interop.Kernel32.GetFileType(safePipeHandle) != 3)
			{
				throw new IOException("Invalid pipe handle.");
			}
		}

		private void InitializeAsyncHandle(SafePipeHandle handle)
		{
			this._threadPoolBinding = ThreadPoolBoundHandle.BindHandle(handle);
		}

		private void DisposeCore(bool disposing)
		{
			if (disposing)
			{
				ThreadPoolBoundHandle threadPoolBinding = this._threadPoolBinding;
				if (threadPoolBinding == null)
				{
					return;
				}
				threadPoolBinding.Dispose();
			}
		}

		private int ReadCore(Span<byte> buffer)
		{
			int num = 0;
			int num2 = this.ReadFileNative(this._handle, buffer, null, out num);
			if (num2 == -1)
			{
				if (num != 109 && num != 233)
				{
					throw Win32Marshal.GetExceptionForWin32Error(num, string.Empty);
				}
				this.State = PipeState.Broken;
				num2 = 0;
			}
			this._isMessageComplete = num != 234;
			return num2;
		}

		private unsafe Task<int> ReadAsyncCore(Memory<byte> buffer, CancellationToken cancellationToken)
		{
			ReadWriteCompletionSource readWriteCompletionSource = new ReadWriteCompletionSource(this, buffer, false);
			int num = 0;
			if (this.ReadFileNative(this._handle, buffer.Span, readWriteCompletionSource.Overlapped, out num) == -1)
			{
				if (num == 109 || num == 233)
				{
					this.State = PipeState.Broken;
					readWriteCompletionSource.Overlapped->InternalLow = IntPtr.Zero;
					readWriteCompletionSource.ReleaseResources();
					this.UpdateMessageCompletion(true);
					return PipeStream.s_zeroTask;
				}
				if (num != 997)
				{
					throw Win32Marshal.GetExceptionForWin32Error(num, "");
				}
			}
			readWriteCompletionSource.RegisterForCancellation(cancellationToken);
			return readWriteCompletionSource.Task;
		}

		private void WriteCore(ReadOnlySpan<byte> buffer)
		{
			int num = 0;
			if (this.WriteFileNative(this._handle, buffer, null, out num) == -1)
			{
				throw this.WinIOError(num);
			}
		}

		private Task WriteAsyncCore(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
		{
			ReadWriteCompletionSource readWriteCompletionSource = new ReadWriteCompletionSource(this, buffer, true);
			int num = 0;
			if (this.WriteFileNative(this._handle, buffer.Span, readWriteCompletionSource.Overlapped, out num) == -1 && num != 997)
			{
				readWriteCompletionSource.ReleaseResources();
				throw this.WinIOError(num);
			}
			readWriteCompletionSource.RegisterForCancellation(cancellationToken);
			return readWriteCompletionSource.Task;
		}

		public void WaitForPipeDrain()
		{
			this.CheckWriteOperations();
			if (!this.CanWrite)
			{
				throw Error.GetWriteNotSupported();
			}
			if (!global::Interop.Kernel32.FlushFileBuffers(this._handle))
			{
				throw this.WinIOError(Marshal.GetLastWin32Error());
			}
		}

		public virtual PipeTransmissionMode TransmissionMode
		{
			get
			{
				this.CheckPipePropertyOperations();
				if (!this._isFromExistingHandle)
				{
					return this._transmissionMode;
				}
				int num;
				if (!global::Interop.Kernel32.GetNamedPipeInfo(this._handle, out num, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero))
				{
					throw this.WinIOError(Marshal.GetLastWin32Error());
				}
				if ((num & 4) != 0)
				{
					return PipeTransmissionMode.Message;
				}
				return PipeTransmissionMode.Byte;
			}
		}

		public virtual int InBufferSize
		{
			get
			{
				this.CheckPipePropertyOperations();
				if (!this.CanRead)
				{
					throw new NotSupportedException("Stream does not support reading.");
				}
				int num;
				if (!global::Interop.Kernel32.GetNamedPipeInfo(this._handle, IntPtr.Zero, IntPtr.Zero, out num, IntPtr.Zero))
				{
					throw this.WinIOError(Marshal.GetLastWin32Error());
				}
				return num;
			}
		}

		public virtual int OutBufferSize
		{
			get
			{
				this.CheckPipePropertyOperations();
				if (!this.CanWrite)
				{
					throw new NotSupportedException("Stream does not support writing.");
				}
				int outBufferSize;
				if (this._pipeDirection == PipeDirection.Out)
				{
					outBufferSize = this._outBufferSize;
				}
				else if (!global::Interop.Kernel32.GetNamedPipeInfo(this._handle, IntPtr.Zero, out outBufferSize, IntPtr.Zero, IntPtr.Zero))
				{
					throw this.WinIOError(Marshal.GetLastWin32Error());
				}
				return outBufferSize;
			}
		}

		public unsafe virtual PipeTransmissionMode ReadMode
		{
			get
			{
				this.CheckPipePropertyOperations();
				if (this._isFromExistingHandle || this.IsHandleExposed)
				{
					this.UpdateReadMode();
				}
				return this._readMode;
			}
			set
			{
				this.CheckPipePropertyOperations();
				if (value < PipeTransmissionMode.Byte || value > PipeTransmissionMode.Message)
				{
					throw new ArgumentOutOfRangeException("value", "For named pipes, transmission mode can be TransmissionMode.Byte or PipeTransmissionMode.Message. For anonymous pipes, transmission mode can be TransmissionMode.Byte.");
				}
				int num = (int)((int)value << 1);
				if (!global::Interop.Kernel32.SetNamedPipeHandleState(this._handle, &num, IntPtr.Zero, IntPtr.Zero))
				{
					throw this.WinIOError(Marshal.GetLastWin32Error());
				}
				this._readMode = value;
			}
		}

		private unsafe int ReadFileNative(SafePipeHandle handle, Span<byte> buffer, NativeOverlapped* overlapped, out int errorCode)
		{
			if (buffer.Length == 0)
			{
				errorCode = 0;
				return 0;
			}
			int num = 0;
			bool flag;
			fixed (byte* reference = MemoryMarshal.GetReference<byte>(buffer))
			{
				byte* ptr = reference;
				flag = (this._isAsync ? global::Interop.Kernel32.ReadFile(handle, ptr, buffer.Length, IntPtr.Zero, overlapped) : global::Interop.Kernel32.ReadFile(handle, ptr, buffer.Length, out num, IntPtr.Zero)) != 0;
			}
			if (flag)
			{
				errorCode = 0;
				return num;
			}
			errorCode = Marshal.GetLastWin32Error();
			if (errorCode != 234)
			{
				return -1;
			}
			return num;
		}

		private unsafe int WriteFileNative(SafePipeHandle handle, ReadOnlySpan<byte> buffer, NativeOverlapped* overlapped, out int errorCode)
		{
			if (buffer.Length == 0)
			{
				errorCode = 0;
				return 0;
			}
			int num = 0;
			bool flag;
			fixed (byte* reference = MemoryMarshal.GetReference<byte>(buffer))
			{
				byte* ptr = reference;
				flag = (this._isAsync ? global::Interop.Kernel32.WriteFile(handle, ptr, buffer.Length, IntPtr.Zero, overlapped) : global::Interop.Kernel32.WriteFile(handle, ptr, buffer.Length, out num, IntPtr.Zero)) != 0;
			}
			if (!flag)
			{
				errorCode = Marshal.GetLastWin32Error();
				return -1;
			}
			errorCode = 0;
			return num;
		}

		internal unsafe static global::Interop.Kernel32.SECURITY_ATTRIBUTES GetSecAttrs(HandleInheritability inheritability)
		{
			global::Interop.Kernel32.SECURITY_ATTRIBUTES security_ATTRIBUTES = default(global::Interop.Kernel32.SECURITY_ATTRIBUTES);
			if ((inheritability & HandleInheritability.Inheritable) != HandleInheritability.None)
			{
				security_ATTRIBUTES = default(global::Interop.Kernel32.SECURITY_ATTRIBUTES);
				security_ATTRIBUTES.nLength = (uint)sizeof(global::Interop.Kernel32.SECURITY_ATTRIBUTES);
				security_ATTRIBUTES.bInheritHandle = global::Interop.BOOL.TRUE;
			}
			return security_ATTRIBUTES;
		}

		internal unsafe static global::Interop.Kernel32.SECURITY_ATTRIBUTES GetSecAttrs(HandleInheritability inheritability, PipeSecurity pipeSecurity, ref GCHandle pinningHandle)
		{
			global::Interop.Kernel32.SECURITY_ATTRIBUTES security_ATTRIBUTES = default(global::Interop.Kernel32.SECURITY_ATTRIBUTES);
			security_ATTRIBUTES.nLength = (uint)sizeof(global::Interop.Kernel32.SECURITY_ATTRIBUTES);
			if ((inheritability & HandleInheritability.Inheritable) != HandleInheritability.None)
			{
				security_ATTRIBUTES.bInheritHandle = global::Interop.BOOL.TRUE;
			}
			if (pipeSecurity != null)
			{
				byte[] securityDescriptorBinaryForm = pipeSecurity.GetSecurityDescriptorBinaryForm();
				pinningHandle = GCHandle.Alloc(securityDescriptorBinaryForm, GCHandleType.Pinned);
				byte[] array;
				byte* ptr;
				if ((array = securityDescriptorBinaryForm) == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				security_ATTRIBUTES.lpSecurityDescriptor = (IntPtr)((void*)ptr);
				array = null;
			}
			return security_ATTRIBUTES;
		}

		private void UpdateReadMode()
		{
			int num;
			if (!global::Interop.Kernel32.GetNamedPipeHandleState(this.SafePipeHandle, out num, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0))
			{
				throw this.WinIOError(Marshal.GetLastWin32Error());
			}
			if ((num & 2) != 0)
			{
				this._readMode = PipeTransmissionMode.Message;
				return;
			}
			this._readMode = PipeTransmissionMode.Byte;
		}

		internal Exception WinIOError(int errorCode)
		{
			if (errorCode <= 38)
			{
				if (errorCode != 6)
				{
					if (errorCode == 38)
					{
						return Error.GetEndOfFile();
					}
				}
				else
				{
					this._handle.SetHandleAsInvalid();
					this._state = PipeState.Broken;
				}
			}
			else if (errorCode == 109 || errorCode - 232 <= 1)
			{
				this._state = PipeState.Broken;
				return new IOException("Pipe is broken.", Win32Marshal.MakeHRFromErrorCode(errorCode));
			}
			return Win32Marshal.GetExceptionForWin32Error(errorCode, "");
		}

		protected PipeStream(PipeDirection direction, int bufferSize)
		{
			if (direction < PipeDirection.In || direction > PipeDirection.InOut)
			{
				throw new ArgumentOutOfRangeException("direction", "For named pipes, the pipe direction can be PipeDirection.In, PipeDirection.Out or PipeDirection.InOut. For anonymous pipes, the pipe direction can be PipeDirection.In or PipeDirection.Out.");
			}
			if (bufferSize < 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", "Non negative number is required.");
			}
			this.Init(direction, PipeTransmissionMode.Byte, bufferSize);
		}

		protected PipeStream(PipeDirection direction, PipeTransmissionMode transmissionMode, int outBufferSize)
		{
			if (direction < PipeDirection.In || direction > PipeDirection.InOut)
			{
				throw new ArgumentOutOfRangeException("direction", "For named pipes, the pipe direction can be PipeDirection.In, PipeDirection.Out or PipeDirection.InOut. For anonymous pipes, the pipe direction can be PipeDirection.In or PipeDirection.Out.");
			}
			if (transmissionMode < PipeTransmissionMode.Byte || transmissionMode > PipeTransmissionMode.Message)
			{
				throw new ArgumentOutOfRangeException("transmissionMode", "For named pipes, transmission mode can be TransmissionMode.Byte or PipeTransmissionMode.Message. For anonymous pipes, transmission mode can be TransmissionMode.Byte.");
			}
			if (outBufferSize < 0)
			{
				throw new ArgumentOutOfRangeException("outBufferSize", "Non negative number is required.");
			}
			this.Init(direction, transmissionMode, outBufferSize);
		}

		private void Init(PipeDirection direction, PipeTransmissionMode transmissionMode, int outBufferSize)
		{
			this._readMode = transmissionMode;
			this._transmissionMode = transmissionMode;
			this._pipeDirection = direction;
			if ((this._pipeDirection & PipeDirection.In) != (PipeDirection)0)
			{
				this._canRead = true;
			}
			if ((this._pipeDirection & PipeDirection.Out) != (PipeDirection)0)
			{
				this._canWrite = true;
			}
			this._outBufferSize = outBufferSize;
			this._isMessageComplete = true;
			this._state = PipeState.WaitingToConnect;
		}

		protected void InitializeHandle(SafePipeHandle handle, bool isExposed, bool isAsync)
		{
			if (isAsync && handle != null)
			{
				this.InitializeAsyncHandle(handle);
			}
			this._handle = handle;
			this._isAsync = isAsync;
			this._isHandleExposed = isExposed;
			this._isFromExistingHandle = isExposed;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this._isAsync)
			{
				return this.ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
			}
			this.CheckReadWriteArgs(buffer, offset, count);
			if (!this.CanRead)
			{
				throw Error.GetReadNotSupported();
			}
			this.CheckReadOperations();
			return this.ReadCore(new Span<byte>(buffer, offset, count));
		}

		public override int Read(Span<byte> buffer)
		{
			if (this._isAsync)
			{
				return base.Read(buffer);
			}
			if (!this.CanRead)
			{
				throw Error.GetReadNotSupported();
			}
			this.CheckReadOperations();
			return this.ReadCore(buffer);
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			this.CheckReadWriteArgs(buffer, offset, count);
			if (!this.CanRead)
			{
				throw Error.GetReadNotSupported();
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			this.CheckReadOperations();
			if (!this._isAsync)
			{
				return base.ReadAsync(buffer, offset, count, cancellationToken);
			}
			if (count == 0)
			{
				this.UpdateMessageCompletion(false);
				return PipeStream.s_zeroTask;
			}
			return this.ReadAsyncCore(new Memory<byte>(buffer, offset, count), cancellationToken);
		}

		public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._isAsync)
			{
				return base.ReadAsync(buffer, cancellationToken);
			}
			if (!this.CanRead)
			{
				throw Error.GetReadNotSupported();
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));
			}
			this.CheckReadOperations();
			if (buffer.Length == 0)
			{
				this.UpdateMessageCompletion(false);
				return new ValueTask<int>(0);
			}
			return new ValueTask<int>(this.ReadAsyncCore(buffer, cancellationToken));
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (this._isAsync)
			{
				return TaskToApm.Begin(this.ReadAsync(buffer, offset, count, CancellationToken.None), callback, state);
			}
			return base.BeginRead(buffer, offset, count, callback, state);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			if (this._isAsync)
			{
				return TaskToApm.End<int>(asyncResult);
			}
			return base.EndRead(asyncResult);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this._isAsync)
			{
				this.WriteAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
				return;
			}
			this.CheckReadWriteArgs(buffer, offset, count);
			if (!this.CanWrite)
			{
				throw Error.GetWriteNotSupported();
			}
			this.CheckWriteOperations();
			this.WriteCore(new ReadOnlySpan<byte>(buffer, offset, count));
		}

		public override void Write(ReadOnlySpan<byte> buffer)
		{
			if (this._isAsync)
			{
				base.Write(buffer);
				return;
			}
			if (!this.CanWrite)
			{
				throw Error.GetWriteNotSupported();
			}
			this.CheckWriteOperations();
			this.WriteCore(buffer);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			this.CheckReadWriteArgs(buffer, offset, count);
			if (!this.CanWrite)
			{
				throw Error.GetWriteNotSupported();
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			this.CheckWriteOperations();
			if (!this._isAsync)
			{
				return base.WriteAsync(buffer, offset, count, cancellationToken);
			}
			if (count == 0)
			{
				return Task.CompletedTask;
			}
			return this.WriteAsyncCore(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken);
		}

		public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._isAsync)
			{
				return base.WriteAsync(buffer, cancellationToken);
			}
			if (!this.CanWrite)
			{
				throw Error.GetWriteNotSupported();
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask(Task.FromCanceled<int>(cancellationToken));
			}
			this.CheckWriteOperations();
			if (buffer.Length == 0)
			{
				return default(ValueTask);
			}
			return new ValueTask(this.WriteAsyncCore(buffer, cancellationToken));
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (this._isAsync)
			{
				return TaskToApm.Begin(this.WriteAsync(buffer, offset, count, CancellationToken.None), callback, state);
			}
			return base.BeginWrite(buffer, offset, count, callback, state);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			if (this._isAsync)
			{
				TaskToApm.End(asyncResult);
				return;
			}
			base.EndWrite(asyncResult);
		}

		private void CheckReadWriteArgs(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", "Buffer cannot be null.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Non negative number is required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non negative number is required.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
		}

		[Conditional("DEBUG")]
		private static void DebugAssertHandleValid(SafePipeHandle handle)
		{
		}

		[Conditional("DEBUG")]
		private static void DebugAssertReadWriteArgs(byte[] buffer, int offset, int count, SafePipeHandle handle)
		{
		}

		public unsafe override int ReadByte()
		{
			byte b;
			if (this.Read(new Span<byte>((void*)(&b), 1)) <= 0)
			{
				return -1;
			}
			return (int)b;
		}

		public unsafe override void WriteByte(byte value)
		{
			this.Write(new ReadOnlySpan<byte>((void*)(&value), 1));
		}

		public override void Flush()
		{
			this.CheckWriteOperations();
			if (!this.CanWrite)
			{
				throw Error.GetWriteNotSupported();
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (this._handle != null && !this._handle.IsClosed)
				{
					this._handle.Dispose();
				}
				this.DisposeCore(disposing);
			}
			finally
			{
				base.Dispose(disposing);
			}
			this._state = PipeState.Closed;
		}

		public bool IsConnected
		{
			get
			{
				return this.State == PipeState.Connected;
			}
			protected set
			{
				this._state = (value ? PipeState.Connected : PipeState.Disconnected);
			}
		}

		public bool IsAsync
		{
			get
			{
				return this._isAsync;
			}
		}

		public bool IsMessageComplete
		{
			get
			{
				if (this._state == PipeState.WaitingToConnect)
				{
					throw new InvalidOperationException("Pipe hasn't been connected yet.");
				}
				if (this._state == PipeState.Disconnected)
				{
					throw new InvalidOperationException("Pipe is in a disconnected state.");
				}
				if (this._handle == null)
				{
					throw new InvalidOperationException("Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?");
				}
				if (this._state == PipeState.Closed || (this._handle != null && this._handle.IsClosed))
				{
					throw Error.GetPipeNotOpen();
				}
				if (this._readMode != PipeTransmissionMode.Message)
				{
					throw new InvalidOperationException("ReadMode is not of PipeTransmissionMode.Message.");
				}
				return this._isMessageComplete;
			}
		}

		internal void UpdateMessageCompletion(bool completion)
		{
			this._isMessageComplete = completion || this._state == PipeState.Broken;
		}

		public SafePipeHandle SafePipeHandle
		{
			get
			{
				if (this._handle == null)
				{
					throw new InvalidOperationException("Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?");
				}
				if (this._handle.IsClosed)
				{
					throw Error.GetPipeNotOpen();
				}
				this._isHandleExposed = true;
				return this._handle;
			}
		}

		internal SafePipeHandle InternalHandle
		{
			get
			{
				return this._handle;
			}
		}

		protected bool IsHandleExposed
		{
			get
			{
				return this._isHandleExposed;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this._canRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this._canWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw Error.GetSeekNotSupported();
			}
		}

		public override long Position
		{
			get
			{
				throw Error.GetSeekNotSupported();
			}
			set
			{
				throw Error.GetSeekNotSupported();
			}
		}

		public override void SetLength(long value)
		{
			throw Error.GetSeekNotSupported();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw Error.GetSeekNotSupported();
		}

		protected internal virtual void CheckPipePropertyOperations()
		{
			if (this._handle == null)
			{
				throw new InvalidOperationException("Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?");
			}
			if (this._state == PipeState.Closed || (this._handle != null && this._handle.IsClosed))
			{
				throw Error.GetPipeNotOpen();
			}
		}

		protected internal void CheckReadOperations()
		{
			if (this._state == PipeState.WaitingToConnect)
			{
				throw new InvalidOperationException("Pipe hasn't been connected yet.");
			}
			if (this._state == PipeState.Disconnected)
			{
				throw new InvalidOperationException("Pipe is in a disconnected state.");
			}
			if (this._handle == null)
			{
				throw new InvalidOperationException("Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?");
			}
			if (this._state == PipeState.Closed || (this._handle != null && this._handle.IsClosed))
			{
				throw Error.GetPipeNotOpen();
			}
		}

		protected internal void CheckWriteOperations()
		{
			if (this._state == PipeState.WaitingToConnect)
			{
				throw new InvalidOperationException("Pipe hasn't been connected yet.");
			}
			if (this._state == PipeState.Disconnected)
			{
				throw new InvalidOperationException("Pipe is in a disconnected state.");
			}
			if (this._handle == null)
			{
				throw new InvalidOperationException("Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?");
			}
			if (this._state == PipeState.Broken)
			{
				throw new IOException("Pipe is broken.");
			}
			if (this._state == PipeState.Closed || (this._handle != null && this._handle.IsClosed))
			{
				throw Error.GetPipeNotOpen();
			}
		}

		internal PipeState State
		{
			get
			{
				return this._state;
			}
			set
			{
				this._state = value;
			}
		}

		internal bool IsCurrentUserOnly
		{
			get
			{
				return this._isCurrentUserOnly;
			}
			set
			{
				this._isCurrentUserOnly = value;
			}
		}

		public PipeSecurity GetAccessControl()
		{
			if (this.State == PipeState.Closed)
			{
				throw Error.GetPipeNotOpen();
			}
			return new PipeSecurity(this.SafePipeHandle, AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
		}

		public void SetAccessControl(PipeSecurity pipeSecurity)
		{
			if (pipeSecurity == null)
			{
				throw new ArgumentNullException("pipeSecurity");
			}
			this.CheckPipePropertyOperations();
			pipeSecurity.Persist(this.SafePipeHandle);
		}

		internal const bool CheckOperationsRequiresSetHandle = true;

		internal ThreadPoolBoundHandle _threadPoolBinding;

		internal const string AnonymousPipeName = "anonymous";

		private static readonly Task<int> s_zeroTask = Task.FromResult<int>(0);

		private SafePipeHandle _handle;

		private bool _canRead;

		private bool _canWrite;

		private bool _isAsync;

		private bool _isCurrentUserOnly;

		private bool _isMessageComplete;

		private bool _isFromExistingHandle;

		private bool _isHandleExposed;

		private PipeTransmissionMode _readMode;

		private PipeTransmissionMode _transmissionMode;

		private PipeDirection _pipeDirection;

		private int _outBufferSize;

		private PipeState _state;
	}
}
