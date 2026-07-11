using System;
using System.Globalization;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	[global::System.MonoTODO("Anonymous pipes are not working even on win32, due to some access authorization issue")]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class AnonymousPipeClientStream : PipeStream
	{
		private static SafePipeHandle ToSafePipeHandle(string pipeHandleAsString)
		{
			if (pipeHandleAsString == null)
			{
				throw new ArgumentNullException("pipeHandleAsString");
			}
			return new SafePipeHandle(new IntPtr(long.Parse(pipeHandleAsString, NumberFormatInfo.InvariantInfo)), false);
		}

		public AnonymousPipeClientStream(string pipeHandleAsString)
			: this(PipeDirection.In, pipeHandleAsString)
		{
		}

		public AnonymousPipeClientStream(PipeDirection direction, string pipeHandleAsString)
			: this(direction, AnonymousPipeClientStream.ToSafePipeHandle(pipeHandleAsString))
		{
		}

		public AnonymousPipeClientStream(PipeDirection direction, SafePipeHandle safePipeHandle)
			: base(direction, 1024)
		{
			base.InitializeHandle(safePipeHandle, false, false);
			base.IsConnected = true;
		}

		~AnonymousPipeClientStream()
		{
		}

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
	}
}
