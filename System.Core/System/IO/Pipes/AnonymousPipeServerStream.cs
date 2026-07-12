using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	public sealed class AnonymousPipeServerStream : PipeStream
	{
		private void Create(PipeDirection direction, HandleInheritability inheritability, int bufferSize)
		{
			this.Create(direction, inheritability, bufferSize, null);
		}

		private void Create(PipeDirection direction, HandleInheritability inheritability, int bufferSize, PipeSecurity pipeSecurity)
		{
			GCHandle gchandle = default(GCHandle);
			SafePipeHandle safePipeHandle;
			bool flag;
			try
			{
				global::Interop.Kernel32.SECURITY_ATTRIBUTES secAttrs = PipeStream.GetSecAttrs(inheritability, pipeSecurity, ref gchandle);
				if (direction == PipeDirection.In)
				{
					flag = global::Interop.Kernel32.CreatePipe(out safePipeHandle, out this._clientHandle, ref secAttrs, bufferSize);
				}
				else
				{
					flag = global::Interop.Kernel32.CreatePipe(out this._clientHandle, out safePipeHandle, ref secAttrs, bufferSize);
				}
			}
			finally
			{
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
			}
			if (!flag)
			{
				throw Win32Marshal.GetExceptionForLastWin32Error("");
			}
			SafePipeHandle safePipeHandle2;
			flag = global::Interop.Kernel32.DuplicateHandle(global::Interop.Kernel32.GetCurrentProcess(), safePipeHandle, global::Interop.Kernel32.GetCurrentProcess(), out safePipeHandle2, 0U, false, 2U);
			if (!flag)
			{
				throw Win32Marshal.GetExceptionForLastWin32Error("");
			}
			safePipeHandle.Dispose();
			base.InitializeHandle(safePipeHandle2, false, false);
			base.State = PipeState.Connected;
		}

		public AnonymousPipeServerStream()
			: this(PipeDirection.Out, HandleInheritability.None, 0)
		{
		}

		public AnonymousPipeServerStream(PipeDirection direction)
			: this(direction, HandleInheritability.None, 0)
		{
		}

		public AnonymousPipeServerStream(PipeDirection direction, HandleInheritability inheritability)
			: this(direction, inheritability, 0)
		{
		}

		public AnonymousPipeServerStream(PipeDirection direction, SafePipeHandle serverSafePipeHandle, SafePipeHandle clientSafePipeHandle)
			: base(direction, 0)
		{
			if (direction == PipeDirection.InOut)
			{
				throw new NotSupportedException("Anonymous pipes can only be in one direction.");
			}
			if (serverSafePipeHandle == null)
			{
				throw new ArgumentNullException("serverSafePipeHandle");
			}
			if (clientSafePipeHandle == null)
			{
				throw new ArgumentNullException("clientSafePipeHandle");
			}
			if (serverSafePipeHandle.IsInvalid)
			{
				throw new ArgumentException("Invalid handle.", "serverSafePipeHandle");
			}
			if (clientSafePipeHandle.IsInvalid)
			{
				throw new ArgumentException("Invalid handle.", "clientSafePipeHandle");
			}
			base.ValidateHandleIsPipe(serverSafePipeHandle);
			base.ValidateHandleIsPipe(clientSafePipeHandle);
			base.InitializeHandle(serverSafePipeHandle, true, false);
			this._clientHandle = clientSafePipeHandle;
			this._clientHandleExposed = true;
			base.State = PipeState.Connected;
		}

		public AnonymousPipeServerStream(PipeDirection direction, HandleInheritability inheritability, int bufferSize)
			: base(direction, bufferSize)
		{
			if (direction == PipeDirection.InOut)
			{
				throw new NotSupportedException("Anonymous pipes can only be in one direction.");
			}
			if (inheritability < HandleInheritability.None || inheritability > HandleInheritability.Inheritable)
			{
				throw new ArgumentOutOfRangeException("inheritability", "HandleInheritability.None or HandleInheritability.Inheritable required.");
			}
			this.Create(direction, inheritability, bufferSize);
		}

		~AnonymousPipeServerStream()
		{
			this.Dispose(false);
		}

		public string GetClientHandleAsString()
		{
			this._clientHandleExposed = true;
			GC.SuppressFinalize(this._clientHandle);
			return this._clientHandle.DangerousGetHandle().ToString();
		}

		public SafePipeHandle ClientSafePipeHandle
		{
			get
			{
				this._clientHandleExposed = true;
				return this._clientHandle;
			}
		}

		public void DisposeLocalCopyOfClientHandle()
		{
			if (this._clientHandle != null && !this._clientHandle.IsClosed)
			{
				this._clientHandle.Dispose();
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this._clientHandleExposed && this._clientHandle != null && !this._clientHandle.IsClosed)
				{
					this._clientHandle.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override PipeTransmissionMode TransmissionMode
		{
			get
			{
				return PipeTransmissionMode.Byte;
			}
		}

		public override PipeTransmissionMode ReadMode
		{
			set
			{
				this.CheckPipePropertyOperations();
				if (value < PipeTransmissionMode.Byte || value > PipeTransmissionMode.Message)
				{
					throw new ArgumentOutOfRangeException("value", "For named pipes, transmission mode can be TransmissionMode.Byte or PipeTransmissionMode.Message. For anonymous pipes, transmission mode can be TransmissionMode.Byte.");
				}
				if (value == PipeTransmissionMode.Message)
				{
					throw new NotSupportedException("Anonymous pipes do not support PipeTransmissionMode.Message ReadMode.");
				}
			}
		}

		public AnonymousPipeServerStream(PipeDirection direction, HandleInheritability inheritability, int bufferSize, PipeSecurity pipeSecurity)
			: base(direction, bufferSize)
		{
			if (direction == PipeDirection.InOut)
			{
				throw new NotSupportedException("Anonymous pipes can only be in one direction.");
			}
			if (inheritability < HandleInheritability.None || inheritability > HandleInheritability.Inheritable)
			{
				throw new ArgumentOutOfRangeException("inheritability", "HandleInheritability.None or HandleInheritability.Inheritable required.");
			}
			this.Create(direction, inheritability, bufferSize, pipeSecurity);
		}

		private SafePipeHandle _clientHandle;

		private bool _clientHandleExposed;
	}
}
