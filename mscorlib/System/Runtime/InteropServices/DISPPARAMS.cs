using System;

namespace System.Runtime.InteropServices
{
	[Obsolete]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPPARAMS
	{
		public IntPtr rgvarg;

		public IntPtr rgdispidNamedArgs;

		public int cArgs;

		public int cNamedArgs;
	}
}
