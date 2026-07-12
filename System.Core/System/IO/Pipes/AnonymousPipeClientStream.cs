using System;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	public sealed class AnonymousPipeClientStream : PipeStream
	{
		public AnonymousPipeClientStream(string pipeHandleAsString)
			: this(PipeDirection.In, pipeHandleAsString)
		{
		}

		public AnonymousPipeClientStream(PipeDirection direction, string pipeHandleAsString)
			: base(direction, 0)
		{
			if (direction == PipeDirection.InOut)
			{
				throw new NotSupportedException("Anonymous pipes can only be in one direction.");
			}
			if (pipeHandleAsString == null)
			{
				throw new ArgumentNullException("pipeHandleAsString");
			}
			long num = 0L;
			if (!long.TryParse(pipeHandleAsString, out num))
			{
				throw new ArgumentException("Invalid handle.", "pipeHandleAsString");
			}
			SafePipeHandle safePipeHandle = new SafePipeHandle((IntPtr)num, true);
			if (safePipeHandle.IsInvalid)
			{
				throw new ArgumentException("Invalid handle.", "pipeHandleAsString");
			}
			this.Init(direction, safePipeHandle);
		}

		public AnonymousPipeClientStream(PipeDirection direction, SafePipeHandle safePipeHandle)
			: base(direction, 0)
		{
			if (direction == PipeDirection.InOut)
			{
				throw new NotSupportedException("Anonymous pipes can only be in one direction.");
			}
			if (safePipeHandle == null)
			{
				throw new ArgumentNullException("safePipeHandle");
			}
			if (safePipeHandle.IsInvalid)
			{
				throw new ArgumentException("Invalid handle.", "safePipeHandle");
			}
			this.Init(direction, safePipeHandle);
		}

		private void Init(PipeDirection direction, SafePipeHandle safePipeHandle)
		{
			base.ValidateHandleIsPipe(safePipeHandle);
			base.InitializeHandle(safePipeHandle, true, false);
			base.State = PipeState.Connected;
		}

		~AnonymousPipeClientStream()
		{
			this.Dispose(false);
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
	}
}
