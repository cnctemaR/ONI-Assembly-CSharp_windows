using System;
using System.Runtime.InteropServices;

namespace System.Data.OleDb
{
	[StructLayout(LayoutKind.Sequential)]
	internal class GdaTime
	{
		public ushort hour;

		public ushort minute;

		public ushort second;

		public long timezone;
	}
}
