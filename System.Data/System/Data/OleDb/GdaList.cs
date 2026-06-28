using System;
using System.Runtime.InteropServices;

namespace System.Data.OleDb
{
	[StructLayout(LayoutKind.Sequential)]
	internal class GdaList
	{
		public IntPtr data;

		public IntPtr next;

		public IntPtr prev;
	}
}
