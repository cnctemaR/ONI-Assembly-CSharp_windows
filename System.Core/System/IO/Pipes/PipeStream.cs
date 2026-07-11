using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public abstract class PipeStream : Stream
	{
		internal static bool IsWindows
		{
			get
			{
				return Win32Marshal.IsWindows;
			}
		}

		internal Exception ThrowACLException()
		{
			return new NotImplementedException("ACL is not supported in Mono");
		}

		internal static PipeAccessRights ToAccessRights(PipeDirection direction)
		{
			switch (direction)
			{
			case PipeDirection.In:
				return PipeAccessRights.ReadData;
			case PipeDirection.Out:
				return PipeAccessRights.WriteData;
			case PipeDirection.InOut:
				return PipeAccessRights.ReadData | PipeAccessRights.WriteData;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		internal static PipeDirection ToDirection(PipeAccessRights rights)
		{
			bool flag = (rights & PipeAccessRights.ReadData) > (PipeAccessRights)0;
			bool flag2 = (rights & PipeAccessRights.WriteData) > (PipeAccessRights)0;
			if (flag)
			{
				if (flag2)
				{
					return PipeDirection.InOut;
				}
				return PipeDirection.In;
			}
			else
			{
				if (flag2)
				{
					return PipeDirection.Out;
				}
				throw new ArgumentOutOfRangeException();
			}
		}

		protected PipeStream(PipeDirection direction, int bufferSize)
			: this(direction, PipeTransmissionMode.Byte, bufferSize)
		{
		}

		protected PipeStream(PipeDirection direction, PipeTransmissionMode transmissionMode, int outBufferSize)
		{
			this.direction = direction;
			this.transmission_mode = transmissionMode;
			this.read_trans_mode = transmissionMode;
			if (outBufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize must be greater than 0");
			}
			this.buffer_size = outBufferSize;
		}

		public override bool CanRead
		{
			get
			{
				return (this.direction & PipeDirection.In) > (PipeDirection)0;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return (this.direction & PipeDirection.Out) > (PipeDirection)0;
			}
		}

		public virtual int InBufferSize
		{
			get
			{
				return this.buffer_size;
			}
		}

		public bool IsAsync { get; private set; }

		public bool IsConnected { get; protected set; }

		internal Stream Stream
		{
			get
			{
				if (!this.IsConnected)
				{
					throw new InvalidOperationException("Pipe is not connected");
				}
				if (this.stream == null)
				{
					this.stream = new FileStream(this.handle.DangerousGetHandle(), this.CanRead ? (this.CanWrite ? FileAccess.ReadWrite : FileAccess.Read) : FileAccess.Write, false, this.buffer_size, this.IsAsync);
				}
				return this.stream;
			}
			set
			{
				this.stream = value;
			}
		}

		private protected bool IsHandleExposed { protected get; private set; }

		[global::System.MonoTODO]
		public bool IsMessageComplete { get; private set; }

		[global::System.MonoTODO]
		public virtual int OutBufferSize
		{
			get
			{
				return this.buffer_size;
			}
		}

		public virtual PipeTransmissionMode ReadMode
		{
			get
			{
				this.CheckPipePropertyOperations();
				return this.read_trans_mode;
			}
			set
			{
				this.CheckPipePropertyOperations();
				this.read_trans_mode = value;
			}
		}

		public SafePipeHandle SafePipeHandle
		{
			get
			{
				this.CheckPipePropertyOperations();
				return this.handle;
			}
		}

		public virtual PipeTransmissionMode TransmissionMode
		{
			get
			{
				this.CheckPipePropertyOperations();
				return this.transmission_mode;
			}
		}

		[global::System.MonoTODO]
		protected internal virtual void CheckPipePropertyOperations()
		{
		}

		[global::System.MonoTODO]
		protected internal void CheckReadOperations()
		{
			if (!this.IsConnected)
			{
				throw new InvalidOperationException("Pipe is not connected");
			}
			if (!this.CanRead)
			{
				throw new NotSupportedException("The pipe stream does not support read operations");
			}
		}

		[global::System.MonoTODO]
		protected internal void CheckWriteOperations()
		{
			if (!this.IsConnected)
			{
				throw new InvalidOperationException("Pipe is not connected");
			}
			if (!this.CanWrite)
			{
				throw new NotSupportedException("The pipe stream does not support write operations");
			}
		}

		protected void InitializeHandle(SafePipeHandle handle, bool isExposed, bool isAsync)
		{
			this.handle = handle;
			this.IsHandleExposed = isExposed;
			this.IsAsync = isAsync;
		}

		protected override void Dispose(bool disposing)
		{
			if (this.handle != null && disposing)
			{
				this.handle.Dispose();
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				return 0L;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public PipeSecurity GetAccessControl()
		{
			return new PipeSecurity(this.SafePipeHandle, AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
		}

		public void SetAccessControl(PipeSecurity pipeSecurity)
		{
			if (pipeSecurity == null)
			{
				throw new ArgumentNullException("pipeSecurity");
			}
			pipeSecurity.Persist(this.SafePipeHandle);
		}

		public void WaitForPipeDrain()
		{
		}

		[global::System.MonoTODO]
		public override int Read([In] byte[] buffer, int offset, int count)
		{
			this.CheckReadOperations();
			return this.Stream.Read(buffer, offset, count);
		}

		[global::System.MonoTODO]
		public override int ReadByte()
		{
			this.CheckReadOperations();
			return this.Stream.ReadByte();
		}

		[global::System.MonoTODO]
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckWriteOperations();
			this.Stream.Write(buffer, offset, count);
		}

		[global::System.MonoTODO]
		public override void WriteByte(byte value)
		{
			this.CheckWriteOperations();
			this.Stream.WriteByte(value);
		}

		[global::System.MonoTODO]
		public override void Flush()
		{
			this.CheckWriteOperations();
			this.Stream.Flush();
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (this.read_delegate == null)
			{
				this.read_delegate = new Func<byte[], int, int, int>(this.Read);
			}
			return this.read_delegate.BeginInvoke(buffer, offset, count, callback, state);
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (this.write_delegate == null)
			{
				this.write_delegate = new Action<byte[], int, int>(this.Write);
			}
			return this.write_delegate.BeginInvoke(buffer, offset, count, callback, state);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return this.read_delegate.EndInvoke(asyncResult);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			this.write_delegate.EndInvoke(asyncResult);
		}

		internal const int DefaultBufferSize = 1024;

		private PipeDirection direction;

		private PipeTransmissionMode transmission_mode;

		private PipeTransmissionMode read_trans_mode;

		private int buffer_size;

		private SafePipeHandle handle;

		private Stream stream;

		private Func<byte[], int, int, int> read_delegate;

		private Action<byte[], int, int> write_delegate;
	}
}
