using System;
using System.Runtime.InteropServices;

namespace System.Data.OleDb
{
	[StructLayout(LayoutKind.Sequential)]
	internal class GdaTimestamp
	{
		public short year;

		public ushort month;

		public ushort day;

		public ushort hour;

		public ushort minute;

		public ushort second;

		public ulong fraction;

		public long timezone;
	}
}
