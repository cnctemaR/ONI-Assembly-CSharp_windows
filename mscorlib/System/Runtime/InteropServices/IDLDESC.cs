using System;

namespace System.Runtime.InteropServices
{
	[Obsolete]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct IDLDESC
	{
		public int dwReserved;

		public IDLFLAG wIDLFlags;
	}
}
