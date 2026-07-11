using System;

namespace System.IO.Ports
{
	internal struct CommStat
	{
		public uint flags;

		public uint BytesIn;

		public uint BytesOut;
	}
}
