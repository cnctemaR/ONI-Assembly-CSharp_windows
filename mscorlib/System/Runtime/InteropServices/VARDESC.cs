using System;

namespace System.Runtime.InteropServices
{
	[Obsolete]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct VARDESC
	{
		public int memid;

		public string lpstrSchema;

		public ELEMDESC elemdescVar;

		public short wVarFlags;

		public VarEnum varkind;

		[ComVisible(false)]
		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct DESCUNION
		{
			[FieldOffset(0)]
			public IntPtr lpvarValue;

			[FieldOffset(0)]
			public int oInst;
		}
	}
}
