using System;
using System.Runtime.InteropServices;

namespace System.IO.Ports
{
	[StructLayout(LayoutKind.Sequential)]
	internal class CommStat
	{
		public uint flags;

		public uint BytesIn;

		public uint BytesOut;
	}
}
