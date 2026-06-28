using System;
using System.Runtime.InteropServices;

namespace System.Data.OleDb
{
	[StructLayout(LayoutKind.Sequential)]
	internal class GdaDate
	{
		public short year;

		public ushort month;

		public ushort day;
	}
}
