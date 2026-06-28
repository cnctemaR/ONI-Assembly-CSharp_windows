using System;

namespace System.Runtime.InteropServices
{
	[Obsolete]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct PARAMDESC
	{
		public IntPtr lpVarValue;

		public PARAMFLAG wParamFlags;
	}
}
