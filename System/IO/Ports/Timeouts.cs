using System;
using System.Runtime.InteropServices;

namespace System.IO.Ports
{
	[StructLayout(LayoutKind.Sequential)]
	internal class Timeouts
	{
		public Timeouts(int read_timeout, int write_timeout)
		{
			this.SetValues(read_timeout, write_timeout);
		}

		public void SetValues(int read_timeout, int write_timeout)
		{
			this.ReadIntervalTimeout = uint.MaxValue;
			this.ReadTotalTimeoutMultiplier = uint.MaxValue;
			this.ReadTotalTimeoutConstant = (uint)((read_timeout == -1) ? (-2) : read_timeout);
			this.WriteTotalTimeoutMultiplier = 0U;
			this.WriteTotalTimeoutConstant = (uint)((write_timeout == -1) ? (-1) : write_timeout);
		}

		public uint ReadIntervalTimeout;

		public uint ReadTotalTimeoutMultiplier;

		public uint ReadTotalTimeoutConstant;

		public uint WriteTotalTimeoutMultiplier;

		public uint WriteTotalTimeoutConstant;

		public const uint MaxDWord = 4294967295U;
	}
}
