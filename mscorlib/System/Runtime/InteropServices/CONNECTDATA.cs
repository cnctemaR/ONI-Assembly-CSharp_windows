using System;

namespace System.Runtime.InteropServices
{
	[Obsolete]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct CONNECTDATA
	{
		[MarshalAs(UnmanagedType.Interface)]
		public object pUnk;

		public int dwCookie;
	}
}
